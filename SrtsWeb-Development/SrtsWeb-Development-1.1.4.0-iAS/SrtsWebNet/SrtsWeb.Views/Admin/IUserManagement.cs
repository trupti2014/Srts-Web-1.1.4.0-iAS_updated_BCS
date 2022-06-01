using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface IUserManagement
    {
        SRTSSession mySession { get; set; }

        List<IndividualEntity> LinkIndividuals { get; set; }

        Int32 SelectedIndividualId { get; }

        List<SiteCodeEntity> SiteCodes { get; set; }

        Boolean IsCmsUser { get; set; }

        String IndividualSiteCode { get; set; }

        Boolean IsCacEnabled { get; set; }
    }
}