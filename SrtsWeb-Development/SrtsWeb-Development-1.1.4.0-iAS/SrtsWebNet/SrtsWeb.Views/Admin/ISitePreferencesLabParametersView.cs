using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface ISitePreferencesLabParametersView
    {
        SRTSSession mySession { get; set; }

        string SiteCode { get; }

        string ErrMessage { get; set; }

        List<FabricationParameterEntitiy> FabricationParameterData { get; set; }

        decimal Cylinder { get; set; }

        decimal MaxPlus { get; set; }

        decimal MaxMinus { get; set; }

        string Material { get; set; }

        Dictionary<String, String> LensMaterial { set; }

        string IsStocked { get; set; }

        int MatParamID { get; set; }

        string CapabilityType { get; set; }

        decimal MaxPrism { get; set; }

        decimal MaxDecentrationPlus { get; set; }

        decimal MaxDecentrationMinus { get; set; }

   
    }
}