using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IFrameRepository
    {
        List<FrameEntity> GetAllFrames();

        Boolean InsertFrame(FrameEntity frame);

        Boolean UpdateFrame(FrameEntity frame);

        void InsertFrameItemsEligibilityAndUnion(string frameCode, List<FrameItemEntity> frameItem, List<OrderPriorityEntity> eligibility, string modifiedBy);

        void InsertFrameEligibilityDemographic(string frameCode, List<string> eligibilities, string modifiedBy);

        List<String> GetEligibilityByFrameCode(string frameCode);

        List<String> GetFrameItemPrioritiesByFrameCode(string frameCode);

        //DataTable GetFrameEligibilityParts();
    }

    public interface IFrameRxRestrictions
    {
        FrameRxRestrictionsEntity GetFrameRxRestrictionsByFrameCode(string frameCode);
    }
}