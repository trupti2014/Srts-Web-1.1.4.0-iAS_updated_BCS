using System.Web;

namespace SrtsWeb
{
    public interface ISiteMapResolver
    {
        SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e);
    }
}