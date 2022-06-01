using Microsoft.Reporting.WebForms;
using SrtsWeb.Base;
using System;
using System.Data;
using System.IO;
using System.Web;

namespace SrtsWeb.PrintForms
{
    public partial class LabelRptViewer : PageBase
    {
        public String DocumentName { get; set; }

        public String DocumentPath { get; set; }

        public String LabelType { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session.Keys.Count.Equals(0)) return;

                var dt = new DataTable();
                var lType = String.Empty;
                var labelPath = String.Empty;

                try
                {
                    dt = (DataTable)Session["LabelTable"];
                    lType = Session["lType"].ToString();

                    Session.Remove("LabelTable");
                    Session.Remove("lType");

                    string FileName = "MailingLabels - " + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf";
                    string[] streamIds = null;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    string deviceInfo = string.Empty;
                    if (lType == "Avery5160.rpt")
                    {
                        deviceInfo = "<DeviceInfo>" +
                        "<OutputFormat>PDF</OutputFormat>" +
                        "<PageWidth>11.5</PageWidth>" +
                        "<PageHeight>8.5</PageHeight>" +
                        "<MarginTop>0.45in</MarginTop>" +
                        "<MarginLeft>0.125in</MarginLeft>" +
                        "<MarginRight>0.125in</MarginRight>" +
                        "<MarginBottom>0.5in</MarginBottom>" +
                        "</DeviceInfo>";
                    }
                    else
                    {
                        deviceInfo = "<DeviceInfo>" +
                        "<OutputFormat>PDF</OutputFormat>" +
                        "<PageWidth>3.3</PageWidth>" +
                        "<PageHeight>.9</PageHeight>" +
                        "<MarginTop>0.01in</MarginTop>" +
                        "<MarginLeft>0.01in</MarginLeft>" +
                        "<MarginRight>0.01in</MarginRight>" +
                        "<MarginBottom>0.01in</MarginBottom>" +
                        "</DeviceInfo>";
                    }
                    Warning[] warnings;

                    LocalReport report = new LocalReport();
                    ReportDataSource rds = new ReportDataSource("dsLabels", dt);
                    report.EnableExternalImages = true;

                    using (dt)
                    {
                        using (report)
                        {
                            report.DataSources.Clear();

                            switch (lType)
                            {
                                case "SingleLabel.rpt":
                                    labelPath = "~/PrintForms/SingleLabel.rdlc";
                                    break;

                                case "Avery5160.rpt":
                                    labelPath = "~/PrintForms/Avery5160_5260.rdlc";
                                    break;
                            }
                            report.ReportEmbeddedResource = HttpContext.Current.Server.MapPath(labelPath);
                            report.ReportPath = HttpContext.Current.Server.MapPath(labelPath);
                            report.DataSources.Clear();
                            report.DataSources.Add(rds);
                            report.Refresh();
                        }
                    }

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
                    CurrentModule("LabelRptViewer.aspx.cs");
                    CurrentModule_Sub(string.Empty);
                }
                catch (Exception ex)
                {
                    srtsMessageBox.Show(ex.ToString());
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return;
                }
            }
        }
    }
}