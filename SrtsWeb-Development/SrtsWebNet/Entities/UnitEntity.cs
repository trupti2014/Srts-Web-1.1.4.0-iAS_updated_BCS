using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class UnitEntity
    {
        public string UIC { get; set; }

        public string UnitName { get; set; }

        public string UnitAddress1 { get; set; }

        public string UnitAddress2 { get; set; }

        public string UnitCity { get; set; }

        public string UnitState { get; set; }

        public string UnitZipCode { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }
    }
}