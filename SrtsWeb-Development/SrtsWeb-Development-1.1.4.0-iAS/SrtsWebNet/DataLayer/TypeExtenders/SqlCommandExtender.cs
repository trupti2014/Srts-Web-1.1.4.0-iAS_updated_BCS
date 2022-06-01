using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SrtsWeb.DataLayer.TypeExtenders
{
    public static class SqlCommandExtender
    {
        public static string connectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static DataTable ExecuteToDataTable(this SqlCommand cmd)
        {
            var dTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString("SRTS")))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.SelectCommand.Connection = conn;
                    conn.Open();
                    da.Fill(dTable);
                    conn.Close();
                }
            }

            return dTable;
        }

        public static DataSet ExecuteToDataSet(this SqlCommand cmd)
        {
            var dSet = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString("SRTS")))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.SelectCommand.Connection = conn;
                    conn.Open();
                    try
                    {
                        da.Fill(dSet);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("ExecuteToDataSet", ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            return dSet;
        }

        public static DbParameterCollection ExecuteToParameters(this SqlCommand cmd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString("SRTS")))
            {
                using (cmd)
                {
                    cmd.Connection = conn;
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("ExecuteToParameters", ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                    return cmd.Parameters as DbParameterCollection;
                }
            }
        }

        public static int ExecuteToScalar(this SqlCommand cmd)
        {
            int retVal;
            using (SqlConnection conn = new SqlConnection(connectionString("SRTS")))
            {
                using (cmd)
                {
                    cmd.Connection = conn;
                    conn.Open();
                    try
                    {
                        retVal = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("ExecuteToScalar", ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return retVal;
        }

        public static int ExecuteToNonQuery(this SqlCommand cmd)
        {
            int retVal;
            using (SqlConnection conn = new SqlConnection(connectionString("SRTS")))
            {
                using (cmd)
                {
                    cmd.Connection = conn;
                    conn.Open();
                    try
                    {
                        retVal = (int)cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("ExecuteNonQuery", ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return retVal;
        }

        public static void AddParameter(this SqlCommand cmd, string parmName, SqlDbType parmType, ParameterDirection parmDirection)
        {
            cmd.Parameters.Add(parmName, parmType);
            cmd.Parameters[parmName].Direction = parmDirection;
        }

        public static void AddParameter(this SqlCommand cmd, string parmName, SqlDbType parmType, ParameterDirection parmDirection, object parmValue)
        {
            AddParameter(cmd, parmName, parmType, parmDirection);
            cmd.Parameters[parmName].Value = parmValue;
        }

        public static void AddParameter(this SqlCommand cmd, string parmName, SqlDbType parmType, int parmSize, ParameterDirection parmDirection)
        {
            cmd.Parameters.Add(parmName, parmType, parmSize);
            cmd.Parameters[parmName].Direction = parmDirection;
        }

        public static void AddParameter(this SqlCommand cmd, string parmName, SqlDbType parmType, int parmSize, ParameterDirection parmDirection, object parmValue)
        {
            cmd.Parameters.Add(parmName, parmType, parmSize);
            cmd.Parameters[parmName].Direction = parmDirection;
            cmd.Parameters[parmName].Value = parmValue;
        }
    }
}