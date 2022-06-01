using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.DataBaseAccessLayer
{
    public interface IDatabaseAccessLayer
    {
        String ConfigConnectionStringName { set; }
        String ConnectionString { get; set; }

        IDbCommand GetCommandObject();

        IDataParameter GetParamenter(String name, Object value, ParameterDirection direction = ParameterDirection.Input);

        IDbConnection GetConnectionObject();
        IDbConnection GetConnectionObject(String connectionString);

        DataTable GetData(IDbCommand cmd);
        DataTable GetData(IDbCommand cmd, IDbConnection conn);

        DataSet GetDataSet(IDbCommand cmd);
        DataSet GetDataSet(IDbCommand cmd, IDbConnection conn);

        IDataReader GetDataReader(IDbCommand cmd);
        IDataReader GetDataReader(IDbCommand cmd, IDbConnection conn);

        Int32 SetData(IDbCommand cmd);
        Int32 SetData(IDbCommand cmd, IDbConnection conn);
    }
}
