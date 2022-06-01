using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Views.Orders;
using SrtsWeb.DataLayer.Repositories;

namespace SrtsWeb.Presenters.Orders
{
    public sealed class OrdersSearchPresenter
    {
        private IOrdersSearchView _view;

        public OrdersSearchPresenter(IOrdersSearchView view)
        {
            _view = view;
        }

        public void CheckForOrder()
        {
            var os = new OrdersService();
            var good = os.CheckForOrder(_view.OrderNumber, _view.mySession.MyUserID);
            _view.Continue = good;
            if (good)
                _view.TempOrderID = _view.OrderNumber;
            else
                _view.TempOrderID = "0";
        }
    }
}