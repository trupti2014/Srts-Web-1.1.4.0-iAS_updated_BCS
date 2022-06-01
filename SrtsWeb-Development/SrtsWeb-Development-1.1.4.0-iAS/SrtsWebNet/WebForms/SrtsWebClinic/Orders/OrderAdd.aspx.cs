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
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    public partial class OrderAdd : PageBase, IOrderAddView, ISiteMapResolver
    {
        private OrderAddPresenter _presenter;

        public OrderAdd()
        {
            _presenter = new OrderAddPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (!Page.IsPostBack)
            {
                // Get the logged in users id from the aspnet profile
                this.mySession.MyIndividualID = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).Personal.IndividualId;
                _presenter.InitView();

                IsComplete = true;

                CurrentModule("Manage Patients - Add Patient Order");

                litContentTop_Title_Right.Text = string.Format("{0} - DOB: {1}", mySession.Patient.Individual.NameLFMi, mySession.Patient.Individual.DateOfBirth != null ? mySession.Patient.Individual.DateOfBirth.Value.ToShortDateString() : string.Empty);
                litBoS.Text = string.Format("Branch: {0} | Rank: {1} | Status: {2}", mySession.Patient.Individual.BOSDescription != null ? mySession.Patient.Individual.BOSDescription : string.Empty, mySession.Patient.Individual.Rank != null ? mySession.Patient.Individual.Rank : string.Empty, mySession.Patient.Individual.StatusDescription != null ? mySession.Patient.Individual.StatusDescription : string.Empty);

                tbODSegHeight.Text = "";
                tbODSegHeight.Enabled = false;
                tbODSegHeight.BorderColor = System.Drawing.Color.LightGray;
                tbOSSegHeight.Text = "";
                tbOSSegHeight.Enabled = false;
                tbOSSegHeight.BorderColor = System.Drawing.Color.LightGray;
                //tbLab.BorderColor = System.Drawing.Color.LightGray;

                SetAddressRbl();

                if (PageWarnings.Count > 0)
                {
                    divAddOrderWarnings.Visible = true;
                    foreach (string item in PageWarnings)
                    {
                        divAddOrderWarnings.InnerHtml = divAddOrderWarnings.InnerHtml + "<p>" + item + "</p><br />";
                    }
                }
                else
                {
                    divAddOrderWarnings.Visible = false;
                }

                if (!string.IsNullOrEmpty(Message))
                {
                    ShowMessage();
                    btnAdd.Enabled = false;
                }
            }

            PageTitle = mySession.AddOrEdit;
            Master.CurrentModuleTitle = "Add Patient Order";
        }

        public string PageTitle
        {
            set { mySession.MainContentTitle = string.Format("{0} Patient Order", value); }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Orders/OrderAdd.aspx", "Add Patient Order");
            child.ParentNode = parent;
            return child;
        }

        private void CheckJustifications()
        {
            // Material Justification
            var justMat = false;

            if (this.ddlMaterial.Items.Count.Equals(0) || this.ddlMaterial.Items.Count.Equals(1))
                justMat = false;
            else
                justMat = !String.IsNullOrEmpty(this.MaterialSelected) && !this.MaterialSelected.Equals("PLAS");

            divOrderJust.Visible = justMat;
            divMaterialJust.Visible = justMat;
            if (!justMat) this.MaterialJust = String.Empty;

            // FOC Justification
            var f = this.FrameListData.Where(x => x.FrameCode == this.FrameSelected).FirstOrDefault();
            if (f == null || !f.IsFoc || !this.RequiresJustification)
            {
                divFocJust.Visible = false;
                this.FocJust = String.Empty;
                return;
            }

            divOrderJust.Visible = true;
            divFocJust.Visible = true;
        }

        private void SetPriority()
        {
            _presenter.SetEligibilityVisibility();
            _presenter.FillFrameData();
            _presenter.SetLab();

            CheckJustifications();
        }

        //private void SetLab()
        //{
        //    // If the selected frame is part of the CustFramToLabColl variable then set the LabSelected property to the correct lab.
        //    if (OrderEntity.CustFrameToLabColl.ContainsKey(this.ddlFrame.SelectedValue))
        //    {
        //        var sc = new List<String>();
        //        OrderEntity.CustFrameToLabColl.TryGetValues(this.ddlFrame.SelectedValue, out sc);

        //        if (!this.ddlFrame.SelectedValue.ToLower().Equals("5am") && !this.ddlFrame.SelectedValue.ToLower().Equals("5am50")
        //             && !this.ddlFrame.SelectedValue.ToLower().Equals("5am52") && !this.ddlFrame.SelectedValue.ToLower().Equals("5am54"))
        //            this.LabSelected = sc[0];
        //        else
        //        {
        //            var s = sc.FirstOrDefault(x => x == mySession.MySite.MultiPrimary);
        //            if (String.IsNullOrEmpty(s))
        //                this.LabSelected = "MNOST1";
        //            else
        //                this.LabSelected = s;
        //        }

        //        // No matter what lens is selected if the frame is required to go to a special lab then exit method before processing lens ddls.
        //        return;
        //    }

        //    // Instead of the RBL use the value of the selected lens
        //    // if (rblSingleMulti.SelectedValue == "true")
        //    if (this.ddlLens.Items.Count > 0)
        //    {
        //        if (this.ddlLens.SelectedValue.Substring(0, 2).ToLower().Equals("sv"))
        //        {
        //            tbLab.Text = string.IsNullOrEmpty(mySession.MySite.SinglePrimary) ? "MNOST1" : mySession.MySite.SinglePrimary;
        //        }
        //        else
        //        {
        //            tbLab.Text = string.IsNullOrEmpty(mySession.MySite.MultiPrimary) ? "MNOST1" : mySession.MySite.MultiPrimary;
        //        }
        //    }
        //}

        private void SetAddressRbl()
        {
            if (_presenter.HasAddress()) return;
            this.rblShipTo.SelectedValue = "C";
            this.rblShipTo.Enabled = false;
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
            //if (this.ddlPriority.SelectedValue.Equals("F"))
            //{
            //    if (this.RequiresJustification)
            //    {
            //        if (!CheckForFocJustification())
            //        {
            //            string Number = String.Empty;
            //            // Do a check to find the 'FOC:' text tag in the comment fields
            //            if (this.Comment1.Contains("FOC:")) Number = "1";
            //            if (this.Comment2.Contains("FOC:")) Number = "2";
            //            /*if (this.Comment1.Length < 60)
            //            {
            //                this.Comment1 = this.Comment1.Length > 0 ? String.Format("{0}  FOC:", this.Comment1) : "FOC:";
            //                Number = "1";
            //            }
            //            else
            //            {
            //                this.Comment2 = this.Comment2.Length > 0 ? String.Format("{0}  FOC:", this.Comment2) : "FOC:";
            //                Number = "2";
            //            }*/
            //            Message = String.Format("FOC was used within the previous year, please provide your justification below in Comment {0} field, after the text \"FOC:\".", Number);
            //            ShowMessage();
            //            return;
            //        }
            //    }
            //}
            if (!Page.IsValid) return;

            Message = string.Empty;
            IsComplete = true;

            //Session["Tech_ID"] = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).Personal.IndividualId;
            _presenter.SaveData();
            //Session.Remove("Tech_ID");

            if (!string.IsNullOrEmpty(Message))
            {
                ShowMessage();
            }
            else
            {
                mySession.AddOrEdit = "EDIT";
                Response.Redirect("../Patients/PatientDetails.aspx?tab=2");
            }
        }

        protected void btnIncomplete_Click(object sender, EventArgs e)
        {
            Message = string.Empty;
            IsComplete = false;
            Session["Tech_ID"] = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).Personal.IndividualId;
            _presenter.SaveIncomplete();
            Session.Remove("Tech_ID");
            if (!string.IsNullOrEmpty(Message))
            {
                ShowMessage();
            }
            else
            {
                Response.Redirect("../Patients/PatientDetails.aspx?tab=2");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Patients/PatientDetails.aspx?tab=0");
        }

        #region PageEvents

        protected void ddlPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPriority();
            ddlFrame.Focus();
        }

        protected void ddlFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.FillItemsData(this.FrameSelected, String.Format("000000B{0}", this.PrioritySelected));
            ddlColor.Focus();
            _presenter.SetLab();

            CheckJustifications();
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
            ddlTint.Focus();

            _presenter.SetLab();
        }

        protected void ddlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckJustifications();
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

        protected void ValidateFocJust(object source, ServerValidateEventArgs args)
        {
            args.Value.Trim();
            if (args.Value.Length > 0)
            {
                if (!(args.IsValid = args.Value.ValidateCommentFormat()))
                {
                    args.IsValid = false;
                    cvFocJust.ErrorMessage = "Invalid characters in FOC Justification";
                }
            }
            else
            {
                args.IsValid = false;
                cvFocJust.ErrorMessage = "FOC Justification is required";
            }
        }

        protected void ValidateMaterialJust(object source, ServerValidateEventArgs args)
        {
            args.Value.Trim();
            if (args.Value.Length > 0)
            {
                if (!(args.IsValid = args.Value.ValidateCommentFormat()))
                {
                    args.IsValid = false;
                    cvMaterialJust.ErrorMessage = "Invalid characters in Material Justification";
                }
            }
            else
            {
                args.IsValid = false;
                cvMaterialJust.ErrorMessage = "Material Justification is required";
            }
        }

        //private bool CheckForFocJustification()
        //{
        //    String FOC = "FOC:";
        //    if (this.Comment1.IsTextAfter(FOC)) return true;
        //    if (this.Comment2.IsTextAfter(FOC)) return true;
        //    return false;
        //}

        #endregion Input Validators

        #region Accessors

        #region Order Accessors

        public bool IsMultiVision
        {
            get
            {
                if (this.ddlLens.SelectedValue.Equals("X")) return false;
                return this.ddlLens.Items.Count > 0 ? !this.ddlLens.SelectedValue.Substring(0, 2).ToLower().Equals("sv") : true;
            }
            set { }
        }

        public bool RequiresJustification
        {
            get { return Convert.ToBoolean(ViewState["RequiresJustification"]); }
            set { ViewState["RequiresJustification"] = value; }
        }

        public string FocJust
        {
            get { return tbFocJust.Text.Trim(); }
            set { this.tbFocJust.Text = value; }
        }

        public string MaterialJust
        {
            get { return tbMaterialJust.Text.Trim(); }
            set { this.tbMaterialJust.Text = value; }
        }

        public bool IsShipToPatient
        {
            get { return this.rblShipTo.SelectedValue == "C" ? false : true; }
            set { rblShipTo.SelectedValue = value.Equals(false) ? "C" : "P"; }
        }

        public List<PersonnelEntity> DoctorData
        {
            set
            {
                ddlVerification.Items.Clear();
                ddlVerification.DataSource = value;
                ddlVerification.DataBind();
                ddlVerification.Items.Insert(0, "-Select-");
                ddlVerification.SelectedIndex = 0;
            }
        }

        public int DoctorSelected
        {
            get { return SrtsExtender.GetIntVal(ddlVerification.SelectedValue); }
            set
            {
                if (ddlVerification.Items.FindByValue(value.ToString()) == null) return;
                ddlVerification.SelectedValue = value.ToString();
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

        public List<FrameEntity> FrameListData
        {
            get
            {
                return ViewState["FrameListData"] as List<FrameEntity>;
            }
            set
            {
                ViewState["FrameListData"] = value;
            }
        }

        public DataTable FrameData
        {
            set
            {
                ddlFrame.Items.Clear();
                ddlFrame.DataSource = value;
                ddlFrame.DataBind();
            }
        }
        private String _FrameSelected;
        public string FrameSelected
        {
            get { return ddlFrame.SelectedValue; }
            set
            {
                if (ddlFrame.Items.FindByValue(value) == null) return;
                _FrameSelected = value;
                ddlFrame.SelectedValue = _FrameSelected;
            }
        }

        public List<FrameItemEntity> ColorData
        {
            set
            {
                ddlColor.Items.Clear();
                ddlColor.DataSource = value;
                ddlColor.DataBind();

                var i = ddlColor.Items.IndexOf(ddlColor.Items.FindByValue("BLK"));

                if (i != -1)
                {
                    ddlColor.SelectedIndex = i;
                }
            }
        }
        public string ColorSelected
        {
            get { return ddlColor.SelectedValue; }
            set
            {
                if (ddlColor.Items.FindByValue(value) == null) return;
                ddlColor.SelectedValue = value;
            }
        }

        public List<FrameItemEntity> EyeData
        {
            set
            {
                ddlEye.Items.Clear();
                ddlEye.DataSource = value;
                ddlEye.DataBind();
            }
        }
        public string EyeSelected
        {
            get { return ddlEye.SelectedValue; }
            set
            {
                if (ddlEye.Items.FindByValue(value) == null) return;
                ddlEye.SelectedValue = value;
            }
        }

        public List<FrameItemEntity> BridgeData
        {
            set
            {
                ddlBridge.Items.Clear();
                ddlBridge.DataSource = value;
                ddlBridge.DataBind();
            }
        }
        public string BridgeSelected
        {
            get { return ddlBridge.SelectedValue; }
            set
            {
                if (ddlBridge.Items.FindByValue(value) == null) return;
                ddlBridge.SelectedValue = value;
            }
        }

        public List<FrameItemEntity> TempleData
        {
            set
            {
                ddlTemple.Items.Clear();
                ddlTemple.DataSource = value;
                ddlTemple.DataBind();
            }
        }
        public string TempleSelected
        {
            get { return ddlTemple.SelectedValue; }
            set
            {
                if (ddlTemple.Items.FindByValue(value) == null) return;
                ddlTemple.SelectedValue = value;
            }
        }

        public List<FrameItemEntity> LensData
        {
            set
            {
                ddlLens.Items.Clear();
                ddlLens.DataSource = value;
                ddlLens.DataBind();

                var i = ddlLens.Items.IndexOf(ddlLens.Items.FindByValue("SVD"));
                if (i != -1)
                {
                    ddlLens.SelectedIndex = i;
                }
            }
        }
        public string LensSelected
        {
            get { return ddlLens.SelectedValue; }
            set
            {
                if (ddlLens.Items.FindByValue(value) == null) return;
                ddlLens.SelectedValue = value;
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

        public List<FrameItemEntity> MaterialData
        {
            set
            {
                ddlMaterial.Items.Clear();
                ddlMaterial.DataSource = value;
                ddlMaterial.DataBind();

                var i = ddlMaterial.Items.IndexOf(ddlMaterial.Items.FindByValue("PLAS"));

                if (i != -1)
                {
                    ddlMaterial.SelectedIndex = i;
                    ddlMaterial_SelectedIndexChanged(this, EventArgs.Empty);
                }
            }
        }
        public string MaterialSelected
        {
            get { return ddlMaterial.SelectedValue; }
            set
            {
                if (ddlMaterial.Items.FindByValue(value) == null) return;
                ddlMaterial.SelectedValue = value;
            }
        }

        public List<FrameItemEntity> TintData
        {
            set
            {
                ddlTint.Items.Clear();
                ddlTint.DataSource = value;
                ddlTint.DataBind();

                var i = ddlTint.Items.IndexOf(ddlTint.Items.FindByValue("CL"));
                if (i != -1)
                {
                    ddlTint.SelectedIndex = i;
                }
            }
        }
        public string TintSelected
        {
            get { return ddlTint.SelectedValue; }
            set
            {
                if (ddlTint.Items.FindByValue(value) == null) return;
                ddlTint.SelectedValue = value;
            }
        }

        public string LastFocOrderNumber
        {
            get;
            set;
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

                    IsMonoCalculation = false;
                }
                else
                {
                    tablePDTotal.Visible = false;
                    tablePDTotalNear.Visible = false;

                    tablePDOD.Visible = true;
                    tablePDODNear.Visible = true;

                    tablePDOS.Visible = true;
                    tablePDOSNear.Visible = true;

                    IsMonoCalculation = true;
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

        private bool _isComplete;

        public bool IsComplete
        {
            get { return _isComplete; }
            set { _isComplete = value; }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public List<OrderPriorityEntity> PriorityList
        {
            set
            {
                ddlPriority.Items.Clear();
                ddlPriority.DataSource = value;
                ddlPriority.DataTextField = "OrderPriorityText";
                ddlPriority.DataValueField = "OrderPriorityValue";
                ddlPriority.DataBind();
            }
        }

        public List<KeyValuePair<string, string>> PriorityData
        {
            set
            {
                ddlPriority.Items.Clear();
                ddlPriority.DataSource = value;
                ddlPriority.DataTextField = "Value";
                ddlPriority.DataValueField = "Key";
                ddlPriority.DataBind();
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
            get { return tbComment1.Text.Trim(); }
            set { tbComment1.Text = value.Trim(); }
        }

        public string Comment2
        {
            get { return tbComment2.Text.Trim(); }
            set { tbComment2.Text = value.Trim(); }
        }

        private List<PersonnelEntity> _TechData;

        public List<PersonnelEntity> TechData
        {
            set
            {
                this._TechData = value;
            }
        }

        private int _TechSelected;

        public int TechSelected
        {
            get
            {
                return this._TechSelected;
                //return SrtsExtender.GetIntVal(ddlTechnician.SelectedValue); 
            }
            set
            {
                //ddlTechnician.SelectedValue = value.ToString(); 
                this._TechSelected = value;
                var r = this._TechData.Where(x => x.ID == this._TechSelected).FirstOrDefault();
                this.tbTechnician.Text = r == null || String.IsNullOrEmpty(r.NameLFMi) ? "N/A" : r.NameLFMi;
            }
        }

        public Dictionary<String, String> LabData
        {
            get { return this.ViewState["labData"] as Dictionary<String, String>; }
            set
            {
                this.ViewState.Add("labData", value);

                ddlLab.Items.Clear();
                ddlLab.SelectedIndex = -1;
                ddlLab.SelectedValue = null;
                ddlLab.ClearSelection();
                ddlLab.DataSource = this.LabData;
                ddlLab.DataTextField = "Key";
                ddlLab.DataValueField = "Value";
                ddlLab.DataBind();
            }
        }

        public string LabSelected
        {
            get { return ddlLab.SelectedValue.Remove(0, 1).ToUpper(); }
            set
            {
                if (ddlLab.Items.FindByValue(value) == null) return;
                ddlLab.SelectedValue = value;
            }
        }

        public DataTable LocationData
        {
            set
            {
                ddlDeployLocation.Items.Clear();
                ddlDeployLocation.DataSource = value;
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

        public string FOCDate
        {
            get { return lblFOCDate.Text; }
            set { lblFOCDate.Text = value; }
        }

        public List<string> PageWarnings = new List<string>();

        public string WarningMsg
        {
            set
            {
                PageWarnings.Add(value);
            }
        }

        #endregion Misc Accessors

        protected void ddlLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.SetLab();
        }

        #endregion Accessors
    }
}