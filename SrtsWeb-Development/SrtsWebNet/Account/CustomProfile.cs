using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Web;

namespace SrtsWeb.Account
{
    /// <summary>
    /// Custom class used to preform operation on a users profile.  The profile is a custom SRTS profile.
    /// </summary>
    public class CustomProfile
    {
        /// <summary>
        /// Ctor
        /// </summary>
        private CustomProfile()
        {
        }

        /// <summary>
        /// Static method used to get the user profile for the currently logged on user.
        /// </summary>
        /// <returns>SrtsWeb.Entities.UserProfile</returns>
        public static UserProfile GetProfile()
        {
            var r = new ProfileRepository();
            return r.GetProfile(HttpContext.Current.User.Identity.Name);
        }

        /// <summary>
        /// Static method used to get the user profile for a provided user name.
        /// </summary>
        /// <param name="userName">String</param>
        /// <returns>SrtsWeb.Entities.UserProfile</returns>
        public static UserProfile GetProfile(string userName)
        {
            var r = new ProfileRepository();
            return r.GetProfile(userName);
        }

        /// <summary>
        /// Static method used to set the currently logged in site code for a user.  Not the same as the SavePrimarySiteCode method.
        /// </summary>
        /// <param name="profile">SrtsWeb.Entities.UserProfile</param>
        public static void SaveLoggedInSiteCode(UserProfile profile)
        {
            var r = new ProfileRepository();
            r.SaveLoggedInSite(profile.UserName.ToLower(), profile.SiteCode);
        }

        /// <summary>
        /// Static method used to set the available site code list for a user.
        /// </summary>
        /// <param name="profile">SrtsWeb.Entities.UserProfile</param>
        public static void SaveAvailableSiteList(UserProfile profile)
        {
            var r = new ProfileRepository();
            r.SaveAvailableSites(profile.UserName.ToLower(), profile.AvailableSiteList);
        }

        /// <summary>
        /// Static method used to set the primary site code for a user.  Not the same as the SaveLoggedInSiteCode method.
        /// </summary>
        /// <param name="profile">SrtsWeb.Entities.UserProfile</param>
        public static void SavePrimarySiteCode(UserProfile profile)
        {
            var r = new ProfileRepository();
            r.SavePrimarySite(profile);
        }

        /// <summary>
        /// Static method used to remove a site code from the list of available sites for a user.
        /// </summary>
        /// <param name="userName">String</param>
        /// <param name="sitesToRemove">System.Collections.Generic.List of ProfileSiteEntity</param>
        public static void DeleteAvailableSiteCode(string userName, List<ProfileSiteEntity> sitesToRemove)
        {
            var r = new ProfileRepository();
            r.DeleteAvailableSite(userName, sitesToRemove);
        }
    }
}