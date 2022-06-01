using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.Views.Lab;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.BusinessLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using System.IO; //Added on 3/20/2019 to debug

namespace SrtsWeb.Presenters.Lab
{
    public sealed class ManageOrdersLabPresenter : CustomEventLogger
    {
        private IManageOrdersLabView _view;

        public ManageOrdersLabPresenter(IManageOrdersLabView view)
        {
            _view = view;
        }

        public static List<ManageOrderEntity> GetManageLabGridOrders(String gridToFill, string siteCode, string myUserId)
        {
            var osr = new OrderStateRepository.OrdersOfStatusRepository();
            var moe = new List<ManageOrderEntity>();

            switch (gridToFill.ToLower())
            {
                case "dispense":
                    moe = osr.GetLabDispenseOrders(siteCode, myUserId); //osr.GetOrdersToDispenseByLabCode(siteCode, myUserId);
                    break;
                case "checkin":
                    moe = osr.GetLabCheckinOrders(siteCode, myUserId); //osr.GetLabOrdersDispenseCheckInByLabCode(siteCode, myUserId, false);
                    break;
                case "redirectreject":
                    moe = osr.GetLabRedirectRejectOrders(siteCode, myUserId);
                    break;
                case "holdforstock":
                    moe = osr.GetLabHoldStockOrders(siteCode, myUserId); //osr.GetOrdersToDispenseByLabCode(siteCode, myUserId);
                    //moe.AddRange(osr.GetLabOrdersDispenseCheckInByLabCode(siteCode, myUserId, false));
                    break;
            }

            moe.ForEach(x => x.IsSelected = false);
            moe = moe.OrderByDescending(x => x.OrderNumber).ToList();

            return moe;
        }

        public Boolean UpdateOrders(List<String> ordersIG, int pageModule, DataTable LabelTable)
        {
            var osr = new OrderStateRepository.OrderStatusRepository();
            var printLabels = false;
            var allOse = new OrderStateEntity();

            allOse.LabCode = _view.mySession.MySite.SiteCode;
            allOse.IsActive = true;
            allOse.ModifiedBy = _view.mySession.ModifiedBy;

            var goodOrders = new StringBuilder();
            switch (pageModule)
            {
                case 0:     // Orders need to be checked in to the lab
                    #region checkin
                    var regularOrders = String.Join(",", ordersIG);

                    // Do status change for regular order check-ins.  These are the orders that are not status 5 (Hold for Stock)
                    if (regularOrders.IsNullOrEmpty().Equals(false))
                    {
                        allOse.OrderNumber = regularOrders;
                        allOse.OrderStatusTypeID = 2;
                        allOse.StatusComment = "Lab Received Order";

                        if (!osr.InsertPatientOrderState(allOse))
                        {
                            LogEvent(String.Format("User {0} unsuccessfully checked in the following order to the lab at {1}:  {2};", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));
                            _view.Message = "An error has occured during processing.";
                        }
                        else
                        {
                            LogEvent(String.Format("User {0} successfully checked in the following order to the lab at {1}:  {2};", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));

                            // Added here so the rptViewerTemplate can get the order numbers.
                            goodOrders.AppendFormat("{0},", regularOrders);
                        }
                    }

                    HttpContext.Current.Session["OrderNbrs"] = goodOrders;
                    break;

                    #endregion checkin

                case 1:     // Orders need to be dispensed from the lab
                    #region dispense
                    // This order state entity is strictly used for orders that ship directly to the patient.
                    var s2pOse = new OrderStateEntity();

                    allOse.OrderNumber = String.Join(",", ordersIG);
                    allOse.OrderStatusTypeID = 7;
                    allOse.StatusComment = String.Format("Lab ({0}) Dispensed Order to Clinic", _view.mySession.MySite.SiteCode);

                    // If the lab does not ship to patient then exit leave switch and set the order statuses
                    if (_view.mySession.MySite.ShipToPatientLab)
                    {
                        // Get the order entities for the order numbers provided.
                        var or = new OrderRepository();
                        var aOs = or.GetOrderByOrderNumberNonGEyes(allOse.OrderNumber, _view.mySession.ModifiedBy);

                        // If there are orders to ship to patient then continue through he if.
                        if (aOs.FirstOrDefault(a => a.OrderDisbursement == "L2P") != null)
                        {
                            var stpOrders = aOs.Where(o => o.OrderDisbursement == "L2P").ToList();
                            aOs = null;

                            // Create order state entity for ship to patient orders.
                            s2pOse.OrderNumber = String.Join(",", stpOrders.Select(s => s.OrderNumber));
                            s2pOse.LabCode = _view.mySession.MySite.SiteCode;
                            s2pOse.IsActive = true;
                            s2pOse.ModifiedBy = _view.mySession.ModifiedBy;
                            s2pOse.OrderStatusTypeID = 19;
                            s2pOse.StatusComment = String.Format("Lab ({0}) Shipped To Patient", _view.mySession.MySite.SiteCode);

                            // Remove the s2p orders from the allOse object
                            var j = allOse.OrderNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            var k = s2pOse.OrderNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            k.ForEach(r =>
                            {
                                j.Remove(r);
                            });

                            allOse.OrderNumber = String.Join(",", j);
                            j = null;
                            k = null;

                            printLabels = true;
                            // Get the inidividual data for each order to fill the label table
                            var shipIndividualList = new List<IndividualEntity>();
                            var emailAddressRecipientList = new List<String>();
                            stpOrders.ForEach(x =>
                            {
                                var ir = new IndividualRepository();
                                var ie = new IndividualEntity();
                                

                                shipIndividualList.Add(ir.GetIndividualByIndividualID(x.IndividualID_Patient, _view.mySession.ModifiedBy).FirstOrDefault());


                                if (x.IsEmailPatient)
                                {
                                    try
                                    {
                                        var _orderemailRespository = new OrderManagementRepository.OrderEmailRepository();
                                        var orderemail = _orderemailRespository.GetOrderEmailByOrderNumber(x.OrderNumber, _view.mySession.ModifiedBy); 
                                        var body = _orderemailRespository.GetOrderEmailMessage(x.OrderNumber); 
                                        if ( String.IsNullOrEmpty(body) )
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
                                        LogEvent(String.Format("An error occurred when sending the email notification for order number {0} : {1} - {2}", x.OrderNumber, ex.Message,  ex.InnerException));
                                        ex.LogException(String.Format("An error occurred when sending the email notification for order number {0} : {1} - {2}", x.OrderNumber, ex.Message, ex.InnerException));
                                    }
                                }
                            });

                            // Determine if the lab preference is set to sort label alphabetically
                            var sr = new SitePreferencesRepository.GeneralPreferencesRepostiory();
                            var sortAlpha = sr.GetPreferences(this._view.mySession.MySite.SiteCode);

                            if (sortAlpha)
                            {
                                // Create a KeyValuePair list so an order by can be performed on it.
                                var shipDict = new List<KeyValuePair<IndividualEntity, OrderEntity>>();

                                // Populate the list with the individual and order entities
                                for (var idx = 0; idx < shipIndividualList.Count; idx++)
                                    shipDict.Add(new KeyValuePair<IndividualEntity, OrderEntity>(shipIndividualList[idx], stpOrders[idx]));

                                // Sort the list
                                var sorted = shipDict.OrderBy(ob => ob.Key.LastName).ThenBy(tb => tb.Key.FirstName).ToList();

                                // Create the label table
                                sorted.ForEach(s =>
                                {
                                    LabelTable.Rows.Add(BuildStpLabel(s.Key, s.Value, LabelTable));
                                });
                            }
                            else // UNSORTED
                            {
                                // Create the label table
                                for (var i = 0; i < shipIndividualList.Count; i++)
                                {
                                    LabelTable.Rows.Add(BuildStpLabel(shipIndividualList[i], stpOrders[i], LabelTable));
                                }
                            }

                            if (osr.InsertPatientOrderState(s2pOse) == false)
                            {
                                LogEvent(String.Format("User {0} unsuccessfully dispensed the following orders directly to the patient(s) at {1}:  {2}", _view.mySession.ModifiedBy, DateTime.Now, s2pOse.OrderNumber));
                                _view.Message = "An error has occured during processing.";
                            }
                            else
                            {
                                LogEvent(String.Format("User {0} successfully dispensed the following orders directly to the patient(s) at {1}:  {2}", _view.mySession.ModifiedBy, DateTime.Now, s2pOse.OrderNumber));
                                // Added here so the rptViewerTemplate can get the order numbers.
                                HttpContext.Current.Session["OrderNbrs"] = s2pOse.OrderNumber;
                            }
                        }
                    }

                    if (osr.InsertPatientOrderState(allOse))
                    {
                        LogEvent(String.Format("User {0} successfully dispensed the following order to the clinic at {1}:  {2}.", _view.mySession.ModifiedBy, DateTime.Now, allOse.OrderNumber));
                    }
                    else
                    {
                        LogEvent(String.Format("User {0} unsuccessfully dispensed the following order to the clinic at {1}:  {2}.", _view.mySession.ModifiedBy, DateTime.Now, allOse.OrderNumber));
                        _view.Message = "An error has occured during processing.";
                    }
                    break;

                    #endregion dispense

                case 2:     // Orders need to have a hold for stock date and reason set
                    #region hold for stock
                    var holdOrders = String.Join(",", ordersIG);
                    // Do status change for Hold for Stock orders
                    if (holdOrders.IsNullOrEmpty().Equals(false))
                    {
                        allOse.OrderNumber = holdOrders;
                        allOse.OrderStatusTypeID = 5;
                        allOse.StatusComment = this._view.HoldForStockReason;

                        if (!osr.InsertPatientOrderState(allOse))
                        {
                            LogEvent(String.Format("User {0} unsuccessfully set hold for stock in the following order to the lab at {1}:  {2};", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));
                            _view.Message = "An error has occured during processing.";
                        }
                        else
                        {
                            LogEvent(String.Format("User {0} successfully set hold for stock in the following order to the lab at {1}:  {2};", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));

                            // Added here so the rptViewerTemplate can get the order numbers.
                            goodOrders.Append(holdOrders);
                            UnLinkOrders(ordersIG);
                            return true;
                        }
                    }
                    break;
                    #endregion

                case 3:     // Orders need to checked in from hold for stock
                    #region checkin from hold for stock
                    var checkinHoldOrders = String.Join(",", ordersIG);
                    // Do status change for Hold for Stock orders
                    if (checkinHoldOrders.IsNullOrEmpty().Equals(false))
                    {
                        allOse.OrderNumber = checkinHoldOrders;
                        allOse.OrderStatusTypeID = 2;
                        allOse.StatusComment = "Lab Received Order from Hold for Stock";

                        if (!osr.InsertPatientOrderState(allOse))
                        {
                            LogEvent(String.Format("User {0} unsuccessfully checked in the following order to the lab at {1}:  {2};", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));
                            _view.Message = "An error has occured during processing.";
                        }
                        else
                        {
                            LogEvent(String.Format("User {0} successfully checked in the following order to the lab at {1}:  {2};", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));

                            // Added here so the rptViewerTemplate can get the order numbers.
                            goodOrders.Append(checkinHoldOrders);
                            _view.Message = "Orders were successfully checked in.";
                            return true;
                        }
                    }
                    break;
                    #endregion
                case 4:
                   #region redirect 
                    var redirectOrders = String.Join(",", ordersIG);

                    // Do status change for regular order check-ins.  
                    if (redirectOrders.IsNullOrEmpty().Equals(false))
                    {
                        allOse.OrderNumber = redirectOrders;
                        allOse.OrderStatusTypeID = 4;
                        allOse.LabCode = _view.RedirectLab;

                        var sb = new StringBuilder();
                        sb.AppendFormat("{0} redirected order to {1}.  ", _view.mySession.MySite.SiteCode, _view.RedirectLab);
                        sb.AppendFormat("Justification: {0}", _view.RejectRedirectJustification);
                        allOse.StatusComment = sb.ToString();

                        if (!osr.InsertPatientOrderState(allOse))
                        {
                            LogEvent(String.Format("User {0} unsuccessfully redirected the following order to the lab at {1}:  {2};", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));
                            _view.Message = "An error has occured during processing.";
                        }
                        else
                        {
                            LogEvent(String.Format("User {0} successfully redirected the following order to the lab at {1}:  {2};", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));
                            string message = String.Format("Orders were successfully redirected to {0}.", _view.RedirectLab);
                            _view.Message = message;
                            
                    //        // Added here so the rptViewerTemplate can get the order numbers.
                    //        goodOrders.AppendFormat("{0},", regularOrders);
                        }
                    }

                    //HttpContext.Current.Session["OrderNbrs"] = goodOrders;

                    break;
                    #endregion
                case 5:
                    #region reject
                    var rejectOrders = String.Join(",", ordersIG);

                    // Do status change for regular order check-ins.  
                    if (rejectOrders.IsNullOrEmpty().Equals(false))
                    {
                        allOse.OrderNumber = rejectOrders;
                        allOse.OrderStatusTypeID = 3;
                        
                        var sb = new StringBuilder();
                        sb.AppendFormat("Order rejected by {0}.  ", _view.mySession.MySite.SiteCode);
                        sb.AppendFormat("Justification: {0}", _view.RejectRedirectJustification);
                        allOse.StatusComment = sb.ToString();

                        if (!osr.InsertPatientOrderState(allOse))
                        {
                            LogEvent(String.Format("User {0} unsuccessfully rejected the following order at {1}: {2}", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));
                            _view.Message = "An error has occured during processing.";
                        }
                        else
                        {
                            LogEvent(String.Format("User {0} successfully rejected the following order at {1}: {2}", _view.mySession.MyUserID, DateTime.Now, allOse.OrderNumber));
                            string message = String.Format("Orders were successfully rejected.");
                            _view.Message = message;

                            //        // Added here so the rptViewerTemplate can get the order numbers.
                            //        goodOrders.AppendFormat("{0},", regularOrders);
                        }
                    }
                    //HttpContext.Current.Session["OrderNbrs"] = goodOrders;

                    break;
                    #endregion
            }
            UnLinkOrders(ordersIG);
            if (printLabels)
            {
                HttpContext.Current.Session["LabelTable"] = LabelTable;
                HttpContext.Current.Session["lType"] = _view.SelectedLabel;
            }

            return printLabels;
        }
        private void UnLinkOrders(List<string> oig)
        {
            var omr = new OrderManagementRepository.OrderRepository();
            omr.UnlinkOrders(oig);
            omr = null;
        }
        private DataRow BuildStpLabel(IndividualEntity ie, OrderEntity oe, DataTable LabelTable)
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
            ose.ModifiedBy = _view.mySession.ModifiedBy;

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

        public List<SiteCodeEntity> GetLabListForRedirect()
        {
            var r = new SiteRepository.SiteCodeRepository();
            var lablistredirect = r.GetSitesByType("LAB");
            return lablistredirect;
        }




    }
}
 