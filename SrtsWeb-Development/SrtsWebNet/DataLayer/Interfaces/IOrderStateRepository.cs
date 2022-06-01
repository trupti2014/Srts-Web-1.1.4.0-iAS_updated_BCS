using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IOrderStatusRepository
    {
        List<OrderStateEntity> GetOrderStateByOrderNumber(string orderNumber);

        Boolean InsertPatientOrderState(OrderStateEntity _orderState);
    }

    public interface IOrderStatusRepositoryTEMP
    {
        List<OrderStateEntity> GetActiveOrderStatusByOrderNumber(string orderNumber);
    }

    public interface IOrdersOfStatusRepository
    {
        List<ManageOrderEntity> GetOrdersForOverDueDispenseByClinicCode(string siteCode, string modifiedBy);

        List<ManageOrderEntity> GetOrdersWithProblemsByClinicCode(string siteCode, string modifiedBy);

        List<ManageOrderEntity> GetOrdersForCheckInToClinicByClinicCode(string clinicCode, string modifiedBy);

        List<ManageOrderEntity> GetOrdersToDispenseByClinicCode(string clinicCode, string modifiedBy);

        List<ManageOrderEntity> GetLabHoldStockOrders(String labCode, String modifiedBy);

        List<ManageOrderEntity> GetLabCheckinOrders(String labCode, String modifiedBy);

        List<ManageOrderEntity> GetLabDispenseOrders(String labCode, String modifiedBy);
    }
}