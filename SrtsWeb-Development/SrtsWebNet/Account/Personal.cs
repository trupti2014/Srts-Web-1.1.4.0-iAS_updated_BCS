using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Account
{
    [Serializable]
    public class Personal
    {
        public Int32 IndividualId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Lastname { get; set; }

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
    }
}