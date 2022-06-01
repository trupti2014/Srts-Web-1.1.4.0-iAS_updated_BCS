using DataBaseAccessLayer;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Data;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class used to perform authorization related operations for loggin in to the system via CAC.
    /// </summary>
    public sealed class AuthorizationRepository : RepositoryBase<DataSet>, IAuthorizationRepository
    {
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public AuthorizationRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="CatalogName"></param>
        public AuthorizationRepository(String CatalogName)
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.DefaultConnStrNm), CatalogName)
        { }

        /// <summary>
        /// Gets a data table of available cac enabled accounts by user name.
        /// </summary>
        /// <param name="strUserName">User name to search with.</param>
        /// <returns>Data table containing cac enabled accounts.</returns>
        public DataTable GetAuthorizationsByUserName(string strUserName)
        {
            using (var cmd = this.DAL.GetCommandObject())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetAuthorizationsByUserName";
                cmd.Parameters.Add(this.DAL.GetParamenter("@userName", strUserName));
                return this.DAL.GetData(cmd);
            }
        }

        /// <summary>
        /// Gets a data table populated with user accounts associated with a cac cert.
        /// </summary>
        /// <param name="CAC_ID">CAC id to search with.</param>
        /// <param name="issuerName">Issuer name to determine cert type.</param>
        /// <returns>Data table of user accounts for cac cert.</returns>
        public DataTable GetAuthorizationsByCAC_ID(string CAC_ID, string issuerName)
        {
            using (var cmd = this.DAL.GetCommandObject())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetAuthorizationsByCAC_ID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@CAC_ID", CAC_ID));
                //cmd.Parameters.Add(this.DAL.GetParamenter("@issuerName", issuerName));
                return this.DAL.GetData(cmd);
            }
        }

        /// <summary>
        /// Updates all user accounts with a new cac id/issuer name for a user.
        /// </summary>
        /// <param name="strUserName">User name to get accounts for.</param>
        /// <param name="CAC_ID">CAC id to add to record.</param>
        /// <param name="issuerName">Issuer name to add to record.</param>
        /// <returns>UNKNOWN</returns>
        public DataTable UpdateAuthorizationCacInfoByUserName(string strUserName, string CAC_ID, string issuerName)
        {
            using (var cmd = this.DAL.GetCommandObject())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "UpdateAuthorizationCacInfoByUserName";
                cmd.Parameters.Add(this.DAL.GetParamenter("@CAC_ID", CAC_ID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@issuerName", issuerName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@UserName", strUserName));
                return this.DAL.GetData(cmd);
            }
        }

        /// <summary>
        /// Updates a user account's SSO user name.
        /// </summary>
        /// <param name="strUserName">User name of account to update.</param>
        /// <param name="strSsoUserName">SSO user name to set.</param>
        /// <returns>UNKNOWN</returns>
        public DataTable UpdateAuthorizationSSOByUserName(string strUserName, string strSsoUserName)
        {
            using (var cmd = this.DAL.GetCommandObject())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "UpdateAuthorizationSSOByUserName";
                cmd.Parameters.Add(this.DAL.GetParamenter("@UserName", strUserName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SSO_UserName", strSsoUserName));
                return this.DAL.GetData(cmd);
            }
        }

        /// <summary>
        /// Delete a user cac authorization by user name.
        /// </summary>
        /// <param name="strUserName">User name to remove cac authorization for.</param>
        /// <returns>Success/Failure of delete.</returns>
        public Boolean DeleteAuthorizationByUserName(string strUserName)
        {
            using (var cmd = this.DAL.GetCommandObject())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "DeleteAuthorizationByUserName";
                cmd.Parameters.Add(this.DAL.GetParamenter("@UserName", strUserName));
                return DeleteData(cmd) > 0;
            }
        }

        /// <summary>
        /// Adds a new cac authorization for an account by user name.
        /// </summary>
        /// <param name="strUserName">User name to add authorization for.</param>
        /// <returns>UNKNOWN</returns>
        public DataTable InsertAuthorizationByUserName(string strUserName)
        {
            using (var cmd = this.DAL.GetCommandObject())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "InsertAuthorizationByUserName";
                cmd.Parameters.Add(this.DAL.GetParamenter("@UserName", strUserName));
                return this.DAL.GetData(cmd);
            }
        }

        /// <summary>
        /// DEPRECATATED
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="userName"></param>
        public void UpdateSiteCodeByUserName(String siteCode, String userName)
        {
            using (var cmd = this.DAL.GetCommandObject())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "UpdateAuthorizationSiteCodeByUserName";
                cmd.Parameters.Add(this.DAL.GetParamenter("@siteCode", siteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@userName", userName));
                UpdateData(cmd);
            }
        }
    }
}