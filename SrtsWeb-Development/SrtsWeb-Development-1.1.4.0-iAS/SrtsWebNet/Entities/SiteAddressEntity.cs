using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class SiteAddressEntity
    {
        public int ID { get; set; }

        public string SiteCode { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string AddressType { get; set; }

        public bool IsConus { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }
    }
}