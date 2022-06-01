using SrtsWeb.CustomErrors;
using System;

namespace SrtsWeb.Public
{
    public partial class PublicPageTemplate : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master._MainMenu.Visible = false;
            mySession.SecurityAcknowledged = true;
        }
    }
}