using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class OrderFrameEntity
    {
        public int ID { get; set; }

        public string FrameCode { get; set; }

        public string FrameColor { get; set; }

        public string FrameEyeSize { get; set; }

        public string FrameBridgeSize { get; set; }

        public string FrameTemple { get; set; }
    }
}