using System;
using System.Collections.Generic;
using System.IO;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class Exam
    {
        public int ExamId { get; set; }

        public DateTime ExamDate { get; set; }

        public DateTime DateLastModified { get; set; }

        public string OdUncorrected { get; set; }

        public string OsUncorrected { get; set; }

        public string OdOsUncorrected { get; set; }

        public string OdCorrected { get; set; }

        public string OsCorrected { get; set; }

        public string OdOsCorrected { get; set; }

        public string ExamComments { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }
    }

    [Serializable]
    public class PatientOrderDTO
    {
        public Int32 IndividualId { get; set; }

        public String Demographic { get; set; }

        public String OrderNumber { get; set; }

        public String PatientSiteCode { get; set; }
    }

    [Serializable]
    public class Individual
    {
        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String FullName { get { return String.Format("{0} {1}", FirstName, LastName); } }

        public String PersonnelType { get; set; }

        public String IndividualValue { get; set; }
    }

    [Serializable]
    public class Order
    {
        public string TechnicianName
        {
            get;
            set;
        }

        public string OdSegHeight
        {
            get;
            set;
        }

        public string OsSegHeight
        {
            get;
            set;
        }

        public int TechnicianId
        {
            get;
            set;
        }

        public int Pairs
        {
            get;
            set;
        }

        public int Cases
        {
            get;
            set;
        }

        public int PrescriptionId
        {
            get;
            set;
        }

        public int PatientId
        {
            get;
            set;
        }

        public bool IsReOrder
        { get; set; }

        public bool ShipToPatient
        {
            get;
            set;
        }

        public string OrderDisbursement
        { get; set; }

        public bool CallPatient
        {
            get;
            set;
        }

        public bool IsComplete
        { get; set; }

        public bool IsGEyes
        {
            get;
            set;
        }

        public bool IsMultivision
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public bool IsEmailPatient
        {
            get;
            set;
        }

        public bool IsPermanentEmailAddressChange
        {
            get;
            set;
        }

        public string OrderEmailAddress
        { get; set; }

        public string FocJustification
        { get; set; }

        public string MaterialJustification
        { get; set; }

        public string CoatingJustification
        { get; set; }

        public string Comment1
        {
            get;
            set;
        }

        public string Comment2
        {
            get;
            set;
        }

        public string Priority
        {
            get;
            set;
        }

        public string LabSiteCode
        {
            get;
            set;
        }

        public string Frame
        {
            get;
            set;
        }

        public string Color
        {
            get;
            set;
        }

        public string Temple
        {
            get;
            set;
        }

        public string Eye
        {
            get;
            set;
        }

        public string Bridge
        {
            get;
            set;
        }

        public string LensType
        {
            get;
            set;
        }

        public string Tint
        {
            get;
            set;
        }

        public string Coatings
        {
            get;
            set;
        }

        public string Material
        {
            get;
            set;
        }

        public string OrderNumber
        {
            get;
            set;
        }

        public string ClinicSiteCode
        {
            get;
            set;
        }

        public string Demographic
        {
            get;
            set;
        }

        public string LocationCode
        {
            get;
            set;
        }

        public string ModifiedBy
        {
            get;
            set;
        }

        public string LinkedId
        {
            get;
            set;
        }

        public string CurrentStatus { get; set; }

        public string ReorderReason { get; set; }

        public DateTime DateLastModified
        {
            get;
            set;
        }

        public byte[] OrderNumberBarCode { get; set; }

        public string DispenseComments
        {
            get;
            set;
        }
        //public string GroupName { get; set; }

        public Order Clone() { return MemberwiseClone() as Order; }
    }

    [Serializable]
    public class LookupData
    {
        public string FrameCode
        {
            get;
            set;
        }

        public string FrameDescription
        {
            get;
            set;
        }

        public string FrameLongDescription
        {
            get;
            set;
        }

        public string FrameNotes
        {
            get;
            set;
        }

        public bool IsInsert
        {
            get;
            set;
        }

        public string TypeEntry
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public bool IsFoc
        {
            get;
            set;
        }

        public int MaxPairs
        {
            get;
            set;
        }
    }

    [Serializable]
    public class OrderDropDownData
    {
        public Dictionary<String, String> PriorityList
        {
            get;
            set;
        }

        public Dictionary<String, String> LabSiteCodeList
        {
            get;
            set;
        }

        public List<FrameEntity> FrameList
        {
            get;
            set;
        }

        public Dictionary<String, String> ColorList
        {
            get;
            set;
        }

        public Dictionary<String, String> TempleList
        {
            get;
            set;
        }

        public Dictionary<String, String> EyeList
        {
            get;
            set;
        }

        public Dictionary<String, String> BridgeList
        {
            get;
            set;
        }

        public Dictionary<String, String> LensTypeList
        {
            get;
            set;
        }

        public Dictionary<String, String> TintList
        {
            get;
            set;
        }

        public Dictionary<String, String> CoatingList
        {
            get;
            set;
        }

        public Dictionary<String, String> MaterialList
        {
            get;
            set;
        }

        public Dictionary<String, String> ReorderList { get; set; }
    }

    [Serializable]
    public class Prescription : ICloneable
    {
        public PrescriptionDocument PrescriptionDocument
        { get; set; }

        public DateTime PrescriptionDate
        { get; set; }

        public DateTime DateLastModified
        { get; set; }

        public int PrescriptionId
        { get; set; }

        public int PrescriptionScanId
        { get; set; }


        public int ExamId
        { get; set; }

        public int PatientId
        { get; set; }



        public int ProviderId
        {
            get;
            set;
        }

        public string PrescriptionName
        {
            get;
            set;
        }

        public bool IsActive
        { get; set; }

        public bool IsUsed
        { get; set; }

        public bool IsMonoCalculation
        { get; set; }

        public decimal OdSphere
        {
            get;
            set;
        }

        public decimal OsSphere
        {
            get;
            set;
        }

        public decimal OdCylinder
        {
            get;
            set;
        }

        public decimal OsCylinder
        {
            get;
            set;
        }

        public int OdAxis
        {
            get;
            set;
        }

        public int OsAxis
        {
            get;
            set;
        }

        public string OdSphereCalc
        {
            get;
            set;
        }

        public string OsSphereCalc
        {
            get;
            set;
        }

        public string OdCylinderCalc
        {
            get;
            set;
        }

        public string OsCylinderCalc
        {
            get;
            set;
        }

        public string OdAxisCalc
        {
            get;
            set;
        }

        public string OsAxisCalc
        {
            get;
            set;
        }

        public decimal OdAdd
        {
            get;
            set;
        }

        public decimal OsAdd
        {
            get;
            set;
        }

        public decimal PdDistTotal
        { get; set; }

        public decimal PdNearTotal
        { get; set; }

        public decimal OdPdDist
        {
            get;
            set;
        }

        public decimal OsPdDist
        {
            get;
            set;
        }

        public decimal OdPdNear
        {
            get;
            set;
        }

        public decimal OsPdNear
        {
            get;
            set;
        }

        public string OdHBase
        {
            get;
            set;
        }

        public string OsHBase
        {
            get;
            set;
        }

        public string OdVBase
        {
            get;
            set;
        }

        public string OsVBase
        {
            get;
            set;
        }

        public decimal OdHPrism
        {
            get;
            set;
        }

        public decimal OsHPrism
        {
            get;
            set;
        }

        public decimal OdVPrism
        {
            get;
            set;
        }

        public decimal OsVPrism
        {
            get;
            set;
        }

        public string OrderedFrameHistory
        {
            get;
            set;
        }

        public List<String> ExtraRxTypes 
        { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [Serializable]
    public class PrescriptionDocument
    {
        public int Id
        { get; set; }

        public int IndividualId
        { get; set; }

        public int PrescriptionId
        { get; set; }

        public string DocumentName
        { get; set; }

        public DateTime ScanDate
        { get; set; }

        public DateTime DelDate
        { get; set; }

        public int DocumentLength
        { get; set; }

        public string DocumentType
        { get; set; }

        public byte[] DocumentImage
        { get; set; }

        //public Stream DocumentFileStream
        //{ get; set; }
    }


    public class RxVals
    {
        public decimal Sphere
        {
            get;
            set;
        }

        public decimal Cylinder
        {
            get;
            set;
        }

        public int Axis
        {
            get;
            set;
        }

        public string SphereCalc
        {
            get;
            set;
        }

        public string CylinderCalc
        {
            get;
            set;
        }

        public string AxisCalc
        {
            get;
            set;
        }
    }

    [Serializable]
    public class NewRxVals
    {
        public string OdSphereCalc
        {
            get;
            set;
        }

        public string OsSphereCalc
        {
            get;
            set;
        }

        public string OdCylinderCalc
        {
            get;
            set;
        }

        public string OsCylinderCalc
        {
            get;
            set;
        }

        public string OdAxisCalc
        {
            get;
            set;
        }

        public string OsAxisCalc
        {
            get;
            set;
        }
    }


    [Serializable]
    public class OrderLabelAddresses
    {
        public String OrderNumber { get; set; }

        public int PatientId { get; set; }

       // public String Patient { get; set; }

        public String FirstName { get; set; }

        public String MiddleName { get; set; }

        public String LastName { get; set; }

        public String Address1 { get; set; }

        public String Address2 { get; set; }

        public String Address3 { get; set; }

        public String City { get; set; }

        public String State { get; set; }

        public String CountryCode { get; set; }

        public String CountryName { get; set; }

        public String ZipCode { get; set; }

        public String ShipAddress1 { get; set; }

        public String ShipAddress2 { get; set; }

        public String ShipAddress3 { get; set; }

        public String ShipCity { get; set; }

        public String ShipState { get; set; }

        public String ShipCountryCode { get; set; }

        public String ShipCountryName { get; set; }

        public String ShipZipCode { get; set; }

        public Boolean UseMailingAddress { get; set; }

        public DateTime DateVerified { get; set; }

        public int? ExpireDays { get; set; }

    }

    [Serializable]
    public class OrderEmail
    {
        public String OrderNumber { get; set; }

        public String EmailAddress { get; set; }

        public String EmailMsg { get; set; }

        public DateTime? EmailDate { get; set; }

        public int PatientId { get; set; }

        public Boolean ChangeEmail { get; set; }

        public Boolean EmailSent { get; set; }
    }

}