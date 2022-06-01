using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.DataLayer.Repositories
{
    public interface IExamRepository
    {
        DataTable GetExamByExamID(int examID, string modifiedBy);

        DataTable GetExamsByIndividualID(int individualID, string modifiedBy);

        DataTable InsertExam(ExamEntity exam);

        DataTable UpdateExam(ExamEntity exam);
    }
}