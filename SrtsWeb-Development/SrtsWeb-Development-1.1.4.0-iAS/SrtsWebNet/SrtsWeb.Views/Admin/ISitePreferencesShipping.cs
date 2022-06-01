using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface ISitePreferencesShipping
    {
        string SiteCode { get; }
        string Shipper { get; set; }
        IEnumerable<LookupTableEntity> ShippingProviderList { get; set; }
    }
}
