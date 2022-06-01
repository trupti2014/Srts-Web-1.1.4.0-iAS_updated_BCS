using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IAddressRepository
    {
        List<AddressEntity> GetAddressesByIndividualID(int individualID, string modifiedBy);

        List<AddressEntity> InsertAddress(AddressEntity addr);

        List<AddressEntity> UpdateAddress(AddressEntity addr);

        //DataTable GetUnitByUIC1(string uic);

        //DataTable GetAllUnits();
    }
}