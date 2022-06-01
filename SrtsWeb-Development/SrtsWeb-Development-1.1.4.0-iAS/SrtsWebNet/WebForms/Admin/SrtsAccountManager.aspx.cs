using SrtsWeb.Account;
using System;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class SrtsAccountManager : System.Web.UI.Page
    {
        // To be removed -- never used
        //private CustomProfile profile = null;
        //private MembershipUserCollection users;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            this.AdminDisableUser1.UserDisabled += AdminDisableUser1_UserDisabled;
            this.AdminEnableUser1.UserEnabled += AdminEnableUser1_UserEnabled;
            Master.CurrentModuleTitle = "Administration - Account Administration";
            Master.uplCurrentModuleTitle.Update();
        }

        protected void AdminDisableUser1_UserDisabled(object sender, EventArgs e)
        {
            this.AdminEnableUser1.LoadListData();
        }

        protected void AdminEnableUser1_UserEnabled(object sender, EventArgs e)
        {
            this.AdminDisableUser1.LoadListData();
        }
    }
}