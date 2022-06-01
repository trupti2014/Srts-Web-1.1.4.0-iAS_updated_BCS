using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface INewUser
    {
        SRTSSession mySession { get; set; }

        //List<SiteCodeEntity> SourceSiteCodes { get; set; }

        //SiteCodeEntity SelectedSourceSiteCode { get; set; }

        List<SiteCodeEntity> DestinationSiteCodes { get; set; }

        SiteCodeEntity SelectedDestinationSiteCode { get; set; }

        List<IndividualEntity> IndividualList { get; set; }

        //IndividualEntity SelectedIndividual { get; set; }
    }
}