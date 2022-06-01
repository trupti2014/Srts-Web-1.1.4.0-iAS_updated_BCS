using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting;
using Microsoft.ReportingServices;
using Microsoft.Reporting.WebForms;


namespace SrtsWeb.Reports
{
    public partial class ClinicOrders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

      //  [DefaultValueAttribute(ContentDisposition.OnlyHtmlInline)]
      //  [WebBrowsableAttribute(true)]
        public ContentDisposition ExportContentDisposition { get; set; }
    }
}