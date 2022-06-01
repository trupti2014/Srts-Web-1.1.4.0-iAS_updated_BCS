using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Individuals
{
    public interface IIndividualDetailsView
    {
        List<LookupTableEntity> LookupCache { get; set; }

        List<AddressEntity> AddressesBind { set; }

        List<EMailAddressEntity> EmailAddressesBind { set; }

        List<IdentificationNumbersEntity> IDNumbersBind { set; }

        List<PhoneNumberEntity> PhoneNumbersBind { set; }

        List<IndividualTypeEntity> IndividualTypesBind { set; }

        List<KeyValueEntity> IndividualTypeLookup { get; set; }

        SRTSSession mySession { get; set; }

        IdentificationNumbersEntity IDUpdate { get; set; }

        List<LookupTableEntity> States { get; set; }

        List<LookupTableEntity> Countries { get; set; }

        String Message { get; set; }

        bool IsAdmin { get; }

        bool IsProvider { get; }

        bool IsPatient { get; }

        bool IsTechnician { get; }
    }
}