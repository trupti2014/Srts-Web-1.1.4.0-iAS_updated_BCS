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
using System.Data.SqlClient;

namespace SrtsWeb.DataLayer.Repositories
{
    public class DocumentManagerRepository
    {
        /// <summary>
        /// A custom repository class to do report management operations.
        /// </summary>
        public class PrescriptionDocumentRepository : RepositoryBase<PrescriptionDocument>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public PrescriptionDocumentRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            { }


            /// <summary>
            /// Inserts an uploaded prescription image to a saved prescription.
            /// <param name="IndividualID">Patient ID the Rx belongs to.</param> 
            /// <param name="RxID">Prescription ID to associated the image with.</param>
            /// <param name="DocName">Document name.</param>
            /// <param name="DocType">Document type.</param>
            /// <param name="RxScan">Binary file data.</param>
            /// <returns>Success/failure of insert.</returns>
            public bool InsertScannedPrescription(Int32 IndividualID, Int32 RxID, String DocName, String DocType, byte[] RxScan)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "InsertScannedRx";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", IndividualID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RxID", RxID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DocName", DocName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DocType", DocType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RxScan", RxScan));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", 0, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }


            public PrescriptionDocument GetScannedPrescription(Int32 RxScanId, Int32 RxId)
            {
                //var cmd = this.DAL.GetCommandObject();
                //cmd.CommandText = "GetScannedRxInfoByID";
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Add(this.DAL.GetParamenter("@RxID", RxScanId));
                //cmd.Parameters.Add(this.DAL.GetParamenter("@ID", RxId));
                //var p = GetRecord(cmd);

                byte[] buffer = null;
                var p = (PrescriptionDocument)null;
               string connStr = SrtsWeb.ExtendersHelpers.Globals.ConnStrNm;
               SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[connStr].ToString());

                //SqlCommand command = new SqlCommand("GetScannedRxFileByID", connection);
                SqlCommand command = new SqlCommand("GetScannedRxInfoByID", connection);
                //SqlCommand command = new SqlCommand("GetScannedRxByID", connection);
                //SqlCommand command = new SqlCommand("GetPrescriptionByID", connection);
                
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RxID", RxScanId);
                command.Parameters.AddWithValue("@ID", RxId);

                //command.Parameters.AddWithValue("@ID", "25565");
                //command.Parameters.AddWithValue("@ModifiedBy", "miller, Jessie");

                SqlDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var e = new PrescriptionDocument();
                        e.DocumentName = reader["DocName"].ToString();
                        e.DocumentType = reader["DocType"].ToString();
                        //p = FillRecord(reader);
                       //
                        //string name = reader["DocName"].ToString();
                        //buffer = (byte[])reader["RxScan"];

                        //  string patientID = reader["IndividualID_Patient"].ToString();

                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    string msg = ex.Message.ToString();
                }
                finally
                {
                    connection.Close();
                }
                           
                
                return p;
            }

            protected override PrescriptionDocument FillRecord(IDataReader dr)
            {
                var e = new PrescriptionDocument();
                e.DocumentName = dr["DocName"].ToString();
                e.DocumentType = dr["DocType"].ToString();
                //e.DocumentImage = (byte[])(dr["RxScan"]);
                e.ScanDate = dr["ScanDate"].ToDateTime();
                e.ScanDate = dr["DelDate"].ToDateTime();
                //e.DocumentFileStream = (System.IO.Stream)(dr["RxScan"]);
                e.PrescriptionId = Convert.ToInt32(dr["RxID"]);
                e.IndividualId = Convert.ToInt32(dr["IndividualID"]);
                return e;
            }


            ///// <summary>
            ///// Gets a single order by order number.
            ///// </summary>
            ///// <param name="orderNumber">Order number to search with.</param>
            ///// <returns>Single order that matches the order number with patient and order address</returns>
            //public OrderLabelAddresses GetScannedRxbyID(String orderNumber)
            //{
            //    var cmd = this.DAL.GetCommandObject();
            //    cmd.CommandText = "GetAddresses";
            //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //    cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
            //    var o = GetRecord(cmd);
            //    return o;
            //}


            //protected override OrderLabelAddresses FillRecord(System.Data.IDataReader dr)
            //{
            //    var orderAddress = new OrderLabelAddresses();
            //    orderAddress.OrderNumber = dr["OrderNumber"].ToString();
            //    orderAddress.PatientId = Convert.ToInt32(dr["IndividualID_Patient"]);
            //    // orderAddress.Patient = dr["Patient"].ToString();
            //    orderAddress.FirstName = dr["Firstname"].ToString();
            //    orderAddress.MiddleName = dr["MiddleName"].ToString();
            //    orderAddress.LastName = dr["LastName"].ToString();
            //    orderAddress.Address1 = dr["Address1"].ToString();
            //    orderAddress.Address2 = dr["Address2"].ToString();
            //    orderAddress.Address3 = dr["Address3"].ToString();
            //    orderAddress.City = dr["City"].ToString();
            //    orderAddress.State = dr["State"].ToString();
            //    orderAddress.CountryCode = dr["CountryCode"].ToString();
            //    orderAddress.CountryName = dr["CountryName"].ToString();
            //    orderAddress.ZipCode = dr["ZipCode"].ToString();
            //    orderAddress.ShipAddress1 = dr["ShipAddress1"].ToString();
            //    orderAddress.ShipAddress2 = dr["ShipAddress2"].ToString();
            //    orderAddress.ShipAddress3 = dr["ShipAddress3"].ToString();
            //    orderAddress.ShipCity = dr["ShipCity"].ToString();
            //    orderAddress.ShipState = dr["ShipState"].ToString();
            //    orderAddress.ShipCountryCode = dr["ShipCountryCode"].ToString();
            //    orderAddress.ShipCountryName = dr["ShipCountryName"].ToString();
            //    orderAddress.ShipZipCode = dr["ShipZipCode"].ToString();
            //    orderAddress.DateVerified = dr["DateVerified"].ToDateTime();
            //    orderAddress.ExpireDays = (string.IsNullOrEmpty(dr["ExpireDays"].ToString())) ? 0 : Convert.ToInt32(dr["ExpireDays"]);
            //    return orderAddress;
            //}
         

        }

    }

    ///// <summary>
    ///// A custom repository class to handle Rx Document operations.
    ///// </summary>
    //public class PrescriptionDocumentRepository : RepositoryBase<PrescriptionDocument>
    //{
    //    /// <summary>
    //    /// Default ctor.
    //    /// </summary>
    //    public PrescriptionDocumentRepository()
    //        : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
    //    {
    //    }

    //    #region PRESCRIPTION DOCUMENT


    //    /// <summary>
    //    /// Inserts an uploaded prescription image to a saved prescription.
    //    /// <param name="IndividualID">Patient ID the Rx belongs to.</param> 
    //    /// <param name="RxID">Prescription ID to associated the image with.</param>
    //    /// <param name="DocName">Document name.</param>
    //    /// <param name="DocType">Document type.</param>
    //    /// <param name="RxScan">Binary file data.</param>
    //    /// <returns>Success/failure of insert.</returns>
    //    public bool InsertScannedPrescription(Int32 IndividualID, Int32 RxID, String DocName, String DocType, byte[] RxScan)
    //    {
    //        var cmd = this.DAL.GetCommandObject();
    //        cmd.CommandText = "InsertScannedRx";
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", IndividualID));
    //        cmd.Parameters.Add(this.DAL.GetParamenter("@RxID", RxID));
    //        cmd.Parameters.Add(this.DAL.GetParamenter("@DocName", DocName));
    //        cmd.Parameters.Add(this.DAL.GetParamenter("@DocType", DocType));
    //        cmd.Parameters.Add(this.DAL.GetParamenter("@RxScan", RxScan));
    //        cmd.Parameters.Add(this.DAL.GetParamenter("@Success", 0, ParameterDirection.Output));

    //        InsertData(cmd);

    //        var p = cmd.Parameters["@Success"] as IDataParameter;
    //        return p.Value.ToInt32().Equals(1);
    //    }



    //    public PrescriptionDocument GetScannedPrescription(Int32 RxScanId, Int32 RxId)
    //    {
    //        var cmd = this.DAL.GetCommandObject();
    //        cmd.CommandText = "GetScannedRxInfoByID";
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.Parameters.Add(this.DAL.GetParamenter("@RxID", RxScanId));
    //        cmd.Parameters.Add(this.DAL.GetParamenter("@ID", RxId));
    //        var p = GetRecord(cmd);
    //        return p;
    //    }

    //   protected override PrescriptionDocument FillRecord(IDataReader dr)
    //    {
    //        var e = new PrescriptionDocument();
    //        e.DocumentName = dr["DocName"].ToString();
    //        e.DocumentType = dr["DocType"].ToString();
    //        e.DocumentFileStream = (System.IO.Stream)(dr["RxScan"]);
    //        e.PrescriptionId = Convert.ToInt32(dr["RxID"]);
    //        e.IndividualId = Convert.ToInt32(dr["IndividualID"]);
    //        return e;
    //    }



    //    #endregion PRESCRIPTION DOCUMENT

    //}
}
