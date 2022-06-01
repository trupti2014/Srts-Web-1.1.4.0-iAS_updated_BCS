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
    /// A custom repository class to handle phone number operations.
    /// </summary>
    public sealed class PhoneRepository : RepositoryBase<PhoneNumberEntity>, IPhoneRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public PhoneRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets a list of phone numbers for an individual by individual ID.
        /// </summary>
        /// <param name="individualID">Individual ID to search with.</param>
        /// <param name="modifiedBy">User performing the search operation.</param>
        /// <returns>PhoneNumberEntity list of phone numbers.</returns>
        public List<PhoneNumberEntity> GetPhoneNumbersByIndividualID(int individualID, string modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetIndividualPhoneNumbersByIndividualID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", individualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Updates an existing phone number in the database.
        /// </summary>
        /// <param name="pne">Phone number to update.</param>
        /// <returns>PhoneNumberEntity list of phone numbers.</returns>
        public List<PhoneNumberEntity> UpdatePhoneNumber(PhoneNumberEntity pne)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateIndividualPhoneNumberByID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", pne.ID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", pne.IndividualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PhoneNumberType", pne.PhoneNumberType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@AreaCode", string.Empty));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PhoneNumber", pne.PhoneNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Extension", pne.Extension));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", pne.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", pne.ModifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Adds a new phone number to the database.
        /// </summary>
        /// <param name="pne">Phone number to add.</param>
        /// <returns>PhoneNumberEntity list of phone numbers.</returns>
        public List<PhoneNumberEntity> InsertPhoneNumber(PhoneNumberEntity pne)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertIndividualPhoneNumber";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", pne.IndividualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PhoneNumberType", pne.PhoneNumberType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@AreaCode", string.Empty));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PhoneNumber", pne.PhoneNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Extension", pne.Extension));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", pne.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", pne.ModifiedBy));
            return GetRecords(cmd).ToList();
        }

        protected override PhoneNumberEntity FillRecord(IDataReader dr)
        {
            var pe = new PhoneNumberEntity();
            pe.IndividualID = dr.ToInt32("IndividualID");
            pe.DateLastModified = dr.ToDateTime("DateLastModified");

            var testPhone = dr.AsString("PhoneNumber");
            if (!testPhone.Contains("-") && testPhone.Length.Equals(7))
                testPhone = string.Format("{0:###-####}", dr.ToInt32("PhoneNumber"));
            pe.PhoneNumber = testPhone;

            pe.ID = dr.ToInt32("ID");
            //pe.PhoneNumberType = dr.AsString("PhoneNumberType");
            pe.IsActive = dr.ToBoolean("IsActive");
            pe.ModifiedBy = dr.AsString("ModifiedBy");
            pe.AreaCode = dr.AsString("AreaCode");
            pe.Extension = dr.AsString("Extension");
            return pe;
        }
    }
}