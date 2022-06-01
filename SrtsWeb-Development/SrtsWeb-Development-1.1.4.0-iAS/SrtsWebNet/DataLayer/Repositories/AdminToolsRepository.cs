using DataBaseAccessLayer;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// Custom repository to handle administrative operations.
    /// </summary>
    public class AdminToolsRepository : RepositoryBase.RepositoryBase<object>
    {
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public AdminToolsRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="CatalogName"></param>
        public AdminToolsRepository(String CatalogName)
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.DefaultConnStrNm), CatalogName)
        { }

        /// <summary>
        /// Gets a list of all active orders for a site by date range.
        /// </summary>
        /// <param name="siteCode">Site code to find orders in.</param>
        /// <param name="startDate">Start of the date range.</param>
        /// <param name="endDate">End of the date range.</param>
        /// <param name="myUserId">User performing the search.</param>
        /// <returns>Active order list.</returns>
        public List<object> GetAllActiveOrders(string siteCode, string startDate, string endDate, string myUserId)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAllActiveOrdersBySiteCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", myUserId));
            cmd.Parameters.Add(this.DAL.GetParamenter("@StartDate", startDate));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EndDate", endDate));

            return GetRecords(cmd).ToList();
        }

        protected override object FillRecord(IDataReader dr)
        {
            var o = new
            {
                OrderNumber = dr[0].ToString(),
                SiteType = dr[1].ToString(),
                StatusDate = dr[2].ToString(),
                StatusComment = dr[3].ToString(),
                FrameCode = dr[4].ToString(),
                LenseType = dr[5].ToString(),
                LastName = dr[6].ToString(),
                FirstName = dr[7].ToString(),
                MiddleName = dr[8].ToString(),
                LastFour = dr[9].ToString()
            };

            return o;
        }
    }
}