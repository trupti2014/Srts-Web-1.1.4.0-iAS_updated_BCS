using Microsoft.Reporting.WebForms;
using SrtsWeb.Base;
using SrtsWeb.Views.Reporting;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Permissions;

namespace SrtsWeb.Reports
{
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabMail")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtDataMgmt")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class rptViewerTemplate : PageBase, IOrder771View
    {
        private string rptID = string.Empty;
        private string rptTitle = string.Empty;
        private bool isReprint = true;
        private string reprintordernumber = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Master._MainMenu.Visible = true;
                Master._ContentAuthenticated.Visible = true;
                rptID = (Request.QueryString["id"]);
                rptTitle = (Request.QueryString["title"]);
                isReprint = bool.Parse((Request.QueryString["isreprint"]).ToString());
                if (isReprint)
                {
                    reprintordernumber = (Request.QueryString["ordernumber"]);
                }

                CurrentModule(" <br />" + "SRTSweb Report Manager - ");
                CurrentModule_Sub(rptTitle);
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                Master.uplCurrentModuleTitle.Update();

                GetReport();
            }
        }

        private void GetReport()
        {
            string connStr = SrtsWeb.ExtendersHelpers.Globals.ConnStrNm;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.ConnectionString = ConfigurationManager.ConnectionStrings[connStr].ConnectionString;

            rptViewerSvr.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
            rptViewerSvr.ServerReport.ReportServerCredentials = new SrtsWeb.Reports.CustomReportCredentials(ConfigurationManager.AppSettings.Get("ReportServerUsername"), ConfigurationManager.AppSettings.Get("ReportServerPwd"), ConfigurationManager.AppSettings.Get("ReportServerDomain"));

            rptViewerSvr.ServerReport.ReportPath = string.Format("{0}", rptID);

            rptViewerSvr.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings.Get("ReportServerUrl"));

            switch (rptID)
            {
                case "/LabRoutingForm":
                case "/DDForm771a":
                    // This parameter is added in ManageOrdersLabPresenter, or SrtsReportsManager
                    var o = Session["OrderNbrs"];
                    var orderNbr = new ReportParameter("OrderNbr", o == null ? null : o.ToString());
                    var modifiedby = new ReportParameter("ModifiedBy", mySession.ModifiedBy);
                    var site = new ReportParameter("SiteCode", mySession.MySite.SiteCode);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { site, orderNbr, modifiedby });
                    break;

                case "/Labels/Single Label":
                case "/Labels/Avery 5160":
                    var siteCode = new ReportParameter("SiteCode", mySession.MySite.SiteCode);
                    var statusTypeId = new ReportParameter("OrderSTatusTypeID", Session["StatusId"].ToString());
                    var modDt = new ReportParameter("DateTime", Session["BatchDate"].ToString());

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { siteCode, statusTypeId, modDt });

                    Session.Remove("StatusId");
                    Session.Remove("BatchDate");
                    break;

                case "/Lab Reports/Rpt54":
                    var labsitecode = new ReportParameter("LabSiteCode", mySession.MySite.SiteCode);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { labsitecode });
                    break;

                case "/WWReport":
                    var clinicsitecode = new ReportParameter("ClinicSiteCode", mySession.MyClinicCode);
                    var lSiteCode = new ReportParameter("LabSiteCode", mySession.MySite.SiteCode);
                    var modby = new ReportParameter("ModifiedBy", mySession.ModifiedBy);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { clinicsitecode, lSiteCode, modby });
                    break;

                case "/Clinic Reports/ClinicDispenseRpt":
                    var csitecode = new ReportParameter("SiteCode", mySession.MyClinicCode);
                    var modby2 = new ReportParameter("ModifiedBy", mySession.ModifiedBy);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode, modby2 });
                    break;

                case "/Clinic Reports/ClinicOrdersRpt":
                    var csitecode3 = new ReportParameter("SiteCode", mySession.MyClinicCode);
                    var modby3 = new ReportParameter("ModifiedBy", mySession.ModifiedBy);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode3, modby3 });
                    break;

                case "/Clinic Reports/ClinicOverdueOrders":
                    var csitecode4 = new ReportParameter("SiteCode", mySession.MyClinicCode);
                    var modby4 = new ReportParameter("ModifiedBy", mySession.ModifiedBy);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode4, modby4 });
                    break;

                case "/Clinic Reports/ClinicProductionRpt":
                    var csitecode1 = new ReportParameter("SiteCode", mySession.MyClinicCode);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode1 });
                    break;

                case "/Clinic Reports/ClinicSummaryRpt":
                    var csitecode2 = new ReportParameter("SiteCode", mySession.MyClinicCode);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode2 });
                    break;

                case "/Clinic Reports/OrderDetailReport":
                    var csitecode5 = new ReportParameter("SiteCode", mySession.MyClinicCode);
                    var modby5 = new ReportParameter("ModifiedBy", mySession.ModifiedBy);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode5, modby5 });
                    break;

                case "/Clinic Reports/TurnAroundTimeRpt":
                    var csitecode6 = new ReportParameter("SiteCode", mySession.MyClinicCode);

                    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode6 });
                    break;
                //case "/Clinic Reports/ClinicGroupRpt": //FS 235: Reporting by groups
                //    var csitecode7 = new ReportParameter("SiteCode", mySession.MyClinicCode);

                //    rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { csitecode7 });
                //    break;
            }
            var datasrc = new ReportParameter("datasrc", builder["Data Source"].ToString()); /* These are not needed anymore */
            var initcat = new ReportParameter("initcat", builder["Initial Catalog"].ToString()); /* These are not needed anymore */

            rptViewerSvr.ServerReport.SetParameters(new ReportParameter[] { datasrc, initcat }); /* These are not needed anymore */

            rptViewerSvr.ShowParameterPrompts = true;
            rptViewerSvr.ShowExportControls = true;
        }

        public DataSet Order771
        {
            get { return (DataSet)ViewState["Order771"]; }
            set { ViewState.Add("Order771", value); }
        }
    }
}