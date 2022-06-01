using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.DataLayer.Repositories;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// A custom class to to send user emails based on administrative account operations.
    /// </summary>
    public sealed class UserNotificationService
    {
        private static String CurrentUserName { get; set; }
        private static String CurrentUserEmail { get; set; }

        /// <summary>
        /// Static function that sends an email to all site admins, and affected individual when a new user is created.
        /// </summary>
        /// <param name="site">Site code used to get site admins.</param>
        /// <param name="role">Admin role to search for.</param>
        /// <param name="userName">User that account action was taken.</param>
        /// <param name="modifiedBy">User that is performing the operation.</param>
        public static void SendNewUserEmails(String site, String role, String userName, String modifiedBy)
        {
            GetCurrentUserData();

            // Get site admins
            var to = GetAdminEmails(site, role);

            var msg = String.Format("User {0} was just created by {1} on {2}.", userName, CurrentUserName, DateTime.Now);
            var subject = "New user creation";

            SendAdminEmails(to, msg, subject, modifiedBy);
        }

        /// <summary>
        /// Static function that sends an email to all site admins, and affected individual when a user account is disabled.
        /// </summary>
        /// <param name="site">Site code used to get site admins.</param>
        /// <param name="role">Admin role to search for.</param>
        /// <param name="userName">User that account action was taken.</param>
        /// <param name="modifiedBy">User that is performing the operation.</param>
        public static void SendDisableUserAdminEmail(String site, String role, String userName, String modifiedBy)
        {
            GetCurrentUserData();

            // Get site admins
            var to = GetAdminEmails(site, role);

            var msg = String.Format("User {0} was disabled by {1} on {2}.", userName, CurrentUserName, DateTime.Now);
            var subject = "Disabled user account";

            SendAdminEmails(to, msg, subject, modifiedBy);
        }

        /// <summary>
        /// Static function that sends an email to all site admins, and affected individual when a user account is enabled.
        /// </summary>
        /// <param name="site">Site code used to get site admins.</param>
        /// <param name="role">Admin role to search for.</param>
        /// <param name="userName">User that account action was taken.</param>
        /// <param name="modifiedBy">User that is performing the operation.</param>
        public static void SendEnabledUserAdminEmail(String site, String role, String userName, String modifiedBy)
        {
            GetCurrentUserData();

            // Get site admins
            var to = GetAdminEmails(site, role);

            var msg = String.Format("User {0} was enabled by {1} on {2}.", userName, CurrentUserName, DateTime.Now);
            var subject = "Enabled user account";

            SendAdminEmails(to, msg, subject, modifiedBy);
        }

        /// <summary>
        /// Static function that sends an email to all site admins, and affected individual when a user account is modified.
        /// </summary>
        /// <param name="site">Site code used to get site admins.</param>
        /// <param name="role">Admin role to search for.</param>
        /// <param name="useraName">User that account action was taken.</param>
        /// <param name="custMsg">Custom message to add to the body of the email.</param>
        /// <param name="modifiedBy">User that is performing the operation.</param>
        public static void SendModifiedUserAdminEmail(String site, String role, String useraName, String custMsg, String modifiedBy)
        {
            GetCurrentUserData();

            // Get site admins
            var to = GetAdminEmails(site, role);

            var subject = "Modified user account";

            SendAdminEmails(to, custMsg, subject, modifiedBy);
        }

        private static void SendAdminEmails(List<String> toEmails, String emailBody, String emailSubject, String modifiedBy)
        {
            var mail = new MailService();

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SendAdminEmails", modifiedBy))
#endif
                {
                    var extraTo = ConfigurationManager.AppSettings["ExtraNotificationEmails"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    toEmails.AddRange(extraTo);
                    mail.SendEmail(emailBody, ConfigurationManager.AppSettings["FromEmail"], toEmails, emailSubject);
                }
            }
            catch (Exception ex)
            {
                ex.TraceErrorException();
            }
        }

        private static void GetCurrentUserData()
        {
            var m = Membership.GetUser();
            CurrentUserEmail = m.Email;
            CurrentUserName = m.UserName;
        }

        private static List<String> GetAdminEmails(String site, String role)
        {
            var r = new UserAdminRepository();
            var to = r.GetAdminEmails(site, role);
            to.Add(CurrentUserEmail);
            return to.Distinct().ToList();
        }
    }
}
