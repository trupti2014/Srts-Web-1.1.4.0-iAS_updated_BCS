using System;
using System.Security.Permissions;

namespace SrtsWeb.Account
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class ProfileManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
        }
    }
}