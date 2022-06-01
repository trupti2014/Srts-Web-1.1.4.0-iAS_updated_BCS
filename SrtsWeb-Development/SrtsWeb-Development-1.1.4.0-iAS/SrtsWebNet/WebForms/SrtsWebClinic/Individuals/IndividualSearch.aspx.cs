using SrtsWeb;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Individuals;
using SrtsWeb.Views.Individuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWebClinic.Individuals
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class IndividualSearch : PageBase, IIndividualSearchView, ISiteMapResolver
    {
        private IndividualSearchPresenter _presenter;

        public IndividualSearch()
        {
            _presenter = new IndividualSearchPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "IndividualSearch_Page_Load", ModifiedBy))
#endif
                {
                    CurrentModule("Administration - Individual Search");
                    CurrentModule_Sub(string.Empty);
                    BuildPageTitle();

                    if (!IsPostBack)
                    {
                        litPageMessage.Visible = true;
                        SearchSortDirection = "ASC";
                        if (mySession == null)
                        {
                            mySession = new SRTSSession();
                        }
                        mySession.AddOrEdit = string.Empty;
                        mySession.Patient = new PatientEntity();
                        mySession.ReturnURL = string.Empty;
                        mySession.SelectedExam = new ExamEntity();
                        mySession.SelectedOrder = new OrderEntity();
                        mySession.SelectedPatientID = 0;
                        mySession.TempID = 0;
                        mySession.tempOrderID = string.Empty;

                        _presenter.InitView();
                        SelectedSiteValue = string.IsNullOrEmpty(mySession.MyClinicCode) ? Globals.SiteCode : mySession.MyClinicCode;

                        if (Roles.GetRolesForUser().Contains("MgmtEnterprise"))
                            this.ddlIndividualType.SelectedValue = "ALL";
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Administration - Individual Search");
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Individuals/IndividualSearch.aspx", "Search Individual");
            child.ParentNode = parent;
            return child;
        }

        public void rbSearch_Click(object sender, CommandEventArgs e)
        {
            try
            {
                var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "IndividualSearch_rbSearch_Click", ModifiedBy))
#endif
                {
                    CustomValidator err;

                    if (String.IsNullOrEmpty(this.LastName) && String.IsNullOrEmpty(this.SearchID))
                    {
                        err = new CustomValidator();
                        err.IsValid = false;
                        err.ErrorMessage = "A last name or ID number are required.  First name is an optional entry.";
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

                    gvSearch.DataSource = null;
                    gvSearch.DataBind();

                    List<IndividualEntity> lie;
                    if (((String)e.CommandArgument) == "S")
                    {
                        lie = _presenter.SearchIndividual("S");
                    }
                    else
                    {
                        lie = _presenter.SearchIndividual("G");
                    }

                    SearchData = lie;
                    gvSearch.DataSource = SearchData;
                    gvSearch.DataBind();
                    gvSearch.SelectedIndex = -1;
                    litPageMessage.Visible = false;
                    divSearchResults.Visible = true;

                    if (!SearchData.IsNullOrEmpty()) return;

                    if (this.SearchID.IsNullOrEmpty())
                        this.gvSearch.EmptyDataText = "Name search returned no results.";
                    else
                        this.gvSearch.EmptyDataText = "Please retry using SSN if DODID was used, or DODID if SSN was used.";

                    this.gvSearch.DataBind();
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void gvSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                mySession.SelectedPatientID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "ID"));
                Globals.PatientID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "ID"));
                e.Row.Attributes["onClick"] = String.Format("location.href='../../SrtsPerson/PersonDetails.aspx?id={0}&isP=false'", DataBinder.Eval(e.Row.DataItem, "ID"));
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#E2F2FE'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
                e.Row.Attributes.Add("style", "cursor:pointer;");
            }
        }

        protected void gvSearch_Sorting(object sender, GridViewSortEventArgs e)
        {
            List<IndividualEntity> lie = SearchData;
            if (e.SortExpression == "LastName")
            {
                if (SearchSortDirection == "DESC")
                {
                    lie.Sort(delegate(IndividualEntity i1, IndividualEntity i2)
                    {
                        return i2.LastName.CompareTo(i1.LastName);
                    });
                    SearchSortDirection = "ASC";
                }
                else
                {
                    lie.Sort(delegate(IndividualEntity i1, IndividualEntity i2)
                    {
                        return i1.LastName.CompareTo(i2.LastName);
                    });
                    SearchSortDirection = "DESC";
                }
            }
            else
            {
                if (SearchSortDirection == "DESC")
                {
                    lie.Sort(delegate(IndividualEntity i1, IndividualEntity i2)
                    {
                        return i2.IDNumber.CompareTo(i1.IDNumber);
                    });
                    SearchSortDirection = "ASC";
                }
                else
                {
                    lie.Sort(delegate(IndividualEntity i1, IndividualEntity i2)
                    {
                        return i1.IDNumber.CompareTo(i2.IDNumber);
                    });
                    SearchSortDirection = "DESC";
                }
            }
            SearchData = lie;
            gvSearch.DataSource = SearchData;
            gvSearch.DataBind();
        }

        protected void gvSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSearch.DataSource = SearchData;
            gvSearch.DataBind();
            gvSearch.PageIndex = e.NewPageIndex;
            gvSearch.DataBind();
        }

        #region Accessors

        public List<IndividualEntity> SearchData
        {
            get { return (List<IndividualEntity>)ViewState["SearchData"]; }
            set { ViewState.Add("SearchData", value); }
        }

        public string SearchSortDirection
        {
            get { return (string)ViewState["SearchSortDirection"]; }
            set { ViewState.Add("SearchSortDirection", value); }
        }

        public string LastName
        {
            get { return tbLastname.Text; }
            set { tbLastname.Text = value; }
        }

        public string FirstName
        {
            get { return this.tbFirstName.Text; }
            set { this.tbFirstName.Text = value; }
        }

        public string SearchID
        {
            get { return tbID.Text; }
            set { tbID.Text = value; }
        }

        private List<SiteCodeEntity> _siteCodes;

        public List<SiteCodeEntity> SiteCodes
        {
            get { return _siteCodes; }
            set
            {
                _siteCodes = value;
                ddlSiteCode.DataSource = _siteCodes;
                ddlSiteCode.Items.Clear();
                ddlSiteCode.DataTextField = "SiteCombinationProfile";
                ddlSiteCode.DataValueField = "SiteCode";
                ddlSiteCode.DataBind();
            }
        }

        public string SelectedSiteValue
        {
            get { return ddlSiteCode.SelectedValue; }
            set { ddlSiteCode.SelectedValue = value; }
        }

        public List<LookupTableEntity> IndividualTypeDDL
        {
            set
            {
                ddlIndividualType.Items.Clear();
                ddlIndividualType.DataSource = value;
                ddlIndividualType.DataTextField = "Text";
                ddlIndividualType.DataValueField = "Value";
                ddlIndividualType.DataBind();
                ddlIndividualType.Items.Insert(0, new ListItem("ALL", "ALL"));
            }
        }

        public string IndividualTypeSelected
        {
            get { return ddlIndividualType.SelectedValue; }
            set { ddlIndividualType.SelectedValue = value; }
        }

        #endregion Accessors
    }
}