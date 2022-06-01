using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.Views.Orders
{
    public interface IOrderEditStatusClinicView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        Dictionary<String, String> LabData { get; set; }

        #region DATATABLES

        DataTable LocationData { get; set; }

        DataTable TintData { get; set; }

        DataTable TempleData { get; set; }

        DataTable MaterialData { get; set; }

        DataTable LensData { get; set; }

        DataTable FrameData { get; set; }

        DataTable EyeData { get; set; }

        DataTable ColorData { get; set; }

        DataTable BridgeData { get; set; }

        #endregion DATATABLES

        #region LISTS

        List<PersonnelEntity> TechData { get; set; }

        List<PersonnelEntity> DoctorData { get; set; }

        List<KeyValuePair<string, string>> PriorityData { get; set; }

        List<OrderPriorityEntity> PriorityList { set; }

        #endregion LISTS

        #region INTEGERS

        int OrderStatusID { get; set; }

        int DoctorSelected { get; set; }

        int TechSelected { get; set; }

        int Pairs { get; set; }

        int Cases { get; set; }

        #endregion INTEGERS

        #region DATETIMES

        DateTime? FOCDate { get; set; }

        DateTime? CurrentFocDate { get; set; }

        #endregion DATETIMES

        #region STRINGS

        string LabSelected { get; set; }

        string LocationSelected { get; set; }

        string LastFocOrderNumber { get; set; }

        string LinkedID { get; set; }

        string Message { get; set; }

        string StatusSelected { get; }

        string JustificationInfo { get; set; }

        string OrderStatus { get; set; }

        string Comment1 { get; set; }

        string Comment2 { get; set; }

        string LabJustification { set; }

        string ODSphere { get; set; }

        string ODCylinder { get; set; }

        string ODAxis { get; set; }

        string ODHPrism { get; set; }

        string ODHBase { get; set; }

        string ODVPrism { get; set; }

        string ODVBase { get; set; }

        string ODAdd { get; set; }

        string OSSphere { get; set; }

        string OSCylinder { get; set; }

        string OSAxis { get; set; }

        string OSHPrism { get; set; }

        string OSHBase { get; set; }

        string OSVPrism { get; set; }

        string OSVBase { get; set; }

        string OSAdd { get; set; }

        string PDOD { get; set; }

        string PDOS { get; set; }

        string PDTotal { get; set; }

        string PDODNear { get; set; }

        string PDOSNear { get; set; }

        string PDTotalNear { get; set; }

        string TintSelected { get; set; }

        string PrioritySelected { get; set; }

        string BridgeSelected { get; set; }

        string ColorSelected { get; set; }

        string EyeSelected { get; set; }

        string FrameSelected { get; set; }

        string LensSelected { get; set; }

        string MaterialSelected { get; set; }

        string ODSegHeight { get; set; }

        string OSSegHeight { get; set; }

        string PageTitle { set; }

        string TempleSelected { get; set; }

        #endregion STRINGS

        #region BOOLEANS

        bool IsApproved { get; set; }

        bool IsMultiVision { get; set; }

        bool EligibilityVisibility { get; set; }

        bool IsMonoCalculation { get; set; }

        bool IsShipToPatient { get; set; }

        bool RequiresJustification { get; set; }

        #endregion BOOLEANS
    }
}