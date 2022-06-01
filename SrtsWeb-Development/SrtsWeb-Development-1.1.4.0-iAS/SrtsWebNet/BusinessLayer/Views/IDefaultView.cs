using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views
{
    public interface IDefaultView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        string AddressDisplay { get; set; }

        string SitePhone { get; set; }

        string SiteCode { get; set; }

        int? ReadyForCheckin { get; set; }

        int? Rejected { get; set; }

        int? OverDue { get; set; }

        int? AvgDispenseDays { get; set; }

        int? ReadyForDispense { get; set; }

        int? ReadyForLabCheckin { get; set; }

        int? ReadyForLabDispense { get; set; }

        int? AvgProductionDays { get; set; }

        String AnnouncementLinks { set; }
    }
}