using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.Presenters.JSpecs;
using SrtsWeb.Views.JSpecs;
using System.Web;
using System.Web.Security;
using System;
using System.Web.UI;
using System.Configuration;

namespace JSpecs.Forms
{
    public partial class JSpecsLogin : PageBaseJSpecs, IJSpecsLoginView
    {
        private JSpecsLoginPresenter _presenter;
        private string _clinicCode;

        public JSpecsLogin()
        {
            _presenter = new JSpecsLoginPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Begin PIV Authentication Message Handling
            if (Session["ErrorMessage"] != null & !String.IsNullOrEmpty((string)Session["ErrorMessage"]))
            {
                string errorMessage = (string)Session["ErrorMessage"];
                messageContent.InnerHtml = errorMessage;
                Session.Remove("ErrorMessage");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "key", "ShowMessage('" + errorMessage + "')", true);
            }//End PIV Authentication Message Handling
            else
            {
                _presenter.InitView();
            }

        }
        public string ClinicCode 
        {
            get { return _clinicCode; }
            set { _clinicCode = value;  }
        }
        public JSpecsSession userInfo
        {
            get { return (JSpecsSession)Session["userInfo"]; }
            set { Session.Add("userInfo", value); }
        }

        protected void btnCacYes_Click(object sender, EventArgs e)
        {
            //_presenter.StubLogin();
            //Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
            //Response.Redirect("~/WebForms/JSpecs/Forms/Account/JSpecsGetCacCert.aspx");
            Response.Redirect(ConfigurationManager.AppSettings["JSpecsCacRequestUrl"], false);
        }

        private void LogOut()
        {
            FormsAuthentication.SignOut();
            Roles.DeleteCookie();
            Session.Remove("userInfo");
            Session.Clear();
            Session.Abandon();
            HttpCookieCollection myCookies = new HttpCookieCollection();
            myCookies.Clear();
            Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
        }

        public string ErrorMessage
        {
            set
            {
                ShowMessage_Redirect(this.Page, value, "/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
            }
        }

        public bool isCACFound
        {
            get;
            set;
        }
    }
}