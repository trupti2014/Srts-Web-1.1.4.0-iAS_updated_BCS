using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseAccessLayer;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.ExtendersHelpers;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class to handle user admin operations.
    /// </summary>
    public class UserAdminRepository : RepositoryBase<Object>
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public UserAdminRepository() : base(DbFactory.GetDbObject(DataBaseType.SQL, "SRTS")) { }

        /// <summary>
        /// Gets a list of admin email addresses.
        /// </summary>
        /// <param name="siteCode">Site code to searh for admins in.</param>
        /// <param name="adminRole">Specific admin roll to look in, i.e. LabAdmin, ClinicAdmin, ...</param>
        /// <returns></returns>
        public List<String> GetAdminEmails(String siteCode, String adminRole)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandText = "GetUserEMail";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", siteCode));
            cmd.Parameters.Add(DAL.GetParamenter("@UserRole", adminRole));
            cmd.Parameters.Add(DAL.GetParamenter("@Success", String.Empty, System.Data.ParameterDirection.Output));

            return this.GetRecords(cmd).Cast<String>().Select(x => x).Distinct().ToList();

            // Code to get success output parameter if needed.
            // var p = cmd.Parameters["@Success"] as IDataParameter;
            // p.Value.ToInt32().Equals(1);
        }

        protected override Object FillRecord(System.Data.IDataReader dr)
        {
            try
            {
                var s = dr.AsString("Email");
                return s;
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}
