using SrtsWeb;
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
using System.Text;
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
    public partial class PatientDetails : PageBase, IPatientDetailsView, IComboAddView, ISiteMapResolver
    {
        private PatientDetailsPresenter _presenter;
        private ComboAddPresenter _presenterAddContactInfo;
        private List<String> userRoles = new List<String>();

        public PatientDetails()
        {
            _presenter = new PatientDetailsPresenter(this);
            _presenterAddContactInfo = new ComboAddPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            int patienid;
            if (int.TryParse(Request.QueryString["id"], out patienid))
            {
                mySession.SelectedPatientID = patienid;
            }

            if (!IsPostBack)
            {
                try
                {
                    if (this.mySession == null)
                        this.mySession = new SRTSSession();

                    this.mySession.Patient = new PatientEntity();
                    this.mySession.Patient.Individual = new IndividualEntity();

                    this.Controls.SetControlStateBtn(false);

                    if (Request.QueryString["examadd"] != null)
                    {
                        if (Request.QueryString["examadd"].ToString().Equals("1"))
                            ScriptManager.RegisterStartupScript(this, typeof(Page), "Toggle", "DoToggle('divExamsGrid', 'ExamArrow');", true);
                    }

                    Boolean isNew = false;
                    Boolean.TryParse(Request.QueryString["newP"], out isNew);

                    litContentTop_Title_Right.Text = string.Format("{0} - {1}", mySession.MySite.SiteName, mySession.MyClinicCode);
                    CurrentModule("Manage Patients");
                    CurrentModule_Sub(string.Empty);

                    mySession.ReturnURL = "PatientDetails.aspx";
                }
                catch (NullReferenceException)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                _presenter.InitView();
                _presenterAddContactInfo.InitView();

                this.divPersonalInfo_edit.Visible = false;

                userRoles = Roles.GetRolesForUser().ToList().ConvertAll(x => x.ToLower());

                if (userRoles.Any(x => x.Contains("clinic")))
                {
                    // If the user is in any clinic role
                    // Enable the buttons needed to modify the patient contact information
                    btnEditPersonalInfo.Enabled = true;
                    btnReturnToSearch.Enabled = true;
                    btnAddIDNumber.Enabled = true;
                    btnSaveIDNumber.Enabled = true;
                    //btnCancelAddIDNumber.Enabled = true;
                    btnAddMailAddress.Enabled = true;
                    btnSaveAddress.Enabled = true;
                    //btnCancelAddress.Enabled = true;
                    btnAddPhoneNumber.Enabled = true;
                    btnSavePhoneNumber.Enabled = true;
                    //btnCancelAddPhone.Enabled = true;
                    btnAddEmailAddress.Enabled = true;
                    btnSaveEmailAddress.Enabled = true;
                    //btnCancelAddEmail.Enabled = true;

                    // Enable the buttons on the Patient Edit page to allow saving new patient data
                    Button btnPatientEditCancel = (Button)modPatientEdit.FindControl("btnCancel");
                    btnPatientEditCancel.Enabled = true;
                    Button btnPatientEditSave = (Button)modPatientEdit.FindControl("btnUpdateRecord");
                    btnPatientEditSave.Enabled = true;
                }

                // If the user is in any lab role then only enable the cancel buttons
                if (userRoles.Any(x => x.Contains("lab")))
                {
                    //btnCancelAddIDNumber.Enabled = true;
                    //btnCancelAddress.Enabled = true;
                    //btnCancelAddPhone.Enabled = true;
                    //btnCancelAddEmail.Enabled = true;

                    // Hide the link buttons in the content sub menu
                    //this.lnkAddPatient.Visible = false;
                    //this.lnkDispenseOrder.Visible = false;
                    //this.lnkOrderCheckin.Visible = false;
                }
            }
        }

        #region Page Setup

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Patients/ManagePatients.aspx/search", "Manage Patients Search");
            child.ParentNode = parent;
            SiteMapNode child2 = new SiteMapNode(e.Provider, "3", "~/SrtsWebClinic/Patients/PatientDetails.aspx", "Patients Details");
            child2.ParentNode = child;
            return child2;
        }

        protected void BuildUserInterface()
        {
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
            try
            {
                litContentTop_Title_Right.Text = string.Empty;
                //litPatientNameHeader_sub.Text = string.Format("{0}", mySession.Patient.Individual.NameFMiL);
            }
            catch (NullReferenceException)
            {
            }
        }

        protected string Status(Boolean status)
        {
            if (status == true)
            {
                return "Active";
            }
            else
            {
                return "Not Active";
            }
        }

        #endregion Page Setup

        #region Patient View
        protected void btnSave(object sender, EventArgs e)
        {
            try
            {
                // divContactMessage
                var msg = String.Empty;
                string savefunction = string.Empty;

                if (object.ReferenceEquals(sender.GetType(), typeof(Button)))
                    savefunction = ((Button)sender).CommandArgument.ToString();

                switch (savefunction)
                {
                    #region SAVE
                    case "idnumber":
                        if (_presenterAddContactInfo.SaveIDNumbers())
                        {
                            _presenter.InitView();
                            _presenter.BindIdentificationNumbers();
                            msg = "Successfully saved new ID number.";
                        }
                        else
                        {
                            this.Message = IDNumberMessage;
                            var cv = new CustomValidator();
                            cv.ValidationGroup = "idnumb";
                            cv.ErrorMessage = IDNumberMessage;
                            cv.IsValid = false;
                            return;
                        }
                        break;

                    case "address":
                        if (_presenterAddContactInfo.SaveAddress())
                        {
                            _presenter.InitView();
                            _presenter.BindAddresses();
                            msg = "Successfully saved new address.";
                        }
                        break;

                    case "phonenumber":
                        if (_presenterAddContactInfo.SavePhone())
                        {
                            _presenter.InitView();
                            _presenter.BindPhoneNumbers();
                            msg = "Successfully saved new phone number.";
                        }
                        break;

                    case "email":
                        if (_presenterAddContactInfo.SaveEmail())
                        {
                            _presenter.InitView();
                            _presenter.BindEmail();
                            msg = "Successfully saved new email address.";
                        }
                        break;
                    #endregion

                    #region EDIT
                    case "idnumber_edit":
                        IdentificationNumbersEntity id = new IdentificationNumbersEntity();
                        id.ID = Convert.ToInt32(mySession.TempID);
                        id.IDNumber = tbIDNumber.Text;
                        id.IndividualID = mySession.Patient.Individual.ID;
                        id.IDNumberType = ddlIDNumberType.SelectedValue;
                        id.IDNumberTypeDescription = ddlIDNumberType.SelectedItem.Text;
                        id.ModifiedBy = mySession.MyUserID;
                        id.IsActive = chkMakeDefaultIDNumber.Checked;
                        id.DateLastModified = DateTime.Now;

                        IdentificationNumbersEntity idOrig = (IdentificationNumbersEntity)Session["idEntity"];

                        if (!IsIdEntityDirty(idOrig, id)) { idOrig = null; break; }
                        if (_presenter.UpdateIDNumbers(id, IsIdStatusUpdateOnly(idOrig, id)))
                        {
                            _presenter.InitView();
                            _presenter.BindIdentificationNumbers();
                            msg = "Successfully updated ID number.";
                        }
                        break;

                    case "address_edit":
                        AddressEntity address = new AddressEntity();
                        address.ID = Convert.ToInt32(mySession.TempID);
                        address.Address1 = tbAddress1.Text;
                        address.Address2 = tbAddress2.Text;
                        address.City = tbCity.Text;
                        address.State = ddlState.SelectedValue.ToString();
                        address.ZipCode = tbZipCode.Text.ToZipCodeDisplay();
                        address.AddressType = ddlAddressType.SelectedValue.ToString();
                        address.Country = ddlCountry.SelectedValue.ToString();
                        address.ModifiedBy = mySession.MyUserID;
                        address.IsActive = chkMakeDefaultAddress.Checked;
                        address.IndividualID = mySession.Patient.Individual.ID;
                        address.DateLastModified = DateTime.Now;
                        address.UIC = tb2UIC.Text;

                        AddressEntity addyOriginal = (AddressEntity)Session["original"];

                        if (!IsAddressEntityDirty(addyOriginal, address)) { addyOriginal = null; break; }
                        if (_presenter.UpdateAddresses(address))
                        {
                            _presenter.InitView();
                            _presenter.BindAddresses();
                            msg = "Successfully updated address.";
                        }
                        break;

                    case "phonenumber_edit":
                        PhoneNumberEntity phonenumber = new PhoneNumberEntity();
                        phonenumber.ID = Convert.ToInt32(mySession.TempID);
                        phonenumber.AreaCode = this.AreaCode;
                        phonenumber.PhoneNumber = tbPhoneNumber.Text;
                        phonenumber.Extension = tbExtension.Text;
                        phonenumber.PhoneNumberType = ddlPhoneType.SelectedValue.ToString();
                        phonenumber.ModifiedBy = mySession.MyUserID;
                        phonenumber.IsActive = chkMakeDefaultPhone.Checked;
                        phonenumber.IndividualID = mySession.Patient.Individual.ID;
                        phonenumber.DateLastModified = DateTime.Now;

                        PhoneNumberEntity phOriginal = (PhoneNumberEntity)Session["original"];

                        if (!IsPhoneEntityDirty(phOriginal, phonenumber)) { phOriginal = null; break; }
                        if (_presenter.UpdatePhones(phonenumber))
                        {
                            _presenter.InitView();
                            _presenter.BindPhoneNumbers();
                            msg = "Successfully updated phone number.";
                        }
                        break;

                    case "email_edit":
                        EMailAddressEntity emailaddress = new EMailAddressEntity();
                        emailaddress.ID = Convert.ToInt32(mySession.TempID);
                        emailaddress.EMailAddress = tbEMailAddress.Text;
                        emailaddress.EMailType = ddlEMailType.SelectedValue.ToString();
                        emailaddress.ModifiedBy = mySession.MyUserID;
                        emailaddress.IsActive = chkMakeDefaultEmail.Checked;
                        emailaddress.IndividualID = mySession.Patient.Individual.ID;
                        emailaddress.DateLastModified = DateTime.Now;

                        EMailAddressEntity emailOriginal = (EMailAddressEntity)Session["original"];

                        if (!IsEmailEntityDirty(emailOriginal, emailaddress)) { emailOriginal = null; break; }
                        if (_presenter.UpdateEmail(emailaddress))
                        {
                            _presenter.InitView();
                            _presenter.BindEmail();
                            msg = "Successfully updated email address.";
                        }
                        break;

                    #endregion

                    default:
                        break;
                }

                if (String.IsNullOrEmpty(msg)) return;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Confirm", String.Format("Confirm('{0}', 'divContactMessage', false, false);", msg), true);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

            }
        }

        private Boolean IsIdStatusUpdateOnly(IdentificationNumbersEntity original, IdentificationNumbersEntity compare)
        {
            if (!original.IDNumber.Equals(compare.IDNumber)) return false;
            if (!original.IDNumberType.Equals(compare.IDNumberType)) return false;
            if (!original.IsActive.Equals(compare.IsActive)) return true;
            return false;
        }

        private Boolean IsIdEntityDirty(IdentificationNumbersEntity original, IdentificationNumbersEntity compare)
        {
            if (!original.IndividualID.Equals(compare.IndividualID)) return true;
            if (!original.IDNumber.Equals(compare.IDNumber)) return true;
            if (!original.IDNumberType.Equals(compare.IDNumberType)) return true;
            if (!original.IsActive.Equals(compare.IsActive)) return true;
            return false;
        }
        private Boolean IsEmailEntityDirty(EMailAddressEntity original, EMailAddressEntity compare)
        {
            if (!original.IndividualID.Equals(compare.IndividualID)) return true;
            if (!original.EMailAddress.ToLower().Equals(compare.EMailAddress.ToLower())) return true;
            if (!original.EMailType.ToLower().Equals(compare.EMailType.ToLower())) return true;
            if (!original.IsActive.Equals(compare.IsActive)) return true;
            return false;
        }
        private Boolean IsPhoneEntityDirty(PhoneNumberEntity original, PhoneNumberEntity compare)
        {
            if (!original.PhoneNumberType.ToLower().Equals(compare.PhoneNumberType.ToLower())) return true;
            if (!original.PhoneNumber.Equals(compare.PhoneNumber)) return true;
            if (!original.Extension.ToLower().Equals(compare.Extension.ToLower())) return true;
            if (!original.IsActive.Equals(compare.IsActive)) return true;
            return false;
        }
        private Boolean IsAddressEntityDirty(AddressEntity original, AddressEntity compare)
        {
            if (!original.AddressType.ToLower().Equals(compare.AddressType.ToLower())) return true;
            if (!original.Address1.ToLower().Equals(compare.Address1.ToLower())) return true;
            if (!original.Address2.ToLower().Equals(compare.Address2.ToLower())) return true;
            if (!original.City.ToLower().Equals(compare.City.ToLower())) return true;
            if (!original.State.ToLower().Equals(compare.State.ToLower())) return true;
            if (!original.Country.ToLower().Equals(compare.Country.ToLower())) return true;
            if (!original.ZipCode.ToLower().Equals(compare.ZipCode.ToLower())) return true;
            if (!original.UIC.ToLower().Equals(compare.UIC.ToLower())) return true;
            if (!original.IsActive.Equals(compare.IsActive)) return true;
            return false;
        }

        protected void btnEditPatientInfo_Click(object sender, EventArgs e)
        {
            mySession.TempID = mySession.Patient.Individual.ID;
            divPersonalInformation.Visible = false;
            divPersonalInfo_edit.Visible = true;
        }

        protected void btnReturnToSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManagePatients.aspx/search");
        }

        protected void gvIDNumbers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#F3E7D7'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
                e.Row.Attributes.Add("style", "cursor:pointer;");
                e.Row.Attributes["onClick"] = ClientScript.GetPostBackClientHyperlink(gvIDNumbers, "Select$" + DataBinder.Eval(e.Row.DataItem, "ID"), true);
            }
        }
        private void EditIdNumberModal(Int32 idNumberId)
        {
            chkMakeDefaultIDNumber.Visible = true;
            lblIDNumberStatus.Visible = true;

            mySession.TempID = idNumberId;
            IEnumerable<IdentificationNumbersEntity> ineSelected =
                                from ine in mySession.Patient.IDNumbers
                                where ine.ID == idNumberId
                                select ine;

            IdentificationNumbersEntity idNumber = new IdentificationNumbersEntity();
            idNumber = ineSelected.FirstOrDefault();

            Session.Add("idEntity", idNumber);

            btnSaveIDNumber.CommandArgument = "idnumber_edit";
            tbIDNumber.Text = idNumber.IDNumber;

            if (ddlIDNumberType.Items.FindByValue(idNumber.IDNumberType.ToString()) != null)
            { ddlIDNumberType.SelectedValue = idNumber.IDNumberType.ToString(); }
            else { ddlIDNumberType.SelectedIndex = 0; }

            this.chkMakeDefaultIDNumber.Checked = idNumber.IsActive;
            this.chkMakeDefaultIDNumber.Enabled = !idNumber.IsActive;
            this.ddlIDNumberType.Enabled = !idNumber.IsActive;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "DoIdNumberDialog(false);", true);
        }
        protected void gvIDNumbers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (Roles.GetRolesForUser().ToList().ConvertAll(x => x.ToLower()).Any(x => x.Contains("lab"))) return;
            EditIdNumberModal(Convert.ToInt32(e.CommandArgument));
        }

        protected void gvEMail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#F3E7D7'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
                e.Row.Attributes.Add("style", "cursor:pointer;");
                e.Row.Attributes["onClick"] = ClientScript.GetPostBackClientHyperlink(gvEMail, "Select$" + DataBinder.Eval(e.Row.DataItem, "ID"), true);
            }
        }
        private void EditEmailModal(Int32 emailId)
        {
            chkMakeDefaultEmail.Visible = true;
            lblMakeDefaultEmail.Visible = true;

            mySession.TempID = emailId;

            IEnumerable<EMailAddressEntity> emaSelected =
                                from ema in mySession.Patient.EMailAddresses
                                where ema.ID == emailId
                                select ema;

            EMailAddressEntity emailAddress = new EMailAddressEntity();
            emailAddress = emaSelected.FirstOrDefault();

            Session.Add("original", emailAddress);

            btnSaveEmailAddress.CommandArgument = "email_edit";
            tbEMailAddress.Text = emailAddress.EMailAddress;

            if (ddlEMailType.Items.FindByValue(emailAddress.EMailType.ToString()) != null)
                ddlEMailType.SelectedValue = emailAddress.EMailType.ToString();
            else
                ddlEMailType.SelectedIndex = 0;

            this.chkMakeDefaultEmail.Checked = emailAddress.IsActive;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "DoEmailDialog(false);", true);
        }
        protected void gvEMail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            EditEmailModal(Convert.ToInt32(e.CommandArgument));
        }

        protected void gvPhones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#F3E7D7'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
                e.Row.Attributes.Add("style", "cursor:pointer;");
                e.Row.Attributes["onClick"] = ClientScript.GetPostBackClientHyperlink(gvPhones, "Select$" + DataBinder.Eval(e.Row.DataItem, "ID"), true);
            }
        }
        private void EditPhonesModal(Int32 phoneId)
        {
            chkMakeDefaultPhone.Visible = true;
            lblMakeDefaultPhone.Visible = true;

            mySession.TempID = phoneId;
            IEnumerable<PhoneNumberEntity> phnSelected =
                                from phn in mySession.Patient.PhoneNumbers
                                where phn.ID == phoneId
                                select phn;
            PhoneNumberEntity phoneNumber = new PhoneNumberEntity();
            phoneNumber = phnSelected.FirstOrDefault();

            Session.Add("original", phoneNumber);

            btnSavePhoneNumber.CommandArgument = "phonenumber_edit";
            tbPhoneNumber.Text = phoneNumber.PhoneNumber;
            tbExtension.Text = phoneNumber.Extension;

            if (ddlPhoneType.Items.FindByValue(phoneNumber.PhoneNumberType) != null)
                ddlPhoneType.SelectedValue = phoneNumber.PhoneNumberType;
            else
                ddlPhoneType.SelectedIndex = 0;

            this.chkMakeDefaultPhone.Checked = phoneNumber.IsActive;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "DoPhoneDialog(false);", true);
        }
        protected void gvPhones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            EditPhonesModal(Convert.ToInt32(e.CommandArgument));
        }

        protected void gvAddresses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#F3E7D7'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
                e.Row.Attributes.Add("style", "cursor:pointer;");
                e.Row.Attributes["onClick"] = ClientScript.GetPostBackClientHyperlink(gvAddresses, "Select$" + DataBinder.Eval(e.Row.DataItem, "ID"), true);
            }
        }
        private void EditAddressModal(Int32 addressId)
        {
            chkMakeDefaultAddress.Visible = true;
            lblMakeDefaultAddress.Visible = true;

            mySession.TempID = addressId;
            IEnumerable<AddressEntity> addressSelected =
                                from address in mySession.Patient.Addresses
                                where address.ID == addressId
                                select address;
            AddressEntity mailingAddress = new AddressEntity();
            mailingAddress = addressSelected.FirstOrDefault();

            Session.Add("original", mailingAddress);

            btnSaveAddress.CommandArgument = "address_edit";
            tbAddress1.Text = mailingAddress.Address1;
            tbAddress2.Text = mailingAddress.Address2;
            tbCity.Text = mailingAddress.City;
            tbZipCode.Text = mailingAddress.ZipCode.ToZipCodeDisplay();
            tb2UIC.Text = string.IsNullOrEmpty(mailingAddress.UIC) ? string.Empty : mailingAddress.UIC;

            if (ddlState.Items.FindByValue(mailingAddress.State.ToString()) != null)
            { ddlState.SelectedValue = mailingAddress.State.ToString(); }
            else
            { ddlState.SelectedIndex = 0; }

            if (ddlCountry.Items.FindByValue(mailingAddress.Country.ToString()) != null)
            { ddlCountry.SelectedValue = mailingAddress.Country.ToString(); }
            else
            { ddlCountry.SelectedIndex = 0; }

            if (ddlAddressType.Items.FindByValue(mailingAddress.AddressType.ToString()) != null)
            { ddlAddressType.SelectedValue = mailingAddress.AddressType.ToString(); }
            else { ddlAddressType.SelectedIndex = 0; }

            this.chkMakeDefaultAddress.Checked = mailingAddress.IsActive;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "DoAddressDialog(false);", true);
        }
        protected void gvAddresses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            EditAddressModal(Convert.ToInt32(e.CommandArgument));
        }

        protected void ValidateIDNumber(object source, ServerValidateEventArgs args)
        {
            string IDType = IDNumberType;
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

                default:

                    break;
            }

            if (args.IsValid = args.Value.ValidateIDNumLength(len))
            {
                if (args.IsValid = args.Value.ValidateIDNumFormat())
                {
                }
                //else
                //{
                //    cvIDNumber.ErrorMessage = "Invalid characters in ID Number";
                //}
                }
            //else
            //{
            //    cvIDNumber.ErrorMessage = string.Format("{0} Number must be {1} digits", type, len);
            //}
            }

        protected void lnbOrderManagement_Click(object sender, EventArgs e)
        {
            var dto = new PatientOrderDTO();
            dto.IndividualId = this.mySession.SelectedPatientID;
            dto.Demographic = this.mySession.Patient.Individual.Demographic;
            dto.PatientSiteCode = this.mySession.Patient.Individual.SiteCodeID;
            Session.Add("PatientOrderDTO", dto);
            Response.Redirect("~/SrtsOrderManagement/OrderManagement.aspx", true);
        }
        protected void OpenDialog_Command(object sender, CommandEventArgs e)
        {
            var f = String.Empty;
            switch (e.CommandArgument.ToString().ToLower())
            {
                case "email":
                    ClearEmail();
                    f = "DoEmailDialog(true);";
                    break;
                case "phone":
                    ClearPhone();
                    f = "DoPhoneDialog(true);";
                    break;
                case "address":
                    ClearAddress();
                    f = "DoAddressDialog(true);";
                    break;
                case "id":
                    ClearIdNums();
                    f = "DoIdNumberDialog(true);";
                    break;
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", f, true);
        }

        private void ClearAddress()
        {
            this.tbAddress1.Text = String.Empty;
            this.tbAddress2.Text = String.Empty;
            this.tbCity.Text = String.Empty;
            this.tbZipCode.Text = String.Empty;
            this.ddlState.SelectedIndex = 0;
            this.ddlCountry.SelectedIndex = 0;
            this.ddlAddressType.SelectedIndex = 0;
            this.tb2UIC.Text = String.Empty;
            this.chkMakeDefaultAddress.Checked = false;
        }
        private void ClearIdNums()
        {
            this.tbIDNumber.Text = String.Empty;
            this.ddlIDNumberType.SelectedIndex = 0;
            this.chkMakeDefaultIDNumber.Checked = false;
            this.chkMakeDefaultIDNumber.Enabled = true;
            this.ddlIDNumberType.Enabled = true;
        }
        private void ClearEmail()
        {
            this.tbEMailAddress.Text = String.Empty;
            this.ddlEMailType.SelectedIndex = 0;
            this.chkMakeDefaultEmail.Checked = false;
        }
        private void ClearPhone()
        {
            this.tbPhoneNumber.Text = String.Empty;
            this.tbExtension.Text = String.Empty;
            this.ddlPhoneType.SelectedIndex = 0;
            this.chkMakeDefaultPhone.Checked = false;
        }
        #endregion Patient View

        #region Accessors_Patient View

        private List<LookupTableEntity> _states;

        public List<LookupTableEntity> States
        {
            get { return _states; }
            set { _states = value; }
        }

        private List<LookupTableEntity> _countries;

        public List<LookupTableEntity> Countries
        {
            get { return _countries; }
            set { _countries = value; }
        }

        private IdentificationNumbersEntity _idUpdate;

        public IdentificationNumbersEntity IDUpdate
        {
            get { return _idUpdate; }
            set { _idUpdate = value; }
        }

        private List<AddressEntity> Addresses;

        public List<AddressEntity> AddressesBind
        {
            get { return Addresses; }
            set
            {
                gvAddresses.DataSource = value;
                gvAddresses.DataBind();
                Addresses = value;
            }
        }

        private List<IdentificationNumbersEntity> _IDNumbersBind;

        public List<IdentificationNumbersEntity> IDNumbersBind
        {
            get { return _IDNumbersBind; }
            set
            {
                _IDNumbersBind = value;
                gvIDNumbers.DataSource = _IDNumbersBind;
                gvIDNumbers.DataBind();
            }
        }

        public List<EMailAddressEntity> EmailAddressesBind
        {
            set
            {
                gvEMail.DataSource = value;
                gvEMail.DataBind();
            }
        }

        public List<PhoneNumberEntity> PhoneNumbersBind
        {
            set
            {
                gvPhones.DataSource = value;
                gvPhones.DataBind();
            }
        }

        public List<ExamEntity> ExamsBind
        {
            set { }
            //{
            //    gvExams.DataSource = value;
            //    gvExams.DataBind();
            //}
        }

        public DataSet OrdersBind
        {
            set { }
            //{
            //    _ordersBind = value;
            //    DataTable dt = _ordersBind.Tables[0];
            //    dt.DefaultView.Sort = "DateCreated DESC";
            //    dt = dt.DefaultView.ToTable();
            //    gvOrders.DataSource = dt;
            //    gvOrders.DataBind();
            //}
        }

        public List<PrescriptionEntity> PrescriptionsBind
        {
            set{}
            //    {
            //        gvPrescriptions.DataSource = value;
            //        gvPrescriptions.DataBind();
            //    }
        }

        #endregion Accessors_Patient View

        #region IDNumber Accessors

        public string IDNumberMessage
        {
            get;
            set;
        }

        public List<LookupTableEntity> IDTypeDDL
        {
            set
            {
                try
                {
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

        public string IDNumber
        {
            get { return tbIDNumber.Text; }
            set { tbIDNumber.Text = value; }
        }

        public string IDNumberType
        {
            get { return ddlIDNumberType.SelectedValue; }
            set { ddlIDNumberType.SelectedValue = value; }
        }

        #endregion IDNumber Accessors

        #region Address Accessors

        public string Address1
        {
            get
            {
                return tbAddress1.Text;
            }
            set
            {
                tbAddress1.Text = value;
            }
        }

        public string Address2
        {
            get
            {
                return tbAddress2.Text;
            }
            set
            {
                tbAddress2.Text = value;
            }
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

        public List<LookupTableEntity> CountryData
        {
            get;
            set;
        }

        public List<LookupTableEntity> CountryDDL
        {
            set
            {
                try
                {
                    ddlCountry.Items.Clear();
                    ddlCountry.DataTextField = "Text";
                    ddlCountry.DataValueField = "Value";
                    ddlCountry.DataSource = value;
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

        public string CountrySelected
        {
            get
            {
                return ddlCountry.SelectedValue;
            }
            set
            {
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

        public string StateSelected
        {
            get
            {
                return ddlState.SelectedValue;
            }
            set
            {
                ddlState.SelectedValue = value;
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
                    ddlAddressType.Items.Insert(0, new ListItem("-Select-", "X"));
                    ddlAddressType.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlAddressType.SelectedIndex = -1;
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

        public string ZipCode
        {
            get
            {
                return tbZipCode.Text;
            }
            set
            {
                tbZipCode.Text = value.ToZipCodeDisplay();
            }
        }

        public DataTable UICDDL
        {
            get;
            set;
        }

        public string UICSelected
        {
            get { return tb2UIC.Text; }

            set { tb2UIC.Text = value; }
        }

        public string UIC
        {
            get { return tb2UIC.Text; }
            set { tb2UIC.Text = value; }
        }

        public string AddrMessage
        {
            get;
            set;
        }

        #endregion Address Accessors

        #region Email Accessors

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
                    ddlEMailType.Items.Insert(0, new ListItem("-Select-", "X"));
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

        #endregion Email Accessors

        #region Phone Accessors

        public string PhoneMessage
        {
            get;
            set;
        }

        /// <summary>
        /// This property is blank for the time being so we don't have to modify the SP or any presenters, interfaces, and/or repositories.
        /// </summary>
        public string AreaCode
        {
            get { return String.Empty; }
            set { var a = value; }
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
                    ddlPhoneType.Items.Insert(0, new ListItem("-Select-", "X"));
                    this.ddlPhoneType.SelectedIndex = 0;
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

        #endregion Phone Accessors

        #region Misc Accessors

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        #endregion Misc Accessors

        public List<OrderPriorityEntity> PriorityList
        {
            set { throw new NotImplementedException(); }
        }
        public string IndividualTypeMessage
        {
            get;
            set;
        }
        public string LastFocOrderNumber
        {
            get;
            set;
        }
    }
}