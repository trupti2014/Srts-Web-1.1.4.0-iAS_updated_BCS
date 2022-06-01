using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SrtsWeb.Entities
{
    [XmlRoot(ElementName = "orders")]
    public sealed class BulkOrderEntityList
    {
        public BulkOrderEntityList()
        {
            Orders = new List<BulkOrderEntity>();
        }

        [XmlElement("order")]
        public List<BulkOrderEntity> Orders { get; set; }
    }

    [XmlRoot(ElementName = "order", IsNullable = true)]
    public sealed class BulkOrderEntity
    {
        [XmlElement("sexcode")]
        public String SexCode { get; set; }

        [XmlElement("rankcode")]
        public String RankCode { get; set; }

        [XmlElement("statuscode")]
        public String StatusCode { get; set; }

        [XmlElement("orderpriority")]
        public String OrderPriority { get; set; }

        [XmlElement("bos")]
        public String BranchOfService { get; set; }

        [XmlElement("techid")]
        public String TechId { get; set; }

        [XmlElement("techidtype")]
        public String TechIdType { get; set; }

        [XmlElement("lenstype")]
        public String LensType { get; set; }

        [XmlElement("lensmaterial")]
        public String LensMaterial { get; set; }

        [XmlElement("tint")]
        public String Tint { get; set; }

        [XmlElement("odsegheight")]
        public String OdSegHeight { get; set; }

        [XmlElement("ossegheight")]
        public String OsSegHeight { get; set; }

        [XmlElement("numberofcases")]
        public Int32 NumberOfCases { get; set; }

        [XmlElement("numberofpairs")]
        public Int32 NumberOfPairs { get; set; }

        [XmlElement("framecode")]
        public String FrameCode { get; set; }

        [XmlElement("framecolor")]
        public String FrameColor { get; set; }

        [XmlElement("framebridgesize")]
        public Int32 FrameBridgeSize { get; set; }

        [XmlElement("frameeyesize")]
        public Int32 FrameEyeSize { get; set; }

        [XmlElement("frametempletype")]
        public String FrameTempleType { get; set; }

        [XmlElement("odpddistant")]
        public Double OdPdDistant { get; set; }

        [XmlElement("ospddistant")]
        public Double OsPdDistant { get; set; }

        [XmlElement("odpdnear")]
        public Double OdPdNear { get; set; }

        [XmlElement("ospdnear")]
        public Double OsPdNear { get; set; }

        [XmlElement("clinicsitecode")]
        public String ClinicSiteCode { get; set; }

        [XmlElement("unitaddress1")]
        public String UnitAddress1 { get; set; }

        [XmlElement("unitaddress2")]
        public String UnitAddress2 { get; set; }

        [XmlElement("unitaddress3")]
        public String UnitAddress3 { get; set; }

        [XmlElement("uic")]
        public String UIC { get; set; }

        [XmlElement("locationcode")]
        public String LocationCode { get; set; }

        [XmlElement("usercomment1")]
        public String UserComment1 { get; set; }

        [XmlElement("usercomment2")]
        public String UserComment2 { get; set; }

        [XmlElement("verifiedby")]
        public Int32 VerifiedBy { get; set; }

        [XmlElement("patientemail")]
        public String PatientEmail { get; set; }

        [XmlElement("patientemailtype")]
        public String PatientEmailType { get; set; }

        [XmlElement("dateordercreated")]
        public DateTime DateOrderCreated { get; set; }

        [XmlElement("odbase")]
        public String OdBase { get; set; }

        [XmlElement("osbase")]
        public String OsBase { get; set; }

        [XmlElement("odadd")]
        public Double OdAdd { get; set; }

        [XmlElement("odaxis")]
        public Int32 OdAxis { get; set; }

        [XmlElement("odcylinder")]
        public Double OdCylinder { get; set; }

        [XmlElement("odhbase")]
        public String OdHBase { get; set; }

        [XmlElement("odhprism")]
        public Double OdHPrism { get; set; }

        [XmlElement("odsphere")]
        public Double OdSphere { get; set; }

        [XmlElement("odvbase")]
        public String OdVBase { get; set; }

        [XmlElement("odvprism")]
        public Double OdVPrism { get; set; }

        [XmlElement("osadd")]
        public Double OsAdd { get; set; }

        [XmlElement("osaxis")]
        public Int32 OsAxis { get; set; }

        [XmlElement("oscylinder")]
        public Double OsCylinder { get; set; }

        [XmlElement("oshbase")]
        public String OsHBase { get; set; }

        [XmlElement("oshprism")]
        public Double OsHPrism { get; set; }

        [XmlElement("ossphere")]
        public Double OsSphere { get; set; }

        [XmlElement("osvbase")]
        public String OsVBase { get; set; }

        [XmlElement("osvprism")]
        public Double OsVPrism { get; set; }

        [XmlElement("odcorrectedacuity")]
        public String OdCorrectedAcuity { get; set; }

        [XmlElement("odoscorrectedacuity")]
        public String OdOsCorrectedAcuity { get; set; }

        [XmlElement("odosuncorrectedacuity")]
        public String OdOsUncorrectedAcuity { get; set; }

        [XmlElement("oduncorrectedacuity")]
        public String OdUncorrectedAcuity { get; set; }

        [XmlElement("oscorrectedacuity")]
        public String OsCorrectedAcuity { get; set; }

        [XmlElement("osuncorrectedacuity")]
        public String OsUncorrectedAcuity { get; set; }

        [XmlElement("examdate")]
        public String ExamDate { get; set; }

        [XmlElement("exmainerid")]
        public String ExaminerId { get; set; }

        [XmlElement("examineridtype")]
        public String ExaminerIdType { get; set; }

        [XmlElement("examcomment")]
        public String ExamComment { get; set; }

        [XmlElement("patientareacode")]
        public String PatientAreaCode { get; set; }

        [XmlElement("patientdob")]
        public DateTime PatientDob { get; set; }

        [XmlElement("firstname")]
        public String PatientFirstName { get; set; }

        [XmlElement("middlename")]
        public String PatientMiddleName { get; set; }

        [XmlElement("lastname")]
        public String PatientLastName { get; set; }

        [XmlElement("providerid")]
        public String ProviderId { get; set; }

        [XmlElement("provideridtype")]
        public String ProviderIdType { get; set; }

        [XmlElement("patientidnumber")]
        public String PatientIdNumber { get; set; }

        [XmlElement("patientidnumbertype")]
        public String PatientIdNumberType { get; set; }

        [XmlElement("patientphonenumber")]
        public String PatientPhoneNumber { get; set; }

        [XmlElement("patientphonenumbertype")]
        public String PatientPhoneNumberType { get; set; }

        [XmlElement("prescriptiondate")]
        public DateTime PrescriptionDate { get; set; }

        public Double PdDistant { get; set; }

        public Double PdNear { get; set; }

        public Boolean IsMultiVision { get; set; }

        public Double OsDistantDeCenter { get; set; }

        public Double OdDistantDeCenter { get; set; }

        public Double OsNearDeCenter { get; set; }

        public Double OdNearDeCenter { get; set; }

        public String OnBarCode { get; set; }
    }
}