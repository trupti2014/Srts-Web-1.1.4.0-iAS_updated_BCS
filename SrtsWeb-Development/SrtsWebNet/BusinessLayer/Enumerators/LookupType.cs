using System.ComponentModel;

namespace SrtsWeb.BusinessLayer.Enumerators
{
    public enum LookupType
    {
        [DescriptionAttribute("Acuity List")]
        AcuityType,

        [DescriptionAttribute("Address Types")]
        AddressType,

        [DescriptionAttribute("Air Crew Types")]
        AirCrewType,

        [DescriptionAttribute("Branch of Service Types")]
        BOSType,

        [DescriptionAttribute("Country List")]
        CountryList,

        [DescriptionAttribute("Country Calling List")]
        CntryCallingCode,

        [DescriptionAttribute("Email Types")]
        EmailType,

        [DescriptionAttribute("Government Holidays")]
        Holiday,

        [DescriptionAttribute("Order Priority Types")]
        OrderPriorityType,

        [DescriptionAttribute("Patient Status Type")]
        PatientStatusType,

        [DescriptionAttribute("Personal Number Types")]
        IDNumberType,

        [DescriptionAttribute("Individual Types")]
        IndividualType,

        [DescriptionAttribute("Phone Types")]
        PhoneType,

        [DescriptionAttribute("Rank Types")]
        RankType,

        [DescriptionAttribute("Site Types")]
        SiteType,

        [DescriptionAttribute("State Lists")]
        StateList
    }
}