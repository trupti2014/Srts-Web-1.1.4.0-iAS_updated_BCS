using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IIndividualPrescriptionRepository
    {
        List<PrescriptionEntity> GetPrescriptionsByIndividualID(int individualID, string modifiedBy);
    }
}