using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.WebForms.Admin
{
    public partial class LabelReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string pdffilename = (Request.QueryString["filename"]);
            if (pdffilename != "")
            {
                using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log13.txt")))
                {

                    sw.WriteLine("_pdfFileName" + pdffilename);

                }

                string src = ConfigurationManager.AppSettings["BCS_File_Loc"] + pdffilename;

                this.BCSFrame.Visible = true;
                this.BCSFrame.Src = src;
                using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log15.txt")))
                {

                    sw.WriteLine("src" + src);

                }
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
    }
}