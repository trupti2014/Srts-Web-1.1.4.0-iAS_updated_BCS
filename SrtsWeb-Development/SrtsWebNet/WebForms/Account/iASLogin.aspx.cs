using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Account;
using SrtsWeb.Views.Account;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.Account
{
    public partial class iASLogin : PageBase, ICertificateInfoView
    {
        public string StrEDIPI;
        public string StrIasAuthId;
        public string StrCertType;
        protected void Page_Load(object sender, EventArgs e)
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "iASLogin_Page_Load", MyUserID))
#endif
            {
                try
                {
                    CustomLogger.Log("load", this.mySession.LogTriggers, "In page load.");

                    //set the URL for the redirect "OK" link button.
                    this.NonProxyServer.PostBackUrl = String.Format("{0}?ss=1", ConfigurationManager.AppSettings["NonProxyWebServerUrl"]);
                    
                    // remove Master page menus
                    Master._MainMenu.Visible = false;
                    Master._ContentAuthenticated.Visible = true;

                    // process certificate(CAC) login
                    ProcessCertificateLogin();
                }
                catch (Exception ex)
                {
                    Exception exx = new Exception();
                    exx.LogException("In iASLogin" + ex.Message);
                }
            }
        }
        /// <summary>
        /// This method begins the process of CAC certificate processing by calling the 'PullCACCertificate()' method.
        /// </summary>
        protected void ProcessCertificateLogin()
        {
            //Pull CAC cert and extract DodID and IssuerID information.
            StrEDIPI = Request.Headers["DOD_EDI_PN_ID"];
            StrIasAuthId = Request.Headers["IAS-AUTH-ID"];
            StrCertType = Request.Headers["CLIENT-CERT-TYPE"];
            //"CAC" User authenticated with certificate issued by a DOD Certificate Authority
            //"VA" User authenticated with certificate issued by a VA Certificate Authority
            //"ECA" User authenticated with certificate issued by an ECA Certificate Authority such as ORC or IdenTrust
            //"ALT" User authenticated with certificate issued by a DOD Certificate Authority and the certificate UPN format is either mJAD or DISA EPUAS
            //"UNK" User authenticated with certificate issued by an unknown Certificate Authority 
            if ((StrEDIPI != null) || (StrIasAuthId != null))
            {
                StrBlueCoatCert = StrEDIPI;
                BluecoatCacFound = true;

                //Trupti comment out
                //LogEvent(String.Format("Ias-Auth-Id: {0}  Client-Cert-Type:{1}  Dod-Edi-Pn-Id: {2}", StrIasAuthId, StrCertType, StrEDIPI));
                //if (HttpContext.Current.User.Identity.IsAuthenticated)
                //{
                //    LogEvent("HttpContext.Current.User.Identity.IsAuthenticated=true ");
                //}
            }

            if (BluecoatCacFound)
            {
                CertificateFound();
            }
            else
            {
                CertificateNotFound();
            }
        }

        /// <summary>
        /// This method processes the valid NonBluecoatCAC or BlueCoadCAC certificate that is found. 
        /// </summary>
        protected void CertificateFound()
        {
            //Create certificate presenter object (gets all User accounts associated with the CAC ID).
            CertificateInfoPresenter _presenter = new CertificateInfoPresenter(this);

            #region CERT FOUND
            CustomLogger.Log("load", this.mySession.LogTriggers, "Cert found");

            if (BluecoatCacFound)
            {
                CAC_ID = StrEDIPI;
                // Add the subject to mySession.MyUserID
                mySession.CertificateSubject = "iAS_ID:" + StrEDIPI;
                //Possibly could be Monitoring soft cert from monitoring application
                if (StrCertType != null)
                {
                    CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("Issuer Found in Cert - {0}", StrCertType));
                    //Check CAC Type (i.e VA vs. DoD)

                    if (StrCertType == "CAC")
                    {
                        //ISSUERNAME
                        issuerName = "DOD";
                        IsDoDCert = true;
                    }
                    if (StrCertType == "VA")
                    {
                        //ISSUERNAME
                        issuerName = "Veterans Affairs User";
                        IsVACert = true;
                    }
                    if (StrCertType == "ECA")
                    {
                        //ISSUERNAME
                        issuerName = "Entrust Managed Services SSP";
                        IsVACert = false;
                    }
                }
                else
                {
                    //ISSUERNAME
                    issuerName = "DOD";
                    IsDoDCert = true;
                }
                if (StrEDIPI == "6132435467") //topazbpm.med.osd.mil
                {
                    CAC_ID = ConfigurationManager.AppSettings["MonitoringCertCAC_ID"]; //topazbpm
                    issuerName = "DOD";
                    IsDoDCert = true;
                    IsPIVCert = true;
                }
            }

            #region TEST CAC VARIABLE

            CustomLogger.Log("load", this.mySession.LogTriggers, String.Format("CAC_ID: {0}, Issuer: {1}", CAC_ID, issuerName));

            if ((CAC_ID != string.Empty) && (issuerName != string.Empty))
            {

                CustomLogger.Log("load", this.mySession.LogTriggers, String.Format("User is authenticated: {0}", User.Identity.IsAuthenticated));


                #region NOT AUTHENTICATED

                CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("In not authenticated...CAC_ID: {0}, Issuer: {1}", CAC_ID, issuerName));
                _presenter.GetAuthorizationsByCAC_ID(CAC_ID, issuerName, String.Empty);
                CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("iASLogin Not Authenticated loop - Issuer Name variable - {0}", issuerName));

                if (isCACFound)
                {
                    #region CAC LOGIN
                    if (!isMultipleAccounts)
                    {
                        gvSitesRoles.DataSource = dtSitesRoles;
                        gvSitesRoles.DataBind();
                        UserName1 = gvSitesRoles.DataKeys[0]["UserName"].ToString();
                        FormsAuthentication.SetAuthCookie(UserName1, false);
                        SetActivityAndLoginDates(UserName1);
                        // Perform session management
                        SessionService.CreateAndStoreSessionTicket(UserName1);
                        Response.Redirect(String.Format("{0}?ss=1", ConfigurationManager.AppSettings["NonProxyWebServerUrl"]));
                    }
                    else
                    {
                        gvSitesRoles.DataSource = dtSitesRoles;
                        gvSitesRoles.DataBind();

                        lblsuccess.Text = string.Empty;
                        lblsuccess.Text = "<br/><br>Select an account to continue.<br/><br/>";
                        lblsuccess.Visible = true;
                        NonProxyServer.Visible = false;
                    }
                    #endregion CAC LOGIN
                }
                else
                {
                    #region CAC NOT FOUND, MUST LOGIN FIRST

                    lblfail.Text = string.Empty;
                    lblfail.Text = "<br><br>You are not authorized to login to SRTSweb with the certificate provided.<br>Please ensure your the CAC or PIV certificate provided is registered before attempting to login.<br><br>";
                    NonProxyServer.Visible = true;
                    lblsuccess.Visible = false;
                    lblfail.Visible = true;

                    LogEvent(String.Format("User (CAC ID: {0}) unsuccessfully logged on -CAC NOT FOUND- at {1}.", CAC_ID, DateTime.Now));

                    #endregion CAC NOT FOUND, MUST LOGIN FIRST
                }

                #endregion NOT AUTHENTICATED
  
            }
            else
            {
                #region CAC variable not found

                Master._MainMenu.Visible = false;
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.SignOut();
                }
                lblfail.Text = "<br><br>The System could not read the CAC certificate.<br>Click OK to close this window and try again.<br><br>";
                Master._ContentAuthenticated.Visible = false;
                //Master._BreadCrumbsTop.Visible = false;
                lblsuccess.Visible = false;
                lblfail.Visible = true;
                NonProxyServer.Visible = true;
                mySession.CacRegistrationCode = 4;

                // Add fail message

                #endregion CAC variable not found
            }

            #endregion TEST CAC VARIABLE

            #endregion CERT FOUND
        }

        /// <summary>
        /// This method displays an error message when a valid certificate is not found.
        /// </summary>
        protected void CertificateNotFound()
        {
            Master._MainMenu.Visible = false;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            lblfail.Text = "<br><br>The System did not detect a valid CAC certificate.<br>Click OK to close this window and try again.<br><br>";
            lblsuccess.Visible = false;
            lblfail.Visible = true;
            NonProxyServer.Visible = true;
            mySession.CacRegistrationCode = 4;

            LogEvent(String.Format("User (CAC ID: {0}) unsuccessfully logged on -CERT NOT FOUND- at {1}.", CAC_ID, DateTime.Now));
        }


        /// <summary>
        /// This region contains the SiteRoles gridview events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region GRID EVENTS

        public void gvSitesRoles_SelectedIndexChanged(Object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "iASLogin_gvSitesRoles_SelectedIndexChanged", mySession.MyUserID))
#endif
            {
                var un = gvSitesRoles.SelectedDataKey.Value.ToString();
                var p = CustomProfile.GetProfile(un);
                if (String.IsNullOrEmpty(p.SiteCode))
                {
                    if (!String.IsNullOrEmpty(p.PrimarySite))
                    {
                        p.SiteCode = p.PrimarySite;
                        CustomProfile.SaveLoggedInSiteCode(p);
                    }
                }
                FormsAuthentication.SetAuthCookie(un, false);

                SetActivityAndLoginDates(un);

                // Perform session management
                SessionService.CreateAndStoreSessionTicket(un);

                Response.Redirect(String.Format("{0}?ss=1", ConfigurationManager.AppSettings["NonProxyWebServerUrl"]), false);
            }
        }

        protected void gvSitesRoles_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            try
            {
                var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "iASLogin_gvSitesRoles_RowDataBound", MyUserID))
#endif
                {
                    if (e.Row.IsNull()) return;
                    if (e.Row.DataItem.IsNull()) return;

                    var un = DataBinder.Eval(e.Row.DataItem, "UserName").ToString();
                    //WriteToFile("iASLogin: After assigning UserName - line 681" + DateTime.Now);
                    var roles = String.Join(", ", Roles.GetRolesForUser(un));
                    var cp = CustomProfile.GetProfile(un);

                    //WriteToFile("iASLogin: line 686" + DateTime.Now);
                    var sites = new List<ProfileSiteEntity>();
                    sites.Add(new ProfileSiteEntity() { SiteCode = cp.PrimarySite, Approved = true });
                    //WriteToFile("iASLogin: After adding to sites. line 688" + DateTime.Now);

                    if (!cp.AvailableSiteList.IsNullOrEmpty())
                        sites.AddRange(cp.AvailableSiteList.Where(x => x.SiteCode != cp.PrimarySite));

                    //WriteToFile("iASLogin: line 694" + DateTime.Now);

                    var unC = e.Row.FindControl("lblUserName") as Label;
                    var rC = e.Row.FindControl("lblRole") as Label;
                    var lb = e.Row.FindControl("lblSites") as Label;

                    //WriteToFile("iASLogin: line 700" + DateTime.Now);
                    unC.Text = un.ToHtmlEncodeString();
                    rC.Text = roles;
                    lb.Text = String.Join("<br />", sites.Where(x => x.Approved == true).Select(s => s.SiteCode));
                    //WriteToFile("iASLogin: line 704" + DateTime.Now);

                }
            }
            catch (Exception ex)
            {
                ex.LogException("In iASLogin - gvSitesRoles_RowDataBound" + ex.Message + " - " + ex.InnerException);
               // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        #endregion GRID EVENTS
            

        /// <summary>
        /// This method sets the last login and last activity date for the currently logged in user
        /// </summary>
        /// <param name="userName"></param>
        private void SetActivityAndLoginDates(String userName)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "iASLogin_SetActivityAndLoginDates", mySession.MyUserID))
#endif
            {
                var user = Membership.GetUser(userName);

                mySession.LastLoginDate = user.LastLoginDate;

                user.LastLoginDate = DateTime.Now;
                user.LastActivityDate = DateTime.Now;
                Membership.UpdateUser(user);
            }
        }
                     
        /// <summary>
        /// This method redirects to the Logout url
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void NonProxyServer_Click(object sender, EventArgs e)
        {
            Response.Redirect(ConfigurationManager.AppSettings["LogoutUrl"], false);  //Aldela 12/20/2019: Added this line
        }


        #region Certificate Validation and Authorization Methods

        #endregion


        /// <summary>
        /// This region contains all of the public properties used by the methods in this class
        /// </summary>
        #region Public Properties

        public string CAC_ID { get; set; } = string.Empty;
        public string issuerName { get; set; }
        public bool isCACFound { get; set; }
        public bool isUserFound { get; set; }
        public bool isCertFound { get; set; }
        public bool isMultipleAccounts { get; set; }
        public DataTable dtSitesRoles { get; set; }
        public bool IsDoDCert { get; set; } = false;
        public bool CertIsPresent { get; set; } = false;
        public bool IsPIVCert { get; set; }
        public bool CertIsValid { get; set; } = false;
        public X509Certificate2 CacCert { get; set; } = null;
        public bool IsVACert { get; set; } = false;
        public char[] PeriodDelim { get; set; } = { '.' };
        public char[] CommaDelim { get; set; } = { ',' };
        public char[] SpaceDelim { get; set; } = { ' ' };
        public string UserName1 { get; set; } = String.Empty;
        public string StrBlueCoatCert { get; set; } = null;
        public string CnIssuerCert { get; set; }
        public bool CacFound { get; set; }
        public string CnUserID { get; set; } = string.Empty;
        public bool BluecoatCacFound { get; set; } = false;

        #endregion Public Properties


        /// <summary>
        /// This region contains the test certificate data for the VA, Entrust and Topaz certificates
        /// </summary>
        /// <returns></returns>
        #region VA/Entrust/Topaz Test Certificates

        private byte[] GetVACert()
        {
            byte[] VaTestCert = Encoding.Unicode.GetBytes("-----BEGIN CERTIFICATE----- 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 -----END CERTIFICATE-----");

            return VaTestCert;
        }

        private byte[] GetEntrustCert()
        {
            byte[] EntrustTestCert = Encoding.Unicode.GetBytes("-----BEGIN CERTIFICATE-----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 -----END CERTIFICATE-----");
            return EntrustTestCert;
        }

        private byte[] GetTopazCert()
        {
            byte[] TopazTestCert = Encoding.Unicode.GetBytes("-----BEGIN CERTIFICATE----- MIIEnzCCA4egAwIBAgIDAkj5MA0GCSqGSIb3DQEBCwUAMFoxCzAJBgNVBAYTAlVTMRgwFgYDVQQKDA9VLlMuIEdvdmVybm1lbnQxDDAKBgNVBAsMA0RvRDEMMAoGA1UECwwDUEtJMRUwEwYDVQQDDAxET0QgU1cgQ0EtNTMwHhcNMjAwMTA3MTQ1NDQxWhcNMjIwNDExMTQ1NDQxWjBwMQswCQYDVQQGEwJVUzEYMBYGA1UECgwPVS5TLiBHb3Zlcm5tZW50MQwwCgYDVQQLDANEb0QxDDAKBgNVBAsMA1BLSTEMMAoGA1UECwwDREhBMR0wGwYDVQQDDBR0b3BhemJwbS5tZWQub3NkLm1pbDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAPzlPVtAOW8V1hvJ/ispvPLEtz1QctrQ6yf2+mn7vQTe67g6R+axNG5Qjk25eYlB3qNLR03aNEF+PZaIxnq15+XLSfJRxdJnPzRm0BliihUhO5AO7PlqISJCXijBymy8xDwwQS9GvLa2NWcGNqDNMVlvmhozi9VtbirjeziJKH67eIrBtB6Sptmh39yQl6MCUhIkClIrORTV9dcwcEcIxHbiKr85u9cVJLwgVedYPsmXZhzawV9usD6dsOGaWwQphmFUftfEMGKxYpxaLc7shWMVdKA2s0lEXDXNuhOuzkU2Na8UfMAys4J9HJTsp5654BLGaAJRtLj9u8ALFaxq520CAwEAAaOCAVYwggFSMB8GA1UdIwQYMBaAFFHEizOZlMB+uzYd4+I6Bb0ydJ1TMB0GA1UdDgQWBBR8i/wFxmOC5qBW6DA9F3f1wj8QkjBlBggrBgEFBQcBAQRZMFcwMwYIKwYBBQUHMAKGJ2h0dHA6Ly9jcmwuZGlzYS5taWwvc2lnbi9ET0RTV0NBXzUzLmNlcjAgBggrBgEFBQcwAYYUaHR0cDovL29jc3AuZGlzYS5taWwwDgYDVR0PAQH/BAQDAgWgMDcGA1UdHwQwMC4wLKAqoCiGJmh0dHA6Ly9jcmwuZGlzYS5taWwvY3JsL0RPRFNXQ0FfNTMuY3JsMB8GA1UdEQQYMBaCFHRvcGF6YnBtLm1lZC5vc2QubWlsMBYGA1UdIAQPMA0wCwYJYIZIAWUCAQsnMCcGA1UdJQQgMB4GCCsGAQUFBwMBBggrBgEFBQcDAgYIKwYBBQUIAgIwDQYJKoZIhvcNAQELBQADggEBAJ0PXUaVEH0F93SlNSs7WSDphtOhQxHVDnE+1eJIs4R99ofI/aUgYgMYsEvptBmFkwfuD636aE2OoVKNXoSBEq5CDSmLFuM+MHNk6hqenPW+Dpcz1cyWU+NmeTyYWdN0IoQrd5hcDViMrgUWLcuGHzBmLvg6KGabiNtRj2wslz1b6CcI0pzavo8dRXA0j26B62VFKfHu9qTiB7mgBvOhh+b4PwXP0r3zSUi6MuPeq8rxkSYXIkmTStCW/Seqjwau08z6UXA3kDWg4xg39UNCx0Mc/b4qHmbo+NN0Jp8FIqIn8IB3L4Q98i9XqetdOiqHwrWapLAmGVuaH5VPvE5s+3g= -----END CERTIFICATE-----");
            return TopazTestCert;
        }

        #endregion

    }
}