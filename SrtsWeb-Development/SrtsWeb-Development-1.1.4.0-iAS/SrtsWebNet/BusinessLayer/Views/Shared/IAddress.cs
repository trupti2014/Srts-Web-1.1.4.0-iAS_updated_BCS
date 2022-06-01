using System;

namespace SrtsWeb.BusinessLayer.Views.Shared
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
        String AddressType { get; set; }
    }
}
