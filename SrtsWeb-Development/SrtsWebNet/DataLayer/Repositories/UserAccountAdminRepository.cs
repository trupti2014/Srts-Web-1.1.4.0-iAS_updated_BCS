using DataBaseAccessLayer;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;


namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// Custom repository to handle administrative operations.
    /// </summary>
    public class UserAccountAdminRepository : RepositoryBase<UserAccountEntity>
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public UserAccountAdminRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }



        /// <summary>
        /// Get all assigned users for a site.
        /// </summary>
        /// <param name="siteCode">Site code to find users in.</param>
        /// <returns>User accounts for site.</returns>
        public List<UserAccountEntity> GetSiteUsersBySite(string siteCode)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetSiteUsers";
            cmd.Parameters.Add(this.DAL.GetParamenter("@Site", siteCode));

            return GetRecords(cmd).ToList();
        }

        protected override UserAccountEntity FillRecord(IDataReader dr)
        {
            UserAccountEntity ur = new UserAccountEntity();

            ur.LowerUserName = dr.AsString("LowerUserName");
            ur.IsApproved = dr.ToBoolean("IsApproved");
            ur.IsLockedOut = dr.ToBoolean("IsLockedOut");
            
            return ur;
        }
    }
}