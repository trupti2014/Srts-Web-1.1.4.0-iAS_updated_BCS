using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.Orders
{
    public interface IOrderProblemView
    {
        SRTSSession mySession { get; set; }

        List<ManageOrderEntity> ProblemData { get; set; }

        List<ManageOrderEntity> OverdueData { get; set; }

        string ClinicCode { get; set; }

        string LabCode { get; set; }

        bool IsActive { get; set; }
    }
}