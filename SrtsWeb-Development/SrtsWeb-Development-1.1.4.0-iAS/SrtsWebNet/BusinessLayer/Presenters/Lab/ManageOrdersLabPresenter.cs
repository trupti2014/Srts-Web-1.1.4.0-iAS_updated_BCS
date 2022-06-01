using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Lab;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SrtsWeb.BusinessLayer.Presenters.Lab
{
    public sealed class ManageOrdersLabPresenter
    {
        private IManageOrdersLabView _view;

        public ManageOrdersLabPresenter(IManageOrdersLabView view)
        {
            _view = view;
        }

        public void InitView(bool isDispense)
        {
            if (!(isDispense))
            {
                // Get CheckIn orders
                GetDispenseCheckInOrders(false);
            }
            else
            {
                // Get Dispense orders
                GetDispenseCheckInOrders(true);
            }
        }

        public void GetDispenseCheckInOrders(bool IsDispense)
        {
            var _repository = new OrderStateRepository.OrdersOfStatusRepository();
            var moe = new List<ManageOrderEntity>();

            if (IsDispense)
                moe = _repository.GetOrdersToDispenseByLabCode(_view.mySession.MySite.SiteCode, _view.mySession.MyUserID);
            else
                moe = _repository.GetLabOrdersDispenseCheckInByLabCode(_view.mySession.MySite.SiteCode, _view.mySession.MyUserID, IsDispense);

            moe.ForEach(x => x.IsSelected = false);
            moe = moe.OrderByDescending(x => x.OrderNumber).ToList();

            if (IsDispense)
                _view.DispenseOrderData = moe;
            else
                _view.CheckInOrderData = moe;
        }

        #region CheckIn
        public void CheckInOrders()
        {
            var rep = new OrderStateRepository.OrderStatusRepository();
            var rs = _view.CheckInOrderData.Where(x => x.IsSelected == true).Select(x => x.OrderNumber).ToList();

            if (rs == null || rs.Count.Equals(0))
            {
                _view.Message = "No orders were selected for check-in.";
                return;
            }
            
            var nums = String.Join(",", rs);

            var ose = new OrderStateEntity();
            ose.OrderNumber = nums;
            ose.OrderStatusTypeID = 2;
            ose.StatusComment = "Lab Received Order";
            ose.LabCode = _view.mySession.MySite.SiteCode;
            ose.IsActive = true;
            ose.ModifiedBy = _view.mySession.MyUserID;

            if (!rep.InsertPatientOrderState(ose))
                _view.Message = "No orders were checked in.";
            else
            {
                // Added here so the rptViewerTemplate can get the order numbers.
                HttpContext.Current.Session["OrderNbrs"] = nums;
            }

            var omr = new OrderManagementRepository.OrderRepository();
            omr.UnlinkOrders(rs);
            omr = null;
        }

        public void GetTotalOrdersCheckIn()
        {
            _view.TotalOrdersToCheckIn = _view.CheckInOrderData.Count;
        }

        public DataTable GetCheckin771Reports(string orderNums)
        {
            var or = new OrderRepository();
            var dt = or.Get711DataByLabCode(_view.mySession.MySite.SiteCode, orderNums, HttpContext.Current.User.Identity.Name);
            return dt;
        }
        #endregion CheckIn

        #region Dispense
        public bool InsertState(OrderStateEntity ose)
        {
            var _orderStateRepository = new OrderStateRepository.OrderStatusRepository();
            return _orderStateRepository.InsertPatientOrderState(ose);
        }

        public void DispenseOrders(DataTable LabelTable)
        {
            var sb = new StringBuilder();
            var ose = new OrderStateEntity();
            var ordersToDispense = _view.DispenseOrderData.OrderByDescending(x => x.OrderNumber).Where(x => x.IsSelected == true).ToList();

            if (ordersToDispense.Count.Equals(0)) { _view.Message = "No orders where selected to check-out."; return; }

            foreach (var dr in ordersToDispense)
            {
                //Create data table that matches the what the xml headers/elements are
                DataRow row = LabelTable.NewRow();
                var sce = new SiteCodeEntity();
                var scr = new SiteRepository.SiteCodeRepository();

                sce = scr.GetSiteBySiteID(_view.mySession.MySite.SiteCode)[0];
                bool ShipToPatientLab = sce.ShipToPatientLab;
                bool ShipToPatientOrder = dr.ShipToPatient;

                // If lab does not ship to patients, or if lab does, but order is not ship to patient, get clinic address
                if (!ShipToPatientLab || !ShipToPatientOrder)
                {
                    string ClinicSiteCode = dr.ClinicSiteCode;

                    var c = scr.GetSiteBySiteID(ClinicSiteCode).Where(x => x.AddressType.ToLower() == "mail").FirstOrDefault();

                    row["Name"] = string.Format("{0} {1}", c.SiteCode, c.SiteName);

                    if (string.IsNullOrEmpty(c.Address2))
                    {
                        row["Address"] = c.Address1;
                    }
                    else
                    {
                        row["Address"] = string.Format("{0}{1}{2}", c.Address1, Environment.NewLine, c.Address2);
                    }
                    row["City"] = c.City;
                    row["State"] = c.State;
                    row["PostalCode"] = c.ZipCode;

                    ose.StatusComment = "Lab Dispensed Order To Clinic";
                    ose.OrderStatusTypeID = 7;
                    LabelTable.Rows.Add(row);
                }
                else
                {
                    row["Name"] = string.Format("{0} {1}", dr.FirstName, dr.LastName);

                    if (string.IsNullOrEmpty(dr.ShipAddress2))
                    {
                        row["Address"] = dr.ShipAddress1;
                    }
                    else
                    {
                        row["Address"] = string.Format("{0}{1}{2}", dr.ShipAddress1, Environment.NewLine, dr.ShipAddress2);
                    }
                    row["City"] = dr.ShipCity;
                    row["State"] = dr.ShipState;
                    row["PostalCode"] = dr.ShipZipCode;

                    ose.StatusComment = "Lab Dispensed And Shipped Order To Patient";
                    ose.OrderStatusTypeID = 19;
                    LabelTable.Rows.Add(row);
                }

                ose.OrderNumber = dr.OrderNumber;
                ose.LabCode = dr.LabSiteCode;
                ose.ModifiedBy = _view.mySession.MyUserID;
                ose.IsActive = true;

                if (!InsertState(ose))
                {
                    sb.Append(string.Format("Order {0} was not dispensed<br />", ose.OrderNumber));
                }
            }

            HttpContext.Current.Session["LabelTable"] = LabelTable;
            HttpContext.Current.Session["lType"] = _view.SelectedLabel;

            _view.Message = sb.ToString();
        }
        #endregion Dispense
    }
}