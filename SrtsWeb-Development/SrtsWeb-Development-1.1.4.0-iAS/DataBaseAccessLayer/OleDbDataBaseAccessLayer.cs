using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.DataBaseAccessLayer
{
    // I am treating this class as a file operations class not an actual db operations class.
    internal class ExcelDataBaseAccessLayer : IDatabaseAccessLayer
    {
        private string _ConnectionString;
        public string ConnectionString
        {
            get
            {
                //return !String.IsNullOrEmpty(_ConnectionString) ?
                //    _ConnectionString : !String.IsNullOrEmpty(this.ConfigConnectionStringName) ?
                //    ConfigurationManager.ConnectionStrings[this.ConfigConnectionStringName].ConnectionString : String.Empty;
                return String.Format(ConfigurationManager.ConnectionStrings[this.ConfigConnectionStringName].ConnectionString, this._ConnectionString);
            }
            set
            {
                _ConnectionString = value;
            }
        }

        private string name;
        public string ConfigConnectionStringName
        {
            private get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public ExcelDataBaseAccessLayer() { }
        public ExcelDataBaseAccessLayer(String ConfigConnectionStringName) { this.ConfigConnectionStringName = ConfigConnectionStringName; }
        public ExcelDataBaseAccessLayer(String ConfigConnectionStringName, String FileLoc) { this.ConfigConnectionStringName = ConfigConnectionStringName; this.ConnectionString = FileLoc; }

        public IDbCommand GetCommandObject()
        {
            return new OleDbCommand();
        }

        public IDataParameter GetParamenter(String name, Object value, ParameterDirection direction = ParameterDirection.Input)
        {
            throw new NotImplementedException();
        }

        public IDbConnection GetConnectionObject()
        {
            return new OleDbConnection(this.ConnectionString);
        }

        public IDbConnection GetConnectionObject(string connectionString)
        {
            return new OleDbConnection(String.Format(this.ConnectionString, connectionString));
        }

        public DataTable GetData(IDbCommand cmd, IDbConnection conn)
        {
            using (cmd)
            {
                using (conn)
                {
                    using (var da = new OleDbDataAdapter((OleDbCommand)cmd))
                    {
                        da.SelectCommand.Connection = (OleDbConnection)conn;
                        da.SelectCommand.Connection.Open();
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public DataTable GetData(IDbCommand cmd)
        {
            return GetData(cmd, GetConnectionObject());
        }

        public int SetData(IDbCommand cmd)
        {
            return SetData(cmd, GetConnectionObject());
        }

        public int SetData(IDbCommand cmd, IDbConnection conn)
        {
            using (cmd)
            {
                using (conn)
                {
                    cmd.Connection = conn;
                    cmd.Connection.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public IDataReader GetDataReader(IDbCommand cmd)
        {
            var conn = GetConnectionObject(this.ConnectionString) as OleDbConnection;
            cmd.Connection = conn;
            cmd.Connection.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public IDataReader GetDataReader(IDbCommand cmd, IDbConnection conn)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetDataGeneric<T>(IDbCommand cmd) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetDataGeneric<T>(IDbCommand cmd, IDbConnection conn) where T : class, new()
        {
            throw new NotImplementedException();
        }


        public DataSet GetDataSet(IDbCommand cmd)
        {
            throw new NotImplementedException();
        }

        public DataSet GetDataSet(IDbCommand cmd, IDbConnection conn)
        {
            throw new NotImplementedException();
        }
    }
}
