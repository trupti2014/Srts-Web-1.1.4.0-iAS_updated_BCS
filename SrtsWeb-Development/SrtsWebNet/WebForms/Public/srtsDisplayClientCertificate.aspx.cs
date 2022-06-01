using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using SrtsWeb.Account;

namespace SrtsWeb.Public
{
    public partial class srtsDisplayClientCertificate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string para = "<div style='margin: 10px 0 0 0; font-weight: bold'>{0}</div>";
            string subpara = "<div style='margin-left: 15px; font-size: 90%'>{0}</div>";

            if (Request.ClientCertificate.IsPresent)
            {
                Response.Write("<hr /><div style='width: 500px; margin: 20px auto'>");
                Response.Write("<h3 style='width: 500px; margin: 20px auto'>Client Certificate Information</h3>");
                try
                {
                    X509Certificate2 x509Cert2 = new X509Certificate2(Page.Request.ClientCertificate.Certificate);

                    Response.Write(string.Format(para, "Issued To:"));
                    Response.Write(string.Format(subpara, x509Cert2.Subject));

                    Response.Write(string.Format(para, "Issued By:"));
                    Response.Write(string.Format(subpara, x509Cert2.Issuer));

                    Response.Write(string.Format(para, "Friendly Name:"));
                    Response.Write(string.Format(subpara, string.IsNullOrEmpty(x509Cert2.Issuer) ? "(None Specified)" : x509Cert2.Issuer));

                    Response.Write(string.Format(para, "Valid Dates:"));
                    Response.Write(string.Format(subpara, "From: " + x509Cert2.GetEffectiveDateString()));
                    Response.Write(string.Format(subpara, "To: " + x509Cert2.GetExpirationDateString()));

                    Response.Write(string.Format(para, "Thumbprint:"));
                    Response.Write(string.Format(subpara, x509Cert2.Thumbprint));

                    Response.Write(string.Format(para, "Serial Number:"));
                    Response.Write(string.Format(subpara, x509Cert2.SerialNumber));


                    //Response.Write(string.Format(para, "Public Key:"));
                    //Response.Write(string.Format(subpara, x509Cert2.GetPublicKeyString()));

                    #region EKU Section - Retrieve EKU info and write out each OID
                    X509EnhancedKeyUsageExtension ekuExtension = (X509EnhancedKeyUsageExtension)x509Cert2.Extensions["Enhanced Key Usage"];
                    if (ekuExtension != null)
                    {
                        Response.Write(string.Format(para, "Enhanced Key Usages (" + ekuExtension.EnhancedKeyUsages.Count.ToString() + " found)"));

                        OidCollection ekuOids = ekuExtension.EnhancedKeyUsages;
                        foreach (Oid ekuOid in ekuOids)
                            Response.Write(string.Format(subpara, ekuOid.FriendlyName + " (OID: " + ekuOid.Value + ")"));
                    }
                    else
                    {
                        Response.Write(string.Format(para, "No EKU Section Data"));
                    }
                    #endregion // EKU Section

                    #region Subject Alternative Name Section
                    X509Extension sanExtension = (X509Extension)x509Cert2.Extensions["Subject Alternative Name"];
                    if (sanExtension != null)
                    {
                        Response.Write(string.Format(para, "Subject Alternative Name:"));
                        Response.Write(string.Format(subpara, sanExtension.Format(true)));
                    }
                    else
                    {
                        Response.Write(string.Format(para, "No Subject Alternative Name Data"));
                    }

                    #endregion // Subject Alternative Name Section

                    #region Certificate Policies Section
                    X509Extension policyExtension = (X509Extension)x509Cert2.Extensions["Certificate Policies"];
                    if (policyExtension != null)
                    {
                        Response.Write(string.Format(para, "Certificate Policies:"));
                        Response.Write(string.Format(subpara, policyExtension.Format(true)));
                    }
                    else
                    {
                        Response.Write(string.Format(para, "No Certificate Policies Data"));
                    }
                    #endregion //Certificate Policies Section


                    // Example on how to enumerate all extensions
                    //foreach (X509Extension extension in x509Cert2.Extensions)
                    //    Response.Write(string.Format(para, extension.Oid.FriendlyName + "(" + extension.Oid.Value + ")"));
                }
                catch (Exception ex)
                {
                    Response.Write(string.Format(para, "An error occured:"));
                    Response.Write(string.Format(subpara, ex.Message));
                    Response.Write(string.Format(subpara, ex.StackTrace));
                }
                finally
                {
                    Response.Write("</div>");
                }

            }
        }
    }
}
