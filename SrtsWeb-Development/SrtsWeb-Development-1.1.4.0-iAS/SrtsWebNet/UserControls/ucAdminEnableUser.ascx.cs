using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Linq;
using System.Security.Permissions;
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
    public partial class ucAdminEnableUser : UserControlBase
    {
        public event EventHandler UserEnabled;

        protected String defaultText = "No users to enable";
        private ListItem selected;

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
            try
            {
                var un = this.lbUsers.SelectedItem.ToString();
                var u = Membership.GetUser(un);
                u.IsApproved = true;
                Membership.UpdateUser(u);

                LoadListData();

                UserEnabled(sender, e);

                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "",
                    String.Format("alert('User {0} has been successfully enabled.');", u.UserName), true);
                LogEvent(String.Format("User {0} successfully enabled account for {1} at {2}.", mySession.MyUserID, un, DateTime.Now));

                SendAdminEnail(un);
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        public void LoadListData()
        {
            this.lbUsers.Items.Clear();
            this.lbUsers.DataSource = null;

            this.lbUsers.DataTextField = "UserName";
            this.lbUsers.DataValueField = "UserName";

            var u = new MembershipUserCollection();

            if (Roles.IsUserInRole("MgmtEnterprise") || Roles.IsUserInRole("MgmtAdmin"))
            {
                u = Membership.GetAllUsers();
                if (!Roles.IsUserInRole("MgmtEnterprise"))
                {
                    var uir = Roles.GetUsersInRole("MgmtEnterprise").ToList();
                    uir.ForEach(x => u.Remove(x));
                }
            }
            else if (Roles.IsUserInRole("TrainingAdmin"))
            {
                var m = Membership.GetAllUsers().GetUsersWithProfiles().Cast<MembershipUser>().Where(x => Roles.GetRolesForUser(x.UserName).Any(c =>
                    c.StartsWith("CLINIC", StringComparison.CurrentCultureIgnoreCase) ||
                    c.StartsWith("LAB", StringComparison.CurrentCultureIgnoreCase) ||
                    c.StartsWith("TRAINING", StringComparison.CurrentCultureIgnoreCase))).ToList();
                m.ForEach(x => u.Add(x));
            }
            else
            {
                u = AdminUsers.GetUsersAtSite(mySession.MySite.SiteCode);
            }

            var users = u.Cast<MembershipUser>().Where(x => x.IsApproved == false).ToList();
            u = null;

            if (users.Count.Equals(0))
                this.lbUsers.Items.Add(defaultText);
            else
            {
                this.lbUsers.DataSource = users;
                this.lbUsers.DataBind();
            }
        }

        private void SendAdminEnail(String userName)
        {
            var lab = new String[] { "M", "S" };
            var p = CustomProfile.GetProfile(userName);
            p.AvailableSiteList.ForEach(x =>
            {
                var s = x.SiteCode.Substring(0, 1);
                var r = lab.Contains(s) ? "LABADMIN" : s == "A" ? "MGMTENTERPRISE" : "CLINICADMIN";
                UserNotificationService.SendEnabledUserAdminEmail(x.SiteCode, r, userName, mySession.MyUserID);
            });
        }
    }
}