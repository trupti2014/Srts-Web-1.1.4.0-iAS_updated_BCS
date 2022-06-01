using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters;
using SrtsWeb.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

public partial class _Default : PageBase, IDefaultView
{
    private DefaultPresenter _presenter;
    private UserProfile profile;
    private CurrentUser myCurrentUser = new CurrentUser();
    public _Default()
    {
        _presenter = new DefaultPresenter(this);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!HttpContext.Current.User.Identity.IsAuthenticated)
        {
            FormsAuthentication.RedirectToLoginPage();
            return;
        }

        try
        {
            //TS 545: Prompt for Rules of Behavior
            //Check rules of behavior acceptance date
            var rb = new SrtsWeb.Presenters.Account.RulesOfBehaviorPresenter();
            if (rb.IsRulesOfBehaviorAcceptDateExpired(Membership.GetUser().UserName))
                Response.Redirect("~/WebForms/Account/RulesOfBehavior.aspx", false);

            //try
            //{
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "_Default_Page_Load", mySession.MyUserID))
#endif
            {
                if (!IsPostBack)
                {
                    if (mySession == null)
                    {
                        mySession = new SRTSSession();
                    }

                    profile = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name);
                    mySession.MyClinicCode = profile.SiteCode;
                    Globals.ModifiedBy = profile.LastName + "," + profile.FirstName + " - " + profile.IndividualId;
                    Globals.IndividualID = profile.IndividualId;
                    Globals.SiteCode = profile.SiteCode;


                    // If the session property SiteSelected is false then show the sites dialog
                    if (Request.QueryString.AllKeys.ToList().Contains("ss") && Request.QueryString["ss"].ToInt32().Equals(1))
                    {
                        var l = new List<String>();
                        if (!profile.AvailableSiteList.IsNullOrEmpty())
                            l.AddRange(profile.AvailableSiteList.Where(w => w.Approved == true).Select(x => x.SiteCode));

                        if (l.Count.Equals(0) || (l.Count > 0 && !l.Any(x => x == profile.SiteCode)))
                            l.Add(profile.SiteCode);

                        if (l.Count > 1)
                        {
                            this.gvSites.DataSource = l;
                            this.gvSites.DataBind();

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenDialog", "DoDialog();", true);
                            return;
                        }
                    }

                    if (String.IsNullOrEmpty(mySession.CertificateSubject))
                        mySession.MyUserID = string.Format("{0},{1} - {2} - {3}", profile.LastName, profile.FirstName, profile.IndividualId, profile.UserName);
                    else
                        mySession.MyUserID = string.Format("{0},{1} - {2} - {3}: {4}", profile.LastName, profile.FirstName, profile.IndividualId, profile.UserName, mySession.CertificateSubject);

                    mySession.ModifiedBy = string.Format("{0},{1} - {2}", profile.LastName, profile.FirstName, profile.IndividualId);

                    LogEvent("User {0} authenticated at {1}.", new Object[] { mySession.MyUserID, DateTime.Now });

                    _presenter.InitView(User.Identity.Name);

                    if (mySession.MySite.SiteType != "ADMIN")
                    {
                        _presenter.GetSummary();
                        if (mySession.MySite.SiteType == "CLINIC")
                        {
                            divLabSummary.Visible = false;
                            divClinicSummary.Visible = true;
                        }
                        else if (mySession.MySite.SiteType == "LAB")
                        {
                            divClinicSummary.Visible = false;
                            divLabSummary.Visible = true;
                        }
                    }
                }
                BuildUserInterface();

                #region Enable/disable user guides and order management links

                if (Roles.IsUserInRole("ClinicTech"))
                {
                    this.lnkCAGuide.Visible = false;
                    this.lnkCTGuide.Visible = true;
                    this.lnkLAGuide.Visible = false;
                    this.lnkLTGuide.Visible = false;
                    this.divSummaryWithLinks.Visible = true;
                }
                if (Roles.IsUserInRole("ClinicAdmin"))
                {
                    this.lnkCAGuide.Visible = true;
                    this.lnkCTGuide.Visible = true;
                    this.lnkLAGuide.Visible = false;
                    this.lnkLTGuide.Visible = false;
                    this.divSummaryWithNoLinks.Visible = true;
                }
                if (Roles.IsUserInRole("LabTech"))
                {
                    this.lnkCAGuide.Visible = false;
                    this.lnkCTGuide.Visible = false;
                    this.lnkLAGuide.Visible = false;
                    this.lnkLTGuide.Visible = true;
                    this.divLabSummaryWithLinks.Visible = true;
                }
                if (Roles.IsUserInRole("LabAdmin"))
                {
                    this.lnkCAGuide.Visible = false;
                    this.lnkCTGuide.Visible = false;
                    this.lnkLAGuide.Visible = true;
                    this.lnkLTGuide.Visible = true;
                    this.divLabSummaryWithNoLinks.Visible = true;
                }

                #endregion Enable/disable user guides and order management links
            }
        }
        catch (Exception ex) { ex.TraceErrorException(); }
    }

    private void BuildUserInterface()
    {
        try
        {
            string CurrentUserName = string.Format("{0} {1}", myCurrentUser.UserPersonalInfo.FirstName, myCurrentUser.UserPersonalInfo.LastName);
            litSiteName.Text = mySession.MySite.SiteName;

            var dtc = Convert.ToDouble(ConfigurationManager.AppSettings["passwordDaysToChange"]);
            var expDays = ConfigurationManager.AppSettings["passwordDuration"];
            var m = Membership.GetUser();

            var dtExp = m.LastPasswordChangedDate.AddDays(Convert.ToInt32(expDays));

            var daysLeft = dtExp.DateDiff(DateTime.Now);

            var msg = new StringBuilder(string.Format("Greetings {0}!  Last Login Date: {1}", CurrentUserName, mySession.LastLoginDate));

            // Check authorization table to see if user has registered thier cac
            var p = new SrtsWeb.Presenters.Account.AuthorizationPresenter();
            if (!p.IsUserCacRegistered(Membership.GetUser().UserName))
            {
                if (daysLeft <= dtc)
                {
                    msg.AppendFormat("<br /><span style=\"color: red;\">Your password will expire in {0} days</span>", daysLeft);
                }
            }
            litContentTop_Title_Right.Text = msg.ToString();

            this._presenter.GetAnnouncements();

            CurrentModule("SRTSweb Dashboard");
            CurrentModule_Sub(string.Empty);
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
            Master.uplCurrentModuleTitle.Update();
        }
        catch (IndexOutOfRangeException)
        {
            CurrentModule("SRTSweb Dashboard");
            CurrentModule_Sub(string.Empty);
        }
        catch (NullReferenceException)
        {
            CurrentModule("SRTSweb Dashboard");
            CurrentModule_Sub(string.Empty);
        }
    }

    protected void gvSites_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        try
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "_Default_gvSites_RowCommand", mySession.MyUserID))
#endif
            {
                var sc = e.CommandArgument.ToString();
                var p = CustomProfile.GetProfile();
                p.SiteCode = sc;
                CustomProfile.SaveLoggedInSiteCode(p);
                mySession.SiteSelected = true;
                Response.Redirect("~/WebForms/Default.aspx", false);
            }
        }
        catch (Exception ex) { ex.TraceErrorException(); }
    }

    protected void gvSites_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow) return;

        var b = e.Row.FindControl("bSiteCode") as Button;
        b.Text = e.Row.DataItem.ToString();
        b.CommandArgument = e.Row.DataItem.ToString();
    }

    #region Announcements

    public String AnnouncementLinks
    {
        set
        {
            if (value != "<ul></ul>")
            {
                this.divSrtsAnnouncements.InnerHtml = value;
            }
        }
    }

    #endregion Announcements

    #region Clinic Accessors
    public int? Pending
    {
        get
        {
            return Convert.ToInt32(litPending.Text);
        }
        set
        {
            litPending.Text = value.ToString();
            litPending1.Text = value.ToString();
        }
    }

    public int? ReadyForCheckin
    {
        get { return Convert.ToInt32(litRetrieval.Text); }
        set
        {
            litRetrieval.Text = value.ToString();
            litRetrieval1.Text = value.ToString();
        }
    }

    public int? ReadyForDispense
    {
        get { return Convert.ToInt32(litDispense.Text); }
        set
        {
            litDispense.Text = value.ToString();
            litDispense1.Text = value.ToString();
        }
    }

    public int? Rejected
    {
        get { return Convert.ToInt32(litRejected.Text); }
        set
        {
            litRejected.Text = value.ToString();
            litRejected1.Text = value.ToString();
        }
    }

    public int? OverDue
    {
        get { return Convert.ToInt32(litOverdue.Text); }
        set
        {
            litOverdue.Text = value.ToString();
            litOverdue1.Text = value.ToString();
        }
    }

    public int? AvgDispenseDays
    {
        get { return Convert.ToInt32(litAvgDispenseTime.Text); }
        set { litAvgDispenseTime.Text = value.ToString(); }
    }

    #endregion Clinic Accessors

    #region Lab Accessors

    public int? ReadyForLabCheckin
    {
        get { return Convert.ToInt32(litLabRetrieval.Text); }
        set
        {
            litLabRetrieval.Text = value.ToString();
            litLabRetrieval1.Text = value.ToString();
        }
    }

    public int? ReadyForLabDispense
    {
        get { return Convert.ToInt32(litLabDispense.Text); }
        set
        {
            litLabDispense.Text = value.ToString();
            litLabDispense1.Text = value.ToString();
        }
    }

    public int? AvgProductionDays
    {
        get { return Convert.ToInt32(litLabProdTime.Text); }
        set { litLabProdTime.Text = value.ToString(); }
    }

    public int? HoldForStockOrders
    {
        get
        {
            return litLabHoldForStock.Text.ToInt32();
        }
        set
        {
            this.litLabHoldForStock.Text = value.ToString();
            this.litLabHoldForStock1.Text = value.ToString();
        }
    }
    #endregion Lab Accessors

    #region User Guides

    protected void DownloadGuide(object sender, CommandEventArgs e)
    {
        try
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "_Default_DownloadGuide", mySession.MyUserID))
#endif
            {
                var fn = _presenter.GetGuideFileName(e.CommandArgument.ToString());
                if (fn != "none" && fn != null)
                {
                    //CR SCR-T 0059909: Format upload tool
                    var ug = new SrtsWeb.Presenters.Admin.ReleaseManagementPresenter.UserGuidesPresenter();
                    var d = ug.GetUserGuide(fn.ToString());
                    if (d != null)
                    { 
                        Response.ClearHeaders();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + fn);
                        Response.Clear();
                        Response.BinaryWrite(d.UserGuideDocument);
                        Response.End();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "", "alert('This user guide is not available!');", true);
                        return;
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "", "alert('This user guide is not available!');", true);
                    return;
                }

            }
        }
        catch (Exception ex) { ex.TraceErrorException(); }
    }

    #endregion User Guides

    protected void lnkHoldForStock_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/WebForms/SrtsWebLab/ManageOrdersLab.aspx?id=holdforstock", false);
    }

   
}