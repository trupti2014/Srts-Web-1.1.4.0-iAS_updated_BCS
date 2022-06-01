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
    /// A custom repository class that implements from RepositoryBase to perform data operations for address data.
    /// </summary>
    public sealed class AddressRepository : RepositoryBase<AddressEntity>, IAddressRepository
    {
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public AddressRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="CatalogName">Database catalog to connect to.</param>
        public AddressRepository(string CatalogName)
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.DefaultConnStrNm), CatalogName)
        { }

        /// <summary>
        /// Updates a single address to the database.
        /// </summary>
        /// <param name="addr">Address entity to update.</param>
        /// <returns>List of addresses.</returns>
        public List<AddressEntity> UpdateAddress(AddressEntity addr)
        {
            var cmd = this.DAL.GetCommandObject();
            /* // Set active to true if the addr comes back as active
            int isActive = 0;

            if(addr.IsActive)
            {
                isActive = 1;
            }*/

            // Set active to always true
            int isActive = 1;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateAddressByID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", addr.ID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", addr.IndividualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Address1", addr.Address1));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Address2", addr.Address2));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Address3", addr.Address3));
            cmd.Parameters.Add(this.DAL.GetParamenter("@City", addr.City));
            cmd.Parameters.Add(this.DAL.GetParamenter("@State", addr.State));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Country", addr.Country));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ZipCode", addr.ZipCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@AddressType", addr.AddressType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@UIC", string.IsNullOrEmpty(addr.UIC) ? string.Empty : addr.UIC));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", isActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", addr.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ExpireDays", addr.ExpireDays));
            // Console.WriteLine("Address active? " + isActive);
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Add a single address to the database.
        /// </summary>
        /// <param name="addr">Address entity to insert.</param>
        /// <returns>List of addresses.</returns>
        public List<AddressEntity> InsertAddress(AddressEntity addr)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertIndividualAddress";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", addr.IndividualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("Address1", addr.Address1));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Address2", addr.Address2));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Address3", addr.Address3));
            cmd.Parameters.Add(this.DAL.GetParamenter("@City", addr.City));
            cmd.Parameters.Add(this.DAL.GetParamenter("@State", addr.State));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Country", addr.Country));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ZipCode", addr.ZipCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@AddressType", addr.AddressType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@UIC", string.IsNullOrEmpty(addr.UIC) ? string.Empty : addr.UIC));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", addr.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", addr.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ExpireDays", addr.ExpireDays));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a list of address entities by an individual id number.
        /// </summary>
        /// <param name="individualID">Individual ID to search with.</param>
        /// <param name="modifiedBy">User performing the database operation.</param>
        /// <returns>List of addresses.</returns>
        public List<AddressEntity> GetAddressesByIndividualID(int individualID, string modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetIndividualAddressesByIndividualID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", individualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        protected override AddressEntity FillRecord(System.Data.IDataReader dr)
        {
            var ae = new AddressEntity();
            ae.Address1 = dr.AsString("Address1");
            ae.Address2 = dr.AsString("Address2");
            ae.Address3 = dr.AsString("Address3");
            ae.ID = dr.ToInt32("ID");
            //ae.AddressType = dr.AsString("AddressType");
            ae.City = dr.AsString("City");
            ae.Country = dr.AsString("Country");
            ae.DateLastModified = dr.ToDateTime("DateLastModified");
            ae.IndividualID = dr.ToInt32("IndividualID");
            ae.IsActive = dr.ToBoolean("IsActive");
            ae.UIC = dr.AsString("UIC");
            ae.ModifiedBy = dr.AsString("ModifiedBy");
            ae.State = dr.AsString("State");
            ae.ZipCode = dr.AsString("ZipCode").ToZipCodeDisplay();
            ae.DateVerified = dr.ToDateTime("DateVerified");
            ae.VerifiedBy = dr.AsString("VerifiedBy");
            ae.ExpireDays = dr.ToInt32("ExpireDays");
            return ae;
        }
    }
}