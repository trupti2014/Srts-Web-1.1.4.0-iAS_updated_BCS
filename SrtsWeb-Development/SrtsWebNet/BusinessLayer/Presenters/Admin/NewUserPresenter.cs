using SrtsWeb.Views.Admin;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Linq;

namespace SrtsWeb.BusinessLayer.Presenters.Admin
{
    public class NewUserPresenter
    {
        private INewUser v;

        public NewUserPresenter(INewUser view)
        {
            this.v = view;
        }

        public void GetIndividuals(String siteId, string modifiedBy)
        {
            if (siteId == null) return;
            var r = new IndividualRepository();
            var l = r.GetAllIndividualsAtSite(siteId, modifiedBy);
            this.v.IndividualList = l.Where(x =>
                x.PersonalType.ToLower() == "technician" ||
                x.PersonalType.ToLower() == "provider" ||
                x.PersonalType.ToLower() == "other").ToList();
        }

        public void GetAllSites()
        {
            var r = new SiteRepository.SiteCodeRepository();
            var scs = r.GetAllSites().Where(x => x.IsActive == true).Distinct().ToList();

            if (scs == null || scs.Count.Equals(0)) return;

            this.v.SourceSiteCodes = scs;
            this.v.DestinationSiteCodes = scs;
        }

        public void SyncUserToIndividual(String userName, Int32 individualId, Boolean forRemove = false)
        {
            if (userName == null || individualId == default(Int32)) return;
            var r = new IndividualRepository();
            r.SyncUserToIndividual(userName, individualId, forRemove);
        }
    }
}