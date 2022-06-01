using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class FrameImageEntity
    {
        public int ID { get; set; }
        public string FrameFamily { get; set; }

        public string FrameCode { get; set; }

        public string ImgName { get; set; }

        public string ImgPath { get; set; }

        public byte[] FrameImage { get; set; }

        public DateTime DateLoaded { get; set; }

        public string ModifiedBy { get; set; }

        public string ContentType { get; set; }

        public string Color { get; set; }

        public bool ISActive { get; set; }

        public int BridgeSize { get; set; }

        public string Temple { get; set; }

        public int EyeSize { get; set; }

        public string ImgAngle { get; set; }

        public string MFGName { get; set; }

        //public FrameImageEntity Clone() { return this.MemberwiseClone() as FrameImageEntity; }

    }

}

