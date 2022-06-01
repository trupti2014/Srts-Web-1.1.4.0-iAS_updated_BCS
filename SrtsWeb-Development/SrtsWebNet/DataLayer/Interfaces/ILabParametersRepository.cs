using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface ILabParametersRepository
    {
        List<LabParameterEntity> GetLabParametersBySiteCode(string siteCode);

        bool InsertUpdateLabParameter(LabParameterEntity param);

       // bool UpdateLabParameter(LabParameterEntity param);


    }
}
