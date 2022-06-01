using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.GEyes;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.GEyes
{
    public sealed class AddressUpdatePresenter
    {
        private IAddressUpdateView _view;
        private List<LookupTableEntity> _lookupData;
        //private IAddressRepository _repository;

        public AddressUpdatePresenter(IAddressUpdateView view)
        {
            _view = view;
            //_repository = new AddressRepository();
        }

        public void InitView()
        {
            GetLookupData();

            _view.Countries = _lookupData.Where(x => x.Code.ToLower() == LookupType.CountryList.ToString().ToLower()).ToList(); // SrtsHelper.GetLookupTypesSelected(_lookupData, LookupType.CountryList.ToString());
        }

        private void GetLookupData()
        {
            var lr = new LookupRepository();
            _lookupData = lr.GetAllLooksups();
        }

        public void GetStates()
        {
            var lookupRepository = new LookupRepository();
            _view.StateList = lookupRepository.GetLookupsByType("StateList").Select(x => new StateEntity() { StateText = x.Value + " - " + x.Text, StateValue = x.Value }).ToList();
        }

        public void FillAddress(int _id)
        {
            var ae = from p in _view.myInfo.Patient.Addresses where p.ID == _id select p;
            foreach (AddressEntity addr in ae)
            {
                _view.Address1 = addr.Address1;
                _view.Address2 = string.IsNullOrEmpty(addr.Address2) ? string.Empty : addr.Address2;

                _view.City = addr.City;
                _view.State = addr.State;
                _view.SelectedCountry = addr.Country;
                _view.ZipCode = addr.ZipCode.ToZipCodeDisplay();
            }
        }

        public void SelectedAddress()
        {
            _view.myInfo.OrderToSave.ShipAddress1 = _view.Address1;
            _view.myInfo.OrderToSave.ShipAddress2 = _view.Address2;
            _view.myInfo.OrderToSave.ShipAddress3 = string.Empty;

            _view.myInfo.OrderToSave.ShipCity = _view.City;
            _view.myInfo.OrderToSave.ShipState = _view.State;
            _view.myInfo.OrderToSave.ShipZipCode = _view.ZipCode.ToZipCodeDisplay();
            _view.myInfo.OrderToSave.ShipCountry = _view.SelectedCountry;
            _view.myInfo.OrderToSave.CorrespondenceEmail = _view.EmailAddress;
        }

        public void SetOrderAddress()
        {
            _view.myInfo.OrderToSave.ShipAddress1 = _view.Address1;
            _view.myInfo.OrderToSave.ShipAddress2 = _view.Address2;
            _view.myInfo.OrderToSave.ShipAddress3 = string.Empty;
            _view.myInfo.OrderToSave.ShipAddressType = _view.City;
            _view.myInfo.OrderToSave.ShipCity = _view.City;
            _view.myInfo.OrderToSave.ShipState = _view.State;
            _view.myInfo.OrderToSave.ShipZipCode = _view.ZipCode.ToZipCodeDisplay();
            _view.myInfo.OrderToSave.ShipCountry = _view.SelectedCountry;
            _view.myInfo.OrderToSave.CorrespondenceEmail = _view.EmailAddress;
        }

        public void SaveAddress()
        {
            AddressEntity ae = new AddressEntity();
            ae.Address1 = _view.Address1;
            ae.Address2 = _view.Address2;
            ae.Address3 = string.Empty;

            //ae.AddressType = _view.City;
            ae.City = _view.City;
            ae.Country = _view.SelectedCountry;
            ae.State = _view.State;
            ae.ZipCode = _view.ZipCode;
            ae.ModifiedBy = "GEyes-Self";
            ae.IsActive = true;
            ae.IndividualID = _view.myInfo.Patient.Individual.ID;
            var _repository = new AddressRepository();
            var dt = _repository.InsertAddress(ae);
            bool cont = false;
            if (dt.Count > 0)
            {
                foreach (var a in dt)
                {
                    if (a.Address1 == ae.Address1)
                    {
                        cont = true;
                    }
                }
                if (cont)
                {
                    _view.myInfo.OrderToSave.ShipAddress1 = _view.Address1;
                    _view.myInfo.OrderToSave.ShipAddress2 = _view.Address2;
                    _view.myInfo.OrderToSave.ShipAddress3 = string.Empty;

                    _view.myInfo.OrderToSave.ShipAddressType = _view.City;
                    _view.myInfo.OrderToSave.ShipCity = _view.City;
                    _view.myInfo.OrderToSave.ShipState = _view.State;
                    _view.myInfo.OrderToSave.ShipZipCode = _view.ZipCode.ToZipCodeDisplay();
                    _view.myInfo.OrderToSave.ShipCountry = _view.SelectedCountry;
                    _view.myInfo.OrderToSave.CorrespondenceEmail = _view.EmailAddress;
                }
            }
            else
            {
                _view.Message = "This address could not be saved, Please try again";
            }
        }
    }
}