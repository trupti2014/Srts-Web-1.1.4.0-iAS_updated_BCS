using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Presenters.Patients;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Patients;
using SrtsWeb.CustomErrors;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    public partial class ucPatientAdd : UserControlBase, IPatientManagementAddView, IComboAddView
    {
        private PatientManagementAddPresenter _presenter;
        private ComboAddPresenter _cPresenter;
        private IDemographicXMLHelper dxHelper;

        public ucPatientAdd()
        {
            _presenter = new PatientManagementAddPresenter(this);
            _cPresenter = new ComboAddPresenter(this);
            dxHelper = new DemographicXMLHelper();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ceEAD.StartDate = DateTime.Today;
                ceEAD.EndDate = DateTime.Today.AddYears(2);
                ceDOB.StartDate = DateTime.Today.AddYears(-100);
                ceDOB.EndDate = DateTime.Today.AddDays(-1);
                try
                {
                    _presenter.InitView();
                    _cPresenter.InitView();
                    mySession.ReturnURL = "PersonDetails.aspx";
                }
                catch (NullReferenceException ex)
                {
                    ExceptionUtility.LogException(ex, "Error in ucPatientAdd on Page_Load, null reference exception..");
                }
                finally
                {
                }
                BoSType = dxHelper.GetALLBOS();

                this.ddlIDNumberType.Focus();

                // This will come from the usercontrol quick search.
                if (Session["qsId"] != null)
                {
                    var i = Session["qsId"].ToString();
                    Session.Remove("qsId");
                    this.IDNumberTypeSelected = i.Length.Equals(9) ? "SSN" : "DIN";
                    this.IDNumber = i;
                    this.tbIDNumber_TextChanged(this, null);
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Page.Validate("allValidators");
            if (!Page.IsValid)
            {
                vsErrors.Visible = true;
                vsAddrErrors.Visible = true;
                FocusOnError();
                return;
            }

            vsErrors.Visible = false;
            vsAddrErrors.Visible = false;
            ErrorMessage = string.Empty;

            _presenter.AddPatientRecord();

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                vsErrors.Visible = true;
                vsAddrErrors.Visible = true;
                ShowMessage();
            }
            else
            {
                if (IsAddressEntered()) _cPresenter.SaveAddress();
                _cPresenter.SaveEmail();
                _cPresenter.SavePhone();
                this.NewPage = String.Format("~/SrtsPerson/PersonDetails.aspx?id={0}&isP=true", this.mySession.Patient.Individual.ID);
            }
        }

        private void ShowMessage()
        {
            CustomValidator cv = new CustomValidator();
            cv.IsValid = false;
            cv.ErrorMessage = ErrorMessage;
            cv.ValidationGroup = "allValidators";
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
            Response.Redirect("~/SrtsWebClinic/Patients/ManagePatients.aspx/search");
        }

        /*protected void ddlIDNumberType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlIDNumberType.SelectedIndex.Equals(0))
            {
                this.taboff.Visible = false;
                rfvIDNumberType.IsValid = false;
                ddlIDNumberType.Focus();
            }
            else
            {
                this.taboff.Visible = true;

                if (!String.IsNullOrEmpty(tbIDNumber.Text))
                {
                    if (ValidateIDNumber())
                    {
                        cvIDNumber.IsValid = true;
                        if (_presenter.SearchPerson())
                        {
                            this.tbIDNumber.Focus();
                            //_presenter.SearchPerson();
                            this.gvSearch.Visible = true;
                            pnlCompleteForm.Visible = false;
                        }
                        // If it is not a duplicate then check in DMDC.  Only for SSN and DODID searches
                        else if (this.IDNumberTypeSelected.Equals("DIN") || this.IDNumberTypeSelected.Equals("SSN"))
                        {
                            _presenter.SearchPersonDmdc(this);

                            this.taboff.Visible = false;
                            this.gvSearch.Visible = false;
                            this.pnlCompleteForm.Visible = true;
                        }
                        //If not duplicate
                        else
                        {
                            this.gvSearch.Visible = false;
                            pnlCompleteForm.Visible = true;
                            taboff.Visible = false;
                            this.txtLastName.Focus();
                        }
                    }
                    else
                    {
                        this.taboff.Visible = true;
                        cvIDNumber.IsValid = false;
                        this.gvSearch.Visible = false;
                        pnlCompleteForm.Visible = false;
                    }
                }
                else
                {
                    this.taboff.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "idNumberFocus", "idNumberFocus();", true);
                }
            }
        }*/

        protected void tbIDNumber_TextChanged(object sender, EventArgs e)
        {
            if (this.ddlIDNumberType.SelectedIndex.Equals(0))
            {
                this.taboff.Visible = false;
                this.gvSearch.Visible = false;
                rfvIDNumberType.IsValid = false;
                this.ddlIDNumberType.Focus();
            }
            else
            {
                this.taboff.Visible = true;

                if (!ValidateIDNumber())
                {
                    this.taboff.Visible = true;
                    ShowCV_IDNumberMessage();
                    pnlCompleteForm.Visible = false;
                    this.gvSearch.Visible = false;
                    return;
                }

                // Get DMDC data if it exists
                // Do the SearchPerson method for all ID's returned.
                if (this.IDNumberTypeSelected.Equals("DIN") || this.IDNumberTypeSelected.Equals("SSN"))
                {
                    if (_presenter.SearchPatientDmdc(this))
                    {
                        //Get the additional DMDC IDs
                        foreach (var id in this.AdditionalDmdcIds)
                        {
                            if (!_presenter.SearchPatient(id.IDNumber, id.IDNumberType)) continue;
                            // They are a duplicate...
                            this.taboff.Visible = false;
                            this.gvSearch.Visible = true;
                            pnlCompleteForm.Visible = false;
                            return;
                        }

                        this.taboff.Visible = false;
                        this.gvSearch.Visible = false;
                        this.pnlCompleteForm.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "lastNameFocus", "lastNameFocus();", true);

                        return;
                    }
                }

                // The search is on an ID type other than SSN or DODID
                if (_presenter.SearchPatient())
                {
                    this.taboff.Visible = false;
                    this.gvSearch.Visible = true;
                    pnlCompleteForm.Visible = false;
                }
                else //If is not a Duplicate
                {
                    this.taboff.Visible = false;
                    this.gvSearch.Visible = false;
                    this.taboff.Visible = false;
                    pnlCompleteForm.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "lastNameFocus", "idNumberFocus();", true);
                }
            }
        }

        protected void ddlBOS_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatusType = dxHelper.GetStatusByBOS(ddlBOS.SelectedValue.ToString());
            this.ddlStatusType.Focus();
        }

        protected void ddlStatusType_SelectedIndexChanged1(object sender, EventArgs e)
        {
            RankType = dxHelper.GetRanksByBOSAndStatus(ddlBOS.SelectedValue.ToString(), ddlStatusType.SelectedValue.ToString());
            this.ddlRank.Focus();
        }

        protected void gvSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var str = String.Format("location.href='../PersonDetails.aspx?id={0}&isP=true'", DataBinder.Eval(e.Row.DataItem, "ID"));

                var pId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "ID"));

                mySession.SelectedPatientID = pId;

                var b = (Button)e.Row.FindControl("btnAddPatientType");
                b.CommandArgument = pId.ToString();

                var g = (Button)e.Row.FindControl("btnGoToPatient");
                g.OnClientClick = str;

                // Determine if there is a person type of patient.  If so then hide the add button else hide the view button
                var p = this.SearchedPatients.Where(x => x.ID == pId);

                var isP = false;
                foreach (var a in p)
                {
                    if (a.IsNewPatient) continue;
                    isP = true;
                }

                g.Visible = isP;
                b.Visible = !isP;

                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#E2F2FE'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
                e.Row.Attributes.Add("style", "cursor:pointer;");
            }
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
                        cvComment.ErrorMessage = "Invalid character(s) in Comments";
                    }
                }
                else
                {
                    cvComment.ErrorMessage = string.Format("Limit is {0} characters in Comments", limit.ToString());
                }
            }
        }

        protected bool IsAddressEntered()
        {
            if (!string.IsNullOrEmpty(Address1) || !string.IsNullOrEmpty(Address2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void ValidateCity(object source, ServerValidateEventArgs args)
        {
            if (IsAddressEntered())
            {
                if (args.Value.Length > 0)
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                    cvCity.ErrorMessage = "City is a required field";
                }
            }
        }

        protected void ValidateState(object source, ServerValidateEventArgs args)
        {
            if (IsAddressEntered())
            {
                if (args.Value != "0")
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                    cvState.ErrorMessage = "State is a required field";
                }
            }
        }

        protected void ValidateZip(object source, ServerValidateEventArgs args)
        {
            if (IsAddressEntered())
            {
                if (args.Value.Length > 0)
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                    cvZipCode.ErrorMessage = "Zipcode is a required field";
                }
            }
        }

        protected void ValidateCountry(object source, ServerValidateEventArgs args)
        {
            if (IsAddressEntered())
            {
                if (args.Value != "0")
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                    cvCountry.ErrorMessage = "Country is a required field";
                }
            }
        }

        protected void ValidateAddressType(object source, ServerValidateEventArgs args)
        {
            if (IsAddressEntered())
            {
                if (args.Value != "0")
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                    cvAddressType.ErrorMessage = "Address type is a required field";
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

        #region Add Patient Accessors

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
            set { Response.Redirect(value); }
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
                if (value.HasValue && !value.Value.Equals(DateTime.MinValue))
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
            set
            {
                if (value.HasValue && !value.Value.Equals(DateTime.MinValue))
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
                    ddlStatusType.SelectedIndex = -1;
                }
                catch
                {
                    this.ddlStatusType.SelectedIndex = -1;
                }
            }
        }

        //private string _indiType = "PATIENT";

        //public string IndividualTypeSelected
        //{
        //    get { return _indiType; }
        //    set { _indiType = "PATIENT"; }
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

        public List<RankEntity> RankType
        {
            set
            {
                try
                {
                    ddlRank.Items.Clear();
                    ddlRank.DataSource = value;
                    ddlRank.DataBind();
                    ddlRank.Items.Insert(0, new ListItem("-Select-", "X"));
                    ddlRank.SelectedIndex = -1;
                }
                catch
                {
                    this.ddlRank.SelectedIndex = -1;
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
                    ddlBOS.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlBOS.SelectedIndex = -1;
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
                    ddlIDNumberType.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlIDNumberType.SelectedIndex = -1;
                }
            }
        }

        public string IDNumberTypeSelected
        {
            get { return ddlIDNumberType.SelectedValue; }
            set { ddlIDNumberType.SelectedValue = value; }
        }

        public List<LookupTableEntity> IndividualType { get; set; }

        public string SiteSelected
        {
            get { return this.mySession.MySite.SiteCode; } // { return ddlSite.SelectedValue; }
            set { var a = value; } // { ddlSite.SelectedValue = value; }
        }

        //private List<SiteCodeEntity> _sites;

        public List<SiteCodeEntity> Sites
        {
            get; //{ return _sites; }
            set;
            //{
            //    try
            //    {
            //        _sites = value;
            //        ddlSite.Items.Clear();
            //        ddlSite.DataSource = _sites;
            //        ddlSite.DataTextField = "SiteCombinationProfile";
            //        ddlSite.DataValueField = "SiteCode";
            //        ddlSite.DataBind();
            //        ddlSite.Items.Insert(0, new ListItem("-Select-", "X"));
            //        ddlSite.SelectedValue = mySession.MyClinicCode;
            //    }
            //    catch
            //    {
            //        FormsAuthentication.RedirectToLoginPage();
            //    }
            //}
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
            set { ddlTheaterLocationCodes.SelectedValue = value; }
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
                    ddlTheaterLocationCodes.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlTheaterLocationCodes.SelectedIndex = -1;
                }
            }
        }

        public List<IndividualEntity> SearchedPatients
        {
            set
            {
                Session["SearchedPatients"] = value;
                this.gvSearch.DataSource = value
                    .Select(x => new { x.ID, x.LastName, x.FirstName, x.IDNumberDisplay, x.IDNumberTypeDescription, x.BOSDescription, x.StatusDescription, x.Rank, x.Gender, x.IsNewPatient })
                    .Distinct()
                    .ToArray();
                this.gvSearch.DataBind();
                this.gvSearch.SelectedIndex = -1;
            }
            get { return Session["SearchedPatients"] as List<IndividualEntity>; }
        }

        public List<KeyValueEntity> IndividualTypeLookup
        {
            set
            {
                Session["IndividualTypeLookup"] = value;
            }
            get { return Session["IndividualTypeLookup"] as List<KeyValueEntity>; }
        }

        public string IndividualTypeMessage
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

        #endregion Add Patient Accessors

        #region IComboAddView

        public List<LookupTableEntity> IDTypeDDL
        {
            set { }
        }

        public string IDNumberMessage
        {
            get;
            set;
        }

        string IComboAddView.IDNumberType
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

        public string Address1
        {
            get { return tbAddress1.Text; }
            set { tbAddress1.Text = value; }
        }

        public string Address2
        {
            get { return tbAddress2.Text; }
            set { tbAddress2.Text = value; }
        }

        public string City
        {
            get
            {
                return tbCity.Text;
            }
            set
            {
                tbCity.Text = value;
            }
        }

        public string CountrySelected
        {
            get
            {
                return ddlCountry.SelectedValue;
            }
            set
            {
                if (ddlCountry.Items.FindByValue(value) != null)
                {
                    ddlCountry.SelectedValue = value;
                }
                else
                {
                    ddlCountry.SelectedValue = "X";
                }
            }
        }

        public List<LookupTableEntity> CountryDDL
        {
            set
            {
                try
                {
                    ddlCountry.Items.Clear();
                    ddlCountry.DataSource = value;
                    ddlCountry.DataTextField = "Text";
                    ddlCountry.DataValueField = "Value";
                    ddlCountry.DataBind();
                    ddlCountry.Items.Insert(0, new ListItem("-Select-", "X"));
                    ddlCountry.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlCountry.SelectedIndex = -1;
                }
            }
        }

        public string StateSelected
        {
            get
            {
                return ddlState.SelectedValue;
            }
            set
            {
                if (ddlState.Items.FindByValue(value) != null)
                {
                    ddlState.SelectedValue = value;
                }
                else
                {
                    ddlState.SelectedValue = "X";
                }
            }
        }

        public List<LookupTableEntity> StateDDL
        {
            set
            {
                try
                {
                    ddlState.Items.Clear();
                    ddlState.DataSource = value;
                    ddlState.DataTextField = "ValueTextCombo";
                    ddlState.DataValueField = "Value";
                    ddlState.DataBind();
                    ddlState.Items.Insert(0, new ListItem("-Select-", "X"));
                    ddlState.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlState.SelectedIndex = -1;
                }
            }
        }

        public string AddressTypeSelected
        {
            get
            {
                return ddlAddressType.SelectedValue;
            }
            set
            {
                ddlAddressType.SelectedValue = value;
            }
        }

        public List<LookupTableEntity> AddressTypeDDL
        {
            set
            {
                try
                {
                    ddlAddressType.Items.Clear();
                    ddlAddressType.DataSource = value;
                    ddlAddressType.DataTextField = "Text";
                    ddlAddressType.DataValueField = "Value";
                    ddlAddressType.DataBind();
                    ddlAddressType.Items.Insert(0, new ListItem("-Select-", "0"));
                    ddlAddressType.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlAddressType.SelectedIndex = -1;
                }
            }
        }

        public string ZipCode
        {
            get
            {
                return tbZipCode.Text;
            }
            set
            {
                tbZipCode.Text = value;
            }
        }

        public string UIC
        {
            get
            {
                return tb2UIC.Text;
            }
            set
            {
                tb2UIC.Text = value;
            }
        }

        public DataTable UICDDL
        {
            get;
            set;
        }

        public string UICSelected
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

        public string AddrMessage
        {
            get
            {
                return litMessage.Text;
            }
            set
            {
                litMessage.Text = value;
            }
        }

        public string EMailAddress
        {
            get
            {
                return tbEMailAddress.Text;
            }
            set
            {
                tbEMailAddress.Text = value;
            }
        }

        public string TypeEMailSelected
        {
            get
            {
                return ddlEMailType.SelectedValue;
            }
            set
            {
                ddlEMailType.SelectedValue = value;
            }
        }

        public List<LookupTableEntity> TypeEmailDDL
        {
            set
            {
                try
                {
                    ddlEMailType.Items.Clear();
                    ddlEMailType.DataSource = value;
                    ddlEMailType.DataTextField = "Text";
                    ddlEMailType.DataValueField = "Value";
                    ddlEMailType.DataBind();
                    ddlEMailType.Items.Insert(0, new ListItem("-Select-", "0"));
                    ddlEMailType.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlEMailType.SelectedIndex = -1;
                }
            }
        }

        public string EmailMessage
        {
            get;
            set;
        }

        public string Extension
        {
            get
            {
                return tbExtension.Text;
            }
            set
            {
                tbExtension.Text = value;
            }
        }

        public string PhoneNumber
        {
            get
            {
                return tbPhoneNumber.Text;
            }
            set
            {
                tbPhoneNumber.Text = value;
            }
        }

        public List<LookupTableEntity> PhoneTypeDDL
        {
            set
            {
                try
                {
                    ddlPhoneType.Items.Clear();
                    ddlPhoneType.DataSource = value;
                    ddlPhoneType.DataTextField = "Text";
                    ddlPhoneType.DataValueField = "Value";
                    ddlPhoneType.DataBind();
                    ddlPhoneType.Items.Insert(0, new ListItem("-Select-", "0"));
                    ddlPhoneType.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlPhoneType.SelectedIndex = -1;
                }
            }
        }

        public string TypePhoneSelected
        {
            get
            {
                return ddlPhoneType.SelectedValue;
            }
            set
            {
                ddlPhoneType.SelectedValue = value;
            }
        }

        public string PhoneMessage
        {
            get
            {
                return tbPhoneNumber.Text;
            }
            set
            {
                tbPhoneNumber.Text = value;
            }
        }

        #endregion IComboAddView

        protected void btnAddPatientType_Command(object sender, CommandEventArgs e)
        {
            // Get the ID and add the patient type to the individual record
            var pId = Convert.ToInt32(e.CommandArgument);
            _presenter.AddIndividualType(pId);
            if (!String.IsNullOrEmpty(this.ErrorMessage))
            {
                ShowMessage();
                return;
            }

            // Add phone number if none exist
            _presenter.AddDefaultPhoneNumber(pId);
            if (!String.IsNullOrEmpty(this.ErrorMessage))
            {
                ShowMessage();
                return;
            }

            // Determine if there is a second ID Number that needs to be added to the Individual record
            foreach (var i in this.AdditionalDmdcIds)
            {
                if (String.IsNullOrEmpty(i.IDNumber)) continue;

                foreach (var p in this.SearchedPatients)
                {
                    if (p.IDNumber.Equals(i.IDNumber)) continue;

                    i.IndividualID = p.ID;

                    if (!_cPresenter.SaveIDNumbers(i))
                    {
                        this.ErrorMessage = this.IDNumberMessage;
                        ShowMessage();
                        return;
                    }
                }
            }

            Response.Redirect(String.Format("~/SrtsPerson/PersonDetails.aspx?id={0}&isP=true", pId), true);
        }

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