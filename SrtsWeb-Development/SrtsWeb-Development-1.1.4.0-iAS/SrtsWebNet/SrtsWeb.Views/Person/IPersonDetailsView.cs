using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Person
{
    public interface IPersonDetailsView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        AddressEntity PrimaryAddress { get; set; }

        EMailAddressEntity PrimaryEmail { get; set; }

        IdentificationNumbersEntity DSS { get; set; }

        IdentificationNumbersEntity DIN { get; set; }

        IdentificationNumbersEntity DBN { get; set; }

        IdentificationNumbersEntity PIN { get; set; }

        PhoneNumberEntity PrimaryPhone { get; set; }

        List<IndividualTypeEntity> IndividualTypesBind { set; }

        List<KeyValueEntity> IndividualTypeLookup { get; set; }

        List<LookupTableEntity> States { get; set; }

        List<LookupTableEntity> Countries { get; set; }

        List<String> AssignedSiteList { get; set; }

        List<SiteCodeEntity> SiteCodes { get; set; }

        String Message { get; set; }

        String IdNumberMessage { get; set; }

        String PhoneNumberMessage { get; set; }

        String EmailMessage { get; set; }

        String AddressMessage { get; set; }

        Boolean IsAdmin { get; }

        Boolean IsProvider { get; }

        Boolean IsPatient { get; }

        Boolean IsTechnician { get; }

        // Service Data
        List<BOSEntity> BoSType { get; set; }

        List<StatusEntity> StatusType { get; set; }

        List<RankEntity> RankType { set; }

        List<TheaterLocationCodeEntity> TheaterLocationCodes { get; set; }

        string TheaterLocationCodeSelected { get; set; }

        string BOSTypeSelected { get; set; }

        string StatusTypeSelected { get; set; }

        string RankTypeSelected { get; set; }

        DateTime? EADStopDate { get; set; }

        string SiteCode { set; }

        String ServiceDataMessage { get; set; }

        // Personal Data
        string SiteSelected { get; set; }

        string FirstName { get; set; }

        string Lastname { get; set; }

        string MiddleName { get; set; }

        string Gender { get; set; }

        List<LookupTableEntity> IndividualType { get; set; }

        DateTime? DOB { get; set; }

        string Comments { get; set; }

        String PersonalDataMessage { get; set; }
    }
}