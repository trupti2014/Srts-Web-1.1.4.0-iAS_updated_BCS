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
    /// A custom repository class to handle lookup table operations.
    /// </summary>
    public sealed class LookupRepository : RepositoryBase<LookupTableEntity>, ILookupRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public LookupRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets a list of all lookup table items.
        /// </summary>
        /// <returns>LookupTableEntity list of all lookup table items.</returns>
        public List<LookupTableEntity> GetAllLooksups()
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAllLookups";

            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Get a list of lookup table items by type.
        /// </summary>
        /// <param name="lookupType">Type of lookup table item to get.</param>
        /// <returns>LookupTableEntity list of lookup table items.</returns>
        public List<LookupTableEntity> GetLookupsByType(string lookupType)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetLookupsByType";
            cmd.Parameters.Add(this.DAL.GetParamenter("@LookupType", lookupType));

            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Adds new lookup table item to the database.
        /// </summary>
        /// <param name="lte">Lookup table item to add.</param>
        /// <returns>Success/failure of insert.</returns>
        public Boolean InsertLookUpTableItem(LookupTableEntity lte)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertLookupTable";
            cmd.Parameters.Add(this.DAL.GetParamenter("@Code", lte.Code));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Text", lte.Text));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Value", lte.Value));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Description", lte.Description));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", lte.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", lte.ModifiedBy));

            return GetRecords(cmd).ToList().Count > 0;
        }

        /// <summary>
        /// Updates a lookup table item in the database.
        /// </summary>
        /// <param name="lte">Lookup table item to update.</param>
        /// <returns>LookupTableEntity list of all lookup table items.</returns>
        public List<LookupTableEntity> UpdateLookUpTable(LookupTableEntity lte)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateLookupTable";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", lte.Id));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Code", lte.Code));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Text", lte.Text));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Value", lte.Value));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Description", lte.Description));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", lte.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", lte.ModifiedBy));

            return GetRecords(cmd).ToList();
        }

        protected override LookupTableEntity FillRecord(IDataReader dr)
        {
            var lt = new LookupTableEntity();
            lt.Id = dr.ToInt32("Id");
            lt.Code = dr.AsString("Code");
            lt.Text = dr.AsString("Text");
            lt.Value = dr.AsString("Value");
            lt.Description = dr.AsString("Description");
            lt.IsActive = dr.ToBoolean("IsActive");
            lt.ModifiedBy = dr.AsString("ModifiedBy");
            lt.DateLastModified = dr.ToDateTime("DateLastModified");
            return lt;
        }
    }
}