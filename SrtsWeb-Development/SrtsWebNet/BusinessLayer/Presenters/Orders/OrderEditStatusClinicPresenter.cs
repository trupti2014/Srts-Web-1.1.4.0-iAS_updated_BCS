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

namespace SrtsWeb.BusinessLayer.Presenters.Orders
{
    public sealed class OrderEditStatusClinicPresenter
    {
        private IOrderEditStatusClinicView _view;
        private IOrderRepository _orderRepository;
        private IFrameRepository _frameRepository;
        private IPrescriptionRepository _prescriptionRepository;
        private IIndividualRepository _individualRepository;
        private OrderEntity _saveOE;

        public OrderEditStatusClinicPresenter(IOrderEditStatusClinicView view)
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
            IOrderStateRepository osr = new OrderStateRepository();
            List<OrderStateEntity> _state = SrtsHelper.ProcessOrderStateTable(osr.GetPatientOrderStatusByOrderNumber(_view.mySession.tempOrderID));
            foreach (OrderStateEntity oe in _state)
            {
                if (oe.OrderStatusTypeID == 16)
                {
                    _view.IsApproved = false;
                }
            }

            PatientEntity pe;
            _individualRepository = new IndividualRepository();

            DataSet ds = _individualRepository.GetAllPatientInfoByIndividualID(_view.mySession.SelectedOrder.IndividualID_Patient, true, _view.mySession.MyUserID.ToString(), _view.mySession.MySite.SiteCode);
            if (ds.Tables[1].Rows.Count > 0)
            {
                pe = new PatientEntity();
                pe.Individual = SrtsHelper.ProcessIndividualRow(ds.Tables[0].Rows[0]);
                pe.IDNumbers = SrtsHelper.ProcessIdentificationNumberTable(ds.Tables[1]);
                pe.EMailAddresses = SrtsHelper.ProcessEMailAddressTable(ds.Tables[2]);
                pe.PhoneNumbers = SrtsHelper.ProcessPhoneTable(ds.Tables[3]);
                pe.Addresses = SrtsHelper.ProcessAddressTable(ds.Tables[4]);
                pe.Orders = SrtsHelper.ProcessOrderTable(ds.Tables[5]);
                pe.Exams = SrtsHelper.ProcessExamTable(ds.Tables[6]);
                pe.Prescriptions = SrtsHelper.ProcessPresciptionTable(ds.Tables[7]);

                _view.mySession.Patient = pe;
                _view.mySession.SelectedPatientID = _view.mySession.Patient.Individual.ID;

                FillPrescriptions();
                FillDropDowns();
                FillFrameData();
                FillPatientOrderData();

                //var foc = String.Empty;
                var focOrder = String.Empty;
                var reqJust = false;

                _view.FOCDate = _orderRepository.GetLastFOCDate(_view.mySession.Patient.Individual.ID, out focOrder, out reqJust);

                //_view.FOCDate = string.IsNullOrEmpty(foc) ? "None Found" : foc;
                _view.LastFocOrderNumber = focOrder;
                _view.RequiresJustification = reqJust;
            }
        }

        private void FillPatientOrderData()
        {
            _view.TechSelected = _view.mySession.SelectedOrder.IndividualID_Tech;
            _view.Pairs = _view.mySession.SelectedOrder.Pairs;
            _view.Cases = _view.mySession.SelectedOrder.NumberOfCases;
            _view.IsShipToPatient = _view.mySession.SelectedOrder.ShipToPatient;
            _view.FrameSelected = _view.mySession.SelectedOrder.FrameCode;
            _view.EyeSelected = _view.mySession.SelectedOrder.FrameEyeSize;
            _view.BridgeSelected = _view.mySession.SelectedOrder.FrameBridgeSize;
            _view.ColorSelected = _view.mySession.SelectedOrder.FrameColor;
            _view.Comment1 = _view.mySession.SelectedOrder.UserComment1;
            _view.Comment2 = _view.mySession.SelectedOrder.UserComment2;
            _view.LabSelected = _view.mySession.SelectedOrder.LabSiteCode.Substring(0, 1).Equals("M") ? String.Format("1{0}", _view.mySession.SelectedOrder.LabSiteCode) : String.Format("0{0}", _view.mySession.SelectedOrder.LabSiteCode);
            _view.LensSelected = _view.mySession.SelectedOrder.LensType;
            _view.TempleSelected = _view.mySession.SelectedOrder.FrameTempleType;
            _view.MaterialSelected = _view.mySession.SelectedOrder.LensMaterial;

            var odsh = SrtsHelper.DecimalToString(_view.mySession.SelectedOrder.ODSegHeight);
            _view.ODSegHeight = odsh.Equals("3") ? "3B" : odsh.Equals("4") ? "4B" : odsh;
            var ossh = SrtsHelper.DecimalToString(_view.mySession.SelectedOrder.OSSegHeight);
            _view.OSSegHeight = ossh.Equals("3") ? "3B" : ossh.Equals("4") ? "4B" : ossh;

            _view.PrioritySelected = _view.mySession.SelectedOrder.Demographic.ToOrderPriorityKey();
            _view.TintSelected = _view.mySession.SelectedOrder.Tint;
            _view.IsMultiVision = _view.mySession.SelectedOrder.IsMultivision;

            if (string.IsNullOrEmpty(_view.mySession.SelectedOrder.VerifiedBy.ToString()))
            {
                _view.EligibilityVisibility = false;
            }
            else
            {
                _view.DoctorSelected = _view.mySession.SelectedOrder.VerifiedBy;
            }

            _view.IsShipToPatient = _view.mySession.SelectedOrder.ShipToPatient;
            _view.CurrentFocDate = _view.mySession.SelectedOrder.FocDate;
            _view.LinkedID = _view.mySession.SelectedOrder.LinkedID;

            IOrderStateRepository osr = new OrderStateRepository();
            DataTable ost = osr.GetPatientOrderStatusByOrderNumber(_view.mySession.SelectedOrder.OrderNumber);
            _view.OrderStatus = ost.Rows[0]["OrderStatusDescription"].ToString();
            _view.OrderStatusID = (Int32)ost.Rows[0]["OrderStatusID"];
            _view.LabJustification = ost.Rows[0]["StatusComment"].ToString();
        }

        private void FillPrescriptions()
        {
            _prescriptionRepository = new PrescriptionRepository();
            DataTable dtp = _prescriptionRepository.GetPrescriptionByPrescriptionID(_view.mySession.SelectedOrder.PrescriptionID, _view.mySession.MyUserID.ToString());
            foreach (DataRow dr in dtp.Rows)
            {
                _view.ODAdd = SrtsHelper.AddValueToString(dr.GetNullableDecimalVal("ODAdd"));
                _view.OSAdd = SrtsHelper.AddValueToString(dr.GetNullableDecimalVal("OSAdd"));
                _view.ODSphere = SrtsHelper.SphereToString(dr.GetNullableDecimalVal("ODSphere"));
                _view.OSSphere = SrtsHelper.SphereToString(dr.GetNullableDecimalVal("OSSphere"));
                _view.ODCylinder = SrtsHelper.CylinderToString(dr.GetNullableDecimalVal("ODCylinder"));
                _view.OSCylinder = SrtsHelper.CylinderToString(dr.GetNullableDecimalVal("OSCylinder"));
                _view.ODAxis = SrtsHelper.CheckAxis(_view.ODCylinder, SrtsHelper.AxisToString(dr.GetNullableIntVal("ODAxis")));
                _view.OSAxis = SrtsHelper.CheckAxis(_view.OSCylinder, SrtsHelper.AxisToString(dr.GetNullableIntVal("OSAxis")));
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

        public bool CheckFOCDate()
        {
            // Make sure that the lastFocOrderNumber is not the same as the order being edited
            if (String.IsNullOrEmpty(_view.LastFocOrderNumber) && _view.FOCDate == null) return false; //(String.IsNullOrEmpty(_view.FOCDate) || _view.FOCDate.ToLower().Equals("none found"))) return false;
            if (_view.LastFocOrderNumber.Equals(_view.mySession.SelectedOrder.OrderNumber)) return false;
            return _view.RequiresJustification;
            //return true;
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

        private void FillDropDowns()
        {
            FillLabDdl();

            _individualRepository = new IndividualRepository();
            DataTable dt2 = _individualRepository.GetIndividualBySiteCodeAndPersonalType(_view.mySession.MyClinicCode, "TECHNICIAN", _view.mySession.MyUserID.ToString());
            _view.TechData = SrtsHelper.ProcessPersonnelDataTable(dt2);
            DataTable dt3 = _individualRepository.GetIndividualBySiteCodeAndPersonalType(_view.mySession.MyClinicCode, "PROVIDER", _view.mySession.MyUserID.ToString());
            ITheaterCodeRepository _locationRepository = new TheaterCodeRepository();
            _view.LocationData = _locationRepository.GetActiveTheaterCodes();
            _view.DoctorData = SrtsHelper.ProcessPersonnelDataTable(dt3);
            _view.LocationSelected = string.IsNullOrEmpty(_view.mySession.Patient.Individual.TheaterLocationCode) ? "000000000" : _view.mySession.Patient.Individual.TheaterLocationCode;
        }

        public void FillLabDdl()
        {
            Dictionary<String, String> _labData = new Dictionary<String, String>();
            string vItem = String.Format("0{0}", _view.mySession.MySite.SinglePrimary);// "SingleVision";
            string kItem = string.Format("{0} - {1}", "SingleVision", string.IsNullOrEmpty(_view.mySession.MySite.SinglePrimary) ? "MNOST1" : _view.mySession.MySite.SinglePrimary);
            _labData.Add(kItem, vItem);

            vItem = String.Format("1{0}", _view.mySession.MySite.MultiPrimary);// "MultiVision";
            kItem = string.Format("{0} - {1}", "MultiVision", string.IsNullOrEmpty(_view.mySession.MySite.MultiPrimary) ? "MNOST1" : _view.mySession.MySite.MultiPrimary);
            _labData.Add(kItem, vItem);
            _view.LabData = _labData;
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
                    _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(OtherEligibilityCodes));//NOAA
                    break;

                case "P":
                    _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(OtherEligibilityCodes));
                    break;

                case "K":
                    _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(OtherEligibilityCodes));//OTHER
                    break;

                default:
                    _view.PriorityData = SrtsExtender.GetEnumDictionaryDescText(typeof(MilitaryEligibilityCodes));//Other
                    break;
            }
        }

        public void FillFrameData()
        {
            DataView dv;
            var fs = String.Empty;

            if (_view.FrameData == null)
            {
                _frameRepository = new FrameRepository();
                if (_view.mySession.AddOrEdit == "EDIT")
                {
                    dv = new DataView(_frameRepository.GetFramesAndItemsByEligibility(_view.mySession.SelectedOrder.Demographic));
                    fs = _view.mySession.SelectedOrder.FrameCode;
                }
                else
                {
                    var dt = _frameRepository.GetFramesAndItemsByEligibility(String.Format("{0}{1}", _view.mySession.Patient.Individual.Demographic.Substring(0, 7), _view.PrioritySelected));

                    fs = dt.Rows[0]["FrameCode"].ToString();
                    dv = new DataView(dt);
                }
                _view.mySession.FrameData = dv;
                _view.FrameData = dv.ToTable(true, "FrameCode", "FrameLongDescription");
                _view.FrameSelected = fs;
            }

            dv = _view.mySession.FrameData;

            FillItemsData(dv);
        }

        private Boolean IsMultiVision()
        {
            if (String.IsNullOrEmpty(_view.ODAdd) && String.IsNullOrEmpty(_view.OSAdd)) return false;
            if (_view.ODAdd.Equals("0.00") && _view.OSAdd.Equals("0.00")) return false;
            return true;
        }

        public void FillItemsData(DataView dv)
        {
            dv.RowFilter = string.Format("TypeEntry = 'LENS_TYPE' AND FrameCode = '{0}'", _view.FrameSelected);
            var dt = new DataTable();
            if (IsMultiVision())
                dt = dv.ToTable(true, "Text", "Value");
            else
            {
                dt.Columns.AddRange(new DataColumn[2] { new DataColumn("Text"), new DataColumn("Value") });
                foreach (DataRow r in dv.ToTable(true, "Text", "Value").Rows)
                {
                    if (!r["Value"].ToString().ToUpper().Contains("SV")) continue;
                    dt.ImportRow(r);
                }
            }
            _view.LensData = dt;
            dv.RowFilter = string.Format("TypeEntry = 'COLOR' AND FrameCode = '{0}'", _view.FrameSelected);
            _view.ColorData = dv.ToTable(true, "Text", "Value");
            dv.RowFilter = string.Format("TypeEntry = 'EYE' AND FrameCode = '{0}'", _view.FrameSelected);
            _view.EyeData = dv.ToTable(true, "Text", "Value");
            dv.RowFilter = string.Format("TypeEntry = 'BRIDGE' AND FrameCode = '{0}'", _view.FrameSelected);
            _view.BridgeData = dv.ToTable(true, "Text", "Value");
            dv.RowFilter = string.Format("TypeEntry = 'TINT' AND FrameCode = '{0}'", _view.FrameSelected);
            _view.TintData = dv.ToTable(true, "Text", "Value");
            dv.RowFilter = string.Format("TypeEntry = 'Material' AND FrameCode = '{0}'", _view.FrameSelected);
            _view.MaterialData = dv.ToTable(true, "Text", "Value");
            dv.RowFilter = string.Format("TypeEntry = 'TEMPLE' AND FrameCode = '{0}'", _view.FrameSelected);
            _view.TempleData = dv.ToTable(true, "Text", "Value");
            _view.Pairs = 1;
            _view.Cases = 1;
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

            if (_view.MaterialSelected == "HI" && _view.Comment1 == string.Empty)
            {
                _view.Message = "You must Justify the HI Index material in the comment1 section";
                return false;
            }

            return true;
        }

        public void SaveData()
        {
            if (_view.mySession.SelectedOrder.PrescriptionID.Equals(0))
            {
                _view.Message = "Prescription ID is a required field.";
                return;
            }

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
            var o = _view.mySession.SelectedOrder;

            if (_view.Pairs > 1)
            {
                nums = _orderRepository.GetNextOrderNumbers(_view.mySession.MyClinicCode, _view.Pairs - 1);
                linkedId = o.OrderNumber;
            }

            _saveOE.ClinicSiteCode = _view.mySession.MyClinicCode;
            _saveOE.Demographic = o.Demographic;
            _saveOE.FrameBridgeSize = _view.BridgeSelected;
            _saveOE.FrameCode = _view.FrameSelected;
            _saveOE.FrameColor = _view.ColorSelected;
            _saveOE.FrameEyeSize = _view.EyeSelected;
            _saveOE.FrameTempleType = _view.TempleSelected;
            _saveOE.PrescriptionID = o.PrescriptionID;
            _saveOE.IndividualID_Patient = o.IndividualID_Patient;
            _saveOE.IndividualID_Tech = _view.TechSelected;
            _saveOE.PatientPhoneID = o.PatientPhoneID;
            _saveOE.IsActive = true;
            _saveOE.IsGEyes = false;
            _saveOE.IsMultivision = _view.IsMultiVision;

            if (_saveOE.IsMultivision)
            {
                if (string.IsNullOrEmpty(_view.ODSegHeight) || string.IsNullOrEmpty(_view.OSSegHeight))
                {
                    _view.Message = "MultiVision requires entries in Seg Height";
                    return;
                }
            }

            _saveOE.LabSiteCode = _view.LabSelected.Substring(_view.LabSelected.Length - 6, 6).Trim();
            _saveOE.LocationCode = SrtsHelper.SetLocationCode(o.LocationCode);
            _saveOE.ModifiedBy = _view.mySession.MyUserID;
            _saveOE.NumberOfCases = _view.Cases;
            _saveOE.LensMaterial = SrtsHelper.CheckString(_view.MaterialSelected);
            if (_view.MaterialSelected == "HI")
            {
                if (_view.Comment1 == string.Empty)
                {
                    _view.Message = "You must Justify the HI Index material in the comment1 section";
                    return;
                }
            }
            _saveOE.LensType = SrtsHelper.CheckString(_view.LensSelected);

            var t1 = _view.ODSegHeight.ToLower();
            _saveOE.ODSegHeight = SrtsHelper.StringToDecimal(_view.LensSelected.Substring(0, 2).ToLower().Equals("sv") ?
                "" :
                t1.Equals("4b") ?
                "4" :
                t1.Equals("3b") ?
                "3" :
                t1);

            var t2 = _view.OSSegHeight.ToLower();
            _saveOE.OSSegHeight = SrtsHelper.StringToDecimal(_view.LensSelected.Substring(0, 2).ToLower().Equals("sv") ?
                "" :
                t2.Equals("4b") ?
                "4" :
                t2.Equals("3b") ?
                "3" :
                t2);

            _saveOE.OrderNumber = o.OrderNumber;
            _saveOE.Pairs = 1; // _view.Pairs;
            _saveOE.Tint = SrtsHelper.CheckString(_view.TintSelected);

            // Compare the selected priority to the priority in the select order in mysession
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

            _saveOE.UserComment1 = SrtsHelper.CheckString(_view.Comment1);
            _saveOE.UserComment2 = SrtsHelper.CheckString(_view.Comment2);

            if (_view.PrioritySelected == "P" || _view.PrioritySelected == "CLINIC")
                _saveOE.VerifiedBy = _view.DoctorSelected;
            else
                _saveOE.VerifiedBy = 0;

            _saveOE.LinkedID = String.IsNullOrEmpty(linkedId) ? o.LinkedID : linkedId;

            // Update the order
            DataTable dt = _orderRepository.UpdateOrder(_saveOE);

            //_view.mySession.Patient.Orders = new List<OrderEntity>();
            //_view.mySession.Patient.Orders.Add(_saveOE);

            //if (_view.mySession.Patient.Orders.Count <= 0)
            if (dt.Rows.Count <= 0)
            {
                _view.Message = "Order was not saved.";
                return;
            }

            dt = null;
            string CurrentOrderStatus = string.Empty;
            IOrderStateRepository osr = new OrderStateRepository();

            dt = osr.GetPatientOrderStatusByOrderNumber(o.OrderNumber);
            CurrentOrderStatus = dt.Rows[0]["OrderStatusTypeID"].ToString();

            if (CurrentOrderStatus != "3")
            {
                if (!_orderRepository.UpdateOrderStatus(_view.LabSelected, o.OrderNumber, true, "Completed Problem Order", _view.mySession.MyUserID, 1))
                {
                    _view.Message = "Order status was not saved.";
                    return;
                }
            }
            else
            {
                if (!_orderRepository.UpdateOrderStatus(_view.LabSelected, o.OrderNumber, true, "Clinic Resubmitted Order", _view.mySession.MyUserID, 9))
                {
                    _view.Message = "Order status was not saved.";
                    return;
                }
            }

            foreach (var num in nums)
            {
                _orderRepository = new OrderRepository();
                var oe = _saveOE;
                oe.OrderNumber = num;
                // Clear out the byte array to prepare for the new barcode.
                oe.ONBarCode = null;

                // GET PATIENT ADDRESS, EMAIL, AND PHONE NUMBER INFORMATION
                _saveOE.ShipToPatient = _view.IsShipToPatient;
                if (_view.IsShipToPatient)
                {
                    IAddressRepository ar = new AddressRepository();
                    DataRow PatientAddress = ar.GetAddressesByIndividualID(_saveOE.IndividualID_Patient, _view.mySession.MyUserID).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();

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
                    DataRow ClinicAddress = sr.GetSiteAddressBySiteID(_view.mySession.MySite.SiteCode).AsEnumerable().Where(x => x["AddressType"].ToString().ToLower() == "mail").FirstOrDefault();

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
                DataRow EmailAddress = er.GetEmailAddressesByIndividualID(_saveOE.IndividualID_Patient, _view.mySession.MyUserID).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();
                if (EmailAddress != null)
                    _saveOE.CorrespondenceEmail = EmailAddress[3].ToString();
                else
                    _saveOE.CorrespondenceEmail = "system@system.system";

                IPhoneRepository pr = new PhoneRepository();
                DataRow PhoneNumber = pr.GetPhoneNumbersByIndividualID(_saveOE.IndividualID_Patient, _view.mySession.MyUserID).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();
                if (PhoneNumber != null)
                    _saveOE.PatientPhoneID = (int)PhoneNumber[3];
                else
                    _saveOE.PatientPhoneID = 0000;

                // Create barcode for order
                IGenBarCodes gbc = new GenerateBarCodes();
                var on = String.Format("*{0}*", num);

                using (Image bp = gbc.GenerateBarCode(on))
                {
                    if (bp != null)
                    {
                        using (MemoryStream ms = new System.IO.MemoryStream())
                        {
                            bp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            oe.ONBarCode = ms.ToArray();
                        }
                    }
                }

                // Insert the new order
                using (var t = _orderRepository.InsertOrder(oe, true))
                {
                    if (t == null || t.Rows.Count.Equals(0))
                    {
                        _view.Message = "An unknown error occurred trying to save the linked order.";
                        return;
                    }
                }
            }

        }

        public void UpdateApproved()
        {
            IOrderStateRepository osr = new OrderStateRepository();
            osr.ApproveOrder(_view.mySession.SelectedOrder.OrderNumber, _view.mySession.MyUserID, _view.LabSelected);
        }

        public void Delete()
        {
            var o = _view.mySession.SelectedOrder;
            o.IsActive = false;
            IOrderRepository or = new OrderRepository();
            or.UpdateOrder(o);
        }
    }
}