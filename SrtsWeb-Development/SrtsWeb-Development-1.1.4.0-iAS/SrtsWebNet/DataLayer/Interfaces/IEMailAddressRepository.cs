using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IEMailAddressRepository
    {
        List<EMailAddressEntity> GetEmailAddressesByIndividualID(int individualID, string modifiedBy);

        List<EMailAddressEntity> InsertEMailAddress(EMailAddressEntity addr);

        List<EMailAddressEntity> UpdateEMailAddress(EMailAddressEntity addr);
    }
}