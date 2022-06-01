using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Patients
{
    public interface IPatientManagementAddView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        List<BOSEntity> BoSType { get; set; }

        List<StatusEntity> StatusType { get; set; }

        List<LookupTableEntity> IDNumberType { get; set; }

        List<LookupTableEntity> IndividualType { get; set; }

        List<KeyValueEntity> IndividualTypeLookup { get; set; }

        //List<OrderPriorityEntity> OrderPriority { get; set; }

        List<RankEntity> RankType { set; }

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

        List<IndividualEntity> SearchedPatients { set; get; }
    }
}