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
    public partial class ReportPageTemplate : PageBase

    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master._MainPageFooter.Visible = true;
            Master._MainMenu.Visible = false;
            Master._ContentAuthenticated.Visible = false;
            mySession.SecurityAcknowledged = true;
            ////////////////////////////////////// shows apppropriate page titles
            CurrentModule("SRTSweb Administration");
            CurrentModule_Sub(" <br /> Report Title");
            SiteMap.SiteMapResolve += new SiteMapResolveEventHandler(this.BuildBreadCrumbs);
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
            Master.uplCurrentModuleTitle.Update();
            litModuleTitle.Text = Master.CurrentModuleTitle;
            //////////////////////////////////////

          
            ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

            string rpt = Request.QueryString["Title"];

            try
            {
                switch (rpt)
                {
                    case "Blank 771 Form":
                        ReportViewer2.ServerReport.ReportPath = "/Blank_DD771";
                        break;
                    case "Patient 771":
                        ReportViewer2.ServerReport.ReportPath = "/DDForm771a";
                        break;
                    case "Report 54":
                        ReportViewer2.ServerReport.ReportPath = "/Lab Reports/Rpt54";         //Report path/name
                        ReportParameter labsitecode = new ReportParameter("LabSiteCode", mySession.MySite.SiteCode.ToString());
                        ReportViewer2.ServerReport.SetParameters(new ReportParameter[] { labsitecode });
                        break;
                    case "Wounded Warrior Report":
                        ReportViewer2.ServerReport.ReportPath = "/Lab Reports/WWReport";
                        ReportParameter clinicsitecode = new ReportParameter("ClinicSiteCode", mySession.MyClinicCode.ToString());
                        ReportParameter lSiteCode = new ReportParameter("LabSiteCode", mySession.MySite.SiteCode.ToString());
                        ReportViewer2.ServerReport.SetParameters(new ReportParameter[] {clinicsitecode});
                        ReportViewer2.ServerReport.SetParameters(new ReportParameter[] { lSiteCode });
                        break;
                    case "Clinic Dispense Report":
                        ReportViewer2.ServerReport.ReportPath = "/Clinic Reports/ClinicDispenseRpt";
                        ReportParameter csitecode = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                         ReportViewer2.ServerReport.SetParameters(new ReportParameter[] {csitecode});

                        break;
                    case "Clinic Production Report":
                        ReportViewer2.ServerReport.ReportPath = "/Clinic Reports/ClinicProductionRpt";
                         ReportParameter csitecode1 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                         ReportViewer2.ServerReport.SetParameters(new ReportParameter[] {csitecode1});
                      break;
                    case "Clinic Summary Report":
                        ReportViewer2.ServerReport.ReportPath = "/Clinic Reports/ClinicSummaryRpt";
                        ReportParameter csitecode2 = new ReportParameter("SiteCode", mySession.MyClinicCode.ToString());
                        ReportViewer2.ServerReport.SetParameters(new ReportParameter[] {csitecode2});
                       break;
                }

                ReportViewer2.ShowParameterPrompts = true;
                ReportViewer2.ShowPrintButton = true;
                ReportViewer2.ShowExportControls = true;
                ReportViewer2.Visible = true;

            }
            catch (SystemException ex)
            {
                ExceptionUtility.LogException(ex, "Error in Getting Selected Report in Report Manager...");
            }
            finally
            {
            }
        }

        private SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode currentNode = null;
            if (Master.CurrentSiteMapPath != null)
            {
                currentNode = SiteMap.RootNode.Clone(true);
                SiteMapNode tempNode = currentNode;
                SiteMapNode tempNodeChild = null;
                SiteMapNode tempNodeParent = tempNode.Clone(true);
                SiteMapNode tempNodeParentRoot = tempNode.Clone(true);

                string path = string.Empty;

                if (HttpContext.Current.Request.RawUrl.Length != 0)
                {
                    path = HttpContext.Current.Request.RawUrl;
                }
                switch (path)
                {
                    case "/Reports/Accessability.aspx":
                        tempNodeParentRoot.Title = string.Format("{0}", "SRTSweb");
                        tempNodeParentRoot.Description = string.Format("{0}", "SRTSweb");
                        tempNodeParentRoot.Url = "~/Default.aspx";

                        tempNode.Title = string.Format("{0}", "SrtsWeb Home");
                        tempNode.Description = string.Format("{0}", "SrtsWeb Home");
                        tempNode.Url = "~/Default.aspx";

                        tempNodeChild = SiteMap.RootNode.Clone(true);
                        tempNodeChild.Title = string.Format("{0}", "Report Title");
                        tempNodeChild.Description = string.Format("{0}", "Report Description");
                        tempNodeChild.Url = "~/Reports/Accessability.aspx";
                        break;
                }
                return currentNode;
            }
            else
            {
                return SiteMap.CurrentNode;
            }
        }

        protected void BuildUserInterface()
        {
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
        }
    }
}