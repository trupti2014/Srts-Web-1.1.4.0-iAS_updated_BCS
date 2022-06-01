using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Views.GEyes;
using System.Drawing;
using System.IO;

namespace SrtsWeb.Presenters.GEyes
{
    public sealed class OrderConfirmationPresenter
    {
        private IOrderConfirmationView _view;

        public OrderConfirmationPresenter(IOrderConfirmationView view)
        {
            _view = view;
        }

        public void InitView()
        {
        }

        public void SaveData(IGenBarCodes gbc)
        {
            var _orderRepository = new OrderRepository();

            if (string.IsNullOrEmpty(_view.myInfo.OrderToSave.OrderNumber))
            {
                _view.Message = "Error creating order number";
                return;
            }

            var ots = this._view.myInfo.OrderToSave;

            try
            {
                if (!_orderRepository.InsertOrder(ots, true))
                {
                    _view.Message = "Error saving order";
                    return;
                }

                // create barcode.
                Image bp = gbc.GenerateBarCode(ots.OrderNumber);

                if (bp != null)
                {
                    MemoryStream ms = new System.IO.MemoryStream();
                    bp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ots.ONBarCode = ms.ToArray();

                    _orderRepository = new OrderRepository();

                    _orderRepository.UpdateOrderWithBarcode(ots);
                }
            }
            catch (System.Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                _view.Message = "Error saving order record.";
            }
            finally
            {
                _view.OrderNumber = ots.OrderNumber;
                _view.EmailAddress = ots.CorrespondenceEmail;
            }
        }
    }
}