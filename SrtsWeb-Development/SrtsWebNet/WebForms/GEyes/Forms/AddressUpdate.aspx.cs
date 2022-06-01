using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.GEyes;
using SrtsWeb.Views.GEyes;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace GEyes.Forms
{
    public partial class AddressUpdate : System.Web.UI.Page, IAddressUpdateView
    {
        private AddressUpdatePresenter _presenter;

        public AddressUpdate()
        {
            _presenter = new AddressUpdatePresenter(this);
        }

        #region LOAD

        protected void Page_Load(object sender, EventArgs e)
        {

            if (myInfo.IsNull())
                Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");

            if (!IsPostBack)
            {
                _presenter.InitView();
                pnlAddress.Visible = true;
                pnlGVAddresses.Visible = false;
                _presenter.GetStates();
                tbZipCode.Text = (string)(Session["DeployZipCode"]).ToString().ToZipCodeDisplay();
                btnNext.Visible = false;
            }
            else
            {
                if (myInfo == null)
                {
                    Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
                }
            }
        }

        #endregion

        #region EVENTS

        protected void lbntAddAddress_Click(object sender, EventArgs e)
        {
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCountry.SelectedItem.Text == "UNITED STATES")
            {
                ddlState.Enabled = true;
            }
            else
            {
                ddlState.SelectedItem.Text = "UNKNOWN";
                ddlState.Enabled = false;
            }
        }


        protected void btnNext_Click(object sender, EventArgs e)
        {
            _presenter.SelectedAddress();
            Response.Redirect("~/WebForms/GEyes/Forms/OrderConfirmation.aspx");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Validated())
            {
                return;
            }
            _presenter.SetOrderAddress();
            if (!string.IsNullOrEmpty(Message))
            {
                ShowMessage();
            }
            else
            {
                Response.Redirect("~/WebForms/GEyes/Forms/OrderConfirmation.aspx");
            }
        }

        #endregion

        #region METHODS

        protected void gvAddresses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int rowID = Convert.ToInt32(gvAddresses.DataKeys[index].Value);
                _presenter.FillAddress(rowID);
                disableTextBoxes();
                btnSubmit.Visible = false;
                btnNext.Visible = true;
                pnlAddress.Visible = true;
                pnlGVAddresses.Visible = false;
                //lblAddrInfo.Text = "You selected the following address.  If this is correct, press the 'Next' button to continue.";
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
            tbAddress1.Text = string.Empty;
            tbAddress2.Text = string.Empty;

            tbZipCode.Text = string.Empty;

            Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
        }

        /// <summary>
        /// Return Error Message for Invalid Fields on Form
        /// </summary>
        protected Boolean Validated()
        {
            try
            {
                var NotValid = false;

                if (ddlState.SelectedValue == "AA" || ddlState.SelectedValue == "AE" || ddlState.SelectedValue == "AP")
                {
                    if (txtCity.Text != "")
                    {
                        if (txtCity.Text.Trim() != "FPO" && txtCity.Text.Trim() != "DPO" && txtCity.Text.Trim() != "APO")
                        {
                            this.hfMsgGE.Value = "City must equal FPO, APO, or DPO when at a AP, AE, AA Location.";

                            return NotValid;
                        }
                    }
                }

                if (ddlState.SelectedValue != "AA" && ddlState.SelectedValue != "AE" && ddlState.SelectedValue != "AP")
                {
                    if (ddlCountry.SelectedValue != "US")
                    {
                        this.hfMsgGE.Value = "Country must be United States whena US address is entered.";
                        return NotValid;
                    }
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        protected void disableTextBoxes()
        {
            tbAddress1.Enabled = false;
            tbAddress2.Enabled = false;

            ddlState.Enabled = false;
            ddlCountry.Enabled = false;
            tbZipCode.Enabled = false;
        }

        protected void enableTextBoxes()
        {
            tbAddress1.Enabled = true;
            tbAddress2.Enabled = true;

            ddlState.Enabled = true;
            ddlCountry.Enabled = true;
            tbZipCode.Enabled = true;
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

        }

        #endregion

        #region IAddressUpdateView Members

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public GEyesSession myInfo
        {
            get
            {
                return (GEyesSession)Session["MyInfo"];
            }
            set
            {
                Session["MyInfo"] = value;
            }
        }

        public string Name
        {
            set { }
            get { return string.Empty; }
        }

        public List<AddressEntity> AddressesBind
        {
            set
            {
                gvAddresses.DataSource = value;
                gvAddresses.DataBind();
            }
        }

        private AddressEntity _addressEntity;

        public AddressEntity SelectedAddress
        {
            get
            {
                return _addressEntity;
            }
            set
            {
                _addressEntity = value;
            }
        }

        /// <summary>
        /// String Value for State
        /// </summary>
        public String State
        {
            get
            {
                return this.ddlState.SelectedValue;
            }
            set
            {
                this.ddlState.SelectedValue = value;
            }
        }
        /// <summary>
        /// List Values for State
        /// </summary>
        public List<StateEntity> StateList
        {
            get
            {
                return ViewState["State"] as List<StateEntity>;
            }
            set
            {
                ViewState["State"] = value;
                this.ddlState.DataTextField = "StateText";
                this.ddlState.DataValueField = "StateValue";
                BindDdl(this.ddlState, value);
            }
        }

        public string SelectedCountry
        {
            get { return ddlCountry.SelectedValue; }
            set { ddlCountry.SelectedValue = value; }
        }

        public List<LookupTableEntity> Countries
        {
            get
            {
                return ddlCountry.DataSource as List<LookupTableEntity>;
            }
            set
            {
                ddlCountry.Items.Clear();
                ddlCountry.DataSource = value;
                ddlCountry.DataTextField = "Text";
                ddlCountry.DataValueField = "Value";
                ddlCountry.DataBind();
                ddlCountry.Items.Insert(0, "-Select-");
                ddlCountry.SelectedValue = "US";
            }
        }

        public string Address1
        {
            get { return tbAddress1.Text; }
            set { tbAddress1.Text = value; }
        }

        public string Address2
        {
            get { return tbAddress2.Text; }
            set { tbAddress2.Text = value; }
        }

        public string City
        {
            get { return txtCity.Text; }
            set { txtCity.Text = value; }
        }

        public string ZipCode
        {
            get { return tbZipCode.Text; }
            set { tbZipCode.Text = value.ToZipCodeDisplay(); }
        }

        public string EmailAddress
        {
            get { return (string)Session["Email"]; }
        }

        #endregion IAddressUpdateView Members


    }
}