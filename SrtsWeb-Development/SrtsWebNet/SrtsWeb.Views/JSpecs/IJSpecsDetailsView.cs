using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.JSpecs
{
    public interface IJSpecsDetailsView
    {
        JSpecsSession userInfo { get; set; }

        List<AddressEntity> UserAddresses { get; set; }

        List<PrescriptionEntity> UserPrescriptions { get; set; }

        List<EMailAddressEntity> UserEmailAddresses { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        List<LookupTableEntity> CountryData { get; set; }

        List<LookupTableEntity> StateData { get; set; }
        string ErrorMessage { set; }
    }
}
