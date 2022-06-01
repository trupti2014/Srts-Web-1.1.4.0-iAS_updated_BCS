using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrtsWeb.BusinessLayer.Concrete
{
    public sealed class NostraService
    {
        private const String DELIM = "|";
        private const String NOSTRA_SITE = "MNOST1";
        private List<NostraFileEntity> _model;
        private INostraRepository _repository;

        public IEnumerable<String> GetNostraFileData()
        {
            IEnumerable<String> fileData = new List<String>();
            _repository = new NostraRepository();
            var s = new SrtsHelperObject();

            try
            {
                var dt = this._repository.GetNostraFileData(NOSTRA_SITE);
                if (dt == null || dt.Rows.Count.Equals(0)) return null;

                _model = s.ProcessNostraFileDataTable(dt);

                if (_model == null || _model.Count.Equals(0)) return null;

                fileData = BuildFileData(_model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            finally
            {
                s = null;
                _repository = null;
            }

            return fileData;
        }

        private IEnumerable<String> BuildFileData(List<NostraFileEntity> listIn)
        {
            var sl = new List<String>();

            foreach (var i in listIn)
            {
                #region TEMP DATA FIELDS FOR CONVERSIONS/FORMATS

                var lmsComment = i.FrameCode.Equals("HLF") ? "HALF EYE FRAME" : String.Empty;

                var priority = String.Empty;
                switch (i.OrderPriority)
                {
                    case "F":
                        priority = "FOC";
                        break;

                    case "P":
                        priority = "DWN PILOT";
                        break;

                    case "R":
                        priority = "READINESS";
                        break;

                    case "T":
                        priority = "TRAINEE";
                        break;

                    case "V":
                        priority = "VIP";
                        break;

                    case "W":
                        priority = "W";
                        break;

                    default:
                        priority = "";
                        break;
                }

                var shipToPatient = i.ShipToPatient.Equals(true) ? "Y" : "N";

                var pupillaryDist = String.Empty;
                if (i.LensType.Equals("SVD") || i.LensType.Equals("SVN") || i.LensType.Equals("SVAL") || i.LensType.Equals("HLF"))
                {
                    if (i.LensType.Equals("SVD"))
                        pupillaryDist = i.PDDistant.Equals(null) ? String.Empty : i.PDDistant.Value.ToString();
                    else
                        pupillaryDist = i.PDNear.Equals(null) ? String.Empty : i.PDNear.Value.ToString();
                }
                else
                    pupillaryDist = String.Format("{0}", i.PDDistant == null || i.PDNear == null ?
                        String.Empty : String.Format("{0}B {1}", i.PDDistant.Value.ToString(), i.PDNear.Value.ToString()));

                var lensStatus = "COMP";
                var odhPrism = i.ODHPrism == null || i.ODHPrism.Value.Equals(0.00) ? String.Empty : i.ODHPrism.Value.ToString();
                var odvPrism = i.ODVPrism == null || i.ODVPrism.Value.Equals(0.00) ? String.Empty : i.ODVPrism.Value.ToString();

                var oshPrism = i.OSHPrism == null || i.OSHPrism.Value.Equals(0.00) ? String.Empty : i.OSHPrism.Value.ToString();
                var osvPrism = i.OSVPrism == null || i.OSVPrism.Value.Equals(0.00) ? String.Empty : i.OSVPrism.Value.ToString();

                var odPrism =
                    String.IsNullOrEmpty(odhPrism) ?
                    String.Format("{0}{1}", odvPrism, i.ODVBase) :
                    String.Format("{0}{1} {2}{3}", odhPrism, i.ODHBase, odvPrism, i.ODVBase);

                var osPrism =
                    String.IsNullOrEmpty(oshPrism) ?
                    String.Format("{0}{1}", osvPrism, i.OSVBase) :
                    String.Format("{0}{1} {2}{3}", oshPrism, i.OSHBase, osvPrism, i.OSVBase);

                var odSegHt = i.ODSegHeight != null && i.ODSegHeight.Value.Equals(-3) ? "3D" : i.ODSegHeight.Value.ToString();
                var osSegHt = i.OSSegHeight != null && i.OSSegHeight.Value.Equals(-3) ? "3D" : i.OSSegHeight.Value.ToString();

                var last4 = i.IndividualID_Patient.ToString().Length >= 4 ?
                    i.IndividualID_Patient.ToString().Substring(i.IndividualID_Patient.ToString().Length - 4, 4) :
                    String.Empty;
                var billingCode = "NEED THIS";

                var templeType =
                    i.FrameTempleType.Substring(i.FrameTempleType.Length - 1, 1).Equals("C") ?
                    String.Format("{0}Z", i.FrameTempleType) :
                    i.FrameTempleType;

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
                    sb.Append(billingCode);
                    sb.Append(DELIM);
                    sb.Append(i.LastName.Trim());
                    sb.Append(AddDelim(3));
                    sb.Append("SRTS");
                    sb.Append(DELIM);
                    sb.Append("N");
                    sb.Append(DELIM);
                    sb.Append("N");
                    sb.Append(AddDelim(7));
                    sb.Append(priority);
                    sb.Append(AddDelim(4));
                    sb.Append(i.LensMaterial);
                    sb.Append(DELIM);
                    sb.Append(i.LensMaterial);
                    sb.Append(DELIM);
                    sb.Append(i.LensType);
                    sb.Append(DELIM);
                    sb.Append(i.LensType);
                    sb.Append(DELIM);
                    sb.Append(i.ODSphere);
                    sb.Append(DELIM);
                    sb.Append(i.OSSphere);
                    sb.Append(DELIM);
                    sb.Append(i.ODCylinder);
                    sb.Append(DELIM);
                    sb.Append(i.OSCylinder);
                    sb.Append(DELIM);
                    sb.Append(i.ODAxis.Value.ToString().Trim());
                    sb.Append(DELIM);
                    sb.Append(i.OSAxis.Value.ToString().Trim());
                    sb.Append(DELIM);
                    sb.Append(pupillaryDist);
                    sb.Append(DELIM);
                    sb.Append(pupillaryDist);
                    sb.Append(DELIM);
                    sb.Append(lensStatus);
                    sb.Append(DELIM);
                    sb.Append(lensStatus);
                    sb.Append(AddDelim(3));
                    sb.Append(i.ODAdd.Value.ToString().Trim());
                    sb.Append(DELIM);
                    sb.Append(i.OSAdd.Value.ToString().Trim());
                    sb.Append(DELIM);
                    sb.Append(odSegHt.Trim());
                    sb.Append(DELIM);
                    sb.Append(osSegHt.Trim());
                    sb.Append(AddDelim(5));
                    sb.Append(odPrism);
                    sb.Append(DELIM);
                    sb.Append(osPrism);
                    sb.Append(AddDelim(14));
                    sb.Append("SUPPLY");
                    sb.Append(DELIM);
                    sb.Append(i.FrameCode);
                    sb.Append(DELIM);
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
                    sb.Append(String.IsNullOrEmpty(i.CorrespondenceEmail) ?
                        i.UserComment1 :
                        i.CorrespondenceEmail);
                    sb.Append(DELIM);

                    sb.Append(i.IsGEyes ? i.UserComment6 : i.UserComment2);
                    sb.Append(DELIM);
                    sb.Append(i.UserComment3);
                    sb.Append(DELIM);
                    sb.Append(i.UserComment4);
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
                    sb.Append(AddDelim(3));
                    sb.Append(i.OrderNumber);

                    sb.Append(AddDelim(26));
                    sb.Append("");

                    sb.Append(DELIM);
                    sb.Append("");

                    sb.Append(AddDelim(15));
                    sb.Append(lmsComment);
                    sb.Append(DELIM);
                    sb.Append(shipToPatient);
                    sb.Append(DELIM);
                    sb.Append(i.Rank);
                    sb.Append(DELIM);
                    sb.Append(i.Status);
                    sb.Append(DELIM);
                    sb.Append(i.Unit);
                    sb.Append(DELIM);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                sl.Add(sb.ToString());

                #endregion DELIMITED STRING DATA
            }
            return sl;
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