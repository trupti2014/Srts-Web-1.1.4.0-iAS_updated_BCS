using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Lab
{
    public interface IManageOrdersLabView
    {
        SRTSSession mySession { get; set; }
        List<LookupTableEntity> LookupCache { get; set; }

        List<ManageOrderEntity> DispenseOrderData { get; set; }
        List<ManageOrderEntity> CheckInOrderData { get; set; }

        DateTime? DateReceivedByLab { get; set; }
        DateTime? DateProductionCompleted { get; set; }
        DateTime? DateSentToClinic { get; set; }
        DateTime? DateLabDispensed { get; set; }
        DateTime? DateClinicReceived { get; set; }
        DateTime? DateClinicDispensed { get; set; }
        DateTime? DateRejected { get; set; }
        DateTime? DateCancelled { get; set; }
        DateTime? DateResubmitted { get; set; }

        int? TotalOrdersToCheckIn { get; set; }

        bool IsActive { get; set; }

        string Message { get; set; }
        string SelectedLabel { get; }
    }
}