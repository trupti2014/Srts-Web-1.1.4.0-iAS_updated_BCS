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
    public sealed class SiteRepository
    {
        /// <summary>
        /// A custom repository class to handle site code operations.
        /// </summary>
        public class SiteCodeRepository : RepositoryBase<SiteCodeEntity>, ISiteCodeRepository
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public SiteCodeRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Adds a new site to the database.
            /// </summary>
            /// <param name="sce">SiteCodeEntity to add.</param>
            /// <returns>Success/failure of insert.</returns>
            public Boolean InsertSite(SiteCodeEntity sce)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertSite";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", sce.SiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteName", sce.SiteName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteType", sce.SiteType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteDescription", sce.SiteDescription));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EMailAddress", sce.EMailAddress));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DSNPhoneNumber", sce.DSNPhoneNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RegPhoneNumber", sce.RegPhoneNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@BOS", sce.BOS));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsMultiVision", sce.IsMultivision));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsAPOCompatible", sce.IsAPOCompatible));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MaxEyeSize", sce.MaxEyeSize));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MaxFramesPerMonth", sce.MaxFramesPerMonth));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MaxPower", sce.MaxPower));
                cmd.Parameters.Add(this.DAL.GetParamenter("@HasLMS", sce.HasLMS));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipToPatientLab", sce.ShipToPatientLab));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Region", sce.Region));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MultiPrimary", sce.MultiPrimary));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MultiSecondary", sce.MultiSecondary));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SinglePrimary", sce.SinglePrimary));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SingleSecondary", sce.SingleSecondary));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", sce.IsActive));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsReimbursable", sce.IsReimbursable));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", sce.ModifiedBy));

                return GetRecords(cmd).ToList().Count > 0;
            }

            /// <summary>
            /// Updates an existing site in the database.
            /// </summary>
            /// <param name="sce">SiteCodeEntity to update.</param>
            /// <returns>Success/failure of update.</returns>
            public Boolean UpdateSite(SiteCodeEntity sce)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateSite";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", sce.SiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteName", sce.SiteName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteType", sce.SiteType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteDescription", sce.SiteDescription));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EMailAddress", sce.EMailAddress));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DSNPhoneNumber", sce.DSNPhoneNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RegPhoneNumber", sce.RegPhoneNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", sce.IsActive));
                cmd.Parameters.Add(this.DAL.GetParamenter("@BOS", sce.BOS));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsMultiVision", sce.IsMultivision));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsAPOCompatible", sce.IsAPOCompatible));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MaxEyeSize", sce.MaxEyeSize));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MaxFramesPerMonth", sce.MaxFramesPerMonth));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MaxPower", sce.MaxPower));
                cmd.Parameters.Add(this.DAL.GetParamenter("@HasLMS", sce.HasLMS));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipToPatientLab", sce.ShipToPatientLab));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Region", sce.Region));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MultiPrimary", sce.MultiPrimary));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MultiSecondary", sce.MultiSecondary));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SinglePrimary", sce.SinglePrimary));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SingleSecondary", sce.SingleSecondary));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", sce.ModifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsReimbursable", sce.IsReimbursable));

                return GetRecords(cmd).ToList().Count > 0;
            }

            /// <summary>
            /// Gets a list of sites.
            /// </summary>
            /// <returns>SiteCodeEntity list of all sites.</returns>
            public List<SiteCodeEntity> GetAllSites()
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetAllSites";
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of all site codes.
            /// </summary>
            /// <returns>String list of all site codes.</returns>
            public List<String> GetAllSiteCodes()
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetAllSiteCodes";
                return GetObjects(cmd).Cast<String>().ToList();
            }

            /// <summary>
            /// Gets a list of all site codes for an individual.
            /// </summary>
            /// <param name="indivID"></param>
            /// <returns></returns>
            public List<String> GetIndivSiteCodes(int indivID)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetIndivSiteCodes";
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndivID", indivID));
                return GetObjects(cmd).Cast<String>().ToList();
            }

            /// <summary>
            /// Gets a list of sites by site type (LAB, CLINIC, ADMIN).
            /// </summary>
            /// <param name="siteType">Site type to search for.</param>
            /// <returns>SiteCodeEntity list of sites.</returns>
            public List<SiteCodeEntity> GetSitesByType(string siteType)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetSitesBySiteType";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteType", siteType));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of sites by site type (LAB, CLINIC, ADMIN) and region code.
            /// </summary>
            /// <param name="siteType">Site type to search for.</param>
            /// <param name="region">Region to search in.</param>
            /// <returns>String list of sites.</returns>
            public List<String> GetSitesByTypeAndRegion(string siteType, int region)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetSitesBySiteTypeAndRegion";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteType", siteType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Region", region));
                return GetObjects(cmd).Cast<String>().ToList();
            }

            /// <summary>
            /// Gets a list of sites by site ID.
            /// </summary>
            /// <param name="siteID">Site ID to search for.</param>
            /// <returns>SiteCodeEntity list of sites.</returns>
            public List<SiteCodeEntity> GetSiteBySiteID(string siteID)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetSiteBySiteID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteID));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of sites by site ID.
            /// </summary>
            /// <param name="siteID">Site ID to search for.</param>
            /// <returns>SiteCodeEntity list of sites.</returns>
            public List<SiteCodeEntity> GetTempSiteBySiteID(string siteID)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTempSiteBySiteID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteID));
                return GetRecords(cmd).ToList();
            }
            /// <summary>
            /// Retrieves information for a specific clinic on their labs ability to ship to patient.
            /// </summary>
            /// <param name="SiteCode">clinic site code to search with.</param>
            /// <returns>Dictionary containing frame data for a ddl.</returns>
            public List<DataRow> GetLabMTPStatus(String SiteCode)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "GetShipToPatientLabs";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", SiteCode));
                return DAL.GetData(cmd).Rows.Cast<DataRow>().ToList();
            }




            protected override SiteCodeEntity FillRecord(IDataReader dr)
            {
                var c = dr.GetColumnNameList();
                var se = new SiteCodeEntity();
                se.ID = dr.ToInt32("ID", c);
                se.AddressType = dr.AsString("AddressType", c);// dr.Table.Columns.Contains("AddressType") ? dr.AsString("AddressType") : String.Empty;
                se.Address1 = dr.AsString("Address1", c);
                se.Address2 = dr.AsString("Address2", c);
                se.Address3 = dr.AsString("Address3", c);
                se.City = dr.AsString("City", c);
                se.Country = dr.AsString("Country", c);
                se.State = dr.AsString("State", c);
                se.ZipCode = dr.AsString("ZipCode", c);
                se.DateLastModified = dr.ToDateTime("DateLastModified", c);
                se.DSNPhoneNumber = dr.AsString("DSNPhoneNumber", c);
                se.EMailAddress = dr.AsString("EMailAddress", c);
                se.IsActive = dr.ToBoolean("IsActive", c);
                se.IsConus = dr.ToBoolean("IsConus", c);
                se.IsReimbursable = dr.ToBoolean("IsReimbursable", c);
                se.ModifiedBy = dr.AsString("ModifiedBy", c);
                se.RegPhoneNumber = dr.AsString("RegPhoneNumber", c);
                se.SiteDescription = dr.AsString("SiteDescription", c);
                se.SiteCode = dr.AsString("SiteCode", c);
                se.SiteName = dr.AsString("SiteName", c);
                se.SiteType = dr.AsString("SiteType", c);
                se.BOS = dr.AsString("BOS", c);
                se.BOSDescription = se.BOS.ToBOSValue(); // dr.AsString("BOS", c).ToBOSValue();
                se.IsMultivision = dr.ToBoolean("IsMultivision", c);
                se.IsAPOCompatible = dr.ToBoolean("IsAPOCompatible", c);
                se.MaxEyeSize = dr.ToInt32("MaxEyeSize", c);
                se.MaxFramesPerMonth = dr.ToInt32("MaxFramesPerMonth", c);
                se.MaxPower = dr.ToDouble("MaxPower", c);
                se.HasLMS = dr.ToBoolean("HasLMS", c);
                se.ShipToPatientLab = dr.ToBoolean("ShipToPatientLab", c);
                se.Region = dr.ToInt32("Region", c);
                se.MultiPrimary = dr.AsString("MultiPrimary", c);
                se.MultiSecondary = dr.AsString("MultiSecondary", c);
                se.SinglePrimary = dr.AsString("SinglePrimary", c);
                se.SingleSecondary = dr.AsString("SingleSecondary", c);
                return se;
            }
        }

        /// <summary>
        /// A custom repository class to do site address operations.
        /// </summary>
        public class SiteAddressRepository : RepositoryBase<SiteAddressEntity>, ISiteAddressRepository
        {
            /// <summary>
            /// Default ctlr.
            /// </summary>
            public SiteAddressRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Gets a list of sites addresses by site code.
            /// </summary>
            /// <param name="siteID">Site code to search with.</param>
            /// <returns>SiteAddressEntity list of a sites addresses.</returns>
            public List<SiteAddressEntity> GetSiteAddressBySiteID(string siteID)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetSiteAddressBySiteID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteID));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Adds a new site address to the dataabse.
            /// </summary>
            /// <param name="sae">Site address to insert.</param>
            /// <returns>Success/failure of insert.</returns>
            public Boolean InsertSiteAddress(SiteAddressEntity sae)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertSiteAddress";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", sae.SiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Address1", sae.Address1));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Address2", sae.Address2));
                cmd.Parameters.Add(this.DAL.GetParamenter("@AddressType", sae.AddressType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Address3", sae.Address3));
                cmd.Parameters.Add(this.DAL.GetParamenter("@City", sae.City));
                cmd.Parameters.Add(this.DAL.GetParamenter("@State", sae.State));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Country", sae.Country));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ZipCode", sae.ZipCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsConus", sae.IsConus));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", sae.IsActive));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", sae.ModifiedBy));

                return GetRecords(cmd).ToList().Count > 0;
            }

            /// <summary>
            /// Updates a sites address in the database.
            /// </summary>
            /// <param name="sae">Site address to update.</param>
            /// <returns>Success/failure of update.</returns>
            public Boolean UpdateSiteAddress(SiteAddressEntity sae)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateSiteAddress";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", sae.SiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Address1", sae.Address1));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Address2", sae.Address2));
                cmd.Parameters.Add(this.DAL.GetParamenter("@AddressType", sae.AddressType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Address3", sae.Address3));
                cmd.Parameters.Add(this.DAL.GetParamenter("@City", sae.City));
                cmd.Parameters.Add(this.DAL.GetParamenter("@State", sae.State));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Country", sae.Country));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ZipCode", sae.ZipCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsConus", sae.IsConus));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", sae.IsActive));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", sae.ModifiedBy));

                return GetRecords(cmd).ToList().Count > 0;
            }

            protected override SiteAddressEntity FillRecord(IDataReader dr)
            {
                var c = dr.GetColumnNameList();
                SiteAddressEntity ae = new SiteAddressEntity();
                ae.ID = dr.ToInt32("ID", c);
                ae.Address1 = dr.AsString("Address1", c);
                ae.Address2 = dr.AsString("Address2", c);
                ae.Address3 = dr.AsString("Address3", c);
                ae.ID = dr.ToInt32("ID", c);
                ae.SiteCode = dr.AsString("SiteCode", c);
                ae.AddressType = dr.AsString("AddressType", c);
                ae.City = dr.AsString("City", c);
                ae.Country = dr.AsString("Country", c);
                ae.DateLastModified = dr.ToDateTime("DateLastModified", c);
                ae.IsConus = dr.ToBoolean("IsConus", c);
                ae.IsActive = dr.ToBoolean("IsActive", c);
                ae.ModifiedBy = dr.AsString("ModifiedBy", c);
                ae.State = dr.AsString("State", c);
                ae.ZipCode = dr.AsString("ZipCode", c).ToZipCodeDisplay();
                return ae;
            }
        }

        /// <summary>
        /// A custom repository class to do site administrator operations.
        /// </summary>
        public class SiteAdministratorsRepository : RepositoryBase<SiteAdministratorEntity>, ISiteAdministratorRepository
        {
            /// <summary>
            /// Default ctlr.
            /// </summary>
            public SiteAdministratorsRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Gets a list of sites administrators info by site code.
            /// </summary>
            /// <param name="siteID">Site code to search with.</param>
            /// <returns>SiteAdministratorEntity list of a sites administrator info.</returns>
            public List<SiteAdministratorEntity> GetSiteAdminInfoBySiteID(string siteID)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetSiteAdminInfo";
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteID));
                return GetRecords(cmd).ToList();
            }


            protected override SiteAdministratorEntity FillRecord(IDataReader dr)
            {
                var c = dr.GetColumnNameList();
                SiteAdministratorEntity sae = new SiteAdministratorEntity();
                sae.SiteCode = dr.AsString("PrimarySiteCode", c);
                sae.FirstName = dr.AsString("FirstName", c);
                sae.LastName = dr.AsString("LastName", c);
                sae.EmailAdddress = dr.AsString("emailaddress", c);
                sae.RegPhoneNumber = dr.AsString("RegPhoneNumber", c);
                sae.DSNPhoneNumber = dr.AsString("DSNPhoneNumber", c);
                sae.Address1 = dr.AsString("Address1", c);
                sae.Address2 = dr.AsString("Address2", c);
                sae.Address3 = dr.AsString("Address3", c);
                sae.City = dr.AsString("City", c);
                sae.State = dr.AsString("State", c);
                sae.Country = dr.AsString("Country", c);
                sae.ZipCode = dr.AsString("ZipCode", c).ToZipCodeDisplay();
                return sae;
            }
        }


    }
}