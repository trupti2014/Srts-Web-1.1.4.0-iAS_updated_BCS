using System;

namespace SrtsWeb.BusinessLayer.Views.Shared
{
    public interface IPersonnelMilitaryData
    {
        String UnitIdentificationCode { get; set; }
        String BranchOfService { get; set; }
        String StatusCategory { get; set; }
        String Grade { get; set; }
    }
}
