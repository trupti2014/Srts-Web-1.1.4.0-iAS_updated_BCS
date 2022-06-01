using SrtsWeb.Account;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Presenters.Account;
using SrtsWeb.Presenters.Individuals;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Individuals;
using SrtsWeb.CustomErrors;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucIndividualEdit : System.Web.UI.UserControl, IIndividualManagementAddView
    {
        private IndividualManagementEditPresenter _presenterEdit;

        private IDemographicXMLHelper dxHelper;

        public ucIndividualEdit()
        {
            _presenterEdit = new IndividualManagementEditPresenter(this);
            dxHelper = new DemographicXMLHelper();
        }

        public SRTSSession mySession
        {
            get
            {
                return (SRTSSession)Session["SRTSSession"];
            }
            set { Session["SRTSSession"] = value; }
        }

        public List<LookupTableEntity> LookupCache
        {
            get { return Cache["SRTSLOOKUP"] as List<LookupTableEntity>; }
            set { Cache["SRTSLOOKUP"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var script = String.Format("function getlblRemainingID() {{ var lblID = '{0}'; return lblID; }} function gettbCommentID() {{ var tbID = '{1}'; return tbID; }}", this.lblRemaining.ClientID, this.tbComments.ClientID);

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "TextLenVal", script, true);

            ceEAD.StartDate = DateTime.Today;
            ceEAD.EndDate = DateTime.Today.AddYears(2);
            ceDOB.StartDate = DateTime.Today.AddYears(-100);
            ceDOB.EndDate = DateTime.Today.AddDays(-1);
            if (!IsPostBack)
            {
                try
                {
                    _presenterEdit.InitView();
                    litPatientNameHeader.Text = string.Format("{0}", mySession.Patient.Individual.NameFMiL);
                    mySession.ReturnURL = "IndividualDetails.aspx?id=personal";
                }
                catch (NullReferenceException ex)
                {
                    ExceptionUtility.LogException(ex, "Error in ucIndividualEdit on Page_Load, null reference exception..");
                }
                finally
                {
                }
                BoSType = dxHelper.GetALLBOS();
                BOSTypeSelected = mySession.Patient.Individual.Demographic.ToBOSKey();
                StatusType = dxHelper.GetStatusByBOS(BOSTypeSelected);
                StatusTypeSelected = mySession.Patient.Individual.Demographic.ToPatientStatusKey();
                RankType = dxHelper.GetRanksByBOSAndStatus(BOSTypeSelected, StatusTypeSelected);
                RankTypeSelected = mySession.Patient.Individual.Demographic.ToRankKey();
            }
        }

        protected SiteCodeEntity GetSiteInfo()
        {
            var siteCodeRepository = new SiteRepository.SiteCodeRepository();

            try
            {
                return siteCodeRepository.GetSiteBySiteID(mySession.Patient.Individual.SiteCodeID).FirstOrDefault();
            }
            catch (NullReferenceException ex)
            {
                ExceptionUtility.LogException(ex, "Error in ucPatientDemographics on Page_Load, null reference exception..");
                return null;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SrtsWebClinic/Individuals/IndividualDetails.aspx");
        }

        public void ddlStatusType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStatusType.SelectedValue == "12" || ddlStatusType.SelectedValue == "15")
            {
                pnlExtra.Visible = true;
            }
            else
            {
                pnlExtra.Visible = false;
            }
            RankType = dxHelper.GetRanksByBOSAndStatus(BOSTypeSelected, StatusTypeSelected);
        }

        protected void ddlRank_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void ddlBOS_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatusType = dxHelper.GetStatusByBOS(BOSTypeSelected);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                _presenterEdit.UpdatePatientRecord();

                // If the individual is also a user in the aspnet_Users table and has a profile in the aspnet_Profile table then
                //    update the SiteCode in the aspnet_Profile table.
                var p = new ProfileServices();
                var lst = p.GetAspnetUserNamesByIndividualId(mySession.Patient.Individual.ID);
                foreach (var s in lst)
                {
                    var cp = CustomProfile.GetProfile(s);
                    cp.Personal.SiteCode = this.SiteSelected;
                    cp.Personal.FirstName = this.FirstName;
                    cp.Personal.Lastname = this.Lastname;
                    cp.Personal.MiddleName = this.MiddleName;
                    cp.Save();
                    cp = null;

                    // Also need to update the users sited code in the Authorization table if the above statement is true.
                    var ap = new AuthorizationPresenter();
                    ap.UpdateSiteCodeByUserName(this.SiteSelected, s);
                    ap = null;
                }
                p = null;

                this.NewPage = "IndividualDetails.aspx";
            }
            else
            {
                vsErrors.Visible = true;
                vsErrors.Focus();
            }
        }

        #region Input Validators

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

        #region Accessors_Patient Edit

        public string ErrorMessage
        {
            set
            {
                vsErrors.ShowSummary = true;
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
            get;
            set;
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
            //get
            //{
            //    return rblGender.SelectedValue;
            //}
            //set { rblGender.SelectedValue = value; }

            get
            {
                return rblGender.SelectedIndex.Equals(-1) ? "N" : rblGender.SelectedValue;
            }
            set
            {
                switch (value)
                {
                    case "M":
                        rblGender.SelectedValue = "M";
                        break;

                    case "F":
                        rblGender.SelectedValue = "F";
                        break;

                    default:
                        rblGender.SelectedIndex = -1;
                        break;
                };
            }
        }

        public DateTime? EADStopDate
        {
            get
            {
                if (!string.IsNullOrEmpty(tbEADExpires.Text))
                {
                    return DateTime.Parse(tbEADExpires.Text);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    tbEADExpires.Text = value.Value.ToShortDateString();
                }
                else
                {
                    tbEADExpires.Text = string.Empty;
                }
            }
        }

        public DateTime? DOB
        {
            get
            {
                if (!string.IsNullOrEmpty(tbDOB.Text))
                {
                    return DateTime.Parse(tbDOB.Text);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    tbDOB.Text = value.Value.ToShortDateString();
                }
                else
                {
                    tbDOB.Text = string.Empty;
                }
            }
        }

        public bool IsActive
        {
            get { return bool.Parse(rblIsActive.SelectedValue); }
            set { rblIsActive.SelectedValue = value.ToString(); }
        }

        public bool IsPOC
        {
            get;
            set;
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

        public string IndividualTypeSelected
        {
            get { return string.Empty; }
            set { }            
            //get { return ddlIndividualType.SelectedValue; }
            //set { ddlIndividualType.SelectedValue = value; }
        }

        //private DataTable _individualType;

        //public DataTable IndividualType
        //{
        //    get { return _individualType; }
        //    set { }
        //    //set
        //    //{
        //    //    try
        //    //    {
        //    //        _individualType = value;
        //    //        ddlIndividualType.Items.Clear();
        //    //        ddlIndividualType.DataSource = _individualType;
        //    //        ddlIndividualType.DataTextField = "Text";
        //    //        ddlIndividualType.DataValueField = "Value";
        //    //        ddlIndividualType.DataBind();
        //    //        ddlIndividualType.Items.Remove("PATIENT");
        //    //        ddlIndividualType.SelectedIndex = -1;
        //    //    }
        //    //    catch
        //    //    {
        //    //        this.ddlIndividualType.SelectedIndex = -1;
        //    //    }
        //    //}
        //}

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

        public string IDNumberTypeSelected
        {
            get;
            set;
        }

        public List<LookupTableEntity> IDNumberType
        {
            get;
            set;
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
                    ddlSite.SelectedValue = mySession.MyClinicCode;
                }
                catch
                {
                    this.ddlSite.SelectedIndex = -1;
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

        public string TheaterLocationCodeSelected
        {
            get { return ddlTheaterLocationCodes.SelectedValue; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    ddlTheaterLocationCodes.SelectedIndex = 1;
                }
                else
                {
                    ddlTheaterLocationCodes.SelectedValue = value;
                }
            }
        }

        private List<TheaterLocationCodeEntity> _theaterLocationCodes;

        public List<TheaterLocationCodeEntity> TheaterLocationCodes
        {
            get { return _theaterLocationCodes; }
            set
            {
                try
                {
                    _theaterLocationCodes = value;
                    ddlTheaterLocationCodes.Items.Clear();
                    ddlTheaterLocationCodes.DataSource = _theaterLocationCodes;
                    ddlTheaterLocationCodes.DataBind();
                    ddlTheaterLocationCodes.Items.Insert(0, new ListItem("-Select-", ""));
                    ddlTheaterLocationCodes.Items.Insert(1, new ListItem("N/A", ""));
                    this.ddlTheaterLocationCodes.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlTheaterLocationCodes.SelectedIndex = -1;
                }

            }
        }

        public List<IndividualEntity> SearchedIndividuals
        {
            set { throw new NotImplementedException(); }
        }

        public List<KeyValueEntity> IndividualTypeLookup
        {
            set
            {
                Session["IndividualTypeLookup"] = value;
            }
            get { return Session["IndividualTypeLookup"] as List<KeyValueEntity>; }
        }

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

                //if (value != null)
                //{
                //    cbAdministrator.Checked = value.Any(x => x.TypeDescription.ToLower() == "other" && x.IsActive == true);
                //    //cbPatient.Checked = value.Any(x => x.TypeDescription.ToLower() == "patient" && x.IsActive == true);
                //    cbProvider.Checked = value.Any(x => x.TypeDescription.ToLower() == "provider" && x.IsActive == true);
                //    cbTechnician.Checked = value.Any(x => x.TypeDescription.ToLower() == "technician" && x.IsActive == true);
                //}
            }
        }

        public bool IsAdmin
        {
            //get { return cbAdministrator.Checked.Equals(true); }
            get { return true; }
        }

        public bool IsTechnician
        {
            //get { return cbTechnician.Checked.Equals(true); }
            get { return true; }
        }

        //public bool IsPatient
        //{
        //    get { return cbPatient.Checked.Equals(true); }
        //}

        public bool IsProvider
        {
            //get { return cbProvider.Checked.Equals(true); }
            get { return true; }
        }


        #endregion Accessors_Patient Edit

        public List<IdentificationNumbersEntity> AdditionalDmdcIds
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}