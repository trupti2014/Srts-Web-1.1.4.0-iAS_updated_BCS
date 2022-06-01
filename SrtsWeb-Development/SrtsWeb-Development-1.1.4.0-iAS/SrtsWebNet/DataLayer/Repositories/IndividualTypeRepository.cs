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
    /// A custom repository class to do individual type operations.
    /// </summary>
    public sealed class IndividualTypeRepository : RepositoryBase<IndividualTypeEntity>, IIndividualTypeRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public IndividualTypeRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Adds multiple individual types to the database.
        /// </summary>
        /// <param name="IndividualId">Individual ID to add the types for.</param>
        /// <param name="ModifiedBy">User performing the insert operation.</param>
        /// <param name="IndTypes">Delemited string of individual types to add.</param>
        /// <param name="DeleteAll">Flag to tell the SP to delete all existing individual type.</param>
        /// <returns>Success/Failure of insert operation.</returns>
 
        public bool InsertIndividualTypes(int IndividualId, string ModifiedBy, string IndTypes, bool DeleteAll)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandText = "InsertIndividualType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", IndividualId));
            cmd.Parameters.Add(this.DAL.GetParamenter("@TypeID", IndTypes));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", 1));
            cmd.Parameters.Add(this.DAL.GetParamenter("@DeleteAll", DeleteAll));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));

            InsertData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1);
        }

        /// <summary>
        /// Updates an individual type in the databsase.
        /// </summary>
        /// <param name="type">Individual type entity to update.</param>
        /// <returns>IndividualTypeEntity list of individual types for an individual.</returns>
        public List<IndividualTypeEntity> UpdateIndividualType(IndividualTypeEntity type)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandText = "UpdateIndividualType";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualTypeID", type.ID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", type.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", type.ModifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a list of individual types for a specific individual by ID.
        /// </summary>
        /// <param name="individualId">ID to get individual types for.</param>
        /// <returns>IndividualTypeEntity list of individual types for an individual.</returns>
        public List<IndividualTypeEntity> GetIndividualTypesByIndividualId(int individualId)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandText = "GetIndividualTypesByIndividualID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", individualId));
            return GetRecords(cmd).ToList();
        }

        protected override IndividualTypeEntity FillRecord(IDataReader dr)
        {
            IndividualTypeEntity ie = new IndividualTypeEntity();
            ie.DateLastModified = dr.ToDateTime("DateLastModified");
            ie.ID = dr.ToInt32("ID");
            ie.TypeId = dr.ToInt32("TypeID");
            ie.TypeDescription = dr.AsString("TypeDescription");
            ie.IndividualId = dr.ToInt32("IndividualID");
            ie.IsActive = dr.ToBoolean("IsActive");
            ie.ModifiedBy = dr.AsString("ModifiedBy");
            return ie;
        }
    }
}