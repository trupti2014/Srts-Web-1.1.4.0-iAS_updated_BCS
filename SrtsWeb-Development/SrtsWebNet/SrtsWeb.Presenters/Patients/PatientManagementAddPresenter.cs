using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Presenters.Dmdc;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Patients;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.Patients
{
    public sealed class PatientManagementAddPresenter
    {
        private IPatientManagementAddView _view;

        public PatientManagementAddPresenter(IPatientManagementAddView view)
        {
            _view = view;
        }

        public void InitView()
        {
            PopulateDropDowns();
        }

        private void PopulateDropDowns()
        {
            _view.IDNumberType = _view.LookupCache.GetByType(LookupType.IDNumberType.ToString());

            var tcr = new TheaterCodeRepository();
            _view.TheaterLocationCodes = tcr.GetActiveTheaterCodes();
            var r = new LookupService();
            _view.IndividualTypeLookup = r.GetLookupsByType(LookupType.IndividualType.ToString()).Select(x => new KeyValueEntity { Key = x.Text, Value = x.Id }).ToList();
        }

        public void AddPatientRecord()
        {
            PatientEntity _patient = new PatientEntity();
            var _individualRepository = new IndividualRepository();
            var ines = _view.AdditionalDmdcIds;
            _patient.Addresses = new List<AddressEntity>();
            _patient.Individual = new IndividualEntity();
            _patient.IDNumbers = new List<IdentificationNumbersEntity>();
            string gender = string.Empty;

            _view.mySession.Patient = _patient;

            // If the id numbers from DMDC are null then use the one typed in
            //  Add to the dmdc id list obj.
            if (ines == null || ines.Count.Equals(0))
            {
                ines = new List<IdentificationNumbersEntity>();
                ines.Add(new IdentificationNumbersEntity()
                {
                    IDNumber = _view.IDNumber.ToSSNRemoveDash(),
                    IDNumberType = _view.IDNumberTypeSelected,
                    ModifiedBy = _view.mySession.MyUserID,
                    IsActive = true
                });
            }

            // Ensure that none of the ID's in the list are duplicates...
            foreach (var i in ines)
            {
                var tmpID = _individualRepository.GetIndividualByIDNumberIDNumberType(i.IDNumber, i.IDNumberType, _view.mySession.MyUserID);
                    if (tmpID != null)
                    if (tmpID.Count > 0)
                        {
                            _view.ErrorMessage = "This is a duplicate ID";
                            return;
                        }
                }

            if (_view.Gender != "")
            {
                gender = _view.Gender.Equals("M") ? "M" : "F";
            }
            else
            {
                gender = "N";
            }
            _patient.IDNumbers = ines;
            _patient.Individual.Demographic = Helpers.BuildProfile(_view.RankTypeSelected, _view.BOSTypeSelected, _view.StatusTypeSelected, gender, "N");
            _patient.Individual.EADStopDate = Helpers.SetDateOrDefault(_view.EADStopDate);
            _patient.Individual.DateOfBirth = Helpers.SetDateOrDefault(_view.DOB);
            _patient.Individual.FirstName = _view.FirstName;
            _patient.Individual.IsActive = true;
            _patient.Individual.IsPOC = false;
            _patient.Individual.LastName = _view.Lastname;
            _patient.Individual.MiddleName = string.IsNullOrEmpty(_view.MiddleName) ? string.Empty : _view.MiddleName;
            _patient.Individual.SiteCodeID = _view.SiteSelected;
            _patient.Individual.Comments = _view.Comments;
            _patient.Individual.TheaterLocationCode = _view.TheaterLocationCodeSelected;
            //_patient.Individual.PersonalType = "PATIENT";
            _patient.Individual.PersonalType = _view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "PATIENT").Value.ToString();
            _patient.Individual.ModifiedBy = _view.mySession.MyUserID;

            var ir = new IndividualRepository.InsertIndividualReository();
            
            var ind = ir.InsertIndividual(_patient).FirstOrDefault();

            if (ind.LastName != null && _view.mySession.Patient.Individual.LastName == ind.LastName)
            {
                _view.mySession.Patient.Individual = ind;
                _view.mySession.SelectedPatientID = _view.mySession.Patient.Individual.ID;

                // If there is another ID number, because DMDC will usually return 2 id numbers, in the id numbers list then add it here...
                if (ines.Count.Equals(2))
                {
                    var r = new IdentificationNumbersRepository();
                    ines[1].IndividualID = _view.mySession.SelectedPatientID;
                    r.InsertIdentificationNumbers(ines[1]);
                    r = null;
                }
            }
            else
            {
                _view.ErrorMessage = "Unknown error is preventing the addition of this patient!";
            }
        }

        public Boolean PhoneNumbersExist(Int32 individualId)
        {
            var _phoneRepository = new PhoneRepository();
            var p = _phoneRepository.GetPhoneNumbersByIndividualID(individualId, _view.mySession.MyUserID);

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
                    pne.ModifiedBy = _view.mySession.MyUserID;
                    pne.PhoneNumber = p[0].PhoneNumber;
                    pne.PhoneNumberType = p[0].PhoneNumberType;
                };

                var dt = _phoneRepository.UpdatePhoneNumber(pne);
                return !dt.Count.Equals(0);
            }
            return a > 0;
        }

        public Boolean SearchPatient()
        {
            var _individualRepository = new IndividualRepository();

            var i = _individualRepository.GetIndividualByIDNumberIDNumberType(_view.IDNumber, _view.IDNumberTypeSelected, _view.mySession.MyUserID);
            _view.SearchedPatients = i;
            _view.ErrorMessage = i.Count.Equals(0) ? String.Empty : "Patient already exists in system, verify last 4 of patient ID.";
            return i.Count > 0;
        }

        public Boolean SearchPatient(String IdNumber, String IdNumberType)
        {
            var _individualRepository = new IndividualRepository();

            var i = _individualRepository.GetIndividualByIDNumberIDNumberType(IdNumber, IdNumberType, _view.mySession.MyUserID);
            _view.SearchedPatients = i;
            _view.ErrorMessage = i.Count.Equals(0) ? String.Empty : "Patient already exists in system, verify last 4 of patient ID.";
            return i.Count > 0;
        }

        public Boolean SearchPatientDmdc(IComboAddView _addView)
        {
            var good = true;
            var idNum = this._view.IDNumber;
            var idNumType = this._view.IDNumberTypeSelected;

            var p = new DmdcPresenter(new DmdcService());
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
            //if (list.IsNullOrEmpty() || list[0].IsNull())
            //{
            //    list.Clear();
            //    // Clear the form...
            //    list.Add(new DmdcPerson()
            //    {
            //        _DmdcIdentifier = new List<DmdcIdentifiers>(),
            //        _DmdcPersonnel = new DmdcPersonnel()
            //    });
            //    good = false;
            //}

            // If a match is found the convert DMDCPerson to Individual class
            //var d = list[0];

            var ids = new List<IdentificationNumbersEntity>();
            foreach (var a in d._DmdcIdentifier)
            {
                ids.Add(new IdentificationNumbersEntity()
                {
                    IDNumber = a.PnId,
                    IDNumberType = a.PnIdType.Equals("S") ? "SSN" : "DIN",
                    ModifiedBy = _view.mySession.MyUserID,
                    IndividualID = _view.mySession.SelectedPatientID
                });
            }

            _view.AdditionalDmdcIds = ids;
            _view.FirstName = d.PnFirstName;
            _view.Lastname = d.PnLastName;
            _view.DOB = d.PnDateOfBirth;

            _addView.Address1 = d.MailingAddress1;
            _addView.Address2 = d.MailingAddress2;
            _addView.City = d.MailingCity;
            _addView.CountrySelected = d.MailingCountry;
            _addView.StateSelected = d.MailingState;
            _addView.ZipCode = String.Format("{0}", String.IsNullOrEmpty(d.MailingZipExtension) ? String.Format("{0}", d.MailingZip) : String.Format("{0}-{1}", d.MailingZip, d.MailingZipExtension));

            var x = new DemographicXMLHelper();
            if (!String.IsNullOrEmpty(d._DmdcPersonnel.PnCategoryCode))
            {
                _view.BoSType = x.GetALLBOS();
                _view.BOSTypeSelected = d._DmdcPersonnel.ServiceCode;

                if (!String.IsNullOrEmpty(d._DmdcPersonnel.PnCategoryCode))
                {
                    _view.StatusType = x.GetStatusByBOS(d._DmdcPersonnel.ServiceCode);
                    _view.StatusTypeSelected = d._DmdcPersonnel.PnCategoryCode;

                    if (!String.IsNullOrEmpty(d._DmdcPersonnel.PayPlanCode) && !String.IsNullOrEmpty(d._DmdcPersonnel.PayGrade))
                    {
                        _view.RankType = x.GetRanksByBOSAndStatus(d._DmdcPersonnel.ServiceCode, d._DmdcPersonnel.PnCategoryCode);

                        // The only grades I can handle at the moment are military and civ
                        if (d._DmdcPersonnel.PayPlanCode.Equals("ME") || d._DmdcPersonnel.PayPlanCode.Equals("MO") || d._DmdcPersonnel.PayPlanCode.Equals("MW"))
                        {
                            var pg = d._DmdcPersonnel.PayPlanCode.Equals("MW") ? String.Format("O{0}", d._DmdcPersonnel.PayGrade.Substring(1)) : d._DmdcPersonnel.PayGrade;
                            _view.RankTypeSelected = String.Format("{0}{1}", d._DmdcPersonnel.PayPlanCode.Substring(1), pg);
                        }
                        else if (d._DmdcPersonnel.PayPlanCode.Equals("GS"))
                            _view.RankTypeSelected = "CIV";
                    }
                }
            }
            else
            {
                _view.RankType = null;
                _view.StatusType = null;

                _view.BoSType = x.GetALLBOS();
                _view.BOSTypeSelected = "X";
            }
            _addView.EMailAddress = d.Email;
            _addView.PhoneNumber = d.PhoneNumber;

            return good;
        }

        public Boolean AddIndividualType(Int32 individualId)
        {
            List<IndividualEntity> i = _view.SearchedPatients;
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
                _view.ErrorMessage = "There was an error updating the individual record to active.";
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
                _view.ErrorMessage = "There was an error updating the individual type record of patient to active.";
                return false;
            }

            try
            {
                // If there is not a patient individual type record then add it...
                //var ite = new IndividualTypeEntity()
                //{
                //    IndividualId = individualId,
                //    IsActive = true,
                //    TypeId = 110,
                //    ModifiedBy = _view.mySession.MyUserID
                //};

                //var dt = r.InsertIndividualType(ite);
                //return !dt.Rows.Count.Equals(0);

                int IndividualId = individualId;
                var ModifiedBy = _view.mySession.MyUserID;

                return r.InsertIndividualTypes(IndividualId, ModifiedBy, "110", false);
            }
            catch
            {
                _view.ErrorMessage = "There was an error adding an individual type of patient.";
                return false;
            }
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
                        pne.ModifiedBy = _view.mySession.MyUserID;
                        pne.PhoneNumber = "0000000";
                        pne.PhoneNumberType = "HOME";
                    };

                    var dt = r.InsertPhoneNumber(pne);
                    return !dt.Count.Equals(0);
                }

                catch
                {
                    _view.ErrorMessage = "There was an error adding the default phone number.";
                    return false;
                }
            }
            return true;
        }
    }
}