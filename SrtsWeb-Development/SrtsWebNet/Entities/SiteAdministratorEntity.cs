using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class SiteAdministratorEntity
    {
        public int ID { get; set; }

        public string SiteCode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAdddress { get; set; }

        public string RegPhoneNumber { get; set; }

        public string DSNPhoneNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string FullName
        {
            get
            {
                    return string.Format("{0} {1}", FirstName,  LastName);
            }
        }

        public string CityStateZipCodeCombination
        {
            get
            {
                return string.Format("{0}, {1} {2}", City, State, ZipCode);
            }
        }
    }
}