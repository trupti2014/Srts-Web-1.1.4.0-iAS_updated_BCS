using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class PrescriptionEntity : IComparable<PrescriptionEntity>
    {
        public int ID { get; set; }

        public int? ExamID { get; set; }

        public int? PrescriptionDocumentID { get; set; }

        public int IndividualID_Patient { get; set; }

        public int IndividualID_Doctor { get; set; }

        public int ODAxis { get; set; }// 1 to 180

        public int OSAxis { get; set; }// 1 to 180

        public int EnteredODAxis { get; set; }

        public int EnteredOSAxis { get; set; }

        public decimal ODSphere { get; set; }//Plano or number -20 to 20

        public decimal OSSphere { get; set; }//Plano or Number -20 to 20

        public decimal ODCylinder { get; set; }//Sphere or number -20 to 20

        public decimal OSCylinder { get; set; }//Sphere or number -20 to 20

        public decimal ODHPrism { get; set; }// -15 to 15

        public decimal OSHPrism { get; set; }// -15 to 15

        public decimal ODVPrism { get; set; }// -15 to 15

        public decimal OSVPrism { get; set; }// -15 to 15

        public decimal ODAdd { get; set; }

        public decimal OSAdd { get; set; }

        public decimal EnteredODSphere { get; set; }

        public decimal EnteredOSSphere { get; set; }

        public decimal EnteredODCylinder { get; set; }

        public decimal EnteredOSCylinder { get; set; }

        public decimal PDTotal { get; set; }

        public decimal PDTotalNear { get; set; }

        public decimal PDOD { get; set; }

        public decimal PDODNear { get; set; }

        public decimal PDOS { get; set; }

        public decimal PDOSNear { get; set; }

        public string ODHBase { get; set; }//In/Out

        public string OSHBase { get; set; }//In/Out

        public string ODVBase { get; set; }//Up/Down

        public string OSVBase { get; set; }//Up/Down

        public string ModifiedBy { get; set; }

        public bool IsMonoCalculation { get; set; }

        public bool IsUsed { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeletable { get; set; }

        public DateTime DateLastModified { get; set; }

        public DateTime PrescriptionDate { get; set; }

        public string RxName { get; set; }

        public int CompareTo(PrescriptionEntity o)
        {
            if (this.ODAxis != o.ODAxis) return -1;
            if (this.ODCylinder != o.ODCylinder) return -1;
            if (this.ODHBase != o.ODHBase) return -1;
            if (this.ODHPrism != o.ODHPrism) return -1;
            if (this.ODAdd != o.ODAdd) return -1;
            if (this.ODSphere != o.ODSphere) return -1;
            if (this.ODVBase != o.ODVBase) return -1;
            if (this.ODVPrism != o.ODVPrism) return -1;
            if (this.OSAxis != o.OSAxis) return -1;
            if (this.OSCylinder != o.OSCylinder) return -1;
            if (this.OSHBase != o.OSHBase) return -1;
            if (this.OSHPrism != o.OSHPrism) return -1;
            if (this.OSAdd != o.OSAdd) return -1;
            if (this.OSSphere != o.OSSphere) return -1;
            if (this.OSVBase != o.OSVBase) return -1;
            if (this.OSVPrism != o.OSVPrism) return -1;

            return 0;
        }
    }

    public interface IComparable<PrescriptionEntity>
    {
        int CompareTo(SrtsWeb.Entities.PrescriptionEntity o);
    }
}