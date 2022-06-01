using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IIndividualRepository
    {
        Int32 InsertIndividualSiteCode(Int32 individualId, String siteCode);

        List<IndividualEntity> UpdateIndividual(IndividualEntity individual);

        void SyncUserToIndividual(String userName, Int32 individualId, Boolean forRemove = false);

        List<IndividualEntity> FindIndividualByLastnameOrLastFour(string IDNumber, string lastName, string firstName, string siteCode, string personalType, bool isActive, string modifiedBy, out int RecCnt);

        List<IndividualEntity> GetIndividualByIDNumberIDNumberType(String IDNumber, String IDNumberType, String modifiedBy);

        List<IndividualEntity> GetIndividualByIndividualID(int individualID, string modifiedBy);

        List<IndividualEntity> GetIndividualBySiteCodeAndPersonalType(string siteCode, string personalType, string modifiedBy);

        List<IndividualEntity> GetAllIndividualsAtSite(String siteCode, string modifiedBy);

        Boolean GetSyncedUser(Int32 individualId);

        Int32 GetIndividualIdByUserName(String userName);
    }

    public interface IPatientRepository
    {
        PatientEntity GetAllPatientInfoByIDNumber(string _IDNumber, string _IDType, string _clinicCode);

        PatientEntity GetAllPatientInfoByIndividualID(int individualID, bool isActive, string modifiedBy, string usersSiteCode);
    }
}