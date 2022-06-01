using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;

namespace SrtsWeb.Presenters.Orders
{
    public sealed class CheckInDispensePresenter : CustomEventLogger
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

        public static List<ManageOrderEntity> GetCheckInOrDispenseOrders(bool IsCheckIn, string siteCode, string myUserId)
        {
            var _repository = new OrderStateRepository.OrdersOfStatusRepository();
            var moe = new List<ManageOrderEntity>();

            if (IsCheckIn)
                moe = _repository.GetOrdersForCheckInToClinicByClinicCode(siteCode, myUserId);
            else
                moe = _repository.GetOrdersToDispenseByClinicCode(siteCode, myUserId);

            moe.ForEach(x => x.IsSelected = false);
            moe = moe.OrderByDescending(x => x.OrderNumber).ToList();

            return moe;
        }

        public bool UpdateOrders(List<string> ordersIG, List<string> ordersNIG, int pageModule, DataTable LabelTable)
        {
            var sb = new StringBuilder();
            var osr = new OrderStateRepository.OrderStatusRepository();
            var or = new OrderRepository();
            var printLabels = false;
            var labs = new List<SiteCodeEntity>(); // This is used to store labs that are looked up to determine if it is a STP lab.

            List<string> allOrders = new List<string>();
            if (ordersIG.Count > 0) allOrders.AddRange(ordersIG);
            if (ordersNIG.Count > 0) allOrders.AddRange(ordersNIG);

            try
            {
                string allOrdersNumbers = allOrders.Aggregate((i, j) => i + ',' + j);
                List<OrderStateEntity> orderStates = osr.GetActiveOrderStatusByOrderNumber(allOrdersNumbers);
                List<OrderEntity> orderData = or.GetOrderByOrderNumberNonGEyes(allOrdersNumbers, _view.mySession.ModifiedBy);

                var errOrder = new List<String>();
                var goodOrder = new List<String>();
                var action = String.Empty;

                var shipIndividualList = new List<IndividualEntity>();
                var shipOrderList = new List<OrderEntity>();
                var emailAddressRecipientList = new List<String>();

                for (int i = 0; i < allOrders.Count; i++)
                {
                    var ose = new OrderStateEntity();
                    var oe = new OrderEntity();
                    ose = orderStates.Where(x => x.OrderNumber == allOrders[i] && x.IsActive == true).FirstOrDefault();
                    oe = orderData.Where(x => x.OrderNumber == allOrders[i]).FirstOrDefault();

                    switch (pageModule)
                    {
                        case 0:     // Orders need to be checked in to the clinic
                            action = "checked in to clinic status";

                            #region checkin

                            switch (ose.OrderStatusTypeID)
                            {
                                case 7:
                                    {
                                        ose.OrderStatusTypeID = 11;
                                        ose.StatusComment = "Clinic Received Order";
                                        break;
                                    }
                                case 2:
                                    {
                                        ose.OrderStatusTypeID = 16;
                                        ose.StatusComment = "Clinic Checkin - No Lab Checkout";
                                        break;
                                    }
                                case 5:
                                    {
                                        ose.OrderStatusTypeID = 16;
                                        ose.StatusComment = "Clinic Checkin - No Lab Checkout from On Hold";
                                        break;
                                    }
                                default:
                                    {
                                        _view.Message = "Order is currently in a status that cannot be checked-in to the clinic.";
                                        return false;
                                    }
                            }
                            var l = labs.FirstOrDefault(x => x.SiteCode == oe.LabSiteCode);
                            if (l == null)
                            {
                                var sr = new SiteRepository.SiteCodeRepository();
                                l = sr.GetSiteBySiteID(oe.LabSiteCode).FirstOrDefault();
                                labs.Add(l);
                            }

                            var od = new List<String>() { "C2P", "L2P" }; //l.ShipToPatientLab ? new List<String>() { "C2P" } : new List<String>() { "C2P", "L2P" };
                            if (od.Contains(oe.OrderDisbursement))
                            {
                                printLabels = true;
                                var ir = new IndividualRepository();
                                var ie = new IndividualEntity();
                                
                                ie = ir.GetIndividualByIndividualID(oe.IndividualID_Patient, _view.mySession.ModifiedBy).FirstOrDefault();

                                shipIndividualList.Add(ie);
                                shipOrderList.Add(oe);

                                if (oe.OrderDisbursement == "C2P" && oe.IsEmailPatient)
                                {
                                    try
                                    {
                                        var _orderemailRespository = new OrderManagementRepository.OrderEmailRepository();
                                        var orderemail = _orderemailRespository.GetOrderEmailByOrderNumber(oe.OrderNumber, _view.mySession.ModifiedBy);
                                        var body = _orderemailRespository.GetOrderEmailMessage(oe.OrderNumber);
                                        if (String.IsNullOrEmpty(body))
                                        {
                                            Exception ex = new Exception();
                                            ex.LogException("GetOrderEmailMsg SP returned null or empty string");
                                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                        }
                                        else
                                        {
                                            orderemail.EmailMsg = body.ToString();
                                            orderemail.EmailDate = DateTime.Now;
                                            orderemail.EmailSent = true;
                                            var m = new SrtsWeb.BusinessLayer.Concrete.MailService();
                                            m.SendEmail(body.ToString(), ConfigurationManager.AppSettings["FromEmail"], new List<String>() { orderemail.EmailAddress }, "Order Completed");
                                            var updateOrderEmailSuccess = _orderemailRespository.UpdateOrderEmail(orderemail, _view.mySession.ModifiedBy);
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        LogEvent(String.Format("An error occurred when sending the email notification for order number {0} : {1} - {2}", oe.OrderNumber, ex.Message, ex.InnerException));
                                        ex.LogException(String.Format("An error occurred when sending the email notification for order number {0} : {1} - {2}", oe.OrderNumber, ex.Message, ex.InnerException));
                                    }
                                }
                            }

                            if (oe.OrderDisbursement == "CD" && oe.IsEmailPatient )
                            {
                                try
                                {
                                    var _orderemailRespository = new OrderManagementRepository.OrderEmailRepository();
                                    var orderemail = _orderemailRespository.GetOrderEmailByOrderNumber(oe.OrderNumber, _view.mySession.ModifiedBy);
                                    var body = _orderemailRespository.GetOrderEmailMessage(oe.OrderNumber);

                                    if (String.IsNullOrEmpty(body))
                                    {
                                        Exception ex = new Exception();
                                        ex.LogException("GetOrderEmailMsg SP returned null or empty string");
                                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                    }
                                    else
                                    {
                                        orderemail.EmailMsg = body.ToString();
                                        orderemail.EmailDate = DateTime.Now;
                                        orderemail.EmailSent = true;
                                        var m = new SrtsWeb.BusinessLayer.Concrete.MailService();
                                        m.SendEmail(body.ToString(), ConfigurationManager.AppSettings["FromEmail"], new List<String>() { orderemail.EmailAddress }, "Order Received at Clinic");
                                        var updateOrderEmailSuccess = _orderemailRespository.UpdateOrderEmail(orderemail, _view.mySession.ModifiedBy);
                                    }
                                }
                                catch(Exception ex)
                                {
                                    LogEvent(String.Format("An error occurred when sending the email notification for order number {0} : {1} - {2}", oe.OrderNumber, ex.Message, ex.InnerException));
                                    ex.LogException(String.Format("An error occurred when sending the email notification for order number {0} : {1} - {2}", oe.OrderNumber, ex.Message, ex.InnerException));
                                }
                            }
                            break;

                            #endregion checkin

                        case 1:     // Orders need to be dispensed from the clinic
                            action = "dispensed from clinic status";
                            #region
                            if (ose.OrderStatusTypeID == 11 || ose.OrderStatusTypeID == 16)
                            {
                                ose.OrderStatusTypeID = 8;
                                ose.StatusComment = "Clinic Dispensed Order";
                            }
                            else
                            {
                                _view.Message = "Order is currently in a status that cannot be dispensed.";
                                return false;
                            }
                            break;
                            #endregion
                        case 2:     // Orders in problem status that need to be resubmitted
                            action = "problem status";
                            #region
                            break;
                            #endregion
                        case 3:     // Orders in overdue status
                            action = "overde status";
                            #region
                            break;
                            #endregion
                    }
                    ose.ModifiedBy = _view.mySession.ModifiedBy;
                    ose.IsActive = true;

                    if (!osr.InsertPatientOrderState(ose))
                    {
                        errOrder.Add(ose.OrderNumber);
                        sb.Append(string.Format("An error occured while updating order # {0}<br />", ose.OrderNumber));
                    }
                    else
                        goodOrder.Add(ose.OrderNumber);
                }

                // Write log entry here for good and error order numbers.
                if (goodOrder.Count.GreaterThan(0))
                    LogEvent(String.Format("User {0} change the status of the following order(s) to {1} status at {2}:  {3}.", _view.mySession.MyUserID, action, DateTime.Now, String.Join(", ", goodOrder)));
                if (errOrder.Count.GreaterThan(0))
                    LogEvent(String.Format("User {0} encountered an error changing the status of the following order(s) to {1} status at {2}:  {3}.", _view.mySession.MyUserID, action, DateTime.Now, String.Join(", ", goodOrder)));

                if (printLabels)
                {
                    // If the site preference for the alpha sort is set to true then do sort on the datatable.
                    var r = new SitePreferencesRepository.GeneralPreferencesRepostiory();
                    var sortAlpha = r.GetPreferences(this._view.mySession.MySite.SiteCode);
                    if (sortAlpha)
                    {
                        var shipDict = new List<KeyValuePair<IndividualEntity, OrderEntity>>();

                        for (var idx = 0; idx < shipIndividualList.Count; idx++)
                            shipDict.Add(new KeyValuePair<IndividualEntity, OrderEntity>(shipIndividualList[idx], shipOrderList[idx]));

                        var sorted = shipDict.OrderBy(ob => ob.Key.LastName).ThenBy(tb => tb.Key.FirstName).ToList();

                        sorted.ForEach(s =>
                        {
                            LabelTable.Rows.Add(BuildStpLabel(s.Key, s.Value, LabelTable));
                        });
                    }
                    else
                    {
                        for (var i = 0; i < shipIndividualList.Count; i++)
                        {
                            LabelTable.Rows.Add(BuildStpLabel(shipIndividualList[i], shipOrderList[i], LabelTable));
                        }
                    }
                    HttpContext.Current.Session["LabelTable"] = LabelTable;
                    HttpContext.Current.Session["lType"] = _view.SelectedLabel;
                }
            }
            catch (Exception ex)
            {
                _view.Message = ex.ToString();
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return printLabels;
        }

        protected DataRow BuildStpLabel(IndividualEntity ie, OrderEntity oe, DataTable LabelTable)
        {
            var dr = LabelTable.NewRow();
            //Create data table that matches the what the xml headers/elements are
            dr["Name"] = string.Format("{0} {1}", ie.FirstName, ie.LastName);
            dr["Address"] = string.IsNullOrEmpty(oe.ShipAddress2) ? oe.ShipAddress1 : string.Format("{0}{1}{2}", oe.ShipAddress1, Environment.NewLine, oe.ShipAddress2);
            dr["City"] = oe.ShipCity;
            dr["State"] = oe.ShipState;
            dr["PostalCode"] = oe.ShipZipCode.ToZipCodeLabelPrint();

            string country = Helpers.GetCountryDescription(oe.ShipCountry);
            dr["Country"] = country;

            // dr["Country"] = oe.ShipCountry;
            return dr;
        }

        private List<IndividualEntity> IndividualList
        {
            get;
            set;
        }
    }
}