using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Views.Admin
{
    public interface IUserManagement
    {
        SRTSSession mySession { get; set; }

        #region OLD ACCESSORS

        String SelectedIndividual { get; set; }
        List<IndividualEntity> SrtsIndividuals { get; set; }
        String SelectedSiteCode { get; set; }
        List<SiteCodeEntity> SiteCodes { get; set; }

        #endregion OLD ACCESSORS

        #region NEW ACCESSORS

        IndividualEntity SrtsProfileIndividual { get; set; }
        String SrtsIndividualSiteCode { get; set; }

        #endregion NEW ACCESSORS
    }
}