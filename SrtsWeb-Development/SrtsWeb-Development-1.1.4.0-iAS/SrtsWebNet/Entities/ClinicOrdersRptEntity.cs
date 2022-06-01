using System;

namespace SrtsWeb.Entities
{
    public sealed class ClinicOrdersRptEntity
    {
        public string ClinicSiteCode { get; set; }

        public string LabSiteCode { get; set; }

        public string OrderNumber { get; set; }

        public string FrameCode { get; set; }

        public int isFOC { get; set; }

        public string LensType { get; set; }

        public string Tint { get; set; }

        public string IndividualID_Patient { get; set; }

        public int Pairs { get; set; }

        public string StatusCode { get; set; }

        public string PriorityCode { get; set; }

        public string bos { get; set; }

        public string MilRank { get; set; }

        public string LastName { get; set; }

        public string FI { get; set; }

        public string SiteName { get; set; }

        public string IDNbr { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateReceivedByLab { get; set; }

        public DateTime DateProductionCompleted { get; set; }

        public DateTime DateLabDispensed { get; set; }

        public DateTime DateClinicReceived { get; set; }

        public DateTime DateClinicDispensed { get; set; }

        public string PkgID { get; set; }

        public DateTime fromDate { get; set; }

        public DateTime toDate { get; set; }

        public string StatusRpt { get; set; }

        public string PriorityRpt { get; set; }
    }
}