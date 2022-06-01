using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class PhoneNumberEntity
    {
        public int ID { get; set; }

        public int IndividualID { get; set; }

        public string PhoneNumberType { get { return "HOME"; } }

        public string AreaCode { get; set; }

        public string PhoneNumber { get; set; }

        public string Extension { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public string PhoneDisplay { get { return String.Format("{0} - ({1}) {2}", PhoneNumberType, AreaCode, PhoneNumber); } }
    }
}