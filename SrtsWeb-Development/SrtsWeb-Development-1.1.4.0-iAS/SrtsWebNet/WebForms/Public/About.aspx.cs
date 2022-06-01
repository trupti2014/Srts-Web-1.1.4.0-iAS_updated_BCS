﻿using SrtsWeb.Base;
using System;

namespace SrtsWeb
{
    public partial class About : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var c = Master.FindControl("pnlContentAuthenticated") as System.Web.UI.WebControls.Panel;
            c.Visible = false;
            mySession.SecurityAcknowledged = true;
        }
    }
}