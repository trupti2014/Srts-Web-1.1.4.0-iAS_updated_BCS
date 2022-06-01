using System;

namespace SrtsWeb.BusinessLayer.Views.Shared
{
    public interface IEmailAddress
    {
        String EmailAddress { get; set; }
        String EmailAddressType { get; set; }
    }
}
