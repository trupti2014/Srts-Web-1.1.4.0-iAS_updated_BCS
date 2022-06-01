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
    public sealed class IndividualPrescriptionRepository : RepositoryBase<PrescriptionEntity>, IIndividualPrescriptionRepository
    {

        public IndividualPrescriptionRepository() 
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {}

        /// <summary>
        /// Get prescriptions by an individuals ID
        /// </summary>
        /// <param name="individualID">A int referencing the individual</param>
        /// <param name="modifiedBy">A string referencing the modifiedby </param>
        /// <returns></returns>
        public List<PrescriptionEntity> GetPrescriptionsByIndividualID(int individualID, String modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetPrescriptionsByIndividualID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", individualID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Fill record of prescriptionEntity
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected override PrescriptionEntity FillRecord(IDataReader dr)
        {
            PrescriptionEntity oe = new PrescriptionEntity();

            oe.DateLastModified = dr.ToDateTime("DateLastModified");
            oe.ExamID = dr.ToInt32("ExamID");
            oe.ID = dr.ToInt32("ID");
            oe.IndividualID_Patient = dr.ToInt32("IndividualID_Patient");
            oe.IndividualID_Doctor = dr.ToInt32("IndividualID_Doctor");
            oe.ModifiedBy = dr.AsString("ModifiedBy");
            oe.ODAxis = dr.ToInt32("ODAxis");
            oe.ODCylinder = dr.ToDecimal("ODCylinder");
            oe.ODHBase = dr.AsString("ODHBase");
            oe.ODHPrism = dr.ToDecimal("ODHPrism");
            oe.ODAdd = dr.ToDecimal("ODAdd");
            oe.ODSphere = dr.ToDecimal("ODSphere");
            oe.ODVBase = dr.AsString("ODVBase");
            oe.ODVPrism = dr.ToDecimal("ODVPrism");
            oe.OSAxis = dr.ToInt32("OSAxis");
            oe.OSCylinder = dr.ToDecimal("OSCylinder");
            oe.OSHBase = dr.AsString("OSHBase");
            oe.OSHPrism = dr.ToDecimal("OSHPrism");
            oe.OSAdd = dr.ToDecimal("OSAdd");
            oe.OSSphere = dr.ToDecimal("OSSphere");
            oe.OSVBase = dr.AsString("OSVBase");
            oe.OSVPrism = dr.ToDecimal("OSVPrism");
            oe.PDTotal = dr.ToDecimal("PDDistant");
            oe.PDOD = dr.ToDecimal("ODPDDistant");
            oe.PDOS = dr.ToDecimal("OSPDDistant");
            oe.PDTotalNear = dr.ToDecimal("PDNear");
            oe.PDODNear = dr.ToDecimal("ODPDNear");
            oe.PDOSNear = dr.ToDecimal("OSPDNear");
            oe.IsMonoCalculation = dr.ToBoolean("IsMonoCalculation");
            oe.EnteredODSphere = dr.ToDecimal("EnteredODSphere");
            oe.EnteredOSSphere = dr.ToDecimal("EnteredOSSphere");
            oe.EnteredODCylinder = dr.ToDecimal("EnteredODCylinder");
            oe.EnteredOSCylinder = dr.ToDecimal("EnteredOSCylinder");
            oe.EnteredODAxis = dr.ToInt32("EnteredODAxis");
            oe.EnteredOSAxis = dr.ToInt32("EnteredOSAxis");
            oe.PrescriptionDate = dr.ToDateTime("PrescriptionDate");
            oe.IsActive = dr.ToBoolean("IsActive");
            oe.IsUsed = dr.ToBoolean("IsUsed");
            oe.RxName = dr.AsString("RxName");

            return oe;
        }

        /// <summary>
        /// Add a single Prescription to the database.
        /// </summary>
        /// <param name="presc">Prescription entity to insert.</param>
        /// <returns>List of addresses.</returns>
        public List<PrescriptionEntity> InsertPrescription(PrescriptionEntity presc)
        {
            
            var cmd = this.DAL.GetCommandObject();

            /*
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertPrescription";
            cmd.Parameters.Add(this.DAL.GetParamenter("@ExamID", presc.ExamID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Patient", presc.IndividualID_Patient));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Doctor", presc.IndividualID_Doctor));

            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODSphere", presc.EnteredODSphere));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSSphere", presc.EnteredOSSphere));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODCylinder", presc.EnteredODCylinder));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSCylinder", presc.EnteredOSCylinder));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODAxis", presc.EnteredODAxis));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSAxis", presc.EnteredOSAxis));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", presc.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsMonoCalculation", presc.IsMonoCalculation));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PDDistant", presc.PDTotal));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PDNear", presc.PDTotalNear));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PrescriptionDate", presc.PrescriptionDate));
            return GetRecords(cmd).ToList();

    */

            cmd.CommandText = "InsertPrescription";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(this.DAL.GetParamenter("@PrescriptionDate", presc.PrescriptionDate));
            cmd.Parameters.Add(this.DAL.GetParamenter("@RxName", presc.RxName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ExamID", presc.ExamID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Patient", presc.IndividualID_Patient));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Doctor", presc.IndividualID_Doctor));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODSphere", presc.EnteredODSphere));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSSphere", presc.EnteredOSSphere));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODCylinder", presc.EnteredODCylinder));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSCylinder", presc.EnteredOSCylinder));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODAxis", presc.EnteredODAxis));
            cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSAxis", presc.EnteredOSAxis));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODSphere", presc.ODSphere));//OdSphereCalc
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSSphere", presc.OSSphere));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODCylinder", presc.ODCylinder));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSCylinder", presc.OSCylinder));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODAxis", presc.ODAxis));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSAxis", presc.OSAxis));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODHPrism", presc.ODHPrism));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSHPrism", presc.OSHPrism));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODVPrism", presc.ODVPrism));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSVPrism", presc.OSVPrism));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODHBase", presc.ODHBase));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSHBase", presc.OSHBase));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODVBase", presc.ODVBase));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSVBase", presc.OSVBase));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODAdd", presc.ODAdd));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSAdd", presc.OSAdd));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", presc.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsMonoCalculation", presc.IsMonoCalculation));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PDDistant", presc.PDTotal));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PDNear", presc.PDTotalNear));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODPDDistant", presc.PDOD));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODPDNear", presc.PDODNear));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSPDDistant", presc.PDOS));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSPDNear", presc.PDOSNear));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", 0, ParameterDirection.Output));
            cmd.Parameters.Add(this.DAL.GetParamenter("@RxID", 0, ParameterDirection.Output));

            //InsertData(cmd);
            return GetRecords(cmd).ToList();
        }
    }
}