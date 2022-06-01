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


namespace SrtsWeb.Reports
{
    public partial class rptViewerTemplate : PageBase
    {
        string rptID = string.Empty;
        string rptTitle = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Master._MainPageFooter.Visible = true;
                Master._MainMenu.Visible = true;
                Master._ContentAuthenticated.Visible = true;
                rptID = (Request.QueryString["id"]);
                rptTitle = (Request.QueryString["title"]);
                ////////////////////////////////////// shows apppropriate page titles
                CurrentModule(" <br />" + "SRTSweb Report Manager - ");
                CurrentModule_Sub(rptTitle);
                SiteMap.SiteMapResolve += new SiteMapResolveEventHandler(this.BuildBreadCrumbs);
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                Master.uplCurrentModuleTitle.Update();
                //////////////////////////////////////
                GetReport();
            }
        }

        private void GetReport()
        {
           
                //displays selected report from the SSRS Report Server.
                rptViewerSvr.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

                rptViewerSvr.ServerReport.ReportPath = string.Format("{0}", rptID);         //report path and name as on report server
                
              //add parameters for specific reports here.....
                
                switch (rptID)
                {
                    case ("/DDForm771a"):
                        ReportParameter site = new ReportParameter("SiteCode", mySession.MySite.SiteCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { site });
                    break;
                    
                    case ("/Lab Reports/Rpt54"):
                        ReportParameter labsitecode = new ReportParameter("LabSiteCode", mySession.MySite.SiteCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { labsitecode });
                    break;

                    case ("/WWReport"):
                        ReportParameter clinicsitecode = new ReportParameter("ClinicSiteCode", mySession.MyClinicCode.ToString());
                        ReportParameter lSiteCode = new ReportParameter("LabSiteCode", mySession.MySite.SiteCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { clinicsitecode });
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { lSiteCode });
                    break;
                
                    case("/Clinic Reports/ClinicDispenseRpt"):
                        ReportParameter csitecode = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode });
                    break;

                    case ("/Clinic Reports/ClinicOrdersRpt"):
                        ReportParameter csitecode3 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode3 });
                    break;

                    case("/Clinic Reports/ClinicOverdueOrders"):
                        ReportParameter csitecode4 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode4 });
                    break;

                    case("/Clinic Reports/ClinicProductionRpt"):
                        ReportParameter csitecode1 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode1 });
                    break;

                    case("/Clinic Reports/ClinicSummaryRpt"):
                        ReportParameter csitecode2 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode2 });
                    break;

                    case ("/Clinic Reports/OrderDetailReport"):
                        ReportParameter csitecode5 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode5 });
                    break;

                    case ("/Clinic Reports/TurnAroundTimeRpt"):
                        ReportParameter csitecode6 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                        rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode6 });
                    break;

                    // Below reports are Unit Specific waiting for approval to implement, designed and ready to go

                    //case ("OrdersByUnitRpt"):
                    //    ReportParameter csitecode6 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                    //    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode6 });
                    //break;

                    //case ("UnitDispenseRpt"):
                    //    ReportParameter csitecode7 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                    //    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode7 });
                    //break;
                }

                rptViewerSvr.ShowParameterPrompts = true;
                rptViewerSvr.ShowPrintButton = true;
                rptViewerSvr.ShowExportControls = true;

 
           }

        private SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode currentNode = null;
            if (Master.CurrentSiteMapPath != null)
            {
                currentNode = SiteMap.RootNode.Clone(true);
                SiteMapNode tempNode = currentNode;
                SiteMapNode tempNodeParentRoot = tempNode.Clone(true);

                if (HttpContext.Current.Request.RawUrl.Length != 0)
                {
                    tempNodeParentRoot.Title = string.Format("{0}", "SRTSweb");
                    tempNodeParentRoot.Description = string.Format("{0}", "SRTSweb");
                    tempNodeParentRoot.Url = "~/Default.aspx";

                    tempNode.Title = string.Format("{0}", "SRTSweb Report Manager");
                    tempNode.Description = string.Format("{0}", "SRTSweb Administration - Report Manager");
                    tempNode.Url = "~/Admin/SrtsReportManager.aspx";
                    return currentNode;
                }
                else
                {
                    return SiteMap.CurrentNode;
                }
            }
            else
            {
                return SiteMap.CurrentNode;
            }
            
        }
    }
}