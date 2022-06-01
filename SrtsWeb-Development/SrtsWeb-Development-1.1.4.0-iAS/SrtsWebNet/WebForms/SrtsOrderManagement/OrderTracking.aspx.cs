using SrtsWeb;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Orders;
using SrtsWeb.Views.Orders;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

namespace SrtsWeb.WebForms.SrtsOrderManagement
{
    public partial class OrderTracking : PageBase, IOrderTrackingView
    {
        private OrderTrackingPresenter _presenter;

        #region LOAD

        /// <summary>
        /// Load 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                try
                {
                    this.SiteCode = this.mySession.MyClinicCode;
                    InitLoad();
                    BuildPageTitle();
                    txtTrackingNumberS.Focus();
                    this.pnlManual.Visible = false;

                }
                catch (NullReferenceException ex)
                {
                    ex.TraceErrorException();
                    Response.Redirect(FormsAuthentication.DefaultUrl, false);
                }
            }

        }

        /// <summary>
        /// Load Initial Dropdowns
        /// </summary>
        private void InitLoad()
        {

            // Load Site Tracked Orders lists
            this._presenter = new OrderTrackingPresenter(this);
            _presenter.GetTrackedOrders(SiteCode);
            gvTracking.DataSource = TrackingBaseList;
            gvTracking.DataBind();
            this.udpTrackOrders.Update();

        }

        /// <summary>
        /// Build the page titles
        /// </summary>
        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = "Manage Orders - Tracking";
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Manage Orders - Tracking");
                CurrentModule_Sub(string.Empty);
            }
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// Barcode Scanner Logic for scanning tracking labels and order numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.txtTrackingNumberS.Text.Trim())) return;

            var track = this.txtTrackingNumberS.Text.Trim();
            this.txtTrackingNumberS.Text = String.Empty;
            if (track.Length < 12) return;

            string scan = GetScanString(track);

            if (scan == "USPS" || scan == "UPS" || scan == "FEDEX")
            {
                this.TrackingScan = string.Empty;
                ///load the SP by the tracking number and set the session variables
                this._presenter = new OrderTrackingPresenter(this);
                _presenter.GetTrackingOrderScan(track);
                if (TrackingScanList.Count != 0)
                {
                    OrderTrackingScanEntity Tracking = (from TrackingNumber in TrackingScanList select TrackingNumber).FirstOrDefault();
                    OrderTrackingScanEntity Patient = (from PatientID in TrackingScanList select PatientID).FirstOrDefault();
                    OrderTrackingScanEntity OrderOne = (from Order1 in TrackingScanList select Order1).FirstOrDefault();
                    OrderTrackingScanEntity OrderTwo = (from Order2 in TrackingScanList select Order2).FirstOrDefault();
                    OrderTrackingScanEntity OrderThree = (from Order3 in TrackingScanList select Order3).FirstOrDefault();
                    OrderTrackingScanEntity OrderFour = (from Order4 in TrackingScanList select Order4).FirstOrDefault();

                    this.TrackingScan = Tracking.TrackingNumber;
                    this.PatientID = Patient.PatientID;
                    this.Shipper = GetScanString(this.TrackingScan);
                    ddlShippingProvider.SelectedValue = this.Shipper;
                    this.OrderOne = OrderOne.Order1;
                    this.OrderTwo = OrderTwo.Order2;
                    this.OrderThree = OrderThree.Order3;
                    this.OrderFour = OrderFour.Order4;
                }

            }

            var Message = string.Empty;

            switch (scan.ToLower())
            {
                case "ordernumber":
                    {
                        ///check if tracking number exist first
                        if (HttpContext.Current.Session["TrackingScan"] == null)
                        {
                            ///if no then send error message please scan tracking number before order number
                            ShowErrorDialog("Please scan a valid tracking number before scanning an order number.");
                        }
                        else
                        {
                            ///tracking number exist move on to checking for duplicates
                            UpdateSession("Check");
                            int ordervalue = CheckOrderValue(track);
                            if (ordervalue == 0)
                            {
                                ///not duplicate proceed to checking max 4 orders
                                int ordercount = GetOrderCount(track);
                                if (ordercount != 0)
                                {
                                    ///if "1" use ordernumber to get patientID
                                    if (ordercount == 1)
                                    {
                                        this._presenter = new OrderTrackingPresenter(this);
                                        _presenter.GetPatientID(track);
                                        //OrderTrackingOrderEntity Patient = (from PatientID in TrackingPatientID select PatientID).FirstOrDefault();
                                        //this.PatientID = Patient.PatientID;
                                    }
                                    ///if no save to the database using tracking session, patient session, order scanned, shipper session, I for what
                                    ///if SP returns wrong patient send error message
                                    ///
                                    OrderTrackingScanEntity ScanEntity = new OrderTrackingScanEntity();

                                    var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;

                                    ScanEntity.TrackingNumber = this.TrackingScan;
                                    ScanEntity.PatientID = this.PatientID;

                                    this._presenter = new OrderTrackingPresenter(this);
                                    var good = _presenter.InsertUpdateTrackingNumber(ScanEntity, track, this.Shipper, "I");
                                    if (good == "WP") { Message = "Wrong Patient"; } else { Message = "Success"; }
                                    if (!String.IsNullOrEmpty(Message))
                                    {
                                        if (good != "WP")
                                        {
                                            if (good == "1")
                                            {
                                                InitLoad();
                                                this._presenter = new OrderTrackingPresenter(this);
                                                _presenter.GetPatientID(track);

                                                OrderTrackingOrderEntity NewPatient = (from PatientID in PatientItem select PatientID).FirstOrDefault();
                                                if (NewPatient.PatientID != 0) { this.PatientID = NewPatient.PatientID; }


                                                this._presenter = new OrderTrackingPresenter(this);
                                                _presenter.GetTrackedOrders(this.PatientID, SiteCode);
                                                gvEditTracking.DataSource = TrackingOrderPatientList;
                                                gvEditTracking.DataBind();
                                                ///set another if condition to check if = 1 "Success" else "error saving"
                                                txtTrackingNumberS.Focus();
                                                //ShowConfirmDialog(String.Format("Tracking for OrderNumber {0} was saved successfully.", track));

                                            }
                                            else
                                            {
                                                ///Error Saving
                                                ShowErrorDialog("There was an error saving this tracking information.  Please try again.");
                                            }
                                        }
                                        else
                                        {
                                            ///Order number does not match patient
                                            ShowErrorDialog(String.Format("OrderNumber {0} does not match patient already saved for this tracking label.", track));
                                        }

                                    }

                                }
                                else
                                {
                                    ///Max 4 met send error message
                                    ShowErrorDialog(String.Format("Order Number {0} was not saved because Max [4] was reached.", track));

                                }
                            }
                            else
                            {
                                ///duplicate / Functionals said ignore
                                txtTrackingNumberS.Focus();
                                return;
                            }


                        }

                        break;
                    }

                case "ups":
                    {

                        if (String.IsNullOrEmpty(HttpContext.Current.Session["TrackingScan"].ToString()))
                        {
                            this.TrackingScan = track;
                            this.Shipper = "UPS";
                            ddlShippingProvider.SelectedValue = this.Shipper;
                            ClearOrders();
                        }
                        else
                        {
                            return;
                        }

                        break;
                    }
                case "usps":
                    {
                        if (String.IsNullOrEmpty(HttpContext.Current.Session["TrackingScan"].ToString()))
                        {
                            this.TrackingScan = track;
                            this.Shipper = "USPS";
                            ddlShippingProvider.SelectedValue = this.Shipper;
                            ClearOrders();
                        }
                        else
                        {
                            return;
                        }

                        break;
                    }
                case "fedex":
                    {
                        if (String.IsNullOrEmpty(HttpContext.Current.Session["TrackingScan"].ToString()))
                        {
                            this.TrackingScan = track;
                            this.Shipper = "FEDEX";
                            ddlShippingProvider.SelectedValue = this.Shipper;
                            ClearOrders();
                        }
                        else
                        {
                            return;
                        }

                        break;
                    }
                case "invalid":
                    {
                        ///send error message "value scanned was invalid.  please try again"
                        ShowErrorDialog("Value scanned was invalid.  Please try again");
                        this.txtTrackingNumberS.Focus();
                        break;
                    }
                default:
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "StatusAlert", "alert('Invalid search criteria or no results found.');", true);
                    break;
            }
            this.txtTrackingNumberS.Focus();
        }

        /// <summary>
        /// Manual Tracking entry for orders.  logic for validating and saving tracking information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!(String.IsNullOrEmpty(this.txtTrackingNumberM.Text.Trim())))
            {
                string scan = GetScanString(this.txtTrackingNumberM.Text.Trim());
                if (scan == "USPS" || scan == "UPS" || scan == "FEDEX")
                {
                    this._presenter = new OrderTrackingPresenter(this);
                    _presenter.GetTrackingOrderScan(this.txtTrackingNumberM.Text.Trim());
                    if (TrackingScanList.Count != 0)
                    {
                        ///Error Saving
                        ShowErrorDialog(String.Format("Tracking Number {0} already exist in the database and cannot be saved.", this.txtTrackingNumberM.Text.Trim()));
                        return;
                    }
                }
            }
            if (Validate())
            {
                var Message = string.Empty;
                OrderTrackingScanEntity ScanEntity = new OrderTrackingScanEntity();

                ScanEntity.TrackingNumber = this.TrackingScan;
                this.Shipper = ddlShippingProvider.SelectedValue;
                string[] str = { txtOrder1.Text.Trim(), txtOrder2.Text.Trim(), txtOrder3.Text.Trim(), txtOrder4.Text.Trim() };
                var Orders = String.Join(",", str);
                this._presenter = new OrderTrackingPresenter(this);
                var good = _presenter.InsertTrackingNumberManual(ScanEntity, Orders, this.Shipper);

                if (!String.IsNullOrEmpty(good))
                {
                    if (good == "1")
                    {
                        this._presenter = new OrderTrackingPresenter(this);
                        _presenter.GetPatientID(this.txtOrder1.Text.Trim());
                        OrderTrackingOrderEntity Patient = (from PatientID in PatientItem select PatientID).FirstOrDefault();
                        this.PatientID = Patient.PatientID;

                        this._presenter = new OrderTrackingPresenter(this);
                        _presenter.GetTrackedOrders(Patient.PatientID, SiteCode);
                        gvEditTracking.DataSource = TrackingOrderPatientList;
                        gvEditTracking.DataBind();
                        ClearOrders();
                        Session["PatientID"] = string.Empty;
                        Session["Shipper"] = string.Empty;
                        Session["TrackingScan"] = string.Empty;
                        txtTrackingNumberM.Text = "";
                        txtOrder1.Text = "";
                        txtOrder2.Text = "";
                        txtOrder3.Text = "";
                        txtOrder4.Text = "";
                        txtTrackingNumberM.Focus();
                        ///set another if condition to check if = 1 "Success" else "error saving"
                        ShowConfirmDialog("Tracking for orders were saved successfully.");

                    }
                    else if (good == "0")
                    {
                        ///Error Saving
                        ShowErrorDialog("There was an error saving this tracking information.  Please try again.");
                    }
                    else
                    {
                        ///wrong patient or Not ship to patient
                        ShowErrorDialog(good + ".  Please correct entry and try again.");
                    }
                }
            }
        }

        /// <summary>
        /// Row commands for clickable items inside the tables 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTracking_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string commandArgument = null;
            string[] arguments = null;
            var f = default(Int32);
            if (!Int32.TryParse(e.CommandArgument.ToString(), out f))
            {
                try
                {
                    commandArgument = e.CommandArgument.ToString();
                    arguments = commandArgument.Split(',');
                }
                catch (Exception ex)
                {
                    return;
                }
            }
            else
            {
                commandArgument = e.CommandArgument.ToString();
            }

            switch (e.CommandName.ToLower())
            {
                case "editrecord":

                    if (TrackingOrderPatientList.Count > 0)
                    {
                        // Get the selected row index
                        var gvr = ((GridViewRow)((ImageButton)e.CommandSource).NamingContainer);
                        var i = gvr.RowIndex;

                        String AddTracking = Convert.ToString(commandArgument);

                        if (AddTracking != string.Empty)
                        {
                            this.TrackingScan = AddTracking;
                            UpdateSession("Load");

                            // Clear other highlighed rows
                            ((GridView)sender).Rows.Cast<GridViewRow>().ToList().ForEach(x => x.BackColor = System.Drawing.Color.Empty);

                            // Highlight the selected row
                            gvr.BackColor = System.Drawing.ColorTranslator.FromHtml("#A1DCF2");

                            TrackingEditPanel.Visible = true;

                            udpTrackOrders.Update();

                        }
                    }
                    break;

                case "deleterecord":

                    if (TrackingOrderPatientList.Count > 0)
                    {
                        // Get the selected row index
                        var gvr = ((GridViewRow)((ImageButton)e.CommandSource).NamingContainer);
                        var i = gvr.RowIndex;

                        String DeleteOrderTracking = Convert.ToString(commandArgument);

                        if (DeleteOrderTracking != string.Empty)
                        {
                            OrderTrackingScanEntity ScanEntity = new OrderTrackingScanEntity();
                            ///load the SP by the tracking number and set the session variables
                            this._presenter = new OrderTrackingPresenter(this);
                            _presenter.GetTrackingOrderScan(this.TrackingScan);
                            if (TrackingScanList.Count != 0)
                            {
                                OrderTrackingScanEntity Tracking = (from TrackingNumber in TrackingScanList select TrackingNumber).FirstOrDefault();
                                OrderTrackingScanEntity Patient = (from PatientID in TrackingScanList select PatientID).FirstOrDefault();

                                this.PatientID = Patient.PatientID;
                                this.Shipper = GetScanString(this.TrackingScan);

                            }
                            else
                            {
                                ShowErrorDialog("Error occured trying to load tracking information. Please try again.");
                            }
                            ScanEntity.TrackingNumber = this.TrackingScan;
                            ScanEntity.PatientID = this.PatientID;

                            this._presenter = new OrderTrackingPresenter(this);
                            var good = _presenter.InsertUpdateTrackingNumber(ScanEntity, DeleteOrderTracking, this.Shipper, "D");
                            if (good != "WP")
                            {

                                _presenter.GetTrackedOrders(this.PatientID, SiteCode);
                                UpdateSession("Delete");
                                InitLoad();
                                gvEditTracking.DataSource = TrackingOrderPatientList;
                                gvEditTracking.DataBind();

                                ///set another if condition to check if = 1 "Success" else "error saving"
                                ShowConfirmDialog(String.Format("Tracking for OrderNumber {0} was Deleted successfully.", DeleteOrderTracking));
                            }
                            else
                            {
                                ///Order number does not match patient
                                ShowErrorDialog("An error occured trying to delete tracking information for this order.  Please try again");
                            }

                            // Clear other highlighed rows
                            ((GridView)sender).Rows.Cast<GridViewRow>().ToList().ForEach(x => x.BackColor = System.Drawing.Color.Empty);

                            TrackingEditPanel.Visible = true;
                            this.rblInputType.SelectedValue = "Scan";

                            udpTrackOrders.Update();

                        }
                    }
                    break;

                case "viewrecords":
                    if (TrackingBaseList.Count > 0)
                    {
                        // Get the selected row index
                        var gvr = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer);
                        var i = gvr.RowIndex;

                        String Tracking = Convert.ToString(commandArgument);

                        if (Tracking != string.Empty)
                        {
                            this.TrackingScan = Tracking;
                            this._presenter = new OrderTrackingPresenter(this);
                            _presenter.GetTrackingOrderScan(Tracking);
                            OrderTrackingScanEntity Patient = (from PatientID in TrackingScanList where PatientID.TrackingNumber == Tracking select PatientID).FirstOrDefault();

                            this._presenter = new OrderTrackingPresenter(this);
                            _presenter.GetTrackedOrders(Patient.PatientID, SiteCode);
                            gvEditTracking.DataSource = TrackingOrderPatientList;
                            gvEditTracking.DataBind();
                            // Clear other highlighed rows
                            ((GridView)sender).Rows.Cast<GridViewRow>().ToList().ForEach(x => x.BackColor = System.Drawing.Color.Empty);

                            // Highlight the selected row
                            gvr.BackColor = System.Drawing.ColorTranslator.FromHtml("#A1DCF2");

                            TrackingEditPanel.Visible = true;

                            udpTrackOrders.Update();

                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Radio Button selection changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rblInputType_TextChanged(object sender, EventArgs e)
        {

            //HtmlGenericControl conScan = (HtmlGenericControl)FindControl("divPanelScan");
            //HtmlGenericControl conManual = (HtmlGenericControl)FindControl("divPanelManual");
            if (rblInputType.SelectedValue == "Manual")
            {
                this.pnlManual.Visible = true;
                this.pnlScan.Visible = false;
                //conManual.Attributes.Add("style", "display: block");
                //conScan.Attributes.Add("style", "display: none");

            }
            else
            {
                this.pnlManual.Visible = false;
                this.pnlScan.Visible = true;

            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Clear Order Session Variable
        /// </summary>
        private void ClearOrders()
        {
            Session["OrderOne"] = string.Empty;
            Session["OrderTwo"] = string.Empty;
            Session["OrderThree"] = string.Empty;
            Session["OrderFour"] = string.Empty;

        }

        /// <summary>
        /// update the session variables 
        /// </summary>
        /// <param name="what"></param>
        private void UpdateSession(string what)
        {
            ///load the SP by the tracking number and set the session variables
            this._presenter = new OrderTrackingPresenter(this);
            _presenter.GetTrackingOrderScan(this.TrackingScan);
            if (TrackingScanList.Count != 0)
            {
                OrderTrackingScanEntity Tracking = (from TrackingNumber in TrackingScanList select TrackingNumber).FirstOrDefault();
                OrderTrackingScanEntity Patient = (from PatientID in TrackingScanList select PatientID).FirstOrDefault();
                OrderTrackingScanEntity OrderOne = (from Order1 in TrackingScanList select Order1).FirstOrDefault();
                OrderTrackingScanEntity OrderTwo = (from Order2 in TrackingScanList select Order2).FirstOrDefault();
                OrderTrackingScanEntity OrderThree = (from Order3 in TrackingScanList select Order3).FirstOrDefault();
                OrderTrackingScanEntity OrderFour = (from Order4 in TrackingScanList select Order4).FirstOrDefault();

                this.TrackingScan = Tracking.TrackingNumber;
                this.PatientID = Patient.PatientID;
                this.Shipper = GetScanString(this.TrackingScan);
                this.OrderOne = OrderOne.Order1;
                this.OrderTwo = OrderTwo.Order2;
                this.OrderThree = OrderThree.Order3;
                this.OrderFour = OrderFour.Order4;
                if (what == "Load")
                {
                    ShowConfirmDialog(String.Format("Tracking Number {0} was successfully loaded.  Please scan order you want to add.", this.TrackingScan));

                    rblInputType.SelectedIndex = 0;
                    this.pnlManual.Visible = false;
                    this.pnlScan.Visible = true;
                    udpTrackOrders.Update();
                    txtTrackingNumberS.Focus();

                }
            }

        }

        /// <summary>
        /// Displays SRTSweb Confirm message dialog
        /// </summary>
        /// <param name="msg">Message to be displayed to the user in the dialog.</param>
        private void ShowConfirmDialog(String msg)
        {
            /// Show global confirm dialog
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Success!','" + msg + "', 'success');", true);
        }

        /// <summary>
        /// Displays SRTSweb error message dialog
        /// </summary>
        /// <param name="msg">Message to be displayed to the user in the dialog.</param>
        private new void ShowErrorDialog(String msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Error!','" + msg + "', 'error');", true);
        }

        #endregion

        #region FUNCTION

        /// <summary>
        /// Get value of open orders remaining
        /// </summary>
        /// <returns></returns>
        protected int GetOrderCount(string order)
        {
            if (String.IsNullOrEmpty(HttpContext.Current.Session["OrderOne"].ToString()))
            {
                this.OrderOne = order;
                return 1;
            }
            else if (String.IsNullOrEmpty(HttpContext.Current.Session["OrderTwo"].ToString()))
            {
                this.OrderTwo = order;
                return 2;
            }
            else if (String.IsNullOrEmpty(HttpContext.Current.Session["OrderThree"].ToString()))
            {
                this.OrderThree = order;
                return 3;
            }
            else if (String.IsNullOrEmpty(HttpContext.Current.Session["OrderFour"].ToString()))
            {
                this.OrderFour = order;
                return 4;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// check if order was already scanned
        /// </summary>
        /// <returns></returns>
        protected int CheckOrderValue(string order)
        {
            if (HttpContext.Current.Session["OrderOne"].ToString() == order)
            {
                return 1;
            }
            else if (HttpContext.Current.Session["OrderTwo"].ToString() == order)
            {
                return 2;
            }
            else if (HttpContext.Current.Session["OrderThree"].ToString() == order)
            {
                return 3;
            }
            else if (HttpContext.Current.Session["OrderFour"].ToString() == order)
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Return Error Message for Invalid Fields on Form
        /// </summary>
        protected new Boolean Validate()
        {
            try
            {
                var NotValid = false;

                if (String.IsNullOrEmpty(this.txtTrackingNumberM.Text.Trim()))
                {
                    ///tracking number was not entered
                    ShowErrorDialog("Tracking number is required.  Please enter a valid tracking number before submitting.");
                    return NotValid;
                }
                if (String.IsNullOrEmpty(this.txtOrder1.Text.Trim()))
                {
                    ///Order number was not entered
                    ShowErrorDialog("Order one is required.  Please enter a valid order number before submitting.");
                    return NotValid;
                }
                if (!(String.IsNullOrEmpty(this.txtTrackingNumberM.Text.Trim())))
                {
                    var track = this.txtTrackingNumberM.Text.Trim();
                    if (track.Length < 12)
                    {
                        ///tracking number was not long enough to be testing
                        ShowErrorDialog("Tracking number is invalid.  Please enter a valid tracking number before submitting.");
                        return NotValid;
                    }

                    string scan = GetScanString(track);

                    if (scan == "OrderNumber" || scan == "Invalid")
                    {
                        ShowErrorDialog("Tracking number is invalid.  Please enter a valid tracking number before submitting.");
                        return NotValid;
                    }
                    else if (scan != this.ddlShippingProvider.SelectedValue)
                    {
                        ShowErrorDialog(String.Format("Tracking number is {0} and does not match shipping selected.  Please select correct shipping provider.", scan));
                        return NotValid;
                    }
                    else { this.TrackingScan = track; this.Shipper = scan; }
                }
                if (!(String.IsNullOrEmpty(this.txtOrder1.Text.Trim())))
                {
                    var _orderOne = this.txtOrder1.Text.Trim();
                    string scan1 = GetScanString(_orderOne);

                    if (scan1 != "OrderNumber" || scan1 == "Invalid")
                    {
                        ShowErrorDialog("Order one is invalid.  Please enter a valid order number one before submitting.");
                        return NotValid;
                    }
                    else { this.OrderOne = _orderOne; }
                }
                if (!(String.IsNullOrEmpty(this.txtOrder2.Text.Trim())))
                {
                    var _orderTwo = this.txtOrder2.Text.Trim();
                    string scan2 = GetScanString(_orderTwo);

                    if (scan2 != "OrderNumber" || scan2 == "Invalid")
                    {
                        ShowErrorDialog("Order two is invalid.  Please enter a valid order number two before submitting.");
                        return NotValid;
                    }
                    else { this.OrderTwo = _orderTwo; }
                }
                if (!(String.IsNullOrEmpty(this.txtOrder3.Text.Trim())))
                {
                    var _orderThree = this.txtOrder3.Text.Trim();
                    string scan3 = GetScanString(_orderThree);

                    if (scan3 != "OrderNumber" || scan3 == "Invalid")
                    {
                        ShowErrorDialog("Order three is invalid.  Please enter a valid order number three before submitting.");
                        return NotValid;
                    }
                    else { this.OrderThree = _orderThree; }
                }
                if (!(String.IsNullOrEmpty(this.txtOrder4.Text.Trim())))
                {
                    var _orderFour = this.txtOrder4.Text.Trim();
                    string scan4 = GetScanString(_orderFour);

                    if (scan4 != "OrderNumber" || scan4 == "Invalid")
                    {
                        ShowErrorDialog("Order four is invalid.  Please enter a valid order number four before submitting.");
                        return NotValid;
                    }
                    else { this.OrderFour = _orderFour; }
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Find Scan string if Valid
        /// </summary>
        /// <returns></returns>
        protected string GetScanString(string Scan)
        {
            string ScanString = "Invalid";

            var Order = Regex.Match(Scan, @"[0-9]{13}-[0-9]{2}");
            if (Order.Success) { ScanString = "OrderNumber"; return ScanString; }
            Match ups1 = Regex.Match(Scan, @"\b(1Z ?[0-9A-Z]{3} ?[0-9A-Z]{3} ?[0-9A-Z]{2} ?[0-9A-Z]{4} ?[0-9A-Z]{3} ?[0-9A-Z]|[\dT]\d\d\d ?\d\d\d\d ?\d\d\d)\b");
            if (ups1.Success) { ScanString = "UPS"; return ScanString; }
            Match ups2 = Regex.Match(Scan, @"^[kKJj]{1}[0-9]{10}$");
            if (ups2.Success) { ScanString = "UPS"; return ScanString; }
            Match usps1 = Regex.Match(Scan, @"(\b\d{30}\b)|(\b91\d+\b)|(\b\d{20}\b)");
            if (usps1.Success) { ScanString = "USPS"; return ScanString; }
            Match usps2 = Regex.Match(Scan, @"(\b\d{30}\b)|(\b91\d+\b)|(\b\d{20}\b)|(\b\d{26}\b)| ^E\D{1}\d{9}\D{2}$|^9\d{15,21}$| ^91[0-9]+$| ^[A-Za-z]{2}[0-9]+US$", RegexOptions.IgnoreCase);
            if (usps2.Success) { ScanString = "USPS"; return ScanString; }
            Match usps3 = Regex.Match(Scan, @"^E\D{1}\d{9}\D{2}$|^9\d{15,21}$");
            if (usps3.Success) { ScanString = "USPS"; return ScanString; }
            Match usps4 = Regex.Match(Scan, @"^ 91[0 - 9] +$");
            if (usps4.Success) { ScanString = "USPS"; return ScanString; }
            Match usps5 = Regex.Match(Scan, @"^[A-Za-z]{2}[0-9]+US$");
            if (usps5.Success) { ScanString = "USPS"; return ScanString; }
            Match usps6 = Regex.Match(Scan, @"(\b\d{30}\b)|(\b91\d+\b)|(\b\d{20}\b)|(\b\d{26}\b)| ^E\D{1}\d{9}\D{2}$|^9\d{15,21}$| ^91[0-9]+$| ^[A-Za-z]{2}[0-9]+US$", RegexOptions.IgnoreCase);
            if (usps6.Success) { ScanString = "USPS"; return ScanString; }
            Match FedEx1 = Regex.Match(Scan, @"(\b96\d{20}\b)|(\b\d{15}\b)|(\b\d{12}\b)");
            if (FedEx1.Success) { ScanString = "FEDEX"; return ScanString; }
            Match FedEx2 = Regex.Match(Scan, @"\b((98\d\d\d\d\d?\d\d\d\d|98\d\d) ?\d\d\d\d ?\d\d\d\d( ?\d\d\d)?)\b");
            if (FedEx2.Success) { ScanString = "FEDEX"; return ScanString; }
            Match FedEx3 = Regex.Match(Scan, @"^[0-9]{15}$");
            if (FedEx3.Success) { ScanString = "FEDEX"; return ScanString; }

            return ScanString;
        }

        #endregion

        #region INTERFACE

        /// <summary>
        /// SiteCode
        /// </summary>
        public String SiteCode
        {
            get { return Session["SiteCode"].ToString(); }
            set { Session["SiteCode"] = value; }
        }

        /// <summary>
        /// Shipping Agent
        /// </summary>
        public String Shipper
        {
            get { return Session["Shipper"].ToString(); }
            set { Session["Shipper"] = value; }
        }

        /// <summary>
        /// Tracking Scanned
        /// </summary>
        public String TrackingScan
        {
            get { return Session["TrackingScan"].ToString(); }
            set { Session["TrackingScan"] = value; }
        }

        /// <summary>
        /// Order Number One
        /// </summary>
        public String OrderOne
        {
            get { return Session["OrderOne"].ToString(); }
            set { Session["OrderOne"] = value; }
        }

        /// <summary>
        /// Order Number Two
        /// </summary>
        public String OrderTwo
        {
            get { return Session["OrderTwo"].ToString(); }
            set { Session["OrderTwo"] = value; }
        }

        /// <summary>
        /// Order Number Three
        /// </summary>
        public String OrderThree
        {
            get { return Session["OrderThree"].ToString(); }
            set { Session["OrderThree"] = value; }
        }

        /// <summary>
        /// Order Number Four
        /// </summary>
        public String OrderFour
        {
            get { return Session["OrderFour"].ToString(); }
            set { Session["OrderFour"] = value; }
        }

        /// <summary>
        /// Get PatientID
        /// </summary>
        public int PatientID
        {
            get { return Session["PatientID"].ToInt32(); }
            set { Session["PatientID"] = value; }
        }

        /// <summary>
        /// List of Site Tracked Orders
        /// </summary>
        public List<OrderTrackingOrderEntity> PatientItem
        {
            get
            {
                return ViewState["PatientItem"] as List<OrderTrackingOrderEntity>;
            }
            set
            {
                ViewState["PatientItem"] = value;
            }
        }

        /// <summary>
        /// List of Site Tracked Orders
        /// </summary>
        public List<OrderTrackingEntity> TrackingBaseList
        {
            get
            {
                return ViewState["SiteTrackingList"] as List<OrderTrackingEntity>;
            }
            set
            {
                ViewState["SiteTrackingList"] = value;
            }
        }

        /// <summary>
        /// List of Patient Tracked Order
        /// </summary>
        public List<OrderTrackingPatientEntity> TrackingOrderPatientList
        {
            get
            {
                return ViewState["PatientTrackingList"] as List<OrderTrackingPatientEntity>;
            }
            set
            {
                ViewState["PatientTrackingList"] = value;
            }
        }

        /// <summary>
        /// List of Scanned order
        /// </summary>
        public List<OrderTrackingScanEntity> TrackingScanList
        {
            get
            {
                return ViewState["TrackingScanList"] as List<OrderTrackingScanEntity>;
            }
            set
            {
                ViewState["TrackingScanList"] = value;
            }
        }

        /// <summary>
        /// PatientID
        /// </summary>
        public List<OrderTrackingOrderEntity> TrackingPatientID
        {
            get
            {
                return ViewState["TrackingPatient"] as List<OrderTrackingOrderEntity>;
            }
            set
            {
                ViewState["TrackingPatient"] = value;
            }
        }

        #endregion

    }
}