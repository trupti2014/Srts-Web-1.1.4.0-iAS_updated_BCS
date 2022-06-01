using SrtsWeb.Entities;
using System;
using System.Data;

namespace SrtsWeb.Views.Reporting
{
    public interface IWWFrameDetailView
    {
        DataSet WWOrders { get; set; }

        SRTSSession mySession { get; set; }

        DateTime fromDate { get; set; }

        DateTime toDate { get; set; }
    }
}