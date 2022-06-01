using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.Views.Patients
{
    public interface IPatientDetailsView
    {
        List<LookupTableEntity> LookupCache { get; set; }
        List<AddressEntity> AddressesBind { set; }
        List<EMailAddressEntity> EmailAddressesBind { set; }
        List<ExamEntity> ExamsBind { set; }
        List<IdentificationNumbersEntity> IDNumbersBind { get; set; }
        DataSet OrdersBind { set; }
        List<PhoneNumberEntity> PhoneNumbersBind { set; }
        List<PrescriptionEntity> PrescriptionsBind { set; }
        SRTSSession mySession { get; set; }
        IdentificationNumbersEntity IDUpdate { get; set; }
        List<LookupTableEntity> States { get; set; }
        List<LookupTableEntity> Countries { get; set; }
        String Message { get; set; }
    }
}