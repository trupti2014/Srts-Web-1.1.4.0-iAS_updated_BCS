using System.Text.RegularExpressions;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Patients;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrtsWeb.Presenters.Patients
{
    public sealed class PatientManagementEditPresenter
    {
        private IPatientManagementAddView _view;

        public PatientManagementEditPresenter(IPatientManagementAddView view)
        {
            _view = view;
        }

        public void InitView()
        {
            PopulateDropDowns();
            FillPatientItems();
        }

        private void PopulateDropDowns()
        {
            _view.IDNumberType = _view.LookupCache.GetByType(LookupType.IDNumberType.ToString());
            _view.IndividualType = _view.LookupCache.GetByType(LookupType.IndividualType.ToString());
            _view.Sites = GetSiteList();

            var tmp = _view.mySession.Patient != null &&
                _view.mySession.Patient.Individual != null &&
                !String.IsNullOrEmpty(_view.mySession.Patient.Individual.SiteCodeID) ?
                _view.mySession.Patient.Individual.SiteCodeID : String.Empty;

            _view.SiteSelected = String.IsNullOrEmpty(tmp) ? _view.mySession.MyClinicCode : tmp;
            //_view.IndividualTypeSelected = "PATIENT";
            var tcr = new TheaterCodeRepository();
            _view.TheaterLocationCodes = tcr.GetActiveTheaterCodes();
        }

        private void FillPatientItems()
        {
            _view.BOSTypeSelected = _view.mySession.Patient.Individual.Demographic.ToBOSKey();
            _view.Comments = _view.mySession.Patient.Individual.Comments;
            _view.DOB = _view.mySession.Patient.Individual.DateOfBirth;
            _view.EADStopDate = Helpers.CheckDateForGoodDate(_view.mySession.Patient.Individual.EADStopDate) ? _view.mySession.Patient.Individual.EADStopDate : null;
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

        private List<SiteCodeEntity> GetSiteList()
        {
            var _repository = new SiteRepository.SiteCodeRepository();
            var rls = System.Web.Security.Roles.GetRolesForUser().ToList();
            if (rls.Any(x => x.ToLower().Contains("admin") || x.ToLower().Contains("mgmt")))
            {
                return _repository.GetAllSites();
            }
            else
            {
                var sc = _view.mySession.Patient.Individual.SiteCodeID.ToUpper();
                var siteType = sc.StartsWith("ADM") ? "ADMIN" : String.IsNullOrEmpty(Regex.Match(sc, "[A-Z]").Value) ? "CLINIC" : "LAB";

                return _repository.GetSitesByType(siteType);
            }
        }

        public void UpdatePatientRecord(Boolean isPatient)
        {
            PatientEntity _patient = new PatientEntity();
            _patient.Individual = new IndividualEntity();

            if (_view.EADStopDate != null)
            {
                var d = _view.EADStopDate.Value;
                if (d.Date < DateTime.Today.Date || d.Date > DateTime.Today.AddYears(2))
                {
                    _view.ErrorMessage = "The EAD Expiration Date is not within range";
                    return;
                }
            }

            //_patient.Individual.Demographic = Helpers.BuildProfile(_view.RankTypeSelected, _view.BOSTypeSelected, _view.StatusTypeSelected, _view.Gender, _view.OrderPrioritySelected);
            _patient.Individual.Demographic = Helpers.BuildProfile(_view.RankTypeSelected, _view.BOSTypeSelected, _view.StatusTypeSelected, _view.Gender, "N");
            _patient.Individual.EADStopDate = Helpers.SetDateOrDefault(_view.EADStopDate);
            _patient.Individual.DateOfBirth = Helpers.SetDateOrDefault(_view.DOB);
            _patient.Individual.FirstName = _view.FirstName;
            //_patient.Individual.IsActive = _view.IsActive;
            _patient.Individual.IsActive = true;
            _patient.Individual.IsPOC = false;
            _patient.Individual.LastName = _view.Lastname;
            _patient.Individual.MiddleName = string.IsNullOrEmpty(_view.MiddleName) ? string.Empty : _view.MiddleName;
            _patient.Individual.SiteCodeID = _view.SiteSelected;
            _patient.Individual.Comments = _view.Comments;
            _patient.Individual.TheaterLocationCode = _view.TheaterLocationCodeSelected;
            //_patient.Individual.PersonalType = "PATIENT";
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
                _view.NewPage = String.Format("PersonDetails.aspx{0}{1}", isPatient ? "?id=" + _view.mySession.SelectedPatientID : String.Empty, isPatient ? "&isP=true" : String.Empty);
            }
        }
    }
}