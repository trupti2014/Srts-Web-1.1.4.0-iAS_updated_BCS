using SrtsWeb.DataLayer.TypeExtenders;
using SrtsWeb.Entities;
using System.Data;
using System.Data.SqlClient;

namespace SrtsWeb.DataLayer.Repositories
{
    public sealed class CustomErrorRepository : ICustomErrorRepository
    {
        #region ICustomErrorRepository Members

        public void InsertError(CustomErrorEntity _error)
        {
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "InsertSQLErrorLog";
                sqlCmd.AddParameter("@InnerExceptionType", SqlDbType.VarChar, ParameterDirection.Input, _error.InnerExceptionType);
                sqlCmd.AddParameter("@InnerException", SqlDbType.VarChar, ParameterDirection.Input, _error.InnerException);
                sqlCmd.AddParameter("@InnerSource", SqlDbType.VarChar, ParameterDirection.Input, _error.InnerSource);
                sqlCmd.AddParameter("@InnerStackTrace", SqlDbType.VarChar, ParameterDirection.Input, _error.InnerStackTrace);
                sqlCmd.AddParameter("@ExceptionType", SqlDbType.VarChar, ParameterDirection.Input, _error.ExceptionType);
                sqlCmd.AddParameter("@Exception", SqlDbType.VarChar, ParameterDirection.Input, _error.Exception);
                sqlCmd.AddParameter("@Source", SqlDbType.VarChar, ParameterDirection.Input, _error.Source);
                sqlCmd.AddParameter("@StackTrace", SqlDbType.VarChar, ParameterDirection.Input, _error.StackTrace);

                sqlCmd.ExecuteToNonQuery();
            }
        }

        #endregion ICustomErrorRepository Members
    }
}