using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class CostItemEntity
    {
        public int ID { get; set; }

        public string Code { get; set; }

        public string CostValue { get; set; }

        public string CostText { get; set; }

        public decimal ConusCost { get; set; }

        public decimal OConusCost { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }
    }
}