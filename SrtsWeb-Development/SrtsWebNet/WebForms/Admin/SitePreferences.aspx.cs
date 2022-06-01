using SrtsWeb.Base;
using SrtsWeb.Entities;
using System;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections.Generic;
using SrtsWeb.Views.Admin;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.ExtendersHelpers;

namespace SrtsWeb.WebForms.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class SitePreferences : PageBase, ISitePreferencesView
    {
        public event EventHandler RefreshUserControl;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                else
                {
                    if (mySession == null)
                    {
                        mySession = new SRTSSession();
                        mySession.ReturnURL = string.Empty;
                    }
                    try
                    {
                        Master.CurrentModuleTitle = string.Empty;
                        mySession.CurrentModule = "Administration - Site Preferences";
                        mySession.CurrentModule_Sub = string.Empty;

                        var p = new SitePreferencesPresenter(this);
                        p.GetSites();
                       
                   }
                    catch (NullReferenceException)
                    {
                        Response.Redirect(FormsAuthentication.DefaultUrl);
                    }
                }
            }
            BuildPageTitle();
            BuildUserInterface();

            //var lmtp = new SitePreferencesPresenter(this);
            //var islmtp = 0;
            //if (Roles.IsUserInRole("MgmtEnterprise"))
            //{
            //    islmtp = lmtp.GetLabShipToPatient(SiteCode);
            //}
            //else
            //{
            //    islmtp = lmtp.GetLabShipToPatient(mySession.MySite.SiteCode);
            //}
         
            //if (islmtp == 1)
            //{
            //    mySession.MySite.ShipToPatientLab = true;
            //    lstLabMailToPatient.Visible = true;
            //}
            //else
            //{
            //    mySession.MySite.ShipToPatientLab = false;
            //    lstLabMailToPatient.Visible = false;
            //}
            //Session["SelectedPreference"] = "MTP";
            //uplMenu.Update();
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Administration - Site Preferences");
                CurrentModule_Sub(string.Empty);
            }
        }

        private void BuildUserInterface()
        {
            litSiteName.Text = SiteCodes.FirstOrDefault(x => x.SiteCode == SiteCode).SiteCombination;
            hdfSiteType.Value = SiteCodes.FirstOrDefault(x => x.SiteCode == SiteCode).SiteType;
            if (Roles.IsUserInRole("MgmtEnterprise"))
            {
                // Get sitecode list
                divSelectSite.Visible = true;
            }
            else
            {
                divSelectSite.Visible = false;
                this.SiteCode = string.IsNullOrEmpty(mySession.MySite.SiteCode) ? Globals.SiteCode : mySession.MySite.SiteCode;
               

            }
            List<String> roles = new List<String>();
            roles = Roles.GetRolesForUser().ToList().ConvertAll(x => x.ToLower());
            hdfUserRole.Value = roles.FirstOrDefault();
        }

        public IEnumerable<SiteCodeEntity> SiteCodes
        {
            get
            {
                return ViewState["SiteCodes"] as List<SiteCodeEntity> ?? new List<SiteCodeEntity>();
            }
            set
            {
                ViewState["SiteCodes"] = value;
                this.ddlSiteCode.DataSource = value;
                this.ddlSiteCode.DataTextField = "SiteCombination";
                this.ddlSiteCode.DataValueField = "SiteCode";
                this.ddlSiteCode.DataBind();
                this.ddlSiteCode.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlSiteCode.SelectedValue = this.mySession.MySite.SiteCode;
            }
        }

        public string SiteCode
        {
            get
            {
                return this.ddlSiteCode.SelectedValue;
            }
            set
            {
                this.ddlSiteCode.SelectedValue = value;
            }
        }

        protected void ddlSiteCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildUserInterface();
            RefreshUserControl(sender, e);
        }
    }
}