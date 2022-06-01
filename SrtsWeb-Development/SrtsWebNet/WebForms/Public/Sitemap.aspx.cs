using SrtsWeb.Base;
using System;

namespace SrtsWeb.Public
{
    public partial class Sitemap : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            mySession.SecurityAcknowledged = true;
        }
    }
}