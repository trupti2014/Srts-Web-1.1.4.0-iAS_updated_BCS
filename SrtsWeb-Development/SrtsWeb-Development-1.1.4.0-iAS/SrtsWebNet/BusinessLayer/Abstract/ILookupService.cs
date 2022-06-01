using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface ILookupService
    {
        List<LookupTableEntity> GetAllLookups();

        List<LookupTableEntity> GetLookupsByType(string type);

        Boolean InsertLookUpTableItem(LookupTableEntity LookUpTableItem);

        List<LookupTableEntity> UpdateLookUpTable(LookupTableEntity LookUpTableItem);
    }
}