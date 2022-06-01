using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Views.Orders;
using SrtsWeb.Entities;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.DataLayer.Repositories;
using System.Drawing;

namespace SrtsWeb.BusinessLayer.Presenters.Orders
{
    public class OrderManagementPresenter
    {
        IOrderManagementView v;
        public OrderManagementPresenter(IOrderManagementView view)
        {
            v = view;
        }

        public void GetPatientData()
        {
            var r = new IndividualRepository();
            this.v.Patient = r.GetIndividualByIndividualID(this.v.PatientId, this.v.mySession.MyUserID).FirstOrDefault();
        }

        public void GetProviderList()
        {
            var r = new IndividualRepository();
            this.v.ProviderList = r.GetIndividualBySiteCodeAndPersonalType(this.v.mySession.MyClinicCode, "PROVIDER", this.v.mySession.MyUserID);
            //this.v.ProviderList = pl.Select(x => new { Key = x.NameLFMi, Value = x.ID }).ToDictionary(x => x.Key, x => x.Value);
            //pl = null;
        }

        #region PRESCRIPTION
        public void GetAllPrescriptions()
        {
            var r = new OrderManagementRepository.PrescriptionRepository();
            v.PrescriptionList = r.GetAllPrescriptions(v.PatientId, this.v.mySession.MyUserID);
        }
        public bool DeletePrescription()
        {
            var pe = this.v.Prescription;
            if (pe.IsNull()) return false;

            var r = new OrderManagementRepository.PrescriptionRepository();

            return r.DeletePrescription(pe.PrescriptionId, this.v.mySession.MyUserID);
        }
        public bool UpdatePrescription()
        {
            var pe = this.v.Prescription;

            pe.OdAxisCalc = GroomCalcFieldForDb(pe.OdAxisCalc);
            pe.OdCylinderCalc = GroomCalcFieldForDb(pe.OdCylinderCalc);
            pe.OdSphereCalc = GroomCalcFieldForDb(pe.OdSphereCalc);
            pe.OsAxisCalc = GroomCalcFieldForDb(pe.OsAxisCalc);
            pe.OsCylinderCalc = GroomCalcFieldForDb(pe.OsCylinderCalc);
            pe.OsSphereCalc = GroomCalcFieldForDb(pe.OsSphereCalc);

            pe.ExamId = string.IsNullOrEmpty(this.v.mySession.SelectedExamID.ToString()) ? 0 : this.v.mySession.SelectedExamID;

            var r = new OrderManagementRepository.PrescriptionRepository();
            return r.UpdatePrescription(pe, v.mySession.MyUserID);
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
            return r.InsertPrescription(pe, v.mySession.MyUserID, out rxId);
        }
        private String GroomCalcFieldForDb(String calcField)
        {
            var d = default(Decimal);
            if (Decimal.TryParse(calcField, out d))
                return d.ToString();
            return default(Decimal).ToString();
        }
        #endregion

        #region ORDER
        public void GetAllOrders()
        {
            var r = new OrderManagementRepository.OrderRepository();
            v.OrderList = r.GetAllOrders(this.v.PatientId, this.v.mySession.MyUserID);
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

            return r.DeleteOrder(o.OrderNumber, this.v.mySession.MyUserID);
        }
        public bool ReclaimOrder()
        {
            // Set order record to "inactive"
            bool isGood = true;
            var o = this.v.Order;
            o.IsActive = false;
            o.LabSiteCode = String.IsNullOrEmpty(this.v.Order.LabSiteCode) || this.v.Order.LabSiteCode.Equals("X") ? this.v.mySession.MySite.MultiPrimary : this.v.Order.LabSiteCode;
            var r = new OrderManagementRepository.OrderRepository();
            if (!r.UpdateOrder(o, this.v.mySession.MyUserID)) isGood = false;

            var osr = new OrderStateRepository.OrderStatusRepository();

            // Modify the status record, change the StatusComment to reflect Clinic deleting the order
            var ose = new OrderStateEntity();

            ose.OrderNumber = o.OrderNumber;
            ose.OrderStatusTypeID = 17;
            ose.StatusComment = "Order Reclaimed by Clinic";
            ose.LabCode = String.IsNullOrEmpty(this.v.Order.LabSiteCode) || this.v.Order.LabSiteCode.Equals("X") ? this.v.mySession.MySite.MultiPrimary : this.v.Order.LabSiteCode;
            ose.IsActive = true;
            ose.ModifiedBy = this.v.mySession.MyUserID;

            // Update the orders' status record in db
            osr.InsertPatientOrderState(ose);

            return isGood;
        }
        public bool UpdateOrder()
        {
            bool isGood = true;
            var o = this.v.Order;
            var extraOrderNumbers = new List<String>();
            var r = new OrderManagementRepository.OrderRepository();
            var tmpPairs = 0;
            //extraOrderNumbers.Add(o.OrderNumber);

            if (o.Pairs > 1)
            {
                //extraOrderNumbers.AddRange(r.GetNextOrderNumbers(o.ClinicSiteCode, o.Pairs - 1));
                o.LinkedId = o.OrderNumber;
                tmpPairs = o.Pairs;
                o.Pairs = 1;
            }

            // Update Order(s) to the database
            var sb = new StringBuilder(o.Comment1);
            if (!String.IsNullOrEmpty(o.FocJustification))
            {
                sb.AppendFormat("FOC:{0}", o.FocJustification);
                o.Comment1 = sb.ToString();
            }

            sb.Clear();

            if (!String.IsNullOrEmpty(o.MaterialJustification))
            {
                sb.Append(o.Comment2);
                sb.AppendFormat("MAT:{0}", o.MaterialJustification);
                o.Comment2 = sb.ToString();
            }

            // Add a barcode to the order
            if (o.OrderNumberBarCode == null || o.OrderNumberBarCode.Length.Equals(0))
                o.OrderNumberBarCode = GetBarcode(o.OrderNumber);

            if (!r.UpdateOrder(o, this.v.mySession.MyUserID)) isGood = false;

            // Update the order status
            InsertOrderStatus();

            if (tmpPairs.Equals(0)) return isGood;

            extraOrderNumbers.AddRange(r.GetNextOrderNumbers(o.ClinicSiteCode, o.Pairs - 1));
            var i = 0;
            do
            {
                o.OrderNumber = extraOrderNumbers[i];
                o.Pairs = 1;
                o.OrderNumberBarCode = GetBarcode(o.OrderNumber);
                r.InsertOrder(o, this.v.mySession.MyUserID);
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

            var sb = new StringBuilder(o.Comment1);
            if (!String.IsNullOrEmpty(o.FocJustification))
            {
                sb.AppendFormat("FOC:{0}", o.FocJustification);
                o.Comment1 = sb.ToString();
            }

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
                o.OrderNumberBarCode = GetBarcode(o.OrderNumber);
                if (!r.InsertOrder(o, this.v.mySession.MyUserID)) isGood = false;

                i++;
            } while (i < extraOrderNumbers.Count);

            return isGood;
        }
        public void InsertOrderStatus()
        {
            var lId = String.Empty;
            var cmt = String.Empty;
            var stat = default(Int32);
            var active = false;

            // Get current active status
            var os = 0;
            //var osh = GetOrderStatusHistory(this.v.Order.OrderNumber);
            //os = osh.Where(x => x.IsActive == true).FirstOrDefault().OrderStatusTypeID; //this.v.OrderStateHistory.Where(x => x.IsActive == true).FirstOrDefault().OrderStatusTypeID;
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
                case 17: // Reclaimed
                    {
                        cmt = "Order was never dispensed to customer.  Setting status to reclaimed.";
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
            ose.LabCode = this.v.Order.LabSiteCode;
            ose.ModifiedBy = this.v.mySession.MyUserID;
            ose.OrderStatusTypeID = stat;
            ose.IsActive = active;

            rr.InsertPatientOrderState(ose);
        }

        public void GetPriorities()
        {
            var h = new DemographicXMLHelper();
            var demo = v.Demographic;
            var p = h.GetOrderPrioritiesByBOSStatusAndRank(demo.ToBOSKey(), demo.ToPatientStatusKey(), demo.ToRankKey()).
                Select(x => new { key = x.OrderPriorityText, value = x.OrderPriorityValue }).
                ToDictionary(x => x.key, x => x.value);
            if (p == null) return;
            var l = new OrderDropDownData();
            l.PriorityList = p;
            v.OrderDdlData = l;
        }
        public void GetFrames(string Demographic, string SiteCode)
        {
            var r = new OrderManagementRepository.FrameRepository();

            var l = r.GetFrameData(Demographic, SiteCode);

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

        }
        public void GetLabs(String FrameCode = "")
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
        }
        public Boolean IsSpecialLab(String FrameCode, out String LabSiteCode)
        {
            LabSiteCode = String.Empty;
            var r = new OrderManagementRepository.OrderRepository();
            var f = String.IsNullOrEmpty(FrameCode) ? this.v.Order.Frame : FrameCode;
            var l = r.GetSpecialFrameLabs(f);
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
        private Byte[] GetBarcode(String OrderNumber)
        {
            var oNum = String.Format("*{0}*", OrderNumber);

            var gbc = new GenerateBarCodes();
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
            //if (v.Order.NextFocDate < DateTime.Now) return false;
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
                //var stats = GetOrderStatusHistory(s).Where(x => x.IsActive == true).FirstOrDefault();
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
            var r = new AddressRepository();
            var addys = r.GetAddressesByIndividualID(patientId, this.v.mySession.MyUserID);

            this.v.PatientHasAddress = addys.Any(x => x.IsActive == true);
        }

        #region LAB STATUS EDIT FOR REDIRECT/REJECT
        public void GetLabListForRedirect()
        {
            var r = new SiteRepository.SiteCodeRepository();
            this.v.RedirectLabList = r.GetSitesByType("LAB");

        }
        public void EditStatusForRedirectOrReject(Boolean IsRedirect)
        {
            var osr = new OrderStateRepository.OrderStatusRepository();
            var ose = new OrderStateEntity();

            ose.DateLastModified = DateTime.Now;
            ose.ModifiedBy = this.v.mySession.MyUserID;
            ose.IsActive = true;
            ose.OrderNumber = this.v.Order.OrderNumber;

            var sb = new StringBuilder();
            if (IsRedirect)
                sb.AppendFormat("{0} redirected order to {1}.  ", this.v.mySession.MySite.SiteCode, this.v.RedirectLab);
            else
                sb.AppendFormat("Order rejected by {0}.  ", this.v.mySession.MySite.SiteCode);
            sb.AppendFormat("Justification: {0}", this.v.RejectRedirectJustification);
            ose.StatusComment = sb.ToString();

            switch (IsRedirect)
            {
                case true:
                    ose.OrderStatusTypeID = 4;
                    ose.LabCode = this.v.RedirectLab;
                    break;

                case false:
                    ose.OrderStatusTypeID = 3;
                    ose.LabCode = this.v.mySession.MySite.SiteCode;
                    break;
            }

            osr.InsertPatientOrderState(ose);
        }
        #endregion
        #endregion

        #region EXAM
        public void GetAllExams()
        {
            var r = new OrderManagementRepository.ExamRepository();
            this.v.ExamList = r.GetAllExams(this.v.PatientId, this.v.mySession.MyUserID);
        }
        //public void GetExamDropDownData()
        //{
        //    var r = new IndividualRepository();
        //    this.v.ExamDdlData.DoctorsData = Helpers.ProcessIndividualTable(r.GetIndividualBySiteCodeAndPersonalType(this.v.mySession.MyClinicCode, "PROVIDER", this.v.mySession.MyUserID)).
        //        Select(x => new { Key = x.NameLFMi, Value = x.ID }).ToDictionary(x => x.Key, x => x.Value);
        //    r = null;
        //}
        public bool InsertExam()
        {
            var r = new OrderManagementRepository.ExamRepository();
            return r.InsertExam(this.v.Exam, this.v.mySession.MyUserID);
        }
        public bool UpdateExam()
        {
            var r = new OrderManagementRepository.ExamRepository();
            return r.UpdateExam(this.v.Exam, this.v.mySession.MyUserID);
        }
        #endregion
    }
}
