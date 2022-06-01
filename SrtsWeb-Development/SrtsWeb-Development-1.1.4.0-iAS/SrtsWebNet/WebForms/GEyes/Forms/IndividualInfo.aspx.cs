using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.GEyes;
using SrtsWeb.Views.GEyes;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GEyes.Forms
{
    public partial class IndividualInfo : PageBase, IIndividualInfoView
    {
        private IndividualInfoPresenter _presenter;

        public IndividualInfo()
        {
            _presenter = new IndividualInfoPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //tbIDNumber.Focus();

            //if (myInfo != null)
            //{
            //    if (myInfo.SecurityAcknowledged)
            //    {
            //        Master._MainPageFooter.Visible = true;
            //    }
            //}
            //else
            //{
            //    Response.Redirect("~/WebForms/Account/Login.aspx");
            //}

            if (myInfo.IsNull())
                Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");

            if (!IsPostBack)
            {
                _presenter.InitView();
                pnlMain.Visible = true;
            }
            else
            {
                if (myInfo == null)
                {
                    Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
                }
            }
        }




        public GEyesSession myInfo
        {
            get { return (GEyesSession)Session["MyInfo"]; }
            set { Session.Add("MyInfo", value); }
        }

        public string IDNumber
        {
            get { return tbIDNumber.Text; }
            set { tbIDNumber.Text = value; }
        }

        public string IDType
        {
            get
            {
                if (rbDoD.Checked == true)
                {
                    return "DIN";
                }
                else
                {
                    return "SSN";
                }
            }
        }

        private string _clinicCode;
        public string ClinicCode
        {
            get { return _clinicCode; }
            set { _clinicCode = "009900"; }  // to do get from sp
        }

        public string EmailAddress
        {
            get { return tbEmail.Text; }
            set { tbEmail.Text = value; }
        }

        public string EmailAddressConfirm
        {
            get { return tbEmailConfirm.Text; }
            set { tbEmailConfirm.Text = value; }
        }

        public string ZipCode
        {
            get { return tbZipCode.Text; }
            set { tbZipCode.Text = value.ToZipCodeDisplay(); }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            CheckZipAndUser();
        }

        protected void CheckZipAndUser()
        {
            if (Page.IsValid)
            {
                if (_presenter.CheckZip())
                {
                    if (_presenter.CheckUser())
                    {
                        Session.Add("DeployZipCode", ZipCode);

                        Session.Add("IDNumber", IDNumber);

                        Session.Add("Email", EmailAddress);

                       // lblWelcome.Text = _presenter.Name();

                        pnlConfirm.Visible = false;
                        pnlMain.Visible = false;
                        Response.Redirect("~/WebForms/GEyes/Forms/SelectOrder.aspx");
                    }
                    else
                    {
                        ShowMessage();
                        pnlMain.Visible = true;
                        tbIDNumber.Focus();
                    }
                }
                else
                {
                    ShowMessage();
                    pnlMain.Visible = true;
                    tbZipCode.Focus();
                }
            }
        }

        private void ShowMessage()
        {
            CustomValidator cv = new CustomValidator();
            cv.ValidationGroup = "ErrorMessage";
            cv.IsValid = false;
            cv.ErrorMessage = Message;
            this.Page.Validators.Add(cv);
            return;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
        }

        protected void btnSubmitType_Click(object sender, EventArgs e)
        {
            //if (radbtnNewOrder.Checked)
            //{
            //    Response.Redirect("~/WebForms/GEyes/Forms/CreateOrder.aspx");
            //}
            //else
            //    Response.Redirect("~/WebForms/GEyes/Forms/SelectOrder.aspx");
        }

        protected void btnCancelType_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
        }
    }
}