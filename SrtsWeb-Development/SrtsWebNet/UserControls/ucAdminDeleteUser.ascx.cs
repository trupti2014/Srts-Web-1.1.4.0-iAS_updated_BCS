using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using SrtsWeb.CustomErrors;
using SrtsWeb.Account;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    public partial class ucAdminDeleteUser : UserControlBase
    {
        protected String defaultText = "No users to delete";
        ListItem selected;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                selected = this.lbUsers.SelectedItem;

            LoadListData();

            if (selected == null) return;
            this.lbUsers.Items.FindByText(selected.Text).Selected = true;
        }
        protected void bSubmitChanges_Click(object sender, EventArgs e)
        {
            var un = this.lbUsers.SelectedValue;
            Membership.DeleteUser(un, true);

            LoadListData();

            // Once a user has been deleted refresh the Unlock User UC by running the LoadListData() method.
            var uc1 = FindUserControl<ucAdminDisableUser>();
            if (uc1 != null)
                uc1.LoadListData();

            var uc2 = FindUserControl<ucAdminEnableUser>();
            if (uc2 != null)
                uc2.LoadListData();

            var uc3 = FindUserControl<ucAdminUnlockUser>();
            if (uc3 != null)
                uc3.LoadListData();

            // Show an alert for confirmation of success
            ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "",
                String.Format("alert('User {0} has been successfully deleted.');", un), true);

        }
        private void LoadListData()
        {
            this.lbUsers.Items.Clear();
            this.lbUsers.DataSource = null;

            this.lbUsers.DataTextField = "UserName";
            this.lbUsers.DataValueField = "UserName";
            var rs = Roles.GetRolesForUser();
            
            var u = rs.Contains("MgmtEnterprise") || rs.Contains("MgmtAdmin") ?
                Membership.GetAllUsers() : AdminUsers.GetUsersAtSite(mySession.MySite.SiteCode);

            if (u.Count.Equals(0))
                this.lbUsers.Items.Add(defaultText);
            else
            {
                this.lbUsers.DataSource = u;
                this.lbUsers.DataBind();
            }
        }
        private T FindUserControl<T>()
            where T : UserControl
        {
            foreach (var c in Parent.Controls)
            {
                if (c is T)
                    return (T)c;
            }

            return null;
        }
    }
}