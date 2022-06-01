using System;
using System.Data;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class JSpecsOrder
    {
        public OrderEntity PatientOrder { get; set; }
        public AddressEntity OrderAddress { get; set; } = null;
        public PrescriptionEntity OrderPrescription { get; set; } = null;
        public EMailAddressEntity OrderEmailAddress { get; set; } = null;
        public string OrderID { get; set; }
        public bool IsReOrder { get; set; }
    }
}