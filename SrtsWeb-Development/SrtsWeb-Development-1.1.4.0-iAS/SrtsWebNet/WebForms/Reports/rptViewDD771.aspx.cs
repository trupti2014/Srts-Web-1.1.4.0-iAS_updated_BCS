using Microsoft.Reporting.WebForms;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.Presenters.Reporting;
using SrtsWeb.Views.Lab;
using SrtsWeb.Views.Reporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Web;
using System.Web.UI.WebControls;

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
    public partial class rptViewDD771 : PageBase, IOrder771View, IManageOrdersLabView
    {
        private RptDDForm771Presenter _presenter;

        public rptViewDD771()
        {
            _presenter = new RptDDForm771Presenter(this);
        }

        public DataSet Order771
        {
            get { return (DataSet)ViewState["Order771"]; }
            set { ViewState.Add("Order771", value); }
        }

        public DataSet CheckInOrders
        {
            get { return (DataSet)ViewState["OrderTransferEntity"]; }
            set { ViewState.Add("OrderTransferEntity", value); }
        }

        public DataTable DispenseOrders
        {
            get { return (DataTable)ViewState["DispenseOrderData"]; }
            set { ViewState.Add("DispenseOrderData", value); }
        }

        public DataTable BOSType
        {
            get { return (DataTable)ViewState["GetLookupsByType(BOSType)"]; }
            set { ViewState.Add("GetLookupsByType(BOSType)", value); }
        }

        public DataTable StatusType
        {
            get { return (DataTable)ViewState["GetLookupsByType(PatientStatusType)"]; }
            set { ViewState.Add("GetLookupsByType(PatientStatusType)", value); }
        }

        public DataTable PriorityType
        {
            get { return (DataTable)ViewState["GetLookupsByType(OrderPriorityType)"]; }
            set { ViewState.Add("GetLookupsByType(OrderPriorityType)", value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var o = Session["OrderNbrs"].ToString();
                _presenter.InitView(o);
                Session["OrderNbrs"] = string.Empty;  // We have the order number, now clear session to prevent duplicate printing.
                try
                {
                    DataTable dt = Order771.Tables[0];

                    //string FileName = "DD771_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf";
                    string FileName = "LRF_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf";
                    string[] streamIds = null;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    string deviceInfo = "<DeviceInfo>" +
                        "<OutputFormat>PDF</OutputFormat>" +
                        "<PageWidth>11.5</PageWidth>" +
                        "<PageHeight>8.5</PageHeight>" +
                        "<MarginTop>0.25in</MarginTop>" +
                        "<MarginLeft>0.25in</MarginLeft>" +
                        "<MarginRight>0.25in</MarginRight>" +
                        "<MarginBottom>0.25in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;

                    LocalReport report = new LocalReport();
                    //report.ReportPath = Server.MapPath("~/Reports/DDForm771a.rdlc");
                    report.ReportPath = Server.MapPath("~/Reports/LabRoutingForm.rdlc");
                    ReportDataSource rds = new ReportDataSource("dtOrders", dt);
                    report.DataSources.Clear();
                    report.DataSources.Add(rds);
                    report.EnableExternalImages = true;
                    report.Refresh();

                    byte[] dataBytes = report.Render("PDF", deviceInfo,
                                    out extension, out encoding,
                                    out mimeType, out streamIds, out warnings);
                    MemoryStream msx = new MemoryStream(dataBytes, 0, dataBytes.Length, false, true);
                    byte[] bytes = msx.GetBuffer();
                    Response.Buffer = true;

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment; filename=" + FileName);
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (NullReferenceException)
                {
                    CurrentModule("rptViewDD771.aspx.cs");
                    CurrentModule_Sub(string.Empty);
                }
                catch (Exception ex)
                {
                    srtsMessageBox.Show(ex.ToString());
                }

                #region Crystal Report Code -- Commented Out

                #endregion Crystal Report Code -- Commented Out
            }
        }

        #region Order771 Assessors

        public string SexCode
        {
            get { return SexCode; }
            set { SexCode = value; }
        }

        public string RankCode
        {
            get { return RankCode; }
            set { RankCode = value; }
        }

        public string StatusCode
        {
            get { return StatusCode; }
            set { StatusCode = value; }
        }

        public string OrderPriority
        {
            get { return OrderPriority; }
            set { OrderPriority = value; }
        }

        public string BOS
        {
            get { return BOS; }
            set { BOS = value; }
        }

        public string TechInitials
        {
            get { return TechInitials; }
            set { TechInitials = value; }
        }

        public string OrderNumber
        {
            get { return OrderNumber; }
            set { OrderNumber = value; }
        }

        public string LensType
        {
            get { return LensType; }
            set { LensType = value; }
        }

        public string LensMaterial
        {
            get { return LensMaterial; }
            set { LensMaterial = value; }
        }

        public string Tint
        {
            get { return Tint; }
            set { Tint = value; }
        }

        public decimal ODSegHeight
        {
            get { return ODSegHeight; }
            set { ODSegHeight = value; }
        }

        public decimal OSSegHeight
        {
            get { return OSSegHeight; }
            set { OSSegHeight = value; }
        }

        public int NumberOfCases
        {
            get { return NumberOfCases; }
            set { NumberOfCases = value; }
        }

        public int Pairs
        {
            get { return Pairs; }
            set { Pairs = value; }
        }

        public string FrameCode
        {
            get { return FrameCode; }
            set { FrameCode = value; }
        }

        public string FrameColor
        {
            get { return FrameColor; }
            set { FrameColor = value; }
        }

        public string FrameEyeSize
        {
            get { return FrameEyeSize; }
            set { FrameEyeSize = value; }
        }

        public string FrameBridgeSize
        {
            get { return FrameBridgeSize; }
            set { FrameBridgeSize = value; }
        }

        public string FrameTempleType
        {
            get { return FrameTempleType; }
            set { FrameTempleType = value; }
        }

        public int PDOD
        {
            get { return PDOD; }
            set { PDOD = value; }
        }

        public int PDOS
        {
            get { return PDOS; }
            set { PDOS = value; }
        }

        public int PDTotal
        {
            get { return PDTotal; }
            set { PDTotal = value; }
        }

        public int PDODNear
        {
            get { return PDODNear; }
            set { PDODNear = value; }
        }

        public int PDOSNear
        {
            get { return PDOSNear; }
            set { PDOSNear = value; }
        }

        public int PDTotalNear
        {
            get { return PDTotalNear; }
            set { PDTotalNear = value; }
        }

        public string ClinicSiteCode
        {
            get { return ClinicSiteCode; }
            set { ClinicSiteCode = value; }
        }

        public string LabSiteCode
        {
            get { return LabSiteCode; }
            set { LabSiteCode = value; }
        }

        public bool ShipToPatient
        {
            get { return ShipToPatient; }
            set { ShipToPatient = value; }
        }

        public string ShipAddress1
        {
            get { return ShipAddress1; }
            set { ShipAddress1 = value; }
        }

        public string ShipAddress2
        {
            get { return ShipAddress2; }
            set { ShipAddress2 = value; }
        }

        public string ShipCity
        {
            get { return ShipCity; }
            set { ShipCity = value; }
        }

        public string ShipState
        {
            get { return ShipState; }
            set { ShipState = value; }
        }

        public string ShipZipCode
        {
            get { return ShipZipCode; }
            set { ShipZipCode = value; }
        }

        public string ShipAddressType
        {
            get { return ShipAddressType; }
            set { ShipAddressType = value; }
        }

        public string LocationCode
        {
            get { return LocationCode; }
            set { LocationCode = value; }
        }

        public string UserComment1
        {
            get { return UserComment1; }
            set { UserComment1 = value; }
        }

        public string UserComment2
        {
            get { return UserComment2; }
            set { UserComment2 = value; }
        }

        public bool IsGEyes
        {
            get { return IsGEyes; }
            set { IsGEyes = value; }
        }

        public bool IsMultivision
        {
            get { return IsMultivision; }
            set { IsMultivision = value; }
        }

        public string PatientEmail
        {
            get { return PatientEmail; }
            set { PatientEmail = value; }
        }

        public string OnHoldForConfirmation
        {
            get { return OnHoldForConfirmation; }
            set { OnHoldForConfirmation = value; }
        }

        public string ODSphere
        {
            get { return ODSphere; }
            set { ODSphere = value; }
        }

        public string OSSphere
        {
            get { return OSSphere; }
            set { OSSphere = value; }
        }

        public string ODCylinder
        {
            get { return ODCylinder; }
            set { ODCylinder = value; }
        }

        public string OSCylinder
        {
            get { return OSCylinder; }
            set { OSCylinder = value; }
        }

        public int ODAxis
        {
            get { return ODAxis; }
            set { ODAxis = value; }
        }

        public int OSAxis
        {
            get { return OSAxis; }
            set { OSAxis = value; }
        }

        public decimal ODHPrism
        {
            get { return ODHPrism; }
            set { ODHPrism = value; }
        }

        public decimal OSHPrism
        {
            get { return OSHPrism; }
            set { OSHPrism = value; }
        }

        public decimal ODVPrism
        {
            get { return ODVPrism; }
            set { ODVPrism = value; }
        }

        public decimal OSVPrism
        {
            get { return OSVPrism; }
            set { OSVPrism = value; }
        }

        public string ODHBase
        {
            get { return ODHBase; }
            set { ODHBase = value; }
        }

        public string OSHBase
        {
            get { return OSHBase; }
            set { OSHBase = value; }
        }

        public string ODVBase
        {
            get { return ODVBase; }
            set { ODVBase = value; }
        }

        public string OSVBase
        {
            get { return OSVBase; }
            set { OSVBase = value; }
        }

        public decimal ODAdd
        {
            get { return ODAdd; }
            set { ODAdd = value; }
        }

        public decimal OSAdd
        {
            get { return OSAdd; }
            set { OSAdd = value; }
        }

        public string ODCorrectedAcuity
        {
            get { return ODCorrectedAcuity; }
            set { ODCorrectedAcuity = value; }
        }

        public string ODUncorrectedAcuity
        {
            get { return ODUncorrectedAcuity; }
            set { ODUncorrectedAcuity = value; }
        }

        public string OSCorrectedAcuity
        {
            get { return OSCorrectedAcuity; }
            set { OSCorrectedAcuity = value; }
        }

        public string OSUncorrectedAcuity
        {
            get { return OSUncorrectedAcuity; }
            set { OSUncorrectedAcuity = value; }
        }

        public string ODOSCorrectedAcuity
        {
            get { return ODOSCorrectedAcuity; }
            set { ODOSCorrectedAcuity = value; }
        }

        public string ODOSUncorrectedAcuity
        {
            get { return ODOSUncorrectedAcuity; }
            set { ODOSUncorrectedAcuity = value; }
        }

        public string FirstName
        {
            get { return FirstName; }
            set { FirstName = value; }
        }

        public string MiddleName
        {
            get { return MiddleName; }
            set { MiddleName = value; }
        }

        public string LastName
        {
            get { return LastName; }
            set { LastName = value; }
        }

        public string doctor
        {
            get { return doctor; }
            set { doctor = value; }
        }

        public DateTime ExamDate
        {
            get { return ExamDate; }
            set { ExamDate = value; }
        }

        public int PatientIDNumber
        {
            get { return PatientIDNumber; }
            set { PatientIDNumber = value; }
        }

        public DateTime DateOrderCreated
        {
            get { return DateOrderCreated; }
            set { DateOrderCreated = value; }
        }

        public string ClinicName
        {
            get { return ClinicName; }
            set { ClinicName = value; }
        }

        public string ClinicAddress1
        {
            get { return ClinicAddress1; }
            set { ClinicAddress1 = value; }
        }

        public string ClinicAddress2
        {
            get { return ClinicAddress2; }
            set { ClinicAddress2 = value; }
        }

        public string ClinicCity
        {
            get { return ClinicCity; }
            set { ClinicCity = value; }
        }

        public string ClinicState
        {
            get { return ClinicState; }
            set { ClinicState = value; }
        }

        public string ClinicZipCode
        {
            get { return ClinicZipCode; }
            set { ClinicZipCode = value; }
        }

        public string LabName
        {
            get { return LabName; }
            set { LabName = value; }
        }

        public string LabAddress1
        {
            get { return LabAddress1; }
            set { LabAddress1 = value; }
        }

        public string LabAddress2
        {
            get { return LabAddress2; }
            set { LabAddress2 = value; }
        }

        public string LabCity
        {
            get { return LabCity; }
            set { LabCity = value; }
        }

        public string LabCountry
        {
            get { return LabCountry; }
            set { LabCountry = value; }
        }

        public string LabState
        {
            get { return LabState; }
            set { LabState = value; }
        }

        public string LabZipCode
        {
            get { return LabZipCode; }
            set { LabZipCode = value; }
        }

        public string PatientPhoneNumber
        {
            get { return PatientPhoneNumber; }
            set { PatientPhoneNumber = value; }
        }

        public decimal ODDistantDecenter
        {
            get { return ODDistantDecenter; }
            set { ODDistantDecenter = value; }
        }

        public decimal OSDistantDecenter
        {
            get { return OSDistantDecenter; }
            set { OSDistantDecenter = value; }
        }

        public decimal ODNearDecenter
        {
            get { return ODNearDecenter; }
            set { ODNearDecenter = value; }
        }

        public decimal OSNearDecenter
        {
            get { return OSNearDecenter; }
            set { OSNearDecenter = value; }
        }

        public ImageField ONBarCode
        {
            get { return ONBarCode; }
            set { ONBarCode = value; }
        }

        public ImageField ClinicBarCode
        {
            get { return ClinicBarCode; }
            set { ClinicBarCode = value; }
        }

        public ImageField LabBarCode
        {
            get { return LabBarCode; }
            set { LabBarCode = value; }
        }

        public string BOSDesc
        {
            get { return BOSDesc; }
            set { BOSDesc = value; }
        }

        public string StatDesc
        {
            get { return StatDesc; }
            set { StatDesc = value; }
        }

        public string OrdPriDesc
        {
            get { return OrdPriDesc; }
            set { OrdPriDesc = value; }
        }

        #endregion Order771 Assessors

        #region BarCode Creation

        public Bitmap CreateBarcode(string data)
        {
            Bitmap barCode = new Bitmap(1, 1);

            Font Wasp39M = new Font("Wasp 39 M", 60,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point);

            Graphics oGraphics = Graphics.FromImage(barCode);

            SizeF dataSize = oGraphics.MeasureString(data, Wasp39M);

            barCode = new Bitmap(barCode, dataSize.ToSize());

            oGraphics.Clear(System.Drawing.Color.White);

            oGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            oGraphics.DrawString(data, Wasp39M, new SolidBrush(System.Drawing.Color.Black), 0, 0);

            oGraphics.Flush();

            Wasp39M.Dispose();
            oGraphics.Dispose();

            return barCode;
        }

        #endregion BarCode Creation

        #region NotImplementedExceptions

        public string Message
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<ManageOrderEntity> CheckInOrderData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int? TotalOrdersToCheckIn
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<ManageOrderEntity> DispenseOrderData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string DispenseOrderNumber
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DateReceivedByLab
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DateProductionCompleted
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DateSentToClinic
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DateLabDispensed
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DateClinicReceived
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DateClinicDispensed
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DateRejected
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DateCancelled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DateResubmitted
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsActive
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool RequiresJustification
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CurrentStatus
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string SelectedLabel
        {
            get { return string.Empty; }
        }

        public string HoldForStockReason
        {
            get { throw new NotImplementedException(); }
        }

        public string RedirectLab
        {
            get
            {
                throw new NotImplementedException();   
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string RejectRedirectJustification
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion NotImplementedExceptions
    }
}