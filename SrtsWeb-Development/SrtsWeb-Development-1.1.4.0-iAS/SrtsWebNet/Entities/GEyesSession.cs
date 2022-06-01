using System;
using System.Data;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class GEyesSession
    {
        public PatientEntity Patient { get; set; }

        public DataView FrameData { get; set; }

        public OrderEntity OrderToSave { get; set; }

        public bool SecurityAcknowledged { get; set; }
    }
}