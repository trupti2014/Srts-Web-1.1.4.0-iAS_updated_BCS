using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Drawing.Imaging;

namespace SrtsWeb.WebForms.SrtsOrderManagement
{
    public partial class PrescriptionDocumentView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["Argument"] != null)
                {
                    GetDocumentImageToDisplay();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void GetDocumentImageToDisplay()
        {
            
            
            string id = string.Empty;
            if (Session["Argument"] != null)
            {
                id = Session["Argument"].ToString();
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                PrescriptionDocument prescriptionDocument = new PrescriptionDocument();
                string[] ids = id.Split(',');
                int PrescriptionScanId = Convert.ToInt32(ids[0]);
                int PrescriptionId = Convert.ToInt32(ids[1]);
                prescriptionDocument = GetScannedRXInfoByID(PrescriptionScanId, PrescriptionId);
                Session["Argument"] = null;
                if (prescriptionDocument != null)
                {
                    string isValid = "true";
                    string[] extensions = new string[] { ".JPG", ".JPEG", ".PNG", ".TIF", ".TIFF", ".GIF", ".PDF" };

                    // get file extention to validate image headers
                    string ext;
                    ext = Path.GetExtension(prescriptionDocument.DocumentName).ToUpper();
                    if (extensions.Contains(ext))
                    {
                        isValid = ValidateImageHeader(ext.ToUpper(), prescriptionDocument);
                    }

                    byte[] binaryData = null;



                    if (prescriptionDocument.DocumentType == "image/tiff")
                    {
                        binaryData = ConvertTifftoPng(prescriptionDocument);
                        if (binaryData.Length > 0)
                        {
                            Response.ContentType = "image/png";
                        }
                        else
                        {
                            ShowErrorDialog("There was an error trying to display the requested document. <br />Please notify system support of this error.<br />");
                            Response.Clear();
                            Response.End();
                            return;
                        }
                    }
                    else
                    {
                        binaryData = prescriptionDocument.DocumentImage;
                        Response.ContentType = prescriptionDocument.DocumentType;
                    }
                    Response.Clear();
                    Response.BinaryWrite(binaryData);
                    Response.End();
                }
            }
        }




        /// <summary>
        /// This method takes a PrescriptionDocument image of .tiff type and
        /// converts it into a .png.
        /// </summary>
        /// <remarks>
        /// Since browsers won't natively display tiff images, we need to
        /// convert the .tiff image into a .png so it will be displayed 
        ///in the browser.
        /// </remarks>
        /// <param name="pd"></param>
        /// <returns>Returns a byte array of the file image</returns>
        protected Byte[] ConvertTifftoPng(PrescriptionDocument pd)
        {
           var memoryStream = new System.IO.MemoryStream();
           MemoryStream stream = new MemoryStream();
            try
            {
                stream.Write(pd.DocumentImage, 0, pd.DocumentImage.Length);
                using (var image = System.Drawing.Image.FromStream(stream))
                    image.Save(memoryStream, ImageFormat.Png);
               // return memoryStream.ToArray();
                //using (var image = System.Drawing.Image.FromStream(memoryStream))
                //    image.Save(memoryStream, ImageFormat.Png);
                //return memoryStream.ToArray();
            }
            catch (System.ArgumentException ex)
            {
                ShowErrorDialog("There was an error trying to display the requested document. <br />Please notify system support of this error.<br />");
                string msg = ex.Message + " - " + ex.InnerException;
                
            }
            return memoryStream.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="RxScanId"></param>
        /// <param name="RxId"></param>
        /// <returns></returns>
        protected PrescriptionDocument GetScannedRXInfoByID(int RxScanId, int RxId)
        {
            PrescriptionDocument prescriptionDocument = new PrescriptionDocument();
            string connStr = SrtsWeb.ExtendersHelpers.Globals.ConnStrNm;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[connStr].ToString());

            connection.Open();
            try
            {
                SqlCommand commandInfo = new SqlCommand("GetScannedRxInfoByID", connection);
                SqlCommand commandFile = new SqlCommand("GetScannedRxFileByID", connection);

                commandInfo.CommandType = System.Data.CommandType.StoredProcedure;
                commandInfo.Parameters.Add("@RxID", SqlDbType.Int).Value = RxId;
                commandInfo.Parameters.Add("@ID", SqlDbType.Int).Value = RxScanId;

                SqlDataReader readerInfo = null;
                SqlDataReader readerFile = null;

                readerInfo = commandInfo.ExecuteReader();
                while (readerInfo.Read())
                {
                    string name = readerInfo["DocName"].ToString();
                    prescriptionDocument.Id = Convert.ToInt32(readerInfo["ID"]);
                    prescriptionDocument.DocumentName = readerInfo["DocName"].ToString();
                    prescriptionDocument.DocumentType = readerInfo["DocType"].ToString();
                    prescriptionDocument.ScanDate = readerInfo["ScanDate"].ToDateTime();
                    prescriptionDocument.DelDate = readerInfo["DelDate"].ToDateTime();
                    prescriptionDocument.PrescriptionId = Convert.ToInt32(readerInfo["RxID"]);
                    prescriptionDocument.IndividualId = Convert.ToInt32(readerInfo["IndividualID"]);
                }
                readerInfo.Close();


                commandFile.CommandType = System.Data.CommandType.StoredProcedure;
                commandFile.Parameters.Add("@RxID", SqlDbType.Int).Value = RxId;
                commandFile.Parameters.Add("@ID", SqlDbType.Int).Value = RxScanId;

                readerFile = commandFile.ExecuteReader();
                while (readerFile.Read())
                {
                    prescriptionDocument.DocumentImage = (byte[])(readerFile["RxScan"]);
                }
                readerFile.Close();

            }
            catch (SqlException ex)
            {
                string msg = ex.Message.ToString();
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return prescriptionDocument;

        }


        protected string ValidateImageHeader(string ext, PrescriptionDocument prescriptionDocument)
        {
            // DICTIONARY OF ALL IMAGE FILE HEADER
            Dictionary<string, byte[]> imageHeader = new Dictionary<string, byte[]>();
            imageHeader.Add(".JPG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
            imageHeader.Add(".JPEG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
            imageHeader.Add(".PNG", new byte[] { 0x89, 0x50, 0x4E, 0x47 });
            imageHeader.Add(".TIF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
            imageHeader.Add(".TIFF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
            imageHeader.Add(".GIF", new byte[] { 0x47, 0x49, 0x46, 0x38 });
            imageHeader.Add(".PDF", new byte[] { 0x25, 0x50, 0x44, 0x46 });

            string isValidContent = "false";

            byte[] header;
                try
                {
                    byte[] tmp = imageHeader[ext];  // file extention
                    header = new byte[tmp.Length];  // file header info
                    if (CompareArray(tmp, header))
                    {
                        isValidContent = "true";
                    }
                    else
                    {
                        isValidContent = "false";
                    }
                }
                catch (KeyNotFoundException ex)
                {
                    return isValidContent = "false";
                }
            return isValidContent;
        }


        private bool CompareArray(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Displays SRTSweb error message dialog
        /// </summary>
        /// <param name="msg">Message to be displayed to the user in the dialog.</param>
        private void ShowErrorDialog(String msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Error!','" + msg + "', 'error');", true);
        }
    }
}