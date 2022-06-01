using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface ISiteCodeRepository
    {
        List<SiteCodeEntity> GetAllSites();

        List<string> GetAllSiteCodes();

        List<SiteCodeEntity> GetSiteBySiteID(string siteID);

        List<SiteCodeEntity> GetTempSiteBySiteID(string siteID);

        List<SiteCodeEntity> GetSitesByType(string siteType);

        List<string> GetSitesByTypeAndRegion(string siteType, int region);

        Boolean InsertSite(SiteCodeEntity sce);

        Boolean UpdateSite(SiteCodeEntity sce);
    }

    public interface ISiteAddressRepository
    {
        List<SiteAddressEntity> GetSiteAddressBySiteID(string siteID);

        Boolean InsertSiteAddress(SiteAddressEntity sae);

        Boolean UpdateSiteAddress(SiteAddressEntity sae);
    }

    public interface ISiteAdministratorRepository
    {
        List<SiteAdministratorEntity> GetSiteAdminInfoBySiteID(string siteID);
    }


}