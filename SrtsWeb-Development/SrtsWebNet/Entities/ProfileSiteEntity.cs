using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class ProfileSiteEntity
    {
        public String SiteCode { get; set; }

        public Boolean Approved { get; set; }
    }
}