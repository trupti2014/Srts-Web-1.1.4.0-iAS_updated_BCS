using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Exams
{
    public interface IExamManagementAddView
    {
        int DoctorID { get; set; }

        List<PersonnelEntity> DoctorsData { set; }

        string ExamComments { get; set; }

        DateTime ExamDate { get; set; }

        string ODCorrected { get; set; }

        string ODOSCorrected { get; set; }

        string ODOSUncorrected { get; set; }

        string ODUncorrected { get; set; }

        string OSCorrected { get; set; }

        string OSUncorrected { get; set; }

        DataTable AcuityValues { get; set; }

        List<PersonnelEntity> TechData { set; }

        int TechID { get; set; }

        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }
    }
}