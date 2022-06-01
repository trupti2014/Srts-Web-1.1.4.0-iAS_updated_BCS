using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.Views.Reporting
{
    public interface IClinicOrdersView
    {
        DataSet ClinicOrdersData { get; set; }

        SRTSSession mySession { get; set; }
    }
}