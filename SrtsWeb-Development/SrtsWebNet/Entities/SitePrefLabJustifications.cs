using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class SitePrefLabJustification
    {
        public String SiteCode { get; set; }
        public String Justification { get; set; }
        public String JustificationReason { get; set; }
    }
}
