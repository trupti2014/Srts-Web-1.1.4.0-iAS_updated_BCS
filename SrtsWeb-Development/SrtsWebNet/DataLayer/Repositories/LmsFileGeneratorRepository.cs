using DataBaseAccessLayer;
using DataToObjectLib;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class to handle LMS file data operations.
    /// </summary>
    public class LmsFileGeneratorRepository : RepositoryBase<LmsFileEntity>, ILmsFileGeneratorRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public LmsFileGeneratorRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        /// <summary>
        /// Gets the same LMS file data that the LMS File Gnerator application generates.
        /// </summary>
        /// <param name="_siteCode">Lab site code to get the data for.</param>
        /// <returns>LMS file data.</returns>
        public IEnumerable<LmsFileEntity> GetLmsFileData(String _siteCode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetNostraFileData";
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", _siteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNbr", null));

            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Updates the status of orders to either lab recieved or lab dispensed.
        /// </summary>
        /// <param name="labCode">Lab site code to perform action against.</param>
        /// <param name="orderNumber">Coma delimeted list of order numbers to update.</param>
        /// <param name="isActive">Active/Inactive status.</param>
        /// <param name="comment">Status comment.</param>
        /// <param name="modifiedBy">User performing the update operation.</param>
        /// <param name="statusId">Status to change the orders to.</param>
        /// <returns>Success/Filure of update.</returns>
        public Boolean UpdateOrderStatus(string labCode, string orderNumber, bool isActive, string comment, string modifiedBy, int statusId)
        {
            var good = false;

            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertPatientOrderStatus";
            cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", labCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", isActive));
            cmd.Parameters.Add(this.DAL.GetParamenter("@StatusComment", comment));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderStatusTypeID", statusId));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));

            var i = UpdateData(cmd);

            if (orderNumber.Contains(','))
                good = i >= 2 ? true : false;
            else
                good = i.Equals(1) ? true : false;

            return good;
            //using (SqlCommand cmd = new SqlCommand())
            //{
            //    using (cmd.Connection = new SqlConnection())
            //    {
            //        cmd.Connection.ConnectionString = CONN_STR;
            //        cmd.Connection.Open();

            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.CommandText = "InsertPatientOrderStatus";
            //        cmd.Parameters.AddWithValue("@LabSiteCode", labCode);
            //        cmd.Parameters.AddWithValue("@OrderNumber", orderNumber);
            //        cmd.Parameters.AddWithValue("@IsActive", isActive);
            //        cmd.Parameters.AddWithValue("@StatusComment", comment);
            //        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
            //        cmd.Parameters.AddWithValue("@OrderStatusTypeID", statusId);

            //        try
            //        {
            //            var i = cmd.ExecuteNonQuery();
            //            if (orderNumber.Contains(","))
            //                good = i >= 2 ? true : false;
            //            else
            //                good = i.Equals(2) ? true : false;
            //        }
            //        catch
            //        {
            //            good = false;
            //        }
            //    }
            //}

            //return good;
        }

        /// <summary>
        /// Updates the status of orders to either lab recieved or lab dispensed.
        /// </summary>
        /// <param name="labCode">Lab site code to perform action against.</param>
        /// <param name="orderNumbers">List of order numbers to update.</param>
        /// <param name="isActive">Active/Inactive status.</param>
        /// <param name="comment">Status comment.</param>
        /// <param name="modifiedBy">User performing the update operation.</param>
        /// <param name="statusId">Status to change the orders to.</param>
        /// <returns>Success/Filure of update.</returns>
        public Boolean UpdateOrderStatus(string labCode, List<string> orderNumbers, bool isActive, string comment, string modifiedBy, int statusId)
        {
            var os = new StringBuilder();
            // Create comma delimited order number string
            foreach (var s in orderNumbers)
                os.AppendFormat("{0},", s);

            var ss = os.ToString();
            ss = ss.Remove(ss.Length - 1);

            return UpdateOrderStatus(labCode, ss, isActive, comment, modifiedBy, statusId);
        }

        protected override LmsFileEntity FillRecord(IDataReader dr)
        {
            var o = new ObjectMapper();
            return o.MapSingleEntity<LmsFileEntity>(dr);
        }
    }
}