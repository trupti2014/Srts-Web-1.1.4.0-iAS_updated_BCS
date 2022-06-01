using DataBaseAccessLayer;
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
    /// A custom repository class to handle individual operations.
    /// </summary>
    public sealed class IndividualRepository : RepositoryBase<IndividualEntity>, IIndividualRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public IndividualRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        /// <summary>
        /// Adds a new record into the IndividualIDSiteCodeUnion table.
        /// </summary>
        /// <param name="individualId">Individual ID to add.</param>
        /// <param name="siteCode">Site code to add.</param>
        /// <returns>Integer that represents the success (1) or failure (0) of the insert.</returns>
        public Int32 InsertIndividualSiteCode(Int32 individualId, String siteCode)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertIndividualSiteCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualId", individualId));
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));

            InsertData(cmd);

            var p = cmd.Parameters["@Success"] as IDataParameter;
            return p.Value.ToInt32();
        }

        /// <summary>
        /// Updates an individuals record in the database.
        /// </summary>
        /// <param name="patient">Individual record to update.</param>
        /// <returns>IndividualEntity list of individuals who have the same ID as the updated individual.</returns>
        public List<IndividualEntity> UpdateIndividual(IndividualEntity patient)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateIndividualByID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", patient.ID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FirstName", patient.FirstName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@MiddleName", string.IsNullOrEmpty(patient.MiddleName) ? string.Empty : patient.MiddleName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@LastName", patient.LastName));
            //cmd.Parameters.Add(this.DAL.GetParamenter("@PersonalType", patient.PersonalType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsPOC", patient.IsPOC));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", patient.IsActive));
            if (patient.DateOfBirth != null)
            {
                cmd.Parameters.Add(this.DAL.GetParamenter("@DateOfBirth", patient.DateOfBirth));
            }
            else
            {
                cmd.Parameters.Add(this.DAL.GetParamenter("@DateOfBirth", DBNull.Value));
            }
            cmd.Parameters.Add(this.DAL.GetParamenter("@EADStopDate", patient.EADStopDate));
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCodeID", patient.SiteCodeID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Comments", patient.Comments));
            cmd.Parameters.Add(this.DAL.GetParamenter("@LocationCode", string.IsNullOrEmpty(patient.TheaterLocationCode) ? string.Empty : patient.TheaterLocationCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", patient.Demographic));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", patient.ModifiedBy));

            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Makes a users CMS enabled/disabled.
        /// </summary>
        /// <param name="userName">User name to enable CMS for.</param>
        /// <param name="individualId">Individual ID that is linked to the user name.</param>
        /// <param name="forRemove">Flag to determine if the user should be unsynced.</param>
        public void SyncUserToIndividual(String userName, Int32 individualId, Boolean forRemove = false)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandText = "SyncUserToIndividual";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(this.DAL.GetParamenter("@aspnetUserName", userName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@individualId", individualId));
            cmd.Parameters.Add(this.DAL.GetParamenter("@doRemove", forRemove));

            InsertData(cmd);
        }

        /// <summary>
        /// Performs a search for an individual by either name, and/or ID number.  The search can be performed at a specific site or globally.
        /// </summary>
        /// <param name="IDNumber">ID number to search with.</param>
        /// <param name="lastName">Last name to search with.</param>
        /// <param name="firstName">First name to search with.  Last name is required for this parameter.</param>
        /// <param name="siteCode">Site code to search in.</param>
        /// <param name="personalType">Personal type to search with.</param>
        /// <param name="isActive">Flag to search for active/inactive records.</param>
        /// <param name="modifiedBy">User performing search operation.</param>
        /// <param name="RecCnt">Output - number of records returned.</param>
        /// <returns>IndividualEntity list of individuals from search.</returns>
        public List<IndividualEntity> FindIndividualByLastnameOrLastFour(string IDNumber, string lastName, string firstName, string siteCode, string personalType, bool isActive, string modifiedBy, out int RecCnt)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SearchForPatients";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumber", IDNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@LastName", lastName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FirstName", firstName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCodeID", siteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PersonalType", personalType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", isActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@RecCnt", "", ParameterDirection.Output));

            var r = GetRecords(cmd).ToList();
            var p = cmd.Parameters["@RecCnt"] as IDataParameter;
            RecCnt = p.Value.ToInt32();
            return r;
        }

        /// <summary>
        /// Gets a list of individuals by an ID number and ID number type.
        /// </summary>
        /// <param name="IDNumber">ID number to search with.</param>
        /// <param name="IDNumberType">ID number type to search with.</param>
        /// <param name="modifiedBy">User performing search operation.</param>
        /// <returns>IndividualEntity list of individuals matching an ID number and type.</returns>
        public List<IndividualEntity> GetIndividualByIDNumberIDNumberType(String IDNumber, String IDNumberType, String modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetIndividualByIDNumberAndIDNumberType";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumber", IDNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumberType", IDNumberType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets an individual record based on an individual ID.  It is a list return but only one record in the list.
        /// </summary>
        /// <param name="individualID">Individual ID to search with.</param>
        /// <param name="modifiedBy">User performing search operation.</param>
        /// <returns>IndividualEntity list of a single individual.</returns>
        public List<IndividualEntity> GetIndividualByIndividualID(int individualID, string modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetIndividualByIndividualID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ID", individualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a list of a type of individuals at a site by site code and personal type (Provider, Technician, ...).
        /// </summary>
        /// <param name="siteCode">Site code to search in.</param>
        /// <param name="personalType">Personal type to search with.</param>
        /// <param name="modifiedBy">User performing search operation.</param>
        /// <returns>IndividualEntity list of individuals.</returns>
        public List<IndividualEntity> GetIndividualBySiteCodeAndPersonalType(string siteCode, string personalType, string modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetIndividualBySiteCodeAndPersonalType";
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PersonalType", personalType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a list of all individuals at a site by site code.
        /// </summary>
        /// <param name="siteCode">Site code to search with.</param>
        /// <param name="modifiedBy">User performing search operation.</param>
        /// <returns>IndividualEntity list of individuals at a site.</returns>
        public List<IndividualEntity> GetAllIndividualsAtSite(string siteCode, string modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandText = "GetIndividualsAtSite";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(this.DAL.GetParamenter("@siteCode", siteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets a flag of weather a user can user CMS based on an individual ID.
        /// </summary>
        /// <param name="individualId">Individual ID to search with.</param>
        /// <returns>CMS enabled flag.</returns>
        public Boolean GetSyncedUser(Int32 individualId)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandText = "GetSyncedUser";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(this.DAL.GetParamenter("@UserID", individualId));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));

            var d = base.GetOutputParameterValues(cmd);
            Object success;
            d.TryGetValue("@Success", out success);
            return success.ToInt32().Equals(1);
        }

        /// <summary>
        /// Gets an individual id by searching with a user name.
        /// </summary>
        /// <param name="userName">User name to search with.</param>
        /// <returns>Individual ID.</returns>
        public Int32 GetIndividualIdByUserName(String userName)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandText = "GetIndividualIdByUserName";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(this.DAL.GetParamenter("@userName", userName));

            return GetObject(cmd).ToInt32();
        }

        protected override IndividualEntity FillRecord(IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            IndividualEntity ie = new IndividualEntity();
            ie.Comments = dr.AsString("Comments", c);
            ie.DateLastModified = dr.ToDateTime("DateLastModified", c);
            ie.DateOfBirth = dr.ToDateTime("DateOfBirth", c);
            ie.EADStopDate = dr.ToNullableDateTime("EADStopDate", c);
            ie.NextFocDate = dr.ToNullableDateTime("NextFOCDate", c);
            ie.FirstName = dr.AsString("FirstName", c);
            ie.Demographic = dr.AsString("Demographic", c);
            ie.ID = dr.ToInt32("ID", c);
            ie.IDNumberType = dr.AsString("IDNumberType", c);
            ie.IDNumber = dr.AsString("IDNumber", c);
            if (!string.IsNullOrEmpty(ie.IDNumberType))
            {
                ie.IDNumberTypeDescription = Helpers.GetIDTypeDescription(ie.IDNumberType);
            }

            if (string.IsNullOrEmpty(ie.IDNumber))
            {
                ie.IDNumber = dr.AsString("IDNbrs", c);
                ie.IDNumberTypeDescription = dr.AsString("IDType", c);
            }

            ie.IsActive = dr.ToBoolean("IsActive", c);
            ie.IsPOC = dr.ToBoolean("IsPOC", c);
            ie.LastName = dr.AsString("LastName", c);
            ie.TheaterLocationCode = dr.AsString("TheaterLocationCode", c);
            ie.MiddleName = dr.AsString("MiddleName", c);
            ie.ModifiedBy = dr.AsString("ModifiedBy", c);
            ie.SiteCodeID = dr.AsString("SiteCodeID", c);
            ie.PersonalType = dr.AsString("PersonalType", c);
            ie.BOSDescription = ie.Demographic.ToBOSValue();
            ie.StatusDescription = ie.Demographic.ToPatientStatusValue();
            ie.IsNewPatient = dr.ToBoolean("IsPatient", c);
            //ie.IsNewPatient = !dr.ToBoolean("IsPatient", c);

            return ie;
        }

        /// <summary>
        /// A custom repository class to handle patient select operations.
        /// </summary>
        public sealed class PatientRepository : RepositoryBase<PatientEntity>, IPatientRepository
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public PatientRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Gets a complete patient record using ID number and ID number type.
            /// </summary>
            /// <param name="_IDNumber">ID number to search with.</param>
            /// <param name="_IDType">ID number type to search with.</param>
            /// <returns>Patient record.</returns>
            public PatientEntity GetAllPatientInfoByIDNumber(string _IDNumber, string _IDType, string _clinicCode)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetAllPatientInfoByIDNumber";
                cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumber", _IDNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@clinic", _clinicCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IDType", _IDType));
                return GetRecord(cmd);
            }
            //public PatientEntity GetAllPatientInfoByIDNumber(string _IDNumber, string _IDType, string _clinicCode)
            //{
            //    var cmd = this.DAL.GetCommandObject();

            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.CommandText = "GetAllPatientInfoByIDNumber";
            //    cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumber", _IDNumber));
            //    cmd.Parameters.Add(this.DAL.GetParamenter("@IDType", _IDType));
            //    cmd.Parameters.Add(this.DAL.GetParamenter("@Clinic", _clinicCode));
            //    return GetRecord(cmd);
            //}
            /// <summary>
            /// Gets a complete patient record using individual ID.
            /// </summary>
            /// <param name="individualID">Individual ID to search with.</param>
            /// <param name="isActive">Search for an active record?</param>
            /// <param name="modifiedBy">User performing the search.</param>
            /// <param name="usersSiteCode">Site code to search in.</param>
            /// <returns>Patient record.</returns>
            public PatientEntity GetAllPatientInfoByIndividualID(int individualID, bool isActive, string modifiedBy, string usersSiteCode)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetAllPatientInfoByIndividualID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", individualID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", isActive));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", usersSiteCode));

                return GetRecord(cmd);
            }

            protected override PatientEntity GetRecord(IDbCommand cmdIn)
            {
                using (cmdIn)
                {
                    var ds = this.DAL.GetDataSet(cmdIn);
                    if (ds == null) return new PatientEntity();
                    return FillRecord(ds);
                }
            }

            protected override PatientEntity FillRecord(DataSet ds)
            {
                var pe = new PatientEntity();

                if (ds.IsNull() || ds.Tables.Count.Equals(0) || ds.Tables[0].Rows.Count.Equals(0)) return pe;

                pe.Individual = ProcessIndividualRow(ds.Tables[0].Rows[0]);
                pe.IDNumbers = ProcessIdentificationNumberTable(ds.Tables[1]);
                pe.EMailAddresses = ProcessEMailAddressTable(ds.Tables[2]);
                pe.PhoneNumbers = ProcessPhoneTable(ds.Tables[3]);
                pe.Addresses = ProcessAddressTable(ds.Tables[4]);
                pe.IndividualTypes = ProcessIndividualTypeTable(ds.Tables[5]);

                if (ds.Tables.Count > 6)
                {
                    pe.Prescriptions = ProcessPresciptionTable(ds.Tables[7]);
                }

                return pe;
            }

            private IndividualEntity ProcessIndividualRow(DataRow dr)
            {
                var ie = new IndividualEntity();
                ie.Comments = dr.AsString("Comments");
                ie.DateLastModified = dr.ToDateTime("DateLastModified");
                ie.DateOfBirth = dr.ToDateTime("DateOfBirth");
                ie.EADStopDate = dr.ToNullableDateTime("EADStopDate");
                ie.NextFocDate = dr.ToNullableDateTime("NextFocDate");
                ie.FirstName = dr.AsString("FirstName");
                ie.Demographic = dr.AsString("Demographic");
                ie.ID = dr.ToInt32("ID");
                ie.IDNumberType = dr.AsString("IDNumberType");
                ie.IDNumber = dr.AsString("IDNumber");
                ie.IDNumberTypeDescription = Helpers.GetIDTypeDescription(ie.IDNumberType);

                if (string.IsNullOrEmpty(ie.IDNumber))
                {
                    ie.IDNumber = dr.AsString("IDNbrs");
                    ie.IDNumberTypeDescription = dr.AsString("IDType");
                }

                ie.IsActive = dr.ToBoolean("IsActive");
                ie.IsPOC = dr.ToBoolean("IsPOC");
                ie.LastName = dr.AsString("LastName");
                ie.TheaterLocationCode = dr.AsString("TheaterLocationCode");
                ie.MiddleName = dr.AsString("MiddleName");
                ie.ModifiedBy = dr.AsString("ModifiedBy");
                ie.SiteCodeID = dr.AsString("SiteCodeID");
                ie.PersonalType = dr.AsString("PersonalType");
                ie.BOSDescription = ie.Demographic.ToBOSValue();
                ie.StatusDescription = ie.Demographic.ToPatientStatusValue();
                if (dr.Table.Columns.Contains("IsPatient"))
                    ie.IsNewPatient = !dr.ToBoolean("IsPatient");

                return ie;
            }

            private List<IdentificationNumbersEntity> ProcessIdentificationNumberTable(DataTable dt)
            {
                var line = new List<IdentificationNumbersEntity>();
                foreach (DataRow dr in dt.Rows)
                {
                    var ine = new IdentificationNumbersEntity();
                    ine.DateLastModified = dr.ToDateTime("DateLastModified");
                    ine.ID = dr.ToInt32("ID");
                    ine.IndividualID = dr.ToInt32("IndividualID");
                    ine.IsActive = dr.ToBoolean("IsActive");
                    ine.ModifiedBy = dr.AsString("ModifiedBy");
                    ine.IDNumber = dr.AsString("IDNumber");
                    ine.IDNumberType = dr.AsString("IDNumberType");
                    ine.IDNumberTypeDescription = Helpers.GetIDTypeDescription(ine.IDNumberType);

                    line.Add(ine);
                }
                return line;
            }

            private List<EMailAddressEntity> ProcessEMailAddressTable(DataTable dt)
            {
                var lea = new List<EMailAddressEntity>();
                foreach (DataRow dr in dt.Rows)
                {
                    var ea = new EMailAddressEntity();
                    ea.DateLastModified = dr.ToDateTime("DateLastModified");
                    ea.EMailAddress = dr.AsString("EMailAddress");
                    ea.ID = dr.ToInt32("ID");
                    //ea.EMailType = dr.AsString("EMailType");
                    ea.IndividualID = dr.ToInt32("IndividualID");
                    ea.IsActive = dr.ToBoolean("IsActive");
                    ea.ModifiedBy = dr.AsString("ModifiedBy");

                    lea.Add(ea);
                }
                return lea;
            }

            private List<PhoneNumberEntity> ProcessPhoneTable(DataTable dt)
            {
                var lpe = new List<PhoneNumberEntity>();
                foreach (DataRow dr in dt.Rows)
                {
                    var pe = new PhoneNumberEntity();
                    pe.IndividualID = dr.ToInt32("IndividualID");
                    pe.DateLastModified = dr.ToDateTime("DateLastModified");

                    var testPhone = dr.AsString("PhoneNumber");
                    if (!testPhone.Contains("-") && testPhone.Length.Equals(7))
                        testPhone = string.Format("{0:###-####}", dr.ToInt32("PhoneNumber"));
                    pe.PhoneNumber = testPhone;

                    pe.ID = dr.ToInt32("ID");
                    //pe.PhoneNumberType = dr.AsString("PhoneNumberType");
                    pe.IsActive = dr.ToBoolean("IsActive");
                    pe.ModifiedBy = dr.AsString("ModifiedBy");
                    pe.AreaCode = dr.AsString("AreaCode");
                    pe.Extension = dr.AsString("Extension");

                    lpe.Add(pe);
                }
                return lpe;
            }

            private List<AddressEntity> ProcessAddressTable(DataTable dt)
            {
                var lae = new List<AddressEntity>();
                foreach (DataRow dr in dt.Rows)
                {
                    var ae = new AddressEntity();
                    ae.Address1 = dr.AsString("Address1");
                    ae.Address2 = dr.AsString("Address2");
                    ae.Address3 = dr.AsString("Address3");
                    ae.ID = dr.ToInt32("ID");
                    //ae.AddressType = dr.AsString("AddressType");
                    ae.City = dr.AsString("City");
                    ae.Country = dr.AsString("Country");
                    ae.DateLastModified = dr.ToDateTime("DateLastModified");
                    ae.DateVerified = dr.ToDateTime("DateVerified");
                    ae.IndividualID = dr.ToInt32("IndividualID");
                    ae.IsActive = dr.ToBoolean("IsActive");
                    ae.UIC = dr.AsString("UIC");
                    ae.ModifiedBy = dr.AsString("ModifiedBy");
                    ae.State = dr.AsString("State");
                    ae.ZipCode = dr.AsString("ZipCode").ToZipCodeDisplay();
                    ae.ExpireDays = dr.ToInt32("ExpireDays");
                    lae.Add(ae);
                }
                return lae;
            }

            private List<OrderEntity> ProcessOrderTable(DataTable dt)
            {
                var loe = new List<OrderEntity>();
                foreach (DataRow dr in dt.Rows)
                {
                    var oe = new OrderEntity();
                    oe.ClinicSiteCode = dr.AsString("ClinicSiteCode");
                    oe.DateLastModified = dr.ToDateTime("DateLastModified");
                    oe.PrescriptionID = dr.ToInt32("PrescriptionID");
                    oe.IndividualID_Patient = dr.ToInt32("IndividualID_Patient");
                    oe.IndividualID_Tech = dr.ToInt32("IndividualID_Tech");
                    oe.PatientPhoneID = dr.ToInt32("PatientPhoneID");
                    oe.LabSiteCode = dr.AsString("LabSiteCode");
                    oe.LensMaterial = dr.AsString("LensMaterial");
                    oe.LensType = dr.AsString("LensType");
                    oe.LocationCode = dr.AsString("LocationCode");
                    oe.ModifiedBy = dr.AsString("ModifiedBy");
                    oe.NumberOfCases = dr.ToInt32("NumberOfCases");
                    oe.Pairs = dr.ToInt32("Pairs");
                    oe.OrderNumber = dr.AsString("OrderNumber");
                    oe.ShipAddress1 = dr.AsString("ShipAddress1");
                    oe.ShipAddress2 = dr.AsString("ShipAddress2");
                    oe.ShipAddress3 = dr.AsString("ShipAddress3");
                    oe.ShipCity = dr.AsString("ShipCity");
                    oe.ShipState = dr.AsString("ShipState");
                    oe.ShipZipCode = dr.AsString("ShipZipCode");
                    oe.ShipAddressType = dr.AsString("ShipAddressType");
                    oe.ShipCountry = dr.AsString("ShipCountry");

                    //oe.ShipToPatient = dr.ToBoolean("ShipToPatient");
                    oe.OrderDisbursement = SharedOperations.ExtractOrderDisbursement(dr.ToBoolean("ShipToPatient"), dr.ToBoolean("ClinicShipToPatient"));

                    oe.Tint = dr.AsString("Tint");
                    oe.UserComment1 = dr.AsString("UserComment1");
                    oe.UserComment2 = dr.AsString("UserComment2");
                    oe.FrameBridgeSize = dr.AsString("FrameBridgeSize");
                    oe.FrameCode = dr.AsString("FrameCode");
                    oe.FrameColor = dr.AsString("FrameColor");
                    oe.FrameEyeSize = dr.AsString("FrameEyeSize");
                    oe.FrameTempleType = dr.AsString("FrameTempleType");
                    oe.Demographic = dr.AsString("Demographic");
                    oe.IsGEyes = dr.ToBoolean("IsGEyes");
                    oe.IsActive = dr.ToBoolean("IsActive");
                    oe.IsMultivision = dr.ToBoolean("IsMultivision");
                    oe.ODSegHeight = dr.AsString("ODSegHeight");
                    oe.OSSegHeight = dr.AsString("OSSegHeight");
                    oe.VerifiedBy = dr.ToInt32("VerifiedBy");
                    oe.CorrespondenceEmail = dr.AsString("PatientEmail");
                    oe.OnholdForConfirmation = dr.ToBoolean("OnholdForConfirmation");
                    oe.MedProsDispense = dr.ToBoolean("MedProsDispense");
                    oe.PimrsDispense = dr.ToBoolean("PimrsDispense");
                    oe.FocDate = dr.ToNullableDateTime("FOCDate");
                    oe.LinkedID = dr.AsString("LinkedID");
                    loe.Add(oe);
                }
                return loe;
            }

            private List<ExamEntity> ProcessExamTable(DataTable dt)
            {
                var lee = new List<ExamEntity>();
                foreach (DataRow dr in dt.Rows)
                {
                    var ee = new ExamEntity();
                    ee.IndividualID_Patient = dr.ToInt32("IndividualID_Patient");
                    ee.ODOSCorrectedAcuity = dr.AsString("ODOSCorrectedAcuity");
                    ee.ODOSUncorrectedAcuity = dr.AsString("ODOSUncorrectedAcuity");
                    ee.OSCorrectedAcuity = dr.AsString("OSCorrectedAcuity");
                    ee.OSUncorrectedAcuity = dr.AsString("OSUncorrectedAcuity");
                    ee.ODCorrectedAcuity = dr.AsString("ODCorrectedAcuity");
                    ee.ODUncorrectedAcuity = dr.AsString("ODUncorrectedAcuity");
                    ee.Comments = dr.AsString("Comment");
                    ee.DateLastModified = dr.ToDateTime("DateLastModified");
                    ee.ExamDate = dr.ToDateTime("ExamDate");
                    ee.ID = dr.ToInt32("ID");
                    ee.ModifiedBy = dr.AsString("ModifiedBy");
                    ee.IndividualID_Examiner = dr.ToInt32("IndividualID_Examiner");
                    ee.IndividualID_Patient = dr.ToInt32("IndividualID_Patient");

                    lee.Add(ee);
                }
                return lee;
            }

            private List<PrescriptionEntity> ProcessPresciptionTable(DataTable dt)
            {
                var lpe = new List<PrescriptionEntity>();
                foreach (DataRow dr in dt.Rows)
                {
                    var pr = new PrescriptionEntity();
                    pr.DateLastModified = dr.ToDateTime("DateLastModified");
                    pr.ExamID = dr.ToInt32("ExamID");
                    pr.ID = dr.ToInt32("ID");
                    pr.IndividualID_Patient = dr.ToInt32("IndividualID_Patient");
                    pr.IndividualID_Doctor = dr.ToInt32("IndividualID_Doctor");
                    pr.ModifiedBy = dr.AsString("ModifiedBy");
                    pr.ODAxis = dr.ToInt32("ODAxis");
                    pr.ODCylinder = dr.ToDecimal("ODCylinder");
                    pr.ODHBase = dr.AsString("ODHBase");
                    pr.ODHPrism = dr.ToDecimal("ODHPrism");
                    pr.ODAdd = dr.ToDecimal("ODAdd");
                    pr.ODSphere = dr.ToDecimal("ODSphere");
                    pr.ODVBase = dr.AsString("ODVBase");
                    pr.ODVPrism = dr.ToDecimal("ODVPrism");
                    pr.OSAxis = dr.ToInt32("OSAxis");
                    pr.OSCylinder = dr.ToDecimal("OSCylinder");
                    pr.OSHBase = dr.AsString("OSHBase");
                    pr.OSHPrism = dr.ToDecimal("OSHPrism");
                    pr.OSAdd = dr.ToDecimal("OSAdd");
                    pr.OSSphere = dr.ToDecimal("OSSphere");
                    pr.OSVBase = dr.AsString("OSVBase");
                    pr.OSVPrism = dr.ToDecimal("OSVPrism");
                    pr.PDTotal = dr.ToDecimal("PDDistant");
                    pr.PDOD = dr.ToDecimal("ODPDDistant");
                    pr.PDOS = dr.ToDecimal("OSPDDistant");
                    pr.PDTotalNear = dr.ToDecimal("PDNear");
                    pr.PDODNear = dr.ToDecimal("ODPDNear");
                    pr.PDOSNear = dr.ToDecimal("OSPDNear");
                    pr.IsMonoCalculation = dr.ToBoolean("IsMonoCalculation");
                    pr.EnteredODSphere = dr.ToDecimal("EnteredODSphere");
                    pr.EnteredOSSphere = dr.ToDecimal("EnteredOSSphere");
                    pr.EnteredODCylinder = dr.ToDecimal("EnteredODCylinder");
                    pr.EnteredOSCylinder = dr.ToDecimal("EnteredOSCylinder");
                    pr.EnteredODAxis = dr.ToInt32("EnteredODAxis");
                    pr.EnteredOSAxis = dr.ToInt32("EnteredOSAxis");
                    pr.PrescriptionDate = dr.ToDateTime("PrescriptionDate");
                    pr.IsActive = dr.ToBoolean("IsActive");
                    pr.IsUsed = dr.ToBoolean("IsUsed");
                    pr.IsDeletable = dr.ToBoolean("IsDeletable");
                    pr.RxName = dr.AsString("RxName");

                    lpe.Add(pr);
                }
                return lpe;
            }

            private List<IndividualTypeEntity> ProcessIndividualTypeTable(DataTable dt)
            {
                var lie = new List<IndividualTypeEntity>();

                foreach (DataRow dr in dt.Rows)
                {
                    IndividualTypeEntity ie = new IndividualTypeEntity();
                    ie.DateLastModified = dr.ToDateTime("DateLastModified");
                    ie.ID = dr.ToInt32("ID");
                    ie.TypeId = dr.ToInt32("TypeID");
                    ie.TypeDescription = dr.AsString("TypeDescription");
                    ie.IndividualId = dr.ToInt32("IndividualID");
                    ie.IsActive = dr.ToBoolean("IsActive");
                    ie.ModifiedBy = dr.AsString("ModifiedBy");
                    lie.Add(ie);
                }

                return lie;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class InsertIndividualReository : RepositoryBase<IndividualEntity>
        {
            /// <summary>
            /// Default Ctor.
            /// </summary>
            public InsertIndividualReository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }

            /// <summary>
            /// Inserts a new individual to the database.
            /// </summary>
            /// <param name="patient">Patient entity with an individuals information to add.</param>
            /// <returns>IndividualEntity list of the newly inserted record.  It is just a single record in the list.</returns>
            public List<IndividualEntity> InsertIndividual(PatientEntity patient)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertNewIndividual";
                cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumber", patient.IDNumbers[0].IDNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumberType", patient.IDNumbers[0].IDNumberType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PersonalType", patient.Individual.PersonalType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FirstName", patient.Individual.FirstName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LastName", patient.Individual.LastName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MiddleName", string.IsNullOrEmpty(patient.Individual.MiddleName) ? string.Empty : patient.Individual.MiddleName));
                if (patient.Individual.DateOfBirth != null)
                {
                    cmd.Parameters.Add(this.DAL.GetParamenter("@DateOfBirth", patient.Individual.DateOfBirth));
                }
                else
                {
                    cmd.Parameters.Add(this.DAL.GetParamenter("@DateOfBirth", DateTime.Parse("01/01/1900")));
                }
                if (patient.Individual.EADStopDate != null)
                {
                    cmd.Parameters.Add(this.DAL.GetParamenter("@EADStopDate", patient.Individual.EADStopDate));
                }
                else
                {
                    cmd.Parameters.Add(this.DAL.GetParamenter("@EADStopDate", DateTime.Parse("01/01/1900")));
                }
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsPOC", patient.Individual.IsPOC));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCodeID", patient.Individual.SiteCodeID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Comments", string.IsNullOrEmpty(patient.Individual.Comments) ? string.Empty : patient.Individual.Comments));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", true));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LocationCode", string.IsNullOrEmpty(patient.Individual.TheaterLocationCode) ? string.Empty : patient.Individual.TheaterLocationCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", patient.Individual.Demographic));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", patient.Individual.ModifiedBy));

                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Inserts a new individual to the database.
            /// </summary>
            /// <param name="person">Patient entity with an individuals information to add.</param>
            /// <param name="idNumber">ID number to use for the individual.</param>
            /// <returns>IndividualEntity list of the newly inserted record.  It is just a single record in the list.</returns>
            public List<IndividualEntity> InsertIndividual(IndividualEntity person, IdentificationNumbersEntity idNumber)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertNewIndividual";
                cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumber", idNumber.IDNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IDNumberType", idNumber.IDNumberType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PersonalType", person.PersonalType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FirstName", person.FirstName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LastName", person.LastName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MiddleName", string.IsNullOrEmpty(person.MiddleName) ? string.Empty : person.MiddleName));
                if (person.DateOfBirth != null)
                {
                    cmd.Parameters.Add(this.DAL.GetParamenter("@DateOfBirth", person.DateOfBirth));
                }
                else
                {
                    cmd.Parameters.Add(this.DAL.GetParamenter("@DateOfBirth", DateTime.Parse("01/01/1900")));
                }
                if (person.EADStopDate != null)
                {
                    cmd.Parameters.Add(this.DAL.GetParamenter("@EADStopDate", person.EADStopDate));
                }
                else
                {
                    cmd.Parameters.Add(this.DAL.GetParamenter("@EADStopDate", DateTime.Parse("01/01/1900")));
                }
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsPOC", person.IsPOC));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCodeID", person.SiteCodeID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Comments", string.IsNullOrEmpty(person.Comments) ? string.Empty : person.Comments));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", true));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LocationCode", string.IsNullOrEmpty(person.TheaterLocationCode) ? string.Empty : person.TheaterLocationCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", person.Demographic));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", person.ModifiedBy));

                return GetRecords(cmd).ToList();
            }

            protected override IEnumerable<IndividualEntity> GetRecords(IDbCommand cmdIn)
            {
                using (cmdIn)
                {
                    var ds = this.DAL.GetDataSet(cmdIn);
                    if (ds == null) return new List<IndividualEntity>();
                    return FillRecords(ds.Tables[2]);
                }
            }

            protected override IEnumerable<IndividualEntity> FillRecords(DataTable dt)
            {
                var l = new List<IndividualEntity>();
                dt.AsEnumerable().ToList().ForEach(x => l.Add(ProcessIndividualRow(x)));
                return l;
            }

            private IndividualEntity ProcessIndividualRow(DataRow dr)
            {
                var ie = new IndividualEntity();
                ie.Comments = dr.AsString("Comments");
                ie.DateLastModified = dr.ToDateTime("DateLastModified");
                ie.DateOfBirth = dr.ToDateTime("DateOfBirth");
                ie.EADStopDate = dr.ToNullableDateTime("EADStopDate");
                ie.NextFocDate = dr.ToNullableDateTime("NextFocDate");
                ie.FirstName = dr.AsString("FirstName");
                ie.Demographic = dr.AsString("Demographic");
                ie.ID = dr.ToInt32("ID");
                ie.IDNumberType = dr.AsString("IDNumberType");
                ie.IDNumber = dr.AsString("IDNumber");
                ie.IDNumberTypeDescription = Helpers.GetIDTypeDescription(ie.IDNumberType);

                if (string.IsNullOrEmpty(ie.IDNumber))
                {
                    ie.IDNumber = dr.AsString("IDNbrs");
                    ie.IDNumberTypeDescription = dr.AsString("IDType");
                }

                ie.IsActive = dr.ToBoolean("IsActive");
                ie.IsPOC = dr.ToBoolean("IsPOC");
                ie.LastName = dr.AsString("LastName");
                ie.TheaterLocationCode = dr.AsString("TheaterLocationCode");
                ie.MiddleName = dr.AsString("MiddleName");
                ie.ModifiedBy = dr.AsString("ModifiedBy");
                ie.SiteCodeID = dr.AsString("SiteCodeID");
                ie.PersonalType = dr.AsString("PersonalType");
                ie.BOSDescription = ie.Demographic.ToBOSValue();
                ie.StatusDescription = ie.Demographic.ToPatientStatusValue();
                if (dr.Table.Columns.Contains("IsPatient"))
                    ie.IsNewPatient = !dr.ToBoolean("IsPatient");

                return ie;
            }
        }
    }
}