using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace SrtsWeb.CustomProviders
{
    /// <summary>
    /// Custom class that implements SqlMembershipProvider user for SRTS membership operations.
    /// </summary>
    public sealed class SrtsMembership : SqlMembershipProvider
    {
        public String ChangeError { get; set; }

        /// <summary>
        /// Custom method that allows a MgmtEnterprise user to change the password for a user without knowing the old password.  The new password cannot be the same as the previous (n) passwords.
        /// </summary>
        /// <param name="username">String</param>
        /// <param name="newPassword">String</param>
        /// <returns></returns>
        [System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = "MgmtEnterprise")]
        public bool AdminSetPassword(String username, String newPassword)
        {
            var ms = new MembershipService(new MembershipRepository());
            if (!CanChangePassword(username, newPassword, ms)) return false;

            var salt = ms.GetCurrentSalt(username);
            var newP = EncodePassword(newPassword, salt, "SHA1");

            return ms.AdminSetPassword(username, newP, salt);
        }

        /// <summary>
        /// Overridden method used to change a users password.  The new password cannot be the same as the previous (n) passwords.
        /// </summary>
        /// <param name="username">String</param>
        /// <param name="oldPassword">String</param>
        /// <param name="newPassword">String</param>
        /// <returns></returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            this.ChangeError = String.Empty;
            if (!CanChangePassword(username, newPassword, new MembershipService(new MembershipRepository()))) return false;
            return base.ChangePassword(username, oldPassword, newPassword);
        }

        private bool CanChangePassword(string username, string newPassword, IMembershipService service)
        {
            // Get password history
            var pH = service.GetPasswordSaltHistory(username);

            // Create hash of new password with salt in each password history item
            // Compare the newPassword to history passwords using accompanying salt
            // If no match found then change the password
            if (pH.Any(x => x.PasswordHash == EncodePassword(newPassword, x.Salt, "SHA1")))
            {
                this.ChangeError = String.Format("Password cannot be the same as the previous {0}.", ConfigurationManager.AppSettings["minPasswordHistory"]);
                return false;
            }
            return true;
        }

        private string EncodePassword(string pass, string salt, string hashingAlgorithm)
        {
            var bytes = Encoding.Unicode.GetBytes(pass);
            var src = Convert.FromBase64String(salt);
            var dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            var algorithm = HashAlgorithm.Create(hashingAlgorithm);
            var inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }
    }
}