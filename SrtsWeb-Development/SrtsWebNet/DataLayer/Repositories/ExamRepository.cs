using SrtsWeb.DataLayer.TypeExtenders;
using SrtsWeb.Entities;
using System.Data;
using System.Data.SqlClient;

namespace SrtsWeb.DataLayer.Repositories
{
    public sealed class ExamRepository : IExamRepository
    {
        public DataTable InsertExam(ExamEntity exam)
        {
            DataTable dt = new DataTable();
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "InsertExam";
                sqlCmd.AddParameter("@IndividualID_Patient", SqlDbType.Int, ParameterDirection.Input, exam.IndividualID_Patient);
                sqlCmd.AddParameter("@IndividualID_Examiner", SqlDbType.Int, ParameterDirection.Input, exam.IndividualID_Examiner);
                sqlCmd.AddParameter("@ODCorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.ODCorrectedAcuity);
                sqlCmd.AddParameter("@ODUncorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.ODUncorrectedAcuity);
                sqlCmd.AddParameter("@OSCorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.OSCorrectedAcuity);
                sqlCmd.AddParameter("@OSUncorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.OSUncorrectedAcuity);
                sqlCmd.AddParameter("@ODOSCorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.ODOSCorrectedAcuity);
                sqlCmd.AddParameter("@ODOSUncorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.ODOSUncorrectedAcuity);
                sqlCmd.AddParameter("@Comment", SqlDbType.VarChar, ParameterDirection.Input, exam.Comments);
                sqlCmd.AddParameter("@ExamDate", SqlDbType.DateTime, ParameterDirection.Input, exam.ExamDate);
                sqlCmd.AddParameter("@IsActive", SqlDbType.Bit, ParameterDirection.Input, true);
                sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, exam.ModifiedBy);
                dt = sqlCmd.ExecuteToDataTable();
            }
            return dt;
        }

        public DataTable UpdateExam(ExamEntity exam)
        {
            DataTable dt = new DataTable();
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "UpdateExamByExamID";
                sqlCmd.AddParameter("@ID", SqlDbType.Int, ParameterDirection.Input, exam.ID);
                sqlCmd.AddParameter("@IndividualID_Patient", SqlDbType.Int, ParameterDirection.Input, exam.IndividualID_Patient);
                sqlCmd.AddParameter("@IndividualID_Examiner", SqlDbType.Int, ParameterDirection.Input, exam.IndividualID_Examiner);
                sqlCmd.AddParameter("@ODCorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.ODCorrectedAcuity);
                sqlCmd.AddParameter("@ODUncorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.ODUncorrectedAcuity);
                sqlCmd.AddParameter("@OSCorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.OSCorrectedAcuity);
                sqlCmd.AddParameter("@OSUncorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.OSUncorrectedAcuity);
                sqlCmd.AddParameter("@ODOSCorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.ODOSCorrectedAcuity);
                sqlCmd.AddParameter("@ODOSUncorrectedAcuity", SqlDbType.VarChar, ParameterDirection.Input, exam.ODOSUncorrectedAcuity);
                sqlCmd.AddParameter("@ExamDate", SqlDbType.DateTime, ParameterDirection.Input, exam.ExamDate);
                sqlCmd.AddParameter("@Comment", SqlDbType.VarChar, ParameterDirection.Input, exam.Comments);
                sqlCmd.AddParameter("@IsActive", SqlDbType.Bit, ParameterDirection.Input, string.IsNullOrEmpty(exam.IsActive.ToString()) ? true : false);
                sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, exam.ModifiedBy);
                dt = sqlCmd.ExecuteToDataTable();
            }
            return dt;
        }

        public DataTable GetExamByExamID(int examID, string modifiedBy)
        {
            DataTable dt = new DataTable();
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "GetExamByExamID";
                sqlCmd.AddParameter("@ID", SqlDbType.Int, ParameterDirection.Input, examID);
                sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, modifiedBy);
                dt = sqlCmd.ExecuteToDataTable();
            }
            return dt;
        }

        public DataTable GetExamsByIndividualID(int individualID, string modifiedBy)
        {
            DataTable dt = new DataTable();
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "GetExamsByIndividualID";
                sqlCmd.AddParameter("@IndividualID", SqlDbType.Int, ParameterDirection.Input, individualID);
                sqlCmd.AddParameter("@ModifiedBy", SqlDbType.VarChar, ParameterDirection.Input, modifiedBy);
                dt = sqlCmd.ExecuteToDataTable();
            }
            return dt;
        }
    }
}