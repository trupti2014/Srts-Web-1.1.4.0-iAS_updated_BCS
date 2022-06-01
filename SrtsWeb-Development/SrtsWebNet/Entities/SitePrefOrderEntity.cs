using System;
using System.Collections.Generic;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class SitePrefOrderEntity
    {
        public List<SitePrefOrderPriorityPreferencesEntity> PriorityFrameCombo { get; set; }

        public String DistributionMethod { get; set; }

        public String Lab { get; set; }

        public String InitialLoadPriority { get; set; }

        public String InitialLoadFrame { get; set; }

        public Boolean AlphaSortLabels { get; set; }

        public String SiteCode { get; set; }

        public SitePrefOrderEntity Clone() { return this.MemberwiseClone() as SitePrefOrderEntity; }
    }
}