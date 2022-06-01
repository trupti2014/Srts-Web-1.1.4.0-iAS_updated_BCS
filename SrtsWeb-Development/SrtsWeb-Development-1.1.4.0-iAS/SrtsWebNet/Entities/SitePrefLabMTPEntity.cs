using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class SitePrefLabMTPEntity
    {
        public String SiteCode { get; set; }
        public String ClinicSiteCode { get; set; }

        public String ClinicActionRequired { get; set; }

        public List<string> ListofClinicActionRequired { get; set; }

        public string IsLabMailToPatient { get; set; }
        public string IsCapabilityOn { get; set; }
        public string StatusReason { get; set; }
        public int Capacity { get; set; }
        public String ModifiedBy { get; set; }
        public String Comments { get; set; }
        public DateTime? DateLastModified { get; set; }
        public DateTime? StopDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? AnticipatedRestartDate { get; set; }
        }
}