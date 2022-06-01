using SrtsWeb;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Orders;
using SrtsWeb.Views.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

namespace SrtsWebClinic.Orders
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabMail")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    public partial class ManageOrders : PageBase, ICheckInDispenseView, IOrderProblemView, ISiteMapResolver
    {
        private CheckInDispensePresenter _presenterCIAndD;
        private ManagerOrdersGridsPresenter _presenterProblem;

        public ManageOrders()
        {
            _presenterCIAndD = new CheckInDispensePresenter(this);
            _presenterProblem = new ManagerOrdersGridsPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "ManageOrders_Page_Load", mySession.MyUserID))
#endif
            {
                if (IsPostBack)
                {
                    hiddenDownload.Src = "";
                    var ea = Request.Params.Get("__EVENTARGUMENT");
                    if (!String.IsNullOrEmpty(ea) && ea.ToLower().Equals("jquery_pb"))
                    {
                        ProcessDtoForOrderManagement();
                        Session.Add("IsDtoRedirect", true);
                        Response.Redirect("~/WebForms/SrtsOrderManagement/OrderManagement.aspx", false);
                    }
                }
                else
                {
                    try
                    {
                        Master.CurrentModuleTitle = string.Empty;

                        this.hfSiteCode.Value = mySession.MySite.SiteCode;

                        // DO NOT CHANGE! Critical functions in order.js strip values from this string
                        litContentTop_Title_Right.Text = string.Format("{0} - {1}", mySession.MySite.SiteName, mySession.MyClinicCode);
                        // DO NOT CHANGE! Critical functions in order.js strip values from this string

                        BuildPageTitle();

                        if (User.IsInRole("clinicclerk"))
                        {
                            this.Controls.SetControlStateBtn(false);
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                        ex.TraceErrorException();
                        Response.Redirect(FormsAuthentication.DefaultUrl, false);
                    }
                    _presenterCIAndD.InitView();
                }
            }
        }
        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = "Manage Orders - Clinic";
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Manage Orders - Clinic");
                CurrentModule_Sub(string.Empty);
            }
        }
        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Orders/ManageOrders.aspx", "Manage Clinic Orders");
            child.ParentNode = parent;
            return child;
        }

        #region Shared Methods

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "ManageOrders_btnSubmit_Click", mySession.MyUserID))
#endif
                {
                    List<string> lstOrdersIG = new List<string>();
                    List<string> lstOrdersNIG = new List<string>();

                    if (!hfOrdersInGrid.Value.IsNullOrEmpty()) lstOrdersIG = hfOrdersInGrid.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (!hfOrdersNotInGrid.Value.IsNullOrEmpty()) lstOrdersNIG = hfOrdersNotInGrid.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    int pageModule = -1;
                    switch (hfPageModule.Value.ToLower())
                    {
                        case "pending":
                        case "checkin":
                            pageModule = 0;
                            break;

                        case "dispense":
                            pageModule = 1;
                            break;

                        case "problem":
                            pageModule = 2;
                            break;

                        case "overdue":
                            pageModule = 3;
                            break;

                        default:
                            pageModule = -1;
                            break;
                    }
                    var printLabels = false;
                    if ((pageModule != -1 && pageModule != 2 && pageModule != 3) && (lstOrdersIG.Count > 0 || lstOrdersNIG.Count > 0))
                    {
                        printLabels = _presenterCIAndD.UpdateOrders(lstOrdersIG, lstOrdersNIG, pageModule, SrtsWeb.Reports.ReportCommon.GetTableTemplateForLabels());
                    }
                    if (printLabels) { hiddenDownload.Src = "~/WebForms/PrintForms/LabelRptViewer.aspx"; }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/Default.aspx");
        }

        private void ShowMessage()
        {
            CustomValidator cv = new CustomValidator();
            cv.IsValid = false;
            cv.ErrorMessage = Message;
            this.Page.Validators.Add(cv);
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static String UpdateOrderStatuses(String OrdersInGrid, String OrdersNotInGrid, String CurrentModule, String SelectedLabel)
        {
            try
            {
                var mySession = HttpContext.Current.Session["SRTSSession"] as SRTSSession;

#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "ManageOrders_UpdateOrderStatuses_WebMethod", mySession.MyUserID))
#endif
                {
                    List<string> lstOrdersIG = new List<string>();
                    List<string> lstOrdersNIG = new List<string>();

                    if (!OrdersInGrid.IsNullOrEmpty()) lstOrdersIG = OrdersInGrid.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (!OrdersNotInGrid.IsNullOrEmpty()) lstOrdersNIG = OrdersNotInGrid.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    int pageModule = -1;
                    switch (CurrentModule.ToLower())
                    {
                        case "pending":
                        case "checkin":
                            pageModule = 0;
                            break;

                        case "dispense":
                            pageModule = 1;
                            break;

                        case "problem":
                            pageModule = 2;
                            break;

                        case "overdue":
                            pageModule = 3;
                            break;

                        default:
                            pageModule = -1;
                            break;
                    }
                    var printLabels = false;

                    var view = new ManageOrders();
                    view.mySession = mySession;
                    view.SelectedLabel = SelectedLabel;
                    var p = new CheckInDispensePresenter(view);
                    if ((pageModule != -1 && pageModule != 2 && pageModule != 3) && (lstOrdersIG.Count > 0 || lstOrdersNIG.Count > 0))
                    {
                        printLabels = p.UpdateOrders(lstOrdersIG, lstOrdersNIG, pageModule, SrtsWeb.Reports.ReportCommon.GetTableTemplateForLabels());
                    }
                    return printLabels ? "~/WebForms/PrintForms/LabelRptViewer.aspx" : "";
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); return ""; }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<object> GetClinicOrderData(string pageMode, string siteCode)
        {
            try
            {
                var ss = HttpContext.Current.Session["SRTSSession"] as SRTSSession;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "ManageOrders_GetClinicOrderData", ss.MyUserID))
#endif
                {
                    List<object> gridDataList = new List<object>();

                    switch (pageMode)
                    {
                        case "pending":
                            var moel = ManagerOrdersGridsPresenter.GetPendingOrders(siteCode, HttpContext.Current.User.Identity.Name);
                            foreach (var p in moel)
                            {
                                gridDataList.AddRange(new object[] {
                                    new {
                                        OrderNumber = p.OrderNumber,
                                        LastName = p.LastName,
                                        FirstName = p.FirstName,
                                        MiddleName = p.MiddleName,
                                        IndividualId = p.IndividualId,
                                        FrameCode = p.FrameCode,
                                        LensType = p.LensType,
                                        ShipToPatient = p.OrderDisbursement,
                                        LabSiteCode = p.LabSiteCode,
                                        DateOrderCreated = p.DateOrderCreated.ToString(),
                                        OrderStatusDescription = p.StatusComment
                                    }
                                });
                            }

                            break;

                        case "checkin":

                            #region checkin

                            var moec = CheckInDispensePresenter.GetCheckInOrDispenseOrders(true, siteCode, HttpContext.Current.User.Identity.Name);
                            foreach (var ci in moec)
                            {
                                gridDataList.AddRange(new object[] {
                                new {
                                    OrderNumber = ci.OrderNumber,
                                    LastName = ci.LastName,
                                    FirstName = ci.FirstName,
                                    MiddleName = ci.MiddleName,
                                    IndividualId = ci.IndividualId,
                                    FrameCode = ci.FrameCode,
                                    LensType = ci.LensType,
                                    ShipToPatient = ci.OrderDisbursement,
                                    LabSiteCode = ci.LabSiteCode,
                                    DateOrderCreated = ci.DateOrderCreated.ToString(),
                                    }
                            });
                            }

                            break;

                        #endregion checkin

                        case "dispense":

                            #region dispense

                            var moed = CheckInDispensePresenter.GetCheckInOrDispenseOrders(false, siteCode, HttpContext.Current.User.Identity.Name);
                            foreach (var ci in moed)
                            {
                                gridDataList.AddRange(new object[] {
                                new {
                                    OrderNumber = ci.OrderNumber,
                                    LastName = ci.LastName,
                                    FirstName = ci.FirstName,
                                    MiddleName = ci.MiddleName,
                                    IndividualId = ci.IndividualId,
                                    FrameCode = ci.FrameCode,
                                    LensType = ci.LensType,
                                    LabSiteCode = ci.LabSiteCode,
                                    DateOrderCreated = ci.DateOrderCreated.ToString()
                                    }
                            });
                            }

                            break;

                        #endregion dispense

                        case "problem":

                            #region problem

                            var moep = ManagerOrdersGridsPresenter.GetProblemOrders(siteCode, HttpContext.Current.User.Identity.Name);
                            foreach (var ci in moep)
                            {
                                gridDataList.AddRange(new object[] {
                                new {
                                    OrderNumber = ci.OrderNumber,
                                    LastName = ci.LastName,
                                    FirstName = ci.FirstName,
                                    MiddleName = ci.MiddleName,
                                    IndividualId = ci.IndividualId,
                                    FrameCode = ci.FrameCode,
                                    LensType = ci.LensType,
                                    ShipToPatient = ci.OrderDisbursement,
                                    LabSiteCode = ci.LabSiteCode,
                                    DateOrderCreated = ci.DateOrderCreated.ToString(),
                                    OrderStatusDescription = ci.OrderStatusDescription
                                    }
                            });
                            }

                            break;

                        #endregion problem

                        case "overdue":

                            #region overdue

                            var moeo = ManagerOrdersGridsPresenter.GetOverdueOrders(siteCode, HttpContext.Current.User.Identity.Name);
                            foreach (var ci in moeo)
                            {
                                gridDataList.AddRange(new object[] {
                                new {
                                    OrderNumber = ci.OrderNumber,
                                    LastName = ci.LastName,
                                    FirstName = ci.FirstName,
                                    MiddleName = ci.MiddleName,
                                    IndividualId = ci.IndividualId,
                                    FrameCode = ci.FrameCode,
                                    LensType = ci.LensType,
                                    LabSiteCode = ci.LabSiteCode,
                                    DateOrderReceived = ci.DateReceivedByLab.ToString(),
                                    DaysPastDue = ci.DaysPastDue,
                                    OrderStatusDescription = ci.StatusComment
                                    }
                            });
                            }

                            break;

                            #endregion overdue
                    }

                    return gridDataList;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); return new List<object>(); }
        }

        #endregion Shared Methods

        #region CheckIn

        #region ICheckInDispenseView Members

        public string ReportName
        {
            get { return ddlLabelFormat.SelectedValue; }
            set { ddlLabelFormat.SelectedValue = value; }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public List<ManageOrderEntity> CheckInData
        {
            get { return (List<ManageOrderEntity>)Session["CheckInData"]; }
            set { Session.Add("CheckInData", value); }
        }

        private XmlDocument _XmlDoc;

        public XmlDocument XmlDoc
        {
            get { return _XmlDoc; }
            set { _XmlDoc = value; }
        }

        public String SelectedLabel
        {
            get { return ViewState["SelectedLabel"].IsNull() ? this.ddlLabelFormat.SelectedValue : ViewState["SelectedLabel"].ToString(); }
            set { ViewState["SelectedLabel"] = value; }
        }

        #endregion ICheckInDispenseView Members

        #endregion CheckIn

        #region Dispense

        #region ICheckInDispenseView Members

        public List<ManageOrderEntity> DispenseData
        {
            get { return (List<ManageOrderEntity>)ViewState["DispenseState"]; }
            set { ViewState.Add("DispenseState", value); }
        }

        #endregion ICheckInDispenseView Members

        #endregion Dispense

        #region Problem

        #region IOrderProblemView Members

        public List<ManageOrderEntity> ProblemData
        {
            get { return (List<ManageOrderEntity>)ViewState["ProblemData"]; }
            set { ViewState.Add("ProblemData", value); }
        }

        public bool IsActive
        {
            get;
            set;
        }

        public string LabCode
        {
            get;
            set;
        }

        public string ClinicCode
        {
            get;
            set;
        }

        #endregion IOrderProblemView Members

        #endregion Problem

        #region Overdue

        #region IOrderProblemView Members

        public List<ManageOrderEntity> OverdueData
        {
            get { return (List<ManageOrderEntity>)ViewState["OverdueData"]; }
            set { ViewState.Add("OverdueData", value); }
        }

        #endregion IOrderProblemView Members

        #endregion Overdue

        private void ProcessDtoForOrderManagement()
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "ManageOrders_ProcessDtoForOrderManagement", mySession.MyUserID))
#endif
            {
                var st = StatusType.VIEW;
                var o = MasterService.DoQuickSearchOrder(this.SelectedOrderNumber, this.mySession.ModifiedBy, out st);
                var dto = MasterService.DoQuickSearchPatient(o.IndividualID_Patient, this.mySession.ModifiedBy);
                dto.OrderNumber = o.OrderNumber;
                Session.Add("PatientOrderDTO", dto);
            }
        }

        public String SelectedOrderNumber { get { return this.hfOrderNumber.Value.ToString(); } }
    }
}