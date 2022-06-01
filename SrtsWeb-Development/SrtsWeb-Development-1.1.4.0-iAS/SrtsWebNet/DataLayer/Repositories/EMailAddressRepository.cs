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
    /// A custom repository class to perform email address related operations.
    /// </summary>
    public sealed class EMailAddressRepository : RepositoryBase<EMailAddressEntity>, IEMailAddressRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public EMailAddressRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="CatalogName"></param>
        public EMailAddressRepository(String CatalogName)
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.DefaultConnStrNm), CatalogName)
        { }

        /// <summary>
        /// Gets all email addresses by an individual ID.
        /// </summary>
        /// <param name="individualID">Individual ID to search with.</param>
        /// <param name="modifiedBy">User performing the search operation.</param>
        /// <returns>EMailAddressEntity list of an individuals email addresses.</returns>
        public List<EMailAddressEntity> GetEmailAddressesByIndividualID(int individualID, string modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetIndividualEMailAddressesByIndividualID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", individualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Updates an existing address in the database.
        /// </summary>
        /// <param name="addr">Address to be updated.</param>
        /// <returns>EMailAddressEntity list of email addresses for an individual.</returns>
        public List<EMailAddressEntity> UpdateEMailAddress(EMailAddressEntity addr)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateIndividualEMailAddressByID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", addr.ID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", addr.IndividualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EMailType", addr.EMailType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EMailAddress", addr.EMailAddress));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", addr.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", addr.ModifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Adds a new email address to the database.
        /// </summary>
        /// <param name="addr">Address to add.</param>
        /// <returns>EMailAddressEntity list of email addresses for an individual.</returns>
        public List<EMailAddressEntity> InsertEMailAddress(EMailAddressEntity addr)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertIndividualEMailAddress";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", addr.IndividualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EMailType", addr.EMailType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EMailAddress", addr.EMailAddress));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", addr.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", addr.ModifiedBy));
            return GetRecords(cmd).ToList();
        }

        protected override EMailAddressEntity FillRecord(IDataReader dr)
        {
            var ea = new EMailAddressEntity();
            ea.DateLastModified = dr.ToDateTime("DateLastModified");
            ea.EMailAddress = dr.AsString("EMailAddress");
            ea.ID = dr.ToInt32("ID");
            //ea.EMailType = dr.AsString("EMailType");
            ea.IndividualID = dr.ToInt32("IndividualID");
            ea.IsActive = dr.ToBoolean("IsActive");
            ea.ModifiedBy = dr.AsString("ModifiedBy");
            return ea;
        }
    }
}