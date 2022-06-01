using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Security.Permissions;
using SrtsWeb.Views.Admin;
using SrtsWeb.Entities;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.BusinessLayer.Concrete;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucSitePreferencesLabJustifications : Base.UserControlBase, ISitePreferencesLabJustification
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((SrtsWeb.WebForms.Admin.SitePreferences)this.Page).RefreshUserControl += ucSitePreferencesLabJustifications_RefreshUserControl;

            if (!IsPostBack)
            {
                DoLoadOperations();
            }
        }
        protected void ucSitePreferencesLabJustifications_RefreshUserControl(object sender, EventArgs e)
        {
            DoLoadOperations();
        }
        protected void bSubmit_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ucSitePreferencesLabJustifications_bSubmit_Click", this.mySession.MyUserID))
#endif
            {
                if (IsObjectSame()) return;
                try
                {
                    var p = new SitePreferencesPresenter.LabJustificationPresenter(this);
                    using (p)
                    {
                        var good = p.SetLabJustifications().Equals(true) ? "1" : "0";
                        this.hfSuccessLabJust.Value = good;
                        this.hfMsgLabJust.Value = good.Equals("1") ? "Successfully saved justification(s)." : "Error deleting justification(s).";
                    }
                }
                catch (Exception ex)
                {
                    ex.TraceErrorException();
                    ex.LogException();
                }
            }
        }
        protected void bDeleteReject_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ucSitePreferencesLabJustifications_bDeleteReject_Click", this.mySession.MyUserID))
#endif
            {
                //   if (this.OriginalJustifications.FirstOrDefault(x => x.JustificationReason == "reject").Justification.IsNullOrEmpty()) return; //Aldela: Commented this line
                //    DoDelete("reject");
                var rj = this.RejectJustification;
                rj.Justification = String.Empty;
                this.RejectJustification = rj;
            }
        }
        protected void bDeleteRedirect_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ucSitePreferencesLabJustifications_bDeleteRedirect_Click", this.mySession.MyUserID))
#endif
            {
                // if (this.OriginalJustifications.FirstOrDefault(x => x.JustificationReason == "redirect").Justification.IsNullOrEmpty()) return; //Aldela: Commented this line
                //  DoDelete("redirect");
                var rj = this.RedirectJustification;
                rj.Justification = String.Empty;
                this.RedirectJustification = rj;
            }
        }

        private void DoDelete(String justificationType)
        {
            var p = new SitePreferencesPresenter.LabJustificationPresenter(this);
            using (p)
            {
                try
                {
                    p.DeleteLabJustifications(justificationType);
                    this.hfSuccessLabJust.Value = "1"; 
                    this.hfMsgLabJust.Value = "Successfully saved justification(s)."; 
                }
                catch (Exception ex)
                {
                    ex.TraceErrorException();
                    ex.LogException();
                }
            }
        }
        private Boolean IsObjectSame()
        {
            return this.Justifications.GetObjectHash().Equals(JustificationHash);
        }
        private void DoLoadOperations()
        {
            var p = new SitePreferencesPresenter.LabJustificationPresenter(this);
            using (p)
            {
                p.InitView();
            }
        }

        #region INTERFACE PROPERTIES
        public string SiteCode
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCode;
            }
        }
        public String JustificationHash
        {
            get
            {
                return ViewState["jHash"].ToString();
            }
            set
            {
                ViewState["jHash"] = value;
            }
        }
        public List<SitePrefLabJustification> Justifications
        {
            get
            {
                var jl = new List<SitePrefLabJustification>();
                jl.Add(this.RedirectJustification);
                jl.Add(this.RejectJustification);
                return jl;
            }
            set
            {
                ViewState["OriginalJustifications"] = value;
                this.RedirectJustification = value.FirstOrDefault(x => x.JustificationReason == "redirect") ?? new SitePrefLabJustification();
                this.RejectJustification = value.FirstOrDefault(x => x.JustificationReason == "reject") ?? new SitePrefLabJustification();
            }
        }
        #endregion

        #region LOCAL PROPERTIES
        public List<SitePrefLabJustification> OriginalJustifications 
        {
            get { return ViewState["OriginalJustifications"] as List<SitePrefLabJustification>; } 
        }
        public SitePrefLabJustification RejectJustification
        {
            get
            {
                var r = new SitePrefLabJustification();
                r.SiteCode = this.SiteCode;
                r.Justification = this.tbReject.Text;
                r.JustificationReason = "reject";
                return r;
            }
            set
            {
                this.tbReject.Text = value.Justification;
            }
        }
        public SitePrefLabJustification RedirectJustification
        {
            get
            {
                var r = new SitePrefLabJustification();
                r.SiteCode = this.SiteCode;
                r.Justification = this.tbRedirect.Text;
                r.JustificationReason = "redirect";
                return r;
            }
            set
            {
                this.tbRedirect.Text = value.Justification;
            }
        }
        #endregion
    }
}