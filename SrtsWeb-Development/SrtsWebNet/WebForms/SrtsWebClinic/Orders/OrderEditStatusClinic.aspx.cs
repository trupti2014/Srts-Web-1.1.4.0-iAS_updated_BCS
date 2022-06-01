using SrtsWeb;
using SrtsWeb.BusinessLayer.Presenters.Orders;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders;
using SrtsWeb.BusinessLayer.Views.Orders;
using SrtsWeb.CustomErrors;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWebClinic.Orders
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    public partial class OrderEditStatusClinic : PageBase, IOrderEditStatusClinicView, ISiteMapResolver
    {
        private OrderEditStatusClinicPresenter _presenter;

        public OrderEditStatusClinic()
        {
            _presenter = new OrderEditStatusClinicPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!Page.IsPostBack)
            {
                CurrentModule("Manage Orders - Edit Status Problem Orders");

                mySession.AddOrEdit = "EDIT";

                _presenter.InitView();

                var demo = mySession.SelectedOrder.Demographic;
                SrtsWeb.BusinessLayer.Concrete.DemographicXMLHelper h = new SrtsWeb.BusinessLayer.Concrete.DemographicXMLHelper();
                this.PriorityList = h.GetOrderPrioritiesByBOSStatusAndRank(demo.ToBOSKey(), demo.ToPatientStatusKey(), demo.ToRankKey());

                this.PrioritySelected = demo.Substring(7, 1);

                ddlPriority.Focus();
                SetLab();
                SetAddressRbl();

                bDeleteButton.Visible = OrderStatus.ToLower().Equals("incomplete order");

                cbApproved.Visible = mySession.SelectedOrder.Demographic.ToBOSKey().Equals("C");

                if (mySession != null)
                {
                    mySession.TempID = 0;
                    mySession.ReturnURL = "PatientDetails.aspx";
                    litName.Text = string.Format("Order Number - {0} - Patient: {1} - Rank: {2} - Priority: {3} - Order Status: {4}",
                        mySession.SelectedOrder.OrderNumber,
                        mySession.Patient.Individual.NameLFMi,
                         SrtsExtender.ToRankValue(mySession.SelectedOrder.Demographic),
                         SrtsExtender.ToOrderPriorityValue(mySession.SelectedOrder.Demographic), OrderStatus);
                }
                if (mySession.SelectedOrder.IsMultivision)
                {
                    LabSelected = "SingleVision";
                }
                else
                {
                    LabSelected = "MultiVision";
                }

                if (!User.IsInRole("clinictech"))
                {
                    this.Controls.SetControlStateBtn(false);
                    bCancelBottom.Enabled = true;
                }
            }

            if (OrderStatusID != 1 &&
                OrderStatusID != 3 &&
                OrderStatusID != 15 &&
                OrderStatusID != 16)
            {
                SetReadOnly();
            }
            else
            {
                bSubmitBottom.Visible = true;
            }
        }

        public string PageTitle
        {
            set { mySession.MainContentTitle = string.Format("{0} Patient Order", value); }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Orders/ManageOrders.aspx", "Manage Clinic Orders");
            child.ParentNode = parent;
            SiteMapNode child2 = new SiteMapNode(e.Provider, "3", "~/SrtsWebClinic/Orders/OrderEditStatusClinic.aspx", "Resolve Order");
            child2.ParentNode = child;
            return child2;
        }

        private void SetReadOnly()
        {
            this.bSubmitBottom.Visible = false;

            this.Controls.SetControlStateTbDdlRbl(false);
        }

        private void SetLab()
        {
            // If the selected frame is part of the CustFramToLabColl variable then set the LabSelected property to the correct lab.
            if (OrderEntity.CustFrameToLabColl.ContainsKey(this.ddlFrame.SelectedValue))
            {
                var sc = new List<String>();
                OrderEntity.CustFrameToLabColl.TryGetValues(this.ddlFrame.SelectedValue, out sc);

                if (!this.ddlFrame.SelectedValue.ToLower().Equals("5am") && !this.ddlFrame.SelectedValue.ToLower().Equals("5am50")
                     && !this.ddlFrame.SelectedValue.ToLower().Equals("5am52") && !this.ddlFrame.SelectedValue.ToLower().Equals("5am54"))
                    this.LabSelected = sc[0];
                else
                {
                    var d = new Dictionary<String, String>();
                    var vItem = String.Empty;
                    var kItem = string.Empty;
                    var s = sc.FirstOrDefault(x => x == mySession.MySite.MultiPrimary);
                    if (String.IsNullOrEmpty(s))
                    {
                        vItem = String.Format("1{0}", "MNOST1");
                        kItem = string.Format("{0} - {1}", "MultiVision", "MNOST1");
                        s = "MNOST1";
                    }
                    else
                    {
                        vItem = String.Format("1{0}", "MNOST1");
                        kItem = string.Format("{0} - {1}", "MultiVision", "MNOST1");
                    }

                    d.Add(kItem, vItem);
                    this.LabData = d;
                    this.LabSelected = String.Format("1{0}", s);
                }
                // No matter what lens is selected if the frame is required to go to a special lab then exit method before processing lens ddls.
                return;
            }

            if (this.ddlLens.Items.Count > 0)
            {
                if (this.ddlLens.SelectedValue.Equals("X")) return;
                if (LensSelected.Substring(0, 2).ToLower().Equals("sv")) return;

                if (this.ddlLab.Items[0].Text.StartsWith("Multi"))
                    this.ddlLab.SelectedIndex = 0;
                else
                    this.ddlLab.SelectedIndex = 1;
            }
            else
            {
                if (this.ddlLab.Items[0].Text.StartsWith("Single"))
                    this.ddlLab.SelectedIndex = 0;
                else
                    this.ddlLab.SelectedIndex = 1;
            }
        }

        private void SetAddressRbl()
        {
            if (_presenter.HasAddress()) return;
            this.rblShipTo.SelectedValue = "C";
            this.rblShipTo.Enabled = false;
        }

        private bool CheckForFocJustification()
        {
            String FOC = "FOC:";
            if (this.Comment1.IsTextAfter(FOC)) return true;
            if (this.Comment2.IsTextAfter(FOC)) return true;
            if (this.Comment1.Contains(FOC)) return false;

            if (this.Comment1.Length < 60)

                this.Comment1 = this.Comment1.Length > 0 ? String.Format("{0}  FOC:", this.Comment1) : "FOC:";
            else
                this.Comment2 = this.Comment2.Length > 0 ? String.Format("{0}  FOC:", this.Comment2) : "FOC:";

            return false;
        }

        private void ShowMessage()
        {
            CustomValidator cv = new CustomValidator();
            cv.IsValid = false;
            cv.ErrorMessage = Message;
            this.Page.Validators.Add(cv);
            return;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

            // THIS MIGHT NEED TO CHANGE TO ACCOUNT FOR EDITING TO A FOC!!!!!!!
            // If it is FOC, and has an existing FOC date/order, and has not current justification
            if (this.ddlPriority.SelectedValue.Equals("F"))
            {
                if (_presenter.CheckFOCDate())
                {
                    if (!CheckForFocJustification())
                    {
                        string Number = Comment1.Contains("FOC:") ? "1" : "2";
                        Message = String.Format("FOC was used within the previous year, please provide your justification below in Comment {0} field, after the text \"FOC:\".", Number);
                        ShowMessage();
                        return;
                    }
                }
            }

            if (!Page.IsValid) return;

            Message = string.Empty;
            string BOSkey = mySession.SelectedOrder.Demographic.ToBOSKey();

            if (IsApproved)
            {
                _presenter.UpdateApproved();
            }

            if (BOSkey != "C" || BOSkey == "C" && IsApproved == true)
            {
                _presenter.SaveData();
            }
            else
            {
                Message = "This order requires approval prior to submission.";
            }

            if (!string.IsNullOrEmpty(Message))
            {
                ShowMessage();
            }
            else
            {
                Response.Redirect("ManageOrders.aspx/problem", false);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageOrders.aspx/problem");
        }

        protected void bDeleteButton_Click(object sender, EventArgs e)
        {
            _presenter.Delete();
            Response.Redirect("ManageOrders.aspx/problem");
        }

        #region PageEvents
        protected void ddlPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySession.AddOrEdit = "";
            _presenter.FillFrameData();

            SetLab();
        }

        protected void ddlFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLab();
            _presenter.FillItemsData(mySession.FrameData);
        }

        protected void ddlLens_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedLens;

            if (ddlLens.SelectedValue.Length > 1)
            {
                SelectedLens = ddlLens.SelectedValue.ToString().Substring(0, 2).ToLower();
            }
            else
            {
                SelectedLens = ddlLens.SelectedValue.ToString().ToLower();
            }

            if (SelectedLens == "sv" || SelectedLens == "x")
            {
                tbODSegHeight.Text = "";
                tbODSegHeight.Enabled = false;
                tbODSegHeight.BorderColor = System.Drawing.Color.LightGray;
                tbOSSegHeight.Text = "";
                tbOSSegHeight.Enabled = false;
                tbOSSegHeight.BorderColor = System.Drawing.Color.LightGray;
            }
            else
            {
                tbODSegHeight.Enabled = true;
                tbODSegHeight.BorderColor = System.Drawing.ColorTranslator.FromHtml("#E4CFAC");
                tbOSSegHeight.Enabled = true;
                tbOSSegHeight.BorderColor = System.Drawing.ColorTranslator.FromHtml("#E4CFAC");
            }

            SetLab();
        }
        #endregion PageEvents

        #region Input Validators
        protected void ValidateComment1Format(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Length > 0)
            {
                int limit = 90;
                if (args.IsValid = args.Value.ValidateCommentLength(limit))
                {
                    if (args.IsValid = args.Value.ValidateCommentFormat())
                    {
                    }
                    else
                    {
                        cvComment1.ErrorMessage = "Invalid characters in Comment 1";
                    }
                }
                else
                {
                    cvComment1.ErrorMessage = string.Format("Limit is {0} characters in Comment 1", limit.ToString());
                }
            }
        }

        protected void ValidateComment2Format(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Length > 0)
            {
                int limit = 90;
                if (args.IsValid = args.Value.ValidateCommentLength(limit))
                {
                    if (args.IsValid = args.Value.ValidateCommentFormat())
                    {
                    }
                    else
                    {
                        cvComment2.ErrorMessage = "Invalid characters in Comment 2";
                    }
                }
                else
                {
                    cvComment2.ErrorMessage = string.Format("Limit is {0} characters in Comment 2", limit.ToString());
                }
            }
        }
        #endregion Input Validators

        #region Order Accessors

        public bool IsShipToPatient
        {
            get { return this.rblShipTo.SelectedValue == "C" ? false : true; }
            set { this.rblShipTo.SelectedValue = value.Equals(true) ? "P" : "C"; }
        }

        public bool IsMultiVision
        {
            get { return this.ddlLens.Items.Count > 0 ? !this.ddlLens.SelectedValue.Substring(0, 2).ToLower().Equals("sv") : true; }
            set { }
        }

        private List<PersonnelEntity> _doctorData;

        public List<PersonnelEntity> DoctorData
        {
            get { return _doctorData; }
            set
            {
                _doctorData = value;
                ddlVerification.Items.Clear();
                ddlVerification.DataSource = _doctorData;
                ddlVerification.DataBind();
            }
        }

        public int DoctorSelected
        {
            get { return SrtsExtender.GetIntVal(ddlVerification.SelectedValue); }
            set
            {
                ddlVerification.SelectedValue = value.ToString();
                if (value > 0)
                {
                    lblVerification.Visible = true;
                    ddlVerification.Visible = true;
                }
            }
        }

        private bool _eligibilityVisibility;

        public bool EligibilityVisibility
        {
            get { return _eligibilityVisibility; }
            set
            {
                _eligibilityVisibility = value;
                lblVerification.Visible = _eligibilityVisibility;
                ddlVerification.Visible = _eligibilityVisibility;
            }
        }

        private DataTable _frameData;

        public DataTable FrameData
        {
            get { return _frameData; }
            set
            {
                _frameData = value;
                ddlFrame.Items.Clear();
                ddlFrame.DataSource = _frameData;
                ddlFrame.DataBind();
            }
        }

        public string FrameSelected
        {
            get { return ddlFrame.SelectedValue; }
            set { ddlFrame.SelectedValue = value; }
        }

        private DataTable _colorData;

        public DataTable ColorData
        {
            get { return _colorData; }
            set
            {
                _colorData = value;
                ddlColor.Items.Clear();
                ddlColor.DataSource = _colorData;
                ddlColor.DataBind();
            }
        }

        public string ColorSelected
        {
            get { return ddlColor.SelectedValue; }
            set { ddlColor.SelectedValue = value; }
        }

        private DataTable _eyeData;

        public DataTable EyeData
        {
            get { return _eyeData; }
            set
            {
                _eyeData = value;
                ddlEye.Items.Clear();
                ddlEye.DataSource = _eyeData;
                ddlEye.DataBind();
            }
        }

        public string EyeSelected
        {
            get { return ddlEye.SelectedValue; }
            set { ddlEye.SelectedValue = value; }
        }

        private DataTable _bridgeData;

        public DataTable BridgeData
        {
            get { return _bridgeData; }
            set
            {
                _bridgeData = value;
                ddlBridge.Items.Clear();
                ddlBridge.DataSource = _bridgeData;
                ddlBridge.DataBind();
            }
        }

        public string BridgeSelected
        {
            get { return ddlBridge.SelectedValue; }
            set { ddlBridge.SelectedValue = value; }
        }

        private DataTable _templeData;

        public DataTable TempleData
        {
            get { return _templeData; }
            set
            {
                _templeData = value;
                ddlTemple.Items.Clear();
                ddlTemple.DataSource = _templeData;
                ddlTemple.DataBind();
            }
        }

        public string TempleSelected
        {
            get { return ddlTemple.SelectedValue; }
            set { ddlTemple.SelectedValue = value; }
        }

        private DataTable _lensData;

        public DataTable LensData
        {
            get { return _lensData; }
            set
            {
                _lensData = value;
                ddlLens.Items.Clear();
                ddlLens.DataSource = _lensData;
                ddlLens.DataBind();
            }
        }

        public string LensSelected
        {
            get { return ddlLens.SelectedValue; }
            set
            {
                ddlLens.SelectedValue = value;
                string SelectedLens;

                if (ddlLens.SelectedValue.Length > 1)
                {
                    SelectedLens = ddlLens.SelectedValue.ToString().Substring(0, 2).ToLower();
                }
                else
                {
                    SelectedLens = ddlLens.SelectedValue.ToString().ToLower();
                }

                if (SelectedLens == "sv" || SelectedLens == "x")
                {
                    tbODSegHeight.Text = "";
                    tbODSegHeight.Enabled = false;
                    tbODSegHeight.BorderColor = System.Drawing.Color.LightGray;
                    tbOSSegHeight.Text = "";
                    tbOSSegHeight.Enabled = false;
                    tbOSSegHeight.BorderColor = System.Drawing.Color.LightGray;
                }
                else
                {
                    tbODSegHeight.Enabled = true;
                    tbODSegHeight.BorderColor = System.Drawing.ColorTranslator.FromHtml("#E4CFAC");
                    tbOSSegHeight.Enabled = true;
                    tbOSSegHeight.BorderColor = System.Drawing.ColorTranslator.FromHtml("#E4CFAC");
                }
            }
        }

        public string ODSegHeight
        {
            get
            {
                return tbODSegHeight.Text;
            }
            set
            {
                tbODSegHeight.Text = value;
            }
        }

        public string OSSegHeight
        {
            get
            {
                return tbOSSegHeight.Text;
            }
            set
            {
                tbOSSegHeight.Text = value;
            }
        }

        private DataTable _materialData;

        public DataTable MaterialData
        {
            get { return _materialData; }
            set
            {
                _materialData = value;
                ddlMaterial.Items.Clear();
                ddlMaterial.DataSource = _materialData;
                ddlMaterial.DataBind();
            }
        }

        public string MaterialSelected
        {
            get { return ddlMaterial.SelectedValue; }
            set { ddlMaterial.SelectedValue = value; }
        }

        private DataTable _tintData;

        public DataTable TintData
        {
            get { return _tintData; }
            set
            {
                _tintData = value;
                ddlTint.Items.Clear();
                ddlTint.DataSource = _tintData;
                ddlTint.DataBind();
            }
        }

        public string TintSelected
        {
            get { return ddlTint.SelectedValue; }
            set { ddlTint.SelectedValue = value; }
        }
        #endregion Order Accessors

        #region Prescription Accessors

        public string ODSphere
        {
            get { return tbODSphere.Text; }
            set { tbODSphere.Text = value; }
        }

        public string ODCylinder
        {
            get { return tbODCylinder.Text; }
            set { tbODCylinder.Text = value; }
        }

        public string ODAxis
        {
            get { return tbODAxis.Text; }
            set { tbODAxis.Text = value; }
        }

        public string ODHPrism
        {
            get { return tbODHPrism.Text; }
            set { tbODHPrism.Text = value; }
        }

        public string ODHBase
        {
            get { return ddlODHBase.SelectedValue; }
            set { ddlODHBase.SelectedValue = value; }
        }

        public string ODVPrism
        {
            get { return tbODVPrism.Text; }
            set { tbODVPrism.Text = value; }
        }

        public string ODVBase
        {
            get { return ddlODVBase.SelectedValue; }
            set { ddlODVBase.SelectedValue = value; }
        }

        public string ODAdd
        {
            get { return tbODAdd.Text; }
            set { tbODAdd.Text = value; }
        }

        public string OSSphere
        {
            get { return tbOSSphere.Text; }
            set { tbOSSphere.Text = value; }
        }

        public string OSCylinder
        {
            get { return tbOSCylinder.Text; }
            set { tbOSCylinder.Text = value; }
        }

        public string OSAxis
        {
            get { return tbOSAxis.Text; }
            set { tbOSAxis.Text = value; }
        }

        public string OSHPrism
        {
            get { return tbOSHPrism.Text; }
            set { tbOSHPrism.Text = value; }
        }

        public string OSHBase
        {
            get { return ddlOSHBase.SelectedValue; }
            set { ddlOSHBase.SelectedValue = value; }
        }

        public string OSVPrism
        {
            get { return tbOSVPrism.Text; }
            set { tbOSVPrism.Text = value; }
        }

        public string OSVBase
        {
            get { return ddlOSVBase.SelectedValue; }
            set { ddlOSVBase.SelectedValue = value; }
        }

        public string OSAdd
        {
            get { return tbOSAdd.Text; }
            set { tbOSAdd.Text = value; }
        }

        public string PDTotal
        {
            get { return tbPDTotal.Text; }
            set
            {
                tbPDTotal.Text = value;
                if (decimal.Parse(value) > 0)
                {
                    tablePDTotal.Visible = true;
                    tablePDTotalNear.Visible = true;

                    tablePDOD.Visible = false;
                    tablePDODNear.Visible = false;

                    tablePDOS.Visible = false;
                    tablePDOSNear.Visible = false;
                }
                else
                {
                    tablePDTotal.Visible = false;
                    tablePDTotalNear.Visible = false;

                    tablePDOD.Visible = true;
                    tablePDODNear.Visible = true;

                    tablePDOS.Visible = true;
                    tablePDOSNear.Visible = true;
                }
            }
        }

        public string PDTotalNear
        {
            get { return tbPDTotalNear.Text; }
            set { tbPDTotalNear.Text = value; }
        }

        public string PDOD
        {
            get { return tbPDOD.Text; }
            set { tbPDOD.Text = value; }
        }

        public string PDOS
        {
            get { return tbPDOS.Text; }
            set { tbPDOS.Text = value; }
        }

        public string PDODNear
        {
            get { return tbPDODNear.Text; }
            set { tbPDODNear.Text = value; }
        }

        public string PDOSNear
        {
            get { return tbPDOSNear.Text; }
            set { tbPDOSNear.Text = value; }
        }

        public bool IsMonoCalculation
        {
            get { return bool.Parse(ViewState["PDisMono"].ToString()); }
            set { ViewState.Add("PDisMono", value); }
        }
        #endregion Prescription Accessors

        #region Misc Accessors

        private int _orderStatusID;

        public int OrderStatusID
        {
            get
            {
                Int32 o = default(Int32);
                if (ViewState["orderstatusid"] == null) return _orderStatusID;

                Int32.TryParse(ViewState["orderstatusid"].ToString(), out o);
                return o;
            }
            set
            {
                _orderStatusID = value;
                ViewState.Add("orderstatusid", _orderStatusID);
            }
        }

        public string OrderStatus
        {
            get { return ViewState["OrderStatus"].ToString(); }
            set { ViewState.Add("OrderStatus", value); }
        }

        public string LabJustification
        {
            set { this.lblLabJust.Text = value; }
        }

        public bool IsApproved
        {
            get
            {
                if (cbApproved.Checked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    cbApproved.Checked = true;
                }
                else
                {
                    cbApproved.Checked = false;
                }
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private List<KeyValuePair<string, string>> _priorityData;

        public List<KeyValuePair<string, string>> PriorityData
        {
            get { return _priorityData; }
            set
            {
                _priorityData = value;
                ddlPriority.Items.Clear();
                ddlPriority.DataSource = _priorityData;
                ddlPriority.DataTextField = "Value";
                ddlPriority.DataValueField = "Key";
                ddlPriority.DataBind();
            }
        }

        public List<OrderPriorityEntity> PriorityList
        {
            set
            {
                ddlPriority.Items.Clear();
                ddlPriority.SelectedValue = null;
                ddlPriority.DataSource = value;
                ddlPriority.DataTextField = "OrderPriorityText";
                ddlPriority.DataValueField = "OrderPriorityValue";
                ddlPriority.DataBind();
                ddlPriority.Items.Remove(ddlPriority.Items.FindByValue("N"));
                ddlPriority.Items.Insert(0, "-Select-");
                ddlPriority.SelectedIndex = 0;
            }
        }

        public string PrioritySelected
        {
            get { return ddlPriority.SelectedValue; }
            set { ddlPriority.SelectedValue = value; }
        }

        public string Comment1
        {
            get { return tbComment1.Text; }
            set { tbComment1.Text = value; }
        }

        public string Comment2
        {
            get { return tbComment2.Text; }
            set { tbComment2.Text = value; }
        }

        private List<PersonnelEntity> _techData;

        public List<PersonnelEntity> TechData
        {
            get { return _techData; }
            set
            {
                _techData = value;
                ddlTechnician.Items.Clear();
                ddlTechnician.DataSource = _techData;
                ddlTechnician.DataBind();
            }
        }

        public int TechSelected
        {
            get { return SrtsExtender.GetIntVal(ddlTechnician.SelectedValue); }
            set { ddlTechnician.SelectedValue = value.ToString(); }
        }

        private Dictionary<String, String> _labData;

        public Dictionary<String, String> LabData
        {
            get { return _labData; }
            set
            {
                _labData = value;
                ddlLab.Items.Clear();
                ddlLab.SelectedIndex = -1;
                ddlLab.SelectedValue = null;
                ddlLab.ClearSelection();
                ddlLab.DataTextField = "Key";
                ddlLab.DataValueField = "Value";
                ddlLab.DataSource = _labData;
                ddlLab.DataBind();
            }
        }

        public string LabSelected
        {
            get { return ddlLab.SelectedValue.Substring(1); }
            set { ddlLab.SelectedValue = value; }
        }

        private DataTable _locationData;

        public DataTable LocationData
        {
            get { return _locationData; }
            set
            {
                _locationData = value;
                ddlDeployLocation.Items.Clear();
                ddlDeployLocation.DataSource = _locationData;
                ddlDeployLocation.DataTextField = "TheaterCode";
                ddlDeployLocation.DataValueField = "TheaterCode";
                ddlDeployLocation.DataBind();
            }
        }

        public string LocationSelected
        {
            get { return ddlDeployLocation.SelectedValue; }
            set { ddlDeployLocation.SelectedValue = value; }
        }

        public int Cases
        {
            get { return SrtsExtender.GetIntVal(tbCases.Text); }
            set { tbCases.Text = value.ToString(); }
        }

        public int Pairs
        {
            get { return SrtsExtender.GetIntVal(tbPair.Text); }
            set { tbPair.Text = value.ToString(); }
        }

        public DateTime? FOCDate
        {
            get { return Session["FOCDate"] as DateTime?; }
            set
            {
                Session["FOCDate"] = value;
                lblFOCDate.Text = value == null ? "None Found" : value.ToMilDateString();
            }
        }

        public string StatusSelected
        {
            get { return "x"; }
        }

        public string JustificationInfo
        {
            get { return tbComment.Text; }
            set { tbComment.Text = value; }
        }

        #endregion Misc Accessors

        public string LastFocOrderNumber
        {
            get
            {
                return Session["LastFocOrderNumber"].ToString();
            }
            set
            {
                Session["LastFocOrderNumber"] = value;
            }
        }

        public DateTime? CurrentFocDate
        {
            get
            {
                return Session["CurrentFocDate"] as DateTime?;
            }
            set
            {
                Session["CurrentFocDate"] = value;
            }
        }

        public string LinkedID
        {
            get
            {
                return Session["LinkedId"].ToString();
            }
            set
            {
                Session["LinkedId"] = value;
                this.tbPair.Enabled = String.IsNullOrEmpty(value);
            }
        }

        protected void ddlLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLab();
        }


        public bool RequiresJustification
        {
            get { return Convert.ToBoolean(ViewState["RequiresJustification"]); }
            set { ViewState["RequiresJustification"] = value; }
        }
    }
}