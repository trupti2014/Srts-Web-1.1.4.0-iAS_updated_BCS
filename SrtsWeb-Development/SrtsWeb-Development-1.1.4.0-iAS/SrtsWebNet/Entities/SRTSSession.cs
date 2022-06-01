using System;
using System.Collections.Generic;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class SRTSSession
    {
        public string ModifiedBy { get; set; }

        public string MyUserID { get; set; }

        public string CertificateSubject { get; set; }

        public Int32 MyIndividualID { get; set; }

        public string MyClinicCode { get; set; }

        public SiteCodeEntity MySite { get; set; }

        public int TempID { get; set; }

        public string tempOrderID { get; set; }

        public int SelectedPatientID { get; set; }

        public PatientEntity Patient { get; set; }

        public ExamEntity SelectedExam { get; set; }

        public int SelectedExamID { get; set; }

        public OrderEntity SelectedOrder { get; set; }

        public OrderEntity CreatePatientOrder { get; set; }

        public string ReturnURL { get; set; }

        public string AddOrEdit { get; set; }

        public string MainContentTitle { get; set; }

        public string CurrentModule { get; set; }

        public string CurrentModule_Sub { get; set; }

        public bool SecurityAcknowledged { get; set; }

        public int CacRegistrationCode { get; set; }

        public string SelectedNewUserName { get; set; }

        public bool SiteSelected { get; set; }

        public List<String> LogTriggers { get; set; }

        public DateTime LastLoginDate { get; set; }
    }
}