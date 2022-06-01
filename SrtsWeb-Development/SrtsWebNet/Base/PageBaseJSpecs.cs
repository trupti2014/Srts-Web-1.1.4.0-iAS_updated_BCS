using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;

namespace SrtsWeb.Base
{
    public class PageBaseJSpecs : System.Web.UI.Page, ICustomEventLogger
    {

        private void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            AutoRedirectAfterSessionEnd();
        }

        /// <summary>
        /// Redirect user when session expires.
        /// </summary>
        public void AutoRedirectAfterSessionEnd()
        {
            int int_MilliSecondsTimeOut = (this.Session.Timeout * 60000);
            string str_Script = @"
               <script type='text/javascript'> 
                   intervalset = window.setInterval('Redirect()'," +
                       int_MilliSecondsTimeOut.ToString() + @");
                   function Redirect()
                   {
                       window.location.href='/WebForms/JSpecs/Forms/JSpecsLogin.aspx'; 
                   }
               </script>";

            ClientScript.RegisterClientScriptBlock(this.GetType(), "Redirect", str_Script);
        }

        /// <summary>
        /// Alert user with error messages
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Message"></param>
        /// <param name="Redirect_URL"></param>
        public void ShowMessage_Redirect(System.Web.UI.Page page, string Message, string Redirect_URL)
        {

            string alertMessage = "<script language=\"javascript\" type=\"text/javascript\">";

            alertMessage += "alert('" + Message + "');";
            alertMessage += "window.location.href=\"";
            alertMessage += Redirect_URL; //currently set to .... WebForms/JSpecs/Forms/JSpecsFAQ.aspx
            alertMessage += "\";";
            alertMessage += "</script>";

            ClientScript.RegisterClientScriptBlock(GetType(), "alertMessage ", alertMessage);

        }

        public List<LookupTableEntity> LookupCache
        {
            get
            {
                if (Cache["SRTSLOOKUP"] == null)
                {
                    LoadLookupTable();
                }
                return Cache["SRTSLOOKUP"] as List<LookupTableEntity>;
            }
            set { Cache["SRTSLOOKUP"] = value; }
        }

        private void LoadLookupTable()
        {
            SrtsWeb.BusinessLayer.Abstract.ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        }

        public void LogEvent(string message)
        {
            var cel = new CustomEventLogger();
            cel.LogEvent(message);
        }

        public void LogEvent(string messageFormat, object[] formatParams)
        {
            var cel = new CustomEventLogger();
            cel.LogEvent(messageFormat, formatParams);
        }
    }
}