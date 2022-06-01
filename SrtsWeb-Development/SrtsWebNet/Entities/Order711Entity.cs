using System;

namespace SrtsWeb.Entities
{
    public sealed class Order711Entity
    {
        public string SexCode { get; set; }

        public string RankCode { get; set; }

        public string StatusCode { get; set; }

        public string OrderPriority { get; set; }

        public string BOS { get; set; }

        public string TechInitials { get; set; }

        public string OrderNumber { get; set; }

        public string LensType { get; set; }

        public string LensMaterial { get; set; }

        public string Tint { get; set; }

        public decimal ODSegHeight { get; set; }

        public decimal OSSegHeight { get; set; }

        public int NumberOfCases { get; set; }

        public int Pairs { get; set; }

        public string FrameCode { get; set; }

        public string FrameColor { get; set; }

        public string FrameEyeSize { get; set; }

        public string FrameBridgeSize { get; set; }

        public string FrameTempleType { get; set; }

        public decimal ODPDDistant { get; set; }

        public decimal OSPDDistant { get; set; }

        public decimal PDDistant { get; set; }

        public decimal ODPDNear { get; set; }

        public decimal OSPDNear { get; set; }

        public decimal PDNear { get; set; }

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

        public string PatientEmail { get; set; }

        public bool OnholdForConfirmation { get; set; }

        public string ODSphere { get; set; }

        public string OSSphere { get; set; }

        public string ODCylinder { get; set; }

        public string OSCylinder { get; set; }

        public int ODAxis { get; set; }

        public int OSAxis { get; set; }

        public decimal ODHPrism { get; set; }

        public decimal OSHPrism { get; set; }

        public decimal ODVPrism { get; set; }

        public decimal OSVPrism { get; set; }

        public string ODHBase { get; set; }

        public string OSHBase { get; set; }

        public string ODVBase { get; set; }

        public string OSVBase { get; set; }

        public decimal ODAdd { get; set; }

        public decimal OSAdd { get; set; }

        public string ODCorrectedAcuity { get; set; }

        public string ODUncorrectedAcuity { get; set; }

        public string OSCorrectedAcuity { get; set; }

        public string OSUncorrectedAcuity { get; set; }

        public string ODOSCorrectedAcuity { get; set; }

        public string ODOSUncorrectedAcuity { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Doctor { get; set; }

        public DateTime? ExamDate { get; set; }

        public string PatientIDNumber { get; set; }

        public DateTime DateOrderCreated { get; set; }

        public string ClinicName { get; set; }

        public string ClinicAddress1 { get; set; }

        public string ClinicAddress2 { get; set; }

        public string ClinicCity { get; set; }

        public string ClinicCountry { get; set; }

        public string ClinicState { get; set; }

        public string ClinicZipCode { get; set; }

        public string LabName { get; set; }

        public string LabAddress1 { get; set; }

        public string LabAddress2 { get; set; }

        public string LabCity { get; set; }

        public string LabCountry { get; set; }

        public string LabState { get; set; }

        public string LabZipCode { get; set; }

        public string PatientPhoneNumber { get; set; }

        public decimal ODDistantDecenter { get; set; }

        public decimal OSDistanctDecenter { get; set; }

        public decimal ODNearDecenter { get; set; }

        public decimal OSNearDecenter { get; set; }

        public string ODBase { get; set; }

        public string OSBase { get; set; }

        public string BOSDesc { get; set; }

        public string StatDesc { get; set; }

        public string OrdPriDesc { get; set; }

        public byte[] ONBarCode { get; set; }
    }
}