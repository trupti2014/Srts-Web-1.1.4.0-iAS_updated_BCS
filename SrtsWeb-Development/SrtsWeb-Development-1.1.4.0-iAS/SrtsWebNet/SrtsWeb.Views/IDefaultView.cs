using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views
{
    public interface IDefaultView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        int? Pending { get; set; }

        int? ReadyForCheckin { get; set; }

        int? Rejected { get; set; }

        int? OverDue { get; set; }

        int? AvgDispenseDays { get; set; }

        int? ReadyForDispense { get; set; }

        int? ReadyForLabCheckin { get; set; }

        int? ReadyForLabDispense { get; set; }

        int? AvgProductionDays { get; set; }

        int? HoldForStockOrders { get; set; }

        String AnnouncementLinks { set; }
    }
}