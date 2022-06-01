using SrtsWeb.Base;
using System;

namespace SrtsWeb.Public
{
    public partial class Accessability : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var c = Master.FindControl("pnlContentAuthenticated") as System.Web.UI.WebControls.Panel;
            c.Visible = false;
            mySession.SecurityAcknowledged = true;

            CurrentModule("SRTSweb Information");
            CurrentModule_Sub(" <br /> Our Commitment to Web Site Accessibility");

            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
            Master.uplCurrentModuleTitle.Update();
            litModuleTitle.Text = Master.CurrentModuleTitle;
        }

        protected void BuildUserInterface()
        {
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
        }
    }
}