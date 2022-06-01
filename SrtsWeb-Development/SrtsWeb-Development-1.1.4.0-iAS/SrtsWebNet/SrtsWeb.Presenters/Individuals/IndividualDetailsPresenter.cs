
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Individuals;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace SrtsWeb.Presenters.Individuals
{
    public sealed class IndividualDetailsPresenter
    {
        private IIndividualDetailsView _view;

        public IndividualDetailsPresenter(IIndividualDetailsView view)
        {
            _view = view;
            //_idRepository = new IdentificationNumbersRepository();
            //_addrRepository = new AddressRepository();
            //_phoneRepository = new PhoneRepository();
            //_emailRepository = new EMailAddressRepository();
            //_indTypeRepository = new IndividualTypeRepository();
        }

        public void InitView()
        {
            if (!string.IsNullOrEmpty(_view.mySession.SelectedPatientID.ToString()) || _view.mySession.SelectedPatientID.ToString() != "0")
            {
                _view.mySession.Patient = GetAllPatientInfo(_view.mySession.SelectedPatientID, _view.mySession.MyUserID.ToString());
                FillGrids();
            }
        }

        private void FillGrids()
        {
            _view.EmailAddressesBind = _view.mySession.Patient.EMailAddresses;
            _view.PhoneNumbersBind = _view.mySession.Patient.PhoneNumbers;
            _view.AddressesBind = _view.mySession.Patient.Addresses;
            _view.IDNumbersBind = _view.mySession.Patient.IDNumbers;
            _view.IndividualTypesBind = _view.mySession.Patient.IndividualTypes;
            var l = new LookupService();
            _view.IndividualTypeLookup = l.GetLookupsByType(LookupType.IndividualType.ToString()).Select(x => new KeyValueEntity { Key = x.Text, Value = x.Id }).ToList();
        }

        public String GetIndivualTypes()
        {
            List<string> indTypes = new List<string>();

            if (_view.IsAdmin) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "OTHER").Value.ToString());

            if (_view.IsTechnician) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "TECHNICIAN").Value.ToString());

            if (_view.IsProvider) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "PROVIDER").Value.ToString());

            return String.Join(",", indTypes);
        }

        private PatientEntity GetAllPatientInfo(int individualID, string modifiedBy)
        {
            var _indRepository = new IndividualRepository.PatientRepository();
            var pe = _indRepository.GetAllPatientInfoByIndividualID(individualID, true, modifiedBy, _view.mySession.MySite.SiteCode);

            return pe;
        }

        public Boolean UpdateIDNumbers(IdentificationNumbersEntity ine, Boolean updateStatusOnly)
        {
            _view.Message = ine.IDNumber.ValidateIDNumLength_forModals(ine.IDNumberType);

            if (!string.IsNullOrEmpty(_view.Message))
            {
                return false;
            }

            var _idRepository = new IdentificationNumbersRepository();

            if (!updateStatusOnly)
            {
                var tmpID = _idRepository.GetIdentificationNumberByIDNumber(ine.IDNumber, ine.IDNumberType, _view.mySession.MyUserID.ToString());
                if (!tmpID.IsNullOrEmpty())
                {
                        _view.Message = "This is a duplicate ID";
                        return false;
                    }
                }

            var ineUpdate = _idRepository.UpdateIdentificationNumbersByID(ine);

            if (ineUpdate.Count > 0)
            {
                _view.IDNumbersBind = ineUpdate;
                return true;
            }
            else { return false; }
        }

        public void BindIdentificationNumbers()
        {
            var _idRepository = new IdentificationNumbersRepository();
            _view.IDNumbersBind = _idRepository.GetIdentificationNumbersByIndividualID(_view.mySession.Patient.Individual.ID, _view.mySession.MyUserID);
        }

        public void BindAddresses()
        {
            var _addrRepository = new AddressRepository();
            _view.AddressesBind = _addrRepository.GetAddressesByIndividualID(_view.mySession.Patient.Individual.ID, _view.mySession.MyUserID.ToString());
        }

        public void BindStates()
        {
            _view.States = _view.LookupCache.GetByType(LookupType.StateList.ToString());
        }

        public void BindCountries()
        {
            _view.Countries = _view.LookupCache.GetByType(LookupType.CountryList.ToString());
        }

        public Boolean UpdateAddresses(AddressEntity addr)
        {
            var _addrRepository = new AddressRepository();
            var addressUpdate = _addrRepository.UpdateAddress(addr);
            if (!addressUpdate.IsNullOrEmpty())
            {
                _view.AddressesBind = addressUpdate;
                return true;
            }
            else { return false; }
        }

        public void BindPhoneNumbers()
        {
            var _phoneRepository = new PhoneRepository();
            _view.PhoneNumbersBind = _phoneRepository.GetPhoneNumbersByIndividualID(_view.mySession.Patient.Individual.ID, _view.mySession.MyUserID.ToString());
        }

        public Boolean UpdatePhones(PhoneNumberEntity pne)
        {
            var _phoneRepository = new PhoneRepository();
            var phoneUpdate = _phoneRepository.UpdatePhoneNumber(pne);
            if (phoneUpdate.Count > 0)
            {
                _view.PhoneNumbersBind = phoneUpdate;
                return true;
            }
            else { return false; }
        }

        public void BindEmail()
        {
            var _emailRepository = new EMailAddressRepository(); 
            _view.EmailAddressesBind = _emailRepository.GetEmailAddressesByIndividualID(_view.mySession.Patient.Individual.ID, _view.mySession.MyUserID);
        }

        public Boolean UpdateEmail(EMailAddressEntity eme)
        {
            var _emailRepository = new EMailAddressRepository(); 
            var email = _emailRepository.UpdateEMailAddress(eme);
            if (email.Count > 0)
            {
                _view.EmailAddressesBind = email;
                return true;
            }
            else { return false; }
        }

        public void BindIndividualType()
        {
            var _indTypeRepository = new IndividualTypeRepository();
            _view.IndividualTypesBind = _indTypeRepository.GetIndividualTypesByIndividualId(_view.mySession.Patient.Individual.ID);
        }

        //public Boolean UpdateIndividualType(IndividualTypeEntity ite)
        //{
        //    var dt = _indTypeRepository.UpdateIndividualType(ite);
        //    if (dt.Rows.Count.Equals(0)) return false;
        //    _view.IndividualTypesBind = SrtsHelper.ProcessIndividualTypeTable(dt);
        //    return true;
        //}

        public Boolean UpdateIndividualTypes(int IndividualId, string ModifiedBy)
        {
            var indTypes = GetIndivualTypes();
            var r = new IndividualTypeRepository();
            return r.InsertIndividualTypes(IndividualId, ModifiedBy, indTypes, true);
        }
    }
}