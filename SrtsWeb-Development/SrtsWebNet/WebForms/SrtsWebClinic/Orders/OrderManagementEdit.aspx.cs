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
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    public partial class OrderManagementEdit : PageBase, IOrderManagementEditView, ISiteMapResolver
    {
        private OrderManagementEditPresenter _presenter;

        public OrderManagementEdit()
        {
            _presenter = new OrderManagementEditPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string selectedOrderNumber;

                if (!String.IsNullOrEmpty(Request.QueryString["id"].ToString()))
                {
                    selectedOrderNumber = Request.QueryString["id"].ToString();
                    mySession.tempOrderID = selectedOrderNumber;
                }
                mySession.AddOrEdit = "EDIT";

                this.mySession.MyIndividualID = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).Personal.IndividualId;

                _presenter.InitView();
                ddlPriority.SelectedValue = mySession.SelectedOrder.Demographic.ToOrderPriorityValue();
                ddlPriority.Focus();

                CheckJustifications();

                SetAddressRbl();

                //var isVisible = OrderStatus.ToLower().Equals("incomplete order");
                this.tbClinicJust.Visible = OrderStatusID.Equals(3);
                this.lblClinicJust.Visible = OrderStatusID.Equals(3);
                this.divLabComment.Visible = OrderStatusID.Equals(3);
                this.lblLabCommentText.Visible = OrderStatusID.Equals(3);

                if (OrderStatusID.Equals(15))
                {
                    btnSubmittop.Text = "Submit Order";
                    btnSubmitBottom.Text = "Submit Order";
                }

                //this.cbApproved.Visible = OrderStatusID.Equals(16);

                this.cbReclaimed.Visible = OrderStatusID.Equals(11);

                if (mySession != null)
                {
                    mySession.TempID = 0;
                    mySession.ReturnURL = "PatientDetails.aspx";
                    litContentTop_Title_Right.Text = string.Format("{0} - DOB: {1}", mySession.Patient.Individual.NameLFMi, mySession.Patient.Individual.DateOfBirth != null ? mySession.Patient.Individual.DateOfBirth.Value.ToShortDateString() : string.Empty);
                    litBoS.Text = string.Format("Branch: {0} | Rank: {1} | Status: {2}", mySession.Patient.Individual.BOSDescription != null ? mySession.Patient.Individual.BOSDescription : string.Empty, mySession.Patient.Individual.Rank != null ? mySession.Patient.Individual.Rank : string.Empty, mySession.Patient.Individual.StatusDescription != null ? mySession.Patient.Individual.StatusDescription : string.Empty);
                    litName.Text = string.Format("Order Number: {0} | Order Status: {1}", mySession.SelectedOrder.OrderNumber.PadRight(25), OrderStatus);
                }
            }
            else
            {
                WebControl wcICausedPostBack = (WebControl)GetControlThatCausedPostBack(sender as Page);
                int indx = wcICausedPostBack.TabIndex;
                var ctrl = from control in wcICausedPostBack.Parent.Controls.OfType<WebControl>()
                           where control.TabIndex > indx
                           select control;
                ctrl.DefaultIfEmpty(wcICausedPostBack).First().Focus();
            }

            switch (mySession.AddOrEdit)
            { // Submit, Dupe, Delete (h), Reprint, Cancel
                case "VIEW":
                    btnSubmitBottom.Visible = false;
                    btnSubmittop.Visible = false;
                    btnReprintDD771top.Visible = true;
                    btnReprintDD771Bottom.Visible = true;
                    break;

                case "ADD":
                    //btnBottomDupe.Visible = false;
                    //btnTopDupe.Visible = false;
                    btnSubmitBottom.Visible = true;
                    btnSubmittop.Visible = true;
                    btnReprintDD771top.Visible = true;
                    btnReprintDD771Bottom.Visible = true;
                    break;

                default:
                    //btnBottomDupe.Visible = true;
                    //btnTopDupe.Visible = true;
                    btnSubmitBottom.Visible = true;
                    btnSubmittop.Visible = true;
                    btnReprintDD771top.Visible = true;
                    btnReprintDD771Bottom.Visible = true;

                    if (!(mySession.SelectedOrder.ClinicSiteCode == mySession.MySite.SiteCode))
                    {
                        btnSubmittop.Enabled = false;
                        btnSubmitBottom.Enabled = false;
                        btnDeleteTop.Enabled = false;
                        btnDeleteBottom.Enabled = false;
                    }
                    break;
            }

            if (OrderStatusID != 1 &&   // Clinic Created
                OrderStatusID != 3 &&   // Lab Rejected
                OrderStatusID != 9 &&   // Clinic ReSubmitted Order
                OrderStatusID != 15 &&  // Incomplete
                OrderStatusID != 16)    // Hold
            {
                BuildPageTitle("Manage Orders - View Patient Order");
                SetReadOnly();
                // Ensure that the order cannot be saved
                //btnSubmitBottom.Visible = false;
                //btnSubmittop.Visible = false;
                btnReprintDD771top.Visible = true;
                btnReprintDD771Bottom.Visible = true;
                btnReprintDD771top.Enabled = true;
                btnReprintDD771Bottom.Enabled = true;
            }
            else
            {
                BuildPageTitle("Manage Orders - Update Patient Order");

                // Set button states
                //btnBottomDupe.Visible = false;
                //btnTopDupe.Visible = false;
                btnSubmitBottom.Visible = true;
                btnSubmittop.Visible = true;
                btnDeleteTop.Visible = mySession.SelectedOrder.ClinicSiteCode.Equals(mySession.MySite.SiteCode);
                btnDeleteTop.Enabled = mySession.SelectedOrder.ClinicSiteCode.Equals(mySession.MySite.SiteCode);
                btnDeleteBottom.Visible = mySession.SelectedOrder.ClinicSiteCode.Equals(mySession.MySite.SiteCode);
                btnDeleteBottom.Enabled = mySession.SelectedOrder.ClinicSiteCode.Equals(mySession.MySite.SiteCode);

                if (!mySession.SelectedOrder.ClinicSiteCode.Equals(mySession.MySite.SiteCode))
                {
                    ddlLab.Enabled = false;
                }

                if (!string.IsNullOrEmpty(tbFocJust.Text))
                {
                    divOrderJust.Visible = true;
                    divFocJust.Visible = true;
                }

                if (!string.IsNullOrEmpty(tbMaterialJust.Text))
                {
                    divOrderJust.Visible = true;
                    divMaterialJust.Visible = true;
                }

                if (!(User.IsInRole("clinictech")))
                {
                    SetReadOnly();
                }
            }

            // Incomplete cannot be duplicated
            if (OrderStatusID.Equals(15))
            {
                this.btnTopDupe.Visible = false;
                this.btnBottomDupe.Visible = false;
            }

        }

        private void BuildPageTitle(string title)
        {
            try
            {
                CurrentModule(title);
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, "");
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Manage Orders - View/Update Patient Orders -");
                CurrentModule_Sub(string.Empty);
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Patients/PatientDetails.aspx?tab=2", "Patient Details");
            child.ParentNode = parent;
            SiteMapNode child2 = new SiteMapNode(e.Provider, "3", "~/SrtsWebClinic/Orders/OrderManagementEdit.aspx", "Edit Patient Order");
            child2.ParentNode = child;
            return child2;
        }

        public string PageTitle
        {
            set { mySession.MainContentTitle = string.Format("{0} Patient Order", value); }
        }

        private void CheckJustifications()
        {
            // Material Justification
            var justMat = false;

            if (this.ddlMaterial.Items.Count.Equals(0) || this.ddlMaterial.Items.Count.Equals(1))
                justMat = false;
            else
                justMat = !MaterialSelected.Equals("PLAS");

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
            _presenter.FillFrameData(this.PrioritySelected);
            _presenter.SetLab();

            CheckJustifications();
        }

        private void SetAddressRbl()
        {
            if (_presenter.HasAddress()) return;
            this.rblShipTo.SelectedValue = "C";
            this.rblShipTo.Enabled = false;
        }

        //private bool CheckForFocJustification()
        //{
        //    String FOC = "FOC:";
        //    if (this.Comment1.IsTextAfter(FOC)) return true;
        //    if (this.Comment2.IsTextAfter(FOC)) return true;
        //    if (this.Comment1.Contains(FOC)) return false;
        //    return false;
        //}

        private void SetReadOnly()
        {
            this.Controls.SetControlStateBtn(false);
            this.Controls.SetControlStateTbDdlRbl(false);
            btnTopDupe.Enabled = true;
            btnBottomDupe.Enabled = true;
            btnCanceltop.Enabled = true;
            btnCancelBottom.Enabled = true;
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
            //    if (_presenter.CheckFOCDate())
            //    {
            //        if (!CheckForFocJustification())
            //        {
            //            string Number = String.Empty;
            //            // Do a check to find the 'FOC:' text tag in the comment fields
            //            if (this.Comment1.Contains("FOC:")) Number = "1";
            //            if (this.Comment2.Contains("FOC:")) Number = "2";

            //            Message = String.Format("FOC was used within the previous year, please provide your justification below in Comment {0} field, after the text \"FOC:\".", Number);
            //            ShowMessage();
            //            return;
            //        }
            //    }
            //}

            if (!Page.IsValid) return;

            mySession.AddOrEdit = "EDIT";

            //if (IsApproved)
            //{
            //    _presenter.UpdateApproved();
            //}

            //Session["Tech_ID"] = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).Personal.IndividualId;
            _presenter.SaveData();
            //Session.Remove("Tech_ID");

            if (!string.IsNullOrEmpty(Message))
            {
                ShowMessage();
                return;
            }
            else
            {
                Response.Redirect("../Patients/PatientDetails.aspx?tab=2");
            }
        }

        protected void btnAddDupe_Click(object sender, EventArgs e)
        {
            mySession.AddOrEdit = "DUPE";
            Session["Tech_ID"] = mySession.MyIndividualID;// SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).Personal.IndividualId;
            _presenter.SaveData();
            Session.Remove("Tech_ID");
            if (!string.IsNullOrEmpty(Message))
            {
                ShowMessage();
                return;
            }
            else
            {
                Response.Redirect("../Patients/PatientDetails.aspx?tab=2");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Patients/PatientDetails.aspx?tab=2");
        }

        protected void bDeleteButton_Click(object sender, EventArgs e)
        {
            _presenter.Delete();
            Response.Redirect("../Patients/PatientDetails.aspx?tab=2");
        }

        protected void btnReprintDD771_Click(object sender, EventArgs e)
        {
            string newURL = string.Format("~/Reports/rptViewerTemplate.aspx?id=/ReprintForm771a&title=Reprint DD Form 771&isreprint=true&ordernumber={0}", mySession.SelectedOrder.OrderNumber);
            Response.Redirect(newURL);
        }


        #region PageEvents

        protected void ddlPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySession.AddOrEdit = "";
            SetPriority();
            ddlFrame.Focus();
        }

        protected void ddlFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.FillItemsData(this.FrameSelected, String.Format("000000B{0}", this.PrioritySelected));
            _presenter.SetLab();

            CheckJustifications();
        }

        protected void ddlLens_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LensSelected = this.ddlLens.SelectedValue;

            //string SelectedLens;

            //if (ddlLens.SelectedValue.Length > 1)
            //{
            //    SelectedLens = ddlLens.SelectedValue.ToString().Substring(0, 2).ToLower();
            //}
            //else
            //{
            //    SelectedLens = ddlLens.SelectedValue.ToString().ToLower();
            //}

            //if (SelectedLens == "sv" || SelectedLens == "x")
            //{
            //    tbODSegHeight.Text = "";
            //    tbODSegHeight.Enabled = false;
            //    tbODSegHeight.BorderColor = System.Drawing.Color.LightGray;
            //    tbOSSegHeight.Text = "";
            //    tbOSSegHeight.Enabled = false;
            //    tbOSSegHeight.BorderColor = System.Drawing.Color.LightGray;
            //}
            //else
            //{
            //    tbODSegHeight.Enabled = true;
            //    tbODSegHeight.BorderColor = System.Drawing.ColorTranslator.FromHtml("#E4CFAC");
            //    tbOSSegHeight.Enabled = true;
            //    tbOSSegHeight.BorderColor = System.Drawing.ColorTranslator.FromHtml("#E4CFAC");
            //}

            _presenter.SetLab();
        }

        protected void ddlLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.SetLab();
        }

        protected void ddlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckJustifications();

            /*if (ddlMaterial.SelectedValue == "PLAS")
            {
                if (divMaterialJust.Visible == true) divMaterialJust.Visible = false;
                if (!this.RequiresMatJustification) divOrderJust.Visible = false;
                tbMaterialJust.Text = string.Empty;
                return;
            }
            if (divOrderJust.Visible == false) divOrderJust.Visible = true;
            if (divMaterialJust.Visible == false) divMaterialJust.Visible = true;*/
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

        #endregion Input Validators

        #region Order Accessors

        public bool IsMultiVision
        {
            get
            {
                if (this.ddlLens.SelectedValue.Equals("X")) return false;
                return !this.ddlLens.SelectedValue.Substring(0, 2).ToLower().Equals("sv");
            }
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
            set
            {
                if (ddlFrame.Items.FindByValue(value) == null) return;
                ddlFrame.SelectedValue = value;
            }
        }

        private List<FrameItemEntity> _colorData;
        public List<FrameItemEntity> ColorData
        {
            get { return _colorData; }
            set
            {
                _colorData = value;
                ddlColor.Items.Clear();
                ddlColor.DataSource = _colorData;
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

        private List<FrameItemEntity> _eyeData;
        public List<FrameItemEntity> EyeData
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
            set
            {
                if (ddlEye.Items.FindByValue(value) == null) return;
                ddlEye.SelectedValue = value;
            }
        }

        private List<FrameItemEntity> _bridgeData;
        public List<FrameItemEntity> BridgeData
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
            set
            {
                if (ddlBridge.Items.FindByValue(value) == null) return;
                ddlBridge.SelectedValue = value;
            }
        }

        private List<FrameItemEntity> _templeData;
        public List<FrameItemEntity> TempleData
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
            set
            {
                if (ddlTemple.Items.FindByValue(value) == null) return;
                ddlTemple.SelectedValue = value;
            }
        }

        public List<FrameItemEntity> LensData
        {
            get { return this.ViewState["lensData"] as List<FrameItemEntity>; }
            set
            {
                this.ViewState.Add("lensData", value);
                ddlLens.Items.Clear();
                ddlLens.DataSource = this.LensData;
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
                if (value.ToLower().Equals("x") || value.Substring(0, 2).ToLower().Equals("sv"))
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

        private List<FrameItemEntity> _tintData;
        public List<FrameItemEntity> TintData
        {
            get { return _tintData; }
            set
            {
                _tintData = value;
                ddlTint.Items.Clear();
                ddlTint.DataSource = _tintData;
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

        public bool RequiresJustification
        {
            get { return Convert.ToBoolean(ViewState["RequiresJustification"]); }
            set { ViewState["RequiresJustification"] = value; }
        }

        public string FocJust
        {
            get { return tbFocJust.Text.Trim(); }
            set { tbFocJust.Text = value; }
        }

        public string MaterialJust
        {
            get { return tbMaterialJust.Text.Trim(); }
            set { tbMaterialJust.Text = value; }
        }

        public string LabComment
        {
            get
            {
                return this.lblLabCommentText.Text;
            }
            set
            {
                this.lblLabCommentText.Text = value;
            }
        }

        public string ClinicJust
        {
            get
            {
                return this.tbClinicJust.Text;
            }
            set
            {
                this.tbClinicJust.Text = value;
            }
        }

        public List<OrderStateEntity> OrderStateHistory
        {
            set
            {
                this.gvStatusHistory.DataSource = value;
                this.gvStatusHistory.DataBind();
            }
        }

        #endregion Order Accessors

        #region Shipping Accessors

        public bool IsShipToPatient
        {
            get { return this.rblShipTo.SelectedValue == "C" ? false : true; }
            set { this.rblShipTo.SelectedValue = value.Equals(true) ? "P" : "C"; }
        }

        #endregion Shipping Accessors

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

                if (this.cbReclaimed.Visible && this.cbReclaimed.Checked) return 17;

                Int32.TryParse(ViewState["orderstatusid"].ToString(), out o);
                return o;
            }
            set
            {
                _orderStatusID = value;
                this.cbReclaimed.Visible = _orderStatusID.Equals(11) || _orderStatusID.Equals(17);
                this.cbReclaimed.Checked = _orderStatusID.Equals(17);
                ViewState.Add("orderstatusid", _orderStatusID);
            }
        }

        private string _orderStatus;

        public string OrderStatus
        {
            get { return _orderStatus; }
            set { _orderStatus = value; }
        }

        public bool IsApproved
        {
            get
            {
                return true;
                //if (cbApproved.Checked)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            set
            {
                var a = value;
                //if (value)
                //{
                //    cbApproved.Checked = true;
                //}
                //else
                //{
                //    cbApproved.Checked = false;
                //}
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
                //ddlPriority.Items.Remove(ddlPriority.Items.FindByValue("N"));
                //ddlPriority.Items.Insert(0, "-Select-");
                //ddlPriority.SelectedIndex = 0;
            }
        }

        public string PrioritySelected
        {
            get { return ddlPriority.SelectedValue; }
            set
            {
                if (ddlPriority.Items.FindByValue(value) == null) return;
                ddlPriority.SelectedValue = value;
            }
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

        private List<PersonnelEntity> _techData;

        public List<PersonnelEntity> TechData
        {
            get { return _techData; }
            set
            {
                _techData = value;
                //ddlTechnician.Items.Clear();
                //ddlTechnician.DataSource = _techData;
                //ddlTechnician.DataBind();
            }
        }

        private int _TechSelected;

        public int TechSelected
        {
            get
            {
                //return SrtsExtender.GetIntVal(ddlTechnician.SelectedValue);
                return this._TechSelected;
            }
            set
            {
                //ddlTechnician.SelectedValue = value.ToString();
                this._TechSelected = value;
                var r = this._techData.Where(x => x.ID == this._TechSelected).FirstOrDefault();
                this.tbTechnician.Text = r == null || String.IsNullOrEmpty(r.NameLFMi) ? "N/A" : r.NameLFMi;
            }
        }

        //private Dictionary<String, String> _labData;

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
            set
            {
                if (ddlDeployLocation.Items.FindByValue(value) == null) return;
                ddlDeployLocation.SelectedValue = value;
            }
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
            get { return Session["FOCDate"] as DateTime?; }//tbFOCDate.Text; }
            set
            {
                Session["FOCDate"] = value;
                lblFOCDate.Text = value == null ? "None Found" : value.ToMilDateString();
            }
        }

        public string OrderNumber
        {
            get;
            set;
        }

        public string LinkedId
        {
            get { return Session["LinkedID"] == null ? "" : Session["LinkedID"].ToString(); }
            set
            {
                Session["LinkedID"] = value;
                tbPair.Enabled = String.IsNullOrEmpty(value);
            }
        }

        #endregion Misc Accessors
    }
}