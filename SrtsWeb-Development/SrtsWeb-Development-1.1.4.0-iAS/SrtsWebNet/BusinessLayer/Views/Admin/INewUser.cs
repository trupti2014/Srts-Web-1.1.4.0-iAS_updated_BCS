using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Views.Admin
{
    public interface INewUser
    {
        List<SiteCodeEntity> SourceSiteCodes { get; set; }
        SiteCodeEntity SelectedSourceSiteCode { get; set; }
        List<SiteCodeEntity> DestinationSiteCodes { get; set; }
        SiteCodeEntity SelectedDestinationSiteCode { get; set; }
        List<IndividualEntity> IndividualList { get; set; }
        IndividualEntity SelectedIndividual { get; set; }
    }
}
