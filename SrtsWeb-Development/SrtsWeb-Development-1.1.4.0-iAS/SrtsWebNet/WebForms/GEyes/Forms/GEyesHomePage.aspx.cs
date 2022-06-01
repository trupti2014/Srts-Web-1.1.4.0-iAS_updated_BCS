using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.Views.GEyes;
using System;
using System.Web;
using System.Web.Security;

namespace GEyes.Forms
{
    public partial class GEyesHomePage : PageBase, IGEyesHomePageView
    {
        public GEyesHomePage()
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
                myInfo = new GEyesSession();
                LoadLookupTable();
            }
        }

        public GEyesSession myInfo
        {
            get
            {
                var gEyesSession = (GEyesSession)Session["MyInfo"] as GEyesSession;
               
                return gEyesSession;
            }
            set
            {
                Session.Add("MyInfo", value);
            }
        }

        protected void SecurityAcknowledged(object sender, EventArgs e)
        {
            myInfo.SecurityAcknowledged = true;
            pnlSecurityMessage.Visible = false;
            pnlMain.Visible = true;
            //Master._MainPageFooter.Visible = true;
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/GEyes/Forms/IndividualInfo.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/Account/Login.aspx");
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
            Response.Redirect("~/WebForms/GEyes/Forms/GEyesHomePage.aspx");
        }
    }
}