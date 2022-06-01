using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
    public partial class SrtsUserManager : PageBase, IUserManagement
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (mySession == null)
                mySession = new SRTSSession();

            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_Page_Load", mySession.MyUserID))
#endif
            {
                var good = true;
                if (!IsPostBack)
                {
                    try
                    {
                        BuildPageTitle();
                        this.divSearchUser.Visible = true;
                        this.divUser.Visible = false;
                        this.divCms.Visible = false;
                        this.divApprove.Visible = false;

                        var p = new UserManagementPresenter(this);
                        p.GetAllSites();
                    }
                    catch (Exception ex)
                    {
                        ex.LogException();
                        good = false;
                        Response.Redirect(FormsAuthentication.DefaultUrl, false);
                    }
                }

                if (!good)
                    if (this.SiteCodes.IsNullOrEmpty())
                    {
                        try
                        {
                            var p = new UserManagementPresenter(this);
                            p.GetAllSites();
                        }
                        catch (Exception ex)
                        {
                            ex.LogException();
                            Response.Redirect(FormsAuthentication.DefaultUrl, false);
                        }
                    }

                this.tbName.Focus();
            }
        }
        private void BuildPageTitle()
        {
            try
            {
                mySession.CurrentModule = "Administration - User Manager";
                Master.CurrentModuleTitle = mySession.CurrentModule;
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                mySession.CurrentModule = "Administration - User Manager";
                mySession.CurrentModule_Sub = string.Empty;
            }
        }
        #region PAGE EVENTS

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            DoCancelAction();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var ModifiedBy = string.IsNullOrEmpty(mySession.ModifiedBy) ? Globals.ModifiedBy : mySession.ModifiedBy;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_btnSubmit_Click", ModifiedBy))
#endif
                {
                    var modifications = new System.Text.StringBuilder();

                    // Update Profile ************************************************************************************************************************************************
                    var prf = this.UserProfile.Clone();

                    if (this.IsClinicLabAdmin && this.divApprove.Visible)
                    {
                        if (this.UserProfile.AvailableSiteList.IsNullOrEmpty())
                        {
                            this.UserProfile.AvailableSiteList.Add(new ProfileSiteEntity()
                            {
                                SiteCode = this.mySession.MyClinicCode,
                                Approved = this.cbApprove.Checked
                            });
                            modifications.AppendFormat("New site, {0}, added and approved: {1}.{2}", this.mySession.MyClinicCode, this.cbApprove.Checked, Environment.NewLine);
                        }
                        else
                        {
                            this.UserProfile.AvailableSiteList.FirstOrDefault(x => x.SiteCode == this.mySession.MyClinicCode).Approved = this.cbApprove.Checked;
                            modifications.AppendFormat("Site, {0}, approved: {1}.{2}", this.mySession.MyClinicCode, this.cbApprove.Checked, Environment.NewLine);
                        }

                        CustomProfile.SaveAvailableSiteList(this.UserProfile);

                        if (!this.mySession.MyClinicCode.Equals(String.IsNullOrEmpty(this.UserProfile.PrimarySite) ? this.UserProfile.SiteCode : this.UserProfile.PrimarySite))
                        {
                            LogEvent(String.Format("User {0} updated user profile for {1} at {2}.", ModifiedBy, this.SelectedUser.UserName, DateTime.Now));
                            ShowConfirmDialog(String.Format("Successfully updated user {0}.", this.SelectedUser.UserName));
                            return;
                        }
                    }

                    prf.AvailableSiteList = this.SelectedSiteCodes;
                    prf.PrimarySite = this.ddlPrimarySiteSelection.Enabled ? this.ddlPrimarySiteSelection.SelectedValue : this.IndividualSiteCode;
                    if (!prf.PrimarySite.Equals(this.UserProfile.PrimarySite))
                        modifications.AppendFormat("Primary site changed to: {0}.{1}", this.mySession.MyClinicCode, Environment.NewLine);

                    if (!this.SelectedSiteCodes.Any(x => x.SiteCode == prf.PrimarySite))
                        prf.AvailableSiteList.Add(new ProfileSiteEntity() { SiteCode = prf.PrimarySite, Approved = false });

                    if (prf.AvailableSiteList.All(x => x.Approved == false))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DoFail", "Confirm('Users are required to have at least one approved site.', 'divUserManagerStatus', true, true);", true);
                        return;
                    }

                    prf.SiteCode = this.SelectedSiteCodes.Any(x => x.SiteCode == this.IndividualSiteCode) ? this.IndividualSiteCode : this.SelectedSiteCodes.FirstOrDefault().SiteCode;

                    // Get list of sites to remove
                    var remSites = new List<ProfileSiteEntity>();
                    this.UserProfile.AvailableSiteList.ForEach(x =>
                    {
                        if (!prf.AvailableSiteList.Any(a => a.SiteCode == x.SiteCode))
                            remSites.Add(x);
                    });

                    // Get a list of sites that are new
                    var addSites = new List<ProfileSiteEntity>();
                    prf.AvailableSiteList.ForEach(x =>
                    {
                        if (this.UserProfile.AvailableSiteList.Any(a => a.SiteCode == x.SiteCode) == false)
                            addSites.Add(x);
                    });
                    if (addSites.Count.GreaterThan(0))
                        modifications.AppendFormat("Added site(s): {0}.{1}", String.Join(", ", addSites.Select(x => x.SiteCode)), Environment.NewLine);
                    addSites = null;

                    CustomProfile.SavePrimarySiteCode(prf);
                    CustomProfile.SaveLoggedInSiteCode(prf);

                    if (remSites.Count.GreaterThan(0))
                    {
                        CustomProfile.DeleteAvailableSiteCode(prf.UserName, remSites);
                        modifications.AppendFormat("Removed site(s): {0}.{1}", String.Join(", ", remSites.Select(x => x.SiteCode)), Environment.NewLine);
                    }

                    CustomProfile.SaveAvailableSiteList(prf);
                    // End Update Profile ********************************************************************************************************************************************


                    // Update Membership Email
                    if (!this.SelectedUser.Email.ToLower().Equals(this.tbProfileEmail.Text.ToLower()))
                    {
                        this.SelectedUser.Email = this.tbProfileEmail.Text;
                        Membership.UpdateUser(this.SelectedUser);
                        modifications.AppendLine("Changed users email address.");
                    }

                    // Update CMS
                    if (this.divCms.Visible)
                    {
                        var p = new UserManagementPresenter(this);
                        p.SyncUserToIndividual(this.SelectedUser.UserName, this.UserProfile.IndividualId, !this.cbCms.Checked);
                        modifications.AppendFormat("Set users CMS flag to: {0}.{1}", this.cbCms.Checked, Environment.NewLine);
                    }

                    if (this.RolesVisible)
                    {
                        // Update Roles
                        var ur = Roles.GetRolesForUser(this.SelectedUser.UserName);

                        var killSession = ur.Any(x => this.lbAssigned.Items.Cast<ListItem>().Any(a => a.Value == x) == false);

                        if (killSession)
                        {
                            var nSites = this.lbAssigned.Items.Cast<ListItem>().Select(x => x.Value).ToArray();

                            modifications.AppendFormat("User roles have changed.{0}Old Role(s): {1}{2}New Role(s): {3}", Environment.NewLine, String.Join(", ", ur), Environment.NewLine, String.Join(", ", nSites));
                            if (!ur.ToList().IsNullOrEmpty())
                                Roles.RemoveUserFromRoles(this.SelectedUser.UserName, ur);
                            Roles.AddUserToRoles(this.SelectedUser.UserName, nSites);

                            SessionService.KillUserSessions(this.SelectedUser.UserName);
                            Exception ex = new Exception();
                            ex.LogException("SessionService.KillUserSessions was called for user:  " + this.SelectedUser.UserName + ".");
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        }
                    }

                    LogEvent(String.Format("User {0} updated user profile for {1} at {2}.", ModifiedBy, this.SelectedUser.UserName, DateTime.Now));

                    /// Show global confirm dialog
                    ShowConfirmDialog(String.Format("Successfully updated user {0}.", this.SelectedUser.UserName));

                    try
                    {
                        var role = String.Empty;
                        switch (prf.PrimarySite.ToLower().Substring(0, 1))
                        {
                            case "m":
                            case "s":
                                role = "LABADMIN";
                                break;
                            case "a":
                                role = "MGMTENTERPRISE";
                                break;
                            default:
                                role = "CLINICADMIN";
                                break;
                        }

                        if (!String.IsNullOrEmpty(modifications.ToString()))
                        {
                            var mm = String.Format("Modifications to user {0}:{1}{2}{3}{4}{5}Made by {6} on {7}.",
                                this.SelectedUser.UserName,
                                Environment.NewLine, Environment.NewLine,
                                modifications.ToString(),
                                Environment.NewLine, Environment.NewLine,
                                mySession.ModifiedBy, DateTime.Now
                            );
                            // Send email to admins
                            UserNotificationService.SendModifiedUserAdminEmail(prf.PrimarySite, role, prf.UserName, mm, ModifiedBy);
                        }
                    }
                    catch (Exception ex) { ex.TraceErrorException(); }

                    ResetValues();
                    this.SelectedUser = Membership.GetUser(this.tbName.Text);
                    LoadUserDetails();
                    this.upUserMgmt.Update();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DoFail", "Confirm('Error updating user.', 'divUserManagerStatus', true, true);", true);
            }
        }

        protected void bSearch_Click(object sender, EventArgs e)
        {
            var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_bSearch_Click", ModifiedBy))
#endif
            {
                if (String.IsNullOrEmpty(this.tbName.Text)) return;

                var un = this.tbName.Text;

                // Try and get a single user by user name
                var u = Membership.GetUser(un);

                if (!u.IsNull())
                {
                    this.SelectedUser = u;
                    LoadUserDetails();
                    return;
                }

                // Try and get a list of users that are similar
                var uAll = Membership.GetAllUsers();
                var uFiltered = new MembershipUserCollection();

                uAll.Cast<MembershipUser>().ToList().ForEach(x =>
                {
                    if (x.UserName.ToLower().Contains(un.ToLower()))
                        uFiltered.Add(x);
                });

                if (uFiltered.IsNull() || uFiltered.Count.Equals(0) || uFiltered.Count > 1)
                {
                    this.Users = uFiltered;
                    BindUserGrid();
                    return;
                }

                this.SelectedUser = uFiltered.Cast<MembershipUser>().ToList()[0];
                LoadUserDetails();
            }
        }

        protected void bShowAll_Click(object sender, EventArgs e)
        {
            GetAllByPageIndex();
        }

        protected void bSetSite_Click(object sender, EventArgs e)
        {
            var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_bSetSite_Click", ModifiedBy))
#endif
            {
                // Save the sitecode to the profile
                this.UserProfile.SiteCode = this.ddlProfileSite.SelectedValue;
                if (this.UserProfile.AvailableSiteList.IsNull())
                    this.UserProfile.AvailableSiteList = new List<ProfileSiteEntity>();
                this.UserProfile.AvailableSiteList.Add(new ProfileSiteEntity() { Approved = true, SiteCode = this.ddlProfileSite.SelectedValue });
                CustomProfile.SaveLoggedInSiteCode(UserProfile);
                CustomProfile.SaveAvailableSiteList(this.UserProfile);
                LoadUserDetails();
            }
        }

        protected void gvUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (this.Users.IsNull())
                GetAllByPageIndex();

            gvUsers.PageIndex = e.NewPageIndex;
            gvUsers.DataSource = this.Users;
            gvUsers.DataBind();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_gvUsers_RowCommand", ModifiedBy))
#endif
            {
                if (!e.CommandName.Equals("Select")) return;

                this.tbName.Text = e.CommandArgument.ToString();

                var u = Membership.GetUser(e.CommandArgument.ToString());
                this.SelectedUser = u;

                this.divSearchUser.Visible = false;
                this.divUser.Visible = true;

                LoadUserDetails();
                this.upUserMgmt.Update();
            }
        }

        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var un = DataBinder.Eval(e.Row.DataItem, "UserName").ToString();
            var r = this.Users[un];

            var lbl = e.Row.FindControl("lblUserName") as Label;
            lbl.Text = r.UserName;

            if (!r.IsApproved)
                lbl.Text = String.Format("{0} (USER IS DISABLED)", lbl.Text);

            if (r.IsLockedOut)
                lbl.Text = String.Format("{0} (USER IS LOCKED OUT)", lbl.Text);
        }

        protected void bAddRole_Click(object sender, ImageClickEventArgs e)
        {
            if (this.lbAvailable.SelectedIndex.Equals(-1)) return;

            var r = this.lbAvailable.SelectedItem;

            if (CanAddRole(r.Value.ToLower().StartsWith("mgmt") || r.Value.ToLower().EndsWith("admin")) == false)
            {
                ShowAlert("An admin role and a non-admin role cannot be assigned together.");
                return;
            }

            var assg = this.lbAssigned.Items.Cast<ListItem>().ToList();
            assg.Add(r);
            assg = assg.OrderBy(x => x.Text).ToList();
            this.lbAvailable.Items.Remove(r);

            this.lbAssigned.Items.Clear();
            this.lbAssigned.DataSource = assg;
            this.lbAssigned.DataBind();
            ShowHideCms();
        }

        protected void bRemRole_Click(object sender, ImageClickEventArgs e)
        {
            if (this.lbAssigned.SelectedIndex.Equals(-1)) return;

            var r = this.lbAssigned.SelectedItem;
            var avail = this.lbAvailable.Items.Cast<ListItem>().ToList();
            avail.Add(r);
            avail = avail.OrderBy(x => x.Text).ToList();
            this.lbAssigned.Items.Remove(r);

            this.lbAvailable.Items.Clear();
            this.lbAvailable.DataSource = avail;
            this.lbAvailable.DataBind();
            ShowHideCms();
        }

        protected void bAddSite_Click(object sender, ImageClickEventArgs e)
        {
            // Hide the approval div
            this.divApprove.Visible = false;

            var s = this.lbAvailSiteCode.SelectedValue;
            var assg = this.SelectedSiteCodes.Select(x => x).ToList();
            assg.Add(new ProfileSiteEntity() { SiteCode = s, Approved = false });

            assg = assg.OrderBy(x => x.SiteCode).ToList();

            this.SelectedSiteCodes = assg;

            if (assg.Count.Equals(1))
                BindRolesLists(true);
        }

        protected void bRemSite_Click(object sender, ImageClickEventArgs e)
        {
            // Hide the approval div
            this.divApprove.Visible = false;

            var s = this.lbAssgSiteCode.SelectedValue;
            var assg = this.SelectedSiteCodes.Select(x => x).ToList();

            assg.RemoveAll(x => x.SiteCode == s);

            assg = assg.OrderBy(x => x.SiteCode).ToList();

            if (assg.IsNullOrEmpty())
            {
                this.lbAssigned.Items.Clear();
                this.lbAvailable.Items.Clear();
            }

            this.SelectedSiteCodes = assg;

            this.divApprove.Visible = false;
        }

        protected void ddlIndividualLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_ddlIndividualLoc_SelectedIndexChanged", ModifiedBy))
#endif
            {
                var p = new UserManagementPresenter(this);
                p.GetIndividualsAtSite(((DropDownList)sender).SelectedValue);
            }
        }

        protected void bLinkIndividual_Click(object sender, EventArgs e)
        {
            var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_bLinkIndividual_Click", ModifiedBy))
#endif
            {
                this.UserProfile.IndividualId = this.ddlLinkIndividual.SelectedValue.ToInt32();
                CustomProfile.SavePrimarySiteCode(this.UserProfile);
                LoadUserDetails();
            }
        }

        protected void lbAssgSiteCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lb = sender as ListBox;
            var SiteCode = string.IsNullOrEmpty(this.mySession.MyClinicCode) ? Globals.SiteCode : this.mySession.MyClinicCode;
            //if (this.IsClinicLabAdmin && !lb.SelectedValue.Equals(this.mySession.MyClinicCode))
            if (this.IsClinicLabAdmin && !lb.SelectedValue.Equals(SiteCode))
            {
                this.divApprove.Visible = false;
                return;
            }

            // Show a checkbox to allow the admin to enable/disable the selected site
            if (lb.SelectedIndex == -1)
            {
                this.divApprove.Visible = false;
                return;
            }

            var s = this.SelectedSiteCodes.FirstOrDefault(x => x.SiteCode == lb.SelectedValue);
            this.divApprove.Visible = true;
            this.cbApprove.Checked = s.Approved;
            this.cbApprove.Text = String.Format("Check to approve user for site {0}", s.SiteCode);
        }

        protected void cbApprove_CheckedChanged(object sender, EventArgs e)
        {
            // Get the selected site code
            var s = new ProfileSiteEntity();
            var MyClinicCode = string.IsNullOrEmpty(this.mySession.MyClinicCode) ? Globals.SiteCode : this.mySession.MyClinicCode;
            if (this.IsClinicLabAdmin)
                s = this.SelectedSiteCodes.FirstOrDefault(x => x.SiteCode == MyClinicCode);
            else
                s = this.SelectedSiteCodes.FirstOrDefault(x => x.SiteCode == this.lbAssgSiteCode.SelectedValue);

            s.Approved = this.cbApprove.Checked;
            this.btnSubmit.Enabled = true;
        }

        #endregion PAGE EVENTS

        #region PRIVATE METHODS

        private void LoadUserDetails()
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_LoadUserDetails", mySession.MyUserID))
#endif
                {
                    Master.CurrentModuleTitle = String.Format("{0}:  {1}", mySession.CurrentModule, this.SelectedUser.UserName);
                    Master.uplCurrentModuleTitle.Update();

                    this.divSearchUser.Visible = false;
                    this.divUser.Visible = true;

                    var u = this.SelectedUser;

                    // Get Users Profile
                    var p = new UserManagementPresenter(this);

                    this.UserProfile = CustomProfile.GetProfile(u.UserName);

                    // Ensure the profile has an inidivuals id attached
                    if (this.UserProfile.IndividualId.Equals(default(Int32)))
                    {
                        BindSiteDdl();
                        this.ddlIndividualLoc.SelectedValue = this.mySession.MyClinicCode;
                        p.GetIndividualsAtSite(this.mySession.MyClinicCode);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenDialog", "DoDialog();", true);
                        return;
                    }

                    // Ensure that the user has a sitecode
                    if (String.IsNullOrEmpty(this.UserProfile.SiteCode) || this.SiteCodes.FirstOrDefault(x => x.SiteCode == this.UserProfile.SiteCode).IsNull())
                    {
                        BindSetProfileSiteDdl();

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenDialog", "DoSetSiteDialog();", true);
                        return;
                    }

                    // Ensure that the user selected is NOT the user logged in.
                    if (Membership.GetUser().UserName.ToLower() == u.UserName.ToLower())
                    {
                        DoCancelAction();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DoFail", "Confirm('The logged on user can NOT make changes to that same account while they are logged on.', 'divSearchUserStatus', true, true);", true);
                        return;
                    }

                    // Clear the approval text and hide the div
                    this.cbApprove.Text = String.Empty;
                    this.divApprove.Visible = false;

                    this.tbFirstName.Text = this.UserProfile.FirstName;
                    this.tbLastName.Text = this.UserProfile.LastName;
                    this.tbMiddleName.Text = this.UserProfile.MiddleName;
                    this.tbProfileEmail.Text = u.Email;

                    // Get users CAC status
                    p.GetIsUserCacEnabled(u.UserName);

                    // Determine if user is a CMS content manager
                    p.IsUserCmsManager();

                    // Bind primary site ddl
                    BindPrimarySiteDdl();

                    // Set users site code from the PrimarySite property in the profile.  If none then use the SiteCode property.
                    this.IndividualSiteCode = this.UserProfile.PrimarySite.IsNull() ? this.UserProfile.SiteCode : this.UserProfile.PrimarySite;

                    this.ddlPrimarySiteSelection.SelectedValue = this.IndividualSiteCode;

                    // If the user is mgmt level or is an admin AND the users site code property matches the selected users site code property or primary site property.
                    this.ddlPrimarySiteSelection.Enabled = this.IsMgmt || (this.IsClinicLabAdmin && this.mySession.MyClinicCode.Equals(this.IndividualSiteCode));

                    // Get site codes assigned to user
                    this.SelectedSiteCodes = this.ActualUserSiteCodes;

                    // If a CA or LA is receiving a user to help out show them the approve/disapprove div
                    if (this.IsClinicLabAdmin)
                    {
                        if (this.ActualUserSiteCodes.Any(x => x.SiteCode == this.mySession.MyClinicCode))
                        {
                            var s = this.ActualUserSiteCodes.FirstOrDefault(x => x.SiteCode == this.mySession.MyClinicCode);
                            this.cbApprove.Checked = s.Approved;
                            this.lbAssgSiteCode.SelectedValue = s.SiteCode;
                            this.lbAssgSiteCode_SelectedIndexChanged(this.lbAssgSiteCode, new EventArgs());
                        }
                    }

                    // Get and bind the roles for the user
                    BindRolesLists(false);

                    var myProfile = CustomProfile.GetProfile();

                    // The page is editable if:
                    //  The logged in user is mgmt level
                    //  The logged in users site code is the same as the primary site of the selected user
                    var editable = this.IsMgmt || this.IndividualSiteCode.Equals(this.mySession.MyClinicCode);

                    SetPageControlsState(editable);
                    ShowHideCms();
                }
            }
            catch (Exception ex)
            {
                ex.TraceErrorException();
            }
        }

        private void DoCancelAction()
        {
            ResetValues();

            this.divSearchUser.Visible = true;
            this.divUser.Visible = false;
            this.tbName.Text = String.Empty;
            this.tbName.Focus();
        }

        private Boolean CanAddRole(Boolean isAdminAdd)
        {
            if (isAdminAdd)
            {
                if (this.lbAssigned.Items.Cast<ListItem>().ToList().Count() == 0) return true;
                if (!this.lbAssigned.Items.Cast<ListItem>().ToList().Any(x => x.Value.ToLower().EndsWith("admin") || x.Value.ToLower().StartsWith("mgmt"))) return false;
            }
            else
            {
                if (this.lbAssigned.Items.Cast<ListItem>().ToList().Any(x => x.Value.ToLower().EndsWith("admin") || x.Value.ToLower().StartsWith("mgmt"))) return false;
            }

            return true;
        }

        private void ResetValues()
        {
            this.SelectedUser = null;
            this.Users = null;
            this.UserProfile = null;
            this.ActualUserSiteCodes = null;

            this.gvUsers.DataSource = null;
            this.gvUsers.DataBind();

            this.LinkIndividuals = null;
            this.IsCmsUser = false;
            this.IndividualSiteCode = String.Empty;

            Master.CurrentModuleTitle = mySession.CurrentModule;
            Master.uplCurrentModuleTitle.Update();
        }

        private void SetPageControlsState(Boolean isOn)
        {
            this.divProfileData.Visible = isOn;

            this.tbFirstName.Enabled = isOn;
            this.tbLastName.Enabled = isOn;
            this.tbMiddleName.Enabled = isOn;
            this.tbProfileEmail.Enabled = isOn;
            this.btnSubmit.Enabled = isOn;
            this.cbCms.Enabled = isOn;
        }

        private void BindUserGrid()
        {
            this.gvUsers.DataSource = this.Users;
            this.gvUsers.DataBind();
        }

        private void BindRolesLists(Boolean clearAssgRolesList)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_BindRolesLists", mySession.MyUserID))
#endif
            {
                var rs = clearAssgRolesList ? new List<String>() : Roles.GetRolesForUser(this.SelectedUser.UserName).ToList();

                // Filter roles based on logged in users role.
                var allR = Roles.GetAllRoles().ToList();
                var myRoles = Roles.GetRolesForUser().ToList();

                if (myRoles.Any(x => new[] { "MgmtAdmin", "TrainingAdmin" }.ToList().Contains(x)))
                    allR.Remove("MgmtEnterprise");
                else if (myRoles.Any(x => x == "ClinicAdmin"))
                {
                    allR.RemoveAll(x => this.LabExclusion.Contains(x));
                }
                else if (myRoles.Any(x => x == "LabAdmin"))
                {
                    allR.RemoveAll(x => this.ClinicExclusion.Contains(x));
                }

                if (myRoles.Any(x => new[] { "MgmtEnterprise", "MgmtAdmin", "TrainingAdmin" }.ToList().Contains(x)))
                {
                    if (this.SiteCodes.FirstOrDefault(x => x.SiteCode == this.SelectedSiteCodes.FirstOrDefault().SiteCode).SiteType.ToLower().Equals("lab"))
                        allR.RemoveAll(x => this.ClinicExclusion.Contains(x));
                    else if (this.SiteCodes.FirstOrDefault(x => x.SiteCode == this.SelectedSiteCodes.FirstOrDefault().SiteCode).SiteType.ToLower().Equals("clinic"))
                        allR.RemoveAll(x => this.LabExclusion.Contains(x));
                    else if (this.SiteCodes.FirstOrDefault(x => x.SiteCode == this.SelectedSiteCodes.FirstOrDefault().SiteCode).SiteType.ToLower().Equals("other"))
                    {
                        allR.RemoveAll(x => this.ClinicExclusion.Contains(x));
                        allR.RemoveAll(x => this.LabExclusion.Contains(x));
                    }
                }
                else
                {
                    allR.RemoveAll(x => this.HumanExclusion.Contains(x));
                    allR.RemoveAll(x => this.MgmtExclusion.Contains(x));
                }

                allR.RemoveAll(x => rs.Contains(x));

                this.lbAvailable.DataSource = allR;
                this.lbAvailable.DataBind();

                this.lbAssigned.DataSource = rs;
                this.lbAssigned.DataBind();
            }
        }

        private void BindSiteLists(List<ProfileSiteEntity> assignedSites)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_BindSiteLists", mySession.MyUserID))
#endif
            {
                var ls = this.SiteCodes.Select(x => x).ToList();
                if (!assignedSites.IsNullOrEmpty())
                    ls.RemoveAll(x => assignedSites.Select(s => s.SiteCode).Contains(x.SiteCode));

                this.lbAvailSiteCode.Items.Clear();

                if (!assignedSites.IsNullOrEmpty())
                {
                    var stt = this.SiteCodes.FirstOrDefault(x => x.SiteCode == this.IndividualSiteCode).SiteType;
                    ls.RemoveAll(x => x.SiteType != stt);
                }
                this.lbAvailSiteCode.DataSource = ls;
                this.lbAvailSiteCode.DataTextField = "SiteCombinationProfile";
                this.lbAvailSiteCode.DataValueField = "SiteCode";
                this.lbAvailSiteCode.DataBind();

                var mySc = this.SiteCodes.Where(x => assignedSites.Any(s => x.SiteCode == s.SiteCode)).ToList();
                this.lbAssgSiteCode.Items.Clear();
                this.lbAssgSiteCode.DataSource = mySc;
                this.lbAssgSiteCode.DataTextField = "SiteCombinationProfile";
                this.lbAssgSiteCode.DataValueField = "SiteCode";
                this.lbAssgSiteCode.DataBind();
            }
        }

        private void BindSiteDdl()
        {
            this.ddlIndividualLoc.Items.Clear();
            this.ddlIndividualLoc.ClearSelection();
            this.ddlIndividualLoc.SelectedIndex = -1;
            this.ddlIndividualLoc.DataSource = this.SiteCodes;
            this.ddlIndividualLoc.DataTextField = "SiteCombinationProfile";
            this.ddlIndividualLoc.DataValueField = "SiteCode";
            this.ddlIndividualLoc.DataBind();
        }

        private void BindSetProfileSiteDdl()
        {
            this.ddlProfileSite.Items.Clear();
            this.ddlProfileSite.ClearSelection();
            this.ddlProfileSite.SelectedIndex = -1;
            this.ddlProfileSite.DataSource = this.SiteCodes;
            this.ddlProfileSite.DataTextField = "SiteCombinationProfile";
            this.ddlProfileSite.DataValueField = "SiteCode";
            this.ddlProfileSite.DataBind();
        }

        private void BindPrimarySiteDdl()
        {
            this.ddlPrimarySiteSelection.Items.Clear();
            this.ddlPrimarySiteSelection.ClearSelection();
            this.ddlPrimarySiteSelection.SelectedIndex = -1;
            this.ddlPrimarySiteSelection.DataSource = this.SiteCodes;
            this.ddlPrimarySiteSelection.DataTextField = "SiteCombinationProfile";
            this.ddlPrimarySiteSelection.DataValueField = "SiteCode";
            this.ddlPrimarySiteSelection.DataBind();
        }

        private void GetAllByPageIndex()
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsUserManager_GetAllByPageIndex", mySession.MyUserID))
#endif
            {
                var all = Membership.GetAllUsers();
                var lAll = all.Cast<MembershipUser>().ToList();
                var myAll = new MembershipUserCollection();
                lAll.ForEach(x =>
                {
                    var p = CustomProfile.GetProfile(x.UserName);
                    if (!p.IsNull() && !p.IsNull())
                        myAll.Add(x);
                });

                this.Users = myAll;

                BindUserGrid();
            }
        }

        private void ShowHideCms()
        {
            // if there is a role in the assigned box that is admin then enable the cms selection
            this.divCms.Visible = this.lbAssigned.Items.Cast<ListItem>().ToList().Any(x => x.Value.ToLower().StartsWith("mgmt") || x.Value.ToLower().EndsWith("admin"));
        }

        private void ShowHideRoles()
        {
            this.upRoles.Visible = this.RolesVisible;
        }

        private void ShowAlert(String message)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "alert", String.Format("alert('{0}');", message), true);
        }

        #endregion PRIVATE METHODS

        #region INTERFACE PROPERTIES

        public List<IndividualEntity> LinkIndividuals
        {
            get { return ViewState["LinkIndividuals"] as List<IndividualEntity>; }
            set
            {
                ViewState["LinkIndividuals"] = value;
                this.ddlLinkIndividual.Items.Clear();
                this.ddlLinkIndividual.ClearSelection();
                this.ddlLinkIndividual.SelectedIndex = -1;
                this.ddlLinkIndividual.DataSource = value;
                this.ddlLinkIndividual.DataTextField = "NameLFMi";
                this.ddlLinkIndividual.DataValueField = "ID";
                this.ddlLinkIndividual.DataBind();
            }
        }

        public List<SiteCodeEntity> SiteCodes
        {
            get
            {
                return ViewState["SiteCodes"] as List<SiteCodeEntity>;
            }
            set
            {
                ViewState["SiteCodes"] = value;
            }
        }

        public bool IsCmsUser
        {
            get
            {
                return this.cbCms.Checked;
            }
            set
            {
                this.cbCms.Checked = value;
            }
        }

        public Int32 SelectedIndividualId
        {
            get
            {
                return this.UserProfile.IndividualId;
            }
        }

        public string IndividualSiteCode
        {
            get
            {
                return ViewState["IndividualSiteCode"] as String;
            }
            set
            {
                ViewState["IndividualSiteCode"] = value;
                this.lblCurrentlyAssigned.Text = String.Format("Currently assigned to: {0}", value);
            }
        }

        public bool IsCacEnabled
        {
            get
            {
                return ViewState["IsCacEnabled"].ToBoolean();
            }
            set
            {
                ViewState["IsCacEnabled"] = value;
                this.lblCacEnabled.Text = value.ToString();
            }
        }

        #endregion INTERFACE PROPERTIES

        #region LOCAL PROPERTIES

        private MembershipUserCollection Users
        {
            get { return ViewState["Users"] as MembershipUserCollection; }
            set { ViewState["Users"] = value; }
        }

        private MembershipUser SelectedUser
        {
            get { return Session["SelectedUser"] as MembershipUser; }
            set { Session["SelectedUser"] = value; }
        }

        private UserProfile UserProfile
        {
            get { return Session["UserProfile"] as UserProfile; }
            set { Session["UserProfile"] = value; }
        }

        private List<ProfileSiteEntity> ActualUserSiteCodes
        {
            get
            {
                if (!ViewState["ActualUserSiteCodes"].IsNull())
                    return ViewState["ActualUserSiteCodes"] as List<ProfileSiteEntity>;

                var l = new List<ProfileSiteEntity>();
                if (!this.UserProfile.IsNull() && !this.UserProfile.IsNull() && !this.UserProfile.AvailableSiteList.IsNullOrEmpty())
                    l.AddRange(this.UserProfile.AvailableSiteList.Where(x => String.IsNullOrEmpty(x.SiteCode) == false));

                if (l.IsNullOrEmpty())
                {
                    l.Add(new ProfileSiteEntity() { SiteCode = this.UserProfile.SiteCode, Approved = true });
                }
                else if (!l.Select(x => x.SiteCode).Any(s => s == this.UserProfile.SiteCode))
                {
                    l.Add(new ProfileSiteEntity() { SiteCode = this.UserProfile.SiteCode, Approved = false });
                }

                ViewState["ActualUserSiteCodes"] = l;
                return l;
            }
            set { ViewState["ActualUserSiteCodes"] = value; }
        }

        private List<ProfileSiteEntity> SelectedSiteCodes
        {
            get
            {
                // Get the sites that match the ones in the ActualUserSiteCodes list
                return ViewState["SelectedSiteCodes"] as List<ProfileSiteEntity>;
            }
            set
            {
                ViewState["SelectedSiteCodes"] = value;
                BindSiteLists(this.SelectedSiteCodes);
            }
        }

        private String ViewableSiteType
        {
            get { return this.mySession.MySite.SiteType; }
        }

        private List<String> LabExclusion
        {
            get
            {
                return new List<string>(){
                    "LabMail",
                    "LabTech",
                    "LabClerk",
                    "LabAdmin"
                };
            }
        }

        private List<String> ClinicExclusion
        {
            get
            {
                return new List<string>(){
                    "ClinicProvider",
                    "ClinicTech",
                    "ClinicClerk",
                    "ClinicAdmin"
                };
            }
        }

        private List<String> MgmtExclusion
        {
            get
            {
                return new List<string>(){
                    "MgmtReport",
                    "MgmtDataMgmt",
                    "MgmtEnterprise",
                    "MgmtAdmin",
                    "TrainingAdmin"
                };
            }
        }

        private List<String> HumanExclusion
        {
            get
            {
                return new List<string>()
                {
                    "HumanAdmin",
                    "HumanTech"
                };
            }
        }

        private Boolean RolesVisible
        {
            get { return !this.SelectedUser.UserName.Equals(Membership.GetUser().UserName); }
        }

        private Boolean IsClinicLabAdmin
        {
            get
            {
                return Roles.GetRolesForUser().Any(x => x.ToLower() == "clinicadmin" || x.ToLower() == "labadmin");
            }
        }

        private Boolean IsMgmt
        {
            get
            {
                return Roles.GetRolesForUser().Any(x => x == "MgmtEnterprise" || x == "MgmtAdmin" || x == "TrainingAdmin");
            }
        }

        #endregion LOCAL PROPERTIES

        private void ShowConfirmDialog(String msg)
        {
            /// Show global confirm dialog
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Success!','" + msg + "', 'success');", true);
        }


    }
}