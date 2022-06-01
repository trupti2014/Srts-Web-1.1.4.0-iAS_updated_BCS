
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Patients;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Data;
using System.Web;
using System.Web.Security;

namespace SrtsWeb.Presenters.Patients
{
    public sealed class PatientDetailsPresenter
    {
        private IPatientDetailsView _view;

        public PatientDetailsPresenter(IPatientDetailsView view)
        {
            _view = view;
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
            // Get order history for user
            //DataSet ds = _repository.GetOrderDisplay(_view.mySession.Patient.Individual.ID, HttpContext.Current.User.Identity.Name, false);

            //int OrderStatusID = 0;
            //string UserSiteCode = _view.mySession.MySite.SiteCode.ToString().Trim();
            //string ClinicSiteCode = String.Empty;
            //string[] UserRoles = Roles.GetRolesForUser();

            //// Step through the orders from the db
            //foreach (DataRow dr in ds.Tables[1].Select(""))
            //{
            //    OrderStatusID = int.Parse(dr["OrderStatusTypeID"].ToString());
            //    if (OrderStatusID == 14 || OrderStatusID == 15)
            //    {
            //        ClinicSiteCode = dr["ClinicSiteCode"].ToString().Trim();

            //        // If the current user is a lab-role user, hide orders in "Clinic Cancelled" and "Incomplete" status
            //        // Also hide cancelled and incomplete orders for clinic users if the order is in "Clinic Cancelled" and "Incomplete" status, AND the current user is NOT at the clinic that created the order
            //        if ((UserRoles[0].Substring(0, 3).ToLower() == "lab") ||
            //            ((UserRoles[0].Substring(0, 6).ToLower() == "clinic") && (!(String.Equals(ClinicSiteCode, UserSiteCode, StringComparison.InvariantCultureIgnoreCase)))))
            //        {
            //            string RemoveOrderNum = dr["OrderNumber"].ToString();
            //            DataRow[] getdr = ds.Tables[0].Select(string.Format("{0} LIKE '%{1}%'", "OrderNumber", RemoveOrderNum));
            //            DataRow rmdr = getdr[0];

            //            // Remove the order from BOTH datatables returned from the db
            //            ds.Tables[0].Rows.Remove(rmdr);
            //            ds.Tables[1].Rows.Remove(dr);
            //        }
            //    }
            //}

            //_view.OrdersBind = ds;
            //_view.ExamsBind = _view.mySession.Patient.Exams;
            _view.EmailAddressesBind = _view.mySession.Patient.EMailAddresses;
            _view.PhoneNumbersBind = _view.mySession.Patient.PhoneNumbers;
            _view.AddressesBind = _view.mySession.Patient.Addresses;
            _view.IDNumbersBind = _view.mySession.Patient.IDNumbers;
            //_view.PrescriptionsBind = _view.mySession.Patient.Prescriptions;
        }

        private PatientEntity GetAllPatientInfo(int individualID, string modifiedBy)
        {
            var _repository = new IndividualRepository.PatientRepository();
            var pe = _repository.GetAllPatientInfoByIndividualID(individualID, true, modifiedBy, _view.mySession.MySite.SiteCode);
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

            if (ineUpdate.Count.Equals(0)) return false;

            _view.IDNumbersBind = ineUpdate;
            return true;
        }

        public void BindIdentificationNumbers()
        {
            var _idRepository = new IdentificationNumbersRepository();
            _view.IDNumbersBind = _idRepository.GetIdentificationNumbersByIndividualID(_view.mySession.Patient.Individual.ID, _view.mySession.MyUserID.ToString());
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
            _view.EmailAddressesBind = _emailRepository.GetEmailAddressesByIndividualID(_view.mySession.Patient.Individual.ID, _view.mySession.MyUserID.ToString());
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
    }
}