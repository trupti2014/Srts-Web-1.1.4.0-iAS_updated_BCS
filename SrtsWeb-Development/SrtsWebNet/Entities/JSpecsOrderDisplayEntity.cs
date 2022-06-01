using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class JSpecsOrderDisplayEntity
    {
        public bool EligibleOrder { get; set; } = false;
        public string OrderNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public string OrderStatus { get; set; }
        public int PrescriptionID { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string FrameCode { get; set; }
        public string FrameCategory { get; set; }
        public string FrameUserFriendlyName { get; set; }
        public bool FrameIsActive { get; set; }
        public string RxName { get; set; }
        public string RxNameUserFriendly { get; set; }
        public string Tint { get; set; }
        public string LensTint { get; set; }
        public string LensTypeLong { get; set; }
        public string LensCoating { get; set; }
        public string FrameEyeSize { get; set; }
        public string FrameBridgeSize { get; set; }
        public string FrameTempleType { get; set; }
        public string FrameColor { get; set; }
        public string FrameImgPath { get; set; }
        public string FrameImgName { get; set; }
        public string FrameImgType { get; set; }
        public int ReorderCount { get; set; }
    }
}