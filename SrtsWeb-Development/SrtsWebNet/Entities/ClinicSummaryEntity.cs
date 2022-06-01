using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class ClinicSummaryEntity
    {
        public int? ReadyForCheckin { get; set; }

        public int? Rejected { get; set; }

        public int? OverDue { get; set; }

        public int? AvgDispenseDays { get; set; }
    }
}