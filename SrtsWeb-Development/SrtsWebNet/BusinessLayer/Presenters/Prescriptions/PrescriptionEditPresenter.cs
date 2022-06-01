using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers;
using SrtsWeb.BusinessLayer.Views.Prescriptions;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Data;
using System.Text;

namespace SrtsWeb.BusinessLayer.Presenters.Prescriptions
{
    public sealed class PrescriptionEditPresenter
    {
        private IPrescriptionEditView _view;
        private IPrescriptionRepository _repository;
        private IIndividualRepository _individualRepository;

        public PrescriptionEditPresenter(IPrescriptionEditView view)
        {
            _view = view;
            _repository = new PrescriptionRepository();
        }

        public void InitView()
        {
            FillDropDowns();
            LoadPatientData();
        }

        public void FillDropDowns()
        {
            _individualRepository = new IndividualRepository();
            DataTable dt = _individualRepository.GetIndividualBySiteCodeAndPersonalType(_view.mySession.MyClinicCode, "PROVIDER", _view.mySession.MyUserID.ToString());
            _view.DoctorsList = SrtsHelper.ProcessPersonnelDataTable(dt);
            _view.ODSphereValuesList = SrtsHelper.SphereValues();
            _view.ODCylinderValuesList = SrtsHelper.CylinderValues();
            _view.ODAxisValuesList = SrtsHelper.AxisValues();
            _view.ODHPrismValuesList = SrtsHelper.PrismValues();
            _view.ODVPrismValuesList = SrtsHelper.PrismValues();
            _view.ODAddValuesList = SrtsHelper.AddValues();
            _view.OSSphereValuesList = SrtsHelper.SphereValues();
            _view.OSCylinderValuesList = SrtsHelper.CylinderValues();
            _view.OSAxisValuesList = SrtsHelper.AxisValues();
            _view.OSHPrismValuesList = SrtsHelper.PrismValues();
            _view.OSVPrismValuesList = SrtsHelper.PrismValues();
            _view.OSAddValuesList = SrtsHelper.AddValues();
            _view.PDTotalValues = SrtsHelper.PDTotalValues();
            _view.PDMonoValues = SrtsHelper.PDMonoValues();
        }

        private void LoadPatientData()
        {
            PrescriptionEntity pe = SrtsHelper.ProcessPresciptionRows(_repository.GetActiveUnusedPrescriptionByPrescriptionID(_view.mySession.TempID, _view.mySession.MyUserID.ToString()).Rows[0]);

            try
            {
                _view.DoctorSelected = pe.IndividualID_Doctor;
                _view.ODHBase = pe.ODHBase;
                _view.ODVBase = pe.ODVBase;
                _view.OSHBase = pe.OSHBase;
                _view.OSVBase = pe.OSVBase;

                _view.ODAdd = SrtsHelper.AddValueToString(pe.ODAdd);
                _view.OSAdd = SrtsHelper.AddValueToString(pe.OSAdd);

                _view.ODAxis = SrtsHelper.FormatAxis(pe.EnteredODAxis);
                _view.OSAxis = SrtsHelper.FormatAxis(pe.EnteredOSAxis);

                _view.ODCylinder = SrtsHelper.AddPlusSign(pe.EnteredODCylinder);
                _view.OSCylinder = SrtsHelper.AddPlusSign(pe.EnteredOSCylinder);

                _view.ODHPrism = SrtsHelper.PrismToString(pe.ODHPrism);
                _view.OSHPrism = SrtsHelper.PrismToString(pe.OSHPrism);

                _view.ODSphere = SrtsHelper.AddPlusSign(pe.EnteredODSphere);
                _view.OSSphere = SrtsHelper.AddPlusSign(pe.EnteredOSSphere);

                _view.ODVPrism = SrtsHelper.PrismToString(pe.ODVPrism);
                _view.OSVPrism = SrtsHelper.PrismToString(pe.OSVPrism);

                _view.PDTotal = SrtsHelper.DecimalToString(pe.PDTotal);
                _view.PDOD = SrtsHelper.DecimalToString(pe.PDOD);
                _view.PDOS = SrtsHelper.DecimalToString(pe.PDOS);
                _view.PDTotalNear = SrtsHelper.DecimalToString(pe.PDTotalNear);
                _view.PDODNear = SrtsHelper.DecimalToString(pe.PDODNear);
                _view.PDOSNear = SrtsHelper.DecimalToString(pe.PDOSNear);

                _view.IsMonoCalculation = pe.IsMonoCalculation;

                _view.IsUsed = pe.IsUsed;
                _view.IsDeletable = pe.IsDeletable;
            }
            catch (Exception)
            {
                _view.msg = string.Format("Error: Unable to edit prescription #{0}", _view.mySession.TempID);
            }
        }

        public void SaveData()
        {
            _repository = new PrescriptionRepository();
            StringBuilder msg = new StringBuilder();
            PrescriptionEntity pe = new PrescriptionEntity();
            pe.IndividualID_Patient = _view.mySession.Patient.Individual.ID;
            pe.ModifiedBy = _view.mySession.MyUserID;

            pe.ODHPrism = SrtsHelper.PrismToDecimal(_view.ODHPrism);
            pe.OSHPrism = SrtsHelper.PrismToDecimal(_view.OSHPrism);
            pe.ODAdd = SrtsHelper.AddValueToDecimal(_view.ODAdd);
            pe.OSAdd = SrtsHelper.AddValueToDecimal(_view.OSAdd);

            pe.ODVPrism = SrtsHelper.PrismToDecimal(_view.ODVPrism);
            pe.OSVPrism = SrtsHelper.PrismToDecimal(_view.OSVPrism);

            pe.ODCylinder = SrtsHelper.CylinderToDecimal(_view.ODCylinder_calc);

            pe.EnteredODCylinder = SrtsHelper.CylinderToDecimal(_view.ODCylinder);

            pe.OSCylinder = SrtsHelper.CylinderToDecimal(_view.OSCylinder_calc);

            pe.EnteredOSCylinder = SrtsHelper.CylinderToDecimal(_view.OSCylinder);

            pe.ODSphere = SrtsHelper.SphereToDecimal(_view.ODSphere_calc);

            pe.EnteredODSphere = SrtsHelper.SphereToDecimal(_view.ODSphere);

            pe.OSSphere = SrtsHelper.SphereToDecimal(_view.OSSphere_calc);

            pe.EnteredOSSphere = SrtsHelper.SphereToDecimal(_view.OSSphere);

            pe.ExamID = string.IsNullOrEmpty(_view.mySession.SelectedExamID.ToString()) ? 0 : _view.mySession.SelectedExamID;

            if (_view.ODAxis_calc == "Unknown")
            {
                msg.Append("You must select Right(OD) Axis!");
                msg.Append("<br />");
            }

            pe.ODAxis = SrtsHelper.AxisToInt(SrtsHelper.CheckAxis(_view.ODCylinder_calc, _view.ODAxis_calc));

            pe.EnteredODAxis = SrtsHelper.AxisToInt(SrtsHelper.CheckAxis(_view.ODCylinder, _view.ODAxis));

            if (!SrtsHelper.CompareBaseToPrism(_view.ODHBase, _view.ODHPrism))
            {
                msg.Append("Right(OD) H-Base requires or has an improper entry!");
                msg.Append("<br />");
            }
            pe.ODHBase = _view.ODHBase;
            if (!SrtsHelper.CompareBaseToPrism(_view.ODVBase, _view.ODVPrism))
            {
                msg.Append("Right(OD) V-Base requires or has an improper entry!");
                msg.Append("<br />");
            }
            pe.ODVBase = _view.ODVBase;

            if (_view.OSAxis_calc == "Unknown")
            {
                msg.Append("You must select Left(OS) Axis!");
                msg.Append("<br />");
            }

            pe.OSAxis = SrtsHelper.AxisToInt(SrtsHelper.CheckAxis(_view.OSCylinder_calc, _view.OSAxis_calc));

            pe.EnteredOSAxis = SrtsHelper.AxisToInt(SrtsHelper.CheckAxis(_view.OSCylinder, _view.OSAxis));

            if (!SrtsHelper.CompareBaseToPrism(_view.OSHBase, _view.OSHPrism))
            {
                msg.Append("Left(OS) H-Base requires or has an improper entry!");
                msg.Append("<br />");
            }
            pe.OSHBase = _view.OSHBase;
            if (!SrtsHelper.CompareBaseToPrism(_view.OSVBase, _view.OSVPrism))
            {
                msg.Append("Left(OS) V-Base requires or has an improper entry!");
                msg.Append("<br />");
            }
            pe.OSVBase = _view.OSVBase;
            pe.IsMonoCalculation = _view.IsMonoCalculation;

            var good = true;
            if (pe.IsMonoCalculation)
            {
                #region validation

                if (_view.PDOD.Equals("X"))
                {
                    pe.PDOD = 0;
                    msg.Append("OD PD Dist is required!<br/>");
                    good = false;
                }

                if (_view.PDODNear.Equals("X"))
                {
                    pe.PDODNear = 0;
                    msg.Append("OS PD Dist is required!<br/>");
                    good = false;
                }

                if (_view.PDOS.Equals("X"))
                {
                    pe.PDOS = 0;
                    msg.Append("OD PD Near is required!<br/>");
                    good = false;
                }

                if (_view.PDOSNear.Equals("X"))
                {
                    pe.PDOSNear = 0;
                    msg.Append("OS PD Near is required!<br/>");
                    good = false;
                }

                #endregion validation

                if (good)
                {
                    pe.PDOD = decimal.Parse(_view.PDOD);
                    pe.PDODNear = decimal.Parse(_view.PDODNear);
                    pe.PDOS = decimal.Parse(_view.PDOS);
                    pe.PDOSNear = decimal.Parse(_view.PDOSNear);

                    if ((pe.PDOD + pe.PDOS) % 1 != 0)
                    {
                        pe.PDTotal = (pe.PDOD + pe.PDOS) + 0.5M;
                    }
                    else
                    {
                        pe.PDTotal = (pe.PDOD + pe.PDOS);
                    }
                    if ((pe.PDODNear + pe.PDOSNear) % 1 != 0)
                    {
                        pe.PDTotalNear = (pe.PDODNear + pe.PDOSNear) + 0.5M;
                    }
                    else
                    {
                        pe.PDTotalNear = (pe.PDODNear + pe.PDOSNear);
                    }
                }
            }
            else
            {
                if (_view.PDTotal.Equals("X"))
                {
                    pe.PDTotal = 0;
                    msg.Append("PD Dist Total is required!<br/>");
                    good = false;
                }

                if (_view.PDTotalNear.Equals("X"))
                {
                    pe.PDTotalNear = 0;
                    msg.Append("PD Dist Near is required!<br/>");
                    good = false;
                }

                if (good)
                {
                    pe.PDTotal = decimal.Parse(_view.PDTotal);
                    pe.PDTotalNear = decimal.Parse(_view.PDTotalNear);
                    pe.PDOD = decimal.Parse(_view.PDTotal) / 2;
                    pe.PDODNear = pe.PDOD - 1.5M;
                    pe.PDOS = pe.PDOD;
                    pe.PDOSNear = pe.PDODNear;
                }
            }

            pe.IndividualID_Doctor = Convert.ToInt32(_view.DoctorSelected);
            if (_view.CommitSave)
            {
                msg.Clear();
            }
            if (!string.IsNullOrEmpty(msg.ToString()) && !_view.CommitSave)
            {
                _view.msg = msg.ToString();
                return;
            }
            else
            {
                _view.CommitSave = true;
            }
            if (_view.CommitSave)
            {
                if (_view.mySession.TempID == 0)
                {
                    _repository.InsertPrescription(pe);
                }
                else
                {
                    pe.IsActive = _view.IsActive;
                    pe.ID = _view.mySession.TempID;
                    _repository.UpdatePrescription(pe);
                }
            }
        }
    }
}