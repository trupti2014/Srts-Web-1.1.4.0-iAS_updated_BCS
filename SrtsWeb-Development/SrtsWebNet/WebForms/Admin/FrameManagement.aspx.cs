using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtDataMgmt")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class FrameManagement : PageBase, IFrameManagement
    {
        private FrameManagementPresenter _presenter;

        public FrameManagement()
        {
            _presenter = new FrameManagementPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();

                // Get the Query String and determine action, Add/Edit
            }
            if (!IsPostBack)
            {
                _presenter.InitView();
            }
            GetSetBosStatusGradeTables();
        }

        private Dictionary<BOSEntity, Dictionary<StatusEntity, List<RankEntity>>> BosStatGrades = new Dictionary<BOSEntity, Dictionary<StatusEntity, List<RankEntity>>>();

        private void GetSetBosStatusGradeTables()
        {
            var img = "<img src=\"../../Styles/images/Opened%20Arrow%20KB.jpg\" data-swap=\"../../Styles/images/Closed%20Arrow%20KB.jpg\" data-src=\"../../Styles/images/Opened%20Arrow%20KB.jpg\" id=\"{0}\" onclick=\"DoToggle('{1}', '{2}');\" />";
            this.BosStatGrades = this._presenter.BuildTableSource();
            var idx = 0;
            foreach (var b in BosStatGrades) // BOS
            {
                this.pnlEligibility.Controls.Add(new LiteralControl(String.Format("<div id = \"div{0}\">", b.Key.BOSValue)));
                this.pnlEligibility.Controls.Add(new LiteralControl("<div style=\"float: left; margin: 18px 0 0 10px\">"));
                this.pnlEligibility.Controls.Add(new LiteralControl(String.Format(img, String.Format("img{0}", b.Key.BOSValue), String.Format("pnl{0}", b.Key.BOSValue), String.Format("img{0}", b.Key.BOSValue))));
                this.pnlEligibility.Controls.Add(new LiteralControl("</div>"));
                this.pnlEligibility.Controls.Add(new LiteralControl(String.Format("<div style=\"float: left; padding-top: 18px; font-weight: bold; padding-left: 5px;\"><h1>{0}</h1></div>", b.Key.BOSText)));
                this.pnlEligibility.Controls.Add(new LiteralControl("<br />"));
                this.pnlEligibility.Controls.Add(new LiteralControl("<br />"));

                var pnl = new Panel();
                pnl.ID = String.Format("pnl{0}", b.Key.BOSValue);
                pnl.ClientIDMode = System.Web.UI.ClientIDMode.Static;

                foreach (var s in b.Value) // Status
                {
                    var tr = new Label() { Text = String.Format("{0}:", s.Key.StatusText), CssClass = "srtsLabel_medium" };
                    pnl.Controls.Add(tr);
                    pnl.Controls.Add(new LiteralControl("<br />"));

                    var tbl = new Table();
                    var tr0 = new TableRow();
                    var tr1 = new TableRow();
                    foreach (var g in s.Value) // Grades
                    {
                        var td0 = new TableCell();
                        var td1 = new TableCell();

                        var lbl = new Label() { Text = g.RankText, CssClass = "srtsLabel_medium" };
                        td0.Controls.Add(lbl);

                        var cb = new CheckBox();
                        cb.ID = String.Format("{0}{1}{2}", g.RankValue, b.Key.BOSValue, s.Key.StatusValue);
                        cb.ClientIDMode = System.Web.UI.ClientIDMode.Static;

                        cb.Checked = true;

                        td1.Controls.Add(cb);

                        tr0.Cells.Add(td0);
                        tr1.Cells.Add(td1);

                        idx++;
                    }
                    tbl.Rows.Add(tr0);
                    tbl.Rows.Add(tr1);

                    pnl.Controls.Add(tbl);

                    pnl.Controls.Add(new LiteralControl("<br />"));
                }

                pnl.Controls.Add(new LiteralControl("<br />"));

                this.pnlEligibility.Controls.Add(pnl);
                this.pnlEligibility.Controls.Add(new LiteralControl("</div>"));
                this.pnlEligibility.Controls.Add(new LiteralControl("<br />"));
                this.pnlEligibility.Controls.Add(new LiteralControl("<hr style=\"text-align: center; width: 75%; margin: 5px 0 5px 0;\" />"));
            }
        }

        private void GetAllEligibilities()
        {
            var elig = new List<String>();

            foreach (var pc in this.pnlEligibility.Controls)
            {
                if (!(pc is Panel)) continue;
                foreach (var tc in ((Panel)pc).Controls)
                {
                    if (!(tc is Table)) continue;
                    foreach (var td in ((Table)tc).Rows[1].Cells)
                    {
                        foreach (var c in ((TableCell)td).Controls)
                        {
                            if (!(c is CheckBox)) continue;
                            if (((CheckBox)c).Checked.Equals(false)) continue;
                            elig.Add(String.Format("{0}{1}", ((CheckBox)c).ID, this.GenderSelected));
                        }
                    }
                }
            }

            this.Eligibilities = elig;
            elig = null;
        }

        private void SetEligibleStates()
        {
            foreach (var pc in this.pnlEligibility.Controls)
            {
                if (!(pc is Panel)) continue;
                foreach (var tc in ((Panel)pc).Controls)
                {
                    if (!(tc is Table)) continue;
                    foreach (var td in ((Table)tc).Rows[1].Cells)
                    {
                        foreach (var c in ((TableCell)td).Controls)
                        {
                            if (!(c is CheckBox)) continue;
                            if (this.Eligibilities == null)
                                ((CheckBox)c).Checked = true;
                            else
                                ((CheckBox)c).Checked = this.Eligibilities.Contains(String.Format("{0}{1}", ((CheckBox)c).ID, this.GenderSelected));
                        }
                    }
                }
            }
        }

        protected void bSubmit_Click(object sender, EventArgs e)
        {
            GetAllEligibilities();

            _presenter.InsertFrame();

            // Re-get all the frames
            _presenter.GetFrames();
            // Set the new/updated frame
            this.FrameCodeSelected = this.FrameInfo.FrameCode;
            // Call the frame ddl selected index changed event to refill the form data
            this.ddlFrames_SelectedIndexChanged("bSubmit_Click", null);
            // Turn off maintain scroll position so the form goes back to the top after postback
            this.MaintainScrollPositionOnPostBack = false;
        }

        protected void bCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/Default.aspx");
        }

        protected void ddlFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set the state of the frame code text box based on the ddl frame selection
            this.tbFrameCode.Enabled = this.FrameCodeSelected.Equals("ADD");
            this.bSubmit.Enabled = this.FrameCodeSelected.Equals("ADD");
            this.lblContactSrts.Visible = !this.FrameCodeSelected.Equals("ADD");

            _presenter.SetSelectedFrameData();
            _presenter.SetSelectedFrameItemData();
            _presenter.SetFrameEligibilities();
            _presenter.SetGenderData();
            _presenter.SetPriorityData();
            SetEligibleStates();
        }

        #region Interface Properties

        public string FrameCodeSelected
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

        public List<FrameEntity> FrameData
        {
            get
            {
                return ViewState["FrameData"] as List<FrameEntity>;
            }
            set
            {
                ViewState["FrameData"] = value;

                this.ddlFrames.DataSource = null;
                this.ddlFrames.DataValueField = "FrameCode";
                this.ddlFrames.DataTextField = "FrameCodeAndDescription";
                this.ddlFrames.DataSource = value;
                this.ddlFrames.DataBind();
                this.ddlFrames.Items.Insert(0, new ListItem("-Add New-", "ADD"));

                var li = new ListItem("────────────", "888888");
                li.Attributes.Add("disabled", "true");

                this.ddlFrames.Items.Insert(1, li);

                this.ddlFrames.SelectedIndex = 0;
            }
        }

        public FrameEntity FrameInfo
        {
            get
            {
                var e = new FrameEntity();
                e.FrameCode = this.tbFrameCode.Text.Trim();
                e.FrameDescription = this.tbDescription.Text.Trim();
                e.FrameNotes = this.tbNotes.Text.Trim();
                e.ImageURL = this.tbImageUrl.Text.Trim();
                e.IsActive = this.cbIsActive.Checked;
                e.IsFoc = this.cbIsFoc.Checked;
                e.IsInsert = this.cbIsInsert.Checked;
                e.MaxPair = Convert.ToInt32(this.tbMaxPair.Text.Trim());

                return e;
            }
            set
            {
                var e = value;

                this.tbFrameCode.Text = e.FrameCode;
                this.tbDescription.Text = e.FrameDescription;
                this.tbNotes.Text = e.FrameNotes;
                this.tbImageUrl.Text = e.ImageURL;
                this.cbIsActive.Checked = e.IsActive;
                this.cbIsFoc.Checked = e.IsFoc;
                this.cbIsInsert.Checked = e.IsInsert;
                this.tbMaxPair.Text = e.MaxPair.ToString();
            }
        }

        public string GenderSelected
        {
            get
            {
                return this.rblGender.SelectedValue;
            }
            set
            {
                this.rblGender.SelectedValue = value;
            }
        }

        public List<FrameItemEntity> FrameItemInfo
        {
            get
            {
                return ViewState["FrameItemInfo"] as List<FrameItemEntity>;
            }
            set
            {
                ViewState["FrameItemInfo"] = value;
            }
        }

        // May want to remove the getters
        public Dictionary<string, string> TempleData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                this.lbTemple.DataSource = null;

                this.lbTemple.DataTextField = "Key";
                this.lbTemple.DataValueField = "Value";
                this.lbTemple.DataSource = value;
                this.lbTemple.DataBind();
                this.lbTemple.Items.Insert(0, new ListItem("-Select-", "X"));
                this.lbTemple.SelectedIndex = 0;
            }
        }

        public Dictionary<string, string> BridgeData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                this.lbBridge.DataSource = null;

                this.lbBridge.DataTextField = "Key";
                this.lbBridge.DataValueField = "Value";
                this.lbBridge.DataSource = value;
                this.lbBridge.DataBind();
                this.lbBridge.Items.Insert(0, new ListItem("-Select-", "X"));
                this.lbBridge.SelectedIndex = 0;
            }
        }

        public Dictionary<string, string> ColorData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                this.lbColor.DataSource = null;

                this.lbColor.DataTextField = "Key";
                this.lbColor.DataValueField = "Value";
                this.lbColor.DataSource = value;
                this.lbColor.DataBind();
                this.lbColor.Items.Insert(0, new ListItem("-Select-", "X"));
                this.lbColor.SelectedIndex = 0;
            }
        }

        public Dictionary<string, string> EyeSizeData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                this.lbEyeSize.DataSource = null;

                this.lbEyeSize.DataTextField = "Key";
                this.lbEyeSize.DataValueField = "Value";
                this.lbEyeSize.DataSource = value;
                this.lbEyeSize.DataBind();
                this.lbEyeSize.Items.Insert(0, new ListItem("-Select-", "X"));
                this.lbEyeSize.SelectedIndex = 0;
            }
        }

        public Dictionary<string, string> LenseTypeData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                this.lbLensType.DataSource = null;

                this.lbLensType.DataTextField = "Key";
                this.lbLensType.DataValueField = "Value";
                this.lbLensType.DataSource = value;
                this.lbLensType.DataBind();
                this.lbLensType.Items.Insert(0, new ListItem("-Select-", "X"));
                this.lbLensType.SelectedIndex = 0;
            }
        }

        public Dictionary<string, string> MaterialData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                this.lbMaterial.DataSource = null;

                this.lbMaterial.DataTextField = "Key";
                this.lbMaterial.DataValueField = "Value";
                this.lbMaterial.DataSource = value;
                this.lbMaterial.DataBind();
                this.lbMaterial.Items.Insert(0, new ListItem("-Select-", "X"));
                this.lbMaterial.SelectedIndex = 0;
            }
        }

        public Dictionary<string, string> TintData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                this.lbTint.DataSource = null;

                this.lbTint.DataTextField = "Key";
                this.lbTint.DataValueField = "Value";
                this.lbTint.DataSource = value;
                this.lbTint.DataBind();
                this.lbTint.Items.Insert(0, new ListItem("-Select-", "X"));
                this.lbTint.SelectedIndex = 0;
            }
        }

        // END may want to remove the getters

        public List<string> TemplesSelected
        {
            get
            {
                return this.lbTemple.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();
            }
            set
            {
                if (value == null || value.Count.Equals(0))
                    this.lbTemple.SelectedIndex = 0;
                else
                    this.lbTemple.Items.Cast<ListItem>().ToList().ForEach(x => x.Selected = value.Contains(x.Value));
            }
        }

        public List<string> BridgesSelected
        {
            get
            {
                return this.lbBridge.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();
            }
            set
            {
                if (value == null || value.Count.Equals(0))
                    this.lbBridge.SelectedIndex = 0;
                else
                    this.lbBridge.Items.Cast<ListItem>().ToList().ForEach(x => x.Selected = value.Contains(x.Value));
            }
        }

        public List<string> ColorsSelected
        {
            get
            {
                return this.lbColor.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();
            }
            set
            {
                if (value == null || value.Count.Equals(0))
                    this.lbColor.SelectedIndex = 0;
                else
                    this.lbColor.Items.Cast<ListItem>().ToList().ForEach(x => x.Selected = value.Contains(x.Value));
            }
        }

        public List<string> EyeSizesSelected
        {
            get
            {
                return this.lbEyeSize.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();
            }
            set
            {
                if (value == null || value.Count.Equals(0))
                    this.lbEyeSize.SelectedIndex = 0;
                else
                    this.lbEyeSize.Items.Cast<ListItem>().ToList().ForEach(x => x.Selected = value.Contains(x.Value));
            }
        }

        public List<string> LensTypesSelected
        {
            get
            {
                return this.lbLensType.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();
            }
            set
            {
                if (value == null || value.Count.Equals(0))
                    this.lbLensType.SelectedIndex = 0;
                else
                    this.lbLensType.Items.Cast<ListItem>().ToList().ForEach(x => x.Selected = value.Contains(x.Value));
            }
        }

        public List<string> MaterialsSelected
        {
            get
            {
                return this.lbMaterial.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();
            }
            set
            {
                if (value == null || value.Count.Equals(0))
                    this.lbMaterial.SelectedIndex = 0;
                else
                    this.lbMaterial.Items.Cast<ListItem>().ToList().ForEach(x => x.Selected = value.Contains(x.Value));
            }
        }

        public List<string> TintsSelected
        {
            get
            {
                return this.lbTint.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();
            }
            set
            {
                if (value == null || value.Count.Equals(0))
                    this.lbTint.SelectedIndex = 0;
                else
                    this.lbTint.Items.Cast<ListItem>().ToList().ForEach(x => x.Selected = value.Contains(x.Value));
            }
        }

        public List<String> Eligibilities
        {
            get
            {
                return ViewState["Eligibilities"] as List<String>;
            }
            set
            {
                ViewState["Eligibilities"] = value;
            }
        }

        public List<OrderPriorityEntity> PriorityData
        {
            get
            {
                return ViewState["PriorityData"] as List<OrderPriorityEntity>;
            }
            set
            {
                ViewState["PriorityData"] = value;
                this.lbPriority.DataSource = null;

                this.lbPriority.DataTextField = "OrderPriorityText";
                this.lbPriority.DataValueField = "OrderPriorityValue";
                this.lbPriority.DataSource = value;
                this.lbPriority.DataBind();
                this.lbPriority.Items.Insert(0, new ListItem("-Select-", "X"));
                this.lbPriority.SelectedIndex = 0;
            }
        }

        public List<string> PrioritiesSelected
        {
            get
            {
                return this.lbPriority.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();
            }
            set
            {
                if (value == null || value.Count.Equals(0))
                    this.lbPriority.SelectedIndex = 0;
                else
                    this.lbPriority.Items.Cast<ListItem>().ToList().ForEach(x => x.Selected = value.Contains(x.Value));
            }
        }

        #endregion Interface Properties
    }
}