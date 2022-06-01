using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class OrderTrackingEntity
    {
        public string TrackingNumber { get; set; }
        public int OrderCount { get; set; }
        public string Patient { get; set; }


    }

    [Serializable]
    public sealed class OrderTrackingScanEntity
    {
        public string TrackingNumber { get; set; }
        public int PatientID { get; set; }
        public string Order1 { get; set; }
        public string Order2 { get; set; }
        public string Order3 { get; set; }
        public string Order4 { get; set; }

    }

    [Serializable]
    public sealed class OrderTrackingPatientEntity
    {
        public string TrackingNumber { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string FrameCode { get; set; }
        public string LensType { get; set; }
    }

    [Serializable]
    public sealed class OrderTrackingOrderEntity
    {
        public int PatientID { get; set; }
    }

}
