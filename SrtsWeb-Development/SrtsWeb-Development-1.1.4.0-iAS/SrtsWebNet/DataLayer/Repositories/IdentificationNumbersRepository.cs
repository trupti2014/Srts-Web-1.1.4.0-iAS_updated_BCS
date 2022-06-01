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
    /// A custom repository class to handle indentification number operations.
    /// </summary>
    public sealed class IdentificationNumbersRepository : RepositoryBase<IdentificationNumbersEntity>, IIdentificationNumbersRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public IdentificationNumbersRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Add a new ID number to the database.
        /// </summary>
        /// <param name="ine">ID number entity to add.</param>
        /// <returns>Recently added ID number.</returns>
        public IdentificationNumbersEntity InsertIdentificationNumbers(IdentificationNumbersEntity ine)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertIndividualIDNumber";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", ine.IndividualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumber", ine.IDNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumberType", ine.IDNumberType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", ine.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", ine.ModifiedBy));
            return GetRecord(cmd);
        }

        /// <summary>
        /// Update an existing ID number in the database
        /// </summary>
        /// <param name="ine">ID number entity to update.</param>
        /// <returns>IdentificationNumbersEntity list of ID numbers that belong to the individual.</returns>
        public List<IdentificationNumbersEntity> UpdateIdentificationNumbersByID(IdentificationNumbersEntity ine)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateIndividualIDNumberByID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", ine.ID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", ine.IndividualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumber", ine.IDNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumberType", ine.IDNumberType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", ine.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", ine.ModifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a list of all ID numbers for an individual.
        /// </summary>
        /// <param name="individualID">Individual ID number to search with.</param>
        /// <param name="modifiedBy">User performing the operation.</param>
        /// <returns>IdentificationNumbersEntity list of ID numbers that belong to the individual.</returns>
        public List<IdentificationNumbersEntity> GetIdentificationNumbersByIndividualID(int individualID, string modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetIndividualIDNumbersByIndividualID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", individualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a list of all ID numbers by ID number (SSN, DODID, ...).
        /// </summary>
        /// <param name="idNumber">ID number to search with.</param>
        /// <param name="idNumberType">Type of ID number being used for search; SSN, DODID, ...</param>
        /// <param name="modifiedBy">User performing the operation.</param>
        /// <returns>IdentificationNumbersEntity list of ID numbers that belong to the ID number.</returns>
        public List<IdentificationNumbersEntity> GetIdentificationNumberByIDNumber(string idNumber, string idNumberType, string modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetIndividualIDNumberByIDNumber";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumber", idNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumberType", idNumberType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        protected override IdentificationNumbersEntity FillRecord(IDataReader dr)
        {
            IdentificationNumbersEntity ine = new IdentificationNumbersEntity();
            ine.DateLastModified = dr.ToDateTime("DateLastModified");
            ine.ID = dr.ToInt32("ID");
            ine.IndividualID = dr.ToInt32("IndividualID");
            ine.IsActive = dr.ToBoolean("IsActive");
            ine.ModifiedBy = dr.AsString("ModifiedBy");
            ine.IDNumber = dr.AsString("IDNumber");
            ine.IDNumberType = dr.AsString("IDNumberType");
            ine.IDNumberTypeDescription = Helpers.GetIDTypeDescription(ine.IDNumberType);
            return ine;
        }
    }
}