using DataBaseAccessLayer;
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
    /// A custom repository class used to perform order status related operations.
    /// </summary>
    public class CheckOrderStatusRepository : RepositoryBase<CheckOrderStatusEntity>
    {
        /// <summary>
        /// Default Ctor.
        /// </summary>
        public CheckOrderStatusRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="CatalogName"></param>
        public CheckOrderStatusRepository(String CatalogName)
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.DefaultConnStrNm), CatalogName)
        { }

        /// <summary>
        /// Gets a list of orders and corresponding order sstatuses for a paitent.
        /// </summary>
        /// <param name="IndividualId">Individual ID to search for.</param>
        /// <returns>CheckOrderStatusEntity list for a patient.</returns>
        public List<CheckOrderStatusEntity> GetPatientOrdersAndStatuses(Int32 IndividualId)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandText = "GetPatientFacingOrders";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(this.DAL.GetParamenter("@Patient", IndividualId));

            var lo = GetRecords(cmd).ToList();

            return lo;
        }

        protected override CheckOrderStatusEntity FillRecord(System.Data.IDataReader dr)
        {
            var c = new CheckOrderStatusEntity();

            c.Bridge = dr.AsString("FrameBridgeSize");
            c.ClinicSiteCode = dr.AsString("Clinic");
            c.Color = dr.AsString("FrameColor");
            c.CurrentStatus = dr.AsString("StatusComment");
            c.CurrentStatusDate = dr.ToDateTime("DateLastModified");
            c.Eye = dr.AsString("FrameEyeSize");
            c.Frame = dr.AsString("FrameCode");
            c.LabSiteCode = dr.AsString("LabSiteCode");
            c.Lens = dr.AsString("LensType");
            c.OrderDate = dr.ToDateTime("OrderDate");
            c.OrderNumber = dr.AsString("OrderNumber");
            c.Temple = dr.AsString("FrameTempleType");
            c.Tint = dr.AsString("Tint");

            return c;
        }
    }
}