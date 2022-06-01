using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface ISitePreferencesRoutingOrdersView
    {
        SRTSSession mySession { get; }

        List<SitePrefRoutingOrdersEntity> LabCapacityHistoryData { get; set; }

        SitePrefRoutingOrdersEntity SitePrefRoutingOrdersEntity { get; set; }

        String SiteCode { get; set; }

        IEnumerable<SiteCodeEntity> SiteCodes { get; }

        Int32 Capacity { get; set; }

        bool PDO { get; set; }
    }
}

