using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class FabricationParameterEntitiy
    {
        public int ID { get; set; }

        public string Material { get; set; }

        public decimal Cylinder { get; set; }

        public decimal MaxPlus { get; set; }

        public decimal MaxMinus { get; set; }

        public bool IsStocked { get; set; }

        public string SiteCode { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public string CapabilityType { get; set; }
    }
}
