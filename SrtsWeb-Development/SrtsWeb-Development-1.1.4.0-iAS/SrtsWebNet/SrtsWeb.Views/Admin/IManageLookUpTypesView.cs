using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface IManageLookUpTypesView
    {
        SRTSSession mySession { get; set; }

        Dictionary<string, string> LookupTypes { get; set; }

        string SelectedType { get; set; }

        List<LookupTableEntity> LookupsBind { get; set; }

        string CodeInput { get; set; }

        string TextInput { get; set; }

        string ValueInput { get; set; }

        string DescriptionInput { get; set; }

        bool IsActiveInput { get; set; }

        int IDInput { get; set; }

        List<LookupTableEntity> CacheData { get; set; }
    }
}