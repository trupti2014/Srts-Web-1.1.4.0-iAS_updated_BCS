using SrtsWeb.Account;
using SrtsWeb.Presenters.Account;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.CustomErrors;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucMembershipUserRoles : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        public void LoadUserList()
        {
            UsersListBox.Items.Clear();
            UsersListBox.DataSource = null;

            UsersListBox.DataTextField = "Key";
            UsersListBox.DataValueField = "Value";
            UsersListBox.DataSource = GetValidUsers();
            UsersListBox.DataBind();
        }

        public void LoadRoleList()
        {
            RolesListBox.Items.Clear();
            RolesListBox.DataSource = GetValidRoles();
            RolesListBox.DataBind();
        }

        public void SetRoles()
        {
            var p = this.UsersListBox.SelectedItem.Text;

            this.lbAssigned.Items.Clear();
            this.lbAssigned.DataSource = Roles.GetRolesForUser(p);
            this.lbAssigned.DataBind();

            var roles = new List<String>();
            if (Roles.IsUserInRole("MgmtEnterprise") || Roles.IsUserInRole("MgmtAdmin"))
                roles = GetMeMaRoles();
            else if (Roles.IsUserInRole("TrainingAdmin"))
                roles = GetTrainingAdminRoles();
            else
                roles = GetValidRoles();

            this.lbAvailable.Items.Clear();
            this.lbAvailable.DataSource = roles.Where(x => !Roles.GetRolesForUser(p).Any(y => y == x));
            this.lbAvailable.DataBind();
        }

        private Dictionary<String, String> GetValidUsers()
        {
            Dictionary<String, String> userSite = new Dictionary<string, string>();
            MembershipUserCollection muc = new MembershipUserCollection();

            var mySiteCd = CustomProfile.GetProfile(Membership.GetUser(false).UserName).Personal.SiteCode;
            var myRoles = Roles.GetRolesForUser().ToList().ConvertAll(x => x.ToLower());

            if (myRoles.Contains("mgmtenterprise")) muc = Membership.GetAllUsers().GetUsersWithProfiles();
            else if (myRoles.Contains("mgmtadmin"))
            {
                var m = Membership.GetAllUsers().GetUsersWithProfiles().Cast<MembershipUser>().Where(x => Roles.GetRolesForUser(x.UserName).Contains("MgmtEnterprise") == false).ToList();
                m.ForEach(x => muc.Add(x));
            }
            else if (myRoles.Contains("trainingadmin"))
            {
                var m = Membership.GetAllUsers().GetUsersWithProfiles().Cast<MembershipUser>().Where(x => Roles.GetRolesForUser(x.UserName).Any(c => c.ToUpper().Contains("CLINIC") || c.ToUpper().Contains("LAB"))).ToList();
                m.ForEach(x => muc.Add(x));
            }
            else muc = GetUsersAtSite(mySiteCd);

            var idx = 0;
            foreach (MembershipUser mu in muc)
            {
                var p = CustomProfile.GetProfile(mu.UserName);
                userSite.Add(mu.UserName, String.Format("{0}-{1}", CustomProfile.GetUserSiteType(p).ToString(), idx.ToString()));
                idx++;
            }

            return userSite;
        }

        private MembershipUserCollection GetUsersAtSite(String siteCode)
        {
            var allUsers = Membership.GetAllUsers().GetUsersWithProfiles();
            MembershipUserCollection muc = new MembershipUserCollection();

            foreach (MembershipUser u in allUsers)
            {
                CustomProfile p = CustomProfile.GetProfile(u.UserName);
                if (p.Personal == null) continue;
                if (p.Personal.SiteCode == null) continue;
                if (p.Personal.SiteCode.ToLower().Equals(siteCode.ToLower()))
                    muc.Add(u);
            }

            return muc;
        }

        private String[] GetUsersInRoleAtSite(String[] usersInRole)
        {
            var validUsers = new List<String>();
            var mySiteCd = CustomProfile.GetProfile(Membership.GetUser(false).UserName).Personal.SiteCode;
            var myRoles = Roles.GetRolesForUser().ToList().ConvertAll(x => x.ToLower());

            if (myRoles.Contains("mgmtenterprise") || myRoles.Contains("mgmtadmin") || myRoles.Contains("trainingadmin")) return usersInRole;
            else
            {
                foreach (var u in usersInRole)
                {
                    CustomProfile p = CustomProfile.GetProfile(u);
                    if (p.Personal == null) continue;
                    if (p.Personal.SiteCode == null) continue;
                    if (p.Personal.SiteCode.ToLower().Equals(mySiteCd.ToLower())) validUsers.Add(u);
                }
            }

            return validUsers.ToArray();
        }

        public void UsersInRoleGrid_RemoveFromRole(object sender, GridViewCommandEventArgs args)
        {
            int index = 0;
            if (!Int32.TryParse(args.CommandArgument.ToString(), out index)) return;

            string username = ((DataBoundLiteralControl)UsersInRoleGrid.Rows[index].Cells[0].Controls[0]).Text;

            try
            {
                Roles.RemoveUserFromRole(username, RolesListBox.SelectedItem.Value);

                AuthorizationPresenter _presenter = new AuthorizationPresenter();
                _presenter.DeleteAuthorizationByUserName(username);

                if (!EventLog.SourceExists("AppLoginLog"))
                {
                    EventLog.CreateEventSource("AppLoginLog", "Application");
                }
                EventLog.WriteEntry("AppLoginLog", "User " + HttpContext.Current.User.Identity.Name.ToString() + " removed " + RolesListBox.SelectedItem.Value.ToString() +
                    " role from " + username.ToString() + " at " + DateTime.Now.ToString(), EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                ExceptionUtility.LogException(e, "<br />An exception of type " + e.GetType().ToString() + " was encountered removing the user from the role.");
            }

            UsersInRoleGrid.DataSource = Roles.GetUsersInRole(RolesListBox.SelectedItem.Value);
            UsersInRoleGrid.DataBind();

            if (this.UsersListBox.SelectedIndex > -1)
                SetRoles();
        }

        protected void UsersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.FindUser.Text = this.UsersListBox.SelectedItem.Text;
            SetRoles();
        }

        protected void RolesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UsersInRoleGrid.DataSource = GetUsersInRoleAtSite(Roles.GetUsersInRole(this.RolesListBox.SelectedValue));
            UsersInRoleGrid.DataBind();
        }

        protected void UsersInRoleGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.UsersInRoleGrid.PageIndex = e.NewPageIndex;
            this.UsersInRoleGrid.DataSource = GetUsersInRoleAtSite(Roles.GetUsersInRole(this.RolesListBox.SelectedValue));
            this.UsersInRoleGrid.DataBind();
        }

        private List<String> GetMeMaRoles()
        {
            var site = this.UsersListBox.SelectedValue.ToLower();
            var roles = new List<String>();

            if (site.Substring(0, 3).Equals("lab"))
                roles = Roles.GetAllRoles().Where(x => x.ToLower().StartsWith("lab") || x.ToLower().StartsWith("mgmt")).ToList();
            else if (site.Substring(0, 6).Equals("clinic"))
                roles = Roles.GetAllRoles().Where(x => x.ToLower().StartsWith("clinic") || x.ToLower().StartsWith("mgmt")).ToList();
            else
                roles = GetValidRoles();

            if (Roles.IsUserInRole("MgmtEnterprise")) return roles;

            roles.Remove("MgmtEnterprise");

            return roles;
        }

        private List<String> GetTrainingAdminRoles()
        {
            return Roles.GetAllRoles().Where(x => 
                x.StartsWith("lab", StringComparison.CurrentCultureIgnoreCase) || 
                x.StartsWith("clinic", StringComparison.CurrentCultureIgnoreCase) ||
                x.StartsWith("training", StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        private List<String> GetValidRoles()
        {
            var myRoles = Roles.GetRolesForUser().ToList().ConvertAll(x => x.ToLower());

            if (myRoles.Contains("mgmtenterprise")) return Roles.GetAllRoles().ToList();
            else if (myRoles.Contains("mgmtadmin")) return Roles.GetAllRoles().Where(x => x.ToLower() != "mgmtenterprise").ToList();
            else if (myRoles.Contains("trainingadmin")) return Roles.GetAllRoles().Where(x => 
                x.StartsWith("lab", StringComparison.CurrentCultureIgnoreCase) || 
                x.StartsWith("clinic", StringComparison.CurrentCultureIgnoreCase) ||
                x.StartsWith("training", StringComparison.CurrentCultureIgnoreCase)).ToList();
            else if (myRoles.Contains("labadmin")) return Roles.GetAllRoles().Where(x => x.StartsWith("lab", StringComparison.CurrentCultureIgnoreCase)).ToList();
            else if (myRoles.Contains("clinicadmin")) return Roles.GetAllRoles().Where(x => x.StartsWith("clinic", StringComparison.CurrentCultureIgnoreCase)).ToList();
            else if (myRoles.Contains("humanadmin")) return Roles.GetAllRoles().Where(x => x.StartsWith("human", StringComparison.CurrentCultureIgnoreCase)).ToList();
            else return new List<String>();
        }

        private List<String> GetSelectedRoles(bool isAdd)
        {
            var roles = new List<String>();
            var idxs = isAdd ? this.lbAvailable.GetSelectedIndices() : this.lbAssigned.GetSelectedIndices();

            foreach (var i in idxs)
                roles.Add(isAdd ? this.lbAvailable.Items[i].Value : this.lbAssigned.Items[i].Value);

            return roles;
        }

        private Boolean CanAddRole(Boolean isAdminAdd)
        {
            foreach (ListItem row in this.lbAssigned.Items)
            {
                if (isAdminAdd)
                {
                    if (!row.Text.ToLower().Contains("admin") && !row.Text.ToLower().Contains("mgmt")) return false;
                }
                else
                {
                    if (row.Text.ToLower().Contains("admin") || row.Text.ToLower().Contains("mgmt")) return false;
                }
            }

            return true;
        }

        private void ShowAlert(String message)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "alert", String.Format("alert('{0}');", message), true);
        }

        private void ShowRoleMsg(Boolean isAdd)
        {
            var msg = String.Empty;
            var roles = GetSelectedRoles(isAdd);

            msg = String.Format("{0} {1} {2} been {3} successfully",
                roles.Count > 1 ? "Roles" : "Role",
                String.Join(", ", roles.ToArray()),
                roles.Count > 1 ? "have" : "has",
                isAdd ? "added" : "removed");

            this.lblRoleMsg.Text = msg;
            this.lblRoleMsg.Visible = true;
        }

        protected void bRemRole_Click_GetUserRoles(object sender, ImageClickEventArgs e)
        {
            if (this.lbAssigned.SelectedIndex.Equals(-1))
            {
                return;
            }
            else
            {
                string[] UserRoles = Roles.GetRolesForUser(this.UsersListBox.SelectedItem.Text.ToString());
                if (UserRoles.Length <= 1)
                {
                    RoleMgmtConfirmRemoval.Visible = true;
                    RoleMgmtConfirmRemoval.Focus();
                }
                else
                {
                    RemRole_RemoveUserRole(sender, e);
                }
            }
        }

        protected void btnRoleMgmtYes_Click(object sender, EventArgs e)
        {
            RemRole_RemoveUserRole(sender, e);
            RoleMgmtConfirmRemoval.Visible = false;
        }

        protected void btnRoleMgmtNo_Click(object sender, EventArgs e)
        {
            RoleMgmtConfirmRemoval.Visible = false;
        }

        protected void RemRole_RemoveUserRole(object sender, EventArgs e)
        {
            if (this.lbAssigned.SelectedIndex.Equals(-1)) return;

            Roles.RemoveUserFromRoles(this.UsersListBox.SelectedItem.Text, GetSelectedRoles(false).ToArray());

            AuthorizationPresenter _presenter = new AuthorizationPresenter();
            _presenter.DeleteAuthorizationByUserName(this.UsersListBox.SelectedItem.Text);

            ShowRoleMsg(false);

            if (!EventLog.SourceExists("AppLoginLog"))
            {
                EventLog.CreateEventSource("AppLoginLog", "Application");
            }
            EventLog.WriteEntry("AppLoginLog", "User " + HttpContext.Current.User.Identity.Name.ToString() + " removed " + this.lbAssigned.SelectedItem.Text.ToString() +
                " role from " + this.UsersListBox.SelectedItem.Text.ToString() + " at " + DateTime.Now.ToString(), EventLogEntryType.Information);

            SetRoles();
            this.RolesListBox.SelectedIndex = -1;
            this.UsersInRoleGrid.DataSource = null;
            this.UsersInRoleGrid.DataBind();
        }

        protected void bAddRole_Click(object sender, ImageClickEventArgs e)
        {
            if (this.lbAvailable.SelectedIndex.Equals(-1)) return;
            if (!CanAddRole(
                this.lbAvailable.SelectedItem.Text.ToLower().Contains("admin") ||
                this.lbAvailable.SelectedItem.Text.ToLower().Contains("mgmt")))
            {
                ShowAlert("An admin role and a non-admin role cannot be assigned together.");
                return;
            }

            string userToAdd = this.UsersListBox.SelectedItem.Text;
            List<String> rolesToAdd = new List<string>();
            rolesToAdd = GetSelectedRoles(true);

            Roles.AddUserToRoles(userToAdd, rolesToAdd.ToArray());

            foreach (string role in rolesToAdd)
            {
                if (!(role.ToLower().Contains("admin") && role.ToLower().Contains("mgmt")))
                {
                    var _itr = new IndividualTypeRepository();
                    IndividualTypeEntity _ite = new IndividualTypeEntity();
                    CustomProfile profile = CustomProfile.GetProfile(userToAdd);

                    var dt = _itr.GetIndividualTypesByIndividualId(profile.Personal.IndividualId);
                    List<int> usersIndivTypes = dt.Select(x => x.TypeId).ToList();// dt.AsEnumerable().Select(r => r.Field<int>("TypeID")).ToList();

                    if (!(usersIndivTypes.Contains(231)))
                    {
                        _ite.IndividualId = profile.Personal.IndividualId;
                        _ite.TypeId = 231;
                        _ite.IsActive = true;
                        _ite.ModifiedBy = HttpContext.Current.User.Identity.Name.ToString();

                        if (_ite.IndividualId != 0)
                        {
                            //_itr.InsertIndividualType(_ite);
                            _itr.InsertIndividualTypes(_ite.IndividualId, _ite.ModifiedBy, "231", false);
                        }
                        else
                        {
                            string error = String.Format("User {0} does not have an IndividualID associated with their ASP Profile!", userToAdd);
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(error));
                        }
                    }
                }
            }

            ShowRoleMsg(true);

            if (!EventLog.SourceExists("AppLoginLog"))
            {
                EventLog.CreateEventSource("AppLoginLog", "Application");
            }
            EventLog.WriteEntry("AppLoginLog", "User " + HttpContext.Current.User.Identity.Name.ToString() + " added " + this.lbAvailable.SelectedItem.Text.ToString() +
                " role to " + this.UsersListBox.SelectedItem.Text.ToString() + " at " + DateTime.Now.ToString(), EventLogEntryType.Information);

            SetRoles();
            this.RolesListBox.SelectedIndex = -1;
            this.UsersInRoleGrid.DataSource = null;
            this.UsersInRoleGrid.DataBind();
        }
    }
}