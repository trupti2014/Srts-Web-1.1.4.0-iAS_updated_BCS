using DataBaseAccessLayer;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repostiroy class to handle BMT Excel operations.
    /// </summary>
    public class ExcelRepository : RepositoryBase<BmtEntity>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="ConfigConnectionStringName">Connection string to interface with Excel; this is not the interop.</param>
        /// <param name="fileLoc">File path on host system.</param>
        public ExcelRepository(String ConfigConnectionStringName, String fileLoc)
            : base(DbFactory.GetDbObject(DataBaseType.Excel))
        {
            base.DAL.ConfigConnectionStringName = ConfigConnectionStringName;
            base.DAL.ConnectionString = fileLoc;
        }

        /// <summary>
        /// Gets a list of BmtEntity class from an Excel file.
        /// </summary>
        /// <returns>BmtEntity list from Excel.</returns>
        public List<BmtEntity> GetBmtFileData()
        {
            using (var cmd = this.DAL.GetCommandObject())
            {
                cmd.CommandText = @"SELECT * FROM [Sheet1$] WHERE (NOT IsNull([LastName])) AND (NOT IsNull([FirstName])) AND (NOT IsNull([IdNumber])) AND (NOT IsNull([IdNumberType]))";
                cmd.CommandType = CommandType.Text;

                return GetRecords(cmd).ToList();
            }
        }

        protected override BmtEntity FillRecord(IDataReader dr)
        {
            var e = new BmtEntity();

            e.Address1 = HttpUtility.HtmlEncode(dr["Address1"].ToString());
            e.Address2 = HttpUtility.HtmlEncode(dr["Address2"].ToString());
            e.Address3 = HttpUtility.HtmlEncode(dr["Address3"].ToString());
            e.BOS = HttpUtility.HtmlEncode(dr["BOS"].ToString().ToUpper());
            e.City = HttpUtility.HtmlEncode(dr["City"].ToString());
            e.Country = HttpUtility.HtmlEncode(dr["Country"].ToString().ToUpper());
            e.DOB = HttpUtility.HtmlEncode(dr["DOB"].ToString());
            e.FirstName = HttpUtility.HtmlEncode(dr["FirstName"].ToString());
            e.Gender = HttpUtility.HtmlEncode(dr["Gender"].ToString().ToUpper());
            e.Grade = HttpUtility.HtmlEncode(dr["Grade"].ToString().ToUpper());
            e.IdNumber = HttpUtility.HtmlEncode(dr["IdNumber"].ToString());
            e.IdNumberType = HttpUtility.HtmlEncode(dr["IdNumberType"].ToString().ToUpper());
            e.LastName = HttpUtility.HtmlEncode(dr["LastName"].ToString());
            e.MiddleName = HttpUtility.HtmlEncode(dr["MiddleName"].ToString());
            e.State = HttpUtility.HtmlEncode(dr["State"].ToString().ToUpper());
            e.UIC = HttpUtility.HtmlEncode(dr["UIC"].ToString());
            e.ZipCode = HttpUtility.HtmlEncode(dr["Zip"].ToString());

            return e;
        }
    }
}