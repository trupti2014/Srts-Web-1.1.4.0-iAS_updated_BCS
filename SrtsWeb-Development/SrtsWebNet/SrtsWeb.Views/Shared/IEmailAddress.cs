using System;

namespace SrtsWeb.Views.Shared
{
    public interface IEmailAddress
    {
        String EmailAddress { get; set; }

        String EmailAddressMessage { get; set; }
    }
}