using SrtsWeb.CustomErrors;
using System;
using System.Security.Permissions;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.Account
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class RoleManagement : System.Web.UI.Page
    {
        private string[] rolesArray2;
        private MembershipUserCollection users;
        private MembershipUser user;
        private string[] usersInRole;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rolesArray2 = Roles.GetAllRoles();
                RolesListBox.Items.Clear();
                RolesListBox.DataSource = rolesArray2;
                RolesListBox.DataBind();

                users = Membership.GetAllUsers();
                UsersListBox.Items.Clear();
                UsersListBox.DataSource = users;
                UsersListBox.DataBind();
            }

            if (RolesListBox.SelectedItem != null)
            {
                usersInRole = Roles.GetUsersInRole(RolesListBox.SelectedItem.Value);
                UsersInRoleGrid.DataSource = usersInRole;
                UsersInRoleGrid.DataBind();
            }
        }

        private bool ValidateProfile(string userName)
        {
            bool passed = false;
            TimeSpan duration = new TimeSpan(-365, 0, 0, 0);
            var profile = CustomProfile.GetProfile(userName);

            if (string.IsNullOrEmpty(profile.FirstName))
            {
                passed = false;
            }
            else
            {
                passed = true;
            }
            return passed;
        }

        public void AddUsers_OnClick(object sender, EventArgs args)
        {
            if (RolesListBox.SelectedItem == null)
            {
                MsgRole.Text = "Please select a role.";
                return;
            }

            if (UsersListBox.SelectedItem == null)
            {
                MsgRole.Text = "Please select one user.";
                return;
            }

            string[] newusers = new string[UsersListBox.GetSelectedIndices().Length];

            for (int i = 0; i < newusers.Length; i++)
            {
                newusers[i] = UsersListBox.Items[UsersListBox.GetSelectedIndices()[i]].Value;
                if (!ValidateProfile(UsersListBox.Items[UsersListBox.GetSelectedIndices()[i]].Value))
                {
                    MsgAdd.Text = "Member does not have either profile or certifications in the system, please add";
                    return;
                }
            }

            try
            {
                Roles.AddUsersToRole(newusers, RolesListBox.SelectedItem.Value);

                usersInRole = Roles.GetUsersInRole(RolesListBox.SelectedItem.Value);
                UsersInRoleGrid.DataSource = usersInRole;
                UsersInRoleGrid.DataBind();
            }
            catch (Exception ex)
            {
                MsgRole.Text = ex.Message.ToString();
            }
        }

        public void UsersInRoleGrid_RemoveFromRole(object sender, GridViewCommandEventArgs args)
        {
            int index = Convert.ToInt32(args.CommandArgument);

            string username = ((DataBoundLiteralControl)UsersInRoleGrid.Rows[index].Cells[0].Controls[0]).Text;

            try
            {
                Roles.RemoveUserFromRole(username, RolesListBox.SelectedItem.Value);
            }
            catch (Exception e)
            {
                ExceptionUtility.LogException(e, "<br />An exception of type " + e.GetType().ToString() + " was encountered removing the user from the role.");
            }

            usersInRole = Roles.GetUsersInRole(RolesListBox.SelectedItem.Value);
            UsersInRoleGrid.DataSource = usersInRole;
            UsersInRoleGrid.DataBind();
        }

        protected void btnFindUser_Click(object sender, EventArgs e)
        {
            user = Membership.GetUser(FindUser.Text);
            UsersListBox.SelectedValue = user.UserName;
        }
    }
}