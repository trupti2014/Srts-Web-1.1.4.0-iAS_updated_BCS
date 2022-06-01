using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.Views.Individuals
{
    public interface IIndividualManagementAddView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        List<BOSEntity> BoSType { get; set; }

        List<StatusEntity> StatusType { get; set; }

        List<LookupTableEntity> IDNumberType { get; set; }

        //DataTable IndividualType { get; set; }

        List<IndividualTypeEntity> IndividualTypesBind { set; }

        List<KeyValueEntity> IndividualTypeLookup { get; set; }

        bool IsAdmin { get; }

        bool IsProvider { get; }

        //bool IsPatient { get; }

        bool IsTechnician { get; }

        //List<OrderPriorityEntity> OrderPriority { get; set; }

        List<RankEntity> RankType { get; set; }

        List<SiteCodeEntity> Sites { get; set; }

        List<TheaterLocationCodeEntity> TheaterLocationCodes { get; set; }

        string TheaterLocationCodeSelected { get; set; }

        string BOSTypeSelected { get; set; }

        string StatusTypeSelected { get; set; }

        string IDNumberTypeSelected { get; set; }

        //string IndividualTypeSelected { get; set; }

        //string OrderPrioritySelected { get; set; }

        string RankTypeSelected { get; set; }

        string SiteSelected { get; set; }

        string FirstName { get; set; }

        string Lastname { get; set; }

        string MiddleName { get; set; }

        string Comments { get; set; }

        string Gender { get; set; }

        bool IsActive { get; set; }

        bool IsPOC { get; set; }

        string IDNumber { get; set; }

        List<IdentificationNumbersEntity> AdditionalDmdcIds { get; set; }

        DateTime? EADStopDate { get; set; }

        DateTime? DOB { get; set; }

        string ErrorMessage { set; }

        string NewPage { set; }

        List<IndividualEntity> SearchedIndividuals { set; }
    }
}