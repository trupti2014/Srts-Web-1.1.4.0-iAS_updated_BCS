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
using SrtsWeb.Base;

namespace SrtsWeb.Reports
{
    public partial class RptViewer : PageBase
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string rpt = (string)Session["SelReport"];

            //This page displays all reports from the SSRS Report Server.
            rptViewerSvr.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
            //rptViewerSvr.ServerReport.ReportServerUrl = new Uri("http:        // - Report Server URL in Smart Tag
            
            //Get correct report based upon user selection
            try
            {
                switch (rpt)
                {
                    case "Blank 771 Form":
                        rptViewerSvr.ServerReport.ReportPath = "/Blank_DD771";
                        break;
                    case "Patient 771":
                        rptViewerSvr.ServerReport.ReportPath = "/DDForm771a";
                        break;
                    case "Report 54":
                        rptViewerSvr.ServerReport.ReportPath = "/Lab Reports/Rpt54";         //Report path/name
                        ReportParameter labsitecode = new ReportParameter("LabSiteCode", mySession.MySite.SiteCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { labsitecode });
                        break;
                    case "Wounded Warrior Report":
                        rptViewerSvr.ServerReport.ReportPath = "/Lab Reports/WWReport";
                        ReportParameter clinicsitecode = new ReportParameter("ClinicSiteCode", mySession.MyClinicCode.ToString());
                        ReportParameter lSiteCode = new ReportParameter("LabSiteCode", mySession.MySite.SiteCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { clinicsitecode });
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { lSiteCode });
                        break;
                    case "Clinic Dispense Report":
                        rptViewerSvr.ServerReport.ReportPath = "/Clinic Reports/ClinicDispenseRpt";
                        ReportParameter csitecode = new ReportParameter("ClinicSiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode });

                        break;
                    case "Clinic Production Report":
                        rptViewerSvr.ServerReport.ReportPath = "/Clinic Reports/ClinicProductionRpt";
                        ReportParameter csitecode1 = new ReportParameter("ClinicSiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode1 });
                        break;
                    case "Clinic Summary Report":
                        rptViewerSvr.ServerReport.ReportPath = "/Clinic Reports/ClinicSummaryRpt";
                        ReportParameter csitecode2 = new ReportParameter("ClinicSiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode2 });
                        break;
                    //case "Blank 771 Form":
                    //    rptViewerSvr.ServerReport.ReportPath = "/Blank_DD771";
                    //    break;
                    //case "Patient 771":
                    //    rptViewerSvr.ServerReport.ReportPath = "/DDForm771a";
                    //    break;
                    //case "Report 54":
                    //    rptViewerSvr.ServerReport.ReportPath = "/Lab Reports/Rpt54";         //Report path/name
                    //    break;
                    //case "Wounded Warrior Report":
                    //    rptViewerSvr.ServerReport.ReportPath = "/Lab Reports/WWReport";
                    //    break;
                    //case "Clinic Dispense Report":
                    //    rptViewerSvr.ServerReport.ReportPath = "/Clinic Reports/ClinicDispenseRpt";
                    //    break;
                    //case "Clinic Production Report":
                    //    rptViewerSvr.ServerReport.ReportPath = "/Clinic Reports/ClinicProductionRpt";
                    //    break;
                    //case "Clinic Summary Report":
                    //    rptViewerSvr.ServerReport.ReportPath = "/Clinic Reports/ClinicSummaryRpt";
                    //    break;
                }

                //if (mySession != null)
                //{
                //    if (rpt != "Wounded Warrior Report")
                //    {
                //        ReportParameter site = new ReportParameter("SiteCode", mySession.MySite.SiteCode.ToString());
                //        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { site });
                //    }
                //}

                rptViewerSvr.ShowParameterPrompts = true;
                rptViewerSvr.ShowPrintButton = true;
                rptViewerSvr.ShowExportControls = true;
            }
            catch (SystemException ex)
            {
                //ExceptionUtility.LogException(ex, "Error in Getting Selected Report in Report Manager...");
            }
            finally
            {

            }
                    
        }
    }
}