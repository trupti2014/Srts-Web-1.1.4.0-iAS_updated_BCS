using SrtsWeb;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Presenters.Individuals;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Individuals;
using SrtsWeb.CustomErrors;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Web;
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
    public partial class IndividualManagementAdd : PageBase, IIndividualManagementAddView, ISiteMapResolver
    {
        private IndividualManagementAddPresenter _presenter;
        private IDemographicXMLHelper dxHelper;

        public IndividualManagementAdd()
        {
            _presenter = new IndividualManagementAddPresenter(this);
            dxHelper = new DemographicXMLHelper();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            CurrentModule("Administration");
            CurrentModule_Sub(" - Add Individual");
            BuildPageTitle();

            if (!Page.IsPostBack)
            {
                _presenter.InitView();
                CalendarExtender1.EndDate = DateTime.Today;
                BoSType = dxHelper.GetALLBOS();
                ddlIDNumberType.Focus();
            }
            if (Page.IsPostBack)
            {
                WebControl wcICausedPostBack = (WebControl)GetControlThatCausedPostBack(sender as Page);
                int indx = wcICausedPostBack.TabIndex;
                var ctrl = from control in wcICausedPostBack.Parent.Controls.OfType<WebControl>()
                           where control.TabIndex > indx
                           select control;
                ctrl.DefaultIfEmpty(wcICausedPostBack).First().Focus();
            }
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
                CurrentModule("Administration");
                CurrentModule_Sub(" - Add Individual");
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/WebForms/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/WebForms/SrtsWebClinic/Individuals/IndividualManagementAdd.aspx", "Add Individual");
            child.ParentNode = parent;
            return child;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                _presenter.AddIndividualRecord();
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ShowMessage();
                }
                else
                {
                    if (mySession.SelectedNewUserName != null)
                    {
                        Response.Redirect("../WebForms/Admin/SrtsUserManager.aspx");
                    }
                    else
                    {
                        Response.Redirect("../WebForms/Individuals/IndividualDetails.aspx");
                    }
                }
            }
            else
            {
                vsErrors.Visible = true;
            }
        }

        private void ShowMessage()
        {
            CustomValidator cv = new CustomValidator();
            cv.IsValid = false;
            cv.ErrorMessage = ErrorMessage;
            this.Page.Validators.Add(cv);
            return;
        }

        private void ShowCV_IDNumberMessage()
        {
            cvIDNumber.IsValid = false;
            cvIDNumber.ErrorMessage = ErrorMessage;
            return;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (mySession.SelectedNewUserName != null)
            {
                Response.Redirect("../WebForms/Admin/SrtsUserManager.aspx");
            }
            else
            {
                Response.Redirect("~/WebForms/default.aspx");
            }
        }

        protected void ddlStatusType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RankType = dxHelper.GetRanksByBOSAndStatus(BOSTypeSelected, StatusTypeSelected);
        }

        protected void ddlBOS_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatusType = dxHelper.GetStatusByBOS(BOSTypeSelected);
        }

        protected void ddlRank_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void gvSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                mySession.SelectedPatientID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "ID"));
                e.Row.Attributes["onClick"] = "location.href='../Individuals/IndividualDetails.aspx?id=" + DataBinder.Eval(e.Row.DataItem, "ID") + "'";
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#E2F2FE'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
                e.Row.Attributes.Add("style", "cursor:pointer;");
            }
        }

        protected void tbIDNumber_TextChanged(object sender, EventArgs e)
        {
            if (this.ddlIDNumberType.SelectedIndex.Equals(0))
            {
                this.taboff.Visible = false;
                this.gvSearch.Visible = false;
                rfvIDNumberType.IsValid = false;
                //this.ddlIndividualType.Focus();
            }
            else
            {
                this.taboff.Visible = true;

                if (!ValidateIDNumber())
                {
                    this.taboff.Visible = true;
                    this.tbIDNumber.Focus();
                    ShowCV_IDNumberMessage();
                    pnlCompleteForm.Visible = false;
                    this.gvSearch.Visible = false;
                }
                else
                {
                    if (_presenter.SearchIndividual())
                    {
                        this.tbIDNumber.Focus();
                        this.taboff.Visible = false;
                        _presenter.SearchIndividual();
                        this.gvSearch.Visible = true;
                        pnlCompleteForm.Visible = false;
                    }
                    // If it is not a duplicate then check in DMDC.  Only for SSN and DODID searches
                    else if (this.IDNumberTypeSelected.Equals("DIN") || this.IDNumberTypeSelected.Equals("SSN"))
                    {
                        _presenter.SearchIndividualDmdc();

                        this.taboff.Visible = false;
                        this.gvSearch.Visible = false;
                        this.pnlCompleteForm.Visible = true;
                    }
                    //If is not a Duplicate
                    else
                    {
                        this.taboff.Visible = false;
                        this.gvSearch.Visible = false;
                        this.taboff.Visible = false;
                        pnlCompleteForm.Visible = true;
                    }
                }
            }
        }

        //protected void ddlIDNumberType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (this.ddlIDNumberType.SelectedIndex.Equals(0))
        //    {
        //        this.taboff.Visible = false;
        //        rfvIDNumberType.IsValid = false;
        //        //this.ddlIndividualType.Focus();
        //    }
        //    else
        //    {
        //        this.taboff.Visible = true;

        //        if (!String.IsNullOrEmpty(tbIDNumber.Text))
        //        {
        //            if (ValidateIDNumber())
        //            {
        //                cvIDNumber.IsValid = true;
        //                if (_presenter.SearchIndividual())
        //                {
        //                    this.tbIDNumber.Focus();
        //                    _presenter.SearchIndividual();
        //                    this.gvSearch.Visible = true;
        //                    pnlCompleteForm.Visible = false;
        //                }
        //                 //If it is not a duplicate then check in DMDC.  Only for SSN and DODID searches
        //                else if (this.IDNumberTypeSelected.Equals("DIN") || this.IDNumberTypeSelected.Equals("SSN"))
        //                {
        //                    _presenter.SearchIndividualDmdc();

        //                    this.taboff.Visible = false;
        //                    this.gvSearch.Visible = false;
        //                    this.pnlCompleteForm.Visible = true;
        //                }
        //               // If not duplicate
        //                else
        //                {
        //                    this.gvSearch.Visible = false;
        //                    pnlCompleteForm.Visible = true;
        //                    taboff.Visible = false;
        //                    this.txtLastName.Focus();
        //                }
        //            }
        //            else
        //            {
        //                this.taboff.Visible = true;
        //                cvIDNumber.IsValid = false;
        //                this.gvSearch.Visible = false;
        //                pnlCompleteForm.Visible = false;
        //            }
        //        }
        //        else
        //        {
        //            this.taboff.Visible = true;
        //            tbIDNumber.Focus();
        //        }
        //    }
        //}

        #region Input Validators

        protected bool ValidateIDNumber()
        {
            string IDType = IDNumberTypeSelected;
            var input = tbIDNumber.Text;
            int len = 0;
            string type = "";

            switch (IDType)
            {
                case "DIN":
                    len = 10;
                    type = "DoD ID";
                    break;

                case "SSN":
                    len = 9;
                    type = "Social Security";
                    break;

                case "PIN":
                    len = 11;
                    type = "Provider ID";
                    break;

                case "DBN":
                    len = 11;
                    type = "DoD Benifits";
                    break;

                case "X":
                    ErrorMessage = "Please select ID Type";
                    return false;

                default:

                    break;
            }

            if (input.ValidateIDNumLength(len))
            {
                if (input.ValidateIDNumFormat())
                {
                    return true;
                }
                else
                {
                    ErrorMessage = "Invalid characters in ID Number";
                    return false;
                }
            }
            else
            {
                ErrorMessage = string.Format("{0} Number must be {1} digits", type, len);
                return false;
            }
        }

        protected void ValidateCommentFormat(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Length > 0)
            {
                int limit = 256;
                if (args.IsValid = args.Value.ValidateCommentLength(limit))
                {
                    if (args.IsValid = args.Value.ValidateCommentFormat())
                    {
                    }
                    else
                    {
                        cvComment.ErrorMessage = "Invalid characters in Comments";
                    }
                }
                else
                {
                    cvComment.ErrorMessage = string.Format("Limit is {0} characters in Comments", limit.ToString());
                }
            }
        }

        protected void ValidateIndTypeCBs(object source, ServerValidateEventArgs args)
        {
            args.IsValid = cbAdministrator.Checked == true || cbProvider.Checked == true || cbTechnician.Checked == true;           
        }

        protected void ValidateDOB(object source, ServerValidateEventArgs args)
        {
            if (String.IsNullOrEmpty(args.Value)) return;

            if (args.IsValid = args.Value.ValidateDOBFormat())
            {
                if (args.IsValid = args.Value.ValidateDOBIsValid())
                {
                    if (args.IsValid = args.Value.ValidateDOBNotFuture())
                    {
                    }
                    else
                    {
                        cvDOB.ErrorMessage = "Invalid Date of Birth (can't be greater than today)";
                    }
                }
                else
                {
                    cvDOB.ErrorMessage = "Date of Birth entry is invalid";
                }
            }
            else
            {
                cvDOB.ErrorMessage = "Invalid characters or format in Date of Birth";
            }
        }

        #endregion Input Validators

        #region Add Individual Accessors

        public List<IndividualTypeEntity> IndividualTypesBind
        {
            //set
            //{
            //    gvIndType.DataSource = value;
            //    gvIndType.DataBind();
            //}
            set
            {
                // Added this code to use the checkbox method of managing types 
                //var a = value.Select(x => x.TypeDescription).ToList();

                if (value != null)
                {
                    cbAdministrator.Checked = value.Any(x => x.TypeDescription.ToLower() == "other" && x.IsActive == true);
                    //cbPatient.Checked = value.Any(x => x.TypeDescription.ToLower() == "patient" && x.IsActive == true);
                    cbProvider.Checked = value.Any(x => x.TypeDescription.ToLower() == "provider" && x.IsActive == true);
                    cbTechnician.Checked = value.Any(x => x.TypeDescription.ToLower() == "technician" && x.IsActive == true);
                }
            }
        }

        public bool IsAdmin
        {
            get { return cbAdministrator.Checked.Equals(true); }
        }

        public bool IsTechnician
        {
            get { return cbTechnician.Checked.Equals(true); }
        }

        //public bool IsPatient
        //{
        //    get { return cbPatient.Checked.Equals(true); }
        //}

        public bool IsProvider
        {
            get { return cbProvider.Checked.Equals(true); }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                if (!String.IsNullOrEmpty(_errorMessage))
                {
                    rfvIDNumber.ErrorMessage = _errorMessage;
                    rfvIDNumber.IsValid = false;
                    vsErrors.ShowSummary = true;
                }
            }
        }

        public string NewPage
        {
            set
            {
                Response.Redirect(value);
            }
        }

        public string FirstName
        {
            get { return tbFirstName.Text; }
            set { tbFirstName.Text = value; }
        }

        public string Lastname
        {
            get { return txtLastName.Text; }
            set { txtLastName.Text = value; }
        }

        public string MiddleName
        {
            get
            {
                if (string.IsNullOrEmpty(tbMiddleName.Text))
                {
                    return string.Empty;
                }
                else
                {
                    return tbMiddleName.Text;
                }
            }
            set { tbMiddleName.Text = value; }
        }

        public string IDNumber
        {
            get { return tbIDNumber.Text; }
            set { tbIDNumber.Text = value; }
        }

        public string Comments
        {
            get
            {
                if (string.IsNullOrEmpty(tbComments.Text))
                {
                    return string.Empty;
                }
                else
                {
                    return tbComments.Text;
                }
            }
            set { tbComments.Text = value; }
        }

        public string Gender
        {
            get
            {
                return rblGender.SelectedValue;
            }
            set { rblGender.SelectedValue = value; }
        }

        public DateTime? EADStopDate
        {
            get;
            set;
        }

        public DateTime? DOB
        {
            get
            {
                DateTime outDate;
                if (!string.IsNullOrEmpty(tbDOB.Text))
                {
                    DateTime.TryParse(tbDOB.Text, out outDate);
                    return outDate;
                }
                else
                {
                    return null;
                }
            }
            set { tbDOB.Text = value.Value.ToStringMil(); }
        }

        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = true; }
        }

        public bool IsPOC
        {
            get { return Convert.ToBoolean(rblIsPOC.SelectedValue); }
            set { rblIsPOC.SelectedValue = value.ToString(); }
        }

        public string BOSTypeSelected
        {
            get { return ddlBOS.SelectedValue; }
            set
            {
                if (ddlBOS.Items.FindByValue(value) != null)
                {
                    ddlBOS.SelectedValue = value;
                }
                else
                {
                    ddlBOS.SelectedValue = "X";
                }
            }
        }

        private List<BOSEntity> _bosType;

        public List<BOSEntity> BoSType
        {
            get { return _bosType; }
            set
            {
                try
                {
                    _bosType = value;
                    ddlBOS.Items.Clear();
                    ddlBOS.DataSource = _bosType;
                    ddlBOS.DataBind();
                    ddlBOS.Items.Insert(0, new ListItem("-Select-", "X"));
                    this.ddlBOS.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlBOS.SelectedIndex = -1;
                }

            }
        }

        public string StatusTypeSelected
        {
            get { return ddlStatusType.SelectedValue; }
            set
            {
                if (ddlStatusType.Items.FindByValue(value) != null)
                {
                    ddlStatusType.SelectedValue = value;
                }
                else
                {
                    ddlStatusType.SelectedValue = "X";
                }
            }
        }

        private List<StatusEntity> _statusType;

        public List<StatusEntity> StatusType
        {
            get { return _statusType; }
            set
            {
                try
                {
                    _statusType = value;
                    ddlStatusType.Items.Clear();
                    ddlStatusType.DataSource = _statusType;
                    ddlStatusType.DataBind();
                    ddlStatusType.Items.Insert(0, new ListItem("-Select-", "X"));
                    this.ddlStatusType.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlStatusType.SelectedIndex = -1;
                }

            }
        }

        public string RankTypeSelected
        {
            get
            {
                if (ddlRank.SelectedValue.StartsWith("*"))
                {
                    return ddlRank.SelectedValue.Remove(0, 1);
                }
                else
                {
                    return ddlRank.SelectedValue;
                }
            }
            set
            {
                if (ddlRank.Items.FindByValue(value) != null)
                {
                    ddlRank.SelectedValue = value;
                }
                else
                {
                    ddlRank.SelectedValue = "X";
                }
            }
        }

        private List<RankEntity> _rankType;

        public List<RankEntity> RankType
        {
            get { return _rankType; }
            set
            {
                try
                {
                    _rankType = value;
                    ddlRank.Items.Clear();
                    ddlRank.DataSource = _rankType;
                    ddlRank.DataBind();
                    ddlRank.Items.Insert(0, new ListItem("-Select-", "X"));
                    this.ddlRank.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlRank.SelectedIndex = -1;
                }

            }
        }

        //public string OrderPrioritySelected
        //{
        //    get { return ddlOrderPriority.SelectedValue; }
        //    set { ddlOrderPriority.SelectedValue = value; }
        //}

        //private List<OrderPriorityEntity> _orderPriority;

        //public List<OrderPriorityEntity> OrderPriority
        //{
        //    get { return _orderPriority; }
        //    set
        //    { }
        //}

        //public string IndividualTypeSelected
        //{
        //    get { return ddlIndividualType.SelectedValue; }
        //    set { ddlIndividualType.SelectedValue = value; }
        //}

        //private DataTable _individualType;

        //public DataTable IndividualType
        //{
        //    get { return _individualType; }
        //    set
        //    {
        //        try
        //        {
        //            _individualType = value;
        //            ddlIndividualType.Items.Clear();
        //            ddlIndividualType.DataSource = _individualType;
        //            ddlIndividualType.DataTextField = "Text";
        //            ddlIndividualType.DataValueField = "Value";
        //            ddlIndividualType.DataBind();
        //            ddlIndividualType.Items.Insert(0, new ListItem("-Select-", "X"));
        //            this.ddlIndividualType.SelectedIndex = 0;
        //        }
        //        catch
        //        {
        //            this.ddlIndividualType.SelectedIndex = -1;
        //        }

        //    }
        //}

        public List<KeyValueEntity> IndividualTypeLookup
        {
            set
            {
                Session["IndividualTypeLookup"] = value;
            }
            get { return Session["IndividualTypeLookup"] as List<KeyValueEntity>; }
        }

        public string IDNumberTypeSelected
        {
            get { return ddlIDNumberType.SelectedValue; }
            set { ddlIDNumberType.SelectedValue = value; }
        }

        private List<LookupTableEntity> _idNumberType;

        public List<LookupTableEntity> IDNumberType
        {
            get { return _idNumberType; }
            set
            {
                try
                {
                    _idNumberType = value;
                    ddlIDNumberType.Items.Clear();
                    ddlIDNumberType.DataSource = _idNumberType;
                    ddlIDNumberType.DataTextField = "Text";
                    ddlIDNumberType.DataValueField = "Value";
                    ddlIDNumberType.DataBind();
                    ddlIDNumberType.Items.Insert(0, new ListItem("-Select-", "X"));
                    this.ddlIDNumberType.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlIDNumberType.SelectedIndex = -1;
                }

            }
        }

        public string SiteSelected
        {
            get { return ddlSite.SelectedValue; }
            set { ddlSite.SelectedValue = value; }
        }

        private List<SiteCodeEntity> _sites;

        public List<SiteCodeEntity> Sites
        {
            get { return _sites; }
            set
            {
                try
                {
                    _sites = value;
                    ddlSite.Items.Clear();
                    ddlSite.DataSource = _sites;
                    ddlSite.DataTextField = "SiteCombinationProfile";
                    ddlSite.DataValueField = "SiteCode";
                    ddlSite.DataBind();
                    ddlSite.Items.Insert(0, new ListItem("X", "-Select-"));
                    ddlSite.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlSite.SelectedIndex = -1;
                }

            }
        }

        public string TheaterLocationCodeSelected
        {
            get { return "000000000"; }
            set { }
        }

        public List<TheaterLocationCodeEntity> TheaterLocationCodes
        {
            get;
            set;
        }

        public List<IndividualEntity> SearchedIndividuals
        {
            set
            {
                this.gvSearch.DataSource = value;
                this.gvSearch.DataBind();
                this.gvSearch.SelectedIndex = -1;
            }
        }

        #endregion Add Individual Accessors

        public List<IdentificationNumbersEntity> AdditionalDmdcIds
        {
            get
            {
                return Session["AdditionalDmdcId"] as List<IdentificationNumbersEntity>;
            }
            set
            {
                Session["AdditionalDmdcId"] = value;
            }
        }
    }
}