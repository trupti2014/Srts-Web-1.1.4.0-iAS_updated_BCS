using SrtsWeb.Entities;
using System.Data;

namespace SrtsWeb.Views.Reporting
{
    public interface ITurnAroundTimeView
    {
        DataSet TurnAroundData { get; set; }

        SRTSSession mySession { get; set; }
    }
}