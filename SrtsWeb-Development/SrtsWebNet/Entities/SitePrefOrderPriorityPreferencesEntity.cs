using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class SitePrefOrderPriorityPreferencesEntity
    {
        public String FrameCode { get; set; }
        public String FrameDescription { get; set; }

        public String PriorityCode { get; set; }
        public String PriorityDescription { get; set; }
    }
}
