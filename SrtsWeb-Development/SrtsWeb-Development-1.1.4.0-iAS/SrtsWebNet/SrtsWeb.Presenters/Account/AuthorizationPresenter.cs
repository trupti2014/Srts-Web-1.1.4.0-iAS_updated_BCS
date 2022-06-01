using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Views.Account;
using System.Data;
using System.Linq;

namespace SrtsWeb.Presenters.Account
{
    public sealed class AuthorizationPresenter
    {
        private IAuthorizationView view;

        public AuthorizationPresenter()
        {
        }

        public AuthorizationPresenter(IAuthorizationView view)
        {
            this.view = view;
        }

        public void InsertAuthorizationByUserName(string strUserName)
        {
            var repository = new AuthorizationRepository();
            repository.InsertAuthorizationByUserName(strUserName);
        }

        public void UpdateAuthorizationSSOByUserName(string strUserName, string strSsoUserName)
        {
            var repository = new AuthorizationRepository();
            repository.UpdateAuthorizationSSOByUserName(strUserName, strSsoUserName);
        }

        public bool DeleteAuthorizationByUserName(string srtsUserName)
        {
            var repository = new AuthorizationRepository();
            return repository.DeleteAuthorizationByUserName(srtsUserName);
        }

        public void GetAuthorizationsByUserName(string srtUserName)
        {
            var repository = new AuthorizationRepository();
            DataTable Authorizations = repository.GetAuthorizationsByUserName(srtUserName);

            view.dtAuthorizations = Authorizations;

            if (Authorizations.Rows.Count > 0)
            {
                view.isUserFound = true;

                view.isMultipleAccounts = true;

                if (Authorizations.Rows.Count < 2)
                {
                    view.isMultipleAccounts = false;
                }

                foreach (DataRow row in Authorizations.Rows)
                {
                    if (row["CacId"].ToString() != "")
                    {
                        view.isCacOnFile = true;
                    }

                    if (row["SSO_UserName"].ToString() != "")
                    {
                        view.isSsoUserNameFound = true;
                    }
                }
            }
        }

        public bool IsUserInAuthorizationTbl(string srtUserName)
        {
            var repository = new AuthorizationRepository();
            DataTable Authorizations = repository.GetAuthorizationsByUserName(srtUserName);
            var isThere = Authorizations.Rows.Count > 0;
            Authorizations.Clear();
            Authorizations = null;
            return isThere;
        }

        public bool IsUserCacRegistered(string srtUserName)
        {
            var repository = new AuthorizationRepository();
            var Authorizations = repository.GetAuthorizationsByUserName(srtUserName);
            if (Authorizations == null || Authorizations.Rows.Count.Equals(0)) return false;
            return Authorizations.AsEnumerable().Cast<DataRow>().ToList().Any(x => !string.IsNullOrEmpty(x["CacId"].ToString()));
        }

        public void GetPublicAnnouncements()
        {
            var r = new CMSRepository.CmsEntityRepository();

            this.view.Announcements = r.GetMessageByContentTypeID("C000");
        }

        public void UpdateSiteCodeByUserName(string SiteCode, string UserName)
        {
            var rep = new AuthorizationRepository();
            rep.UpdateSiteCodeByUserName(SiteCode, UserName);
        }
    }
}