using SrtsWeb.Entities;
using System.Data;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Views.Orders
{
    public interface IOrderProblemView
    {
        SRTSSession mySession { get; set; }

        //DataTable ProblemData { get; set; }
        List<ManageOrderEntity> ProblemData { get; set; }

        //DataTable OverdueData { get; set; }
        List<ManageOrderEntity> OverdueData { get; set; }

        //string OrderNumber { get; set; }

        //string ClinicCode { get; set; }

        //string LabCode { get; set; }

        //bool IsActive { get; set; }
    }
}