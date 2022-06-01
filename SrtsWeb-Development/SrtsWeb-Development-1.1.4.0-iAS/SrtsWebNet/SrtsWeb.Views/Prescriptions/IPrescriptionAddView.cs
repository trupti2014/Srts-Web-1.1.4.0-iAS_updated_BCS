using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Prescriptions
{
    public interface IPrescriptionAddView
    {
        #region Strings

        string ODAdd { get; }

        string ODAxis { get; }

        string ODAxis_calc { get; }

        string ODCylinder { get; }

        string ODCylinder_calc { get; }

        string ODHBase { get; set; }

        string ODHPrism { get; }

        string ODSphere { get; }

        string ODSphere_calc { get; }

        string ODVBase { get; set; }

        string ODVPrism { get; }

        string OSAdd { get; }

        string OSAxis { get; }

        string OSAxis_calc { get; }

        string OSCylinder { get; }

        string OSCylinder_calc { get; }

        string OSHBase { get; set; }

        string OSHPrism { get; }

        string OSSphere { get; }

        string OSSphere_calc { get; }

        string OSVBase { get; set; }

        string OSVPrism { get; }

        string PDTotal { get; }

        string PDTotalNear { get; }

        string PDOD { get; }

        string PDODNear { get; }

        string PDOS { get; }

        string PDOSNear { get; }

        string msg { get; set; }

        #endregion Strings

        #region Lists

        List<String> ODAddValuesList { get; set; }

        List<String> ODAxisValuesList { get; set; }

        List<String> ODCylinderValuesList { get; set; }

        List<String> ODHPrismValuesList { get; set; }

        List<String> ODSphereValuesList { get; set; }

        List<String> ODVPrismValuesList { get; set; }

        List<String> OSAddValuesList { get; set; }

        List<String> OSAxisValuesList { get; set; }

        List<String> OSCylinderValuesList { get; set; }

        List<String> OSHPrismValuesList { get; set; }

        List<String> OSSphereValuesList { get; set; }

        List<String> OSVPrismValuesList { get; set; }

        List<String> PDTotalValues { get; set; }

        List<String> PDMonoValues { get; set; }

        List<PersonnelEntity> DoctorsList { get; set; }

        #endregion Lists

        int DoctorSelected { get; set; }

        bool IsMonoCalculation { get; set; }

        bool CommitSave { get; set; }

        SRTSSession mySession { get; set; }
    }
}