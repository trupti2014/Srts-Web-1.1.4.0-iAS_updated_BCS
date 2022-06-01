using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface ISiteCodeEditView
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

        bool UseAddress { get; set; }

        string ErrMessage { get; set; }

        List<LookupTableEntity> SiteTypes { get; set; }

        List<LookupTableEntity> StateData { get; set; }

        List<LookupTableEntity> CountryData { get; set; }

        List<LookupTableEntity> BOSData { get; set; }

        List<KeyValuePair<string, string>> MPrimary { get; set; }

        List<KeyValuePair<string, string>> MSecondary { get; set; }

        List<KeyValuePair<string, string>> SPrimary { get; set; }

        List<KeyValuePair<string, string>> SSecondary { get; set; }

        //List<FabricationParameterEntitiy> FabricationParameterData { get; set; }

        //decimal Cylinder { get; set; }

        //decimal MaxPlus { get; set; }

        //decimal MaxMinus { get; set; }

        //string Material { get; set; }

        //Dictionary<String, String> LensMaterial { set; }

        //string IsStocked { get; set; }

        //int MatParamID { get; set; }
    }
}