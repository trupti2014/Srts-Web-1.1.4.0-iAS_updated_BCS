using System.ComponentModel;

namespace SrtsWeb.ExtendersHelpers
{
    public enum StatusType { CREATED, RESUBMITTED, REJECTED, REDIRECTED, CANCELLED, DISPENSED, RECEIVED, FORCEDRECEIVED, LAB_DISPENSED, LAB_RECEIVED, RETURN_TO_STOCK, HOLD_FOR_STOCK, GEYES_CREATED_ORDER, ORDER_SENT_PMRS, ORDER_SENT_MEDPROS, INCOMPLETE, FABRICATION_COMPLETE, SHIPPED_TO_PATIENT, EDIT_SUCCESSFUL, NONE, VIEW };

    public enum SiteType { CLINIC, LABORATORY, HUMANITARIAN, ADMINSTRATIVE, OTHER };

    public enum QuickSearchType { DODID, ORDERNUMBER, SSN };

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

    public enum Gender
    {
        [Description("Male")]
        M,

        [Description("Female")]
        F,

        [Description("Both")]
        B
    }

    public enum BOSCodes
    {
        [Description("Army")]
        A,

        [Description("Air Force")]
        F,

        [Description("Navy")]
        N,

        [Description("Marines")]
        M,

        [Description("Coast Guard")]
        C,

        [Description("NOAA")]
        B,

        [Description("PHS")]
        P,

        [Description("Other")]
        K
    }

    #region Grades

    public enum AirForceRanks
    {
        [Description("E01")]
        E01,

        [Description("E02")]
        E02,

        [Description("E03")]
        E03,

        [Description("E04")]
        E04,

        [Description("E05")]
        E05,

        [Description("E06")]
        E06,

        [Description("E07")]
        E07,

        [Description("E08")]
        E08,

        [Description("E09")]
        E09,

        [Description("O01")]
        O01,

        [Description("O02")]
        O02,

        [Description("O03")]
        O03,

        [Description("O04")]
        O04,

        [Description("O05")]
        O05,

        [Description("O06")]
        O06,

        [Description("O07")]
        O07,

        [Description("O08")]
        O08,

        [Description("O09")]
        O09,

        [Description("O10")]
        O10,

        [Description("O11")]
        O11,

        [Description("Dependent")]
        DEP,

        [Description("Cadet")]
        CDT,

        [Description("N/A")]
        NA
    }

    public enum ArmyRanks
    {
        [Description("E01")]
        E01,

        [Description("E02")]
        E02,

        [Description("E03")]
        E03,

        [Description("E04")]
        E04,

        [Description("E05")]
        E05,

        [Description("E06")]
        E06,

        [Description("E07")]
        E07,

        [Description("E08")]
        E08,

        [Description("E09")]
        E09,

        [Description("O01")]
        O01,

        [Description("O02")]
        O02,

        [Description("O03")]
        O03,

        [Description("O04")]
        O04,

        [Description("O05")]
        O05,

        [Description("O06")]
        O06,

        [Description("O07")]
        O07,

        [Description("O08")]
        O08,

        [Description("O09")]
        O09,

        [Description("O10")]
        O10,

        [Description("O11")]
        O11,

        [Description("W01")]
        W01,

        [Description("W02")]
        W02,

        [Description("W03")]
        W03,

        [Description("W04")]
        W04,

        [Description("W05")]
        W05,

        [Description("Cadet")]
        CDT,

        [Description("Dependent")]
        DEP,

        [Description("N/A")]
        NA
    }

    public enum MarineRanks
    {
        [Description("E01")]
        E01,

        [Description("E02")]
        E02,

        [Description("E03")]
        E03,

        [Description("E04")]
        E04,

        [Description("E05")]
        E05,

        [Description("E06")]
        E06,

        [Description("E07")]
        E07,

        [Description("E08")]
        E08,

        [Description("E09")]
        E09,

        [Description("O01")]
        O01,

        [Description("O02")]
        O02,

        [Description("O03")]
        O03,

        [Description("O04")]
        O04,

        [Description("O05")]
        O05,

        [Description("O06")]
        O06,

        [Description("O07")]
        O07,

        [Description("O08")]
        O08,

        [Description("O09")]
        O09,

        [Description("O10")]
        O10,

        [Description("O11")]
        O11,

        [Description("W01")]
        W01,

        [Description("W02")]
        W02,

        [Description("W03")]
        W03,

        [Description("W04")]
        W04,

        [Description("W05")]
        W05,

        [Description("Dependent")]
        DEP,

        [Description("N/A")]
        NA
    }

    public enum CoastGuardRanks
    {
        [Description("E01")]
        E01,

        [Description("E02")]
        E02,

        [Description("E03")]
        E03,

        [Description("E04")]
        E04,

        [Description("E05")]
        E05,

        [Description("E06")]
        E06,

        [Description("E07")]
        E07,

        [Description("E08")]
        E08,

        [Description("E09")]
        E09,

        [Description("O01")]
        O01,

        [Description("O02")]
        O02,

        [Description("O03")]
        O03,

        [Description("O04")]
        O04,

        [Description("O05")]
        O05,

        [Description("O06")]
        O06,

        [Description("O07")]
        O07,

        [Description("O08")]
        O08,

        [Description("O09")]
        O09,

        [Description("O10")]
        O10,

        [Description("O11")]
        O11,

        [Description("W01")]
        W01,

        [Description("W02")]
        W02,

        [Description("W03")]
        W03,

        [Description("W04")]
        W04,

        [Description("W05")]
        W05,

        [Description("Cadet")]
        CDT,

        [Description("Dependent")]
        DEP,

        [Description("N/A")]
        NA
    }

    public enum NavyRanks
    {
        [Description("E01")]
        E01,

        [Description("E02")]
        E02,

        [Description("E03")]
        E03,

        [Description("E04")]
        E04,

        [Description("E05")]
        E05,

        [Description("E06")]
        E06,

        [Description("E07")]
        E07,

        [Description("E08")]
        E08,

        [Description("E09")]
        E09,

        [Description("O01")]
        O01,

        [Description("O02")]
        O02,

        [Description("O03")]
        O03,

        [Description("O04")]
        O04,

        [Description("O05")]
        O05,

        [Description("O06")]
        O06,

        [Description("O07")]
        O07,

        [Description("O08")]
        O08,

        [Description("O09")]
        O09,

        [Description("O10")]
        O10,

        [Description("O11")]
        O11,

        [Description("W01")]
        W01,

        [Description("W02")]
        W02,

        [Description("W03")]
        W03,

        [Description("W04")]
        W04,

        [Description("W05")]
        W05,

        [Description("Cadet")]
        CDT,

        [Description("Dependent")]
        DEP,

        [Description("N/A")]
        NA
    }

    public enum NoaaRanks
    {
        [Description("Civilian")]
        CIV,

        [Description("Dependent")]
        DEP
    }

    public enum OtherRanks
    {
        [Description("Civilian")]
        CIV,

        [Description("Dependent")]
        DEP,

        [Description("Foreigner")]
        FOR,

        [Description("N/A")]
        NA
    }

    public enum PHSRanks
    {
        [Description("E01")]
        E01,

        [Description("E02")]
        E02,

        [Description("E03")]
        E03,

        [Description("E04")]
        E04,

        [Description("E05")]
        E05,

        [Description("E06")]
        E06,

        [Description("E07")]
        E07,

        [Description("E08")]
        E08,

        [Description("E09")]
        E09,

        [Description("O01")]
        O01,

        [Description("O02")]
        O02,

        [Description("O03")]
        O03,

        [Description("O04")]
        O04,

        [Description("O05")]
        O05,

        [Description("O06")]
        O06,

        [Description("O07")]
        O07,

        [Description("O08")]
        O08,

        [Description("O09")]
        O09,

        [Description("O10")]
        O10,

        [Description("O11")]
        O11,

        [Description("Dependent")]
        DEP,

        [Description("N/A")]
        NA
    }

    #endregion Grades

    #region Status

    public enum ArmyPrimaryStatusCodes
    {
        [Description("Active Duty")]
        A11,

        [Description("Reserve")]
        A12,

        [Description("Cadet")]
        A14,

        [Description("National Guard")]
        A15,

        [Description("ROTC")]
        A21,

        [Description("Retired")]
        A31,

        [Description("PDRL")]
        A32,

        [Description("Former POW")]
        A36
    }

    public enum ArmySecondaryStatusCodes
    {
        [Description("Dep Active Duty")]
        A41,

        [Description("Dep Retired")]
        A43
    }

    public enum NOAAPrimaryStatusCodes
    {
        [Description("Active Duty")]
        B11,

        [Description("Retired")]
        B31,

        [Description("PDRL")]
        B32,

        [Description("Former POW")]
        B36
    }

    public enum NOAASecondaryStatusCodes
    {
        [Description("Dep Active Duty")]
        B41,

        [Description("Dep Retired")]
        B43
    }

    public enum CoastGuardPrimaryStatusCodes
    {
        [Description("Active Duty")]
        C11,

        [Description("Reserve")]
        C12,

        [Description("Cadet")]
        C14,

        [Description("Retired")]
        C31,

        [Description("PDRL")]
        C32,

        [Description("Former POW")]
        C36
    }

    public enum CoastGuardSecondaryStatusCodes
    {
        [Description("Dep Active Duty")]
        C41,

        [Description("Dep Retired")]
        C43
    }

    public enum AirForcePrimaryStatusCodes
    {
        [Description("Active Duty")]
        F11,

        [Description("Reserve")]
        F12,

        [Description("Cadet")]
        F14,

        [Description("National Guard")]
        F15,

        [Description("ROTC")]
        F21,

        [Description("Retired")]
        F31,

        [Description("PDRL")]
        F32,

        [Description("Former POW")]
        F36
    }

    public enum AirForceSecondaryStatusCodes
    {
        [Description("Dep Active Duty")]
        F41,

        [Description("Dep Retired")]
        F43
    }

    public enum OtherPrimaryStatusCodes
    {
        [Description("PDRL")]
        K32,

        [Description("POW")]
        K36,

        [Description("State Dept Employee - Overseas")]
        K51,

        [Description("State Dept Dependent - Overseas")]
        K52,

        [Description("Other Fed Agency Employee")]
        K53,

        [Description("Other Fed Agency Dependent")]
        K54,

        [Description("DOD Remote Area Employee - CONUS")]
        K55,

        [Description("DOD Remote Area Dependent - CONUS")]
        K56,

        [Description("DOD Occupational Health")]
        K57,

        [Description("Other Employee and Dep(USO, RED CROSS)")]
        K59,

        [Description("VA Beneficiary")]
        K61,

        [Description("Off Workman's Comp Program (OWCP)")]
        K62,

        [Description("Service Home - Other Than Retired")]
        K63,

        [Description("Other Federal Agency")]
        K64,

        [Description("Contract Employee")]
        K65,

        [Description("Federal Prisoner")]
        K66,

        [Description("American Indian, Aleut, Eskimo")]
        K67,

        [Description("Micronesia, Somoa, Trust Territory")]
        K68,

        [Description("Other US Government Beneficiary")]
        K69,

        [Description("IMET/Sales")]
        k71,

        [Description("NATO Military")]
        K72,

        [Description("NON-NATO Military")]
        K74,

        [Description("Foreign Civilian")]
        K76,

        [Description("Foreign POW/Internee")]
        K78,

        K79,
        K81,
        K82,
        K83,
        K84,
        K91,
        K92,
        K97,
        K98,
        K99,

        [Description("DOD Civilian Employee")]
        K00
    }

    public enum MarinePrimaryStatusCodes
    {
        [Description("Active Duty")]
        M11,

        [Description("Reserve")]
        M12,

        [Description("Retired")]
        M31,

        [Description("PDRL")]
        M32,

        [Description("Former POW")]
        M36
    }

    public enum MarineSecondaryStatusCodes
    {
        [Description("Dep Active Duty")]
        M41,

        [Description("Dep Retired")]
        M43
    }

    public enum NavyPrimaryStatusCodes
    {
        [Description("Active Duty")]
        N11,

        [Description("Reserve")]
        N12,

        [Description("Cadet")]
        N14,

        [Description("ROTC")]
        N21,

        [Description("Retired")]
        N31,

        [Description("PDRL")]
        N32,

        [Description("Former POW")]
        N36
    }

    public enum NavySecondaryStatusCodes
    {
        [Description("Dep Active Duty")]
        N41,

        [Description("Dep Retired")]
        N43
    }

    public enum PHSPrimaryStatusCodes
    {
        [Description("Active Duty")]
        P11,

        [Description("Reserve")]
        P12,

        [Description("National Guard")]
        P15,

        [Description("Retired")]
        P31,

        [Description("PDRL")]
        P32,

        [Description("Former POW")]
        P36
    }

    public enum PHSSecondaryStatusCodes
    {
        [Description("Dep Active Duty")]
        P41,

        [Description("Dep Retired")]
        P43
    }

    #endregion Status

    #region Eligibility Codes

    public enum OtherEligibilityCodes
    {
        [Description("Standard Issue")]
        S,

        [Description("Frame Of Choice")]
        F
    }

    public enum MilitaryEligibilityCodes
    {
        [Description("Readiness")]
        R,

        [Description("Downed Pilot")]
        P,

        [Description("Trainee")]
        T,

        [Description("VIP")]
        V,

        [Description("Standard Issue")]
        S,

        [Description("Frame Of Choice")]
        F,

        [Description("Wounded Warrior")]
        W
    }

    public enum MilitaryOtherEligibilityCodes
    {
        [Description("Standard Issue")]
        S,

        [Description("Frame Of Choice")]
        F
    }

    #endregion Eligibility Codes

    public enum SrtsLogPriority { Low, High };

    public enum SrtsTraceSource { LoginSource, ClinicOrderSource, LabOrderSource, RxSource, ExamSource, PersonSource, ClinicManageSource, LabManageSource, AdminSource, ErrorSource, JSPECSSource };
}