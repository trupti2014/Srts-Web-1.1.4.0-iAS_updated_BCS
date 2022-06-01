using DataBaseAccessLayer;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class to handle frame item operations.
    /// </summary>
    public class FrameItemsRepository : RepositoryBase<FrameItemEntity>, IFrameItemsRepository
    {
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public FrameItemsRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets a list of frame items by a frame code and a patients eligibility (Demographic).
        /// </summary>
        /// <param name="patient">Patient entity that contains the demographic.</param>
        /// <param name="frameCode">Frame code to search with.</param>
        /// <returns>FrameItemEntity list for specified frame and patient.</returns>
        public List<FrameItemEntity> GetFrameItemsByFrameCodeAndEligibility(PatientEntity patient, string frameCode)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFrameItemsByFrameCodeAndEligibility";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frameCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", patient.Individual.Demographic));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a list of frame items by a frame code and a patients eligibility (Demographic).
        /// </summary>
        /// <param name="frameCode">Frame code to search with.</param>
        /// <param name="eligibility">Patient demographic to search with.</param>
        /// <returns>FrameItemEntity list for specified frame and patient.</returns>
        public List<FrameItemEntity> GetFrameItemsByFrameCodeAndEligibility(string frameCode, string eligibility)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFrameItemsByFrameCodeAndEligibility";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frameCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", eligibility));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a list of eligibilities based on a frame code.  Eligibilty is a combination of Gender and Priority; BR = Both and Readiness.
        /// </summary>
        /// <param name="frameCode">Frame code to seach with.</param>
        /// <returns>String list of eligibilities.</returns>
        public List<string> GetFrameItemEligibilityByFrameCode(string frameCode)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFrameItemEligibilityByFrameCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frameCode));
            return GetObjects(cmd).Cast<string>().ToList();
        }

        /// <summary>
        /// Gets a list of frame items by a frame code.
        /// </summary>
        /// <param name="frameCode">Frame code to search with.</param>
        /// <returns>FrameItemEntity list for a specified frame.</returns>
        public List<FrameItemEntity> GetFrameItemByFrameCode(string frameCode)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFrameItemByFrameCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frameCode));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a list of all frame items.
        /// </summary>
        /// <returns>FrameItemEntity list of all frame items.</returns>
        public List<FrameItemEntity> GetFrameItems()
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFrameItems";
            return GetRecords(cmd).ToList();
        }

        protected override FrameItemEntity FillRecord(IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            var fe = new FrameItemEntity();
            fe.Value = dr.AsString("Value", c);
            fe.TypeEntry = dr.AsString("TypeEntry", c);
            fe.DateLastModified = dr.ToDateTime("DateLastModified", c);
            fe.Text = dr.AsString("Text", c);
            fe.IsActive = dr.ToBoolean("IsActive", c);
            fe.ModifiedBy = dr.AsString("ModifiedBy", c);
            fe.Availability = dr.AsString("Availability", c);
            return fe;
        }
    }
}