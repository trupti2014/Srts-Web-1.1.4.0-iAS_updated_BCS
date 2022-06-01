using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface ISitePreferencesLabJustification
    {
        SRTSSession mySession { get; set; }
        List<SitePrefLabJustification> Justifications { get; set; }
        String JustificationHash { get; set; }
        String SiteCode { get; }
        //SitePrefLabJustification RejectJustification { get; set; }
        //SitePrefLabJustification RedirectJustification { get; set; }
    }
}
