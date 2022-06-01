using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.DataLayer.RepositoryBase;
using DataBaseAccessLayer;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// A custom repository class to do report management operations.
    /// </summary>
    public class ReportsManagerRepository : RepositoryBase<ReprintEntity>
    {
        private String StatusType;

        /// <summary>
        /// Default ctor.
        /// </summary>
        public ReportsManagerRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        /// <summary>
        /// Gets a list of reprint form data.
        /// </summary>
        /// <param name="status">Order status to determine what reprint data to return.</param>
        /// <param name="siteCode">Site code to search with.</param>
        /// <returns>ReprintEntity</returns>
        public List<ReprintEntity> GetReprintCountList(String status, String siteCode)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetReprintCountList";
            this.StatusType = status;
            cmd.Parameters.Add(DAL.GetParamenter("@OrderStatusTypeID", status));
            cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", siteCode));

            return GetRecords(cmd).ToList();
        }

        public List<ReprintEntity> GetReprintOnDemandCountList(String siteCode)
        {
            var cmd = DAL.GetCommandObject();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetLabelsOnDemandCount";
            this.StatusType = "od";
            cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", siteCode));

            return GetRecords(cmd).ToList();
        }

        protected override ReprintEntity FillRecord(System.Data.IDataReader dr)
        {
            var re = new ReprintEntity();
            re.Modifier = dr.AsString("ModifiedBy");

            if (this.StatusType.ToString() == "od")
            {
                re.ReportCount = dr.ToInt32("OrderCnt");
                re.BatchDate = dr.ToDateTime("BatchDate");
            }
            else
            {
                re.ReportType = this.StatusType.ToString();
                re.ReportCount = dr.ToInt32("RecCount");
                re.BatchDate = dr.ToDateTime("OrderKey");

            }

            return re;
            //var re = new ReprintEntity();
            //re.ReportType = this.StatusType.ToString();
            //re.ReportCount = dr.ToInt32("RecCount");
            //re.Modifier = dr.AsString("ModifiedBy");
            //re.BatchDate = dr.ToDateTime("OrderKey");

            //return re;
        }

        /// <summary>
        /// A custom repository class to do reprint operations.
        /// </summary>
        public class ReprintReturnRepository : RepositoryBase<ReprintReturnEntity>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public ReprintReturnRepository() :
                base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm)) { }

            /// <summary>
            /// Gets a list of label data for reprint.
            /// </summary>
            /// <param name="status">Status to determine what type of data to return.</param>
            /// <param name="siteCode">Site code where data was generated.</param>
            /// <param name="batchDate">Compound key for the reprint data.</param>
            /// <returns>ReprintReturnEntity list of label data.</returns>
            public List<ReprintReturnEntity> GetReprintLabel(String status, String siteCode, DateTime batchDate)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "ReprintReturn";
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", siteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@OrderSTatusTypeID", status));
                cmd.Parameters.Add(DAL.GetParamenter("@DateTime", batchDate));

                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a comma delimited list of order numbers for reprint.
            /// </summary>
            /// <param name="status">Status to determine what type of data to return.</param>
            /// <param name="siteCode">Site code where data was generated.</param>
            /// <param name="batchDate">Compound key for the reprint data.</param>
            /// <returns>Comma delimited string or order numbers.</returns>
            public String GetReprint771(String status, String siteCode, DateTime batchDate)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "ReprintReturn";
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", siteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@OrderSTatusTypeID", status));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@DateTime", System.Data.SqlDbType.SmallDateTime) { Value = batchDate });//DAL.GetParamenter("@DateTime",batchDate.ToShortDateString()));

                var o = GetObject(cmd);
                return o.IsNull() ? String.Empty : o.ToString();
            }

            protected override ReprintReturnEntity FillRecord(System.Data.IDataReader dr)
            {
                var r = new ReprintReturnEntity();

                r.Address1 = dr.AsString("ShipAddress1");
                r.Address2 = dr.AsString("ShipAddress2");
                r.Address3 = dr.AsString("ShipAddress3");
                r.City = dr.AsString("ShipCity");
                r.Country = dr.AsString("ShipCountry");
                r.ID = dr.ToInt32("ID");
                r.Name = dr.AsString("Patient");
                r.State = dr.AsString("ShipState");
                r.ZipCode = dr.AsString("ShipZipCode");

                return r;
            }
        }




    }

    public class OnDemandLabelsRepository
    {
        /// <summary>
        /// A custom repository class to do report management operations.
        /// </summary>
        public class OrderLabelAddressesRepository : RepositoryBase<OrderLabelAddresses>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public OrderLabelAddressesRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }


            /// <summary>
            /// Gets a single order by order number.
            /// </summary>
            /// <param name="orderNumber">Order number to search with.</param>
            /// <returns>Single order that matches the order number with patient and order address</returns>
            public OrderLabelAddresses GetAddressesByOrderNumber(String orderNumber)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetAddresses";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
                var o = GetRecord(cmd);
                return o;
            }


            protected override OrderLabelAddresses FillRecord(System.Data.IDataReader dr)
            {
                var orderAddress = new OrderLabelAddresses();
                orderAddress.OrderNumber = dr["OrderNumber"].ToString();
                orderAddress.PatientId = Convert.ToInt32(dr["IndividualID_Patient"]);
                // orderAddress.Patient = dr["Patient"].ToString();
                orderAddress.FirstName = dr["Firstname"].ToString();
                orderAddress.MiddleName = dr["MiddleName"].ToString();
                orderAddress.LastName = dr["LastName"].ToString();
                orderAddress.Address1 = dr["Address1"].ToString();
                orderAddress.Address2 = dr["Address2"].ToString();
                orderAddress.Address3 = dr["Address3"].ToString();
                orderAddress.City = dr["City"].ToString();
                orderAddress.State = dr["State"].ToString();
                orderAddress.CountryCode = dr["CountryCode"].ToString();
                orderAddress.CountryName = dr["CountryName"].ToString();
                orderAddress.ZipCode = dr["ZipCode"].ToString();
                orderAddress.ShipAddress1 = dr["ShipAddress1"].ToString();
                orderAddress.ShipAddress2 = dr["ShipAddress2"].ToString();
                orderAddress.ShipAddress3 = dr["ShipAddress3"].ToString();
                orderAddress.ShipCity = dr["ShipCity"].ToString();
                orderAddress.ShipState = dr["ShipState"].ToString();
                orderAddress.ShipCountryCode = dr["ShipCountryCode"].ToString();
                orderAddress.ShipCountryName = dr["ShipCountryName"].ToString();
                orderAddress.ShipZipCode = dr["ShipZipCode"].ToString();
                orderAddress.DateVerified = dr["DateVerified"].ToDateTime();
                orderAddress.ExpireDays = (string.IsNullOrEmpty(dr["ExpireDays"].ToString())) ? 0 : Convert.ToInt32(dr["ExpireDays"]);
                return orderAddress;
            }
        }


        /// <summary>
        /// A custom repository class to do report management operations.
        /// </summary>
        public class ReprintOnDemandRepository : RepositoryBase<ReprintOnDemandInsertEntity>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public ReprintOnDemandRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }


            /// <summary>
            /// Inserts an on demand label for reprint.
            /// </summary>
            /// <param name="sitecode">Frame item preference to add/update.</param>
            /// <param name="modifiedBy">User performing operation.</param>
            /// <param name="addressType">Type of address to use.</param>
            /// <param name="orderNumber">Order number this applies to</param>
            /// <returns>Success/failure of operation.</returns>
            public Boolean InsertOnDemandLabel(ReprintOnDemandInsertEntity entity, String modifiedBy)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandText = "InsertLabelsOnDemand";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", entity.SiteCode));
                cmd.Parameters.Add(DAL.GetParamenter("@OrderNumber", entity.OrderNumber));
                cmd.Parameters.Add(DAL.GetParamenter("@AddressType", entity.AddressType));
                cmd.Parameters.Add(DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }



        }


        /// <summary>
        /// A custom repository class to do reprint operations.
        /// </summary>
        public class ReprintOnDemandReturnRepository : RepositoryBase<ReprintOnDemandReturnEntity>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public ReprintOnDemandReturnRepository() :
                base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm)) { }

            /// <summary>
            /// Gets a list of label data for reprint.
            /// </summary>
            /// <param name="status">Status to determine what type of data to return.</param>
            /// <param name="siteCode">Site code where data was generated.</param>
            /// <param name="batchDate">Compound key for the reprint data.</param>
            /// <returns>ReprintReturnEntity list of label data.</returns>
            public List<ReprintOnDemandReturnEntity> GetOnDemandReprintLabel(String siteCode, DateTime batchDate)
            {
                var cmd = DAL.GetCommandObject();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetLabelsOnDemand";
                cmd.Parameters.Add(DAL.GetParamenter("@BatchDate", batchDate));
                //cmd.Parameters.Add(DAL.GetParamenter("@SiteCode", siteCode));
                //cmd.Parameters.Add(DAL.GetParamenter("@OrderSTatusTypeID", status));


                return GetRecords(cmd).ToList();
            }


            protected override ReprintOnDemandReturnEntity FillRecord(System.Data.IDataReader dr)
            {
                var r = new ReprintOnDemandReturnEntity();

                r.Ordernumber = dr.AsString("OrderNumber");
                r.AddressType = dr.AsString("AddressType");
                r.FirstName = dr.AsString("FirstName");
                r.MiddleName = dr.AsString("MiddleName");
                r.LastName = dr.AsString("Lastname");
                r.Address1 = dr.AsString("Address1");
                r.Address2 = dr.AsString("Address2");
                r.Address3 = dr.AsString("Address3");
                r.City = dr.AsString("City");
                r.Country = dr.AsString("Country");
                r.State = dr.AsString("State");
                r.ZipCode = dr.AsString("ZipCode");

                return r;
            }

        }

    }
}
