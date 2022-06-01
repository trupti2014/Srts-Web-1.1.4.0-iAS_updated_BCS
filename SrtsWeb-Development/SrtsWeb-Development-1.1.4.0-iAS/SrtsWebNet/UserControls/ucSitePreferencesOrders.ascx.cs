using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucSitePreferencesOrders : UserControlBase, IOrderPreferencesView//, IClinicGroupsView
    {
        private SitePreferencesPresenter.OrderPreferencesPresenter p;
        //private SitePreferencesPresenter.ClinicGroupsPreferencesPresenter c;
        protected void Page_Load(object sender, EventArgs e)
        {
            ((SrtsWeb.WebForms.Admin.SitePreferences)this.Page).RefreshUserControl += ucSitePreferencesOrders_RefreshUserControl;
            if (!IsPostBack)
            {
                DoLoadActions();
            }
        }

        private void ucSitePreferencesOrders_RefreshUserControl(object sender, EventArgs e)
        {
            ClearForm();
            DoLoadActions();
        }

        protected void bSubmitOrderPreferences_Click(object sender, EventArgs e)
        {
            var currObj = GetCurrentPrefObject();

            // If there are no changes to save then leave.
            if (IsCurrentObjectEmpty(currObj)) return;

            // If the hashes are the same then there are no changes so leave the event.
            if (this.OrderPreferences.GetObjectHash().Equals(currObj.GetObjectHash())) return;

            // Since the priority is a primary key in the DB it is required at a minimum, if it is not set leave the event.
            if (currObj.PriorityFrameCombo.IsNullOrEmpty() && currObj.InitialLoadPriority.IsNullOrEmpty()) {
                this.hfMsgOrders.Value = "At least one 'Priority Preference' is required to add/edit order preferences.";
                return; 
            }

            // Make a copy of the local OrderPreferences class.
            var tObj = this.OrderPreferences.Clone();

            // Set the local OrderPreferences class equal to the currObj with the new changes.
            this.OrderPreferences = currObj;

            p = new SitePreferencesPresenter.OrderPreferencesPresenter(this);
            if (!p.SetPreferencesToDb())
            {
                this.hfMsgOrders.Value = "There was an error attempting to save order preferences.";
                this.OrderPreferences = tObj;
                return;
            }
            this.hfSuccessOrders.Value = "1";
            this.hfMsgOrders.Value = "Successfully saved order preferences.";
            this.lblLabDiff.Visible = false;
        }

        protected void bSetPreference_Click(object sender, EventArgs e)
        {
            // If nothing is selected then exit the event
            if (this.PriorityPriority.IsNullOrEmpty() && this.PriorityFrame.IsNullOrEmpty()) return;

            // If a priority is selected but no frame the show the error else remove the error and continue
            if (this.PriorityFrame.IsNullOrEmpty())
            {
                this.divPriorityPreferencesError.InnerText = "A frame is required when a priority is selected.";
                return;
            }
            else
                this.divPriorityPreferencesError.InnerText = String.Empty;

            // Get the preference history
            var pph = this.PriorityPreferenceHistory as List<SitePrefOrderPriorityPreferencesEntity>;

            // Determine if the selected priority is already in the history list
            var eP = pph.FirstOrDefault(x => x.PriorityCode == this.PriorityPriority);
            if (eP.IsNull())
            {
                // Add the new values to the local history
                pph.Add(new SitePrefOrderPriorityPreferencesEntity()
                {
                    FrameCode = this.PriorityFrameList.FirstOrDefault(x => x.Value == this.PriorityFrame).Value,
                    FrameDescription = this.PriorityFrameList.FirstOrDefault(x => x.Value == this.PriorityFrame).Key,
                    PriorityCode = this.PriorityPriorityList.FirstOrDefault(x => x.Value == this.PriorityPriority).Value,
                    PriorityDescription = this.PriorityPriorityList.FirstOrDefault(x => x.Value == this.PriorityPriority).Text
                });
            }
            else
            {
                // Update the existing item
                var uI = this.PriorityFrameList.FirstOrDefault(x => x.Value == this.PriorityFrame);
                eP.FrameCode = uI.Value;
                eP.FrameDescription = uI.Key;
            }
            // Set the history property, which will to a bind, with the local history
            this.PriorityPreferenceHistory = pph;

            this.ddlPpPriority.SelectedIndex = 0;
            this.PriorityFrameList = new Dictionary<String, String>();
        }

        protected void ddlInitialPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.p = new SitePreferencesPresenter.OrderPreferencesPresenter(this);
            // Get all frames that are eligible for the selected priority.
            this.InitialLoadFrameList = p.GetFramesByPriority(this.InitialLoadPriority);
        }

        protected void ddlPpPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.p = new SitePreferencesPresenter.OrderPreferencesPresenter(this);
            // Get all frames that are eligible for the selected priority.
            this.PriorityFrameList = p.GetFramesByPriority(this.PriorityPriority);

            // If available highlight the row in the grid where the priorities match
            this.gvPriorityHistory.Rows.Cast<GridViewRow>().ToList().ForEach(x => x.BackColor = System.Drawing.Color.Empty);

            // Determine if the selected priority is already in the current preferences list
            var pref = this.PriorityPreferenceHistory.FirstOrDefault(a => a.PriorityCode == this.PriorityPriority);
            if (pref.IsNull()) return;
            this.PriorityFrame = pref.FrameCode;
            this.gvPriorityHistory.Rows[this.PriorityPreferenceHistory.IndexOf(pref)].BackColor = System.Drawing.ColorTranslator.FromHtml("#A1DCF2");
        }

        protected void gvPriorityHistory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.ToLower().Equals("select")) return;

            // Get the selected row index
            var gvr = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer);
            var i = gvr.RowIndex;

            // Get the cell text
            var pT = ((GridView)sender).Rows[i].Cells[1].Text;
            var fT = ((GridView)sender).Rows[i].Cells[2].Text;

            // Get the priority value
            var pV = this.PriorityPriorityList.FirstOrDefault(x => x.Text == pT).Value;

            // Get the frame list
            this.p = new SitePreferencesPresenter.OrderPreferencesPresenter(this);
            this.PriorityFrameList = this.p.GetFramesByPriority(pV);

            // Get the frame value
            var fV = this.PriorityFrameList.FirstOrDefault(x => x.Key == fT).Value;

            // Set the selected priority and frame
            this.PriorityPriority = pV;
            this.PriorityFrame = fV;

            // Clear other highlighed rows
            ((GridView)sender).Rows.Cast<GridViewRow>().ToList().ForEach(x => x.BackColor = System.Drawing.Color.Empty);

            // Highlight the selected row
            gvr.BackColor = System.Drawing.ColorTranslator.FromHtml("#A1DCF2");
        }

        private void BindDdl<T>(DropDownList ddl, T SourceIn, Boolean AddSelect = true)
        {
            ddl.DataSource = SourceIn;
            ddl.DataBind();
            if (!AddSelect) return;
            ddl.Items.Insert(0, new ListItem("-Select-", "X"));
        }

        private SitePrefOrderEntity GetCurrentPrefObject()
        {
            var a = new SitePrefOrderEntity();
            a.DistributionMethod = this.GlobalDistributionMethod;
            a.InitialLoadFrame = this.InitialLoadFrame;
            a.InitialLoadPriority = this.InitialLoadPriority;
            a.Lab = this.GlobalOrderLab;
            a.PriorityFrameCombo = this.PriorityPreferenceHistory;
            if (a.PriorityFrameCombo.Count.Equals(0) && a.InitialLoadPriority.IsNullOrEmpty().Equals(false))
            {
                a.PriorityFrameCombo.Add(new SitePrefOrderPriorityPreferencesEntity()
                {
                    PriorityCode = a.InitialLoadPriority,
                    PriorityDescription = this.InitialLoadPriorityList.FirstOrDefault(x => x.Value == a.InitialLoadPriority).Text,
                    FrameCode = a.InitialLoadFrame,
                    FrameDescription = this.InitialLoadFrameList.FirstOrDefault(f => f.Value == a.InitialLoadFrame).Key
                });
                this.PriorityPreferenceHistory = a.PriorityFrameCombo;
            }
            a.SiteCode = this.SiteCode;
            return a;
        }

        public Boolean IsCurrentObjectEmpty(SitePrefOrderEntity eIn)
        {
            if (!eIn.DistributionMethod.IsNullOrEmpty()) return false;
            if (!eIn.InitialLoadFrame.IsNullOrEmpty()) return false;
            if (!eIn.InitialLoadPriority.IsNullOrEmpty()) return false;
            if (!eIn.Lab.IsNullOrEmpty()) return false;
            if (!eIn.PriorityFrameCombo.IsNullOrEmpty()) return false;
            return true;
        }

        private void PopulatePreferences()
        {
            this.p = new SitePreferencesPresenter.OrderPreferencesPresenter(this);
            var a = this.OrderPreferences;

            this.InitialLoadPriority = a.InitialLoadPriority.IsNullOrEmpty() ? "X" : a.InitialLoadPriority;

            if (!a.InitialLoadPriority.IsNullOrEmpty())
            {
                this.InitialLoadFrameList = p.GetFramesByPriority(this.InitialLoadPriority);
                if (!a.InitialLoadFrame.IsNullOrEmpty())
                    this.InitialLoadFrame = a.InitialLoadFrame;
            }

            this.GlobalOrderLab = a.Lab.IsNullOrEmpty() ? "X" : a.Lab;
            this.GlobalDistributionMethod = a.DistributionMethod.IsNullOrEmpty() ? "X" : a.DistributionMethod;

            this.PriorityPreferenceHistory = a.PriorityFrameCombo.IsNullOrEmpty() ? new List<SitePrefOrderPriorityPreferencesEntity>() : a.PriorityFrameCombo;
        }

        private void DoLoadActions()
        {
            this.lblLabDiff.Visible = false;
            this.p = new SitePreferencesPresenter.OrderPreferencesPresenter(this);
           
            p.GetPriorityList();
            p.GetLabList();
            p.GetDistributionMethodList();
            p.GetPreferences();
            PopulatePreferences();
            //BindClinicGroups();

        }

        private void ClearForm()
        {
            this.PriorityFrameList = new Dictionary<String, String>();
            this.PriorityPriorityList = new List<LookupTableEntity>();
            this.InitialLoadFrameList = new Dictionary<String, String>();
            this.InitialLoadPriorityList = new List<LookupTableEntity>();
            this.PriorityPreferenceHistory = new List<SitePrefOrderPriorityPreferencesEntity>();
        }

        #region INTERFACE PROPERTIES

        public string GlobalOrderLab
        {
            get
            {
                return this.ddlGlobalLab.SelectedValue.Equals("X") ? String.Empty : this.ddlGlobalLab.SelectedValue;
            }
            set
            {
                if (this.LabList.Any(x => x.Value == value) || value.Equals("X"))
                {
                    this.ddlGlobalLab.SelectedValue = value.IsNullOrEmpty() ? "X" : value;
                    this.lblLabDiff.Visible = false;
                }
                else
                {
                    this.ddlGlobalLab.SelectedValue = "X";
                    this.lblLabDiff.Text = String.Format("Current lab preference, \"{0}\", is not in list.  Please make a new selection.", value.Substring(1));
                    this.lblLabDiff.Visible = true;
                }
            }
        }

        public string GlobalDistributionMethod
        {
            get
            {
                return this.ddlGlobalDistMethod.SelectedValue.Equals("X") ? String.Empty : this.ddlGlobalDistMethod.SelectedValue;
            }
            set
            {
                this.ddlGlobalDistMethod.SelectedValue = value;
            }
        }

        public IDictionary<String, String> DistributionMethodList
        {
            get
            {
                return ViewState["DistributionMethodList"] as Dictionary<String, String> ?? new Dictionary<String, String>();
            }
            set
            {
                ViewState["DistributionMethodList"] = value;

                this.ddlGlobalDistMethod.DataTextField = "Key";
                this.ddlGlobalDistMethod.DataValueField = "Value";
                BindDdl(this.ddlGlobalDistMethod, value);
            }
        }

        public IDictionary<String, String> LabList
        {
            get
            {
                return ViewState["LabList"] as Dictionary<String, String> ?? new Dictionary<String, String>();
            }
            set
            {
                ViewState["LabList"] = value;
                this.ddlGlobalLab.DataTextField = "Key";
                this.ddlGlobalLab.DataValueField = "Value";
                BindDdl(this.ddlGlobalLab, value);
            }
        }

        public string InitialLoadPriority
        {
            get
            {
                return this.ddlInitialPriority.SelectedValue.Equals("X") ? String.Empty : this.ddlInitialPriority.SelectedValue;
            }
            set
            {
                this.ddlInitialPriority.SelectedValue = value;
            }
        }

        public string InitialLoadFrame
        {
            get
            {
                return this.ddlInitialFrame.SelectedValue.Equals("X") ? String.Empty : this.ddlInitialFrame.SelectedValue;
            }
            set
            {
                this.ddlInitialFrame.SelectedValue = value;
            }
        }

        public IDictionary<String, String> InitialLoadFrameList
        {
            get
            {
                return ViewState["InitialLoadFrameList"] as Dictionary<String, String> ?? new Dictionary<String, String>();
            }
            set
            {
                ViewState["InitialLoadFrameList"] = value;

                this.ddlInitialFrame.DataTextField = "Key";// "FrameCodeAndDescription";
                this.ddlInitialFrame.DataValueField = "Value";// "FrameCode";
                BindDdl(this.ddlInitialFrame, value);
            }
        }

        public IEnumerable<LookupTableEntity> InitialLoadPriorityList
        {
            get
            {
                return ViewState["InitialLoadPriorityList"] as List<LookupTableEntity> ?? new List<LookupTableEntity>();
            }
            set
            {
                ViewState["InitialLoadPriorityList"] = value;
                this.ddlInitialPriority.DataTextField = "Text";
                this.ddlInitialPriority.DataValueField = "Value";
                BindDdl(this.ddlInitialPriority, value);
            }
        }

        public string PriorityPriority
        {
            get
            {
                return this.ddlPpPriority.SelectedValue.Equals("X") ? String.Empty : this.ddlPpPriority.SelectedValue;
            }
            set
            {
                this.ddlPpPriority.SelectedValue = value;
            }
        }

        public string PriorityFrame
        {
            get
            {
                return this.ddlPpFrame.SelectedValue.Equals("X") ? String.Empty : this.ddlPpFrame.SelectedValue;
            }
            set
            {
                this.ddlPpFrame.SelectedValue = value;
            }
        }

        public IDictionary<String, String> PriorityFrameList
        {
            get
            {
                return ViewState["PriorityFrameList"] as Dictionary<String, String> ?? new Dictionary<String, String>();
            }
            set
            {
                ViewState["PriorityFrameList"] = value;

                this.ddlPpFrame.DataTextField = "Key";// "FrameLongDescription";
                this.ddlPpFrame.DataValueField = "Value";// "FrameCode";
                BindDdl(this.ddlPpFrame, value);
            }
        }

        public IEnumerable<LookupTableEntity> PriorityPriorityList
        {
            get
            {
                return ViewState["PriorityPriorityList"] as List<LookupTableEntity> ?? new List<LookupTableEntity>();
            }
            set
            {
                ViewState["PriorityPriorityList"] = value;

                this.ddlPpPriority.DataTextField = "Text";
                this.ddlPpPriority.DataValueField = "Value";
                BindDdl(this.ddlPpPriority, value);
            }
        }

        public List<SitePrefOrderPriorityPreferencesEntity> PriorityPreferenceHistory
        {
            get
            {
                return ViewState["PriorityPreferenceHistory"] as List<SitePrefOrderPriorityPreferencesEntity> ?? new List<SitePrefOrderPriorityPreferencesEntity>();
            }
            set
            {
                ViewState["PriorityPreferenceHistory"] = value;
                this.gvPriorityHistory.DataSource = value;
                this.gvPriorityHistory.DataBind();
            }
        }

        /// <summary>
        /// This is the object as is appears in the database and is used to compare against the data on the form for a dirty test.
        /// </summary>
        public SitePrefOrderEntity OrderPreferences
        {
            get
            {
                return ViewState["OrderPreferences"] as SitePrefOrderEntity ?? new SitePrefOrderEntity();
            }
            set
            {
                ViewState["OrderPreferences"] = value;
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

        public IEnumerable<SiteCodeEntity> SiteCodes
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCodes;
            }
        }

        #endregion INTERFACE PROPERTIES
        /*
        #region CLINIC GROUPS

        //Add a New Group Click
        protected void bCreateGroupBtn_Click(object sender, EventArgs e)
        {
            var eventMsg = "added";
            bool activeState = true;
            var gN = this.GroupNameTextBox.Text;
            var gD = this.GroupDescTextBox.Text;
            bool valid = false;

            // If no value then exit the event
            if (this.GroupNameTextBox.Text.IsNull() && this.GroupDescTextBox.Text.IsNull())
            {
                valid = false;
                return;
            }

            //If null or less than char limit, give error and return
            if (this.GroupNameTextBox.Text.IsNull())
            {
                this.divAddClinicGroupError.InnerText = "A name is required for the group.";
                valid = false;
                return;
            }
            else if (this.GroupNameTextBox.Text.Length >= 25)
            {
                this.divAddClinicGroupError.InnerText = "The name must be less than 25 characters.";
                valid = false;
                return;
            }
            else if (this.GroupDescTextBox.Text.IsNull())
            {
                this.divAddClinicGroupError.InnerText = "A description is required for the group.";
                valid = false;
                return;
            }
            else if (this.GroupDescTextBox.Text.Length >= 100)
            {
                this.divAddClinicGroupError.InnerText = "The description must be less than 100 characters.";
                valid = false;
                return;
            }
            else
            {
                this.divAddClinicGroupError.InnerText = String.Empty;
                valid = true;
            }

            if (valid)
            {
                //if validation successful, then add group
                InsertClinicGroup(eventMsg, activeState, gN, gD);
                //reset the text box's value
                this.GroupNameTextBox.Text = "Group Name";
                this.GroupDescTextBox.Text = "Group Description";
            }

           
        }

        //adds a single clinic group
        protected void InsertClinicGroup(string eventMsg, bool activeState, string gN, string gD)
        {
            c = new SitePreferencesPresenter.ClinicGroupsPreferencesPresenter(this);
            SitePrefClinicGroupsEntity group = new SitePrefClinicGroupsEntity();
            group.ClinicSite = SiteCode;
            group.IsActive = activeState;
            group.GroupName = gN;
            group.GroupDesc = gD;

            var result = c.InsertClinicGroup(group);

            // send message of status
            if (!result)
            {
                this.divAddClinicGroupError.InnerText = "There was an error attempting to " + eventMsg + " the Clinic Group.";
                return;
            }
            this.divAddClinicGroupError.InnerText = String.Empty;
            this.divAddClinicGroupSuccess.InnerText = "Successfully " + eventMsg + " Clinic Group.";

            BindClinicGroups();
        }

        protected void BindClinicGroups()
        {
            List<SitePrefClinicGroupsEntity> groups = new List<SitePrefClinicGroupsEntity>();
            var getGroups = new SitePreferencesPresenter.ClinicGroupsPreferencesPresenter(this);
            groups = getGroups.GetClinicGroupsDefaults().ToList();
            this.gvClinicGroups.DataSource = groups;
            this.gvClinicGroups.DataBind();
        }

        protected void gvClinicGroups_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var rowItem = (SitePrefClinicGroupsEntity)(e.Row.DataItem);
                if (!rowItem.IsActive)
                {
                    e.Row.CssClass = "gvGroupDisabled";
                }
            }

        }

        protected void gvClinicGroups_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Get the selected row index
            var gvr = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer);
            var i = gvr.RowIndex;

            // Get the cell text
            var nT = ((GridView)sender).Rows[i].Cells[1].Text;
            var dT = ((GridView)sender).Rows[i].Cells[2].Text;

            // Get the names and sort alphabetically
            var nV = this.GroupName.FirstOrDefault(x => x.ToString() == nT);
            // Get the descriptions and sort alphabetically (as secondary sorting)
            var dV = this.GroupDesc.FirstOrDefault(x => x.ToString() == dT);

        }

        //activates a single group
        protected void gvrClinicGroup_Activate(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            //find the row that the button was pressed
            int rowIndex = ((GridViewRow)((System.Web.UI.Control)sender).NamingContainer).RowIndex;
            //get the text from the cells in that row
            var gN = gvClinicGroups.Rows[rowIndex].Cells[1].Text;
            var gD = gvClinicGroups.Rows[rowIndex].Cells[2].Text;
            var lbl = gvClinicGroups.Rows[rowIndex].FindControl("");
            var eventMsg = "";
            bool activeState = false;

            //get correct button (enabled or disabled) and set the 'isActive' state
            if (btn.Text.ToLower() == "enable")
            {
                lbl = gvClinicGroups.Rows[rowIndex].FindControl("EnableSingleGroup");
                activeState = true;
                eventMsg = "enabled";
            }
            else if (btn.Text.ToLower() == "disable")
            {
                lbl = gvClinicGroups.Rows[rowIndex].FindControl("DisableSingleGroup");
                activeState = false;
                eventMsg = "disabled";
            }
            
            //if button isn't null (which it should never be), adjust the grouping accordingly
            if (lbl != null)
            {
                InsertClinicGroup(eventMsg, activeState, gN, gD);
            }
        }

        //activates or deactives ALL groups
        protected void bAllGroupsStatus_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            c = new SitePreferencesPresenter.ClinicGroupsPreferencesPresenter(this);
            SitePrefClinicGroupsEntity group = new SitePrefClinicGroupsEntity();
            group.ClinicSite = SiteCode;
 
           
            var eventMsg = "";
            //is the checkbox checked?
            if (btn.Text.ToLower().Contains("enable"))
            {
                group.IsActive = true;
                eventMsg = "enabled";
            }
            else if (btn.Text.ToLower().Contains("disable"))
            {
                group.IsActive = false;
                eventMsg = "disabled";
            }

            var result = c.ActivateAllClinicGroups(group);
            // send message of status
            if (!result)
            {
                this.divAddClinicGroupError.InnerText = "There was an error attempting to " + eventMsg + " all of the Clinic Groups.";
                return;
            }
            this.divAddClinicGroupError.InnerText = String.Empty;
            this.divAddClinicGroupSuccess.InnerText = "Successfully " + eventMsg + " all of the Clinic Groups.";

            BindClinicGroups();
        }

        #endregion
        public string GroupName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string GroupDesc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }*/
    }
}