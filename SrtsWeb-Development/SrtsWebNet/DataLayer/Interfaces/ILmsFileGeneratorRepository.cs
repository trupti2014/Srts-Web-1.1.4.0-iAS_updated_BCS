using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface ILmsFileGeneratorRepository
    {
        IEnumerable<LmsFileEntity> GetLmsFileData(string _siteCode);

        Boolean UpdateOrderStatus(string labCode, string orderNumber, bool isActive, string comment, string modifiedBy, int statusId);

        Boolean UpdateOrderStatus(string labCode, List<string> orderNumbers, bool isActive, string comment, string modifiedBy, int statusId);
    }
}