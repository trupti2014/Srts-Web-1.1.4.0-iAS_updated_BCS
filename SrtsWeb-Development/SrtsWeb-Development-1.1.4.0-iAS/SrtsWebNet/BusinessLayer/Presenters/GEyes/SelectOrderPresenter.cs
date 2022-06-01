using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.GEyes;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Web;

namespace SrtsWeb.BusinessLayer.Presenters.GEyes
{
    public sealed class SelectOrderPresenter : PresenterBase
    {
        private ISelectOrderView _view;
        //private IOrderRepository _orderRepository;

        public SelectOrderPresenter(ISelectOrderView view)
        {
            _view = view;
        }

        public void InitView()
        {
            FillGrids();
        }

        public void FillGrids()
        {
            var _orderRepository = new OrderRepository.DisplayRepository();
            _view.OrdersData = _orderRepository.GetOrderDisplay(_view.myInfo.Patient.Individual.ID, HttpContext.Current.User.Identity.Name, true);
        }

        public void FillSelection(string orderNumber)
        {
            var _gRepository = new OrderRepository.GEyesOrderRepository();

            _view.myInfo.OrderToSave = _gRepository.GetOrderByOrderNumber(orderNumber, HttpContext.Current.User.Identity.Name.ToString());

            var oRep = new OrderRepository();
            var nums = oRep.GetNextOrderNumbers(this.GeyesSiteCode, 1);
            if (nums == null || nums.Count.Equals(0))
            {
                nums.Add(string.Empty);
            }

            _view.myInfo.OrderToSave.OrderNumber = nums[0];
            _view.FrameDescription = _view.myInfo.OrderToSave.FrameDescription;
            _view.LensTypeLong = _view.myInfo.OrderToSave.LensTypeLong;
            _view.LensTint = _view.myInfo.OrderToSave.LensTint;
            _view.myInfo.OrderToSave.ClinicSiteCode = this.GeyesSiteCode;

            var d = _view.myInfo.OrderToSave.Demographic.Substring(0, 7) + "R";  // All G-Eyes orders are to be "READINESS" priority
            _view.myInfo.OrderToSave.Demographic = d;

            var fc = _view.myInfo.OrderToSave.FrameCode;
            if (OrderEntity.CustFrameToLabColl.ContainsKey(fc))
            {
                var sc = new List<String>();
                OrderEntity.CustFrameToLabColl.TryGetValues(fc, out sc);

                if (!fc.ToLower().Equals("5am") && !fc.ToLower().Equals("5am50")
                     && !fc.ToLower().Equals("5am52") && !fc.ToLower().Equals("5am54"))
                    _view.myInfo.OrderToSave.LabSiteCode = sc[0];
                else
                {
                    _view.myInfo.OrderToSave.LabSiteCode = "MNOST1";
                }
            }
            else
                _view.myInfo.OrderToSave.LabSiteCode = "MNOST1";

            _view.myInfo.OrderToSave.IsGEyes = true;
            _view.myInfo.OrderToSave.ModifiedBy = "Geyes-Self";
            _view.myInfo.OrderToSave.ShipToPatient = true;
        }

        public void SetData()
        {
            _view.myInfo.OrderToSave.UserComment1 = string.IsNullOrEmpty(_view.Comment) ? string.Empty : _view.Comment;
        }
    }
}