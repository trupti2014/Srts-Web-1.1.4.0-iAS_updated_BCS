using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class SitePrefFrameItemEntity
    {
        public String SiteInfo { get; set; }
        public String Frame { get; set; }
        public String Color { get; set; }
        public String Eye { get; set; }
        public String Bridge { get; set; }
        public String Temple { get; set; }
        public String Lens { get; set; }
        public String Tint { get; set; }
      //  public String Coating { get; set; }
        public String Material { get; set; }
        public String Coatings { get; set; }
        public String OdSegHt { get; set; }
        public String OsSegHt { get; set; }

        public SitePrefFrameItemEntity Clone() { return this.MemberwiseClone() as SitePrefFrameItemEntity; }
    }
}
