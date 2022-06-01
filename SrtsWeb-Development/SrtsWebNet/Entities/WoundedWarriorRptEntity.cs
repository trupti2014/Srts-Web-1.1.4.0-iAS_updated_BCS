using System;

namespace SrtsWeb.Entities
{
    public sealed class WoundedWarriorRptEntity
    {
        public string OrderNumber { get; set; }

        public string LastFour { get; set; }

        public string LastName { get; set; }

        public string FirstInitial { get; set; }

        public string ClinicSiteCode { get; set; }

        public string LabSiteCode { get; set; }

        public string Frame { get; set; }

        public string Lens { get; set; }

        public string Material { get; set; }

        public string Tint { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime Fromdate { get; set; }
    }
}