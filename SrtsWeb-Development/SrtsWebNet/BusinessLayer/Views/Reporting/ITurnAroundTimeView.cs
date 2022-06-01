using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Reporting
{
    public interface ITurnAroundTimeView
    {
        DataSet TurnAroundData { get; set; }

        SRTSSession mySession { get; set; }
    }
}