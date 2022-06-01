using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    public partial class ucCms : UserControlBase, ICMSContentMessageView
    {
        public event EventHandler RefreshParent;

        public ucCms()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (mySession == null)
                mySession = new Entities.SRTSSession();

            var p = CustomProfile.GetProfile();
            if (!p.IsCmsUser)
            {
                this.lblErr.Text = "Invalid IndividualID";
                this.lblErr.Visible = true;
                foreach (var c in this.Controls)
                {
                    if (c is TextBox)
                        ((TextBox)c).Enabled = false;
                    else if (c is DropDownList)
                        ((DropDownList)c).Enabled = false;
                    else if (c is Button)
                        ((Button)c).Enabled = false;
                }
                return;
            }

            if (!this.IsPostBack)
            {
                var _presenter = new CMSPresenter(this);
                _presenter.InitView();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var s = String.Empty;
            var v = String.Empty;

            if (HttpContext.Current.Session == null || HttpContext.Current.Session["action"] == null || HttpContext.Current.Session["action"].Equals("add"))
                s = HttpContext.Current.Session["action"].ToString();

            v = validateForm(!s.Equals("add"));

            if (String.IsNullOrEmpty(v))
            {
                if (s.Equals("add"))
                    addNewContent();
                else
                    updateExistingContent();

                RefreshParent(sender, e);

                this.lblErr.Text = String.Empty;
                this.lblErr.Visible = false;
            }
            else
            {
                this.lblErr.Text = v;
                this.lblErr.Visible = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            var _presenter = new CMSPresenter(this);
            _presenter.InitView();
            RefreshParent(sender, e);
        }

        private void addNewContent()
        {
            var _presenter = new CMSPresenter(this);
            _presenter.AddNewContent();
            ScriptManager.RegisterStartupScript(this, typeof(UserControl), "", "alert('Added New Content Successfully!');", true);
            LogEvent(String.Format("User {0} successfully added content to CMS at {1}.", mySession.MyUserID, DateTime.Now));
        }

        private void updateExistingContent()
        {
            var _presenter = new CMSPresenter(this);
            _presenter.UpdateExistingContent();
            ScriptManager.RegisterStartupScript(this, typeof(UserControl), "", "alert('Updated Message Successfully!');", true);
            LogEvent(String.Format("User {0} successfully updated content to CMS at {1}.", mySession.MyUserID, DateTime.Now));
        }

        private String validateForm(Boolean isUpdate)
        {
            var d = new DateTime();
            var good = false;
            var err = new System.Text.StringBuilder();

            good = DateTime.TryParse(this.txtDisplayStartDate.Text, out d);
            if (!good) return "Start date is required";
            good = DateTime.TryParse(this.txtDisplayEndDate.Text, out d);
            if (!good) return "End date is required";

            if (!isUpdate)
                if (DateTime.Parse(this.txtDisplayStartDate.Text) < DateTime.Today) err.AppendLine("Start date cannot be before today.<br />");
            if (DateTime.Parse(this.txtDisplayStartDate.Text) > d) err.AppendLine("Start date must be on or before end date.<br />");
            if (String.IsNullOrEmpty(this.txtContentTitle.Text)) err.AppendLine("A message title is required.<br />");
            if (String.IsNullOrEmpty(this.txtContentDescription.Text)) err.AppendLine("A message body is required.<br />");

            return err.ToString();
        }

        #region INTERFACE MEMBERS

        public Int32 ContentId { get { return Convert.ToInt32(this.hfCId.Value); } set { this.hfCId.Value = value.ToString(); } }

        public List<CMSEntity> ContentType
        {
            set
            {
                ddlSelContentType.Items.Clear();
                ddlSelContentType.DataSource = value;
                ddlSelContentType.DataTextField = "cmsContentTypeName";
                ddlSelContentType.DataValueField = "cmsContentTypeID";
                ddlSelContentType.ToolTip = "cmsContentTypeDescription";
                ddlSelContentType.DataBind();
                ddlSelContentType.Items.Insert(0, new ListItem("-Select-", "0"));
                ddlSelContentType.SelectedIndex = 0;
            }
        }

        public string SelectedContentTypeId
        {
            get { return this.ddlSelContentType.SelectedValue; }
            set
            {
                try
                {
                    this.ddlSelContentType.SelectedValue = value;
                }
                catch
                {
                    this.ddlSelContentType.SelectedIndex = -1;
                }
            }
        }

        public List<CmsSite> FacilityClinic
        {
            set
            {
                ddlSelFacilityName_Clinic.Items.Clear();
                ddlSelFacilityName_Clinic.DataSource = value;

                ddlSelFacilityName_Clinic.DataTextField = "SiteName";
                ddlSelFacilityName_Clinic.DataValueField = "SiteCode";
                ddlSelFacilityName_Clinic.DataBind();
                ddlSelFacilityName_Clinic.Items.Insert(0, new ListItem("-Select-", "0"));
                ddlSelFacilityName_Clinic.SelectedIndex = 0;
            }
        }

        public String SelectedFacilityClinicId
        {
            get { return ddlSelFacilityName_Clinic.SelectedValue; }
            set
            {
                try
                {
                    ddlSelFacilityName_Clinic.SelectedValue = value;
                }
                catch
                {
                    this.ddlSelFacilityName_Clinic.SelectedIndex = -1;
                }
            }
        }

        public List<CmsSite> FacilityLab
        {
            set
            {
                ddlSelFacilityName_Lab.Items.Clear();
                ddlSelFacilityName_Lab.DataSource = value;

                ddlSelFacilityName_Lab.DataTextField = "SiteName";
                ddlSelFacilityName_Lab.DataValueField = "SiteCode";
                ddlSelFacilityName_Lab.DataBind();
                ddlSelFacilityName_Lab.Items.Insert(0, new ListItem("-Select-", "0"));
                ddlSelFacilityName_Lab.SelectedIndex = 0;
            }
        }

        public String SelectedFacilityLabId
        {
            get { return ddlSelFacilityName_Lab.SelectedValue; }
            set
            {
                try
                {
                    ddlSelFacilityName_Lab.SelectedValue = value;
                }
                catch
                {
                    this.ddlSelFacilityName_Lab.SelectedIndex = -1;
                }
            }
        }

        public List<CMSEntity> ContentRecipientType
        {
            set
            {
                ddlSelRecipientType.Items.Clear();
                ddlSelRecipientType.DataSource = value;
                ddlSelRecipientType.DataTextField = "cmsRecipientTypeName";
                ddlSelRecipientType.DataValueField = "cmsContentRecipientTypeID";
                ddlSelRecipientType.DataBind();
                ddlSelRecipientType.Items.Insert(0, new ListItem("-Select-", "0"));
                ddlSelRecipientType.SelectedIndex = 0;

                ddlSelRecipientType.Items.Remove(mySession.MySite.SiteType.ToLower().Equals("clinic") ?
                    ddlSelRecipientType.Items.FindByText("Lab") :
                    ddlSelRecipientType.Items.FindByText("Clinic"));
            }
        }

        public string SelectedContentRecipientTypeId
        {
            get { return ddlSelRecipientType.SelectedValue; }
            set
            {
                try
                {
                    ddlSelRecipientType.SelectedValue = value;
                }
                catch { this.ddlSelRecipientType.SelectedIndex = -1; }
            }
        }

        public List<CMSEntity> RecipientGroupType
        {
            set
            {
                ddlSelRecipientGroupType.Items.Clear();
                ddlSelRecipientGroupType.DataSource = value;
                ddlSelRecipientGroupType.DataTextField = "cmsRecipientGroupname";
                ddlSelRecipientGroupType.DataValueField = "cmsContentRecipientGroupID";
                ddlSelRecipientGroupType.DataBind();
                ddlSelRecipientGroupType.Items.Insert(0, new ListItem("-Select-", "0"));
                ddlSelRecipientGroupType.SelectedIndex = 0;
            }
        }

        public String SelectedRecipientGroupTypeId
        {
            get { return ddlSelRecipientGroupType.SelectedValue; }
            set
            {
                try
                {
                    ddlSelRecipientGroupType.SelectedValue = value;
                }
                catch
                {
                    this.ddlSelRecipientGroupType.SelectedIndex = -1;
                }
            }
        }

        public string ContentTitle
        {
            get { return this.txtContentTitle.Text; }
            set { this.txtContentTitle.Text = value; }
        }

        public string ContentDescription
        {
            get { return this.txtContentDescription.Text; }
            set { this.txtContentDescription.Text = value; }
        }

        public DateTime ContentStartDate
        {
            get
            {
                return Convert.ToDateTime(this.txtDisplayStartDate.Text);
            }
            set
            {
                var d = value.Equals(DateTime.MinValue) ? DateTime.Today : value;
                this.txtDisplayStartDate.Text = d.ToShortDateString();
                this.calStart.StartDate = d;
                this.calStart.SelectedDate = d;
            }
        }

        public DateTime ContentExpirationDate
        {
            get
            {
                return Convert.ToDateTime(this.txtDisplayEndDate.Text);
            }
            set
            {
                var d = value.Equals(DateTime.MinValue) ? DateTime.Today : value;
                this.txtDisplayEndDate.Text = d.ToShortDateString();
                this.calEnd.StartDate = d;
                this.calEnd.SelectedDate = d;
            }
        }

        public String SelectedRecipientSiteId
        {
            get
            {
                return String.IsNullOrEmpty(SelectedFacilityClinicId) || SelectedFacilityClinicId.Equals("0") ?
                    String.IsNullOrEmpty(SelectedFacilityLabId) || SelectedFacilityLabId.Equals("0") ?
                        String.Empty :
                        SelectedFacilityLabId :
                    SelectedFacilityClinicId;
            }
        }

        public List<CMSEntity> ContentAuthors
        {
            set { }
        }

        public int SelectedContentAuthorId
        {
            get;
            set;
        }

        public int ContentRecipientIndividualId
        {
            get;
            set;
        }

        public string Message
        {
            get
            {
                return this.lblErr.Text;
            }
            set
            {
                this.lblErr.Text = value;
                this.lblErr.Visible = true;
            }
        }

        #endregion INTERFACE MEMBERS
    }
}