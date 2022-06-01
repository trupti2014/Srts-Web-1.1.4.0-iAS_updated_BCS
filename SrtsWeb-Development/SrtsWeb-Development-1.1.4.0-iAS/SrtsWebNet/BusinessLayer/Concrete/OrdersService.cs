using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class used to perform order related operations.
    /// </summary>
    public class OrdersService
    {
        /// <summary>
        /// Gets a flag to determine if an order exists.
        /// </summary>
        /// <param name="orderNumber">Order number to search for.</param>
        /// <param name="modifiedBy">User performing the search.</param>
        /// <returns>Flag of weather or not an order exists.</returns>
        public Boolean CheckForOrder(String orderNumber, String modifiedBy)
        {
            var r = new OrderRepository();
            return r.GetOrderByOrderNumberNonGEyes(orderNumber, modifiedBy).Count > 0;
        }

        /// <summary>
        /// Gets an OrderEntity object by order number.
        /// </summary>
        /// <param name="orderNumber">Order number to search for.</param>
        /// <param name="modifiedBy">User performing the search.</param>
        /// <returns>OrderEntity object for the supplied order number or a null.</returns>
        public OrderEntity FindOrder(String orderNumber, String modifiedBy)
        {
            var r = new OrderRepository();
            var l = r.GetOrderByOrderNumberNonGEyes(orderNumber, modifiedBy);

            if (l.IsNullOrEmpty()) return null;
            return l[0];
        }

        /// <summary>
        /// Gets the order status history for a supplied order number.
        /// </summary>
        /// <param name="orderNumber">Order number to search for.</param>
        /// <returns>Order status history list.</returns>
        public List<OrderStateEntity> GetOrderStatuses(String orderNumber)
        {
            var r = new OrderStateRepository.OrderStatusRepository();
            var ss = r.GetOrderStateByOrderNumber(orderNumber);
            r = null;
            return ss;
        }

        /// <summary>
        /// Gets a custom status type for the current active order status by order number.
        /// </summary>
        /// <param name="orderNumber">Order number to search for.</param>
        /// <returns>Custom status type representation of the current active order status.</returns>
        public StatusType GetLatestStatus(String orderNumber)
        {
            var ss = GetOrderStatuses(orderNumber);

            if (ss == null || ss.Count.Equals(0)) return StatusType.NONE;
            var s = ss.Where(x => x.IsActive = true).FirstOrDefault();
            if (s == null) return StatusType.NONE;

            return GetOrderStatusByTypeID(s.OrderStatusTypeID);
        }

        /// <summary>
        /// Get order status by order status type id
        /// </summary>
        /// <param name="orderStatusByID">id to get orderstatus</param>
        /// <returns></returns>
        public StatusType GetOrderStatusByTypeID(int orderStatusByID)
        {
            StatusType st = StatusType.NONE;
            switch (orderStatusByID)
            {
                case 1:
                    st = StatusType.CREATED;
                    break;

                case 2:
                    st = StatusType.LAB_RECEIVED;
                    break;

                case 3:
                    st = StatusType.REJECTED;
                    break;

                case 4:
                    st = StatusType.REDIRECTED;
                    break;
                case 5:
                    st = StatusType.HOLD_FOR_STOCK;
                    break;
                case 6:
                    st = StatusType.GEYES_CREATED_ORDER;
                    break;
                case 7:
                    st = StatusType.LAB_DISPENSED;
                    break;

                case 8:
                    st = StatusType.DISPENSED;
                    break;

                case 9:
                    st = StatusType.RESUBMITTED;
                    break;

                case 11:
                    st = StatusType.RECEIVED;
                    break;
                case 12:
                    st = StatusType.ORDER_SENT_PMRS;
                    break;
                case 13:
                    st = StatusType.ORDER_SENT_MEDPROS;
                    break;
                case 14:
                    st = StatusType.CANCELLED;
                    break;
                case 15:
                    st = StatusType.INCOMPLETE;
                    break;
                case 16:
                    st = StatusType.FORCEDRECEIVED;
                    break;
                case 17:
                    st = StatusType.RETURN_TO_STOCK;
                    break;
                case 18:
                    st = StatusType.FABRICATION_COMPLETE;
                    break;
                //case 19:
                //    st = StatusType.SHIPPED_TO_PATIENT;
                //    break;
                case 20:
                    st = StatusType.EDIT_SUCCESSFUL;
                    break;

                default: // A status of 19 is ship to patient and is a StatusType of view so I let it fall to the default option.
                    st = StatusType.VIEW;
                    break;
            }

            return st;
        }

        /// <summary>
        /// Adds a new order status to the database.
        /// </summary>
        /// <param name="s">Order status entity to insert.</param>
        /// <returns>Flag of weather or not the add was successfull.</returns>
        public Boolean UpdateOrderStatus(OrderStateEntity s)
        {
            var r = new OrderStateRepository.OrderStatusRepository();
            return r.InsertPatientOrderState(s);
        }
    }
}