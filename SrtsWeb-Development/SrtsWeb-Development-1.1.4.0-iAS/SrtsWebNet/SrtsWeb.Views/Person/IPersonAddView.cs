using SrtsWeb.Entities;
using SrtsWeb.Views.Shared;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Person
{
    public interface IPersonAddView : IAddress, IEmailAddress, IIdNumber, IPersonnel, IPersonnelMilitaryData, IPhoneNumber
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        List<BOSEntity> BosList { get; set; }

        List<StatusEntity> StatusList { get; set; }

        List<RankEntity> RankList { set; }

        List<LookupTableEntity> IDNumberTypeList { get; set; }

        List<KeyValueEntity> IndividualTypeLookupList { get; set; }

        List<SiteCodeEntity> SiteList { get; set; }

        List<TheaterLocationCodeEntity> TheaterLocationCodeList { get; set; }

        List<IdentificationNumbersEntity> AdditionalDmdcIdList { get; set; }

        List<IndividualEntity> SearchedPersonList { set; get; }

        string TheaterLocationCode { get; set; }

        string SiteCode { get; set; }

        string Comments { get; set; }

        string ErrorMessage { get; set; }

        string NewPage { set; }

        bool IsActive { get; }

        bool IsAdmin { get; set; }

        bool IsPOC { get; set; }

        bool IsAdminType { get; }

        bool IsTechType { get; }

        bool IsProviderType { get; }

        DateTime? EADStopDate { get; set; }
    }
}