using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;

namespace SrtsWeb.Account
{
    /// <summary>
    /// Custom class used to get data for the currently logged in user.
    /// </summary>
    [Serializable]
    public class CurrentUser
    {
        private UserProfile _CurrentUserPersonalInfo;
        private MembershipUser _CurrentUserMembership;
        private string[] _CurrentUserRoles;
        private List<SiteCodeEntity> _CurrentUserSiteInfo;

        /// <summary>
        /// Custom UserProfile for the currently logged in user.
        /// </summary>
        public UserProfile UserPersonalInfo
        {
            get
            {
                return _CurrentUserPersonalInfo;
            }
            set
            {
                _CurrentUserPersonalInfo = value;
            }
        }

        /// <summary>
        /// Membership data for currently logged in user.
        /// </summary>
        public MembershipUser UserMembership
        {
            get
            {
                return _CurrentUserMembership;
            }
            set
            {
                _CurrentUserMembership = value;
            }
        }

        /// <summary>
        /// List of roles the currently logged on user is assigned to.
        /// </summary>
        public string[] UserRoles
        {
            get
            {
                return _CurrentUserRoles;
            }
            set
            {
                _CurrentUserRoles = value;
            }
        }

        /// <summary>
        /// Even though this is a list it is actually just a single SiteCodeEntity for the actively logged in site for the current user.
        /// </summary>
        public List<SiteCodeEntity> UserSiteInfo
        {
            get
            {
                return _CurrentUserSiteInfo;
            }
            set
            {
                _CurrentUserSiteInfo = value;
            }
        }

        /// <summary>
        /// Constructor used for the currently logged in user to fill public properties: UserPersonalInfo, UserMembership, UserRoles, and UserSiteInfo.
        /// </summary>
        public CurrentUser()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var profile = CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name);
                var siteCodeRepository = new SiteRepository.SiteCodeRepository();

                try
                {
                    UserMembership = Membership.GetUser(profile.UserName);
                    UserRoles = Roles.GetRolesForUser(profile.UserName);
                    UserPersonalInfo = CustomProfile.GetProfile(profile.UserName);
                    if (UserPersonalInfo.SiteCode != null)
                    {
                        UserSiteInfo = siteCodeRepository.GetSiteBySiteID(UserPersonalInfo.SiteCode);
                    }
                    else
                    {
                        UserPersonalInfo.SiteCode = "000147";
                        UserSiteInfo = siteCodeRepository.GetSiteBySiteID(UserPersonalInfo.SiteCode);
                    }
                }
                catch (ArgumentNullException)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
            }
        }
    }
}