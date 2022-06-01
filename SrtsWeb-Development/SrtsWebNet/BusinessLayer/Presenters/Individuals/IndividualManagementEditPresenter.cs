using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Individuals;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Presenters.Individuals
{
    public sealed class IndividualManagementEditPresenter
    {
        private IIndividualManagementAddView _view;
        //private IIndividualRepository _individualRepository;

        public IndividualManagementEditPresenter(IIndividualManagementAddView view)
        {
            _view = view;
            var _individualRepository = new IndividualRepository();
        }

        public void InitView()
        {
            PopulateDropDowns();
            FillPatientItems();
        }

        private void PopulateDropDowns()
        {
            //_view.IDNumberType = SrtsHelper.GetLookupTypesSelected(_view.LookupCache, LookupType.IDNumberType.ToString());
            //_view.IndividualType = SrtsHelper.GetLookupTypesSelected(_view.LookupCache, LookupType.IndividualType.ToString());
            _view.Sites = GetSiteList();
            var tmp = _view.mySession.Patient != null &&
                _view.mySession.Patient.Individual != null &&
                !String.IsNullOrEmpty(_view.mySession.Patient.Individual.SiteCodeID) ?
                _view.mySession.Patient.Individual.SiteCodeID : String.Empty;
            _view.SiteSelected = String.IsNullOrEmpty(tmp) ? _view.mySession.MyClinicCode : tmp;
            var tcr = new TheaterCodeRepository();
            _view.TheaterLocationCodes = tcr.GetActiveTheaterCodes();
        }

        private void FillPatientItems()
        {
            _view.Comments = _view.mySession.Patient.Individual.Comments;
            _view.DOB = _view.mySession.Patient.Individual.DateOfBirth;
            _view.EADStopDate = Helpers.CheckDateForGoodDate(_view.mySession.Patient.Individual.EADStopDate) ? _view.mySession.Patient.Individual.EADStopDate : null;
            _view.FirstName = _view.mySession.Patient.Individual.FirstName;
            _view.Gender = _view.mySession.Patient.Individual.Demographic.ToGenderKey();
            //_view.IsActive = _view.mySession.Patient.Individual.IsActive;
            _view.Lastname = _view.mySession.Patient.Individual.LastName;
            _view.MiddleName = _view.mySession.Patient.Individual.MiddleName;

            _view.SiteSelected = _view.mySession.Patient.Individual.SiteCodeID;
            _view.TheaterLocationCodeSelected = Helpers.DisplayLocationCode(_view.mySession.Patient.Individual.TheaterLocationCode);
            //_view.IndividualTypeSelected = _view.mySession.Patient.Individual.PersonalType;

            //_view.OrderPrioritySelected = SrtsExtender.ToOrderPriorityKey(_view.mySession.Patient.Individual.Demographic);
        }

        private List<SiteCodeEntity> GetSiteList()
        {
            var _repository = new SiteRepository.SiteCodeRepository();
            return _repository.GetAllSites();
        }

        public void UpdatePatientRecord()
        {
            PatientEntity _patient = new PatientEntity();
            _patient.Individual = new IndividualEntity();

            //_patient.Individual.Demographic = SrtsHelper.BuildProfile(_view.RankTypeSelected, _view.BOSTypeSelected, _view.StatusTypeSelected, _view.Gender, _view.OrderPrioritySelected);
            _patient.Individual.Demographic = Helpers.BuildProfile(_view.RankTypeSelected, _view.BOSTypeSelected, _view.StatusTypeSelected, _view.Gender, "N");
            _patient.Individual.EADStopDate = Helpers.SetDateOrDefault(_view.EADStopDate);
            _patient.Individual.DateOfBirth = Helpers.SetDateOrDefault(_view.DOB);
            _patient.Individual.FirstName = _view.FirstName;
            //_patient.Individual.IsActive = _view.IsActive;
            _patient.Individual.IsPOC = false;
            _patient.Individual.LastName = _view.Lastname;
            _patient.Individual.MiddleName = string.IsNullOrEmpty(_view.MiddleName) ? string.Empty : _view.MiddleName;
            _patient.Individual.SiteCodeID = _view.SiteSelected;
            _patient.Individual.Comments = _view.Comments;
            _patient.Individual.TheaterLocationCode = _view.TheaterLocationCodeSelected;
            //_patient.Individual.PersonalType = _view.IndividualTypeSelected;
            _patient.Individual.ModifiedBy = _view.mySession.MyUserID;
            _patient.Individual.ID = _view.mySession.Patient.Individual.ID;
            var _individualRepository = new IndividualRepository();
            _patient.Individual = _individualRepository.UpdateIndividual(_patient.Individual)[0];

            if (_patient.Individual == null)
            {
                _view.ErrorMessage = "Unknown error is preventing the addition of this patient!";
            }
            else
            {
                _view.mySession.Patient.Individual = _patient.Individual;
                _view.mySession.SelectedPatientID = _view.mySession.Patient.Individual.ID;
                //_view.NewPage = "IndividualDetails.aspx";
            }
        }
    }
}