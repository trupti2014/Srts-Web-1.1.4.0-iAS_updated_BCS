using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IOrderRepository
    {
        List<String> GetNextOrderNumbers(String siteCode, Int32 numberOfNumbers);

        OrderEntity InsertOrder(OrderEntity order, bool _state);

        DataSet GetClinicSummary(String _siteCode);

        DataSet GetLabSummary(string _siteCode);

        DataTable Get711DataByLabCode(String _siteCode, String _orderNbr, String modifiedBy);

        DataTable GetWoundedWarriorReportData(DateTime fromDate, DateTime toDate);

        Boolean UpdateOrderWithBarcode(OrderEntity order);

        List<String> GetPatientOrdersNoBarCodes();

        List<OrderEntity> GetOrderByOrderNumberNonGEyes(string orderNumber, string modifiedBy);
    }

    public interface IGEyesOrderRepository
    {
        OrderEntity GetOrderByOrderNumber(string orderNumber, string modifiedBy);
    }

    public interface IJSpecsOrderRepository
    {
        List<JSpecsOrderDisplayEntity> GetOrdersByIndividualID(int individualID, string modifiedBy, string clinicSiteCode);
    }

    public interface IJSpecsOrderDetailsRepository
    {
        JSpecsOrderDetailsDisplayEntity GetOrderByOrderNumber(string orderNumber, string modifiedBy);
    }

    public interface IDisplayRepository
    {
        List<OrderDisplayEntity> GetOrderDisplay(int individualID, string modifiedBy, Boolean isGeyes = false);
    }
}