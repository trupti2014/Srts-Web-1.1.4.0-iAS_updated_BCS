using AjaxControlToolkit;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Base;
using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb
{
    public partial class srtsMaster : MasterBase
    {
        private readonly string AntiXsrfTokenKey = ConfigurationManager.AppSettings["AntiXsrfTokenKey"];
        private readonly string AntiXsrfUserNameKey = ConfigurationManager.AppSettings["AntiXsrfUserNameKey"];
        private readonly string DeltaPWordKey = ConfigurationManager.AppSettings["DeltaPWordKey"];
        private string _antiXsrfTokenValue;

        #region IMPLEMENTING ANTI CSRF CODE
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                //First, check for the existence of the Anti-XSS cookie        
                var requestCookie = Request.Cookies[AntiXsrfTokenKey];
                Guid requestCookieGuidValue;
                //If the CSRF cookie is found, parse the token from the cookie.        
                //Then, set the global page variable and view state user ey. The global variable will be used to validate that it matches in the view state form field in the Page.PreLoad method.        
                if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
                {
                    //Set the global token variable so the cookie value can be validated against the value in the view state form field in the Page.PreLoad method.            
                    _antiXsrfTokenValue = requestCookie.Value;
                    //Set the view state user key, which will be validated by the framework during each request            
                    Page.ViewStateUserKey = _antiXsrfTokenValue;
                }
                //If the CSRF cookie is not found, then this is a new session.        
                else
                {
                    //Generate a new Anti-XSRF token
                    _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                    //Set the view state user key, which will be validated by the framework during each request
                    Page.ViewStateUserKey = _antiXsrfTokenValue;
                    //Create the non-persistent CSRF cookie
                    var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                    {
                        //Set the HttpOnly property to prevent the cookie from being accessed by client side script
                        HttpOnly = true,
                        //Add the Anti-XSRF token to the cookie value
                        Value = _antiXsrfTokenValue
                    };
                    //If we are using SSL, the cookie should be set to secure to prevent it from being sent over HTTP connections
                    if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                        responseCookie.Secure = true;
                    //Add the CSRF cookie to the response
                    Response.Cookies.Set(responseCookie);
                }
                Page.PreLoad += master_Page_PreLoad;
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                //During the initial page load, add the Anti-XSRF token and user name to the ViewState
                if (!IsPostBack)
                {
                    //Set Anti-XSRF token
                    ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                    //If a user name is assigned, set the user name
                    ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
                }
                //During all subsequent post backs to the page, the token value from the cookie should be validated against the token in the view state
                //form field. Additionally user name should be compared to the authenticated users name
                else
                {
                    //Validate the Anti-XSRF token
                    if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                    {
                        if (Response.Cookies[DeltaPWordKey] != null) { Response.Cookies.Remove(DeltaPWordKey); return; }
                        throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }
        #endregion

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
                this.litTitleLeft_Top.InnerHtml = DateTime.Now.ToString(Globals.TitleDtFmt);

                string ver = Convert.ToString(Assembly.GetExecutingAssembly().GetName().Version);
                litVersion.Text = ver;

                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    pnlMainContent.Visible = true;
                    divMainMenu.Visible = true;
                    divMainContent.Visible = true;
                    this.extAlerts.Collapsed = true;
                    this.extAlerts.ClientState = "true";

                    this.quickSearch.Visible = (Roles.IsUserInRole("ClinicClerk") || Roles.IsUserInRole("ClinicTech") || Roles.IsUserInRole("LabClerk") || Roles.IsUserInRole("LabTech"));

                    if (Cache["SRTSLOOKUP"] == null)
                    {
                        LoadLookupTable();
                    }
                }
                else
                {
                    this.quickSearch.Visible = false;
                    this.wrapper.Style.Add("padding-top", "29px");
                    divMainMenu.Visible = false;
                    pnlMainContent.Visible = true;
                    this.extAlerts.Collapsed = false;
                    this.extAlerts.ClientState = "false";
                }

                SetAlerts();
            }
        }

        #region MESSAGING

        private void SetAlerts()
        {
            IMessageService _service = new MessageService();
            var isAlerts = _service.GetAlerts();
            if (isAlerts != "No Alerts at This Time!")
            {
                this.litAlert.Text = isAlerts;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showAlert", "setAlert();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showAlert", "setNoAlert();", true);
            }
        }

        #endregion MESSAGING

        internal void HideMainContent()
        {
            this.pnlContentAuthenticated.Visible = false;
        }

        private string _myClinic;

        public string MyClinic
        {
            get { return _myClinic; }
            set { _myClinic = value; }
        }

        public LoginStatus LoginStatus
        {
            get { return this.lsSrtsLoginStatus; }
        }

        public UpdatePanel MainContentPanel
        {
            get;
            set;
        }

        public Panel _ContentAuthenticated
        {
            get { return pnlContentAuthenticated; }
            set { pnlContentAuthenticated = value; }
        }

        public Panel _MainPageFooter
        {
            get;
            set;
        }

        public Panel _MainMenu
        {
            get { return pnlMainMenu; }
            set { pnlMainMenu = value; }
        }

        public Panel _BreadCrumbsTop
        {
            get;
            set;
        }

        public Panel _BreadCrumbsBottom
        {
            get;
            set;
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

        public SiteMapPath CurrentSiteMapPath
        {
            get;
            set;
        }

        public UpdatePanel uplCurrentModuleTitle
        {
            get { return uplContentTitle; }
            set { uplContentTitle = value; }
        }

        public void LoadLookupTable()
        {
            ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
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

        protected void lsSrtsLoginStatus_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            SessionService.SessionLogoff(HttpContext.Current.User.Identity.Name);
        }

        protected void lsSrtsLoginStatus_LoggedOut(object sender, EventArgs e)
        {
            var mUid = mySession.MyUserID;

            MembershipService.DoLogOut(Context.User.Identity.Name);

            var url = String.Format(ConfigurationManager.AppSettings["LogoutUrl"] + "?cs=3&u={0}", mUid);

            Response.Redirect(url, true);
        }

        public Int32 TimeOut
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated) return ConfigurationManager.AppSettings["nonAdminTimeOutTicks"].ToInt32();
                var r = Roles.GetRolesForUser();
                if (r[0].ToLower().EndsWith("admin")) return ConfigurationManager.AppSettings["adminTimeOutTicks"].ToInt32();
                if (r[0].ToLower().StartsWith("mgmt")) return ConfigurationManager.AppSettings["adminTimeOutTicks"].ToInt32();
                return ConfigurationManager.AppSettings["nonAdminTimeOutTicks"].ToInt32();
            }
        }
    }
}