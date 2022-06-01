using AjaxControlToolkit;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb
{
    public partial class srtsMasterRptViewer : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.extAlerts.Collapsed = false;
                this.extAlerts.ClientState = "false";

                if (mySession == null)
                {
                    mySession = new SRTSSession();
                    mySession.MainContentTitle = "My SRTSweb";
                }

                Page.Title = "DoD Spectacle Request Transmission System ( SRTS Web )";
                litlastupdated.Text = DateTime.Now.ToString(Globals.TitleDtFmt);
                litTitleLeft_Top.Text = DateTime.Now.ToString(Globals.TitleDtFmt);

                string ver = Convert.ToString(Assembly.GetExecutingAssembly().GetName().Version);
                litVersion.Text = ver;

                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    divBreadCrumbsLogoutTop.Visible = true;
                    pnlMainContent.Visible = true;
                    divMainMenuRptViewer.Visible = true;
                    divMainContentRptViewer.Visible = true;
                    MainPageFooterCols.Visible = true;
                    this.extAlerts.Collapsed = true;
                    this.extAlerts.ClientState = "true";

                    MenuItem newItem = new MenuItem();
                    newItem.Text = "My New Item";
                    mainMenu.Items.AddAt(0, newItem);

                    if (
                        Roles.IsUserInRole("ClinicClerk") ||
                        Roles.IsUserInRole("HumanTech") ||
                        Roles.IsUserInRole("ClinicProvider") ||
                        Roles.IsUserInRole("ClinicTech"))
                    {
                        contentSubMenuLab.Visible = false;
                        contentSubMenu.Visible = true;
                    }
                    else if (
                        Roles.IsUserInRole("LabClerk") ||
                        Roles.IsUserInRole("LabMail") ||
                        Roles.IsUserInRole("LabTech")
                        )
                    {
                        contentSubMenuLab.Visible = true;
                        contentSubMenu.Visible = false;
                    }
                    else
                    {
                        contentSubMenuLab.Visible = false;
                        contentSubMenu.Visible = false;
                    }
                    uplAppType.Update();
                    if (Cache["SRTSLOOKUP"] == null)
                    {
                        LoadLookupTable();
                    }
                }
                else
                {
                    appType.Visible = false;
                    divMainMenuRptViewer.Visible = false;
                    divBreadCrumbsLogoutTop.Visible = false;
                    pnlMainContent.Visible = true;
                    this.extAlerts.Collapsed = false;
                    this.extAlerts.ClientState = "false";
                }

                SetAlerts();
            }

            ApplyRoles();
        }

        protected void ApplyRoles()
        {
            if (Roles.IsUserInRole("ClinicAdmin") ||
                Roles.IsUserInRole("LabAdmin") ||
                Roles.IsUserInRole("HumanAdmin")
                )
            {
                contentSubMenu.Visible = false;
            }
            else contentSubMenu.Visible = true;
        }

        #region MESSAGING

        private void SetAlerts()
        {
            IMessageService _service = new MessageService();
            this.litAlert.Text = _service.GetAlerts();
        }

        #endregion MESSAGING

        protected SRTSSession mySession
        {
            get { return (SRTSSession)Session["SRTSSession"]; }
            set { Session.Add("SRTSSession", value); }
        }

        private string _myClinic;

        public string MyClinic
        {
            get { return _myClinic; }
            set { _myClinic = value; }
        }

        public UpdatePanel MainContentPanel
        {
            get { return uplMainContent; }
            set { uplMainContent = value; }
        }

        public Panel _ContentAuthenticated
        {
            get { return pnlContentAuthenticated; }
            set { pnlContentAuthenticated = value; }
        }

        public Panel _MainPageFooter
        {
            get { return MainPageFooterCols; }
            set { MainPageFooterCols = value; }
        }

        public Panel _MainMenu
        {
            get { return pnlMainMenu; }
            set { pnlMainMenu = value; }
        }

        public Panel _BreadCrumbsTop
        {
            get { return pnlBreadCrumbsTop; }
            set { pnlBreadCrumbsTop = value; }
        }

        public ToolkitScriptManager MasterScriptManager
        {
            get { return ToolkitManager1; }
            set { ToolkitManager1 = value; }
        }

        public string CurrentModuleTitle
        {
            get { return litContentTop_Title.Text; }
            set { litContentTop_Title.Text = value; }
        }

        public UpdatePanel uplCurrentModuleTitle
        {
            get { return uplContentTitle; }
            set { uplContentTitle = value; }
        }

        private void LoadLookupTable()
        {
            ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            //Response.Redirect("~/Account/Login.aspx");
        }

        protected void MySRTSDashboard(object sender, EventArgs e)
        {
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void mainMenu_MenuItemDataBound(object sender, MenuEventArgs e)
        {
            SiteMapNode mapNode = (SiteMapNode)e.Item.DataItem;

            if (!Roles.IsUserInRole("MgmtEnterprise"))
            {
                if (mapNode.Title == "Data Manager (Frames)" ||
                     mapNode.Title == "Data Manager (Lookup Types)" ||
                     mapNode.Title == "Error Log")
                {
                    MenuItem parent = e.Item.Parent;
                    if (parent != null)
                    {
                        parent.ChildItems.Remove(e.Item);
                    }
                }
            }

            if (Roles.IsUserInRole("LabClerk") || Roles.IsUserInRole("LabTech"))
            {
                if (mapNode.Title.ToLower().Equals("patient add"))
                {
                    MenuItem parent = e.Item.Parent;
                    if (parent != null)
                        parent.ChildItems.Remove(e.Item);
                }
            }

            if (Roles.IsUserInRole("MgmtAdmin"))
            {
                if (mapNode.Title.ToLower().Equals("add individual"))
                {
                    var parent = e.Item.Parent;
                    if (parent != null)
                        parent.ChildItems.Remove(e.Item);
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Roles.DeleteCookie();
            this.mySession = null;
            Session.Clear();
            Response.Redirect("~/Account/Login.aspx", true);//ConfigurationManager.AppSettings["NonProxyWebServerUrl"], false);
        }

        //private void DoLogout()
        //{
        //    FormsAuthentication.SignOut();
        //    Roles.DeleteCookie();
        //    Session.Clear();
        //    FormsAuthentication.RedirectToLoginPage();
        //}
    }
}