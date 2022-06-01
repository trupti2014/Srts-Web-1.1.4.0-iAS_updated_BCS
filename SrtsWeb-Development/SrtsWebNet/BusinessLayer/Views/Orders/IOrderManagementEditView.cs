using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Orders
{
    public interface IOrderManagementEditView
    {
        SRTSSession mySession { get; set; }

        Dictionary<String, String> LabData { get; set; }

        DateTime? FOCDate { get; set; }

        DateTime? CurrentFocDate { get; set; }

        #region DATATABLE

        List<LookupTableEntity> LookupCache { get; set; }

        List<FrameItemEntity> BridgeData { get; set; }
        List<FrameItemEntity> ColorData { get; set; }
        List<FrameItemEntity> EyeData { get; set; }
        List<FrameItemEntity> TempleData { get; set; }
        List<FrameItemEntity> LensData { get; set; }
        List<FrameItemEntity> TintData { get; set; }
        List<FrameItemEntity> MaterialData { set; }

        DataTable FrameData { get; set; }

        DataTable LocationData { get; set; }

        #endregion DATATABLE

        #region LISTS
        List<FrameEntity> FrameListData { get; set; }

        List<KeyValuePair<string, string>> PriorityData { get; set; }

        List<OrderPriorityEntity> PriorityList { set; }

        List<PersonnelEntity> TechData { get; set; }

        List<PersonnelEntity> DoctorData { get; set; }

        List<OrderStateEntity> OrderStateHistory { set; }

        #endregion LISTS

        #region INTEGERS

        int OrderStatusID { get; set; }

        int Pairs { get; set; }

        int Cases { get; set; }

        int DoctorSelected { get; set; }

        int TechSelected { get; set; }

        #endregion INTEGERS

        #region STRINGS

        string OrderNumber { get; set; }

        string LinkedId { get; set; }

        string PageTitle { set; }

        string OrderStatus { get; set; }

        string LabSelected { get; set; }

        string LocationSelected { get; set; }

        string PrioritySelected { get; set; }

        string BridgeSelected { get; set; }

        string ColorSelected { get; set; }

        string EyeSelected { get; set; }

        string FrameSelected { get; set; }

        string LensSelected { get; set; }

        string MaterialSelected { get; set; }

        string TempleSelected { get; set; }

        string TintSelected { get; set; }

        string LastFocOrderNumber { get; set; }

        string FocJust { get; set; }

        string MaterialJust { get; set; }

        string ODSegHeight { get; set; }

        string OSSegHeight { get; set; }

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

        string PDOS { get; set; }

        string PDODNear { get; set; }

        string PDOSNear { get; set; }

        string Comment1 { get; set; }

        string Comment2 { get; set; }

        string LabComment { get; set; }

        string ClinicJust { get; set; }

        string Message { get; set; }

        #endregion STRINGS

        #region BOOLEANS

        bool IsMonoCalculation { get; set; }

        bool EligibilityVisibility { set; }

        bool IsMultiVision { get; set; }

        bool IsShipToPatient { get; set; }

        bool RequiresJustification { get; set; }

        #endregion BOOLEANS
    }
}