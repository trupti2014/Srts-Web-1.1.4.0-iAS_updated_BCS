using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IPhoneRepository
    {
        List<PhoneNumberEntity> GetPhoneNumbersByIndividualID(int individualID, string modifiedBy);

        //DataTable GetPhoneNumberByPhoneID(int phoneID);

        List<PhoneNumberEntity> InsertPhoneNumber(PhoneNumberEntity pne);

        List<PhoneNumberEntity> UpdatePhoneNumber(PhoneNumberEntity pne);
    }
}