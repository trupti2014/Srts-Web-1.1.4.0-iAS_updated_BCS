using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Orders
{
    public interface IOrderAddView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        Dictionary<String, String> LabData { get; set; }

        #region DATATABLES

        List<FrameItemEntity> BridgeData { set; }
        List<FrameItemEntity> ColorData { set; }
        List<FrameItemEntity> EyeData { set; }
        List<FrameItemEntity> LensData { set; }
        List<FrameItemEntity> MaterialData { set; }
        List<FrameItemEntity> TempleData { set; }
        List<FrameItemEntity> TintData { set; }

        DataTable FrameData { set; }
        DataTable LocationData { set; }

        #endregion DATATABLES

        #region LISTS
        List<FrameEntity> FrameListData { get; set; }

        List<KeyValuePair<string, string>> PriorityData { set; }

        List<OrderPriorityEntity> PriorityList { set; }

        List<PersonnelEntity> TechData { set; }

        List<PersonnelEntity> DoctorData { set; }

        #endregion LISTS

        #region INTEGERS

        int DoctorSelected { get; set; }

        int TechSelected { get; set; }

        int Pairs { get; set; }

        int Cases { get; set; }

        #endregion INTEGERS

        #region STRINGS

        string BridgeSelected { get; set; }

        string ColorSelected { get; set; }

        string EyeSelected { get; set; }

        string FrameSelected { get; set; }

        string LensSelected { get; set; }

        string MaterialSelected { get; set; }

        string ODSegHeight { get; set; }

        string OSSegHeight { get; set; }

        string TempleSelected { get; set; }

        string TintSelected { get; set; }

        string PrioritySelected { get; set; }

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

        string PDTotal { get; set; }

        string PDTotalNear { get; set; }

        string PDOD { get; set; }

        string PDODNear { get; set; }

        string PDOS { get; set; }

        string PDOSNear { get; set; }

        string Comment1 { get; set; }

        string Comment2 { get; set; }

        string LabSelected { get; set; }

        string LocationSelected { get; set; }

        string FOCDate { get; set; }

        string LastFocOrderNumber { get; set; }

        string Message { get; set; }

        string WarningMsg { set; }

        string FocJust { get; set; }

        string MaterialJust { get; set; }

        #endregion STRINGS

        #region BOOLEANS

        bool IsMonoCalculation { get; set; }

        bool EligibilityVisibility { set; }

        bool IsMultiVision { get; set; }

        bool IsComplete { get; set; }

        bool RequiresJustification { get; set; }

        bool IsShipToPatient { get; set; }

        #endregion BOOLEANS
    }
}