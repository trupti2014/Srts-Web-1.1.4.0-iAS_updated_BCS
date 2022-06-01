using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface IFrameItemsPreferencesView
    {
        SRTSSession mySession { get; set; }

        String Frame { get; set; }
        IEnumerable<FrameEntity> FrameList { get; set; }

        String Color { get; set; }
        Dictionary<String, String> ColorList { get; set; }

        String Eye { get; set; }
        Dictionary<String, String> EyeList { get; set; }

        String Bridge { get; set; }
        Dictionary<String, String> BridgeList { get; set; }

        String Temple { get; set; }
        Dictionary<String, String> TempleList { get; set; }

        String Lens { get; set; }
        Dictionary<String, String> LensList { get; set; }

        String Tint { get; set; }
        Dictionary<String, String> TintList { get; set; }

        String Coating { get; set; }
        Dictionary<String, String> CoatingList { get; set; }

        String Material { get; set; }
        Dictionary<String, String> MaterialList { get; set; }

        String OdSegHt { get; set; }
        String OsSegHt { get; set; }

        FrameItemDefaultEntity FrameItemDefault { get; set; }

        SitePrefFrameItemEntity FrameItemPreference { get; }
        IEnumerable<SitePrefFrameItemEntity> FrameItemPreferenceList { get; set; }

        String SiteCode { get; }
    }
}