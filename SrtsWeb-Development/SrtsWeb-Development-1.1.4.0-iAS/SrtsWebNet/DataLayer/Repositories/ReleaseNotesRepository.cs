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
    /// A custom repository class to do release note operations.
    /// </summary>
    public class ReleaseNotesRepository : RepositoryBase<ReleaseNote>, IReleaseNotesRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public ReleaseNotesRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets the list of release notes.
        /// </summary>
        /// <returns></returns>
        public List<ReleaseNote> GetReleaseNotes()
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetReleaseChanges";
            cmd.Parameters.Add(this.DAL.GetParamenter("@Version", null));

            return GetRecords(cmd).ToList();
        }

        protected override ReleaseNote FillRecord(IDataReader dr)
        {
            var n = new ReleaseNote()
            {
                ReleaseNotes = dr.AsString("ChangesMade"),
                VersionDate = dr.ToDateTime("VersionDate"),
                VersionNumber = dr.AsString("VersionNbr")
            };

            return n;
        }
    }
}