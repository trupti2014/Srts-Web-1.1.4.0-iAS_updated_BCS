using SrtsWeb.Views.Reporting;
using SrtsWeb.DataLayer.Repositories;
using System;
using System.Data;

namespace SrtsWeb.BusinessLayer.Presenters.Reporting
{
    public sealed class RptWWFrameDetailPresenter
    {
        //private IOrderRepository orderRepository;
        private IWWFrameDetailView _view;
        public DateTime fromDate;
        public DateTime toDate;
        public DateTime curDate;
        private int _year;
        private int _month;
        private int _days;

        public RptWWFrameDetailPresenter(IWWFrameDetailView view)
        {
            _view = view;
        }

        public void InitView()
        {
            DateTime curDate = DateTime.Now;
            _year = curDate.Year;
            _month = curDate.Month;
            _days = DateTime.DaysInMonth(_year, _month - 1);

            DateTime _fromDate = new DateTime(_year, _month - 1, 1);
            DateTime _toDate = new DateTime(_year, _month - 1, _days);

            fromDate = _fromDate;
            toDate = _toDate;

            GetWWReportData();
        }

        public void GetWWReportData()
        {
            DataSet ds = new DataSet();

            var orderRepository = new OrderRepository();
            DataTable dt = orderRepository.GetWoundedWarriorReportData(fromDate, toDate);
            if (dt.Rows.Count > 0)
            {
                dt.TableName = "WWOrderData";
                ds.Tables.Add(dt);
                _view.WWOrders = ds;
            }
        }
    }
}