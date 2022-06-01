using Microsoft.Web.Administration;
using SrtsWeb.CustomErrors;
using SrtsWeb.Entities;
using System;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabMail")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtDataMgmt")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class SrtsPageTemplate : PageBase, ISiteMapResolver
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }
            if (!IsPostBack)
            {
                if (mySession == null)
                {
                    mySession = new SRTSSession();
                    mySession.AddOrEdit = string.Empty;
                    mySession.Patient = new PatientEntity();
                    mySession.ReturnURL = string.Empty;
                    mySession.SelectedExam = new ExamEntity();
                    mySession.SelectedOrder = new OrderEntity();
                    mySession.SelectedPatientID = 0;
                    mySession.TempID = 0;
                    mySession.tempOrderID = string.Empty;
                }
                try
                {
                    Master.CurrentModuleTitle = string.Empty;

                    CurrentModule("Administration - SRTSweb Report Manager");
                    CurrentModule_Sub(string.Empty);
                    GetSelectedPageOption("1");
                    BuildPageTitle();
                }
                catch (NullReferenceException)
                {
                    Response.Redirect(FormsAuthentication.DefaultUrl);
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                GetSelectedPageOption(Request.QueryString["id"]);
            }
        }

        protected void ActiveTabChanged(object sender, EventArgs e)
        {
            int tabid = tbcTemplate.ActiveTabIndex;
            if (tabid >= 0)
            {
                switch (tabid)
                {
                    case 0:
                        GetSelectedPageOption("0");
                        break;

                    case 1:
                        GetSelectedPageOption("1");
                        break;
                }
                BuildPageTitle();
            }
        }

        protected void GetSelectedPageOption(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    path = (Request.QueryString["id"]);
                }
                else if (Request.PathInfo.Length != 0)
                {
                    path = Request.PathInfo.Substring(1);
                }
            }
            switch (path)
            {
                case "0":
                    Response.Redirect("../WebForms/Default.aspx");
                    break;

                case "1":
                    CurrentModule_Sub("- View Reports");
                    tbcTemplate.ActiveTabIndex = 1;
              
                    break;

                default:

                    CurrentModule_Sub("- View Reports");
                    tbcTemplate.ActiveTabIndex = 1;
                    tbpanelTemplate.Focus();
                    break;
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Admin/SrtsReportManager.aspx", "Manage Reports");
            child.ParentNode = parent;
            return child;
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
                CurrentModule("Administration - SRTSweb Report Manager");
                CurrentModule_Sub(string.Empty);
            }
        }

    }
}