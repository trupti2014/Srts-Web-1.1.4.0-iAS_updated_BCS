using System;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.GEyes
{
    public partial class GeyesMasterResponsive : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Page.Title = "DoD G-Eyes";
                string ver = Convert.ToString(Assembly.GetExecutingAssembly().GetName().Version);
                litVersion.Text = ver;
            }
        }
    }
}