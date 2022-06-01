
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.Views.GEyes;
using System;
using System.Web;
using System.Web.Security;

namespace JSpecs.Forms
{
    public partial class JSpecsHomePage : PageBase
    {
        public JSpecsHomePage()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                LogOut();
            }

            if (!IsPostBack)
            {
                myInfo = new JSpecsSession();
                LoadLookupTable();
            }
        }

        public void GoToLogin(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
        }

        public JSpecsSession myInfo
        {
            get { return (JSpecsSession)Session["MyInfo"]; }
            set { Session.Add("MyInfo", value); }
        }

        private void LoadLookupTable()
        {
            ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        }

        private void LogOut()
        {
            FormsAuthentication.SignOut();
            Roles.DeleteCookie();
            Session.Clear();
            Session.Abandon();
            HttpCookieCollection myCookies = new HttpCookieCollection();
            myCookies.Clear();
            Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
        }
    }
}