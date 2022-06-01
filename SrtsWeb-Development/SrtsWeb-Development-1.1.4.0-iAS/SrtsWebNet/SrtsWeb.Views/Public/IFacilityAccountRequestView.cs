using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.Public
{
    public interface IFacilityAccountRequestView
    {
        List<LookupTableEntity> LookupCache { get; set; }

        string UnitName { get; set; }

        string UnitZipCode { get; set; }

        string UnitAddress1 { get; set; }

        string UnitAddress2 { get; set; }

        string UnitAddress3 { get; set; }

        string UnitCity { get; set; }

        string UnitState { get; set; }

        string UnitCountry { get; set; }

        string Comments { get; set; }

        string RequestorsName { get; set; }

        string RequestorsTitle { get; set; }

        string RequestorsWorkPhone { get; set; }

        string RequestorsDSNPhone { get; set; }

        string RequestorsFax { get; set; }

        string RequestorsEmail { get; set; }

        string UnitFacilityType { get; set; }

        string FacilityBOS { get; set; }

        string FacilityComponent { get; set; }

        List<LookupTableEntity> StateDDL { set; }

        List<LookupTableEntity> BOS_DDL { set; }

        List<LookupTableEntity> FacilityTypeDDL { set; }

        List<LookupTableEntity> CountryDDL { set; }
    }
}