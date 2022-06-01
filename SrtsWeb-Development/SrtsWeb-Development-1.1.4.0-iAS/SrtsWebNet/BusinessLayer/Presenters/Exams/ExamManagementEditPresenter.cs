using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers;
using SrtsWeb.BusinessLayer.Views.Exams;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Presenters.Exams
{
    public sealed class ExamManagementEditPresenter
    {
        private IExamManagementEditView _view;
        private IIndividualRepository _iRepository;
        private IExamRepository _eRepository;

        public ExamManagementEditPresenter(IExamManagementEditView view)
        {
            _view = view;
            _iRepository = new IndividualRepository();
            _eRepository = new ExamRepository();
        }

        public void InitView()
        {
            FillDropDowns();
            FillExamData();
        }

        private void FillDropDowns()
        {
            _view.TechData = GetTechnicianBySiteCode(_view.mySession.MyClinicCode);
            _view.DoctorsData = GetDoctorBySiteCode(_view.mySession.MyClinicCode);
            _view.AcuityValues = SrtsHelper.GetLookupTypesSelected(_view.LookupCache, LookupType.AcuityType.ToString());
        }

        public void SaveData()
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

            ee.IndividualID_Examiner = _view.DoctorID;
            ee.IndividualID_Patient = _view.mySession.Patient.Individual.ID;
            ee.ModifiedBy = string.Format("{0}-{1}", _view.mySession.MyUserID, _view.mySession.MyClinicCode);
            ee.ID = _view.mySession.TempID;
            _view.mySession.Patient.Exams = SrtsHelper.ProcessExamTable(_eRepository.UpdateExam(ee));
        }

        private void FillExamData()
        {
            DataTable dt = _eRepository.GetExamByExamID(_view.mySession.TempID, _view.mySession.MyUserID.ToString());
            if (dt.Rows.Count >= 0)
            {
                ExamEntity ee = SrtsHelper.ProcessExamRows(dt.Rows[0]);
                _view.DoctorID = ee.IndividualID_Examiner;
                _view.ExamComments = ee.Comments;
                _view.ExamDate = ee.ExamDate;
                _view.ODCorrected = ee.ODCorrectedAcuity;
                _view.ODOSCorrected = ee.ODOSCorrectedAcuity;
                _view.ODOSUncorrected = ee.ODOSUncorrectedAcuity;
                _view.ODUncorrected = ee.ODUncorrectedAcuity;
                _view.OSCorrected = ee.OSCorrectedAcuity;
                _view.OSUncorrected = ee.OSUncorrectedAcuity;
                _view.TechID = ee.IndividualID_Examiner;
                _view.ExamComments = ee.Comments;
            }
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