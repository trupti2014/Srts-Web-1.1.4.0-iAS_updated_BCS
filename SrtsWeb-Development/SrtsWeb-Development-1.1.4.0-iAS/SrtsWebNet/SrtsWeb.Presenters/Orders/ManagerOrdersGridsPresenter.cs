using System.Collections.Generic;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Views.Orders;
using SrtsWeb.Entities;

namespace SrtsWeb.Presenters.Orders
{
    public sealed class ManagerOrdersGridsPresenter
    {
        private IOrderProblemView _view;

        public ManagerOrdersGridsPresenter(IOrderProblemView view)
        {
            _view = view;
        }

        public static List<ManageOrderEntity> GetOverdueOrders(string siteCode, string myUserId)
        {
            var _repository = new OrderStateRepository.OrdersOfStatusRepository();
            return _repository.GetOrdersForOverDueDispenseByClinicCode(siteCode, myUserId);
        }

        public static List<ManageOrderEntity> GetProblemOrders(string siteCode, string myUserId)
        {
            var _repository = new OrderStateRepository.OrdersOfStatusRepository();
            return _repository.GetOrdersWithProblemsByClinicCode(siteCode, myUserId);
        }

        public static List<ManageOrderEntity> GetPendingOrders(string siteCode, string myUserId)
        {
            var _repository = new OrderStateRepository.OrdersOfStatusRepository();
            return _repository.GetOrdersPendingByClinicCode(siteCode, myUserId);
        }
    }
}