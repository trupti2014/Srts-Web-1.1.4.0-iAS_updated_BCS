using System;

namespace SrtsWeb
{
    public class basePageSessionExpire : System.Web.UI.Page
    {
        public basePageSessionExpire()
        {
        }

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Context.Session == null)
            {
                Response.Redirect("~/WebForms/WebForms/sessionTimedOut.aspx");
            }
        }
    }
}