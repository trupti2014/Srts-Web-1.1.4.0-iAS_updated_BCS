using System;
using SrtsWeb.Base;

namespace SrtsWeb.Account
{
    public partial class ChangePasswordSuccess : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogEvent(String.Format("User {0} changed their password at {1}.", mySession.MyUserID, DateTime.Now));
        }
    }
}