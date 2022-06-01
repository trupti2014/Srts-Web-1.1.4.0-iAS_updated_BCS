using DataBaseAccessLayer;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System.Data;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class to do frame item preferences operations.
    /// </summary>
    public class FrameItemsDefaultsRepository : RepositoryBase<FrameItemDefaultEntity>
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public FrameItemsDefaultsRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        /// <summary>
        /// Gets a list of frame item preferences for a particular frame.
        /// </summary>
        /// <param name="frameCode">Frame to get item preferences for.</param>
        /// <returns>Frame item preferences for frame.</returns>
        public FrameItemDefaultEntity GetFrameItemsDefaults(string frameCode)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFrameDefaults";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frameCode));
            return GetRecord(cmd);
        }

        protected override FrameItemDefaultEntity FillRecord(IDataReader dr)
        {
            var fe = new FrameItemDefaultEntity();
            fe.DefaultBridge = dr.AsString("BridgeSize");
            fe.DefaultEyeSize = dr.AsString("EyeSize");
            fe.DefaultTemple = dr.AsString("Temple");
            return fe;
        }
    }
}