using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.Orders
{
    public sealed class OrderTrackingPresenter
    {
        private IOrderTrackingView _view;

        private OrderTrackingRepository OrderTrackingRepository;
        private OrderTrackingScanRepository OrderTrackingScanRepository;
        private OrderTrackingOrderRepository OrderTrackingOrderRepository;
        private OrderTrackingPatientRepository OrderTrackingPatientRepository;

        public OrderTrackingPresenter(IOrderTrackingView view)
        {
            _view = view;
        }

        #region GET FUNCTION

        /// <summary>
        /// get initial table
        /// </summary>
        /// <param name="SiteCode"></param>
        public void GetTrackedOrders(string SiteCode)
        {
            OrderTrackingRepository = new OrderTrackingRepository();
            this._view.TrackingBaseList = OrderTrackingRepository.GetTrackedOrders(SiteCode);
        }

        /// <summary>
        /// get edit table of patient tracked orders
        /// </summary>
        /// <param name="PatientID"></param>
        /// <param name="SiteCode"></param>
        public void GetTrackedOrders(int PatientID, string SiteCode)
        {
            OrderTrackingPatientRepository = new OrderTrackingPatientRepository();
            this._view.TrackingOrderPatientList = OrderTrackingPatientRepository.GetTrackingNumbersPatientSite(PatientID, SiteCode);
        }

        /// <summary>
        /// get scan items
        /// </summary>
        /// <param name="TrackingNumber"></param>
        public void GetTrackingOrderScan(string TrackingNumber)
        {
            OrderTrackingScanRepository = new OrderTrackingScanRepository();
            this._view.TrackingScanList = OrderTrackingScanRepository.GetTrackingNumberOrders(TrackingNumber);
        }

        /// <summary>
        /// get Patient
        /// </summary>
        /// <param name="OrderNumber"></param>
        public void GetPatientID(string OrderNumber)
        {
            OrderTrackingOrderRepository = new OrderTrackingOrderRepository();
            this._view.PatientItem = OrderTrackingOrderRepository.GetPatientID(OrderNumber);
        }

        #endregion

        #region UPDATE FUNCTION
        public String InsertUpdateTrackingNumber(OrderTrackingScanEntity ScanEntity, string OrderNumber, string Shipper, string What)
        {
            OrderTrackingScanRepository = new OrderTrackingScanRepository();

            return OrderTrackingScanRepository.IUTrackingNumber(ScanEntity, OrderNumber, Shipper, What);

        }

        public String InsertTrackingNumberManual(OrderTrackingScanEntity ScanEntity, string OrderNumber, string Shipper)
        {
            OrderTrackingScanRepository = new OrderTrackingScanRepository();

            return OrderTrackingScanRepository.ITrackingNumberManual(ScanEntity, OrderNumber, Shipper);

        }



        #endregion


    }
}
