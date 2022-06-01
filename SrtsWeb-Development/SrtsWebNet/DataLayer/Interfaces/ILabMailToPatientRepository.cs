using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface ILabMailToPatientRepository
    {
        List<SitePrefLabMTPEntity> GetParametersBySiteCode(string siteCode);


        //List<SitePrefLabMTPEntity> GetAllSites();

        //List<string> GetAllSiteCodes();

        //List<SitePrefLabMTPEntity> GetSiteBySiteID(string siteID);

        //List<SitePrefLabMTPEntity> GetTempSiteBySiteID(string siteID);

        //List<SitePrefLabMTPEntity> GetSitesByType(string siteType);

        //List<string> GetSitesByTypeAndRegion(string siteType, int region);

        //Boolean InsertSite(SitePrefLabMTPEntity sce);

        //Boolean UpdateSite(SitePrefLabMTPEntity sce);
    }

    //public interface ISiteAddressRepository
    //{
    //    List<SiteAddressEntity> GetSiteAddressBySiteID(string siteID);

    //    Boolean InsertSiteAddress(SiteAddressEntity sae);

    //    Boolean UpdateSiteAddress(SiteAddressEntity sae);
    //}
}