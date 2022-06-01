using System;

namespace SrtsWeb.Views.Shared
{
    public interface IPhoneNumber
    {
        String PhoneNumber { get; set; }

        String Extension { get; set; }

        String PhoneNumberMessage { get; set; }
    }
}