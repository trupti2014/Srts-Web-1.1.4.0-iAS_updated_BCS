using SrtsWeb.BusinessLayer.Presenters.GEyes;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.BusinessLayer.Views.GEyes;
using SrtsWeb.CustomErrors;
using SrtsWeb.Entities;
using System;
using System.Data;

namespace GEyes.Forms
{
    public partial class CreateOrder : PageBase, ICreateOrderView
    {
        private CreateOrderPresenter _presenter;

        public CreateOrder()
        {
            _presenter = new CreateOrderPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.InitView();
                _presenter.FillGrids(Session["DeployZipCode"].ToString());
            }
        }

        protected void ddlPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void ddlDeployLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void ddlFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.FillItemsData(myInfo.FrameData);
        }

        protected void ddlColor_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void ddlTint_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            _presenter.SaveData();
            if (!string.IsNullOrEmpty(Message))
            {
                return;
            }
            Response.Redirect("AddressUpdate.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("GEyesHomePage.aspx");
        }

        #region Order Accessors

        public GEyesSession myInfo
        {
            get { return (GEyesSession)Session["MyInfo"]; }
            set { Session["MyInfo"] = value; }
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
                ddlOrderPriority.Items.Clear();
                ddlOrderPriority.DataSource = _priorityData;
                ddlOrderPriority.DataTextField = "Text";
                ddlOrderPriority.DataValueField = "Value";
                ddlOrderPriority.DataBind();
            }
        }

        public string PrioritySelected
        {
            get { return ddlOrderPriority.SelectedValue; }
            set { ddlOrderPriority.SelectedValue = value; }
        }

        public string Comment
        {
            get { return tbComment.Text; }
            set { tbComment.Text = value; }
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
            get { return tbCases.Text.ToInt32(); }
            set { tbCases.Text = value.ToString(); }
        }

        public int Pairs
        {
            get { return tbPair.Text.ToInt32(); }
            set { tbPair.Text = value.ToString(); }
        }

        #endregion Order Accessors
    }
}