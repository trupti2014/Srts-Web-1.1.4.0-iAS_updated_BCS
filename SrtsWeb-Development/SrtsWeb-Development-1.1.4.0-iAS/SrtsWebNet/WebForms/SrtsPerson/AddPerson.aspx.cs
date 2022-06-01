using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Person;
using SrtsWeb.Views.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.SrtsPerson
{
    // Admin roles
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    //Non-Admin roles
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    public partial class AddPerson : PageBase, IPersonAddView
    {
        private PersonAddPresenter _presenter;

        public AddPerson()
        {
            this._presenter = new PersonAddPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "AddPerson_Page_Load", mySession.MyUserID))
#endif
            {
                if (!Page.IsPostBack)
                {
                    this.IsAdmin = Roles.GetRolesForUser().ToList().Any(x => x.ToLower().EndsWith("admin") || x.ToLower().Equals("mgmtenterprise"));

                    ceEAD.StartDate = DateTime.Today;
                    ceEAD.EndDate = DateTime.Today.AddYears(2);
                    ceDOB.StartDate = DateTime.Today.AddYears(-100);
                    ceDOB.EndDate = DateTime.Today.AddDays(-1);

                    try
                    {
                        _presenter.InitView();
                    }
                    catch (NullReferenceException ex)
                    {
                        ex.LogException("Error in AddPerson on Page_Load, null reference exception..");
                    }

                    this.ddlIDNumberType.Focus();

                    // This will come from the usercontrol quick search.
                    if (Session["qsId"] != null)
                    {
                        var i = Session["qsId"].ToString();
                        Session.Remove("qsId");
                        this.IdNumberType = i.Length.Equals(9) ? "SSN" : "DIN";
                        this.IdNumber = i;
                        this.tbIDNumber_TextChanged(this, null);
                    }

                    BuildPageTitle();
                }
            }
        }

        protected void tbIDNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "AddPerson_tbIDNumber_TextChanged", mySession.MyUserID))
#endif
                {
                    var _presenter = new PersonAddPresenter(this);

                    if (this.ddlIDNumberType.SelectedIndex.Equals(0))
                    {
                        this.lblTaboff.Visible = false;
                        this.gvSearch.Visible = false;
                        this.rfvIDNumberType.IsValid = false;
                        this.divButtons.Visible = false;
                        this.ddlIDNumberType.Focus();
                    }
                    else
                    {
                        this.lblTaboff.Visible = true;

                        if (!ValidateIDNumber())
                        {
                            this.lblTaboff.Visible = true;
                            ShowCV_IDNumberMessage();
                            this.pnlCompleteForm.Visible = false;
                            this.gvSearch.Visible = false;
                            this.divButtons.Visible = false;
                            return;
                        }

                        // Get DMDC data if it exists
                        // Do the SearchPerson method for all ID's returned.
                        if (this.IdNumberType.Equals("DIN") || this.IdNumberType.Equals("SSN"))
                        {
                            if (_presenter.SearchPersonDmdc())
                            {
                                //Get the additional DMDC IDs
                                foreach (var id in this.AdditionalDmdcIdList)
                                {
                                    if (!_presenter.SearchPerson(id.IDNumber, id.IDNumberType)) continue;
                                    // They are a duplicate...
                                    this.lblTaboff.Visible = false;
                                    this.gvSearch.Visible = true;
                                    this.pnlCompleteForm.Visible = false;
                                    this.divButtons.Visible = false;
                                    return;
                                }

                                this.lblTaboff.Visible = false;
                                this.gvSearch.Visible = false;
                                this.pnlCompleteForm.Visible = true;
                                this.divButtons.Visible = true;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "lastNameFocus", "lastNameFocus();", true);

                                return;
                            }
                        }

                        // The search is on an ID type other than SSN or DODID
                        if (_presenter.SearchPerson())
                        {
                            this.lblTaboff.Visible = false;
                            this.gvSearch.Visible = true;
                            this.pnlCompleteForm.Visible = false;
                            this.divButtons.Visible = false;
                        }
                        else //If is not a Duplicate
                        {
                            this.lblTaboff.Visible = false;
                            this.gvSearch.Visible = false;
                            this.pnlCompleteForm.Visible = true;
                            this.divButtons.Visible = true;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "lastNameFocus", "idNumberFocus();", true);
                        }
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void ddlBOS_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "AddPerson_ddlBOS_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    _presenter.GetStatus();
                    this.ddlStatusType.Focus();
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void ddlStatusType_SelectedIndexChanged1(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "AddPerson_", mySession.MyUserID))
#endif
                {
                    _presenter.GetGrade();
                    this.ddlRank.Focus();
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void gvSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var pId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "ID"));

            mySession.SelectedPatientID = pId;

            var b = (Button)e.Row.FindControl("btnAddIndividualType");
            b.CommandArgument = pId.ToString();

            var g = (Button)e.Row.FindControl("btnGoToPerson");

            g.OnClientClick = String.Format("window.location.href = '/WebForms/SrtsPerson/PersonDetails.aspx?id={0}&isP={1}'; return false;", pId, !this.IsAdmin);

            // Determine if there is a person type of patient.  If so then hide the add button else hide the view button
            var p = this.SearchedPersonList.Where(x => x.ID == pId);

            var isPatient = p.Any(x => x.IsNewPatient);

            b.Visible = !isPatient && !this.IsAdmin; // Show button to add patient type.
            g.Visible = isPatient || this.IsAdmin;

            e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#E2F2FE'");
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
            e.Row.Attributes.Add("style", "cursor:pointer;");
        }

        protected void btnAddIndividualType_Command(object sender, CommandEventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "AddPerson_", mySession.MyUserID))
#endif
                {
                    // Get the ID and add the patient type to the individual record
                    var pId = Convert.ToInt32(e.CommandArgument);
                    this.ErrorMessage = String.Empty;

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
                    foreach (var i in this.AdditionalDmdcIdList)
                    {
                        if (String.IsNullOrEmpty(i.IDNumber)) continue;

                        foreach (var p in this.SearchedPersonList)
                        {
                            if (p.IDNumber.Equals(i.IDNumber)) continue;

                            i.IndividualID = p.ID;

                            if (!_presenter.SaveIDNumbers(i))
                            {
                                ShowMessage();
                                return;
                            }
                        }
                    }

                    Response.Redirect(String.Format("~/WebForms/SrtsPerson/PersonDetails.aspx?id={0}&isP=true", pId), false);
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (IsAdmin)
                Response.Redirect("~/WebForms/default.aspx");
            else
                Response.Redirect("~/WebForms/SrtsWebClinic/Patients/ManagePatients.aspx/search");
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Page.Validate("allValidators");

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "AddPerson_", mySession.MyUserID))
#endif
                {
                    if (this.IsAdmin)
                    {
                        if (Page.IsValid)
                        {
                            _presenter.AddIndividualRecord();

                            if (!string.IsNullOrEmpty(ErrorMessage))
                            {
                                LogEvent(String.Format("User {0} unsuccessfully added a new individual record at {1}", mySession.MyUserID, DateTime.Now));
                                ShowMessage();
                            }
                            else
                            {
                                LogEvent(String.Format("User {0} added a new individual record at {1}", mySession.MyUserID, DateTime.Now));
                                Response.Redirect(String.Format("~/WebForms/SrtsPerson/PersonDetails.aspx?id={0}&isP={1}", this.mySession.Patient.Individual.ID, !this.IsAdmin), false);
                            }
                        }
                        else
                        {
                            vsErrors.Visible = true;
                        }
                    }
                    else
                    {
                        if (!Page.IsValid)
                        {
                            vsErrors.Visible = true;
                            FocusOnError();
                            return;
                        }

                        vsErrors.Visible = false;
                        ErrorMessage = string.Empty;

                        _presenter.AddPatientRecord();

                        if (!string.IsNullOrEmpty(ErrorMessage))
                        {
                            LogEvent(String.Format("User {0} unsuccessfully added new patient record at {1}.", mySession.MyUserID, DateTime.Now));
                            vsErrors.Visible = true;
                            ShowMessage();
                        }
                        else
                        {
                            LogEvent(String.Format("User {0} added new patient record at {1}.", mySession.MyUserID, DateTime.Now));

                            var m = String.Empty;

                            m = _presenter.SaveAddress() ? "added new patient address record" : "unsuccessfully added new patient address record";
                            LogEvent(String.Format("User {0} {1} at {2}.", mySession.MyUserID, m, DateTime.Now));

                            m = _presenter.SaveEmail() ? "added new patient email record" : "unsuccessfully added new patient email record";
                            LogEvent(String.Format("User {0} {1} at {2}.", mySession.MyUserID, m, DateTime.Now));

                            m = _presenter.SavePhone() ? "added new patient phone record" : "unsuccessfully added new patient phone record";
                            LogEvent(String.Format("User {0} {1} at {2}.", mySession.MyUserID, m, DateTime.Now));

                            Response.Redirect(String.Format("~/WebForms/SrtsPerson/PersonDetails.aspx?id={0}&isP=true", this.mySession.Patient.Individual.ID), false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex.InnerException.IsNull() ? ex : ex.InnerException);
                ex.TraceErrorException();
            }
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = this.IsAdmin ? "Administration - Add Individual" : "Manage Patients - Add Patients";
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Administration");
                CurrentModule_Sub(string.Empty);
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
            cvIDNumber.ErrorMessage = this.ErrorMessage;
            return;
        }

        #region Input Validators

        protected void ValidateIndTypeCBs(object source, ServerValidateEventArgs args)
        {
            args.IsValid = cbAdministrator.Checked == true || cbProvider.Checked == true || cbTechnician.Checked == true;
        }

        protected bool ValidateIDNumber()
        {
            string IDType = this.IdNumberType;
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

        #region Interface Properties

        public List<KeyValueEntity> IndividualTypeLookupList
        {
            get
            {
                return Session["IndividualTypeLookup"] as List<KeyValueEntity>;
            }
            set
            {
                Session["IndividualTypeLookup"] = value;
            }
        }

        public List<LookupTableEntity> IDNumberTypeList
        {
            get
            {
                return ViewState["IDNumberTypeList"] as List<LookupTableEntity>;
            }
            set
            {
                try
                {
                    ViewState["IDNumberTypeList"] = value;
                    ddlIDNumberType.Items.Clear();
                    ddlIDNumberType.DataSource = value;
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

        public List<TheaterLocationCodeEntity> TheaterLocationCodeList
        {
            get
            {
                return ViewState["TheaterLocationCodeList"] as List<TheaterLocationCodeEntity>;
            }
            set
            {
                try
                {
                    ViewState["TheaterLocationCodeList"] = value;
                    ddlTheaterLocationCodes.Items.Clear();
                    ddlTheaterLocationCodes.DataSource = value;
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

        public List<BOSEntity> BosList
        {
            get
            {
                return ViewState["BosList"] as List<BOSEntity>;
            }
            set
            {
                try
                {
                    ViewState["BosList"] = value;
                    ddlBOS.Items.Clear();
                    ddlBOS.DataSource = value;
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

        public List<StatusEntity> StatusList
        {
            get
            {
                return ViewState["StatusList"] as List<StatusEntity>;
            }
            set
            {
                try
                {
                    ViewState["StatusList"] = value;
                    ddlStatusType.Items.Clear();
                    ddlStatusType.DataSource = value;
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

        public List<RankEntity> RankList
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

        public List<IdentificationNumbersEntity> AdditionalDmdcIdList
        {
            get
            {
                return ViewState["AdditionalDmdcId"] as List<IdentificationNumbersEntity>;
            }
            set
            {
                ViewState["AdditionalDmdcId"] = value;
            }
        }

        public List<IndividualEntity> SearchedPersonList
        {
            get { return ViewState["SearchedPersonList"] as List<IndividualEntity>; }
            set
            {
                ViewState["SearchedPersonList"] = value;
                this.gvSearch.DataSource = value
                    .Select(x => new { x.ID, x.LastName, x.FirstName, x.IDNumberDisplay, x.IDNumberTypeDescription, x.BOSDescription, x.StatusDescription, x.Rank, x.Gender, x.IsNewPatient })
                    .Distinct()
                    .ToArray();
                this.gvSearch.DataBind();
                this.gvSearch.SelectedIndex = -1;
            }
        }

        public List<SiteCodeEntity> SiteList
        {
            get { return ViewState["SiteList"] as List<SiteCodeEntity>; }
            set
            {
                try
                {
                    ViewState["SiteList"] = value;
                    ddlSite.Items.Clear();
                    ddlSite.DataSource = value;
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

        public string SiteCode
        {
            get { return ddlSite.SelectedValue; }
            set { ddlSite.SelectedValue = value; }
        }

        public string TheaterLocationCode
        {
            get
            {
                return this.ddlTheaterLocationCodes.SelectedValue;
            }
            set
            {
                this.ddlTheaterLocationCodes.SelectedValue = value;
            }
        }

        public string Comments
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get { return ViewState["ErrorMessage"].ToString(); }
            set
            {
                ViewState["ErrorMessage"] = value;
                if (!String.IsNullOrEmpty(value))
                {
                    rfvIDNumber.ErrorMessage = value;
                    rfvIDNumber.IsValid = false;
                    vsErrors.ShowSummary = true;
                }
            }
        }

        public string NewPage
        {
            set { Response.Redirect(value); }
        }

        public string Address1
        {
            get { return ViewState["Address1"].IsNull() ? String.Empty : ViewState["Address1"].ToString(); }
            set { ViewState["Address1"] = value; }
        }

        public string Address2
        {
            get { return ViewState["Address2"].IsNull() ? String.Empty : ViewState["Address2"].ToString(); }
            set { ViewState["Address2"] = value; }
        }

        public string City
        {
            get { return ViewState["City"].IsNull() ? String.Empty : ViewState["City"].ToString(); }
            set { ViewState["City"] = value; }
        }

        public string Country
        {
            get { return ViewState["Country"].IsNull() ? String.Empty : ViewState["Country"].ToString(); }
            set { ViewState["Country"] = value; }
        }

        public string State
        {
            get { return ViewState["State"].IsNull() ? String.Empty : ViewState["State"].ToString(); }
            set { ViewState["State"] = value; }
        }

        public string ZipCode
        {
            get { return ViewState["ZipCode"].IsNull() ? String.Empty : ViewState["ZipCode"].ToString(); }
            set { ViewState["ZipCode"] = value; }
        }

        public string UnitIdentificationCode
        {
            get { return ViewState["UnitIdentificationCode"].IsNull() ? String.Empty : ViewState["UnitIdentificationCode"].ToString(); }
            set { ViewState["UnitIdentificationCode"] = value; }
        }

        public string EmailAddress
        {
            get { return ViewState["EmailAddress"].IsNull() ? String.Empty : ViewState["EmailAddress"].ToString(); }
            set { ViewState["EmailAddress"] = value; }
        }

        public string IdNumber
        {
            get
            {
                return this.tbIDNumber.Text;
            }
            set
            {
                this.tbIDNumber.Text = value;
            }
        }

        public string IdNumberType
        {
            get
            {
                return this.ddlIDNumberType.SelectedValue;
            }
            set
            {
                this.ddlIDNumberType.SelectedValue = value;
            }
        }

        public string FirstName
        {
            get
            {
                return this.tbFirstName.Text;
            }
            set
            {
                this.tbFirstName.Text = value;
            }
        }

        public string MiddleName
        {
            get
            {
                return this.tbMiddleName.Text;
            }
            set
            {
                this.tbMiddleName.Text = value;
            }
        }

        public string LastName
        {
            get
            {
                return this.tbLastName.Text;
            }
            set
            {
                this.tbLastName.Text = value;
            }
        }

        public string Gender
        {
            get
            {
                return this.rblGender.SelectedValue;
            }
            set
            {
                this.rblGender.SelectedValue = value;
            }
        }

        public string BranchOfService
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

        public string StatusCategory
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

        public string Grade
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

        public string PhoneNumber
        {
            get { return ViewState["PhoneNumber"].IsNull() ? String.Empty : ViewState["PhoneNumber"].ToString(); }
            set { ViewState["PhoneNumber"] = value; }
        }

        public string Extension
        {
            get { return ViewState["Extension"].IsNull() ? String.Empty : ViewState["Extension"].ToString(); }
            set { ViewState["Extension"] = value; }
        }

        public bool IsActive
        {
            get
            {
                return true;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return Convert.ToBoolean(ViewState["IsAdmin"]);
            }
            set
            {
                ViewState["IsAdmin"] = value;
                this.divIndTypes.Visible = value;
                this.divSiteCodes.Visible = value;
            }
        }

        public bool IsPOC
        {
            get { return rblIsPOC.SelectedValue.ToBoolean(); }
            set { rblIsPOC.SelectedValue = value.ToString(); }
        }

        public bool IsAdminType
        {
            //get { return cblIndType.SelectedValue.Equals("O"); }
            get { return cbAdministrator.Checked.Equals(true); }
        }

        public bool IsTechType
        {
            //get { return cblIndType.SelectedValue.Equals("T"); }
            get { return cbTechnician.Checked.Equals(true); }
        }

        public bool IsProviderType
        {
            //get { return cblIndType.SelectedValue.Equals("P"); }
            get { return cbProvider.Checked.Equals(true); }
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

        public DateTime? DateOfBirth
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
                    tbDOB.Text = value.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    tbDOB.Text = string.Empty;
                }
            }
        }

        #region Unused Properties

        public string Address3
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

        public string AddressMessage
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

        public string EmailAddressMessage
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

        public string IdNumberMessage
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

        public string PhoneNumberMessage
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

        #endregion Unused Properties

        #endregion Interface Properties
    }
}