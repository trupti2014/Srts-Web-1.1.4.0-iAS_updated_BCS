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
    public sealed class FrameFamilyRepository : RepositoryBase<FrameFamilyEntity>, IFrameFamilyRepository
    {

        /// <summary>
        /// Default ctor.
        /// </summary>
        public FrameFamilyRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }


        /// <summary>
        /// Gets Frame Family.
        /// </summary>
        /// <returns>FrameFamilyEntity list of all frame families.</returns>
        public List<FrameFamilyEntity> GetFrameFamily()
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFrameFamilyName";
            return GetRecords(cmd).ToList();
        }


        /// <summary>
        /// Fill Frame Image Record
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected override FrameFamilyEntity FillRecord(IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            var ffe = new FrameFamilyEntity();
            ffe.FamilyName = dr.AsString("FamilyName", c);
            ffe.ID = dr.ToInt32("ID", c);

            return ffe;
        }

    }
}
