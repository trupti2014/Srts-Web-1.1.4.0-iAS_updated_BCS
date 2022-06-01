using System;

namespace SrtsWeb.Views.Shared
{
    public interface IIdNumber
    {
        String IdNumber { get; set; }

        String IdNumberType { get; set; }

        String IdNumberMessage { get; set; }
    }
}