using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Linq;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class used to perform quick search operations.
    /// </summary>
    public class MasterService
    {
        /// <summary>
        /// Gets an order entity by a provided order number.  The output parameter of status can be VIEW, NONE, DISPENSED, or RECEIVED
        /// </summary>
        /// <param name="orderNumber">Order to look up.</param>
        /// <param name="modifiedBy">User performing the look up.</param>
        /// <param name="status">Out - StatusType - Current status of the order</param>
        /// <returns>OrderEntity</returns>
        public static OrderEntity DoQuickSearchOrder(String orderNumber, String modifiedBy, out StatusType status)
        {
            status = StatusType.NONE;
            var os = new OrdersService();
            var o = os.FindOrder(orderNumber, modifiedBy);
            if (o == null) { status = StatusType.NONE; return null; }
            // get active status
            status = os.GetLatestStatus(orderNumber);

            return o;
        }

        /// <summary>
        /// Updates the status of an order using provided order number and order status.
        /// </summary>
        /// <param name="orderNumber">Order to update the status to.</param>
        /// <param name="newStatus">Status to update the order to.</param>
        /// <param name="modifiedBy">User performing the update.</param>
        /// <returns></returns>
        public static Boolean DoStatusUpdate(String orderNumber, String newStatus, String modifiedBy)
        {
            // Get existing status'
            var os = new OrdersService();
            var lss = os.GetOrderStatuses(orderNumber);
            if (lss == null || lss.Count.Equals(0)) return false;
            var ls = lss.Where(x => x.IsActive == true).FirstOrDefault();

            var newStat = new OrderStateEntity()
            {
                IsActive = true,
                LabCode = ls.LabCode,
                ModifiedBy = modifiedBy,
                OrderNumber = orderNumber,
                OrderStatusTypeID = newStatus.ToLower().Equals("c") ? 11 : newStatus.ToLower().Equals("d") ? 8 : 16,
                StatusComment = newStatus.ToLower().Equals("c") ? "Clinic Received Order" : newStatus.ToLower().Equals("d") ? "Clinic Dispensed Order" : "Clinic Received Order - No Lab Checkout"
            };

            return os.UpdateOrderStatus(newStat);
        }

        /// <summary>
        /// Gets a patient order data transfer object based on an individuals ID number; SSN, DODID.
        /// </summary>
        /// <param name="idNum">ID number to search.</param>
        /// <param name="modifiedBy">User performing the search.</param>
        /// <returns>Patient and order data from the search.</returns>
        public static PatientOrderDTO DoQuickSearchPatient(String idNum, String modifiedBy)
        {
            var lie = PatientsService.SearchIndividual("G", idNum, null, null, null, true, modifiedBy);
            var dto = new PatientOrderDTO();

            if (lie.Count > 0)
            {
                dto.Demographic = lie[0].Demographic;
                dto.IndividualId = lie[0].ID;
                dto.PatientSiteCode = lie[0].SiteCodeID;
            }
            else
                dto = null;

            return dto;
        }

        /// <summary>
        /// Gets a patient order data transfer object based on an individuals ID number: Primary key on table.
        /// </summary>
        /// <param name="IndividualId">Individual ID number to search.</param>
        /// <param name="modifiedBy">User performing the search.</param>
        /// <returns>Patient and order data from the search.</returns>
        public static PatientOrderDTO DoQuickSearchPatient(Int32 IndividualId, String modifiedBy)
        {
            var lie = PatientsService.SearchIndividual(IndividualId, modifiedBy);
            var dto = new PatientOrderDTO();

            if (lie.Count > 0)
            {
                dto.Demographic = lie[0].Demographic;
                dto.IndividualId = lie[0].ID;
                dto.PatientSiteCode = lie[0].SiteCodeID;
            }
            else
                dto = null;

            return dto;
        }
    }
}