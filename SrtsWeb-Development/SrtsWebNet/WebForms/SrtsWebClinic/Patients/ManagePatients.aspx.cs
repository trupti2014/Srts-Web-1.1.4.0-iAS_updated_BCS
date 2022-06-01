using Microsoft.Web.Administration;
using SrtsWeb;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Patients;
using SrtsWeb.Views.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWebClinic.Patients
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    public partial class ManagePatients : PageBase, IManagePatientsView, ISiteMapResolver
    {
        private ManagePatientsPresenter _presenter;

        public ManagePatients()
        {
            _presenter = new ManagePatientsPresenter(this);
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
                using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "ManagePatients_Page_Load", mySession.MyUserID))
#endif
                {
                    if (!IsPostBack)
                    {
                        if (mySession == null)
                            mySession = new SRTSSession();

                        SelectedSiteValue = mySession.MyClinicCode;
                        litPageMessage.Visible = true;
                        SearchSortDirection = "ASC";

                        if (Request.Url.Segments.Any(x => x.ToLower() == "search"))
                        {
                            if (Roles.IsUserInRole("LabClerk") || Roles.IsUserInRole("LabTech"))
                            {
                                btnSearchLocal.Visible = false;
                                this.pnlSearch.DefaultButton = this.btnSearchGlobal.UniqueID;
                            }
                        }
                    }
                    BindSearch();
                    if (SearchData != null)
                    {
                        divSearchResults.Visible = true;
                        hdrSearchResults.Visible = true;
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
                Master.CurrentModuleTitle = "Manage Patients - Search Patients";
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Manage Patients");
                CurrentModule_Sub(string.Empty);
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child;

            child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Patients/ManagePatients.aspx/search", "Manage Patients Search"); //break;

            return child;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/SrtsWebClinic/Patients/ManagePatients.aspx");
        }

        public void rbSearch_Click(object sender, CommandEventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "ManagePatients_rbSearch_Click", mySession.MyUserID))
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
                        litPageMessage.Text = string.Format("{0}", "No search results returned. Try a new search or Add New Patient!");
                        divSearchResults.Visible = true;
                        hdrSearchResults.Visible = false;
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
                    hdrSearchResults.Visible = true;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
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
                using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "ManagePatients_gvSearch_RowCommand", mySession.MyUserID))
#endif
                {
                    if (e.CommandName.ToLower().Equals("select"))
                        Response.Redirect(String.Format("~/WebForms/SrtsPerson/PersonDetails.aspx?id={0}&isP=true", e.CommandArgument), false);
                    else if (e.CommandName.ToLower().Equals("orders"))
                    {
                        var ie = this.SearchData[Convert.ToInt32(e.CommandArgument) + (this.gvSearch.PageIndex * this.gvSearch.PageSize)];
                        var dto = new PatientOrderDTO();
                        dto.IndividualId = Convert.ToInt32(ie.ID);
                        var ind = this.SearchData.FirstOrDefault(x => x.ID == Convert.ToInt32(ie.ID));
                        dto.Demographic = ind.Demographic;
                        dto.PatientSiteCode = ind.SiteCodeID;
                        Session.Add("PatientOrderDTO", dto);
                        Session.Add("IsDtoRedirect", true);
                        Response.Redirect("~/WebForms/SrtsOrderManagement/OrderManagement.aspx", false);
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
            get { return Convert.ToBoolean(rblActiveOnly.SelectedValue); }
            set { rblActiveOnly.SelectedValue = value.ToString(); }
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
    }
}