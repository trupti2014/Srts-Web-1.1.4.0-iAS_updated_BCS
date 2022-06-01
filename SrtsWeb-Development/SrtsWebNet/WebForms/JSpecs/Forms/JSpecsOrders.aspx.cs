using System;
using System.Data;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.JSpecs;
using SrtsWeb.Views.JSpecs;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace JSpecs.Forms
{

    public partial class JSpecsOrders : PageBaseJSpecs, IJSpecsOrdersView
    { 

        private JSpecsOrdersPresenter _presenter;

        public JSpecsOrders()
        {
            _presenter = new JSpecsOrdersPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userInfo"] != null)
            {
                if (!IsPostBack)
                {
                    _presenter.InitView();
                }
                cantOrderMsg.Visible = false;
                filterIncompleteOrders();
            }
            else
            {
                Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
            }
        }

        public void filterIncompleteOrders()
        {
            var dataSource = (List<JSpecsOrderDisplayEntity>)jspecsOrders.DataSource;

            if (dataSource != null)
            {
                for (int i = 0; i < jspecsOrders.Items.Count; i++)
                {
                    if (dataSource[i].OrderStatus.ToString() == "INCOMPLETE")
                    {
                        jspecsOrders.Items[i].Visible = false;
                    }
                }
            }
        }

        public string setThumbnailImage (string image)
        {
            if(image == "/JSpecs/imgs/Fallback/glasses.svg")
            {
                return "/JSpecs/imgs/Fallback/Frame_Not_Available.png";
            }
            else
            {
                return image;
            }
        }

        /// <summary>
        ///  Displays the order details popup modal.
        /// </summary>
        /// <param name="sender">Details button that was clicked</param>
        /// <param name="e"></param>
        protected void btnDisplayOrderDetails_click(object sender, EventArgs e)
        {
            string[] commandArgs = new string[2];
            string orderNumber = string.Empty;
            bool orderAvailable = false;
            string frameImagePath = string.Empty;
            string frameImageName = string.Empty;
            string frameImageType = string.Empty;
            LinkButton btn = (LinkButton)(sender);

            commandArgs = btn.CommandArgument.ToString().Split(';');
            orderNumber = commandArgs[0];
            orderAvailable = Convert.ToBoolean(commandArgs[1]);

            if (SetOrderDetailsFields(orderNumber, orderAvailable))
            {
                MPEOrderDetails.Show();
            }
            else
            {
                ErrorMessage = @"We\'re sorry. We cannot find your order at this time.";
            }
        }

        /// <summary>
        /// Set Order Details fields based on order selected.
        /// </summary>
        /// <param name="orderIndex">Index of selected order</param>
        /// <returns>True if order existed, and fields were set.</returns>
        private bool SetOrderDetailsFields(string orderNumber, bool orderAvailable)
        {
            JSpecsOrderDetailsDisplayEntity order = _presenter.GetUsersDisplayOrder(orderNumber);

                // Order Frame Details
                orderPlaced.Text = order.DateLastModified.ToString("M/d/yyyy");
                if (!order.FrameImgName.IsNullOrEmpty() && !order.FrameImgPath.IsNullOrEmpty() && !order.FrameImgType.IsNullOrEmpty())
                {
                    modalImg.Src = "/JSpecs/" + order.FrameImgPath + order.FrameImgName + "." + order.FrameImgType;

                    if (modalImg.Src == "/JSpecs/imgs/Fallback/glasses.svg")
                    {
                        modalImg.Src = "/JSpecs/imgs/Fallback/Frame_Not_Available.png";
                    }
                }

                modalFrameDetails.InnerText = order.FrameCode + ", " + order.FrameColor + ", " + order.FrameEyeSize + "-" + order.FrameBridgeSize + "-" + order.FrameTempleType;

                // Order Details
                orderPlaced2.Text = order.DateLastModified.ToString("M/d/yyyy");
                prescriptionDate.Text = order.PrescriptionDate.ToString("M/d/yyyy");
                rxName.Text = order.RxNameUserFriendly;
                odSph.InnerText = order.ODSphere;
                odCyl.InnerText = order.ODCylinder;
                odAxis.InnerText = order.ODAxis;
                odAdd.InnerText = order.ODAdd;
                osSph.InnerText = order.OSSphere;
                osCyl.InnerText = order.OSCylinder;
                osAxis.InnerText = order.OSAxis;
                osAdd.InnerText = order.OSAdd;
                pd.Text = Decimal.ToInt32(order.PDDistant).ToString();
                orderStatus.Text = order.OrderStatus;


                AddressEntity currentAddress = order.Address;
                if (currentAddress.Address2 == "")
                {
                mailingAddress.Text = currentAddress.Address1 + ", " + currentAddress.City + ", " + currentAddress.State + " " + currentAddress.ZipCode;
                // Console.WriteLine("Order details: " + mailingAddress.Text + " " + DateTime.Now);
                }
                else
                {
                    mailingAddress.Text = order.Address.FormattedAddress;
                }

                //OrderDetailsTracking.Text = ; FUNCTIONALITY NOT IMPLIMENTED YET

                if (orderAvailable)
                {
                    lbtnModalOrderAgain.CommandArgument = orderNumber + ";" + orderAvailable;
                }
                else
                {
                    lbtnModalOrderAgain.Enabled = false;
                }

            return true;
        }

        /// <summary>
        /// This function sets the PatientOrder, and redirects the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReOrder_Click(object sender, EventArgs e)
        {
            string[] commandArgs = new string[2];
            string orderNumber = string.Empty;
            bool orderAvailable = false;
            LinkButton btn = (LinkButton)(sender);

            commandArgs = btn.CommandArgument.ToString().Split(';');
            orderNumber = commandArgs[0];
            orderAvailable = Convert.ToBoolean(commandArgs[1]);

            if (orderAvailable)
            {
                userInfo.JSpecsOrder = new JSpecsOrder();
                userInfo.JSpecsOrder.OrderID = orderNumber;
                Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsDetails.aspx");
            }
            else
            {
                //ErrorMessage = @"It appears that you are unable to order these frames at this time.";
                cantOrderMsg.Visible = true;
            }
        }

        /// <summary>
        /// Clears order details fields.
        /// </summary>
        private void ClearOrderDetailsFields()
        {
            // Order Frame Details
            orderPlaced.Text = string.Empty;
            modalImg.Src = "/JSpecs/svg/glasses.svg";
            modalFrameDetails.InnerText = string.Empty;

            // Order Details
            orderPlaced2.Text = string.Empty;
            prescriptionDate.Text = string.Empty;
            odSph.InnerText = string.Empty;
            odCyl.InnerText = string.Empty;
            odAxis.InnerText = string.Empty;
            odAdd.InnerText = string.Empty;
            osSph.InnerText = string.Empty;
            osCyl.InnerText = string.Empty;
            osAxis.InnerText = string.Empty;
            osAdd.InnerText = string.Empty;
            pd.Text = string.Empty;
            orderStatus.Text = string.Empty;
            mailingAddress.Text = string.Empty;
            //OrderDetailsTracking.Text = string.Empty; FUNCTIONALITY NOT IMPLIMENTED YET

            lbtnModalOrderAgain.Enabled = true;
            lbtnModalOrderAgain.CommandArgument = string.Empty;
        }

        /// <summary>
        /// Closes the modal.
        /// </summary>
        /// <param name="sender">Order Details Close Button</param>
        /// <param name="e"></param>
        protected void btnCloseOrderDetails_click(object sender, EventArgs e)
        {
            ClearOrderDetailsFields();
            MPEOrderDetails.Hide();
        }

        /// <summary>
        /// Filter all the users orders based on eligibility.
        /// AD/activated reservists can re order any si, pmi or eyepro insert if available.
        /// AD/activated reservists if there are no FOC orders in the last 11 months, then FOC should have a reorder available if frame available
        /// AD/Activated reservists if there is a FOC order in last 11 months, no FOC should have an available order.
        /// Retirees only stand issue frames should have reorder. Only if not order in previous 11 months should SI frames have a re order if frame is available
        /// </summary>
        /// <param name="orders">List of users orders</param>
        /// <returns></returns>
        private List<JSpecsOrderDisplayEntity> FilterByEligibility(List<JSpecsOrderDisplayEntity> orders)
        {
            DateTime today = DateTime.UtcNow;
            int fourYears = 1460;
            int elevenMonths = 334;
            bool isThereARecentFOCOrder;

            // Remove orders > four years old, and sort in descending order from most recent
            orders.Where(order => (today - order.DateCreated).TotalDays < fourYears);

            // Check if FOC order in last 11 months
            isThereARecentFOCOrder = orders.FirstOrDefault(order => order.FrameCategory != "PMI" && (today - order.DateCreated).TotalDays < elevenMonths) != null ? true : false;

            for (int i = 0; i < orders.Count; i++)
            {
                if (userInfo.Patient.Individual.StatusDescription == "Active Duty")
                {
                    if(orders[i].FrameCategory == "PMI" && orders[i].FrameIsActive || orders[i].FrameCategory != "PMI" && !isThereARecentFOCOrder && orders[i].FrameIsActive)
                    {
                        orders[i].EligibleOrder = true;
                    }
                    else
                    {
                        orders[i].EligibleOrder = false;
                    }
                }
                else if (userInfo.Patient.Individual.StatusDescription == "Retired"
                        && orders[i].FrameCategory == "Standard"
                        && (today - orders[0].DateCreated).TotalDays > elevenMonths)
                {
                    orders[i].EligibleOrder = true;
                }
                else
                {
                    orders[i].EligibleOrder = false;
                }
            }

            return orders;
        }

        #region JSpecsOrdersView members
        public JSpecsSession userInfo
        {
            get { return (JSpecsSession)Session["userInfo"]; }
            set { Session.Add("userInfo", value); }
        }

        private List<JSpecsOrderDisplayEntity> _ordersData;

        public List<JSpecsOrderDisplayEntity> OrdersData
        {
            get { return _ordersData; }
            set
            {
                _ordersData = value;

                if (_ordersData.Count == 0)
                {
                    ErrorMessage = @"It appears that you\'re not eligible to order with the app at this time. Have a question? Check out our FAQ page or contact your optometry Clinic.";
                    return;
                }
                else
                {
                    _ordersData = _ordersData.OrderByDescending(order => order.DateCreated).ToList();
                    _ordersData = FilterByEligibility(_ordersData);
                }

                jspecsOrders.DataSource = _ordersData;
                jspecsOrders.DataBind();
            }
        }

        public string ErrorMessage
        {
            set
            {
                ShowMessage_Redirect(this.Page, value, "/WebForms/JSpecs/Forms/JSpecsFAQ.aspx");
            }
        }
        #endregion JSpecsOrderView members
    }
}