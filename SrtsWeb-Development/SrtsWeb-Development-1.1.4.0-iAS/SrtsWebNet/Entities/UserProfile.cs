using System;
using System.Collections.Generic;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class UserProfile
    {
        public string UserName { get; set; }

        public Boolean IsCmsUser { get; set; }

        public Int32 IndividualId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// This is the site code where the user is actively logged in and working.  This can be different that the Primary Site.
        /// </summary>
        public string SiteCode { get; set; }

        /// <summary>
        /// This is the site code where the user is currently (physically) assigned.
        /// </summary>
        public string PrimarySite { get; set; }

        /// <summary>
        /// This is the list of available sites where the user is allowed to log on and do work.  When one of these is selected it will become the SiteCode.
        /// </summary>
        public List<ProfileSiteEntity> AvailableSiteList { get; set; }

        public UserProfile Clone()
        {
            return this.MemberwiseClone() as UserProfile;
        }
    }
}