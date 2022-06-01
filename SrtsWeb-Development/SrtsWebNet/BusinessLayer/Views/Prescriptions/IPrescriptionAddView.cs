using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Views.Prescriptions
{
    public interface IPrescriptionAddView
    {
        #region Strings

        string ODAdd { get; }

        string ODAxis { get; }

        string ODAxis_calc { get; } // Added by DB 03 April, 2014

        string ODCylinder { get; }

        string ODCylinder_calc { get; } // Added by DB 03 April, 2014

        string ODHBase { get; set; }

        string ODHPrism { get; }

        string ODSphere { get; }

        string ODSphere_calc { get; } // Added by DB 03 April, 2014

        string ODVBase { get; set; }

        string ODVPrism { get; }

        string OSAdd { get; }

        string OSAxis { get; }

        string OSAxis_calc { get; } // Added by DB 03 April, 2014

        string OSCylinder { get; }

        string OSCylinder_calc { get; } // Added by DB 03 April, 2014

        string OSHBase { get; set; }

        string OSHPrism { get; }

        string OSSphere { get; }

        string OSSphere_calc { get; } // Added by DB 03 April, 2014

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

        List<String> ODAddValuesList { get; set; } // Added by DB 31 Mar, 2014

        List<String> ODAxisValuesList { get; set; } // Added by DB 27 Mar, 2014

        List<String> ODCylinderValuesList { get; set; } // Added by DB 27 Mar, 2014

        List<String> ODHPrismValuesList { get; set; } // Added by DB 31 Mar, 2014

        List<String> ODSphereValuesList { get; set; } // Added by DB 27 Mar, 2014

        List<String> ODVPrismValuesList { get; set; } // Added by DB 31 Mar, 2014

        List<String> OSAddValuesList { get; set; } // Added by DB 31 Mar, 2014

        List<String> OSAxisValuesList { get; set; } // Added by DB 27 Mar, 2014

        List<String> OSCylinderValuesList { get; set; } // Added by DB 27 Mar, 2014

        List<String> OSHPrismValuesList { get; set; } // Added by DB 31 Mar, 2014

        List<String> OSSphereValuesList { get; set; } // Added by DB 27 Mar, 2014

        List<String> OSVPrismValuesList { get; set; } // Added by DB 31 Mar, 2014

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