using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Reporting
{
    public interface IOrder771View
    {
        DataSet Order771 { get; set; }

        SRTSSession mySession { get; set; }
    }
}