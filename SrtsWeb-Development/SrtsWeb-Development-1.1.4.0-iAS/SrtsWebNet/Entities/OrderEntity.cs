using System;
using System.Collections.Generic;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class OrderEntity
    {
        public DateTime DateLastModified { get; set; }

        public DateTime? FocDate { get; set; }

        public Byte[] ONBarCode { get; set; }

        //public string GroupName { get; set; }

        public static List<Tuple<String, String>> CustFrameToLabColl
        {
            get
            {
                return new List<Tuple<String, String>>()
                {
                    Tuple.Create<String, String>("ALEP1", "MJACK1"),
                    Tuple.Create<String, String>("ALEP2", "MJACK1"),
                    Tuple.Create<String, String>("5AM", "MJACK1"),
                    Tuple.Create<String, String>("5AM", "MBAMC1"),
                    Tuple.Create<String, String>("5AM", "MNOST1"),
                    Tuple.Create<String, String>("5AM50", "MJACK1"),
                    Tuple.Create<String, String>("5AM50", "MBAMC1"),
                    Tuple.Create<String, String>("5AM50", "MNOST1"),
                    Tuple.Create<String, String>("5AM52", "MJACK1"),
                    Tuple.Create<String, String>("5AM52", "MBAMC1"),
                    Tuple.Create<String, String>("5AM52", "MNOST1"),
                    Tuple.Create<String, String>("5AM54", "MJACK1"),
                    Tuple.Create<String, String>("5AM54", "MBAMC1"),
                    Tuple.Create<String, String>("5AM54", "MNOST1")
                };
            }
        }

        public String ODSegHeight { get; set; }

        public String OSSegHeight { get; set; }

        #region IntDecimal

        public int NumberOfCases { get; set; }

        public int Pairs { get; set; }

        public int PrescriptionID { get; set; }

        public int IndividualID_Patient { get; set; }

        public int IndividualID_Tech { get; set; }

        public int PatientPhoneID { get; set; }

        public int VerifiedBy { get; set; }

        #endregion IntDecimal

        #region Strings

        public string OrderNumber { get; set; }

        public string Demographic { get; set; }

        public string LensType { get; set; }

        public string LensTint { get; set; }   // Added for G-Eyes DB 02 Dec 2013

        public string LensCoating { get; set; } // Added for G-Eyes DB 01 Jan 2018

        public string LensTypeLong { get; set; } // Added for G-Eyes DB 02 Dec 2013

        public string LensMaterial { get; set; }

        public string Tint { get; set; }

        public string FrameCode { get; set; }

        public string FrameColor { get; set; }

        public string FrameCategory { get; set; } // Added for JSpecs 

        public string FrameFamily { get; set; } // Added for JSpecs



        public string FrameEyeSize { get; set; }

        public string FrameBridgeSize { get; set; }

        public string FrameTempleType { get; set; }

        public string FrameDescription { get; set; }  // Added for G-Eyes DB 02 Dec 2013

        public string ClinicSiteCode { get; set; }

        public string LabSiteCode { get; set; }

        public string ShipAddress1 { get; set; }

        public string ShipAddress2 { get; set; }

        public string ShipAddress3 { get; set; }

        public string ShipCity { get; set; }

        public string ShipState { get; set; }

        public string ShipZipCode { get; set; }

        public string ShipCountry { get; set; }

        public string ShipAddressType { get; set; }

        public string LocationCode { get; set; }

        public string UserComment1 { get; set; }

        public string UserComment2 { get; set; }

        public string CorrespondenceEmail { get; set; }

        public string ModifiedBy { get; set; }

        public string StringDate
        {
            get { return DateLastModified.ToShortDateString(); }
        }

        public string LinkedID { get; set; }

        public string OrderDisbursement { get; set; }
        #endregion Strings

        #region Bools

        //public bool ShipToPatient { get; set; }

        public bool IsGEyes { get; set; }

        public bool IsActive { get; set; }

        public bool IsMultivision { get; set; }

        public bool IsComplete { get; set; }

        public bool IsEmailPatient { get; set; }

        public bool OnholdForConfirmation { get; set; }

        public bool MedProsDispense { get; set; }

        public bool PimrsDispense { get; set; }

        #endregion Bools
    }
}