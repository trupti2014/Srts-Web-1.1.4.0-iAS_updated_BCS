using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IIdentificationNumbersRepository
    {
        List<IdentificationNumbersEntity> GetIdentificationNumbersByIndividualID(int individualID, string modifiedBy);

        //DataTable GetIdentificationNumberByID(int idNumber);

        List<IdentificationNumbersEntity> GetIdentificationNumberByIDNumber(string idNumber, string idNumberType, string modifiedBy);

        IdentificationNumbersEntity InsertIdentificationNumbers(IdentificationNumbersEntity ine);

        List<IdentificationNumbersEntity> UpdateIdentificationNumbersByID(IdentificationNumbersEntity ine);
    }
}