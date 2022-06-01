using SrtsWeb.Account;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace SrtsWeb.WebForms.Reports
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        private string _logonToken;
        private string _URL = ConfigurationManager.AppSettings["BCSURL"];
        private static HttpClient _client = new HttpClient();
        private string _documentID = string.Empty;
        private string _pdfFileName = string.Empty;
        private string _jsonBody = string.Empty;
        private int _maxTries = 5;
        private CurrentUser _myCurrentUser = new CurrentUser();
        private string rptTitle = string.Empty;


        public void ProcessRequest(HttpContext context)
        {

            //HttpWebRequest exportRequest = (HttpWebRequest)WebRequest.Create(_URL + urlParameters);
            //exportRequest.Method = "GET";
            //exportRequest.Accept = "application/pdf";
            //exportRequest.Headers.Add("X-SAP-LogonToken", _logonToken);
            //WebResponse myWebResponse = exportRequest.GetResponse();
            //FileStream stream = new FileStream(Request.PhysicalApplicationPath + "\\TMP\\output.pdf", FileMode.Create);
            //myWebResponse.GetResponseStream().CopyTo(stream);
            //stream.Close();
            //Response.Redirect("output.pdf");
        }

        //private HttpResponseMessage getLogonTokenFromBCS()
        //{
        //    try
        //    {
        //        string urlParameters = ConfigurationManager.AppSettings["BCSLogonToken"];

        //        HttpWebRequest exportRequest = (HttpWebRequest)WebRequest.Create(_URL + urlParameters);
        //        exportRequest.Method = "GET";
        //        exportRequest.Accept = "application/pdf";
        //        exportRequest.Headers.Add("x-sap-trusted-user", ConfigurationManager.AppSettings["BCSUser"]);

        //        HttpWebResponse jsonResult = (HttpWebResponse)exportRequest.GetResponse();

        //        if (jsonResult.Headers)
        //        {
        //            // Parse the response body.

        //            HttpHeaders headers = jsonResult.Headers;
        //            IEnumerable<string> values;
        //            if (headers.TryGetValues("X-SAP-LogonToken", out values))
        //            {
        //                string responseString = values.First(); //jsonResult.Headers.ToString();

        //                // Decerialize the string to object
        //                //LogonToken logonToken = JsonConvert.DeserializeObject<LogonToken>(responseString);

        //                _logonToken = responseString;
        //            }//logonToken.logonToken;
        //            //mySession.BCSLogonToken = _logonToken;
        //            using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log2.txt")))
        //            {

        //                sw.WriteLine("_logonToken: " + _logonToken);


        //            }
        //        }

        //        return jsonResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        //ex.LogException("In rptViewerTemplate - getLogonTokenFromBCS" + ex.Message + " - " + ex.InnerException);
        //        throw ex;
        //    }
        //}

        private async Task getPDFResultFromBCSNoRefresh()
        {
            try
            {
                string urlParameters = string.Format(ConfigurationManager.AppSettings["BCSDocument"] + "{0}", _documentID);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _URL + urlParameters);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));
                request.Headers.Add("X-SAP-LOGONTOKEN", _logonToken);
                HttpResponseMessage pdfResult = await _client.SendAsync(request, CancellationToken.None);


                if (pdfResult.IsSuccessStatusCode)
                {
                    //returnPDFResponse(pdfResult);

                    savePDFtoFile(pdfResult);
                    
                }
                else if (pdfResult.StatusCode == HttpStatusCode.Unauthorized)
                {
                    using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log5.txt")))
                    {

                        sw.WriteLine("fail: " + pdfResult.StatusCode);

                    }
                    // Try a maximum tries to get the value before ending the loop to prevent infinite loop
                    if (_maxTries-- > 0)
                    {
                        // Possible logon token expired; reaquire logon token
                        //HttpResponseMessage authenticationResult = await getLogonTokenFromBCS();
                        //if (authenticationResult.IsSuccessStatusCode)
                        //{

                        //    // Try Again by calling itself
                        //    await getPDFResultFromBCSNoRefresh();
                        //}
                    }
                }
                else if (pdfResult.StatusCode == HttpStatusCode.BadRequest)
                {
                    // json body formatting incorrect
                }
                else if (pdfResult.StatusCode == HttpStatusCode.NotFound)
                {
                    // incorrect URL in request call; possibly DocumentID has changed
                }

            }
            catch (Exception ex)
            {
               // ex.LogException("In rptViewerTemplate - getPDFResultFromBCS" + ex.Message + " - " + ex.InnerException);
                throw ex;
            }
        }

        private void savePDFtoFile(HttpResponseMessage pdfResult)
        {
            try
            {
                byte[] pdfFileBuffer = pdfResult.Content.ReadAsByteArrayAsync().Result;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}