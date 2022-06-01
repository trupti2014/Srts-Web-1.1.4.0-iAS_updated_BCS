using SrtsWeb.Entities;
using System.Data;

namespace SrtsWebTrainingAdmin.Repositories
{
    public interface ISiteCodeRepository
    {
        DataTable InsertSite(SiteCodeEntity sce);

        DataTable InsertSiteAddress(SiteAddressEntity sae);

        bool IsSiteCodeAvailable(string _class);

        //void DeleteClass(string _class);
    }
}