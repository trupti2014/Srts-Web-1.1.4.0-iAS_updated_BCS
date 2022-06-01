using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Reporting
{
    public interface IClinicDispenseView
    {
        DataSet ClinicDispenseData { get; set; }

        SRTSSession mySession { get; set; }
    }
}