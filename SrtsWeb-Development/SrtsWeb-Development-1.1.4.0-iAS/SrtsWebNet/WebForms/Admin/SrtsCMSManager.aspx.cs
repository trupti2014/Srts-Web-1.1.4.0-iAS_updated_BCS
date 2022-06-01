using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class SrtsCMSManager : PageBase, ICmsManagerView
    {
        private CmsManagerPresenter _presenter;

        public SrtsCMSManager()
        {
            this._presenter = new CmsManagerPresenter(this);

            cmsUpdate = new UserControls.ucCms();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsCMSManager_Page_Load", mySession.MyUserID))
#endif
            {
                this.cmsUpdate.RefreshParent += cmsUpdate_RefreshParent;

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
                        mySession.ReturnURL = string.Empty;
                        mySession.TempID = 0;
                    }
                    try
                    {
                        this._presenter.InitView();

                        this.btnDeleteContent.Enabled = false;

                        Master.CurrentModuleTitle = string.Empty;

                        CurrentModule("Administration - Content Management System");
                        CurrentModule_Sub(string.Empty);
                        BuildPageTitle();
                    }
                    catch (NullReferenceException ex)
                    {
                        ex.LogException();
                        Response.Redirect(FormsAuthentication.DefaultUrl, false);
                    }
                }
            }
        }

        protected void cmsUpdate_RefreshParent(object sender, EventArgs e)
        {
            RefreshPage();
        }

        protected void ddlTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsCMSManager_ddlTitles_SelectedIndexChanged", mySession.MyUserID))
#endif
            {
                var i = Convert.ToInt32(this.ddlTitles.SelectedValue);
                this.btnDeleteContent.Enabled = !i.Equals(0);

                var c = new CMSPresenter(this.cmsUpdate);
                if (!i.Equals(0))
                {
                    c.InitView(i);
                }
                else
                {
                    RefreshPage();
                    c.InitView();
                }
            }
        }

        protected void btnDeleteContent_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsCMSManager_btnDeleteContent_Click", mySession.MyUserID))
#endif
            {
                this._presenter = new CmsManagerPresenter(this);
                this._presenter.DeleteContent(Convert.ToInt32(this.ddlTitles.SelectedValue));
                LogEvent(String.Format("User {0} deleted CMS content '{1}' at {2}.", mySession.MyUserID, this.ddlTitles.SelectedItem, DateTime.Now));
                this._presenter.InitView();

                var c = new CMSPresenter(this.cmsUpdate);
                c.InitView();
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
                CurrentModule("Administration - Content Management System (CMS)");
                CurrentModule_Sub(string.Empty);
            }
        }

        private void RefreshPage()
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SrtsCMSManager_RefreshPage", mySession.MyUserID))
#endif
            {
                this._presenter = new CmsManagerPresenter(this);
                this._presenter.InitView();

                this.btnDeleteContent.Enabled = false;
            }
        }

        #region INTERFACE PROPERTIES

        private List<CmsMessage> _ContentTitles;

        public List<CmsMessage> ContentTitles
        {
            get { return _ContentTitles; }
            set
            {
                _ContentTitles = value;
                this.ddlTitles.Items.Clear();
                this.ddlTitles.DataTextField = "cmsContentTitle";
                this.ddlTitles.DataValueField = "cmsContentID";
                this.ddlTitles.DataSource = _ContentTitles;
                this.ddlTitles.DataBind();
                this.ddlTitles.Items.Insert(0, new ListItem("--ADD NEW--", "0"));
            }
        }

        public Int32 SelectedContentTitleId
        {
            get { return Convert.ToInt32(this.ddlTitles.SelectedValue); }
            set { this.ddlTitles.SelectedValue = value.ToString(); }
        }

        public int CurrentAuthorId
        {
            get { return mySession.MyIndividualID; }
        }

        #endregion INTERFACE PROPERTIES
    }
}