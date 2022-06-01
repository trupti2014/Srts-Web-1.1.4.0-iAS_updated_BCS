using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SrtsWeb.Presenters.Admin
{
    public class SiteAdministratorsPresenter : IDisposable
    {
        private ISiteAdministratorsView v;
        private SiteRepository.SiteCodeRepository r;
        private Boolean disposed = false;
        public SiteAdministratorsPresenter(ISiteAdministratorsView view)
        {
            this.v = view;
        }


        public void GetSites(SiteType sitetype)
        {
            string stype = sitetype.ToString();
            if (stype == "LABORATORY")
                stype = stype.Substring(0, 3);
            r = new SiteRepository.SiteCodeRepository();
            this.v.SiteCodes = r.GetAllSites().Where(x => x.SiteType == stype && x.IsActive == true).ToList(); 
        }


        public List<SiteAdministratorEntity> SearchSiteAdministrators(string sitecode)
        {
            var sar = new  SiteRepository.SiteAdministratorsRepository();
            return sar.GetSiteAdminInfoBySiteID(sitecode);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                if (v != null)
                    v = null;
                if (r != null)
                    r = null;
            }
            disposed = true;
        }


    }
}