using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Person;
using SrtsWeb.Views.Person;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace SrtsWeb.SrtsPerson
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
    public partial class PersonDetails : PageBase, IPersonDetailsView, ISiteMapResolver
    {
        private PersonDetailsPresenter _presenter;
        private List<String> userRoles = new List<String>();
        private AddressEntity edAddress = new AddressEntity();
        private AddressEntity VerifiedAddress = new AddressEntity();
        private IDemographicXMLHelper dxHelper;
        private String selectedGrade;
        private Boolean canEditProvider;

        private Boolean IsPatientView
        {
            get { return ViewState["isP"].ToBoolean(); }
            set { ViewState["isP"] = value; }
        }

        public PersonDetails()
        {
            _presenter = new PersonDetailsPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_Page_Load", mySession.MyUserID))
#endif
                {
                    int patienid;
                    if (int.TryParse(Request.QueryString["id"], out patienid))
                    {
                        mySession.SelectedPatientID = patienid;
                    }

                    #region PersonalData/ServiceData

                    var script = String.Format("function getlblRemainingID() {{ var lblID = '{0}'; return lblID; }} function gettbCommentID() {{ var tbID = '{1}'; return tbID; }}", this.lblRemaining.ClientID, this.tbComments.ClientID);

                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "TextLenVal", script, true);

                    ceEAD.StartDate = DateTime.Today;
                    ceEAD.EndDate = DateTime.Today.AddYears(2);
                    ceDOB.StartDate = DateTime.Today.AddYears(-100);
                    ceDOB.EndDate = DateTime.Today.AddDays(-1);

                    #endregion PersonalData/ServiceData

                    userRoles = Roles.GetRolesForUser().ToList().ConvertAll(x => x.ToLower());

                    canEditProvider = (userRoles.Any(x => x == "clinicadmin") || userRoles.Any(x => x == "mgmtenterprise"));

                    // Set DeersSuccess to true and let the click methods set it to false
                    DeersSuccess = true;

                    if (!IsPostBack)
                    {
                        try
                        {
                            if (this.mySession == null)
                                this.mySession = new SRTSSession();

                            this.IsPatientView = Request.QueryString["isP"].ToBoolean();

                            this.divOrderMgmt.Visible = this.IsPatientView;
                            this.divIndType.Visible = !this.IsPatientView;

                            this.mySession.Patient = new PatientEntity();
                            this.mySession.Patient.Individual = new IndividualEntity();

                            litContentTop_Title_Right.Text = string.Format("{0} - {1}", mySession.MySite.SiteName, mySession.MyClinicCode);

                            _presenter.InitView(this.IsPatientView);

                            SetPageVeiw();

                            CurrentModule_Sub(string.Empty);
                            BuildUserInterface();

                            mySession.ReturnURL = "PersonDetails.aspx";
                        }
                        catch (NullReferenceException)
                        {
                            FormsAuthentication.RedirectToLoginPage();
                        }

                        #region PersonalData/ServiceData
                        ValidateAddress();
                        txtLastName.Text = string.Format("{0}", mySession.Patient.Individual.LastName);
                        tbFirstName.Text = string.Format("{0}", mySession.Patient.Individual.FirstName);
                        tbMiddleName.Text = string.Format("{0}", mySession.Patient.Individual.MiddleName);
                        tbDOB.Text = mySession.Patient.Individual.DateOfBirth != null ? mySession.Patient.Individual.DateOfBirth.Value.ToString("MM/dd/yyyy") : string.Empty;
                        rblGender.SelectedValue = mySession.Patient.Individual.Gender;
                        tbComments.Text = mySession.Patient.Individual.Comments;

                        dxHelper = new DemographicXMLHelper();
                        BoSType = dxHelper.GetALLBOS();
                        BOSTypeSelected = mySession.Patient.Individual.Demographic.ToBOSKey();
                        StatusType = dxHelper.GetStatusByBOS(BOSTypeSelected);
                        StatusTypeSelected = mySession.Patient.Individual.Demographic.ToPatientStatusKey();
                        RankType = dxHelper.GetRanksByBOSAndStatus(BOSTypeSelected, StatusTypeSelected);
                        RankTypeSelected = mySession.Patient.Individual.Demographic.ToRankKey();

                        #endregion PersonalData/ServiceData

                        var isEnabled = (userRoles.Any(x => x.StartsWith("clinic")) || userRoles.Any(x => x.Contains("admin")) || userRoles.Any(x => x.ToLower().StartsWith("mgmt")));


                        bSaveEmailAddress.Visible = isEnabled;
                        bSaveIdNumbers.Visible = isEnabled;
                        bSavePhoneNumber.Visible = isEnabled;
                        bSavePersonalServiceData.Visible = isEnabled;

                        if (isEnabled) return;

                        this.tbDss.Text = MaskIdNumber(this.tbDss.Text);
                        this.tbDin.Text = MaskIdNumber(this.tbDin.Text);
                        this.tbDbn.Text = MaskIdNumber(this.tbDbn.Text);
                        this.tbPin.Text = MaskIdNumber(this.tbPin.Text);
                    }
                    else
                    {
                        if (this.IsPatientView)
                        {
                            this.divAssignedSites.Visible = false;
                        }
                        else
                        {
                            this.divAssignedSites.Visible = IsProvider && canEditProvider;
                            this.cbProvider.Enabled = canEditProvider;
                        }
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        #region Page Setup

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = null, child2 = null;

            if (this.IsPatientView)
            {
                child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Patients/ManagePatients.aspx/search", "Manage Patients Search");
                child.ParentNode = parent;
                child2 = new SiteMapNode(e.Provider, "3", "~/SrtsWebClinic/Patients/PatientDetails.aspx", "Patients Details");
            }
            else
            {
                child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Individuals/IndividualSearch.aspx", "Manage Individuals Search");
                child.ParentNode = parent;
                child2 = new SiteMapNode(e.Provider, "3", "~/SrtsWebClinic/Individuals/IndividualDetails.aspx", "Individual Details");
            }

            child2.ParentNode = child;
            return child2;
        }

        protected void BuildUserInterface()
        {
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
            try
            {
                litContentTop_Title_Right.Text = string.Empty;
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

        protected void SetPageVeiw()
        {
            if (this.IsPatientView)
            {
                CurrentModule("Manage Patient");
                this.divAssignedSites.Visible = false;
                this.divPatientNameHeader.Attributes.Add("class", "patientnameheader_Patient");
                lnbPatientNameHeader.Text = string.Format("Current Patient:  {0} {1}", mySession.Patient.Individual.FirstName, mySession.Patient.Individual.LastName);
            }
            else
            {
                CurrentModule("Manage Individuals");
                this.divAssignedSites.Visible = IsProvider && canEditProvider;
                this.cbProvider.Enabled = canEditProvider;
                this.divPatientNameHeader.Attributes.Add("class", "patientnameheader_Individual");
                lnbPatientNameHeader.Text = string.Format("Current Individual:  {0} {1}", mySession.Patient.Individual.FirstName, mySession.Patient.Individual.LastName);
            }
        }


        #endregion Page Setup

        #region Page Events/Methods

        protected void lnbOrderManagement_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_lnbOrderManagement_Click", mySession.MyUserID))
#endif
                {
                    var dto = new PatientOrderDTO();
                    dto.IndividualId = this.mySession.SelectedPatientID;
                    dto.Demographic = this.mySession.Patient.Individual.Demographic;
                    dto.PatientSiteCode = this.mySession.Patient.Individual.SiteCodeID;
                    Session.Add("PatientOrderDTO", dto);
                    Session.Add("IsDtoRedirect", true);
                    Response.Redirect("~/WebForms/SrtsOrderManagement/OrderManagement.aspx", true);
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void btnUpdateIndTypes_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_btnUpdateIndTypes_Click", mySession.MyUserID))
#endif
                {
                    int IndividualId = mySession.SelectedPatientID;
                    var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;
                    var good = _presenter.UpdateIndividualTypes(IndividualId, ModifiedBy);

                    var m = String.Empty;
                    if (good)
                    {
                        m = "Individual types successfully updated";
                        this.IndividualTypeMessage = m;
                    }
                    else
                    {
                        m = "Error updating individual types";
                        this.IndividualTypeMessage = m;
                    }
                    LogEvent(String.Format("{1} by user {0} at {2}", mySession.MyUserID, m, DateTime.Now));

                    if (!String.IsNullOrEmpty(this.IndividualTypeMessage))
                    {
                        if (good)
                        {
                            ShowConfirmDialog(this.IndividualTypeMessage);
                        }
                        else
                        {
                            ShowContactMessage(this.IndividualTypeMessage, "indTypesMessage", !good, false);
                        }
                        this.IndividualTypeMessage = String.Empty;
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void ValidateIndTypeCBs(object source, ServerValidateEventArgs args)
        {
            args.IsValid = cbAdministrator.Checked == true || cbProvider.Checked == true || cbTechnician.Checked == true || cbPatient.Checked == true;
        }

        protected void bSavePersonalServiceData_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_bSavePersonalServiceData_Click", mySession.MyUserID))
#endif
                {
                    var good = false;
                    good = _presenter.DoSaveServiceInfo(this.PersonalInfo);
                    if (!String.IsNullOrEmpty(this.ServiceDataMessage))
                    {
                        if (sender.ToString() != "DeersRefreshClick")
                        {
                            var m = String.Empty;
                            if (good)
                            {
                                m = "Personal and service data successfully updated";
                                ShowConfirmDialog(this.ServiceDataMessage);
                            }
                            else
                            {
                                m = "Error updating personal and service data";
                                ShowContactMessage(this.ServiceDataMessage, "serviceDataMessage", !good, false);
                            }
                            LogEvent(String.Format("{1} by user {0} at {2}", mySession.MyUserID, m, DateTime.Now));
                        }
                        this.ServiceDataMessage = String.Empty;
                        if (good) return;
                        DeersSuccess = false;
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
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_ddlBOS_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    dxHelper = new DemographicXMLHelper();
                    this.StatusTypeSelected = this.ddlStatusType.SelectedValue;
                    StatusType = dxHelper.GetStatusByBOS(BOSTypeSelected);
                    if (StatusTypeSelected.Equals(0)) return;

                    this.RankTypeSelected = this.ddlRank.SelectedValue;
                    RankType = dxHelper.GetRanksByBOSAndStatus(BOSTypeSelected, StatusTypeSelected);
                    this.ddlStatusType.Focus();
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void ddlStatusType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_ddlStatusType_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    dxHelper = new DemographicXMLHelper();
                    this.selectedGrade = this.ddlRank.SelectedValue;
                    RankType = dxHelper.GetRanksByBOSAndStatus(BOSTypeSelected, StatusTypeSelected);
                    this.ddlRank.Focus();
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void bSaveIdNumbers_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_bSaveIdNumbers_Click", mySession.MyUserID))
#endif
            {
                var good = false;
                var m = String.Empty;

                #region DSS

                try
                {
#if DEBUG
                    using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_SaveSSN", mySession.MyUserID))
#endif
                    {
                        good = _presenter.DoSaveIdNumber(this.DSS);
                        if (!String.IsNullOrEmpty(this.IdNumberMessage))
                        {
                            if (sender.ToString() != "DeersRefreshClick")
                                if (good)
                                {
                                    m = "SSN saved successfully";
                                    ShowConfirmDialog(this.IdNumberMessage);
                                }
                                else
                                {
                                    m = "SSN was unsuccessfully saved";
                                    ShowContactMessage(this.IdNumberMessage, "idMessage", !good, false);
                                }
                            LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));
                            this.IdNumberMessage = String.Empty;
                            if (!good) DeersSuccess = false;
                        }
                    }
                }
                catch (Exception ex) { ex.TraceErrorException(); }

                #endregion DSS

                #region DIN

                try
                {
#if DEBUG
                    using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_SaveDIN", mySession.MyUserID))
#endif
                    {
                        good = _presenter.DoSaveIdNumber(this.DIN);
                        if (!String.IsNullOrEmpty(this.IdNumberMessage))
                        {
                            if (sender.ToString() != "DeersRefreshClick")
                                if (good)
                                {
                                    m = "DIN saved successfully";
                                    ShowConfirmDialog(this.IdNumberMessage);
                                }
                                else
                                {
                                    m = "DIN was unsuccessfully saved";
                                    ShowContactMessage(this.IdNumberMessage, "idMessage", !good, false);
                                }
                            LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));
                            this.IdNumberMessage = String.Empty;
                            if (!good) DeersSuccess = false;
                        }
                    }
                }
                catch (Exception ex) { ex.TraceErrorException(); }

                #endregion DIN

                #region DBN

                try
                {
#if DEBUG
                    using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_SaveDBN", mySession.MyUserID))
#endif
                    {
                        good = _presenter.DoSaveIdNumber(this.DBN);
                        if (!String.IsNullOrEmpty(this.IdNumberMessage))
                        {
                            if (sender.ToString() != "DeersRefreshClick")
                                if (good)
                                {
                                    m = "DBN saved successfully";
                                    ShowConfirmDialog(this.IdNumberMessage);
                                }
                                else
                                {
                                    m = "DBN was unsuccessfully saved";
                                    ShowContactMessage(this.IdNumberMessage, "idMessage", !good, false);
                                }
                            LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));
                            this.IdNumberMessage = String.Empty;
                            if (!good) DeersSuccess = false;
                        }
                    }
                }
                catch (Exception ex) { ex.TraceErrorException(); }

                #endregion DBN

                #region PIN

                try
                {
#if DEBUG
                    using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_SavePIN", mySession.MyUserID))
#endif
                    {
                        good = _presenter.DoSaveIdNumber(this.PIN);
                        if (!String.IsNullOrEmpty(this.IdNumberMessage))
                        {
                            if (sender.ToString() != "DeersRefreshClick")
                                if (good)
                                {
                                    m = "PIN saved successfully";
                                    ShowConfirmDialog(this.IdNumberMessage);
                                }
                                else
                                {
                                    m = "PIN was unsuccessfully saved";
                                    ShowContactMessage(this.IdNumberMessage, "idMessage", !good, false);
                                }
                            LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));
                            this.IdNumberMessage = String.Empty;
                            if (!good) DeersSuccess = false;
                        }
                    }
                }
                catch (Exception ex) { ex.TraceErrorException(); }

                #endregion PIN
            }
        }


        protected void bSaveAddress_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdfIsValid.Value == "false") { return; }
                if (this.rblAddressType.SelectedValue == "FN")
                {
                    SaveAddress(sender, e);
                }
                else
                {
                    this.edAddress.ID = this.PrimaryAddress.ID;
                    this.edAddress.IndividualID = this.PrimaryAddress.IndividualID;
                    this.edAddress.Address1 = CleanAddress(this.tbPrimaryAddress1.Text);
                    this.edAddress.Address2 = CleanAddress(this.tbPrimaryAddress2.Text);
                    this.edAddress.City = CleanAddress(this.tbPrimaryCity.Text);
                    this.edAddress.State = CleanAddress(this.ddlPrimaryState.SelectedValue);
                    this.edAddress.ZipCode = CleanAddress(this.tbPrimaryZipCode.Text);
                    this.edAddress.Country = this.ddlPrimaryCountry.SelectedValue;
                    this.edAddress.UIC = CleanAddress(this.tbPrimaryUIC.Text);
                    ViewState["EnteredAddress"] = this.edAddress;
                    if (this.edAddress.Country != "" && this.edAddress.Country == "US" || this.edAddress.Country == "UM")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearPreviousAddress", "ClearPreviousAddress();", true);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "GetSavedAddress", "SetAddress();", true);
                        var result = VerifyUSPSAddress(this.edAddress);
                        var array = result.Split(new string[] { ":" }, StringSplitOptions.None);
                        if (array[0].ToLower() != "error")
                        {
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "VerifyAddressResult", "USPSVerifyAddressResult(" + result + ");", true);
                        }
                        else
                        {
                            string msg = array[1].ToString();
                            string newMsg = Regex.Replace(msg, "[@,\\.\";'\\\\]", string.Empty);

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "VerifyAddressResult", "USPSVerifyAddressResult('" + array[0].ToLower() + "');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        public static string CleanAddress(string textIn)
        {
            string textout = string.Empty;
            char[] arr = textIn.Where(c => (char.IsLetterOrDigit(c) ||
                             char.IsWhiteSpace(c))).ToArray();
            textout = new string(arr);
            return textout;
        }

        protected void SaveAddress(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_bSaveAddress_Click", mySession.MyUserID))
#endif
            {
                try
                {
                    string btnName;
                    if (sender.GetType().Name.ToString() == "ImageButton")
                    {
                        btnName = ((ImageButton)sender).CommandName;
                    }
                    else
                    {
                        btnName = ((Button)sender).CommandName;
                    }
                    switch (btnName)
                    {
                        case "SaveEnteredAddress":
                            this.edAddress = ViewState["EnteredAddress"] as AddressEntity;
                            this.edAddress.ID = this.PrimaryAddress.ID;
                            this.edAddress.IndividualID = this.PrimaryAddress.IndividualID;
                            this.edAddress.Country = this.PrimaryAddress.Country;
                            this.edAddress.UIC = this.PrimaryAddress.UIC;
                            this.edAddress.UIC = "30"; // valid for 30 days
                            this.PrimaryAddress = this.edAddress;
                            this.PrimaryAddress.ModifiedBy = Globals.ModifiedBy;
                            this.PrimaryAddress.ExpireDays = 30; //valid for 30 days
                            break;
                        case "SaveVerifiedAddress":
                            this.VerifiedAddress = ViewState["VerifiedAddress"] as AddressEntity;
                            this.VerifiedAddress.ID = this.PrimaryAddress.ID;
                            this.VerifiedAddress.IndividualID = this.PrimaryAddress.IndividualID;
                            this.VerifiedAddress.Country = this.PrimaryAddress.Country;
                            this.VerifiedAddress.UIC = this.PrimaryAddress.UIC;
                            this.VerifiedAddress.ExpireDays = 90; //valid for 90 days
                            this.PrimaryAddress = this.VerifiedAddress;
                            this.PrimaryAddress.ModifiedBy = Globals.ModifiedBy;
                            this.PrimaryAddress.ExpireDays = 90; //valid for 90 days
                            break;
                    }


                    if (this.rblAddressType.SelectedValue == "FN")
                    {
                        this.PrimaryAddress.ExpireDays = 30;
                        this.PrimaryAddress.DateVerified = DateTime.Now;
                        this.PrimaryAddress.State = "NA";
                    }

                    var good = _presenter.DoSaveAddress(this.PrimaryAddress);
                    //if (sender.ToString() != "DeersRefreshClick")
                    //{
                    var m = String.Empty;
                    if (good)
                    {
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        AddressEntity newAddress = new AddressEntity();
                        newAddress = Session["PrimaryAddress"] as AddressEntity;
                        var savedAddress = "";
                        if (newAddress != null)
                        {
                            hdfVerifiedExpiry.Value = "";
                            hdfDateVerified.Value = "";
                            this.PrimaryAddress = newAddress;
                            savedAddress = serializer.Serialize(newAddress).ToString();
                        }
                        else
                        {
                            savedAddress = serializer.Serialize(this.PrimaryAddress).ToString();
                        }

                        // ValidateAddress();

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "SetSavedAddress", "SetSavedAddress(" + savedAddress + "," + newAddress.ExpireDays + ");", true);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Close", "CloseAddressVerificationDialog();", true);
                        upAddresses.Update();
                        UpdatePanel1.Update();
                        m = "Address saved successfully";
                        ShowConfirmDialog(m);

                        if (Session["CurrentProcess"] != null && Session["CurrentOrderNumber"] != null)
                        {
                            var currentProcess = Session["CurrentProcess"];
                            var currentOrderNumber = Session["CurrentOrderNumber"];

                            if (currentProcess.ToString() == "Order Management" || currentProcess.ToString() == "Do New Order")
                            {
                                //redirect to order management page
                                var redirectURL = "../SrtsOrderManagement/OrderManagement.aspx";
                                this.Redirect(redirectURL, false);
                            }
                        }

                    }
                    else
                    {
                        m = "Address unsuccessfully saved";
                        ShowContactMessage(this.AddressMessage, "addressMessage", !good, false);
                    }
                    LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));

                    this.AddressMessage = String.Empty;
                    if (good) return;
                    // DeersSuccess = false;
                    //}
                }
                catch (Exception ex) { ex.TraceErrorException(); }
            }
        }

        protected void ValidateAddress()
        {
            var status = "";
            var verifyExpiration = "";
            var isExpired = false;

            if (hdfDateVerified.Value != "1/1/0001" && hdfDateVerified.Value != null && hdfDateVerified.Value != "" && PrimaryAddress.ExpireDays != 0)
            {
                var expDate = new DateTime(); // date address was last verified
                expDate = this.PrimaryAddress.DateVerified;
                if (PrimaryAddress.ExpireDays == 30)
                {
                    expDate = expDate.AddDays(30);  // set to expire in 30 days
                }
                else if (PrimaryAddress.ExpireDays == 90)
                {
                    expDate = expDate.AddDays(90); ;  // set to expire in 90 days
                }

                verifyExpiration = expDate.ToLongDateString();
                var currDate = DateTime.Now;
                if (expDate.Date < currDate.Date)
                {
                    isExpired = true;
                }
                if (isExpired)
                {
                    status = "<span style='color:#FF4500'>Address validation expired on " + expDate.ToLongDateString() + ". <br /> Please validate this address.</span>";   // verification has expxired
                }
                else if (!isExpired)
                {
                    status = "<span style='color:#228B22'>Address validation is current until " + expDate.ToLongDateString() + ".</span>";   // verification has not expired;
                }
            }
            else
            {
                status = "<span style='color:#DC143C'>Address has not been validated. Please validate this address.</span>";   // verification has never been done.
            }
            addressHeader.InnerHtml = status;
        }

        protected void btnCancelAddressSave_Click(object sender, EventArgs e)
        {
            string patient;
            bool isPatient = this.IsPatientView;
            if (isPatient)
            {
                patient = "&isP=true";
            }
            else
            {
                patient = "&isP=false";
            }
            //reload person details page
            var redirectURL = "../SrtsPerson/PersonDetails.aspx?id=" + mySession.SelectedPatientID + patient;
            this.Redirect(redirectURL, false);
        }
        
        protected void bSaveEmailAddress_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_bSaveEmailAddress_Click", mySession.MyUserID))
#endif
                {
                    var good = _presenter.DoSaveEmail(this.PrimaryEmail);
                    if (!String.IsNullOrEmpty(this.EmailMessage))
                    {
                        if (sender.ToString() != "DeersRefreshClick")
                        {
                            var m = String.Empty;
                            if (good)
                            {
                                m = "Email address saved successfully";
                                ShowConfirmDialog(this.EmailMessage);
                            }
                            else
                            {
                                m = "Email address unsuccessfully saved";
                                ShowContactMessage(this.EmailMessage, "emailMessage", !good, false);
                            }
                            LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));
                        }
                        this.EmailMessage = String.Empty;
                        if (good) return;
                        DeersSuccess = false;
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void bSavePhoneNumber_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_bSavePhoneNumber_Click", mySession.MyUserID))
#endif
                {
                    var good = _presenter.DoSavePhone(this.PrimaryPhone);
                    if (!String.IsNullOrEmpty(this.PhoneNumberMessage))
                    {
                        if (sender.ToString() != "DeersRefreshClick")
                        {
                            var m = String.Empty;
                            if (good)
                            {
                                m = "Phone number added successfully";
                                ShowConfirmDialog(this.PhoneNumberMessage);
                            }
                            else
                            {
                                m = "Phone number unsuccessfully added";
                                ShowContactMessage(this.PhoneNumberMessage, "phoneMessage", !good, false);
                            }
                            LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));
                        }
                        this.PhoneNumberMessage = String.Empty;
                        if (good) return;
                        DeersSuccess = false;
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void btnAddSite_Click(object sender, ImageClickEventArgs e)
        {
            if (this.lboxAvailSites.SelectedIndex == -1) return;

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_btnAddSite_Click", mySession.MyUserID))
#endif
                {
                    var l = new List<String>();
                    var i = this.lboxAvailSites.SelectedValue.ToString();

                    var cas = this.CuurentAssignedSites;

                    if (cas.Contains(i.ToString()))
                    {
                        this.lboxAvailSites.SelectedIndex = -1;
                        return;
                    }
                    else
                    {
                        cas.Add(i);
                    }

                    l = cas;
                    l.Sort();

                    BindAssignedSitesListBox(l);

                    this.lboxAvailSites.SelectedIndex = -1;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void btnRemSite_Click(object sender, ImageClickEventArgs e)
        {
            if (this.lboxAssignedSites.SelectedIndex == -1) return;

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_btnRemSite_Click", mySession.MyUserID))
#endif
                {
                    var l = new List<String>();
                    var i = this.lboxAssignedSites.SelectedValue.ToString();

                    var cas = this.CuurentAssignedSites;

                    if (cas.Remove(i))
                    {
                        l = cas;
                    }
                    else
                    {
                        this.lboxAssignedSites.SelectedIndex = -1;
                        return;
                    }

                    l.Sort();

                    BindAssignedSitesListBox(l);
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void btnUpdateIndivSites_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_btnUpdateIndivSites_Click", mySession.MyUserID))
#endif
                {
                    int IndId = mySession.SelectedPatientID;
                    var l = lboxAssignedSites.Items.Cast<ListItem>().Select(x => x.Value).ToList();
                    var good = _presenter.InsertIndivualSites(l, IndId);

                    if (good)
                    {
                        LogEvent(String.Format("User {0} added site code(s) {1} to person ID {2} at {3}", mySession.MyUserID, String.Join(", ", l), IndId, DateTime.Now));
                        this.IndividualSitesMessage = "Site codes successfully updated";
                    }
                    else
                    {
                        LogEvent(String.Format("An error occured adding site code(s) {0} to person ID {1} by user {2} at {3}.", String.Join(", ", l), IndId, mySession.MyUserID, DateTime.Now));
                        this.IndividualSitesMessage = "Error updating site codes";
                    }

                    if (!String.IsNullOrEmpty(this.IndividualSitesMessage))
                    {
                        ShowConfirmDialog(this.IndividualSitesMessage);
                        this.IndividualSitesMessage = String.Empty;
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void lnbPatientNameHeader_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_lnbPatientNameHeader_Click", mySession.MyUserID))
#endif
                {
                    var searchDin = !String.IsNullOrEmpty(this.DIN.IDNumber);

                    // Determine which ID numbers are available.
                    if (String.IsNullOrEmpty(this.DSS.IDNumber) && String.IsNullOrEmpty(this.DIN.IDNumber))
                    {
                        LogEvent(String.Format("User {0} attempted to perform a DEERs refresh on {1} without a DSS or DODID.", mySession.MyUserID, DateTime.Now));
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DoAlert", "alert('A DSS or DODID Number are required to do a DEERS data refresh.');", true);
                        return;
                    }

                    //var id = String.IsNullOrEmpty(this.DSS.IDNumber) ? this.DIN.IDNumber : this.DSS.IDNumber;
                    //var idT = id.Length.Equals(9) ? "DSS" : "DIN";

                    var p = new PersonDetailsPresenter(this);

                    this.DssRefreshData = String.IsNullOrEmpty(this.DSS.IDNumber) ? null : p.SearchPersonDmdc(this.DSS.IDNumber, "DSS");
                    // If it is not null then compare the refresh DODID to the original DODID
                    if (!this.DssRefreshData.IsNull())
                    {
                        // If the DODID in the refresh is not the same as the original set flag to do DODID refresh
                        if (this.DssRefreshData.PnIdType1.Equals("D"))
                        {
                            if (this.DssRefreshData.PnId1.Equals(this.DIN.IDNumber))
                                searchDin = false;
                        }
                        else if (this.DssRefreshData.PnIdType2.Equals("D"))
                        {
                            if (this.DssRefreshData.PnId2.Equals(this.DIN.IDNumber))
                                searchDin = false;
                        }
                    }

                    // If searchDin is true then the DODID number belongs to someone else
                    this.DinRefreshData = searchDin ? p.SearchPersonDmdc(this.DIN.IDNumber, "DIN") : null;

                    // Fill dialog items then show dialog
                    if (this.DssRefreshData.IsNull() && this.DinRefreshData.IsNull())
                    {
                        LogEvent(String.Format("User {0} attempted to perform a DEERs refresh on {1} and no data was returned.", mySession.MyUserID, DateTime.Now));
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DoAlert", "alert('No data returned from DEERs/DMDC to update.');", true);
                        ValidateAddress();
                        return;
                    }
                    else if (!this.DssRefreshData.IsNull() || !this.DinRefreshData.IsNull())
                    {
                        SetRefreshDialog(); //this.DssRefreshData, this.DinRefreshData);

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DoDialog", "DoRefreshDialog();", true);

                        if (!this.DssRefreshData.IsNull() && !this.DinRefreshData.IsNull())
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "DoDialogWidth", "$('#divRefreshDialog').dialog({ width: 780 });", true);
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void bSaveRefresh_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_bSaveRefresh_Click", mySession.MyUserID))
#endif
                {
                    if (rbDss.Checked)
                    {
                        SetDataFromRefresh(this.DssRefreshData);
                    }
                    else if (rbDin.Checked)
                    {
                        SetDataFromRefresh(this.DinRefreshData);
                    }

                    // Set radio button list for APO, DPO, FPO
                    switch (this.PrimaryAddress.City)
                    {
                        case "APO":
                        case "FPO":
                        case "DPO":
                            this.rblCity.SelectedValue = this.PrimaryAddress.City;
                            break;

                        default:
                            this.rblCity.SelectedIndex = -1;
                            break;
                    }

                    // DO SAVE OPS HERE
                    bSavePersonalServiceData_Click("DeersRefreshClick", null);
                    bSaveAddress_Click("DeersRefreshClick", null);
                    bSaveEmailAddress_Click("DeersRefreshClick", null);
                    bSaveIdNumbers_Click("DeersRefreshClick", null);
                    bSavePhoneNumber_Click("DeersRefreshClick", null);

                    var m = string.Empty;
                    if (DeersSuccess)
                    {
                        m = "Successfully refreshed data from the DEERS/DMDC system";
                        LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));
                        upAddresses.Update();
                        UpdatePanel1.Update();
                        ShowConfirmDialog(m);
                    }
                    else
                    {
                        m = "An error occured when user {1} attempted to update data from the DEERs refresh";
                        LogEvent(String.Format("{0} on {2}", m, mySession.MyUserID, DateTime.Now));
                        // do fail message
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DoFail", "Confirm('There was an error updating the data refresh from the DEERS/DMDC system.', 'divPersonError', true, false);", true);
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        [System.Web.Services.WebMethod]
        public static string GetUSPSAddressbyZip(string data)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////
            //expected response sample
            //<?xml version="1.0" encoding="UTF-8"?>
            //<CityStateLookupResponse>
            //<ZipCode ID="0">
            //<Zip5>90210</Zip5>
            //<City>BEVERLY HILLS</City>
            //<State>CA</State>
            //</ZipCode>
            //</CityStateLookupResponse>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            try
            {
                var ss = HttpContext.Current.Session["SRTSSession"] as SRTSSession;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_GetUSPSAddressbyZip", ss.MyUserID))
#endif
                {
                    // build the xml request string
                    string Zip5 = data.Substring(0, 5);
                    string userID = ConfigurationManager.AppSettings["uspsUsername"];
                    string requestBaseURL = ConfigurationManager.AppSettings["uspsAPICallBase"];
                    string requestStartAPICall = "?API=CityStateLookup&XML=<CityStateLookupRequest";
                    string requestUserID = " USERID=" + '"' + userID + '"' + ">";
                    string requestQueryID = "<ZipCode ID= " + '"' + "0" + '"' + ">";
                    string requestQueryItem = "<Zip5>" + Zip5 + "</Zip5>";
                    string requestEndAPICall = "</ZipCode></CityStateLookupRequest>";
                    string getResponse = String.Format("{0}{1}{2}{3}{4}{5}",
                        requestBaseURL,
                        requestStartAPICall,
                        requestUserID,
                        requestQueryID,
                        requestQueryItem,
                        requestEndAPICall);

                    // send the request
                    ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                    XmlDocument responseDoc = new XmlDocument();
                    responseDoc.XmlResolver = null;
                    responseDoc.Load(getResponse);

                    // parse the xmldocument response to get the address info
                    string result;

                    // check response for error
                    XmlNodeList error = responseDoc.GetElementsByTagName("Error");
                    XmlNodeList errordescription = responseDoc.GetElementsByTagName("Description");

                    // if response is good
                    if (error.Count == 0)
                    {
                        XmlNodeList city = responseDoc.GetElementsByTagName("City");
                        XmlNodeList state = responseDoc.GetElementsByTagName("State");
                        XmlNodeList zip = responseDoc.GetElementsByTagName("Zip5");

                        // return address info
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        AddressEntity uspsAddress = new AddressEntity();
                        uspsAddress.City = city[0].InnerText;
                        uspsAddress.State = state[0].InnerText;

                        result = serializer.Serialize(uspsAddress).ToString();
                    }
                    else
                    {
                        // 0 means error
                        result = "error:  " + errordescription[0].InnerText;
                    }
                    return result;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); return String.Empty; }
        }

        public static string VerifyUSPSAddress(AddressEntity ent)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////
            //expected response sample
            //<AddressValidateResponse>
            //<Address ID="0">
            //<Address2>6406 IVY LN</Address2>
            //<City>GREENBELT</City>
            //<State>MD</State>
            //<Zip5>20770</Zip5>
            //<Zip4>1441</Zip4>
            //</Address>
            //</AddressValidateResponse>
            //https://secure.shippingapis.com/ShippingAPITest.dll?API=Verify&XML=<AddressValidateRequest USERID="929DEFEN1030"><Address ID="0"><Address1></Address1><Address2>6406 Ivy Lane</Address2><City>Greenbelt</City><State>MD</State><Zip5></Zip5><Zip4></Zip4></Address></AddressValidateRequest>Address1><Address2>1234 é street #2</Address2><City>CLEVELAND</City><State>OH</State><Zip5>44115</Zip5><Zip4></Zip4></Address></AddressValidateRequest>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            try
            {
                var ss = HttpContext.Current.Session["SRTSSession"] as SRTSSession;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_VerifyUSPSAddress", ss.MyUserID))
#endif
                {
                    // build the xml request string
                    string Address1 = ent.Address1;
                    string Address2 = ent.Address2;
                    string City = ent.City;
                    string State = ent.State;
                    string Zip5 = ent.ZipCode;
                    string userID = ConfigurationManager.AppSettings["uspsUsername"];
                    string requestBaseURL = ConfigurationManager.AppSettings["uspsAPICallBase"];
                    string requestStartAPICall = "?API=Verify&XML=<AddressValidateRequest";
                    string requestUserID = " USERID=" + '"' + userID + '"' + ">";
                    string requestAddress = "<Address ID=" + '"' + "0" + '"' + ">";
                    string requestAddress1 = "<Address1></Address1>";
                    string requestAddress2 = "<Address2>" + Address1 + "</Address2>";
                    string requestCity = "<City>" + City + "</City>";
                    string requestState = "<State>" + State + "</State>";
                    string requestZip5 = "<Zip5>" + Zip5 + "</Zip5>";
                    string requestZip4 = "<Zip4></Zip4>";
                    string requestEndAddress = "</Address>";
                    string requestEndAPICall = "</AddressValidateRequest>";
                    string myRequest = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}",
                        requestBaseURL,
                        requestStartAPICall,
                        requestUserID,
                        requestAddress,
                        requestAddress1,
                        requestAddress2,
                        requestCity,
                        requestState,
                        requestZip5,
                        requestZip4,
                        requestEndAddress,
                        requestEndAPICall
                        );

                    // send the request
                    XmlDocument responseDoc = new XmlDocument();
                    responseDoc.XmlResolver = null;
                    try
                    {
                        // Error generated if ServicePointManager is not used:
                        //The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.
                        //- Inner Exception: System.Security.Authentication.AuthenticationException: The remote certificate is invalid according to the validation procedure.
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        responseDoc.Load(myRequest);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message + " - Inner Exception: " + ex.InnerException;

                    }
                    // parse the xmldocument response to get the address info
                    string result;

                    // check response for error
                    XmlNodeList error = responseDoc.GetElementsByTagName("Error");
                    XmlNodeList errordescription = responseDoc.GetElementsByTagName("Description");

                    // if response is good
                    if (error.Count == 0)
                    {
                        XmlNodeList address2 = responseDoc.GetElementsByTagName("Address2");
                        XmlNodeList city = responseDoc.GetElementsByTagName("City");
                        XmlNodeList state = responseDoc.GetElementsByTagName("State");
                        XmlNodeList zip5 = responseDoc.GetElementsByTagName("Zip5");
                        XmlNodeList zip4 = responseDoc.GetElementsByTagName("Zip4");

                        // return address info
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        AddressEntity uspsAddress = new AddressEntity();
                        uspsAddress.Address1 = address2[0].InnerText;
                        uspsAddress.Address2 = Address2.ToUpper();
                        uspsAddress.City = city[0].InnerText;
                        uspsAddress.State = state[0].InnerText;
                        uspsAddress.ZipCode = zip5[0].InnerText + "-" + zip4[0].InnerText;


                        PersonDetails pd = HttpContext.Current.Handler as PersonDetails;
                        if (pd != null)
                        {
                            pd.ViewState["VerifiedAddress"] = uspsAddress;
                        }
                        result = serializer.Serialize(uspsAddress).ToString();
                    }
                    else
                    {
                        // 0 means error
                        result = "error:  " + errordescription[0].InnerText;
                    }
                    return result;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); return String.Empty; }
        }


        #endregion Page Events/Methods

        #region Service Info Accessors

        public string BOSTypeSelected
        {
            get { return ddlBOS.SelectedValue; }
            set { ddlBOS.SelectedValue = value; }
        }

        public List<BOSEntity> BoSType
        {
            get { return ViewState["BosType"] as List<BOSEntity>; }
            set
            {
                ViewState["BosType"] = value;
                try
                {
                    ddlBOS.Items.Clear();
                    ddlBOS.DataSource = value;
                    ddlBOS.DataBind();
                }
                catch { }
            }
        }

        public string StatusTypeSelected
        {
            get { return ddlStatusType.SelectedValue; }
            set { ddlStatusType.SelectedValue = value; }
        }

        public List<StatusEntity> StatusType
        {
            get { return ViewState["StatusType"] as List<StatusEntity>; }
            set
            {
                ViewState["StatusType"] = value;
                try
                {
                    ddlStatusType.Items.Clear();
                    ddlStatusType.DataSource = value;
                    ddlStatusType.DataBind();
                }
                catch { }
            }
        }

        public string RankTypeSelected
        {
            get { return ddlRank.SelectedValue; }
            set { ddlRank.SelectedValue = value; }
        }

        public List<RankEntity> RankType
        {
            get { return ViewState["RankType"] as List<RankEntity>; }
            set
            {
                ViewState["RankType"] = value;
                try
                {
                    ddlRank.Items.Clear();
                    ddlRank.DataSource = value;
                    ddlRank.DataBind();
                    if (!value.Any(x => x.RankValue == this.selectedGrade)) return;
                    ddlRank.SelectedValue = this.selectedGrade;
                }
                catch { }
            }
        }

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

        public List<TheaterLocationCodeEntity> TheaterLocationCodes
        {
            get { return ViewState["TheaterLocationCodes"] as List<TheaterLocationCodeEntity>; }
            set
            {
                ViewState["TheaterLocationCodes"] = value;
                try
                {
                    ddlTheaterLocationCodes.Items.Clear();
                    ddlTheaterLocationCodes.DataSource = value;
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
                    tbEADExpires.Text = value.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    tbEADExpires.Text = string.Empty;
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

        public string ServiceDataMessage
        {
            get;
            set;
        }

        #endregion Service Info Accessors

        #region Personal Info Accessors

        public IndividualEntity PersonalInfo
        {
            get
            {
                var e = ViewState["PersonalInfo"] as IndividualEntity;
                if (e.IsNull())
                    e = new IndividualEntity();
                e.FirstName = this.tbFirstName.Text;
                e.LastName = this.txtLastName.Text;
                e.MiddleName = this.tbMiddleName.Text;
                e.Demographic = String.Format("{0}{1}{2}{3}N", this.RankTypeSelected, this.BOSTypeSelected, this.StatusTypeSelected, this.Gender);
                e.DateOfBirth = this.DOB;
                e.Comments = this.tbComments.Text;
                return e;
            }
            set
            {
                if (value.IsNull()) return;
                ViewState["PersonalInfo"] = value;
                this.FirstName = value.FirstName;
                this.Lastname = value.LastName;
                this.MiddleName = value.MiddleName;
                this.Gender = value.Gender;
                this.Comments = value.Comments;
            }
        }

        public string FirstName
        {
            get { return this.tbFirstName.Text; }
            set { this.tbFirstName.Text = value; }
        }

        public string Lastname
        {
            get { return this.txtLastName.Text; }
            set { this.txtLastName.Text = value; }
        }

        public string MiddleName
        {
            get { return this.tbMiddleName.Text; }
            set { this.tbMiddleName.Text = value; }
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

        public DateTime? DOB
        {
            get { return this.tbDOB.Text.ToNullableDateTime(); }
            set { this.tbDOB.Text = value.Value.ToString("MM/dd/yyyy"); }
        }

        public string Comments
        {
            get { return this.tbComments.Text; }
            set { this.tbComments.Text = value; }
        }

        public string PersonalDataMessage
        {
            get;
            set;
        }

        public string SiteSelected
        {
            get;
            set;
        }

        public List<LookupTableEntity> IndividualType
        {
            get;
            set;
        }

        #endregion Personal Info Accessors

        #region IDNumber Accessors

        public IdentificationNumbersEntity DSS
        {
            get
            {
                var e = ViewState["DSS"] as IdentificationNumbersEntity;
                if (e.IsNull())
                    e = new IdentificationNumbersEntity();
                e.IDNumber = this.tbDss.Text.Trim();
                e.IDNumberType = "DSS";
                return e;
            }
            set
            {
                if (value.IsNull()) { ViewState["DSS"] = new IdentificationNumbersEntity(); return; }
                ViewState["DSS"] = value;
                this.tbDss.Text = value.IDNumber;
            }
        }

        public IdentificationNumbersEntity DIN
        {
            get
            {
                var e = ViewState["DIN"] as IdentificationNumbersEntity;
                if (e.IsNull())
                    e = new IdentificationNumbersEntity();
                e.IDNumber = this.tbDin.Text.Trim();
                e.IDNumberType = "DIN";
                return e;
            }
            set
            {
                if (value.IsNull()) { ViewState["DIN"] = new IdentificationNumbersEntity(); return; }
                ViewState["DIN"] = value;
                this.tbDin.Text = value.IDNumber;
            }
        }

        public IdentificationNumbersEntity DBN
        {
            get
            {
                var e = ViewState["DBN"] as IdentificationNumbersEntity;
                if (e.IsNull())
                    e = new IdentificationNumbersEntity();
                e.IDNumber = this.tbDbn.Text.Trim();
                e.IDNumberType = "DBN";
                return e;
            }
            set
            {
                if (value.IsNull()) { ViewState["DBN"] = new IdentificationNumbersEntity(); return; }
                ViewState["DBN"] = value;
                this.tbDbn.Text = value.IDNumber;
            }
        }

        public IdentificationNumbersEntity PIN
        {
            get
            {
                var e = ViewState["PIN"] as IdentificationNumbersEntity;
                if (e.IsNull())
                    e = new IdentificationNumbersEntity();
                e.IDNumber = this.tbPin.Text.Trim();
                e.IDNumberType = "PIN";
                return e;
            }
            set
            {
                if (value.IsNull()) { ViewState["PIN"] = new IdentificationNumbersEntity(); return; }
                ViewState["PIN"] = value;
                this.tbPin.Text = value.IDNumber;
            }
        }

        public string IdNumberMessage
        {
            get;
            set;
        }

        #endregion IDNumber Accessors

        #region Address Accessors

        public List<LookupTableEntity> States
        {
            get { throw new NotImplementedException(); }
            set
            {
                try
                {
                    ddlPrimaryState.Items.Clear();
                    ddlPrimaryState.DataSource = value;
                    ddlPrimaryState.DataTextField = "ValueTextCombo";
                    ddlPrimaryState.DataValueField = "Value";
                    ddlPrimaryState.DataBind();
                    ddlPrimaryState.Items.Insert(0, new ListItem("-Select-", "X"));
                    ddlPrimaryState.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlPrimaryState.SelectedIndex = -1;
                }
            }
        }

        public List<LookupTableEntity> Countries
        {
            get { throw new NotImplementedException(); }
            set
            {
                try
                {
                    ddlPrimaryCountry.Items.Clear();
                    ddlPrimaryCountry.DataTextField = "Text";
                    ddlPrimaryCountry.DataValueField = "Value";
                    ddlPrimaryCountry.DataSource = value;
                    ddlPrimaryCountry.DataBind();
                    ddlPrimaryCountry.Items.Insert(0, new ListItem("-Select-", "X"));
                    ddlPrimaryCountry.SelectedIndex = 0;
                }
                catch
                {
                    this.ddlPrimaryCountry.SelectedIndex = -1;
                }
            }
        }

        public AddressEntity PrimaryAddress
        {
            get
            {
                var e = ViewState["PrimaryAddress"] as AddressEntity;
                if (e.IsNull())
                    e = new AddressEntity();
                e.Address1 = this.tbPrimaryAddress1.Text;
                e.Address2 = this.tbPrimaryAddress2.Text;
                e.City = this.tbPrimaryCity.Text;
                e.State = this.ddlPrimaryState.SelectedValue;
                e.Country = this.ddlPrimaryCountry.SelectedValue;
                e.ZipCode = this.tbPrimaryZipCode.Text;
                e.UIC = this.tbPrimaryUIC.Text;
                e.IsActive = true;
                return e;
            }
            set
            {
                if (value.IsNull()) return;
                ViewState["PrimaryAddress"] = value;
                this.tbPrimaryAddress1.Text = value.Address1;
                this.tbPrimaryAddress2.Text = value.Address2;
                this.tbPrimaryCity.Text = value.City;
                hdfCity.Value = value.City;
                this.ddlPrimaryState.SelectedValue = value.State;
                hdfState.Value = value.State;
                this.ddlPrimaryCountry.SelectedValue = value.Country;
                this.tbPrimaryZipCode.Text = value.ZipCode;
                this.tbPrimaryUIC.Text = value.UIC;
                hdfDateVerified.Value = value.DateVerified.ToShortDateString();
                hdfVerifiedExpiry.Value = value.ExpireDays.ToString();
            }
        }

        public string AddressMessage
        {
            get;
            set;
        }

        #endregion Address Accessors

        #region Email Accessors

        public EMailAddressEntity PrimaryEmail
        {
            get
            {
                var e = ViewState["PrimaryEmail"] as EMailAddressEntity;
                if (e.IsNull())
                    e = new EMailAddressEntity();
                e.EMailAddress = this.tbPrimaryEmail.Text;
                e.IsActive = true;
                return e;
            }
            set
            {
                if (value.IsNull()) return;
                ViewState["PrimaryEmail"] = value;
                this.tbPrimaryEmail.Text = value.EMailAddress;
            }
        }

        public string EmailMessage
        {
            get;
            set;
        }

        #endregion Email Accessors

        #region Phone Accessors

        public PhoneNumberEntity PrimaryPhone
        {
            get
            {
                var e = ViewState["PrimaryPhone"] as PhoneNumberEntity;
                if (e.IsNull())
                    e = new PhoneNumberEntity();
                e.Extension = this.tbPrimaryExtension.Text;
                e.PhoneNumber = this.tbPrimaryPhoneNumber.Text;
                e.IsActive = true;
                return e;
            }
            set
            {
                if (value.IsNull()) return;
                ViewState["PrimaryPhone"] = value;
                this.tbPrimaryExtension.Text = value.Extension;
                this.tbPrimaryPhoneNumber.Text = value.PhoneNumber;
            }
        }

        public string PhoneNumberMessage
        {
            get;
            set;
        }

        #endregion Phone Accessors

        #region Individual Type & Sites Accessors

        public string IndividualTypeMessage
        {
            get;
            set;
        }

        public List<IndividualTypeEntity> IndividualTypesBind
        {
            set
            {
                if (!value.IsNull())
                {
                    cbAdministrator.Checked = value.Any(x => x.TypeDescription.ToLower() == "other" && x.IsActive == true);
                    cbPatient.Checked = value.Any(x => x.TypeDescription.ToLower() == "patient" && x.IsActive == true);
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

        public bool IsPatient
        {
            get { return cbPatient.Checked.Equals(true); }
        }

        public bool IsProvider
        {
            get { return cbProvider.Checked.Equals(true); }
        }

        public bool IsPatientOnly
        {
            get { return IsAdmin.Equals(false) && IsTechnician.Equals(false) && IsProvider.Equals(false) && IsPatient.Equals(true); }
        }

        public List<KeyValueEntity> IndividualTypeLookup
        {
            set
            {
                Session["IndividualTypeLookup"] = value;
            }
            get { return Session["IndividualTypeLookup"] as List<KeyValueEntity>; }
        }

        private List<String> CuurentAssignedSites
        {
            get
            {
                var l = new List<String>();

                if (!ViewState["CuurentAssignedSites"].IsNull())
                    l = ViewState["CuurentAssignedSites"] as List<String>;

                return l;
            }
            set
            {
                ViewState["CuurentAssignedSites"] = value;
            }
        }

        public List<String> AssignedSiteList
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                BindAssignedSitesListBox(value);
                this.CuurentAssignedSites = value;
            }
        }

        public void BindAssignedSitesListBox(List<String> _list)
        {
            var l = this.SiteCodes.Where(x => _list.Contains(x.SiteCode));

            this.lboxAssignedSites.DataSource = l;
            this.lboxAssignedSites.DataTextField = "SiteCombinationProfile";
            this.lboxAssignedSites.DataValueField = "SiteCode";
            this.lboxAssignedSites.DataBind();
            this.lboxAssignedSites.SelectedIndex = -1;
        }

        public void BindAvailSitesListBox()
        {
            var l = this.SiteCodes.Where(x => x.SiteType == "CLINIC" && x.SiteCode != "GEyes1" && x.SiteCode != "009900").ToList();

            this.lboxAvailSites.DataSource = l;
            this.lboxAvailSites.DataTextField = "SiteCombinationProfile";
            this.lboxAvailSites.DataValueField = "SiteCode";
            this.lboxAvailSites.DataBind();
            this.lboxAvailSites.SelectedIndex = -1;
        }

        public List<SiteCodeEntity> SiteCodes
        {
            get
            {
                return ViewState["SiteCodes"] as List<SiteCodeEntity>;
            }
            set
            {
                ViewState["SiteCodes"] = value;
                BindAvailSitesListBox();
            }
        }

        public string IndividualSitesMessage
        {
            get;
            set;
        }

        #endregion Individual Type & Sites Accessors

        #region Misc Accessors

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        #endregion Misc Accessors

        private void SetRefreshDialog()
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_SetRefreshDialog", mySession.MyUserID))
#endif
            {
                this.lblOrigAddress1.Text = Server.HtmlEncode(this.PrimaryAddress.Address1);
                this.lblOrigAddress2.Text = Server.HtmlEncode(this.PrimaryAddress.Address2);
                this.lblOrigBos.Text = this.BOSTypeSelected.ToHtmlEncodeString();
                this.lblOrigCity.Text = Server.HtmlEncode(this.PrimaryAddress.City);
                this.lblOrigCountry.Text = Server.HtmlEncode(this.PrimaryAddress.Country);
                this.lblOrigDin.Text = Server.HtmlEncode(this.DIN.IDNumber);
                this.lblOrigDob.Text = this.DOB.Value.ToShortDateString();
                this.lblOrigEmail.Text = Server.HtmlEncode(this.PrimaryEmail.EMailAddress);
                this.lblOrigPhone.Text = Server.HtmlEncode(this.PrimaryPhone.PhoneNumber);
                this.lblOrigFirstName.Text = this.FirstName.ToHtmlEncodeString();
                this.lblOrigGrade.Text = this.RankTypeSelected.ToHtmlEncodeString();
                this.lblOrigLastName.Text = this.Lastname.ToHtmlEncodeString();
                this.lblOrigMiddleName.Text = this.MiddleName.ToHtmlEncodeString();
                this.lblOrigPhone.Text = Server.HtmlEncode(this.PrimaryPhone.PhoneNumber);
                this.lblOrigDss.Text = Server.HtmlEncode(this.DSS.IDNumber);//SSN.IDNumber); //Aldela: Did SSN change to DSS?
                this.lblOrigState.Text = Server.HtmlEncode(this.PrimaryAddress.State);
                this.lblOrigStatus.Text = this.StatusTypeSelected.ToHtmlEncodeString();
                this.lblOrigZip.Text = Server.HtmlEncode(this.PrimaryAddress.ZipCode);
            }
        }

        private void SetDataFromRefresh(DmdcPerson_Flat person)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_SetDataFromRefresh", mySession.MyUserID))
#endif
            {
                // Set ID Numbers
                var din = this.DIN;
                din.IDNumber = person.PnIdType1.Equals("D") ? person.PnId1 : person.PnId2;//ids.FirstOrDefault(j => j.IDNumberType == "DIN").IDNumber;
                din.IsActive = !String.IsNullOrEmpty(din.IDNumber);

                var dss = this.DSS;
                dss.IDNumber = person.PnIdType1.Equals("S") ? person.PnId1 : person.PnId2;//ids.FirstOrDefault(k => k.IDNumberType == "DSS").IDNumber;
                dss.IsActive = !String.IsNullOrEmpty(dss.IDNumber);

                this.DIN = din;
                this.DSS = dss;

                // Set Personal Data
                this.FirstName = person.PnFirstName;
                this.Lastname = person.PnLastName;
                this.DOB = person.PnDateOfBirth;

                // Set Address
                var pa = this.PrimaryAddress;
                pa.Address1 = person.MailingAddress1;
                pa.Address2 = person.MailingAddress2;
                pa.City = person.MailingCity;
                pa.Country = person.MailingCountry;
                pa.State = person.MailingState;
                pa.ZipCode = String.Format("{0}", String.IsNullOrEmpty(person.MailingZipExtension) ? String.Format("{0}", person.MailingZip) : String.Format("{0}-{1}", person.MailingZip, person.MailingZipExtension));
                pa.UIC = String.Empty;
                this.PrimaryAddress = pa;

                // Set Service Data
                var x = new DemographicXMLHelper();
                if (!String.IsNullOrEmpty(person.ServiceCode))
                {
                    // Get all BOS
                    this.BoSType = x.GetALLBOS();

                    this.BOSTypeSelected = person.ServiceCode;

                    if (!String.IsNullOrEmpty(person.PnCategoryCode))
                    {
                        // Get Statuses
                        this.StatusType = x.GetStatusByBOS(person.ServiceCode);

                        this.StatusTypeSelected = person.PnCategoryCode;

                        if (!String.IsNullOrEmpty(person.PayGrade)) //!String.IsNullOrEmpty(d._DmdcPersonnel.PayPlanCode) &&
                        {
                            // Get Grades
                            this.RankType = x.GetRanksByBOSAndStatus(person.ServiceCode, person.PnCategoryCode);

                            // The only grades I can handle at the moment are military and civ
                            if (person.PayPlanCode.Equals("ME") || person.PayPlanCode.Equals("MO") || person.PayPlanCode.Equals("MW"))
                            {
                                var pg = person.PayPlanCode.Equals("MW") ? String.Format("O{0}", person.PayGrade.Substring(1)) : person.PayGrade;
                                this.RankTypeSelected = String.Format("{0}{1}", person.PayPlanCode.Substring(1), pg);
                            }
                            else if (person.PayPlanCode.Equals("GS") || String.IsNullOrEmpty(person.PayPlanCode))
                                this.RankTypeSelected = "CIV";
                        }
                    }
                }

                // Set Email and Phone Data
                var pe = this.PrimaryEmail;
                pe.EMailAddress = person.Email;
                pe.IsActive = true;
                this.PrimaryEmail = pe;

                var pp = this.PrimaryPhone;
                pp.PhoneNumber = person.PhoneNumber;
                pp.Extension = String.Empty;
                pp.IsActive = true;
                this.PrimaryPhone = pp;
            }
        }

        private String MaskIdNumber(String idNumIn)
        {
            if (String.IsNullOrEmpty(idNumIn)) return idNumIn;

            var len = idNumIn.Length;
            var mask = String.Empty;
            mask = mask.PadRight(len - 4, '*');
            return String.Format("{0}{1}", mask, idNumIn.Substring(idNumIn.Length - 4));
        }

        private void ShowContactMessage(String msg, String msgCtId, Boolean isError, Boolean persistMsg)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Confirm", String.Format("Confirm('{0}', '{1}', {2}, {3});", msg, msgCtId, isError.ToString().ToLower(), persistMsg.ToString().ToLower()), true);
        }

        private void ShowConfirmDialog(String msg)
        {
            /// Show global confirm dialog
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Success!','" + msg + "', 'success');", true);
        }

        private void ShowErrorDialog(String msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayErrorDialogMessage", "displaySrtsMessage('Error!','" + msg + "', 'error');", true);
        }


        public string SiteCode
        {
            set { this.lblSiteCode.Text = string.Format("Site Code: {0}", value); }
        }

        private Boolean DeersSuccess { get; set; }

        private DmdcPerson_Flat DssRefreshData
        {
            get { return ViewState["DssRefreshData"] as DmdcPerson_Flat; }
            set
            {
                ViewState["DssRefreshData"] = value;
                if (!value.IsNull())
                {
                    var dss = value;

                    this.lblDssAddress1.Text = dss.MailingAddress1;
                    this.lblDssAddress2.Text = dss.MailingAddress2;
                    this.lblDssBos.Text = dss.ServiceCode;
                    this.lblDssCity.Text = dss.MailingCity;
                    this.lblDssCountry.Text = dss.MailingCountry;
                    this.lblDssDin.Text = dss.PnIdType1.Equals("D") ? dss.PnId1 : dss.PnId2;
                    this.lblDssDob.Text = dss.PnDateOfBirth.ToShortDateString();
                    this.lblDssEmail.Text = dss.Email;
                    this.lblDssFirstName.Text = dss.PnFirstName;

                    // The only grades I can handle at the moment are military and civ
                    if (dss.PayPlanCode.Equals("ME") || dss.PayPlanCode.Equals("MO") || dss.PayPlanCode.Equals("MW"))
                    {
                        var pg = dss.PayPlanCode.Equals("MW") ? String.Format("O{0}", dss.PayGrade.Substring(1)) : dss.PayGrade;
                        this.lblDssGrade.Text = String.Format("{0}{1}", dss.PayPlanCode.Substring(1), pg);
                    }
                    else if (dss.PayPlanCode.Equals("GS") || String.IsNullOrEmpty(dss.PayPlanCode))
                        this.lblDssGrade.Text = "CIV";

                    this.lblDssLastName.Text = dss.PnLastName;
                    this.lblDssMiddleName.Text = dss.PnMiddleName;
                    this.lblDssPhone.Text = dss.PhoneNumber;
                    this.lblDssDss.Text = dss.PnIdType1.Equals("S") ? dss.PnId1 : dss.PnId2;
                    this.lblDssState.Text = dss.MailingState;
                    this.lblDssStatus.Text = dss.PnCategoryCode;
                    this.lblDssZip.Text = dss.MailingZip;

                    this.rbDss.Checked = true;
                }
                else
                {
                    this.divDssRefresh.Visible = false;
                    this.rbDin.Visible = false;
                }
            }
        }

        private DmdcPerson_Flat DinRefreshData
        {
            get { return ViewState["DinRefreshData"] as DmdcPerson_Flat; }
            set
            {
                ViewState["DinRefreshData"] = value;

                if (!value.IsNull())
                {
                    var din = value;
                    this.lblDinAddress1.Text = din.MailingAddress1;
                    this.lblDinAddress2.Text = din.MailingAddress2;
                    this.lblDinBos.Text = din.ServiceCode;
                    this.lblDinCity.Text = din.MailingCity;
                    this.lblDinCountry.Text = din.MailingCountry;
                    this.lblDinDin.Text = din.PnIdType1.Equals("D") ? din.PnId1 : din.PnId2;
                    this.lblDinDob.Text = din.PnDateOfBirth.ToShortDateString();
                    this.lblDinEmail.Text = din.Email;
                    this.lblDinFirstName.Text = din.PnFirstName;

                    // The only grades I can handle at the moment are military and civ
                    if (din.PayPlanCode.Equals("ME") || din.PayPlanCode.Equals("MO") || din.PayPlanCode.Equals("MW"))
                    {
                        var pg = din.PayPlanCode.Equals("MW") ? String.Format("O{0}", din.PayGrade.Substring(1)) : din.PayGrade;
                        this.lblDinGrade.Text = String.Format("{0}{1}", din.PayPlanCode.Substring(1), pg);
                    }
                    else if (din.PayPlanCode.Equals("GS") || String.IsNullOrEmpty(din.PayPlanCode))
                        this.lblDinGrade.Text = "CIV";

                    this.lblDinLastName.Text = din.PnLastName;
                    this.lblDinMiddleName.Text = din.PnMiddleName;
                    this.lblDinPhone.Text = din.PhoneNumber;
                    this.lblDinDss.Text = din.PnIdType1.Equals("S") ? din.PnId1 : din.PnId2;
                    this.lblDinState.Text = din.MailingState;
                    this.lblDinStatus.Text = din.PnCategoryCode;
                    this.lblDinZip.Text = din.MailingZip;

                    if (this.DssRefreshData.IsNull())
                        this.rbDin.Checked = true;
                }
                else
                {
                    this.divDinRefresh.Visible = false;
                    this.rbDss.Visible = false;
                }
            }
        }
    }
}


//protected void bSaveAddress_Click(object sender, EventArgs e)
//{
//    try
//    {
//        if (hdfIsValid.Value == "false") { return; }
//        using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_bSaveAddress_Click", mySession.MyUserID))
//        {
//            var good = _presenter.DoSaveAddress(this.PrimaryAddress);
//            if (!String.IsNullOrEmpty(this.AddressMessage))
//            {
//                if (sender.ToString() != "DeersRefreshClick")
//                {
//                    var m = String.Empty;
//                    if (good)
//                    {
//                        ScriptManager.RegisterStartupScript(this, GetType(), "ClearPreviousAddress", "ClearPreviousAddress();", true);
//                        ScriptManager.RegisterStartupScript(this, GetType(), "GetSavedAddress", "SetAddress();", true);
//                        m = "Address saved successfully";
//                        ShowConfirmDialog(this.AddressMessage);
//                    }
//                    else
//                    {
//                        m = "Address unsuccessfully saved";
//                        ShowContactMessage(this.AddressMessage, "addressMessage", !good, false);
//                    }
//                    LogEvent(String.Format("{0} by user {1} at {2}.", m, mySession.MyUserID, DateTime.Now));
//                }
//                this.AddressMessage = String.Empty;
//                if (good) return;
//                DeersSuccess = false;
//            }
//        }
//    }
//    catch (Exception ex) { ex.TraceErrorException(); }
//}