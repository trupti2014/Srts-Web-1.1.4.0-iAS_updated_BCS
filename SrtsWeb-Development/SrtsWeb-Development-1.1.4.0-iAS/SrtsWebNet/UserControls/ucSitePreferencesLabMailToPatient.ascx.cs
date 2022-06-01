using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.Views.Admin;
using System.Runtime.InteropServices;
using System.Configuration;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucSitePreferencesLabMailToPatient : UserControlBase, 
        ISitePreferencesLabMailToPatientView, IEmailMessageView
    {
        private SitePreferencesPresenter.LabMailToPatientPresenter p;
        private EmailMessagePresenter emailpresenter;

        public ucSitePreferencesLabMailToPatient()
        {
            p = new SitePreferencesPresenter.LabMailToPatientPresenter(this);
            emailpresenter = new EmailMessagePresenter();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.mySession.MyIndividualID = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).IndividualId;
            ((SrtsWeb.WebForms.Admin.SitePreferences)this.Page).RefreshUserControl += ucSitePreferences_RefreshUserControl;
            if (!IsPostBack)
            {
                this.p = new SitePreferencesPresenter.LabMailToPatientPresenter(this);
                SitePrefLabMTPEntity spe = new SitePrefLabMTPEntity();
                spe = this.p.InitView();
                SetMailToPatientStatus(spe);
            }
        }
        protected void SetMailToPatientStatus(SitePrefLabMTPEntity spe)
        {
            if (spe.IsCapabilityOn == "True")
            {
                rblMailToPatient.SelectedIndex = 0;
            }
            else
            {
                rblMailToPatient.SelectedIndex = 1;
                rblNoMailToPaientReason.SelectedValue = spe.StatusReason;
                txtOtherComments.Text = spe.Comments;
                txtStopDate.Text = spe.StopDate.ToDateTime().ToShortDateString();
                txtRestartDate.Text = spe.AnticipatedRestartDate.ToDateTime().ToShortDateString();
            }
            hdfLabMTPStatus.Value = spe.IsCapabilityOn.ToLower();
        }

        private void ucSitePreferences_RefreshUserControl(object sender, EventArgs e)
        {
            Page.Response.Redirect(Page.Request.Url.ToString(), false);
        }


        protected void btnSaveLabMTPPref_Click(object sender, EventArgs e)
        {
            SitePrefLabMTPEntity.ModifiedBy = mySession.MyUserID;
            SitePrefLabMTPEntity.SiteCode = SiteCode;
            SitePrefLabMTPEntity.IsLabMailToPatient = mySession.MySite.ShipToPatientLab.ToString();
            SitePrefLabMTPEntity.IsCapabilityOn = rblMailToPatient.SelectedValue;
            SitePrefLabMTPEntity.StopDate = txtStopDate.Text.ToDateTime();
            SitePrefLabMTPEntity.StartDate = null;

            bool update = true;

            if (rblMailToPatient.SelectedValue == "true")
            {
                // If we are not disabling or enabling clinics, then save lab preferences.
                if (!String.IsNullOrEmpty(hdfClinicsToDisable.Value) || (!String.IsNullOrEmpty(hdfClinicsToEnable.Value)))
                {
                    // Check to see if we are disabling clinics, if so send email and update clinc status in database.
                    if (!String.IsNullOrEmpty(hdfClinicsToDisable.Value))
                    {
                        DisableClinics();
                    }
                    // Check to see if we are enabling clinics, if so send email and update clinc status in database.
                    if (!String.IsNullOrEmpty(hdfClinicsToEnable.Value))
                    {
                        EnableClinics();
                    }
                }
                else  // save lab preference changes
                {
                    SitePrefLabMTPEntity.StatusReason = string.Empty;  // turning on capability.....  need to enable lab and notifiy admins
                    SitePrefLabMTPEntity.StopDate = null;
                    SitePrefLabMTPEntity.StartDate = txtStartDate.Text.ToDateTime();
                    SitePrefLabMTPEntity.AnticipatedRestartDate = null;
                    SitePrefLabMTPEntity.Comments = "Capability Available";
                    InsertUpdateLabMTPPref(SitePrefLabMTPEntity);
                    EnableLab("CapabilityAvailable");
                }
            }
            else
            {
                SitePrefLabMTPEntity.StatusReason = rblNoMailToPaientReason.SelectedValue;  // turning off capability
                // if 'Other' selected as reason
                if (SitePrefLabMTPEntity.StatusReason == "Other")
                {
                    if (string.IsNullOrEmpty(txtOtherComments.Text))
                    {
                        update = false;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "SendAlert", "ValidateComments();", true);
                    }
                    else
                    {
                        SitePrefLabMTPEntity.Comments = this.txtOtherComments.Text;
                        SitePrefLabMTPEntity.AnticipatedRestartDate = null;
                        update = true;
                        DisableLab(SitePrefLabMTPEntity.StatusReason);
                    }
                }
                if (SitePrefLabMTPEntity.StatusReason == "NoCapacity")
                {
                    SitePrefLabMTPEntity.AnticipatedRestartDate = txtRestartDate.Text.ToDateTime();
                    if (String.IsNullOrEmpty(this.txtOtherComments.Text))
                    {
                        SitePrefLabMTPEntity.Comments = "Reason: No Capacity";
                    }
                    else
                    {
                        SitePrefLabMTPEntity.Comments = this.txtOtherComments.Text;
                        update = true;
                        DisableLab(SitePrefLabMTPEntity.StatusReason);
                    }
                }
                if (update)
                {
                    InsertUpdateLabMTPPref(SitePrefLabMTPEntity);
                }
            }
            ucSitePreferences_RefreshUserControl(sender, e);
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "DisplayMailToPatient", "<script>DisplayLabMailToPatient();</script>", false);
        }

        protected void EnableClinics()
        {
            try
            {
                List<string> clinicsToEnable = new List<string>();
                string clinicstoenable = hdfClinicsToEnable.Value;

                if (!string.IsNullOrEmpty(clinicstoenable)) // do enable clinics
                {
                    clinicsToEnable = clinicstoenable.Split(',').ToList<string>();
                    clinicsToEnable.RemoveAll(clinic => clinic == "");
                    hdfClinicsToEnable.Value = string.Empty;
                }

                // if there are clinics to disable, notify clinic admins and update database status
                if (clinicsToEnable.Count > 0)
                {
                    DateTime? effectiveDate = null;
                    if (!String.IsNullOrEmpty(txtClinicRestartDate.Text))
                    {
                        effectiveDate = txtClinicRestartDate.Text.ToDateTime();
                    }

                    foreach (string clinic in clinicsToEnable)
                    {
                        if (!string.IsNullOrEmpty(clinic))
                        {
                            //update clinic status in database - set action required to false
                            if (p.InsertUpdateClinicStatus(clinic, "false", effectiveDate))
                            {
                                // send email to notify clinic admins that they are now enabled
                                if (PrepareClinicAdminEmailNotification(clinic, null, effectiveDate))
                                {
                                    // update database email notifications
                                    if (UpdateDatabaseEmailNotificationsSent())
                                    {
                                        ShowConfirmDialog("Clinic status successfully updated.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var Message = "Error occured on the page ucSitePreferencesLabMailToPatient.ascx.cs in the EnableClinics() method.";
                ex.LogException(String.Format("{0}", "{1}", "{2}", Message, ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Disables the 'Lab Ship to Patient' option for a clinic when placing an order for a patient.  
        /// All Clinic users receive an email notification.
        /// The database is updated with the clinic's status and a copy of the email sent. 
        /// </summary>
        protected void DisableClinics()
        {
            try
            {
                List<string> clinicsToDisable = new List<string>();
                string clinicstodisable = hdfClinicsToDisable.Value;

                if (!String.IsNullOrEmpty(clinicstodisable))
                {
                    clinicsToDisable = clinicstodisable.Split(',').ToList<string>();
                    clinicsToDisable.RemoveAll(clinic => clinic == "");
                    hdfClinicsToDisable.Value = string.Empty;
                }

                // if there are clinics to disable, notify all clinic users and update database status
                if (clinicsToDisable.Count > 0)
                {
                    DateTime effectivedate = txtClinicStopDate.Text.ToDateTime();
                    var comments = txtCClinicStopComment.Text;
                    foreach (string clinic in clinicsToDisable)
                    {
                        if (!string.IsNullOrEmpty(clinic))
                        {
                            // update clinic status in database - set action required to true
                            if (p.InsertUpdateClinicStatus(clinic, "true", effectivedate, comments))
                            {
                                // send email to notify clinic users that they are disabled
                                if (PrepareClinicUserEmailNotification(clinic, null, effectivedate))
                                {
                                    // update database with email notifications
                                    if (UpdateDatabaseEmailNotificationsSent())
                                    {
                                        ShowConfirmDialog("Clinic status successfully Updated.");
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var Message = "Error occured on the page ucSitePreferencesLabMailToPatient.ascx.cs in the DisableClinics() method.";
                ex.LogException(String.Format("{0}", "{1}", "{2}", Message, ex.Message, ex.InnerException));
            }
        }

       

        /// <summary>
        /// Updates the database with the email that was sent.
        /// </summary>
        protected bool UpdateDatabaseEmailNotificationsSent()
        {
            var result = false;
            // insert email into database
            if (this.EmailNotification != null & this.EmailNotification.Count > 0)
            {
                foreach (EmailMessageEntity emailnotification in EmailNotification)
                {
                    emailnotification.LabSiteCode = mySession.MySite.SiteCode;
                    if (ValidateEmailNotification(emailnotification))
                    {
                        result = InsertEmailNotificationSent(emailnotification);
                    }
                    else
                    {
                        result = false;
                        var message = "ucSitePreferencesLabMailToPatient.cs - UpdateDatabaseEmailNotificationsSent() - ValidateEmailNotification(): emailnotification did not pass validation";
                        DoResultHandling(result, message);
                    }
                }
               // this.EmailNotification.Clear();
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        protected void DoResultHandling(bool result, string message)
        {
            if (result)
            {
                hdfNotificationValue.Value = "success";
            }
            else
            {
                Exception ex = new Exception();
                var msg = message;
                ex.LogException(String.Format("An error occured in {0}", msg));
                hdfNotificationValue.Value = "error";
            }
            hdfNotificationMessage.Value = message;
            // display message
        }


        protected void DisableLab(string reason)
        {
            // send notification based on reason
            List<string> sites = new List<string>();
            List<SiteCodeEntity> allclinics = new List<SiteCodeEntity>();
            if (this.Clinics != null && this.Clinics.ToList().Count > 0)
            {
                allclinics = this.Clinics.ToList();
            }
            else
            {
                p.GetClinicsforLab(SitePrefLabMTPEntity.SiteCode);
                allclinics = this.Clinics.ToList();
            }

            if (allclinics.Count > 0)
            {
                foreach (SiteCodeEntity site in allclinics)
                {
                    sites.Add(site.SiteCode);
                }
            }

            var result = false;
            foreach (string sitecode in sites)
            {
                //if (sitecode == "000003" || sitecode == "00000C")  // only doing 2 sites for testing ///////////////////////////////////////////////
                //{
                    if (!string.IsNullOrEmpty(sitecode))
                    {
                        // send email to notify clinic users that lab is not shipping to patients
                        result = PrepareClinicUserEmailNotification(sitecode, reason);
                        if (result)
                        {
                            // update database email notifications
                            result = UpdateDatabaseEmailNotificationsSent();
                        }
                    }
                    else
                    {
                        hdfNotificationValue.Value = "error";
                        hdfNotificationMessage.Value = "There was a problem Updating the Clinic Status.";
                        result = false;
                    }
            }
            sites.Clear();
        }

        protected void EnableLab(string reason)
        {
            // send notification based on reason
            List<string> sites = new List<string>();
            List<SiteCodeEntity> allclinics = new List<SiteCodeEntity>();
            if (this.Clinics != null && this.Clinics.ToList().Count > 0)
            {
                allclinics = this.Clinics.ToList();
            }
            else
            {
                p.GetClinicsforLab(SitePrefLabMTPEntity.SiteCode);
                allclinics = this.Clinics.ToList();
            }

            if (allclinics.Count > 0)
            {
                foreach (SiteCodeEntity site in allclinics)
                {
                    sites.Add(site.SiteCode);
                }
            }

            var result = true;
            foreach (string sitecode in sites)
            {
                if (!string.IsNullOrEmpty(sitecode))
                {
                    // send email to notify clinic users that lab is not shipping to patients
                    result = PrepareClinicAdminEmailNotification(sitecode, reason);
                }
                else
                {
                    result = false;
                }

                if (result)
                {
                    // insert email into database
                    if (this.EmailNotification != null & this.EmailNotification.Count > 0)
                    {
                        foreach (EmailMessageEntity emailmessageentity in EmailNotification)
                        {
                            emailmessageentity.LabSiteCode = mySession.MySite.SiteCode;
                            if (ValidateEmailNotification(emailmessageentity))
                            {
                                InsertEmailNotificationSent(emailmessageentity);
                            }
                            else
                            {
                                Exception ex = new Exception();
                                ex.Source = "ucSitePrefgerenccesLabMailToPatient.cs - EnableLab(), Line 398";
                                ex.LogException(String.Format("An error occured in {0}, {1}, {2}", ex.Source, ex.Message, ex.InnerException));
                                // display error dialog............................................
                            }
                        }
                        EmailNotification.Clear();
                    }
                }
            }
            sites.Clear();
        }

        protected bool PrepareClinicUserEmailNotification(string clinicsitecode, [Optional] string reason, [Optional] DateTime? effectiveDate)
        {
            var result = false;
            EmailMessageEntity emailtosend = new EmailMessageEntity();

           
            // Get notification Reason - Lab-Capacity; Lab-ClinicActionRequired or Lab-Other
            string notificationReason = string.Empty;
            if (string.IsNullOrEmpty(reason))
            {
                if (rblNoMailToPaientReason.SelectedIndex > 0)
                {
                    notificationReason = rblNoMailToPaientReason.SelectedValue;
                }
                else
                {
                    notificationReason = "ClinicActionRequired";
                    if (effectiveDate == null)
                    {
                        effectiveDate = DateTime.Now;
                    }
                }
            }
            else { notificationReason = reason; }

            emailtosend.ClinicSiteCode = clinicsitecode;
            emailtosend.EmailType = notificationReason;
           
            // Get Email message for users - Lab-Capacity; Lab-ClinicActionRequired or Lab-Other
            if (!String.IsNullOrEmpty(notificationReason))
            {
                    switch (notificationReason)
                {
                    case "NoCapacity":
                        EmailMessageType = emailpresenter.GetEmailMessageByType("Lab-LabCapacity");
                        emailtosend.EmailBody = EmailMessageType.MessagePart1 +
                                    SiteCode +
                                    EmailMessageType.MessagePart2 + txtStopDate.Text + ".  " + 
                                    SiteCode + EmailMessageType.MessagePart3 +
                                    txtRestartDate.Text + ".";
                        break;
                    case "Other":
                        if (effectiveDate != null)
                        {
                            EmailMessageType = emailpresenter.GetEmailMessageByType("Lab-Other");
                            emailtosend.EmailBody = SiteCode + EmailMessageType.MessagePart1 +
                                        effectiveDate +
                                        EmailMessageType.MessagePart2;
                        }
                        else
                        {
                            hdfNotificationMessage.Value = "A date is required for this action.";
                            hdfNotificationValue.Value = "1";
                            return false;
                        }
                        break;
                    case "ClinicActionRequired":
                        if (effectiveDate != null)
                        {
                            EmailMessageType = emailpresenter.GetEmailMessageByType("Lab-ClinicActionRequired");
                            emailtosend.EmailBody = SiteCode + EmailMessageType.MessagePart1 +
                                        effectiveDate + "." +
                                        EmailMessageType.MessagePart2;
                        }
                        else
                        {
                            hdfNotificationMessage.Value = "A date is required for this action.";
                            hdfNotificationValue.Value = "1";
                            return false;
                        }
                        break;
                }
                if (!String.IsNullOrEmpty(emailtosend.EmailBody))
                {
                    // Get email addresses for users
                    emailtosend.EmailAddresses = GetUserEmailAddressesforClinic(clinicsitecode);
                    if (emailtosend.EmailAddresses.Count > 0)
                    {
                        // check for and remove any duplicate email addresses
                        List<string> distinctemailaddresses = emailtosend.EmailAddresses.Distinct().ToList();
                        emailtosend.EmailAddresses = distinctemailaddresses;
                        emailtosend.ClinicSiteCode = clinicsitecode;
                        result = PrepareFinalizedEmailNotification(emailtosend);
                    }
                    else
                    {
                        emailtosend.EmailAddress = "None Found";
                        emailtosend.EmailRecipient = "Clinic User";
                        emailtosend.ClinicSiteCode = clinicsitecode;
                        emailtosend.EmailAddresses.Add(emailtosend.EmailAddress);

                        result = PrepareFinalizedEmailNotification(emailtosend);
                    }
                }
            }
            return result;
        }

        protected bool PrepareClinicAdminEmailNotification(string clinicsitecode, [Optional] string reason, [Optional] DateTime? effectiveDate)
        {
            var result = false;
            EmailMessageEntity emailtosend = new EmailMessageEntity();

            // Get Email message for admin - Lab-ClinicEnabled or Lab-CinicEnabledDate
            if (effectiveDate != null)
            {
                EmailMessageType = GetEmailMessageByType("Lab-ClinicEnabledDate");
                emailtosend.EmailBody = EmailMessageType.MessagePart1 +
                                    SiteCode + EmailMessageType.MessagePart2 + " " +
                                    effectiveDate + "." +
                                    EmailMessageType.MessagePart3;
                emailtosend.EmailType = "Lab-ClinicEnabledDate";
            }
            else
            {
                EmailMessageType = GetEmailMessageByType("Lab-ClinicEnabled");
                emailtosend.EmailBody = EmailMessageType.MessagePart1 + 
                                     SiteCode + "." + 
                                     EmailMessageType.MessagePart2;
                emailtosend.EmailType = "Lab-ClinicEnabled";
            }

            if (!String.IsNullOrEmpty(emailtosend.EmailBody))
            {
                // Get email addresses for admins
                emailtosend.EmailAddresses = GetAdminEmailAddressesClinic(clinicsitecode);
                if (emailtosend.EmailAddresses.Count > 0)
                {
                    // check for and remove any duplicate email addresses
                    List<string> distinctemailaddresses = emailtosend.EmailAddresses.Distinct().ToList();
                    emailtosend.EmailAddresses = distinctemailaddresses;
                    emailtosend.ClinicSiteCode = clinicsitecode;
                    result = PrepareFinalizedEmailNotification(emailtosend);
                }
                else
                {
                    emailtosend.EmailAddress = "None Found";
                    emailtosend.EmailRecipient = "Clinic Admin";
                    emailtosend.ClinicSiteCode = clinicsitecode;
                    emailtosend.EmailAddresses.Add(emailtosend.EmailAddress);
                    result = PrepareFinalizedEmailNotification(emailtosend);
                }
            }
            return result;
        }

        protected bool PrepareFinalizedEmailNotification(EmailMessageEntity email)
        {
            var result = false;

            if (EmailNotification == null)
            {
                EmailNotification = new List<EmailMessageEntity>();
            }
            else
            {
                EmailNotification.Clear();
            }

            foreach (string emailaddress in email.EmailAddresses)
            {
                EmailMessageEntity emailtosend = new EmailMessageEntity();

                // check for no email found
                if (emailaddress != "None Found")
                {
                    //parse email to get recipient
                    if (emailaddress.Split(',').Length > 0)
                    {
                        emailtosend.EmailRecipient = emailaddress.Split(',')[1];
                        emailtosend.EmailAddress = emailaddress.Split(',')[0];
                    }
                    else
                    {
                        emailtosend.EmailRecipient = emailaddress;
                        emailtosend.EmailAddress = emailaddress;
                    }
                }
                else
                {
                    emailtosend.EmailAddress = email.EmailAddress;
                    emailtosend.EmailRecipient = email.EmailRecipient;
                }

                emailtosend.ClinicSiteCode = email.ClinicSiteCode;
                emailtosend.EmailBody = email.EmailBody;
                emailtosend.EmailSubject = "SRTSweb Lab Notification for Clinic " + emailtosend.ClinicSiteCode + "";
                emailtosend.EmailType = email.EmailType;
#if Debug
                emailtosend.EmailSubject = "SRTSweb Lab Notification (Development Test) for Clinic " + email.ClinicSiteCode + "";
#endif
                EmailNotification.Add(emailtosend);
            }
            if (EmailNotification.Count > 0)
            {
                foreach (EmailMessageEntity emailrecipient in EmailNotification)
                {
                    if (emailrecipient.EmailAddress != "None Found")
                    {
                        // try to send email
                        if (ValidateEmailIsReadytoSend(emailrecipient))
                        {
                            result = SendEmailMessage(emailrecipient);
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {

                        result = true;
                    }
                }
            }
            return result;
        }

        protected bool ValidateEmailNotification(EmailMessageEntity notification)
        {
            var result = true;
            if (notification.LabSiteCode == null) result = false;
            if (notification.ClinicSiteCode == null) result = false;
            if (notification.EmailBody == null) result = false;
            if (notification.EmailRecipient == null) result = false;
            if (notification.EmailAddress == null) result = false;
            if (notification.EmailType == null) result = false;
            return result;
        }

        protected bool ValidateEmailIsReadytoSend(EmailMessageEntity email)
        {
            var result = true;
            if (email.EmailSubject == null) result = false;
            if (email.EmailBody == null) result = false;
            if (email.EmailRecipient == null) result = false;
            if (email.EmailAddress == null) result = false;
            if (email.ClinicSiteCode == null) result = false;
            return result;
        }

        protected bool SendEmailMessage(EmailMessageEntity email)
        {
            var result = true;
            try
            {
                if (email != null)
                {
                    var m = new SrtsWeb.BusinessLayer.Concrete.MailService();
                    m.SendEmail(email.EmailBody, ConfigurationManager.AppSettings["FromEmail"], new List<string>(){ email.EmailAddress }, email.EmailSubject);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message + "-" + ex.InnerException;
                if (ex.Message.Contains("Failure sending mail"))
                {
#if DEBUG
                        return true;
#endif
                }
                else
                {
                    result = false;
                    ex.LogException(String.Format("An error occurred when sending the email notification - {0}, {1}", ex.Message, ex.InnerException));
                }
            }
            return result;
        }

        protected void InsertUpdateLabMTPPref(SitePrefLabMTPEntity entity)
        {
            if (entity != null)
            {
                var msg = String.Empty;
                msg = p.InsertUpdateLabMTPPref(entity) ? "Success" : "Fail";
                //if (msg == "Fail")
                //{
                //    ShowErrorDialog("There was an error updating your mail-to-patient preferences.");
                //}
                //else
                //{
                //    ShowConfirmDialog("Your mail-to-patient preferences were successfully updated!");
                //}
            }
        }
        /// <summary>
        /// Validates the 'emailnotification and inserts it into the database if valid.
        /// </summary>
        /// <param name="emailnotification"></param>
        /// <returns>True if successfully inserted.  False if validation fails or if not successfully inserted into database.</returns>
        protected Boolean InsertEmailNotificationSent(EmailMessageEntity emailnotification)
        {
            if (ValidateEmailNotification(emailnotification))
            {
                return p.InsertLabNotificationEmailSent(emailnotification);
            }
            else { return false; }
        }

        protected void rblMailToPatient_SelectedIndexChanged(object sender, EventArgs e)
        {
            /////// need to check if was set to on, if so ask reason.
            if (rblMailToPatient.SelectedIndex == 1)
            {
                rblNoMailToPaientReason.Visible = true;
            }
            else
            {
                rblNoMailToPaientReason.Visible = false;
            }
        }

        protected void btnCancelClinicMTP_Click(object sender, EventArgs e)
        {
             Page.Response.Redirect(Page.Request.Url.ToString(), false);
        }

        private EmailMessageEntity GetEmailMessageByType(string emailtype)
        {
            return emailpresenter.GetEmailMessageByType(emailtype);
        }

        private List<EmailMessageEntity> GetAllEmailMesageTypes()
        {
            return emailpresenter.GetAllEmailMessageTypes();
        }

        private List<string> GetAdminEmailAddressesClinic(string sitecode)
        {
            return emailpresenter.GetAdminEmailAddresses(sitecode);
        }

        private List<string> GetUserEmailAddressesforClinic(string sitecode)
        {
            return emailpresenter.GetUserEmailAddresses(sitecode);
        }


        public EmailMessageEntity InsertEmailMessage(EmailMessageEntity emailmessage)
        {
            throw new NotImplementedException();
        }

        public EmailMessageEntity UpdateEMailMessage(EmailMessageEntity emailmessage)
        {
            throw new NotImplementedException();
        }


#region INTERFACE PROPERTIES

        public SitePrefLabMTPEntity SitePrefLabMTPEntity
        {
            get { return ViewState["SitePrefsLabMTP"] as SitePrefLabMTPEntity; }
            set { ViewState["SitePrefsLabMTP"] = value; }
        }

        private string _errMessage;
        public string ErrMessage
        {
            get { return _errMessage; }
            set { _errMessage = value; }
        }

        private string _siteCode;
        public string SiteCode
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCode;
            }
            set
            {
                _siteCode = value;
            }
        }

        private IEnumerable<SiteCodeEntity> _Clinics;
        public IEnumerable<SiteCodeEntity> Clinics
        {
            get
            {
                return _Clinics;
            }
            set
            {
                _Clinics = value;
                List<string> inactiveClinics = new List<string>();
                if (DisabledClinics != null && DisabledClinics.ToList().Count > 0)
                {
                    // get the list of disabled clinics
                    foreach (SitePrefLabMTPEntity mte in DisabledClinics)
                    {
                        inactiveClinics.Add(mte.ClinicSiteCode);
                    }
                }
                    // remove the disabled clinics from the list of all clinis for the lab
                    var activeClinics = _Clinics.Where(x => !inactiveClinics.Contains(x.SiteCode)).ToList();
                    var disabledClinics = _Clinics.Where(x => inactiveClinics.Contains(x.SiteCode)).ToList();

                    // put the active clinics in the active clinics checkbox list
                    _Clinics = activeClinics;
                    foreach (SiteCodeEntity clinic in _Clinics)
                    {
                        ListItem liClinic = new ListItem();
                        liClinic.Text = String.Format("{0} - {1}", clinic.SiteCode, Helpers.ToTitleCase(clinic.SiteName));
                        liClinic.Value = clinic.SiteCode.ToString();
                        liClinic.Attributes.Add("class", "site");
                        chkClinics.Items.Add(liClinic);
                    }

                    // put the disabled clinics in the disabled clinics checkbox list
                    foreach (SiteCodeEntity clinic in disabledClinics)
                    {
                        ListItem liClinicsDisabled = new ListItem();
                        liClinicsDisabled.Text = String.Format("{0} - {1}", clinic.SiteCode, Helpers.ToTitleCase(clinic.SiteName));
                        liClinicsDisabled.Value = clinic.SiteCode.ToString();
                        liClinicsDisabled.Attributes.Add("class", "site");
                        chkDisabledList.Items.Add(liClinicsDisabled);
                    }
              
            }
        }

        private IEnumerable<SitePrefLabMTPEntity> _DisabledClinics;
        public IEnumerable<SitePrefLabMTPEntity> DisabledClinics
        {
            get
            {
                return _DisabledClinics;
            }
            set
            {
                _DisabledClinics = value;
                _DisabledClinics = (from c in value
                                    where c.ClinicActionRequired == "True"
                                    select c).ToList();
            }
        }



        private IEnumerable<SitePrefLabMTPEntity> _EnabledClinics;
        public IEnumerable<SitePrefLabMTPEntity> EnabledClinics
        {
            get
            {
                return _EnabledClinics;
            }
            set
            {
                _EnabledClinics = value;
                _EnabledClinics = (from c in value
                                    where c.ClinicActionRequired == "False"
                                    select c).ToList();
            }
        }



        private string _isLabMailToPatient;
        public string IsLabMailToPatient
        {
            get
            {
                return _isLabMailToPatient;
            }
            set
            {
                _isLabMailToPatient = value;
            }
        }

        private string _isCapabilityOn;
        public string IsCapabilityOn
        {
            get
            {
                return _isCapabilityOn;
            }
            set
            {
                _isCapabilityOn = value;
                this.rblMailToPatient.SelectedValue = IsLabMailToPatient.ToString();
                hdfLabMTPStatus.Value = IsLabMailToPatient.ToString();
            }
        }

        private string __noCapabilityReason;
        public string NoCapabilityReason
        {
            get
            {
                return __noCapabilityReason;
            }
            set
            {
                __noCapabilityReason = value;
                this.rblNoMailToPaientReason.SelectedValue = __noCapabilityReason;
            }
        }

        private int _capacity;
        public Int32 Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                _capacity = value;
            }
        }

        private String _comments;
        public String Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
                this.txtOtherComments.Text = _comments;
            }
        }

        private String _modifiedBy;
        public String ModifiedBy
        {
            get
            {
                return _comments;
            }
            set
            {
                _modifiedBy = value;
            }
        }


        private string _emailtype;
        public String EmailType
        {
            get
            {
                return _emailtype;
            }
            set
            {
                _emailtype = value;
            }
        }
        private string _messagepart1;
        public String MessagePart1
        {
            get
            {
                return _messagepart1;
            }
            set
            {
                _messagepart1 = value;
            }
        }
        private string _messagepart2;
        public String MessagePart2
        {
            get
            {
                return _messagepart2;
            }
            set
            {
                _messagepart2 = value;
            }
        }
        private string _messagepart3;
        public String MessagePart3
        {
            get
            {
                return _messagepart3;
            }
            set
            {
                _messagepart3 = value;
            }
        }
        private string _messagepart4;
        public String MessagePart4
        {
            get
            {
                return _messagepart4;
            }
            set
            {
                _messagepart4 = value;
            }
        }
        private EmailMessageEntity _emailmessagetype;
        public EmailMessageEntity EmailMessageType
        {
            get
            {
                return _emailmessagetype;
            }
            set
            {
                _emailmessagetype = value;
            }
        }
        private List<EmailMessageEntity> _emailmessagetypes;
        public List<EmailMessageEntity> EmailMessageTypes
        {
            get
            {
                return _emailmessagetypes;
            }
            set
            {
                _emailmessagetypes = value;
            }
        }

        private List<string> _emailaddresses;
        public List<string> EmailAddresses
        {
            get
            {
                return _emailaddresses;
            }
            set
            {
                _emailaddresses = value;
            }
        }

        private string _emailaddress;
        public String EmailAddress
        {
            get
            {
                return _emailaddress;
            }
            set
            {
                _emailaddress = value;
            }
        }

        private string _emailsubject;
        public String EmailSubject
        {
            get
            {
                return _emailsubject;
            }
            set
            {
                _emailsubject = value;
            }
        }

        private string _emailbody;
        public String EmailBody
        {
            get
            {
                return _emailbody;
            }
            set
            {
                _emailbody = value;
            }
        }

        private List<EmailMessageEntity> _emailnotification;
        public List<EmailMessageEntity> EmailNotification
        {
            get
            {
                return _emailnotification;
            }
            set
            {
                _emailnotification = value;
            }
        }

#endregion
    }
}

