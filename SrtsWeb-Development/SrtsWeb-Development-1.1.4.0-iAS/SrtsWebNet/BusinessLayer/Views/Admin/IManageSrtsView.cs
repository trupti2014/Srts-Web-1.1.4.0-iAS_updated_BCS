using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Admin
{
    public interface IManageSrtsView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        string SiteCode { get; set; }

        string SiteName { get; set; }

        string SiteType { get; set; }

        string SiteDescription { get; set; }

        string BOSSelected { get; set; }

        List<LookupTableEntity> BOSData { get; set; }

        bool IsMultivision { get; set; }

        string Address1 { get; set; }

        string Address2 { get; set; }

        string City { get; set; }

        string StateSelected { get; set; }

        List<LookupTableEntity> StateData { get; set; }

        string CountrySelected { get; set; }

        List<LookupTableEntity> CountryData { get; set; }

        string ZipCode { get; set; }

        bool IsConus { get; set; }

        string EMailAddress { get; set; }

        string DSNPhoneNumber { get; set; }

        string RegPhoneNumber { get; set; }

        bool IsAPOCompatible { get; set; }

        int MaxEyeSize { get; set; }

        int MaxFramesPerMonth { get; set; }

        double MaxPower { get; set; }

        bool IsActive { get; set; }
    }
}