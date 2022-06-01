using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IProfileRepository
    {
        void SavePrimarySite(UserProfile profile);

        void SaveAvailableSites(String userName, List<ProfileSiteEntity> availableSites);

        UserProfile GetProfile(String userName);

        void SaveLoggedInSite(String userName, String siteCode);

        void DeleteAvailableSite(String userName, List<ProfileSiteEntity> removeSites);
    }
}