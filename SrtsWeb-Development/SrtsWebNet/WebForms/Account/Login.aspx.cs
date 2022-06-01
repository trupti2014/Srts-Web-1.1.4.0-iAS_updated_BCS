using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.CustomProviders;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Account;
using SrtsWeb.Views.Account;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.Account
{
    public partial class Login : PageBase, IAuthorizationView
    {
        private CustomEventTracer tracer;

        private String loginUserData
        {
            get
            {
                var cc = Request.ClientCertificate;
                if (cc.IsPresent && cc.IsValid)
                {
                    var x = new X509Certificate2(cc.Certificate);
                    return String.Format("{0}: {1}", this.LoginUser.UserName, x.Subject);
                }
                return this.LoginUser.UserName;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (mySession == null)
                mySession = new SRTSSession();

            if (!Page.IsPostBack)
            {
                if (!this.mySession.SecurityAcknowledged)
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "SecurityMessage", "DoSecurityDialog();", true);

                if (User.Identity.IsAuthenticated && mySession.CacRegistrationCode == 0)
                {
                    Response.Redirect("~/WebForms/Default.aspx?a=1&id=1&ss=1", true);
                    litLoginMessage.Text = string.Empty;
                }

                if (mySession.CacRegistrationCode > 1)
                {
                    mySession.SecurityAcknowledged = true;
                    pnlMainContent.Visible = true;
                    litLoginMessage.Text = string.Empty;
                    tbcLogin.ActiveTabIndex = 0;

                    if (mySession.CacRegistrationCode == 3)
                    {
                        lblCacError.Text = "Invalid cert selected.<br>";
                    }
                    if (mySession.CacRegistrationCode == 4)
                    {
                        tbcLogin.ActiveTabIndex = 0;
                        lblCacError.Text = "Register CAC prior to login.<br>";
                    }

                    if (mySession.CacRegistrationCode == 5)
                    {
                        tbcLogin.ActiveTabIndex = 1;
                        divLoginPanel.Visible = false;
                        lblUTAQuestion.Text = string.Empty;
                        lblUTAQuestion.Text = "Would you like to link<br>this Account to another <br>SRTS Account?<br>";
                        divUserNameToAccount.Visible = true;
                        Master._ContentAuthenticated.Visible = false;
                    }

                    mySession.CacRegistrationCode = 0;
                    lblCacError.Visible = true;
                }
            }

            if (mySession != null)
            {
                pnlMainContent.Visible = true;
                if (mySession.CacRegistrationCode > 0)
                {
                    mySession.SecurityAcknowledged = true;
                    tbcLogin.ActiveTabIndex = 0;
                    lnkbRegisterCAC.Visible = false;
                    litLoginMessage.Text = string.Empty;

                    if (mySession.CacRegistrationCode == 1)
                    {
                        tbcLogin.ActiveTabIndex = 1;
                        lblRegisterCAC.Visible = true;
                        lblRegisterCAC.Text = "Please login to Register a CAC";
                    }

                    if (mySession.CacRegistrationCode == 2)
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["CacRequestUrl"], false);
                    }

                    lblCacError.Visible = true;
                }
            }
            var p = new AuthorizationPresenter(this);
            p.GetPublicAnnouncements();
        }

        protected void SetConnectionData()
        {
            Session["userName"] = this.LoginUser.UserName;

            var un = this.LoginUser.UserName.ToLower();
            var trainingSites = ConfigurationManager.AppSettings["trainingSites"].Split(new[] { ',' }).ToList();
            Session["connStrNm"] = trainingSites.FirstOrDefault(x => un.StartsWith(x.ToLower())) ?? "SRTS";

            // Ensure that the membership provider's connection string is correct
            FieldInfo fiMP = typeof(SrtsMembership).BaseType.GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
            fiMP.SetValue(Membership.Provider, ConfigurationManager.ConnectionStrings[Globals.ConnStrNm].ConnectionString);

            // Ensure that the role provider's connection string is correct
            FieldInfo fiRole = Roles.Provider.GetType().GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
            fiRole.SetValue(Roles.Provider, ConfigurationManager.ConnectionStrings[Globals.ConnStrNm].ConnectionString);
        }

        protected void LoginUser_LoggingIn(object sender, LoginCancelEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_LoginUser_LoggingIn", this.loginUserData))
#endif
            try
            {
#if DEBUG
                    using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_SetConnectionData", this.loginUserData))
#endif
                        SetConnectionData();

                    // If the userName entered reflects a current user and the user is not locked out and isApproved
                    // check to see if the user is CAC enabled and if so, set the CacRegistrationCode = 7 to alert to login with CAC
                    hdfCacRegistrationCode.Value = string.Empty;
                    tbcLogin.ActiveTabIndex = 1;

                    var thisUser = Membership.GetUser(this.LoginUser.UserName);

                    if (thisUser.IsNull())
                    {
                        this.LoginUser.FailureText = "Current username or password is invalid,<br />contact your site administrator for assistance.";
                        LogEvent(String.Format("Username: {0} unsuccessfully logged on at {1}.", this.LoginUser.UserName, DateTime.Now));
                        return;
                    }

                    if (!thisUser.IsLockedOut || !thisUser.IsApproved)
                    {
                        AuthorizationPresenter _presenter = new AuthorizationPresenter(this);
#if DEBUG
                        using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_GetAuthorizationsByUserName", this.loginUserData))
#endif
                            _presenter.GetAuthorizationsByUserName(LoginUser.UserName);

                        if (isCacOnFile)
                        {
                            mySession.CacRegistrationCode = 7; // set code to user is CAC enabled

                            // Set the value in a hidden field so javaScript can check it on page load and display alert
                            hdfCacRegistrationCode.Value = mySession.CacRegistrationCode.ToString();

                            // Clear the login failure text and UserName field
                            // Set active tab to CAC login tab
                            LoginUser.FailureText = string.Empty;
                            ((TextBox)LoginUser.FindControl("UserName")).Text = string.Empty;
                            LoginUser.UserName = string.Empty;
                            tbcLogin.ActiveTabIndex = 0;
                            e.Cancel = true;
                            return;
                        }
                    }

                    // Set the last logon date/time here.  This MUST be called before the ValidateUser method gets called.
                    mySession.LastLoginDate = thisUser.LastLoginDate;
#if DEBUG
                    using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_ValidateUser", this.loginUserData))
#endif
                        if (!Membership.ValidateUser(this.LoginUser.UserName, this.LoginUser.Password))
                        {
                            this.LoginUser.FailureText = "Current username or password is invalid,<br />contact your site administrator for assistance.";
                            LogEvent(String.Format("Username: {0} unsuccessfully logged on at {1}.", this.LoginUser.UserName, DateTime.Now));
                            return;
                        }

                    // Get the membership record for the loggedin user
                    MembershipUser m;
#if DEBUG
                    using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_GetUser", this.loginUserData))
#endif
                        m = Membership.GetUser(this.LoginUser.UserName);

                    if (m == null) { e.Cancel = true; return; }
                    if (!m.IsApproved) { e.Cancel = true; return; }

                    // Get the number of days between password resets
                    var pwdExpire = ConfigurationManager.AppSettings["passwordDuration"];

                    // Create Expiration date for comparison
                    var dtExp = m.LastPasswordChangedDate.AddDays(Convert.ToDouble(pwdExpire));

                    // If the created date and the last password change date are the same OR if the password expiration date is past due then force the user to change their password.
                    this.lblMessage.Visible = false;
                    if (m.CreationDate.Equals(m.LastPasswordChangedDate) || dtExp < DateTime.Now)
                    {
                        if (dtExp < DateTime.Now)
                        {
                            this.lblMessage.Text = "Your password has expired";
                            this.lblMessage.Visible = true;
                        }

                        ShowChangePassword(true);
                        this.cpNewFact2.UserName = this.LoginUser.UserName;
                        e.Cancel = true;
                        return;
                    }

                    // If the users account hasn't been accessed in the specified amount of days then disable the account.
                    var unusedDays = ConfigurationManager.AppSettings["unusedAccountDuration"];
                    var dtUnused = m.LastLoginDate.AddDays(Convert.ToDouble(unusedDays));
                    if (dtUnused < DateTime.Now)
                    {
                        m.IsApproved = false;
#if DEBUG
                        using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_Membership.UpdateUser", this.loginUserData))
#endif
                            Membership.UpdateUser(m);
                        e.Cancel = true;
                    }
                }
                catch (Exception ex)
                {
                    ex.TraceErrorException();
                }

        }

        protected void LoginUser_Authenticate(object sender, AuthenticateEventArgs e)
        {
            try
            {
                tracer = new CustomEventTracer("SrtsTraceListener", this.loginUserData, SrtsTraceSource.LoginSource);

                MembershipUser u;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_Membership.GetUser", this.loginUserData))
#endif
                    u = Membership.GetUser(this.LoginUser.UserName);

                if (u.IsNull()) return;

                var p = CustomProfile.GetProfile(this.LoginUser.UserName);

                if (p == null) return;
                if (String.IsNullOrEmpty(p.SiteCode))
                {
                    if (String.IsNullOrEmpty(p.PrimarySite)) return;
                    p.SiteCode = p.PrimarySite;

                    tracer.TraceVerbose("Save site code '{0}' to user profile for {1}.", new Object[] { p.SiteCode, this.loginUserData });
                    CustomProfile.SaveLoggedInSiteCode(p);
                }

                String[] r;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_GetRolesForUser", this.loginUserData))
#endif
                    r = Roles.GetRolesForUser(this.LoginUser.UserName);

                if (r.Length <= 0) return;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_Membership.Provider.ValidateUser", this.loginUserData))
#endif
                {
                    if (Membership.Provider.ValidateUser(LoginUser.UserName, LoginUser.Password) == true)
                    {
                        tracer.TraceVerbose("Set auth cookie.");
                        FormsAuthentication.SetAuthCookie(LoginUser.UserName, false);

                        if (isCacOnFile && mySession != null && mySession.CacRegistrationCode != 1)
                        {
                            btnCacNo.Visible = false;
                            btnCacYes.Text = "OK";
                            divLoginPanel.Visible = false;
                            lblCacQuestion.Visible = true;
                            lblCacQuestion.Text = "You must use a CAC to login<br>with this account.</br></br><br>Please insert CAC and click ok.";
                            divCacQuestion.Visible = true;
                            mySession.CacRegistrationCode = 2;
                            pnlMainContent.Visible = true;
                            FormsAuthentication.SignOut();
                        }
                        else
                        {
                            if (mySession != null && mySession.CacRegistrationCode == 1)
                            {
                                divLoginPanel.Visible = false;
                                lblCacQuestion.Text = "Insert CAC and click Register. <br/><br/>Click Cancel to proceed without a CAC";
                                btnCacYes.Visible = true;
                                btnCacYes.Text = "Register";
                                btnCacNo.Text = "Cancel";
                                btnCacNo.Visible = true;
                                divCacQuestion.Visible = true;
                                pnlMainContent.Visible = true;
                                mySession.CacRegistrationCode = 0;
                            }
                            else
                            {
                                var _presenter = new AuthorizationPresenter(this);
#if DEBUG
                                using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "Login_GetAuthorizationsByUserName", this.loginUserData))
#endif
                                    _presenter.GetAuthorizationsByUserName(LoginUser.UserName);

                                divCacQuestion.Visible = false;
                                divLoginPanel.Visible = false;
                                divSiteRole.Visible = false;
                                pnlMainContent.Visible = true;

                                if (isMultipleAccounts)
                                {
                                    gvSitesRoles.DataSource = dtAuthorizations;
                                    gvSitesRoles.DataBind();
                                    divSiteRole.Visible = true;
                                }
                                else
                                {
                                    LogEvent("User {0} logged in at {1}.", new Object[] { this.loginUserData, DateTime.Now });
                                    SessionService.CreateAndStoreSessionTicket(this.LoginUser.UserName);
                                    Response.Redirect("~/WebForms/default.aspx?ss=1", false);
                                }
                            }
                        }
                    }
                    else
                    {
                        pnlMainContent.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.TraceErrorException();
            }
        }

        protected Boolean ValidatePassword()
        {
            return LoginUser.Password.ValidatePasswordCharacters();
        }

        protected void btnCacYes_Click(object sender, EventArgs e)
        {
            Response.Redirect(ConfigurationManager.AppSettings["CacRequestUrl"], false);
        }

        protected void btnCacNo_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/default.aspx?ss=1");
        }

        public void gvSitesRoles_SelectedIndexChanged(Object sender, EventArgs e)
        {
            FormsAuthentication.RedirectFromLoginPage(gvSitesRoles.SelectedDataKey.Value.ToString(), false);
        }

#region IAuthorization View Implementation

        public DataTable dtAuthorizations
        {
            get;
            set;
        }

        public bool isCacOnFile
        {
            get;
            set;
        }

        public bool isUserFound
        {
            get;
            set;
        }

        public bool isSsoUserNameFound
        {
            get;
            set;
        }

        public bool isMultipleAccounts
        {
            get;
            set;
        }

#endregion IAuthorization View Implementation

        protected void btnValidateButtonUTA_Click(object sender, EventArgs e)
        {
            string _user = HttpContext.Current.User.Identity.Name.ToString();

            if (Membership.ValidateUser(this.txbUserNameUTA.Text, this.txbFact2UTA.Text) == true)
            {
                AuthorizationPresenter _presenter = new AuthorizationPresenter(this);

                _presenter.UpdateAuthorizationSSOByUserName(_user, this.txbUserNameUTA.Text);

                _presenter.GetAuthorizationsByUserName(_user);

                gvSitesRoles.DataSource = dtAuthorizations;
                gvSitesRoles.DataBind();

                divUserNameToAccount.Visible = false;

                divLoginPanel.Visible = false;
                divCacQuestion.Visible = false;
                divSiteRole.Visible = true;
                pnlMainContent.Visible = true;

                lblSiteRoles.Text = "<BR><p>The following accounts are now linked:<p><BR>";
            }
            else
            {
                pnlUTA.Visible = true;
                divLoginPanel.Visible = false;
                divCacQuestion.Visible = false;
                divSiteRole.Visible = false;
                pnlMainContent.Visible = true;
                FailureText.Text = "UserName/Password is incorrect OR Accounts could not be linked, contact your site administrator for assistance..<BR>";
            }
        }

        protected void btnUTAYes_Click(object sender, EventArgs e)
        {
            pnlUTAQuestion.Visible = false;
            pnlUTA.Visible = true;
            lblUTA.Text = "Enter the Account to be link<BR>";
        }

        protected void btnUTANo_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/default.aspx?ss=1");
        }

        protected void btnSkipUTA_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/default.aspx?ss=1");
        }

        protected void lnkbRegisterCAC_Click(object sender, EventArgs e)
        {
            tbcLogin.ActiveTabIndex = 1;
            mySession.CacRegistrationCode = 1;
            Response.Redirect("~/WebForms/Account/login.aspx", false);
        }

#region ANNOUNCEMENTS

        public List<CMSEntity> Announcements
        {
            set
            {
                this.rpAnnouncements.DataSource = value;
                this.rpAnnouncements.DataBind();
            }
        }

        protected void rpAnnouncements_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e != null && e.Item.DataItem is CMSEntity)
            {
                var i = e.Item.DataItem as CMSEntity;
                var h = (Literal)e.Item.FindControl("litHeadline");
                var s = (Literal)e.Item.FindControl("litSummary");

                string cTitle = Server.HtmlEncode(i.cmsContentTitle);
                string cDate = Server.HtmlEncode(i.cmsCreatedDate.ToString(Globals.DtFmt));
                string author = Server.HtmlEncode(String.Format("{0} {1}", i.AuthorFirstName, i.AuthorLastName));
                string cmsBody = Server.HtmlEncode(i.cmsContentBody);
                h.Text = String.Format("<p> {0}<br /><span class=\"newsdate\">Posted: {1} by {2}</span></p>",
                  cTitle,
                  cDate,
                  author);
                s.Text = String.Format("<p class=\"newssummary\">{0}</p>", cmsBody);
            }
        }

#endregion ANNOUNCEMENTS

        protected void CancelPushButton_Click(object sender, EventArgs e)
        {
            ShowChangePassword(false);
        }

        protected void lbtnChangePwd_Click(object sender, EventArgs e)
        {
            ShowChangePassword(true);
        }

        private void ShowChangePassword(Boolean show)
        {
            this.divLoginPanel.Visible = !show;
            this.divChangePassword.Visible = show;
        }

        protected void cpNewFact2_ChangingPassword(object sender, LoginCancelEventArgs e)
        {
            this.cpNewFact2.ChangePasswordFailureText = String.Empty;
            var err = new StringBuilder();

            var p = cpNewFact2.NewPassword;
            var r = String.Empty;
            p.ValidatePasswordComplexity(out r);

            var g = Guid.NewGuid().ToString("N");
            var c = new HttpCookie(ConfigurationManager.AppSettings["DeltaPWordKey"])
            {
                HttpOnly = true,
                Secure = true,
                Value = g
            };
            Response.Cookies.Set(c);

            if (!String.IsNullOrEmpty(r))
                err.AppendLine(r);

            if (String.IsNullOrEmpty(err.ToString()))
            {
                this.lblMessage.Visible = false;
                return;
            }

            this.cpNewFact2.ChangePasswordFailureText = err.ToString();
            e.Cancel = true;
        }

        protected void bSecurityMessage_Click(object sender, EventArgs e)
        {
            if (this.mySession.IsNull())
                this.mySession = new SRTSSession();

            this.mySession.SecurityAcknowledged = true;
        }

        protected void CustPasswordDiff_ServerValidate(object source, ServerValidateEventArgs args)
        {
            this.lblChangeError.Text = String.Empty;
            var oldP = cpNewFact2.Controls[0].FindControl("CurrentPassword") as TextBox;
            var s1 = oldP.Text;
            var s2 = args.Value;

            var good = Enumerable.Range(0, Math.Min(s1.Length, s2.Length)).Count(i => s1[i] != s2[i]);
            var minChars = ConfigurationManager.AppSettings["minPasswordCharChange"].ToInt32();
            args.IsValid = good.GreaterThanET(minChars);
            if (args.IsValid) return;
            this.lblChangeError.Text = String.Format("At least {0} characters must be different between the old and new password", minChars);
        }

        protected void cpNewFact2_ChangePasswordError(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(((SrtsMembership)Membership.Provider).ChangeError)) return;
            else
                this.cpNewFact2.ChangePasswordFailureText = ((SrtsMembership)Membership.Provider).ChangeError;
        }

        protected void cpNewFact2_ContinueButtonClick(object sender, EventArgs e)
        {
            ShowChangePassword(false);
        }

        protected void ddlChooseCert_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cert = ddlChooseCert.SelectedIndex;

            switch (cert)
            {
                case 0:
                    break;
                case 1:
                    //// Entrust Cert
                    Session.Add("Cert", "Entrust");
                    break;
                case 2:
                    // Topaz Cert
                    Session.Add("Cert", "Topaz");
                    break;
                case 3:
                    // VA Cert
                    Session.Add("Cert", "VA");
                    break;
            }
            Response.Redirect(ConfigurationManager.AppSettings["CacRequestUrl"], false);
        }
    }
}