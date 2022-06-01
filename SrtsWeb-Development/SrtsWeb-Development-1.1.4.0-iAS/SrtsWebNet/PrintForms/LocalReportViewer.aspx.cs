using Microsoft.Reporting.Common;
using Microsoft.Reporting.WebForms;
using Microsoft.ReportingServices.WebServer;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.CustomErrors;
using SrtsWeb.PrintForms;

namespace SrtsWeb.PrintForms
{
    public partial class LocalReportViewer : PageBase 
    {
        //THis page displays local reports, not remote.  For remote use RptViewer.

        protected void Page_Load(object sender, EventArgs e)
        {
            
            RptViewerLocal.Visible = true;
            ReportDataSource rds = new ReportDataSource();

            RptViewerLocal.Reset();
            RptViewerLocal.ProcessingMode = ProcessingMode.Local;

            RptViewerLocal.LocalReport.ReportPath = "\\PrintForms\\Avery5160_5260.rdlc";
            
            RptViewerLocal.LocalReport.DataSources.Clear();
            RptViewerLocal.LocalReport.DataSources.Add(rds);
                       
        }
    }
}