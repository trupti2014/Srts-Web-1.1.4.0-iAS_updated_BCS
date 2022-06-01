using DataBaseAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.DataLayer.RepositoryBase
{
    /// <summary>
    /// A custom base abstract class for all SRTSWeb repository classes to do database operations.
    /// </summary>
    /// <typeparam name="T">Generic class type T of a class to use in this base class.  T has to be a class and have a constructor.</typeparam>
    public abstract class RepositoryBase<T> where T : class, new()
    {
        protected IDatabaseAccessLayer DAL;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="DAL">Instantiated object of DatabaseAccessLayer.dll</param>
        public RepositoryBase(IDatabaseAccessLayer DAL)
        {
            this.DAL = DAL;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="DAL">Instantiated object of DatabaseAccessLayer.dll</param>
        /// <param name="catalogName">Initial database catalog to connect to.</param>
        public RepositoryBase(IDatabaseAccessLayer DAL, String catalogName)
        {
            this.DAL = DAL;
            this.DAL.CatalogName = catalogName;
        }

        /// <summary>
        /// Overridden this method will populate a class of type T.
        /// </summary>
        /// <param name="dr">Data reader to fill the class.</param>
        /// <returns>Populated class of type T.</returns>
        protected virtual T FillRecord(IDataReader dr)
        {
            return null;
        }

        /// <summary>
        /// Overridden this method will populate an object of type T.
        /// </summary>
        /// <param name="dt">Data table to fill the class.</param>
        /// <returns>Populated class of type T.</returns>
        protected virtual T FillRecord(DataTable dt)
        {
            return null;
        }

        /// <summary>
        /// Overridden this method will populate an enumeration of an object of type T.
        /// </summary>
        /// <param name="dt">Data table to fill the class enumeration.</param>
        /// <returns></returns>
        protected virtual IEnumerable<T> FillRecords(DataTable dt)
        {
            return null;
        }

        /// <summary>
        /// Overridden this method will populate an object of type T.
        /// </summary>
        /// <param name="ds">Dataset to fill the class.</param>
        /// <returns>Populated class of type T.</returns>
        protected virtual T FillRecord(DataSet ds)
        {
            return null;
        }

        /// <summary>
        /// Overridden this method will populate an enumeration of an object of type T.
        /// </summary>
        /// <param name="ds">Dataset to fill the class enumeration.</param>
        /// <returns></returns>
        protected virtual IEnumerable<T> FillRecords(DataSet ds)
        {
            return null;
        }

        /// <summary>
        /// Gets the object returned from the database.  Ex. DataTable.Rows[0][0]
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <returns>Single object from database.</returns>
        protected virtual Object GetObject(IDbCommand cmdIn)
        {
            using (cmdIn)
            {
                using (var dr = this.DAL.GetDataReader(cmdIn))
                {
                    using (cmdIn.Connection)
                    {
                        if (!dr.Read()) return null;
                        return dr[0];
                    }
                }
            }
        }

        /// <summary>
        /// Gets an object from the database by column name.  Ex. DataTable.Rows[0][columnName]
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <param name="columnName">Column name to get the data from the return set.</param>
        /// <returns>Single object from database.</returns>
        protected virtual Object GetObject(IDbCommand cmdIn, String columnName)
        {
            using (cmdIn)
            {
                using (var dr = this.DAL.GetDataReader(cmdIn))
                {
                    using (cmdIn.Connection)
                    {
                        if (!dr.Read()) return null;
                        return dr[columnName];
                    }
                }
            }
        }

        /// <summary>
        /// Gets an enumeration of a single object in the first column of every row in a return set.
        /// Ex.  for(var i = 0; i \< DataTable.Rows.Count; i++)
        ///         DataTable.Rows[i][0]
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <returns>Enumeration of objects from database.</returns>
        protected virtual IEnumerable<Object> GetObjects(IDbCommand cmdIn)
        {
            var ol = new List<Object>();
            using (cmdIn)
            {
                using (var dr = this.DAL.GetDataReader(cmdIn))
                {
                    using (cmdIn.Connection)
                    {
                        while (dr.Read())
                        {
                            ol.Add(dr[0]);
                        }
                    }
                }
            }

            return ol;
        }

        /// <summary>
        /// Returns an enumeration of an object by column name.
        /// Ex.  for(var i = 0; i \< DataTable.Rows.Count; i++)
        ///         DataTable.Rows[i][columnName]
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <param name="columnName">Column name to get the data from the return set.</param>
        /// <returns>Enumeration of objects from database.</returns>
        protected virtual IEnumerable<Object> GetObjects(IDbCommand cmdIn, String columnName)
        {
            var ol = new List<Object>();
            using (cmdIn)
            {
                using (var dr = this.DAL.GetDataReader(cmdIn))
                {
                    using (cmdIn.Connection)
                    {
                        while (dr.Read())
                        {
                            ol.Add(dr[columnName]);
                        }
                    }
                }
            }

            return ol;
        }

        /// <summary>
        /// Gets a single class of type T from the database.
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <returns>Generic class of type T.</returns>
        protected virtual T GetRecord(IDbCommand cmdIn)
        {
            using (cmdIn)
            {
                using (var dr = this.DAL.GetDataReader(cmdIn))
                {
                    using (cmdIn.Connection)
                    {
                        if (!dr.Read()) return null;
                        return FillRecord(dr);
                    }
                }
            }
        }

        /// <summary>
        /// Gets an enumeration of a class of type T from the database.
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <returns>Enumerated generic class of type T.</returns>
        protected virtual IEnumerable<T> GetRecords(IDbCommand cmdIn)
        {
            var tl = new List<T>();
            using (cmdIn)
            {
                using (var dr = this.DAL.GetDataReader(cmdIn))
                {
                    using (cmdIn.Connection)
                    {
                        if (dr.IsClosed) return tl;

                        while (dr.Read())
                        {
                            var t = FillRecord(dr);
                            tl.Add(t);
                        }
                    }
                }
            }
            return tl;
        }

        /// <summary>
        /// Gets a dictionary of the output parameter names and values from the command object after execution against the database.
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <returns>Dictionary of output parameter values.</returns>
        protected virtual IDictionary<String, Object> GetOutputParameterValues(IDbCommand cmdIn)
        {
            using (cmdIn)
            {
                using (cmdIn.Connection)
                {
                    this.DAL.SetData(cmdIn);
                    return cmdIn.Parameters.Cast<IDataParameter>().ToList().Where(x => x.Direction == ParameterDirection.Output).Select(s => new { Key = s.ParameterName, Value = s.Value }).ToDictionary(d => d.Key, d => d.Value);
                }
            }
        }

        /// <summary>
        /// Inserts data into the database.
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <returns>Number of rows affected.</returns>
        protected virtual Int32 InsertData(IDbCommand cmdIn)
        {
            using (cmdIn)
            {
                using (cmdIn.Connection)
                {
                    return this.DAL.SetData(cmdIn);
                }
            }
        }

        /// <summary>
        /// Updates data already in the database.
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <returns>Number of rows affected.</returns>
        protected virtual Int32 UpdateData(IDbCommand cmdIn)
        {
            using (cmdIn)
            {
                using (cmdIn.Connection)
                {
                    return this.DAL.SetData(cmdIn);
                }
            }
        }

        /// <summary>
        /// Deletes data from the database.
        /// </summary>
        /// <param name="cmdIn">Command object to execute against the database.</param>
        /// <returns>Number of rows affected.</returns>
        protected virtual Int32 DeleteData(IDbCommand cmdIn)
        {
            using (cmdIn)
            {
                using (cmdIn.Connection)
                {
                    return this.DAL.SetData(cmdIn);
                }
            }
        }
    }
}