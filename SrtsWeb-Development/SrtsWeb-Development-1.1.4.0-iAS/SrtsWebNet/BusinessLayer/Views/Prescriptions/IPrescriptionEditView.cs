using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Views.Prescriptions
{
    public interface IPrescriptionEditView
    {
        int DoctorSelected { get; set; }

        bool IsMonoCalculation { get; set; }

        bool CommitSave { get; set; }

        bool IsActive { get; }

        bool IsUsed { get; set; }

        bool IsDeletable { get; set; }

        SRTSSession mySession { get; set; }

        #region Strings

        string ODAdd { get; set; }

        string ODAxis { get; set; }

        string ODAxis_calc { get; }

        string ODCylinder { get; set; }

        string ODCylinder_calc { get; }

        string ODHBase { get; set; }

        string ODHPrism { get; set; }

        string ODSphere { get; set; }

        string ODSphere_calc { get; }

        string ODVBase { get; set; }

        string ODVPrism { get; set; }

        string OSAdd { get; set; }

        string OSAxis { get; set; }

        string OSAxis_calc { get; }

        string OSCylinder { get; set; }

        string OSCylinder_calc { get; }

        string OSHBase { get; set; }

        string OSHPrism { get; set; }

        string OSSphere { get; set; }

        string OSSphere_calc { get; }

        string OSVBase { get; set; }

        string OSVPrism { get; set; }

        string PDTotal { get; set; }

        string PDTotalNear { get; set; }

        string PDOD { get; set; }

        string PDODNear { get; set; }

        string PDOS { get; set; }

        string PDOSNear { get; set; }

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
    }
}