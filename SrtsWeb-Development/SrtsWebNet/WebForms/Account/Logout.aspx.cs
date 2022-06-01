using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Web;
using System.Configuration;


namespace SrtsWeb.Account
{
    public partial class Logout : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.FindControl("lsSrtsLoginStatus").Visible = false;

            var m = String.Empty;
            var l = String.Empty;

            if (Request.QueryString == null || Request.QueryString.Count == 0) return;
            switch (Request.QueryString["cs"])
            {
                case "0":
                    m = "User was automatically logged out by the system administrators.";
                    l = String.Format("User {0} was automatically logged out by the system administrators at {1}.", new Object[] { mySession.MyUserID, DateTime.Now });

                    MembershipService.DoLogOut(HttpContext.Current.User.Identity.Name);

                    break;

                case "1":
                    m = "User was automatically logged out due to having too many concurrent sessions.";
                    l = String.Format("User {0} was automatically logged out due to having too many concurrent sessions at {1}.", new Object[] { mySession.MyUserID, DateTime.Now });

                    MembershipService.DoLogOut(HttpContext.Current.User.Identity.Name);

                    break;

                case "2":
                    m = "User was automatically logged out due to session timeout.";
                    l = String.Format("User {0} session timed out at {1}.", new Object[] { mySession.MyUserID, DateTime.Now });

                    MembershipService.DoLogOut(HttpContext.Current.User.Identity.Name);

                    //Response.Redirect("~/WebForms/Account/Logout.aspx", false);
                    Response.Redirect(ConfigurationManager.AppSettings["LogoutUrl"], false);  //Aldela 05/07/2018: Added this line
                    break;

                case "3":
                    m = "You have successfully logged out.";
                    var u = "Unknown User";

                    if (Request.QueryString["u"] != null)
                        u = Request.QueryString["u"];

                    l = String.Format("User {0} successfully logged out at {1}.", new Object[] { u, DateTime.Now });
                    MembershipService.DoLogOut(HttpContext.Current.User.Identity.Name);
                    break;
                case "4":
                    m = "User was automatically logged out.";
                    //var u = "Unknown User";

                    //if (Request.QueryString["u"] != null)
                    //    u = Request.QueryString["u"];
                    var c = Master.FindControl("pnlContentAuthenticated") as System.Web.UI.WebControls.Panel;
                    c.Visible = false;
                    l = String.Format("User {0} has been logged out at {1}.", new Object[] { mySession.MyUserID, DateTime.Now });
                    MembershipService.DoLogOut(HttpContext.Current.User.Identity.Name);

                    break;
            }
            Globals.ClearGlobals();
            this.litMessage.Text = m;
            LogEvent(l);
        }
    }
}