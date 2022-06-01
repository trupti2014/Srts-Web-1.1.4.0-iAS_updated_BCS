using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Views.GEyes;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System.Data;
using System.Drawing;
using System.IO;
using System;

namespace SrtsWeb.BusinessLayer.Presenters.GEyes
{
    public sealed class OrderConfirmationPresenter
    {
        private IOrderConfirmationView _view;
        //private IOrderRepository _orderRepository;

        public OrderConfirmationPresenter(IOrderConfirmationView view)
        {
            _view = view;
        }

        public void InitView()
        {
        }

        public void SaveData()
        {
            var _orderRepository = new OrderRepository();

            var od = new OrderStateEntity();

            try
            {
                if (string.IsNullOrEmpty(_view.myInfo.OrderToSave.OrderNumber))
                {
                    _view.Message = "Error creating order number";
                    return;
                }
                var oe = _orderRepository.InsertOrder(_view.myInfo.OrderToSave, true);

                if (oe != null)
                {
                    od.LabCode = _view.myInfo.OrderToSave.LabSiteCode;
                    od.IsActive = true;
                    od.ModifiedBy = "Geyes-Self";
                    od.OrderNumber = oe.OrderNumber;
                    od.OrderStatusTypeID = 1;
                    od.StatusComment = string.Empty;

                    IGenBarCodes gbc = new GenerateBarCodes();
                    var on = String.Format("*{0}*", oe.OrderNumber);

                    Image bp = gbc.GenerateBarCode(on);

                    if (bp != null)
                    {
                        MemoryStream ms = new System.IO.MemoryStream();
                        bp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        //oe.OrderNumber = dt.Rows[0]["OrderNumber"].ToString();
                        oe.ONBarCode = ms.ToArray();
                        _orderRepository = new OrderRepository();
                        //DataTable Orderdt = _orderRepository.UpdateOrderWithBarcode(oe);
                        //_view.myInfo.Patient.Orders = SrtsHelper.ProcessOrderTable(Orderdt);
                        _orderRepository.UpdateOrderWithBarcode(oe);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                _view.Message = "Error saving order record.";
            }

            if (od == null)
            {
                _view.Message += "Error saving order record.";
            }
            else
            {
                _view.OrderNumber = od.OrderNumber;
                _view.EmailAddress = _view.myInfo.OrderToSave.CorrespondenceEmail;
            }
        }
    }
}