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
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class ManageFrames : PageBase
    {
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
                        mySession.CurrentModule = "Management - Manage Frames";
                        mySession.CurrentModule_Sub = string.Empty;

                        //var p = new SitePreferencesPresenter(this);
                        //p.GetSites();

                    }
                    catch (NullReferenceException)
                    {
                        Response.Redirect(FormsAuthentication.DefaultUrl);
                    }
                }
            }
            BuildPageTitle();

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
                CurrentModule("Management - Manage Frames");
                CurrentModule_Sub(string.Empty);
            }
        }


    }
}