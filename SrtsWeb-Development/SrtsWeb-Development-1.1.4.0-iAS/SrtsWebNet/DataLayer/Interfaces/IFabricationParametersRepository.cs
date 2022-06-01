using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IFabricationParametersRepository
    {
        List<FabricationParameterEntitiy> GetAllParametersBySiteCode(string siteCode);

        bool InsertParameter(FabricationParameterEntitiy param);

        bool UpdateParameter(FabricationParameterEntitiy param);

        bool DeleteParameter(int id);
    }
}
