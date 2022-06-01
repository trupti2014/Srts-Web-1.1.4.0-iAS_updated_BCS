using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface IBmtService
    {
        IEnumerable<BmtEntity> UpdateBmt(IEnumerable<BmtEntity> traineeList, String siteCode, out Int32 traineesAdded);
    }
}