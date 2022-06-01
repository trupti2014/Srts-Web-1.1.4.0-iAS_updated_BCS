using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class LookupTableEntity
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Text { get; set; }

        public string Value { get; set; }

        public string ValueTextCombo { get { return String.Format("{0} - {1}", Value, Text); } }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }
    }
}