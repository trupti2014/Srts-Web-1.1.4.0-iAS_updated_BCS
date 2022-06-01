using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Views.Orders;
using SrtsWeb.DataLayer.Repositories;

namespace SrtsWeb.BusinessLayer.Presenters.Orders
{
    public sealed class OrdersSearchPresenter
    {
        private IOrdersSearchView _view;
        //private IOrderRepository _orderRepository;

        public OrdersSearchPresenter(IOrdersSearchView view)
        {
            _view = view;
        }

        public void InitView()
        {
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

            //_orderRepository = new OrderRepository();
            //DataTable dt = _orderRepository.GetOrderByOrderNumber(_view.OrderNumber, _view.mySession.MyUserID.ToString());
            //if (dt.Rows.Count > 0)
            //{
            //    _view.TempOrderID = _view.OrderNumber;
            //    _view.Continue = true;
            //}
            //else
            //{
            //    _view.TempOrderID = "0";
            //    _view.Continue = false;
            //}
        }
    }
}