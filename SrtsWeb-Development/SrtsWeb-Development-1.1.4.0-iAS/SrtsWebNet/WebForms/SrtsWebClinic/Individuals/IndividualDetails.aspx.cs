using SrtsWeb;
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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWebClinic.Individuals
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class IndividualDetails : PageBase, IIndividualDetailsView, IComboAddView, ISiteMapResolver
    {
        private IndividualDetailsPresenter _presenter;
        private ComboAddPresenter _presenterAddContactInfo;

        public IndividualDetails()
        {
            _presenter = new IndividualDetailsPresenter(this);
            _presenterAddContactInfo = new ComboAddPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }
            if (!IsPostBack)
            {
                if (Session["CONFIRM"] != null && !String.IsNullOrEmpty(Session["CONFIRM"].ToString()))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Confirm", String.Format("Confirm('{0}', 'statusMessage', false);", Session["CONFIRM"].ToString()), true);
                    Session.Remove("CONFIRM");
                }
                else if (Session["FAIL"] != null && !String.IsNullOrEmpty(Session["FAIL"].ToString()))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Fail", String.Format("Confirm('{0}', 'statusMessage', true);", Session["FAIL"].ToString()), true);
                    Session.Remove("FAIL");
                }

                try
                {
                    int patienid;
                    if (int.TryParse(Request.QueryString["id"], out patienid))
                    {
                        mySession.SelectedPatientID = patienid;
                    }

                    //litContentTop_Title_Right.Text = string.Format("{0} - {1}", mySession.MySite.SiteName, mySession.MyClinicCode);
                    CurrentModule("Manage Individuals");
                    CurrentModule_Sub(string.Empty);
                    GetSelectedPageOption("personal");
                    mySession.ReturnURL = "IndividualDetails.aspx";
                }
                catch (NullReferenceException)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                _presenter.InitView();
                _presenterAddContactInfo.InitView();
                //litPatientNameHeader.Text = string.Format("{0}", mySession.Patient.Individual.NameFMiL);
                int tabid;
                if (int.TryParse(Request.QueryString["tab"], out tabid))
                {
                    //tbcPatientInformation.ActiveTabIndex = tabid;
                }
            }
        }

        #region Page Setup

        //protected void ActiveTabChanged(object sender, EventArgs e)
        //{
        //    int tabid = tbcPatientInformation.ActiveTabIndex;
        //    if (tabid >= 0)
        //    {
        //        switch (tabid)
        //        {
        //            case 0:
        //                GetSelectedPageOption("personal");
        //                break;

        //            case 1:
        //                GetSelectedPageOption("contact");
        //                break;
        //        }
        //    }
        //}

        public void GetSelectedPageOption(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (Request.PathInfo.Length != 0)
                {
                    path = Request.PathInfo.Substring(1);
                }
            }
            if (mySession.SelectedPatientID != 0)
            {
                _presenter.InitView();
                BuildUserInterface();
            }

            switch (path)
            {
                case "personal":
                    CurrentModule_Sub("- Individual Information");
                    divPersonalInformation.Visible = true;
                    divPersonalInfo_edit.Visible = false;
                    //tbcPatientInformation.ActiveTabIndex = 0;
                    btnEditPersonalInfo.Focus();
                    break;

                case "personal_edit":
                    CurrentModule_Sub("- Edit Individual Information");
                    //tbcPatientInformation.ActiveTabIndex = 0;
                    break;

                case "contact":
                    CurrentModule_Sub("- Individual Contact Information");
                    //tbcPatientInformation.ActiveTabIndex = 1;
                    ClientScript.RegisterStartupScript(GetType(), "identification", "ShowContactContent('divIDNumber','lidivIDNumber', 'Identification Numbers');", true);
                    break;

                default:
                    CurrentModule_Sub("- Individual Information");
                    //tbcPatientInformation.ActiveTabIndex = 0;
                    break;
            }
            BuildPageTitle();
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
            }
            catch (NullReferenceException)
            {
                CurrentModule("Search Individuals");
                CurrentModule_Sub(string.Empty);
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Individuals/IndividualSearch.aspx", "Manage Individuals Search");
            child.ParentNode = parent;
            SiteMapNode child2 = new SiteMapNode(e.Provider, "3", "~/SrtsWebClinic/Individuals/IndividualDetails.aspx", "Individual Details");
            child2.ParentNode = child;
            return child2;
        }

        protected void BuildUserInterface()
        {
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
            //try
            //{
            //    litContentTop_Title_Right.Text = string.Format("{0} - DOB: {1}",
            //    mySession.Patient.Individual.NameLFMi, mySession.Patient.Individual.DateOfBirth != null ?
            //    mySession.Patient.Individual.DateOfBirth.Value.ToShortDateString() : string.Empty);
            //    litPatientNameHeader.Text = string.Format("{0}", mySession.Patient.Individual.NameFMiL);
            //    litPatientNameHeader_sub.Text = string.Format("{0}", mySession.Patient.Individual.NameFMiL);
            //}
            //catch (NullReferenceException)
            //{
            //}
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

        #region Individual View

        protected void btnUpdateIndTypes_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var confirm = String.Empty;
            var fail = String.Empty;

            int IndividualId = mySession.SelectedPatientID;
            var ModifiedBy = mySession.MyUserID;

            if (_presenter.UpdateIndividualTypes(IndividualId, ModifiedBy))
            {
                confirm = "Individual types successfully updated";
            }
            else
            {
                fail = "Error updating individual types";
            }

            if (!String.IsNullOrEmpty(confirm))
            {
                Session["CONFIRM"] = confirm;
            }
            else if (!String.IsNullOrEmpty(fail))
            {
                Session["FAIL"] = fail;
            }
            Response.Redirect(this.Request.Url.ToString());
        }


        public List<KeyValueEntity> IndividualTypeLookup
        {
            set
            {
                Session["IndividualTypeLookup"] = value;
            }
            get { return Session["IndividualTypeLookup"] as List<KeyValueEntity>; }
        }

        protected void AddPatientInfo_Click(object sender, EventArgs e)
        {
            divAddEmailAddress.Visible = false;
            divAddMailingAddress.Visible = false;
            divAddPatientIdNumber.Visible = false;
            divAddPhoneNumber.Visible = false;
            divAddIndType.Visible = false;

            divUpdatePatientInformation.Visible = false;
            divAddFunction.Visible = false;
            divConfirmation.Visible = false;
            litConfirmationText.Text = string.Empty;

            string selection = "-Select-";

            string addfunction = string.Empty;
            if (object.ReferenceEquals(sender.GetType(), typeof(Button)))
            {
                addfunction = ((Button)sender).CommandArgument.ToString();
            }
            switch (addfunction)
            {
                case "idnumber":
                    divUpdatePatientInformation.Visible = true;
                    divAddFunction.Visible = true;
                    divAddPatientIdNumber.Visible = true;

                    chkMakeDefaultIDNumber.Visible = false;
                    lblIDNumberStatus.Visible = false;

                    btnSaveIDNumber.CommandName = "idnumber";
                    btnSaveIDNumber.CommandArgument = "idnumber";
                    tbIDNumber.Text = string.Empty;
                    if (!ddlIDNumberType.Items[0].Text.StartsWith("-"))
                    {
                        ddlIDNumberType.Items.Insert(0, selection);
                    }
                    ddlIDNumberType.SelectedIndex = 0;
                    litActionHeader.Text = "Add a New Identification Number";
                    litActionInstruction.Text = "Add a new identification number using the form on the right.  <br /><br />Click 'Save' to complete changes or 'Cancel' to exit and return to individual detail.";
                    this.ddlIDNumberType.Enabled = true;
                    mdlAddContactInfo_ID.Show();
                    break;

                case "address":
                    tbAddress1.Text = string.Empty;
                    tbAddress2.Text = string.Empty;
                    tbCity.Text = string.Empty;
                    tbZipCode.Text = string.Empty;
                    tb2UIC.Text = string.Empty;
                    if (!ddlState.Items[0].Text.StartsWith("-"))
                    {
                        ddlState.Items.Insert(0, selection);
                    }
                    if (!ddlAddressType.Items[0].Text.StartsWith("-"))
                    {
                        ddlAddressType.Items.Insert(0, selection);
                    }
                    if (!ddlCountry.Items[0].Text.StartsWith("-"))
                    {
                        ddlCountry.Items.Insert(0, selection);
                    }
                    ddlState.SelectedIndex = 0;
                    ddlCountry.SelectedIndex = 0;
                    ddlAddressType.SelectedIndex = 0;

                    chkMakeDefaultAddress.Visible = false;
                    lblMakeDefaultAddress.Visible = false;

                    divUpdatePatientInformation.Visible = true;
                    divAddFunction.Visible = true;
                    divAddMailingAddress.Visible = true;
                    btnSaveAddress.CommandArgument = addfunction;
                    litActionHeader.Text = "Add a New Mailing Address";
                    litActionInstruction.Text = "Add a new address using the form on the right.  <br /><br />Click 'Save' to complete changes or 'Cancel' to exit and return to individual detail.";
                    mdlAddMailingAddress.Show();
                    break;

                case "phonenumber":
                    if (!ddlPhoneType.Items[0].Text.StartsWith("-"))
                    {
                        ddlPhoneType.Items.Insert(0, selection);
                    }
                    ddlPhoneType.SelectedIndex = 0;
                    tbPhoneNumber.Text = string.Empty;
                    tbExtension.Text = string.Empty;

                    chkMakeDefaultPhone.Visible = false;
                    lblMakeDefaultPhone.Visible = false;

                    divUpdatePatientInformation.Visible = true;
                    divAddFunction.Visible = true;
                    divAddPhoneNumber.Visible = true;
                    btnSavePhoneNumber.CommandArgument = addfunction;
                    litActionHeader.Text = "Add a New Phone Number";
                    litActionInstruction.Text = "Add a new phone number using the form on the right.  <br /><br />Click 'Save' to complete changes or 'Cancel' to exit and return to individual detail.";
                    mdlAddPhoneNumber.Show();
                    break;

                case "email":
                    tbEMailAddress.Text = string.Empty;
                    if (!ddlEMailType.Items[0].Text.StartsWith("-"))
                    {
                        ddlEMailType.Items.Insert(0, selection);
                    }
                    ddlEMailType.SelectedIndex = 0;

                    chkMakeDefaultEmail.Visible = false;
                    lblMakeDefaultEmail.Visible = false;

                    divUpdatePatientInformation.Visible = true;
                    divAddFunction.Visible = true;
                    divAddEmailAddress.Visible = true;
                    btnSaveEmailAddress.CommandArgument = addfunction;
                    litActionHeader.Text = "Add a New Email Address";
                    litActionInstruction.Text = "Add a new email address using the form on the right.  <br /><br />Click 'Save' to complete changes or 'Cancel' to exit and return to individual detail.";
                    mdlAddEmailAddress.Show();
                    break;

                case "individualtype":
                    if (!ddlIndType.Items[0].Text.StartsWith("-"))
                    {
                        ddlIndType.Items.Insert(0, selection);
                    }
                    ddlIndType.SelectedIndex = 0;

                    chkMakeDefaultIndType.Visible = false;
                    lblMakeDefaultIndType.Visible = false;

                    divUpdatePatientInformation.Visible = true;
                    divAddFunction.Visible = true;
                    divAddIndType.Visible = true;
                    btnSaveIndType.CommandArgument = addfunction;
                    litActionHeader.Text = "Add a New Individual Type";
                    litActionInstruction.Text = "Add a new individual type using the form on the right.  <br /><br />Click 'Save' to complete changes or 'Cancel' to exit and return to individual detail.";
                    mdlAddIndType.Show();
                    break;
            }
        }

        protected void btnSave(object sender, EventArgs e)
        {
            divUpdatePatientInformation.Visible = false;
            divConfirmation.Visible = false;
            divAddFunction.Visible = false;

            string savefunction = string.Empty;
            if (object.ReferenceEquals(sender.GetType(), typeof(Button)))
            {
                savefunction = ((Button)sender).CommandArgument.ToString();
            }
            {
                switch (savefunction)
                {
                    case "idnumber":
                        if (_presenterAddContactInfo.SaveIDNumbers())
                        {
                            _presenter.InitView();
                            _presenter.BindIdentificationNumbers();
                            //gvShowContent_ContactInfo(divIDNumber.ClientID);
                            ConfirmManageContactInfo(savefunction);
                        }
                        else
                        {
                            this.Message = IDNumberMessage;
                            ErrorManageContactInfo(savefunction);
                        }
                        break;

                    case "address":
                        if (_presenterAddContactInfo.SaveAddress())
                        {
                            _presenter.InitView();
                            _presenter.BindAddresses();
                            //gvShowContent_ContactInfo(divAddress.ClientID);
                            ConfirmManageContactInfo(savefunction);
                        }
                        break;

                    case "phonenumber":
                        if (_presenterAddContactInfo.SavePhone())
                        {
                            _presenter.InitView();
                            _presenter.BindPhoneNumbers();
                            //gvShowContent_ContactInfo(divPhone.ClientID);
                            ConfirmManageContactInfo(savefunction);
                        }
                        break;

                    case "email":
                        if (_presenterAddContactInfo.SaveEmail())
                        {
                            _presenter.InitView();
                            _presenter.BindEmail();
                            //gvShowContent_ContactInfo(divEmail.ClientID);
                            ConfirmManageContactInfo(savefunction);
                        }
                        break;

                    //case "individualtype":
                    //    if (_presenterAddContactInfo.SaveIndividualType())
                    //    {
                    //        _presenter.InitView();
                    //        _presenter.BindIndividualType();
                    //        //gvShowContent_ContactInfo(divIndType.ClientID);
                    //        ConfirmManageContactInfo(savefunction);
                    //    }
                    //    break;

                    case "idnumber_edit":
                        IdentificationNumbersEntity id = new IdentificationNumbersEntity();
                        id.ID = Convert.ToInt32(mySession.TempID);
                        id.IDNumber = tbIDNumber.Text;
                        id.IndividualID = mySession.Patient.Individual.ID;
                        id.IDNumberType = ddlIDNumberType.SelectedValue;
                        id.ModifiedBy = mySession.MyUserID;
                        id.IsActive = chkMakeDefaultIDNumber.Checked;
                        id.IndividualID = mySession.SelectedPatientID;
                        id.DateLastModified = DateTime.Now;

                        IdentificationNumbersEntity idOrig = (IdentificationNumbersEntity)ViewState["idEntity"];

                        if (_presenter.UpdateIDNumbers(id, IsStatusUpdateOnly(idOrig, id)))
                        {
                            _presenter.InitView();
                            _presenter.BindIdentificationNumbers();
                            //gvShowContent_ContactInfo(divIDNumber.ClientID.ToString());
                            ConfirmManageContactInfo(savefunction);
                        }
                        else
                        {
                            ErrorManageContactInfo(savefunction);
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
                        address.IndividualID = mySession.SelectedPatientID;
                        address.DateLastModified = DateTime.Now;
                        address.UIC = tb2UIC.Text;

                        AddressEntity addyOrig = (AddressEntity)ViewState["original"];

                        if (!IsAddressEntityDirty(addyOrig, address)) { addyOrig = null; break; }
                        if (_presenter.UpdateAddresses(address))
                        {
                            _presenter.InitView();
                            _presenter.BindAddresses();
                            //gvShowContent_ContactInfo(divAddress.ClientID.ToString());
                            ConfirmManageContactInfo(savefunction);
                        }
                        break;

                    case "phonenumber_edit":
                        PhoneNumberEntity phonenumber = new PhoneNumberEntity();
                        phonenumber.ID = Convert.ToInt32(mySession.TempID);
                        phonenumber.PhoneNumber = tbPhoneNumber.Text;
                        phonenumber.Extension = tbExtension.Text;
                        phonenumber.PhoneNumberType = ddlPhoneType.SelectedValue.ToString();
                        phonenumber.ModifiedBy = mySession.MyUserID;
                        phonenumber.IsActive = chkMakeDefaultPhone.Checked;
                        phonenumber.IndividualID = mySession.SelectedPatientID;
                        phonenumber.DateLastModified = DateTime.Now;

                        PhoneNumberEntity phOrig = (PhoneNumberEntity)ViewState["original"];

                        if (!IsPhoneEntityDirty(phOrig, phonenumber)) { phOrig = null; break; }
                        if (_presenter.UpdatePhones(phonenumber))
                        {
                            _presenter.InitView();
                            _presenter.BindPhoneNumbers();
                            //gvShowContent_ContactInfo(divPhone.ClientID.ToString());
                            ConfirmManageContactInfo(savefunction);
                        }
                        break;

                    case "email_edit":
                        EMailAddressEntity emailaddress = new EMailAddressEntity();
                        emailaddress.ID = Convert.ToInt32(mySession.TempID);
                        emailaddress.EMailAddress = tbEMailAddress.Text;
                        emailaddress.EMailType = ddlEMailType.SelectedValue.ToString();
                        emailaddress.ModifiedBy = mySession.MyUserID;
                        emailaddress.IsActive = chkMakeDefaultEmail.Checked;
                        emailaddress.IndividualID = mySession.SelectedPatientID;
                        emailaddress.DateLastModified = DateTime.Now;

                        EMailAddressEntity emailOrig = (EMailAddressEntity)ViewState["original"];

                        if (!IsEmailEntityDirty(emailOrig, emailaddress)) { emailOrig = null; break; }
                        if (_presenter.UpdateEmail(emailaddress))
                        {
                            _presenter.InitView();
                            _presenter.BindEmail();
                            //gvShowContent_ContactInfo(divEmail.ClientID);
                            ConfirmManageContactInfo(savefunction);
                        }
                        break;

                    //case "individualtype_edit":
                    //    var ite = new IndividualTypeEntity()
                    //    {
                    //        ID = Convert.ToInt32(mySession.TempID),
                    //        IndividualId = mySession.SelectedPatientID,
                    //        IsActive = chkMakeDefaultIndType.Checked,
                    //        ModifiedBy = mySession.MyUserID,
                    //        TypeDescription = IndividualTypeDescription,
                    //        TypeId = Convert.ToInt32(IndividualTypeSelected)
                    //    };

                    //    var typeOrig = (IndividualTypeEntity)ViewState["original"];
                    //    if (!IsIndividualTypeEntityDirty(typeOrig, ite)) { typeOrig = null; break; }
                    //    //if (_presenter.UpdateIndividualType(ite))
                    //    {
                    //        _presenter.InitView();
                    //        _presenter.BindIndividualType();
                    //        //gvShowContent_ContactInfo(divIndType.ClientID);
                    //        ConfirmManageContactInfo(savefunction);
                    //    }
                    //    break;

                    default:
                        //gvShowContent_ContactInfo(divIDNumber.ClientID);
                        break;
                }
            }
        }

        protected void btnCancel(object sender, EventArgs e)
        {
            divUpdatePatientInformation.Visible = false;
            divAddFunction.Visible = false;
            string cancelfunction = string.Empty;
            if (object.ReferenceEquals(sender.GetType(), typeof(Button)))
            {
                cancelfunction = ((Button)sender).CommandArgument;
            }
            switch (cancelfunction)
            {
                case "idnumber":
                case "idnumber_edit":
                    mdlAddContactInfo_ID.Hide();
                    //gvShowContent_ContactInfo(divIDNumber.ClientID);
                    break;

                case "address":
                case "address_edit":
                    mdlAddMailingAddress.Hide();
                    //gvShowContent_ContactInfo(divAddress.ClientID);
                    break;

                case "phonenumber":
                case "phonenumber_edit":
                    mdlAddPhoneNumber.Hide();
                    //gvShowContent_ContactInfo(divPhone.ClientID);
                    break;

                case "email":
                case "email_edit":
                    mdlAddEmailAddress.Hide();
                    //gvShowContent_ContactInfo(divEmail.ClientID);
                    break;

                //case "individualtype":
                //case "individualtype_edit":
                //    mdlAddIndType.Hide();
                //    //gvShowContent_ContactInfo(divIndType.ClientID);
                //    break;
            }
        }

        protected void ConfirmManageContactInfo(string confirmfunction)
        {
            btnYes.Visible = false;
            btnOK.Visible = false;
            btnNo.Visible = false;

            divUpdatePatientInformation.Visible = true;
            divAddFunction.Visible = false;
            divConfirmation.Visible = true;
            divConfirmButtons.Visible = true;
            litConfirmationText.Text = string.Empty;
            litActionHeader.Text = string.Empty;

            switch (confirmfunction)
            {
                case "idnumber":
                    btnYes.Visible = true;
                    btnNo.Visible = true;
                    litConfirmationText.Text = "Identification number added successfully!<br /><br />  Would you like to add another?<br /><br />";
                    break;

                case "address":
                    btnYes.Visible = true;
                    btnNo.Visible = true;
                    litConfirmationText.Text = "Address added successfully!<br /><br />  Would you like to add another?<br /><br />";
                    break;

                case "phonenumber":
                    btnYes.Visible = true;
                    btnNo.Visible = true;
                    litConfirmationText.Text = "Phone number added successfully!<br /><br />  Would you like to add another?<br /><br />";
                    break;

                case "email":
                    btnYes.Visible = true;
                    btnNo.Visible = true;
                    litConfirmationText.Text = "Email address added successfully!<br /><br />  Would you like to add another?<br /><br />";
                    break;

                //case "individualtype":
                //    btnYes.Visible = true;
                //    btnNo.Visible = true;
                //    litConfirmationText.Text = "Individual type added successfully!<br /><br />  Would you like to add another?<br /><br />";
                //    break;

                case "idnumber_edit":
                    litConfirmationText.Text = "Identification number updated successfully!<br /><br /> ";
                    btnOK.Visible = true;
                    confirmfunction = "idnumber_OK";
                    break;

                case "address_edit":
                    litConfirmationText.Text = "Address updated successfully!<br /><br /> ";
                    btnOK.Visible = true;
                    break;

                case "phonenumber_edit":
                    litConfirmationText.Text = "Phone number updated successfully!<br /><br /> ";
                    btnOK.Visible = true;
                    break;

                case "email_edit":
                    litConfirmationText.Text = "Email address updated successfully!<br /><br /> ";
                    btnOK.Visible = true;
                    break;

                //case "individualtype_edit":
                //    litConfirmationText.Text = "Individual type updated successfully!<br /><br /> ";
                //    btnOK.Visible = true;
                //    break;
            }
            btnOK.CommandArgument = confirmfunction;
            btnYes.CommandArgument = confirmfunction;
            btnNo.CommandArgument = confirmfunction;
            mdlAddContactConfirm.Show();
        }

        protected void ErrorManageContactInfo(String confirmFunction)
        {
            btnYes.Visible = false;
            btnOK.Visible = false;
            btnNo.Visible = false;

            divUpdatePatientInformation.Visible = true;
            divAddFunction.Visible = false;
            divConfirmation.Visible = true;
            divConfirmButtons.Visible = true;
            litConfirmationText.Text = string.Empty;
            litActionHeader.Text = string.Empty;

            btnYes.Visible = true;
            btnNo.Visible = true;
            litConfirmationText.Text = String.Format("{0}<br /><br />  Would you like to try again?<br /><br />", Message);

            btnOK.CommandArgument = confirmFunction;
            btnYes.CommandArgument = confirmFunction;
            btnNo.CommandArgument = confirmFunction;
            mdlAddContactConfirm.Show();
        }

        private Boolean IsStatusUpdateOnly(IdentificationNumbersEntity idOriginal, IdentificationNumbersEntity idCompare)
        {
            if (!idOriginal.IDNumber.Equals(idCompare.IDNumber)) return false;
            if (!idOriginal.IDNumberType.Equals(idCompare.IDNumberType)) return false;
            if (!idOriginal.IsActive.Equals(idCompare.IsActive)) return true;
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
            if (!original.AreaCode.Equals(compare.AreaCode)) return true;
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

        private Boolean IsIndividualTypeEntityDirty(IndividualTypeEntity original, IndividualTypeEntity compare)
        {
            if (!original.TypeId.Equals(compare.TypeId)) return true;
            if (!original.IsActive.Equals(compare.IsActive)) return true;
            return false;
        }

        protected void Confirmation_Click(object sender, EventArgs e)
        {
            string confirmfunction = ((Button)sender).CommandArgument.ToString();
            {
                switch (confirmfunction)
                {
                    case "idnumber":
                    case "address":
                    case "phonenumber":
                    case "email":
                    case "individualtype":
                        AddPatientInfo_Click(sender, e);
                        break;

                    case "idnumber_edit":
                        EditIdNumberModal(mySession.TempID);
                        break;

                    default:
                        btnCancel(sender, e);
                        break;
                }
            }
        }

        protected void btnEditPatientInfo_Click(object sender, EventArgs e)
        {
            mySession.TempID = mySession.Patient.Individual.ID;
            divPersonalInformation.Visible = false;
            divPersonalInfo_edit.Visible = true;
            GetSelectedPageOption("personal_edit");
        }

        protected void btnReturnToSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("IndividualSearch.aspx");
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

        private void EditIdNumberModal(Int32 idNumberID)
        {
            divAddEmailAddress.Visible = false;
            divAddMailingAddress.Visible = false;
            divAddPatientIdNumber.Visible = false;
            divAddPhoneNumber.Visible = false;
            divAddIndType.Visible = false;

            divUpdatePatientInformation.Visible = false;
            divAddFunction.Visible = false;
            divConfirmation.Visible = false;
            litConfirmationText.Text = string.Empty;

            chkMakeDefaultIDNumber.Visible = true;
            lblIDNumberStatus.Visible = true;

            mySession.TempID = idNumberID;
            IEnumerable<IdentificationNumbersEntity> ineSelected =
                                from ine in mySession.Patient.IDNumbers
                                where ine.ID == idNumberID
                                select ine;
            IdentificationNumbersEntity idNumber = new IdentificationNumbersEntity();
            idNumber = ineSelected.FirstOrDefault();

            ViewState.Add("idEntity", idNumber);

            btnSaveIDNumber.CommandArgument = "idnumber_edit";
            tbIDNumber.Text = idNumber.IDNumber;
            litActionHeader.Text = "Edit Individual Identification Number";
            litActionInstruction.Text = "Edit identification number information on the right.  <br /><br />Click 'Save' when you are done or 'Cancel' to exit and return to individual search.";
            litConfirmationText.Text = string.Empty;
            if (ddlIDNumberType.Items.FindByValue(idNumber.IDNumberType.ToString()) != null)
            { ddlIDNumberType.SelectedValue = idNumber.IDNumberType.ToString(); }
            else { ddlIDNumberType.SelectedIndex = 0; }
            divUpdatePatientInformation.Visible = true;
            divAddFunction.Visible = true;
            divAddPatientIdNumber.Visible = true;
            divConfirmButtons.Visible = false;
            this.chkMakeDefaultIDNumber.Checked = idNumber.IsActive;
            this.chkMakeDefaultIDNumber.Enabled = !idNumber.IsActive;
            this.ddlIDNumberType.Enabled = !idNumber.IsActive;
            this.mdlEditWindow.Show();
        }

        protected void gvIDNumbers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
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
            divAddEmailAddress.Visible = false;
            divAddMailingAddress.Visible = false;
            divAddPatientIdNumber.Visible = false;
            divAddPhoneNumber.Visible = false;
            divAddIndType.Visible = false;

            divUpdatePatientInformation.Visible = false;
            divAddFunction.Visible = false;
            divConfirmation.Visible = false;
            litConfirmationText.Text = string.Empty;

            chkMakeDefaultEmail.Visible = true;
            lblMakeDefaultEmail.Visible = true;

            mySession.TempID = emailId;
            IEnumerable<EMailAddressEntity> emaSelected =
                                from ema in mySession.Patient.EMailAddresses
                                where ema.ID == emailId
                                select ema;
            EMailAddressEntity emailAddress = new EMailAddressEntity();

            emailAddress = emaSelected.FirstOrDefault();

            ViewState.Add("original", emailAddress);

            btnSaveEmailAddress.CommandArgument = "email_edit";
            tbEMailAddress.Text = emailAddress.EMailAddress;
            litActionHeader.Text = "Edit Individual Email Address";

            if (ddlEMailType.Items.FindByValue(emailAddress.EMailType.ToString()) != null)
                ddlEMailType.SelectedValue = emailAddress.EMailType.ToString();
            else
                ddlEMailType.SelectedIndex = 0;

            litActionInstruction.Text = "Edit email address information on the right.  <br /><br />Click 'Save' when you are done or 'Cancel' to exit and return to individual detail.";
            divUpdatePatientInformation.Visible = true;
            divAddFunction.Visible = true;
            divAddEmailAddress.Visible = true;
            divConfirmButtons.Visible = false;
            this.chkMakeDefaultEmail.Checked = emailAddress.IsActive;
            this.mdlEditWindow.Show();
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

        private void EditPhoneModal(Int32 phoneId)
        {
            divAddEmailAddress.Visible = false;
            divAddMailingAddress.Visible = false;
            divAddPatientIdNumber.Visible = false;
            divAddPhoneNumber.Visible = false;
            divAddIndType.Visible = false;

            divUpdatePatientInformation.Visible = false;
            divAddFunction.Visible = false;
            divConfirmation.Visible = false;
            litConfirmationText.Text = string.Empty;

            chkMakeDefaultPhone.Visible = true;
            lblMakeDefaultPhone.Visible = true;

            mySession.TempID = phoneId;
            IEnumerable<PhoneNumberEntity> phnSelected =
                                from phn in mySession.Patient.PhoneNumbers
                                where phn.ID == phoneId
                                select phn;
            PhoneNumberEntity phoneNumber = new PhoneNumberEntity();

            phoneNumber = phnSelected.FirstOrDefault();

            ViewState.Add("original", phoneNumber);

            btnSavePhoneNumber.CommandArgument = "phonenumber_edit";
            tbPhoneNumber.Text = phoneNumber.PhoneNumber;
            tbExtension.Text = phoneNumber.Extension;
            litActionHeader.Text = "Edit Individual Phone Number";
            if (ddlPhoneType.Items.FindByValue(ddlPhoneType.SelectedValue.ToString()) != null)
                ddlPhoneType.SelectedValue = ddlPhoneType.SelectedValue.ToString();
            else
                ddlPhoneType.SelectedIndex = 0;

            litActionInstruction.Text = "Edit phone number information on the right.  <br /><br />Click 'Save' when you are done or 'Cancel' to exit and return to individual detail.";
            litConfirmationText.Text = string.Empty;
            divUpdatePatientInformation.Visible = true;
            divAddFunction.Visible = true;
            divAddPhoneNumber.Visible = true;
            divConfirmButtons.Visible = false;
            this.chkMakeDefaultPhone.Checked = phoneNumber.IsActive;
            this.mdlEditWindow.Show();
        }

        protected void gvPhones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            EditPhoneModal(Convert.ToInt32(e.CommandArgument));
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
            divAddEmailAddress.Visible = false;
            divAddMailingAddress.Visible = false;
            divAddPatientIdNumber.Visible = false;
            divAddPhoneNumber.Visible = false;
            divAddIndType.Visible = false;

            divUpdatePatientInformation.Visible = false;
            divAddFunction.Visible = false;
            divConfirmation.Visible = false;
            litConfirmationText.Text = string.Empty;

            chkMakeDefaultAddress.Visible = true;
            lblMakeDefaultAddress.Visible = true;

            mySession.TempID = addressId;

            IEnumerable<AddressEntity> addressSelected =
                                from address in mySession.Patient.Addresses
                                where address.ID == addressId
                                select address;
            AddressEntity mailingAddress = new AddressEntity();

            mailingAddress = addressSelected.FirstOrDefault();

            ViewState.Add("original", mailingAddress);

            btnSaveAddress.CommandArgument = "address_edit";
            tbAddress1.Text = mailingAddress.Address1;
            tbAddress2.Text = mailingAddress.Address2;
            tbCity.Text = mailingAddress.City;
            tbZipCode.Text = mailingAddress.ZipCode.ToZipCodeDisplay();
            tb2UIC.Text = string.IsNullOrEmpty(mailingAddress.UIC) ? string.Empty : mailingAddress.UIC;
            ddlState.SelectedValue = mailingAddress.State.ToString();
            ddlCountry.SelectedValue = mailingAddress.Country.ToString();

            if (ddlAddressType.Items.FindByValue(mailingAddress.AddressType.ToString()) != null)
                ddlAddressType.SelectedValue = mailingAddress.AddressType.ToString();
            else
                ddlAddressType.SelectedIndex = 0;

            litActionHeader.Text = "Edit Individual Mailing Address";
            litActionInstruction.Text = "Edit mailing address information on the right.  <br /><br />Click 'Save' when you are done or 'Cancel' to exit and return to individual detail.";
            litConfirmationText.Text = string.Empty;
            divUpdatePatientInformation.Visible = true;
            divAddFunction.Visible = true;
            divAddMailingAddress.Visible = true;
            divConfirmButtons.Visible = false;
            this.chkMakeDefaultAddress.Checked = mailingAddress.IsActive;
            this.mdlEditWindow.Show();
        }

        protected void gvAddresses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            EditAddressModal(Convert.ToInt32(e.CommandArgument));
        }

        //protected void gvIndType_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#F3E7D7'");
        //        e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''");
        //        e.Row.Attributes.Add("style", "cursor:pointer;");
        //        e.Row.Attributes["onClick"] = ClientScript.GetPostBackClientHyperlink(gvIndType, "Select$" + DataBinder.Eval(e.Row.DataItem, "ID"), true);
        //    }
        //}

        private void EditIndividualTypeModal(Int32 individualTypeId)
        {
            divAddEmailAddress.Visible = false;
            divAddMailingAddress.Visible = false;
            divAddPatientIdNumber.Visible = false;
            divAddPhoneNumber.Visible = false;
            divAddIndType.Visible = false;

            divUpdatePatientInformation.Visible = false;
            divAddFunction.Visible = false;
            divConfirmation.Visible = false;
            litConfirmationText.Text = string.Empty;

            chkMakeDefaultIndType.Visible = true;
            lblMakeDefaultIndType.Visible = true;

            mySession.TempID = individualTypeId;

            IEnumerable<IndividualTypeEntity> indTypeSelected =
                                from indType in mySession.Patient.IndividualTypes
                                where indType.ID == individualTypeId
                                select indType;
            IndividualTypeEntity individualType = new IndividualTypeEntity();

            individualType = indTypeSelected.FirstOrDefault();

            ViewState.Add("original", individualType);

            btnSaveIndType.CommandArgument = "individualtype_edit";
            ddlIndType.SelectedValue = individualType.TypeId.ToString();

            litActionHeader.Text = "Edit Individual Type";
            litActionInstruction.Text = "Edit individual type information on the right.  <br /><br />Click 'Save' when you are done or 'Cancel' to exit and return to individual detail.";
            litConfirmationText.Text = string.Empty;
            divUpdatePatientInformation.Visible = true;
            divAddFunction.Visible = true;
            divAddIndType.Visible = true;
            divConfirmButtons.Visible = false;
            this.chkMakeDefaultIndType.Checked = individualType.IsActive;
            this.mdlEditWindow.Show();
        }

        protected void gvIndType_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            EditIndividualTypeModal(Convert.ToInt32(e.CommandArgument));
        }

        //protected void gvShowContent_ContactInfo(string div)
        //{
        //    divIDNumber.Style.Add("display", "none");
        //    divAddress.Style.Add("display", "none");
        //    divPhone.Style.Add("display", "none");
        //    divEmail.Style.Add("display", "none");
        //    divIndType.Style.Add("display", "none");

        //    //lidivIDNumber.Attributes.Clear();
        //    //lidivAddress.Attributes.Clear();
        //    //lidivPhone.Attributes.Clear();
        //    //lidivEmail.Attributes.Clear();
        //    //lidivIndType.Attributes.Clear();

        //    switch (div)
        //    {
        //        case "divIDNumber":
        //            divIDNumber.Style.Add("display", "block");
        //            //lidivIDNumber.Attributes.Add("class", "current");
        //            containertop_header.InnerHtml = "Identification Numbers";
        //            return;

        //        case "divAddress":
        //            divAddress.Style.Add("display", "block");
        //            //lidivAddress.Attributes.Add("class", "current");
        //            containertop_header.InnerHtml = "Addresses";
        //            return;

        //        case "divPhone":
        //            divPhone.Style.Add("display", "block");
        //            //lidivPhone.Attributes.Add("class", "current");
        //            containertop_header.InnerHtml = "Phone Numbers";
        //            return;

        //        case "divEmail":
        //            divEmail.Style.Add("display", "block");
        //            //lidivEmail.Attributes.Add("class", "current");
        //            containertop_header.InnerHtml = "Email Addresses";
        //            return;

        //        case "divIndType":
        //            divIndType.Style.Add("display", "block");
        //            //lidivIndType.Attributes.Add("class", "current");
        //            containertop_header.InnerHtml = "Individual Types";
        //            return;
        //    }
        //}

        #endregion Individual View

        #region Accessors_Individual View

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

        public List<IdentificationNumbersEntity> IDNumbersBind
        {
            set
            {
                gvIDNumbers.DataSource = value;
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
                    cbPatient.Checked = value.Any(x => x.TypeDescription.ToLower() == "patient" && x.IsActive == true);
                    cbProvider.Checked = value.Any(x => x.TypeDescription.ToLower() == "provider" && x.IsActive == true);
                    cbTechnician.Checked = value.Any(x => x.TypeDescription.ToLower() == "technician" && x.IsActive == true);
                }
            }
        }

        #endregion Accessors_Individual View

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
                ddlIDNumberType.Items.Clear();
                ddlIDNumberType.DataSource = value;
                ddlIDNumberType.DataTextField = "Text";
                ddlIDNumberType.DataValueField = "Value";
                ddlIDNumberType.DataBind();
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

        public string CountrySelected
        {
            get
            {
                return ddlCountry.SelectedValue;
            }
            set
            {
                ddlCountry.SelectedValue = value;
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
                ddlCountry.Items.Clear();
                ddlCountry.DataSource = value;
                ddlCountry.DataTextField = "Text";
                ddlCountry.DataValueField = "Value";
                ddlCountry.DataBind();
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

        public List<LookupTableEntity> StateDDL
        {
            set
            {
                ddlState.Items.Clear();
                ddlState.DataSource = value;
                ddlState.DataTextField = "ValueTextCombo";
                ddlState.DataValueField = "Value";
                ddlState.DataBind();
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
                ddlAddressType.Items.Clear();
                ddlAddressType.DataSource = value;
                ddlAddressType.DataTextField = "Text";
                ddlAddressType.DataValueField = "Value";
                ddlAddressType.DataBind();
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
            get;
            set;
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
                ddlEMailType.Items.Clear();
                ddlEMailType.DataSource = value;
                ddlEMailType.DataTextField = "Text";
                ddlEMailType.DataValueField = "Value";
                ddlEMailType.DataBind();
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

        public string AreaCode
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
                ddlPhoneType.Items.Clear();
                ddlPhoneType.DataSource = value;
                ddlPhoneType.DataTextField = "Text";
                ddlPhoneType.DataValueField = "Value";
                ddlPhoneType.DataBind();
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

        #region Individual Type Accessors

        public string IndividualTypeMessage
        {
            get;
            set;
        }

        public String IndividualTypeSelected
        {
            get
            {
                return ddlIndType.SelectedValue;
            }
            set
            {
                ddlIndType.SelectedValue = value;
            }
        }

        public String IndividualTypeDescription
        {
            get
            {
                return this.ddlIndType.SelectedItem.Text;
            }
        }

        public List<LookupTableEntity> IndividualTypeDDL
        {
            set
            {
                this.ddlIndType.Items.Clear();
                this.ddlIndType.DataSource = value;
                ddlIndType.DataTextField = "Text";
                ddlIndType.DataValueField = "Id";
                this.ddlIndType.DataBind();
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

        public bool IsPatient
        {
            get { return cbPatient.Checked.Equals(true); }
        }

        public bool IsProvider
        {
            get { return cbProvider.Checked.Equals(true); }
        }

        #endregion Individual Type Accessors

        private int _addressID;

        public int AddressID
        {
            get { return _addressID; }
            set { _addressID = value; }
        }

        private List<LookupTableEntity> _addressTypeData;

        public List<LookupTableEntity> AddressTypeData
        {
            get { return _addressTypeData; }
            set
            {
                _addressTypeData = value;
                ddlAddressType.Items.Clear();
                ddlAddressType.DataSource = _addressTypeData;
                ddlAddressType.DataTextField = "Text";
                ddlAddressType.DataValueField = "Value";
                ddlAddressType.DataBind();
            }
        }

        private List<LookupTableEntity> _stateData;

        public List<LookupTableEntity> StateData
        {
            get { return _stateData; }
            set
            {
                _stateData = value;
                ddlState.Items.Clear();
                ddlState.DataSource = _stateData;
                ddlState.DataTextField = "ValueTextCombo";
                ddlState.DataValueField = "Value";
                ddlState.DataBind();
            }
        }

        #region Misc Accessors

        public int PhoneID
        {
            get { return Convert.ToInt32(ViewState["PhoneID"]); }
            set
            {
                ViewState.Add("PhoneID", value);
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        #endregion Misc Accessors

        #region Validators
        protected void ValidateIndTypeCBs(object source, ServerValidateEventArgs args)
        {
            args.IsValid = cbAdministrator.Checked == true || cbProvider.Checked == true || cbTechnician.Checked == true || cbPatient.Checked == true;
        }
        #endregion
    }
}