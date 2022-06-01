using System;

namespace SrtsWeb.Views.Shared
{
    public interface IPersonnel
    {
        String FirstName { get; set; }

        String MiddleName { get; set; }

        String LastName { get; set; }

        DateTime? DateOfBirth { get; set; }

        String Gender { get; set; }
    }
}