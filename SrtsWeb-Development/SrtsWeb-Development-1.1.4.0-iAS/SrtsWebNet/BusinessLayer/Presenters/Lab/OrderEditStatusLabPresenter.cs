using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers;
using SrtsWeb.BusinessLayer.Views.Lab;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace SrtsWeb.BusinessLayer.Presenters.Lab
{
    public sealed class OrderEditStatusLabPresenter
    {
        private IOrderEditStatusLabView _view;
        private IOrderRepository _orderRepository;
        private IPrescriptionRepository _prescriptionRepository;
        private ISiteCodeRepository _siteRepository;
        private IIndividualRepository _individualRepository;
        private IOrderStateRepository _stateRepository;

        public OrderEditStatusLabPresenter(IOrderEditStatusLabView view)
        {
            _view = view;
        }

        public void InitView()
        {
            _orderRepository = new OrderRepository();
            if (!string.IsNullOrEmpty(_view.mySession.tempOrderID))
            {
                foreach (DataRow dr in _orderRepository.GetOrderByOrderNumberNonGEyes(_view.mySession.tempOrderID, _view.mySession.MyUserID).Rows)
                {
                    _view.mySession.SelectedOrder = SrtsHelper.ProcessOrderRow(dr);
                }
                _view.mySession.TempID = _view.mySession.SelectedOrder.PrescriptionID;
            }
            if (_view.mySession.Patient.Individual == null)
            {
                PatientEntity pe = new PatientEntity();
                DataSet ds = new DataSet();
                _individualRepository = new IndividualRepository();
                ds = _individualRepository.GetAllPatientInfoByIndividualID(_view.mySession.SelectedOrder.IndividualID_Patient, true, _view.mySession.MyUserID, _view.mySession.MySite.SiteCode);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    pe.Individual = SrtsHelper.ProcessIndividualRow(ds.Tables[0].Rows[0]);
                    pe.IDNumbers = SrtsHelper.ProcessIdentificationNumberTable(ds.Tables[1]);
                    pe.EMailAddresses = SrtsHelper.ProcessEMailAddressTable(ds.Tables[2]);
                    pe.PhoneNumbers = SrtsHelper.ProcessPhoneTable(ds.Tables[3]);
                    pe.Addresses = SrtsHelper.ProcessAddressTable(ds.Tables[4]);
                    pe.Orders = SrtsHelper.ProcessOrderTable(ds.Tables[5]);
                    pe.Exams = SrtsHelper.ProcessExamTable(ds.Tables[6]);
                    pe.Prescriptions = SrtsHelper.ProcessPresciptionTable(ds.Tables[7]);
                }
                _view.mySession.Patient = pe;
                _view.mySession.SelectedPatientID = _view.mySession.Patient.Individual.ID;
            }
            FillDropDowns();
            FillAddress();
            if (!string.IsNullOrEmpty(_view.mySession.TempID.ToString()))
            {
                _prescriptionRepository = new PrescriptionRepository();
                FillPrescriptions(_prescriptionRepository.GetPrescriptionByPrescriptionID(_view.mySession.TempID, _view.mySession.MyUserID.ToString()));
            }
            FillPatientOrderData();

            var foc = String.Empty;
            var focOrder = String.Empty;
            var reqJust = false;

            foc = _orderRepository.GetLastFOCDate(_view.mySession.Patient.Individual.ID, out focOrder, out reqJust).ToMilDateString();

            _view.FOCDate = string.IsNullOrEmpty(foc) ? "None Found" : foc;
            _view.RequiresJustification = reqJust;

            //_view.FOCDate = string.IsNullOrEmpty(_orderRepository.GetLastFOCDate(_view.mySession.Patient.Individual.ID).ToMilDateString()) ? "None Found" : _orderRepository.GetLastFOCDate(_view.mySession.Patient.Individual.ID).ToMilDateString();

            _stateRepository = new OrderStateRepository();
            OrderStateEntity ose = SrtsHelper.ProcessOrderStateRows(_stateRepository.GetOrderStateByOrderNumber(_view.mySession.SelectedOrder.OrderNumber).Rows[0]);
            _view.JustificationInfo = ose.StatusComment;
        }

        private void FillPatientOrderData()
        {
            _view.Pairs = _view.mySession.SelectedOrder.Pairs;
            _view.Cases = _view.mySession.SelectedOrder.NumberOfCases;
            _view.Frame = _view.mySession.SelectedOrder.FrameCode;
            _view.Eye = _view.mySession.SelectedOrder.FrameEyeSize;
            _view.Address1 = _view.mySession.SelectedOrder.ShipAddress1;
            _view.Address2 = _view.mySession.SelectedOrder.ShipAddress2;
            _view.AddressType = _view.mySession.SelectedOrder.ShipAddressType;
            _view.City = _view.mySession.SelectedOrder.ShipCity;
            _view.AddressState = _view.mySession.SelectedOrder.ShipState;
            _view.ZipCode = _view.mySession.SelectedOrder.ShipZipCode.ToZipCodeDisplay();

            _view.Bridge = _view.mySession.SelectedOrder.FrameBridgeSize;
            _view.Color = _view.mySession.SelectedOrder.FrameColor;

            _view.LabSelected = _view.mySession.SelectedOrder.LabSiteCode;
            _view.Lens = _view.mySession.SelectedOrder.LensType;
            _view.Material = _view.mySession.SelectedOrder.LensMaterial;

            _view.ODSegHeight = _view.mySession.SelectedOrder.ODSegHeight;
            _view.OSSegHeight = _view.mySession.SelectedOrder.OSSegHeight;

            _view.PrioritySelected = _view.mySession.SelectedOrder.Demographic.ToOrderPriorityKey();
            _view.Tint = _view.mySession.SelectedOrder.Tint;
            _view.Temple = _view.mySession.SelectedOrder.FrameTempleType;
            _view.IsMultiVision = _view.mySession.SelectedOrder.IsMultivision;
            _view.Location = string.IsNullOrEmpty(_view.mySession.Patient.Individual.TheaterLocationCode) ? "N/A" : _view.mySession.Patient.Individual.TheaterLocationCode;

            var tmp = _view.mySession.Patient.Individual.Demographic.ToRankKey();
            int tmpInt;

            if (!Int32.TryParse(tmp.Substring(1, 2), out tmpInt)) return;

            if (tmp.Substring(0, 1).Equals("O") && tmpInt > 6)
            {
                _view.PrioritySelected = "V";
            }
        }

        private void FillPrescriptions(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                _view.ODAdd = SrtsHelper.DecimalToString(dr.GetNullableDecimalVal("ODAdd"));
                _view.OSAdd = SrtsHelper.DecimalToString(dr.GetNullableDecimalVal("OSAdd"));
                _view.ODSphere = SrtsHelper.SphereToString(dr.GetDecimalVal("ODSphere"));
                _view.OSSphere = SrtsHelper.SphereToString(dr.GetDecimalVal("OSSphere"));
                _view.ODCylinder = SrtsHelper.CylinderToString(dr.GetDecimalVal("ODCylinder"));
                _view.OSCylinder = SrtsHelper.CylinderToString(dr.GetDecimalVal("OSCylinder"));
                _view.ODAxis = dr.GetStringVal("ODAxis");
                _view.OSAxis = dr.GetStringVal("OSAxis");
                _view.ODHPrism = SrtsHelper.DecimalToString(dr.GetDecimalVal("ODHPrism"));
                _view.ODVPrism = SrtsHelper.DecimalToString(dr.GetDecimalVal("ODVPrism"));
                _view.OSHPrism = SrtsHelper.DecimalToString(dr.GetDecimalVal("OSHPrism"));
                _view.OSVPrism = SrtsHelper.DecimalToString(dr.GetDecimalVal("OSVPrism"));
                _view.ODHBase = dr.GetStringVal("ODHBase");
                _view.ODVBase = dr.GetStringVal("ODVBase");
                _view.OSHBase = dr.GetStringVal("OSHBase");
                _view.OSVBase = dr.GetStringVal("OSVBase");
                _view.PDTotal = SrtsHelper.DecimalToString(dr.GetDecimalVal("PDDistant"));
                _view.PDTotalNear = SrtsHelper.DecimalToString(dr.GetDecimalVal("PDNear"));
                _view.PDOD = SrtsHelper.DecimalToString(dr.GetDecimalVal("ODPDDistant"));
                _view.PDODNear = SrtsHelper.DecimalToString(dr.GetDecimalVal("ODPDNear"));
                _view.PDOS = SrtsHelper.DecimalToString(dr.GetDecimalVal("OSPDDistant"));
                _view.PDOSNear = SrtsHelper.DecimalToString(dr.GetDecimalVal("OSPDNear"));
            }
        }

        //public bool CheckFOCDate()
        //{
        //    if (!string.IsNullOrEmpty(_view.FOCDate))
        //    {
        //        DateTime checkDate;
        //        DateTime.TryParse(_view.FOCDate, out checkDate);
        //        if (DateTime.Compare(DateTime.Now, checkDate.AddDays(365)) < 0)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        private void FillDropDowns()
        {
            _view.PriorityData = SrtsHelper.GetLookupTypesSelected(_view.LookupCache, LookupType.OrderPriorityType.ToString());
            _siteRepository = new SiteCodeRepository();
            var l = SrtsHelper.ProcessSiteTable(_siteRepository.GetSitesByType("LAB"));

            if (_view.mySession.SelectedOrder.IsMultivision)
                _view.LabData = l.Where(x => x.IsMultivision == true).ToList();
            else
                _view.LabData = l;

            _individualRepository = new IndividualRepository();
        }

        public void FillAddress()
        {
            _view.Address1 = _view.mySession.SelectedOrder.ShipAddress1;
            _view.Address2 = string.IsNullOrEmpty(_view.mySession.SelectedOrder.ShipAddress2) ? string.Empty : _view.mySession.SelectedOrder.ShipAddress2;
            _view.City = _view.mySession.SelectedOrder.ShipCity;
            _view.ZipCode = _view.mySession.SelectedOrder.ShipZipCode.ToZipCodeDisplay();
            _view.AddressState = _view.mySession.SelectedOrder.ShipState;
            _view.AddressType = _view.mySession.SelectedOrder.ShipAddressType;
            _view.Country = (from t in SrtsHelper.ProcessLookupTable(_view.LookupCache)
                             where t.Code.Equals("CountryList") && t.Value.Equals(_view.mySession.SelectedOrder.ShipCountry)
                             select t.Text).FirstOrDefault();
        }

        public void SaveData()
        {
            IOrderStateRepository osr = new OrderStateRepository();
            OrderStateEntity ose = new OrderStateEntity();

            ose.DateLastModified = DateTime.Now;
            ose.ModifiedBy = _view.mySession.MyUserID;
            ose.IsActive = true;
            ose.OrderNumber = _view.mySession.SelectedOrder.OrderNumber;
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("Site Taking Action: {0}<br />", _view.mySession.MySite.SiteCode));
            sb.Append(string.Format("Justification: {0}<br />", _view.OHComment));
            ose.StatusComment = sb.ToString();
            switch (_view.StatusSelected)
            {
                case "R":

                    ose.OrderStatusTypeID = 3;
                    ose.LabCode = _view.mySession.MySite.SiteCode;
                    break;

                case "C":

                    ose.OrderStatusTypeID = 5;
                    ose.LabCode = _view.mySession.MySite.SiteCode;
                    break;

                case "D":

                    ose.OrderStatusTypeID = 4;
                    ose.LabCode = _view.LabSelected;
                    break;

                default:
                    break;
            }
            osr.InsertPatientOrderState(ose);
        }
    }
}