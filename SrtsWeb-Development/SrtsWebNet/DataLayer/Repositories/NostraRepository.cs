using SrtsWeb.DataLayer.TypeExtenders;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SrtsWeb.DataLayer.Repositories
{
    public sealed class NostraRepository : INostraRepository
    {
        public DataTable GetNostraFileData(String _siteCode)
        {
            DataTable ds = new DataTable();
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "GetNostraFileData";
                sqlCmd.Parameters.AddWithValue("@SiteCode", _siteCode);
                sqlCmd.Parameters.AddWithValue("@OrderNbr", null);
                ds = sqlCmd.ExecuteToDataTable();
            }
            return ds;
        }

        public bool SetNostraCsvData(List<NostraFileEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}