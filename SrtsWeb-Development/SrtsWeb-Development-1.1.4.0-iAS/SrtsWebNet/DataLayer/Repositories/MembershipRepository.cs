using DataBaseAccessLayer;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class to handle SQL Membership operations.
    /// </summary>
    public sealed class MembershipRepository : RepositoryBase<PasswordHistory>, IMembershipRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public MembershipRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets a historical list of a users passwords.
        /// </summary>
        /// <param name="UserName">User name to search with.</param>
        /// <returns>PasswordHistory list of users passwords.</returns>
        public List<PasswordHistory> GetPasswordHistoryByUserName(string UserName)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandText = "GetPasswordHistoryByName";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(DAL.GetParamenter("@UserName", UserName));
            return GetRecords(cmd).ToList();
        }

        protected override PasswordHistory FillRecord(IDataReader dr)
        {
            var pE = new PasswordHistory();
            pE.Salt = dr.AsString("PasswordSalt");
            pE.PasswordHash = dr.AsString("Password");
            pE.HistoryDate = dr.ToDateTime("DateChanged");
            return pE;
        }

        /// <summary>
        /// Gets the current password salt for a user.
        /// </summary>
        /// <param name="UserName">User name to search with.</param>
        /// <returns>Users current password salt.</returns>
        public string GetCurrentSalt(string UserName)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandText = "GetPasswordSalt";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(DAL.GetParamenter("@lowerusername", UserName.ToLower()));
            return GetObject(cmd).ToString();
        }

        /// <summary>
        /// Updates a users password without knowing the old password.  Performed by MgmtEnterprise ONLY!
        /// </summary>
        /// <param name="UserName">User name to update password for.</param>
        /// <param name="password">Hashed password to be changed to.</param>
        /// <param name="salt">Password salt.</param>
        /// <returns>Success/failure of change.</returns>
        [System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = "MgmtEnterprise")]
        public bool AdminSetPasswordHash(string UserName, string password, string salt)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandText = "aspnet_Membership_SetPassword";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(DAL.GetParamenter("@ApplicationName", System.Web.Security.Membership.ApplicationName));
            cmd.Parameters.Add(DAL.GetParamenter("@UserName", UserName));
            cmd.Parameters.Add(DAL.GetParamenter("@NewPassword", password));
            cmd.Parameters.Add(DAL.GetParamenter("@PasswordSalt", salt));
            cmd.Parameters.Add(DAL.GetParamenter("@CurrentTimeUtc", DateTime.Now));
            cmd.Parameters.Add(DAL.GetParamenter("@PasswordFormat", 1));

            return UpdateData(cmd).GreaterThan(1);
        }
    }
}