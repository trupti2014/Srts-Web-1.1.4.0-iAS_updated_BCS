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
    public sealed class OrderStateRepository
    {
        /// <summary>
        /// A custom repository class to handle order status operations for the OrderStateEntity class.
        /// </summary>
        public sealed class OrderStatusRepository : RepositoryBase<OrderStateEntity>, IOrderStatusRepository
        {
            private List<String> c;

            /// <summary>
            /// Default ctor.
            /// </summary>
            public OrderStatusRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Gets the order status history for an order.
            /// </summary>
            /// <param name="orderNumber">Order number to get status history with.</param>
            /// <returns>OrderStateEntity list of status history.</returns>
            public List<OrderStateEntity> GetOrderStateByOrderNumber(string orderNumber)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetOrderStateHistoryByOrderNumber";
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Adds a new order status and deactives the previous status at the same time.
            /// </summary>
            /// <param name="_orderState"></param>
            /// <returns>Success/failure of insert.</returns>
            public Boolean InsertPatientOrderState(OrderStateEntity _orderState)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertPatientOrderStatus";
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", _orderState.OrderNumber));
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderStatusTypeID", _orderState.OrderStatusTypeID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@IsActive", _orderState.IsActive));
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", _orderState.LabCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@StatusComment", _orderState.StatusComment));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", _orderState.ModifiedBy));
                cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));
                InsertData(cmd);

                var p = cmd.Parameters["@Success"] as IDataParameter;
                return p.Value.ToInt32().Equals(1); ;                
            }

            /// <summary>
            /// Gets a list of the active statuses for multiple orders by a comma delimited order number string..
            /// </summary>
            /// <param name="orderNumber"></param>
            /// <returns>OrderStateEntity list of status history.</returns>
            public List<OrderStateEntity> GetActiveOrderStatusByOrderNumber(string orderNumber)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPatientOrderStatusPresentCodeByOrderNumber";
                cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", orderNumber));
                return GetRecords(cmd).ToList();
            }

            protected override OrderStateEntity FillRecord(IDataReader dr)
            {
                if (this.c == null || this.c.Count.Equals(0))
                    this.c = dr.GetColumnNameList();

                OrderStateEntity ose = new OrderStateEntity();
                ose.ID = dr.ToInt32("ID", c);
                ose.OrderNumber = dr.AsString("OrderNumber", c);
                ose.OrderStatusTypeID = dr.ToInt32("OrderStatusTypeID", c);
                ose.OrderStatusType = dr.AsString("OrderStatusDescription", c);
                ose.StatusComment = dr.AsString("StatusComment", c);
                ose.LabCode = dr.AsString("LabSiteCode", c);
                ose.IsActive = dr.ToBoolean("IsActive", c);
                ose.ModifiedBy = dr.AsString("ModifiedBy", c);
                ose.DateLastModified = dr.ToDateTime("DateLastModified", c);
                return ose;
            }
        }

        /// <summary>
        /// A custom repository class to handle order status operations for the ManageOrderEntity class.
        /// </summary>
        public sealed class OrdersOfStatusRepository : RepositoryBase<ManageOrderEntity>, IOrdersOfStatusRepository
        {
            private List<String> ColList;

            /// <summary>
            /// Default ctor.
            /// </summary>
            public OrdersOfStatusRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Gets a list of order by clinic for the problem orders grid by site code.
            /// </summary>
            /// <param name="siteCode">Site code to search in.</param>
            /// <param name="modifiedBy">User performing the problem orders operation.</param>
            /// <returns>ManageOrderEntity list of problem orders.</returns>
            public List<ManageOrderEntity> GetOrdersWithProblemsByClinicCode(string siteCode, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPatientOrdersWithProblemsByClinicCode";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", siteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of order by clinic for the overdue grid by site code.
            /// </summary>
            /// <param name="siteCode">Site code to search in.</param>
            /// <param name="modifiedBy">User performing the overdue operation.</param>
            /// <returns>ManageOrderEntity list of overdue orders.</returns>
            public List<ManageOrderEntity> GetOrdersForOverDueDispenseByClinicCode(string siteCode, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPatientOrdersForOverDueProductionByClinicCode";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", siteCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of order by clinic for the check-in grid by site code.
            /// </summary>
            /// <param name="clinicCode">Site code to search in.</param>
            /// <param name="modifiedBy">User performing the check-in operation.</param>
            /// <returns>ManageOrderEntity list of check-in orders.</returns>
            public List<ManageOrderEntity> GetOrdersForCheckInToClinicByClinicCode(string clinicCode, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPatientOrdersForCheckInToClinicByClinicCode";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", clinicCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of order by clinic for the dispense grid by site code.
            /// </summary>
            /// <param name="clinicCode">Site code to search in.</param>
            /// <param name="modifiedBy">User performing the dispense operation.</param>
            /// <returns>ManageOrderEntity list of dispense orders.</returns>
            public List<ManageOrderEntity> GetOrdersToDispenseByClinicCode(string clinicCode, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPatientOrdersForClinicToDispenseByClinicCode";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", clinicCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of order for a clinic for the pending grid by site code.
            /// </summary>
            /// <param name="clinicCode">Site code to search in.</param>
            /// <param name="modifiedBy">User performing the dispense operation.</param>
            /// <returns>ManageOrderEntity list of pending orders in a lab.</returns>
            public List<ManageOrderEntity> GetOrdersPendingByClinicCode(string clinicCode, string modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPatientOrdersAtLabByClinicCode";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ClinicSiteCode", clinicCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of orders by lab for the dispense grid by site code.
            /// </summary>
            /// <param name="labCode">Site code to search in.</param>
            /// <param name="modifiedBy">User performing the dispense operation.</param>
            /// <returns>ManageOrderEntity list of dispense orders.</returns>
            public List<ManageOrderEntity> GetLabDispenseOrders(String labCode, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetLabOrdersForDispenseCheckInByLabCode";
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", labCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                return GetRecords(cmd).Where(x => x.OrderStatusTypeID == 2).ToList();
            }

            /// <summary>
            /// Gets a list of orders by lab for the check-in grid by site code.
            /// </summary>
            /// <param name="labCode">Site code to search in.</param>
            /// <param name="modifiedBy">User performing the check-in operation.</param>
            /// <returns>ManageOrderEntity list of check-in orders.</returns>
            public List<ManageOrderEntity> GetLabCheckinOrders(String labCode, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetLabOrdersForDispenseCheckInByLabCode";
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", labCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                return GetRecords(cmd).Where(x => x.OrderStatusTypeID == 1 || x.OrderStatusTypeID == 4 || x.OrderStatusTypeID == 6 || x.OrderStatusTypeID == 9 || x.OrderStatusTypeID == 10).ToList();
            }

            /// <summary>
            /// Gets a list of orders by lab for the redirect/reject grid by site code.
            /// </summary>
            /// <param name="labCode">Site code to search in.</param>
            /// <param name="modifiedBy">User performing the dispense operation.</param>
            /// <returns>ManageOrderEntity list of dispense orders.</returns>
            public List<ManageOrderEntity> GetLabRedirectRejectOrders(String labCode, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetLabOrdersForDispenseCheckInByLabCode";
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", labCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));
                return GetRecords(cmd).Where(x => x.OrderStatusTypeID == 2).ToList();
            }

            /// <summary>
            /// Gets a list of orders by lab for the hold for stock grid by site code.
            /// </summary>
            /// <param name="labCode">Site code to search in.</param>
            /// <param name="modifiedBy">User performing the check-in operation.</param>
            /// <returns>ManageOrderEntity list of hold for stock related orders.</returns>
            public List<ManageOrderEntity> GetLabHoldStockOrders(String labCode, String modifiedBy)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetLabOrdersForDispenseCheckInByLabCode";
                cmd.Parameters.Add(this.DAL.GetParamenter("@LabSiteCode", labCode));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ModifiedBy", modifiedBy));

                return GetRecords(cmd).ToList();
            }

            protected override ManageOrderEntity FillRecord(IDataReader dr)
            {
                if (this.ColList == null || this.ColList.Count.Equals(0))
                    this.ColList = dr.GetColumnNameList();

                var moe = new ManageOrderEntity();

                moe.ClinicSiteCode = dr.AsString("ClinicSiteCode", this.ColList);
                moe.DateLastModified = dr.ToDateTime("DateLastModified", this.ColList);
                moe.DateOrderCreated = dr.ToDateTime("DateOrderCreated", this.ColList);
                moe.DateReceivedByLab = dr.ToDateTime("DateReceivedByLab", this.ColList);
                moe.DaysPastDue = dr.ToInt32("DaysPastDue", this.ColList);
                moe.FirstName = dr.AsString("FirstName", this.ColList);
                moe.FrameCode = dr.AsString("FrameCode", this.ColList);
                moe.IsActive = dr.ToBoolean("IsActive", this.ColList);
                moe.LabSiteCode = dr.AsString("LabSiteCode", this.ColList);
                moe.LastName = dr.AsString("LastName", this.ColList);
                moe.LensMaterial = dr.AsString("LensMaterial", this.ColList);
                moe.LensType = dr.AsString("LensType", this.ColList);
                moe.MiddleName = dr.AsString("MiddleName", this.ColList);
                moe.ModifiedBy = dr.AsString("ModifiedBy", this.ColList);
                moe.OrderNumber = dr.AsString("OrderNumber", this.ColList);
                moe.OrderStatusDescription = dr.AsString("OrderStatusDescription", this.ColList);
                moe.OrderStatusTypeID = dr.ToInt32("OrderStatusTypeID", this.ColList);
                moe.ShipAddress1 = dr.AsString("ShipAddress1", this.ColList);
                moe.ShipAddress2 = dr.AsString("ShipAddress2", this.ColList);
                moe.ShipAddress3 = dr.AsString("ShipAddress3", this.ColList);
                moe.ShipCity = dr.AsString("ShipCity", this.ColList);
                moe.ShipCountry = dr.AsString("ShipCountry", this.ColList);
                moe.ShipState = dr.AsString("ShipState", this.ColList);
                moe.OrderDisbursement = dr.AsString("Distribution", this.ColList); //SharedOperations.ExtractOrderDisbursement(dr.ToBoolean("ShipToPatient", this.ColList), dr.ToBoolean("clinicShipToPatient", this.ColList));
                moe.ShipZipCode = dr.AsString("ShipZipCode", this.ColList);
                moe.StatusComment = dr.AsString("StatusComment", this.ColList);

                if (moe.DateOrderCreated.Equals(default(DateTime)))
                    moe.DateOrderCreated = dr.ToDateTime("DateCreated", this.ColList);

                moe.IndividualId = dr.ToInt32("IndividualID", this.ColList);
                moe.Priority = dr.AsString("Priority", this.ColList);
                return moe;
            }
        }
    }
}