using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Admin
{
    public interface ISiteCodeAddView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        string SiteCode { get; set; }

        string SiteName { get; set; }

        string SiteType { get; set; }

        string SiteDescription { get; set; }

        string BOS { get; set; }

        bool IsMultivision { get; set; }

        bool IsReimbursable { get; set; }

        string Address1 { get; set; }

        string Address2 { get; set; }

        string Address3 { get; set; }

        string City { get; set; }

        string State { get; set; }

        string Country { get; set; }

        string ZipCode { get; set; }

        string MailAddress1 { get; set; }

        string MailAddress2 { get; set; }

        string MailAddress3 { get; set; }

        string MailCity { get; set; }

        string MailState { get; set; }

        string MailCountry { get; set; }

        string MailZipCode { get; set; }

        bool IsConusMail { get; set; }

        bool IsConus { get; set; }

        string EMailAddress { get; set; }

        string DSNPhoneNumber { get; set; }

        string RegPhoneNumber { get; set; }

        int MaxEyeSize { get; set; }

        int MaxFramesPerMonth { get; set; }

        double MaxPower { get; set; }

        bool HasLMS { get; set; }

        bool ShipToPatientLab { get; set; }

        int Region { get; set; }

        string MultiPrimary { get; set; }

        string MultiSecondary { get; set; }

        string SinglePrimary { get; set; }

        string SingleSecondary { get; set; }

        bool IsActive { get; set; }

        bool NewSite { get; set; }

        string SiteCodetb { get; set; }

        bool UseAddress { get; set; }

        string ErrMessage { get; set; }

        List<LookupTableEntity> StateData { get; set; }

        List<LookupTableEntity> CountryData { get; set; }

        List<LookupTableEntity> BOSData { get; set; }

        List<string> MPrimary { get; set; }

        List<string> MSecondary { get; set; }

        List<string> SPrimary { get; set; }

        List<string> SSecondary { get; set; }

        List<LookupTableEntity> SiteCodes { get; set; }
    }
}