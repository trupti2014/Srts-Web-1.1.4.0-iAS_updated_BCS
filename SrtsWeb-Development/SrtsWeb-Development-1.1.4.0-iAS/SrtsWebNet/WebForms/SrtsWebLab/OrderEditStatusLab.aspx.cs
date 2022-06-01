using SrtsWeb;
using SrtsWeb.BusinessLayer.Presenters.Lab;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders;
using SrtsWeb.BusinessLayer.Views.Lab;
using SrtsWeb.CustomErrors;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWebLab
{
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    public partial class OrderEditStatusLab : PageBase, IOrderEditStatusLabView, ISiteMapResolver
    {
        private OrderEditStatusLabPresenter _presenter;

        public OrderEditStatusLab()
        {
            _presenter = new OrderEditStatusLabPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!Page.IsPostBack)
            {
                _presenter.InitView();
                ddlPriority.Focus();
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

            Master.CurrentModuleTitle = string.Empty;
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            else
            {
                if (mySession == null)
                {
                    mySession = new SRTSSession();
                }
                else
                {
                    try
                    {
                        litContentTop_Title_Right.Text = string.Format("{0} - {1}", mySession.MySite.SiteName, mySession.MyClinicCode);
                    }
                    catch (NullReferenceException)
                    {
                        Response.Redirect(FormsAuthentication.LoginUrl);
                    }

                    CurrentModule("Manage Lab Orders");
                    CurrentModule_Sub(string.Empty);
                    CurrentModule_Sub("- View Order Information");
                    BuildPageTitle();

                    BuildPageTitle();
                }
            }
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Manage Lab Orders");
                CurrentModule_Sub(string.Empty);
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebLab/ManageOrders.aspx", "Manage Orders");
            child.ParentNode = parent;
            SiteMapNode child2 = new SiteMapNode(e.Provider, "2", "~/SrtsWebLab/OrderEditStatusLab.aspx", "Order Status");
            child2.ParentNode = parent;
            return child2;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            _presenter.SaveData();
            if (!string.IsNullOrEmpty(Message))
            {
                ShowMessage();
            }
            else
            {
                Response.Redirect("Rejected.aspx");
            }
        }

        private void ShowMessage()
        {
            CustomValidator cv = new CustomValidator();
            cv.IsValid = false;
            cv.ErrorMessage = Message;
            this.Page.Validators.Add(cv);
            return;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Rejected.aspx");
        }

        protected void rblAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatusSelected = rblAction.SelectedValue;
        }

        protected void ddlLab_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        #region Order Accessors

        public string StatusSelected
        {
            get { return rblAction.SelectedValue; }
            set { rblAction.SelectedValue = value; }
        }

        public string JustificationInfo
        {
            get { return lblJustificationInfo.Text; }
            set { lblJustificationInfo.Text = value; }
        }

        public bool IsMultiVision
        {
            get { return Convert.ToBoolean(rblSingleMulti.SelectedValue); }
            set { rblSingleMulti.SelectedValue = value.ToString(); }
        }

        public string Frame
        {
            get { return tbFrameCode.Text; }
            set { tbFrameCode.Text = value; }
        }

        public string Color
        {
            get { return tbFrameColor.Text; }
            set { tbFrameColor.Text = value; }
        }

        public string Eye
        {
            get { return tbEyeSize.Text; }
            set { tbEyeSize.Text = value; }
        }

        public string Bridge
        {
            get { return tbBridgeSize.Text; }
            set { tbBridgeSize.Text = value; }
        }

        public string Temple
        {
            get { return tbTempleSize.Text; }
            set { tbTempleSize.Text = value; }
        }

        public string Lens
        {
            get { return tbLens.Text; }
            set { tbLens.Text = value; }
        }

        public string ODSegHeight
        {
            get { return tbODSegHeight.Text; }
            set { tbODSegHeight.Text = value; }
        }

        public string OSSegHeight
        {
            get { return tbOSSegHeight.Text; }
            set { tbOSSegHeight.Text = value; }
        }

        public string Material
        {
            get { return tbMaterial.Text; }
            set { tbMaterial.Text = value; }
        }

        public string Tint
        {
            get { return tbTint.Text; }
            set { tbTint.Text = value; }
        }

        #endregion Order Accessors

        #region Shipping Accessors

        private int _addressID;

        public int AddressID
        {
            get { return _addressID; }
            set { _addressID = value; }
        }

        public string AddressType
        {
            get { return tbAddressType.Text; }
            set { tbAddressType.Text = value; }
        }

        public string Address1
        {
            get { return tbStreet1.Text; }
            set { tbStreet1.Text = value; }
        }

        public string Address2
        {
            get { return tbStreet2.Text; }
            set { tbStreet2.Text = value; }
        }

        public string ZipCode
        {
            get { return tbZipCode.Text; }
            set { tbZipCode.Text = value.ToZipCodeDisplay(); }
        }

        public string City
        {
            get { return tbCity.Text; }
            set { tbCity.Text = value; }
        }

        public string AddressState
        {
            get { return tbState.Text; }
            set { tbState.Text = value; }
        }

        public string Country
        {
            get { return tbCountry.Text; }
            set { tbCountry.Text = value; }
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

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private DataTable _priorityData;

        public DataTable PriorityData
        {
            get { return _priorityData; }
            set
            {
                _priorityData = value;
                ddlPriority.Items.Clear();
                ddlPriority.DataSource = _priorityData;
                ddlPriority.DataTextField = "Text";
                ddlPriority.DataValueField = "Value";
                ddlPriority.DataBind();
            }
        }

        public string PrioritySelected
        {
            get { return ddlPriority.SelectedValue; }
            set { ddlPriority.SelectedValue = value; }
        }

        public string OHComment
        {
            get { return tbComment.Text.Trim(); }
            set { tbComment.Text = value.Trim(); }
        }

        private List<SiteCodeEntity> _labData;

        public List<SiteCodeEntity> LabData
        {
            get { return _labData; }
            set
            {
                _labData = value;
                ddlLab.Items.Clear();
                ddlLab.DataTextField = "SiteCombination";
                ddlLab.DataValueField = "SiteCode";
                ddlLab.DataSource = _labData.Select(x => new { x.SiteCombination, x.SiteCode }).Distinct();
                ddlLab.DataBind();
                ddlLab.Enabled = false;
            }
        }

        public string LabSelected
        {
            get { return ddlLab.SelectedValue; }
            set { ddlLab.SelectedValue = value; }
        }

        public string Location
        {
            get { return tbLocation.Text; }
            set { tbLocation.Text = value; }
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
            get { return tbFOCDate.Text; }
            set { tbFOCDate.Text = value; }
        }

        #endregion Misc Accessors

        public bool RequiresJustification
        {
            get { return Convert.ToBoolean(ViewState["RequiresJustification"]); }
            set { ViewState["RequiresJustification"] = value; }
        }
    }
}