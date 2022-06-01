using SrtsWeb.Entities;

namespace SrtsWeb.Views.GEyes
{
    public interface IIndividualInfoView
    {
        GEyesSession myInfo { get; set; }

        string IDNumber { get; set; }

        string IDType { get; }

        string EmailAddress { get; set; }

        string EmailAddressConfirm { get; set; }

        string ZipCode { get; set; }

        string Message { get; set; }

        string ClinicCode { get; set; }
    }
}