using SrtsWeb.Entities;
using System;

namespace SrtsWeb.Views.Reporting
{
    public interface IUnitOrderCntView
    {
        SRTSSession mySession { get; set; }

        string SiteCode { get; set; }

        DateTime fromDate { get; set; }

        DateTime toDate { get; set; }
    }
}