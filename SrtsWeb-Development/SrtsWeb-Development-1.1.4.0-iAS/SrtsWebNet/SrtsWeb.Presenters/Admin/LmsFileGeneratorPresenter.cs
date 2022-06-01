using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrtsWeb.Presenters.Admin
{
    public class LmsFileGeneratorPresenter
    {
        private const String DELIM = "|";
        private List<LmsFileEntity> _model;

        private ILmsFileGeneratorView v;

        public LmsFileGeneratorPresenter(ILmsFileGeneratorView v)
        {
            this.v = v;
        }

        public void GetLmsSites()
        {
            var r = new SiteRepository.SiteCodeRepository();
            this.v.LabList = r.GetSitesByType("LAB");
        }

        private ILmsFileGeneratorRepository _repository;

        public void GetLmsFileData()
        {
            String LabSiteCode = this.v.SiteCode;
            Boolean updateStatusToReceived = this.v.MarkComplete;

            var bad = new List<String>();

            _repository = new LmsFileGeneratorRepository();

            try
            {
                _model = this._repository.GetLmsFileData(LabSiteCode).ToList();
                if (_model == null || _model.Count.Equals(0)) return;

                this.v.GoodOrders = BuildFileData(_model, out bad).ToList();

                if (updateStatusToReceived)
                {
                    try
                    {
                        var orders = new List<String>();

                        foreach (var m in _model)
                        {
                            orders.Add(m.OrderNumber);
                        }

                        UpdateOrderStatus(LabSiteCode, orders, true, "Order was checked in to the lab via the LMS Service", String.Format("{0}SYSTEM", LabSiteCode), LmsStatusType.RECEIVED);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                this.v.ErrorMessage = String.Format("ERROR|{0}", ex.Message);
            }
            finally
            {
                _repository = null;
                this.v.BadOrders = bad;
            }
        }

        public Boolean UpdateOrderStatus(string labCode, List<string> orderNumbers, bool isActive, string comment, string modifiedBy, LmsStatusType status)
        {
            if (this._repository == null)
                this._repository = new LmsFileGeneratorRepository();

            var statusId = GetLmsStatusTypeId(status);

            return this._repository.UpdateOrderStatus(labCode, orderNumbers, isActive, comment, modifiedBy, statusId);
        }

        public Boolean UpdateOrderStatus(string labCode, string orderNumber, bool isActive, string comment, string modifiedBy, LmsStatusType status)
        {
            if (this._repository == null)
                this._repository = new LmsFileGeneratorRepository();

            var statusId = GetLmsStatusTypeId(status);

            return this._repository.UpdateOrderStatus(labCode, orderNumber, isActive, comment, modifiedBy, statusId);
        }

        private IEnumerable<String> BuildFileData(List<LmsFileEntity> listIn, out List<String> badOrders)
        {
            var bad = new List<String>();
            var sl = new List<String>();
            foreach (var i in listIn)
            {
                try
                {
                    #region TEMP DATA FIELDS FOR CONVERSIONS/FORMATS

                    var lmsComment = i.FrameCode.Equals("HLF") ? "HALF EYE FRAME" : String.Empty;

                    var odPd = String.Empty;
                    var osPd = String.Empty;
                    var odPdDist = Math.Round(i.ODPDDistant.Value, 1).ToString().Replace(".0", "");
                    var osPdDist = Math.Round(i.OSPDDistant.Value, 1).ToString().Replace(".0", "");
                    var odPdNear = Math.Round(i.ODPDNear.Value, 1).ToString().Replace(".0", "");
                    var osPdNear = Math.Round(i.OSPDNear.Value, 1).ToString().Replace(".0", "");
                    var pdDist = Math.Round(i.PDDistant.Value, 1).ToString().Replace(".0", "");
                    var pdNear = Math.Round(i.PDNear.Value, 1).ToString().Replace(".0", "");

                    if (i.RawLensType.Equals("SVD") || i.RawLensType.Equals("SVN") || i.RawLensType.Equals("SVAL") || i.RawLensType.Equals("HLF"))
                    {
                        if (i.RawLensType.Equals("SVD") || i.RawLensType.Equals("SVAL"))
                        {
                            // If there are Mono measurements, use them
                            if ((!odPdDist.Equals("0") && !osPdDist.Equals("0")) && !odPdDist.Equals(osPdDist))
                            {
                                odPd = String.Format("{0}M", odPdDist);
                                osPd = String.Format("{0}M", osPdDist);
                            }
                            // else use the combined measurements
                            else
                            {
                                odPd = String.Format("{0}B", pdDist);
                                osPd = String.Format("{0}B", pdDist);
                            }
                        }
                        else
                        {
                            // If there are Mono measurements, use them
                            if ((!odPdNear.Equals("0") && !osPdNear.Equals("0")) && !odPdNear.Equals(osPdNear))
                            {
                                odPd = String.Format("{0}M", odPdNear);
                                osPd = String.Format("{0}M", osPdNear);
                            }
                            // else use the combined measurements
                            else
                            {
                                odPd = String.Format("{0}B", pdNear);
                                osPd = String.Format("{0}B", pdNear);
                            }
                        }
                    }
                    else
                    {
                        odPd = String.Format("{0}B {1}", pdDist, pdNear);
                        osPd = String.Format("{0}B {1}", pdDist, pdNear);
                    }

                    var lensStatus = "COMP";

                    var odShpere = String.Empty;
                    var osSphere = String.Empty;
                    var odCylinder = String.Empty;
                    var osCylinder = String.Empty;
                    var odAxis = String.Empty;
                    var osAxis = String.Empty;

                    if (i.ODSphere.Contains("-"))
                        odShpere = String.Format("-{0,5:#0.00}", Double.Parse(i.ODSphere.Substring(1)));
                    else
                        odShpere = String.Format("+{0,5:#0.00}", Double.Parse(i.ODSphere));

                    if (i.OSSphere.Contains("-"))
                        osSphere = String.Format("-{0,5:#0.00}", Double.Parse(i.OSSphere.Substring(1)));
                    else
                        osSphere = String.Format("+{0,5:#0.00}", Double.Parse(i.OSSphere));

                    if (i.ODCylinder.Contains("-"))
                        odCylinder = String.Format("-{0,5:#0.00}", Double.Parse(i.ODCylinder.Substring(1)));
                    else
                        odCylinder = String.Format("+{0,5:#0.00}", Double.Parse(i.ODCylinder));

                    if (i.OSCylinder.Contains("-"))
                        osCylinder = String.Format("-{0,5:#0.00}", Double.Parse(i.OSCylinder.Substring(1)));
                    else
                        osCylinder = String.Format("+{0,5:#0.00}", Double.Parse(i.OSCylinder));

                    odAxis = String.Format("{0,3:000}", i.ODAxis.Value);
                    osAxis = String.Format("{0,3:000}", i.OSAxis.Value);

                    // Add power
                    var odAdd = String.Empty;
                    var osAdd = String.Empty;
                    odAdd = i.ODAdd.Value.Equals(0) ? "" : i.ODAdd.Value.ToString();
                    osAdd = i.OSAdd.Value.Equals(0) ? "" : i.OSAdd.Value.ToString();

                    // Segment Height
                    var odSeg = String.Empty;
                    var osSeg = String.Empty;

                    odSeg = String.IsNullOrEmpty(i.ODSegHeight) || i.ODSegHeight.Equals("0") ? "" :
                        i.ODSegHeight.Substring(i.ODSegHeight.Length - 1, 1).ToLower().Equals("b") ?
                        String.Format("{0}D", i.ODSegHeight.Substring(0, i.ODSegHeight.Length - 1)) : i.ODSegHeight;

                    osSeg = String.IsNullOrEmpty(i.OSSegHeight) || i.OSSegHeight.Equals("0") ? "" :
                        i.OSSegHeight.Substring(i.OSSegHeight.Length - 1, 1).ToLower().Equals("b") ?
                        String.Format("{0}D", i.OSSegHeight.Substring(0, i.OSSegHeight.Length - 1)) : i.OSSegHeight;

                    var odhPrism = i.ODHPrism == null || i.ODHPrism.Value.Equals(0) ? String.Empty : i.ODHPrism.Value.ToString();
                    var odvPrism = i.ODVPrism == null || i.ODVPrism.Value.Equals(0) ? String.Empty : i.ODVPrism.Value.ToString();

                    var oshPrism = i.OSHPrism == null || i.OSHPrism.Value.Equals(0) ? String.Empty : i.OSHPrism.Value.ToString();
                    var osvPrism = i.OSVPrism == null || i.OSVPrism.Value.Equals(0) ? String.Empty : i.OSVPrism.Value.ToString();

                    var odPrism =
                        String.IsNullOrEmpty(odhPrism) ?
                        String.Format("{0}{1}", odvPrism, i.ODVBase) :
                        String.Format("{0}{1} {2}{3}", odhPrism, i.ODHBase, odvPrism, i.ODVBase);

                    var osPrism =
                        String.IsNullOrEmpty(oshPrism) ?
                        String.Format("{0}{1}", osvPrism, i.OSVBase) :
                        String.Format("{0}{1} {2}{3}", oshPrism, i.OSHBase, osvPrism, i.OSVBase);

                    var last4 = i.IndividualID_Patient.ToString().Length >= 4 ?
                        i.IndividualID_Patient.ToString().Substring(i.IndividualID_Patient.ToString().Length - 4, 4) :
                        String.Empty;

                    var templeType =
                        i.FrameTempleType.Substring(i.FrameTempleType.Length - 1, 1).Equals("C") ?
                        String.Format("{0}Z", i.FrameTempleType) :
                        i.FrameTempleType;

                    var provider = i.ProviderName.Length < 14 ? i.ProviderName : i.ProviderName.Substring(0, 14);

                    var procType = String.Empty;
                    switch (i.Tint)
                    {
                        case "AM60":
                        case "AM80":
                        case "BL65":
                        case "BL85":
                        case "FL41":
                        case "PK60":
                        case "PK85":
                        case "N40":
                        case "N15":
                        case "N31":
                        case "N60":
                        case "SL1":
                        case "SL2":
                        case "SL3":
                            procType = "TNT";
                            break;

                        case "AR":
                            procType = "ARC";
                            break;

                        case "UV4":
                            procType = "UVC";
                            break;

                        default:
                            procType = String.Empty;
                            break;
                    }

                    #endregion TEMP DATA FIELDS FOR CONVERSIONS/FORMATS

                    #region DELIMITED STRING DATA

                    var sb = new StringBuilder();
                    try
                    {
                        sb.Append("0");
                        sb.Append(DELIM);
                        sb.Append("REMOTE");
                        sb.Append(AddDelim(3));
                        sb.Append(DateTime.Today.ToString("yyyyMMdd"));
                        sb.Append(AddDelim(2));
                        sb.Append("0100");
                        sb.Append(DELIM);
                        sb.Append("0101");
                        sb.Append(DELIM);
                        sb.Append("1");
                        sb.Append(DELIM);
                        sb.Append("1");
                        sb.Append(DELIM);
                        sb.Append("RX");
                        sb.Append(AddDelim(4));
                        sb.Append(i.ClinicSiteCode.Length >= 4 ?
                            i.ClinicSiteCode.Substring(i.ClinicSiteCode.Length - 4, 4) :
                            String.Empty);
                        sb.Append(DELIM);
                        sb.Append("ALL");
                        sb.Append(AddDelim(3));
                        sb.Append("ALL");
                        sb.Append(AddDelim(2));
                        sb.Append(i.BillingCode);
                        sb.Append(DELIM);
                        sb.Append(i.LastName.Trim());
                        sb.Append(AddDelim(3));
                        sb.Append("SRTS");
                        sb.Append(DELIM);
                        sb.Append("N");
                        sb.Append(DELIM);
                        sb.Append("N");
                        sb.Append(AddDelim(7));
                        sb.Append(i.OrderPriority);
                        sb.Append(AddDelim(4));
                        sb.Append(i.LensMaterial);
                        sb.Append(DELIM);
                        sb.Append(i.LensMaterial);
                        sb.Append(DELIM);
                        sb.Append(i.LensType);
                        sb.Append(DELIM);
                        sb.Append(i.LensType);

                        sb.Append(DELIM);
                        sb.Append(odShpere);

                        sb.Append(DELIM);
                        sb.Append(osSphere);

                        sb.Append(DELIM);
                        sb.Append(odCylinder);

                        sb.Append(DELIM);
                        sb.Append(osCylinder);

                        sb.Append(DELIM);
                        sb.Append(odAxis);
                        sb.Append(DELIM);
                        sb.Append(osAxis);
                        sb.Append(DELIM);
                        sb.Append(odPd);
                        sb.Append(DELIM);
                        sb.Append(osPd);
                        sb.Append(DELIM);
                        sb.Append(lensStatus);
                        sb.Append(DELIM);
                        sb.Append(lensStatus);
                        sb.Append(AddDelim(3));
                        sb.Append(odAdd);
                        sb.Append(DELIM);
                        sb.Append(osAdd);
                        sb.Append(DELIM);
                        sb.Append(odSeg);
                        sb.Append(DELIM);
                        sb.Append(osSeg);
                        sb.Append(AddDelim(5));
                        sb.Append(odPrism);
                        sb.Append(DELIM);
                        sb.Append(osPrism);
                        sb.Append(AddDelim(14));
                        sb.Append("SUPPLY");
                        sb.Append(DELIM);
                        sb.Append(i.FrameCode);
                        sb.Append(AddDelim(2));
                        sb.Append(i.FrameCode);
                        sb.Append(DELIM);
                        sb.Append(i.FrameEyeSize);
                        sb.Append(DELIM);
                        sb.Append(i.FrameBridgeSize);
                        sb.Append(DELIM);
                        sb.Append(i.FrameColor);
                        sb.Append(DELIM);
                        sb.Append(templeType);
                        sb.Append(AddDelim(11));
                        sb.Append(i.Tint);
                        sb.Append(DELIM);
                        sb.Append(i.Tint);
                        sb.Append(AddDelim(3));
                        sb.Append(i.UserComment1.Replace("\n", ""));
                        sb.Append(DELIM);
                        sb.Append(i.UserComment2.Replace("\n", ""));
                        sb.Append(DELIM);
                        sb.Append(i.CorrespondenceEmail);
                        sb.Append(DELIM);
                        sb.Append(i.UserComment3);
                        sb.Append(AddDelim(3));
                        sb.Append(i.ClinicAddress1);
                        sb.Append(DELIM);
                        sb.Append(i.ClinicCity);
                        sb.Append(DELIM);
                        sb.Append(i.ClinicAddress2);
                        sb.Append(DELIM);
                        sb.Append(i.ClinicState);
                        sb.Append(AddDelim(2));
                        sb.Append(i.ClinicZipCode);
                        sb.Append(AddDelim(14));
                        sb.Append(String.Format("{0} {1}", i.LastName, i.FirstName));
                        sb.Append(DELIM);
                        sb.Append(last4);

                        sb.Append(AddDelim(2));
                        sb.Append(i.ShipAddress1);

                        sb.Append(DELIM);
                        sb.Append(i.ShipAddress2);

                        sb.Append(DELIM);
                        sb.Append(i.ShipCity);

                        sb.Append(DELIM);
                        sb.Append(i.ShipState);

                        sb.Append(DELIM);
                        sb.Append(i.ShipZipCode);

                        sb.Append(DELIM);
                        sb.Append(i.PatientPhoneNumber);
                        sb.Append(DELIM);
                        sb.Append(provider);
                        sb.Append(AddDelim(2));
                        sb.Append(i.OrderNumber);

                        sb.Append(AddDelim(26));
                        sb.Append(i.Tint); // Process1

                        sb.Append(DELIM);
                        sb.Append(procType); // ProcessType_1

                        sb.Append(AddDelim(15));
                        sb.Append(lmsComment);
                        sb.Append(DELIM);
                        sb.Append(i.ShipToPatient);
                        sb.Append(DELIM);
                        sb.Append(i.Rank);
                        sb.Append(DELIM);
                        sb.Append(String.Format("{0}{1}", i.BOS, i.Status));
                        sb.Append(DELIM);
                        sb.Append(i.Unit);
                        sb.Append(DELIM);
                        if (!i.LabSiteCode.ToLower().Equals("mnost1"))
                        {
                            sb.Append("--optivision--");
                            sb.Append(String.Format("TECH:{0}~", i.TechInitials));
                            sb.Append(String.Format("RawLensType:{0}~", i.RawLensType));
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        throw ex;
                    }

                    sl.Add(sb.ToString());

                    #endregion DELIMITED STRING DATA
                }
                catch
                {
                    bad.Add(i.OrderNumber);
                }
            }
            badOrders = bad;
            return sl;
        }

        private Int32 GetLmsStatusTypeId(LmsStatusType status)
        {
            var statusId = 0;

            switch (status)
            {
                case LmsStatusType.CANCELLED:
                    statusId = 5;
                    break;

                case LmsStatusType.DISPENSED:
                    statusId = 7;
                    break;

                case LmsStatusType.RECEIVED:
                    statusId = 2;
                    break;

                case LmsStatusType.REDIRECTED:
                    statusId = 4;
                    break;

                case LmsStatusType.REJECTED:
                    statusId = 3;
                    break;

                case LmsStatusType.RETURN_TO_STOCK:
                    statusId = 17;
                    break;
            }

            return statusId;
        }

        private String AddDelim(Int32 numToAdd = 1)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < numToAdd; i++)
                sb.Append(DELIM);

            return sb.ToString();
        }
    }
}