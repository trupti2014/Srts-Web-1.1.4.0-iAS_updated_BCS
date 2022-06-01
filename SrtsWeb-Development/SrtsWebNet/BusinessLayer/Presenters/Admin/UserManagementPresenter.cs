using SrtsWeb.Views.Admin;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Linq;

namespace SrtsWeb.BusinessLayer.Presenters.Admin
{
    public sealed class UserManagementPresenter
    {
        private IUserManagement _view;

        public UserManagementPresenter(IUserManagement view)
        { this._view = view; }

        public void GetIndividuals(String siteId, string modifiedBy)
        {
            if (siteId == null) return;
            var r = new IndividualRepository();
            var l = r.GetAllIndividualsAtSite(siteId, modifiedBy);
            this._view.SrtsIndividuals = l.Where(x => x.PersonalType.ToLower() != "patient").ToList();
        }

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

        public void GetSelectedIndividual(Int32 individualId, String modifiedBy)
        {
            if (individualId.Equals(default(Int32))) return;
            var r = new IndividualRepository();

            var i = r.GetIndividualByIndividualID(individualId, modifiedBy)[0];

            _view.SrtsIndividualSiteCode = i.SiteCodeID;

            if (!_view.mySession.MySite.SiteCode.Equals("ADM001") && !_view.mySession.MySite.SiteCode.Equals("ADM002"))
                i.SiteCodeID = _view.mySession.MySite.SiteCode;

            this._view.SrtsProfileIndividual = i;
        }

        public void GetSelectedIndividual(String userName, String modifiedBy)
        {
            if (userName == null) return;
            var r = new IndividualRepository();
            var i = r.GetIndividualIdByAspnetUserName(userName);
            if (i.Equals(default(Int32))) { _view.SrtsProfileIndividual = null; return; }
            GetSelectedIndividual(i, modifiedBy);
        }

        public void GetSiteData()
        {
            _view.SrtsIndividualSiteCode = _view.mySession.MySite.SiteCode;
        }

        public Int32 InsertIndividualSiteCode(Int32 individualId, String siteCode)
        {
            var r = new IndividualRepository();
            return r.InsertIndividualSiteCode(individualId, siteCode);
        }

        public void GetSyncedIndividual(String userName)
        {
            var r = new IndividualRepository();
            var i = r.GetSyncedUser(userName);
            if (String.IsNullOrEmpty(i)) return;
            _view.SelectedIndividual = i;
        }
    }
}