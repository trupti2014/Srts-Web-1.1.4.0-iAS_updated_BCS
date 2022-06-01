using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class AddressEntity
    {
        public Object Clone() { return MemberwiseClone(); }

        public int ID { get; set; }

        public int IndividualID { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string AddressType { get { return "MAIL"; } }

        public bool IsActive { get; set; }

        public string UIC { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }
        
        public string VerifiedBy { get; set; }

        public DateTime DateVerified { get; set; }

        public int ExpireDays { get; set; }

        public string FormattedAddress
        {
            get
            {
                string formattedAddress = string.Empty;
                formattedAddress += Address1;
                formattedAddress += Address2 != null ? ", " + Address2 + ", " : ",";
                formattedAddress += City + ", " + State + " " + ZipCode;
                return formattedAddress;
            }
        }
    }
}