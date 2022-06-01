using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.GEyes
{
    public interface IAddressUpdateView
    {
        GEyesSession myInfo { get; set; }

        List<AddressEntity> AddressesBind { set; }

        AddressEntity SelectedAddress { get; set; }

        List<LookupTableEntity> States { get; set; }

        List<LookupTableEntity> Countries { get; set; }

        string EmailAddress { get; }

        string SelectedState { get; set; }

        string SelectedCountry { get; set; }

        string Address1 { get; set; }

        string Address2 { get; set; }

        string City { get; set; }

        string ZipCode { get; set; }

        string Message { get; set; }

        string Name { get; set; }
    }
}