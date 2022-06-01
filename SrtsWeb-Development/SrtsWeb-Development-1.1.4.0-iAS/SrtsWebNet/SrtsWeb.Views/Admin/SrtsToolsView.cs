using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface ISrtsToolsView
    {
        SRTSSession mySession { get; set; }
    }
}