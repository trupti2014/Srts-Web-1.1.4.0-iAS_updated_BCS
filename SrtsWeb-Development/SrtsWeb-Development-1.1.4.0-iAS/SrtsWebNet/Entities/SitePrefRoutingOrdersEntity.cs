using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    public class SitePrefRoutingOrdersEntity
    {
        public String SiteCode { get; set; }
        public int Capacity { get; set; }
        public bool PDO { get; set; }

        public DateTime DateLastModified { get; set; }
        public String ModifiedBy { get; set; }


    }
}
