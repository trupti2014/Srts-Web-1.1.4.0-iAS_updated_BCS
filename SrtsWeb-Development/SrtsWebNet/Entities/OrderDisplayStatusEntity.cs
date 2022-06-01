using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class OrderDisplayStatusEntity
    {
        public string OrderNumber { get; set; }

        public DateTime DateLastModified { get; set; }

        public bool IsActive { get; set; }

        public string LabSiteCode { get; set; }

        public string ModifiedBy { get; set; }

        public int OrderStatusTypeID { get; set; }

        public string StatusComment { get; set; }

        public string OrderStatusDescription { get; set; }
    }
}