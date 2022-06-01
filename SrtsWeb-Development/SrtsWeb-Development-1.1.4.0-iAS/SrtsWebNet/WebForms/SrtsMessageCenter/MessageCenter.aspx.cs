using Microsoft.Web.Administration;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.MessageCenter;
using SrtsWeb.Views.MessageCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SrtsWeb.SrtsMessageCenter
{
    public partial class MessageCenter : PageBase, IMEssageCenterView
    {
        private MessageCenterPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            this._presenter = new MessageCenterPresenter(this);
            this._presenter.InitView();

            if (!String.IsNullOrEmpty(Request.QueryString["tId"]))
            {
                var i = default(Int32);
                Int32.TryParse(Request.QueryString["tId"], out i);
                var p = this.accMaster1.Panes;
                for (var a = 0; a < p.Count; a++)
                {
                    var hf = (HiddenField)p[a].FindControl("hfId");
                    if (!String.IsNullOrEmpty(hf.Value) && i.Equals(Convert.ToInt32(hf.Value)))
                    {
                        this.accMaster1.SelectedIndex = a;
                        break;
                    }
                }
            }

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
                    Master.CurrentModuleTitle = String.Empty;

                    CurrentModule("My Message Center");
                    CurrentModule_Sub(string.Empty);
                    BuildPageTitle();
                }
                catch (NullReferenceException)
                {
                    Response.Redirect(FormsAuthentication.DefaultUrl);
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
                CurrentModule("My Message Center");
                CurrentModule_Sub(string.Empty);
            }
        }

        private List<CMSEntity> _Messages;

        public List<CMSEntity> Messages
        {
            get
            {
                return _Messages;
            }
            set
            {
                _Messages = value;

                this.accMaster1.DataSource = _Messages;
                this.accMaster1.DataBind();
            }
        }

        protected void accMaster1_ItemDataBound(object sender, AjaxControlToolkit.AccordionItemEventArgs e)
        {
            if (e != null && e.Item is CMSEntity)
            {
                var i = e.Item as CMSEntity;
                var hf = (HiddenField)e.AccordionItem.FindControl("hfId");
                var lh = (Literal)e.AccordionItem.FindControl("litHeader");
                var lc = (Literal)e.AccordionItem.FindControl("litContent");

                if (hf != null)
                    hf.Value = i.cmsContentID.ToString();
                if (lh != null)
                    lh.Text = String.Format(
                        "<p class=\"accordianNewsHeadLine\">{0} | <span class=\"accordianNewsDate\">Posted: {1} by {2}.  Message will be visible for <b>{3}</b> more days</span></p>",
                        i.cmsContentTitle,
                        i.cmsCreatedDate.ToString(Globals.DtFmt),
                        String.Format("{0} {1}", i.AuthorFirstName, i.AuthorLastName),
                        i.cmsContentExpireDate.DateDiff(DateTime.Today));
                if (lc != null)
                {
                    var s = new StringBuilder();
                    var ls = i.cmsContentBody.Replace(Environment.NewLine, "`").Split(new[] { '`' }, StringSplitOptions.None).ToList();
                    foreach (var l in ls)
                        s.AppendFormat("<p class=\"newssummary\">{0}</p>", l);
                    lc.Text = String.Format(s.ToString());
                }
            }
        }
    }
}