using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Reporting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Xml;
using Aspose.Cells;

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

        private bool _isDevEnvironment = false;
        private string _debugString = "";
        private string _logonToken;
        private CurrentUser _myCurrentUser = new CurrentUser();
        // private string _pdfFileName = string.Empty;

        // Global form fields
        private string _URL = ConfigurationManager.AppSettings["BCSURL"];
        private string _documentID = string.Empty;
        private string _jsonBody = string.Empty;
        private string _labSiteCode = string.Empty;
        private string _clinicSiteCode = string.Empty;
        private string _modifiedBy = string.Empty;
        private int _maxTries = 5;

        private static HttpClient _client = new HttpClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            //var bcsUrl = ConfigurationManager.AppSettings["BCS-Url"];
            //var bcsKey = ConfigurationManager.AppSettings["BCS-EncryptionKey"];
            //var bcsUser = BCSReportHelper.GetBCSUser(mySession);
            //var encryptedUser = EncryptionHelper.Encrypt(bcsUser, bcsKey);
            //var redirectUrl = $"{bcsUrl}?bcsUser={Uri.EscapeDataString(encryptedUser)}";
            //var helper = new BCSReportHelper(); // Commented by Imrul
            //Response.Redirect(helper.GetLandingRedirectUrl()); // Commented by Imrul

            // Register Asynchronous Task
            // RegisterAsyncTask(new PageAsyncTask(getPDFResultFromBCS));

            Master._MainMenu.Visible = true;
            Master._ContentAuthenticated.Visible = true;
            rptID = (Request.QueryString["id"]);
            rptTitle = (Request.QueryString["title"]);
            isReprint = bool.Parse((Request.QueryString["isreprint"]).ToString());
            if (isReprint)
            {
                reprintordernumber = (Request.QueryString["ordernumber"]);
            }

            using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log1.txt")))
            {

                sw.WriteLine("rptID: " + rptID);
                sw.WriteLine("rptTitle: " + rptTitle);

            }
            CurrentModule(" <br />" + "SRTSweb Report Manager - ");
            CurrentModule_Sub(rptTitle);
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
            Master.uplCurrentModuleTitle.Update();

            //this.BCSFrame.Visible = true;
            //this.BCSFrame.Src = ConfigurationManager.AppSettings["BCS_File_Loc"] + "output.pdf";
            // _pdfFileName = ConfigurationManager.AppSettings["filepath"];
            //"BCS_Report_t.patel.me_Blank 771 Form_2022031895319.pdf";
            // this.BCSFrame.Visible = true;
            //string path = Request.Url+"TMP\\output.pdf";
            //if (File.Exists(path))
            //{
            //    using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log3.txt")))
            //    {
            //        sw.WriteLine("file exist: " + _pdfFileName);

            //    }
            //  if(this.BCSFrame.Visible)
            //        this.BCSFrame.Src = "output.pdf";//String.Format("~/TMP/{0}", _pdfFileName);
            //}

            //using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log2.txt")))
            //{
            //    sw.WriteLine("file path: " + path);

            //}
            _logonToken = mySession.BCSLogonToken;

            if (_logonToken.IsNullOrEmpty())
            {

               // RegisterAsyncTask(new PageAsyncTask(getLogonTokenFromBCS));
                getLogonTokenFromBCS();
            }

            if (!IsPostBack)
            {
                ShowReportParameters();
            }
            else
            {
                ShowReportsWithNoParameters();
            }

            if (_isDevEnvironment)
            {
                lblDebug.Visible = true;
                _debugString += lblDebug.Text;
                _debugString += "<br /> Header Values: <br />";
                NameValueCollection headers = Request.Headers;
                string[] keys = headers.AllKeys;
                foreach (string key in keys)
                {
                    _debugString += "<br />" + key + ": " + headers[key];
                }

                lblDebug.Text = _debugString;

            }
        }

        // This method is called with postback or after reports with parameters are shown
        private void ShowReportsWithNoParameters()
        {
            switch (rptID)
            {
                case "/Labels/Single Label":
                    _documentID = "15441434";

                    // var statusTypeId = new ReportParameter("OrderSTatusTypeID", Session["StatusId"].ToString());
                    string singleLabelModDt = Convert.ToDateTime(Session["BatchDate"].ToString()).ToString("yyyy-MM-ddTHH:mm:ss.000Z");

                    if (!mySession.MySite.SiteCode.IsNullOrEmpty() && !singleLabelModDt.IsNullOrEmpty())
                    {
                        string singleLabelOrderNumberJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {}},";
                        string singleLabelDateTimeJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + singleLabelModDt + "\"]}}},";

                        string singleLabelSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + mySession.MySite.SiteCode + "\"]}}}";

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        singleLabelOrderNumberJsonParameter +
                                        singleLabelDateTimeJsonParameter +
                                        singleLabelSiteCodeJsonParameter +
                                    "]}}";
                    }

                    ReportControlPanel.Visible = false;
                   // RegisterAsyncTask(new PageAsyncTask(getPDFResultFromBCS));
                    getLogonTokenFromBCS();
                    Session.Remove("StatusId");
                    Session.Remove("BatchDate");
                    break;

                case "/Labels/Avery 5160":
                    _documentID = "15441427";

                    string averyModDt = Convert.ToDateTime(Session["BatchDate"].ToString()).ToString("yyyy-MM-ddTHH:mm:ss.000Z");

                    if (!mySession.MySite.SiteCode.IsNullOrEmpty() && !averyModDt.IsNullOrEmpty())
                    {
                        string averyDateTimeJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + averyModDt + "\"]}}},";

                        string averySiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + mySession.MySite.SiteCode + "\"]}}}";

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        averyDateTimeJsonParameter +
                                        averySiteCodeJsonParameter +
                                    "]}}";
                    }

                    ReportControlPanel.Visible = false;
                    //RegisterAsyncTask(new PageAsyncTask(getPDFResultFromBCS));
                    getPDFResultFromBCS();
                    Session.Remove("StatusId");
                    Session.Remove("BatchDate");
                    break;

                case "/Blank_DD771":
                    _documentID = "15704160";
                    // _jsonBody ="{\"parameters\": {\"parameter\": [{\"id\": 0,\"answer\": {}},{\"id\": 1,\"answer\": {}}]}}";
                    ReportControlPanel.Visible = false;
                    //RegisterAsyncTask(new PageAsyncTask(getPDFResultFromBCSNoRefresh));
                    getPDFResultFromBCSNoRefresh();
                    break;

            }
        }

        private void ShowReportParameters()
        {
            switch (rptID)
            {
                case "/LabRoutingForm":
                    // This parameter is added in ManageOrdersLabPresenter, or SrtsReportsManager
                    //var o = Session["OrderNbrs"];
                    //var orderNbr = o == null ? null : o.ToString();
                    LabRoutingSiteTextBox.Text = ((mySession.MySite != null) ? mySession.MySite.SiteCode : mySession.MyClinicCode); ;
                    //LabRoutingOrderNumbersTextBox.Text = orderNbr;
                    LabRoutingFormPanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/Lab Reports/Rpt54":
                    Report54Panel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/WWReport":
                    WWPanel.Visible = true;
                    WWClinicSiteTextBox.Text = (!(mySession.MyClinicCode.IsNullOrEmpty()) ? mySession.MyClinicCode : "");
                    WWLabSiteTextBox.Text = ((mySession.MySite != null) ? mySession.MySite.SiteCode : mySession.MyClinicCode);
                    ReportControlPanel.Visible = true;
                    break;

                case "/Clinic Reports/ClinicDispenseRpt":
                    ClinicDispensePanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/Clinic Reports/ClinicOrdersRpt":
                    ClinicOrdersPanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/Clinic Reports/ClinicOverdueOrders":
                    ClinicOverduePanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/Clinic Reports/ClinicProductionRpt":
                    ClinicProductionPanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/Clinic Reports/ClinicSummaryRpt":
                    ClinicSummaryPanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/Clinic Reports/OrderDetailReport":
                    OrderDetailPanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/Clinic Reports/TurnAroundTimeRpt":
                    TurnAroundPanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/Administration Rpts/AccountInfo":
                    AccountInfoPanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/Administration Rpts/TheaterLocationCodes":
                    _documentID = "15441528";
                    ReportControlPanel.Visible = false;
                    getPDFResultFromBCSNoRefresh();
                    //RegisterAsyncTask(new PageAsyncTask(getPDFResultFromBCSNoRefresh));
                    break;

                case "/Administration Rpts/GEyesCount":
                    _documentID = "15441521";
                    GEyesCountPanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;

                case "/rLabel":
                    ReprintFormPanel.Visible = true;
                    ReportControlPanel.Visible = true;
                    break;
            }

            ShowReportsWithNoParameters();
        }

        protected void viewReport_Click(object sender, EventArgs e)
        {
            //LoadBCSIFrame();

            switch (rptID)
            {
                case "/LabRoutingForm":
                    // Set the document ID for json calls
                    _documentID = "15320091";

                    string orderNumbersJsonParameter = string.Empty;
                    if (!LabRoutingOrderNumbersTextBox.Text.IsNullOrEmpty())
                    {
                        // Wrap each order numbers with double quote
                        string formattedOrderNumbers = string.Empty;
                        string[] orderNumbers = LabRoutingOrderNumbersTextBox.Text.Split(',');

                        foreach (string orderNumber in orderNumbers)
                        {
                            formattedOrderNumbers += string.Format("\"{0}\",", orderNumber);
                        }

                        // Remove the trailing comma
                        formattedOrderNumbers = formattedOrderNumbers.TrimEnd(',');

                        orderNumbersJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [" + formattedOrderNumbers + "]}}},";
                    }
                    else
                    {
                        orderNumbersJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {}},";
                    }

                    string labRoutingSiteCodeJsonParameter = string.Empty;
                    if (!LabRoutingSiteTextBox.Text.IsNullOrEmpty())
                    {
                        labRoutingSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + LabRoutingSiteTextBox.Text + "\"]}}}";
                    }
                    else
                    {
                        labRoutingSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {}}";
                    }

                    _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                    orderNumbersJsonParameter +
                                    labRoutingSiteCodeJsonParameter +
                                "]}}";

                    break;

                case "/Labels/Single Label":
                case "/Labels/Avery 5160":

                    break;

                case "/Lab Reports/Rpt54":
                    _documentID = "15786830";
                    string Rpt54SiteCode = "";
                    if (mySession.MySite == null)
                        Rpt54SiteCode = mySession.MyClinicCode;
                    else
                        Rpt54SiteCode = mySession.MySite.SiteCode;

                    if (!Report54FromDateText.Text.IsNullOrEmpty() && !Report54ToDateText.Text.IsNullOrEmpty() && !Rpt54SiteCode.IsNullOrEmpty())
                    {
                        string report54FromDate = Convert.ToDateTime(Report54FromDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string report54ToDate = Convert.ToDateTime(Report54ToDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";

                        string report54FromDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + report54FromDate + "\"]}}},";
                        string report54ToDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + report54ToDate + "\"]}}},";

                        string report54LabSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + Rpt54SiteCode + "\"]}}}";

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        report54FromDateJsonParameter +
                                        report54ToDateJsonParameter +
                                        report54LabSiteCodeJsonParameter +
                                    "]}}";
                    }

                    break;

                case "/WWReport":
                    _documentID = "15441518";

                    if (!WWFromDateText.Text.IsNullOrEmpty() && !WWToDateText.Text.IsNullOrEmpty())
                    {
                        string wwFromDate = Convert.ToDateTime(WWFromDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string wwToDate = Convert.ToDateTime(WWToDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";

                        string wwFromDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + wwFromDate + "\"]}}},";
                        string wwToDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + wwToDate + "\"]}}},";

                        string wwClinicSiteCodeJsonParameter = string.Empty;
                        if (!WWClinicSiteNULLCheckBox.Checked)
                        {
                            wwClinicSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + WWClinicSiteTextBox.Text + "\"]}}},";
                        }
                        else
                        {
                            wwClinicSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {}},";
                        }

                        string wwLabSiteCodeJsonParameter = string.Empty;
                        if (!WWLabSiteNULLCheckBox.Checked && !WWLabSiteTextBox.Text.IsNullOrEmpty())
                        {
                            wwLabSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {\"values\": {\"@type\":\"Text\",\"value\": [\"" + WWLabSiteTextBox.Text + "\"]}}}";
                        }
                        else
                        {
                            wwLabSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {}}";
                        }

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        wwFromDateJsonParameter +
                                        wwToDateJsonParameter +
                                        wwClinicSiteCodeJsonParameter +
                                        wwLabSiteCodeJsonParameter +
                                    "]}}";
                    }

                    break;

                case "/Clinic Reports/ClinicDispenseRpt":
                    // Set the document ID for json calls
                    _documentID = "15441334";

                    if (!ClinicDispenseFromDateText.Text.IsNullOrEmpty() && !ClinicDispenseToDateText.Text.IsNullOrEmpty())
                    {
                        string clinicDispenseFromDate = Convert.ToDateTime(ClinicDispenseFromDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string clinicDispenseToDate = Convert.ToDateTime(ClinicDispenseToDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";

                        string clinicDispenseFromDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicDispenseFromDate + "\"]}}},";
                        string clinicDispenseToDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicDispenseToDate + "\"]}}},";

                        // Get clinic site code and create jason formatted parameter
                        string clinicDispenseSiteCodeJsonParameter = string.Empty;
                        if (!mySession.MyClinicCode.IsNullOrEmpty())
                        {
                            clinicDispenseSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + mySession.MyClinicCode + "\"]}}},";
                        }
                        else
                        {
                            clinicDispenseSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {}},";
                        }

                        string clinicDispenseStatusJsonParameter = string.Empty;
                        if (!ClinicDispenseStatusNULLCheckBox.Checked)
                        {
                            clinicDispenseStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicDispenseStatusTextBox.Text + "\"]}}},";
                        }
                        else
                        {
                            clinicDispenseStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {}},";
                        }

                        string clinicDispensePriorityJsonParameter = string.Empty;
                        if (!ClinicDispensePriorityNULLCheckBox.Checked)
                        {
                            clinicDispensePriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicDispensePriorityTextBox.Text + "\"]}}}";
                        }
                        else
                        {
                            clinicDispensePriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {}}";
                        }

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        clinicDispenseFromDateJsonParameter +
                                        clinicDispenseToDateJsonParameter +
                                        clinicDispenseSiteCodeJsonParameter +
                                        clinicDispenseStatusJsonParameter +
                                        clinicDispensePriorityJsonParameter +
                                    "]}}";
                    }

                    break;

                case "/Clinic Reports/ClinicOrdersRpt":
                    _documentID = "15707603";

                    if (!ClinicOrdersFromDateText.Text.IsNullOrEmpty() && !ClinicOrdersToDateText.Text.IsNullOrEmpty())
                    {
                        string clinicOrdersTypeJsonParameter = string.Empty;
                        if (!ClinicOrdersTypeList.SelectedValue.Equals("All"))
                        {
                            clinicOrdersTypeJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicOrdersTypeList.SelectedValue + "\"]}}},";
                        }
                        else
                        {
                            clinicOrdersTypeJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {}},";
                        }

                        string clinicOrdersFromDate = Convert.ToDateTime(ClinicOrdersFromDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string clinicOrdersToDate = Convert.ToDateTime(ClinicOrdersToDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";

                        string clinicOrdersFromDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicOrdersFromDate + "\"]}}},";
                        string clinicOrdersToDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicOrdersToDate + "\"]}}},";

                        string clinicOrdersPriorityJsonParameter = string.Empty;
                        if (!ClinicOrdersPriorityNULLCheckBox.Checked)
                        {
                            clinicOrdersPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicOrdersPriorityTextBox.Text + "\"]}}},";
                        }
                        else
                        {
                            clinicOrdersPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {}},";
                        }

                        string clinicOrdersStatusJsonParameter = string.Empty;
                        if (!ClinicOrdersStatusNULLCheckBox.Checked)
                        {
                            clinicOrdersStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicOrdersStatusTextBox.Text + "\"]}}},";
                        }
                        else
                        {
                            clinicOrdersStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {}},";
                        }


                        // Get clinic site code and create jason formatted parameter
                        //string clinicOrdersSiteCodeJsonParameter = string.Empty;
                        //if (!mySession.MyClinicCode.IsNullOrEmpty())
                        //{
                        //    clinicOrdersSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 5,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + mySession.MyClinicCode + "\"]}}}";
                        //}
                        //else
                        //{
                        //    clinicOrdersSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 5,\"answer\": {}}";
                        //}



                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        clinicOrdersTypeJsonParameter +
                                        clinicOrdersFromDateJsonParameter +
                                        clinicOrdersToDateJsonParameter +
                                        clinicOrdersPriorityJsonParameter +
                                        clinicOrdersStatusJsonParameter +
                                    //clinicOrdersSiteCodeJsonParameter +
                                    "]}}";
                    }

                    break;

                case "/Clinic Reports/ClinicOverdueOrders":
                    _documentID = "15441386";

                    if (!ClinicOverdueFromDateText.Text.IsNullOrEmpty() && !ClinicOverdueToDateText.Text.IsNullOrEmpty() && !mySession.MyClinicCode.IsNullOrEmpty())
                    {

                        // Get clinic site code and create jason formatted parameter
                        string clinicOverdueSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"values\": {\"value\": [\"" + mySession.MyClinicCode + "\"]}}},";

                        string clinicOverdueFromDate = Convert.ToDateTime(ClinicOverdueFromDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string clinicOverdueToDate = Convert.ToDateTime(ClinicOverdueToDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";

                        string clinicOverdueFromDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicOverdueFromDate + "\"]}}},";
                        string clinicOverdueToDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicOverdueToDate + "\"]}}},";

                        string clinicOverdueStatusJsonParameter = string.Empty;
                        if (!ClinicOverdueStatusNULLCheckBox.Checked)
                        {
                            clinicOverdueStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicOverdueStatusTextBox.Text + "\"]}}},";
                        }
                        else
                        {
                            clinicOverdueStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {}},";
                        }

                        string clinicOverduePriorityJsonParameter = string.Empty;
                        if (!ClinicOverduePriorityNULLCheckBox.Checked)
                        {
                            clinicOverduePriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicOverduePriorityTextBox.Text + "\"]}}}";
                        }
                        else
                        {
                            clinicOverduePriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {}}";
                        }

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        clinicOverdueSiteCodeJsonParameter +
                                        clinicOverdueFromDateJsonParameter +
                                        clinicOverdueToDateJsonParameter +
                                        clinicOverdueStatusJsonParameter +
                                        clinicOverduePriorityJsonParameter +
                                    "]}}";
                    }

                    break;

                case "/Clinic Reports/ClinicProductionRpt":
                    _documentID = "15441393";

                    if (!ClinicProductionFromDateText.Text.IsNullOrEmpty() && !ClinicProductionToDateText.Text.IsNullOrEmpty() && !mySession.MyClinicCode.IsNullOrEmpty())
                    {

                        // Get clinic site code and create jason formatted parameter
                        string clinicProductionSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + mySession.MyClinicCode + "\"]}}},";

                        string clinicProductionFromDate = Convert.ToDateTime(ClinicProductionFromDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string clinicProductionFromDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicProductionFromDate + "\"]}}},";

                        string clinicProductionPriorityJsonParameter = string.Empty;
                        if (!ClinicProductionPriorityNULLCheckBox.Checked)
                        {
                            clinicProductionPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicProductionPriorityTextBox.Text + "\"]}}},";
                        }
                        else
                        {
                            clinicProductionPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {}},";
                        }

                        string clinicProductionToDate = Convert.ToDateTime(ClinicProductionToDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string clinicProductionToDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicProductionToDate + "\"]}}},";

                        string clinicProductionStatusJsonParameter = string.Empty;
                        if (!ClinicProductionStatusNULLCheckBox.Checked)
                        {
                            clinicProductionStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": \"@type\":\"Text\",{\"values\": {\"value\": [\"" + ClinicProductionStatusTextBox.Text + "\"]}}}";
                        }
                        else
                        {
                            clinicProductionStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {}}";
                        }


                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        clinicProductionSiteCodeJsonParameter +
                                        clinicProductionFromDateJsonParameter +
                                        clinicProductionPriorityJsonParameter +
                                        clinicProductionToDateJsonParameter +
                                        clinicProductionStatusJsonParameter +
                                    "]}}";
                    }

                    break;

                case "/Clinic Reports/ClinicSummaryRpt":
                    _documentID = "15441308";

                    if (!ClinicSummaryFromDateText.Text.IsNullOrEmpty() && !ClinicSummaryToDateText.Text.IsNullOrEmpty() && !mySession.MyClinicCode.IsNullOrEmpty())
                    {
                        string clinicSummaryFromDate = Convert.ToDateTime(ClinicSummaryFromDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string clinicSummaryToDate = Convert.ToDateTime(ClinicSummaryToDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";

                        string clinicSummaryFromDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicSummaryFromDate + "\"]}}},";
                        string clinicSummaryToDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + clinicSummaryToDate + "\"]}}},";

                        // Get clinic site code and create jason formatted parameter
                        string clinicSummarySiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + mySession.MyClinicCode + "\"]}}},";

                        string clinicSummaryStatusJsonParameter = string.Empty;
                        if (!ClinicSummaryStatusNULLCheckBox.Checked)
                        {
                            clinicSummaryStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicSummaryStatusTextBox.Text + "\"]}}},";
                        }
                        else
                        {
                            clinicSummaryStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {}},";
                        }

                        string clinicSummaryPriorityJsonParameter = string.Empty;
                        if (!ClinicSummaryPriorityNULLCheckBox.Checked)
                        {
                            clinicSummaryPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + ClinicSummaryPriorityTextBox.Text + "\"]}}}";
                        }
                        else
                        {
                            clinicSummaryPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {}}";
                        }

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        clinicSummaryFromDateJsonParameter +
                                        clinicSummaryToDateJsonParameter +
                                        clinicSummarySiteCodeJsonParameter +
                                        clinicSummaryStatusJsonParameter +
                                        clinicSummaryPriorityJsonParameter +
                                    "]}}";
                        using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log3.txt")))
                        {
                            sw.WriteLine("Date:" + clinicSummaryFromDate + " " + clinicSummaryToDate + "" + clinicSummarySiteCodeJsonParameter);
                            sw.WriteLine("_jsonBody: " + _jsonBody);


                        }
                    }

                    break;

                case "/Clinic Reports/OrderDetailReport":
                    _documentID = "15698202";

                    if (!OrderDetailFromDateText.Text.IsNullOrEmpty() && !OrderDetailToDateText.Text.IsNullOrEmpty() && !mySession.MyClinicCode.IsNullOrEmpty())
                    {
                        string orderDetailFromDate = Convert.ToDateTime(OrderDetailFromDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string orderDetailToDate = Convert.ToDateTime(OrderDetailToDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";

                        string orderDetailFromDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + orderDetailFromDate + "\"]}}},";
                        string orderDetailToDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + orderDetailToDate + "\"]}}},";

                        // Get clinic site code and create jason formatted parameter
                        string orderDetailSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + mySession.MyClinicCode + "\"]}}},";

                        string orderDetailStatusJsonParameter = string.Empty;
                        if (!OrderDetailStatusNULLCheckBox.Checked)
                        {
                            orderDetailStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + OrderDetailStatusTextBox.Text + "\"]}}},";
                        }
                        else
                        {
                            orderDetailStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {}},";
                        }

                        string orderDetailPriorityJsonParameter = string.Empty;
                        if (!OrderDetailPriorityNULLCheckBox.Checked)
                        {
                            orderDetailPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + OrderDetailPriorityTextBox.Text + "\"]}}}";
                        }
                        else
                        {
                            orderDetailPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {}}";
                        }

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        orderDetailFromDateJsonParameter +
                                        orderDetailToDateJsonParameter +
                                        orderDetailSiteCodeJsonParameter +
                                        orderDetailStatusJsonParameter +
                                        orderDetailPriorityJsonParameter +
                                    "]}}";
                    }

                    break;

                case "/Clinic Reports/TurnAroundTimeRpt":
                    _documentID = "15441319";

                    if (!TurnAroundFromDateText.Text.IsNullOrEmpty() && !TurnAroundToDateText.Text.IsNullOrEmpty() && !mySession.MyClinicCode.IsNullOrEmpty())
                    {
                        string turnAroundFromDate = Convert.ToDateTime(TurnAroundFromDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";
                        string turnAroundToDate = Convert.ToDateTime(TurnAroundToDateText.Text).ToString("yyyy-MM-dd") + "T00:00:00.000Z";

                        string turnAroundFromDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + turnAroundFromDate + "\"]}}},";
                        string turnAroundToDateJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"DateTime\",\"values\": {\"value\": [\"" + turnAroundToDate + "\"]}}},";

                        // Get clinic site code and create jason formatted parameter
                        string turnAroundSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 2,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + mySession.MyClinicCode + "\"]}}},";

                        string turnAroundStatusJsonParameter = string.Empty;
                        if (!TurnAroundStatusNULLCheckBox.Checked)
                        {
                            turnAroundStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + TurnAroundStatusTextBox.Text + "\"]}}},";
                        }
                        else
                        {
                            turnAroundStatusJsonParameter = "{\"@type\":\"prompt\",\"id\": 3,\"answer\": {}},";
                        }

                        string turnAroundPriorityJsonParameter = string.Empty;
                        if (!TurnAroundPriorityNULLCheckBox.Checked)
                        {
                            turnAroundPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + TurnAroundPriorityTextBox.Text + "\"]}}}";
                        }
                        else
                        {
                            turnAroundPriorityJsonParameter = "{\"@type\":\"prompt\",\"id\": 4,\"answer\": {}}";
                        }

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        turnAroundFromDateJsonParameter +
                                        turnAroundToDateJsonParameter +
                                        turnAroundSiteCodeJsonParameter +
                                        turnAroundStatusJsonParameter +
                                        turnAroundPriorityJsonParameter +
                                    "]}}";
                    }

                    break;

                case "/Administration Rpts/AccountInfo":
                    _documentID = "15441525";

                    if (!AccountInfoListBox.SelectedItem.Text.IsNullOrEmpty())
                    {
                        string AccountInfoJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + AccountInfoListBox.SelectedItem.Text + "\"]}}}";

                        _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                        AccountInfoJsonParameter +
                                    "]}}";
                    }

                    break;

                case "/Administration Rpts/GEyesCount":
                    _documentID = "15441521";

                    string YearJsonParameter = "";
                    string MonthJsonParameter = "";
                    if (!listboxYear.SelectedItem.Text.IsNullOrEmpty())
                    {
                        YearJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + listboxYear.SelectedItem.Text + "\"]}}},";

                    }
                    else
                    {
                        YearJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {}},";
                    }
                    if (!listboxMonth.SelectedItem.Text.IsNullOrEmpty())
                    {
                        MonthJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + listboxMonth.SelectedItem.Text + "\"]}}}";

                    }
                    else
                    {
                        MonthJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {}}";
                    }

                    _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                       YearJsonParameter +
                                       MonthJsonParameter +
                                   "]}}";
                    break;

                case "/rLabel":
                    // Set the document ID for json calls
                    _documentID = "15438214";

                    string txtOrderNumberJsonParameter = string.Empty;
                    if (!txtOrderNumber.Text.IsNullOrEmpty())
                    {

                        txtOrderNumberJsonParameter = "{\"@type\":\"prompt\",\"id\": 0,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [" + txtOrderNumber.Text + "]}}},";
                    }


                    string FormSiteCodeJsonParameter = string.Empty;
                    if (!chkLabSiteCode.Checked)
                    {
                        if (mySession.MySite.SiteCode != null)
                            txtLabSiteCode.Text = mySession.MySite.SiteCode;

                        FormSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {\"@type\":\"Text\",\"values\": {\"value\": [\"" + txtLabSiteCode.Text + "\"]}}}";
                    }
                    else
                    {
                        FormSiteCodeJsonParameter = "{\"@type\":\"prompt\",\"id\": 1,\"answer\": {}}";
                    }

                    _jsonBody = "{\"parameters\": {\"parameter\": [" +
                                    txtOrderNumberJsonParameter +
                                    FormSiteCodeJsonParameter +
                                "]}}";

                    break;
            }

            if (!_jsonBody.IsNullOrEmpty())
            {
                getPDFResultFromBCS();

               // RegisterAsyncTask(new PageAsyncTask(getPDFResultFromBCS));

            }
        }

        public DataSet Order771
        {
            get { return (DataSet)ViewState["Order771"]; }
            set { ViewState.Add("Order771", value); }
        }


        private HttpWebResponse getLogonTokenFromBCS()
        {
            try
            {
                string urlParameters = ConfigurationManager.AppSettings["BCSLogonToken"];
                HttpWebRequest exportRequest = (HttpWebRequest)WebRequest.Create(_URL + urlParameters);
                exportRequest.Method = "GET";
                exportRequest.Accept = "application/json";
                exportRequest.Headers.Add("x-sap-trusted-user", ConfigurationManager.AppSettings["BCSUser"]);
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)exportRequest.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    // Parse the response body.

                    _logonToken = myHttpWebResponse.GetResponseHeader("X-SAP-LogonToken");
                    mySession.BCSLogonToken = _logonToken;
                    using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log2.txt")))
                    {
                        sw.WriteLine("_logonToken: " + _logonToken);
                    }
                }
                return myHttpWebResponse;
            }
            catch (Exception ex)
            {
                ex.LogException("In rptViewerTemplate - getLogonTokenFromBCS" + ex.Message + " - " + ex.InnerException);
                throw ex;
            }
        }


        private void getPDFResultFromBCSNoRefresh()
        {
            try
            {
                string urlParameters = string.Format(ConfigurationManager.AppSettings["BCSDocument"] + "{0}", _documentID);
                //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _URL + urlParameters);
                //request.Headers.Accept.Clear();
                //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));
                //request.Headers.Add("X-SAP-LOGONTOKEN", _logonToken);
                //HttpResponseMessage pdfResult = await _client.SendAsync(request, CancellationToken.None);


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL + urlParameters);
                request.Method = "GET";
                request.Accept = "application/pdf";
                request.Headers.Add("X-SAP-LogonToken", _logonToken);
                HttpWebResponse ResponseResult = (HttpWebResponse)request.GetResponse();

                if (ResponseResult.StatusCode == HttpStatusCode.OK)
                {
                    //returnPDFResponse(pdfResult);

                    string _pdfFileName = savePDFtoFile(ResponseResult);
                    if (!_pdfFileName.IsNullOrEmpty())
                        displayPDFinIFrame(_pdfFileName);
                }
                else if (ResponseResult.StatusCode == HttpStatusCode.Unauthorized)
                {

                    // Try a maximum tries to get the value before ending the loop to prevent infinite loop
                    if (_maxTries-- > 0)
                    {
                        // Possible logon token expired; reaquire logon token
                        HttpWebResponse authenticationResult = getLogonTokenFromBCS();
                        if (authenticationResult.StatusCode == HttpStatusCode.OK)
                        {

                            // Try Again by calling itself
                            getPDFResultFromBCSNoRefresh();
                        }
                    }
                }
                else if (ResponseResult.StatusCode == HttpStatusCode.BadRequest)
                {
                    // json body formatting incorrect
                }
                else if (ResponseResult.StatusCode == HttpStatusCode.NotFound)
                {
                    // incorrect URL in request call; possibly DocumentID has changed
                }

            }
            catch (Exception ex)
            {
                ex.LogException("In rptViewerTemplate - getPDFResultFromBCS" + ex.Message + " - " + ex.InnerException);
                throw ex;
            }
        }

        private void getPDFResultFromBCS()
        {
            try
            {
                string urlParameters = string.Format(ConfigurationManager.AppSettings["BCSDocument"] + "{0}", _documentID);

                HttpWebResponse refreshResult = refreshBCSForm();
                if (refreshResult.StatusCode == HttpStatusCode.OK)
                {

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL + urlParameters);
                    request.Method = "GET";
                    request.Accept = "application/pdf";
                    request.Headers.Add("X-SAP-LogonToken", _logonToken);
                    HttpWebResponse ResponseResult = (HttpWebResponse)request.GetResponse();

                    if (ResponseResult.StatusCode == HttpStatusCode.OK)
                    {
                        //returnPDFResponse(pdfResult);

                       string _pdfFileName= savePDFtoFile(ResponseResult);
                        if(!_pdfFileName.IsNullOrEmpty())
                            displayPDFinIFrame(_pdfFileName);
                    }
                }
                else if (refreshResult.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Try a maximum tries to get the value before ending the loop to prevent infinite loop
                    if (_maxTries-- > 0)
                    {
                        // Possible logon token expired; reaquire logon token
                        getLogonTokenFromBCS();
                        if (!_logonToken.IsNullOrEmpty())
                        {
                            // Try Again by calling itself
                            getPDFResultFromBCS();
                        }
                    }
                }
                else if (refreshResult.StatusCode == HttpStatusCode.BadRequest)
                {
                    // json body formatting incorrect
                }
                else if (refreshResult.StatusCode == HttpStatusCode.NotFound)
                {
                    // incorrect URL in request call; possibly DocumentID has changed
                }
            }
            catch (Exception ex)
            {
                ex.LogException("In rptViewerTemplate - getPDFResultFromBCS" + ex.Message + " - " + ex.InnerException);
                throw ex;
            }
        }

        private HttpWebResponse refreshBCSForm()
        {
            try
            {
                string urlParameters = string.Format(ConfigurationManager.AppSettings["BCSDocument"] + "{0}/parameters", _documentID);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL + urlParameters);
                request.Method = "GET";
                request.Accept = "application/json";
                request.Headers.Add("x-sap-trusted-user", ConfigurationManager.AppSettings["BCSUser"]);
                HttpWebResponse refreshResult = (HttpWebResponse)request.GetResponse();
                using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log9.txt")))
                {

                    sw.WriteLine("refreshResult: " + refreshResult.StatusCode);

                }
                return refreshResult;
            }
            catch (Exception ex)
            {
                ex.LogException("In rptViewerTemplate - refreshBCSForm" + ex.Message + " - " + ex.InnerException);
                throw ex;
            }

        }

        // Method to create a file in the server using the PDF stream
        private string savePDFtoFile(HttpWebResponse pdfResult)
        {
            try
            {
                string _pdfFileName=string.Empty;
                Stream pdfFileBuffer = pdfResult.GetResponseStream();//Content.ReadAsByteArrayAsync().Result;
                if (pdfFileBuffer != null)
                {
                    string now = DateTime.Now.ToString("yyyyMMddhmmss");
                    _pdfFileName = "BCS_Report_" + _myCurrentUser.UserMembership.UserName + "_" + rptTitle + "_" + now + ".pdf";
                    //_pdfFileName = "BCS_Report_" + rptTitle + "_" + now + ".pdf";
                    using (var fileStream = new FileStream(Server.MapPath("\\TMP\\" +_pdfFileName), FileMode.Create, FileAccess.Write))
                    {
                        pdfFileBuffer.CopyTo(fileStream);
                    }
                }
                return _pdfFileName;

            }
            catch (Exception ex)
            {
                ex.LogException("In rptViewerTemplate - savePDFtoFile" + ex.Message + " - " + ex.InnerException);
                throw ex;
            }
        }

        // Method to display pdf in iFrame
        private void displayPDFinIFrame(string _pdfFile)
        {
            try
            {
                using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log20.txt")))
                {

                    sw.WriteLine("In  : ");

                }
                if (!_pdfFile.IsNullOrEmpty())
                {
                    string src =Server.MapPath("\\TMP\\" + _pdfFile);
                    Thread.Sleep(6000);
                    
                    if (File.Exists(src))
                    {
                        using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log22.txt")))
                        {
                            sw.WriteLine("File exist  : ");

                            sw.WriteLine("SRC: " + src);

                            sw.WriteLine("SRC:2 " + "https:\\\\srts-demo.csd.disa.mil\\TMP\\" + _pdfFile);
                        }
                        this.BCSFrame.Visible = true;
                        this.BCSFrame.Src = ConfigurationManager.AppSettings["BCS_File_Loc"] + _pdfFile;
                    }
                    else
                    {
                        //this.BCSFrame.Attributes["src"] = ResolveUrl("Handler1.ashx");
                        using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log21.txt")))
                        {

                            sw.WriteLine("SRC: " + src);

                        }
                    }
                }
                else
                {
                    string src = String.Format("~/TMP/filenotfound.txt");

                    this.BCSFrame.Visible = true;
                    this.BCSFrame.Src = src;

                    using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log22.txt")))
                    {

                        sw.WriteLine("else: " + src);

                    }
                }
                using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log23.txt")))
                {

                    sw.WriteLine("out: ");

                }
            }
            catch (Exception ex)
            {
                ex.LogException("In rptViewerTemplate - displayPDFinIFrame" + ex.Message + " - " + ex.InnerException);
                using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log24.txt")))
                {

                    sw.WriteLine("ex: " + ex.Message);

                }
                throw ex;
            }
        }
        //private async Task<HttpResponseMessage> getLogonTokenFromBCS()
        //{
        //    try
        //    {
        //        string urlParameters = ConfigurationManager.AppSettings["BCSLogonToken"];
        //        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _URL + urlParameters);
        //        request.Headers.Accept.Clear();
        //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        request.Headers.Add("x-sap-trusted-user", ConfigurationManager.AppSettings["BCSUser"]);
        //        HttpResponseMessage jsonResult = await _client.SendAsync(request, CancellationToken.None);

        //        if (jsonResult.IsSuccessStatusCode)
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
        //            mySession.BCSLogonToken = _logonToken;
        //            using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log2.txt")))
        //            {

        //                sw.WriteLine("_logonToken: " + _logonToken);


        //            }
        //        }

        //        return jsonResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.LogException("In rptViewerTemplate - getLogonTokenFromBCS" + ex.Message + " - " + ex.InnerException);
        //        throw ex;
        //    }
        //}

        //private async Task getPDFResultFromBCS()
        //{
        //    try
        //    {
        //        string urlParameters = string.Format(ConfigurationManager.AppSettings["BCSDocument"] + "{0}" , _documentID);

        //        HttpResponseMessage refreshResult = await refreshBCSForm();
        //        if (refreshResult.IsSuccessStatusCode)
        //        {
        //            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _URL + urlParameters);
        //            request.Headers.Accept.Clear();
        //            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));
        //            request.Headers.Add("X-SAP-logontoken", _logonToken);

        //            HttpResponseMessage pdfResult = await _client.SendAsync(request, CancellationToken.None);
        //            using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log7.txt")))
        //            {

        //                sw.WriteLine("_logonToken: " + _logonToken);

        //            }
        //            if (pdfResult.IsSuccessStatusCode)
        //            {
        //                //returnPDFResponse(pdfResult);
        //                savePDFtoFile(pdfResult);
        //                displayPDFinIFrame();
        //            }
        //        }
        //        else if (refreshResult.StatusCode == HttpStatusCode.Unauthorized)
        //        {
        //            // Try a maximum tries to get the value before ending the loop to prevent infinite loop
        //            if (_maxTries-- > 0)
        //            {
        //                // Possible logon token expired; reaquire logon token
        //                HttpResponseMessage authenticationResult = await getLogonTokenFromBCS();
        //                if (authenticationResult.IsSuccessStatusCode)
        //                {
        //                    // Try Again by calling itself
        //                    await getPDFResultFromBCS();
        //                }
        //            }
        //        }
        //        else if (refreshResult.StatusCode == HttpStatusCode.BadRequest)
        //        {
        //            // json body formatting incorrect
        //        }
        //        else if (refreshResult.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            // incorrect URL in request call; possibly DocumentID has changed
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.LogException("In rptViewerTemplate - getPDFResultFromBCS" + ex.Message + " - " + ex.InnerException);
        //        throw ex;
        //    }
        //}

        //private async Task<HttpResponseMessage> refreshBCSForm()
        //{
        //    try
        //    {
        //        string urlParameters = string.Format(ConfigurationManager.AppSettings["BCSDocument"] + "{0}/parameters", _documentID);

        //        // Prepare the request
        //        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, _URL + urlParameters);
        //        request.Headers.Accept.Clear();
        //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        request.Headers.Add("X-SAP-LOGONTOKEN", _logonToken);
        //        request.Content = new StringContent(_jsonBody, Encoding.UTF8, "application/json");
        //        using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log8.txt")))
        //        {

        //            sw.WriteLine("Content: " + _jsonBody);

        //        }
        //        // Make WS call and get result
        //        HttpResponseMessage refreshResult = await _client.SendAsync(request, CancellationToken.None);
        //        using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log9.txt")))
        //        {

        //            sw.WriteLine("refreshResult: " + refreshResult.IsSuccessStatusCode);

        //        }
        //        return refreshResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.LogException("In rptViewerTemplate - refreshBCSForm" + ex.Message + " - " + ex.InnerException);
        //        throw ex;
        //    }

        //}



        // Method to return the PDF directly to the browser
        // This is not being used
        private void returnPDFResponse(HttpResponseMessage pdfResult)
        {
            try
            {
                Byte[] pdfFileBuffer = pdfResult.Content.ReadAsByteArrayAsync().Result;
                if (pdfFileBuffer != null)
                {
                    Response.ClearContent();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Length", pdfFileBuffer.Length.ToString());
                    Response.AddHeader("Content-Disposition", "inline; filename=" + "result.pdf");
                    Response.BinaryWrite(pdfFileBuffer);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ex.LogException("In rptViewerTemplate - returnPDFResponse" + ex.Message + " - " + ex.InnerException);
                throw ex;
            }
        }

        // Method to load BCS to an iFrame using OpenDocument
        // This is not being used
        private void LoadBCSIFrame()
        {
            string fromDate = Convert.ToDateTime(ClinicDispenseFromDateText.Text).ToString("yyyy,M,d");
            string toDate = Convert.ToDateTime(ClinicDispenseToDateText.Text).ToString("yyyy,M,d");
            string siteCode = mySession.MySite.SiteCode;
            string userID = _myCurrentUser.UserMembership.UserName;
            string src = String.Format(ConfigurationManager.AppSettings["BCSURL"] + "BOE/OpenDocument/opendoc/openDocument.jsp?sIDType=CUID&iDocID=FoyYmwA.BgEAFl4AAAAXb9cAAFBWgkYT&user={0}&sRefresh=Y&lsSEnter Order Date (Start)=DateTime({1})&lsSEnter Order Date (End)=DateTime({2})&lsSEnter Clinic Site Code={3}&sOutputFormat=P&sWindow=Same", userID, fromDate, toDate, siteCode);

            this.BCSFrame.Visible = true;
            this.BCSFrame.Src = src;
        }

        //ECRS Service Authentication before BCS API
        //public void ECRSServiceCall()
        //{
        //    try
        //    {
        //        XmlDocument SOAPReqBody = new XmlDocument();
        //        //SOAP Body Request    


        //        string xml = string.Format(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:uas=""http://uas.ias.com/"">" +
        //                            @"<soapenv:Header/>
        //                                 <soapenv:Body>
        //                                   <uas:getEcrsAcctStatusAccessLevelandSetLoginDate>
        //                                      <appId>bcs</appId>
        //                                      <userId>{0}</userId>
        //                                      <userIdType>E</userIdType>
        //                                     <logindate>{1}</logindate>
        //                                    </uas:getEcrsAcctStatusAccessLevelandSetLoginDate>
        //                                  </soapenv:Body>
        //                                </soapenv:Envelope>", "123456789", DateTime.Now.ToString("yyyyMMddHHmmss"));

        //        SOAPReqBody.LoadXml(xml);

        //        var req = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["hostname"]);
        //        req.ContentType = ConfigurationManager.AppSettings["content"];
        //        req.Method = ConfigurationManager.AppSettings["postmethod"];
        //        req.Accept = "text/xml";
        //        var certFile = Path.Combine(ConfigurationManager.AppSettings["Certificate"]);
        //        var certificate = new X509Certificate2(System.IO.File.ReadAllBytes(certFile), ConfigurationManager.AppSettings["Password"]);
        //        req.ClientCertificates.Add(certificate);

        //        using (Stream stream = req.GetRequestStream())
        //        {
        //            SOAPReqBody.Save(stream);
        //        }
        //        //Geting response from request    
        //        using (WebResponse Serviceres = req.GetResponse())
        //        {
        //            using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log.txt")))
        //            {
        //                sw.WriteLine("webservices success" + Serviceres.ContentLength);
        //            }
        //            using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
        //            {
        //                //reading stream    
        //                var ServiceResult = rd.ReadToEnd();
        //                XmlDocument xdoc = new XmlDocument();

        //                xdoc.LoadXml(ServiceResult);

        //                XmlNodeList nodeList_Status = xdoc.GetElementsByTagName("ecrsaccountstatusaccesslevelandsetlogindate");
        //                XmlNodeList nodeList_UserID = xdoc.GetElementsByTagName("userId");
        //                string status = nodeList_Status[0].InnerText.ToString();
        //                string UserId = nodeList_UserID[0].InnerText.ToString();
        //                if (status.Contains("active"))
        //                {

        //                }
        //                //var sessionId = rootElement.Element("ecrsaccountstatusaccesslevelandsetlogindate").Value;
        //                //writting stream result on console    
        //                using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log.txt")))
        //                {

        //                    sw.WriteLine("Response: " + ServiceResult);
        //                    sw.WriteLine(" User: " + UserId);
        //                    sw.WriteLine(" Status: " + status);
        //                }
        //                Console.WriteLine(ServiceResult);
        //                Console.ReadLine();
        //            }
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        string message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
        //        using (StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("log.txt")))
        //        {

        //            sw.WriteLine("error: " + message);
        //        }
        //    }
        //}
    }
}