using System;

namespace SrtsWeb.BusinessLayer.Views.Shared
{
    public interface IPhoneNumber
    {
        String PhoneNumber { get; set; }
        String Extension { get; set; }
        String PhoneNumberType { get; set; }
    }
}
