using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.BusinessLayer.Views.Orders
{
    public interface IOrderManagementView
    {
        SRTSSession mySession { get; set; }

        Exam Exam { get; set; }
        List<Exam> ExamList { get; set; }
        List<IndividualEntity> ProviderList { get; set; }

        Prescription Prescription { get; set; }
        List<Prescription> PrescriptionList { get; set; }
        NewRxVals CalculatedRxValues { get; set; }

        Order Order { get; set; }
        List<Order> OrderList { get; set; }
        OrderDropDownData OrderDdlData { get; set; }
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
        #endregion

        IndividualEntity Patient { get;  set; }
        Boolean PatientHasAddress { get; set; }
        Int32 PatientId { get; set; }
        String Demographic { get; set; }
        String SiteCode { get; set; }
    }
}
