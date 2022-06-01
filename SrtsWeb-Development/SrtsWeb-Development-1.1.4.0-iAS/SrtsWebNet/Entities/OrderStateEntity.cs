using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class OrderStateEntity
    {
        public int ID { get; set; }

        public string OrderNumber { get; set; }

        public int OrderStatusTypeID { get; set; }

        public string OrderStatusType { get; set; }

        public string StatusComment { get; set; }

        public string LabCode { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }
    }
}