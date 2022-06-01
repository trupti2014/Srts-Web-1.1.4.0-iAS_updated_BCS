using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Orders
{
    public interface IOrderManagementView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        Exam Exam { get; set; }

        List<Exam> ExamList { get; set; }

        List<IndividualEntity> ProviderList { get; set; }

        Prescription Prescription { get; set; }

        List<Prescription> PrescriptionList { get; set; }

        PrescriptionDocument PrescriptionDocument { get; set; }

        NewRxVals CalculatedRxValues { get; set; }

        Order Order { get; set; }

        Order OriginalOrder { get; set; }

        List<Order> OrderList { get; set; }

        OrderDropDownData OrderDdlData { get; set; }

        FrameItemDefaultEntity DefaultFrameItems { get; set; }

        Dictionary<String, String> SpecialLabTintList { get; set; }

        List<LookupData> LookupDataList { get; set; }

        String SelectedPriority { get; set; }

        List<OrderStateEntity> OrderStateHistory { get; set; }

        String ClinicJustification { get; set; }

        String LabJustification { get; set; }

        #region LAB PROPERTIES

        String LabAction { get; set; }

        String RejectRedirectJustification { get; set; }

        String RedirectLab { get; set; }

        List<SiteCodeEntity> RedirectLabList { get; set; }

        DateTime HoldForStockDate { get; set; }

        #endregion LAB PROPERTIES

        IndividualEntity Patient { get; set; }

        AddressEntity PatientAddress { get; set; }

        List<LookupTableEntity> Countries { get; set; }

        List<LookupTableEntity> States { get; set; }

        Boolean PatientHasAddress { get; set; }

        Int32 PatientId { get; set; }

        String Demographic { get; set; }

        String SiteCode { get; set; }

        Boolean OrderIsPrefill { get; set; }

        //String GroupName { get; set; }

        SitePrefFrameItemEntity FramePreferences { get; }
        List<SitePrefFrameItemEntity> FramePreferencesList { get; set; }

        SitePrefRxEntity SitePrefsRX { get; set; }

        SitePrefOrderEntity OrderPreferences { get; set; }

        List<SitePrefLabJustification> LabJustificationPreferences { get; set; }
      
    }
}