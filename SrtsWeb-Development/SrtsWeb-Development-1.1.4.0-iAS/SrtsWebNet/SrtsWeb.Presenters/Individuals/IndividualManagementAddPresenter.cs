using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Presenters.Dmdc;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Individuals;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.Presenters.Individuals
{
    public sealed class IndividualManagementAddPresenter
    {
        private IIndividualManagementAddView _view;

        public IndividualManagementAddPresenter(IIndividualManagementAddView view)
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
            var _repository = new SiteRepository.SiteCodeRepository();
            _view.Sites = _repository.GetAllSites();
            _view.SiteSelected = _view.mySession.MyClinicCode;

            _view.IndividualTypeLookup = _view.LookupCache.GetByType(LookupType.IndividualType.ToString()).Select(x => new KeyValueEntity { Key = x.Text, Value = x.Id }).ToList(); ;// Helpers.GetLookupTypesSelected(_view.LookupCache, LookupType.IndividualType.ToString())
        }

        public Boolean SearchIndividual()
        {
            var _individualRepository = new IndividualRepository();

            var i = _individualRepository.GetIndividualByIDNumberIDNumberType(_view.IDNumber, _view.IDNumberTypeSelected, _view.mySession.MyUserID);
            _view.SearchedIndividuals = i;
            _view.ErrorMessage = i.Count.Equals(0) ? String.Empty : "Individual already exists in system, verify last 4 of patient ID.";
            return i.Count > 0;
        }

        public Boolean SearchIndividualDmdc()
        {
            var good = true;
            var idNum = this._view.IDNumber;
            var idNumType = this._view.IDNumberTypeSelected;

            var p = new DmdcPresenter(new DmdcService());
            var list = p.Get(idNum);

            // If there are no matches then return false
            //if (list.Count.Equals(0)) return false;
            if (list.IsNull())
            {
                list = new DmdcPerson()
                {
                    _DmdcIdentifier = new List<DmdcIdentifiers>(),
                    _DmdcPersonnel = new DmdcPersonnel()
                };
                good = false;
            }

            // If a match is found the convert DMDCPerson to Individual class
            //var d = list[0];

            // Transfer data from the DmdcPerson object to the IndividualEntity object
            var ids = new List<IdentificationNumbersEntity>();
            foreach (var a in list._DmdcIdentifier)
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

            _view.FirstName = list.PnFirstName;
            _view.MiddleName = list.PnMiddleName;
            _view.Lastname = list.PnLastName;
            _view.DOB = list.PnDateOfBirth;

            var x = new DemographicXMLHelper();
            if (!String.IsNullOrEmpty(list._DmdcPersonnel.PnCategoryCode))
            {
                _view.BoSType = x.GetALLBOS();
                switch (list._DmdcPersonnel.PnCategoryCode)
                {
                    case "00":
                    case "32":
                    case "65":
                    case "B":
                    case "C":
                    case "I":
                    case "K":
                    case "L":
                    case "M":
                    case "T":
                    case "U":
                    case "W":
                    case "Y":
                        list._DmdcPersonnel.ServiceCode = "K";
                        break;
                }
                _view.BOSTypeSelected = list._DmdcPersonnel.ServiceCode;

                if (!String.IsNullOrEmpty(list._DmdcPersonnel.PnCategoryCode))
                {
                    _view.StatusType = x.GetStatusByBOS(list._DmdcPersonnel.ServiceCode);
                    _view.StatusTypeSelected = list._DmdcPersonnel.PnCategoryCode;

                    if (!String.IsNullOrEmpty(list._DmdcPersonnel.PayPlanCode) && !String.IsNullOrEmpty(list._DmdcPersonnel.PayGrade))
                    {
                        _view.RankType = x.GetRanksByBOSAndStatus(list._DmdcPersonnel.ServiceCode, list._DmdcPersonnel.PnCategoryCode);

                        // The only grades I can handle at the moment are military and civ
                        if (list._DmdcPersonnel.PayPlanCode.Equals("ME") || list._DmdcPersonnel.PayPlanCode.Equals("MO") || list._DmdcPersonnel.PayPlanCode.Equals("MW"))
                            _view.RankTypeSelected = String.Format("{0}{1}", list._DmdcPersonnel.PayPlanCode.Substring(1), list._DmdcPersonnel.PayGrade);
                        else if (list._DmdcPersonnel.PayPlanCode.Equals("GS"))
                            _view.RankTypeSelected = "CIV";
                    }
                }
            }

            return true;
        }

        public void AddIndividualRecord()
        {
            PatientEntity _patient = new PatientEntity();
            _patient.Individual = new IndividualEntity();
            _patient.IDNumbers = new List<IdentificationNumbersEntity>();

            var ines = _view.AdditionalDmdcIds;
            var _individualRepository = new IndividualRepository();

            // If the id numbers from DMDC are null then use the one typed in
            //  Add to the dmdc id list obj.
            if (ines.IsNullOrEmpty())
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
                if (!tmpID.IsNullOrEmpty() && tmpID.Count > 0)
                {
                    _view.ErrorMessage = "Individual already exists in system, verify last 4 of patient ID.";
                    return;
                }
                tmpID = null;
            }

            _patient.IDNumbers = ines;
            //_patient.Individual.Demographic = SrtsHelper.BuildProfile(_view.RankTypeSelected, _view.BOSTypeSelected, _view.StatusTypeSelected, _view.Gender, _view.OrderPrioritySelected);
            _patient.Individual.Demographic = Helpers.BuildProfile(_view.RankTypeSelected, _view.BOSTypeSelected, _view.StatusTypeSelected, _view.Gender, "N");
            _patient.Individual.EADStopDate = Convert.ToDateTime("01/01/1900");

            if (_view.DOB != null)
            {
                var db = new DateTime();
                if (!DateTime.TryParse(_view.DOB.Value.ToString(), out db))
                {
                    _view.ErrorMessage = "Date Of Birth is Not Correct";
                    return;
                }
                _patient.Individual.DateOfBirth = db;
            }

            _patient.Individual.FirstName = _view.FirstName;
            _patient.Individual.IsActive = true;
            _patient.Individual.IsPOC = _view.IsPOC;
            _patient.Individual.LastName = _view.Lastname;
            _patient.Individual.MiddleName = _view.MiddleName;
            _patient.Individual.SiteCodeID = _view.SiteSelected;
            _patient.Individual.Comments = _view.Comments;
            //_patient.Individual.PersonalType = _view.IndividualTypeSelected;
            _patient.Individual.PersonalType = GetIndivualTypes();

            _patient.Individual.ModifiedBy = _view.mySession.MyUserID;

            var ir = new IndividualRepository.InsertIndividualReository();
            var lPat = ir.InsertIndividual(_patient);

            if (!lPat.IsNullOrEmpty()) //Table 2 is the select return of the newly inserted record...
            {
                _view.mySession.Patient.Individual = _patient.Individual;
                _view.mySession.SelectedPatientID = lPat[0].ID;// Convert.ToInt32(ds.Tables[2].Rows[0]["ID"].ToString());
                _view.mySession.Patient.Individual.ID = _view.mySession.SelectedPatientID;

                if (lPat[0].LastName.ToUpper().Equals(_patient.Individual.LastName.ToUpper()))
                {
                    // If there is another ID number, because DMDC will usually return 2 id numbers, in the id numbers list then add it here...
                    if (ines.Count.Equals(2))
                    {
                        var r = new IdentificationNumbersRepository();
                        ines[1].IndividualID = _view.mySession.SelectedPatientID;
                        r.InsertIdentificationNumbers(ines[1]);
                        r = null;
                    }
                }
                //else
                //{
                //    _view.ErrorMessage = "This is a duplicate ID Number, correct or search for this ID Number to find out who it belongs to.";
                //}
            }

        }

        public String GetIndivualTypes()
        {
            List<string> indTypes = new List<string>();

            if (_view.IsAdmin) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "OTHER").Value.ToString());

            //if (_view.IsPatient) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "PATIENT").Value.ToString());

            if (_view.IsTechnician) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "TECHNICIAN").Value.ToString());

            if (_view.IsProvider) indTypes.Add(_view.IndividualTypeLookup.FirstOrDefault(x => x.Key.ToString() == "PROVIDER").Value.ToString());

            return String.Join(",", indTypes);
        }
    }
}