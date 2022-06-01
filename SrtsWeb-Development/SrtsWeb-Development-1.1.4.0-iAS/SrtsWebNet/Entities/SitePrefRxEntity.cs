using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class SitePrefRxEntity
    {
        public String SiteCode { get; set; }
        public String RxType { get; set; }
        public int ? ProviderId { get; set; }
        public Decimal ? PDDistance { get; set; }
        public Decimal ? PDNear { get; set; }
        public String ModifiedBy { get; set; }
    }
}