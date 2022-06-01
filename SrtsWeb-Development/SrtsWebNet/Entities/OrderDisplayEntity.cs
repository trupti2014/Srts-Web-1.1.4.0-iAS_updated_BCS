using System;
using System.Collections.Generic;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class OrderDisplayEntity
    {
        public string LensType { get; set; }

        public string LensTypeLong { get; set; }

        public string LensTint { get; set; }

        public string LensCoating { get; set; }

        public string FrameDescription { get; set; }

        public string FrameCode { get; set; }

        public string FrameColor { get; set; }

        public string ODSphere { get; set; }

        public string ODCylinder { get; set; }

        public int ODAxis { get; set; }

        public string OSSphere { get; set; }

        public string OSCylinder { get; set; }

        public int OSAxis { get; set; }

        public double ODAdd { get; set; }

        public double OSAdd { get; set; }

        public string OrderNumber { get; set; }

        public string LabSiteCode { get; set; }

        public string ClinicSiteCode { get; set; }

        public DateTime DateCreated { get; set; }

        public List<OrderDisplayStatusEntity> OrderStatus { get; set; }

    }
}