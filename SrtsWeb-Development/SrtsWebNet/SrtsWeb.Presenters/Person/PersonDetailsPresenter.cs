using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrtsWeb.Presenters.Person
{
    public class PersonDetailsPresenter
    {
        protected IPersonDetailsView _view;

        public PersonDetailsPresenter(IPersonDetailsView view)
        {
            this._view = view;
        }

        public void InitView(Boolean isPatient)
        {
            if (!string.IsNullOrEmpty(_view.mySession.SelectedPatientID.ToString()) || _view.mySession.SelectedPatientID.ToString() != "0")
            {
                _view.mySession.Patient = GetAllPatientInfo(_view.mySession.SelectedPatientID, _view.mySession.ModifiedBy);
                FillPersonData(isPatient);
            }
        }

        private PatientEntity GetAllPatientInfo(int individualID, string modifiedBy)
        {
            var _repository = new IndividualRepository.PatientRepository();
            var pe = _repository.GetAllPatientInfoByIndividualID(individualID, true, modifiedBy, _view.mySession.MySite.SiteCode);
            return pe;
        }

        private void FillPersonData(Boolean isPatient)
        {
            PopulateDropDowns();

            //Personal Info
            //Service Info
            FillPatientItems();

            // ID Numbers
            var ids = _view.mySession.Patient.IDNumbers;

            this._view.DSS = ids.FirstOrDefault(x => x.IDNumberType == "SSN" && x.IsActive == true);
            this._view.DIN = ids.FirstOrDefault(x => x.IDNumberType == "DIN" && x.IsActive == true);
            this._view.DBN = ids.FirstOrDefault(x => x.IDNumberType == "DBN" && x.IsActive == true);
            this._view.PIN = ids.FirstOrDefault(x => x.IDNumberType == "PIN" && x.IsActive == true);

            // Address DDLs
            this._view.States = _view.LookupCache.GetByType(LookupType.StateList.ToString());
            this._view.Countries = _view.LookupCache.GetByType(LookupType.CountryList.ToString());

            // Addresses
            var addy = _view.mySession.Patient.Addresses;
            this._view.PrimaryAddress = addy.FirstOrDefault(x => x.IsActive == true);

            // Phone Numbers
            var ph = _view.mySession.Patient.PhoneNumbers;
            this._view.PrimaryPhone = ph.FirstOrDefault(x => x.IsActive == true);

            // Email Addresses
            var email = _view.mySession.Patient.EMailAddresses;
            this._view.PrimaryEmail = email.FirstOrDefault(x => x.IsActive == true);

            // Individual Types
            if (isPatient) return;
            _view.IndividualTypesBind = _view.mySession.Patient.IndividualTypes;
            var l = new LookupService();
            _view.IndividualTypeLookup = l.GetLookupsByType(LookupType.IndividualType.ToString()).Select(x => new KeyValueEntity { Key = x.Text, Value = x.Id }).ToList();

            // Individual Assigned Sites
            var r = new SiteRepository.SiteCodeRepository();
            _view.SiteCodes = r.GetAllSites().Where(x => x.IsActive == true).ToList();
            _view.AssignedSiteList = r.GetIndivSiteCodes(_view.mySession.SelectedPatientID);
        }

        #region DEERS Refresh

        public DmdcPerson_Flat SearchPersonDmdc(String IdNum, String IdType)
        {
            try
            {
                var p = new SrtsWeb.Presenters.Dmdc.DmdcPresenter(new DmdcService());
                var d = p.Get(IdNum);
                //var ds = new DmdcService();
                //var d = ds.GetMockData(IdNum, IdType);

                // If there are no matches then return false
                if (d.IsNull()) return null;

                // Populate the flattened dmdc entity
                //var fdl = DmdcService.FlattenDmdcPerson(new List<SrtsWeb.BusinessLayer.DmdcMock.DmdcPersonWs>() { d });
                var fdl = DmdcService.FlattenDmdcPerson(new List<DmdcPerson>() { d });
                if (fdl.IsNullOrEmpty())
                    return null;

                return fdl.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        #endregion DEERS Refresh

        #region PersonalInfo OPS

        public Boolean DoSavePersonalInfo(IndividualEntity entity)
        {
            // Compare the entity with the personal info in the session
            if (!IsPersonalInfoDirty(_view.mySession.Patient.Individual, entity)) { _view.PersonalDataMessage = String.Empty; return true; }

            // The entity is dirty so do an update
            return UpdatePersonalInfo();
        }

        public Boolean DoSaveServiceInfo(IndividualEntity entity)
        {
            // The entity is dirty so do an update
            return UpdateServiceInfo();
        }

        private Boolean IsPersonalInfoDirty(IndividualEntity original, IndividualEntity compare)
        {
            if (!original.FirstName.ToLower().Equals(compare.FirstName.ToLower())) return true;
            if (!original.LastName.ToLower().Equals(compare.LastName.ToLower())) return true;
            if (!original.MiddleName.ToLower().Equals(compare.MiddleName.ToLower())) return true;
            if (!original.Gender.ToLower().Equals(compare.Gender.ToLower())) return true;
            if (!original.DateOfBirth.Equals(compare.DateOfBirth)) return true;
            if (!original.Comments.ToLower().Equals(compare.Comments.ToLower())) return true;

            return false;
        }

        private Boolean UpdatePersonalInfo()
        {
            PatientEntity _patient = new PatientEntity();
            _patient.Individual = new IndividualEntity();

            if (_view.EADStopDate != null)
            {
                var d = _view.EADStopDate.Value;
                if (d.Date < DateTime.Today.Date || d.Date > DateTime.Today.AddYears(2))
                {
                    _view.PersonalDataMessage = "The EAD Expiration Date is not within range";
                    return false;
                }
            }
            _patient.Individual.Demographic = Helpers.BuildProfile(_view.RankTypeSelected, _view.BOSTypeSelected, _view.StatusTypeSelected, _view.Gender, "N");
            _patient.Individual.EADStopDate = Helpers.SetDateOrDefault(_view.EADStopDate);
            _patient.Individual.DateOfBirth = Helpers.SetDateOrDefault(_view.DOB);
            _patient.Individual.FirstName = _view.FirstName;
            _patient.Individual.IsActive = true;
            _patient.Individual.IsPOC = false;
            _patient.Individual.LastName = _view.Lastname;
            _patient.Individual.MiddleName = string.IsNullOrEmpty(_view.MiddleName) ? string.Empty : _view.MiddleName;
            _patient.Individual.SiteCodeID = _view.mySession.Patient.Individual.SiteCodeID;
            _patient.Individual.Comments = _view.Comments;
            _patient.Individual.TheaterLocationCode = _view.TheaterLocationCodeSelected;
            _patient.Individual.ModifiedBy = _view.mySession.ModifiedBy;
            _patient.Individual.ID = _view.mySession.Patient.Individual.ID;

            var _individualRepository = new IndividualRepository();
            var indUpdate = _individualRepository.UpdateIndividual(_patient.Individual);

            _view.mySession.Patient.Individual = _patient.Individual;
            _view.mySession.SelectedPatientID = _view.mySession.Patient.Individual.ID;

            if (!indUpdate.IsNullOrEmpty())
            {
                _view.PersonalDataMessage = "Your Personal Data was Updated";
                return true;
            }
            else
            {
                _view.PersonalDataMessage = "Your Personal Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        private Boolean UpdateServiceInfo()
        {
            PatientEntity _patient = new PatientEntity();
            _patient.Individual = new IndividualEntity();

            if (_view.EADStopDate != null)
            {
                var d = _view.EADStopDate.Value;
                if (d.Date < DateTime.Today.Date || d.Date > DateTime.Today.AddYears(2))
                {
                    _view.ServiceDataMessage = "The EAD expiration date is not within range!";
                    return false;
                }
            }
            _patient.Individual.Demographic = Helpers.BuildProfile(_view.RankTypeSelected, _view.BOSTypeSelected, _view.StatusTypeSelected, _view.Gender, "N");
            _patient.Individual.EADStopDate = Helpers.SetDateOrDefault(_view.EADStopDate);
            _patient.Individual.DateOfBirth = Helpers.SetDateOrDefault(_view.DOB);
            _patient.Individual.FirstName = _view.FirstName;
            _patient.Individual.IsActive = true;
            _patient.Individual.IsPOC = false;
            _patient.Individual.LastName = _view.Lastname;
            _patient.Individual.MiddleName = string.IsNullOrEmpty(_view.MiddleName) ? string.Empty : _view.MiddleName;
            _patient.Individual.SiteCodeID = _view.mySession.Patient.Individual.SiteCodeID;
            _patient.Individual.Comments = _view.Comments;
            _patient.Individual.TheaterLocationCode = _view.TheaterLocationCodeSelected;
            _patient.Individual.ModifiedBy = _view.mySession.ModifiedBy;
            _patient.Individual.ID = _view.mySession.Patient.Individual.ID;

            var _individualRepository = new IndividualRepository();
            var indUpdate = _individualRepository.UpdateIndividual(_patient.Individual);

            _view.mySession.Patient.Individual = _patient.Individual;
            _view.mySession.SelectedPatientID = _view.mySession.Patient.Individual.ID;

            if (!indUpdate.IsNullOrEmpty())
            {
                _view.ServiceDataMessage = "Your personal and service information was updated!";
                return true;
            }
            else
            {
                _view.ServiceDataMessage = "Your personal and service information was NOT successfully saved.<br />  Please try again later or contact the SRTS team to report the problem!";
                return false;
            }
        }

        #endregion PersonalInfo OPS

        #region ID NUMBER OPS

        public Boolean DoSaveIdNumber(IdentificationNumbersEntity entity)
        {
            if (entity.IsNull()) { _view.IdNumberMessage = "No ID number to save"; return false; }

            // Make sure that the supplied entity is of a type that exists in the original.  If not then it's a new add.
            if (_view.mySession.Patient.IDNumbers.FirstOrDefault(x => x.IDNumberType == entity.IDNumberType && x.IsActive).IsNull())
            {
                if (String.IsNullOrEmpty(entity.IDNumber)) { _view.IdNumberMessage = String.Empty; return true; }
                return SaveIdNumber(entity);
            }
            // Compare the entity with the original values in the session
            if (!IsIdEntityDirty(_view.mySession.Patient.IDNumbers.FirstOrDefault(x => x.IDNumberType == entity.IDNumberType && x.IsActive), entity)) { _view.IdNumberMessage = String.Empty; return true; }

            // The type is not new and the entity IS dirty then do an update
            return UpdateIdNumber(entity, false);
        }

        private Boolean IsIdEntityDirty(IdentificationNumbersEntity original, IdentificationNumbersEntity compare)
        {
            if (!original.IndividualID.Equals(compare.IndividualID)) return true;
            if (!original.IDNumber.Equals(compare.IDNumber)) return true;
            if (!original.IDNumberType.Equals(compare.IDNumberType)) return true;

            return false;
        }

        private Boolean SaveIdNumber(IdentificationNumbersEntity ine)
        {
            ine = FillIdNumberElements(ine);

            var _idRepository = new IdentificationNumbersRepository();

            _view.IdNumberMessage = ine.IDNumber.ValidateIDNumLength_forModals(ine.IDNumberType);

            if (!string.IsNullOrEmpty(_view.IdNumberMessage))
            {
                return false;
            }

            var tmpID = _idRepository.GetIdentificationNumberByIDNumber(ine.IDNumber, ine.IDNumberType, _view.mySession.ModifiedBy);
            if (!tmpID.IsNullOrEmpty())
            {
                _view.IdNumberMessage = "This " + ine.IDNumberType + " is a duplicate ID";
                return false;
            }

            var dt = _idRepository.InsertIdentificationNumbers(ine);

            if (!dt.IsNull())
            {
                _view.IdNumberMessage = "Your Identification Data was Saved";
                _view.mySession.Patient.IDNumbers.Add(dt);
                return true;
            }
            else
            {
                _view.IdNumberMessage = "Your Identification Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        private Boolean UpdateIdNumber(IdentificationNumbersEntity ine, Boolean updateStatusOnly)
        {
            ine = FillIdNumberElements(ine);

            _view.IdNumberMessage = ine.IDNumber.ValidateIDNumLength_forModals(ine.IDNumberType);

            if (!string.IsNullOrEmpty(_view.IdNumberMessage))
            {
                return false;
            }

            var _idRepository = new IdentificationNumbersRepository();

            if (!updateStatusOnly)
            {
                var tmpID = _idRepository.GetIdentificationNumberByIDNumber(ine.IDNumber, ine.IDNumberType, _view.mySession.ModifiedBy);
                if (!tmpID.IsNullOrEmpty())
                {
                    _view.IdNumberMessage = "This " + ine.IDNumberType + " is a duplicate ID";
                    return false;
                }
            }

            var ineUpdate = _idRepository.UpdateIdentificationNumbersByID(ine);

            if (ineUpdate.Count > 0)
            {
                _view.IdNumberMessage = "Your Identification Data was Updated";
                return true;
            }
            else
            {
                _view.IdNumberMessage = "Your Identification Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        private IdentificationNumbersEntity FillIdNumberElements(IdentificationNumbersEntity ine)
        {
            ine.IndividualID = _view.mySession.Patient.Individual.ID;
            ine.ModifiedBy = _view.mySession.ModifiedBy;
            return ine;
        }

        #endregion ID NUMBER OPS

        #region ADDRESS OPS
        public Boolean DoSaveAddress(AddressEntity entity)
        {
            var msg = String.Empty;
            var svc = new SharedAddressService(entity, _view.mySession);
            var good = svc.DoSaveAddress(out msg);
            _view.AddressMessage = msg;
            return good;
        }

        #region DELETE AFTER TESTING
        //public Boolean DoSaveAddress(AddressEntity entity)
        //{
        //    if (entity.IsNull()) { _view.AddressMessage = "No address to save"; return false; }
        //    // If there is no original entity then this is an add
        //    if (_view.mySession.Patient.Addresses.FirstOrDefault(x => x.AddressType == entity.AddressType && x.IsActive).IsNull())
        //    {
        //        if (String.IsNullOrEmpty(entity.Address1) && String.IsNullOrEmpty(entity.City) && entity.State.Equals("X") && entity.Country.Equals("X") && String.IsNullOrEmpty(entity.ZipCode)) { _view.AddressMessage = String.Empty; return true; }
        //        return SaveAddress(entity);
        //    }
        //    if (!IsAddressEntityDirty(_view.mySession.Patient.Addresses.FirstOrDefault(x => x.AddressType == entity.AddressType && x.IsActive), entity)) { _view.AddressMessage = String.Empty; return true; }
        //    // if the address fields are blank then it is to be removed
        //    entity.IsActive = !entity.Address1.IsNull() && !entity.Address2.IsNull() && !entity.City.IsNull() && !entity.ZipCode.IsNull() && !entity.UIC.IsNull();

        //    return UpdateAddress(entity);
        //}

        //private Boolean IsAddressEntityDirty(AddressEntity original, AddressEntity compare)
        //{
        //    if (!original.AddressType.ToLower().Equals(compare.AddressType.ToLower())) return true;
        //    if (!original.Address1.ToLower().Equals(compare.Address1.ToLower())) return true;
        //    if (!original.Address2.ToLower().Equals(compare.Address2.ToLower())) return true;
        //    if (!original.City.ToLower().Equals(compare.City.ToLower())) return true;
        //    if (!original.State.ToLower().Equals(compare.State.ToLower())) return true;
        //    if (!original.Country.ToLower().Equals(compare.Country.ToLower())) return true;
        //    if (!original.ZipCode.ToLower().Equals(compare.ZipCode.ToLower())) return true;
        //    if (!original.UIC.ToLower().Equals(compare.UIC.ToLower())) return true;
        //    return false;
        //}

        //private Boolean SaveAddress(AddressEntity ae)
        //{
        //    ae = FillAddressElements(ae);

        //    var _addrRepository = new AddressRepository();

        //    var dt = _addrRepository.InsertAddress(ae);

        //    if (dt.Count > 0)
        //    {
        //        _view.AddressMessage = "Your Address Data was Saved";
        //        _view.mySession.Patient.Addresses.AddRange(dt.ToArray());
        //        return true;
        //    }
        //    else
        //    {
        //        _view.AddressMessage = "Your Address Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
        //        return false;
        //    }
        //}

        //private Boolean UpdateAddress(AddressEntity addr)
        //{
        //    addr = FillAddressElements(addr);

        //    var _addrRepository = new AddressRepository();
        //    var addressUpdate = _addrRepository.UpdateAddress(addr);
        //    if (!addressUpdate.IsNullOrEmpty())
        //    {
        //        _view.AddressMessage = "Your Address Data was Updated";
        //        return true;
        //    }
        //    else
        //    {
        //        _view.AddressMessage = "Your Address Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
        //        return false;
        //    }
        //}

        //private AddressEntity FillAddressElements(AddressEntity addr)
        //{
        //    addr.IndividualID = _view.mySession.Patient.Individual.ID;
        //    addr.ModifiedBy = _view.mySession.ModifiedBy;
        //    return addr;
        //}
        #endregion

        #endregion ADDRESS OPS

        #region EMAIL OPS

        public Boolean DoSaveEmail(EMailAddressEntity entity)
        {
            if (entity.IsNull()) { _view.EmailMessage = "No email address to save"; return false; }
            if (_view.mySession.Patient.EMailAddresses.FirstOrDefault(x => x.EMailType == entity.EMailType && x.IsActive).IsNull())
                return SaveEmail(entity);

            if (_view.mySession.Patient.EMailAddresses.FirstOrDefault(x => x.EMailType == entity.EMailType && x.IsActive).EMailAddress.Equals(entity.EMailAddress)) { _view.EmailMessage = String.Empty; return true; }

            return UpdateEmail(entity);
        }

        private Boolean SaveEmail(EMailAddressEntity eae)
        {
            eae = FillEmailElements(eae);

            var _emailRepository = new EMailAddressRepository();
            var dt = _emailRepository.InsertEMailAddress(eae);

            if (dt.Count > 0)
            {
                _view.EmailMessage = "Your Email Address Data was Saved";
                _view.mySession.Patient.EMailAddresses.AddRange(dt.ToArray());
                return true;
            }
            else
            {
                _view.EmailMessage = "Your Email Address Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        private Boolean UpdateEmail(EMailAddressEntity eme)
        {
            eme = FillEmailElements(eme);

            var _emailRepository = new EMailAddressRepository();
            var email = _emailRepository.UpdateEMailAddress(eme);
            if (email.Count > 0)
            {
                _view.EmailMessage = "Your Email Address Data was Updated";
                return true;
            }
            else
            {
                _view.EmailMessage = "Your Email Address Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        private EMailAddressEntity FillEmailElements(EMailAddressEntity eme)
        {
            eme.EMailAddress = String.IsNullOrEmpty(eme.EMailAddress.Trim()) ? "MAIL.MAIL@MAIL.MAIL" : eme.EMailAddress;
            eme.IndividualID = _view.mySession.Patient.Individual.ID;
            eme.ModifiedBy = _view.mySession.ModifiedBy;
            return eme;
        }

        #endregion EMAIL OPS

        #region PHONE OPS

        public Boolean DoSavePhone(PhoneNumberEntity entity)
        {
            if (entity.IsNull()) { _view.PhoneNumberMessage = "No phone number to save"; return false; }
            if (_view.mySession.Patient.PhoneNumbers.FirstOrDefault(x => x.PhoneNumberType == entity.PhoneNumberType && x.IsActive).IsNull())
                return AddPhone(entity);

            if (!IsPhoneEntityDirty(_view.mySession.Patient.PhoneNumbers.FirstOrDefault(x => x.PhoneNumberType == entity.PhoneNumberType && x.IsActive), entity)) { _view.PhoneNumberMessage = String.Empty; return true; }

            return UpdatePhone(entity);
        }

        private Boolean IsPhoneEntityDirty(PhoneNumberEntity original, PhoneNumberEntity entity)
        {
            if (!original.PhoneNumber.Equals(entity.PhoneNumber)) return true;
            if (!original.Extension.Equals(entity.Extension)) return true;
            if (!original.PhoneNumberType.Equals(entity.PhoneNumberType)) return true;
            return false;
        }

        private Boolean AddPhone(PhoneNumberEntity pne)
        {
            pne = FillPhoneElements(pne);
            var _phoneRepository = new PhoneRepository();
            var dt = _phoneRepository.InsertPhoneNumber(pne);

            if (dt.Count > 0)
            {
                _view.PhoneNumberMessage = "Your Phone Data was Saved";
                _view.mySession.Patient.PhoneNumbers.AddRange(dt.ToArray());
                return true;
            }
            else
            {
                _view.PhoneNumberMessage = "Your Phone Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        private Boolean UpdatePhone(PhoneNumberEntity pne)
        {
            pne = FillPhoneElements(pne);
            var _phoneRepository = new PhoneRepository();
            var phoneUpdate = _phoneRepository.UpdatePhoneNumber(pne);

            if (phoneUpdate.Count > 0)
            {
                _view.PhoneNumberMessage = "Your Phone Data was Updated";
                return true;
            }
            else
            {
                _view.PhoneNumberMessage = "Your Phone Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        private PhoneNumberEntity FillPhoneElements(PhoneNumberEntity pne)
        {
            pne.Extension = String.IsNullOrEmpty(pne.Extension.Trim()) ? String.Empty : pne.Extension;
            pne.IndividualID = _view.mySession.Patient.Individual.ID;
            pne.ModifiedBy = _view.mySession.ModifiedBy;
            pne.PhoneNumber = String.IsNullOrEmpty(pne.PhoneNumber.Trim()) ? "000-0000" : pne.PhoneNumber;
            return pne;
        }

        #endregion PHONE OPS

        #region INDIVIDUAL OPS

        public void BindIndividualType()
        {
            var _indTypeRepository = new IndividualTypeRepository();
            _view.IndividualTypesBind = _indTypeRepository.GetIndividualTypesByIndividualId(_view.mySession.SelectedPatientID);
        }

        public Boolean UpdateIndividualTypes(int IndividualId, string ModifiedBy)
        {
            var indTypes = GetIndivualTypes();
            var r = new IndividualTypeRepository();
            return r.InsertIndividualTypes(IndividualId, ModifiedBy, indTypes, true);
        }

        public String GetIndivualTypes()
        {
            List<string> indTypes = new List<string>();

            if (_view.IsAdmin) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "OTHER").Value.ToString());

            if (_view.IsTechnician) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "TECHNICIAN").Value.ToString());

            if (_view.IsProvider) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "PROVIDER").Value.ToString());

            return String.Join(",", indTypes);
        }

        public Boolean InsertIndivualSites(List<String> _sites, int IndividualId)
        {
            String sites = String.Join(",", _sites);
            var r = new IndividualRepository();

            return r.InsertIndividualSiteCode(IndividualId, sites) == 1;
        }

        #endregion INDIVIDUAL OPS

        #region PersonalData/ServiceData

        private void PopulateDropDowns()
        {
            _view.IndividualType = _view.LookupCache.GetByType(LookupType.IndividualType.ToString());

            var tmp = _view.mySession.Patient != null &&
                _view.mySession.Patient.Individual != null &&
                !String.IsNullOrEmpty(_view.mySession.Patient.Individual.SiteCodeID) ?
                _view.mySession.Patient.Individual.SiteCodeID : String.Empty;

            _view.SiteSelected = String.IsNullOrEmpty(tmp) ? _view.mySession.MyClinicCode : tmp;
            var tcr = new TheaterCodeRepository();
            //_view.TheaterLocationCodes = tcr.GetActiveTheaterCodes();
        }

        private void FillPatientItems()
        {
            _view.BOSTypeSelected = _view.mySession.Patient.Individual.Demographic.ToBOSKey();
            _view.Comments = _view.mySession.Patient.Individual.Comments;
            _view.DOB = _view.mySession.Patient.Individual.DateOfBirth;
            _view.EADStopDate = Helpers.CheckDateForGoodDate(_view.mySession.Patient.Individual.EADStopDate) ? _view.mySession.Patient.Individual.EADStopDate : null;
            _view.SiteCode = _view.mySession.Patient.Individual.SiteCodeID;
            _view.FirstName = _view.mySession.Patient.Individual.FirstName;
            _view.Gender = _view.mySession.Patient.Individual.Demographic.ToGenderKey();
            _view.Lastname = _view.mySession.Patient.Individual.LastName;
            _view.MiddleName = _view.mySession.Patient.Individual.MiddleName;
            string demoRank = _view.mySession.Patient.Individual.Demographic.ToRankKey();
            if (demoRank.ToLower().Substring(0, 2) == "w0")
            {
                var ie = new IndividualEntity();
                ie.ID = _view.mySession.Patient.Individual.ID;
                ie.PersonalType = _view.mySession.Patient.Individual.PersonalType;
                ie.FirstName = _view.mySession.Patient.Individual.FirstName;
                ie.LastName = _view.mySession.Patient.Individual.LastName;
                ie.MiddleName = _view.mySession.Patient.Individual.MiddleName;
                ie.DateOfBirth = _view.mySession.Patient.Individual.DateOfBirth;
                ie.EADStopDate = _view.mySession.Patient.Individual.EADStopDate;
                ie.IsPOC = _view.mySession.Patient.Individual.IsPOC;
                ie.SiteCodeID = _view.mySession.Patient.Individual.SiteCodeID;
                ie.Comments = _view.mySession.Patient.Individual.Comments;
                ie.IsActive = _view.mySession.Patient.Individual.IsActive;
                ie.TheaterLocationCode = _view.mySession.Patient.Individual.TheaterLocationCode;
                ie.Demographic = _view.mySession.Patient.Individual.Demographic.Remove(0, 2).Insert(0, "WO");
                ie.ModifiedBy = "SRTSWebSystem";

                var ir = new IndividualRepository();
                ir.UpdateIndividual(ie);

                _view.mySession.Patient.Individual.Demographic = ie.Demographic;
                var tsb = new StringBuilder(demoRank);
                tsb.Remove(0, 2);
                tsb.Insert(0, "WO");
                demoRank = tsb.ToString();
            }
            _view.RankTypeSelected = demoRank;
            _view.SiteSelected = _view.mySession.Patient.Individual.SiteCodeID;
            _view.StatusTypeSelected = _view.mySession.Patient.Individual.Demographic.ToPatientStatusKey();
            _view.TheaterLocationCodeSelected = Helpers.DisplayLocationCode(_view.mySession.Patient.Individual.TheaterLocationCode);
        }

        #endregion PersonalData/ServiceData
    }
}