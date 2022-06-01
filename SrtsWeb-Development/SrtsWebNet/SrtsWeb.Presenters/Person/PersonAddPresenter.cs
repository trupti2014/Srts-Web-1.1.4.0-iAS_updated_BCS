using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Person;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.Person
{
    public class PersonAddPresenter
    {
        private IPersonAddView v;

        public PersonAddPresenter(IPersonAddView view)
        {
            this.v = view;
        }

        public void InitView()
        {
            FillDdls();
        }

        private void FillDdls()
        {
            try
            {
                var dx = new DemographicXMLHelper();
                this.v.BosList = dx.GetALLBOS();
                dx = null;

                var tcr = new TheaterCodeRepository();
               // this.v.TheaterLocationCodeList = tcr.GetActiveTheaterCodes();
                tcr = null;

                ///GET THIS CODE FROM THE INDIVIDUAL STUFF
                var r = new LookupService();
                this.v.IndividualTypeLookupList = r.GetLookupsByType(LookupType.IndividualType.ToString()).Select(x => new KeyValueEntity { Key = x.Text, Value = x.Id }).ToList();
                r = null;

                this.v.IDNumberTypeList = this.v.LookupCache.GetByType(LookupType.IDNumberType.ToString());

                if (!this.v.IsAdmin) return;

                var _repository = new SiteRepository.SiteCodeRepository();
                this.v.SiteList = _repository.GetAllSites().Where(x => x.IsActive == true).ToList();
                this.v.SiteCode = this.v.mySession.MyClinicCode;
                _repository = null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        public void GetStatus()
        {
            var dx = new DemographicXMLHelper();
            this.v.StatusList = dx.GetStatusByBOS(this.v.BranchOfService);
        }

        public void GetGrade()
        {
            var dx = new DemographicXMLHelper();
            this.v.RankList = dx.GetRanksByBOSAndStatus(this.v.BranchOfService, this.v.StatusCategory);
        }

        public Boolean SearchPerson()
        {
            var _individualRepository = new IndividualRepository();
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var i = _individualRepository.GetIndividualByIDNumberIDNumberType(this.v.IdNumber, this.v.IdNumberType, ModifiedBy);
            this.v.SearchedPersonList = i;
            this.v.ErrorMessage = i.Count.Equals(0) ? String.Empty : "Person already exists in system, verify last 4 of their ID.";
            return i.Count > 0;
        }

        public Boolean SearchPerson(String IdNumber, String IdNumberType)
        {
            var _individualRepository = new IndividualRepository();
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var i = _individualRepository.GetIndividualByIDNumberIDNumberType(IdNumber, IdNumberType, ModifiedBy);
            this.v.SearchedPersonList = i;
            this.v.ErrorMessage = i.Count.Equals(0) ? String.Empty : "Person already exists in system, verify last 4 of their ID.";
            return i.Count > 0;
        }

        public Boolean SearchPersonDmdc()
        {
            var good = true;
            var idNum = this.v.IdNumber;
            var idNumType = this.v.IdNumberType;

            var p = new SrtsWeb.Presenters.Dmdc.DmdcPresenter(new DmdcService());
            var d = p.Get(idNum);

            // If there are no matches then return false
            if (d.IsNull())
            {
                d = new DmdcPerson()
                {
                    _DmdcIdentifier = new List<DmdcIdentifiers>(),
                    _DmdcPersonnel = new DmdcPersonnel()
                };
                good = false;
            }

            var ids = new List<IdentificationNumbersEntity>();
            foreach (var a in d._DmdcIdentifier)
            {
                ids.Add(new IdentificationNumbersEntity()
                {
                    IDNumber = a.PnId,
                    IDNumberType = a.PnIdType.Equals("S") ? "SSN" : "DIN",
                    ModifiedBy = this.v.mySession.ModifiedBy,
                    IndividualID = this.v.mySession.SelectedPatientID
                });
            }

            this.v.AdditionalDmdcIdList = ids;
            this.v.FirstName = d.PnFirstName;
            this.v.LastName = d.PnLastName;
            this.v.DateOfBirth = d.PnDateOfBirth;

            this.v.Address1 = d.MailingAddress1;
            this.v.Address2 = d.MailingAddress2;
            this.v.City = d.MailingCity;
            this.v.Country = d.MailingCountry;
            this.v.State = d.MailingState;
            this.v.ZipCode = String.Format("{0}", String.IsNullOrEmpty(d.MailingZipExtension) ? String.Format("{0}", d.MailingZip) : String.Format("{0}-{1}", d.MailingZip, d.MailingZipExtension));

            var x = new DemographicXMLHelper();
            if (!String.IsNullOrEmpty(d._DmdcPersonnel.PnCategoryCode))
            {
                this.v.BosList = x.GetALLBOS();
                this.v.BranchOfService = d._DmdcPersonnel.ServiceCode;

                if (!String.IsNullOrEmpty(d._DmdcPersonnel.PnCategoryCode))
                {
                    this.v.StatusList = x.GetStatusByBOS(d._DmdcPersonnel.ServiceCode);
                    this.v.StatusCategory = d._DmdcPersonnel.PnCategoryCode;

                    if (!String.IsNullOrEmpty(d._DmdcPersonnel.PayPlanCode) && !String.IsNullOrEmpty(d._DmdcPersonnel.PayGrade))
                    {
                        this.v.RankList = x.GetRanksByBOSAndStatus(d._DmdcPersonnel.ServiceCode, d._DmdcPersonnel.PnCategoryCode);

                        // The only grades I can handle at the moment are military and civ
                        if (d._DmdcPersonnel.PayPlanCode.Equals("ME") || d._DmdcPersonnel.PayPlanCode.Equals("MO") || d._DmdcPersonnel.PayPlanCode.Equals("MW"))
                        {
                            var pg = d._DmdcPersonnel.PayPlanCode.Equals("MW") ? String.Format("O{0}", d._DmdcPersonnel.PayGrade.Substring(1)) : d._DmdcPersonnel.PayGrade;
                            this.v.Grade = String.Format("{0}{1}", d._DmdcPersonnel.PayPlanCode.Substring(1), pg);
                        }
                        else if (d._DmdcPersonnel.PayPlanCode.Equals("GS"))
                            this.v.Grade = "CIV";
                    }
                }
            }
            else
            {
                this.v.RankList = null;
                this.v.StatusList = null;

                this.v.BosList = x.GetALLBOS();
                this.v.BranchOfService = "X";
            }
            this.v.EmailAddress = d.Email;
            this.v.PhoneNumber = d.PhoneNumber;

            return good;
        }

        public Boolean AddIndividualType(Int32 individualId)
        {
            List<IndividualEntity> i = this.v.SearchedPersonList;
            try
            {
                // First, check if the individual record is active
                //  If not then activate it and keep going...
                var j = i.Where(x => x.IsActive == false).FirstOrDefault();
                if (j != null)
                {
                    j.IsActive = true;
                    // Update the individual record
                    var _individualRepository = new IndividualRepository();
                    _individualRepository.UpdateIndividual(j);
                }
            }
            catch
            {
                this.v.ErrorMessage = "There was an error updating the individual record to active.";
                return false;
            }

            var r = new IndividualTypeRepository();
            try
            {
                // Second, check if there is an individual type of patient
                //  If so then check if it is active
                //   If not then activate it and move on...
                var a = i.Where(x => x.PersonalType.ToLower() == "patient").FirstOrDefault();
                if (a != null)
                {
                    // Get all the types and find the Patient type...
                    List<IndividualTypeEntity> it = r.GetIndividualTypesByIndividualId(individualId);
                    var t = it.Where(x => x.TypeId == 110).FirstOrDefault();
                    if (t.IsActive.Equals(true)) return true;
                    else
                    {
                        // Update the individual type to be active...
                        a = null;

                        t.IsActive = true;
                        r.UpdateIndividualType(t);
                        return true;
                    }
                }
            }
            catch
            {
                this.v.ErrorMessage = "There was an error updating the individual type record of patient to active.";
                return false;
            }

            try
            {
                int IndividualId = individualId;
                var ModifiedBy = this.v.mySession.ModifiedBy;

                var pt = this.v.IndividualTypeLookupList.FirstOrDefault(x => x.Key.ToString() == "PATIENT").Value.ToString();
                return r.InsertIndividualTypes(IndividualId, ModifiedBy, pt, false);
            }
            catch
            {
                this.v.ErrorMessage = "There was an error adding an individual type of patient.";
                return false;
            }
        }

        public Boolean PhoneNumbersExist(Int32 individualId)
        {
            var _phoneRepository = new PhoneRepository();
            var p = _phoneRepository.GetPhoneNumbersByIndividualID(individualId, this.v.mySession.ModifiedBy);

            // Check to see if there are any phone numbers
            if (p.Count.Equals(0)) return false;

            // If so, check if any are active
            var a = p.Where(x => x.IsActive == true).Count();
            if (a.Equals(0))
            {
                // If not, make the first one active
                var pne = new PhoneNumberEntity();
                {
                    pne.Extension = p[0].Extension;
                    pne.IndividualID = individualId;
                    pne.ID = p[0].ID;
                    pne.IsActive = true;
                    pne.ModifiedBy = this.v.mySession.ModifiedBy;
                    pne.PhoneNumber = p[0].PhoneNumber;
                };

                var dt = _phoneRepository.UpdatePhoneNumber(pne);
                return !dt.Count.Equals(0);
            }
            return a > 0;
        }

        public Boolean AddDefaultPhoneNumber(Int32 individualId)
        {
            // Check if there are any phone numbers.  If not, add a default number.
            if (!PhoneNumbersExist(individualId))
            {
                try
                {
                    var r = new PhoneRepository();
                    var pne = new PhoneNumberEntity();
                    {
                        pne.Extension = string.Empty;
                        pne.IndividualID = individualId;
                        pne.IsActive = true;
                        pne.ModifiedBy = this.v.mySession.ModifiedBy;
                        pne.PhoneNumber = "0000000";
                    };

                    var dt = r.InsertPhoneNumber(pne);
                    return !dt.Count.Equals(0);
                }
                catch
                {
                    this.v.ErrorMessage = "There was an error adding the default phone number.";
                    return false;
                }
            }
            return true;
        }

        public Boolean SaveIDNumbers(IdentificationNumbersEntity ine)
        {
            var _idRepository = new IdentificationNumbersRepository();

            this.v.ErrorMessage = ine.IDNumber.ValidateIDNumLength_forModals(ine.IDNumberType);

            if (!string.IsNullOrEmpty(this.v.ErrorMessage))
                return false;

            var tmpID = _idRepository.GetIdentificationNumberByIDNumber(ine.IDNumber, ine.IDNumberType, this.v.mySession.ModifiedBy.ToString());
            if (!tmpID.IsNullOrEmpty())
            {
                this.v.ErrorMessage = "This is a duplicate ID";
                return false;
            }

            var dt = _idRepository.InsertIdentificationNumbers(ine);

            if (!dt.IsNull())
            {
                this.v.ErrorMessage = "Your Identification Data was Saved";
                return true;
            }
            else
            {
                this.v.ErrorMessage = "Your Identification Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        public void AddIndividualRecord()
        {
            PatientEntity _patient = new PatientEntity();
            _patient.Individual = new IndividualEntity();
            _patient.IDNumbers = new List<IdentificationNumbersEntity>();

            var ines = this.v.AdditionalDmdcIdList;
            var _individualRepository = new IndividualRepository();

            // If the id numbers from DMDC are null then use the one typed in
            //  Add to the dmdc id list obj.
            if (ines.IsNullOrEmpty())
            {
                ines = new List<IdentificationNumbersEntity>();
                ines.Add(new IdentificationNumbersEntity()
                {
                    IDNumber = this.v.IdNumber.ToSSNRemoveDash(),
                    IDNumberType = this.v.IdNumberType,
                    ModifiedBy = this.v.mySession.ModifiedBy,
                    IsActive = true
                });
            }

            // Ensure that none of the ID's in the list are duplicates...
            foreach (var i in ines)
            {
                var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
                var tmpID = _individualRepository.GetIndividualByIDNumberIDNumberType(i.IDNumber, i.IDNumberType, ModifiedBy);
                if (!tmpID.IsNullOrEmpty() && tmpID.Count > 0)
                {
                    this.v.ErrorMessage = "Individual already exists in system, verify last 4 of patient ID.";
                    return;
                }
                tmpID = null;
            }

            _patient.IDNumbers = ines;
            _patient.Individual.Demographic = Helpers.BuildProfile(this.v.Grade, this.v.BranchOfService, this.v.StatusCategory, this.v.Gender, "N");
            _patient.Individual.EADStopDate = Convert.ToDateTime("01/01/1900");

            if (this.v.DateOfBirth != null)
            {
                var db = new DateTime();
                if (!DateTime.TryParse(this.v.DateOfBirth.Value.ToString(), out db))
                {
                    this.v.ErrorMessage = "Date Of Birth is Not Correct";
                    return;
                }
                _patient.Individual.DateOfBirth = db;
            }

            _patient.Individual.FirstName = this.v.FirstName;
            _patient.Individual.IsActive = true;
            _patient.Individual.IsPOC = this.v.IsPOC;
            _patient.Individual.LastName = this.v.LastName;
            _patient.Individual.MiddleName = this.v.MiddleName;
            _patient.Individual.SiteCodeID = this.v.SiteCode;
            _patient.Individual.Comments = this.v.Comments;
            _patient.Individual.PersonalType = GetIndivualTypes();

            _patient.Individual.ModifiedBy = this.v.mySession.ModifiedBy;

            var ir = new IndividualRepository.InsertIndividualReository();
            var lPat = ir.InsertIndividual(_patient);

            if (!lPat.IsNullOrEmpty()) //Table 2 is the select return of the newly inserted record...
            {
                this.v.mySession.Patient.Individual = _patient.Individual;
                this.v.mySession.SelectedPatientID = lPat[0].ID;
                this.v.mySession.Patient.Individual.ID = this.v.mySession.SelectedPatientID;

                if (lPat[0].LastName.ToUpper().Equals(_patient.Individual.LastName.ToUpper()))
                {
                    // If there is another ID number, because DMDC will usually return 2 id numbers, in the id numbers list then add it here...
                    if (ines.Count.Equals(2))
                    {
                        var r = new IdentificationNumbersRepository();
                        ines[1].IndividualID = this.v.mySession.SelectedPatientID;
                        r.InsertIdentificationNumbers(ines[1]);
                        r = null;
                    }
                }
            }
        }

        public String GetIndivualTypes()
        {
            List<string> indTypes = new List<string>();
            try
            {
                if (this.v.IsAdminType) indTypes.Add(this.v.IndividualTypeLookupList.FirstOrDefault(x => x.Key.ToString() == "OTHER").Value.ToString());

                if (this.v.IsTechType) indTypes.Add(this.v.IndividualTypeLookupList.FirstOrDefault(x => x.Key.ToString() == "TECHNICIAN").Value.ToString());

                if (this.v.IsProviderType) indTypes.Add(this.v.IndividualTypeLookupList.FirstOrDefault(x => x.Key.ToString() == "PROVIDER").Value.ToString());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return String.Join(",", indTypes);
        }

        public void AddPatientRecord()
        {
            var _individualRepository = new IndividualRepository();
            var ines = this.v.AdditionalDmdcIdList;
            var ind = new IndividualEntity();
            var idNums = new List<IdentificationNumbersEntity>();
            string gender = string.Empty;

            this.v.mySession.Patient = new PatientEntity();

            // If the id numbers from DMDC are null then use the one typed in
            //  Add to the dmdc id list obj.
            if (ines == null || ines.Count.Equals(0))
            {
                ines = new List<IdentificationNumbersEntity>();
                ines.Add(new IdentificationNumbersEntity()
                {
                    IDNumber = this.v.IdNumber.ToSSNRemoveDash(),
                    IDNumberType = this.v.IdNumberType,
                    ModifiedBy = this.v.mySession.ModifiedBy,
                    IsActive = true
                });
            }

            // Ensure that none of the ID's in the list are duplicates...
            foreach (var i in ines)
            {
                var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
                var tmpID = _individualRepository.GetIndividualByIDNumberIDNumberType(i.IDNumber, i.IDNumberType, this.v.mySession.ModifiedBy);
                if (tmpID != null)
                    if (tmpID.Count > 0)
                    {
                        this.v.ErrorMessage = "This is a duplicate ID";
                        return;
                    }
            }

            if (this.v.Gender != "")
                gender = this.v.Gender.Equals("M") ? "M" : "F";
            else
                gender = "N";

            idNums = ines;
            ind.Demographic = Helpers.BuildProfile(this.v.Grade, this.v.BranchOfService, this.v.StatusCategory, gender, "N");
            ind.EADStopDate = Helpers.SetDateOrDefault(this.v.EADStopDate);
            ind.DateOfBirth = Helpers.SetDateOrDefault(this.v.DateOfBirth);
            ind.FirstName = this.v.FirstName;
            ind.IsActive = true;
            ind.IsPOC = false;
            ind.LastName = this.v.LastName;
            ind.MiddleName = string.IsNullOrEmpty(this.v.MiddleName) ? string.Empty : this.v.MiddleName;
            ind.SiteCodeID = this.v.SiteCode.Equals("X") ? this.v.mySession.MySite.SiteCode : this.v.SiteCode;
            ind.Comments = this.v.Comments;
            ind.TheaterLocationCode = this.v.TheaterLocationCode;
            ind.PersonalType = this.v.IndividualTypeLookupList.FirstOrDefault(x => x.Key.ToString() == "PATIENT").Value.ToString();
            ind.ModifiedBy = this.v.mySession.ModifiedBy;
            
            this.v.mySession.Patient.Individual = ind;
            this.v.mySession.Patient.IDNumbers = idNums;

            var ir = new IndividualRepository.InsertIndividualReository();
            var insInd = ir.InsertIndividual(ind, idNums[0]).FirstOrDefault();

            if (insInd.LastName != null && this.v.mySession.Patient.Individual.LastName == insInd.LastName)
            {
                this.v.mySession.Patient.Individual = insInd;
                this.v.mySession.SelectedPatientID = this.v.mySession.Patient.Individual.ID;

                // If there is another ID number, because DMDC will usually return 2 id numbers, in the id numbers list then add it here...
                if (ines.Count.Equals(2))
                {
                    var r = new IdentificationNumbersRepository();
                    ines[1].IndividualID = this.v.mySession.SelectedPatientID;
                    r.InsertIdentificationNumbers(ines[1]);
                    r = null;
                }
            }
            else
            {
                this.v.ErrorMessage = "Unknown error is preventing the addition of this patient!";
            }
        }

        public Boolean SaveAddress()
        {
            if (string.IsNullOrEmpty(this.v.Address1) && string.IsNullOrEmpty(this.v.Address2)) return true;

            AddressEntity ae = new AddressEntity();

            ae.Address1 = this.v.Address1;
            ae.Address2 = this.v.Address2;
            ae.City = this.v.City;
            ae.State = this.v.State;
            ae.ZipCode = this.v.ZipCode.ToZipCodeDisplay();
            ae.Country = this.v.Country;
            ae.UIC = this.v.UnitIdentificationCode;
            ae.IndividualID = this.v.mySession.Patient.Individual.ID;
            ae.IsActive = true;
            ae.ModifiedBy = this.v.mySession.ModifiedBy;

            var _addrRepository = new AddressRepository();

            var dt = _addrRepository.InsertAddress(ae);

            return dt.Count > 0;
            }

        public Boolean SaveEmail()
        {
            var eae = new EMailAddressEntity();
            eae.EMailAddress = String.IsNullOrEmpty(this.v.EmailAddress.Trim()) ? "MAIL.MAIL@MAIL.MAIL" : this.v.EmailAddress;
            eae.IndividualID = this.v.mySession.Patient.Individual.ID;
            eae.IsActive = true;
            eae.ModifiedBy = this.v.mySession.ModifiedBy;

            var _emailRepository = new EMailAddressRepository();
            var dt = _emailRepository.InsertEMailAddress(eae);

            return dt.Count > 0;
            }

        public Boolean SavePhone()
        {
            var pne = new PhoneNumberEntity();
            pne.Extension = String.IsNullOrEmpty(this.v.Extension.Trim()) ? string.Empty : this.v.Extension;
            pne.IndividualID = this.v.mySession.Patient.Individual.ID;
            pne.IsActive = true;
            pne.ModifiedBy = this.v.mySession.ModifiedBy;
            pne.PhoneNumber = String.IsNullOrEmpty(this.v.PhoneNumber.Trim()) ? "0000000" : this.v.PhoneNumber;

            var _phoneRepository = new PhoneRepository();
            var dt = _phoneRepository.InsertPhoneNumber(pne);

            return dt.Count > 0;
        }
    }
}
