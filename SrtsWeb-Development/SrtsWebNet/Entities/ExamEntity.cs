using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class ExamEntity
    {
        public int ID { get; set; }

        public int IndividualID_Patient { get; set; }

        public int AddressID_Patient { get; set; }

        public int IndividualID_Examiner { get; set; }

        public string ODCorrectedAcuity { get; set; }

        public string ODUncorrectedAcuity { get; set; }

        public string OSCorrectedAcuity { get; set; }

        public string OSUncorrectedAcuity { get; set; }

        public string ODOSCorrectedAcuity { get; set; }

        public string ODOSUncorrectedAcuity { get; set; }

        public string Comments { get; set; }

        public DateTime ExamDate { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }
    }
}