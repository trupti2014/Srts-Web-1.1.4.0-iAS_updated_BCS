using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Account
{
    public interface ICertificateInfoView
    {
        string CAC_ID { get; set; }

        string issuerName { get; set; }

        bool isUserFound { get; set; }

        bool isCACFound { get; set; }

        bool isCertFound { get; set; }

        bool isMultipleAccounts { get; set; }

        DataTable dtSitesRoles { get; set; }
    }
}