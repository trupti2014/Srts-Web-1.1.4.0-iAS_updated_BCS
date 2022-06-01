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
    public sealed class LabParametersRepository : RepositoryBase<LabParameterEntity>, ILabParametersRepository
    {
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public LabParametersRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Adds a new fabrication parameter to the database.
        /// </summary>
        /// <param name="param">Fabrication parameter to add.</param>
        /// <returns>Success/failure of insert.</returns>
        public bool InsertUpdateLabParameter(LabParameterEntity param)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "IUSitePref_Lab";
            cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", param.SiteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MaxPrism", param.MaxPrism));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MaxDecentrationPlus", param.MaxDecentrationPlus));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MaxDecentrationMinus", param.MaxDecentrationMinus));
            //cmd.Parameters.Add(this.DAL.GetParamenter("@PatientDirectedOrders", 0));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Comment", String.Empty));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", param.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

            InsertData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1); 
        }


        /// <summary>
        /// Gets a list of all lab parameter for a site.
        /// </summary>
        /// <param name="siteCode">Site to get lab parameters for.</param>
        /// <returns>LabParameterEntity list.</returns>
        public List<LabParameterEntity> GetLabParametersBySiteCode(string siteCode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetSitePrefLabs";
            cmd.Parameters.Add(this.DAL.GetParamenter("@LabSite", siteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
            return GetRecords(cmd).ToList();
        }


        protected override LabParameterEntity FillRecord(System.Data.IDataReader dr)
        {
            var lpe = new LabParameterEntity();
            lpe.SiteCode = dr.AsString("LabSiteCode");
            lpe.MaxPrism = dr.ToDecimal("MaxPrism");
            lpe.MaxDecentrationPlus = dr.ToDecimal("MaxDecentrationPlus");
            lpe.MaxDecentrationMinus = dr.ToDecimal("MaxDecentrationMinus");
            return lpe;
        }

    }
}