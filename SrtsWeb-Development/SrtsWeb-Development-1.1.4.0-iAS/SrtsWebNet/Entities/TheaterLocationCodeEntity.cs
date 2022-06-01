using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class TheaterLocationCodeEntity
    {
        public string TheaterCode { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public string Description { get; set; }
    }
}