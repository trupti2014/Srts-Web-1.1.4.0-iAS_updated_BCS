using DataBaseAccessLayer;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SrtsWeb.DataLayer.Repositories
{
    public class OrderManagementRepository
    {
        /// <summary>
        /// A custom repository class to handle order operations.
        /// </summary>
        public class OrderRepository : RepositoryBase<Order>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public OrderRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            #region ORDER

            /// <summary>
            /// Gets a single order by order number.
            /// </summary>
            /// <param name="orderNumber">Order number to search with.</param>
            /// <param name="modifiedBy">User performing the get operation.</param>
            /// <returns>Single order that matches the order number</returns>
            public Order GetOrderByOrderNumber(String orderNumber, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetPatientOrderNONGEyesByOrderNumber";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));

                var o = GetRecord(cmd);

                return o;
            }

            /// <summary>
            /// Gets all orders for a patient.
            /// </summary>
            /// <param name="patientId">Patient ID to search with.</param>
            /// <param name="modifiedBy">User performing the get operation.</param>
            /// <returns>List of orders for a patient.</returns>
            public List<Order> GetAllOrders(Int32 patientId, String modifiedBy, string clinicCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetPatientOrderByIndividualIdNonGEyes";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", patientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicCode", clinicCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));

                var lo = GetRecords(cmd).ToList();

                return lo;
            }

            /// <summary>
            /// Adds a new order to the database.
            /// </summary>
            /// <param name="order">Order to be inserted.</param>
            /// <param name="modifiedBy">User performing the get operation.</param>
            /// <returns>Success/failure of insert</returns>
            public bool InsertOrder(Order order, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = order.IsComplete ? "InsertPatientOrder" : "InsertPatientOrderIncomplete";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", order.OrderNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Patient", order.PatientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Tech", order.TechnicianId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", order.Demographic));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LensType", order.LensType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LensMaterial", order.Material));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Tint", order.Tint));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Coatings", order.Coatings));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODSegHeight", order.OdSegHeight));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSSegHeight", order.OsSegHeight));
                cmd.Parameters.Add(this.DAL.GetParamenter("@NumberOfCases", order.Cases));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Pairs", order.Pairs));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PrescriptionID", order.PrescriptionId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PatientPhoneID", 0));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", order.Frame));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameColor", order.Color));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameEyeSize", order.Eye));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameBridgeSize", order.Bridge));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameTempleType", order.Temple));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", order.ClinicSiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", order.LabSiteCode));

                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipToPatient", order.OrderDisbursement.Equals("L2P")));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicShipToPatient", order.OrderDisbursement.Equals("C2P")));
                
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddress1", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddress2", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddress3", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipCity", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipState", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipZipCode", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipCountry", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddressType", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LocationCode", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@UserComment1", order.Comment1));
                cmd.Parameters.Add(this.DAL.GetParamenter("@UserComment2", order.Comment2));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", order.IsActive));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsGEyes", order.IsGEyes));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsMultivision", order.IsMultivision));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailPatient", order.IsEmailPatient));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PatientEmail", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OnholdForConfirmation", false));
                cmd.Parameters.Add(this.DAL.GetParamenter("@VerifiedBy", 0));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsComplete", order.IsComplete));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LinkedID", order.LinkedId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ONBarCode", order.OrderNumberBarCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ReOrder", order.IsReOrder));
                cmd.Parameters.Add(this.DAL.GetParamenter("@TheaterLocationCode", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DispenseComments", order.DispenseComments));
                //cmd.Parameters.Add(this.DAL.GetParamenter("@GroupName", order.GroupName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Updates an order in the database.
            /// </summary>
            /// <param name="order">Order to be updated.</param>
            /// <param name="modifiedBy">User performing the get operation.</param>
            /// <returns>Success/failure of update</returns>
            public string UpdateOrder(Order order, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "UpdatePatientOrderByOrderNumber";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", order.OrderNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Patient", order.PatientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Tech", order.TechnicianId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", order.Demographic));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LensType", order.LensType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LensMaterial", order.Material));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Tint", order.Tint));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Coatings", order.Coatings));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODSegHeight", order.OdSegHeight));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSSegHeight", order.OsSegHeight));
                cmd.Parameters.Add(this.DAL.GetParamenter("@NumberOfCases", order.Cases));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Pairs", order.Pairs));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PrescriptionID", order.PrescriptionId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PatientPhoneID", 0));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", order.Frame));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameColor", order.Color));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameEyeSize", order.Eye));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameBridgeSize", order.Bridge));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameTempleType", order.Temple));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", order.ClinicSiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", order.LabSiteCode));

                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipToPatient", order.OrderDisbursement.Equals("L2P")));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicShipToPatient", order.OrderDisbursement.Equals("C2P")));

                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddress1", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddress2", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddress3", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipCity", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipState", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipZipCode", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipCountry", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddressType", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LocationCode", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@UserComment1", order.Comment1));
                cmd.Parameters.Add(this.DAL.GetParamenter("@UserComment2", order.Comment2));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", order.IsActive));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsGEyes", order.IsGEyes));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsMultivision", order.IsMultivision));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PatientEmail", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@MedProsDispense", false));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PimrsDispense", false));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OnholdForConfirmation", false));
                cmd.Parameters.Add(this.DAL.GetParamenter("@VerifiedBy", 0));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsComplete", order.IsComplete));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LinkedID", order.LinkedId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ONBarCode", order.OrderNumberBarCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@TheaterLocationCode", String.Empty));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DoLinkedUpdate", !String.IsNullOrEmpty(order.LinkedId)));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DispenseComments", order.DispenseComments));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailPatient", order.IsEmailPatient));
                //cmd.Parameters.Add(this.DAL.GetParamenter("@GroupName", order.GroupName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
                SqlParameter labsitecode = new SqlParameter
                {
                    ParameterName = "@LabOut",
                    Direction = ParameterDirection.Output,
                    DbType = DbType.String,
                    Size = 8,
                    Value = DBNull.Value,
                };
                cmd.Parameters.Add(labsitecode);
                //cmd.Parameters.Add(this.DAL.GetParamenter("@Success", 0, ParameterDirection.Output));
                //GetObject(cmd);
                //var p = cmd.Parameters["@LabSiteCode"] as IDataParameter;
                //return p.Value.ToString();

                UpdateData(cmd);
                //var p = cmd.Parameters["@Success"] as IDataParameter;
                var l = cmd.Parameters["@LabOut"] as IDataParameter;
                return string.IsNullOrEmpty(l.Value.ToString()) ? String.Empty : l.Value.ToString();
            }

            /// <summary>
            /// Deletes an order from the database.
            /// </summary>
            /// <param name="orderNumber">Order number to delete.</param>
            /// <param name="modifiedBy">User performing the get operation.</param>
            /// <returns>Success/failure of delete</returns>
            public bool DeleteOrder(String orderNumber, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "DeletePatientOrder";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
                DeleteData(cmd);
                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Gets a list of labs that can handle a specific frame for a given site code.
            /// </summary>
            /// <param name="frameCode">Frame code to determine special lab requirements.</param>
            /// <param name="siteCode">Ordering clinic site code.</param>
            /// <returns>List of labs that can handle specified frame.</returns>
            public List<String> GetSpecialFrameLabs(String frameCode, String siteCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetFrameLabRestrictions";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", frameCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteCode));
                var lo = GetObjects(cmd, "SiteCode");
                return lo.Cast<String>().ToList();
            }

            /// <summary>
            /// Gets a list of the next (n) number or order numbers available.  This is used when there is more than one pair set in the original order.
            /// </summary>
            /// <param name="siteCode">Ordering clinic site code.</param>
            /// <param name="numberOfNumbers">Number or orders to generate new numbers for.</param>
            /// <returns>Order numbers to be used with linked orders.</returns>
            public List<String> GetNextOrderNumbers(string siteCode, int numberOfNumbers)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetNextOrderNumberBySiteCode";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", siteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RecCnt", numberOfNumbers));
                var tmp = GetObjects(cmd);
                var l = new List<String>(numberOfNumbers);
                tmp.Cast<String>().ToList().ForEach(x => l.AddRange(x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
                return l;
            }

            /// <summary>
            /// Gets a list of all linked order numbers that are linked together.
            /// </summary>
            /// <param name="linkedId">Order number of an order linked to other orders.</param>
            /// <returns>List of order numbers that are linked together.</returns>
            public List<String> GetLinkedOrders(String linkedId)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetLinkedOrders";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@LinkedID", linkedId));
                var l = GetObjects(cmd);
                return l.Cast<String>().ToList();
            }

            /// <summary>
            /// Unlinks all orders that are linked together.
            /// </summary>
            /// <param name="orderNumbers">Order numbers that are linked together.</param>
            public void UnlinkOrders(List<String> orderNumbers)
            {
                var o = String.Join(",", orderNumbers);
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "UnlinkOrders";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", o));
                InsertData(cmd);
            }

            public string GetLabForOrder(Order order)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetPatientOrderLab";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", order.ClinicSiteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PrescriptionID", order.PrescriptionId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", order.Frame));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LensMaterial", order.Material));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LensType", order.LensType));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Tint", order.Tint));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", order.Demographic));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FES", order.Eye));
                cmd.Parameters.Add(this.DAL.GetParamenter("@FBS", order.Bridge));
                SqlParameter labsitecode = new SqlParameter
                {
                    ParameterName = "@LabSiteCode",
                    Direction = ParameterDirection.Output,
                    DbType = DbType.String,
                    Size = 8,
                    Value = DBNull.Value,
                };
                cmd.Parameters.Add(labsitecode);
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", 0, ParameterDirection.Output));
                GetObject(cmd);
                var p = cmd.Parameters["@LabSiteCode"] as IDataParameter;
                return p.Value.ToString();
            }



            protected override Order FillRecord(IDataReader dr)
            {
                var o = new Order();

                o.Bridge = dr["FrameBridgeSize"].ToString();
                o.CallPatient = true;
                o.Cases = 1;
                o.ClinicSiteCode = dr["ClinicSiteCode"].ToString();
                o.Color = dr["FrameColor"].ToString();
                o.Comment1 = dr["UserComment1"].ToString();
                o.Comment2 = dr["UserComment2"].ToString();
                o.DateLastModified = Convert.ToDateTime(dr["DateLastModified"]);
                o.Demographic = dr["Demographic"].ToString();
                o.DispenseComments = dr["DispenseComments"].ToString();
                o.Eye = dr["FrameEyeSize"].ToString();
                o.Frame = dr["FrameCode"].ToString();
                o.IsActive = Convert.ToBoolean(dr["IsActive"]);
                o.IsComplete = Convert.ToBoolean(dr["IsComplete"]);
                o.IsGEyes = Convert.ToBoolean(dr["IsGEyes"]);
                o.IsMultivision = Convert.ToBoolean(dr["IsMultivision"]);
                o.IsEmailPatient = Convert.ToBoolean(dr["EmailPatient"]);
                o.LabSiteCode = dr["LabSiteCode"].ToString();
                o.LensType = dr["LensType"].ToString();
                o.LinkedId = dr["LinkedID"].ToString();
                o.LocationCode = dr["LocationCode"].ToString();
                o.Material = dr["LensMaterial"].ToString();
                o.ModifiedBy = dr["ModifiedBy"].ToString();
                o.CurrentStatus = dr["StatusComment"].ToString();
                o.OdSegHeight = dr["ODSegHeight"].ToString();
                o.OrderNumber = dr["OrderNumber"].ToString();
                o.OsSegHeight = dr["OSSegHeight"].ToString();
                o.Pairs = Convert.ToInt32(dr["Pairs"]);
                o.PatientId = Convert.ToInt32(dr["IndividualID_Patient"]);
                o.PrescriptionId = Convert.ToInt32(dr["PrescriptionID"]);
                if (o.Demographic.Length.Equals(8))
                    o.Priority = o.Demographic.ToOrderPriorityKey();
                else
                    o.Priority = "N";
                o.OrderDisbursement = SharedOperations.ExtractOrderDisbursement(dr.ToBoolean("ShipToPatient"), dr.ToBoolean("ClinicShipToPatient"));
                o.TechnicianId = Convert.ToInt32(dr["IndividualID_Tech"]);
                o.TechnicianName = dr["TechName"].ToString();
                o.Temple = dr["FrameTempleType"].ToString();
                o.Tint = dr["Tint"].ToString();
                o.Coatings = dr["Coatings"].ToString();
                //o.GroupName = dr["GroupName"].ToString();
                if (!String.IsNullOrEmpty(o.Comment1))
                {
                    int hasFOC = o.Comment1.IndexOf("FOC:");
                    int hasCOATING = o.Comment1.IndexOf("COATING");
                    if (hasFOC != -1 && hasCOATING == -1)
                    {
                        try
                        {
                            o.FocJustification = o.Comment1.Substring(o.Comment1.IndexOf("FOC:") + 4);
                            o.Comment1 = o.Comment1.Remove(o.Comment1.IndexOf("FOC:"));
                        }
                        catch { }
                    }
                    if (hasFOC != -1 && hasCOATING != -1) //has both FOC justification and COATING justification
                    {
                        try
                        {
                            o.FocJustification = o.Comment1.Substring(o.Comment1.IndexOf("FOC:") + 4, ( (o.Comment1.IndexOf("COATING") - 1 ) - (o.Comment1.IndexOf("FOC:") + 4 ) + 1) );
                            o.CoatingJustification = o.Comment1.Substring(o.Comment1.IndexOf("COATING") + 7);
                            o.Comment1 = o.Comment1.Remove(o.Comment1.IndexOf("FOC:"));
                        }
                        catch { }
                    }
                    if (hasFOC == -1 && hasCOATING != -1)
                    {
                        try
                        {
                            o.CoatingJustification = o.Comment1.Substring(o.Comment1.IndexOf("COATING") + 7);
                            o.Comment1 = o.Comment1.Remove(o.Comment1.IndexOf("COATING"));
                        }
                        catch { }
                    }

                }
                if (!String.IsNullOrEmpty(o.Comment2))
                    if (o.Comment2.IndexOf("MAT:") != -1)
                    {
                        try
                        {
                            o.MaterialJustification = o.Comment2.Substring(o.Comment2.IndexOf("MAT:") + 4);
                            o.Comment2 = o.Comment2.Remove(o.Comment2.IndexOf("MAT:"));
                        }
                        catch { }
                    }

                return o;
            }
            #endregion ORDER
        }

        /// <summary>
        /// A custom repository class to handle Rx operations.
        /// </summary>
        public class PrescriptionRepository : RepositoryBase<Prescription>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public PrescriptionRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            #region PRESCRIPTION

            /// <summary>
            /// Gets all Rxs for a patient.
            /// </summary>
            /// <param name="patientId">Patient ID to get Rxs for.</param>
            /// <param name="modifiedBy">User performing the get operation.</param>
            /// <returns>List of prescriptions for a patient.</returns>
            public List<Prescription> GetAllPrescriptions(Int32 patientId, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPrescriptionsByIndividualID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", patientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                var lp = GetRecords(cmd).ToList();
                return lp;
            }

            /// <summary>
            /// Gets a single Rx by ID.
            /// </summary>
            /// <param name="prescriptionId">Rx ID to get Rx for.</param>
            /// <param name="modifiedBy">User performing the get operation.</param>
            /// <returns>Prescription entity matching the RxId.</returns>
            public Prescription GetPrescriptionById(Int32 prescriptionId, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPrescriptionByID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ID", prescriptionId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                var p = GetRecord(cmd);
                return p;
            }

            /// <summary>
            /// Adds a new Rx to the database.
            /// </summary>
            /// <param name="prescription">Rx to add.</param>
            /// <param name="modifiedBy">User performing the insert operation.</param>
            /// <param name="prescriptionId">Output - RxId for newly added Rx.</param>
            /// <returns>Success/failure of insert.</returns>
            public Int32 InsertPrescription(Prescription prescription, String modifiedBy, out Int32 prescriptionId)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "InsertPrescription";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@PrescriptionDate", prescription.PrescriptionDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RxName", prescription.PrescriptionName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ExamID", prescription.ExamId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Patient", prescription.PatientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Doctor", prescription.ProviderId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODSphere", prescription.OdSphere));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSSphere", prescription.OsSphere));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODCylinder", prescription.OdCylinder));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSCylinder", prescription.OsCylinder));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODAxis", prescription.OdAxis));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSAxis", prescription.OsAxis));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODSphere", prescription.OdSphereCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSSphere", prescription.OsSphereCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODCylinder", prescription.OdCylinderCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSCylinder", prescription.OsCylinderCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODAxis", prescription.OdAxisCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSAxis", prescription.OsAxisCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODHPrism", prescription.OdHPrism));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSHPrism", prescription.OsHPrism));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODVPrism", prescription.OdVPrism));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSVPrism", prescription.OsVPrism));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODHBase", prescription.OdHBase));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSHBase", prescription.OsHBase));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODVBase", prescription.OdVBase));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSVBase", prescription.OsVBase));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODAdd", prescription.OdAdd));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSAdd", prescription.OsAdd));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsMonoCalculation", prescription.IsMonoCalculation));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PDDistant", prescription.PdDistTotal));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PDNear", prescription.PdNearTotal));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODPDDistant", prescription.OdPdDist));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODPDNear", prescription.OdPdNear));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSPDDistant", prescription.OsPdDist));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSPDNear", prescription.OsPdNear));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", 0, ParameterDirection.Output));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RxID", 0, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@RxID"] as IDataParameter;
                prescriptionId = p.Value.ToInt32();
                p = null;
                p = cmd.Parameters["@Success"] as IDataParameter;
                return prescriptionId;
                // return p.Value.ToInt32().Equals(1);
            }


            /// <summary>
            /// Inserts an uploaded prescription image to a saved prescription.
            /// <param name="IndividualID">Patient ID the Rx belongs to.</param> 
            /// <param name="RxID">Prescription ID to associated the image with.</param>
            /// <param name="DocName">Document name.</param>
            /// <param name="DocType">Document type.</param>
            /// <param name="RxScan">Binary file data.</param>
            /// <returns>Success/failure of insert.</returns>
            public Int32 InsertScannedPrescription(Int32 IndividualID, Int32 RxID, String DocName, String DocType, byte[] RxScan, out Int32 scanId)
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
                cmd.Parameters.Add(this.DAL.GetParamenter("@ScannedID", 0, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@ScannedID"] as IDataParameter;
                scanId = p.Value.ToInt32();
                p = null;
                p = cmd.Parameters["@Success"] as IDataParameter;
                return scanId;

                //var p = cmd.Parameters["@Success"] as IDataParameter;
                //return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Deletes a scanned Rx document from the database.
            /// </summary>
            /// <param name="RxScanId">ScannedRx document to delete.</param>
            /// <param name="DelScanDate">Date of the delete operation.</param>
            /// <returns>Success/failure of delete</returns>
            public bool DeleteScannedPrescription(Int32 RxScanId)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "DeleteScannedRx";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@ID", RxScanId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DelScanDate", DateTime.Now));

                return DeleteData(cmd).Equals(1);
            }


            /// <summary>
            /// Updates an existing Rx in the database.
            /// </summary>
            /// <param name="prescription">Rx to update.</param>
            /// <param name="modifiedBy">User performing the update operation.</param>
            /// <returns>Success/failure of update.</returns>
            /// <returns>Success/failure of update.</returns>
            public bool UpdatePrescription(Prescription prescription, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "UpdatePrescription";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@PrescriptionDate", prescription.PrescriptionDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RxName", prescription.PrescriptionName));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ID", prescription.PrescriptionId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ScannedRxID", prescription.PrescriptionScanId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ExamID", prescription.ExamId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Patient", prescription.PatientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Doctor", prescription.ProviderId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODSphere", prescription.OdSphereCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSSphere", prescription.OsSphereCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODCylinder", prescription.OdCylinderCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSCylinder", prescription.OsCylinderCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODAxis", prescription.OdAxisCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSAxis", prescription.OsAxisCalc));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODHPrism", prescription.OdHPrism));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSHPrism", prescription.OsHPrism));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODVPrism", prescription.OdVPrism));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSVPrism", prescription.OsVPrism));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODHBase", prescription.OdHBase));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSHBase", prescription.OsHBase));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODVBase", prescription.OdVBase));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSVBase", prescription.OsVBase));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODAdd", prescription.OdAdd));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSAdd", prescription.OsAdd));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODSphere", prescription.OdSphere));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSSphere", prescription.OsSphere));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODCylinder", prescription.OdCylinder));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSCylinder", prescription.OsCylinder));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredODAxis", prescription.OdAxis));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EnteredOSAxis", prescription.OsAxis));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", prescription.IsActive));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsMonoCalculation", prescription.IsMonoCalculation));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PDDistant", prescription.PdDistTotal));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PDNear", prescription.PdNearTotal));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODPDDistant", prescription.OdPdDist));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODPDNear", prescription.OdPdNear));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSPDDistant", prescription.OsPdDist));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSPDNear", prescription.OsPdNear));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                UpdateData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;

                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Deletes a Rx from the database.
            /// </summary>
            /// <param name="prescriptionId">Rx to delete.</param>
            /// <param name="modifiedBy">User performing the delete operation.</param>
            /// <returns>Success/failure of delete</returns>
            public bool DeletePrescription(Int32 prescriptionId, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "DeletePrescription";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@ID", prescriptionId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));

                return DeleteData(cmd).Equals(1);
            }

            protected override Prescription FillRecord(IDataReader dr)
            {
                var p = new Prescription();

                p.DateLastModified = dr.ToDateTime("DateLastModified");
                p.ExamId = dr.ToInt32("ExamID");
                p.PrescriptionScanId = dr.ToInt32("RxScanID");
                p.IsActive = dr.ToBoolean("IsActive");
                p.IsMonoCalculation = dr.ToBoolean("IsMonoCalculation");
                p.IsUsed = dr.ToBoolean("IsUsed");
                p.OdAdd = dr.ToDecimal("ODAdd");

                p.OdAxis = dr.ToInt32("EnteredODAxis");
                p.OdCylinder = dr.ToDecimal("EnteredODCylinder");
                p.OdSphere = dr.ToDecimal("EnteredODSphere");
                p.OdAxisCalc = dr["ODAxis"].ToString();
                p.OdCylinderCalc = dr["ODCylinder"].ToString();
                p.OdSphereCalc = dr["ODSphere"].ToString();

                p.OdHBase = dr["ODHBase"].ToString();
                p.OdHPrism = dr.ToDecimal("ODHPrism");
                p.OdPdDist = dr.ToDecimal("ODPDDistant");
                p.OdPdNear = dr.ToDecimal("ODPDNEar");
                p.OdVBase = dr["ODVBase"].ToString();
                p.OdVPrism = dr.ToDecimal("ODVPrism");
                p.OsAdd = dr.ToDecimal("OSAdd");

                p.OsAxis = dr.ToInt32("EnteredOSAxis");
                p.OsCylinder = dr.ToDecimal("EnteredOSCylinder");
                p.OsSphere = dr.ToDecimal("EnteredOSSphere");
                p.OsAxisCalc = dr["OSAxis"].ToString();
                p.OsCylinderCalc = dr["OSCylinder"].ToString();
                p.OsSphereCalc = dr["OSSphere"].ToString();

                p.OsHBase = dr["OSHBase"].ToString();
                p.OsHPrism = dr.ToDecimal("OSHPrism");
                p.OsPdDist = dr.ToDecimal("OSPDDIstant");
                p.OsPdNear = dr.ToDecimal("OSPDNEar");
                p.OsVBase = dr["OSVBase"].ToString();
                p.OsVPrism = dr.ToDecimal("OSVPrism");
                p.PatientId = dr.ToInt32("IndividualID_Patient");
                p.PdDistTotal = dr.ToDecimal("PDDistant");
                p.PdNearTotal = dr.ToDecimal("PDNear");
                p.PrescriptionId = dr.ToInt32("ID");
                p.PrescriptionName = String.IsNullOrEmpty(dr["RxName"].ToString()) || dr["RxName"].ToString().Equals("X") ? String.Empty : dr["RxName"].ToString();
                p.PrescriptionDate = dr.ToDateTime("PrescriptionDate");
                p.ProviderId = dr.ToInt32("IndividualID_Doctor");
                p.OrderedFrameHistory = dr["OrderedFrameHistory"].ToString();

                return p;
            }

            #endregion PRESCRIPTION
        }

        /// <summary>
        /// A custom repository class to handle exam operations.
        /// </summary>
        public class ExamRepository : RepositoryBase<Exam>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public ExamRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            #region EXAM

            /// <summary>
            /// Adds a new exam to the database.
            /// </summary>
            /// <param name="exam">Exam to add.</param>
            /// <param name="modifiedBy">User performing the insert operation.</param>
            /// <returns>Success/failure of insert.</returns>
            public bool InsertExam(Exam exam, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "InsertExam";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Patient", exam.PatientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Examiner", exam.DoctorId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODCorrectedAcuity", exam.OdCorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODUncorrectedAcuity", exam.OdUncorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSCorrectedAcuity", exam.OsCorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSUncorrectedAcuity", exam.OsUncorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODOSCorrectedAcuity", exam.OdOsCorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODOSUncorrectedAcuity", exam.OdOsUncorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Comment", exam.ExamComments));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ExamDate", exam.ExamDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", true));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;

                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Updates an existing exam in the database.
            /// </summary>
            /// <param name="exam">Exam to update.</param>
            /// <param name="modifiedBy">User performing the insert operation.</param>
            /// <returns>Success/failure of insert.</returns>
            public bool UpdateExam(Exam exam, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "UpdateExamByExamID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@ID", exam.ExamId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Patient", exam.PatientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Examiner", exam.DoctorId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODCorrectedAcuity", exam.OdCorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODUncorrectedAcuity", exam.OdUncorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSCorrectedAcuity", exam.OsCorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OSUncorrectedAcuity", exam.OsUncorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODOSCorrectedAcuity", exam.OdOsCorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ODOSUncorrectedAcuity", exam.OdOsUncorrected));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Comment", exam.ExamComments));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ExamDate", exam.ExamDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", true));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                UpdateData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;

                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Gets a list of all exams for a patient.
            /// </summary>
            /// <param name="patientId">Patient ID to get exams for.</param>
            /// <param name="modifiedBy">User performing the insert operation.</param>
            /// <returns>Exam list for a patient.</returns>
            public List<Exam> GetAllExams(Int32 patientId, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetExamsByIndividualID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", patientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                var el = GetRecords(cmd).ToList();
                return el;
            }

            protected override Exam FillRecord(IDataReader dr)
            {
                var e = new Exam();
                e.DateLastModified = Convert.ToDateTime(dr["DateLastModified"]);
                e.DoctorId = Convert.ToInt32(dr["IndividualID_Examiner"]);
                e.ExamComments = dr["Comment"].ToString();
                e.ExamDate = Convert.ToDateTime(dr["ExamDate"]);
                e.ExamId = Convert.ToInt32(dr["ID"]);
                e.OdCorrected = dr["ODCorrectedAcuity"].ToString();
                e.OdOsCorrected = dr["ODOSCorrectedAcuity"].ToString();
                e.OdOsUncorrected = dr["ODOSUncorrectedAcuity"].ToString();
                e.OdUncorrected = dr["ODUncorrectedAcuity"].ToString();
                e.OsCorrected = dr["OSCorrectedAcuity"].ToString();
                e.OsUncorrected = dr["OSUncorrectedAcuity"].ToString();
                e.PatientId = Convert.ToInt32(dr["IndividualID_Patient"]);

                return e;
            }

            #endregion EXAM
        }

        /// <summary>
        /// A custom repository class to handle frame related operations.
        /// </summary>
        public class FrameRepository : RepositoryBase<FrameEntity>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public FrameRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Gets a list of frames for an order based on demographic, site code, and Rx.
            /// </summary>
            /// <param name="demographic">Information about a patient used to identify allowable frames.</param>
            /// <param name="siteCode">Site code to search with.</param>
            /// <param name="RxID">Rx ID to identify frame restrictions.</param>
            /// <returns>FrameEntity list of allowable frames.</returns>
           public List<FrameEntity> GetFrameData(String demographic, String siteCode, int RxID)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetFramesByEligibility";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", demographic));
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RxID", RxID));
                var lf = GetRecords(cmd);
                return lf.ToList();
            }

            protected override FrameEntity FillRecord(IDataReader dr)
            {
                FrameEntity fe = new FrameEntity();
                fe.FrameCode = dr["FrameCode"].ToString();
                fe.DateLastModified = dr.ToDateTime("DateLastModified");
                fe.FrameDescription = dr["FrameDescription"].ToString();
                fe.FrameNotes = dr["FrameNotes"].ToString();
                fe.IsActive = dr.ToBoolean("IsActive");
                fe.IsInsert = dr.ToBoolean("IsInsert");
                fe.ModifiedBy = dr["ModifiedBy"].ToString();
                fe.MaxPair = dr.ToInt32("MaxPair");
                fe.IsFoc = dr.ToBoolean("IsFOC");
                fe.FrameType = dr["FrameType"].ToString();

                return fe;
            }
        }


        /// <summary>
        /// A custom repository class to handle order email operations.
        /// </summary>
        public class OrderEmailRepository : RepositoryBase<OrderEmail>
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public OrderEmailRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            #region ORDER EMAIL

            /// <summary>
            /// Gets a single orderemail by order number.
            /// </summary>
            /// <param name="orderNumber">Order number to search with.</param>
            /// <param name="modifiedBy">User performing the get operation.</param>
            /// <returns>Single orderemail that matches the order number</returns>
            public OrderEmail GetOrderEmailByOrderNumber(String orderNumber, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetOrderEmail";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNbr", orderNumber));
                //cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));

                var o = GetRecord(cmd);

                return o;
            }


            /// <summary>
            /// Gets a Email Message by order number.
            /// </summary>
            /// <param name="orderNumber">Order number to search with.</param>
            /// <returns>Email Message</returns>
            public string GetOrderEmailMessage(String orderNumber)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetOrderEmailMsg";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));

                /*For output parameters, the size of the parameter defines the size of the buffer holding the output parameter.  The output parameter
                 * can be truncated to a size specified with Size. */
                IDbDataParameter emailMessageParameter = cmd.CreateParameter();
                emailMessageParameter.ParameterName = "@EmailMsg";
                emailMessageParameter.Direction = System.Data.ParameterDirection.Output;
                emailMessageParameter.DbType = System.Data.DbType.String;
                emailMessageParameter.Size = 2100;
                cmd.Parameters.Add(emailMessageParameter);

                //cmd.Parameters.Add(this.DAL.GetParamenter("@EmailMsg", String.Empty, ParameterDirection.Output));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", 0, ParameterDirection.Output));
                               
                var d  = GetObject(cmd);

                var em = cmd.Parameters["@EmailMsg"] as IDataParameter;
                var su = cmd.Parameters["@Success"] as IDataParameter;

                if (su.Value.ToInt32().Equals(1))
                    return em.Value.ToString();
                else
                    return null;
            }



            /// <summary>
            /// Adds a new order email to the database.
            /// </summary>
            /// <param name="exam">Order Email to add.</param>
            /// <returns>Success/failure of insert.</returns>
            public bool InsertOrderEmail(OrderEmail orderemail, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "InsertOrderEmail";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderemail.OrderNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailAddress", orderemail.EmailAddress));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailMsg", orderemail.EmailMsg));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailDateTime", orderemail.EmailDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@PatientID", orderemail.PatientId));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ChangeEMail", orderemail.ChangeEmail));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailSent", orderemail.EmailSent));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));
                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;

                return p.Value.ToInt32().Equals(1);
            }

            ///// <summary>
            ///// Updates an existing order email in the database.
            ///// </summary>
            ///// <param name="exam">OrderEmail to update.</param>
            ///// <param name="modifiedBy">User performing the insert operation.</param>
            ///// <returns>Success/failure of insert.</returns>
            public bool UpdateOrderEmail(OrderEmail orderemail, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "UpdatePatientOrderEmail";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNbr", orderemail.OrderNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailAddress", orderemail.EmailAddress));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailMsg", orderemail.EmailMsg));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailDate", orderemail.EmailDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@EmailSent", orderemail.EmailSent));
                //cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", String.Empty, ParameterDirection.Output));

                UpdateData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;

                return p.Value.ToInt32().Equals(1);
            }

            /// <summary>
            /// Gets all order notifications for a patient.
            /// </summary>
            /// <param name="patientId">Patient ID to search with.</param>
            /// <returns>List of order notifications for a patient.</returns>
            public List<OrderEmail> GetAllOrderEmails(Int32 patientId)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetPatientEmails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@PatientID", patientId));

                var lo = GetRecords(cmd).ToList();

                return lo;
            }

            protected override OrderEmail FillRecord(IDataReader dr)
            {
                var oe = new OrderEmail();
                oe.OrderNumber = dr["OrderNumber"].ToString();
                oe.EmailAddress = dr["EmailAddress"].ToString();
                oe.EmailMsg = dr["EmailMsg"].ToString();
                oe.EmailDate = dr["EmailDate"] == DBNull.Value ? (DateTime?) null : Convert.ToDateTime(dr["EmailDate"]);
                oe.PatientId = Convert.ToInt32(dr["PatientID"]);
                oe.ChangeEmail = dr.ToBoolean("ChangeEmail");
                oe.EmailSent = dr.ToBoolean("EmailSent");

                return oe;
            }

            #endregion ORDER EMAIL
        }

    }
}