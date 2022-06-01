using Microsoft.Web.Administration;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using System;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class SrtsAdministration : PageBase
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

                        CurrentModule("Administration");
                        CurrentModule_Sub(string.Empty);
                        GetSelectedPageOption("0");
                        BuildPageTitle();
                    }
                    catch (NullReferenceException)
                    {
                        Response.Redirect(FormsAuthentication.DefaultUrl);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    GetSelectedPageOption(Request.QueryString["id"]);
                }
            }
        }

        protected void GetSelectedPageOption(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (Request.PathInfo.Length != 0)
                {
                    path = Request.PathInfo.Substring(1, 1);
                }
            }
            switch (path)
            {
                case "0":
                    CurrentModule_Sub("");
                    tbcAdministration.ActiveTabIndex = 0;
                    break;

                case "1":
                    Response.Redirect("~/WebForms/Admin/SrtsUserManager.aspx?id=1");
                    break;

                case "2":
                    Response.Redirect("~/WebForms/Admin/SrtsUserManager.aspx?id=2");
                    break;

                case "3":
                    Response.Redirect("~/WebForms/Admin/SrtsUserManager.aspx?id=3");
                    break;

                default:

                    break;
            }
            BuildPageTitle();
        }

        protected void ActiveTabChanged(object sender, EventArgs e)
        {
            CurrentModule("Administration");
            int tabid;

            tabid = tbcAdministration.ActiveTabIndex;
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

                    case 2:
                        GetSelectedPageOption("2");
                        break;

                    case 3:
                        GetSelectedPageOption("3");
                        break;
                }
            }
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
                CurrentModule("Administration - SRTSweb Administration");
                CurrentModule_Sub(string.Empty);
            }
        }
    }
}