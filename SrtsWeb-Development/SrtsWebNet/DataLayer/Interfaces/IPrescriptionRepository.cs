using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IPrescriptionRepository
    {
        DataTable GetPrescriptionByPrescriptionID(int PrescriptionID, string modifiedBy);

        DataTable GetPrescriptionsByIndividualID(int individualID, string modifiedBy);

        //DataTable InsertPrescription(PrescriptionEntity _prescription);

        //DataTable UpdatePrescription(PrescriptionEntity _prescription);

        //DataTable GetPrescriptionByExamID(int examID);

        //DataTable GetActiveUnusedPrescriptionByPrescriptionID(int prescriptionID, string modifiedBy);
    }
}