using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface ISitePreferencesGeneral
    {
        Boolean LabelSortedAlpha { get; set; }
        String SiteCode { get; }
        IEnumerable<SiteCodeEntity> SiteCodes { get; }
    }
}
