using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class IndividualTypeEntity
    {
        public Int32 ID { get; set; }

        public Int32 IndividualId { get; set; }

        public Int32 TypeId { get; set; }

        public String TypeDescription { get; set; }

        public String ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public Boolean IsActive { get; set; }
    }
}