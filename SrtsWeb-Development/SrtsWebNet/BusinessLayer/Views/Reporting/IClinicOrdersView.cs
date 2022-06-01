using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Reporting
{
    public interface IClinicOrdersView
    {
        DataSet ClinicOrdersData { get; set; }

        SRTSSession mySession { get; set; }
    }
}