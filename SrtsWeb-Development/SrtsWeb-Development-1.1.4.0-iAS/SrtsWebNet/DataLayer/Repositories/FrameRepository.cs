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
    /// A custom repository class to handle frame operations.
    /// </summary>
    public sealed class FrameRepository : RepositoryBase<FrameEntity>, IFrameRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public FrameRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        protected override FrameEntity FillRecord(IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            var fe = new FrameEntity();
            fe.FrameCode = dr.AsString("FrameCode", c);
            fe.DateLastModified = dr.ToDateTime("DateLastModified", c);
            fe.FrameDescription = dr.AsString("FrameDescription", c);
            fe.FrameNotes = dr.AsString("FrameNotes", c);
            fe.IsActive = dr.ToBoolean("IsActive", c);
            fe.IsInsert = dr.ToBoolean("IsInsert", c);
            fe.ModifiedBy = dr.AsString("ModifiedBy", c);
            fe.MaxPair = dr.ToInt32("MaxPair", c);
            fe.IsFoc = dr.ToBoolean("IsFOC", c);
            fe.FrameType = dr.AsString("FrameType", c);

            return fe;
        }

        /// <summary>
        /// Gets a list of all frames.
        /// </summary>
        /// <returns>FrameEntity list of all frames.</returns>
        public List<FrameEntity> GetAllFrames()
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAllFrames";
            return GetRecords(cmd).ToList();
        }
        public List<FrameEntity> GetAllFrames(string _siteCode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAllFrames";
            cmd.Parameters.Add(this.DAL.GetParamenter("@Clinic", _siteCode));
            return GetRecords(cmd).ToList();
        }
        /// <summary>
        /// Gets a list of all frames that can have preferences associated with them.
        /// </summary>
        /// <returns>FrameEntity list of all frames that can have preferences associated with them.</returns>
        public List<FrameEntity> GetAllFrameswithPreferences()
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAllFrameswithPreferences";
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Adds a new frame to the database.
        /// </summary>
        /// <param name="frame">Frame entity to add.</param>
        /// <returns>Success/failure of insert.</returns>
        public Boolean InsertFrame(FrameEntity frame)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertFrame";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frame.FrameCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameDescription", frame.FrameDescription));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameNotes", frame.FrameNotes));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MaxPair", frame.MaxPair));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ImageURL", frame.ImageURL));

            //cmd.Parameters.Add(this.DAL.GetParamenter("@FrameImg", frame.ImageURL));

            cmd.Parameters.Add(this.DAL.GetParamenter("@IsInsert", frame.IsInsert));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", frame.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsFOC", frame.IsFoc));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", frame.ModifiedBy));

            return GetRecord(cmd) != null;
        }

        /// <summary>
        /// Modifies an existing frame in the databse.
        /// </summary>
        /// <param name="frame">Frame entity to modify.</param>
        /// <returns>Success/failure of update.</returns>
        public Boolean UpdateFrame(FrameEntity frame)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateFrame";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frame.FrameCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameDescription", frame.FrameDescription));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameNotes", frame.FrameNotes));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MaxPair", frame.MaxPair));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ImageURL", frame.ImageURL));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsInsert", frame.IsInsert));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", frame.IsActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsFOC", frame.IsFoc));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", frame.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@DateLastModified", frame.DateLastModified));

            return GetRecord(cmd) != null;
        }

        /// <summary>
        /// Gets a list of eligibilities by frame code (Grade, BOS, Status, Gender; ex. E09C11B).
        /// </summary>
        /// <param name="frameCode">Frame code to search with.</param>
        /// <returns>String list of eligibilities for a frame.</returns>
        public List<String> GetEligibilityByFrameCode(string frameCode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetEligibilityByFrameCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frameCode));
            return GetObjects(cmd).Cast<String>().ToList();
        }

        /// <summary>
        /// Gets a list of priorities by frame code (Gender, Priority; ex. BR = Both and Readiness).
        /// </summary>
        /// <param name="frameCode">Frame code to search with.</param>
        /// <returns>String list of priorities for a frame.</returns>
        public List<String> GetFrameItemPrioritiesByFrameCode(string frameCode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFrameItemEligibilityByFrameCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frameCode));
            return GetObjects(cmd).Cast<String>().ToList();
        }

        /// <summary>
        /// Gets the frame image url associated to the frame
        /// </summary>
        /// <param name="frameFamily">Frame family</param>
        /// <param name="frameColor">Frame color</param>
        /// <returns>An string of the url of the image</returns>
        public String GetFrameImageByFrameFamilyAndFrameColor(string frameFamily, string frameColor)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFrameImageUrl";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameFamily", frameFamily));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameColor", frameColor));
            return GetObject(cmd).ToString();
        }

        /// <summary>
        /// Adds new frame items and corresponding eligibilities for a specific frame.
        /// </summary>
        /// <param name="frameCode">Frame code to add items to.</param>
        /// <param name="frameItem">Frame items to add.</param>
        /// <param name="eligibility">Eligibilities to add.</param>
        /// <param name="modifiedBy">User performing operation.</param>
        public void InsertFrameItemsEligibilityAndUnion(string frameCode, List<FrameItemEntity> frameItem, List<OrderPriorityEntity> eligibility, string modifiedBy)
        {
            using (var dt = new DataTable("TVP_FrameItemsAndEligibility"))
            {
                // Create data table with column names
                dt.Columns.AddRange(new DataColumn[]{
                    new DataColumn("Value"),
                    new DataColumn("TypeEntry"),
                    new DataColumn("Text"),
                    new DataColumn("ModifiedBy"),
                    new DataColumn("FrameCode"),
                    new DataColumn("EligibilityCode"),
                    new DataColumn("ISActive"),
                    new DataColumn("Availability")
                });

                // Populate data table with data
                foreach (var f in frameItem)
                {
                    foreach (var e in eligibility)
                    {
                        var dr = dt.NewRow();
                        dr["Value"] = f.Value;
                        dr["TypeEntry"] = f.TypeEntry;
                        dr["Text"] = f.Text;
                        dr["ModifiedBy"] = modifiedBy;
                        dr["FrameCode"] = frameCode;
                        dr["EligibilityCode"] = e.OrderPriorityValue;
                        dr["ISActive"] = f.IsActive;
                        dr["Availability"] = "A";
                        dt.Rows.Add(dr);
                    }
                }

                // Insert the records into the database
                using (var cmd = this.DAL.GetCommandObject())
                {
                    //cmd.CommandText = "InsertFrameItemEligibilityAndUnion";
                    cmd.CommandText = "TVP_Insert";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(this.DAL.GetParamenter("@TVP", dt));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Add new eligibilities for a specific frame.
        /// </summary>
        /// <param name="frameCode">Frame code to add eligibilities to.</param>
        /// <param name="eligibilities">Eligibilities to add.</param>
        /// <param name="modifiedBy">User performing operation.</param>
        public void InsertFrameEligibilityDemographic(string frameCode, List<string> eligibilities, string modifiedBy)
        {
            using (var dt = new DataTable("TVP_FrameEligibility"))
            {
                dt.Columns.AddRange(new DataColumn[]{
                    new DataColumn("FrameCode"),
                    new DataColumn("Eligibility"),
                    new DataColumn("ModifiedBy"),
                    new DataColumn("DateLastModified")
                });

                // Need to send both true and false eligibilities.  That way for the update we can insert/delete any changes.
                foreach (var e in eligibilities)
                {
                    var dr = dt.NewRow();
                    dr["FrameCode"] = frameCode;
                    dr["Eligibility"] = e;
                    dr["ModifiedBy"] = modifiedBy;
                    dr["DateLastModified"] = System.DateTime.Now;

                    dt.Rows.Add(dr);
                }

                using (var cmd = this.DAL.GetCommandObject())
                {
                    cmd.CommandText = "TVP_Eligible";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(this.DAL.GetParamenter("@TVP", dt));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// A custom repository class to handle eligibility operations.
        /// </summary>
        public sealed class EligibilityPartRepository : RepositoryBase<Dictionary<string, Dictionary<string, List<string>>>>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public EligibilityPartRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Gets all frame eligibilities and eligibility components (Grade, BOS, and Status).
            /// </summary>
            /// <returns></returns>
            public Dictionary<string, Dictionary<string, List<string>>> GetFrameEligibilityParts()
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "GetFrameEligiblityParts";
                cmd.CommandType = CommandType.StoredProcedure;

                return GetRecord(cmd) as Dictionary<string, Dictionary<string, List<string>>>;
            }

            protected override Dictionary<string, Dictionary<string, List<string>>> GetRecord(IDbCommand cmdIn)
            {
                using (cmdIn)
                {
                    var dt = this.DAL.GetData(cmdIn);
                    if (dt == null) return null;
                    return FillRecord(dt);
                }
            }

            protected override Dictionary<string, Dictionary<string, List<string>>> FillRecord(DataTable dt)
            {
                var l = dt.Rows.Cast<DataRow>().ToList(); ;
                var branches = l.Select(x => x["BOS"].ToString()).Distinct().ToList();

                var bosStatGrades = new Dictionary<String, Dictionary<String, List<String>>>();

                foreach (var b in branches)
                {
                    var stats = l.Where(x => x["BOS"].ToString().ToLower() == b.ToLower()).Select(x => x["Status"].ToString()).Distinct().ToList();
                    var statGrades = new Dictionary<String, List<String>>();
                    foreach (var stat in stats)
                    {
                        var grades = l.Where(x => x["BOS"].ToString().ToLower() == b.ToLower() && x["Status"].ToString().ToLower() == stat.ToLower()).Select(x => x["Grade"].ToString()).ToList();
                        statGrades.Add(stat, grades);
                    }
                    bosStatGrades.Add(b, statGrades);
                }

                return bosStatGrades;
            }
        }
        public sealed class FrameRxRestrictions : RepositoryBase<FrameRxRestrictionsEntity>, IFrameRxRestrictions
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public FrameRxRestrictions()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Get frame restrictions by frame code.
            /// </summary>
            /// <param name="frameCode">a FrameCode to look up</param>
            /// <returns></returns>
            public FrameRxRestrictionsEntity GetFrameRxRestrictionsByFrameCode(string frameCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetFrameRxRestrictions";//GetFrameRxRestrictionsByFrameCode
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frameCode));
                return GetRecord(cmd);
            }

            /// <summary>
            /// Fill record of order display entity
            /// </summary>
            /// <param name="dr"></param>
            /// <returns></returns>
            protected override FrameRxRestrictionsEntity FillRecord(IDataReader dr)
            {
                FrameRxRestrictionsEntity oe = new FrameRxRestrictionsEntity();

                // Use these values if null is returned
                decimal maximum = 20.0M;
                decimal minimum = -20.0M;

                oe.FrameCode = dr.AsString("FrameCode");
                oe.MaxCylinder = dr.IsDBNull("MaxCylinder") ? maximum : dr.ToDecimal("MaxCylinder");
                oe.MinCylinder = dr.IsDBNull("MinCylinder") ? minimum : dr.ToDecimal("MinCylinder");
                oe.MaxSphere = dr.IsDBNull("MaxSphere") ? maximum : dr.ToDecimal("MaxSphere");
                oe.MinSphere = dr.IsDBNull("MinSphere") ? minimum : dr.ToDecimal("MinSphere");
                oe.Material= dr.AsString("Material");

                return oe;
            }
        }
    }
}