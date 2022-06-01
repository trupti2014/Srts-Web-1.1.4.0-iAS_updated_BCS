using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Individuals
{
    public interface IIndividualSearchView
    {
        List<LookupTableEntity> LookupCache { get; set; }

        string LastName { get; set; }

        string FirstName { get; set; }

        List<IndividualEntity> SearchData { get; set; }

        string SearchID { get; set; }

        string SelectedSiteValue { get; set; }

        List<SiteCodeEntity> SiteCodes { get; set; }

        List<LookupTableEntity> IndividualTypeDDL { set; }

        string IndividualTypeSelected { get; set; }
    }
}