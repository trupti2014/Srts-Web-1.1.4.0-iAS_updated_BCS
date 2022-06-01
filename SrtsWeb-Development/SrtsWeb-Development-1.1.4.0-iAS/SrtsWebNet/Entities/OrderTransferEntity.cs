using System;

namespace SrtsWeb.Entities
{
    public sealed class OrderTransferEntity
    {
        public string OrderNumber { get; set; }

        public int IndividualID_Tech { get; set; }

        public string Demographic { get; set; }

        public string LensType { get; set; }

        public string LensMaterial { get; set; }

        public string Tint { get; set; }

        public decimal? ODSegHeight { get; set; }

        public decimal? OSSegHeight { get; set; }

        public int NumberOfCases { get; set; }

        public int Pairs { get; set; }

        public string FrameCode { get; set; }

        public string FrameColor { get; set; }

        public string FrameEyeSize { get; set; }

        public string FrameBridgeSize { get; set; }

        public string FrameTempleType { get; set; }

        public int? ODPDDistant { get; set; }

        public int? OSPDDistant { get; set; }

        public int? PDDistant { get; set; }

        public int? ODPDNear { get; set; }

        public int? OSPDNear { get; set; }

        public int? PDNear { get; set; }

        public string ClinicSiteCode { get; set; }

        public string LabSiteCode { get; set; }

        public bool ShipToPatient { get; set; }

        public string ShipAddress1 { get; set; }

        public string ShipAddress2 { get; set; }

        public string ShipCity { get; set; }

        public string ShipState { get; set; }

        public string ShipZipCode { get; set; }

        public string ShipAddressType { get; set; }

        public string LocationCode { get; set; }

        public string UserComment1 { get; set; }

        public string UserComment2 { get; set; }

        public bool IsGEyes { get; set; }

        public bool IsMultivision { get; set; }

        public int VerifiedBy { get; set; }

        public string CorrespondenceEmail { get; set; }

        public bool OnholdForConfirmation { get; set; }

        public string ODSphere { get; set; }

        public string OSSphere { get; set; }

        public string ODCylinder { get; set; }

        public string OSCylinder { get; set; }

        public int? ODAxis { get; set; }

        public int? OSAxis { get; set; }

        public decimal? ODHPrism { get; set; }

        public decimal? OSHPrism { get; set; }

        public decimal? ODVPrism { get; set; }

        public decimal? OSVPrism { get; set; }

        public string ODHBase { get; set; }

        public string OSHBase { get; set; }

        public string ODVBase { get; set; }

        public string OSVBase { get; set; }

        public decimal? ODAdd { get; set; }

        public decimal? OSAdd { get; set; }

        public string ODCorrectedAcuity { get; set; }

        public string ODUncorrectedAcuity { get; set; }

        public string OSCorrectedAcuity { get; set; }

        public string OSUncorrectedAcuity { get; set; }

        public string ODOSCorrectedAcuity { get; set; }

        public string ODOSUncorrectedAcuity { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string TechInitials { get; set; }

        public string Rank
        {
            get { return Demographic.Substring(0, 3).Replace("*", ""); }
        }

        public string Status
        {
            get { return Demographic.Substring(3, 2); }
        }

        public string BOS
        {
            get { return Demographic.Substring(5, 1); }
        }

        public string OrderPriority
        {
            get { return Demographic.Substring(6, 1); }
        }

        public string Gender
        {
            get { return Demographic.Substring(7, 1); }
        }

        public DateTime OrderDate { get; set; }

        public DateTime ExamDate { get; set; }
    }
}