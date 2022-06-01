using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.GEyes
{
    public interface IAddressUpdateView
    {
        GEyesSession myInfo { get; set; }

        string State { get; set; }
        List<StateEntity> StateList { get; set; }

        List<AddressEntity> AddressesBind { set; }

        AddressEntity SelectedAddress { get; set; }

        List<LookupTableEntity> Countries { get; set; }

        string EmailAddress { get; }

        string SelectedCountry { get; set; }

        string Address1 { get; set; }

        string Address2 { get; set; }

        string City { get; set; }

        string ZipCode { get; set; }

        string Message { get; set; }

        string Name { get; set; }
    }
}