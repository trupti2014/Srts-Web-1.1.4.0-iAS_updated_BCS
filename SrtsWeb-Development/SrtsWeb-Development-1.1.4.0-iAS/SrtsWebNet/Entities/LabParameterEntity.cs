using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class LabParameterEntity
    {
        public string SiteCode { get; set; }

        public decimal MaxPrism { get; set; }

        public decimal MaxDecentrationPlus { get; set; }

        public decimal MaxDecentrationMinus { get; set; }

        public bool PatientDirectedOrders { get; set; }

        public string Comment { get; set; }

        public string ModifiedBy { get; set; }

    }
}
