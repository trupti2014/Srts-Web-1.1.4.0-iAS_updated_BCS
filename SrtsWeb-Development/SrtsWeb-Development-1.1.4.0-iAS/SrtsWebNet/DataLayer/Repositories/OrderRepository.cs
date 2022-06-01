using DataBaseAccessLayer;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.DataLayer.Repositories
{
    public sealed class OrderRepository : RepositoryBase<OrderEntity>
    {
        public OrderRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        {
        }

        protected override OrderEntity FillRecord(IDataReader dr)
        {
            OrderEntity oe = new OrderEntity();
            var c = dr.GetColumnNameList();
            oe.ClinicSiteCode = dr.AsString("ClinicSiteCode", c);
            oe.DateLastModified = dr.ToDateTime("DateLastModified", c);
            oe.PrescriptionID = dr.ToInt32("PrescriptionID", c);
            oe.IndividualID_Patient = dr.ToInt32("IndividualID_Patient", c);
            oe.IndividualID_Tech = dr.ToInt32("IndividualID_Tech", c);
            oe.PatientPhoneID = dr.ToInt32("PatientPhoneID", c);
            oe.LabSiteCode = dr.AsString("LabSiteCode", c);
            oe.LensMaterial = dr.AsString("LensMaterial", c);
            oe.LensType = dr.AsString("LensType", c);
            oe.LocationCode = dr.AsString("LocationCode", c);
            oe.ModifiedBy = dr.AsString("ModifiedBy", c);
            oe.NumberOfCases = dr.ToInt32("NumberOfCases", c);
            oe.Pairs = dr.ToInt32("Pairs", c);
            oe.OrderNumber = dr.AsString("OrderNumber", c);
            oe.ShipAddress1 = dr.AsString("ShipAddress1", c);
            oe.ShipAddress2 = dr.AsString("ShipAddress2", c);
            oe.ShipAddress3 = dr.AsString("ShipAddress3", c);
            oe.ShipCity = dr.AsString("ShipCity", c);
            oe.ShipState = dr.AsString("ShipState", c);
            oe.ShipZipCode = dr.AsString("ShipZipCode", c);
            oe.ShipAddressType = dr.AsString("ShipAddressType", c);
            oe.ShipCountry = dr.AsString("ShipCountry", c);

            //oe.ShipToPatient = dr.ToBoolean("ShipToPatient", c);
            oe.OrderDisbursement = SharedOperations.ExtractOrderDisbursement(dr.ToBoolean("ShipToPatient", c), dr.ToBoolean("ClinicShipToPatient", c));

            oe.Tint = dr.AsString("Tint", c);
            oe.UserComment1 = dr.AsString("UserComment1", c);
            oe.UserComment2 = dr.AsString("UserComment2", c);
            oe.FrameBridgeSize = dr.AsString("FrameBridgeSize", c);
            oe.FrameCode = dr.AsString("FrameCode", c);
            oe.FrameColor = dr.AsString("FrameColor", c);
            oe.FrameEyeSize = dr.AsString("FrameEyeSize", c);
            oe.FrameTempleType = dr.AsString("FrameTempleType", c);
            oe.Demographic = dr.AsString("Demographic", c);
            oe.IsGEyes = dr.ToBoolean("IsGEyes", c);
            oe.IsActive = dr.ToBoolean("IsActive", c);
            oe.IsMultivision = dr.ToBoolean("IsMultivision", c);
            oe.IsEmailPatient = dr.ToBoolean("EmailPatient", c);
            oe.ODSegHeight = dr.AsString("ODSegHeight", c);
            oe.OSSegHeight = dr.AsString("OSSegHeight", c);
            oe.VerifiedBy = dr.ToInt32("VerifiedBy", c);
            oe.CorrespondenceEmail = dr.AsString("PatientEmail", c);
            oe.MedProsDispense = dr.ToBoolean("MedProsDispense", c);
            oe.PimrsDispense = dr.ToBoolean("PimrsDispense", c);
            oe.FocDate = dr.ToNullableDateTime("FOCDate", c);
            oe.LinkedID = dr.AsString("LinkedID", c);
            
            try
            {
                oe.OnholdForConfirmation = dr.ToBoolean("OnholdForConfirmation", c);
            }
            catch (System.IndexOutOfRangeException e)
            {
                oe.OnholdForConfirmation = dr.ToBoolean("OnHoldForComfirmation", c);
            }
            return oe;
        }

        /// <summary>
        /// Gets a list of order numbers for orders that do not have barcodes.
        /// </summary>
        /// <returns>Order number list for orders without barcodes.</returns>
        public List<String> GetPatientOrdersNoBarCodes()
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetPatientOrdersNoBarCodes";
            return GetObjects(cmd).Cast<String>().ToList();
        }

        /// <summary>
        /// Gets all data for Wounded Warrior report based on a date range.
        /// </summary>
        /// <param name="fromDate">Search start date.</param>
        /// <param name="toDate">Search end date.</param>
        /// <returns>Data table for Wounded Warrior report.</returns>
        public DataTable GetWoundedWarriorReportData(DateTime fromDate, DateTime toDate)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetWoundedWarriorReportData";
            cmd.Parameters.Add(this.DAL.GetParamenter("@FromDate", fromDate));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ToDate", toDate));
            return this.DAL.GetData(cmd);
        }

        /// <summary>
        /// Gets a list that contains a single order that is missing some GEyes data by order number.
        /// </summary>
        /// <param name="orderNumber">Order number to search with.</param>
        /// <param name="modifiedBy">User performing the search operation.</param>
        /// <returns>OrderEntity list that contains a single order.</returns>
        public List<OrderEntity> GetOrderByOrderNumberNonGEyes(string orderNumber, string modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetPatientOrderNONGEyesByOrderNumber";
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Gets all of the summary data for a clinic that shows up on the dashboard after login.
        /// </summary>
        /// <param name="_siteCode">Site code to search in.</param>
        /// <returns>Data set of all clinic summary data.</returns>
        public DataSet GetClinicSummary(String _siteCode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetPatientOrdersSummaryClinicBySiteCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", _siteCode));
            return this.DAL.GetDataSet(cmd);
        }

        /// <summary>
        /// Gets all of the summary data for a lab that shows up on the dashboard after login.
        /// </summary>
        /// <param name="_siteCode"></param>
        /// <returns>Data set of all lab summary data.</returns>
        public DataSet GetLabSummary(String _siteCode)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetPatientOrdersSummaryLabBySiteCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", _siteCode));
            return this.DAL.GetDataSet(cmd);
        }

        /// <summary>
        /// Gets a data table containing all the data to populate the DD771 report by a comma delimited order string or by a lab site code.
        /// </summary>
        /// <param name="_siteCode">Site code to search in.</param>
        /// <param name="_orderNbr">Order number to search with.</param>
        /// <param name="modifiedBy"User performing the search operation.></param>
        /// <returns>Data table containing order data to fill the DD771.</returns>
        public DataTable Get711DataByLabCode(String _siteCode, String _orderNbr, String modifiedBy)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Get711DataByLabCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", _siteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNbr", _orderNbr));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
            return this.DAL.GetData(cmd);
        }

        #region GEYES
        /// <summary>
        /// A custom repostiroy class to handle GEyes order operations with the OrderEntity entity class.
        /// </summary>
        public sealed class GEyesOrderRepository : RepositoryBase<OrderEntity>, IGEyesOrderRepository
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public GEyesOrderRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Gets an order for the GEyes app by an order number.
            /// </summary>
            /// <param name="orderNumber">Order number to search with.</param>
            /// <param name="modifiedBy">User performing the search operation.</param>
            /// <returns>Single OrderEntity for GEyes.</returns>
            public OrderEntity GetOrderByOrderNumber(string orderNumber, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPatientOrderByOrderNumber";
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                return GetRecord(cmd);
            }

            protected override OrderEntity FillRecord(IDataReader dr)
            {
                OrderEntity oe = new OrderEntity();
                oe.ClinicSiteCode = dr.AsString("ClinicSiteCode");
                oe.DateLastModified = dr.ToDateTime("DateLastModified");
                oe.PrescriptionID = dr.ToInt32("PrescriptionID");
                oe.IndividualID_Patient = dr.ToInt32("IndividualID_Patient");
                oe.IndividualID_Tech = dr.ToInt32("IndividualID_Tech");
                oe.PatientPhoneID = dr.ToInt32("PatientPhoneID");
                oe.LabSiteCode = dr.AsString("LabSiteCode");
                oe.LensMaterial = dr.AsString("LensMaterial");
                oe.LensType = dr.AsString("LensType");
                oe.LensTypeLong = dr.AsString("LensTypeLong");
                oe.LensTint = dr.AsString("LensTint");
                oe.LensCoating = dr.AsString("LensCoating");
                oe.LocationCode = dr.AsString("LocationCode");
                oe.ModifiedBy = dr.AsString("ModifiedBy");
                //oe.NumberOfCases = dr.ToInt32("NumberOfCases");
                oe.NumberOfCases = 1;
                //oe.Pairs = dr.ToInt32("Pairs");
                oe.Pairs = 1;
                oe.OrderNumber = dr.AsString("OrderNumber");
                oe.ShipAddress1 = dr.AsString("ShipAddress1");
                oe.ShipAddress2 = dr.AsString("ShipAddress2");
                oe.ShipAddress3 = dr.AsString("ShipAddress3");
                oe.ShipCity = dr.AsString("ShipCity");
                oe.ShipState = dr.AsString("ShipState");
                oe.ShipZipCode = dr.AsString("ShipZipCode");
                oe.ShipAddressType = dr.AsString("ShipAddressType");
                oe.ShipCountry = dr.AsString("ShipCountry");
                //oe.ShipToPatient = dr.ToBoolean("ShipToPatient");
                oe.OrderDisbursement = "L2P";
                oe.Tint = dr.AsString("Tint");
                oe.UserComment1 = dr.AsString("UserComment1");
                oe.UserComment2 = dr.AsString("UserComment2");
                oe.FrameBridgeSize = dr.AsString("FrameBridgeSize");
                oe.FrameCode = dr.AsString("FrameCode");
                oe.FrameColor = dr.AsString("FrameColor");
                oe.FrameFamily = dr.AsString("FrameFamily");
                oe.FrameCategory = dr.AsString("Category");
                oe.FrameEyeSize = dr.AsString("FrameEyeSize");
                oe.FrameTempleType = dr.AsString("FrameTempleType");
                oe.FrameDescription = dr.AsString("FrameDescription");

                oe.Demographic = dr.AsString("Demographic");
                oe.IsGEyes = dr.ToBoolean("IsGEyes");
                oe.IsActive = dr.ToBoolean("IsActive");
                oe.IsMultivision = dr.ToBoolean("IsMultivision");
                oe.ODSegHeight = dr.AsString("ODSegHeight");
                oe.OSSegHeight = dr.AsString("OSSegHeight");
                oe.VerifiedBy = dr.ToInt32("VerifiedBy");
                oe.CorrespondenceEmail = dr.AsString("PatientEmail");
                oe.OnholdForConfirmation = dr.ToBoolean("OnholdForConfirmation");
                oe.MedProsDispense = dr.ToBoolean("MedProsDispense");
                oe.PimrsDispense = dr.ToBoolean("PimrsDispense");
                return oe;
            }
        }

        /// <summary>
        /// Add a new order to the database.
        /// </summary>
        /// <param name="order">Order to add.</param>
        /// <param name="_state"></param>
        /// <returns>Success/failure of insert.</returns>
        public Boolean InsertOrder(OrderEntity order, bool _state)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertPatientOrder";
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", order.OrderNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Patient", order.IndividualID_Patient));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID_Tech", order.IndividualID_Tech));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PatientPhoneID", order.PatientPhoneID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Demographic", order.Demographic));
            cmd.Parameters.Add(this.DAL.GetParamenter("@LensType", string.IsNullOrEmpty(order.LensType) ? string.Empty : order.LensType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@LensMaterial", string.IsNullOrEmpty(order.LensMaterial) ? string.Empty : order.LensMaterial));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Tint", string.IsNullOrEmpty(order.Tint) ? string.Empty : order.Tint));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OSSegHeight", order.OSSegHeight));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ODSegHeight", order.ODSegHeight));
            cmd.Parameters.Add(this.DAL.GetParamenter("@NumberOfCases", string.IsNullOrEmpty(order.NumberOfCases.ToString()) ? 0 : order.NumberOfCases));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Pairs", string.IsNullOrEmpty(order.Pairs.ToString()) ? 0 : order.Pairs));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PrescriptionID", order.PrescriptionID));

            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameCode", string.IsNullOrEmpty(order.FrameCode) ? string.Empty : order.FrameCode));

            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameColor", string.IsNullOrEmpty(order.FrameColor) ? string.Empty : order.FrameColor));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameBridgeSize", string.IsNullOrEmpty(order.FrameBridgeSize) ? string.Empty : order.FrameBridgeSize));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameEyeSize", string.IsNullOrEmpty(order.FrameEyeSize) ? string.Empty : order.FrameEyeSize));
            cmd.Parameters.Add(this.DAL.GetParamenter("@FrameTempleType", string.IsNullOrEmpty(order.FrameTempleType) ? string.Empty : order.FrameTempleType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", order.ClinicSiteCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", order.LabSiteCode));

            cmd.Parameters.Add(this.DAL.GetParamenter("@ShipToPatient", order.OrderDisbursement.Equals("L2P")));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicShipToPatient", order.OrderDisbursement.Equals("C2P")));

            cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddress1", order.ShipAddress1));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddress2", order.ShipAddress2));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddress3", order.ShipAddress3));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ShipCity", order.ShipCity));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ShipState", order.ShipState));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ShipZipCode", order.ShipZipCode.ToZipCodeRemoveDash()));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ShipCountry", order.ShipCountry));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ShipAddressType", order.ShipAddressType));
            cmd.Parameters.Add(this.DAL.GetParamenter("@LocationCode", order.LocationCode));
            cmd.Parameters.Add(this.DAL.GetParamenter("@UserComment1", order.UserComment1));
            cmd.Parameters.Add(this.DAL.GetParamenter("@UserComment2", order.UserComment2));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsGEyes", order.IsGEyes));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsMultivision", order.IsMultivision));
            cmd.Parameters.Add(this.DAL.GetParamenter("@VerifiedBy", order.VerifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PatientEmail", order.CorrespondenceEmail));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OnholdForConfirmation", order.OnholdForConfirmation));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", order.ModifiedBy));
            cmd.Parameters.Add(this.DAL.GetParamenter("@IsComplete", _state));
            //cmd.Parameters.Add(this.DAL.GetParamenter("@FOCDate", order.FocDate));  // This param was removed from the SP on 15 Dec, 2015
            cmd.Parameters.Add(this.DAL.GetParamenter("@LinkedID", order.IsGEyes == true ? string.Empty : order.LinkedID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@ONBarCode", order.ONBarCode));
            //cmd.Parameters.Add(this.DAL.GetParamenter("@GroupName", order.GroupName));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));

            InsertData(cmd);
            var p = cmd.Parameters["@Success"] as IDataParameter;

            

            return p.Value.ToInt32().Equals(1);
        }

        /// <summary>
        /// Updates an order in the database.
        /// </summary>
        /// <param name="order">Order to update.</param>
        /// <returns>Success/failure of update.</returns>
        public Boolean UpdateOrderWithBarcode(OrderEntity order)
        {
            var cmd = this.DAL.GetCommandObject();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdatePatientOrder_BarCode";
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", order.OrderNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@BarCode", order.ONBarCode));
            return GetRecord(cmd) != null;
        }

        /// <summary>
        /// Gets a list of new order numbers.
        /// </summary>
        /// <param name="siteCode">Site code to generate new order numbers for.</param>
        /// <param name="numberOfNumbers">Number of additional order numbers to get.</param>
        /// <returns>List of new order numbers.</returns>
        public List<String> GetNextOrderNumbers(string siteCode, int numberOfNumbers)
        {
            var r = new OrderManagementRepository.OrderRepository();
            return r.GetNextOrderNumbers(siteCode, numberOfNumbers);

            //using (var cmd = new SqlCommand())
            //{
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.CommandText = "GetNextOrderNumberBySiteCode";
            //    cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", siteCode));
            //    cmd.Parameters.Add(this.DAL.GetParamenter("@RecCnt", numberOfNumbers));

            //    var dt = new DataTable();// cmd.ExecuteToDataTable();

            //    var l = new List<String>(numberOfNumbers);

            //    foreach (DataRow r in dt.Rows)
            //    {
            //        l = r[0].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //    }

            //    return l;
            //}
        }

        /// <summary>
        /// A custom repository class to handle order operations for the OrderDisplayEntity class.
        /// </summary>
        public sealed class DisplayRepository : RepositoryBase<OrderDisplayEntity>, IDisplayRepository
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public DisplayRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, "SRTS"))
            { }

            /// <summary>
            /// Gets a list of orders and it's corresponding Rx for a patient.
            /// </summary>
            /// <param name="individualID">Individual ID of patient to search with.</param>
            /// <param name="modifiedBy">User performing the search operation.</param>
            /// <param name="isGeyes">Flag to determine if order is a GEyes order.</param>
            /// <returns>OrderDisplayEntity of orders and it's corresponding Rx.</returns>
            public List<OrderDisplayEntity> GetOrderDisplay(int individualID, string modifiedBy, Boolean isGeyes = false)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPatientOrderWithPrescriptionByIndividualID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", individualID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                //cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", usersSiteCode);
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsGeyes", isGeyes));
                return GetRecords(cmd).ToList();
            }

            protected override OrderDisplayEntity FillRecord(IDataReader dr)
            {
                OrderDisplayEntity oe = new OrderDisplayEntity();
                oe = new OrderDisplayEntity();
                oe.FrameCode = dr.AsString("FrameCode");
                oe.FrameColor = dr.AsString("FrameColor");
                oe.LensType = dr.AsString("LensType");
                oe.ODAdd = dr.ToDouble("ODAdd");
                oe.ODAxis = dr.ToInt32("ODAxis");
                oe.ODCylinder = dr.AsString("ODCylinder");
                oe.ODSphere = dr.AsString("ODSphere");
                oe.OSAdd = dr.ToDouble("OSAdd");
                oe.OSAxis = dr.ToInt32("OSAxis");
                oe.OSCylinder = dr.AsString("OSCylinder");
                oe.OSSphere = dr.AsString("OSSphere");
                oe.OrderNumber = dr.AsString("OrderNumber");
                oe.ClinicSiteCode = dr.AsString("ClinicSiteCode");
                oe.LabSiteCode = dr.AsString("LabSiteCode");
                oe.OrderNumber = dr.AsString("OrderNumber");
                oe.DateCreated = dr.ToDateTime("DateCreated");
                oe.LensTypeLong = dr.AsString("LensTypeLong");

                oe.LensTint = dr.AsString("LensTint");
                oe.LensCoating = dr.AsString("LensCoating");

                oe.FrameDescription = dr.AsString("FrameDescription");

                return oe;
            }
        }

        #endregion GEYES
        #region JSpecs
        /// <summary>
        /// A custom repository class to handle JSpecs order operations with the OrderEntity entity class.
        /// </summary>
        public sealed class JSpecsOrderRepository : RepositoryBase<JSpecsOrderDisplayEntity>, IJSpecsOrderRepository
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            public JSpecsOrderRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }
            
            public List<JSpecsOrderDisplayEntity> GetOrdersByIndividualID(int individualID, string modifiedBy, string clinicSiteCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = "GetPatientOrdersDetailsByIndividualIDJSpecs";
                cmd.CommandText = "GetPatientOrderByIndividualIdNonGEyes";
                cmd.Parameters.Add(this.DAL.GetParamenter("@IndividualID", individualID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicCode", clinicSiteCode));
                return GetRecords(cmd).ToList();
            }

            protected override JSpecsOrderDisplayEntity FillRecord(IDataReader dr)
            {
                JSpecsOrderDisplayEntity oe = new JSpecsOrderDisplayEntity();

                oe.OrderNumber = dr.AsString("OrderNumber");
                oe.DateCreated = dr.ToDateTime("DateLastModified"); // This was used as date created in other stored procedures
                oe.PrescriptionID = dr.ToInt32("PrescriptionID");
                oe.RxName = dr.AsString("RxName");
                oe.PrescriptionDate = dr.ToDateTime("PrescriptionDate");
                oe.Tint = dr.AsString("Tint");
                oe.LensTint = dr.AsString("LensTint");
                oe.LensCoating = dr.AsString("Coatings");
                oe.LensTypeLong = dr.AsString("LensType");
                oe.FrameCode = dr.AsString("FrameCode");
                oe.FrameIsActive = dr.ToBoolean("IsActive");
                oe.FrameCategory = dr.AsString("Category");
                oe.FrameEyeSize = dr.AsString("FrameEyeSize");
                oe.FrameBridgeSize = dr.AsString("FrameBridgeSize");
                oe.FrameTempleType = dr.AsString("FrameTempleType");
                oe.FrameColor = dr.AsString("FrameColor");
                oe.FrameImgPath = dr.AsString("ImgPath");
                oe.FrameImgName = dr.AsString("ImgName");
                oe.FrameImgType = dr.AsString("ContentType");

                oe.FrameUserFriendlyName = oe.FrameCategory.ToUserFriendlyFrameName(oe.FrameCode);
                oe.RxNameUserFriendly = oe.RxName.ToRxUserFriendlyName();

                return oe;
            }

        }

        /// <summary>
        /// A custom repository class to handle JSpecs order operations with the OrderEntity entity class.
        /// </summary>
        public sealed class JSpecsOrderDetailsRepository : RepositoryBase<JSpecsOrderDetailsDisplayEntity>, IJSpecsOrderDetailsRepository
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            public JSpecsOrderDetailsRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Gets an order for the JSpecs app by an order number.
            /// </summary>
            /// <param name="orderNumber">Order number to search with.</param>
            /// <param name="modifiedBy">User performing the search operation.</param>
            /// <returns>Single OrderEntity for JSpecs.</returns>
            public JSpecsOrderDetailsDisplayEntity GetOrderByOrderNumber(string orderNumber, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = "GetPatientOrderDetailsByOrderNumberJSpecs";
                cmd.CommandText = "GetPatientOrderByOrderNumber";
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                Console.WriteLine("Order number: " + orderNumber);
                Console.WriteLine("Modified by: " + modifiedBy);
                Console.WriteLine("Type of Modified by: " + modifiedBy.GetType());
                return GetRecord(cmd);
            }

            /// <summary>
            /// Fill record of order display entity
            /// </summary>
            /// <param name="dr"></param>
            /// <returns></returns>
            protected override JSpecsOrderDetailsDisplayEntity FillRecord(IDataReader dr)
            {
                JSpecsOrderDetailsDisplayEntity oe = new JSpecsOrderDetailsDisplayEntity();

                oe.OrderNumber = dr.AsString("OrderNumber");
                oe.DateLastModified = dr.ToDateTime("DateLastModified");
                oe.Address.Address1 = dr.AsString("ShipAddress1");
                oe.Address.Address2 = dr.AsString("ShipAddress2");
                oe.Address.Address3 = dr.AsString("ShipAddress3");
                oe.Address.ZipCode = dr.AsString("ShipZipCode");
                oe.Address.City = dr.AsString("ShipCity");
                oe.Address.State = dr.AsString("ShipState");
                oe.Address.Country = dr.AsString("ShipCountry");

                oe.PrescriptionID = dr.ToInt32("PrescriptionID");
                oe.PrescriptionDate = dr.ToDateTime("PrescriptionDate");
                oe.RxName = dr.AsString("RxName");
                oe.RxNameUserFriendly = oe.RxName.ToRxUserFriendlyName();
                oe.PDDistant = dr.ToDecimal("PDDistant");
                oe.PDNear = dr.ToDecimal("PDNear");
                oe.ODSphere = dr.AsString("ODSphere");
                oe.ODCylinder = dr.AsString("ODCylinder");
                oe.ODAxis = dr.AsString("ODAxis");
                oe.ODAdd = dr.AsString("ODAdd");
                oe.OSSphere = dr.AsString("OSSphere");
                oe.OSCylinder = dr.AsString("OSCylinder");
                oe.OSAxis = dr.AsString("OSAxis");
                oe.OSAdd = dr.AsString("OSAdd");

                oe.FrameCode = dr.AsString("FrameCode");
                oe.FrameEyeSize = dr.AsString("FrameEyeSize");
                oe.FrameBridgeSize = dr.AsString("FrameBridgeSize");
                oe.FrameTempleType = dr.AsString("FrameTempleType");
                oe.FrameColor = dr.AsString("FrameColor");
                oe.FrameImgPath = dr.AsString("ImgPath");
                oe.FrameImgName = dr.AsString("ImgName");
                oe.FrameImgType = dr.AsString("ContentType");

                // Removed after switching from JSpecs stored procedure to extending an already existing stored procedure.
                //if (dr.NextResult())
                //{
                //    dr.Read();
                //    oe.OrderStatusTypeID = dr.ToInt32("OrderStatusTypeID");
                //    oe.OrderStatus = string.Empty;
                //}

                return oe;
            }

        }
        #endregion JSpecs end
    }
}