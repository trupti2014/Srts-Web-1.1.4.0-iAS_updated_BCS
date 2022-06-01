using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace SrtsWeb.BusinessLayer.Concrete
{
    public sealed class SrtsWsService
    {
        public DataSet GetMedProsFirstPass(String strSsn)
        {
            IOrderRepository _repository = new OrderRepository();

            return _repository.GetMedProsFirstPass(strSsn);
        }

        public DataSet GetMedProsOrderByOrderNumber(string orderId, bool updateDispensedFlag)
        {
            IOrderRepository _repository = new OrderRepository();

            var ds = _repository.GetMedProsOrderByOrderNumber(orderId);

            if (ds == null || ds.Tables.Count.Equals(0)) return null;

            if (updateDispensedFlag)
                _repository.UpdateMedprosDispensedFlag(orderId);

            return ds;
        }

        public DataSet PimrsGetLatestOrders()
        {
            IOrderRepository _repository = new OrderRepository();

            var ds = _repository.PimrGetLatestOrders();

            if (ds == null || ds.Tables.Count.Equals(0)) return null;

            return ds;
        }

        public DataSet PimrsGetOrdersByDates(DateTime dtStartDtg, DateTime dtEndDtg)
        {
            IOrderRepository _repository = new OrderRepository();

            return _repository.PimrGetOrdersByDates(dtStartDtg, dtEndDtg);
        }

        public DataSet GetOrdersForLabBySiteCode(string siteCode, string modifiedBy, bool updateStatusToReceived)
        {
            IOrderRepository _repository = new OrderRepository();

            var ds = _repository.GetOrdersForLabBySiteCode(siteCode);

            if (updateStatusToReceived)

                try
                {
                    var orders = new List<String>();

                    foreach (DataTable dt in ds.Tables)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            orders.Add(dr["ordernumber"].ToString());
                        }
                    }

                    foreach (var o in orders)
                        UpdateOrderStatus(o, siteCode, true, String.Empty, modifiedBy, StatusType.RECEIVED);
                }
                catch { }

            return ds;
        }

        public String GetXmlOrdersForLabBySiteCode(String siteCode, String modifiedBy, Boolean updateStatusToReceived)
        {
            var ds = GetOrdersForLabBySiteCode(siteCode, modifiedBy, updateStatusToReceived);
            return ds.GetXml();
        }

        public Boolean UpdateOrderStatus(string labCode, string orderNumber, bool isActive, string comment, string modifiedBy, StatusType status)
        {
            IOrderRepository _repository = new OrderRepository();

            var statusId = 0;

            switch (status)
            {
                case StatusType.CANCELLED:
                    statusId = 5;
                    break;

                case StatusType.DISPENSED:
                    statusId = 7;
                    break;

                case StatusType.RECEIVED:
                    statusId = 2;
                    break;

                case StatusType.REDIRECTED:
                    statusId = 4;
                    break;

                case StatusType.REJECTED:
                    statusId = 3;
                    break;

                case StatusType.RECLAIMED:
                    statusId = 17;
                    break;
            }

            return _repository.UpdateOrderStatus(labCode, orderNumber, isActive, comment, modifiedBy, statusId);
        }

        public List<String> InsertBatchOrders(XmlDocument orders)
        {
            List<Int32> failureList;
            BulkOrderEntityList orderList;

            orderList = orders.DeserializeXml<BulkOrderEntityList>();

            IOrderRepository _repository = new OrderRepository();

            failureList = _repository.InsertBulkOrders(orderList);

            var xList = orders.ConvertToStringList();

            return GetFailureList(xList, failureList);
        }

        private List<String> GetFailureList(List<String> xmlList, List<Int32> failureIdxList)
        {
            if (failureIdxList.Equals(null) || failureIdxList.Count.Equals(0)) return new List<string>();

            List<String> xmlFailures = new List<string>();

            foreach (var i in failureIdxList)
                xmlFailures.Add(xmlList[i]);

            return xmlFailures;
        }
    }
}