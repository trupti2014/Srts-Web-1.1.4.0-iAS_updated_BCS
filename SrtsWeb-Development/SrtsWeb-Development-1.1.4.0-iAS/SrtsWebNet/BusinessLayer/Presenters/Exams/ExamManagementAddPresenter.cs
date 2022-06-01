using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers;
using SrtsWeb.BusinessLayer.Views.Exams;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Presenters.Exams
{
    public sealed class ExamManagementAddPresenter
    {
        private IExamManagementAddView _view;
        private IIndividualRepository _iRepository;
        private IExamRepository _eRepository;

        public ExamManagementAddPresenter(IExamManagementAddView view)
        {
            _view = view;
            _iRepository = new IndividualRepository();
            _eRepository = new ExamRepository();
        }

        public void InitView()
        {
            FillDropDowns();
            _view.ExamDate = DateTime.Now;
        }

        private void FillDropDowns()
        {
            _view.TechData = GetTechnicianBySiteCode(_view.mySession.MyClinicCode);
            _view.DoctorsData = GetDoctorBySiteCode(_view.mySession.MyClinicCode);
            _view.AcuityValues = SrtsHelper.GetLookupTypesSelected(_view.LookupCache, LookupType.AcuityType.ToString());
        }

        public Boolean SaveData()
        {
            SiteCodeRepository scr = new SiteCodeRepository();
            DataTable dt = scr.GetSiteAddressBySiteID(_view.mySession.MySite.SiteCode);

            ExamEntity ee = new ExamEntity();
            ee.ODCorrectedAcuity = _view.ODCorrected;
            ee.ODOSCorrectedAcuity = _view.ODOSCorrected;
            ee.ODOSUncorrectedAcuity = _view.ODOSUncorrected;
            ee.ODUncorrectedAcuity = _view.ODUncorrected;
            ee.OSCorrectedAcuity = _view.OSCorrected;
            ee.OSUncorrectedAcuity = _view.OSUncorrected;
            ee.Comments = _view.ExamComments;
            ee.ExamDate = _view.ExamDate;
            if (string.IsNullOrEmpty(_view.DoctorID.ToString()))
            {
                ee.IndividualID_Examiner = _view.TechID;
            }
            else
            {
                ee.IndividualID_Examiner = _view.DoctorID;
            }
            ee.IndividualID_Patient = _view.mySession.Patient.Individual.ID;
            ee.ModifiedBy = string.Format("{0}-{1}", _view.mySession.MyUserID, _view.mySession.MyClinicCode);

            if (_eRepository.InsertExam(ee).Rows.Count > 0)
                return true;
            else
                return false;
        }

        public List<PersonnelEntity> GetTechnicianBySiteCode(string siteCode)
        {
            DataTable dt = _iRepository.GetIndividualBySiteCodeAndPersonalType(siteCode, "TECHNICIAN", _view.mySession.MyUserID.ToString());
            return SrtsHelper.ProcessPersonnelDataTable(dt);
        }

        public List<PersonnelEntity> GetDoctorBySiteCode(string siteCode)
        {
            DataTable dt = _iRepository.GetIndividualBySiteCodeAndPersonalType(siteCode, "PROVIDER", _view.mySession.MyUserID.ToString());
            return SrtsHelper.ProcessPersonnelDataTable(dt);
        }
    }
}