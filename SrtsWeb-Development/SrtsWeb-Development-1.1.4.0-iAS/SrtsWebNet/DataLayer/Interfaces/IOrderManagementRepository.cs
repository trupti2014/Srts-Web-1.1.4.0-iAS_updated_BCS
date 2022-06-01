using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IOrderManagementRepository
    {
        List<Order> GetAllOrders(Int32 patientId, String modifiedBy);

        List<Order> GetAllLabOrders(Int32 patientId, String siteCode, String modifiedBy);

        Order GetOrderByOrderNumber(String orderNumber, String modifiedBy);

        List<Order> MapOrderData(DataTable dt);

        List<Prescription> GetAllPrescriptions(Int32 patientId, String modifiedBy);

        Prescription GetPrescriptionById(Int32 prescriptionId, String modifiedBy);

        Prescription GetPrescriptionByName(String prescriptionName, String modifiedBy);

        List<Prescription> MapPrescriptionData(DataTable dt);

        List<LookupData> GetLookupData(String demographic, String siteCode);

        List<LookupData> MapLookupData(DataTable dt);

        bool InsertPrescription(Prescription prescription, String modifiedBy, out Int32 prescriptionId);

        bool UpdatePrescription(Prescription prescription, String modifiedBy);

        bool DeletePrescription(Int32 prescriptionId, String modifiedBy);

        bool InsertOrder(Order order, String modifiedBy);

        bool UpdateOrder(Order order, String modifiedBy);

        bool DeleteOrder(String orderNumber, string modifiedBy);

        List<String> GetNextOrderNumbers(string siteCode, int numberOfNumbers);

        void UnlinkOrders(List<String> orderNumbers);

        //Dictionary<String, String> GetPriorities(String bos, String status, String grade);
    }
}