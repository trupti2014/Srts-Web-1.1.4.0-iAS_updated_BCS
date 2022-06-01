using SrtsWeb.ExtendersHelpers;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.Views.JSpecs;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Diagnostics;

namespace SrtsWeb.Presenters.JSpecs
{
    public sealed class JSpecsOrdersPresenter
    {
        private IJSpecsOrdersView _view;

        public JSpecsOrdersPresenter(IJSpecsOrdersView view)
        {
            _view = view;
        }

        public void InitView()
        {
            GetUsersOrdersAndStatus();
        }

        /// <summary>
        /// Retrieve the orders for the user currently logged in.
        /// </summary>
        private void GetUsersOrdersAndStatus()
        {
            OrderRepository.JSpecsOrderRepository _orderRepository = new OrderRepository.JSpecsOrderRepository();
            OrdersService ordersService = new OrdersService();

            if (_view.userInfo.Patient.Individual != null)
            {
                List<JSpecsOrderDisplayEntity> orders = _orderRepository.GetOrdersByIndividualID(_view.userInfo.Patient.Individual.ID, HttpContext.Current.User.Identity.Name, "008094");
                orders.ForEach(o => o.OrderStatus = ordersService.GetLatestStatus(o.OrderNumber).ToString());

                _view.OrdersData = orders;
            }
            else
            {
                throw new Exception("Patient Individual Id is empty in JSpecsOrderPresenter.cs at line 45.");
            }
        }

        /// <summary> 
        /// Get users display order.
        /// </summary>
        /// <param name="orderNumber">A String identifying an orderNumber</param>
        /// <returns></returns>
        public JSpecsOrderDetailsDisplayEntity GetUsersDisplayOrder(string orderNumber)
        {
            var _orderRepository = new OrderRepository.JSpecsOrderDetailsRepository();
            OrdersService ordersService = new OrdersService();

            JSpecsOrderDetailsDisplayEntity order = _orderRepository.GetOrderByOrderNumber(orderNumber, HttpContext.Current.User.Identity.Name);
            order.OrderStatus = ordersService.GetOrderStatusByTypeID(order.OrderStatusTypeID).ToString();
            if(order == null)
            {
                return null;
            }
            else
            {
                return order;
            }
            
        }
    }
}
