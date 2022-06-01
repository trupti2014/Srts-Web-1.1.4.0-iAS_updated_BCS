using DataToObjectLib;
using System;

namespace SrtsWeb.Entities
{
    public enum LmsStatusType { REJECTED, REDIRECTED, CANCELLED, DISPENSED, RECEIVED, RETURN_TO_STOCK };

    public sealed class LmsFileEntity
    {
        public string OrderNumber { get; set; }

        public string DocumentNumber { get; set; }

        public string IndividualID_Patient { get; set; } // SSN, DODID, ...

        public string Demographic { get; set; }

        public string BillingCode { get; set; }

        public string LensType { get; set; }

        public string RawLensType { get; set; }

        public string LensMaterial { get; set; }

        public string Tint { get; set; }

        public String ODSegHeight { get; set; }

        public String OSSegHeight { get; set; }

        public string FrameCode { get; set; }

        public string FrameColor { get; set; }

        public string FrameEyeSize { get; set; }

        public string FrameBridgeSize { get; set; }

        public string FrameTempleType { get; set; }

        public decimal? ODPDDistant { get; set; }

        public decimal? OSPDDistant { get; set; }

        public decimal? PDDistant { get; set; }

        public decimal? ODPDNear { get; set; }

        public decimal? OSPDNear { get; set; }

        public decimal? PDNear { get; set; }

        public string ClinicSiteCode { get; set; }

        public string ClinicAddress1 { get; set; }

        public string ClinicAddress2 { get; set; }

        public string ClinicCity { get; set; }

        public string ClinicState { get; set; }

        public string ClinicCountry { get; set; }

        public string ClinicZipCode { get; set; }

        public string LabSiteCode { get; set; }

        public string ShipToPatient { get; set; }

        public string ShipAddress1 { get; set; }

        public string ShipAddress2 { get; set; }

        public string ShipCity { get; set; }

        public string ShipState { get; set; }

        public string ShipZipCode { get; set; }

        public string ShipAddressType { get; set; }

        public string LocationCode { get; set; }

        public string UserComment1 { get; set; }

        public string UserComment2 { get; set; }

        public string UserComment3 { get; set; }

        public string UserComment4 { get; set; }

        public string UserComment5 { get; set; }

        public string UserComment6 { get; set; }

        public bool IsGEyes { get; set; }

        [FieldName("PatientEmail")]
        public string CorrespondenceEmail { get; set; }

        public string PatientPhoneNumber { get; set; }

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

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Unit { get; set; }

        public String TechInitials { get; set; }

        [FieldName("Doctor")]
        public String ProviderName { get; set; }

        private Object a;

        public string Rank
        {
            get { return Demographic.Substring(0, 3).Replace("*", ""); }
            set { a = value; }
        }

        public string Status
        {
            get { return Demographic.Substring(4, 2); }
            set { a = value; }
        }

        public string BOS
        {
            get { return Demographic.Substring(3, 1); }
            set { a = value; }
        }

        public string OrderPriority
        {
            //get { return Demographic.Length.Equals(8) ? Demographic.Substring(7, 1) : ""; }
            get;
            set;
        }

        public string Gender
        {
            get { return Demographic.Substring(6, 1); }
            set { a = value; }
        }

        public DateTime OrderDate { get; set; }

        public DateTime ExamDate { get; set; }
    }
}