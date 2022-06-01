using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System;
using System.Linq;

namespace SrtsWeb.Presenters.Admin
{
    public sealed class UserManagementPresenter
    {
        private IUserManagement _view;

        public UserManagementPresenter(IUserManagement view)
        { this._view = view; }

        public void GetAllSites()
        {
            var r = new SiteRepository.SiteCodeRepository();
            var scs = r.GetAllSites().Where(x => x.IsActive == true).Distinct().ToList();
            if (scs != null && scs.Count > 0)
                this._view.SiteCodes = scs;
        }

        public void SyncUserToIndividual(String userName, Int32 individualId, Boolean forRemove = false)
        {
            if (userName == null || individualId == default(Int32)) return;
            var r = new IndividualRepository();
            r.SyncUserToIndividual(userName, individualId, forRemove);
        }

        public void IsUserCmsManager()
        {
            var r = new IndividualRepository();
            _view.IsCmsUser = r.GetSyncedUser(_view.SelectedIndividualId);
        }

        public void GetIndividualsAtSite(String siteCode)
        {
            var r = new IndividualRepository();
            var i = r.GetAllIndividualsAtSite(siteCode, _view.mySession.ModifiedBy);
            _view.LinkIndividuals = i.Where(x => x.PersonalType.ToLower() == "technician" || x.PersonalType.ToLower() == "other" || x.PersonalType.ToLower() == "provider").ToList();
        }

        public void GetIsUserCacEnabled(String userName)
        {
            var r = new AuthorizationRepository();
            var dt = r.GetAuthorizationsByUserName(userName);
            if (dt.IsNull() || dt.Rows.Count.Equals(0)) { this._view.IsCacEnabled = false; return; }
            var row = dt.Rows.Cast<System.Data.DataRow>().ToList().FirstOrDefault(x => x["UserName"].ToString().ToLower() == userName.ToLower());
            if (row.IsNull()) { this._view.IsCacEnabled = false; return; }
            this._view.IsCacEnabled = !String.IsNullOrEmpty(row["CacId"].ToString());
        }
    }
}