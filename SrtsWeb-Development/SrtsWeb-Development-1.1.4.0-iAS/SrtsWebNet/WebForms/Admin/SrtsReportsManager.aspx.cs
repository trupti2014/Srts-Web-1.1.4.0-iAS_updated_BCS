using SrtsWeb.Base;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.WebControls;
using SrtsWeb.WebForms.Admin;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using SrtsWeb.BusinessLayer.Concrete;
using System.Configuration;
using System.Net;
using System.Xml;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Drawing;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabMail")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtDataMgmt")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class SrtsReportsManager : PageBase, ISiteMapResolver, IReportsManagerView
    {
        private ReportsManagerPresenter _presenterReportManager;
        private AddressEntity edAddress = new AddressEntity();
        private AddressEntity VerifiedAddress = new AddressEntity();
        const string colorAlternate = "#e6f2ff";

        public SrtsReportsManager()
        {
            _presenterReportManager = new ReportsManagerPresenter(this);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }
            if (!IsPostBack)
            {
                if (mySession == null)
                {
                    mySession = new SRTSSession();
                    mySession.AddOrEdit = string.Empty;
                    mySession.Patient = new PatientEntity();
                    mySession.ReturnURL = string.Empty;
                    mySession.SelectedExam = new ExamEntity();
                    mySession.SelectedOrder = new OrderEntity();
                    mySession.SelectedPatientID = 0;
                    mySession.TempID = 0;
                    mySession.tempOrderID = string.Empty;

                    this.mySession.Patient = new PatientEntity();
                    this.mySession.Patient.Individual = new IndividualEntity();
                }
                try
                {
                    BuildPageTitle();
                }
                catch (NullReferenceException)
                {
                    Response.Redirect(FormsAuthentication.DefaultUrl);
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                int id;
                string strId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(strId) && Int32.TryParse(strId, out id))
                {
                    GetSelectedPageOption(strId);
                }
                else
                {
                    ApplicationException ex = new ApplicationException();
                    ex.LogException("Error in SrtsReportManager...Invalid query string detected: " + strId);
                    Response.Redirect(FormsAuthentication.DefaultUrl);
                }
            }

            //if (OrderAddressesList != null)
            //{
                BindGridviewOrderAddresses();
            //}
            //else
            //{
            //    DisplayEmptyGrid();
            //}
        }

        protected void GetSelectedPageOption(string path)
        {
            //if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            //{
            //    path = (Request.QueryString["id"]);
            //}
            //else if (Request.PathInfo.Length != 0)
            //{
            //    path = Request.PathInfo.Substring(1);
            //}
            if (string.IsNullOrEmpty(path))
            {
                if (Request.PathInfo.Length != 0)
                {
                    path = Request.PathInfo.Substring(1);
                }
                else
                {
                    path = "1";
                }
            }
            switch (path)
            {
                case "2":
                    divPrintLabels.Visible = true;
                    divPrintReports.Visible = false;
                    Master.CurrentModuleTitle = "Manage Labels - Print Shipping Labels";
                    Master.uplCurrentModuleTitle.Update();
                    if (PrintOrderNumbers.Count > 0)
                    {
                        BindGridviewOrderAddresses();
                    }
                    break;
                case "1":
                    divPrintLabels.Visible = false;
                    divPrintReports.Visible = true;
                    Master.CurrentModuleTitle = "Manage Reports - View/Print Reports";
                    Master.uplCurrentModuleTitle.Update();
                    BuildReportsList();
                    ddlReportSelection.Focus();
                    break;
                default:
                    divPrintLabels.Visible = false;
                    divPrintReports.Visible = true;
                    BuildReportsList();
                    ddlReportSelection.Focus();
                    break;
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Admin/SrtsReportManager.aspx", "Manage Reports");
            child.ParentNode = parent;
            return child;
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = "Manage Reports";
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Manage Reports");
                CurrentModule_Sub(string.Empty);
            }
        }

        private void BuildReportsList()
        {
            if (Roles.IsUserInRole("MgmtEnteprise") ||
                Roles.IsUserInRole("MgmtAdmin") ||
                 Roles.IsUserInRole("LabAdmin") ||
                 Roles.IsUserInRole("LabTech") ||
                 Roles.IsUserInRole("LabClerk") ||
                 Roles.IsUserInRole("LabMail")

                )
            {
                ListItem lstReports001 = new ListItem();
                lstReports001.Text = "Blank 771 Form";

                lstReports001.Value = "/Blank_DD771";
                if (!IsInList(lstReports001.Text)) { ddlReportSelection.Items.Add(lstReports001); }

                ListItem lstReports016 = new ListItem();
                lstReports016.Text = "Reprint Lab Routing Forms";
                lstReports016.Value = "rLrf";
                if (!IsInList(lstReports016.Text)) { ddlReportSelection.Items.Add(lstReports016); }

                ListItem lstReports018 = new ListItem();
                lstReports018.Text = "Reprint Shipping Labels";
                lstReports018.Value = "rLabel";
                if (!IsInList(lstReports018.Text)) { ddlReportSelection.Items.Add(lstReports018); }

                ListItem lstReports002 = new ListItem();
                lstReports002.Text = "Report 54";
                lstReports002.Value = "/Lab Reports/Rpt54";

                if (!IsInList(lstReports002.Text)) { ddlReportSelection.Items.Add(lstReports002); }

                ListItem lstReports003 = new ListItem();
                lstReports003.Text = "Patient 771";

                //lstReports003.Value = "/DDForm771a";
                lstReports003.Value = "/LabRoutingForm";
       

                if (!IsInList(lstReports003.Text)) { ddlReportSelection.Items.Add(lstReports003); }
            }

            if (Roles.IsUserInRole("MgmtEnterprise") ||
                Roles.IsUserInRole("MgmtAdmin") ||
                Roles.IsUserInRole("ClinicAdmin") ||
                Roles.IsUserInRole("ClinicTech") ||
                Roles.IsUserInRole("ClinicProvider") ||
                Roles.IsUserInRole("ClinicClerk")
                )
            {
                ListItem lstReports004 = new ListItem();
                lstReports004.Text = "Blank 771 Form";
                lstReports004.Value = "/Blank_DD771";
                if (!IsInList(lstReports004.Text)) { ddlReportSelection.Items.Add(lstReports004); }

                ListItem lstReports017 = new ListItem();
                lstReports017.Text = "Reprint Shipping Labels";
                lstReports017.Value = "rLabel";
                if (!IsInList(lstReports017.Text)) { ddlReportSelection.Items.Add(lstReports017); }

                ListItem lstReports005 = new ListItem();
                lstReports005.Text = "Clinic Dispense Report";
                lstReports005.Value = "/Clinic Reports/ClinicDispenseRpt";
                if (!IsInList(lstReports005.Text)) { ddlReportSelection.Items.Add(lstReports005); }

                if (!User.IsInRole("clinicclerk"))
                {
                    ListItem lstReports006 = new ListItem();
                    lstReports006.Text = "Clinic Orders Report";
                    lstReports006.Value = "/Clinic Reports/ClinicOrdersRpt";
                    if (!IsInList(lstReports006.Text)) { ddlReportSelection.Items.Add(lstReports006); }
                }

                ListItem lstReports007 = new ListItem();
                lstReports007.Text = "Clinic Overdue Orders Report";
                lstReports007.Value = "/Clinic Reports/ClinicOverdueOrders";
                if (!IsInList(lstReports007.Text)) { ddlReportSelection.Items.Add(lstReports007); }

                ListItem lstReports008 = new ListItem();
                lstReports008.Text = "Clinic Production Report";
                lstReports008.Value = "/Clinic Reports/ClinicProductionRpt";
                if (!IsInList(lstReports008.Text)) { ddlReportSelection.Items.Add(lstReports008); }

                ListItem lstReports009 = new ListItem();
                lstReports009.Text = "Clinic Summary Report";
                lstReports009.Value = "/Clinic Reports/ClinicSummaryRpt";
                if (!IsInList(lstReports009.Text)) { ddlReportSelection.Items.Add(lstReports009); }

                ListItem lstReports010 = new ListItem();
                lstReports010.Text = "Order Detail Report";
                lstReports010.Value = "/Clinic Reports/OrderDetailReport";
                if (!IsInList(lstReports010.Text)) { ddlReportSelection.Items.Add(lstReports010); }

                ListItem lstReports011 = new ListItem();
                lstReports011.Text = "Turn Around Time Report";
                lstReports011.Value = "/Clinic Reports/TurnAroundTimeRpt";
                if (!IsInList(lstReports011.Text)) { ddlReportSelection.Items.Add(lstReports011); }

                //ListItem lstReports016 = new ListItem();
                //lstReports016.Text = "Clinic Group Report";
                //lstReports016.Value = "/Clinic Reports/ClinicGroupRpt";
                //if (!IsInList(lstReports016.Text)) { ddlReportSelection.Items.Add(lstReports016); }

                if (Roles.IsUserInRole("MgmtEnterprise") ||
                    Roles.IsUserInRole("MgmtAdmin")
                    )
                {
                    ListItem lstReports012 = new ListItem();
                    lstReports012.Text = "Wounded Warrior Report";
                    lstReports012.Value = "/WWReport";
                    if (!IsInList(lstReports012.Text)) { ddlReportSelection.Items.Add(lstReports012); }

                    ListItem lstReports013 = new ListItem();
                    lstReports013.Text = "Account Information";
                    lstReports013.Value = "/Administration Rpts/AccountInfo";
                    if (!IsInList(lstReports013.Text)) { ddlReportSelection.Items.Add(lstReports013); }

                    ListItem lstReports014 = new ListItem();
                    lstReports014.Text = "Theater Location Codes";
                    lstReports014.Value = "/Administration Rpts/TheaterLocationCodes";
                    if (!IsInList(lstReports014.Text)) { ddlReportSelection.Items.Add(lstReports014); }

                    ListItem lstReports015 = new ListItem();
                    lstReports015.Text = "GEyes Order Counts";
                    lstReports015.Value = "/Administration Rpts/GEyesCount";
                    if (!IsInList(lstReports015.Text)) { ddlReportSelection.Items.Add(lstReports015); }
                }
            }
        }

        protected bool IsInList(string item)
        {
            bool result;
            if (ddlReportSelection.Items.FindByText(item) != null)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        protected void ddlReportSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlReportSelection.SelectedIndex > 0)
            {
                if (ddlReportSelection.SelectedValue.ToLower().In(new[] { "rlrf", "rlabel" }))
                {
                    var p = new ReportsManagerPresenter(this);

                    // Show sub ddl to to allow the user to choose which 'batch' of items to generate a PDF for
                    if (ddlReportSelection.SelectedValue.ToLower().Equals("rlrf"))
                        p.GetOrderCountItems("2", mySession.MySite.SiteCode);
                    else
                    {
                        if (mySession.MySite.SiteType.ToLower().Equals("lab"))
                            p.GetOrderCountItems("7,19", mySession.MySite.SiteCode); // 7, 19
                        else if (mySession.MySite.SiteType.ToLower().Equals("clinic"))
                            p.GetOrderCountItems("11,16", mySession.MySite.SiteCode); // 8, 16
                    }
                    this.divReprint.Visible = true;
                }
                else
                {
                    this.divReprint.Visible = false;
                    string selReportId = ddlReportSelection.SelectedValue.ToString();
                    string selReportTitle = ddlReportSelection.SelectedItem.ToString();
                    try
                    {
                        string reportURL = string.Format("~/WebForms/Reports/rptViewerTemplate.aspx?id={0}&title={1}&isreprint=false", selReportId, selReportTitle);
                        if (!CustomRedirect.SanitizeRedirect(reportURL, false))
                        {
                            ShowErrorDialog("There was a problem printing this report.  Please report this error.");
                        };
                        //{
                        //    Response.Redirect(reportURL, false);
                        //}
                        //else
                        //{
                        //SystemException ex = new SystemException();
                        //ex.LogException("Error in Getting Selected Report in Report Manager...There was a problem with the url redirect.");
                        //FormsAuthentication.RedirectToLoginPage();
                        //}
                    }
                    catch (SystemException ex)
                    {
                        ex.LogException("Error in Getting Selected Report in Report Manager...");
                    }
                }
            }
        }
        
        protected void ddlReprint_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected key and then get the order numbers for the selected batch
            var p = new ReportsManagerPresenter(this);
            switch (this.ddlReportSelection.SelectedValue)
            {
                case "rLrf":
                    p.GetReprint771(this.mySession.MySite.SiteCode, this.SelectedReprintItem.BatchDate);
                    this.divLblFormat.Visible = false;
                    if (String.IsNullOrEmpty(this.ReprintOrderNumbers)) return;
                    break;

                case "rLabel":
                    if (this.mySession.MySite.SiteType.ToLower().Equals("lab"))
                    {
                        p.GetReprintLabels("7,19", this.mySession.MySite.SiteCode, this.SelectedReprintItem.BatchDate);
                        Session["StatusId"] = "7,19";
                    }
                    else if (this.mySession.MySite.SiteType.ToLower().Equals("clinic"))
                    {
                        p.GetReprintLabels("11,16", this.mySession.MySite.SiteCode, this.SelectedReprintItem.BatchDate);
                        Session["StatusId"] = "11,16";
                    }
                    Session["BatchDate"] = this.SelectedReprintItem.BatchDate;
                    if (this.ReprintLabels.IsNullOrEmpty()) return;
                    this.divLblFormat.Visible = true;
                    return;
            }

            try
            {
                if (ddlReportSelection.SelectedValue.ToString().Equals("rLabel")) return;  // Label report id is set in the format selection ddl

                var selReportId = "/LabRoutingForm";
                var selReportTitle = ddlReportSelection.SelectedItem.ToString();

                string reportURL = (string.Format("~/WebForms/Reports/rptViewerTemplate.aspx?id={0}&title={1}&isreprint=false", selReportId, selReportTitle));

                if (!CustomRedirect.SanitizeRedirect(reportURL, false))
                {
                    ShowErrorDialog("There was a problem printing this report.  Please report this error.");
                };
                
                
                //if (!Helpers.isAbsolute(reportURL))
                //    {
                //        Response.Redirect(reportURL, false);
                //    }
                //    else
                //    {
                //        SystemException ex = new SystemException();
                //        ex.LogException("Error in Getting Selected Report in Report Manager...There was a problem with the url redirect.");
                //        if (!Helpers.isAbsolute("~/WebForms/Default.aspx"))
                //        {
                //            FormsAuthentication.RedirectToLoginPage();
                //        }
                //    }
            }
            catch (SystemException ex)
            {
                ex.LogException("Error in Getting Selected Report in Report Manager...");
            }
        }

        protected void ddlLabelFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Get label format type and determine what the report id is. Avery5160, SingleLabel
                var selReportId = @"/Labels/" + this.ddlLabelFormat.SelectedValue;  //"/SOME_LABEL_NAME_BASED_ON_FORMAT";
                var selReportTitle = ddlReportSelection.SelectedItem.ToString();

                string reportURL = (string.Format("~/WebForms/Reports/rptViewerTemplate.aspx?id={0}&title={1}&isreprint=false", selReportId, selReportTitle));
                if (!CustomRedirect.SanitizeRedirect(reportURL, false))
                {
                    ShowErrorDialog("There was a problem printing this report.  Please report this error.");
                };

                //CustomRedirect.SanitizeRedirect(reportURL, false);    
                //if (!Helpers.isAbsolute(reportURL))
                //    {
                //        Response.Redirect(reportURL, false);
                //    }
                //    else
                //    {
                //        SystemException ex = new SystemException();
                //        ex.LogException("Error in Getting Selected Report in Report Manager...There was a problem with the url redirect.");
                //        if (!Helpers.isAbsolute("~/WebForms/Default.aspx"))
                //        {
                //            FormsAuthentication.RedirectToLoginPage();
                //        }
                //    }
                //Response.Redirect(string.Format("~/WebForms/Reports/rptViewerTemplate.aspx?id={0}&title={1}&isreprint=false", selReportId, selReportTitle), false);           
            }
            catch (SystemException ex)
            {
                ex.LogException("Error in Getting Selected Report in Report Manager...");
            }
        }


        public static List<ManageOrderEntity> GetCheckInOrDispenseOrders(bool IsCheckIn, string siteCode, string myUserId)
        {
            var _repository = new OrderStateRepository.OrdersOfStatusRepository();
            var moe = new List<ManageOrderEntity>();

            if (IsCheckIn)
                moe = _repository.GetOrdersForCheckInToClinicByClinicCode(siteCode, myUserId);
            else
                moe = _repository.GetOrdersToDispenseByClinicCode(siteCode, myUserId);

            moe.ForEach(x => x.IsSelected = false);
            moe = moe.OrderByDescending(x => x.OrderNumber).ToList();

            return moe;
        }

        public List<ReprintEntity> ReprintItems
        {
            get
            {
                return ViewState["ReprintItems"] as List<ReprintEntity>;
            }
            set
            {
                ViewState["ReprintItems"] = value;
                this.ddlReprint.DataSource = value;
                this.ddlReprint.DataTextField = "CombinedKey";
                this.ddlReprint.DataValueField = "CombinedKey";
                this.ddlReprint.DataBind();
                this.ddlReprint.Items.Insert(0, new ListItem("-Select-", "X"));

                this.ddlHistoryItems.DataSource = value;
                this.ddlHistoryItems.DataTextField = "CombinedKey";
                this.ddlHistoryItems.DataValueField = "CombinedKey";
                this.ddlHistoryItems.DataBind();
                this.ddlHistoryItems.Items.Insert(0, new ListItem("-Select-", "X"));
            }
        }
        public String ReprintOrderNumbers
        {
            get
            {
                return Session["OrderNbrs"].ToString();
            }
            set
            {
                Session["OrderNbrs"] = value;
            }
        }
        public ReprintEntity SelectedReprintItem
        {
            get
            {
                return this.ReprintItems.FirstOrDefault(x => x.CombinedKey == this.ddlReprint.SelectedValue);
            }
            set
            {
                this.ddlReprint.SelectedValue = value.CombinedKey;
            }
        }
        public List<ReprintReturnEntity> ReprintLabels
        {
            get
            {
                return ViewState["ReprintLabels"] as List<ReprintReturnEntity>;
            }
            set
            {
                ViewState["ReprintLabels"] = value;
            }
        }

        public List<ReprintOnDemandReturnEntity> ReprintOnDemandLabels
        {
            get
            {
                return ViewState["ReprintOnDemandLabels"] as List<ReprintOnDemandReturnEntity>;
            }
            set
            {
                ViewState["ReprintOnDemandLabels"] = value;
            }
        }


       protected void BindGridviewOrderAddresses()
        {
            if (OrderAddressesList != null)
            {
                if (OrderAddressesList.Count > 0)
                {
                    // check if the use mailing address option should be checked
                    foreach (GridViewRow row in gvOrderAddresses.Rows)
                    {
                        CheckBox chkUseMailingAddress = (CheckBox)row.FindControl("chkUseMailingAddress");
                        Label OrderNumber = (Label)row.FindControl("OrderNumber");
                        if (chkUseMailingAddress.Checked == true)
                        {
                            foreach (OrderLabelAddresses oa in OrderAddressesList)
                            {
                                if (oa.OrderNumber == OrderNumber.Text)
                                {
                                    oa.UseMailingAddress = true;
                                }
                            }
                        }
                    }
                    gvOrderAddresses.DataSource = OrderAddressesList;
                    gvOrderAddresses.DataBind();
                }
                else
                {
                    DisplayEmptyGrid();
                }
            }
            else
            {

                if (Session["OrderAddresses"] as List<OrderLabelAddresses> != null)
                {
                    List<OrderLabelAddresses> addresses = new List<OrderLabelAddresses>();
                    addresses = Session["OrderAddresses"] as List<OrderLabelAddresses>;
                    if (!addresses.IsNull() && addresses.Count > 0)
                    {
                        OrderAddressesList = addresses;
                        // check if the use mailing address option should be checked
                        foreach (GridViewRow row in gvOrderAddresses.Rows)
                        {
                            CheckBox chkUseMailingAddress = (CheckBox)row.FindControl("chkUseMailingAddress");
                            Label OrderNumber = (Label)row.FindControl("OrderNumber");
                            if (chkUseMailingAddress.Checked == true)
                            {
                                foreach (OrderLabelAddresses oa in addresses)
                                {
                                    if (oa.OrderNumber == OrderNumber.Text)
                                    {
                                        oa.UseMailingAddress = true;
                                    }
                                }
                            }
                        }
                        gvOrderAddresses.DataSource = addresses;
                        gvOrderAddresses.DataBind();
                        Session["OrderAddresses"] = "";
                    }
                }
                else
                {
                    DisplayEmptyGrid();
                }
            }
        }
        protected void DisplayEmptyGrid()
        {
            ViewState["IsReprint"] = "false";
            OrderLabelAddresses ola = new OrderLabelAddresses();
            List<OrderLabelAddresses> olal = new List<OrderLabelAddresses>();
            olal.Add(ola);
            gvOrderAddresses.DataSource = olal;
            gvOrderAddresses.DataBind();
            gvOrderAddresses.Rows[0].Visible = false;

            gvOrderAddresses.Columns[2].HeaderText = "Patient Order Address"; // reset patient address column header
            gvOrderAddresses.Columns[3].Visible = true; // show order address column
            gvOrderAddresses.Columns[4].Visible = true; // show actions column

        }


        protected void DisplayEmptyHistoryGrid()
        {
            ViewState["IsReprint"] = "true";
            OrderLabelAddresses ola = new OrderLabelAddresses();
            List<OrderLabelAddresses> olal = new List<OrderLabelAddresses>();
            olal.Add(ola);
            gvOrderAddresses.DataSource = olal;
            gvOrderAddresses.DataBind();
            gvOrderAddresses.Rows[0].Visible = false;


            gvOrderAddresses.Columns[2].HeaderText = "Address"; // change patient address column header
            gvOrderAddresses.Columns[3].Visible = false; // hide order address column
            gvOrderAddresses.Columns[4].Visible = false; // hide actions column
        }

        protected void clearOrderAddresses_Click(object sender, EventArgs e)
        {
            if (OrderAddressesList != null && OrderAddressesList.Count > 0)
            {
                string isReprint = ViewState["IsReprint"] as String;
                if (!string.IsNullOrEmpty(isReprint) && isReprint == "true")
                {
                    ViewState["IsReprint"] = "false";
                    ddlHistoryItems.SelectedIndex = 0;
                    divHistoryItems.Visible = false;

                    upAddresses.Update();
                    uplOrderAddresses.Update();
                }
                else
                {
                    PrintOrderNumbers.Clear();
                }
                OrderAddressesList.Clear();
                BindGridviewOrderAddresses();
                DisplayEmptyGrid();
                divHistoryItems.Visible = false;
                upAddresses.Update();
                uplOrderAddresses.Update();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "IsClearValid", "PerformFunction('IsFunctionValid','clear', 'false');", true);
            }
        }

        protected void grdAddressesUpdate_Click(object sender, EventArgs e)
        {
            ImageButton button = sender as ImageButton;
            string command = button.CommandName.ToString();
            string comArgument = button.CommandArgument.ToString();
            string[] arguments = comArgument.Split(',');
            GridViewRow gvr = (GridViewRow)button.NamingContainer;
            int RowIndex = gvr.RowIndex;
            int PatientId = Convert.ToInt32(arguments[1]);
            string OrderNumber = arguments[0];
            mySession.SelectedPatientID = PatientId;
            mySession.SelectedOrder.OrderNumber = OrderNumber;
            this.PatientId = PatientId;
            switch (command)
            {
                case "Delete":
                    // remove order number if it exists
                    OrderAddressesList = OrderAddressesList.Where(x => x.OrderNumber != OrderNumber).ToList();
                    if (OrderAddressesList != null && OrderAddressesList.Count == 0)
                    {
                        //if (ReprintItems != null) { ddlHistoryItems.SelectedIndex = 0; }
                        //ViewState["IsReprint"] = "false";
                        DisplayEmptyGrid();
                    }
                    ShowConfirmDialog("Order number " + OrderNumber + " has been removed.");
                    break;
                case "Modify":
                    // get the order address to modify
                    _presenterReportManager.FillPatientData();
                    ShowEditDialog("Edit Patient Mailing Address");
                    break;
            }

        }
        protected void btnAddOrderToGrid_Click(object sender, EventArgs e)
        {
            // if there are history items in grid, clear them first
            string isReprint = ViewState["IsReprint"] as String;
            if (!string.IsNullOrEmpty(isReprint) && isReprint == "true")
            {
                clearOrderAddresses_Click(sender, e);
            }
            List<string> orderNumbers = new List<string>();
            switch ((sender as Button).CommandName)
            {
                case "Single": // single order
                    orderNumbers.Add(tbSingleReadScan.Text);
                    tbSingleReadScan.Text = string.Empty;
                    break;
                case "Bulk": // bulk orders
                    orderNumbers = hdfBulkOrders.Value.Split(',').ToList();
                    break;
                case "SaveVerifiedAddress": // runtime
                    orderNumbers.Add(mySession.SelectedOrder.OrderNumber);
                    break;
            }


            foreach (string n in orderNumbers)
            {
                GetOrderDataForLabel(n, null);
            }
            //hdfIsGridItems.Value = "true";
            PrintOrderNumbers = orderNumbers;
            orderNumbers.Clear();
            string numbers = string.Empty;
            if (OrderNumbersNotAdded.Count > 0)
            {
                numbers = string.Join("<br />", OrderNumbersNotAdded);
                ShowErrorDialog("The following order numbers could not be added: <br /><br /> " + numbers + "<br /><br />");
                OrderNumbersNotAdded.Clear();
            }
        }



        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if ((sender as Button).Text.ToLower() == "history")
                {
                    var p = new ReportsManagerPresenter(this);
                    var SiteCode = string.IsNullOrEmpty(mySession.MySite.SiteCode) ? Globals.SiteCode : mySession.MySite.SiteCode;
                    var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
                    p.GetOnDemandCountItems(MyUserID, SiteCode);
                    if (ReprintItems != null && ReprintItems.Count > 0)
                    {
                        ViewState["IsReprint"] = "true";
                        divHistoryItems.Visible = true;
                        ddlHistoryItems.Focus();

                        gvOrderAddresses.Columns[2].HeaderText = "Address"; // change patient address column header
                        gvOrderAddresses.Columns[3].Visible = false; // hide order address column
                        gvOrderAddresses.Columns[4].Visible = false; // hide actions column

                        DisplayEmptyHistoryGrid();
                        upAddresses.Update();
                        uplOrderAddresses.Update();
                        return;
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "IsHistoryValid", "PerformFunction('IsFunctionValid','history', 'false');", true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            try
            {
                List<OrderLabelAddresses> orderLabelAddressList = new List<OrderLabelAddresses>();
                if (OrderAddressesList != null && OrderAddressesList.Count > 0)
                {
                    List<ReprintOnDemandInsertEntity> rLabel = new List<ReprintOnDemandInsertEntity>();
                    foreach (GridViewRow row in gvOrderAddresses.Rows)
                    {
                        ReprintOnDemandInsertEntity ent = new ReprintOnDemandInsertEntity();
                        CheckBox chkUseMailingAddress = (CheckBox)row.FindControl("chkUseMailingAddress");
                        Label OrderNumber = (Label)row.FindControl("OrderNumber");

                        if (chkUseMailingAddress.Checked == true)
                        {
                            foreach (OrderLabelAddresses oa in OrderAddressesList)
                            {
                                if (oa.OrderNumber == OrderNumber.Text)
                                {
                                    oa.UseMailingAddress = true;
                                    ent.AddressType = "mail";
                                }
                            }
                        }
                        else
                        {
                            ent.AddressType = "order";
                        }
                        ent.OrderNumber = OrderNumber.Text;
                        ent.SiteCode = this.mySession.MySite.SiteCode;

                        rLabel.Add(ent);

                        string myId = string.Concat(this.mySession.MyUserID.TakeWhile((c) => c != '-'));

                        string isReprint = ViewState["IsReprint"] as String;
                        if (string.IsNullOrEmpty(isReprint) || isReprint == "false")
                        {
                            var result = _presenterReportManager.InsertOnDemandReprintLabel(ent, myId);
                        }
                        orderLabelAddressList = OrderAddressesList;
                    }
                    var view = new SrtsReportsManager();
                    view.mySession = mySession;
                    view.SelectedLabel = SelectedLabel;
                    _presenterReportManager.PrintLabels(orderLabelAddressList, SrtsWeb.Reports.ReportCommon.GetTableTemplateForLabels());
                    hiddenDownload.Src = "~/WebForms/PrintForms/LabelRptViewer.aspx";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "PerformFunction", "PerformFunction('IsFunctionValid','print', 'false');", true);
                }
            }
            catch (Exception ex)
            {
                //ex.TraceErrorException();
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        protected void ddlHistoryItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (OrderAddressesList != null) { OrderAddressesList.Clear(); }

                if (ddlHistoryItems.SelectedIndex > 0 && ReprintItems != null)
                {
                    SelectedReprintItem = this.ReprintItems.FirstOrDefault(x => x.CombinedKey == this.ddlHistoryItems.SelectedValue);
                    var p = new ReportsManagerPresenter(this);
                    if (ReprintOnDemandLabels != null && ReprintOnDemandLabels.Count > 0)
                    {
                        ReprintOnDemandLabels.Clear();
                    }

                    p.GetReprintLabels("od", this.mySession.MySite.SiteCode, this.SelectedReprintItem.BatchDate);
                    Session["StatusId"] = "od";

                    if (ReprintOnDemandLabels != null)
                    {
                        ViewState["IsReprint"] = "true";
                        DisplayEmptyHistoryGrid();
                        foreach (ReprintOnDemandReturnEntity n in ReprintOnDemandLabels)
                        {
                            GetOrderDataForLabel(n.Ordernumber, n.AddressType);
                        }

                    }
                }
                else { return; }
            }
            catch
            {
                //;
            }

        }

        [System.Web.Services.WebMethod]
        public static string GetUSPSAddressbyZip(string data)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////
            //expected response sample
            //<?xml version="1.0" encoding="UTF-8"?>
            //<CityStateLookupResponse>
            //<ZipCode ID="0">
            //<Zip5>90210</Zip5>
            //<City>BEVERLY HILLS</City>
            //<State>CA</State>
            //</ZipCode>
            //</CityStateLookupResponse>
            // Note:  This method is also used in PersonDetails.cs
            /* It is a common thing that some of your application functionalities depend on an external HTTPS endpoint. However, renewal of SSL certificate for the external party is out of your control and you have                  to rely on the third party that certificate will be renewed on time. If renewal does not happen on time, SSL certificate becomes invalid.
                .NET has by default build in mechanism to throw an exception if you are trying to make a WebRequest to HTTPS endpoint which has invalid SSL certificate. In other words, .NET is doin SSL certificate                   validation for you under the hood.
             Error Message received if invalid certificate: 
             "The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel."inner Exception.Message is:"The remote certificate is invalid according to the validation procedure."

            This exception is caused by invalid or expired SSL certificate. As soon as SSL certificate is expired, server will start to use self-signed certificate which fails validation.
             */
            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            try
            {
                var ss = HttpContext.Current.Session["SRTSSession"] as SRTSSession;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_GetUSPSAddressbyZip", ss.MyUserID))
#endif
                {
                    // build the xml request string
                    string Zip5 = data.Substring(0, 5);
                    string userID = ConfigurationManager.AppSettings["uspsUsername"];
                    string requestBaseURL = ConfigurationManager.AppSettings["uspsAPICallBase"];
                    string requestStartAPICall = "?API=CityStateLookup&XML=<CityStateLookupRequest";
                    string requestUserID = " USERID=" + '"' + userID + '"' + ">";
                    string requestQueryID = "<ZipCode ID= " + '"' + "0" + '"' + ">";
                    string requestQueryItem = "<Zip5>" + Zip5 + "</Zip5>";
                    string requestEndAPICall = "</ZipCode></CityStateLookupRequest>";
                    string myRequest = String.Format("{0}{1}{2}{3}{4}{5}",
                        requestBaseURL,
                        requestStartAPICall,
                        requestUserID,
                        requestQueryID,
                        requestQueryItem,
                        requestEndAPICall);
                    // send the request
                    XmlDocument responseDoc = new XmlDocument();
                    responseDoc.XmlResolver = null;
                    try
                    {
                        // Error generated if ServicePointManager is not used:
                        //The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.
                        //- Inner Exception: System.Security.Authentication.AuthenticationException: The remote certificate is invalid according to the validation procedure.
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        responseDoc.Load(myRequest);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message + " - Inner Exception: " + ex.InnerException;
                    }

                    // parse the xmldocument response to get the address info
                    string result;

                    // check response for error
                    XmlNodeList error = responseDoc.GetElementsByTagName("Error");
                    XmlNodeList errordescription = responseDoc.GetElementsByTagName("Description");

                    // if response is good
                    if (error.Count == 0)
                    {
                        XmlNodeList city = responseDoc.GetElementsByTagName("City");
                        XmlNodeList state = responseDoc.GetElementsByTagName("State");
                        XmlNodeList zip = responseDoc.GetElementsByTagName("Zip5");

                        // return address info
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        AddressEntity uspsAddress = new AddressEntity();
                        uspsAddress.City = city[0].InnerText;
                        uspsAddress.State = state[0].InnerText;

                        result = serializer.Serialize(uspsAddress).ToString();
                    }
                    else
                    {
                        // 0 means error
                        result = "error:  " + errordescription[0].InnerText;
                    }
                    return result;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); return String.Empty; }
        }

        public static string VerifyUSPSAddress(AddressEntity ent)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////
            //expected response sample
            //<AddressValidateResponse>
            //<Address ID="0">
            //<Address2>6406 IVY LN</Address2>
            //<City>GREENBELT</City>
            //<State>MD</State>
            //<Zip5>20770</Zip5>
            //<Zip4>1441</Zip4>
            //</Address>
            //</AddressValidateResponse>
            //https://secure.shippingapis.com/ShippingAPITest.dll?API=Verify&XML=<AddressValidateRequest USERID="929DEFEN1030"><Address ID="0"><Address1></Address1><Address2>6406 Ivy Lane</Address2><City>Greenbelt</City><State>MD</State><Zip5></Zip5><Zip4></Zip4></Address></AddressValidateRequest>Address1><Address2>1234 é street #2</Address2><City>CLEVELAND</City><State>OH</State><Zip5>44115</Zip5><Zip4></Zip4></Address></AddressValidateRequest>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            try
            {
                var ss = HttpContext.Current.Session["SRTSSession"] as SRTSSession;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_VerifyUSPSAddress", ss.MyUserID))
#endif
                {
                    // build the xml request string
                    string Address1 = ent.Address1;
                    string Address2 = ent.Address2;
                    string City = ent.City;
                    string State = ent.State;
                    string Zip5 = ent.ZipCode;
                    string userID = ConfigurationManager.AppSettings["uspsUsername"];
                    string requestBaseURL = ConfigurationManager.AppSettings["uspsAPICallBase"];
                    string requestStartAPICall = "?API=Verify&XML=<AddressValidateRequest";
                    string requestUserID = " USERID=" + '"' + userID + '"' + ">";
                    string requestAddress = "<Address ID=" + '"' + "0" + '"' + ">";
                    string requestAddress1 = "<Address1></Address1>";
                    string requestAddress2 = "<Address2>" + Address1 + " " + Address2 + "</Address2>";
                    string requestCity = "<City>" + City + "</City>";
                    string requestState = "<State>" + State + "</State>";
                    string requestZip5 = "<Zip5>" + Zip5 + "</Zip5>";
                    string requestZip4 = "<Zip4></Zip4>";
                    string requestEndAddress = "</Address>";
                    string requestEndAPICall = "</AddressValidateRequest>";
                    string getResponse = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}",
                        requestBaseURL,
                        requestStartAPICall,
                        requestUserID,
                        requestAddress,
                        requestAddress1,
                        requestAddress2,
                        requestCity,
                        requestState,
                        requestZip5,
                        requestZip4,
                        requestEndAddress,
                        requestEndAPICall
                        );

                    // send the request
                    ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                    XmlDocument responseDoc = new XmlDocument();
                    responseDoc.XmlResolver = null;
                    responseDoc.Load(getResponse);

                    // parse the xmldocument response to get the address info
                    string result;

                    // check response for error
                    XmlNodeList error = responseDoc.GetElementsByTagName("Error");
                    XmlNodeList errordescription = responseDoc.GetElementsByTagName("Description");

                    // if response is good
                    if (error.Count == 0)
                    {
                        XmlNodeList address2 = responseDoc.GetElementsByTagName("Address2");
                        XmlNodeList city = responseDoc.GetElementsByTagName("City");
                        XmlNodeList state = responseDoc.GetElementsByTagName("State");
                        XmlNodeList zip5 = responseDoc.GetElementsByTagName("Zip5");
                        XmlNodeList zip4 = responseDoc.GetElementsByTagName("Zip4");

                        // return address info
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        AddressEntity uspsAddress = new AddressEntity();
                        uspsAddress.Address1 = address2[0].InnerText;
                        uspsAddress.City = city[0].InnerText;
                        uspsAddress.State = state[0].InnerText;
                        uspsAddress.ZipCode = zip5[0].InnerText + "-" + zip4[0].InnerText;


                        SrtsReportsManager pd = HttpContext.Current.Handler as SrtsReportsManager;
                        if (pd != null)
                        {
                            pd.ViewState["VerifiedAddress"] = uspsAddress;
                        }
                        result = serializer.Serialize(uspsAddress).ToString();
                    }
                    else
                    {
                        // 0 means error
                        result = "error:  " + errordescription[0].InnerText;
                    }
                    return result;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); return String.Empty; }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

            FormsAuthentication.RedirectToLoginPage();

        }

        public void GetOrderDataForLabel(string orderNumber, string addressType)
        {
            var _order = GetAddressesByOrderNumber(orderNumber);


            if (_order != null)
            {
                tbSingleReadScan.Text = string.Empty;
                List<OrderLabelAddresses> patientAddresses = new List<OrderLabelAddresses>();
                List<string> patientOrderNumbers = new List<string>();

                if (OrderAddressesList != null)
                {

                    patientAddresses = OrderAddressesList;
                    foreach (OrderLabelAddresses o in patientAddresses)
                    {
                        patientOrderNumbers.Add(o.OrderNumber);
                    }
                }
                if (!string.IsNullOrEmpty(addressType))
                {
                    // specific to reprint history items
                    if (addressType == "mail") { _order.UseMailingAddress = true; }
                    if (addressType == "order") { _order.UseMailingAddress = false; }
                }
                OrderLabelAddresses patientAddress = new OrderLabelAddresses
                {
                    OrderNumber = _order.OrderNumber,
                    FirstName = _order.FirstName,
                    MiddleName = _order.MiddleName,
                    LastName = _order.LastName,
                    PatientId = _order.PatientId,
                    Address1 = _order.Address1,
                    Address2 = _order.Address2,
                    Address3 = _order.Address3,
                    City = _order.City,
                    State = _order.State,
                    CountryCode = _order.CountryCode,
                    CountryName = _order.CountryName,
                    ZipCode = _order.ZipCode,
                    ShipAddress1 = _order.ShipAddress1,
                    ShipAddress2 = _order.ShipAddress2,
                    ShipAddress3 = _order.ShipAddress3,
                    ShipCity = _order.ShipCity,
                    ShipState = _order.ShipState,
                    ShipCountryCode = _order.ShipCountryCode,
                    ShipCountryName = _order.ShipCountryName,
                    ShipZipCode = _order.ShipZipCode,
                    UseMailingAddress = _order.UseMailingAddress,
                    DateVerified = _order.DateVerified,
                    ExpireDays = _order.ExpireDays
                };
                // remove order number if it exists
                patientAddresses = patientAddresses.Where(x => x.OrderNumber != patientAddress.OrderNumber).ToList();
                patientAddresses.Add(patientAddress);

                patientOrderNumbers.Add(patientAddress.OrderNumber);

                OrderAddressesList = patientAddresses;
                Session["OrderAddresses"] = OrderAddressesList;
                PrintOrderNumbers.Clear();
                PrintOrderNumbers = patientOrderNumbers;

            }
            else
            {
                List<string> numbersNotAdded = new List<string>();
                if (OrderNumbersNotAdded != null)
                {
                    numbersNotAdded = OrderNumbersNotAdded;
                }
                numbersNotAdded.Add(orderNumber);
                OrderNumbersNotAdded = numbersNotAdded;
            }
            tbSingleReadScan.Focus();
        }
        protected void bSaveAddress_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "OrderManagement_bSaveAddress_Click", mySession.MyUserID))
            //    {
            //        this.PatientAddress.ModifiedBy = Globals.ModifiedBy;
            //        this.PatientAddress.ExpireDays = 0; //not validated

            //        var good = _presenterReportManager.DoSaveAddress(this.PatientAddress);
            //        if (!good)
            //        {
            //            ScriptManager.RegisterStartupScript(this, this.GetType(), "Fail", String.Format("Confirm('{0}', 'statusMessage', true, false);", "An error occurred saving the address."), true);
            //            return;
            //        }
            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "Close", "CloseEditDialogMessage('true');", true);


            //        foreach (OrderLabelAddresses address in OrderAddressesList)
            //        {
            //            GetOrderDataForLabel(address.OrderNumber, null);
            //        }
            //        //PrintOrderNumbers = orderNumbers;


            //        BindGridviewOrderAddresses();
            //        ShowConfirmDialog("Address successfully updated.");
            //        upAddresses.Update();
            //        uplOrderAddresses.Update();

            //        LogEvent("Address saved successfully by user {0} at {1}.", new Object[] { mySession.MyUserID, DateTime.Now });
            //    }
            //}
            //catch (Exception ex) { ex.TraceErrorException(); }


            //////////////////////////////////////////////////////////////////////////

            try
            {
                if (hdfIsValid.Value == "false") { return; }
                if (this.rblAddressType.SelectedValue == "FN")
                {
                    SaveAddress(sender, e);
                }
                else
                {
                    this.edAddress.ID = this.PrimaryAddress.ID;
                    this.edAddress.IndividualID = this.PrimaryAddress.IndividualID;
                    this.edAddress.Address1 = CleanAddress(this.tbPrimaryAddress1.Text);
                    this.edAddress.Address2 = CleanAddress(this.tbPrimaryAddress2.Text);
                    this.edAddress.City = CleanAddress(this.tbPrimaryCity.Text);
                    this.edAddress.State = this.ddlPrimaryState.SelectedValue;
                    this.edAddress.ZipCode = this.tbPrimaryZipCode.Text;
                    this.edAddress.Country = this.ddlPrimaryCountry.SelectedValue;
                    ViewState["EnteredAddress"] = this.edAddress;
                    if (this.edAddress.Country != "" && this.edAddress.Country == "US" || this.edAddress.Country == "UM")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseEditDialog", "CloseAddressEditDialogMessage();", true);
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearPreviousAddress", "ClearPreviousAddress();", true);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "GetSavedAddress", "SetAddress();", true);
                        var result = VerifyUSPSAddress(this.edAddress);
                        var array = result.Split(new string[] { ":" }, StringSplitOptions.None);

                        if (array[0].ToLower() != "error")
                        {
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "VerifyAddressResult", "USPSVerifyAddressResult(" + result + ");", true);
                        }
                        else
                        {
                            string msg = array[1].ToString();
                            string newMsg = Regex.Replace(msg, "[@,\\.\";'\\\\]", string.Empty);

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "VerifyAddressResult", "USPSVerifyAddressResult('" + array[0].ToLower() + "');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }


        }
        protected void btnCancelAddressSave_Click(object sender, EventArgs e)
        {
            //string patient;
            //bool isPatient = this.IsPatientView;
            //if (isPatient)
            //{
            //    patient = "&isP=true";
            //}
            //else
            //{
            //    patient = "&isP=false";
            //}
            ////reload person details page
            //var redirectURL = "../SrtsPerson/PersonDetails.aspx?id=" + mySession.SelectedPatientID + patient;
            //this.Redirect(redirectURL, false);
        }
        public static string CleanAddress(string textIn)
        {
            string textout = string.Empty;
            char[] arr = textIn.Where(c => (char.IsLetterOrDigit(c) ||
                             char.IsWhiteSpace(c))).ToArray();
            textout = new string(arr);
            return textout;
        }

        protected void SaveAddress(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_bSaveAddress_Click", mySession.MyUserID))
#endif
            {
                try
                {
                    string btnName;
                    if (sender.GetType().Name.ToString() == "ImageButton")
                    {
                        btnName = ((ImageButton)sender).CommandName;
                    }
                    else
                    {
                        btnName = ((Button)sender).CommandName;
                    }
                    switch (btnName)
                    {
                        case "SaveEnteredAddress":
                            this.edAddress = ViewState["EnteredAddress"] as AddressEntity;
                            this.edAddress.ID = this.PrimaryAddress.ID;
                            this.edAddress.IndividualID = this.PrimaryAddress.IndividualID;
                            this.edAddress.Country = this.PrimaryAddress.Country;
                            this.PrimaryAddress = this.edAddress;
                            this.PrimaryAddress.ID = mySession.Patient.Addresses.FirstOrDefault(x => x.IsActive == true).ID;
                            this.PrimaryAddress.ModifiedBy = Globals.ModifiedBy;
                            this.PrimaryAddress.ExpireDays = 30; //valid for 30 days
                            break;
                        case "SaveVerifiedAddress":
                            this.VerifiedAddress = ViewState["VerifiedAddress"] as AddressEntity;
                            this.VerifiedAddress.ID = this.PrimaryAddress.ID;
                            this.VerifiedAddress.IndividualID = mySession.SelectedPatientID;
                            this.VerifiedAddress.Country = this.PrimaryAddress.Country;
                            this.PrimaryAddress = this.VerifiedAddress;
                            this.PrimaryAddress.ModifiedBy = Globals.ModifiedBy;
                            if (mySession.Patient.Addresses != null && mySession.Patient.Addresses.Count > 0)
                            {
                                this.PrimaryAddress.ID = mySession.Patient.Addresses.FirstOrDefault(x => x.IsActive == true).ID;
                            }
                            else
                            {
                                //var _repository = new IndividualRepository.PatientRepository();
                                //var pe = _repository.GetAllPatientInfoByIndividualID(PatientId, true, Globals.ModifiedBy, mySession.MySite.SiteCode);
                                this.PrimaryAddress.ID = 0;
                            }


                            this.PrimaryAddress.ModifiedBy = Globals.ModifiedBy;
                            this.PrimaryAddress.ExpireDays = 90; //valid for 90 days
                            break;
                    }



                    if (this.rblAddressType.SelectedValue == "FN")
                    {
                        this.PrimaryAddress.ExpireDays = 30;
                    }

                    var good = _presenterReportManager.DoSaveAddress(this.PrimaryAddress);
                    //if (sender.ToString() != "DeersRefreshClick")
                    //{
                    var m = String.Empty;
                    if (good)
                    {
                        List<string> orderNumbers = new List<string>();
                        foreach (OrderLabelAddresses oa in OrderAddressesList)
                        {
                            orderNumbers.Add(oa.OrderNumber);
                        }
                        foreach (string n in orderNumbers)
                        {
                            GetOrderDataForLabel(n, null);
                        }


                        PrintOrderNumbers = orderNumbers;

                        //orderNumbers.Clear();

                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        AddressEntity newAddress = new AddressEntity();
                        newAddress = Session["PrimaryAddress"] as AddressEntity;
                        var savedAddress = "";
                        if (newAddress != null)
                        {
                            hdfVerifiedExpiry.Value = "";
                            hdfDateVerified.Value = "";
                            this.PrimaryAddress = newAddress;
                            savedAddress = serializer.Serialize(newAddress).ToString();
                        }
                        else
                        {
                            savedAddress = serializer.Serialize(this.PrimaryAddress).ToString();
                        }

                        // ValidateAddress();

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "SetSavedAddress", "SetSavedAddress(" + savedAddress + "," + newAddress.ExpireDays + ");", true);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Close", "CloseAddressVerificationDialog();", true);
                        //upAddresses.Update();
                        //UpdatePanel1.Update();
                        m = "Address saved successfully";
                        ShowConfirmDialog(m);

                        if (Session["CurrentProcess"] != null && Session["CurrentOrderNumber"] != null)
                        {
                            var currentProcess = Session["CurrentProcess"];
                            var currentOrderNumber = Session["CurrentOrderNumber"];

                            if (currentProcess.ToString() == "Order Management" || currentProcess.ToString() == "Do New Order")
                            {
                                //redirect to order management page
                                var redirectURL = "../SrtsOrderManagement/OrderManagement.aspx";
                                if (!CustomRedirect.SanitizeRedirect(redirectURL, false))
                                {
                                    ShowErrorDialog("An Error has occurred after saving the address.");
                                };
                                //CustomRedirect.SanitizeRedirect(redirectURL, false);
                                //if (IsLocalUrl(redirectURL) && !Helpers.isAbsolute(redirectURL))
                                //{
                                //    this.Redirect(redirectURL, false);
                                //}                                   
                            }
                        }

                    }
                    else
                    {
                        m = "Address unsuccessfully saved";
                        ShowContactMessage(this.AddressMessage, "addressMessage", !good, false);
                    }
                    LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));

                    this.AddressMessage = String.Empty;
                    string rawURL = Request.RawUrl;
                    if (!CustomRedirect.SanitizeRedirect(rawURL, false))
                    {
                        ShowErrorDialog("An Error has occurred.");
                    };
                    //CustomRedirect.SanitizeRedirect(rawURL, true);
                    //if(IsLocalUrl(rawURL) && !Helpers.isAbsolute(rawURL))
                    //{
                    //Response.Redirect(Request.RawUrl);
                    //}
                }
                catch (Exception ex) { ex.TraceErrorException(); }
            }
        }


        protected void gvOrderAddresses_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState == DataControlRowState.Normal)
                e.Row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState == DataControlRowState.Alternate)
                e.Row.BackColor = ColorTranslator.FromHtml('"' + colorAlternate + '"');
        }

        protected void gvOrderAddresses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                OrderLabelAddresses drv = (OrderLabelAddresses)e.Row.DataItem;
                int patientId = Convert.ToInt32(drv.PatientId);

                CheckBox chkUseMailingAddress = (CheckBox)e.Row.FindControl("chkUseMailingAddress");
                chkUseMailingAddress.Attributes.Add("onmouseover", "GetToolTip('" + e.Row.RowIndex + "','" + drv.OrderNumber + "')");
                chkUseMailingAddress.Attributes.Add("onmouseout", "Hide('lblIsValid" + drv.OrderNumber + "')");


                var expireDays = drv.ExpireDays;
                var siteType = mySession.MySite.SiteType;
                DateTime dateVerified = drv.DateVerified.ToDateTime();

                ImageButton btnEdit = (ImageButton)e.Row.FindControl("btnEdit");
                Label sState = (Label)e.Row.FindControl("ShipState");
                Label State = (Label)e.Row.FindControl("State");
                Label Patient = (Label)e.Row.FindControl("Patient");
                Label CountryName = (Label)e.Row.FindControl("CountryName");
                Label lblIsValid = (Label)e.Row.FindControl("lblIsValid");

                if (drv.State == "NA")  // 'NA' should indicate this is a foreign address; we do not need to show state
                {
                    State.Text = "";
                }
                if (drv.ShipState == "NA")  // 'NA' should indicate this is a foreign address; we do not need to show state
                {
                    sState.Text = "";
                }
                if (drv.CountryCode == "US" || drv.CountryCode == "UM")
                {
                    CountryName.Visible = false;
                }

                if (siteType == "LAB") 
                {
                    btnEdit.Visible = false;// lab should not be able to edit the address

                    if (expireDays == 0 || expireDays == null)  // address needs to be validated.
                    {
                        chkUseMailingAddress.Enabled = false;
                        
                        lblIsValid.Text = "This address can not be used because it needs to be validated.";
                    }
                    else
                    {
                        chkUseMailingAddress.Enabled = true;
                        chkUseMailingAddress.Attributes.Remove("onmouseover");
                        chkUseMailingAddress.Attributes.Remove("onmouseout");
                    }                
                }

                if (siteType != "LAB" && expireDays == 0 || expireDays == null)
                {
                    chkUseMailingAddress.Enabled = false;
                    btnEdit.Visible = true;
                    lblIsValid.Text = "This address needs to be validated before it can be used.";
                }
                else
                {
                    chkUseMailingAddress.Enabled = true;
                    btnEdit.Visible = true;
                    lblIsValid.Text = string.Empty;
                }

                lblIsValid.ID = Server.HtmlEncode("lblIsValid" + drv.OrderNumber);

                string mInitial = string.IsNullOrEmpty(drv.MiddleName) ? " " : " " + drv.MiddleName.FirstOrDefault().ToString() + ". ";
                Patient.Text = Server.HtmlEncode(drv.FirstName + mInitial + drv.LastName);

                if (drv.UseMailingAddress == true)
                {
                    chkUseMailingAddress.Checked = true;

                    // if this is history display
                    // show mailing address in order address column
                    // find order address column fields
                    string isReprint = ViewState["IsReprint"] as String;
                    if (!string.IsNullOrEmpty(isReprint) && isReprint == "true")
                    {
                        HtmlGenericControl pShipAddress1 = (HtmlGenericControl)e.Row.FindControl("pShipAddress1");
                        HtmlGenericControl pShipAddress2 = (HtmlGenericControl)e.Row.FindControl("pShipAddress2");
                        HtmlGenericControl pShipAddress3 = (HtmlGenericControl)e.Row.FindControl("pShipAddress3");
                        Label ShipCity = (Label)e.Row.FindControl("ShipCity");
                        Label ShipState = (Label)e.Row.FindControl("ShipState");
                        Label ShipZipCode = (Label)e.Row.FindControl("ShipZipCode");
                        Label ShipCountryName = (Label)e.Row.FindControl("ShipCountryName");

                        // display mailing address data in order address fields
                        pShipAddress1.InnerText = drv.Address1;
                        pShipAddress2.InnerText = drv.Address2;
                        pShipAddress3.InnerText = drv.Address3;
                        ShipCity.Text = Server.HtmlEncode(drv.City);
                        ShipState.Text = Server.HtmlEncode(drv.State);
                        ShipZipCode.Text = Server.HtmlEncode(drv.ZipCode);
                        if (drv.CountryCode == "US" || drv.CountryCode == "UM")
                        {
                            ShipCountryName.Visible = false;
                        }
                    }
                }
            }
        }
        private void ShowEditDialog(String msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "DisplayEditDialogMessage('Success!','" + msg + "', 'success');", true);
        }
        private void ShowConfirmDialog(String msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Success!','" + msg + "', 'success');", true);
        }
        private void ShowErrorDialog(String msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Error!','" + msg + "', 'error');", true);
        }
        private void ShowContactMessage(String msg, String msgCtId, Boolean isError, Boolean persistMsg)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Confirm", String.Format("Confirm('{0}', '{1}', {2}, {3});", msg, msgCtId, isError.ToString().ToLower(), persistMsg.ToString().ToLower()), true);
        }
        protected void OrderCheckBox_OnCheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;

            if (chk.Checked)
            {
                GridViewRow row = (GridViewRow)chk.NamingContainer;
                string orderNumber = row.Cells[0].Text;
                string addressType = row.Cells[1].Text;
            }


        }
        public static OrderLabelAddresses GetAddressesByOrderNumber(String OrderNumber)
        {
            var _repository = new OnDemandLabelsRepository.OrderLabelAddressesRepository();
            var orderAddresses = _repository.GetAddressesByOrderNumber(OrderNumber);
            return orderAddresses;
        }
        public OrderLabelAddresses OrderAddresses
        {
            get
            {
                return ViewState["OrderAddress"] as OrderLabelAddresses;
            }
            set
            {
                ViewState["OrderAddress"] = value;
            }
        }
        public List<ReprintOnDemandInsertEntity> InsertReprintLabels
        {
            get
            {
                return ViewState["InsertOnDemandReprintLabels"] as List<ReprintOnDemandInsertEntity>;
            }
            set
            {
                ViewState["InsertOnDemandReprintLabels"] = value;
            }
        }

        public List<OrderLabelAddresses> OrderAddressesList
        {
            get
            {
                return ViewState["OrderAddresses"] as List<OrderLabelAddresses>;
            }
            set
            {
                ViewState["OrderAddresses"] = value;
                this.gvOrderAddresses.DataSource = value;
                this.gvOrderAddresses.DataBind();
            }
        }

        public String SelectedLabel
        {
            get
            {
                return ViewState["SelectedLabel"].IsNull() ? this.ddlSelectLabel.SelectedValue : ViewState["SelectedLabel"].ToString();
            }
            set
            {
                ViewState["SelectedLabel"] = value;
            }
        }

        public List<String> PrintOrderNumbers
        {
            get
            {
                var numbers = new List<String>();
                if (!ViewState["PrintOrderNbrs"].IsNull())
                    numbers = ViewState["PrintOrderNbrs"] as List<String>;
                return numbers;
            }
            set
            {
                ViewState["PrintOrderNbrs"] = value;
            }
        }

        public List<String> OrderNumbersNotAdded
        {
            get
            {
                var n = new List<String>();
                if (!ViewState["OrderNumbersNotAdded"].IsNull())
                    n = ViewState["OrderNumbersNotAdded"] as List<String>;
                return n;
            }
            set
            {
                ViewState["OrderNumbersNotAdded"] = value;
            }
        }

        public Boolean UseMailingAddress
        {
            get
            {
                var _useMailingAddress = Session["UseMailingAddress"].IsNull() ? false : (Boolean)ViewState["UseMailingAddress"];
                return _useMailingAddress;
            }
            set
            {
                Session["UseMailingAddress"] = value;
            }
        }

        public Int32 PatientId
        {
            get { return Convert.ToInt32(Session["PatientId"]); }
            set { Session["PatientId"] = value; }
        }

        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String LastName { get; set; }

        public AddressEntity PrimaryAddress
        {
            get
            {
                var e = ViewState["PrimaryAddress"] as AddressEntity;
                if (e.IsNull())
                    e = new AddressEntity();
                e.Address1 = this.tbPrimaryAddress1.Text;
                e.Address2 = this.tbPrimaryAddress2.Text;
                e.City = this.tbPrimaryCity.Text;
                e.State = this.ddlPrimaryState.SelectedValue;
                e.Country = this.ddlPrimaryCountry.SelectedValue;
                e.ZipCode = this.tbPrimaryZipCode.Text;
                e.UIC = this.tbPrimaryUIC.Text;
                e.IsActive = true;
                return e;
            }
            set
            {
                if (value.IsNull()) return;
                ViewState["PrimaryAddress"] = value;
                this.tbPrimaryAddress1.Text = value.Address1;
                this.tbPrimaryAddress2.Text = value.Address2;
                this.tbPrimaryCity.Text = value.City;
                hdfCity.Value = value.City;
                this.ddlPrimaryState.SelectedValue = value.State;
                hdfState.Value = value.State;
                this.ddlPrimaryCountry.SelectedValue = value.Country;
                this.tbPrimaryZipCode.Text = value.ZipCode;
                this.tbPrimaryUIC.Text = value.UIC;
                hdfDateVerified.Value = value.DateVerified.ToShortDateString();
                hdfVerifiedExpiry.Value = value.ExpireDays.ToString();
            }
        }

        public string AddressMessage
        {
            get;
            set;
        }



        public AddressEntity PatientAddress
        {
            get
            {
                var a = ViewState["PatientAddress"] as AddressEntity;
                if (a.IsNull()) a = new AddressEntity();

                a.Address1 = this.tbPrimaryAddress1.Text;
                a.Address2 = this.tbPrimaryAddress2.Text;
                a.City = this.tbPrimaryCity.Text;
                a.Country = this.ddlPrimaryCountry.SelectedValue;
                a.IndividualID = this.PatientId;
                a.IsActive = true;
                a.State = this.ddlPrimaryState.SelectedValue;
                a.UIC = this.tbPrimaryUIC.Text;
                a.ZipCode = this.tbPrimaryZipCode.Text;

                return a;
            }
            set
            {
                if (value.IsNull()) return;

                ViewState["PatientAddress"] = value;

                this.tbPrimaryAddress1.Text = value.Address1;
                this.tbPrimaryAddress2.Text = value.Address2;
                this.tbPrimaryCity.Text = value.City;
                this.ddlPrimaryCountry.SelectedValue = String.IsNullOrEmpty(value.Country) ? "US" : value.Country;
                this.ddlPrimaryState.SelectedValue = value.State;
                this.tbPrimaryUIC.Text = value.UIC;
                this.tbPrimaryZipCode.Text = value.ZipCode;
            }
        }

        public List<LookupTableEntity> Countries
        {
            get
            {
                return ViewState["Countries"] as List<LookupTableEntity>;
            }
            set
            {
                ViewState["Countries"] = value;
                this.ddlPrimaryCountry.DataTextField = "ValueTextCombo";
                this.ddlPrimaryCountry.DataValueField = "Value";
                this.ddlPrimaryCountry.DataSource = value;
                this.ddlPrimaryCountry.DataBind();
                this.ddlPrimaryCountry.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlPrimaryCountry.SelectedIndex = 0;
            }
        }

        public List<LookupTableEntity> States
        {
            get
            {
                return ViewState["States"] as List<LookupTableEntity>;
            }
            set
            {
                ViewState["States"] = value;
                this.ddlPrimaryState.DataTextField = "ValueTextCombo";
                this.ddlPrimaryState.DataValueField = "Value";
                this.ddlPrimaryState.DataSource = value;
                this.ddlPrimaryState.DataBind();
                this.ddlPrimaryState.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlPrimaryState.SelectedIndex = 0;
            }
        }

        public IndividualEntity Patient
        {
            get
            {
                return Session["Patient"] as IndividualEntity;
            }
            set
            {
                Session["Patient"] = value;
            }
        }

        public bool PatientHasAddress
        {
            get
            {
                return Convert.ToBoolean(Session["PatientHasAddress"]);
            }
            set
            {
                Session["PatientHasAddress"] = value;
            }
        }
    }
}