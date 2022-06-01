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
    public sealed class OrderAddPresenter
    {
        private IOrderAddView _view;
        private IOrderRepository _orderRepository;
        private IFrameRepository _frameRepository;
        private IPrescriptionRepository _prescriptionRepository;
        private IIndividualRepository _individualRepository;
        private OrderEntity _saveOE;

        public OrderAddPresenter(IOrderAddView view)
        {
            _view = view;
        }

        public void InitView()
        {
            var demo = _view.mySession.Patient.Individual.Demographic;
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

            FillDropDowns();
            ProcessAdd();
            SetEligibilityVisibility();

            if (!string.IsNullOrEmpty(_view.mySession.TempID.ToString()))
            {
                _prescriptionRepository = new PrescriptionRepository();
                FillPrescriptions(_prescriptionRepository.GetPrescriptionByPrescriptionID(_view.mySession.TempID, _view.mySession.MyUserID.ToString()));
            }

            _orderRepository = new OrderRepository();
            var foc = String.Empty;
            var focOrder = String.Empty;
            var reqJust = false;

            foc = _orderRepository.GetLastFOCDate(_view.mySession.Patient.Individual.ID, out focOrder, out reqJust).ToMilDateString();

            _view.FOCDate = string.IsNullOrEmpty(foc) ? "None Found" : foc;
            _view.LastFocOrderNumber = focOrder;
            _view.RequiresJustification = reqJust;
            _view.Pairs = 1;

            VerifyPatientData();
        }

        private void ProcessAdd()
        {
            var tmp = _view.mySession.Patient.Individual.Demographic.ToRankKey();
            int tmpInt;

            if (!Int32.TryParse(tmp.Substring(1, 2), out tmpInt)) return;

            if (tmp.Substring(0, 1).Equals("O") && tmpInt > 6)
            {
                _view.PrioritySelected = "V";
                FillFrameData();
            }
        }

        private void VerifyPatientData()
        {
            var msg = new System.Text.StringBuilder();

            IEMailAddressRepository er = new EMailAddressRepository();
            using (var EmailAddress = er.GetEmailAddressesByIndividualID(_view.mySession.SelectedPatientID, _view.mySession.MyUserID))
            {
                if (EmailAddress == null || EmailAddress.Rows.Count.Equals(0))
                    msg.Append("Email address is required.<br />");
            }

            IPhoneRepository pr = new PhoneRepository();
            using (var PhoneNumber = pr.GetPhoneNumbersByIndividualID(_view.mySession.SelectedPatientID, _view.mySession.MyUserID))
            {
                if (PhoneNumber == null || PhoneNumber.Rows.Count.Equals(0))
                    msg.Append("Phone number is required.<br />");
            }

            _view.Message = String.IsNullOrEmpty(msg.ToString()) ? _view.Message : msg.ToString();

            if (_view.mySession.Patient.Addresses.Count < 1)
            {
                _view.WarningMsg = "Patient has no address on file, orders will only ship to clinic.";
            }

            if (_view.ODAdd == "0.00" && _view.OSAdd == "0.00")
            {
                _view.WarningMsg = "Selected prescription has no 'Add Power', only single-vision lenses will be displayed.";
            }
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

        //public bool CheckFOCDate()
        //{
        //    return !String.IsNullOrEmpty(_view.FOCDate) && !_view.FOCDate.ToLower().Equals("none found");
        //}

        public void SetEligibilityVisibility()
        {
            _view.EligibilityVisibility = _view.PrioritySelected.Equals("P") || _view.PrioritySelected.Equals("C");
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
                    _view.LabSelected = vItem;//j String.Format("1{0}", s);
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
            //else if (_view.LabData == null) return;
            else if (_view.LabData == null || _view.LabData.Count.Equals(1))
            {
                // reload the ddl
                FillLabData();
            }

            if (String.IsNullOrEmpty(_view.LensSelected)) return;
            if (_view.LensSelected.Equals("X")) return;

            // Multi to multi
            // Single to single or multi
            var l = _view.LensSelected.Substring(0, 2).ToLower().Equals("sv") ? "Single" : "Multi";
            if (l.Equals("Single")) return;
            _view.LabSelected = _view.LabData.Where(x => x.Key.StartsWith(l)).FirstOrDefault().Value;
        }

        public void SetLabTb()
        {
            // If the selected frame is part of the CustFramToLabColl variable then set the LabSelected property to the correct lab.
            if (OrderEntity.CustFrameToLabColl.ContainsKey(_view.FrameSelected))//this.ddlFrame.SelectedValue))
            {
                var sc = new List<String>();
                OrderEntity.CustFrameToLabColl.TryGetValues(_view.FrameSelected, out sc);

                if (_view.FrameSelected.ToLower().StartsWith("alep"))
                    _view.LabSelected = sc[0];
                else
                {
                    var s = sc.FirstOrDefault(x => x == _view.mySession.MySite.MultiPrimary);
                    if (String.IsNullOrEmpty(s))
                        _view.LabSelected = "MNOST1";
                    else
                        _view.LabSelected = s;
                }

                // No matter what lens is selected if the frame is required to go to a special lab then exit method before processing lens ddls.
                return;
            }

            if (String.IsNullOrEmpty(_view.LensSelected)) return;
            if (_view.LensSelected.Equals("X")) return;

            _view.LabSelected = _view.LensSelected.Substring(0, 2).ToLower().Equals("sv") ? _view.mySession.MySite.SinglePrimary : _view.mySession.MySite.MultiPrimary;
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

        private void FillDropDowns()
        {
            _individualRepository = new IndividualRepository();

            DataTable dt2 = _individualRepository.GetIndividualBySiteCodeAndPersonalType(_view.mySession.MyClinicCode, "TECHNICIAN", _view.mySession.MyUserID.ToString());
            _view.TechData = SrtsHelper.ProcessPersonnelDataTable(dt2);
            _view.TechSelected = _view.mySession.MyIndividualID;

            DataTable dt3 = _individualRepository.GetIndividualBySiteCodeAndPersonalType(_view.mySession.MyClinicCode, "PROVIDER", _view.mySession.MyUserID.ToString());
            _view.DoctorData = SrtsHelper.ProcessPersonnelDataTable(dt3);

            if (_view.mySession.Patient.IDNumbers.Count == 0)
            {
                _view.Message = "Patient Does not have an active ID number on record. You must add one before continuing!";
            }
        }

        private void FillPrescriptions(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                _view.ODAdd = SrtsHelper.AddValueToString(dr.GetNullableDecimalVal("ODAdd"));
                _view.OSAdd = SrtsHelper.AddValueToString(dr.GetNullableDecimalVal("OSAdd"));
                _view.ODSphere = SrtsHelper.SphereToString(dr.GetNullableDecimalVal("ODSphere"));
                _view.OSSphere = SrtsHelper.SphereToString(dr.GetNullableDecimalVal("OSSphere"));
                _view.ODCylinder = SrtsHelper.CylinderToString(dr.GetNullableDecimalVal("ODCylinder"));
                _view.OSCylinder = SrtsHelper.CylinderToString(dr.GetNullableDecimalVal("OSCylinder"));
                _view.ODAxis = dr.GetStringVal("ODAxis");
                _view.OSAxis = dr.GetStringVal("OSAxis");
                _view.ODHPrism = SrtsHelper.PrismToString(dr.GetNullableDecimalVal("ODHPrism"));
                _view.ODVPrism = SrtsHelper.PrismToString(dr.GetNullableDecimalVal("ODVPrism"));
                _view.OSHPrism = SrtsHelper.PrismToString(dr.GetNullableDecimalVal("OSHPrism"));
                _view.OSVPrism = SrtsHelper.PrismToString(dr.GetNullableDecimalVal("OSVPrism"));
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

        public void FillFrameData()
        {
            if (_view.PrioritySelected.Equals("X"))
            {
                ClearItemsData();
                return;
            }

            _frameRepository = new FrameRepository();
            string demo = _view.mySession.Patient.Individual.Demographic;

            demo = demo.Substring(0, 7) + _view.PrioritySelected;

            var dt = _frameRepository.GetFramesByEligibility(demo, _view.mySession.MyClinicCode);

            var fl = SrtsHelper.ProcessFrameTable(dt);

            //var fl = dt.AsEnumerable().Select(x => new FrameEntity()
            //{
            //    FrameCode = x["FrameCode"].ToString(),
            //    FrameDescription = x["FrameDescription"].ToString(),
            //    FrameNotes = x["FrameNotes"].ToString(),
            //    IsInsert = Convert.ToBoolean(x["IsInsert"]),
            //    IsFoc = Convert.ToBoolean(x["IsFOC"]),
            //    IsActive = true,
            //    MaxPair = Convert.ToInt32(x["MaxPair"])
            //}).ToList();

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

                _view.FrameData = dt;
            }
        }

        //public void FillItemsData(DataView dv)
        //{
        //    var dtTmp = new DataTable();
        //    DataRow drT;
        //    dv.RowFilter = string.Format("TypeEntry = 'LENS_TYPE' AND FrameCode = '{0}'", _view.FrameSelected);
        //    var dt = new DataTable();

        //    if (IsMultiVision())
        //        dt = dv.ToTable(true, "Text", "Value");
        //    else
        //    {
        //        dt.Columns.AddRange(new DataColumn[2] { new DataColumn("Text"), new DataColumn("Value") });
        //        foreach (DataRow r in dv.ToTable(true, "Text", "Value").Rows)
        //        {
        //            if (!r["Value"].ToString().ToUpper().Contains("SV")) continue;
        //            dt.ImportRow(r);
        //        }
        //    }
        //    dtTmp = dt;
        //    if (dtTmp.Rows.Count > 1)
        //    {
        //        drT = dtTmp.NewRow();
        //        drT["Text"] = "-Select-";
        //        drT["Value"] = "X";
        //        dtTmp.Rows.InsertAt(drT, 0);
        //    }
        //    _view.LensData = dtTmp;

        //    drT = null;
        //    dtTmp.Clear();

        //    dv.RowFilter = string.Format("TypeEntry = 'COLOR' AND FrameCode = '{0}'", _view.FrameSelected);
        //    dtTmp = dv.ToTable(true, "Text", "Value");
        //    if (dtTmp.Rows.Count > 1)
        //    {
        //        drT = dtTmp.NewRow();
        //        drT["Text"] = "-Select-";
        //        drT["Value"] = "X";
        //        dtTmp.Rows.InsertAt(drT, 0);
        //    }
        //    _view.ColorData = dtTmp;

        //    drT = null;
        //    dtTmp.Clear();

        //    dv.RowFilter = string.Format("TypeEntry = 'EYE' AND FrameCode = '{0}'", _view.FrameSelected);
        //    dtTmp = dv.ToTable(true, "Text", "Value");
        //    if (dtTmp.Rows.Count > 1)
        //    {
        //        drT = dtTmp.NewRow();
        //        drT["Text"] = "-Select-";
        //        drT["Value"] = "X";
        //        dtTmp.Rows.InsertAt(drT, 0);
        //    }
        //    _view.EyeData = dtTmp;

        //    drT = null;
        //    dtTmp.Clear();

        //    dv.RowFilter = string.Format("TypeEntry = 'BRIDGE' AND FrameCode = '{0}'", _view.FrameSelected);
        //    dtTmp = dv.ToTable(true, "Text", "Value");
        //    if (dtTmp.Rows.Count > 1)
        //    {
        //        drT = dtTmp.NewRow();
        //        drT["Text"] = "-Select-";
        //        drT["Value"] = "X";
        //        dtTmp.Rows.InsertAt(drT, 0);
        //    }
        //    _view.BridgeData = dtTmp;

        //    drT = null;
        //    dtTmp.Clear();

        //    dv.RowFilter = string.Format("TypeEntry = 'TINT' AND FrameCode = '{0}'", _view.FrameSelected);
        //    dtTmp = dv.ToTable(true, "Text", "Value");
        //    if (dtTmp.Rows.Count > 1)
        //    {
        //        drT = dtTmp.NewRow();
        //        drT["Text"] = "-Select-";
        //        drT["Value"] = "X";
        //        dtTmp.Rows.InsertAt(drT, 0);
        //    }
        //    _view.TintData = dtTmp;

        //    drT = null;
        //    dtTmp.Clear();

        //    dv.RowFilter = string.Format("TypeEntry = 'Material' AND FrameCode = '{0}'", _view.FrameSelected);
        //    dtTmp = dv.ToTable(true, "Text", "Value");
        //    if (dtTmp.Rows.Count > 1)
        //    {
        //        drT = dtTmp.NewRow();
        //        drT["Text"] = "-Select-";
        //        drT["Value"] = "X";
        //        dtTmp.Rows.InsertAt(drT, 0);
        //    }
        //    _view.MaterialData = dtTmp;

        //    drT = null;
        //    dtTmp.Clear();

        //    dv.RowFilter = string.Format("TypeEntry = 'TEMPLE' AND FrameCode = '{0}'", _view.FrameSelected);
        //    dtTmp = dv.ToTable(true, "Text", "Value");
        //    if (dtTmp.Rows.Count > 1)
        //    {
        //        drT = dtTmp.NewRow();
        //        drT["Text"] = "-Select-";
        //        drT["Value"] = "X";
        //        dtTmp.Rows.InsertAt(drT, 0);
        //    }
        //    _view.TempleData = dtTmp;

        //    drT = null;
        //    dtTmp.Clear();

        //    //_view.Pairs = 1;
        //    _view.Cases = 0;
        //}

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


            if (bt.Count >1)
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

            _view.Cases = 0;
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

        //public void ClearItemsData()
        //{
        //    _view.FrameData = new DataTable();
        //    _view.LensData = new DataTable();
        //    _view.ColorData = new DataTable();
        //    _view.EyeData = new DataTable();
        //    _view.BridgeData = new DataTable();
        //    _view.TintData = new DataTable();
        //    _view.MaterialData = new DataTable();
        //    _view.TempleData = new DataTable();
        //}

        private Boolean IsMultiVision()
        {
            if (String.IsNullOrEmpty(_view.ODAdd) && String.IsNullOrEmpty(_view.OSAdd)) return false;
            if (_view.ODAdd.Equals("0.00") && _view.OSAdd.Equals("0.00")) return false;
            return true;
        }

        private Boolean DoViewValidation()
        {
            if (_view.IsComplete == true)
            {
                if (_view.mySession.TempID.Equals(0))
                {
                    _view.Message = "Prescription ID is required.";
                    return false;
                }

                if (_view.IsMultiVision)
                {
                    if (string.IsNullOrEmpty(_view.ODSegHeight) || string.IsNullOrEmpty(_view.OSSegHeight))
                    {
                        _view.Message = "MultiVision requires entries in Seg Height.";
                        return false;
                    }
                }

                var fd = _view.FrameListData;
                var mp = Convert.ToInt32(fd.Where(x => x.FrameCode.ToLower() == _view.FrameSelected.ToLower()).FirstOrDefault().MaxPair);
                if (_view.Pairs > mp)
                {
                    _view.Message = String.Format("You have exceeded the max pairs ({0}) of eyeware that can be ordered.", mp);
                    return false;
                }
            }
            return true;
        }

        public void SaveData()
        {
            if (!DoViewValidation()) return;
            _view.Message = string.Empty;

            var linkedId = String.Empty;

            /*
             * First, get the order numbers.
             * Second, create the order objects barcode included
             * Third, insert the total order to include linked id if necessary
            */

            _orderRepository = new OrderRepository();
            var nums = _orderRepository.GetNextOrderNumbers(_view.mySession.MyClinicCode, _view.Pairs);
            if (nums.Count > 1) linkedId = nums[0];
            foreach (var num in nums)
            {
                _saveOE = new OrderEntity();
                _saveOE.OrderNumber = num;
                _saveOE.PrescriptionID = _view.mySession.TempID;
                _saveOE.ClinicSiteCode = _view.mySession.MyClinicCode;
                _saveOE.Demographic = SrtsHelper.BuildProfile(
                    _view.mySession.Patient.Individual.Demographic.ToRankKey(),
                    _view.mySession.Patient.Individual.Demographic.ToBOSKey(),
                    _view.mySession.Patient.Individual.Demographic.ToPatientStatusKey(),
                    _view.mySession.Patient.Individual.Demographic.ToGenderKey(),
                    _view.PrioritySelected);
                _saveOE.FrameBridgeSize = SrtsHelper.CheckString(_view.BridgeSelected);
                _saveOE.FrameCode = SrtsHelper.CheckString(_view.FrameSelected);
                _saveOE.FrameColor = SrtsHelper.CheckString(_view.ColorSelected);
                _saveOE.FrameEyeSize = SrtsHelper.CheckString(_view.EyeSelected);
                _saveOE.FrameTempleType = SrtsHelper.CheckString(_view.TempleSelected);
                _saveOE.IndividualID_Patient = _view.mySession.Patient.Individual.ID;

                _saveOE.IndividualID_Tech = _view.mySession.MyIndividualID;
                //_saveOE.IndividualID_Tech = Convert.ToInt32(System.Web.HttpContext.Current.Session["Tech_ID"].ToString());

                _saveOE.IsActive = true;
                _saveOE.IsGEyes = false;
                _saveOE.IsMultivision = _view.IsMultiVision;
                _saveOE.LabSiteCode = _view.LabSelected;
                _saveOE.LocationCode = SrtsHelper.SetLocationCode(_view.LocationSelected);
                _saveOE.ModifiedBy = _view.mySession.MyUserID;
                _saveOE.NumberOfCases = _view.Cases;
                _saveOE.LensMaterial = _view.MaterialSelected;
                _saveOE.LensType = _view.LensSelected;

                _saveOE.ODSegHeight = _view.ODSegHeight;
                _saveOE.OSSegHeight = _view.OSSegHeight;

                _saveOE.Pairs = 1;
                _saveOE.Tint = SrtsHelper.CheckString(_view.TintSelected);

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
                if (EmailAddress == null)
                {
                    _view.Message = "An active email address is required.";
                    return;
                }
                _saveOE.CorrespondenceEmail = EmailAddress[3].ToString();

                IPhoneRepository pr = new PhoneRepository();
                DataRow PhoneNumber = pr.GetPhoneNumbersByIndividualID(_saveOE.IndividualID_Patient, _view.mySession.MyUserID).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();
                if (PhoneNumber == null)
                {
                    _view.Message = "An active phone number is required.";
                    return;
                }
                _saveOE.PatientPhoneID = (int)PhoneNumber[3];

                if (_view.PrioritySelected.Equals("F"))
                    _saveOE.FocDate = DateTime.Now;
                else
                    _saveOE.FocDate = null;

                if (string.IsNullOrEmpty(_view.FocJust))
                {
                    _saveOE.UserComment1 = SrtsHelper.CheckString(_view.Comment1);
                }
                else
                {
                    _saveOE.UserComment1 = SrtsHelper.CheckString(_view.Comment1) + " FOC: " + SrtsHelper.CheckString(_view.FocJust);
                }

                if (string.IsNullOrEmpty(_view.MaterialJust))
                {
                    _saveOE.UserComment2 = SrtsHelper.CheckString(_view.Comment2);
                }
                else
                {
                    _saveOE.UserComment2 = SrtsHelper.CheckString(_view.Comment2) + " MAT: " + SrtsHelper.CheckString(_view.MaterialJust);
                }

                if (_view.PrioritySelected == "P" || _view.PrioritySelected == "C")
                {
                    _saveOE.VerifiedBy = _view.DoctorSelected;
                }
                else
                {
                    _saveOE.VerifiedBy = 0;
                }

                _saveOE.LinkedID = linkedId;

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
                            _saveOE.ONBarCode = ms.ToArray();
                        }
                    }
                }

                // Insert data
                using (var dt = _orderRepository.InsertOrder(_saveOE, _view.IsComplete))
                {
                    if (dt.Rows.Count <= 0)
                    {
                        _view.Message = "Order was not saved.";
                        return;
                    }
                }
            }
        }

        public void SaveIncomplete()
        {
            _view.Message = string.Empty;
            _saveOE = new OrderEntity();
            var linkedId = String.Empty;

            /*
              * First, get the order numbers.
              * Second, create the order objects barcode included
              * Third, insert the total order to include linked id if necessary
             */

            _orderRepository = new OrderRepository();
            _view.Pairs = _view.Pairs.Equals(0) ? 1 : _view.Pairs;
            var nums = _orderRepository.GetNextOrderNumbers(_view.mySession.MyClinicCode, _view.Pairs);
            if (nums.Count > 1) linkedId = nums[0];
            foreach (var num in nums)
            {
                _saveOE.OrderNumber = num;
                _saveOE.PrescriptionID = _view.mySession.TempID;
                _saveOE.ClinicSiteCode = _view.mySession.MyClinicCode;

                _saveOE.Demographic = SrtsHelper.BuildProfile(
                    _view.mySession.Patient.Individual.Demographic.ToRankKey(),
                    _view.mySession.Patient.Individual.Demographic.ToBOSKey(),
                    _view.mySession.Patient.Individual.Demographic.ToPatientStatusKey(),
                    _view.mySession.Patient.Individual.Demographic.ToGenderKey(),
                    _view.PrioritySelected.ToOrderPriorityKey());
                _saveOE.FrameBridgeSize = string.IsNullOrEmpty(_view.BridgeSelected) ? string.Empty : _view.BridgeSelected;
                _saveOE.FrameCode = string.IsNullOrEmpty(_view.FrameSelected) ? string.Empty : _view.FrameSelected;
                _saveOE.FrameColor = string.IsNullOrEmpty(_view.ColorSelected) ? string.Empty : _view.ColorSelected;
                _saveOE.FrameEyeSize = string.IsNullOrEmpty(_view.EyeSelected) ? string.Empty : _view.EyeSelected;
                _saveOE.FrameTempleType = string.IsNullOrEmpty(_view.TempleSelected) ? string.Empty : _view.TempleSelected;
                _saveOE.IndividualID_Patient = _view.mySession.Patient.Individual.ID;

                _saveOE.IndividualID_Tech = _view.mySession.MyIndividualID;
                //_saveOE.IndividualID_Tech = Convert.ToInt32(System.Web.HttpContext.Current.Session["Tech_ID"].ToString());

                _saveOE.IsActive = true;
                _saveOE.IsGEyes = false;
                _saveOE.IsMultivision = _view.IsMultiVision;
                _saveOE.LabSiteCode = string.IsNullOrEmpty(_view.LabSelected) ? _view.mySession.MySite.MultiPrimary : _view.LabSelected;
                _saveOE.LocationCode = SrtsHelper.SetLocationCode(_view.LocationSelected);
                _saveOE.ModifiedBy = _view.mySession.MyUserID;
                _saveOE.NumberOfCases = string.IsNullOrEmpty(_view.Cases.ToString()) ? 0 : _view.Cases;
                _saveOE.LensMaterial = string.IsNullOrEmpty(_view.MaterialSelected) ? string.Empty : _view.MaterialSelected;
                _saveOE.LensType = string.IsNullOrEmpty(_view.LensSelected) ? string.Empty : _view.LensSelected;

                _saveOE.ODSegHeight = _view.ODSegHeight;
                _saveOE.OSSegHeight = _view.OSSegHeight;

                _saveOE.Pairs = string.IsNullOrEmpty(_view.Pairs.ToString()) ? 0 : 1;
                _saveOE.Tint = string.IsNullOrEmpty(SrtsHelper.CheckString(_view.TintSelected)) ? string.Empty : SrtsHelper.CheckString(_view.TintSelected);
                _saveOE.UserComment1 = string.IsNullOrEmpty(SrtsHelper.CheckString(_view.Comment1)) ? string.Empty : SrtsHelper.CheckString(_view.Comment1);
                _saveOE.UserComment2 = string.IsNullOrEmpty(SrtsHelper.CheckString(_view.Comment2)) ? string.Empty : SrtsHelper.CheckString(_view.Comment2);
                _saveOE.LinkedID = linkedId;

                if (string.IsNullOrEmpty(_view.FocJust))
                    _saveOE.UserComment1 = SrtsHelper.CheckString(_view.Comment1);
                else
                    _saveOE.UserComment1 = SrtsHelper.CheckString(_view.Comment1) + " FOC: " + SrtsHelper.CheckString(_view.FocJust);

                if (string.IsNullOrEmpty(_view.MaterialJust))
                    _saveOE.UserComment2 = SrtsHelper.CheckString(_view.Comment2);
                else
                    _saveOE.UserComment2 = SrtsHelper.CheckString(_view.Comment2) + " MAT: " + SrtsHelper.CheckString(_view.MaterialJust);


                _saveOE.ShipToPatient = _view.IsShipToPatient;
                if (_view.IsShipToPatient)
                {
                    IAddressRepository ar = new AddressRepository();
                    DataRow PatientAddress = ar.GetAddressesByIndividualID(_saveOE.IndividualID_Patient, _view.mySession.MyUserID).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();
                    if (PatientAddress != null && PatientAddress[2] != null)
                    {
                        _saveOE.ShipAddress1 = PatientAddress[2].ToString();
                        _saveOE.ShipAddress2 = PatientAddress[3].ToString();
                        _saveOE.ShipAddress3 = PatientAddress[4].ToString();
                        _saveOE.ShipCity = PatientAddress[5].ToString();
                        _saveOE.ShipState = PatientAddress[6].ToString();
                        _saveOE.ShipCountry = PatientAddress[7].ToString();
                        _saveOE.ShipZipCode = PatientAddress[8].ToString();
                        _saveOE.ShipAddressType = PatientAddress[9].ToString();
                    }
                }
                else
                {
                    ISiteCodeRepository sr = new SiteCodeRepository();
                    DataRow ClinicAddress = sr.GetSiteAddressBySiteID(_view.mySession.MySite.SiteCode).AsEnumerable().Where(x => x["AddressType"].ToString().ToLower().Equals("mail")).FirstOrDefault();

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
                if (EmailAddress != null && EmailAddress[0] != null)
                {
                    _saveOE.CorrespondenceEmail = EmailAddress[3].ToString();
                }

                IPhoneRepository pr = new PhoneRepository();
                DataRow PhoneNumber = pr.GetPhoneNumbersByIndividualID(_saveOE.IndividualID_Patient, _view.mySession.MyUserID).AsEnumerable().Where(x => Convert.ToBoolean(x["IsActive"]) == true).FirstOrDefault();
                if (PhoneNumber != null && PhoneNumber[0] != null)
                {
                    _saveOE.PatientPhoneID = (int)PhoneNumber[3];
                }

                if (_view.PrioritySelected == "P" || _view.PrioritySelected == "C")
                {
                    _saveOE.VerifiedBy = _view.DoctorSelected;
                }
                else
                {
                    _saveOE.VerifiedBy = 0;
                }

                IGenBarCodes gbc = new GenerateBarCodes();
                var on = String.Format("*{0}*", num);

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

                using (var dt = _orderRepository.InsertIncompleteOrder(_saveOE, _view.IsComplete))
                {
                    if (dt.Rows.Count <= 0)
                    {
                        _view.Message = "Data was not saved";
                    }
                }
            }
        }
    }
}