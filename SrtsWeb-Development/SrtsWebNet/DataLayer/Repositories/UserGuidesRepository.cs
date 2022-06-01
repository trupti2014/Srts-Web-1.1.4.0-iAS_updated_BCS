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
    /// A custom repository class to do user guide operations.
    /// </summary>
    public sealed class UserGuidesRepository : RepositoryBase<ReleaseManagementUserGuideEntity>, IUserGuidesRepository
    {
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public UserGuidesRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Adds a new user guide to the database.
        /// </summary>
        /// <param name="param">User Guide to add.</param>
        /// <returns>Success/failure of insert.</returns>
        public bool InsertUpdateUserGuide(ReleaseManagementUserGuideEntity param)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "IUUserGuide";
            cmd.Parameters.Add(this.DAL.GetParamenter("@GuideName", param.GuideName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@UserGuide", param.UserGuideDocument));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", param.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

            InsertData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1); ;
        }


        /// <summary>
        /// Delete a user guide in the database.
        /// </summary>
        /// <param name="id">User Guide to delete.</param>
        /// <returns>Success/failure of delete.</returns>
        public bool DeleteUserGuide(string guidename)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteUserGuides";
            cmd.Parameters.Add(this.DAL.GetParamenter("@GuideName", guidename));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

            UpdateData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1); ;
        }

        /// <summary>
        /// Gets a list of all user guides
        /// </summary>
        /// <returns>ReleaseManagementUserGuideEntity list.</returns>
        public List<ReleaseManagementUserGuideEntity> GetAllUserGuides()
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetUserGuides";
            return GetRecords(cmd).ToList();
        }


        /// <summary>
        /// Gets a single user guide by name.
        /// </summary>
        /// <param name="userguidename">User Guide name.</param>
        /// <returns>ReleaseManagementUserGuideEntity entity matching the userguidename.</returns>
        public ReleaseManagementUserGuideEntity GetUserGuideByName(string userguidename)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetUserGuides";
            cmd.Parameters.Add(this.DAL.GetParamenter("@GuideName", userguidename));
            var p = GetRecord(cmd);
            return p;
        }

        protected override ReleaseManagementUserGuideEntity FillRecord(System.Data.IDataReader dr)
        {
            var uge = new ReleaseManagementUserGuideEntity();
            uge.GuideName = dr.AsString("GuideName");
            uge.UserGuideDocument = (byte[])(dr["UserGuide"]);
            uge.DateLastModified= dr.ToDateTime("DateLastModified");
            uge.ModifiedBy = dr.AsString("ModifiedBy");
            return uge;
        }
    }
}