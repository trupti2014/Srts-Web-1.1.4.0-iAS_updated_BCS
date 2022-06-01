using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class SiteCodeEntity
    {
        public string SiteCode { get; set; }

        public string SiteName { get; set; }

        public string SiteType { get; set; }

        public string SiteDescription { get; set; }

        public string BOS { get; set; }

        public string BOSDescription { get; set; }

        public bool IsMultivision { get; set; }

        public int ID { get; set; }

        public string AddressType { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public bool IsConus { get; set; }

        public string EMailAddress { get; set; }

        public string DSNPhoneNumber { get; set; }

        public string RegPhoneNumber { get; set; }

        public int NextOrderNumber { get; set; }

        public bool IsAPOCompatible { get; set; }

        public bool ShipToPatientLab { get; set; }

        public int MaxEyeSize { get; set; }

        public int MaxFramesPerMonth { get; set; }

        public double MaxPower { get; set; }

        public bool HasLMS { get; set; }

        public int Region { get; set; }

        public string MultiPrimary { get; set; }

        public string MultiSecondary { get; set; }

        public string SinglePrimary { get; set; }

        public string SingleSecondary { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public string AddressDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(Address2))
                {
                    return string.Format("{0}<br />{1}<br />{2}, {3} {4}", AddressType, SiteName, Address1, City, State, ZipCode);
                }
                else if (string.IsNullOrEmpty(Address3))
                {
                    return string.Format("{0}<br />{1}<br />{2}<br /> {3}, {4} {5}", AddressType, SiteName, Address1, Address2, City, State, ZipCode);
                }
                else
                {
                    return string.Format("{0}<br />{1}<br />{2}<br /> {3}<br />, {4} {5} {6}", AddressType, SiteName, Address1, Address2, Address3, City, State, ZipCode);
                }
            }
        }

        public string SiteCombination
        {
            get { return string.Format("{0} - {1}", SiteName, SiteCode); }
        }

        public string SiteCombinationProfile
        {
            get { return string.Format("{0} - {1}", SiteCode, SiteName); }
        }

        public bool IsReimbursable { get; set; }
    }
}