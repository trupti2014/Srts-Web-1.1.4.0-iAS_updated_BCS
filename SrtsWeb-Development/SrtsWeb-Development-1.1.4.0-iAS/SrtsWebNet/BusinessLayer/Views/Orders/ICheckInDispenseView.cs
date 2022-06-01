using SrtsWeb.Entities;
using System.Data;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Views.Orders
{
    public interface ICheckInDispenseView
    {
        SRTSSession mySession { get; set; }

        //DataTable CheckInData { get; set; }
        List<ManageOrderEntity> CheckInData { get; set; }

        //DataTable DispenseData { get; set; }
        List<ManageOrderEntity> DispenseData { get; set; }

        string SelectedOrderNumbers { get; }

        string SelectedLabel { get; }

        string ReportName { get; set; }

        //string OrderNumber { get; set; }

        string Message { get; set; }
    }
}