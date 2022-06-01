using System;

namespace SrtsWeb.Views.Shared
{
    public interface IAddress
    {
        String ID { get; set; }

        String Address1 { get; set; }

        String Address2 { get; set; }

        String Address3 { get; set; }

        String City { get; set; }

        String Country { get; set; }

        String State { get; set; }

        String ZipCode { get; set; }

        String AddressMessage { get; set; }

        String UnitIdentificationCode { get; set; }
    }
}