using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Admin
{
    public interface IFrameManagement
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        List<OrderPriorityEntity> PriorityData { get; set; }
        List<string> PrioritiesSelected { get; set; }

        string GenderSelected { get; set; }

        /// <summary>
        /// This is the collection of frame data that once one is selected can be set to the FrameData property and the page will load those frame items
        /// </summary>
        List<FrameEntity> FrameData { get; set; }
        
        /// <summary>
        /// This is the single entity that will be used to populate the individual frame info controls
        /// </summary>
        FrameEntity FrameInfo { get; set; }
        
        /// <summary>
        /// Master list used by all the item data lists
        /// </summary>
        List<FrameItemEntity> FrameItemInfo { get; set; }

        Dictionary<string, string> TempleData { get; set; }
        Dictionary<string, string> BridgeData { get; set; }
        Dictionary<string, string> ColorData { get; set; }
        Dictionary<string, string> EyeSizeData { get; set; }
        Dictionary<string, string> LenseTypeData { get; set; }
        Dictionary<string, string> MaterialData { get; set; }
        Dictionary<string, string> TintData { get; set; }

        /// <summary>
        /// These are the selected id/codes of the individual frame items
        /// </summary>
        List<string> TemplesSelected { get; set; }
        List<string> BridgesSelected { get; set; }
        List<string> ColorsSelected { get; set; }
        List<string> EyeSizesSelected { get; set; }
        List<string> LensTypesSelected { get; set; }
        List<string> MaterialsSelected { get; set; }
        List<string> TintsSelected { get; set; }

        string FrameCodeSelected { get; set; }

        List<string> Eligibilities { get; set; }
    }
}