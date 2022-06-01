using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class FrameItemEntity
    {
        public string Value { get; set; }

        public string Text { get; set; }

        public string TypeEntry { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public String Availability { get; set; }
    }
}