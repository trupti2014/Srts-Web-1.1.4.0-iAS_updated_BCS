using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Reporting
{
    public interface ISiteCodeView
    {
        DataSet SiteCodeData { get; set; }

        SRTSSession mySession { get; set; }
    }
}