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
    public sealed class FrameImageRepository : RepositoryBase<FrameImageEntity>, IFrameImageRepository
    {

        /// <summary>
        /// Default ctor.
        /// </summary>
        public FrameImageRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        /// <summary>
        /// Gets a Frame Image.
        /// </summary>
        /// <returns>FrameImageEntity list of all frames.</returns>
        public List<FrameImageEntity> GetFrameImage(String FrameCode, String FrameFamily)
        {
            var Command = this.DAL.GetCommandObject();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "GetFrameImagesByFrameCodeandFamily";

            Command.Parameters.Add(this.DAL.GetParamenter("@FrameCode", FrameCode));
            Command.Parameters.Add(this.DAL.GetParamenter("@FrameFamily", FrameFamily));

            Command.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));
            return GetRecords(Command).ToList();
        }

        /// <summary>
        /// Gets a Frame Image.
        /// </summary>
        /// <returns>FrameImageEntity list of all frames.</returns>
        public List<FrameImageEntity> GetFrame(int RecordID)
        {
            var Command = this.DAL.GetCommandObject();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "GetFrameImage_AllInfoByID";

            Command.Parameters.Add(this.DAL.GetParamenter("@ID", RecordID));

            Command.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));
            return GetRecords(Command).ToList();
        }

        /// <summary>
        /// Gets all frames for a site by priority.
        /// </summary>
        /// <param name="FrameFamily">Priority to narrow down frame list with.</param>
        /// <returns>Dictionary containing frame data for a ddl.</returns>
        public Dictionary<String, String> GetFramesByFrameFamily(int FamilyID)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandText = "GetFrameByFrameFamily";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(DAL.GetParamenter("@FamilyID", FamilyID));

            return DAL.GetData(cmd).Rows.Cast<DataRow>().ToList().Select(x => new { Key = x["FrameLongDescription"].ToString(), Value = x["FrameCode"].ToString() }).Distinct().ToDictionary(d => d.Key, d => d.Value);
        }

        /// <summary>
        /// Adds Updates a new Frame Image to the database.
        /// </summary>
        /// <param name="FrameImage">FrameImage entity to add.</param>
        /// <returns>Success/failure of insert.</returns>
        public Boolean InsertUpdateFrameImage(FrameImageEntity FrameImage)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "IUFrameImages";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameFamily", FrameImage.FrameFamily));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", FrameImage.FrameCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ImgName", FrameImage.ImgName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ImgPath", FrameImage.ImgPath));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameImg", FrameImage.FrameImage));
            cmd.Parameters.Add(this.DAL.GetParamenter("@DateLoaded", FrameImage.DateLoaded));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", FrameImage.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ContentType", FrameImage.ContentType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Color", FrameImage.Color));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", FrameImage.ISActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@BridgeSize", FrameImage.BridgeSize));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EyeSize", FrameImage.EyeSize));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Temple", FrameImage.Temple));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MfgName", FrameImage.MFGName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ImgAngle", FrameImage.ImgAngle));

            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));


            InsertData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1);
        }

        /// <summary>
        /// Adds Updates a new Frame Image to the database.
        /// </summary>
        /// <param name="FrameImage">FrameImage entity to add.</param>
        /// <returns>Success/failure of insert.</returns>
        public Boolean UpdateFrameImage(FrameImageEntity FrameImage)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "IUFrameImages";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", FrameImage.ID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ImgName", FrameImage.ImgName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameImg", FrameImage.FrameImage));
            cmd.Parameters.Add(this.DAL.GetParamenter("@DateLoaded", FrameImage.DateLoaded));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", FrameImage.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ContentType", FrameImage.ContentType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MfgName", FrameImage.MFGName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", FrameImage.ISActive));

            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));


            UpdateData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32().Equals(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="Ordinal"></param>
        /// <returns></returns>
        public static byte[] ReadByteArray(IDataRecord dr, int Ordinal)
        {
            long length = dr.GetBytes(Ordinal, 0, null, 0, int.MaxValue);
            var buffer = new byte[length];
            dr.GetBytes(Ordinal, 0, buffer, 0, (int)length);
            return buffer;

        }

        /// <summary>
        /// Fill Frame Image Record
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected override FrameImageEntity FillRecord(IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            var fie = new FrameImageEntity();
            fie.ID = dr.ToInt32("ID");
            fie.FrameFamily = dr.AsString("FrameFamily", c);
            fie.FrameCode = dr.AsString("FrameCode", c);
            fie.ImgName = dr.AsString("ImgName", c);
            fie.ImgPath = dr.AsString("ImgPath", c);
            fie.FrameImage = ReadByteArray(dr, dr.GetOrdinal("FrameImg"));
            fie.DateLoaded = dr.ToDateTime("DateLoaded", c);
            fie.ModifiedBy = dr.AsString("ModifiedBy", c);
            fie.ContentType = dr.AsString("ContentType", c);
            fie.Color = dr.AsString("Color", c);
            fie.ISActive = dr.ToBoolean("IsActive", c);
            fie.BridgeSize = dr.ToInt32("BridgeSize", c);
            fie.Temple = dr.AsString("Temple", c);
            fie.EyeSize = dr.ToInt32("EyeSize", c);
            fie.ImgAngle = dr.AsString("ImgAngle", c);
            fie.MFGName = dr.AsString("MfgName", c);
            return fie;
        }

    }
}
