using BarcodeLib;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.GEyes;
using SrtsWeb.Views.GEyes;
using System;
using System.Web.UI.WebControls;

namespace GEyes.Forms
{
    public partial class OrderConfirmation : PageBase, IOrderConfirmationView
    {
        private OrderConfirmationPresenter _presenter;
        private Boolean IsPageRefresh = false;

        public OrderConfirmation()
        {
            _presenter = new OrderConfirmationPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
                if (Session["OrderSubmitted"] != null)
                {
                    Session["OrderSubmitted"] = null;
                    Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
                }
                else
                {
                    ViewState["postids"] = System.Guid.NewGuid().ToString();
                    Session["postid"] = ViewState["postids"].ToString();

                    _presenter.InitView();
                    pnlConfirm.Visible = true;
                    pnlSuccess.Visible = false;
                }
            }
            else
            {
                if (ViewState["postids"].ToString() != Session["postid"].ToString())
                {
                    IsPageRefresh = true;
                }
                Session["postid"] = System.Guid.NewGuid().ToString();
                ViewState["postids"] = Session["postid"];
            }
        }

        public GEyesSession myInfo
        {
            get { return (GEyesSession)Session["MyInfo"]; }
            set { Session["MyInfo"] = value; }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public string EmailAddress
        {
            get;
            set;
            //get { return lblEmailAddress.Text; }
            //set { lblEmailAddress.Text = value; }
        }

        public string OrderNumber
        {
            get { return lblOrderNumber.Text; }
            set { lblOrderNumber.Text = value; }
        }

        protected void OrderConfirmationCaptchaValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
        //    // validate the Captcha to check we're not dealing with a bot
            args.IsValid = OrderConfirmationCaptcha.Validate(args.Value);

            OrderConfirmationCaptchaCode.Text = null; // clear previous user input
        }

        protected void btnEixt_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (!IsPageRefresh)
                {
                    pnlConfirm.Visible = false;
                    btnSubmit.Enabled = false;
                    _presenter.SaveData(new GenerateBarCodes(new Barcode()));

                    var m = String.Empty;
                    if (!string.IsNullOrEmpty(Message))
                    {
                        lblMessage.Text = Message;
                        pnlSuccess.Visible = false;
                        m = "unsuccessfully created G-Eyes order";
                        return;
                    }
                    else
                    {
                        Session["OrderSubmitted"] = true;
                        pnlSuccess.Visible = true;
                        m = "successfully created G-Eyes order";
                    }
                    var person = String.Format("{0},{1}-{2}", myInfo.Patient.Individual.LastName, myInfo.Patient.Individual.FirstName, myInfo.Patient.Individual.ID);
                    LogEvent("{0} {1} at {2}", new Object[] { person, m, DateTime.Now });
                }
                else
                {
                    Session["OrderSubmitted"] = null;
                    Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
                }
            }
            else
            {
                btnSubmit.Enabled = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
        }
    }
}