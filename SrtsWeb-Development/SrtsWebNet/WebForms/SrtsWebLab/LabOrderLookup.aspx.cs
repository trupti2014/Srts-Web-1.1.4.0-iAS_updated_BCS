using Microsoft.Web.Administration;
using SrtsWeb;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Patients;
using SrtsWeb.Presenters.Orders;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Presenters.Individuals;
using SrtsWeb.Views.Individuals;
using SrtsWeb.Views.Patients;
using SrtsWeb.Views.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWebLab
{
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabMail")]

    public partial class LabOrderLookup : PageBase, IManagePatientsView, IOrderManagementView, ISiteMapResolver
    {
        private ManagePatientsPresenter _presenter;
        private OrderManagementPresenter _presenterOrderManagement;
        public LabOrderLookup()
        {
            _presenter = new ManagePatientsPresenter(this);
            _presenterOrderManagement = new OrderManagementPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.CurrentModuleTitle = string.Empty;
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "LabOrderLookup_Page_Load", mySession.MyUserID))
#endif
                {
                    if (!IsPostBack)
                    {
                        if (mySession == null)
                            mySession = new SRTSSession();

                        litPageMessage.Visible = true;
                        SearchSortDirection = "ASC";

                        if (Roles.IsUserInRole("LabClerk") || Roles.IsUserInRole("LabTech"))
                        {
                            this.pnlSearch.DefaultButton = this.btnSearchGlobal.UniqueID;
                        }
                    }
                    if (SearchData != null && SearchData.Count > 0)
                    {
                        BindSearch();
                        divSearchResults.Visible = true;
                    }
                }
                BuildPageTitle();
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = "Lab Orders - Lab Order Lookup";
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Lab Orders");
                CurrentModule_Sub(string.Empty);
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child;

            child = new SiteMapNode(e.Provider, "2", "~/SrtsWebLab/LabOrderLookup.aspx", "Lab Order Lookup"); //break;

            return child;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/SrtsWebLab/LabOrderLookup.aspx");
        }

        public void rbSearch_Click(object sender, CommandEventArgs e)
        {
            try           
            {
                litContentTop_Title_Right.Text = string.Empty;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "LabOrderLookup_rbSearch_Click", mySession.MyUserID))
#endif
                {
                    CustomValidator err;

                    if (String.IsNullOrEmpty(this.LastName) && String.IsNullOrEmpty(this.SearchID))
                    {
                        err = new CustomValidator();
                        err.IsValid = false;
                        err.ErrorMessage = "A last name or ID number is required.  First name is an optional entry.";
                        Page.Validators.Add(err);
                        return;
                    }

                    // Check for a minimum of 2 characters in the last name field after a trim
                    if (this.LastName.Length < 2 && String.IsNullOrEmpty(this.SearchID))
                    {
                        err = new CustomValidator();
                        err.IsValid = false;
                        err.ErrorMessage = "A minimum of 2 characters is required when doing a last name search.";
                        Page.Validators.Add(err);
                        return;
                    }

                    litPageMessage.Visible = false;

                    SortColumn = null;
                    SearchSortDirection = null;

                    gvSearch.DataSource = null;
                    gvSearch.DataBind();

                    List<IndividualEntity> lie;

                    if (((String)e.CommandArgument) == "S") // Local Search
                        lie = _presenter.SearchIndividual("S");
                    else // Global Search
                        lie = _presenter.SearchIndividual("G");

                    if (lie.Count.Equals(0))
                    {
                        SearchData = new List<IndividualEntity>();
                        litPageMessage.Text = string.Format("{0}", "No search results returned.");
                        divSearchResults.Visible = true;
                        if (this.SearchID.IsNullOrEmpty())
                            this.gvSearch.EmptyDataText = "Name search returned no results.";
                        else
                            this.gvSearch.EmptyDataText = "Please retry using SSN if DODID was used, or DODID if SSN was used.";

                        this.gvSearch.DataBind();
                        return;
                    }

                    SearchData = lie;
                    BindSearch();
                    litPageMessage.Visible = false;
                    divSearchResults.Visible = true;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void clearGrids_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        protected void ClearAll()
        {
            gvSearch.DataSource = null;
            gvSearch.DataBind();

            gvPrescription.DataSource = null;
            gvPrescription.DataBind();

            gvPatientInfo.DataSource = null;
            gvPatientInfo.DataBind();

            gvOrderInfo.DataSource = null;
            gvOrderInfo.DataBind();

            gvOrderStatusHistory.DataSource = null;
            gvOrderStatusHistory.DataBind();

            divOrderInformation.Visible = false;
            divLab.Visible = false;
            divSearchResults.Visible = false;
            tbID.Text = string.Empty;
            tbFirstName.Text = string.Empty;
            tbLastName.Text = string.Empty;
            litContentTop_Title_Right.Text = string.Empty;
        }

        protected void bQuickSearch_Click(object sender, EventArgs e)
        {
            litContentTop_Title_Right.Text = string.Empty;
            if (String.IsNullOrEmpty(this.tbID.Text.Trim())) return;
            
            var t = this.tbID.Text.Trim();
            if (t.Length < 9) return;
            switch (t.Length)
            {
                case 9:
                case 10:
                    {
                        var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;
                        var dto = MasterService.DoQuickSearchPatient(t, ModifiedBy);
                        if (dto == null)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "StatusAlert", "alert('Invalid search criteria or no results found.');", true);
                        }
                        else
                        {
                            DoPatientQuickSearch(dto);
                        }

                        break;
                    }
                case 16:
                    {
                        var st = StatusType.VIEW;
                        var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;

                        var o = MasterService.DoQuickSearchOrder(t, ModifiedBy, out st);

                        if (o == null)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "StatusAlert", String.Format("alert('No results found for order number {0}.');", t), true);
                            this.tbID.Text = String.Empty;
                            break;
                        }
                            DoLabQuickSearch(o.LabSiteCode, st);
                            divSearchResults.Visible = false;
                            divLab.Visible = false;
                        break;
                    }
                default:
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "StatusAlert", "alert('Invalid search criteria or no results found.');", true);
                    this.tbID.Text = String.Empty;
                    break;
            }
            this.tbID.Text = String.Empty;
        }
        private void DoLabQuickSearch(String siteCode, StatusType st)
        {
            litContentTop_Title_Right.Text = string.Empty;
            var t = this.tbID.Text.Trim();
            GetSelectedOrderForLab(ProcessDtoForOrderManagement());
        }

        private Boolean GetSelectedOrderForLab(String key)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "OrderManagement_GetSelectedOrderForLab", mySession.MyUserID))
#endif
            {

                var PatientOrder = GetSelectedOrder(key, false);
                if (PatientOrder == null) return false;
                Session.Add("LabSiteCode", PatientOrder.ClinicSiteCode);
                if (PatientOrder != null)
                {
                    divOrderInformation.Visible = true;
                    List<Order> lstOrders = new List<Order>();
                    lstOrders.Add(PatientOrder);
                    BindOrderInfo(lstOrders);
                    litContentTop_Title_Right.Text = string.Format("Order Number: {0}", PatientOrder.OrderNumber);
                }
                _presenterOrderManagement.GetOrderStatusHistory(PatientOrder.OrderNumber);

                if (OrderStateHistory != null && OrderStateHistory.Count > 0)
                {
                    BindOrderStatusHistory(OrderStateHistory);
                }
                return true;

            }
        }
        private Boolean GetSelectedPrescription(Int32 key)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.RxSource, "OrderManagement_GetSelectedPrescription", mySession.MyUserID))
#endif
            {
                List<Prescription> lstPrescription = new List<Prescription>();

                if (this.PrescriptionList != null & this.PrescriptionList.Count > 0)
                {
                    var thisPrescription = (from p in this.PrescriptionList
                                            where p.PrescriptionId == key
                                            select p).FirstOrDefault();

                    if (thisPrescription != null)
                    {
                        lstPrescription.Add(thisPrescription);
                        _presenterOrderManagement.GetPrescriptionProvider(thisPrescription.ProviderId);
                        this.PrescriptionList.Clear();
                        this.PrescriptionList = lstPrescription;

                        BindPrescriptionGrid();
                        return true;
                    }
                    else
                    {
                        this.PrescriptionList.Clear();
                        this.PrescriptionList = lstPrescription;
                        BindPrescriptionGrid();
                        gvPrescription.EmptyDataText = "Prescription Not found for this order.";
                        return false;
                    }
                }
                else { return false; }


            }
        }

        private void BindLabOrderHistoryGrid()
        {
                this.gvLabOrders.DataSource = this.OrderList.Where(x => x.IsComplete == true && x.IsActive == true);
                this.gvLabOrders.DataBind();
        }


        private void BindPrescriptionGrid()
        {
            this.gvPrescription.DataKeyNames = new[] { "PrescriptionId" };
            this.gvPrescription.DataSource = this.PrescriptionList;
            this.gvPrescription.DataBind();
        }

        private void BindPatientGrid(List<OrderLabelAddresses> patientAddresses)
        {
            if (patientAddresses != null && patientAddresses.Count > 0)
            {
                    this.gvPatientInfo.DataKeyNames = new[] { "PatientId" };
                    this.gvPatientInfo.DataSource = patientAddresses;
                    this.gvPatientInfo.DataBind();

            }
        }



        private void BindOrderInfo(List<Order> lstOrders)
        {
            this.gvOrderInfo.DataKeyNames = new[] { "OrderNumber" };
            this.gvOrderInfo.DataSource = lstOrders;
            this.gvOrderInfo.DataBind();
        }

        private void BindOrderStatusHistory(List<OrderStateEntity> lstOrderHistory)
        {
            this.gvOrderStatusHistory.DataKeyNames = new[] { "ID" };
            this.gvOrderStatusHistory.DataSource = lstOrderHistory;
            this.gvOrderStatusHistory.DataBind();
        }



        private void DoPatientQuickSearch(PatientOrderDTO dto)
        {
            Session.Add("PatientOrderDTO", dto);
            Session.Add("IsDtoRedirect", true);
            PatientId = dto.IndividualId;
            ClearAll();
            divSearchResults.Visible = false;
            divOrderInformation.Visible = false;
            divLab.Visible = true;
            string message = GetLabOrderHistory();
            if (!string.IsNullOrEmpty(message))
            {
                litPageMessage.Visible = true;
                litPageMessage.Text = message;
            }
        }


        private void DoClinicQuickSearch(String siteCode, StatusType st)
        {
            ProcessDtoForOrderManagement();
            // If the orders clinic site code and the logged in users site code don't match then just view the order.
            if (!siteCode.Equals(this.mySession.MySite.SiteCode))
                st = StatusType.VIEW;

            switch (st)
            {
                case StatusType.CREATED:
                case StatusType.REJECTED:
                case StatusType.RESUBMITTED:
                case StatusType.VIEW:
                case StatusType.RETURN_TO_STOCK:
                    Session.Add("IsDtoRedirect", true);
                    Response.Redirect("~/WebForms/SrtsOrderManagement/OrderManagement.aspx", true);
                    break;
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "StatusDialog", "$(function(){DoStatusDialog();});", true);
        }

        private String ProcessDtoForOrderManagement()
        {
            var st = StatusType.VIEW;
            var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;

            var o = MasterService.DoQuickSearchOrder(this.tbID.Text.Trim(), ModifiedBy, out st);
            var dto = MasterService.DoQuickSearchPatient(o.IndividualID_Patient, ModifiedBy);

            dto.OrderNumber = o.OrderNumber;
            dto.IndividualId = Convert.ToInt32(o.IndividualID_Patient);
            Session.Add("PatientOrderDTO", dto);
            PatientId = dto.IndividualId;
            _presenterOrderManagement.GetPatientData();
            _presenterOrderManagement.GetAllPrescriptions();
            return dto.OrderNumber;
        }


        private Order GetSelectedOrder(String key, Boolean forPrefill)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "LabOrderLookup_GetSelectedOrder", mySession.MyUserID))
#endif
            {

                Order o = null;
                o = _presenterOrderManagement.GetOrderByOrderNumber(key);
                if (o == null) { Session["FAIL"] = String.Format("Error retrieving order: {0}", key); return null; }

                this.PatientId = o.PatientId;
                Session.Add("LabSiteCode", o.ClinicSiteCode);
                GetSelectedPrescription(o.PrescriptionId);
                GetOrderAddresses(o.OrderNumber);
                this.Order = o;
                return this.Order;
            }
        }

        public void GetOrderAddresses(string orderNumber)
        {
            var _order = GetAddressesByOrderNumber(orderNumber);


            if (_order != null)
            {
                List<OrderLabelAddresses> patientAddresses = new List<OrderLabelAddresses>();
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
                patientAddresses.Add(patientAddress);
                List<IndividualEntity> lie;
                this.SearchID = _order.PatientId.ToString();
                lie = _presenter.SearchIndividual("G");// Global Search
                SearchData = (from i in lie
                              where i.ID == _order.PatientId
                              select i).ToList();
                PatientList = SearchData;
                litContentTop_Title_Right.Text = string.Format("{0}", _order.OrderNumber);
                if (patientAddresses != null && patientAddresses.Count > 0)
                {
                    BindPatientGrid(patientAddresses);
                    divSearchResults.Visible = false;
                    divLab.Visible = false;
                }
            }
        }


        public string GetLabOrderHistory()
        {
            string message = string.Empty;

            _presenterOrderManagement.GetPatientData();
            if (this.Patient != null)
            {
                _presenterOrderManagement.GetAllOrders();
                if (this.OrderList == null || this.OrderList.Count == 0)
                {
                    message += "No orders found.<br />";
                }
                else
                {
                    if (Patient != null)
                    {
                        litPatientName.Text = Patient.NameFMiL;
                    }
                    BindLabOrderHistoryGrid();
                }
                _presenterOrderManagement.GetAllPrescriptions();
                if (this.PrescriptionList == null || this.PrescriptionList.Count == 0)
                {
                    message += "No prescriptions found.";
                }              
            }
            else
            {
                message = "No data returned.";
            }
            return message;
        }

        public static OrderLabelAddresses GetAddressesByOrderNumber(String OrderNumber)
        {
            var _repository = new OnDemandLabelsRepository.OrderLabelAddressesRepository();
            var orderAddresses = _repository.GetAddressesByOrderNumber(OrderNumber);
            return orderAddresses;
        }

        protected void gvSearch_Sorting(object sender, GridViewSortEventArgs e)
        {
            List<IndividualEntity> lie = SearchData;

            if (SortColumn == null && e.SortExpression == "LastName")  // Set this to the Column that is sorted on page load
            {
                lie = Misc.DoSort(lie, e.SortExpression, SortDirection.Ascending);  // Set this to same dirction of sort on page load
                SearchSortDirection = "DESC";  // Set this to opposite dirction of sort on page load
            }
            else
            {
                if (SortColumn == e.SortExpression)
                {
                    if (SearchSortDirection == "ASC")
                    {
                        lie = Misc.DoSort(lie, e.SortExpression, SortDirection.Ascending);
                        SearchSortDirection = "DESC";
                    }
                    else
                    {
                        lie = Misc.DoSort(lie, e.SortExpression, SortDirection.Descending);
                        SearchSortDirection = "ASC";
                    }
                }
                else
                {
                    lie = Misc.DoSort(lie, e.SortExpression, SortDirection.Descending);
                    SearchSortDirection = "ASC";
                }
            }

            SortColumn = e.SortExpression;
            SearchData = lie;
            BindSearch();
        }

        private void BindSearch()
        {
            if (SearchData != null)
            {
                gvSearch.DataSource = SearchData;
                gvSearch.DataBind();
                gvSearch.SelectedIndex = -1;
            }
            divOrderInformation.Visible = false;
            divLab.Visible = false;
        }

        protected void gvSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSearch.DataSource = SearchData;
            gvSearch.PageIndex = e.NewPageIndex;
            gvSearch.DataBind();
        }

        protected void gvSearch_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (SortColumn != null && SearchSortDirection != null)
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    foreach (TableCell tableCell in e.Row.Cells)
                    {
                        if (tableCell.HasControls())
                        {
                            LinkButton sortLinkButton = null;
                            if (tableCell.Controls[0] is LinkButton)
                            {
                                sortLinkButton = (LinkButton)tableCell.Controls[0];
                            }
                            if (sortLinkButton != null && SortColumn == sortLinkButton.CommandArgument)
                            {
                                Image img = new Image();
                                if (SearchSortDirection == "ASC")
                                {
                                    img.ImageUrl = "~/Styles/images/ArrowUp.gif";
                                }
                                else
                                {
                                    img.ImageUrl = "~/Styles/images/ArrowDown.gif";
                                }
                                tableCell.Controls.Add(new LiteralControl("&nbsp;"));
                                tableCell.Controls.Add(img);
                            }
                        }
                    }
                }
            }
        }

        protected void gvSearch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "LabOrderLookup_gvSearch_RowCommand", mySession.MyUserID))
#endif
                {
                    if (e.CommandName.ToLower().Equals("select"))
                    {
                        var dto = new PatientOrderDTO();
                        dto.IndividualId = Convert.ToInt32(e.CommandArgument);
                        var ind = this.SearchData.FirstOrDefault(x => x.ID == Convert.ToInt32(e.CommandArgument));
                        dto.PatientSiteCode = ind.SiteCodeID;
                        Session.Add("PatientOrderDTO", dto);
                        Session.Add("IsDtoRedirect", true);
                        PatientId = dto.IndividualId;
                        ClearAll();
                        divSearchResults.Visible = false;
                        divOrderInformation.Visible = false;

                        string message = GetLabOrderHistory();
                        if (!string.IsNullOrEmpty(message))
                        {
                            litPageMessage.Visible = true;
                            litPageMessage.Text = message;
                        }
                        else
                        {
                            divLab.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void gvSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var id = DataBinder.Eval(e.Row.DataItem, "ID").ToString();
            mySession.SelectedPatientID = Convert.ToInt32(id);

            e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#E2F2FE'");
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");

            for (var i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Attributes.Add("style", "cursor:pointer;");
                e.Row.Cells[i].Attributes["onClick"] = Page.ClientScript.GetPostBackClientHyperlink(this.gvSearch, String.Format("Select${0}", DataBinder.Eval(e.Row.DataItem, "ID")), true);
            }
        }


        protected void gvLabOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var orderNumber = DataBinder.Eval(e.Row.DataItem, "OrderNumber").ToString();
            e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#E2F2FE'");
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
        }


        protected void gvPrescription_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            Prescription prescription = (Prescription)e.Row.DataItem;

            int providerId = Convert.ToInt32(prescription.ProviderId);
            TextBox txtProvider = (TextBox)e.Row.FindControl("txtProvider");
            txtProvider.Text = GetProviderName(providerId);

            SetSpereValues(prescription, e);

            SetCylinderValues(prescription, e);

            SetCalaculatedSpereValues(prescription, e);

            SetCalaculatedCylinderValues(prescription, e);

            SetCalaculatedAxisValues(prescription, e);
        }


        protected void SetSpereValues(Prescription prescription, GridViewRowEventArgs e)
        {
            TextBox txtSphRight = (TextBox)e.Row.FindControl("txtSphRight");
            TextBox txtSphLeft = (TextBox)e.Row.FindControl("txtSphLeft");

            if (prescription.OdSphere >= 1)
            {
                txtSphRight.Text = "+" + prescription.OdSphere;
            }
            else
            {
                txtSphRight.Text = prescription.OdSphere.ToString();
            }


            if (prescription.OsSphere >= 1)
            {
                txtSphLeft.Text = "+" + prescription.OsSphere;
            }
            else
            {
                txtSphLeft.Text = prescription.OsSphere.ToString();
            }
        }
        protected void SetCylinderValues(Prescription prescription, GridViewRowEventArgs e)
        {
            TextBox txtCylRight = (TextBox)e.Row.FindControl("txtCylRight");
            TextBox txtCylLeft = (TextBox)e.Row.FindControl("txtCylLeft");

            if (prescription.OdCylinder >= 1)
            {
                txtCylRight.Text = "+" + prescription.OdCylinder;
            }
            else
            {
                txtCylRight.Text = prescription.OdCylinder.ToString();
            }


            if (prescription.OsCylinder >= 1)
            {
                txtCylLeft.Text = "+" + prescription.OsCylinder;
            }
            else
            {
                txtCylLeft.Text = prescription.OsCylinder.ToString();
            }

        }
        protected void SetCalaculatedSpereValues(Prescription prescription, GridViewRowEventArgs e)
        {
            TextBox txtCalculatedSphRight = (TextBox)e.Row.FindControl("txtCalculatedSphRight");
            TextBox txtCalculatedSphLeft = (TextBox)e.Row.FindControl("txtCalculatedSphLeft");
            double odsphere = 0;
            if (prescription.OdSphereCalc == "0.00")
            {
                txtCalculatedSphRight.Text = "Plano";
            }
            else if (double.TryParse(prescription.OdSphereCalc, out odsphere))
            {
                if (odsphere >= 1)
                {
                    txtCalculatedSphRight.Text = "+" + prescription.OdSphereCalc;
                }
                else
                {
                    txtCalculatedSphRight.Text = prescription.OdSphereCalc.ToString();
                }
            }

            double ossphere = 0;
            if (prescription.OsSphereCalc == "0.00")
            {
                txtCalculatedSphLeft.Text = "Plano";
            }
            else if (double.TryParse(prescription.OsSphereCalc, out ossphere))
            {
                if (ossphere >= 1)
                {
                    txtCalculatedSphLeft.Text = "+" + prescription.OsSphereCalc;
                }
                else
                {
                    txtCalculatedSphLeft.Text = prescription.OsSphereCalc.ToString();
                }
            }
        }
        protected void SetCalaculatedCylinderValues(Prescription prescription, GridViewRowEventArgs e)
        {
            TextBox txtCalculatedCylRight = (TextBox)e.Row.FindControl("txtCalculatedCylRight");
            TextBox txtCalculatedCylLeft = (TextBox)e.Row.FindControl("txtCalculatedCylLeft");
            double odcylinder = 0;
            if (prescription.OdCylinderCalc == "0.00")
            {
                txtCalculatedCylRight.Text = "Sphere";
            }
            else if (double.TryParse(prescription.OdCylinderCalc, out odcylinder))
            {
                if (odcylinder >= 1)
                {
                    txtCalculatedCylRight.Text = "+" + prescription.OdCylinderCalc;
                }
                else
                {
                    txtCalculatedCylRight.Text = prescription.OdCylinderCalc.ToString();
                }
            }

            double oscylinder = 0;
            if (prescription.OsCylinderCalc == "0.00")
            {
                txtCalculatedCylLeft.Text = "Sphere";
            }
            else if (double.TryParse(prescription.OsCylinderCalc, out oscylinder))
            {
                if (oscylinder >= 1)
                {
                    txtCalculatedCylLeft.Text = "+" + prescription.OsCylinderCalc;
                }
                else
                {
                    txtCalculatedCylLeft.Text = prescription.OsCylinderCalc.ToString();
                }
            }
        }
        protected void SetCalaculatedAxisValues(Prescription prescription, GridViewRowEventArgs e)
        {
            TextBox txtCalculatedAxisRight = (TextBox)e.Row.FindControl("txtCalculatedAxisRight");
            TextBox txtCalculatedAxisLeft = (TextBox)e.Row.FindControl("txtCalculatedAxisLeft");
            if (prescription.OdAxisCalc == "000")
            {
                txtCalculatedAxisRight.Text = "N/A";
            }
            if (prescription.OsAxisCalc == "000")
            {
                txtCalculatedAxisLeft.Text = "N/A";
            }
        }

        protected void gvLabOrders_Click(object sender, EventArgs e)
        {
            ImageButton button = sender as ImageButton;
            string command = button.CommandName.ToString();
            string comArgument = button.CommandArgument.ToString();
            GridViewRow gvr = (GridViewRow)button.NamingContainer;
            int RowIndex = gvr.RowIndex;
            string OrderNumber = comArgument;
            mySession.SelectedOrder.OrderNumber = OrderNumber;
            switch (command)
            {
                case "Select":
                    divLab.Visible = false;
                    divSearchResults.Visible = false;
                    tbID.Text = OrderNumber;
                    bQuickSearch_Click(sender, e);                   
                    break;
            }
        }

        protected string GetProviderName(int providerID)
        {
            string providerName = string.Empty;
            if (ProviderList != null)
            {
                providerName = (from p in ProviderList
                                where p.PersonalType == "PROVIDER"
                                select p.NameFMiL.ToString()).FirstOrDefault();
            }
            return providerName;
        }

        #region Search Accessors

        public string IndividualTypeSelected
        {
            get;
            set;
        }

        public string NewPage
        {
            set
            {
                Response.Redirect(value);
            }
        }

        public bool ActiveOnly
        {
            get; 
            set;
        }

        public List<IndividualEntity> SearchData
        {
            get { return (List<IndividualEntity>)Session["SearchData"]; }
            set { Session.Add("SearchData", value); }
        }

        public string SearchSortDirection
        {
            get { return (string)ViewState["SearchSortDirection"]; }
            set { ViewState.Add("SearchSortDirection", value); }
        }

        public string SortColumn
        {
            get { return (string)ViewState["SortColumn"]; }
            set { ViewState.Add("SortColumn", value); }
        }

        public string LastName
        {
            get { return tbLastName.Text.Trim(); }
            set { tbLastName.Text = value; }
        }

        public string FirstName
        {
            get { return tbFirstName.Text.Trim(); }
            set { tbFirstName.Text = value; }
        }

        public string SearchID
        {
            get { return tbID.Text.Trim(); }
            set { tbID.Text = value; }
        }

        public List<SiteCodeEntity> SiteCodes
        {
            get;
            set;
        }

        public string SelectedSiteValue
        {
            get { return this.mySession.MySite.SiteCode; }
            set { var a = value; }
        }




        #endregion Search Accessors

        private Order _Order;

        public Order Order
        {
            get
            {
                this._Order = Session["Order"] as Order;
                return _Order;
            }
            set
            {
                Session.Add("Order", value);
            }
        }
        private Prescription _Prescription;

        public Prescription Prescription
        {
            get
            {
                this._Prescription = Session["Prescription"] as Prescription; 
                return this._Prescription;
            }
            set
            {
                Session.Add("Prescription", value);
            }
        }

        public List<Order> OrderList
        {
            get
            {
                return ViewState["OrderList"] as List<Order>;
            }
            set
            {
                ViewState["OrderList"] = value;
            }
        }
        public List<Prescription> PrescriptionList
        {
            get
            {
                return ViewState["PrescriptionList"] as List<Prescription>;
            }
            set
            {
                ViewState["PrescriptionList"] = value;
            }
        }

        private PrescriptionDocument _PrescriptionDocument;
        public PrescriptionDocument PrescriptionDocument
        {
            get
            {
                this._PrescriptionDocument = Session["PrescriptionDocument"] as PrescriptionDocument;
                return this._PrescriptionDocument;
            }
            set
            {
                Session.Add("PrescriptionDocument", value);
            }
        }

        public List<IndividualEntity> ProviderList
        {
            get
            {
                return ViewState["ProviderList"] as List<IndividualEntity>;
            }
            set
            {
                ViewState["ProviderList"] = value;
            }
        }

        public List<IndividualEntity> PatientList
        {
            get
            {
                return ViewState["PatientList"] as List<IndividualEntity>;
            }
            set
            {
                ViewState["PatientList"] = value;
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

        public OrderDropDownData OrderDdlData
        {
            get
            {
                return ViewState["OrderDdlData"] as OrderDropDownData;
            }
            set
            {
                ViewState["OrderDdlData"] = value;
            }
        }

        public List<LookupData> LookupDataList
        {
            get
            {
                return ViewState["LookupDataList"] as List<LookupData>;
            }
            set
            {
                ViewState["LookupDataList"] = value;
            }
        }

        public String SelectedPriority
        {
            get;
            set;
        }

        public List<OrderStateEntity> OrderStateHistory
        {
            get { return ViewState["OrderStateHistory"] as List<OrderStateEntity>; }
            set
            {
                ViewState["OrderStateHistory"] = value;
            }
        }

        public String ClinicJustification
        {
            get;
            set;
        }

        public String LabJustification
        {
            get;
            set;
        }

        public Int32 PatientId
        {
            get { return Convert.ToInt32(Session["PatientId"]); }
            set { Session["PatientId"] = value; }
        }

        public String Demographic
        {
            get { return Session["Demographic"] == null ? String.Empty : Session["Demographic"].ToString(); }
            set { Session["Demographic"] = value; }
        }

        public String SiteCode
        {
            get { return Session["SiteCode"].ToString(); }
            set { Session["SiteCode"] = value; }
        }

        public string LabAction
        {
            get;
            set;
        }

        public string RejectRedirectJustification
        {
            get;
            set;
        }

        public string RedirectLab
        {
            get;
            set;
        }

        public List<SiteCodeEntity> RedirectLabList
        {
            get
            {
                return Session["RedirectLabList"] as List<SiteCodeEntity>;
            }
            set
            {
                Session["RedirectLabList"] = value;
            }
        }

        public Dictionary<string, string> SpecialLabTintList
        {
            get
            {
                return ViewState["SpecialLabTintList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["SpecialLabTintList"] = value;
            }
        }

        public FrameItemDefaultEntity DefaultFrameItems
        {
            get;
            set;
        }

        public AddressEntity PatientAddress
        {
            get
            {
                //var a = Session["orderAddress"] as AddressEntity;
                var a = ViewState["PatientAddress"] as AddressEntity;
                if (a.IsNull()) a = new AddressEntity();
                return a;
            }
            set
            {
                if (value.IsNull()) return;

                ViewState["PatientAddress"] = value;
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
            }
        }

        public SitePrefFrameItemEntity FramePreferences
        {
            get
            {
                return this.FramePreferencesList.FirstOrDefault(x => x.Frame == this.Order.Frame) ?? new SitePrefFrameItemEntity();
            }
        }

        public List<SitePrefFrameItemEntity> FramePreferencesList
        {
            get
            {
                return ViewState["FramePreferencesList"] as List<SitePrefFrameItemEntity> ?? new List<SitePrefFrameItemEntity>();
            }
            set
            {
                ViewState["FramePreferencesList"] = value;
            }
        }

        public SitePrefRxEntity SitePrefsRX
        {
            get
            {
                return ViewState["SitePrefsRX"] as SitePrefRxEntity;
            }
            set
            {
                ViewState["SitePrefsRX"] = value;
            }
        }

        public SitePrefOrderEntity OrderPreferences
        {
            get
            {
                return ViewState["OrderPreferences"] as SitePrefOrderEntity ?? new SitePrefOrderEntity();
            }
            set
            {
                ViewState["OrderPreferences"] = value;
            }
        }

        public DateTime HoldForStockDate
        {
            get;
            set;
        }

        public List<SitePrefLabJustification> LabJustificationPreferences
        {
            get { return ViewState["LabJustificationPreferences"] as List<SitePrefLabJustification>; }
            set
            {
                ViewState["LabJustificationPreferences"] = value;
                var rd = value.FirstOrDefault(x => x.JustificationReason == "redirect");
                var rj = value.FirstOrDefault(x => x.JustificationReason == "reject");
                rd = null;
                rj = null;
            }
        }

        public bool OrderIsPrefill
        {
            get { return ViewState["OrderIsPrefill"].ToBoolean(); }
            set { ViewState["OrderIsPrefill"] = value; }
        }
        public Order OriginalOrder
        {
            get { return ViewState["OriginalOrder"] as Order; }
            set { ViewState["OriginalOrder"] = value; }
        }
        public NewRxVals CalculatedRxValues
        {
            get
            {
                return Session["CalculatedRxValues"] as NewRxVals;
            }
            set
            {
                Session["CalculatedRxValues"] = value;
            }
        }
        private Exam _Exam;

        public Exam Exam
        {
            get
            {
                this._Exam = Session["Exam"] as Exam;
                return this._Exam;
            }
            set
            {
                Session.Add("Exam", value);
            }
        }

        public List<Exam> ExamList
        {
            get
            {
                return Session["ExamList"] as List<Exam>;
            }
            set
            {
                Session["ExamList"] = value;
            }
        }

        //public string GroupName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected void gvPatientInfo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                OrderLabelAddresses drv = (OrderLabelAddresses)e.Row.DataItem;
                TextBox Patient = (TextBox)e.Row.FindControl("txtPatientName");
                TextBox CityStateZip = (TextBox)e.Row.FindControl("txtCityStateZip");
                TextBox ShipCityStateZip = (TextBox)e.Row.FindControl("txtShipCityStateZip");
                TextBox Country = (TextBox)e.Row.FindControl("txtCountry");
                TextBox ShipCountry = (TextBox)e.Row.FindControl("txtShipCountry");

                string mInitial = string.IsNullOrEmpty(drv.MiddleName) ? " " : " " + drv.MiddleName.FirstOrDefault().ToString() + ". ";
                Patient.Text = drv.FirstName + mInitial + drv.LastName;
                litPatientName.Text = Server.HtmlEncode(drv.FirstName + mInitial + drv.LastName);

                if (drv.State == "NA")
                {
                    CityStateZip.Visible = false;
                    Country.Visible = true;
                }
                else
                {
                    CityStateZip.Visible = true;
                    Country.Visible = false;
                }
                if (drv.ShipState == "NA")
                {
                    ShipCityStateZip.Visible = false;
                    ShipCountry.Visible = true;
                }
                else
                {
                    ShipCityStateZip.Visible = true;
                    ShipCountry.Visible = false;
                }

            }
        }


        protected void gvOrderInfo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Order order = (Order)e.Row.DataItem;
                TextBox txtDispenseMethod = (TextBox)e.Row.FindControl("txtDispenseMethod");
                switch (order.OrderDisbursement.ToString().ToLower())
                {
                    case "c2p":
                        txtDispenseMethod.Text = "Clinic Ship to Patient";
                        break;
                    case "l2p":
                        txtDispenseMethod.Text = "Lab Ship to Patient";
                        break;
                    case "cd":
                        txtDispenseMethod.Text = "Clinic Distribution";
                        break;
                }

            }
        }

        protected void gvOrderStatusHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}