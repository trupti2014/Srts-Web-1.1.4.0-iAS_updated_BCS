using SrtsWeb.DataLayer.TypeExtenders;
using SrtsWeb.Entities;
using System.Data;
using System.Data.SqlClient;

namespace SrtsWebTrainingAdmin.Repositories
{
    public sealed class SiteCodeRepository : ISiteCodeRepository
    {
        #region CLASS MAINTENANCE

        //public void DeleteClass(string _class)
        //{
        //    using (SqlCommand sqlCmd = new SqlCommand())
        //    {
        //        sqlCmd.CommandType = CommandType.StoredProcedure;
        //        sqlCmd.CommandText = "DeleteClass";
        //        sqlCmd.AddParameter("@ClassCode", SqlDbType.VarChar, ParameterDirection.Input, _class);
        //        sqlCmd.ExecuteToNonQuery();
        //    }
        //}

        public DataTable InsertSite(SiteCodeEntity sce)
        {
            DataTable dt = new DataTable();
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "InsertSite";
                sqlCmd.AddParameter("@SiteCode", SqlDbType.VarChar, ParameterDirection.Input, sce.SiteCode);
                sqlCmd.AddParameter("@SiteName", SqlDbType.VarChar, ParameterDirection.Input, sce.SiteName);
                sqlCmd.AddParameter("@SiteType", SqlDbType.VarChar, ParameterDirection.Input, sce.SiteType);
                sqlCmd.AddParameter("@SiteDescription", SqlDbType.VarChar, ParameterDirection.Input, sce.SiteDescription);
                sqlCmd.AddParameter("@EMailAddress", SqlDbType.VarChar, ParameterDirection.Input, sce.EMailAddress);
                sqlCmd.AddParameter("@DSNPhoneNumber", SqlDbType.VarChar, ParameterDirection.Input, sce.DSNPhoneNumber);
                sqlCmd.AddParameter("@RegPhoneNumber", SqlDbType.VarChar, ParameterDirection.Input, sce.RegPhoneNumber);
                sqlCmd.AddParameter("@BOS", SqlDbType.VarChar, ParameterDirection.Input, sce.BOS);
                sqlCmd.AddParameter("@IsMultiVision", SqlDbType.Bit, ParameterDirection.Input, sce.IsMultivision);
                sqlCmd.AddParameter("@IsAPOCompatible", SqlDbType.Bit, ParameterDirection.Input, sce.IsAPOCompatible);
                sqlCmd.AddParameter("@MaxEyeSize", SqlDbType.Int, ParameterDirection.Input, sce.MaxEyeSize);
                sqlCmd.AddParameter("@MaxFramesPerMonth", SqlDbType.Int, ParameterDirection.Input, sce.MaxFramesPerMonth);
                sqlCmd.AddParameter("@MaxPower", SqlDbType.Decimal, ParameterDirection.Input, sce.MaxPower);
                sqlCmd.AddParameter("@HasLMS", SqlDbType.Bit, ParameterDirection.Input, sce.HasLMS);
                sqlCmd.AddParameter("@Region", SqlDbType.Int, ParameterDirection.Input, sce.Region);
                sqlCmd.AddParameter("@MultiPrimary", SqlDbType.VarChar, ParameterDirection.Input, sce.MultiPrimary);
                sqlCmd.AddParameter("@MultiSecondary", SqlDbType.VarChar, ParameterDirection.Input, sce.MultiSecondary);
                sqlCmd.AddParameter("@SinglePrimary", SqlDbType.VarChar, ParameterDirection.Input, sce.SinglePrimary);
                sqlCmd.AddParameter("@SingleSecondary", SqlDbType.VarChar, ParameterDirection.Input, sce.SingleSecondary);
                sqlCmd.AddParameter("@IsActive", SqlDbType.VarChar, ParameterDirection.Input, sce.IsActive);
                sqlCmd.AddParameter("@ShipToPatientLab", SqlDbType.Bit, ParameterDirection.Input, sce.ShipToPatientLab);
                sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, sce.ModifiedBy);
                dt = sqlCmd.ExecuteToDataTable();
            }
            return dt;
        }

        public DataTable InsertSiteAddress(SiteAddressEntity sae)
        {
            DataTable dt = new DataTable();
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "InsertSiteAddress";
                sqlCmd.AddParameter("@SiteCode", SqlDbType.VarChar, ParameterDirection.Input, sae.SiteCode);
                sqlCmd.AddParameter("@Address1", SqlDbType.VarChar, ParameterDirection.Input, sae.Address1);
                sqlCmd.AddParameter("@Address2", SqlDbType.VarChar, ParameterDirection.Input, sae.Address2);
                sqlCmd.AddParameter("@AddressType", SqlDbType.VarChar, ParameterDirection.Input, sae.AddressType);
                sqlCmd.AddParameter("@Address3", SqlDbType.VarChar, ParameterDirection.Input, sae.Address3);
                sqlCmd.AddParameter("@City", SqlDbType.VarChar, ParameterDirection.Input, sae.City);
                sqlCmd.AddParameter("@State", SqlDbType.VarChar, ParameterDirection.Input, sae.State);
                sqlCmd.AddParameter("@Country", SqlDbType.VarChar, ParameterDirection.Input, sae.Country);
                sqlCmd.AddParameter("@ZipCode", SqlDbType.VarChar, ParameterDirection.Input, sae.ZipCode);
                sqlCmd.AddParameter("@IsConus", SqlDbType.Bit, ParameterDirection.Input, sae.IsConus);
                sqlCmd.AddParameter("@IsActive", SqlDbType.VarChar, ParameterDirection.Input, sae.IsActive);
                sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, sae.ModifiedBy);
                dt = sqlCmd.ExecuteToDataTable();
            }
            return dt;
        }

        public bool IsSiteCodeAvailable(string _class)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetSiteBySiteID";
                cmd.AddParameter("@SiteCode", SqlDbType.VarChar, ParameterDirection.Input, _class + "C");
                var dt = cmd.ExecuteToDataTable();
                if (dt.Rows.Count > 0)
                {
                    dt.Rows.Clear();
                    dt.Clear();
                    dt.Dispose();
                    return false;
                }
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetSiteBySiteID";
                cmd.AddParameter("@SiteCode", SqlDbType.VarChar, ParameterDirection.Input, _class + "L");
                var dt = cmd.ExecuteToDataTable();
                if (dt.Rows.Count > 0)
                {
                    dt.Rows.Clear();
                    dt.Clear();
                    dt.Dispose();
                    return false;
                }
            }

            return true;
        }

        #endregion CLASS MAINTENANCE
    }
}