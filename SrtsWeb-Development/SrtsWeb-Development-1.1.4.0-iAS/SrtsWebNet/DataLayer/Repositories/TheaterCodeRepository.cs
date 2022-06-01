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
    /// A custom sealed repository class to handle theater code operations.
    /// </summary>
    public sealed class TheaterCodeRepository : RepositoryBase<TheaterLocationCodeEntity>, ITheaterCodeRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public TheaterCodeRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets a list of all active theater codes.
        /// </summary>
        /// <returns>TheaterLocationCodeEntity list of active theater codes.</returns>
        public List<TheaterLocationCodeEntity> GetActiveTheaterCodes()
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetTheaterLocationCodes";
            return GetRecords(cmd).ToList();
        }

        protected override TheaterLocationCodeEntity FillRecord(IDataReader dr)
        {
            var tlc = new TheaterLocationCodeEntity();
            tlc.DateLastModified = dr.ToDateTime("DateLastModified");
            tlc.Description = dr.AsString("Description");
            tlc.IsActive = dr.ToBoolean("IsActive");
            tlc.ModifiedBy = dr.AsString("ModifiedBy");
            tlc.TheaterCode = dr.AsString("TheaterCode");
            return tlc;
        }
    }
}