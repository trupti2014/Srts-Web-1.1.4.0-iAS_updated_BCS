using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.BusinessLayer;
using System.IO;

namespace SrtsWeb.WebForms.SrtsOrderManagement
{
    public partial class PrescriptionUpload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack && fileUpload.PostedFile != null)
            {
                if (fileUpload.PostedFile.FileName.Length > 0)
                {
                    string isValid = "false";
                    lblInfo.Text = "";
                    string[] extensions = new string[] { ".JPG", ".JPEG", ".PNG", ".TIF", ".TIFF", ".GIF", ".PDF" };

                    lblInfo.Text = "";
                    if (this.fileUpload.HasFile)
                    {
                        // get file extention to validate image headers
                        string ext;
                        ext = Path.GetExtension(fileUpload.PostedFile.FileName).ToUpper();
                        
                        if (extensions.Contains(ext))
                        {
                            isValid = ValidateImageHeader(ext.ToUpper(), fileUpload.FileBytes);
                            isValid = GetFileUploadSize();
                        }
                        else
                        {
                            Session["isFileUploadValid"] = "InValid";
                            lblInfo.Text = "Sorry, this is not a valid file.";
                        }                     
                        if (isValid == "true")
                        {
                            Session["fileupload"] = fileUpload;
                            Session["isFileUploadValid"] = "Valid";
                            Session["fileuploadNamne"] = fileUpload.FileName;
                            lblInfo.Text = "The file is valid.  Click 'Update/Save' to continue or 'Cancel' to remove the file.";
                            lblFileUpload.Text = "-- " + Session["fileuploadNamne"].ToString() + " --";
                            fileUpload.Visible = false;
                            divPrescriptionDoc.Visible = false;
                            lblFileUpload.Visible = true;
                            btnCancelUpload.Visible = true;
                        }                      
                    }
                }
            }
        }

        protected string GetFileUploadSize()
        {
            string isValidSize = "false";
            if (this.fileUpload.HasFile)
            {
                if (this.fileUpload.FileBytes.Length > 5242880 || this.fileUpload.FileBytes.Length < 1024) // 5242880 equals 5mb - 1024 equal 1 byte
                {
                    isValidSize = "false";
                }
                else
                {
                    isValidSize = "true";
                }
            }
            return isValidSize;
        }

        protected string ValidateImageHeader(string ext, byte[] header)
        {
            // DICTIONARY OF ALL IMAGE FILE HEADERS
            Dictionary<string, byte[]> imageExt = new Dictionary<string, byte[]>();
            imageExt.Add(".JPG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
            imageExt.Add(".JPEG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
            imageExt.Add(".PNG", new byte[] { 0x89, 0x50, 0x4E, 0x47 });
            imageExt.Add(".TIF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
            imageExt.Add(".TIFF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
            imageExt.Add(".GIF", new byte[] { 0x47, 0x49, 0x46, 0x38 });
            imageExt.Add(".PDF", new byte[] { 0x25, 0x50, 0x44, 0x46 });

            string isValidContent = "false";

            byte[] imageheader = header.Take(4).ToArray();// file header info
            try
            {
                byte[] tmp = imageExt[ext];  // file extention
                if (CompareArray(tmp, imageheader))
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

        protected void btnCancelUpload_Click(object sender, EventArgs e)
        {
            divPrescriptionDoc.Visible = true;
            fileUpload.Visible = true;
            lblFileUpload.Text = "";
            lblInfo.Text = "";
            lblFileUpload.Visible = false;
            Session["fileupload"] = "";
            Session["isFileUploadValid"] = "";
            Session["fileuploadNamne"] = "";
            btnCancelUpload.Visible = false;
        }

    }
}