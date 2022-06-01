using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class FrameEntity
    {
        public string FrameCode { get; set; }

        public string FrameDescription { get; set; }

        public string FrameCodeAndDescription { get { return String.Format("{0} - {1}", FrameCode, FrameDescription); } }

        public string FrameNotes { get; set; }

        public string FrameType { get; set; }

        public bool IsInsert { get; set; }

        public int MaxPair { get; set; }

        public string ImageURL { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public bool IsFoc { get; set; }
        //public object FrameImg { get; set; }
    }
}