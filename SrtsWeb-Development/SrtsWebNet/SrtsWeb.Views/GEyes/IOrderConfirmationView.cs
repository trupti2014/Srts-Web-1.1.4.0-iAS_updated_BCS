using SrtsWeb.Entities;

namespace SrtsWeb.Views.GEyes
{
    public interface IOrderConfirmationView
    {
        GEyesSession myInfo { get; set; }

        string Message { get; set; }

        string EmailAddress { get; set; }

        string OrderNumber { get; set; }
    }
}