using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class CheckOrderStatusEntity
    {
        public String ClinicSiteCode { get; set; }
        public String FrameCombo { get { return String.Format("{0} {1}-{2}-{3}", Frame, Eye, Bridge, Temple); } } // Frame Code, Eye, Bridge, Temple
        public String OrderNumber { get; set; }
        
        public String LabSiteCode { get; set; }
        public DateTime OrderDate { get; set; }
        public String Frame { get; set; }
        public String Eye { get; set; }
        public String Bridge { get; set; }
        public String Temple { get; set; }
        public String Color { get; set; }
        public String Lens { get; set; }
        public String Tint { get; set; }
        public String CurrentStatus { get; set; }
        public DateTime CurrentStatusDate { get; set; }
    }
}
