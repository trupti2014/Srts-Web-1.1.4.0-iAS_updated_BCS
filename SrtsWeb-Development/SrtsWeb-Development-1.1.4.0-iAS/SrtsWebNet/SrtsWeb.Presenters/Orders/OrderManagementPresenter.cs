using BarcodeLib;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static SrtsWeb.DataLayer.Repositories.SiteRepository;

namespace SrtsWeb.Presenters.Orders
{
    public class OrderManagementPresenter
    {
        private IOrderManagementView v;
        
        public OrderManagementPresenter(IOrderManagementView view)
        {
            v = view;
        }

        public void GetPatientData()
        {
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var r = new IndividualRepository();
            this.v.Patient = r.GetIndividualByIndividualID(this.v.PatientId, ModifiedBy).FirstOrDefault();
            FillPatientAddressDdls();
        }


        public async Task<IndividualEntity> GetPatientDataAsync()
        {
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var r = new IndividualRepository();
            return await Task.Run<IndividualEntity>(() =>
            {
                return r.GetIndividualByIndividualID(this.v.PatientId, ModifiedBy).FirstOrDefault();
            });
        }

        public void GetProviderList()
        {
            var MyClinicCode = string.IsNullOrEmpty(this.v.mySession.MyClinicCode) ? Globals.SiteCode : this.v.mySession.MyClinicCode;
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var r = new IndividualRepository();
            this.v.ProviderList = r.GetIndividualBySiteCodeAndPersonalType(MyClinicCode, "PROVIDER", ModifiedBy);
        }

        public async Task<List<IndividualEntity>> GetProviderListAsync()
        {
            var MyClinicCode = string.IsNullOrEmpty(this.v.mySession.MyClinicCode) ? Globals.SiteCode : this.v.mySession.MyClinicCode;
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var r = new IndividualRepository();
            return await Task.Run<List<IndividualEntity>>(() =>
            {
                return r.GetIndividualBySiteCodeAndPersonalType(MyClinicCode, "PROVIDER", ModifiedBy);
            });
        }

        public void MakePatientActiveDuty()
        {
            var r = new IndividualRepository();
            var p = this.v.Patient;
            p.Demographic = this.v.Demographic;
            r.UpdateIndividual(p);
        }

        #region PRESCRIPTION

        public void GetAllPrescriptions()
        {
            var r = new OrderManagementRepository.PrescriptionRepository();
            v.PrescriptionList = r.GetAllPrescriptions(v.PatientId, this.v.mySession.ModifiedBy);
        }

        public void GetRxPreferences()
        {
            var r = new SitePreferencesRepository.RxPreferencesRepository();
            this.v.SitePrefsRX = r.GetRxDefaults(this.v.mySession.MySite.SiteCode);
        }

        public async Task<List<Prescription>> GetAllPrescriptionsAsync()
        {
            var r = new OrderManagementRepository.PrescriptionRepository();
            return await Task.Run<List<Prescription>>(() => { return r.GetAllPrescriptions(v.PatientId, this.v.mySession.ModifiedBy); });
        }

        public void GetPrescriptionbyID(int key)
        {
            var r = new OrderManagementRepository.PrescriptionRepository();
            v.Prescription = r.GetPrescriptionById(key, this.v.mySession.ModifiedBy);
        }

        public void GetPrescriptionProvider(int id)
        {
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var r = new IndividualRepository();
            List<IndividualEntity> provider = new List<IndividualEntity>();
            provider = r.GetIndividualByIndividualID(id, ModifiedBy);
            this.v.ProviderList = provider;
        }

        public bool DeletePrescription()
        {
            var pe = this.v.Prescription;
            if (pe.IsNull()) return false;

            var r = new OrderManagementRepository.PrescriptionRepository();

            return r.DeletePrescription(pe.PrescriptionId, this.v.mySession.ModifiedBy);
        }


        public bool DeletePrescriptionScan(int RxScanId)
        {
            var pe = this.v.PrescriptionDocument;
            if (pe.IsNull()) return false;

            var r = new OrderManagementRepository.PrescriptionRepository();

            return r.DeleteScannedPrescription(RxScanId);
        }


        public bool UpdatePrescription(Prescription pe)
        {
            pe.OdAxisCalc = GroomCalcFieldForDb(pe.OdAxisCalc);
            pe.OdCylinderCalc = GroomCalcFieldForDb(pe.OdCylinderCalc);
            pe.OdSphereCalc = GroomCalcFieldForDb(pe.OdSphereCalc);
            pe.OsAxisCalc = GroomCalcFieldForDb(pe.OsAxisCalc);
            pe.OsCylinderCalc = GroomCalcFieldForDb(pe.OsCylinderCalc);
            pe.OsSphereCalc = GroomCalcFieldForDb(pe.OsSphereCalc);

            pe.ExamId = string.IsNullOrEmpty(this.v.mySession.SelectedExamID.ToString()) ? 0 : this.v.mySession.SelectedExamID;

            var r = new OrderManagementRepository.PrescriptionRepository();
            var good = r.UpdatePrescription(pe, v.mySession.ModifiedBy);

            if (!good) return false;

            if (pe.ExtraRxTypes.Count.Equals(0)) return true;

            if (pe.ExtraRxTypes.Contains("D"))
            {
                var dvo = CreateDvoRx(pe);
                var dvoRxId = 0;
                var a = r.InsertPrescription(dvo, v.mySession.ModifiedBy, out dvoRxId);
            }

            if (pe.ExtraRxTypes.Contains("N"))
            {
                var nvo = CreateNvoRx(pe);
                var nvoRxId = 0;
                var b = r.InsertPrescription(nvo, v.mySession.ModifiedBy, out nvoRxId);
            }

            return good;

        }

        public bool InsertPrescription()
        {
            var r = default(Int32);
            return InsertPrescription(out r);
        }

        public bool InsertPrescription(out Int32 rxId)
        {
            var pe = this.v.Prescription;
            pe.OdAxisCalc = GroomCalcFieldForDb(pe.OdAxisCalc);
            pe.OdCylinderCalc = GroomCalcFieldForDb(pe.OdCylinderCalc);
            pe.OdSphereCalc = GroomCalcFieldForDb(pe.OdSphereCalc);
            pe.OsAxisCalc = GroomCalcFieldForDb(pe.OsAxisCalc);
            pe.OsCylinderCalc = GroomCalcFieldForDb(pe.OsCylinderCalc);
            pe.OsSphereCalc = GroomCalcFieldForDb(pe.OsSphereCalc);

            var r = new OrderManagementRepository.PrescriptionRepository();
            var result = r.InsertPrescription(pe, v.mySession.ModifiedBy, out rxId);
            var good = result.GreaterThan(0);
            // return p.Value.ToInt32().Equals(1);
            if (!good) return false;
            if (pe.PrescriptionDocument != null && pe.PrescriptionDocument.Id == 0)
            {
                pe.PrescriptionId = result;
                pe.PrescriptionDocument.PrescriptionId = pe.PrescriptionId;
                pe.PrescriptionDocument.DocumentName = Path.GetFileName(pe.PrescriptionDocument.DocumentName);
                if (pe.PrescriptionDocument.DocumentImage != null)
                {
                    var insertResult = InsertScannedPrescription(pe);
                }
                pe.PrescriptionDocument = null;
            }

            if (pe.ExtraRxTypes.Count.Equals(0)) return true;

            if (pe.ExtraRxTypes.Contains("D"))
            {
                var dvo = CreateDvoRx(pe);
                var dvoRxId = 0;
                var a = r.InsertPrescription(dvo, v.mySession.ModifiedBy, out dvoRxId);
                dvo = null;
            }

            if (pe.ExtraRxTypes.Contains("N"))
            {
                var nvo = CreateNvoRx(pe);
                var nvoRxId = 0;
                var b = r.InsertPrescription(nvo, v.mySession.ModifiedBy, out nvoRxId);
                nvo = null;
            }

            return good;
        }

        public Int32 InsertScannedPrescription(Prescription pe)
        {
            var r = new OrderManagementRepository.PrescriptionRepository();
            var scanId = 0;
            var result = r.InsertScannedPrescription(pe.PatientId, pe.PrescriptionId, pe.PrescriptionDocument.DocumentName, pe.PrescriptionDocument.DocumentType, pe.PrescriptionDocument.DocumentImage, out scanId);
            return result;
        }


        public bool IsHighPowerLensRx()
        {
            //If either Abs(sph) or Abs(sph + cyl) is >= 6, then select hi-index.  
            //For example, a +7.00 -2.00 would be high index, as would a -5.00-2.00. 

            var odSph = this.v.Prescription.OdSphereCalc.ToDecimal();
            var odCyl = this.v.Prescription.OdCylinderCalc.ToDecimal();
            var osSph = this.v.Prescription.OsSphereCalc.ToDecimal();
            var osCyl = this.v.Prescription.OsCylinderCalc.ToDecimal();

            return Math.Abs(odSph) >= 6 || Math.Abs(odSph + odCyl) >= 6 || Math.Abs(osSph) >= 6 || Math.Abs(osSph + osCyl) >= 6;
        }

        private String GroomCalcFieldForDb(String calcField)
        {
            var d = default(Decimal);
            if (Decimal.TryParse(calcField, out d))
                return d.ToString();
            return default(Decimal).ToString();
        }
        private Prescription CreateNvoRx(Prescription rxIn)
        {
            var nvo = (Prescription)rxIn.Clone();

            var odS = nvo.OdSphere + nvo.OdAdd;
            var osS = nvo.OsSphere + nvo.OsAdd;

            nvo.OdSphere = odS;
            nvo.OsSphere = osS;

            nvo.OdSphereCalc = odS.ToString("0.00");
            nvo.OsSphereCalc = osS.ToString("0.00");

            nvo.OdAdd = 0;
            nvo.OsAdd = 0;

            nvo.OdPdDist = 0;
            nvo.OsPdDist = 0;
            nvo.PdDistTotal = 0;

            nvo.PrescriptionName = "NVO";

            return nvo;
        }
        private Prescription CreateDvoRx(Prescription rxIn)
        {
            var dvo = (Prescription)rxIn.Clone();
            dvo.OdAdd = 0;
            dvo.OsAdd = 0;
            dvo.PrescriptionName = "DVO";
            return dvo;
        }
        #endregion PRESCRIPTION

        #region ORDER

        public Order GetOrderByOrderNumber(String OrderNumber)
        {
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var r = new OrderManagementRepository.OrderRepository();
            var o = r.GetOrderByOrderNumber(OrderNumber, ModifiedBy);

            var oer = new OrderManagementRepository.OrderEmailRepository();
            var oe = oer.GetOrderEmailByOrderNumber(OrderNumber, ModifiedBy);
            if (oe != null)
            {
                o.OrderEmailAddress = oe.EmailAddress;
                o.IsPermanentEmailAddressChange = oe.ChangeEmail;
            }

            return o;
        }

        public void GetAllOrders()
        {
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var clinicCode = string.IsNullOrEmpty(this.v.mySession.MyClinicCode) ? Globals.SiteCode : this.v.mySession.MyClinicCode;
            var r = new OrderManagementRepository.OrderRepository();
            v.OrderList = r.GetAllOrders(this.v.PatientId, ModifiedBy, clinicCode);
            foreach(Order order in v.OrderList)
            {
                var oer = new OrderManagementRepository.OrderEmailRepository();
                var oe = oer.GetOrderEmailByOrderNumber(order.OrderNumber, ModifiedBy);
                if (oe != null)
                {
                    order.OrderEmailAddress = oe.EmailAddress;
                    order.IsPermanentEmailAddressChange = oe.ChangeEmail;
                }
            }
        }

        public void GetOrderStatusHistory(String OrderNumber)
        {
            var r = new OrderStateRepository.OrderStatusRepository();
            var h = r.GetOrderStateByOrderNumber(OrderNumber);
            r = null;
            this.v.OrderStateHistory = h;

            // Check if the order is a lab rejected order.
            DoIsOrderRejected();
        }

        public EMailAddressEntity GetEmailAddressByPatientID()
        {
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var _emailRepository = new EMailAddressRepository();
            var emailAddress = _emailRepository.GetEmailAddressesByIndividualID(this.v.PatientId, ModifiedBy).Where(ia => ia.IsActive).FirstOrDefault();
            return emailAddress;           
        }

        public bool CompareOrders(Order original, Order compare)
        {
            if (!original.Bridge.Equals(compare.Bridge)) return false;
            if (!original.Cases.Equals(compare.Cases)) return false;
            if (!original.Color.Equals(compare.Color)) return false;
            if (!original.Eye.Equals(compare.Eye)) return false;
            if (!original.Frame.Equals(compare.Frame)) return false;
            if (!original.LabSiteCode.Equals(compare.LabSiteCode)) return false;
            if (!original.LensType.Equals(compare.LensType)) return false;
            if (!original.Material.Equals(compare.Material)) return false;
            if (!original.OdSegHeight.Equals(compare.OdSegHeight)) return false;
            if (!original.OsSegHeight.Equals(compare.OsSegHeight)) return false;
            if (!original.Pairs.Equals(compare.Pairs)) return false;
            if (!original.Priority.Equals(compare.Priority)) return false;
            if (!original.OrderDisbursement.Equals(compare.OrderDisbursement)) return false;
            if (!original.Temple.Equals(compare.Temple)) return false;
            if (!original.Tint.Equals(compare.Tint)) return false;
            if (!original.Coatings.Equals(compare.Coatings)) return false;

            return true;
        }

        public Boolean IsReorder()
        {
            if (this.v.OrderStateHistory.FirstOrDefault(x => x.OrderStatusType.ToLower() == "clinic created order").DateLastModified.DateDiff(DateTime.Today) > 30) return false;
            // Get a known, unaltered, copy of the order to compare to the potentially altered order.
            var selO = GetOrderByOrderNumber(this.v.Order.OrderNumber);
            var pageO = this.v.Order;
            // Comparer the selected order against the page order to see if it is the same.
            return CompareOrders(selO, pageO);
        }

        public Boolean IsReorderWithin30Days()
        {
            var pageO = this.v.Order;
            GetAllOrders();
            foreach (Order o in this.v.OrderList)
            {
                if ( CompareOrders(o, pageO) && o.DateLastModified.DateDiff(DateTime.Today) < 30 && o.CurrentStatus.ToLower() == "clinic order created" && this.v.Prescription.PrescriptionId == o.PrescriptionId) 
                    return true;
            }
            return false;
        }

        public Boolean IsOrderDataValid()
        {
            return false;
        }

        public void DoIsOrderRejected()
        {
            var j = this.v.OrderStateHistory.FirstOrDefault(x => x.IsActive == true);
            if (!j.OrderStatusTypeID.Equals(3)) return;
            this.v.LabJustification = j.StatusComment;
        }

        public bool DeleteOrder()
        {
            var o = this.v.Order;
            if (o.IsNull()) return false;

            var r = new OrderManagementRepository.OrderRepository();
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            return r.DeleteOrder(o.OrderNumber, ModifiedBy);
        }

        public bool ReturnOrderToStock()
        {
            bool isGood = true;
            var o = this.v.Order;
            o.IsActive = true;
            o.LabSiteCode = String.IsNullOrEmpty(this.v.Order.LabSiteCode) || this.v.Order.LabSiteCode.Equals("X") ? this.v.mySession.MySite.MultiPrimary : this.v.Order.LabSiteCode;
            var r = new OrderManagementRepository.OrderRepository();
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            //if (!r.UpdateOrder(o, ModifiedBy)) isGood = false;

            string orderLab = r.UpdateOrder(o, ModifiedBy);
            if (string.IsNullOrEmpty(orderLab)) isGood = false;

            var osr = new OrderStateRepository.OrderStatusRepository();

            // Modify the status record, change the StatusComment to reflect Clinic returning order to stock
            var ose = new OrderStateEntity();

            ose.OrderNumber = o.OrderNumber;
            ose.OrderStatusTypeID = 17;
            ose.StatusComment = "Order Returned to Stock by Clinic";
            ose.LabCode = orderLab;//String.IsNullOrEmpty(this.v.Order.LabSiteCode) || this.v.Order.LabSiteCode.Equals("X") ? this.v.mySession.MySite.MultiPrimary : this.v.Order.LabSiteCode;
            ose.IsActive = true;
            ose.ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;

            // Update the orders' status record in db
            osr.InsertPatientOrderState(ose);

            return isGood;
        }

        public bool DispenseOrderFromStock()
        {
            var osr = new OrderStateRepository.OrderStatusRepository();

            // Modify the status record, change the StatusComment to reflect Clinic returning order to stock
            var ose = new OrderStateEntity();

            ose.OrderNumber = this.v.Order.OrderNumber;
            ose.OrderStatusTypeID = 8;
            ose.StatusComment = "Order Dispensed from Clinic Stock";
            ose.LabCode = String.IsNullOrEmpty(this.v.Order.LabSiteCode) || this.v.Order.LabSiteCode.Equals("X") ? this.v.mySession.MySite.MultiPrimary : this.v.Order.LabSiteCode;
            ose.IsActive = true;
            ose.ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;

            // Update the orders' status record in db
            return osr.InsertPatientOrderState(ose);
        }

        public bool UpdateOrder()
        {
            bool isGood = true;
            var o = this.v.Order;
            var extraOrderNumbers = new List<String>();
            var r = new OrderManagementRepository.OrderRepository();
            var tmpPairs = 0;

            if (o.Pairs > 1)
            {
                o.LinkedId = o.OrderNumber;
                tmpPairs = o.Pairs;
                o.Pairs = 1;
            }

            //o.GroupName = this.v.GroupName;

            // Update Order(s) to the database
            var sb = new StringBuilder(o.Comment1);
            if (!String.IsNullOrEmpty(o.FocJustification))
            {
                sb.AppendFormat("FOC:{0} ", o.FocJustification);
                //o.Comment1 = sb.ToString();
            }
            if (!String.IsNullOrEmpty(o.CoatingJustification))
            {
                sb.AppendFormat("COATING {0}:{1} ", o.Coatings, o.CoatingJustification);  
            }
            o.Comment1 = sb.ToString();
            sb.Clear();

            if (!String.IsNullOrEmpty(o.MaterialJustification))
            {
                sb.Append(o.Comment2);
                sb.AppendFormat("MAT:{0}", o.MaterialJustification);
                o.Comment2 = sb.ToString();
            }

            // Add a barcode to the order
            if (o.OrderNumberBarCode == null || o.OrderNumberBarCode.Length.Equals(0))
                o.OrderNumberBarCode = GetBarcode(new GenerateBarCodes(new Barcode()), o.OrderNumber);
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            //Add Order Email Address update here
            if (o.IsEmailPatient)
            {
                var _orderemailRespository = new OrderManagementRepository.OrderEmailRepository();
                var orderemail = _orderemailRespository.GetOrderEmailByOrderNumber(o.OrderNumber, this.v.mySession.ModifiedBy);
                if( orderemail == null )
                {
                    var neworderemail = new OrderEmail();
                    neworderemail.OrderNumber = o.OrderNumber;
                    neworderemail.EmailAddress = o.OrderEmailAddress;
                    neworderemail.EmailMsg = "";
                    neworderemail.EmailDate = null;
                    neworderemail.PatientId = o.PatientId;
                    neworderemail.ChangeEmail = o.IsPermanentEmailAddressChange;
                    neworderemail.EmailSent = false;
                    var insertOrderEmailSuccess = _orderemailRespository.InsertOrderEmail(neworderemail, o.ModifiedBy);
                }
                else 
                {
                    bool needupdate = false;
                    if (!orderemail.EmailAddress.Equals(o.OrderEmailAddress))
                    {
                        orderemail.EmailAddress = o.OrderEmailAddress;
                        needupdate = true;
                    }
                    if (orderemail.ChangeEmail != o.IsPermanentEmailAddressChange)
                    {
                        orderemail.ChangeEmail = o.IsPermanentEmailAddressChange;
                        needupdate = true;

                    }
                    if (needupdate)
                    {
                        var updateOrderEmailSuccess = _orderemailRespository.UpdateOrderEmail(orderemail, this.v.mySession.ModifiedBy);
                    }
                    if (o.IsPermanentEmailAddressChange && needupdate)
                    {
                        EMailAddressEntity entity = GetEmailAddressByPatientID();
                        entity.EMailAddress = o.OrderEmailAddress;
                        UpdateEmailAddress(entity);
                    }
                   
                }
            }
            string orderLab = r.UpdateOrder(o, ModifiedBy);
            if (string.IsNullOrEmpty(orderLab)) isGood = false;

            //if (!r.UpdateOrder(o, ModifiedBy)) isGood = false;

            // Update the order status
            InsertOrderStatus(orderLab);

            if (tmpPairs.Equals(0)) return isGood;

            extraOrderNumbers.AddRange(r.GetNextOrderNumbers(o.ClinicSiteCode, o.Pairs - 1));
            var i = 0;
            do
            {
                o.OrderNumber = extraOrderNumbers[i];
                o.Pairs = 1;
                o.OrderNumberBarCode = GetBarcode(new GenerateBarCodes(new Barcode()), o.OrderNumber);
                r.InsertOrder(o, ModifiedBy);
                i++;
            } while (i < extraOrderNumbers.Count);

            return isGood;
        }

        public bool InsertOrder()
        {
            var o = this.v.Order;
            var extraOrderNumbers = new List<String>();
            var r = new OrderManagementRepository.OrderRepository();
            bool isGood = true;

            // If more than one pair of eyewear is requesten then get extra order numbers

            extraOrderNumbers = r.GetNextOrderNumbers(this.v.mySession.MyClinicCode, o.Pairs.Equals(0) ? 1 : o.Pairs);
            if (extraOrderNumbers.Count.Equals(1))
            {
                o.LinkedId = String.Empty;
            }
            else
            {
                o.LinkedId = extraOrderNumbers[0];
                o.Pairs = 1;
            }

            // Add Order(s) to the database
            o.Demographic = String.Format("{0}{1}", this.v.Demographic.Substring(0, 7), o.Priority);
            o.PatientId = this.v.PatientId;
            o.TechnicianId = this.v.mySession.MyIndividualID;
            o.ClinicSiteCode = this.v.mySession.MyClinicCode;
            o.LabSiteCode = String.IsNullOrEmpty(o.LabSiteCode) || o.LabSiteCode.Equals("X") ? this.v.mySession.MySite.MultiPrimary : o.LabSiteCode;
            o.PrescriptionId = this.v.Prescription.PrescriptionId;
            o.IsActive = true;
            //o.GroupName = this.v.GroupName;

            var sb = new StringBuilder(o.Comment1);
            if (!String.IsNullOrEmpty(o.FocJustification))
            {
                sb.AppendFormat("FOC:{0} ", o.FocJustification);
                //o.Comment1 = sb.ToString();
            }
            if (!String.IsNullOrEmpty(o.CoatingJustification))
            {
                sb.AppendFormat("COATING {0}: {1} ", o.Coatings, o.CoatingJustification);
            }
            o.Comment1 = sb.ToString();
            sb.Clear();

            if (!String.IsNullOrEmpty(o.MaterialJustification))
            {
                sb.Append(o.Comment2);
                sb.AppendFormat("MAT:{0}", o.MaterialJustification);
                o.Comment2 = sb.ToString();
            }

            var i = 0;
            do
            {
                o.OrderNumber = extraOrderNumbers[i];
                // Add a barcode to the order
                o.OrderNumberBarCode = GetBarcode(new GenerateBarCodes(new Barcode()), o.OrderNumber);
                var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
                if (!r.InsertOrder(o, ModifiedBy)) isGood = false;

                i++;
            } while (i < extraOrderNumbers.Count);

            if (isGood && o.IsEmailPatient)
            {
                var orderemail = new OrderEmail();
                orderemail.OrderNumber = o.OrderNumber;
                orderemail.EmailAddress = o.OrderEmailAddress;
                orderemail.EmailMsg = "";
                orderemail.EmailDate = null;
                orderemail.PatientId = o.PatientId;
                orderemail.ChangeEmail = o.IsPermanentEmailAddressChange;
                orderemail.EmailSent = false;

                var _orderemailRespository = new OrderManagementRepository.OrderEmailRepository();
                var insertOrderEmailSuccess = _orderemailRespository.InsertOrderEmail(orderemail, o.ModifiedBy);
            }
            //o.GroupName = this.v.GroupName;
            return isGood;
        }

        //public String GetOrderGroupName()
        //{
        //    return this.v.GroupName;
        //}
        public void InsertOrderStatus(string orderLabSiteCode)
        {
            var lId = String.Empty;
            var cmt = String.Empty;
            var stat = default(Int32);
            var active = false;

            // Get current active status
            var os = 0;

            if (this.v.OrderStateHistory == null)
                GetOrderStatusHistory(this.v.Order.OrderNumber);
            os = this.v.OrderStateHistory.Where(x => x.IsActive == true).FirstOrDefault().OrderStatusTypeID;

            switch (os)
            {
                case 1:
                    cmt = "Order was successfully edited";
                    stat = 1;
                    active = true;
                    lId = String.IsNullOrEmpty(this.v.Order.LinkedId) ? this.v.Order.OrderNumber : GetLinkableOrderNumbers(this.v.Order);

                    break;

                case 3: // Lab Rejected to Clinic Resubmitted
                    {
                        cmt = this.v.ClinicJustification;
                        stat = 9;
                        active = true;
                        lId = this.v.Order.OrderNumber;

                        break;
                    }
                case 17: // Return to stock
                    {
                        cmt = "Order was never dispensed to customer.  Setting status to return to stock.";
                        stat = 17;
                        active = false;
                        lId = this.v.Order.OrderNumber;

                        break;
                    }
                case 15: // Incomplete to Clinic Created
                    {
                        cmt = "Changing order from Incomplete to Clinic Created";
                        stat = 1;

                        active = true;
                        lId = String.IsNullOrEmpty(this.v.Order.LinkedId) ? this.v.Order.OrderNumber : GetLinkableOrderNumbers(this.v.Order);

                        break;
                    }
                case 0: // Order is a duplicate
                    {
                        // The stored procedure for saving a duplicate automatically adds the incomplete status to the patient order status table
                        return;
                    }
                default:
                    {
                        break;
                    }
            }

            // Do Update...
            var rr = new OrderStateRepository.OrderStatusRepository();
            var ose = new OrderStateEntity();

            ose.OrderNumber = lId;
            ose.StatusComment = cmt;
            ose.LabCode = orderLabSiteCode;//this.v.Order.LabSiteCode;
            ose.ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            ose.OrderStatusTypeID = stat;
            ose.IsActive = active;

            rr.InsertPatientOrderState(ose);
        }

        public void GetPriorities()
        {
            var h = new DemographicXMLHelper();
            var demo = v.Demographic;
            var p = h.GetOrderPrioritiesByBOSStatusAndRank(demo.ToBOSKey(), demo.ToPatientStatusKey(), demo.ToRankKey()).
                Where(x => x.OrderPriorityValue != "N").  //CR 0059911 Priority list giving "N" as an option
                Select(x => new { key = x.OrderPriorityText, value = x.OrderPriorityValue }).
                ToDictionary(x => x.key, x => x.value);
            if (p == null) return;
            var l = new OrderDropDownData();
            l.PriorityList = p;
            v.OrderDdlData = l;
        }

        public void GetFrames(string Demographic, string SiteCode, int RxId)
        {
            var r = new OrderManagementRepository.FrameRepository();

            var l = r.GetFrameData(Demographic, SiteCode, RxId);

            v.OrderDdlData.FrameList = l;
        }

        public void GetFrameItems(String Demographic, String FrameCode)
        {
            var r = new FrameItemsRepository();
            var l = r.GetFrameItemsByFrameCodeAndEligibility(FrameCode, String.Format("000000B{0}", Demographic.ToOrderPriorityKey()));

            v.OrderDdlData.BridgeList = l.Where(x => x.TypeEntry.ToLower() == "bridge").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

            v.OrderDdlData.ColorList = l.Where(x => x.TypeEntry.ToLower() == "color").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

            v.OrderDdlData.EyeList = l.Where(x => x.TypeEntry.ToLower() == "eye").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

            v.OrderDdlData.LensTypeList = l.Where(x => x.TypeEntry.ToLower() == "lens_type").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

            v.OrderDdlData.MaterialList = l.Where(x => x.TypeEntry.ToLower() == "material").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

            v.OrderDdlData.TempleList = l.Where(x => x.TypeEntry.ToLower() == "temple").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

            v.OrderDdlData.TintList = l.Where(x => x.TypeEntry.ToLower() == "tint").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

            v.OrderDdlData.CoatingList = l.Where(x => x.TypeEntry.ToLower() == "coating").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

            v.SpecialLabTintList = l.Where(x => x.TypeEntry.ToLower() == "tint" && x.Availability.ToLower() == "w").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

            var fidr = new FrameItemsDefaultsRepository();
            v.DefaultFrameItems = fidr.GetFrameItemsDefaults(FrameCode);

            GetReOrderReasons();
        }

        public void GetLabs(String FrameCode = "", String TintCode = "")
        {
            if (this.v.OrderDdlData.LabSiteCodeList == null)
                this.v.OrderDdlData.LabSiteCodeList = new Dictionary<string, string>();
            this.v.OrderDdlData.LabSiteCodeList.Clear();

            var l = String.Empty;
            // If the selected frame has a special lab requirement then use the special lab.
            if (IsSpecialLab(FrameCode, out l))
            {
                this.v.OrderDdlData.LabSiteCodeList.Clear();
                this.v.OrderDdlData.LabSiteCodeList.Add(String.Format("{0} - {1}", l.StartsWith("M") ? "MV" : "SV", l), String.Format("0{0}", l));
            }
            else // Use the labs assigned to the logged on users clinic site.
            {
                if (this.v.mySession.MySite == null) return;
                var d = new Dictionary<String, String>();

                d.Add(String.Format("{0} - {1}", "MV", this.v.mySession.MySite.MultiPrimary), String.Format("0{0}", this.v.mySession.MySite.MultiPrimary));
                d.Add(String.Format("{0} - {1}", "SV", this.v.mySession.MySite.SinglePrimary), String.Format("1{0}", this.v.mySession.MySite.SinglePrimary));
                this.v.OrderDdlData.LabSiteCodeList = d;
                d = null;
            }

            SetTintLab(TintCode);
        }

        public List<FabricationParameterEntitiy> GetFabricationParemeters(String LabSiteCode)
        {
            var fpr = new FabricationParametersRepository();

            var pl = fpr.GetAllParametersBySiteCode(LabSiteCode);

            return pl;
        }

        public List<LabParameterEntity> GetLabParameters(String LabSiteCode)
        {
            var lpr = new LabParametersRepository();

            var lpl = lpr.GetLabParametersBySiteCode(LabSiteCode);

            return lpl;
        }

        public Boolean CanLabFabricate(String SiteCode)
        {
            // Get fabrication parameters for site.
            //var fabParms = GetFabricationParemeters(SiteCode);

            //// Determine if the material is in the list
            //if (fabParms.Any(x => x.Material == this.v.Order.Material).Equals(false)) return true;

            //// Get parameters by material
            //var matParams = fabParms.Where(x => x.Material == this.v.Order.Material).ToList();

            //// Compare Cyl, Max Plus/Minul(Sph)
            //var rx = this.v.Prescription;

            //// Find Cyl parameter in material parameters
            ////if (matParams.Any(x => x.Cylinder == rx.OdCylinder || x.Cylinder == rx.OsCylinder).Equals(false)) return true;  //OLD
            ////if (matParams.Any(x => x.Cylinder > rx.OdCylinder && x.Cylinder > rx.OsCylinder)) return true;  

            //foreach (var mp in matParams)
            //{
            //    //var lParm = matParams.FirstOrDefault(x => x.Cylinder == rx.OdCylinder);
            //    //var rParm = matParams.FirstOrDefault(x => x.Cylinder == rx.OsCylinder);
              
            //        // Cyl  Max Plus  Max Minus
            //        // When Cyl = (n) then Sph has to be between Max Plus and Max Minus
            //        //return rx.OdSphere.Between(parm.MaxMinus, parm.MaxPlus) && rx.OsSphere.Between(parm.MaxMinus, parm.MaxPlus);
            //        var lGood = false;
            //        var rGood = false;

            //        lGood = GetAndCompareLabParametersToValues(mp, rx.OsSphere, rx.OsCylinder);
            //        rGood = GetAndCompareLabParametersToValues(mp, rx.OdSphere, rx.OdCylinder);

            //        if (lGood.Equals(true) && rGood.Equals(true))
            //            return true;
                
            //}
            return true;

        }

        private Boolean GetAndCompareLabParametersToValues(FabricationParameterEntitiy param, decimal sphere, decimal cylinder)
        {
            return  cylinder.Between(0, param.Cylinder) && sphere.Between(param.MaxMinus, param.MaxPlus) && this.v.Order.LensType.Substring(0,2).Equals(param.CapabilityType);
        }

        

        public bool IsMaterialInStock(String SiteCode)
        {
            var parms = GetFabricationParemeters(SiteCode);
            if (parms.IsNullOrEmpty()) return true;
            if (parms.Any(x => x.Material == this.v.Order.Material).Equals(false)) return true;
            return parms.FirstOrDefault().IsStocked;
        }

        public void GetReOrderReasons()
        {
            var r = new ReOrderReasonRepository();
            this.v.OrderDdlData.ReorderList = r.GetReorderReasons().Cast<ReOrderEntity>().ToDictionary(x => x.RoReason, x => x.ID.ToString());
        }

        public Boolean IsSpecialLab(String FrameCode, out String LabSiteCode)
        {
            LabSiteCode = String.Empty;
            var r = new OrderManagementRepository.OrderRepository();
            var f = String.IsNullOrEmpty(FrameCode) ? this.v.Order.Frame : FrameCode;
            var sc = this.v.mySession.MyClinicCode;
            var l = r.GetSpecialFrameLabs(f, sc);
            if (l == null || l.Count.Equals(0)) return false;
            if (l.Count.Equals(1)) { LabSiteCode = l[0]; return true; }

            var n = l.Where(x => x.ToLower() == this.v.mySession.MySite.SinglePrimary.ToLower() || x.ToLower() == this.v.mySession.MySite.MultiPrimary.ToLower()).FirstOrDefault();
            if (String.IsNullOrEmpty(n))
            {
                LabSiteCode = l[0];
                return true;
            }
            LabSiteCode = n;
            return true;
        }

        public Boolean ValidateOrderForSave()
        {
            var good = true;
            var o = this.v.Order;

            if (!IsSegHtValid(this.v.Order.OdSegHeight)) good = false;
            if (!IsSegHtValid(this.v.Order.OsSegHeight)) good = false;
            if (!IsPairsValid()) good = false;
            if (o.Bridge.Equals("X")) good = false;
            if (o.Color.Equals("X")) good = false;
            if (o.Eye.Equals("X")) good = false;
            if (o.Frame.Equals("X")) good = false;
            if (o.LabSiteCode.Equals("X")) good = false;
            if (o.LensType.Equals("X")) good = false;
            if (o.Material.Equals("X")) good = false;
            if (o.Priority.Equals("X")) good = false;
            if (o.Temple.Equals("X")) good = false;
            if (o.Tint.Equals("X")) good = false;

            return good;
        }

        public Boolean IsSegHtValid(String input)
        {
            var i = default(Int32);
            if (Int32.TryParse(input, out i))
                return i >= 10 && i <= 35;
            else
                return input.ToLower().Equals("3b") || input.ToLower().Equals("4b");
        }

        public Boolean IsPairsValid()
        {
            try
            {
                return v.LookupDataList.Where(x => x.FrameCode == v.Order.Frame).FirstOrDefault().MaxPairs >= Convert.ToInt32(v.Order.Pairs);
            }
            catch { return false; }
        }

        public Boolean RequiresJustification(string JustificationType)
        {
            if (!JustificationType.ToUpper().Equals("F") || !JustificationType.ToUpper().Equals("M")) return false;

            var ret = false;

            switch (JustificationType)
            {
                case "F":
                    ret = RequiresFocJustification();
                    break;

                case "M":
                    ret = RequiresMaterialJustification();
                    break;
            }
            return ret;
        }

        private Byte[] GetBarcode(IGenBarCodes gbc, String OrderNumber)
        {
            var oNum = String.Format("*{0}*", OrderNumber);

            using (Image bp = gbc.GenerateBarCode(oNum))
            {
                if (bp != null)
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        bp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }

            return null;
        }

        private Boolean RequiresFocJustification()
        {
            var f = v.LookupDataList.Where(x => x.FrameCode == v.Order.Frame).FirstOrDefault();
            if (f == null) return false;
            if (!f.IsFoc) return false;
            if (v.Patient.NextFocDate == null || v.Patient.NextFocDate.Value < DateTime.Now) return false;
            return true;
        }

        private Boolean RequiresMaterialJustification()
        {
            if (v.OrderDdlData.MaterialList.Count.Equals(1)) return false;
            return !v.Order.Material.Equals("PLAS");
        }

        private String GetLinkableOrderNumbers(Order e)
        {
            if (String.IsNullOrEmpty(e.LinkedId)) return String.Empty;

            var tList = new List<String>();
            var r = new OrderManagementRepository.OrderRepository();

            var tbl = r.GetLinkedOrders(e.LinkedId);

            if (tbl == null || tbl.Count.Equals(0)) return String.Empty;

            foreach (var s in tbl)
            {
                if (s.Equals(e.OrderNumber))
                {
                    tList.Add(s);
                    continue;
                }

                // Make sure that the linked order number is capable of being edited...
                var stats = this.v.OrderStateHistory.Where(x => x.IsActive == true).FirstOrDefault();
                switch (stats.OrderStatusTypeID)
                {
                    case 1:
                    case 15:

                        tList.Add(s);
                        break;
                }
            }

            if (tList.Count.Equals(0)) return String.Empty;

            var lIdSb = new StringBuilder();

            foreach (var l in tList)
                lIdSb.AppendFormat("{0},", l);

            return lIdSb.ToString().Remove(lIdSb.ToString().Length - 1);
        }

        public void IsPatientAddressActive(Int32 patientId)
        {
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var r = new AddressRepository();
            var addys = r.GetAddressesByIndividualID(patientId, ModifiedBy);

            this.v.PatientHasAddress = addys.Any(x => x.IsActive == true);
        }

        public Boolean SetTintLab(String tint)
        {
            var st = this.v.SpecialLabTintList.Any(x => x.Value == tint);
            if (!st) return false;

            this.v.OrderDdlData.LabSiteCodeList.Clear();
            this.v.OrderDdlData.LabSiteCodeList.Add("MV - MNOST1", "MNOST1");
            return true;
        }

        public void AddRemoveSpecialTints(Boolean Add)
        {
            var tl = this.v.OrderDdlData.TintList;

            this.v.SpecialLabTintList.ToList().ForEach(x =>
            {
                if (Add)
                    tl.Add(x.Key, x.Value);
                else
                    tl.Remove(x.Key);
            });

            tl = tl.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            this.v.OrderDdlData.TintList = tl;
        }

        public void GetOrderPreferences()
        {
            var r = new SitePreferencesRepository.OrderPreferencesRepository();
            this.v.OrderPreferences = r.GetPreferences(this.v.mySession.MySite.SiteCode);
        }

        public void GetFrameItemPreferences()
        {
            var r = new SitePreferencesRepository.FrameItemsPreferencesRepository();
            this.v.FramePreferencesList = r.GetFrameItemPreferences(this.v.mySession.MySite.SiteCode);
        }


        public List<DataRow> IsLabMTP()
        {
            var sitecoderepository = new SiteCodeRepository();
            return sitecoderepository.GetLabMTPStatus(this.v.mySession.MySite.SiteCode);
        }

        public string GetLabForOrder(Order orderparams)
        {
            var r = new OrderManagementRepository.OrderRepository();
            return r.GetLabForOrder(orderparams).ToString();
        }

 

        #region ORDER EMAILS

        public async Task<List<OrderEmail>> GetAllOrderEmailsAsync()
        {
            var r = new OrderManagementRepository.OrderEmailRepository();
            return await Task.Run<List<OrderEmail>>(() => { return r.GetAllOrderEmails(this.v.PatientId); });
        }

        public bool InsertEmailAddress(EMailAddressEntity eae)
        {
            eae.EMailAddress = String.IsNullOrEmpty(eae.EMailAddress.Trim()) ? "MAIL.MAIL@MAIL.MAIL" : eae.EMailAddress;
            eae.IndividualID = this.v.PatientId;
            eae.ModifiedBy = this.v.mySession.ModifiedBy;
            eae.IsActive = true;

            var _emailRepository = new EMailAddressRepository();
            var dt = _emailRepository.InsertEMailAddress(eae);

            if (dt.Count > 0)
                return true;
            else
                return false;
        }

        public bool UpdateEmailAddress(EMailAddressEntity entity)
        {
            var _emailRepository = new EMailAddressRepository();
            entity.ModifiedBy = this.v.mySession.ModifiedBy;
          
            var email = _emailRepository.UpdateEMailAddress(entity);
            
            if (email.Count > 0)
                return true;
            else
                return false;
         }


        #endregion ORDER EMAILS 

        #region LAB STATUS EDIT FOR REDIRECT/REJECT

        public void GetLabListForRedirect()
        {
            var r = new SiteRepository.SiteCodeRepository();
            this.v.RedirectLabList = r.GetSitesByType("LAB");
        }

        public void EditStatusForLab(String LabAction, Boolean CheckInHoldForStock = false)
        {
            var osr = new OrderStateRepository.OrderStatusRepository();
            var ose = new OrderStateEntity();

            ose.DateLastModified = DateTime.Now;
            ose.ModifiedBy = this.v.mySession.ModifiedBy;
            ose.IsActive = true;
            ose.OrderNumber = this.v.Order.OrderNumber;

            var sb = new StringBuilder();

            switch (LabAction)
            {
                case "redirect":
                    ose.OrderStatusTypeID = 4;
                    ose.LabCode = this.v.RedirectLab;
                    sb.AppendFormat("{0} redirected order to {1}.  ", this.v.mySession.MySite.SiteCode, this.v.RedirectLab);
                    break;

                case "reject":
                    ose.OrderStatusTypeID = 3;
                    ose.LabCode = this.v.mySession.MySite.SiteCode;
                    sb.AppendFormat("Order rejected by {0}.  ", this.v.mySession.MySite.SiteCode);
                    break;
                case "hfs":
                    if (!CheckInHoldForStock)
                    {
                        ose.OrderStatusTypeID = 5;
                        ose.LabCode = this.v.mySession.MySite.SiteCode;
                        sb.AppendFormat("Hold: Anticipated stock date is {0}.  ", this.v.HoldForStockDate.ToString());
                    }
                    else
                    {
                        ose.OrderStatusTypeID = 2;
                        ose.LabCode = this.v.mySession.MySite.SiteCode;
                        sb.Append("Lab Received Order from Hold for Stock.");
                    }
                    break;
            }

            if (!CheckInHoldForStock)
                sb.AppendFormat("Justification: {0}", this.v.RejectRedirectJustification);

            ose.StatusComment = sb.ToString();
            osr.InsertPatientOrderState(ose);
        }

        public void GetLabJustificationPreferences()
        {
            var r = new SitePreferencesRepository.LabJustificationRepositoryPreferencesRepository();
            this.v.LabJustificationPreferences = r.GetLabJustifications(this.v.mySession.MySite.SiteCode);
        }
        #endregion LAB STATUS EDIT FOR REDIRECT/REJECT

        #endregion ORDER

        #region EXAM

        public void GetAllExams()
        {
            var r = new OrderManagementRepository.ExamRepository();
            this.v.ExamList = r.GetAllExams(this.v.PatientId, this.v.mySession.ModifiedBy);
        }

        public async Task<List<Exam>> GetAllExamsAsync()
        {
            var r = new OrderManagementRepository.ExamRepository();
            return await Task.Run<List<Exam>>(() => { return r.GetAllExams(this.v.PatientId, this.v.mySession.ModifiedBy); });
        }

        public bool InsertExam()
        {
            var r = new OrderManagementRepository.ExamRepository();
            return r.InsertExam(this.v.Exam, this.v.mySession.ModifiedBy);
        }

        public bool UpdateExam()
        {
            var r = new OrderManagementRepository.ExamRepository();
            return r.UpdateExam(this.v.Exam, this.v.mySession.ModifiedBy);
        }

        #endregion EXAM

        #region ADDRESS OPS

        public void FillPatientAddressData()
        {
            if (this.v.mySession.Patient.Individual.IsNull())
                this.v.mySession.Patient.Individual = new IndividualEntity();
            this.v.mySession.Patient.Individual.ID = this.v.PatientId;

            // Get patient addresses from DB
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var r = new AddressRepository();
            var addy = r.GetAddressesByIndividualID(this.v.PatientId, ModifiedBy);

            this.v.mySession.Patient.Addresses = addy;

            var orderAddress = addy.FirstOrDefault(x => x.IsActive == true);

            this.v.PatientAddress = orderAddress.IsNull() ? new AddressEntity() : orderAddress;
        }

        public void FillPatientAddressDdls()
        {
            // Address DDLs
            this.v.States = v.LookupCache.GetByType(LookupType.StateList.ToString());
            this.v.Countries = v.LookupCache.GetByType(LookupType.CountryList.ToString());
        }

        public Boolean DoSaveAddress(AddressEntity entity)
        {
            var msg = String.Empty;
            var svc = new SharedAddressService(entity, v.mySession);
            return svc.DoSaveAddress(out msg);
        }

  

        #region DELETE AFTER TESTING
        //public Boolean DoSaveAddress(AddressEntity entity)
        //{
        //    // if the address fields are blank then it is to be removed
        //    entity.IsActive = !entity.Address1.IsNull() && !entity.Address2.IsNull() && !entity.City.IsNull() && !entity.ZipCode.IsNull() && !entity.UIC.IsNull();

        //    return UpdateAddress(entity);
        //}

        //private Boolean SaveAddress(AddressEntity ae)
        //{
        //    ae = FillAddressElements(ae);

        //    var _addrRepository = new AddressRepository();

        //    var dt = _addrRepository.InsertAddress(ae);

        //    if (dt.Count > 0)
        //    {
        //        v.mySession.Patient.Addresses.AddRange(dt.ToArray());
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //private Boolean UpdateAddress(AddressEntity addr)
        //{
        //    addr = FillAddressElements(addr);

        //    var _addrRepository = new AddressRepository();
        //    var addressUpdate = _addrRepository.UpdateAddress(addr);
        //    if (!addressUpdate.IsNullOrEmpty())
        //        return true;
        //    else
        //        return false;
        //}

        //private AddressEntity FillAddressElements(AddressEntity addr)
        //{
        //    addr.IndividualID = v.mySession.Patient.Individual.ID;
        //    addr.ModifiedBy = v.mySession.ModifiedBy;
        //    return addr;
        //}
        #endregion

        #endregion ADDRESS OPS


    }

    public class OrderManagementPresenterAjax
    {
        public IEnumerable<Order> GetAllOrders(Int32 PatientId, String UserId)
        {
            var r = new OrderManagementRepository.OrderRepository();
            var s = System.Web.HttpContext.Current.Session["SRTSSession"] as SRTSSession;
            var clinicCode = string.IsNullOrEmpty(s.MyClinicCode) ? Globals.SiteCode : s.MyClinicCode;
            return r.GetAllOrders(PatientId, UserId, clinicCode);
        }

        public Order GetOrderByOrderNumber(String OrderNumber, String UserId)
        {
            var ModifiedBy = string.IsNullOrEmpty(UserId) ? Globals.ModifiedBy : UserId;
            var r = new OrderManagementRepository.OrderRepository();
            var o = r.GetOrderByOrderNumber(OrderNumber, ModifiedBy);
            return o;
        }

        public AddressEntity FillPatientAddressData(Int32 patientId, String modifiedBy)
        {
            var s = System.Web.HttpContext.Current.Session["SRTSSession"] as SRTSSession;
            if (s.Patient.Individual.IsNull())
                s.Patient.Individual = new IndividualEntity();
            s.Patient.Individual.ID = patientId;

            // Get patient addresses from DB
            var ModifiedBy = string.IsNullOrEmpty(modifiedBy) ? Globals.ModifiedBy : modifiedBy;
            var r = new AddressRepository();
            var addy = r.GetAddressesByIndividualID(patientId, ModifiedBy);

            s.Patient.Addresses = addy;

            var orderAddress = addy.FirstOrDefault(x => x.IsActive == true);

            if (orderAddress.IsNull()) return new AddressEntity();

            System.Web.HttpContext.Current.Session["orderAddress"] = orderAddress.Clone() as AddressEntity;
            return orderAddress;
        }

    }
}