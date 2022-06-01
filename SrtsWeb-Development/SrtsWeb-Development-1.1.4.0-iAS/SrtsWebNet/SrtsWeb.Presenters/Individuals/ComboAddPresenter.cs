
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Individuals;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;

namespace SrtsWeb.Presenters.Individuals
{
    public sealed class ComboAddPresenter
    {
        private IComboAddView _view;

        public ComboAddPresenter(IComboAddView view)
        {
            _view = view;
        }

        public void InitView()
        {
            FillGrids();
        }

        private void FillGrids()
        {
            _view.IDTypeDDL = _view.LookupCache.GetByType(LookupType.IDNumberType.ToString());
            _view.StateDDL = _view.LookupCache.GetByType(LookupType.StateList.ToString());
            _view.CountryDDL = _view.LookupCache.GetByType(LookupType.CountryList.ToString());
            _view.AddressTypeDDL = _view.LookupCache.GetByType(LookupType.AddressType.ToString());
            _view.TypeEmailDDL = _view.LookupCache.GetByType(LookupType.EmailType.ToString());
            _view.PhoneTypeDDL = _view.LookupCache.GetByType(LookupType.PhoneType.ToString());
            _view.IndividualTypeDDL = _view.LookupCache.GetByType(LookupType.IndividualType.ToString());
        }

        public Boolean SaveIDNumbers()
        {
            var _idRepository = new IdentificationNumbersRepository();
            IdentificationNumbersEntity ine = new IdentificationNumbersEntity();
            ine.IndividualID = _view.mySession.Patient.Individual.ID;
            ine.IsActive = true;
            ine.ModifiedBy = _view.mySession.MyUserID;
            ine.IDNumber = _view.IDNumber.ToSSNRemoveDash();
            ine.IDNumberType = _view.IDNumberType;

            _view.IDNumberMessage = ine.IDNumber.ValidateIDNumLength_forModals(ine.IDNumberType);

            if (!string.IsNullOrEmpty(_view.IDNumberMessage))
            {
                return false;
            }

            var tmpID = _idRepository.GetIdentificationNumberByIDNumber(ine.IDNumber, ine.IDNumberType, _view.mySession.MyUserID.ToString());
            if (!tmpID.IsNullOrEmpty())
                {
                    _view.IDNumberMessage = "This is a duplicate ID";
                    return false;
                }

            var dt = _idRepository.InsertIdentificationNumbers(ine);

            if (!dt.IsNull())
            {
                _view.IDNumberMessage = "Your Identification Data was Saved";
                return true;
            }
            else
            {
                _view.IDNumberMessage = "Your Identification Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        public Boolean SaveAddress()
        {
            var _addrRepository = new AddressRepository();
            AddressEntity ae = new AddressEntity();
            ae.Address1 = _view.Address1;
            ae.Address2 = _view.Address2;
            ae.AddressType = _view.AddressTypeSelected;
            ae.City = _view.City;
            ae.Country = _view.CountrySelected;
            ae.IndividualID = _view.mySession.Patient.Individual.ID;
            ae.IsActive = true;
            ae.ModifiedBy = _view.mySession.MyUserID;
            ae.State = _view.StateSelected;
            ae.ZipCode = _view.ZipCode.ToZipCodeDisplay();
            var dt = _addrRepository.InsertAddress(ae);
            if (dt.Count > 0)
            {
                _view.AddrMessage = "Your Address Data was Saved";
                return true;
            }
            else
            {
                _view.AddrMessage = "Your Address Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        public Boolean SaveEmail()
        {
            var _emailRepository = new EMailAddressRepository();
            EMailAddressEntity eae = new EMailAddressEntity();
            eae.EMailAddress = _view.EMailAddress;
            eae.EMailType = _view.TypeEMailSelected;
            eae.IndividualID = _view.mySession.Patient.Individual.ID;
            eae.IsActive = true;
            eae.ModifiedBy = _view.mySession.MyUserID;
            var dt = _emailRepository.InsertEMailAddress(eae);
            if (dt.Count > 0)
            {
                _view.EmailMessage = "Your Email Address Data was Saved";
                return true;
            }
            else
            {
                _view.EmailMessage = "Your Email Address Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        public Boolean SavePhone()
        {
            var _phoneRepository = new PhoneRepository();
            PhoneNumberEntity pne = new PhoneNumberEntity();
            pne.AreaCode = _view.AreaCode;
            pne.Extension = _view.Extension;
            pne.IndividualID = _view.mySession.Patient.Individual.ID;
            pne.IsActive = true;
            pne.ModifiedBy = _view.mySession.MyUserID;
            pne.PhoneNumber = _view.PhoneNumber;
            pne.PhoneNumberType = _view.TypePhoneSelected;
            var dt = _phoneRepository.InsertPhoneNumber(pne);
            if (dt.Count > 0)
            {
                _view.PhoneMessage = "Your Phone Data was Saved";
                return true;
            }
            else
            {
                _view.PhoneMessage = "Your Phone Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        //public Boolean SaveIndividualType()
        //{
        //    var te = new IndividualTypeEntity()
        //    {
        //        IndividualId = _view.mySession.Patient.Individual.ID,
        //        TypeId = Convert.ToInt32(_view.IndividualTypeSelected),
        //        ModifiedBy = _view.mySession.MyUserID,
        //        IsActive = true
        //    };

        //    _typeRepository = new IndividualTypeRepository();
        //    if (_typeRepository.InsertIndividualType(te).Rows.Count.Equals(0))
        //    {
        //        _view.IndividualTypeMessage = "Your Individual Type data was NOT saved, please try again later or contact the SRTS team to report the problem";
        //        return false;
        //    }
        //    else
        //    {
        //        _view.IndividualTypeMessage = "Your Individual Type data was saved";
        //        return true;
        //    }
        //}
    }
}