using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.Orders
{
    public interface ICheckInDispenseView
    {
        SRTSSession mySession { get; set; }

        List<ManageOrderEntity> CheckInData { get; set; }

        List<ManageOrderEntity> DispenseData { get; set; }

        string SelectedLabel { get; set; }

        string ReportName { get; set; }

        string Message { get; set; }
    }
}