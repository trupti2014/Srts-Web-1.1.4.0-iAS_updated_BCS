using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class to perform SRTS membership operations.
    /// </summary>
    public class MembershipService : IMembershipService
    {
        private IMembershipRepository repository;

        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="repository">Sets the membership repository using direct injection.</param>
        public MembershipService(IMembershipRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets a history of a users passwords, salts, and set dates.
        /// </summary>
        /// <param name="userName">User to get password history.</param>
        /// <returns>PasswordHistory list of passwords, salts, and set dates.</returns>
        public List<PasswordHistory> GetPasswordSaltHistory(string userName)
        {
            return repository.GetPasswordHistoryByUserName(userName);
        }

        /// <summary>
        /// Allows an admin to set a users password without the old password.
        /// </summary>
        /// <param name="userName">User to set password for.</param>
        /// <param name="passwordHash">The hashed password text.</param>
        /// <param name="salt">The password salt.</param>
        /// <returns>Success/Failure of change.</returns>
        [System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = "MgmtEnterprise")]
        public bool AdminSetPassword(string userName, string passwordHash, string salt)
        {
            return repository.AdminSetPasswordHash(userName, passwordHash, salt);
        }

        /// <summary>
        /// Gets the current password salt for the provided user.
        /// </summary>
        /// <param name="username">User name to get salt for.</param>
        /// <returns>Password salt.</returns>
        public string GetCurrentSalt(string username)
        {
            return repository.GetCurrentSalt(username);
        }

        /// <summary>
        /// Performs a logout opperation for a provided user.
        /// </summary>
        /// <param name="userName">User to log out.</param>
        public static void DoLogOut(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
                SessionService.SessionLogoff(userName);
            FormsAuthentication.SignOut();
            Roles.DeleteCookie();
            var myContext = HttpContext.Current;
            var mySession = myContext.Session["SRTSSession"] as SRTSSession;
            mySession = null;
            myContext.Session.Abandon();
            myContext.Session.Clear();
        }
    }
}