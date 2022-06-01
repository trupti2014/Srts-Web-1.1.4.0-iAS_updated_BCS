using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IIndividualTypeRepository
    {
        List<IndividualTypeEntity> GetIndividualTypesByIndividualId(Int32 individualId);

        bool InsertIndividualTypes(int IndividualId, string ModifiedBy, string IndTypes, bool DeleteAll);

        List<IndividualTypeEntity> UpdateIndividualType(IndividualTypeEntity type);

        //DataTable InsertIndividualType(IndividualTypeEntity type);
        //bool InsertIndividualTypes(Int32 IndividualId, string ModifiedBy, string IndTypes, bool DeleteAll);
    }
}