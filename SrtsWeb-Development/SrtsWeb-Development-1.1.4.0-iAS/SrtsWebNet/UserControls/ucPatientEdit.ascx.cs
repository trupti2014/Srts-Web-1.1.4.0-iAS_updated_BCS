using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Presenters.Patients;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Patients;
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
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucPatientEdit : System.Web.UI.UserControl, IPatientManagementAddView
    {
        private PatientManagementEditPresenter _presenterEdit;
        private IDemographicXMLHelper dxHelper;
        public event EventHandler CancelClick;
        public Boolean IsPatient;
        private String selectedGrade;

        public ucPatientEdit()
        {
            _presenterEdit = new PatientManagementEditPresenter(this);
            dxHelper = new DemographicXMLHelper();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var script = String.Format("function getlblRemainingID() {{ var lblID = '{0}'; return lblID; }} function gettbCommentID() {{ var tbID = '{1}'; return tbID; }}", this.lblRemaining.ClientID, this.tbComments.ClientID);

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "TextLenVal", script, true);

            ceEAD.StartDate = DateTime.Today;
            ceEAD.EndDate = DateTime.Today.AddYears(2);
            ceDOB.StartDate = DateTime.Today.AddYears(-100);
            ceDOB.EndDate = DateTime.Today.AddDays(-1);

            if (Session != null && Session["isP"] != null)
            {
                this.IsPatient = Session["isP"].ToBoolean();
                //Session.Remove("isP");
            }
            if (!IsPostBack)
            {
                try
                {
                    _presenterEdit.InitView();
                    litPatientNameHeader.Text = string.Format("{0}", mySession.Patient.Individual.NameFMiL);
                    mySession.ReturnURL = "PersonDetails.aspx?id=personal";
                }
                catch (NullReferenceException ex)
                {
                    ExceptionUtility.LogException(ex, "Error in ucPatientEdit on Page_Load, null reference exception..");
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

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            _presenterEdit.UpdatePatientRecord(IsPatient);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            CancelClick(sender, e);
            //Response.Redirect(String.Format("~/SrtsPerson/PersonDetails.aspx?id={0}&isP={1}", this.mySession.SelectedPatientID));
        }

        protected void ddlBOS_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatusType = dxHelper.GetStatusByBOS(BOSTypeSelected);
            if (StatusTypeSelected.Equals(0)) return;

            RankType = dxHelper.GetRanksByBOSAndStatus(BOSTypeSelected, StatusTypeSelected);
            this.ddlStatusType.Focus();
        }

        public void ddlStatusType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedGrade = this.ddlRank.SelectedValue;
            RankType = dxHelper.GetRanksByBOSAndStatus(BOSTypeSelected, StatusTypeSelected);
            this.ddlRank.Focus();
        }

        #region Input Validators

        protected void ValidateEAD(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Length >= 1)
            {
                if (args.IsValid = args.Value.ValidateDOBFormat())
                {
                    if (args.IsValid = args.Value.ValidateDOBIsValid())
                    {
                        if (args.IsValid = args.Value.ValidateDateInRange(DatePart.Year, 2, true))
                        {
                        }
                        else
                        {
                            cvEad.ErrorMessage = "Invalid Extended Active Date (can't be greater than 2 years)";
                            cvEad.IsValid = false;
                            args.IsValid = false;
                        }
                    }
                    else
                    {
                        cvEad.ErrorMessage = "Extended Active Date entry is invalid";
                        cvEad.IsValid = false;
                        args.IsValid = false;
                    }
                }
                else
                {
                    cvEad.ErrorMessage = "Invalid characters or format in Extended Active Date";
                    cvEad.IsValid = false;
                    args.IsValid = false;
                }
            }
        }

        protected void ValidateFirstName(object source, ServerValidateEventArgs args)
        {
            if (args.IsValid = args.Value.ValidateNameLength())
            {
                if (args.IsValid = args.Value.ValidateNameFormat())
                {
                }
                else
                {
                    cvFirstName.ErrorMessage = "Invalid characters in First Name";
                }
            }
            else
            {
                cvFirstName.ErrorMessage = "Patient first name required (1-40 characters)";
            }
        }

        protected void ValidateMiddleName(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Length >= 1)
            {
                if (args.IsValid = args.Value.ValidateNameLength())
                {
                    if (args.IsValid = args.Value.ValidateNameFormat())
                    {
                    }
                    else
                    {
                        cvMiddleName.ErrorMessage = "Invalid characters in Middle Name";
                    }
                }
                else
                {
                    cvMiddleName.ErrorMessage = "Patient middle name can't be more than 40 characters";
                }
            }
        }

        protected void ValidateLastName(object source, ServerValidateEventArgs args)
        {
            if (args.IsValid = args.Value.ValidateNameLength())
            {
                if (args.IsValid = args.Value.ValidateNameFormat())
                {
                }
                else
                {
                    cvLastName.ErrorMessage = "Invalid characters in Last Name";
                }
            }
            else
            {
                cvLastName.ErrorMessage = "Patient last name required (1-40 characters)";
            }
        }

        protected void ValidateDOB(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Length >= 1)
            {
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
                        cvComment.ErrorMessage = "Invalid character(s) in Comments";
                    }
                }
                else
                {
                    cvComment.ErrorMessage = string.Format("Limit is {0} characters in Comments", limit.ToString());
                }
            }
        }

        protected void FocusOnError()
        {
            foreach (BaseValidator validator in Page.Validators)
            {
                if (!validator.IsValid)
                {
                    if (validator.ID != "cvGender")
                    {
                        validator.FindControl(validator.ControlToValidate).Focus();
                        break;
                    }
                }
            }
        }

        #endregion Input Validators

        #region Accessors_Patient Edit

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
                var d = new DateTime();
                if (!string.IsNullOrEmpty(tbEADExpires.Text))
                {
                    if (DateTime.TryParse(tbEADExpires.Text, out d)) return d;
                    return null;
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
                    tbEADExpires.Text = value.Value.ToString(Globals.DtFmt);
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
                    return DateTime.Parse("1/1/1900");
                }
            }
            set
            {
                if (value.HasValue && (value.ToString().Substring(0, 8) != "1/1/1900"))
                {
                    tbDOB.Text = value.Value.ToStringMil();
                }
                else
                {
                    tbDOB.Text = string.Empty;
                }
            }
        }

        public bool IsActive
        {
            get;
            set;
        }

        public bool IsPOC
        {
            get;
            set;
        }

        public List<IndividualEntity> SearchedPatients
        {
            set { throw new NotImplementedException(); }
            get { throw new NotImplementedException(); }
        }

        public string StatusTypeSelected
        {
            get { return ddlStatusType.SelectedValue; }
            set { ddlStatusType.SelectedValue = value; }
        }

        private List<StatusEntity> _statusType;

        public List<StatusEntity> StatusType
        {
            get { return _statusType; }
            set
            {
                _statusType = value;
                try
                {
                    ddlStatusType.Items.Clear();
                    ddlStatusType.DataSource = _statusType;
                    ddlStatusType.DataBind();
                }
                catch { }
            }
        }

        public string IndividualTypeSelected
        {
            get { return "PATIENT"; }
            set { }
        }

        public List<LookupTableEntity> IndividualType
        {
            get;
            set;
        }

        public List<KeyValueEntity> IndividualTypeLookup
        {
            set
            {
                Session["IndividualTypeLookup"] = value;
            }
            get { return Session["IndividualTypeLookup"] as List<KeyValueEntity>; }
        }

        public string RankTypeSelected
        {
            get { return ddlRank.SelectedValue; }
            set { ddlRank.SelectedValue = value; }
        }

        private List<RankEntity> _rankType;

        public List<RankEntity> RankType
        {
            get { return _rankType; }
            set
            {
                _rankType = value;
                try
                {
                    ddlRank.Items.Clear();
                    ddlRank.DataSource = _rankType;
                    ddlRank.DataBind();
                    if (!value.Any(x => x.RankValue == this.selectedGrade)) return;
                    ddlRank.SelectedValue = this.selectedGrade;
                }
                catch { }
            }
        }

        public string BOSTypeSelected
        {
            get { return ddlBOS.SelectedValue; }
            set { ddlBOS.SelectedValue = value; }
        }

        private List<BOSEntity> _bosType;

        public List<BOSEntity> BoSType
        {
            get { return _bosType; }
            set
            {
                _bosType = value;
                try
                {
                    ddlBOS.Items.Clear();
                    ddlBOS.DataSource = _bosType;
                    ddlBOS.DataBind();
                }
                catch { }
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
                _sites = value;
                try
                {
                    ddlSite.Items.Clear();
                    ddlSite.DataSource = _sites;
                    ddlSite.DataTextField = "SiteCombinationProfile";
                    ddlSite.DataValueField = "SiteCode";
                    ddlSite.DataBind();
                    ddlSite.SelectedValue = mySession.MyClinicCode;
                }
                catch { }
            }
        }

        //public string OrderPrioritySelected
        //{
        //    get;// { return ddlOrderPriority.SelectedValue; }
        //    set;// { ddlOrderPriority.SelectedValue = value; }
        //}

        //private List<OrderPriorityEntity> _orderPriority;

        //public List<OrderPriorityEntity> OrderPriority
        //{
        //    get;// { return _orderPriority; }
        //    set;// { }
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
                _theaterLocationCodes = value;
                try
                {
                    ddlTheaterLocationCodes.Items.Clear();
                    ddlTheaterLocationCodes.DataSource = _theaterLocationCodes;
                    ddlTheaterLocationCodes.DataBind();
                    ddlTheaterLocationCodes.Items.Insert(0, new ListItem("-Select-", "", true));
                    ddlTheaterLocationCodes.Items.Insert(1, new ListItem("N/A", "", true));
                }
                catch
                {
                    this.ddlTheaterLocationCodes.SelectedIndex = -1;
                }
            }
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