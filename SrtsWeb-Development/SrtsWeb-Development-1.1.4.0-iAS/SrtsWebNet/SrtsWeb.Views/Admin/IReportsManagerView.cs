using System;
using System.Collections.Generic;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Admin
{
    public interface IReportsManagerView
    {
        SRTSSession mySession { get; set; }
        String ReprintOrderNumbers { get; set; }

        List<ReprintEntity> ReprintItems { get; set; }

        ReprintEntity SelectedReprintItem { get; set; }

        List<ReprintReturnEntity> ReprintLabels { get; set; }

        List<ReprintOnDemandReturnEntity> ReprintOnDemandLabels { get; set; }

        OrderLabelAddresses OrderAddresses { get; set; }

        string SelectedLabel { get; set; }

        List<string> PrintOrderNumbers { get; set; }

        List<string> OrderNumbersNotAdded { get; set; }

        Boolean UseMailingAddress { get; set; }

        //IndividualEntity Patient { get; set; }

        String FirstName { get; set; }

        String MiddleName { get; set; }

        String LastName { get; set; }

        AddressEntity PatientAddress { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        List<LookupTableEntity> Countries { get; set; }

        List<LookupTableEntity> States { get; set; }

        Boolean PatientHasAddress { get; set; }

        Int32 PatientId { get; set; }
    }
}