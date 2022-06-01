using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Public;
using SrtsWeb.Views.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SrtsWeb.Public
{
    public partial class ReleaseNotes : PageBase, IReleaseNotesView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var p = new ReleaseNotesPresenter(this);
            p.GetAllReleaseNotes();

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

                    BuildPageTitle();
                }
                catch (NullReferenceException)
                {
                    Response.Redirect(FormsAuthentication.DefaultUrl);
                }
            }

            var c = Master.FindControl("pnlContentAuthenticated") as System.Web.UI.WebControls.Panel;
            c.Visible = false;
        }

        protected void accReleaseNotes_ItemDataBound(object sender, AjaxControlToolkit.AccordionItemEventArgs e)
        {
            if (e != null && e.Item is ReleaseNote)
            {
                var s = new StringBuilder();
                var i = e.Item as ReleaseNote;
                var lh = (Literal)e.AccordionItem.FindControl("litHeader");
                var lc = (Literal)e.AccordionItem.FindControl("litContent");

                if (lh != null)
                {
                    s.AppendFormat(
                        "<p class=\"accordianNewsHeadLine\">Version: {0} | <span class=\"accordianNewsDate\">Release Date: {1}</span>.</p>",
                        i.VersionNumber,
                        i.VersionDate.ToString(Globals.DtFmt));

                    lh.Text = s.ToString();
                }

                if (lc != null)
                {
                    s.Clear();
                    var l = i.ReleaseNotes.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var a in l)
                        s.AppendFormat("<p class=\"newssummary\">-- {0}</p>", a);

                    lc.Text = s.ToString();
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
                CurrentModule("SRTSWeb Release Notes");
                CurrentModule_Sub(string.Empty);
            }
        }

        private List<ReleaseNote> _ReleaseNotes;

        public List<ReleaseNote> ReleaseNoteList
        {
            get { return _ReleaseNotes; }
            set
            {
                _ReleaseNotes = value;
                this.accReleaseNotes.DataSource = _ReleaseNotes;
                this.accReleaseNotes.DataBind();
            }
        }
    }
}