using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers;
using SrtsWeb.BusinessLayer.Views.Orders;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace SrtsWeb.BusinessLayer.Presenters.Orders
{
    public sealed class OrderManagementEditPresenter
    {
        private IOrderManagementEditView _view;
        private IOrderRepository _orderRepository;
        private IFrameRepository _frameRepository;
        private IPrescriptionRepository _prescriptionRepository;

        //private ISiteCodeRepository _siteRepository;
        private IIndividualRepository _individualRepository;

        private OrderEntity _saveOE;

        public OrderManagementEditPresenter(IOrderManagementEditView view)
        {
            _view = view;
        }

        public void InitView()
        {
            _orderRepository = new OrderRepository();
            if (!string.IsNullOrEmpty(_view.mySession.tempOrderID))
            {
                foreach (DataRow dr in _orderRepository.GetOrderByOrderNumberNonGEyes(_view.mySession.tempOrderID, _view.mySession.MyUserID.ToString()).Rows)
                {
                    _view.mySession.SelectedOrder = SrtsHelper.ProcessOrderRow(dr);
                }
                _view.mySession.TempID = _view.mySession.SelectedOrder.PrescriptionID;
            }

            PatientEntity pe = new PatientEntity();
            DataSet ds = new DataSet();
            _individualRepository = new IndividualRepository();
            ds = _individualRepository.GetAllPatientInfoByIndividualID(_view.mySession.SelectedOrder.IndividualID_Patient, true, _view.mySession.MyUserID.ToString(), _view.mySession.MySite.SiteCode.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                pe.Individual = SrtsHelper.ProcessIndividualRow(ds.Tables[0].Rows[0]);
                pe.IDNumbers = SrtsHelper.ProcessIdentificationNumberTable(ds.Tables[1]);
                pe.EMailAddresses = SrtsHelper.ProcessEMailAddressTable(ds.Tables[2]);
                pe.PhoneNumbers = SrtsHelper.ProcessPhoneTable(ds.Tables[3]);
                pe.Addresses = SrtsHelper.ProcessAddressTable(ds.Tables[4]);
                //pe.Orders = SrtsHelper.ProcessOrderTable(ds.Tables[5]);
                //pe.Exams = SrtsHelper.ProcessExamTable(ds.Tables[6]);
                //pe.Prescriptions = SrtsHelper.ProcessPresciptionTable(ds.Tables[7]);

                _view.mySession.Patient = pe;
                _view.mySession.SelectedPatientID = _view.mySession.Patient.Individual.ID;
            }
            else
            {
                _view.Message = "Record could not be found";
                return;
            }

            if (!string.IsNullOrEmpty(_view.mySession.TempID.ToString()))
            {
                _prescriptionRepository = new PrescriptionRepository();
                FillPrescriptions(_prescriptionRepository.GetPrescriptionByPrescriptionID(_view.mySession.TempID, _view.mySession.MyUserID.ToString()));
            }

            FillDropDowns();

            FillFrameData(_view.mySession.SelectedOrder.Demographic.ToOrderPriorityValue());
            if (!String.IsNullOrEmpty(_view.FrameSelected) && _view.FrameSelected != "X")
                FillItemsData(_view.FrameSelected,_view.mySession.SelectedOrder.Demographic);

            IOrderStateRepository osr = new OrderStateRepository();
            using (DataTable ost = osr.GetPatientOrderStatusByOrderNumber(_view.mySession.SelectedOrder.OrderNumber))
            {
                if (ost != null && ost.Rows.Count > 0)
                {
                    _view.OrderStatus = ost.Rows[0]["OrderStatusDescription"].ToString();
                    _view.OrderStatusID = (Int32)ost.Rows[0]["OrderStatusID"];
                    _view.LabComment = string.Format("Lab {0} rejected this order with the following justification:<br/>{1}", ost.Rows[0]["LabSiteCode"].ToString(), string.IsNullOrEmpty(ost.Rows[0]["StatusComment"].ToString()) ? "None provided" : ost.Rows[0]["StatusComment"].ToString());
                }
            }

            ProcessOther();

            //var foc = String.Empty;
            var focOrder = String.Empty;
            var reqJust = false;

            _view.FOCDate = _orderRepository.GetLastFOCDate(_view.mySession.Patient.Individual.ID, out focOrder, out reqJust);

            //_view.FOCDate = string.IsNullOrEmpty(foc) ? "None Found" : foc;
            _view.LastFocOrderNumber = focOrder;
            _view.RequiresJustification = reqJust;

            SetEligibilityVisibility();

            SetLab();
        }

        private void ProcessOther()
        {
            FillPatientOrderData();
            FillOrderStatusHistory();
        }

        public void SetLab()
        {
            if (String.IsNullOrEmpty(_view.PrioritySelected) || _view.PrioritySelected.Equals("X")) { _view.LabData = null; return; }
            if (String.IsNullOrEmpty(_view.FrameSelected) || _view.FrameSelected.Equals("X")) { _view.LabData = null; return; }

            var kItem = String.Empty;
            var vItem = String.Empty;
            var d = new Dictionary<String, String>();

            // If the selected frame is part of the CustFramToLabColl variable then set the LabSelected property to the correct lab.
            if (OrderEntity.CustFrameToLabColl.ContainsKey(_view.FrameSelected))
            {
                var sc = new List<String>();
                OrderEntity.CustFrameToLabColl.TryGetValues(_view.FrameSelected, out sc);

                if (_view.FrameSelected.ToLower().StartsWith("5am"))
                {
                    var s = sc.FirstOrDefault(x => x == _view.mySession.MySite.MultiPrimary);

                    if (String.IsNullOrEmpty(s))
                    {
                        vItem = String.Format("1{0}", "MNOST1");
                        kItem = string.Format("{0} - {1}", "MultiVision", "MNOST1");
                        s = "MNOST1";
                    }
                    else
                    {
                        vItem = String.Format("1{0}", s);
                        kItem = string.Format("{0} - {1}", "MultiVision", s);
                    }

                    d.Add(kItem, vItem);
                    _view.LabData = d;
                    _view.LabSelected = vItem;// String.Format("1{0}", s);
                }
                else
                {
                    // Replace what is in the LabData property then set the lab selected to the value from custframetolabcoll
                    vItem = String.Format("1{0}", sc[0]);
                    kItem = String.Format("{0} - {1}", sc[0].Substring(0, 1).ToLower().Equals("m") ? "MultiVision" : "SingleVision", sc[0]);

                    d.Add(kItem, vItem);

                    _view.LabData = d;
                    _view.LabSelected = String.Format("1{0}", sc[0]);
                }
                // No matter what lens is selected if the frame is required to go to a special lab then exit method before processing lens ddls.
                return;
            }
            else if (!_view.mySession.SelectedOrder.ClinicSiteCode.Equals(_view.mySession.MySite.SiteCode))
            {
                ISiteCodeRepository r = new SiteCodeRepository();
                var s = SrtsHelper.ProcessSiteTable(r.GetSiteBySiteID(_view.mySession.SelectedOrder.ClinicSiteCode));

                vItem = String.Format("0{0}", s[0].SinglePrimary);
                kItem = string.Format("{0} - {1}", "SingleVision", string.IsNullOrEmpty(s[0].SinglePrimary) ? "MNOST1" : s[0].SinglePrimary);
                d.Add(kItem, vItem);

                vItem = String.Format("1{0}", s[0].MultiPrimary);

                kItem = string.Format("{0} - {1}", "MultiVision", string.IsNullOrEmpty(s[0].MultiPrimary) ? "MNOST1" : s[0].MultiPrimary);

                d.Add(kItem, vItem);
                _view.LabData = d;

                _view.LabSelected = String.Format("{0}{1}", s[0].SinglePrimary.Equals(_view.mySession.SelectedOrder.LabSiteCode) ? "0" : "1", _view.mySession.SelectedOrder.LabSiteCode);
            }
            else if (_view.LabData == null || _view.LabData.Count.Equals(1))// reload the ddl
            {
                FillLabData();
            }

            if (String.IsNullOrEmpty(_view.LensSelected)) return;
            if (_view.LensSelected.Equals("X")) return;

            var l = _view.LensSelected.Substring(0, 2).ToLower().Equals("sv") ? "Single" : "Multi";
            if (l.Equals("Single")) return;
            _view.LabSelected = _view.LabData.Where(x => x.Key.StartsWith(l)).FirstOrDefault().Value;
        }

        private void FillPatientOrderData()
        {
            //var p = _view.mySession.SelectedOrder.Demographic.ToOrderPriorityKey();
            _view.PrioritySelected = _view.mySession.SelectedOrder.Demographic.ToOrderPriorityKey();
            if (!_view.PrioritySelected.Equals("X"))
            {
                if (!_view.FrameSelected.Equals("X"))
                {
                    _view.FrameSelected = _view.mySession.SelectedOrder.FrameCode;
                    _view.EyeSelected = _view.mySession.SelectedOrder.FrameEyeSize;
                    _view.BridgeSelected = _view.mySession.SelectedOrder.FrameBridgeSize;
                    _view.ColorSelected = _view.mySession.SelectedOrder.FrameColor;
                    _view.LensSelected = _view.mySession.SelectedOrder.LensType;
                    _view.TempleSelected = _view.mySession.SelectedOrder.FrameTempleType;
                    _view.MaterialSelected = _view.mySession.SelectedOrder.LensMaterial;
                    _view.TintSelected = _view.mySession.SelectedOrder.Tint;
                }
            }
            _view.TechSelected = _view.mySession.SelectedOrder.IndividualID_Tech;
            _view.Pairs = _view.mySession.SelectedOrder.Pairs;
            _view.Cases = _view.mySession.SelectedOrder.NumberOfCases;
            _view.IsShipToPatient = _view.mySession.SelectedOrder.ShipToPatient;

            if (!_view.mySession.SelectedOrder.UserComment1.Contains("FOC:"))
            {
                _view.Comment1 = _view.mySession.SelectedOrder.UserComment1;
            }
            else
            {
                int focIndex = _view.mySession.SelectedOrder.UserComment1.IndexOf("FOC:");
                _view.FocJust = _view.mySession.SelectedOrder.UserComment1.Substring(focIndex + 5);
                _view.Comment1 = _view.mySession.SelectedOrder.UserComment1.Remove(focIndex);
            }

            if (!_view.mySession.SelectedOrder.UserComment2.Contains("MAT:"))
            {
                _view.Comment2 = _view.mySession.SelectedOrder.UserComment2;
            }
            else
            {
                int matIndex = _view.mySession.SelectedOrder.UserComment2.IndexOf("MAT:");
                _view.MaterialJust = _view.mySession.SelectedOrder.UserComment2.Substring(matIndex + 5);
                _view.Comment2 = _view.mySession.SelectedOrder.UserComment2.Remove(matIndex);
            }

            _view.LabSelected = _view.mySession.SelectedOrder.LabSiteCode.Substring(0, 1).Equals("M") ? String.Format("1{0}", _view.mySession.SelectedOrder.LabSiteCode) : String.Format("0{0}", _view.mySession.SelectedOrder.LabSiteCode);

            _view.ODSegHeight = _view.mySession.SelectedOrder.ODSegHeight;
            _view.OSSegHeight = _view.mySession.SelectedOrder.OSSegHeight;

            //_view.PrioritySelected = p;
            _view.IsMultiVision = _view.mySession.SelectedOrder.IsMultivision;

            if (_view.PrioritySelected == "C" || _view.PrioritySelected == "P")
            {
                _view.EligibilityVisibility = true;
                _view.DoctorSelected = _view.mySession.SelectedOrder.VerifiedBy;
            }
            else
            {
                _view.EligibilityVisibility = false;
            }
            _view.CurrentFocDate = _view.mySession.SelectedOrder.FocDate;
            _view.LinkedId = _view.mySession.SelectedOrder.LinkedID;
        }

        private void FillOrderStatusHistory()
        {
            var r = new OrderStateRepository();
            var h = SrtsHelper.ProcessOrderStateTable(r.GetOrderStateByOrderNumber(_view.mySession.SelectedOrder.OrderNumber));
            _view.OrderStateHistory = h;
            r = null;
            h = null;
        }

        private void FillPrescriptions(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                _view.ODAdd = SrtsHelper.DecimalToString(dr.GetNullableDecimalVal("ODAdd"));
                _view.OSAdd = SrtsHelper.DecimalToString(dr.GetNullableDecimalVal("OSAdd"));
                _view.ODSphere = SrtsHelper.SphereToString(dr.GetNullableDecimalVal("ODSphere"));
                _view.OSSphere = SrtsHelper.SphereToString(dr.GetNullableDecimalVal("OSSphere"));
                _view.ODCylinder = SrtsHelper.CylinderToString(dr.GetNullableDecimalVal("ODCylinder"));
                _view.OSCylinder = SrtsHelper.CylinderToString(dr.GetNullableDecimalVal("OSCylinder"));
                _view.ODAxis = dr.GetStringVal("ODAxis");
                _view.OSAxis = dr.GetStringVal("OSAxis");
                _view.ODHPrism = SrtsHelper.DecimalToString(dr.GetNullableDecimalVal("ODHPrism"));
                _view.ODVPrism = SrtsHelper.DecimalToString(dr.GetNullableDecimalVal("ODVPrism"));
                _view.OSHPrism = SrtsHelper.DecimalToString(dr.GetNullableDecimalVal("OSHPrism"));
                _view.OSVPrism = SrtsHelper.DecimalToString(dr.GetNullableDecimalVal("OSVPrism"));
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

        private void FillDropDowns()
        {
            var demo = _view.mySession.SelectedOrder.Demographic;
            SrtsWeb.BusinessLayer.Concrete.DemographicXMLHelper h = new SrtsWeb.BusinessLayer.Concrete.DemographicXMLHelper();
            var pl = h.GetOrderPrioritiesByBOSStatusAndRank(demo.ToBOSKey(), demo.ToPatientStatusKey(), demo.ToRankKey());
            pl.Remove(pl.Where(x => x.OrderPriorityValue == "N").FirstOrDefault());
            var ope = new OrderPriorityEntity()
            {
                OrderPriorityText = "-Select-",
                OrderPriorityValue = "X"
            };
            pl.Insert(0, ope);
            _view.PriorityList = pl;

            FillLabData();

            _individualRepository = new IndividualRepository();

            DataTable dt2 = _individualRepository.GetIndividualBySiteCodeAndPersonalType(_view.mySession.MyClinicCode, "TECHNICIAN", _view.mySession.MyUserID.ToString());
            _view.TechData = SrtsHelper.ProcessPersonnelDataTable(dt2);

            DataTable dt3 = _individualRepository.GetIndividualBySiteCodeAndPersonalType(_view.mySession.MyClinicCode, "PROVIDER", _view.mySession.MyUserID.ToString());

            ITheaterCodeRepository _locationRepository = new TheaterCodeRepository();

            _view.LocationData = _locationRepository.GetActiveTheaterCodes();

            _view.DoctorData = SrtsHelper.ProcessPersonnelDataTable(dt3);

            _view.LocationSelected = _view.mySession.Patient.Individual.TheaterLocationCode;
        }

        private void FillLabData()
        {
            Dictionary<String, String> _labData = new Dictionary<String, String>();
            string vItem = String.Format("0{0}", _view.mySession.MySite.SinglePrimary.ToUpper());

            string kItem = string.Format("{0} - {1}", "SingleVision", string.IsNullOrEmpty(_view.mySession.MySite.SinglePrimary) ? "MNOST1" : _view.mySession.MySite.SinglePrimary.ToUpper());
            _labData.Add(kItem, vItem);

            vItem = String.Format("1{0}", _view.mySession.MySite.MultiPrimary.ToUpper());

            kItem = string.Format("{0} - {1}", "MultiVision", string.IsNullOrEmpty(_view.mySession.MySite.MultiPrimary) ? "MNOST1" : _view.mySession.MySite.MultiPrimary.ToUpper());
            _labData.Add(kItem, vItem);
            _view.LabData = _labData;
        }

        public void FillFrameData(String priority)
        {
            if (priority.Equals("X"))
            {
                ClearItemsData();
                return;
            }

            _frameRepository = new FrameRepository();

            var demo = String.Empty;
            if (_view.mySession.AddOrEdit.Equals("EDIT"))
                demo = _view.mySession.SelectedOrder.Demographic;
            else
                demo = String.Format("{0}{1}", _view.mySession.Patient.Individual.Demographic.Substring(0, 7), _view.PrioritySelected);

            var dt = _frameRepository.GetFramesByEligibility(demo, _view.mySession.MyClinicCode);

            var fl = SrtsHelper.ProcessFrameTable(dt);

            _view.FrameListData = fl;

            if (fl.Count.Equals(0))
            {
                _view.FrameData = null;
            }
            else
            {
                var dr = dt.NewRow();
                dr["FrameCode"] = "X";
                dr["FrameLongDescription"] = "-Select-";
                dt.Rows.InsertAt(dr, 0);

                if ((String.IsNullOrEmpty(_view.ODAdd) && String.IsNullOrEmpty(_view.ODAdd)) || (_view.ODAdd.Equals("0.00") && _view.OSAdd.Equals("0.00")))
                {
                    dt = dt.AsEnumerable().Where(r => !r["FrameCode"].ToString().Contains("5AM")).CopyToDataTable();
                }

                _view.FrameListData.Insert(0, new FrameEntity() { FrameCode = "X", FrameDescription = "-Select-" });
                _view.FrameData = dt;
                _view.FrameSelected = _view.mySession.AddOrEdit.Equals("EDIT") ? _view.mySession.SelectedOrder.FrameCode : fl[0].FrameCode;
            }
        }

        public void FillItemsData(String frameCode, String demographic)
        {
            var r = new FrameItemsRepository();
            var enumItems = new List<FrameItemEntity>();

            using (var dt = r.GetFrameItemsByFrameCodeAndEligibility(frameCode, demographic))
                enumItems = SrtsHelper.ProcessFrameItemTable(dt);

            var lt = enumItems.Where(x => x.TypeEntry == "LENS_TYPE").ToList();
            var ct = enumItems.Where(x => x.TypeEntry == "COLOR").ToList();
            var et = enumItems.Where(x => x.TypeEntry == "EYE").ToList();
            var bt = enumItems.Where(x => x.TypeEntry == "BRIDGE").ToList();
            var temple = enumItems.Where(x => x.TypeEntry == "TEMPLE").ToList();
            var tint = enumItems.Where(x => x.TypeEntry == "TINT").ToList();
            var mt = enumItems.Where(x => x.TypeEntry == "MATERIAL").ToList();


            lt = IsMultiVision() ? lt : lt.Where(x => x.Value.Contains("SV")).ToList();
            if (lt.Count > 1)
                lt.Insert(0, new FrameItemEntity() { Text = "-Select-", Value = "X" });
            _view.LensData = lt;
            lt = null;


            if (ct.Count > 1)
                ct.Insert(0, new FrameItemEntity() { Text = "-Select-", Value = "X" });
            _view.ColorData = ct;
            ct = null;


            if (et.Count > 1)
                et.Insert(0, new FrameItemEntity() { Text = "-Select-", Value = "X" });
            _view.EyeData = et;
            et = null;


            if (bt.Count > 1)
                bt.Insert(0, new FrameItemEntity() { Text = "-Select-", Value = "X" });
            _view.BridgeData = bt;
            bt = null;


            if (temple.Count > 1)
                temple.Insert(0, new FrameItemEntity() { Text = "-Select-", Value = "X" });
            _view.TempleData = temple;
            temple = null;


            if (mt.Count > 1)
                mt.Insert(0, new FrameItemEntity() { Text = "-Select-", Value = "X" });
            _view.MaterialData = mt;
            mt = null;


            if (tint.Count > 1)
                tint.Insert(0, new FrameItemEntity() { Text = "-Select-", Value = "X" });
            _view.TintData = tint;
            tint = null;

            _view.Pairs = 1;
            _view.Cases = 0;
        }

        public bool HasAddress()
        {
            try
            {
                IAddressRepository ar = new AddressRepository();
                using (var PatientAddress = ar.GetAddressesByIndividualID(_view.mySession.SelectedPatientID, _view.mySession.MyUserID))
                {
                    if (PatientAddress == null || PatientAddress.Rows.Count.Equals(0))
                        return false;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public void ClearItemsData()
        {
            _view.FrameData = new DataTable();
            _view.LensData = new List<FrameItemEntity>();
            _view.ColorData = new List<FrameItemEntity>();
            _view.EyeData = new List<FrameItemEntity>();
            _view.BridgeData = new List<FrameItemEntity>();
            _view.TintData = new List<FrameItemEntity>();
            _view.MaterialData = new List<FrameItemEntity>();
            _view.TempleData = new List<FrameItemEntity>();
        }

        public bool CheckFOCDate()
        {
            if (String.IsNullOrEmpty(_view.LastFocOrderNumber) && _view.FOCDate == null) return false;
            if (_view.LastFocOrderNumber.Equals(_view.mySession.SelectedOrder.OrderNumber)) return false;
            return _view.RequiresJustification;
        }

        public void SetPriority()
        {
            switch (_view.mySession.Patient.Individual.Demographic.ToBOSKey().ToUpper())
            {
                case "A":
                    if (_view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("E") || _view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("O") ||
                        _view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("W"))
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryEligibilityCodes));
                    }
                    else
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryOtherEligibilityCodes));
                    }
                    break;

                case "F":
                    if (_view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("E") || _view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("O"))
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryEligibilityCodes));
                    }
                    else
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryOtherEligibilityCodes));
                    }
                    break;

                case "N":
                    if (_view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("E") || _view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("O") ||
                        _view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("W"))
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryEligibilityCodes));
                    }
                    else
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryOtherEligibilityCodes));
                    }
                    break;

                case "M":
                    if (_view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("E") || _view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("O") ||
                        _view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("W"))
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryEligibilityCodes));
                    }
                    else
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryOtherEligibilityCodes));
                    }
                    break;

                case "C":
                    if (_view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("E") || _view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("O") ||
                        _view.mySession.Patient.Individual.Demographic.ToRankKey().StartsWith("W"))
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryEligibilityCodes));
                    }
                    else
                    {
                        _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryOtherEligibilityCodes));
                    }
                    break;

                case "B":
                    _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(OtherEligibilityCodes));

                    break;

                case "P":
                    _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(OtherEligibilityCodes));
                    break;

                case "K":
                    _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(OtherEligibilityCodes));

                    break;

                default:
                    _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryEligibilityCodes));

                    break;
            }
        }

        public void SetEligibilityVisibility()
        {
            _view.EligibilityVisibility = _view.PrioritySelected.Equals("P") || _view.PrioritySelected.Equals("C");
        }

        private Boolean DoViewValidation()
        {
            if (_view.IsMultiVision)
            {
                if (string.IsNullOrEmpty(_view.ODSegHeight) || string.IsNullOrEmpty(_view.OSSegHeight))
                {
                    _view.Message = "MultiVision requires entries in Seg Height";
                    return false;
                }
            }

            if (_view.PrioritySelected.ToLower().Equals("f") && !_view.Pairs.Equals(1))
            {
                _view.Message = "You may only have one pair of Frame Of Choice frames.";
                return false;
            }

            if (_view.Pairs < 1 || _view.Pairs > 2)
            {
                _view.Message = "The maximum pair of non-FOC frames is two.";
                return false;
            }
            return true;
        }

        private Boolean IsMultiVision()
        {
            if (String.IsNullOrEmpty(_view.ODAdd) && String.IsNullOrEmpty(_view.OSAdd)) return false;
            if (_view.ODAdd.Equals("0.00") && _view.OSAdd.Equals("0.00")) return false;
            return true;
        }

        public void SaveData()
        {
            if (!DoViewValidation()) return;
            _view.Message = string.Empty;

            var linkedId = String.Empty;
            var nums = new List<String>();
            _orderRepository = new OrderRepository();
            /*
             * First, get the order numbers.
             * Second, create the order objects barcode included
             * Third, insert the total order to include linked id if necessary
             *
             * If there is to be more than one pair of eyeware then set the linked id to the order number of the order to be updated
             * do the update
             * loop through the remaining pairs and just replace the order number with the new order number and do the insert
            */

            _saveOE = new OrderEntity();
            _saveOE.OrderNumber = _view.mySession.SelectedOrder.OrderNumber;

            if (_view.Pairs > 1)
            {
                nums = _orderRepository.GetNextOrderNumbers(_view.mySession.MyClinicCode, _view.Pairs - 1);
                linkedId = _saveOE.OrderNumber;
            }

            _saveOE.ClinicSiteCode = _view.mySession.MyClinicCode;
            _saveOE.Demographic = SrtsHelper.BuildProfile(_view.mySession.Patient.Individual.Demographic.ToRankKey(), _view.mySession.Patient.Individual.Demographic.ToBOSKey(), _view.mySession.Patient.Individual.Demographic.ToPatientStatusKey(), _view.mySession.Patient.Individual.Demographic.ToGenderKey(), _view.PrioritySelected);
            _saveOE.FrameBridgeSize = _view.BridgeSelected;
            _saveOE.FrameCode = _view.FrameSelected;
            _saveOE.FrameColor = _view.ColorSelected;
            _saveOE.FrameEyeSize = _view.EyeSelected;
            _saveOE.FrameTempleType = _view.TempleSelected;
            _saveOE.IndividualID_Patient = _view.mySession.Patient.Individual.ID;
            _saveOE.IndividualID_Tech = _view.mySession.MyIndividualID;
            _saveOE.IsGEyes = false;
            _saveOE.IsActive = !_view.OrderStatusID.Equals(17);
            _saveOE.IsMultivision = _view.IsMultiVision;
            _saveOE.LabSiteCode = _view.LabSelected;
            _saveOE.LocationCode = _view.LocationSelected;
            _saveOE.ModifiedBy = _view.mySession.MyUserID;
            _saveOE.NumberOfCases = _view.Cases;
            _saveOE.LensMaterial = _view.MaterialSelected;
            _saveOE.LensType = _view.LensSelected;
            _saveOE.ODSegHeight = _view.ODSegHeight;
            _saveOE.OSSegHeight = _view.OSSegHeight;
            _saveOE.Pairs = 1; // _view.Pairs;
            _saveOE.Tint = _view.TintSelected;
            _saveOE.ShipToPatient = _view.IsShipToPatient;
            _saveOE.PrescriptionID = _view.mySession.SelectedOrder.PrescriptionID;
            _saveOE.ONBarCode = _view.mySession.SelectedOrder.ONBarCode;
            _saveOE.LinkedID = String.IsNullOrEmpty(linkedId) ? _view.LinkedId : linkedId;

            if (_view.IsShipToPatient)
            {
                IAddressRepository ar = new AddressRepository();
                DataRow PatientAddress = ar.GetAddressesByIndividualID(_saveOE.IndividualID_Patient, _view.mySession.MyUserID.ToString()).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();

                _saveOE.ShipAddress1 = PatientAddress[2].ToString();
                _saveOE.ShipAddress2 = PatientAddress[3].ToString();
                _saveOE.ShipAddress3 = PatientAddress[4].ToString();
                _saveOE.ShipCity = PatientAddress[5].ToString();
                _saveOE.ShipState = PatientAddress[6].ToString();
                _saveOE.ShipCountry = PatientAddress[7].ToString();
                _saveOE.ShipZipCode = PatientAddress[8].ToString();
                _saveOE.ShipAddressType = PatientAddress[9].ToString();
            }
            else
            {
                ISiteCodeRepository sr = new SiteCodeRepository();
                DataRow ClinicAddress = sr.GetSiteAddressBySiteID(_view.mySession.MySite.SiteCode.ToString()).AsEnumerable().Where(x => x["AddressType"].ToString().ToLower() == "mail").FirstOrDefault();

                _saveOE.ShipAddress1 = ClinicAddress[2].ToString();
                _saveOE.ShipAddress2 = ClinicAddress[3].ToString();
                _saveOE.ShipAddress3 = ClinicAddress[4].ToString();
                _saveOE.ShipCity = ClinicAddress[5].ToString();
                _saveOE.ShipState = ClinicAddress[6].ToString();
                _saveOE.ShipCountry = ClinicAddress[7].ToString();
                _saveOE.ShipZipCode = ClinicAddress[8].ToString();
                _saveOE.ShipAddressType = ClinicAddress[9].ToString();
            }

            IEMailAddressRepository er = new EMailAddressRepository();
            DataRow EmailAddress = er.GetEmailAddressesByIndividualID(_saveOE.IndividualID_Patient, _view.mySession.MyUserID.ToString()).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();
            if (EmailAddress != null)
                _saveOE.CorrespondenceEmail = EmailAddress[3].ToString();
            else
                _saveOE.CorrespondenceEmail = "system@system.system";

            IPhoneRepository pr = new PhoneRepository();
            DataRow PhoneNumber = pr.GetPhoneNumbersByIndividualID(_saveOE.IndividualID_Patient, _view.mySession.MyUserID.ToString()).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();
            if (PhoneNumber != null)
                _saveOE.PatientPhoneID = (int)PhoneNumber[3];
            else
                _saveOE.PatientPhoneID = 0000;

            // Compare the selected priority to the priority in the select order in mysession
            //---------------------------
            // Check to see if the selected frame is a FOC frame and if it matches the frame in the order in mySession
            // If they are the same then don't mess with the current foc date
            // If they are different then determine if it is a change TO foc.
            // If it is then set the current foc date
            // If it is away from foc then remove the current foc date
            if (_view.PrioritySelected.Equals(_view.mySession.SelectedOrder.Demographic.Substring(_view.mySession.SelectedOrder.Demographic.Length - 1)))
                if (_view.LastFocOrderNumber.Equals(_view.mySession.SelectedOrder.OrderNumber))
                    _saveOE.FocDate = Convert.ToDateTime(_view.FOCDate);
                else
                    _saveOE.FocDate = _view.CurrentFocDate;
            else
            {
                if (_view.PrioritySelected.Equals("F"))
                    _saveOE.FocDate = DateTime.Now;
                else
                    _saveOE.FocDate = null;
            }

            if (string.IsNullOrEmpty(_view.FocJust))
                _saveOE.UserComment1 = SrtsHelper.CheckString(_view.Comment1);
            else
                _saveOE.UserComment1 = SrtsHelper.CheckString(_view.Comment1) + " FOC: " + SrtsHelper.CheckString(_view.FocJust);

            if (string.IsNullOrEmpty(_view.MaterialJust))
                _saveOE.UserComment2 = SrtsHelper.CheckString(_view.Comment2);
            else
                _saveOE.UserComment2 = SrtsHelper.CheckString(_view.Comment2) + " MAT: " + SrtsHelper.CheckString(_view.MaterialJust);

            if (_view.PrioritySelected == "P" || _view.PrioritySelected == "C")
                _saveOE.VerifiedBy = _view.DoctorSelected;
            else
                _saveOE.VerifiedBy = 0;

            DataTable dt = null;

            if (_view.mySession.AddOrEdit == "DUPE")
            {
                #region SAVE A DUPLICATE ORDER
                // Get the lab site code that belongs to the logged in lab.  This is just in case you duplicate an order from another clinic with a different lab than yours
                _saveOE.LabSiteCode = _saveOE.IsMultivision ? _view.mySession.MySite.MultiPrimary.ToUpper() : _view.mySession.MySite.SinglePrimary.ToUpper();

                // If the selected priority is FOC (F) then overwrite any FOCDate to NOW.
                if (_view.PrioritySelected.Equals("F"))
                    _saveOE.FocDate = DateTime.Now;

                // Set the linked id to blank for a duplicate.
                _saveOE.LinkedID = String.Empty;

                IGenBarCodes gbc = new GenerateBarCodes();
                var newOrderNum = _orderRepository.GetNextOrderNumbers(_saveOE.ClinicSiteCode, 1);
                _saveOE.OrderNumber = newOrderNum[0];
                string on = String.Format("*{0}*", _saveOE.OrderNumber);

                using (Image bp = gbc.GenerateBarCode(on))
                {
                    if (bp != null)
                    {
                        using (MemoryStream ms = new System.IO.MemoryStream())
                        {
                            bp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            _saveOE.ONBarCode = ms.ToArray();
                        }
                    }
                }

                dt = _orderRepository.InsertOrder(_saveOE, false);

                _view.OrderStatusID = 0;  // Using 0 to let the logic after the else know that this is to go to a status of incomplete.
                #endregion
            }
            else
            {
                #region UPDATE EXISTING ORDER
                // If the order is in status 1, or 15 include the linked id
                switch (_view.OrderStatusID)
                {
                    case 1: // Clinic Created Order
                    case 15: // Incomplete Order
                        {
                            var doLinkedUpdate = _view.OrderStatusID.Equals(15);

                            // Get all editable order numbers and then loop through those and update each order seperately
                            if (String.IsNullOrEmpty(_saveOE.LinkedID)) // If updating 1 order
                            {
                                dt = _orderRepository.UpdateOrder(_saveOE, doLinkedUpdate);
                            }
                            else if (nums != null && nums.Count > 0) // If there is more than one pair in the pairs property then update the original order then add the others
                            {
                                dt = _orderRepository.UpdateOrder(_saveOE, doLinkedUpdate);
                                // Do the linked order inserts.
                                foreach (var n in nums)
                                {
                                    var soe = _saveOE;
                                    soe.OrderNumber = n;
                                    soe.ONBarCode = null;

                                    // Create barcode for order
                                    IGenBarCodes gbc = new GenerateBarCodes();
                                    var on = String.Format("*{0}*", n);

                                    using (Image bp = gbc.GenerateBarCode(on))
                                    {
                                        if (bp != null)
                                        {
                                            using (MemoryStream ms = new System.IO.MemoryStream())
                                            {
                                                bp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                                soe.ONBarCode = ms.ToArray();
                                            }
                                        }
                                    }

                                    // Insert the new order and add the result to the data table
                                    dt.Merge(_orderRepository.InsertOrder(soe, true));
                                }
                            }
                            else // There is a linked id and no new orders to add based on the number of pairs
                            {
                                var os = GetLinkableOrderNumbers(_saveOE).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var o in os)
                                {
                                    var soe = _saveOE;
                                    soe.OrderNumber = o;

                                    using (var tbl = _orderRepository.UpdateOrder(soe, false))
                                    {
                                        if (dt == null) dt = new DataTable();
                                        dt.Merge(tbl);
                                        soe = null;
                                    }
                                }
                            }
                            break;
                        }
                    //case 15: // Incomplete Order
                    //    {
                    //        dt = _orderRepository.UpdateOrder(_saveOE);
                    //        break;
                    //    }
                    default:
                        {
                            dt = _orderRepository.UpdateOrder(_saveOE, false);
                            break;
                        }
                }
                #endregion
            }

            // If the save failed then add error message and leave function
            if (dt.Rows.Count <= 0)
            {
                _view.Message = "Order was not saved.";
                return;
            }

            #region Update the order status
            var lId = String.Empty;

            var cmt = String.Empty;
            var stat = default(Int32);
            var active = false;
            switch (_view.OrderStatusID)
            {
                case 1:
                    cmt = "Order was successfully edited";
                    stat = _view.OrderStatusID;
                    active = true;
                    lId = String.IsNullOrEmpty(_saveOE.LinkedID) ? _saveOE.OrderNumber : GetLinkableOrderNumbers(_saveOE);

                    break;

                case 3: // Lab Rejected to Clinic Resubmitted
                    {
                        cmt = _view.ClinicJust;
                        stat = 9;
                        active = true;
                        lId = _saveOE.OrderNumber;

                        break;
                    }
                case 17: // Reclaimed
                    {
                        cmt = "Order was never dispensed to customer.  Setting status to reclaimed.";
                        stat = 17;
                        active = false;
                        lId = _saveOE.OrderNumber;

                        break;
                    }
                case 15: // Incomplete to Clinic Created
                    {
                        cmt = "Changing order from Incomplete to Clinic Created";
                        stat = 1;

                        active = true;
                        lId = String.IsNullOrEmpty(_saveOE.LinkedID) ? _saveOE.OrderNumber : GetLinkableOrderNumbers(_saveOE);

                        break;
                    }
                case 0: // Order is a duplicate
                    {
                        // The stored procedure for saving a duplicate automatically adds the incomplete status to the patient order status table
                        return;
                    }
            }

            // Do Update...
            var rr = new OrderStateRepository();
            var ose = new OrderStateEntity();

            ose.OrderNumber = lId;
            ose.StatusComment = cmt;
            ose.LabCode = _view.LabSelected;
            ose.ModifiedBy = _view.mySession.MyUserID;
            ose.OrderStatusTypeID = stat;
            ose.IsActive = active;

            rr.InsertPatientOrderState(ose);
            #endregion

            return;
        }

        public void Delete()
        {
            // Set order record to "inactive"
            var o = _view.mySession.SelectedOrder;
            o.IsActive = false;
            IOrderRepository r = new OrderRepository();
            r.UpdateOrder(o);

            IOrderStateRepository osr = new OrderStateRepository();

            // Modify the status record, change the StatusComment to reflect Clinic deleting the order
            OrderStateEntity ose = new OrderStateEntity();

            ose.OrderNumber = GetLinkableOrderNumbers(o);
            ose.OrderStatusTypeID = 14;
            ose.StatusComment = "Order Deleted by Clinic";
            ose.LabCode = _view.mySession.SelectedOrder.LabSiteCode;
            ose.IsActive = true;
            ose.ModifiedBy = _view.mySession.MyUserID;

            // Update the orders' status record in db
            osr.InsertPatientOrderState(ose);
        }

        private String GetLinkableOrderNumbers(OrderEntity e)
        {
            if (String.IsNullOrEmpty(e.LinkedID)) return String.Empty;

            var tList = new List<String>();
            _orderRepository = new OrderRepository();

            using (DataTable tbl = _orderRepository.GetLinkedOrders(e.LinkedID))
            {
                if (tbl == null || tbl.Rows.Count.Equals(0)) return String.Empty;

                var sr = new OrderStateRepository();
                foreach (DataRow r in tbl.Rows)
                {
                    if (r[0].ToString().Equals(e.OrderNumber))
                    {
                        tList.Add(r[0].ToString());
                        continue;
                    }

                    // Make sure that the linked order number is capable of being edited...
                    var stats = sr.GetOrderStateByOrderNumber(r[0].ToString()).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();

                    switch (stats["OrderStatusTypeID"].ToString())
                    {
                        case "1":
                        case "15":

                            tList.Add(r[0].ToString());
                            break;
                    }
                }
                sr = null;

                if (tList.Count.Equals(0)) return String.Empty;

                var lIdSb = new StringBuilder();

                foreach (var l in tList)
                    lIdSb.AppendFormat("{0},", l);

                return lIdSb.ToString().Remove(lIdSb.ToString().Length - 1);
            }
        }
    }
}