using DataBaseAccessLayer;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class to do re-order reason operations.
    /// </summary>
    public class ReOrderReasonRepository : RepositoryBase<ReOrderEntity>
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public ReOrderReasonRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets the re-order reasons.
        /// </summary>
        /// <returns>ReOrderEntity list of re-order reasons.</returns>
        public IEnumerable<ReOrderEntity> GetReorderReasons()
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandText = "GetReOrderReasons";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return GetRecords(cmd);
        }

        protected override ReOrderEntity FillRecord(System.Data.IDataReader dr)
        {
            var r = new ReOrderEntity();
            r.ID = dr["ID"].ToInt32();
            r.RoReason = dr.AsString("ROReason");
            return r;
        }
    }
}