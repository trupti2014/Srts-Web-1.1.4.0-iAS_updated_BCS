using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarcodeLib;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.JSpecs;

namespace SrtsWeb.Presenters.JSpecs
{
    public sealed class JSpecsConfirmOrderPresenter : PresenterBase
    {
        private IJSpecsConfirmOrderView _view;

        public JSpecsConfirmOrderPresenter(IJSpecsConfirmOrderView view)
        {
            _view = view;
        }

        public void InitView()
        {

        }

        public bool SubmitOrder()
        {



            #if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.JSPECSSource, " JSpecsConfirmOrderPresenter_SubmitOrder", _view.userInfo.Patient.Individual.IDNumber))
            #endif
            {
            

                OrderRepository _orderRepository = new OrderRepository();
                OrderEntity orderToSubmit = _view.userInfo.JSpecsOrder.PatientOrder;

                // Set address as active
                AddressRepository _addressRepository = new AddressRepository();
                _view.userInfo.JSpecsOrder.OrderAddress.IsActive = true;
                _addressRepository.UpdateAddress(_view.userInfo.JSpecsOrder.OrderAddress);

                // Set email as active
                EMailAddressRepository _emailAddressRepository = new EMailAddressRepository();
                _view.userInfo.JSpecsOrder.OrderEmailAddress.IsActive = true;
                _emailAddressRepository.UpdateEMailAddress(_view.userInfo.JSpecsOrder.OrderEmailAddress);

                // Order Number, disbursment, clinic site code, and lab site code
                orderToSubmit.OrderNumber = GetNextOrderNumber();
                orderToSubmit.OrderDisbursement = "L2P";
                orderToSubmit.ClinicSiteCode = this.JSpecsSiteCode;
                orderToSubmit.LabSiteCode = GetLabSiteCode(orderToSubmit.FrameCode);

                // Demographics
                string demographic = orderToSubmit.Demographic.Substring(0, 7) + "R";  // All G-Eyes orders are to be "READINESS" priority
                orderToSubmit.Demographic = demographic;

                // Prescription
                orderToSubmit.PrescriptionID = _view.userInfo.JSpecsOrder.OrderPrescription.ID;

                // Address
                orderToSubmit.ShipAddress1 = _view.userInfo.JSpecsOrder.OrderAddress.Address1;
                orderToSubmit.ShipAddress2 = _view.userInfo.JSpecsOrder.OrderAddress.Address2;
                orderToSubmit.ShipAddress3 = _view.userInfo.JSpecsOrder.OrderAddress.Address3;
                orderToSubmit.ShipZipCode = _view.userInfo.JSpecsOrder.OrderAddress.ZipCode;
                orderToSubmit.ShipCity = _view.userInfo.JSpecsOrder.OrderAddress.City;
                orderToSubmit.ShipState = _view.userInfo.JSpecsOrder.OrderAddress.State;
                orderToSubmit.ShipCountry = _view.userInfo.JSpecsOrder.OrderAddress.Country;
                orderToSubmit.ShipAddressType = _view.userInfo.JSpecsOrder.OrderAddress.AddressType;

                // Email
                orderToSubmit.CorrespondenceEmail = _view.userInfo.JSpecsOrder.OrderEmailAddress.EMailAddress;

                // Submitted by
                orderToSubmit.ModifiedBy = "JSPECS-SELF";

                // reset fields
                orderToSubmit.UserComment1 = string.Empty;
                orderToSubmit.UserComment2 = string.Empty;
                orderToSubmit.IsGEyes = false;
                orderToSubmit.LinkedID = string.Empty;

                try
                {
                    if (!_orderRepository.InsertOrder(orderToSubmit, true))
                    {
                        // Unable to save order
                        return false;
                    }
                    else
                    {
                        // Add bar code to order
                        AddBarCodeToOrder(orderToSubmit);
                    }
                }
                catch (Exception ex) // error saving order
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return false;
                }

                // clear order
                _view.userInfo.JSpecsOrder = null;

                return true;

            }
            }

        /// <summary>
        /// Get frame image url.
        /// </summary>
        /// <param name="frameFamily"></param>
        /// <param name="frameColor"></param>
        /// <returns></returns>
        public string GetFrameImage(string frameFamily, string frameColor)
        {
            FrameRepository _frameRepository = new FrameRepository();
            string frameImgUrl = _frameRepository.GetFrameImageByFrameFamilyAndFrameColor(frameFamily, frameColor);
            string frameImgFull;
            try
            {
                frameImgFull = "/JSpecs/" + frameImgUrl;
            }
            catch(NullReferenceException nre)
            {
                frameImgFull = "/JSpecs/imgs/Fallback/glasses.svg";
            }
            


            return frameImgFull;
        }

        /// <summary>
        /// Get order number for next order.
        /// </summary>
        /// <returns>a string of the new order number</returns>
        private string GetNextOrderNumber()
        {
            OrderRepository oRep = new OrderRepository();
            List<string> nums = oRep.GetNextOrderNumbers(this.JSpecsSiteCode, 1);

            if (nums == null || nums.Count.Equals(0))
            {
                nums.Add(string.Empty);
            }

            return nums[0];
        }

          
        /// <summary>
        /// Get lab site code for the frame.
        /// </summary>
        /// <param name="frameCode">frame code to lookup labsite code</param>
        /// <returns>string of the frame labsite code</returns>
        private string GetLabSiteCode(string frameCode)
        {
            string labSiteCode = string.Empty;

            if (OrderEntity.CustFrameToLabColl.ContainsKey(frameCode))
            {
                var sc = new List<String>();
                OrderEntity.CustFrameToLabColl.TryGetValues(frameCode, out sc);

                if (!frameCode.ToLower().Equals("5am") && !frameCode.ToLower().Equals("5am50")
                     && !frameCode.ToLower().Equals("5am52") && !frameCode.ToLower().Equals("5am54"))
                {
                    labSiteCode = sc[0];
                }
                else
                {
                    labSiteCode = "MNOST1";
                }
            }
            else
            {
                labSiteCode = "MNOST1";
            }

            return labSiteCode;
        }

        /// <summary>
        /// Generate a bar code, and add it to the order.
        /// </summary>
        /// <param name="order">a OrderEntity to update its barcode</param>
        private void AddBarCodeToOrder(OrderEntity order)
        {
            IGenBarCodes gbc = new GenerateBarCodes(new Barcode());

            // create barcode.
            Image bp = gbc.GenerateBarCode(order.OrderNumber);

            if (bp != null)
            {
                MemoryStream ms = new System.IO.MemoryStream();
                bp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                order.ONBarCode = ms.ToArray();

                OrderRepository _orderRepository = new OrderRepository();

                _orderRepository.UpdateOrderWithBarcode(order);
            }
        }
    }
}
