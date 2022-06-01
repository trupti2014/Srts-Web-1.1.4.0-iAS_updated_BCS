using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Views.Account;
using System.Data;

namespace SrtsWeb.Presenters.Account
{
    public sealed class CertificateInfoPresenter
    {
        private ICertificateInfoView view;

        public CertificateInfoPresenter()
        {
        }

        public CertificateInfoPresenter(ICertificateInfoView view)
        {
            this.view = view;
        }

        public void GetAuthorizationsByCAC_ID(string CAC_ID, string issuerName, string currUser)
        {
            AuthorizationRepository repository = new AuthorizationRepository();
            DataTable AuthorizationsRecord = repository.GetAuthorizationsByCAC_ID(CAC_ID, issuerName);

            view.dtSitesRoles = AuthorizationsRecord;

            if (AuthorizationsRecord.Rows.Count > 0)
            {
                view.isCACFound = false;
                view.isCertFound = false;
                view.isMultipleAccounts = false;

                if (AuthorizationsRecord.Rows.Count > 1)
                {
                    view.isMultipleAccounts = true;
                }

                string user = string.Empty;
                string cacID = string.Empty;
                foreach (DataRow row in AuthorizationsRecord.Rows)
                {
                    if (row["UserName"].ToString() == currUser)
                    {
                        view.isUserFound = true;
                        user = row["UserName"].ToString();
                        cacID = row["CacId"].ToString();
                    }

                    if (row["CacIssuer"].ToString() == issuerName && row["CacId"].ToString() == CAC_ID)
                    {
                        view.isCertFound = true;
                        view.isCACFound = true;
                    }
                }

                if (user == currUser && cacID != CAC_ID && currUser != string.Empty)
                {
                    view.isCACFound = false;
                }
            }
            else
            {
                view.isCACFound = false;
                view.isMultipleAccounts = false;
            }
        }

        public void UpdateAuthorizationCacInfoByUserName(string strUserName, string CAC_ID, string issuerName)
        {
            AuthorizationRepository repository = new AuthorizationRepository();
            repository.UpdateAuthorizationCacInfoByUserName(strUserName, CAC_ID, issuerName);
        }
    }
}