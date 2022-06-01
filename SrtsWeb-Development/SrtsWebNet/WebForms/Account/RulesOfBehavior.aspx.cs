using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Account;
using SrtsWeb.Views.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Configuration;
using SrtsWeb.BusinessLayer.Concrete;

namespace SrtsWeb.Account
{
    public partial class RulesOfBehavior : PageBase, IRulesOfBehaviorView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var p = new RulesOfBehaviorPresenter(this);
            p.GetAllRulesOfBehavior();

            //if (!IsPostBack)
            //{
            //    if (mySession == null)
            //    {
            //        mySession = new SRTSSession();
            //        mySession.ReturnURL = string.Empty;
            //        mySession.TempID = 0;
            //    }
            //    try
            //    {
            //        Master.CurrentModuleTitle = String.Empty;

            //        BuildPageTitle();
            //    }
            //    catch (NullReferenceException)
            //    {
            //        Response.Redirect(FormsAuthentication.DefaultUrl);
            //    }
            //}

            var c = Master.FindControl("pnlContentAuthenticated") as System.Web.UI.WebControls.Panel;
            c.Visible = false;
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            var p = new RulesOfBehaviorPresenter(this);
            p.InsertUpdateRulesOfBehaviorDate(Membership.GetUser().UserName, DateTime.Today);

            Response.Redirect("~/WebForms/default.aspx?ss=1", false);
        }

        protected void btnDoNotAccept_Click(object sender, EventArgs e)
        {
            //Response.Redirect("~/WebForms/Account/Logout.aspx?cs=4", false);
            var mUid = mySession.MyUserID;

            MembershipService.DoLogOut(Context.User.Identity.Name);

            var url = String.Format(ConfigurationManager.AppSettings["LogoutUrl"] + "?cs=4&u={0}", mUid);

            Response.Redirect(url, true);
        }
        private void BuildPageTitle()
        {
            try
            {
                //Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                //Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                //CurrentModule("SRTSWeb Rules of Behavior");
                //CurrentModule_Sub(string.Empty);
            }
        }

        private List<RuleOfBehavior> _RulesOfBehavior;

        public List<RuleOfBehavior> RulesOfBehaviorList
        {
            get { return _RulesOfBehavior; }
            set
            {
                _RulesOfBehavior = value;
                this.blcRulesOfBehavior.DataSource = _RulesOfBehavior;
                this.blcRulesOfBehavior.DataBind();
                
            }
        }
    }
}