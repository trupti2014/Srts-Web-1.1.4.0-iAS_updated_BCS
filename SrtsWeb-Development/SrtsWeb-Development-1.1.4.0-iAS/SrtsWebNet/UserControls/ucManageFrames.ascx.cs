using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing.Imaging;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class ucManageFrames : UserControlBase, IManageFramesView
    {
        private ManageFramePresenter _presenter;

        #region PAGE LOAD

        /// <summary>
        /// Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

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
                        // get file extention to validate image header
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
                            Session["ContentType"] = Path.GetExtension(fileUpload.PostedFile.FileName).ToUpper();
                            Session["fileupload"] = fileUpload;
                            Session["isFileUploadValid"] = "Valid";
                            Session["fileuploadName"] = fileUpload.FileName;
                            Session["File"] = fileUpload.FileBytes;
                            imgFrameImage.ImageUrl = "data:image;base64," + Convert.ToBase64String(fileUpload.FileBytes);
                            lblInfo.Attributes.Add("Style", "color:Green; font-weight: normal;");
                            lblInfo.Text = "The file is valid.  Click 'Submit' to continue.";
                            lblFileUpload.Text = "-- " + Session["fileuploadName"].ToString() + " --";
                            txtMFGName.Text = Path.GetFileNameWithoutExtension(fileUpload.PostedFile.FileName).ToUpper();
                            lblFileUpload.Visible = true;

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
                    if (!Validate())
                    {
                        return;
                    }

                    if (Populated())
                    {
                        this.hfSuccessFrames.Value = "0";
                        FrameImageEntity imageEntity = new FrameImageEntity();

                        var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;

                        imageEntity.FrameFamily = ddlFrameFamily.SelectedValue;
                        imageEntity.FrameCode = ddlFrames.SelectedValue;
                        imageEntity.ImgName = txtImageName.Text.Trim();
                        imageEntity.ImgPath = "~/ImageFolder/";
                        imageEntity.FrameImage = (Byte[])Session["File"];
                        imageEntity.DateLoaded = DateTime.Now;
                        imageEntity.ModifiedBy = ModifiedBy;
                        imageEntity.ContentType = (string)Session["ContentType"];
                        imageEntity.Color = ddlColor.SelectedValue;
                        imageEntity.ISActive = true;
                        imageEntity.BridgeSize = Convert.ToInt32(ddlBridge.SelectedValue);
                        imageEntity.EyeSize = Convert.ToInt32(ddlEye.SelectedValue);
                        imageEntity.Temple = ddlTemple.SelectedValue;
                        imageEntity.MFGName = txtMFGName.Text.Trim();
                        imageEntity.ImgAngle = ddlImageAngle.SelectedValue;

                        this._presenter = new ManageFramePresenter(this);
                        var good = _presenter.InsertUpdateFrameImage(imageEntity);

                        var Message = String.Empty;
                        if (good)
                        {
                            Message = "Frame Image successfully Uploaded";
                            lblInfo.Attributes.Add("Style", "color:Green; font-weight: bold;");
                            lblInfo.Text = (string)Session["fileuploadName"] + " was Saved Successfully.";
                            ResetImageItems();
                            this.imgFrameImage.ImageUrl = "~/Styles/images/DefaultGlasses.png";
                        }
                        else
                        {
                            Message = "Error updating Frame Image";
                            lblInfo.Attributes.Add("Style", "color:Red; font-weight: normal;");
                            lblInfo.Text = "Error Updating Frame Image " + (string)Session["fileuploadName"] + ".";
                        }
                        LogEvent(String.Format("{1} by user {0} at {2}", mySession.MyUserID, Message, DateTime.Now));

                        if (!String.IsNullOrEmpty(Message))
                        {
                            if (good)
                            {
                                Session["File"] = null;
                                this.hfSuccessFrames.Value = "1";
                                this.hfMsgFrames.Value = String.Format("Image for frame {0} was saved successfully.", this.Frame);
                            }
                            else
                            {
                                this.hfMsgFrames.Value = String.Format("An error occurred attempting to save Image for frame {0}.", this.Frame);
                            }
                            Message = String.Empty;
                        }
                    }
                    else
                    {
                        if (Session["File"] == null)
                        {
                            this.hfMsgFrames.Value = "Please Add an Image before Submitting.";
                        }
                        else
                        {
                            this.hfMsgFrames.Value = "Please Populate all Fields before Submitting.";
                        }
                        
                    }
                    //this.hfSuccessFrames.Value = "1";
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }

        }

        /// <summary>
        /// Loads the ASsociated Frame Items depending on the Frame Selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ucManageFrames_ddlFrames_SelectedIndexChanged", this.mySession.MyUserID))
#endif
            {
                if (!(ddlFrames.SelectedIndex == 0))
                {
                    LoadFrameItemData();
                }
                else
                {
                    ResetFrameItems();
                }
            }
        }

        /// <summary>
        /// Loads the Frames Associated with the Frame Family Selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlFrameFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ucManageFrames_ddlFrameFamily_SelectedIndexChanged", this.mySession.MyUserID))
#endif
            {
                if (!(ddlFrameFamily.SelectedIndex == 0))
                {
                    hdfFrameFamily.Value = ddlFrameFamily.SelectedValue;
                    LoadFrameData();
                }
                else
                {
                    this.ddlFrames.Items.Clear();
                    lblFileUpload.Text = "";
                    lblInfo.Text = "";
                    ResetImageItems();
                    this.imgFrameImage.ImageUrl = "~/Styles/images/DefaultGlasses.png";

                }
                ResetFrameItems();
            }
        }

        #endregion

        #region FUNCTIONS

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
        /// This method takes a Document image of .tiff type and
        /// converts it into a .png.
        /// </summary>
        /// <remarks>
        /// Since browsers won't natively display tiff images, we need to
        /// convert the .tiff image into a .png so it will be displayed 
        ///in the browser.
        /// </remarks>
        /// <param name="pd"></param>
        /// <returns>Returns a byte array of the file image</returns>
        protected Byte[] GetImageBytes(FrameImageEntity entity)
        {
            var memoryStream = new System.IO.MemoryStream();
            MemoryStream stream = new MemoryStream();

            try
            {
                stream.Write(entity.FrameImage, 0, entity.FrameImage.Length);
                using (var image = System.Drawing.Image.FromStream(stream))
                    image.Save(memoryStream, ImageFormat.Png);

            }
            catch (System.ArgumentException ex)
            {
                ShowErrorDialog("There was an error trying to display the requested document. <br />Please notify system support of this error.<br />");
                string msg = ex.Message + " - " + ex.InnerException;

            }
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Check if fields are completed
        /// </summary>
        /// <returns></returns>
        protected Boolean Populated()
        {
            bool isPopulated = false;
            if (Session["File"] == null)
            {
                return isPopulated;
            }

            if ((ddlBridge.SelectedIndex == 0) || (ddlColor.SelectedIndex == 0) || (ddlEye.SelectedIndex == 0) || (txtImageName.Text == String.Empty) ||
                            (ddlFrameFamily.SelectedIndex == 0) || (ddlFrames.SelectedIndex == 0) || (ddlImageAngle.SelectedIndex == 0) || (txtMFGName.Text == String.Empty))
            {
                return isPopulated;
            }
            isPopulated = true;
            return isPopulated;
        }

        /// <summary>
        /// Return Error Message for Invalid Fields on Form
        /// </summary>
        protected Boolean Validate()
        {
            try
            {
                var NotValid = false;

                if (!(this.ddlFrameFamily.SelectedIndex > 0))
                {
                    this.hfMsgFrames.Value = "Please Select a Valid Frame Family.";
                    return NotValid;
                }
                else if (!(this.ddlFrames.SelectedIndex > 0))
                {
                    this.hfMsgFrames.Value = "Please Select a Valid Frame.";
                    return NotValid;
                }
                else if (!(this.ddlColor.SelectedIndex > 0))
                {
                    this.hfMsgFrames.Value = "Please Select a Valid Color.";
                    return NotValid;
                }
                if (!(this.ddlEye.SelectedIndex > 0))
                {
                    this.hfMsgFrames.Value = "Please Select a Valid Eye Size.";
                    return NotValid;
                }
                else if (!(this.ddlBridge.SelectedIndex > 0))
                {
                    this.hfMsgFrames.Value = "Please Select a Valid Bridge Size.";
                    return NotValid;
                }
                else if (!(this.ddlTemple.SelectedIndex > 0))
                {
                    this.hfMsgFrames.Value = "Please Select a Valid Temple Size.";
                    return NotValid;
                }
                else if (!(this.ddlImageAngle.SelectedIndex > 0))
                {
                    this.hfMsgFrames.Value = "Please Select a Valid Image Angle.";
                    return NotValid;
                }
                return true;
            }
            catch (Exception)
            {

                throw;
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
            this._presenter = new ManageFramePresenter(this);
            _presenter.GetFrameFamily();
            _presenter.GetImageAngle();
        }

        /// <summary>
        /// Load Frame item data after Frame is selected
        /// </summary>
        private void LoadFrameItemData()
        {
            // Load frame image lists
            this._presenter = new ManageFramePresenter(this);
            _presenter.GetFrameItemsList();
            // Get frame item global defaults
            _presenter.GetGlobalDefaultList();
            // Set the defaults
            ////p.SetDefaultsForFrame();
        }

        /// <summary>
        /// Load Frame after frame family is selected
        /// </summary>
        private void LoadFrameData()
        {
            // Load frame lists
            this._presenter = new ManageFramePresenter(this);
            _presenter.GetFramesByFrameFamily(Convert.ToInt32(ddlFrameFamily.SelectedValue));

        }

        /// <summary>
        /// Reset Frame items if FrameFamily is changed
        /// </summary>
        private void ResetFrameItems()
        {
            this.ddlBridge.Items.Clear();
            this.ddlColor.Items.Clear();
            this.ddlEye.Items.Clear();
            this.ddlTemple.Items.Clear();

        }

        /// <summary>
        /// Reset Image items After the file is saved
        /// </summary>
        private void ResetImageItems()
        {
            this.txtImageName.Text = "";
            this.txtMFGName.Text = "";
            this.ddlImageAngle.SelectedIndex = 0;
            //lblInfo.Text = "";

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

        #endregion

        #region INTERFACE PROPERTIES

        /// <summary>
        /// Byte Array for Frame Image
        /// </summary>
        public byte[] FrameImage { get; set; }

        /// <summary>
        /// String Value for ContentType
        /// </summary>
        public String ContentType { get; set; }

        /// <summary>
        /// string value for Frame Family
        /// </summary>
        public String FrameFamily
        {
            get
            {
                return this.ddlFrameFamily.SelectedValue;
            }
            set
            {
                this.ddlFrameFamily.SelectedValue = value;
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
                this.ddlFrameFamily.DataTextField = "FamilyName";
                this.ddlFrameFamily.DataValueField = "ID";
                BindDdl(this.ddlFrameFamily, value);
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

        /// <summary>
        /// String value for color
        /// </summary>
        public String Color
        {
            get
            {
                return this.ddlColor.SelectedValue;
            }
            set
            {
                this.ddlColor.SelectedValue = value;
            }
        }
        /// <summary>
        /// Dictionary Values for Colors
        /// </summary>
        public Dictionary<String, String> ColorList
        {
            get
            {
                return ViewState["ColorList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["ColorList"] = value;
                this.ddlColor.DataTextField = "Key";
                this.ddlColor.DataValueField = "Value";
                BindDdl(this.ddlColor, value);
            }
        }

        /// <summary>
        /// String value for EyeSize
        /// </summary>
        public String Eye
        {
            get
            {
                return this.ddlEye.SelectedValue;
            }
            set
            {
                this.ddlEye.SelectedValue = value;
            }
        }
        /// <summary>
        /// Dictionary Values for Eye Sizes
        /// </summary>
        public Dictionary<String, String> EyeList
        {
            get
            {
                return ViewState["EyeList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["EyeList"] = value;
                this.ddlEye.DataTextField = "Key";
                this.ddlEye.DataValueField = "Value";
                BindDdl(this.ddlEye, value);
            }
        }

        /// <summary>
        /// String value for Bridge Size
        /// </summary>
        public String Bridge
        {
            get
            {
                return this.ddlBridge.SelectedValue;
            }
            set
            {
                this.ddlBridge.SelectedValue = value;
            }
        }
        /// <summary>
        /// Dictionary Values for Bridge Sizes
        /// </summary>
        public Dictionary<String, String> BridgeList
        {
            get
            {
                return ViewState["BridgeList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["BridgeList"] = value;
                this.ddlBridge.DataTextField = "Key";
                this.ddlBridge.DataValueField = "Value";
                BindDdl(this.ddlBridge, value);
            }
        }

        /// <summary>
        /// String value for Temple
        /// </summary>
        public String Temple
        {
            get
            {
                return this.ddlTemple.SelectedValue;
            }
            set
            {
                this.ddlTemple.SelectedValue = value;
            }
        }
        /// <summary>
        /// Dictionary Values for Temples
        /// </summary>
        public Dictionary<String, String> TempleList
        {
            get
            {
                return ViewState["TempleList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["TempleList"] = value;
                this.ddlTemple.DataTextField = "Key";
                this.ddlTemple.DataValueField = "Value";
                BindDdl(this.ddlTemple, value);
            }
        }

        /// <summary>
        /// String Value for Image Angle
        /// </summary>
        public String ImageAngle
        {
            get
            {
                return this.ddlImageAngle.SelectedValue;
            }
            set
            {
                this.ddlImageAngle.SelectedValue = value;
            }
        }
        /// <summary>
        /// List Values for Image Angles
        /// </summary>
        public List<FrameImageImageAngleEntity> ImageAngleList
        {
            get
            {
                return ViewState["ImageAngle"] as List<FrameImageImageAngleEntity>;
            }
            set
            {
                ViewState["ImageAngle"] = value;
                this.ddlImageAngle.DataTextField = "ImageAngleText";
                this.ddlImageAngle.DataValueField = "ImageAngleValue";
                BindDdl(this.ddlImageAngle, value);
            }
        }

        /// <summary>
        /// Frame Image Entity
        /// </summary>
        public FrameImageEntity FrameImageItem
        {
            get
            {
                return this.FrameImageItemList.FirstOrDefault(x => x.FrameCode == this.Frame) ?? new FrameImageEntity();
            }
        }
        /// <summary>
        /// Enumerable for Frame Image Items Entity
        /// </summary>
        public IEnumerable<FrameImageEntity> FrameImageItemList
        {
            get
            {
                return ViewState["FrameImageItemList"] as List<FrameImageEntity>;
            }
            set
            {
                ViewState["FrameImageItemList"] = value;
            }
        }

        /// <summary>
        /// Frame Items Default
        /// </summary>
        public FrameItemDefaultEntity FrameItemDefault
        {
            get
            {
                return ViewState["FrameItemDefault"] as FrameItemDefaultEntity ?? new FrameItemDefaultEntity();
            }
            set
            {
                ViewState["FrameItemDefault"] = value;
            }
        }

        #endregion

    }
}