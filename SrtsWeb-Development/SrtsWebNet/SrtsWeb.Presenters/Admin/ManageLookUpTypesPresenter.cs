using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.Views.Admin;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SrtsWeb.Presenters.Admin
{
    public sealed class ManageLookUpTypesPresenter
    {
        private IManageLookUpTypesView _view;

        private OrderEntity _findOE;

        public ManageLookUpTypesPresenter(IManageLookUpTypesView view)
        {
            _view = view;
        }

        public void InitView()
        {
            _findOE = new OrderEntity();
            if (_view.CacheData != null)
            {
                _view.LookupTypes = _view.CacheData.Select(x => new { Key = x.Code, Value = x.Code }).Distinct().ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public void LoadSelectedTypes()
        {
            _view.LookupsBind = _view.CacheData.Where(x => x.Code.ToLower() == _view.SelectedType.ToLower()).ToList();
        }

        public void UpdateLookup()
        {
            LookupTableEntity lte = new LookupTableEntity();
            lte.Code = _view.CodeInput;
            lte.DateLastModified = DateTime.Now;
            lte.Description = _view.DescriptionInput;
            lte.Id = _view.IDInput;
            lte.IsActive = _view.IsActiveInput;
            lte.ModifiedBy = _view.mySession.ModifiedBy;
            lte.Text = _view.TextInput;
            lte.Value = _view.ValueInput;
            var _repository = new LookupRepository();
            var c = _repository.UpdateLookUpTable(lte);
            _view.CacheData = c;
        }

        public void InsertLookup()
        {
            LookupTableEntity lte = new LookupTableEntity();
            lte.Code = _view.CodeInput;
            lte.DateLastModified = DateTime.Now;
            lte.Description = _view.DescriptionInput;
            lte.IsActive = _view.IsActiveInput;
            lte.ModifiedBy = _view.mySession.ModifiedBy;
            lte.Text = _view.TextInput;
            lte.Value = _view.ValueInput;
            var _repository = new LookupRepository();
            _repository.InsertLookUpTableItem(lte);
            _view.CacheData.Add(lte);
        }

        public int GenerateLegacyOrderBarcodes(IGenBarCodes gbc)
        {
            _findOE = new OrderEntity();
            var _orderRepository = new OrderRepository();

            int CountAffectedRows = 0;

            var lBc = _orderRepository.GetPatientOrdersNoBarCodes();
            CountAffectedRows = lBc.Count;

            foreach (var bc in lBc)
            {
                var barCode = String.Format("*{0}*", bc);

                Image OrderBarCode = gbc.GenerateBarCode(barCode);

                if (OrderBarCode != null)
                {
                    MemoryStream ms = new System.IO.MemoryStream();
                    OrderBarCode.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    _findOE.OrderNumber = bc;
                    _findOE.ONBarCode = ms.ToArray();
                    _orderRepository.UpdateOrderWithBarcode(_findOE);
                }
            }
            return CountAffectedRows;
        }
    }
}