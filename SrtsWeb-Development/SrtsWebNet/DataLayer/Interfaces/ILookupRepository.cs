using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface ILookupRepository
    {
        List<LookupTableEntity> GetAllLooksups();

        List<LookupTableEntity> GetLookupsByType(string lookupType);

        Boolean InsertLookUpTableItem(LookupTableEntity lte);

        List<LookupTableEntity> UpdateLookUpTable(LookupTableEntity lte);
    }
}