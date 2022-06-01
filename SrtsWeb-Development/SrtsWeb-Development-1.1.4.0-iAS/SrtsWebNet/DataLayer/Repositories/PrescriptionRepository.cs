using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.TypeExtenders;
using System.Data;
using System.Data.SqlClient;

namespace SrtsWeb.DataLayer.Repositories
{
    public sealed class PrescriptionRepository : IPrescriptionRepository
    {
        public DataTable GetPrescriptionByPrescriptionID(int prescriptionID, string modifiedBy)
        {
            DataTable dt = new DataTable();
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "GetPrescriptionByID";
                sqlCmd.AddParameter("@ID", SqlDbType.Int, ParameterDirection.Input, prescriptionID);
                sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, modifiedBy);
                dt = sqlCmd.ExecuteToDataTable();
            }
            return dt;
        }

        public DataTable GetPrescriptionsByIndividualID(int individualID, string modifiedBy)
        {
            DataTable dt = new DataTable();
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "GetPrescriptionsByIndividualID";
                sqlCmd.AddParameter("@IndividualID", SqlDbType.Int, ParameterDirection.Input, individualID);
                sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, modifiedBy);
                dt = sqlCmd.ExecuteToDataTable();
            }
            return dt;
        }

        //public DataTable InsertPrescription(PrescriptionEntity _prescription)
        //{
        //    DataTable dt = new DataTable();
        //    using (SqlCommand sqlCmd = new SqlCommand())
        //    {
        //        sqlCmd.CommandType = CommandType.StoredProcedure;
        //        sqlCmd.CommandText = "InsertPrescription";
        //        if (_prescription.ExamID == null)
        //        {
        //            sqlCmd.AddParameter("@ExamID", SqlDbType.Int, ParameterDirection.Input, DBNull.Value);
        //        }
        //        else
        //        {
        //            sqlCmd.AddParameter("@ExamID", SqlDbType.Int, ParameterDirection.Input, _prescription.ExamID);
        //        }
        //        sqlCmd.AddParameter("@IndividualID_Patient", SqlDbType.Int, ParameterDirection.Input, _prescription.IndividualID_Patient);
        //        sqlCmd.AddParameter("@IndividualID_Doctor", SqlDbType.Int, ParameterDirection.Input, _prescription.IndividualID_Doctor);
        //        sqlCmd.AddParameter("@ODSphere", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODSphere);
        //        sqlCmd.AddParameter("@OSSphere", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSSphere);
        //        sqlCmd.AddParameter("@ODCylinder", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODCylinder);
        //        sqlCmd.AddParameter("@OSCylinder", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSCylinder);
        //        sqlCmd.AddParameter("@ODAxis", SqlDbType.Int, ParameterDirection.Input, _prescription.ODAxis);
        //        sqlCmd.AddParameter("@OSAxis", SqlDbType.Int, ParameterDirection.Input, _prescription.OSAxis);
        //        sqlCmd.AddParameter("@ODHprism", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODHPrism);
        //        sqlCmd.AddParameter("@OSHprism", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSHPrism);
        //        sqlCmd.AddParameter("@ODVprism", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODVPrism);
        //        sqlCmd.AddParameter("@OSVprism", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSVPrism);
        //        sqlCmd.AddParameter("@ODHBase", SqlDbType.VarChar, ParameterDirection.Input, _prescription.ODHBase);
        //        sqlCmd.AddParameter("@OSHBase", SqlDbType.VarChar, ParameterDirection.Input, _prescription.OSHBase);
        //        sqlCmd.AddParameter("@ODVBase", SqlDbType.VarChar, ParameterDirection.Input, _prescription.ODVBase);
        //        sqlCmd.AddParameter("@OSVBase", SqlDbType.VarChar, ParameterDirection.Input, _prescription.OSVBase);
        //        sqlCmd.AddParameter("@ODAdd", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODAdd);
        //        sqlCmd.AddParameter("@OSAdd", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSAdd);
        //        sqlCmd.AddParameter("@EnteredODSphere", SqlDbType.Decimal, ParameterDirection.Input, _prescription.EnteredODSphere);
        //        sqlCmd.AddParameter("@EnteredOSSphere", SqlDbType.Decimal, ParameterDirection.Input, _prescription.EnteredOSSphere);
        //        sqlCmd.AddParameter("@EnteredODCylinder", SqlDbType.Decimal, ParameterDirection.Input, _prescription.EnteredODCylinder);
        //        sqlCmd.AddParameter("@EnteredOSCylinder", SqlDbType.Decimal, ParameterDirection.Input, _prescription.EnteredOSCylinder);
        //        sqlCmd.AddParameter("@EnteredODAxis", SqlDbType.Int, ParameterDirection.Input, _prescription.EnteredODAxis);
        //        sqlCmd.AddParameter("@EnteredOSAxis", SqlDbType.Int, ParameterDirection.Input, _prescription.EnteredOSAxis);
        //        sqlCmd.AddParameter("@PDDistant", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDTotal);
        //        sqlCmd.AddParameter("@PDNear", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDTotalNear);
        //        sqlCmd.AddParameter("@ODPDDistant", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDOD);
        //        sqlCmd.AddParameter("@ODPDNear", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDODNear);
        //        sqlCmd.AddParameter("@OSPDDistant", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDOS);
        //        sqlCmd.AddParameter("@OSPDNear", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDOSNear);
        //        sqlCmd.AddParameter("@IsMonoCalculation", SqlDbType.Bit, ParameterDirection.Input, _prescription.IsMonoCalculation);
        //        sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, _prescription.ModifiedBy);
        //        dt = sqlCmd.ExecuteToDataTable();
        //    }
        //    return dt;
        //}

        //public DataTable UpdatePrescription(PrescriptionEntity _prescription)
        //{
        //    DataTable dt = new DataTable();
        //    using (SqlCommand sqlCmd = new SqlCommand())
        //    {
        //        sqlCmd.CommandType = CommandType.StoredProcedure;
        //        sqlCmd.CommandText = "UpdatePrescription";
        //        sqlCmd.AddParameter("@ID", SqlDbType.Int, ParameterDirection.Input, _prescription.ID);
        //        sqlCmd.AddParameter("@ExamID", SqlDbType.Int, ParameterDirection.Input, _prescription.ExamID);
        //        sqlCmd.AddParameter("@IndividualID_Patient", SqlDbType.Int, ParameterDirection.Input, _prescription.IndividualID_Patient);
        //        sqlCmd.AddParameter("@IndividualID_Doctor", SqlDbType.Int, ParameterDirection.Input, _prescription.IndividualID_Doctor);
        //        sqlCmd.AddParameter("@ODSphere", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODSphere);
        //        sqlCmd.AddParameter("@OSSphere", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSSphere);
        //        sqlCmd.AddParameter("@ODCylinder", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODCylinder);
        //        sqlCmd.AddParameter("@OSCylinder", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSCylinder);
        //        sqlCmd.AddParameter("@ODAxis", SqlDbType.Int, ParameterDirection.Input, _prescription.ODAxis);
        //        sqlCmd.AddParameter("@OSAxis", SqlDbType.Int, ParameterDirection.Input, _prescription.OSAxis);
        //        sqlCmd.AddParameter("@ODHprism", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODHPrism);
        //        sqlCmd.AddParameter("@OSHprism", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSHPrism);
        //        sqlCmd.AddParameter("@ODVprism", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODVPrism);
        //        sqlCmd.AddParameter("@OSVprism", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSVPrism);
        //        sqlCmd.AddParameter("@ODHBase", SqlDbType.VarChar, ParameterDirection.Input, _prescription.ODHBase);
        //        sqlCmd.AddParameter("@OSHBase", SqlDbType.VarChar, ParameterDirection.Input, _prescription.OSHBase);
        //        sqlCmd.AddParameter("@ODVBase", SqlDbType.VarChar, ParameterDirection.Input, _prescription.ODVBase);
        //        sqlCmd.AddParameter("@OSVBase", SqlDbType.VarChar, ParameterDirection.Input, _prescription.OSVBase);
        //        sqlCmd.AddParameter("@ODAdd", SqlDbType.Decimal, ParameterDirection.Input, _prescription.ODAdd);
        //        sqlCmd.AddParameter("@OSAdd", SqlDbType.Decimal, ParameterDirection.Input, _prescription.OSAdd);
        //        sqlCmd.AddParameter("@EnteredODSphere", SqlDbType.Decimal, ParameterDirection.Input, _prescription.EnteredODSphere);
        //        sqlCmd.AddParameter("@EnteredOSSphere", SqlDbType.Decimal, ParameterDirection.Input, _prescription.EnteredOSSphere);
        //        sqlCmd.AddParameter("@EnteredODCylinder", SqlDbType.Decimal, ParameterDirection.Input, _prescription.EnteredODCylinder);
        //        sqlCmd.AddParameter("@EnteredOSCylinder", SqlDbType.Decimal, ParameterDirection.Input, _prescription.EnteredOSCylinder);
        //        sqlCmd.AddParameter("@EnteredODAxis", SqlDbType.Int, ParameterDirection.Input, _prescription.EnteredODAxis);
        //        sqlCmd.AddParameter("@EnteredOSAxis", SqlDbType.Int, ParameterDirection.Input, _prescription.EnteredOSAxis);
        //        sqlCmd.AddParameter("@PDDistant", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDTotal);
        //        sqlCmd.AddParameter("@PDNear", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDTotalNear);
        //        sqlCmd.AddParameter("@ODPDDistant", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDOD);
        //        sqlCmd.AddParameter("@ODPDNear", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDODNear);
        //        sqlCmd.AddParameter("@OSPDDistant", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDOS);
        //        sqlCmd.AddParameter("@OSPDNear", SqlDbType.Decimal, ParameterDirection.Input, _prescription.PDOSNear);
        //        sqlCmd.AddParameter("@IsMonoCalculation", SqlDbType.Bit, ParameterDirection.Input, _prescription.IsMonoCalculation);
        //        sqlCmd.AddParameter("@IsActive", SqlDbType.Bit, ParameterDirection.Input, _prescription.IsActive);
        //        sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, _prescription.ModifiedBy);
        //        dt = sqlCmd.ExecuteToDataTable();
        //    }
        //    return dt;
        //}

        //public DataTable GetPrescriptionByExamID(int examID)
        //{
        //    DataTable dt = new DataTable();
        //    using (SqlCommand sqlCmd = new SqlCommand())
        //    {
        //        sqlCmd.CommandType = CommandType.StoredProcedure;
        //        sqlCmd.CommandText = "GetPrescriptionsByExamID";
        //        sqlCmd.AddParameter("@ExamID", SqlDbType.Int, ParameterDirection.Input, examID);
        //        dt = sqlCmd.ExecuteToDataTable();
        //    }
        //    return dt;
        //}
        //public DataTable GetActiveUnusedPrescriptionByPrescriptionID(int prescriptionID, string modifiedBy)
        //{
        //    DataTable dt = new DataTable();
        //    using (SqlCommand sqlCmd = new SqlCommand())
        //    {
        //        sqlCmd.CommandType = CommandType.StoredProcedure;
        //        sqlCmd.CommandText = "GetActiveUnusedPrescriptionByID";
        //        sqlCmd.AddParameter("@ID", SqlDbType.Int, ParameterDirection.Input, prescriptionID);
        //        sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, modifiedBy);
        //        dt = sqlCmd.ExecuteToDataTable();
        //    }
        //    return dt;
        //}
    }
}