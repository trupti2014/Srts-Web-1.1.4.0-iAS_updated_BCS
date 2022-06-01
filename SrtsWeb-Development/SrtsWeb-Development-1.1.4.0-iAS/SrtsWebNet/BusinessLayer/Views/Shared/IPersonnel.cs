using System;

namespace SrtsWeb.BusinessLayer.Views.Shared
{
    public interface IPersonnel
    {
        String FirstName { get; set; }
        String MiddleName { get; set; }
        String LastName { get; set; }
        String DateOfBirth { get; set; }
        String Gender { get; set; }
    }
}
