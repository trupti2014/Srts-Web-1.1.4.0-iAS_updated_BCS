using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.Views.Admin;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucSitePreferencesFrames : UserControlBase, IFrameItemsPreferencesView
    {
        private SitePreferencesPresenter.FrameItemsPreferencesPresenter p;

        protected void Page_Load(object sender, EventArgs e)
        {
            ((SrtsWeb.WebForms.Admin.SitePreferences)this.Page).RefreshUserControl += ucSitePreferencesOrders_RefreshUserControl;
            if (!IsPostBack)
            {
                // First load frames
                LoadFramesAndPreferences();
            }
        }

        private void ucSitePreferencesOrders_RefreshUserControl(object sender, EventArgs e)
        {
            LoadFramesAndPreferences();
        }

        protected void ddlFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ucSitePreferencesFrames_ddlFrames_SelectedIndexChanged", this.mySession.MyUserID))
#endif
            {
                LoadFrameItemData();
                SetControlStates();
            }
        }

        protected void bSubmit_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ucSitePreferencesFrames_bSubmit_Click", this.mySession.MyUserID))
#endif
            {
                if (this.Frame.Equals("X")) return;

                try
                {
                    this.p = new SitePreferencesPresenter.FrameItemsPreferencesPresenter(this);
                    p.SetPreferencesToDb();
                    this.hfMsgFrames.Value = String.Format("Preferences for frame {0} were saved successfully.", this.Frame);
                }
                catch (Exception ex)
                {
                    ex.TraceErrorException();
                    this.hfMsgFrames.Value = String.Format("An error occurred attempting to save preferences for frame {0}.", this.Frame);
                }

                p.GetSitePreferences();

                this.hfSuccessFrames.Value = "1";
            }
        }

        private void LoadFramesAndPreferences()
        {
            this.p = new SitePreferencesPresenter.FrameItemsPreferencesPresenter(this);
           // p.GetFrameList();
            p.GetAllFrameswithPreferences();
            p.GetSitePreferences();
        }

        private void LoadFrameItemData()
        {
            // Load frame item lists
            this.p = new SitePreferencesPresenter.FrameItemsPreferencesPresenter(this);
            p.GetFrameItemsList();
            // Get frame item global defaults
            p.GetGlobalPreferencesList();
            // Set the preferences/defaults
            p.SetDefaultsForFrame();
        }

        private void SetControlStates()
        {
            var g = this.Lens.StartsWith("SV") || this.Lens.Equals("N") || this.Lens.Equals("X");
            this.tbOdSegHt.Enabled = !g;
            this.tbOsSegHt.Enabled = !g;
        }

        private void BindCbl<T>(CheckBoxList cbl, T SourceIn)
        {
            cbl.DataSource = SourceIn;
            cbl.DataBind();
            cbl.Items.Insert(0, new ListItem("-Global-", "G"));
            cbl.Items.Insert(1, new ListItem("-No Default-", "N"));
        }

        private void BindDdl<T>(DropDownList ddl, T SourceIn)
        {
            ddl.DataSource = SourceIn;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("-Global-", "G"));
            ddl.Items.Insert(1, new ListItem("-No Default-", "N"));
        }

        #region INTERFACE PROPERTIES
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
        public IEnumerable<FrameEntity> FrameList
        {
            get
            {
                return ViewState["FrameList"] as List<FrameEntity>;
            }
            set
            {
                ViewState["FrameList"] = value.ToList();
                this.ddlFrames.DataTextField = "FrameCodeAndDescription";
                this.ddlFrames.DataValueField = "FrameCode";
                this.ddlFrames.DataSource = value;
                this.ddlFrames.DataBind();
                this.ddlFrames.Items.Insert(0, new ListItem("-Select-", "X"));
            }
        }

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

        public string Lens
        {
            get
            {
                return this.ddlLens.SelectedValue;
            }
            set
            {
                this.ddlLens.SelectedValue = value;
            }
        }
        public Dictionary<string, string> LensList
        {
            get
            {
                return ViewState["LensList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["LensList"] = value;
                this.ddlLens.DataTextField = "Key";
                this.ddlLens.DataValueField = "Value";
                BindDdl(this.ddlLens, value);
            }
        }

        public string Tint
        {
            get
            {
                return this.ddlTint.SelectedValue;
            }
            set
            {
                this.ddlTint.SelectedValue = value;
            }
        }
        public Dictionary<string, string> TintList
        {
            get
            {
                return ViewState["TintList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["TintList"] = value;
                this.ddlTint.DataTextField = "Key";
                this.ddlTint.DataValueField = "Value";
                BindDdl(this.ddlTint, value);
            }
        }

        public string Coating
        {
            get
            {
                //return this.ddlCoating.SelectedValue;
                List<string> selectedValues = this.ddlCoating.Items.Cast<ListItem>()
                                                        .Where(li => li.Selected)
                                                        .Select(li => li.Value)
                                                        .ToList();
                return string.Join(",", selectedValues);
            }
            set
            {
                //this.ddlCoating.SelectedValue = value;
                if (String.IsNullOrEmpty(value))
                    this.ddlCoating.ClearSelection();
                else
                {
                    string[] coatings = value.Split(new[] { "," }, StringSplitOptions.None);
                    foreach (string s in coatings)
                        this.ddlCoating.Items.FindByValue(s).Selected = true;
                }
            }
        }
        public Dictionary<string, string> CoatingList
        {
            get
            {
                return ViewState["CoatingList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["CoatingList"] = value;
                this.ddlCoating.DataTextField = "Key"; 
                this.ddlCoating.DataValueField = "Value";
                BindCbl(this.ddlCoating, value);
            }
        }

        public string Material
        {
            get
            {
                return this.ddlMaterial.SelectedValue;
            }
            set
            {
                this.ddlMaterial.SelectedValue = value;
            }
        }
        public Dictionary<string, string> MaterialList
        {
            get
            {
                return ViewState["MaterialList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["MaterialList"] = value;
                this.ddlMaterial.DataTextField = "Key";
                this.ddlMaterial.DataValueField = "Value";
                BindDdl(this.ddlMaterial, value);
            }
        }

        public string OdSegHt
        {
            get
            {
                return this.tbOdSegHt.Text;
            }
            set
            {
                this.tbOdSegHt.Text = value;
            }
        }

        public string OsSegHt
        {
            get
            {
                return this.tbOsSegHt.Text;
            }
            set
            {
                this.tbOsSegHt.Text = value;
            }
        }

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

        public SitePrefFrameItemEntity FrameItemPreference
        {
            get
            {
                return this.FrameItemPreferenceList.FirstOrDefault(x => x.Frame == this.Frame) ?? new SitePrefFrameItemEntity();
            }
        }
        public IEnumerable<SitePrefFrameItemEntity> FrameItemPreferenceList
        {
            get
            {
                return ViewState["FrameItemPreferenceList"] as List<SitePrefFrameItemEntity>;
            }
            set
            {
                ViewState["FrameItemPreferenceList"] = value;
            }
        }

        public string SiteCode
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCode;
            }
        }
        #endregion
    }
}