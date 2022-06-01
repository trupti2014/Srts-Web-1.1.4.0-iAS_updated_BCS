using System;

namespace SrtsWeb.Views.Shared
{
    public interface IPersonnelMilitaryData
    {
        String BranchOfService { get; set; }

        String StatusCategory { get; set; }

        String Grade { get; set; }
    }
}