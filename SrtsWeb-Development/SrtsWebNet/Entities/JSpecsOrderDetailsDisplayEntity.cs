using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class JSpecsOrderDetailsDisplayEntity
    {
        public string OrderNumber { get; set; }
        public DateTime DateLastModified { get; set; }
        public AddressEntity Address { get; set; } = new AddressEntity();
        public int PrescriptionID { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public decimal PDDistant { get; set; }
        public decimal PDNear { get; set; }
        public string ODSphere { get; set; }
        public string ODCylinder { get; set; }
        public string ODAxis { get; set; }
        public string ODAdd { get; set; }
        public string OSSphere { get; set; }
        public string OSCylinder { get; set; }
        public string OSAxis { get; set; }
        public string OSAdd { get; set; }
        public string FrameCode { get; set; }
        public string FrameFamily { get; set; }
        public string FrameEyeSize { get; set; }
        public string FrameBridgeSize { get; set; }
        public string FrameTempleType { get; set; }
        public string FrameColor { get; set; }
        public string FrameImgPath { get; set; }
        public string FrameImgName { get; set; }
        public string FrameImgType { get; set; }
        public string RxName { get; set; }
        public string RxNameUserFriendly { get; set; }
        public int OrderStatusTypeID { get; set; }
        public string OrderStatus { get; set; }
    }
}