using SrtsWeb.Views.Orders;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace SrtsWeb.BusinessLayer.Presenters.Orders
{
    public sealed class CheckInDispensePresenter
    {
        private ICheckInDispenseView _view;

        public CheckInDispensePresenter(ICheckInDispenseView view)
        {
            _view = view;
        }

        public void InitView()
        {
            _view.ReportName = "Avery5160.rpt";
        }

        public List<ManageOrderEntity> GetCheckInOrDispenseOrders(bool IsCheckIn)
        {
            var orderData = new List<ManageOrderEntity>();
            var osr = new OrderStateRepository.OrdersOfStatusRepository();
            if (IsCheckIn)
                orderData = osr.GetOrdersForCheckInToClinicByClinicCode(_view.mySession.MySite.SiteCode, _view.mySession.MyUserID);
            else
                orderData = osr.GetOrdersToDispenseByClinicCode(_view.mySession.MySite.SiteCode, _view.mySession.MyUserID);
            return orderData;
        }

        public bool UpdateRow(OrderStateEntity ose)
        {
            var _repository = new OrderStateRepository.OrderStatusRepository();
            return _repository.InsertPatientOrderState(ose);
        }

        public bool CheckInOrders(string[] ordersToCheckIn, DataTable LabelTable)
        {
            var sb = new StringBuilder();
            var ose = new OrderStateEntity();
            var _printIt = false;

            foreach (string order in ordersToCheckIn)
            {
                var e = _view.CheckInData.Where(x => x.OrderNumber == order).FirstOrDefault();
                if (e == null) continue;

                if (e.ShipToPatient)
                {
                    //Create data table that matches the what the xml headers/elements are
                    var row = LabelTable.NewRow();

                    row["Name"] = string.Format("{0} {1}", e.FirstName, e.LastName);
                    if (string.IsNullOrEmpty(e.ShipAddress2))
                        row["Address"] = e.ShipAddress1;
                    else
                        row["Address"] = string.Format("{0}{1}{2}", e.ShipAddress1, Environment.NewLine, e.ShipAddress2);
                    row["City"] = e.ShipCity;
                    row["State"] = e.ShipState;
                    row["PostalCode"] = e.ShipZipCode;
                    LabelTable.Rows.Add(row);

                    _printIt = true;
                }

                ose.OrderNumber = e.OrderNumber;
                ose.LabCode = e.LabSiteCode;
                ose.ModifiedBy = _view.mySession.MyUserID;
                ose.OrderStatusTypeID = 11;
                ose.IsActive = true;
                ose.StatusComment = "Clinic Received Order";

                if (!(UpdateRow(ose)))
                {
                    sb.Append(string.Format("Order {0} was not checked in<br />", ose.OrderNumber));
                }
            }

            if (_printIt)
            {
                HttpContext.Current.Session["LabelTable"] = LabelTable;
                HttpContext.Current.Session["lType"] = _view.SelectedLabel;
            }
            _view.Message = sb.ToString();
            return _printIt;
        }

        public bool DispenseOrdersFromLabForClinicCheckIn(List<String> orderNumbers)
        {
            var r = new OrderStateRepository.OrderStatusRepository();

            var s = new OrderStateEntity();
            s.IsActive = true;
            s.ModifiedBy = this._view.mySession.MyUserID;
            s.OrderStatusType = "";
            s.OrderStatusTypeID = 0;
            s.StatusComment = "";
            foreach (var o in orderNumbers)
            {
                var ls = r.GetOrderStateByOrderNumber(o);
                var cs = ls.FirstOrDefault(x => x.IsActive == true);
                if (cs.OrderStatusType.ToLower().Equals("lab checked in order"))
                {
                    // Do dispense from lab with special status
                    s.OrderNumber = o;
                    s.LabCode = cs.LabCode;
                    //r.InsertPatientOrderState(s);
                }
            }
            return false;
        }

        public void SearchGvForOrders(string[] ordersToFind, GridView CurrentGv)
        {
            foreach (string order in ordersToFind)
            {
                var rowIndex = (CurrentGv.Rows.Cast<GridViewRow>()
                    .Where(x => ((Label)x.Cells[2].FindControl("lblOrderNumber")).Text == order).FirstOrDefault()).RowIndex;

                ((CheckBox)CurrentGv.Rows[rowIndex].FindControl("cbCheckedIn")).Checked = true;
            }
        }

        public void InsertDispenseStatus()
        {
            var _repository = new OrderStateRepository.OrderStatusRepository();

            var le = _view.DispenseData.Where(x => x.IsSelected == true).ToList();
            if (le == null || le.Count.Equals(0)) return;

            foreach (var i in le)
            {
                var ose = new OrderStateEntity();
                ose.OrderNumber = i.OrderNumber;
                ose.LabCode = i.LabSiteCode;
                ose.ModifiedBy = _view.mySession.MyUserID;
                ose.OrderStatusTypeID = 8;
                ose.StatusComment = "Dispensed By Clinic";
                ose.IsActive = true;

                if (!_repository.InsertPatientOrderState(ose))
                    _view.Message = string.Format("Order {0} was not dispensed<br />", ose.OrderNumber);
            }
            GetCheckInOrDispenseOrders(false);
        }

        //public void UpdateTableSelectDispense(string _orderNumber)
        //{
        //    foreach (DataRow dr in _view.DispenseData.Rows)
        //    {
        //        if (dr["OrderNumber"].ToString() == _orderNumber)
        //        {
        //            dr["Dispensed"] = true;
        //        }
        //    }
        //}

        //public void UpdateTableDeSelectDispense(string _orderNumber)
        //{
        //    foreach (DataRow dr in _view.DispenseData.Rows)
        //    {
        //        if (dr["OrderNumber"].ToString() == _orderNumber)
        //        {
        //            dr["Dispensed"] = false;
        //        }
        //    }
        //}
    }
}