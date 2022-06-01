using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class ManageOrderEntity
    {
        public String OrderNumber { get; set; }

        public String ClinicSiteCode { get; set; }

        public String LabSiteCode { get; set; }

        public String ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public DateTime DateOrderCreated { get; set; }

        public DateTime DateHoldStockEnd { get; set; }

        public String FrameCode { get; set; }

        public String LensType { get; set; }

        public String LensMaterial { get; set; }

        public String LastName { get; set; }

        public String MiddleName { get; set; }

        public String FirstName { get; set; }

        public int OrderStatusTypeID { get; set; }

        public String OrderStatusDescription { get; set; }

        public String StatusComment { get; set; }

        public bool IsActive { get; set; }

        public bool IsSelected { get; set; }

        public String OrderDisbursement { get; set; }

        public String ShipAddress1 { get; set; }

        public String ShipAddress2 { get; set; }

        public String ShipAddress3 { get; set; }

        public String ShipCity { get; set; }

        public String ShipState { get; set; }

        public String ShipCountry { get; set; }

        public String ShipZipCode { get; set; }

        public DateTime DateReceivedByLab { get; set; }

        public int DaysPastDue { get; set; }

        public int IndividualId { get; set; }

        public String Priority { get; set; }
    }
}