using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// A custom HttpModule.
    /// Enforce rules setup for number of concurrent sessions and to also determine if a user is still actively logged in.  
    /// If an administrator forces a user out of the system this is the mechanism that will log them out as well.
    /// </summary>
    public class SessionEnforcementService : IHttpModule
    {
        private void OnPostAuthenticate(object sender, EventArgs e)
        {
            Guid sessionToken;

            // Get application and context members
            var httpApp = (HttpApplication)sender;
            var httpContext = httpApp.Context;

            // Check user's session token
            if (!httpContext.User.Identity.IsAuthenticated) return;
            var authTicket = ((FormsIdentity)httpContext.User.Identity).Ticket;

            if (!String.IsNullOrEmpty(authTicket.UserData))
            {
                sessionToken = new Guid(authTicket.UserData);
            }
            else // No ticket found so log user out
            {
                if (httpContext.Request.FilePath.EndsWith("Login.aspx")) return;

                FormsAuthentication.SignOut();
                HttpContext.Current.Response.Redirect("~/WebForms/Account/Logout.aspx", true);

                return;
            }

            var user = Membership.GetUser(authTicket.Name);

            // Here is where I will need to get and parse all of the tokens in the user and decide how to proceed based on the max allowed sessions.
            var userTokens = new List<Guid>();
            user.Comment.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => userTokens.Add(new Guid(x)));

            if (userTokens.Contains(sessionToken)) return;
            var qs = String.Empty;
            if (userTokens.Count > 0)
            {
                CustomEventLogger.WriteEntry("SessionEnforcementService", String.Format("User {0} was automatically logged out on {2} due to having more than {1} concurrent sessions.",
                    user.UserName,
                    System.Configuration.ConfigurationManager.AppSettings["concurrentSessions"],
                    DateTime.Now));
                qs = "cs=1";
            }
            else
            {
                CustomEventLogger.WriteEntry("SessionEnforcementService", String.Format("User {0} was automatically logged out on {2} by the system administrators.",
                    user.UserName,
                    System.Configuration.ConfigurationManager.AppSettings["concurrentSessions"],
                    DateTime.Now));
                qs = "cs=0";
            }
            FormsAuthentication.SignOut();
            HttpContext.Current.Response.Redirect(String.Format("~/WebForms/Account/Logout.aspx?{0}", qs), true);
        }

        public void Dispose()
        {
            // Nothing here
        }

        /// <summary>
        /// Registers an event handler for PostAuthenticateRequest.
        /// </summary>
        /// <param name="context">Context form where the init event is called to hookup the post authenticate requst event.  This is done automatically since this is a HttpModule defined in the web.config file.</param>
        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += new EventHandler(OnPostAuthenticate);
        }
    }
}
