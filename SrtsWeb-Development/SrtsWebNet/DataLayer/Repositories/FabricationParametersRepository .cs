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
    /// A custom repository class to do fabrication parameter operations.
    /// </summary>
    public sealed class FabricationParametersRepository : RepositoryBase<FabricationParameterEntitiy>, IFabricationParametersRepository
    {
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public FabricationParametersRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Adds a new fabrication parameter to the database.
        /// </summary>
        /// <param name="param">Fabrication parameter to add.</param>
        /// <returns>Success/failure of insert.</returns>
        public bool InsertParameter(FabricationParameterEntitiy param)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertLensStock";
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", param.SiteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Material", param.Material));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Cylinder", param.Cylinder));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MaxPlus", param.MaxPlus));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MaxMinus", param.MaxMinus));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsStocked", param.IsStocked));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Capability", param.CapabilityType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", param.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

            InsertData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1); ;
        }

        /// <summary>
        /// Modified an existing fabricaton parameter in the database.
        /// </summary>
        /// <param name="param">Fabrication parameter to modify.</param>
        /// <returns>Success/failure of update.</returns>
        public bool UpdateParameter(FabricationParameterEntitiy param)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateLensStock";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", param.ID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Material", param.Material));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Cylinder", param.Cylinder));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MaxPlus", param.MaxPlus));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MaxMinus", param.MaxMinus));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsStocked", param.IsStocked));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Capability", param.CapabilityType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", param.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

            UpdateData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1); ;
        }

        /// <summary>
        /// Delete a fabricaton parameter in the database.
        /// </summary>
        /// <param name="id">Fabrication parameter to delete.</param>
        /// <returns>Success/failure of delete.</returns>
        public bool DeleteParameter(int id)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteLensStock";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", id));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

            UpdateData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1); ;
        }

        /// <summary>
        /// Gets a list of all fabrication parameter for a site.
        /// </summary>
        /// <param name="siteCode">Site to get fabrication parameters for.</param>
        /// <returns>FabricationParameterEntitiy list.</returns>
        public List<FabricationParameterEntitiy> GetAllParametersBySiteCode(string siteCode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetLensStock";
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteCode));
            return GetRecords(cmd).ToList();
        }

        protected override FabricationParameterEntitiy FillRecord(System.Data.IDataReader dr)
        {
            var fpe = new FabricationParameterEntitiy();
            fpe.ID = dr.ToInt32("ID");
            fpe.Material = dr.AsString("Material");
            fpe.Cylinder = dr.ToDecimal("Cylinder");
            fpe.MaxPlus = dr.ToDecimal("MaxPLus");
            fpe.MaxMinus = dr.ToDecimal("MaxMinus");
            fpe.IsStocked = dr.ToBoolean("IsStocked");
            fpe.CapabilityType = dr.AsString("Capability");
            return fpe;
        }
    }
}