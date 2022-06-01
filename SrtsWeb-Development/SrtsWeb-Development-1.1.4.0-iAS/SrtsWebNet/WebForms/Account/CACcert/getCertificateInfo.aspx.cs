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

namespace SrtsWeb.Account.CACcert
{
    public partial class getCertificateInfo : PageBase, ICertificateInfoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "getCertificateInfo_Page_Load", MyUserID))
#endif
            {
                try
                {
                    //Reset CAC registration errors.
                    if (!Page.IsPostBack)
                    {
                        if (mySession != null)
                        {
                            mySession.CacRegistrationCode = 0;
                        }
                    }

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
                    exx.LogException("In getCertificateInfo" + ex.Message);
                }
            }
        }
        /// <summary>
        /// This method begins the process of CAC certificate processing by calling the 'PullCACCertificate()' method.
        /// </summary>
        protected void ProcessCertificateLogin()
        {
            //Pull CAC cert and extract DodID and IssuerID information.
            PullCACCertificate();

            if (CacFound || BluecoatCacFound)
            {
                CertificateFound();
            }
            else
            {
                CertificateNotFound();
            }
        }

        /// <summary>
        /// This method extracts personal or test certificate information to process
        /// </summary>
        protected void PullCACCertificate()
        {

#if DEBUG
            //////////////  Begin certificate testing //////////////
            //This section is used to test the VA, Entrust and Topaz certificates in development and testing
            //////////////               
            try
            {
                //check for test certificate selection and grab certificate array
                byte[] testCertValue = null;
                if (Session["Cert"] != null && !String.IsNullOrEmpty(Session["Cert"].ToString()))
                {
                    string testCert = Session["Cert"].ToString();

                    switch (testCert)
                    {
                        case "VA": // Va Cert
                            testCertValue = GetVACert();
                            break;
                        case "Entrust": // Entrust Cert
                            testCertValue = GetEntrustCert();
                            break;
                        case "Topaz": // Topaz Cert
                            testCertValue = GetTopazCert();
                            break;
                    }
                    CacCert = new X509Certificate2(testCertValue);
                    CertIsValid = true;
                    CertIsPresent = true;
                }
                else
                {
                    // test certificate not selected so use personal certificate
                    CacCert = new X509Certificate2(Request.ClientCertificate.Certificate);
                    CertIsValid = Request.ClientCertificate.IsValid;
                    CertIsPresent = Request.ClientCertificate.IsPresent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Do nothing now, later display error loading certificate
            };  /////////////////// End certificate testing //////////////
#else
                    // not in debug mode so use personal certificate
                    cacCert = new X509Certificate2(Request.ClientCertificate.Certificate);
                    certIsValid = Request.ClientCertificate.IsValid;
                    certIsPresent = Request.ClientCertificate.IsPresent;
#endif



            if (CertIsPresent && CertIsValid) { CacFound = true; }
            StrBlueCoatCert = Request.Headers["Client-Cert"];
            if (StrBlueCoatCert != null && StrBlueCoatCert.Contains("sno")) { BluecoatCacFound = true; }
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
                // Process the BlueCoatCAC found
                BlueCoatCACFound();
            }
            else
            {   // Process the non BlueCoatCAC found
                NonBlueCoatCACFound();
            }

            #region TEST CAC VARIABLE

            CustomLogger.Log("load", this.mySession.LogTriggers, String.Format("CAC_ID: {0}, Issuer: {1}", CAC_ID, issuerName));

            if ((CAC_ID != string.Empty) && (issuerName != string.Empty))
            {

                CustomLogger.Log("load", this.mySession.LogTriggers, String.Format("User is authenticated: {0}", User.Identity.IsAuthenticated));


                /* Beginning of code for - CR SCR-T 0059883  Handle the RMF requirement for PIV */
                DateTime dtPIVCertRequired = DateTime.Parse(ConfigurationManager.AppSettings["PIVCertificateRequiredStartDate"]);
                if (!IsPIVCert)
                {      //CR SCR-T 0059883  Handle the RMF requirement for PIV
                    if (DateTime.Compare(DateTime.Now, dtPIVCertRequired) >= 0)
                    {
                        //FormsAuthentication.SignOut();
                        lblfail.Text = string.Empty;
                        string message = "<br><br>A PIV (Authentication) certificate is required to access this application.<br>If your PIV has not been activated, please follow the instructions provided" +
                        " by the ID Card Office Online (IDCO) at <a href ='https://www.dmdc.osd.mil/self_service' target = '_self'>https://www.dmdc.osd.mil/self_service</a>. <br><br> After you have activated your PIV, please return to the application" +
                        " and select your PIV certificate to log into the application.  If you have issues, please close and re-open a new browser window. <br><br>Please contact the help desk at 1-800-600-9332 for further assistance.<br>";
                        lblfail.Text = message;
                        lblsuccess.Visible = false;
                        lblfail.Visible = true;
                        NonProxyServer.Visible = false;

                        Master.MainContentPanel.Visible = false;

                        LogEvent(String.Format("User (CAC ID: {0}) unsuccessfully logged on -WRONG CERT- at {1}.", CAC_ID, DateTime.Now));

                    }
                    else
                    {
                        string message = "A PIV (Authentication) certificate will be required to access this application beginning Jan 1, 2020. If your PIV has not been activated, please follow the instructions provided" +
                        " by the ID Card Office Online (IDCO) at https://www.dmdc.osd.mil/self_service. Please contact the help desk at 1-800-600-9332 for further assistance.";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", String.Format("alert('{0}', 'message');", message), true);
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "DoDialogWidth", "$('#divRefreshDialog').dialog({ width: 780 });", true);
                    }
                }
                #region NOT AUTHENTICATED

                CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("In not authenticated...CAC_ID: {0}, Issuer: {1}", CAC_ID, issuerName));
                _presenter.GetAuthorizationsByCAC_ID(CAC_ID, issuerName, String.Empty);
                CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("getCertificateInfo Not Authenticated loop - Issuer Name variable - {0}", issuerName));

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
        /// This method extracts certificate information from the BluecoatCAC 
        /// </summary>
        protected void BlueCoatCACFound()
        {
            StrBlueCoatCert = StrBlueCoatCert.Replace("%2C", ",").Replace("%3D", "=").Replace("&amp;", "&");
            CustomLogger.Log("cac", this.mySession.LogTriggers, "bluecoatCert");
            foreach (string certificateElements in StrBlueCoatCert.Split('&'))
            {
                if (certificateElements.StartsWith("subject="))
                {
                    // Add the subject to mySession.MyUserID
                    mySession.CertificateSubject = certificateElements;

                    foreach (string subjectElements in certificateElements.Split(','))
                    {
                        if (subjectElements.StartsWith("+CN="))
                        {
                            CnUserID = subjectElements.Split('=')[1];
                            Match CacIDMatch = Regex.Match(subjectElements, @"[0-9]{4,10}"); // 4 to 10 digits 
                            if (CacIDMatch.Success)
                            {
                                CAC_ID = CacIDMatch.Value;
                            }
                            else
                            {
                                //Possibly could be Monitoring soft cert from monitoring application
                                if (subjectElements.Contains(ConfigurationManager.AppSettings["MonitoringCertFriendlyName"])) //topazbpm.med.osd.mil
                                {
                                    CAC_ID = ConfigurationManager.AppSettings["MonitoringCertCAC_ID"]; //topazbpm
                                }
                            }
                        }
                    }
                }
                if (certificateElements.StartsWith("issuer="))
                {
                    CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("Issuer Found in Cert - {0}", certificateElements));
                    foreach (string issuerElements in certificateElements.Split(','))
                    {
                        Match issuerCertMatch = Regex.Match(issuerElements, @"CN\=(.+)"); //

                        CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("CN found in Subject? - {0}", issuerCertMatch));
                        if (issuerCertMatch.Success)
                        {
                            CnIssuerCert = issuerCertMatch.Groups[1].Value.Replace('+', ' ');
                        }
                    }
                }
                if (certificateElements.StartsWith("policy_oids=")) //CR SCR-T 0059883  Handle the RMF requirement for PIV
                {
                    CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("policy_oids Found in Cert - {0}", certificateElements));
                    char[] delimiterChars = { '=', ',' };
                    string[] policyObjectIdentifierElements = certificateElements.Split(delimiterChars);

                    IsPIVCert = policyObjectIdentifierElements.Any(s => ConfigurationManager.AppSettings["PIVCertPolicyObjectID"].Contains(s));
                }

            }

            if (CAC_ID.CompareTo(ConfigurationManager.AppSettings["MonitoringCertCAC_ID"]) == 0) //topazbpm
                IsPIVCert = true;

            CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("Issuer Name variable - {0}", CnIssuerCert));
            //Check CAC Type (i.e VA vs. DoD)

            if (CnIssuerCert.StartsWith("DOD "))
            {
                //ISSUERNAME
                issuerName = "DOD";
                IsDoDCert = true;
            }
            else
            {
                if (CnIssuerCert.StartsWith("Veterans Affairs User"))
                {
                    //ISSUERNAME
                    issuerName = "Veterans Affairs User";
                    IsVACert = true;
                }
                if (CnIssuerCert.StartsWith("Entrust Managed Services SSP"))
                {
                    //ISSUERNAME
                    issuerName = "Entrust Managed Services SSP";
                    IsVACert = false;
                }
            }
        }

        /// <summary>
        /// This method extracts certificate information from the CAC
        /// </summary>
        protected void NonBlueCoatCACFound()
        {
            lblsuccess.Visible = true;
            var cet = new CustomEventTracer("Page_Load_CacFound", mySession.MyUserID, SrtsTraceSource.LoginSource);

            cet.TraceInfo("Getting x509 cert.");
            //var cert = new X509Certificate2(cacCert.Certificate);
            //X509Certificate2 cert = new X509Certificate2(Page.Request.ClientCertificate.Certificate);
            cet.TraceInfo(String.Format("Original x509 cert: {0}", CacCert.Subject));

            // Add the subject to mySession.MyUserID
            mySession.CertificateSubject = CacCert.Subject;

            //ISSUER
            cet.TraceInfo(String.Format("Issuer {0}", CacCert.Issuer.Split(CommaDelim)[0]));
            CnIssuerCert = CacCert.Issuer.Split(CommaDelim)[0];

            cet.TraceInfo(String.Format("Number of subject split: {0}", CacCert.Subject.Split(CommaDelim).Length));
            string cnSubjectCert = null;


            cnSubjectCert = CacCert.Subject.Split(CommaDelim)[0];
            CnUserID = cnSubjectCert.Replace("CN=", "");


            foreach (var a in cnSubjectCert.Split(PeriodDelim).ToList())
            {
                var r = new Regex("[0-9]{10}");
                if (r.Match(a).Success)
                {
                    CAC_ID = a;
                    break;
                }
                else
                {
                    //Possibly could be Monitoring soft cert from monitoring application
                    if (a.Contains(ConfigurationManager.AppSettings["MonitoringCertFriendlyName"])) //topazbpm.med.osd.mil
                    {
                        CAC_ID = ConfigurationManager.AppSettings["MonitoringCertCAC_ID"]; //topazbpm
                        break;
                    }
                }
            }

            string userprincipalname = CacCert.GetNameInfo(X509NameType.UpnName, false);


            foreach (X509Extension ext in CacCert.Extensions)
            {
                AsnEncodedData asnData = new AsnEncodedData(ext.Oid, ext.RawData);
                string oid = asnData.Format(true);

                if (oid.Contains("Certificate Policy"))
                {
                    if (oid.Contains(ConfigurationManager.AppSettings["PIVCertPolicyObjectID"]))
                    {
                        IsPIVCert = true;
                        CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("policy_oids Found in Cert - {0}", oid));
                        break;
                    }
                }
            }

            if (CAC_ID.CompareTo(ConfigurationManager.AppSettings["MonitoringCertCAC_ID"]) == 0) //topazbpm
                IsPIVCert = true;

            if (CnIssuerCert.StartsWith("CN=DOD "))
            {
                //ISSUERNAME
                issuerName = "DOD";
                IsDoDCert = true;
            }
            else
            {
                if (CnIssuerCert.StartsWith("CN=Veterans Affairs User")) // VA
                {
                    //ISSUERNAME
                    issuerName = "Veterans Affairs User";
                    IsVACert = true;
                    CAC_ID = CnUserID;
                }

                if (CnIssuerCert.StartsWith("OU=Entrust Managed Services SSP"))  // Entrust
                {
                    //ISSUERNAME
                    issuerName = "Entrust Managed Services SSP";
                    IsVACert = false;
                    CAC_ID = CnUserID;
                }
                if (CnIssuerCert.StartsWith("CN=DOD SW")) // TOPAZ
                {
                    //ISSUERNAME
                    issuerName = "DOD SW";
                    IsVACert = false;
                    CAC_ID = CnUserID;
                }
            }
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
            using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "getCertificateInfo_gvSitesRoles_SelectedIndexChanged", mySession.MyUserID))
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
                using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "getCertificateInfo_gvSitesRoles_RowDataBound", MyUserID))
#endif
                {
                    if (e.Row.IsNull()) return;
                    if (e.Row.DataItem.IsNull()) return;

                    var un = DataBinder.Eval(e.Row.DataItem, "UserName").ToString();
                    //WriteToFile("getCertificateInfo: After assigning UserName - line 681" + DateTime.Now);
                    var roles = String.Join(", ", Roles.GetRolesForUser(un));
                    var cp = CustomProfile.GetProfile(un);

                    //WriteToFile("getCertificateInfo: line 686" + DateTime.Now);
                    var sites = new List<ProfileSiteEntity>();
                    sites.Add(new ProfileSiteEntity() { SiteCode = cp.PrimarySite, Approved = true });
                    //WriteToFile("getCertificateInfo: After adding to sites. line 688" + DateTime.Now);

                    if (!cp.AvailableSiteList.IsNullOrEmpty())
                        sites.AddRange(cp.AvailableSiteList.Where(x => x.SiteCode != cp.PrimarySite));

                    //WriteToFile("getCertificateInfo: line 694" + DateTime.Now);

                    var unC = e.Row.FindControl("lblUserName") as Label;
                    var rC = e.Row.FindControl("lblRole") as Label;
                    var lb = e.Row.FindControl("lblSites") as Label;

                    //WriteToFile("getCertificateInfo: line 700" + DateTime.Now);
                    unC.Text = un.ToHtmlEncodeString();
                    rC.Text = roles;
                    lb.Text = String.Join("<br />", sites.Where(x => x.Approved == true).Select(s => s.SiteCode));
                    //WriteToFile("getCertificateInfo: line 704" + DateTime.Now);

                }
            }
            catch (Exception ex)
            {
                ex.LogException("In getCertificateInfo - gvSitesRoles_RowDataBound" + ex.Message + " - " + ex.InnerException);
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
            using (MethodTracer.Trace(SrtsTraceSource.LoginSource, "getCertificateInfo_SetActivityAndLoginDates", mySession.MyUserID))
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

        
        /// <sumary>
        /// This region contains commented code no longer being used; needs to be removed eventually
        /// </sumary>
        #region commented code
        //isPIVCert =  userprincipalname.Count(Char.IsDigit) > 10 ? true : false;
        //cnUserID = cnSubjectCert.Split('=')[1];
        //if (cacCert.IsPresent && cacCert.IsValid) { cacFound = true; }

        // jrp - 2014-01-15
        // If sent through a bluecoat reverse web proxy,
        // then the CAC cert is in a Header string called Client-Cert
        // strBlueCoatCert = Request.Headers["Client-Cert"];
        // need to also check server on next line

        //Next to lines
        //cacFound = false;

        //strBlueCoatCert = "sno=4095202+%280x3e7ce2%29&subject=DC%3Dgov%2C+DC%3Dva%2C+O%3Dinternal%2C+OU%3Dpeople%2FUID%3Ddanuel.sisco%40va.gov%2C+CN%3DUSER+SOME+FICTITIOUS+123456&validfrom=Apr+23+16%3A43%3A17+2014+GMT&validto=Mar+21+23%3A59%3A59+2017+GMT&issuer=DC%3Dgov%2C+DC%3Dva%2C+OU%3DServices%2C+OU%3DPKI%2C+CN%3DVeterans+Affairs+User+CA+B1";

        //strBlueCoatCert = "sno=1959515+%280x1de65b%29&subject=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+OU%3DCONTRACTOR%2C+CN%3DDEL+GROSSO.AMANDA.1291272521&validfrom=Nov++7+00%3A00%3A00+2017+GMT&validto=Oct+10+23%3A59%3A59+2020+GMT&issuer=C%3DUS%2C+O%3DU.S.+ Government%2C+OU%3DDoD%2C+OU% 3DPKI%2C+CN%3DDOD+ID+CA-44&policy_oids=2.16.840.1.101.2.1.11.42%2C2.16.840.1.101.3.2.1.3.13";

        //strBlueCoatCert = "sno=1673761+%280x198a21%29&amp;subject=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+OU%3DDHA%2C+CN%3DPOWELL.JOSEPH.R.1214655175&amp;validfrom=Sep+16+00%3A00%3A00+2019+GMT&amp;validto=Sep+15+23%3A59%3A59+2022+GMT&amp;issuer=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+CN%3DDOD+ID+CA-49&amp;policy_oids=2.16.840.1.101.2.1.11.42%2C2.16.840.1.101.3.2.1.3.13&";
        //if (strBlueCoatCert != null && strBlueCoatCert.Contains("sno")) { bluecoatCacFound = true; }








        //                            if (User.Identity.IsAuthenticated)
        //                            {
        //                                //Master._BreadCrumbsTop.Visible = false;
        //                                Master._ContentAuthenticated.Visible = false;

        //#region AUTHENTICATED

        //                                bool isAdmin = User.IsInRole("mgmtenterprise") || User.IsInRole("labadmin") || User.IsInRole("clinicadmin") || User.IsInRole("humanadmin") || User.IsInRole("mgmtadmin") || User.IsInRole("trainingadmin");
        //                                bool isEmailCert = issuerName.IndexOf("EMAIL") != -1;

        //                                //If User did not clicked the Email cert and they are in an non-admin role OR If User clicked the Email Cert and is in an Admin
        //                                if (!isPIVCert)//!isDoDCert && !isVACert) //CR SCR-T 0059883  Handle the RMF requirement for PIV
        //                                {
        //#region WRONG CAC

        //                                    //FormsAuthentication.SignOut();
        //                                    lblfail.Text = string.Empty;
        //                                    lblfail.Text = "<br><br>You are not authorized to login to SRTSweb with the Access Card (CAC/PIV) provided.<br><br>";
        //                                    lblsuccess.Visible = false;
        //                                    lblfail.Visible = true;
        //                                    NonProxyServer.Visible = true;

        //                                    LogEvent(String.Format("User (CAC ID: {0}) unsuccessfully logged on -WRONG CAC- at {1}.", CAC_ID, DateTime.Now));

        //#endregion WRONG CAC
        //                                }
        //                                else
        //                                {
        ////                                    if (!isPIVCert)//isDoDCert && ((isEmailCert && isAdmin) || (!isEmailCert && !isAdmin)))  //CR SCR-T 0059883  Handle the RMF requirement for PIV
        ////                                    {
        ////#region WRONG CERT

        ////                                        //FormsAuthentication.SignOut();
        ////                                        lblfail.Text = string.Empty;
        ////                                        lblfail.Text = "<br><br>You are not authorized to login to SRTSweb with the certificate provided.<br>Please ensure the CAC or PIV certificate provided is registered before attempting to login.<br><br>";
        ////                                        lblsuccess.Visible = false;
        ////                                        lblfail.Visible = true;
        ////                                        NonProxyServer.Visible = true;

        ////                                        LogEvent(String.Format("User (CAC ID: {0}) unsuccessfully logged on -WRONG CERT- at {1}.", CAC_ID, DateTime.Now));

        ////#endregion WRONG CERT
        ////                                    }
        ////                                    else
        ////                                    {
        //#region CORRECT CERT

        //                                        //Get the list of account in the Authorization table that are associated with the CAC certificate selected.
        //                                        _presenter.GetAuthorizationsByCAC_ID(CAC_ID, issuerName, User.Identity.Name.ToString());

        //                                        //If the CACcert and the User Account are already in the table prompts the user to login.
        //                                        if (!isCACFound)
        //                                        {
        //#region REGISTER NEW CAC

        //                                            //Update the Authorization table with the account used to login.
        //                                            _presenter.UpdateAuthorizationCacInfoByUserName(User.Identity.Name.ToString(), CAC_ID, issuerName);

        //                                            //Update the Authorization table with the account used to login.
        //                                            _presenter.GetAuthorizationsByCAC_ID(CAC_ID, issuerName, User.Identity.Name.ToString());

        //                                            if (!isUserFound)
        //                                            {
        //#region FAILED REGISTRATION

        //                                                lblfail.Text = string.Empty;
        //                                                lblfail.Text = "<br/>The system was not able to register the CAC provided.<br/>";
        //                                                lblfail.Visible = true;
        //                                                lblsuccess.Visible = false;
        //                                                NonProxyServer.Visible = true;

        //#endregion FAILED REGISTRATION
        //                                            }
        //                                            else
        //                                            {
        //#region SUCCESSFUL REGISTRATION

        //                                                gvSitesRoles.DataSource = dtSitesRoles;
        //                                                gvSitesRoles.DataBind();

        //                                                if (!isMultipleAccounts)
        //                                                {
        //                                                    lblsuccess.Text = string.Empty;
        //                                                    lblsuccess.Text = "<br/>You have successfuly registered your CAC with the <b>" + User.Identity.Name.ToString() + "</b> user account.<br><BR> Click Select to continue<br/><br/>";
        //                                                    lblsuccess.Visible = true;
        //                                                    NonProxyServer.Visible = false;
        //                                                }
        //                                                else
        //                                                {
        //                                                    lblsuccess.Text = string.Empty;
        //                                                    lblsuccess.Text = "<br/><br/>You have successfuly registered your CAC with the <b>" + User.Identity.Name.ToString() + "</b> user account. <BR><bR> Please select an account to continue<br/><br/>";
        //                                                    lblsuccess.Visible = true;
        //                                                    NonProxyServer.Visible = false;
        //                                                }

        //#endregion SUCCESSFUL REGISTRATION
        //                                            }

        //#endregion REGISTER NEW CAC
        //                                        }
        //                                        else
        //                                        {
        //#region CAC LOGIN

        //                                            if (isCACFound && isUserFound)
        //                                            {
        //#region CAC LOGIN

        //                                                gvSitesRoles.DataSource = dtSitesRoles;
        //                                                gvSitesRoles.DataBind();

        //                                                if (!isMultipleAccounts)
        //                                                {
        //                                                    UserName = gvSitesRoles.DataKeys[0]["UserName"].ToString();
        //                                                    FormsAuthentication.SetAuthCookie(UserName, false);
        //                                                    SetActivityAndLoginDates(UserName);
        //                                                    // Perform session management
        //                                                    SessionService.CreateAndStoreSessionTicket(UserName);
        //                                                    Response.Redirect(String.Format("{0}?ss=1", ConfigurationManager.AppSettings["NonProxyWebServerUrl"]));
        //                                                }
        //                                                else
        //                                                {
        //                                                    lblsuccess.Text = string.Empty;
        //                                                    lblsuccess.Text = "<br><BR>Please select an account to continue<br><br>";
        //                                                    lblsuccess.Visible = true;
        //                                                    NonProxyServer.Visible = false;
        //                                                }

        //#endregion CAC LOGIN
        //                                            }
        //                                            else
        //                                            {
        //#region ADD ADDITIONAL ACCOUNT TO A REGISTERED CAC

        //                                                //Update the Authorization table with the account used to login.
        //                                                _presenter.UpdateAuthorizationCacInfoByUserName(User.Identity.Name.ToString(), CAC_ID, issuerName);

        //                                                //Get all account associated to the CAC.
        //                                                _presenter.GetAuthorizationsByCAC_ID(CAC_ID, issuerName, User.Identity.Name.ToString());

        //#endregion ADD ADDITIONAL ACCOUNT TO A REGISTERED CAC

        //                                                if (isCertFound && isUserFound)
        //                                                {
        //#region SUCCESSFUL ADD

        //                                                    gvSitesRoles.DataSource = dtSitesRoles;
        //                                                    gvSitesRoles.DataBind();

        //                                                    //Master._BreadCrumbsTop.Visible = true;

        //                                                    if (!isMultipleAccounts)
        //                                                    {
        //                                                        //FormsAuthentication.RedirectFromLoginPage(gvSitesRoles.DataKeys[0]["UserName"].ToString(), false);
        //                                                        lblsuccess.Text = string.Empty;
        //                                                        lblsuccess.Text = "You have successfuly registered your CAC with the <b>" + User.Identity.Name.ToString() + "</b> user account.<br><BR> Click Select to continue<br/><br/>";
        //                                                        lblfail.Visible = false;
        //                                                        NonProxyServer.Visible = false;
        //                                                    }

        //                                                    lblsuccess.Text = string.Empty;
        //                                                    lblsuccess.Text = "<br/><br/>You have successfuly registered your CAC with the <b>" + User.Identity.Name.ToString() + "</b> user account. <BR><bR> Please select an account to continue<br/><br/>";

        //                                                    lblsuccess.Visible = true;
        //                                                    lblfail.Visible = false;
        //                                                    NonProxyServer.Visible = false;

        //#endregion SUCCESSFUL ADD
        //                                                }
        //                                                else
        //                                                {
        //#region COULD NOT ADD AN ADDITIONAL ACCOUNT

        //                                                    FormsAuthentication.SignOut();
        //                                                    lblfail.Text = string.Empty;
        //                                                    lblfail.Text = "<br>An error ocurred registering the CAC.<br>Click OK to return to the login screen and try again<br>";
        //                                                    lblsuccess.Visible = false;
        //                                                    lblfail.Visible = true;
        //                                                    NonProxyServer.Visible = true;

        //#endregion COULD NOT ADD AN ADDITIONAL ACCOUNT
        //                                                }
        //                                            }

        //#endregion CAC LOGIN
        //                                        }

        //#endregion CORRECT CERT
        //                                    //}
        //                                }

        //#endregion AUTHENTICATED
        //                            }
        // else
        //{

        //if (Session["Cert"] != null && !String.IsNullOrEmpty(Session["Cert"].ToString()))
        //{
        //    if (Session["Cert"].ToString() == "Entrust")
        //    {
        //        cnSubjectCert = cacCert.Subject.Split(commaDelim)[0];
        //        cnUserID = cnSubjectCert.Split('=')[1];
        //    }
        //    else
        //    {
        //        cnSubjectCert = cacCert.Subject.Split(commaDelim)[0];
        //        cnUserID = cnSubjectCert.Split('=')[1];
        //    }
        //}
        //else
        //{
        //    cnSubjectCert = cacCert.Subject.Split(commaDelim)[0];
        //    cnUserID = cnSubjectCert.Split('=')[1];
        //}

        #region Interpret BlueCoat Reverse Proxy Client Cert

        // BlueCoat Reverse Proxy return client certificate with following format in "Client-Cert" header

        //DOD CAC
        //sno=1949134+%280x1dbdce%29&subject=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+OU%3DCONTRACTOR%2C+CN%3DUSER.SOME.FICTITIOUS.1234567890&validfrom=Apr+14+00%3A00%3A00+2011+GMT&validto=Jun+30+23%3A59%3A59+2012+GMT&issuer=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+CN%3DDOD+CA-25

        //sno=1949134+%280x1dbdce%29
        //&subject=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+OU%3DCONTRACTOR%2C+CN%3DUSER.SOME.FICTITIOUS.1234567890
        //&validfrom=Apr+14+00%3A00%3A00+2011+GMT
        //&validto=Jun+30+23%3A59%3A59+2012+GMT
        //&issuer=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+CN%3DDOD+CA-25
        //&policy_oids=2.16.840.1.101.2.1.11.42%2C2.16.840.1.101.3.2.1.3.13
        //
        //sno = SerialNumber, subject= Subject, validfrom = ValidFrom, validto = ValidTo, issuer = Issuer
        //
        //sno=1949134 (0x1dbdce)
        //&subject=C=US, O=U.S. Government, OU=DoD, OU=PKI, OU=CONTRACTOR, CN=USER.SOME.FICTITIOUS.1234567890
        //&validfrom=Apr 14 00:00:00 2011 GMT
        //&validto=Jun 30 23:59:59 2012 GMT
        //&issuer=C=US, O=U.S. Government, OU=DoD, OU=PKI, CN=DOD CA-25
        //&policy_oids=2.16.840.1.101.2.1.11.42, 2.16.840.1.101.3.2.1.3.13


        //VA PIV
        //
        //sno=4095202+%280x3e7ce2%29&subject=DC%3Dgov%2C+DC%3Dva%2C+O%3Dinternal%2C+OU%3Dpeople%2FUID%3Ddanuel.sisco%40va.gov%2C+CN%3DUSER+SOME+FICTITIOUS+123456&validfrom=Apr+23+16%3A43%3A17+2014+GMT&validto=Mar+21+23%3A59%3A59+2017+GMT&issuer=DC%3Dgov%2C+DC%3Dva%2C+OU%3DServices%2C+OU%3DPKI%2C+CN%3DVeterans+Affairs+User+CA+B1
        //sno=4095202+%280x3e7ce2%29
        //&subject=DC%3Dgov%2C+DC%3Dva%2C+O%3Dinternal%2C+OU%3Dpeople%2FUID%3Ddanuel.sisco%40va.gov%2C+CN%3DUSER+SOME+FICTITIOUS+123456
        //&validfrom=Apr+23+16%3A43%3A17+2014+GMT
        //&validto=Mar+21+23%3A59%3A59+2017+GMT
        //&issuer=DC%3Dgov%2C+DC%3Dva%2C+OU%3DServices%2C+OU%3DPKI%2C+CN%3DVeterans+Affairs+User+CA+B1
        //
        //sno=4095202(0x3e7ce2)
        //&subject=DC=gov, DC=va, O=internal, OU=people/UID=danuel.sisco@va.gov, CN=USER SOME FICTITIOUS 123456
        //&validfrom=Apr 23 16:43:17 2014 GMT
        //&validto=Mar 21 23:59:59 2017 GMT
        //&issuer=DC=gov, DC=va, OU=Services, OU=PKI, CN=Veterans Affairs User CA B1

        #endregion Interpret BlueCoat Reverse Proxy Client Cert

        // example id
        //                       Certificate Policy:  //Policy Identifier = 2.16.840.1.101.3.2.1.3.13


        //if (cert.ToString  certificateElements.StartsWith("policy_oids=")) //CR SCR-T 0059883  Handle the RMF requirement for PIV
        //{
        //    CustomLogger.Log("cac", this.mySession.LogTriggers, String.Format("policy_oids Found in Cert - {0}", certificateElements));
        //    char[] delimiterChars = { '=', ',' };
        //    string[] policyObjectIdentifierElements = certificateElements.Split(delimiterChars);

        //    isPIVCert = policyObjectIdentifierElements.Any(s => ConfigurationManager.AppSettings["PIVCertPolicyObjectID"].Contains(s));
        //}

        //Check CAC Type (i.e VA vs. DoD)

        //public static void WriteToFile(string Message)
        //{
        //    string path = "C:\\Srts_Logs";
        //    if (!Directory.Exists(path))
        //    {
        //        // creates the directory
        //        Directory.CreateDirectory(path);
        //    }
        //    string filepath = "C:\\Srts_Logs\\getCertificateInfo" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
        //    if (!System.IO.File.Exists(filepath))
        //    {
        //        //creates a file to write to.
        //        using (StreamWriter sw = System.IO.File.CreateText(filepath))
        //        {
        //            sw.WriteLine(Message);
        //        }
        //    }
        //    else
        //    {
        //        using (StreamWriter sw = System.IO.File.AppendText(filepath))
        //        {
        //            sw.WriteLine(Message);
        //        }
        //    }
        //}
        #endregion
    }
}