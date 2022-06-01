using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.JSpecs;
using SrtsWeb.Views.JSpecs;
using System.Web;
using System;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Reflection;
using System.Collections;

namespace SrtsWeb.WebForms.JSpecs.Forms.Account
{
    public partial class JSpecsGetCacCert : PageBaseJSpecs, IJSpecsLoginView
    {
        private JSpecsLoginPresenter _presenter;
        private string _clinicCode;

        public JSpecsGetCacCert()
        {
            _presenter = new JSpecsLoginPresenter(this);
            _clinicCode = "008094";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string UserName = String.Empty;
                bool cacFound = false;
                bool bluecoatCacFound = false;
                bool isPIVCert = false;
                char[] periodDelim = { '.' };
                char[] commaDelim = { ',' };
                char[] spaceDelim = { ' ' };


                #region PULL CAC CERTIFICATE
                //Gathers CAC cert and extracts DodID and IssuerID information.
                HttpClientCertificate cacCert = null;

                String strBlueCoatCert = null;

                cacCert = Request.ClientCertificate;
                
                if (cacCert.IsPresent && cacCert.IsValid)
                {
                    cacFound = true;
                }
                    // jrp - 2014-01-15
                    // If sent through a bluecoat reverse web proxy,
                    // then the CAC cert is in a Header string called Client-Cert
                strBlueCoatCert = Request.Headers["Client-Cert"];
                    // need to also check server on next line
                    //Next to lines
                    //cacFound = false;
                    //strBlueCoatCert = "sno=4095202+%280x3e7ce2%29&subject=DC%3Dgov%2C+DC%3Dva%2C+O%3Dinternal%2C+OU%3Dpeople%2FUID%3Ddanuel.sisco%40va.gov%2C+CN%3DUSER+SOME+FICTITIOUS+123456&validfrom=Apr+23+16%3A43%3A17+2014+GMT&validto=Mar+21+23%3A59%3A59+2017+GMT&issuer=DC%3Dgov%2C+DC%3Dva%2C+OU%3DServices%2C+OU%3DPKI%2C+CN%3DVeterans+Affairs+User+CA+B1";
                    //strBlueCoatCert = "sno=1959515+%280x1de65b%29&subject=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+OU%3DCONTRACTOR%2C+CN%3DDEL+GROSSO.AMANDA.1291272521&validfrom=Nov++7+00%3A00%3A00+2017+GMT&validto=Oct+10+23%3A59%3A59+2020+GMT&issuer=C%3DUS%2C+O%3DU.S.+ Government%2C+OU%3DDoD%2C+OU% 3DPKI%2C+CN%3DDOD+ID+CA-44&policy_oids=2.16.840.1.101.2.1.11.42%2C2.16.840.1.101.3.2.1.3.13";
                    //strBlueCoatCert = "sno=1673761+%280x198a21%29&amp;subject=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+OU%3DDHA%2C+CN%3DPOWELL.JOSEPH.R.1214655175&amp;validfrom=Sep+16+00%3A00%3A00+2019+GMT&amp;validto=Sep+15+23%3A59%3A59+2022+GMT&amp;issuer=C%3DUS%2C+O%3DU.S.+Government%2C+OU%3DDoD%2C+OU%3DPKI%2C+CN%3DDOD+ID+CA-49&amp;policy_oids=2.16.840.1.101.2.1.11.42%2C2.16.840.1.101.3.2.1.3.13&";

                if (strBlueCoatCert != null && strBlueCoatCert.Contains("sno")) { bluecoatCacFound = true; }

                #endregion PULL CAC CERTIFICATE

                if (cacFound || bluecoatCacFound)
                {
                    #region CERT FOUND

                    string CAC_ID = string.Empty;
                    string cnIssuerCert = string.Empty;
                    bool isApprovedAffiliation = false;
                    bool isApprovedIssuer = false;

                    if (bluecoatCacFound)
                    {
                        strBlueCoatCert = strBlueCoatCert.Replace("%2C", ",").Replace("%3D", "=").Replace("&amp;", "&");

                        // Not needed at this time.  Saving for future need
                        #region Interpret BlueCoat Reverse Proxy Client Cert
                            // Not needed at this time.  Saving for future need
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

                        #region Extract bluecoatCert
                        foreach (string certificateElements in strBlueCoatCert.Split('&'))
                            {
                                if (certificateElements.StartsWith("subject="))
                                {
                                    foreach (string subjectElement in certificateElements.Split(','))
                                    {
                                        if (subjectElement.StartsWith("+CN="))
                                        {
                                            Match CacIDMatch = Regex.Match(subjectElement, @"[0-9]{4,10}"); // 4 to 10 digits 
                                            if (CacIDMatch.Success)
                                            {
                                                CAC_ID = CacIDMatch.Value;
                                            }
                                            else
                                            {
                                                //Possibly could be Monitoring soft cert from monitoring application
                                                if (subjectElement.Contains(ConfigurationManager.AppSettings["MonitoringCertFriendlyName"])) //topazbpm.med.osd.mil
                                                {
                                                    CAC_ID = ConfigurationManager.AppSettings["MonitoringCertCAC_ID"]; //topazbpm
                                                }
                                            }
                                        }
                                        if (subjectElement.StartsWith("+OU="))
                                        {
                                            foreach (string affiliate in ConfigurationManager.AppSettings["ApprovedAffiliateList"].Split(','))
                                            {
                                                if (subjectElement.Contains(affiliate))
                                                {
                                                    isApprovedAffiliation = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (certificateElements.StartsWith("issuer="))
                                {
                                    foreach (string issuerElements in certificateElements.Split(','))
                                    {
                                        Match issuerCertMatch = Regex.Match(issuerElements, @"CN\=(.+)"); //

                                        if (issuerCertMatch.Success)
                                        {
                                            cnIssuerCert = issuerCertMatch.Groups[1].Value.Replace('+', ' ');
                                        }
                                    }
                                }
                                if (certificateElements.StartsWith("policy_oids=")) //CR SCR-T 0059883  Handle the RMF requirement for PIV
                                {
                                    char[] delimiterChars = { '=', ',' };
                                    string[] policyObjectIdentifierElements = certificateElements.Split(delimiterChars);

                                    isPIVCert = policyObjectIdentifierElements.Any(s => ConfigurationManager.AppSettings["PIVCertPolicyObjectID"].Contains(s));
                                }
                            }
                        foreach (string approvedIssuer in ConfigurationManager.AppSettings["ApprovedIssuerList"].Split(','))
                            {
                                if (cnIssuerCert.StartsWith(approvedIssuer))
                                {
                                    isApprovedIssuer = true;
                                    break;
                                }
                            }
                        #endregion Extract bluecoatCert
                    }
                    else
                    {   // cac found
                        #region Extract CAC Info
                        var cert = new X509Certificate2(cacCert.Certificate);

                        cnIssuerCert = cert.Issuer.Split(commaDelim)[0].Substring(3);

                        //SUBJECT
                        string cnSubjectCert = cert.Subject.Split(commaDelim)[0];

                        foreach (string subjectElement in cert.Subject.Split(','))
                        {
                            if (subjectElement.StartsWith("CN="))
                            {
                                Match CacIDMatch = Regex.Match(subjectElement, @"[0-9]{4,10}"); // 4 to 10 digits 
                                if (CacIDMatch.Success)
                                {
                                    CAC_ID = CacIDMatch.Value;
                                }
                                else
                                {
                                    //Possibly could be Monitoring soft cert from monitoring application
                                    if (subjectElement.Contains(ConfigurationManager.AppSettings["MonitoringCertFriendlyName"])) //topazbpm.med.osd.mil
                                    {
                                        CAC_ID = ConfigurationManager.AppSettings["MonitoringCertCAC_ID"]; //topazbpm
                                    }
                                }
                            }
                            if (subjectElement.StartsWith(" OU="))
                            {
                                foreach (string affiliate in ConfigurationManager.AppSettings["ApprovedAffiliateList"].Split(','))
                                {
                                    if (subjectElement.Contains(affiliate))
                                    {
                                        isApprovedAffiliation = true;
                                        break;
                                    }
                                }
                            }
                        }

                        string userprincipalname = cert.GetNameInfo(X509NameType.UpnName, false);
                        isPIVCert = userprincipalname.Count(Char.IsDigit) > 10 ? true : false;

                        foreach (string approvedIssuer in ConfigurationManager.AppSettings["ApprovedIssuerList"].Split(','))
                        {
                            if (cnIssuerCert.StartsWith(approvedIssuer))
                            {
                                isApprovedIssuer = true;
                                break;
                            }
                        }
                        #endregion Extract CAC Info
                    }

                    #region TEST CAC VARIABLE
                    if (CAC_ID != string.Empty)
                    {
                        if (!isPIVCert)
                        {
                            Session["ErrorMessage"] = "You did not select your PIV Authentication certificate. Please try again." + "</br></br >" + " If you continue to see this message, you may need to close your browser and then try again. " + "</br></br >" + "  If you continue to experience problems, contact the help desk at 1-800-600-9332 for further assistance.";
                            Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
                        }
                        else if (isPIVCert && isApprovedAffiliation && isApprovedIssuer)
                        {
                            #region CAC LOGIN
                            if (_presenter.GetPatientByID(CAC_ID, "DIN", _clinicCode))
                            {
                                Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
                            }
                            else
                            {
                                // Issue with getPatientByID
                                Session["ErrorMessage"] = "We are sorry, your information was not found in SRTSweb.  If you believe this is an error, please contact the help desk at 1-800-600-9332 for further assistance.";
                                Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
                            }
                            #endregion CAC LOGIN
                        }
                        else
                        {
                            Session["ErrorMessage"] = "An error occurred.  Please close your browser and try again.  If you continue to experience problems, contact the help desk at 1-800-600-9332 for further assistance.";
                            Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
                        }
                    }
                    else
                    {
                        #region CAC variable not found
                        Session["ErrorMessage"] = "<br><br>The System could not read the CAC certificate.<br>Click the X to close this window and try again.<br><br>";
                        Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
                        #endregion CAC variable not found
                    }
                    #endregion TEST CAC VARIABLE
                    #endregion CERT FOUND
                }
                else
                {
                    #region CERT NOT FOUND
                        Session["ErrorMessage"] = "<br><br>The System did not detect a valid CAC certificate.<br>Click the X to close this window and try again.<br><br>";
                        Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
                        #endregion CERT NOT FOUND
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != "Thread was being aborted.")
                {
                    Exception exx = new Exception();
                    exx.LogException("In JSpecsGetCacCert" + ex.Message);
                }
            }
        }

        public JSpecsSession userInfo
        {
            get { return (JSpecsSession)Session["userInfo"]; }
            set { Session.Add("userInfo", value); }
        }
        public string ClinicCode
        {
            get { return _clinicCode;  }
            set { _clinicCode = value;  }
        }
        public string ErrorMessage
        {
            set
            {
                ShowMessage_Redirect(this.Page, value, "/WebForms/JSpecs/Forms/JSpecsFAQ.aspx");
            }
        }
    }
}