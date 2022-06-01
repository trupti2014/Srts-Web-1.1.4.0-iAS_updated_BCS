using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Patients;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;

namespace SrtsWeb.BusinessLayer.Presenters.Patients
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

        public Boolean SaveIDNumbers(IdentificationNumbersEntity ine)
        {
            var _idRepository = new IdentificationNumbersRepository();

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
            if (string.IsNullOrEmpty(_view.Address1) && string.IsNullOrEmpty(_view.Address2)) return true;

            AddressEntity ae = new AddressEntity();

            ae.Address1 = _view.Address1;
            ae.Address2 = _view.Address2;
            ae.AddressType = _view.AddressTypeSelected;
            ae.City = _view.City;
            ae.State = _view.StateSelected;
            ae.ZipCode = _view.ZipCode.ToZipCodeDisplay();
            ae.Country = _view.CountrySelected;
            ae.UIC = _view.UIC;
            ae.IndividualID = _view.mySession.Patient.Individual.ID;
            ae.IsActive = true;
            ae.ModifiedBy = _view.mySession.MyUserID;

            var _addrRepository = new AddressRepository();

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
            var eae = new EMailAddressEntity();
            eae.EMailAddress = String.IsNullOrEmpty(_view.EMailAddress.Trim()) ? "MAIL.MAIL@MAIL.MAIL" : _view.EMailAddress;
            eae.EMailType = String.IsNullOrEmpty(_view.TypeEMailSelected) || _view.TypeEMailSelected.Equals("0") ? "OTHER" : _view.TypeEMailSelected;
            eae.IndividualID = _view.mySession.Patient.Individual.ID;
            eae.IsActive = true;
            eae.ModifiedBy = _view.mySession.MyUserID;

            var _emailRepository = new EMailAddressRepository();
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
            var pne = new PhoneNumberEntity();
            pne.Extension = String.IsNullOrEmpty(_view.Extension.Trim()) ? string.Empty : _view.Extension;
            pne.IndividualID = _view.mySession.Patient.Individual.ID;
            pne.IsActive = true;
            pne.ModifiedBy = _view.mySession.MyUserID;
            pne.PhoneNumber = String.IsNullOrEmpty(_view.PhoneNumber.Trim()) ? "0000000" : _view.PhoneNumber;
            pne.PhoneNumberType = String.IsNullOrEmpty(_view.TypePhoneSelected) || _view.TypePhoneSelected.Equals("0") ? "HOME" : _view.TypePhoneSelected;

            var _phoneRepository = new PhoneRepository();
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
        //        IndividualId = _view.mySession.SelectedPatientID,
        //        TypeId = 110,
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

        public void SetDmdcPatientFields(DmdcPerson person)
        {
            _view.Address1 = person.MailingAddress1;
            _view.City = person.MailingCity;
            if (!String.IsNullOrEmpty(person.MailingState))
                _view.StateSelected = person.MailingState;
            _view.ZipCode = String.Format("{0}-{1}", person.MailingZip, person.MailingZipExtension);
            if (!String.IsNullOrEmpty(person.MailingCountry))
                _view.CountrySelected = person.MailingCountry;
            _view.AddressTypeSelected = "MAIL";

            _view.EMailAddress = person.Email;
        }
    }
}