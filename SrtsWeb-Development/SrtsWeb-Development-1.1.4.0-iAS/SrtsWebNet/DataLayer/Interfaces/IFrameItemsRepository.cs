using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IFrameItemsRepository
    {
        List<FrameItemEntity> GetFrameItemsByFrameCodeAndEligibility(PatientEntity patient, string frameCode);

        List<FrameItemEntity> GetFrameItemsByFrameCodeAndEligibility(string eligibility, string frameCode);

        //List<FrameItemEntity> GetFramesAndItemsByEligibility(PatientEntity patient, string clinicSiteCode);

        List<String> GetFrameItemEligibilityByFrameCode(string frameCode);

        List<FrameItemEntity> GetFrameItemByFrameCode(string frameCode);

        List<FrameItemEntity> GetFrameItems();
    }
}