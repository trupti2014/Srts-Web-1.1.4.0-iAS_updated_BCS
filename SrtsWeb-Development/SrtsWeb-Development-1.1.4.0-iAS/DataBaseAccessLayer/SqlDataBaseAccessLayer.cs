using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.DataBaseAccessLayer
{
    public class SqlDataBaseAccessLayer : IDatabaseAccessLayer
    {
        public SqlDataBaseAccessLayer() { }
        public SqlDataBaseAccessLayer(String ConfigConnectionStringName)
        {
            this.ConfigConnectionStringName = ConfigConnectionStringName;
        }

        public IDbCommand GetCommandObject()
        {
            return new SqlCommand();
        }

        public IDataParameter GetParamenter(String name, Object value, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = new SqlParameter(name, value);
            p.Direction = direction;
            return p;
        }

        public IDbConnection GetConnectionObject()
        {
            return new SqlConnection(this.ConnectionString);
        }
        public IDbConnection GetConnectionObject(string ConnectionString)
        {
            return new SqlConnection(ConnectionString);
        }

        public IDataReader GetDataReader(IDbCommand cmd)
        {
            var conn = GetConnectionObject(this.ConnectionString) as SqlConnection;
            cmd.Connection = conn;
            cmd.Connection.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        public IDataReader GetDataReader(IDbCommand cmd, IDbConnection conn)
        {
            throw new NotImplementedException();
        }

        public DataTable GetData(IDbCommand cmd)
        {
            return GetData(cmd, GetConnectionObject(this.ConnectionString));
        }
        public DataTable GetData(IDbCommand cmd, IDbConnection conn)
        {
            using (cmd)
            {
                using (conn)
                {
                    using (var da = new SqlDataAdapter((SqlCommand)cmd))
                    {
                        da.SelectCommand.Connection = conn as SqlConnection;
                        da.SelectCommand.Connection.Open();
                        var dt = new DataTable();
                        da.Fill(dt);

                        return dt;
                    }
                }
            }
        }

        public DataSet GetDataSet(IDbCommand cmd)
        {
            return GetDataSet(cmd, GetConnectionObject(this.ConnectionString));
        }
        public DataSet GetDataSet(IDbCommand cmd, IDbConnection conn)
        {
            using (cmd)
            {
                using (conn)
                {
                    using (var da = new SqlDataAdapter((SqlCommand)cmd))
                    {
                        da.SelectCommand.Connection = conn as SqlConnection;
                        da.SelectCommand.Connection.Open();
                        var ds = new DataSet();
                        da.Fill(ds);

                        return ds;
                    }
                }
            }
        }

        public IEnumerable<T> GetDataGeneric<T>(IDbCommand cmd) where T : class, new()
        {
            return GetDataGeneric<T>(cmd, GetConnectionObject(this.ConnectionString));
        }
        public IEnumerable<T> GetDataGeneric<T>(IDbCommand cmd, IDbConnection conn) where T : class, new()
        {
            var l = new List<T>();
            using (cmd)
            {
                using (conn)
                {
                    cmd.Connection = conn;
                    cmd.Connection.Open();
                    using (var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        var t = new T();
                        var ps = t.GetType().GetProperties(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance |
                                                           BindingFlags.Public | BindingFlags.SetField | BindingFlags.SetProperty).ToList();

                        while (dr.Read())
                        {
                            for (var i = 0; i < dr.FieldCount; i++)
                            {
                                var n = ps.FirstOrDefault(x => x.Name.ToLower() == dr.GetName(i).ToLower());
                                if (n == null) continue;
                                try
                                {
                                    n.SetValue(t, dr[i], null);
                                }
                                catch
                                {
                                    // Do Nothing
                                }
                            }
                            l.Add(t);
                            t = new T();
                        }
                    }
                }
            }
            return l;
        }

        public int SetData(IDbCommand cmd)
        {
            using (cmd)
            {
                using (var conn = new SqlConnection(this.ConnectionString))
                {
                    cmd.Connection = conn;
                    cmd.Connection.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public int SetData(IDbCommand cmd, IDbConnection conn)
        {
            throw new NotImplementedException();
        }

        private string _ConnectionString;
        public string ConnectionString
        {
            get
            {
                return !String.IsNullOrEmpty(_ConnectionString) ? _ConnectionString :
                    !String.IsNullOrEmpty(this.ConfigConnectionStringName) ? ConfigurationManager.ConnectionStrings[this.ConfigConnectionStringName].ConnectionString : String.Empty;
            }
            set
            {
                _ConnectionString = value;
            }
        }

        private String name;
        public String ConfigConnectionStringName
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
    }
}
