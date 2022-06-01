using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface IFabricationParametersView
    {
        List<FabricationParameterEntitiy> FabricationParameterData { get; set; }
        
        decimal Cylinder { get; set; }

        decimal MaxPlus { get; set; }

        decimal MaxMinus { get; set; }
    }
}
