using SrtsWeb.ExtendersHelpers;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom static class used as a helper for session related operations.  Session meaning logon session, NOT session state.
    /// </summary>
    public static class SessionService
    {
        /// <summary>
        /// A static method that creates and stores a session ticket (GUID) in the membership comments to uniquely identify a logon session.
        /// </summary>
        /// <param name="userName">User to create session ticket for.</param>
        public static void CreateAndStoreSessionTicket(String userName)
        {
            var r = HttpContext.Current.Response;

            // Make a 'session token'
            var sessionToken = Guid.NewGuid();

            // Get authentication cookie and ticket
            var authCookie = r.Cookies[FormsAuthentication.FormsCookieName];
            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            // Create a new authentication ticket
            var newAuthTicket = new FormsAuthenticationTicket(
                authTicket.Version,
                authTicket.Name,
                authTicket.IssueDate,
                authTicket.Expiration,
                authTicket.IsPersistent,
                sessionToken.ToString(),
                authTicket.CookiePath);

            // Add sessionToken(GUID) to the Membership comment.  The assumption is that only session data will be stored here.
            var m = Membership.GetUser(userName);
            if (!String.IsNullOrEmpty(m.Comment))
            {
                var ss = m.Comment.Split(new[] { ',' }).ToList();
                var cs = ConfigurationManager.AppSettings["concurrentSessions"].ToInt32();
                if (ss.Count.Equals(cs))
                    ss.RemoveAt(0);
                ss.Add(sessionToken.ToString());
                m.Comment = String.Join(",", ss);
            }
            else
                m.Comment = String.Format("{0},", sessionToken.ToString());
            Membership.UpdateUser(m);

            // Replace the current authentication cookie
            r.Cookies.Remove(FormsAuthentication.FormsCookieName);

            var newAuthCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(newAuthTicket));
            newAuthCookie.HttpOnly = authCookie.HttpOnly;
            newAuthCookie.Path = authCookie.Path;
            newAuthCookie.Secure = authCookie.Secure;
            newAuthCookie.Domain = authCookie.Domain;
            newAuthCookie.Expires = authCookie.Expires;

            r.Cookies.Add(newAuthCookie);
        }

        /// <summary>
        /// A static method that kills all of a users logon sessions by clearing out the membership comments.
        /// </summary>
        /// <param name="userName">User to kill all logon sessions for.</param>
        public static void KillUserSessions(String userName)
        {
            var m = Membership.GetUser(userName);
            m.Comment = String.Empty;
            Membership.UpdateUser(m);
        }

        /// <summary>
        /// A static method that removes the logon session ticket.
        /// </summary>
        /// <param name="userName">User to remove the logon session for.</param>
        public static void SessionLogoff(String userName)
        {
            var id = HttpContext.Current.User.Identity as FormsIdentity;
            var authTicket = id.Ticket;

            var u = Membership.GetUser(userName);
            var c = u.Comment.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            c.Remove(authTicket.UserData);
            u.Comment = String.Join(",", c);
            Membership.UpdateUser(u);
        }
    }
}