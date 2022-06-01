using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface IPrescriptionPreferencesView
    {
        SRTSSession mySession { get; }
        
        List<IndividualEntity> ProviderList { get; set; }

        SitePrefRxEntity SitePrefsRX { get; set; }

        Decimal ? PDDistance { get; set; }

        Decimal ? PDNear { get; set; }

        int ? Provider { get; set; }

        String RxType { get; set; }

        String SiteCode { get; }
    }
}
