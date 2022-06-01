using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class UserManagement : PageBase, INewUser
    {
        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_Page_Load", mySession.MyUserID))
#endif
            {
                if (!this.IsPostBack)
                {
                    DoFormSetup();

                    CurrentModule("Administration - Create New User");
                    CurrentModule_Sub(string.Empty);

                    BuildUserInterface();
                }
            }
        }

        #region FORM EVENTS

        protected void BuildUserInterface()
        {
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_btnSearch_Click", MyUserID))
#endif
            {
                var p = new NewUserPresenter(this);
                p.GetIndividual(this.tbIDNumber.Text.ToString(), MyUserID);

                if (!IndividualList.IsNull())
                {
                    lblName.Text = IndividualList[0].NameFMiL;
                    pnlComplete.Visible = true;
                    searchError.Visible = false;

                    var idN = String.Empty;
                    var idp = new SrtsWeb.Presenters.Admin.NewUserPresenter(this);
                    if (!idp.GetUserDodId(out idN))
                    {
                        this.cbCacEnable.Checked = false;
                        this.cbCacEnable.Visible = false;
                    }
                    else
                    {
                        this.cbCacEnable.Visible = true;
                        this.cbCacEnable.Checked = true;
                    }

                    this.bSubmit.Visible = true;
                    //this.bExit.Visible = true;
                }
                else
                {
                    lblName.Text = "";
                    pnlComplete.Visible = false;
                    searchError.Visible = true;
                }
            }
        }

        protected void bSubmit_Click(object sender, EventArgs e)
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_bSubmit_Click", MyUserID))
#endif
            {
                if (this.IndividualList.IsNull()) return;

                var good = true;
                if (this.lbAssigned.Items.Count.Equals(0))
                {
                    this.lblRoleMsg.Visible = true;
                    good = false;
                }

                if (this.ddlDestinationSiteCodes.SelectedIndex.Equals(0))
                {
                    this.lblDestSiteCodeError.Visible = true;
                    good = false;
                }

                if (String.IsNullOrEmpty(this.tbEmail.Text))
                {
                    this.lblEmailError.Visible = true;
                    good = false;
                }

                if (!good) return;

                try
                {
                    this.UserName = GenUserName();
                    this.Password = Helpers.GetRandomPwd(Membership.MinRequiredPasswordLength);

                    // Create the user
                    Membership.CreateUser(this.UserName, this.Password, this.Email);

                    // Create the profile
                    var p = new UserProfile()
                    {
                        UserName = this.UserName,
                        IndividualId = this.IndividualList[0].ID,

                        SiteCode = this.SelectedDestinationSiteCode.SiteCode,
                        PrimarySite = this.SelectedDestinationSiteCode.SiteCode,
                        IsCmsUser = this.IsCms.ToLower().Equals("yes"),
                        AvailableSiteList = new List<ProfileSiteEntity>() { new ProfileSiteEntity() { Approved = true, SiteCode = this.SelectedDestinationSiteCode.SiteCode } }
                    };

                    CustomProfile.SavePrimarySiteCode(p);
                    CustomProfile.SaveLoggedInSiteCode(p);
                    CustomProfile.SaveAvailableSiteList(p);

                    // Add the roles
                    Roles.AddUserToRoles(this.UserName, this.RolesList.ToArray());

                    if (this.cbCacEnable.Checked.Equals(false))
                    {
                        try
                        {
                            var m = new SrtsWeb.BusinessLayer.Concrete.MailService();
                            m.SendEmail(String.Format("Password: {0}", this.Password), ConfigurationManager.AppSettings["FromEmail"], new List<String>() { this.Email }, "SRTS Web Acct");
                        }
                        catch (Exception ex) { ex.TraceErrorException(); }
                    }
                    else
                        CacEnableUser();

                    // Send email to admins to notify them of user creation.
                    try
                    {
                        var role = String.Empty;
                        switch (this.SelectedDestinationSiteCode.SiteType.ToLower())
                        {
                            case "clinic":
                                role = "CLINICADMIN";
                                break;

                            case "lab":
                                role = "LABADMIN";
                                break;

                            case "admin":
                                role = "MGMTENTERPRISE";
                                break;
                        }
                        UserNotificationService.SendNewUserEmails(p.PrimarySite, role, p.UserName, MyUserID);
                    }
                    catch (Exception ex) { ex.TraceErrorException(); }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    LogEvent(String.Format("User {0} unsuccessfully added new user {1} at {2}.", MyUserID, this.UserName, DateTime.Now));
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "FailConfirm", "Confirm('Error creating new user, please try again or contact the SRTS Web admins.', 'divError', true, true);", true);
                    return;
                }

                // Fill items on the success dialog
                this.lblUserName.Text = this.UserName;
                this.lblPassword.Text = this.Password;
                this.lblFirstName.Text = this.IndividualList[0].FirstName;
                this.lblLastName.Text = this.IndividualList[0].LastName;
                this.lblLocation.Text = this.SelectedDestinationSiteCode.SiteCombinationProfile;
                this.lblRoles.Text = this.RolesCsv;
                this.lblEmail.Text = this.Email.ToHtmlEncodeString();
                this.lblIsCms.Text = this.IsCms;
                this.lblCacEnable.Text = this.IsCacEnabled;

                LogEvent(String.Format("User {0} added new user {1} at {2}.", MyUserID, this.UserName, DateTime.Now));
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "DoSuccessDialog();", true);
            }
        }

        protected void bExit_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/default.aspx", true);
        }

        protected void ddlDestinationSiteCodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoggedInUserRoles = Roles.GetRolesForUser().ToList();
            LoadRoles();
            FilterRoles();
        }

        protected void bAddRole_Click(object sender, ImageClickEventArgs e)
        {
            if (!IsRolesAddable(this.lbAvailable.GetSelectedIndices())) return;

            this.lbAvailable.GetSelectedIndices().ToList().ForEach(x => this.lbAssigned.Items.Add(this.lbAvailable.Items[x]));
            var idx = this.lbAvailable.GetSelectedIndices();
            for (var i = idx.Length - 1; i >= 0; i--)
                this.lbAvailable.Items.RemoveAt(idx[i]);
            this.lbAssigned.Sort();
            SetRoleErrorMessage(this.lbAssigned.Items.Equals(0) ? "* The new user must have at least one assigned role." : String.Empty);
            this.bAddRole.Enabled = false;
        }

        protected void bRemRole_Click(object sender, ImageClickEventArgs e)
        {
            this.lbAssigned.GetSelectedIndices().ToList().ForEach(x => this.lbAvailable.Items.Add(this.lbAssigned.Items[x]));
            var idx = this.lbAssigned.GetSelectedIndices();
            for (var i = idx.Length - 1; i >= 0; i--)
                this.lbAssigned.Items.RemoveAt(idx[i]);
            this.lbAvailable.Sort();
            SetRoleErrorMessage(this.lbAssigned.Items.Equals(0) ? "* The new user must have at least one assigned role." : String.Empty);
            this.bAddRole.Enabled = this.lbAssigned.Items.Count.Equals(0);
        }

        #endregion FORM EVENTS

        #region CUSTOM METHODS

        private void DoFormSetup()
        {
            try
            {
                var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
                var MySiteCode = string.IsNullOrEmpty(this.mySession.MySite.SiteCode) ? Globals.SiteCode : this.mySession.MySite.SiteCode;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_DoFormSetup", MyUserID))
#endif
                {
                    // Populate sites
                    LoadSiteCodes();
                    var sc = MySiteCode;
                    this.SelectedDestinationSiteCode = this.DestinationSiteCodes.FirstOrDefault(x => x.SiteCode == sc);

                    // Load user selection
                    SetDestinationSiteState();
                    this.cbCacEnable.Visible = false;

                    // Populate roles
                    this.LoggedInUserRoles = Roles.GetRolesForUser().ToList();
                    LoadRoles();
                    FilterRoles();

                    // Hide Submit and Cancel buttons till a successful search is complete
                    this.bSubmit.Visible = false;
                    //this.bExit.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Helpers.LogElmahMessage(ex + "Error in DoFormSetup.  SiteCode - " + this.mySession.MySite.SiteCode + "UserID = " + mySession.MyUserID);
            }
        }

        private Boolean IsRolesAddable(Int32[] roleIdxs)
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_IsRolesAddable", MyUserID))
#endif
            {
                var lst = new List<ListItem>();
                roleIdxs.ToList().ForEach(x => lst.Add(this.lbAvailable.Items[x]));
                var hasAdmin = lst.Any(x => x.Text.ToLower().Contains("mgmt") || x.Text.ToLower().Contains("admin"));
                var nonAdmin = lst.Any(x => !x.Text.ToLower().Contains("mgmt") && !x.Text.ToLower().Contains("admin"));
                if (hasAdmin && nonAdmin)
                {
                    SetRoleErrorMessage("* The new user cannot have an admin role and a non-admin role assigned together.");
                    return false;
                }
                if (this.lbAssigned.Items.Count.Equals(0))
                {
                    SetRoleErrorMessage(String.Empty);
                    return true;
                }
                if (!hasAdmin)
                {
                    if (this.lbAssigned.GetItemsList().Any(x => x.Text.ToLower().Contains("mgmt") || x.Text.ToLower().Contains("admin")).Equals(true))
                    {
                        SetRoleErrorMessage("* The new user cannot have an admin role and a non-admin role assigned together.");
                        return false;
                    }
                }
                else
                {
                    if (this.lbAssigned.GetItemsList().Any(x => !x.Text.ToLower().Contains("mgmt") && !x.Text.ToLower().Contains("admin")).Equals(true))
                    {
                        SetRoleErrorMessage("* The new user cannot have an admin role and a non-admin role assigned together.");
                        return false;
                    }
                }

                SetRoleErrorMessage(String.Empty);
                return true;
            }
        }

        private void SetRoleErrorMessage(String Message)
        {
            this.lblRoleMsg.Text = Message;
            this.lblRoleMsg.Visible = !String.IsNullOrEmpty(Message);
        }

        private void SetDestinationSiteState()
        {
            this.ddlDestinationSiteCodes.Enabled = !Roles.GetRolesForUser().Any(x =>
                x.StartsWith("labadmin", StringComparison.CurrentCultureIgnoreCase) ||
                x.StartsWith("clinicadmin", StringComparison.CurrentCultureIgnoreCase));
        }

        private void FilterRoles()
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_FilterRoles", MyUserID))
#endif
            {
                var s = this.SelectedDestinationSiteCode.SiteType;
                var rs = new List<ListItem>();
                switch (s.ToLower())
                {
                    case "lab":
                        rs = this.lbAvailable.GetItemsList().Where(x => x.Value.StartsWith("clinic", StringComparison.CurrentCultureIgnoreCase) == false).ToList();
                        break;

                    case "clinic":
                        rs = this.lbAvailable.GetItemsList().Where(x => x.Value.StartsWith("lab", StringComparison.CurrentCultureIgnoreCase) == false).ToList();
                        break;

                    default:
                        rs = this.lbAvailable.GetItemsList();
                        break;
                }
                this.lbAvailable.Items.Clear();
                rs.ForEach(x => this.lbAvailable.Items.Add(x));
            }
        }

        private void LoadRoles()
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_LoadRoles", MyUserID))
#endif
            {
                this.lbAvailable.Items.Clear();

                if (this.LoggedInUserRoles.Contains("MgmtEnterprise"))
                {
                    this.lbAvailable.DataSource = Roles.GetAllRoles();
                }
                else if (this.LoggedInUserRoles.Contains("MgmtAdmin"))
                {
                    this.lbAvailable.DataSource = Roles.GetAllRoles().ToList().Where(x => x.ToLower() != "mgmtenterprise");
                }
                else if (this.LoggedInUserRoles.Contains("TrainingAdmin"))
                {
                    this.lbAvailable.DataSource = Roles.GetAllRoles().ToList().Where(x => x.ToLower() != "mgmtenterprise" && x.ToLower() != "mgmtadmin");
                }
                else if (this.LoggedInUserRoles.Contains("ClinicAdmin"))
                {
                    this.lbAvailable.DataSource = Roles.GetAllRoles().ToList().Where(x => x.ToLower().StartsWith("clinic"));
                }
                else if (this.LoggedInUserRoles.Contains("LabAdmin"))
                {
                    this.lbAvailable.DataSource = Roles.GetAllRoles().ToList().Where(x => x.ToLower().StartsWith("lab"));
                }
                else if (this.LoggedInUserRoles.Contains("HumanAdmin"))
                {
                    this.lbAvailable.DataSource = Roles.GetAllRoles().ToList().Where(x => x.ToLower().StartsWith("human"));
                }

                this.lbAvailable.DataBind();
            }
        }

        private void LoadSiteCodes()
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_", MyUserID))
#endif
            {
                var p = new NewUserPresenter(this);
                p.GetAllSites();
            }
        }

        private String GenUserName()
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_GenUserName", MyUserID))
#endif
            {
                var rx = new Regex("[^a-zA-Z]");
                var isTrainingUser = false;
                var siteName = String.Empty;

                isTrainingUser = ConfigurationManager.AppSettings["TrainingSites"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Any(x =>
                    HttpContext.Current.User.Identity.Name.StartsWith(x, StringComparison.CurrentCultureIgnoreCase));

                if (isTrainingUser)
                {
                    siteName = ConfigurationManager.AppSettings["TrainingSites"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault(x => HttpContext.Current.User.Identity.Name.StartsWith(x, StringComparison.CurrentCultureIgnoreCase));
                }

                var un = !isTrainingUser ?
                    String.Format("{0}.{1}.{2}", this.IndividualList[0].FirstName.Substring(0, 1), rx.Replace(this.IndividualList[0].LastName, ""), AbbreviateRole(this.RolesList[0])) :
                    String.Format("{0}.{1}.{2}.{3}", siteName, this.IndividualList[0].FirstName.Substring(0, 1), rx.Replace(this.IndividualList[0].LastName, ""), AbbreviateRole(this.RolesList[0]));

                if (IsUserNameAvailable(un)) return un;

                var idx = 1;
                do
                {
                    un = !isTrainingUser ?
                        String.Format("{0}.{1}{2}.{3}", this.IndividualList[0].FirstName.Substring(0, 1),
                        rx.Replace(this.IndividualList[0].LastName, ""), idx.ToString(), AbbreviateRole(this.RolesList[0])) :
                        String.Format("{0}.{1}.{2}{3}.{4}", siteName, this.IndividualList[0].FirstName.Substring(0, 1),
                        rx.Replace(this.IndividualList[0].LastName, ""), idx.ToString(), AbbreviateRole(this.RolesList[0]));

                    idx++;
                } while (!IsUserNameAvailable(un));

                return un;
            }
        }

        private String AbbreviateRole(String roleName)
        {
            switch (roleName.ToLower())
            {
                case "clinictech":
                    return "CT";

                case "clinicclerk":
                    return "CC";

                case "clinicadmin":
                    return "CA";

                case "clinicprovider":
                    return "CP";

                case "labtech":
                    return "LT";

                case "labclerk":
                    return "LC";

                case "labmail":
                    return "LM";

                case "labadmin":
                    return "LA";

                case "mgmtenterprise":
                    return "ME";

                case "mgmtadmin":
                    return "MA";

                case "mgmtdatamgmt":
                    return "MD";

                case "mgmtreport":
                    return "MR";

                case "trainingadmin":
                    return "TA";

                default:
                    return "user";
            }
        }

        private Boolean IsUserNameAvailable(String userName)
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_IsUserNameAvailable", MyUserID))
#endif
            {
                var u = Membership.GetUser(userName);
                return u.IsNull();
            }
        }

        public void CacEnableUser()
        {
            var MyUserID = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "UserManagement_CacEnableUser", MyUserID))
#endif
            {
                Membership.GetUser(this.UserName).ChangePassword(this.Password, Helpers.GetRandomPwd(Membership.MinRequiredPasswordLength));

                var a = new SrtsWeb.Presenters.Account.AuthorizationPresenter();
                a.InsertAuthorizationByUserName(this.UserName);

                var p = new SrtsWeb.Presenters.Account.CertificateInfoPresenter();
                // Get DodIdNumber
                var din = String.Empty;
                var idp = new SrtsWeb.Presenters.Admin.NewUserPresenter(this);
                idp.GetUserDodId(out din);

                p.UpdateAuthorizationCacInfoByUserName(this.UserName, din, /*this.RolesList[0].EndsWith("Admin") || this.RolesList[0].StartsWith("Mgmt") ? */"DOD"/* : "DOD EMAIL"*/);
            }
        }

        #endregion CUSTOM METHODS

        #region LOCAL PROPERTIES

        public List<String> LoggedInUserRoles { get; set; }

        public String UserName { get { return ViewState["UserName"].ToString(); } set { ViewState["UserName"] = value; } }

        public String Password { get { return ViewState["skoal"].ToString(); } set { ViewState["skoal"] = value; } }

        public String Email { get { return this.tbEmail.Text; } }

        public String IsCms { get { return this.cbIsCms.Checked ? "Yes" : "No"; } }

        public String IsCacEnabled { get { return this.cbCacEnable.Checked ? "Yes" : "No"; } }

        public List<String> RolesList { get { return this.lbAssigned.GetItemsList().Select(x => x.Value).ToList(); } }

        public String RolesCsv { get { return String.Join(", ", this.RolesList); } }

        #endregion LOCAL PROPERTIES

        #region INTERFACE MEMBERS

        public List<SiteCodeEntity> DestinationSiteCodes
        {
            get
            {
                return ViewState["DestinationSiteCodes"] as List<SiteCodeEntity>;
            }
            set
            {
                ViewState["DestinationSiteCodes"] = value;
                this.ddlDestinationSiteCodes.Items.Clear();
                this.ddlDestinationSiteCodes.SelectedIndex = -1;
                this.ddlDestinationSiteCodes.SelectedValue = null;
                this.ddlDestinationSiteCodes.ClearSelection();
                this.ddlDestinationSiteCodes.DataTextField = "SiteCombinationProfile";
                this.ddlDestinationSiteCodes.DataValueField = "SiteCode";
                this.ddlDestinationSiteCodes.DataSource = value;
                this.ddlDestinationSiteCodes.DataBind();
                this.ddlDestinationSiteCodes.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlDestinationSiteCodes.SelectedIndex = 0;
            }
        }

        public SiteCodeEntity SelectedDestinationSiteCode
        {
            get
            {
                return this.DestinationSiteCodes.FirstOrDefault(x => x.SiteCode == this.ddlDestinationSiteCodes.SelectedValue);
            }
            set
            {
                try
                {
                    this.ddlDestinationSiteCodes.SelectedValue = value.SiteCode;
                }
                catch
                {
                    this.ddlDestinationSiteCodes.SelectedIndex = -1;
                }
            }
        }

        public List<IndividualEntity> IndividualList
        {
            get
            {
                return ViewState["IndividualList"] as List<IndividualEntity>;
            }
            set
            {
                ViewState["IndividualList"] = value;
            }
        }

        #endregion INTERFACE MEMBERS
    }
}