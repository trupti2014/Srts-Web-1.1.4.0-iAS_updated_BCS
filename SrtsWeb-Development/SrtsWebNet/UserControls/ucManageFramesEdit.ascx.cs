
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Web.UI;
using SrtsWeb.WebForms.Admin;

namespace SrtsWeb.UserControls
{

    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [Serializable]
    public partial class ucManageFramesEdit : UserControlBase, IManageFramesEditView
    {
        private ManageFramesEditPresenter _presenter;

        #region PAGE LOAD

        /// <summary>
        /// Load Event for the Page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.hfpanel.Value = "0";

            if (!IsPostBack)
            {
                InitLoad();
            }
            if (IsPostBack && fileUpload.PostedFile != null)
            {
                if (fileUpload.PostedFile.FileName.Length > 0)
                {
                    string isValid = "false";
                    lblInfo.Text = "";
                    string[] extensions = new string[] { ".JPG", ".JPEG", ".PNG", ".TIF", ".TIFF", ".GIF" };

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
                            Session["isNewFileUploadValid"] = "InValid";
                            lblInfo.Text = "Sorry, this is not a valid file.";
                            this.hfpanel.Value = "1";
                        }
                        if (isValid == "true")
                        {
                            Session["NewContentType"] = Path.GetExtension(fileUpload.PostedFile.FileName).ToUpper();
                            Session["Newfileupload"] = fileUpload;
                            Session["isNewFileUploadValid"] = "Valid";
                            Session["NewfileuploadName"] = fileUpload.FileName;
                            Session["NewFile"] = fileUpload.FileBytes;
                            imgFrameImage.ImageUrl = "data:image;base64," + Convert.ToBase64String(fileUpload.FileBytes);
                            lblInfo.Attributes.Add("Style", "color:Green; font-weight: normal;");
                            lblInfo.Text = "The file is valid.  Click 'Submit' to save changes.";
                            lblFileUpload.Text = "-- " + Session["NewfileuploadName"].ToString() + " --";
                            txtMFGName.Text = Path.GetFileNameWithoutExtension(fileUpload.PostedFile.FileName).ToUpper();
                            lblFileUpload.Visible = true;
                            this.hfpanel.Value = "1";

                        }
                    }
                }
            }
        }
        #endregion

        #region EVENTS

        /// <summary>
        /// Validates and sends the image and information about that image to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnApplyImage_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "btnApplyImage_Click", mySession.MyUserID))
#endif
                {
                    if (!(this.ddlFrameFamilyEdit.SelectedIndex > 0))
                    {
                        this.hfeMsgFrames.Value = "Please Select a Valid Frame Family.";
                        return;
                    }
                    else if (!(this.ddlFrames.SelectedIndex > 0))
                    {
                        this.hfeMsgFrames.Value = "Please Select a Valid Frame.";
                        return;
                    }

                    if (Populated())
                    {
                        this.hfeSuccessFrames.Value = "0";
                        FrameImageEntity imageEntity = new FrameImageEntity();

                        var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;

                        imageEntity.ID = (int)Session["FrameImageID"];
                        imageEntity.ImgName = txtImageName.Text.Trim();
                        imageEntity.FrameImage = (Byte[])Session["NewFile"];
                        imageEntity.DateLoaded = DateTime.Now;
                        imageEntity.ModifiedBy = ModifiedBy;
                        imageEntity.ContentType = (string)Session["NewContentType"];
                        imageEntity.MFGName = txtMFGName.Text.Trim();
                        imageEntity.ISActive = Convert.ToBoolean(rblIsActive.SelectedValue);

                        //bool good = true;
                        this._presenter = new ManageFramesEditPresenter(this);
                        var good = _presenter.UpdateFrameImage(imageEntity);

                        var Message = String.Empty;
                        if (good)
                        {
                            Message = "Frame Image successfully Updated";
                            lblInfo.Attributes.Add("Style", "color:Green; font-weight: bold;");
                            lblInfo.Text = (string)Session["NewfileuploadName"] + " was Saved Successfully.";
                            this._presenter = new ManageFramesEditPresenter(this);
                            if (ddlFrames.SelectedIndex != 0 && ddlFrameFamilyEdit.SelectedIndex != 0)
                            {
                                string frame = ddlFrames.SelectedValue;
                                string family = ddlFrameFamilyEdit.SelectedValue;
                                if (_presenter.GetAllFrames(frame, family))
                                {
                                    gvFrameImage.DataSource = FrameImageRecords;
                                    gvFrameImage.DataBind();
                                    ResetImageItems();
                                    this.pnlEdit.Visible = false;
                                    upFrameImagesEdit.Update();
                                }

                            }


                            ResetImageItems();
                           
                        }
                        else
                        {
                            Message = "Error updating Frame Image";
                            lblInfo.Attributes.Add("Style", "color:Red; font-weight: normal;");
                            lblInfo.Text = "Error Updating Frame Image " + (string)Session["NewfileuploadName"] + ".";
                        }
                        LogEvent(String.Format("{1} by user {0} at {2}", mySession.MyUserID, Message, DateTime.Now));

                        if (!String.IsNullOrEmpty(Message))
                        {
                            if (good)
                            {
                                Session["NewFile"] = null;
                                this.hfeSuccessFrames.Value = "1";
                                this.hfeMsgFrames.Value = String.Format("Image for frame {0} was saved successfully.", this.Frame);
                            }
                            else
                            {
                                this.hfeMsgFrames.Value = String.Format("An error occurred attempting to save Image for frame {0}.", this.Frame);
                            }
                            Message = String.Empty;
                        }
                    }
                    else
                    {
                        if (Session["NewFile"] == null)
                        {
                            this.hfeMsgFrames.Value = "Please Add an Image before Submitting.";
                        }
                        else
                        {
                            this.hfeMsgFrames.Value = "Please Populate all Fields before Submitting.";
                        }

                    }
                    
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }

        }

        /// <summary>
        /// Selected index change from Frame drop down control.  
        /// Loads Data Grid View Data if valid selection or it ensures items get removed from screen if the index is invalid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            Unbind();
            ResetImageItems();
            this.pnlEdit.Visible = false;
            upFrameImagesEdit.Update();

            var Message = String.Empty;
            this._presenter = new ManageFramesEditPresenter(this);
            if (ddlFrames.SelectedIndex != 0 && ddlFrameFamilyEdit.SelectedIndex != 0)
            {
                string frame = ddlFrames.SelectedValue;
                string family = ddlFrameFamilyEdit.SelectedValue;
                if (_presenter.GetAllFrames(frame, family))
                {
                    gvFrameImage.DataSource = FrameImageRecords;
                    gvFrameImage.DataBind();


                }
                else
                {
                    Unbind();
                    lblFound.Text = String.Format("no records were found in the database for frame {0} .", this.Frame);
                    lblFound.Visible = true;

                }
            }
            else
            {
                lblFound.Text = "";
                lblFound.Visible = false;
            }

            if (!(ddlFrames.SelectedIndex == 0))
            {
                this.bGetFrameImages.Enabled = true;
            }
            else
            {
                this.bGetFrameImages.Enabled = false;

            }
        }

        /// <summary>
        /// Selected index change from Frame Family drop down control.  
        /// Loads Frame Data if valid selection or it ensures items get removed from screen if the index is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlFrameFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ucManageFrames_ddlFrameFamily_SelectedIndexChanged", this.mySession.MyUserID))
#endif
            {
                Unbind();
                ResetImageItems();
                this.pnlEdit.Visible = false;
                upFrameImagesEdit.Update();
                this.bGetFrameImages.Enabled = false;

                if (!(ddlFrameFamilyEdit.SelectedIndex == 0))
                {
                    LoadFrameData();
                }
                else
                {
                    this.ddlFrames.Items.Clear();


                }

            }
        }

        /// <summary>
        /// After any row command Retrieves command names and command arguments to process code logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvFrameImage_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string commandArgument = null;
            string[] arguments = null;
            var f = default(Int32);
            if (!Int32.TryParse(e.CommandArgument.ToString(), out f))
            {
                try
                {
                    commandArgument = e.CommandArgument.ToString();
                    arguments = commandArgument.Split(',');
                }
                catch (Exception ex)
                {
                    return;
                }
            }
            else
            {
                commandArgument = e.CommandArgument.ToString();
            }

            switch (e.CommandName.ToLower())
            {
                case "viewimage":
                    int imageID = Convert.ToInt32(arguments[0]);
                    if (FrameImageRecords.Count > 0)
                    {
                        var imageData = from frameImage in FrameImageRecords
                                        where frameImage.ID == imageID
                                        select (frameImage.FrameImage).ToArray();
                        if (imageData != null)
                        {
                            var image = imageData.GetEnumerator();
                            bool ok = image.MoveNext();
                            if (ok)
                            {
                                if (image.Current != null)
                                {
                                    byte[] bytes = image.Current;
                                    string base64 = Convert.ToBase64String(bytes);
                                    string imageURL = string.Format("data:image/jpg;base64," + base64 + ");");
                                    divFrameImage.Style["background-image"] = @"url(" + imageURL + ") no-repeat;";
                                    divFrameImage.Style["background-repeat"] = @"no-repeat";
                                    divFrameImage.Style["background-size"] = @"cover";

                                    FrameImageEntity imageInfo = (from frameImage in FrameImageRecords
                                                                  where frameImage.ID == imageID
                                                                  select frameImage).FirstOrDefault();
                                    litFooterInfo.Text = "Image Name:  " + imageInfo.ImgName;
                                }
                                uplImageModalView.Update();
                            }
                        }
                    }
                    break;

                case "editrecord":
                    if (FrameImageRecords.Count > 0)
                    {
                        // Get the selected row index
                        var gvr = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer);
                        var i = gvr.RowIndex;
                        int RecordID = Convert.ToInt32(commandArgument);
                        if (RecordID >= 1)
                        {
                            Session["FrameImageID"] = RecordID;
                            FrameImageEntity imageInfo = (from frameImage in FrameImageRecords where frameImage.ID == RecordID select frameImage).FirstOrDefault();
                            this.imgFrameImage.ImageUrl = "data:image;base64," + Convert.ToBase64String(imageInfo.FrameImage);
                            Session["NewFile"] = imageInfo.FrameImage;
                            FrameImageEntity imageContent = (from ContentType in FrameImageRecords where ContentType.ID == RecordID select ContentType).FirstOrDefault();
                            Session["NewContentType"] = imageContent.ContentType;
                            FrameImageEntity imageMfgName = (from MFGName in FrameImageRecords where MFGName.ID == RecordID select MFGName).FirstOrDefault();
                            this.txtMFGName.Text = imageMfgName.MFGName;
                            FrameImageEntity imageName = (from ImgName in FrameImageRecords where ImgName.ID == RecordID select ImgName).FirstOrDefault();
                            this.txtImageName.Text = imageName.ImgName;
                            FrameImageEntity Active = (from ISActive in FrameImageRecords where ISActive.ID == RecordID select ISActive).FirstOrDefault();
                            this.rblIsActive.SelectedValue = Convert.ToString(Active.ISActive);
                            this.lblInfo.Text = "";
                            lblFileUpload.Text = "";
                            // Clear other highlighed rows
                            ((GridView)sender).Rows.Cast<GridViewRow>().ToList().ForEach(x => x.BackColor = System.Drawing.Color.Empty);

                            // Highlight the selected row
                            gvr.BackColor = System.Drawing.ColorTranslator.FromHtml("#A1DCF2");

                            pnlEdit.Visible = true;
                            upFrameImagesEdit.Update();
                            
                        }
                    }
                    break;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Load Initial Dropdowns
        /// </summary>
        private void InitLoad()
        {

            // Load frame image lists
            this._presenter = new ManageFramesEditPresenter(this);
            _presenter.GetFrameFamily();

        }

        /// <summary>
        /// Ubind DataGridView
        /// </summary>
        private void Unbind()
        {
            gvFrameImage.DataSource = null;
            gvFrameImage.DataBind();
            lblFound.Visible = false;

        }

        /// <summary>
        /// Load Frame after frame family is selected
        /// </summary>
        private void LoadFrameData()
        {
            // Load frame lists
            this._presenter = new ManageFramesEditPresenter(this);
            _presenter.GetFramesByFrameFamily(Convert.ToInt32(ddlFrameFamilyEdit.SelectedValue));

        }

        /// <summary>
        /// Validate if Size of image is not too large
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Validate if type of image is acceptable
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="header"></param>
        /// <returns></returns>
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

        /// <summary>
        /// used to compare two byte arrays
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
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
        /// Check if fields are completed
        /// </summary>
        /// <returns></returns>
        protected Boolean Populated()
        {
            bool isPopulated = false;
            if (Session["NewFile"] == null)
            {
                return isPopulated; 
            }

            if ( (txtImageName.Text == String.Empty) || (ddlFrameFamilyEdit.SelectedIndex == 0) || (ddlFrames.SelectedIndex == 0) || (txtMFGName.Text == String.Empty))
            {
                return isPopulated;
            }
            isPopulated = true;
            return isPopulated;
        }

        /// <summary>
        /// Reset Image items After the file is saved
        /// </summary>
        private void ResetImageItems()
        {
            this.txtImageName.Text = "";
            this.txtMFGName.Text = "";
            this.lblInfo.Text = "";
            lblFileUpload.Text = "";
            this.imgFrameImage.ImageUrl = "~/Styles/images/DefaultGlasses.png";

        }

        /// <summary>
        /// bind the dropdown controls
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ddl"></param>
        /// <param name="SourceIn"></param>
        private void BindDdl<T>(DropDownList ddl, T SourceIn)
        {
            ddl.DataSource = SourceIn;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("-Select-", "X"));
            //ddl.Items.Insert(1, new ListItem("-No Default-", "N"));
        }

        /// <summary>
        /// Returns the querystring key value and returns it as an integer. This process will error out if the value is null, empty or non numeric
        /// </summary>
        /// <param name="QueryStringId"></param>
        /// <returns></returns>
        private long GetQueryStringNumericValue(string QueryStringId, bool Required = false)
        {
            long numericValue = 0;
            try
            {
                //Get the key value from the query string.
                string queryStringValue = Request.QueryString[QueryStringId];

                //Raise exception if the querystring was not passed or the value was missing
                if (string.IsNullOrEmpty(queryStringValue))
                {
                    if (!Required)
                    {
                        return (0);
                    }
                    //Raise an error if the query string is null or missing
                    throw new System.ArgumentException("The query string '" + QueryStringId + "' was not provided.");
                }

                //Check to see if the value is numeric
                if (!long.TryParse(queryStringValue, out numericValue))
                {
                    //Raise an error if the query string is not numeric
                    throw new System.ArgumentException("The value '" + queryStringValue + "' for the query string key '" + QueryStringId + "' is not a numeric value.");
                }
            }
            catch (Exception e)
            {
                //Uncomment the second line if you are using a parameter list in this method.
                Session["ParameterList"] = null;
                //Session["ParameterList"] = parameterList;

                //Display the error page and message.
                StackFrame stackFrame = new StackFrame();
                Session["MethodName"] = stackFrame.GetMethod().Name.ToString();
                Session["ErrorMessage"] = e.Message;

                //Try and get the stored procedure name from the system error message
                if (e.Message.StartsWith("Procedure or function"))
                {
                    string ProcedureName = e.Message.Substring(24, e.Message.IndexOf("'", 24) - 24);
                    Session["StoredProcedureName"] = ProcedureName;
                }
                else
                {
                    Session["StoredProcedureName"] = "N/A";
                }
                Server.ClearError();
                Context.ApplicationInstance.CompleteRequest();
                Response.Redirect("../../ErrorPage.aspx", true);

            }
            return (numericValue);
        }

        #endregion

        #region INTERFACE PROPERTIES

        /// <summary>
        /// List of Frame Images
        /// </summary>
        public List<FrameImageEntity> FrameImageRecords
        {
            get
            {
                return ViewState["FrameImageList"] as List<FrameImageEntity>;
            }
            set
            {
                ViewState["FrameImageList"] = value;
            }
        }

        /// <summary>
        /// string value for Frame Family
        /// </summary>
        public String FrameFamily
        {
            get
            {
                return this.ddlFrameFamilyEdit.SelectedValue;
            }
            set
            {
                this.ddlFrameFamilyEdit.SelectedValue = value;
            }
        }
        /// <summary>
        /// List values for Frame Families
        /// </summary>
        public List<FrameFamilyEntity> FrameFamilyList
        {
            get
            {
                return ViewState["FrameFamily"] as List<FrameFamilyEntity>;
            }
            set
            {
                ViewState["FrameFamily"] = value;
                this.ddlFrameFamilyEdit.DataTextField = "FamilyName";
                this.ddlFrameFamilyEdit.DataValueField = "ID";
                BindDdl(this.ddlFrameFamilyEdit, value);
            }
        }

        /// <summary>
        /// String Value of Frame
        /// </summary>
        public String Frame
        {
            get
            {
                return this.ddlFrames.SelectedValue;
            }
            set
            {
                this.ddlFrames.SelectedValue = value;
            }
        }
        /// <summary>
        /// Dictionary Values for Frames
        /// </summary>
        public Dictionary<String, String> FrameList
        {
            get
            {
                return ViewState["FrameList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["FrameList"] = value.ToList();
                this.ddlFrames.DataTextField = "Key";
                this.ddlFrames.DataValueField = "Value";
                this.ddlFrames.DataSource = value;
                this.ddlFrames.DataBind();
                this.ddlFrames.Items.Insert(0, new ListItem("-Select-", "X"));
            }
        }

        #endregion


    }
}