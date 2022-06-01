using System;
using System.Web.Security;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.DataLayer.Repositories;
using System.Linq;

namespace SrtsWeb.Account
{
    /// <summary>
    /// Custom class used for user membership operations.
    /// </summary>
    public static class AdminUsers
    {
        /// <summary>
        /// Returns a collection of user's at a specified site.  The site must be in the list of available sites in the user's profile.
        /// </summary>
        /// <param name="siteCode">String - Site code used to compare agains the list of available sites in a user's profile.</param>
        /// <returns>MembershipUserCollection -Users in a specified site.</returns>
        public static MembershipUserCollection GetUsersAtSite(String siteCode)
        {
            MembershipUserCollection muc = new MembershipUserCollection();
            try
            {
                var allUsers = Membership.GetAllUsers();

                var r = new UserAccountAdminRepository();
                var siteUsers = r.GetSiteUsersBySite(siteCode);

                foreach (MembershipUser u in allUsers)
                {
                //    var p = CustomProfile.GetProfile(u.UserName);
                //    if (p == null) continue;
                //    if (p.SiteCode == null) continue;
                //    if (p.SiteCode.ToLower().Equals(siteCode.ToLower()))
                    if( siteUsers.Any( username => username.LowerUserName.Equals(u.UserName, StringComparison.InvariantCultureIgnoreCase)))
                        muc.Add(u);
                }  
            }
            catch
            {
                Exception ex = new Exception();
                ex.LogException("In AdminUsers.cs, GetUsersAtSite Method");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return muc;
        }
    }
}