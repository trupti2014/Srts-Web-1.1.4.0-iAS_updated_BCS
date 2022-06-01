using SrtsWeb.Entities;

namespace SrtsWeb.Views.Orders
{
    public interface IOrdersSearchView
    {
        SRTSSession mySession { get; set; }

        string OrderNumber { get; set; }

        bool Continue { get; set; }

        string TempOrderID { get; set; }
    }
}