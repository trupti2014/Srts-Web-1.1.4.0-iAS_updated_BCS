            using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class SitePrefClinicGroupsEntity
    {
        public String ClinicSite { get; set; }
        public String GroupName { get; set; }
        public String GroupDesc { get; set; }
        public Boolean IsActive { get; set; }
        public List<SitePrefClinicGroupsEntity> ClinicGroups { get; set; }
    }
}
