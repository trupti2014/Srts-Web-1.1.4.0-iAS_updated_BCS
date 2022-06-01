using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface ISiteAdministratorsView
    {
        IEnumerable<SiteCodeEntity> SiteCodes { get; set; }
        String SiteCodeDDL { get; set; }

        String SiteCodeTxtBox { get; set; }
    }
}
