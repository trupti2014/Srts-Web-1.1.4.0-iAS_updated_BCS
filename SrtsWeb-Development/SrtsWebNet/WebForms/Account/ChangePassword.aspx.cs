using SrtsWeb.CustomProviders;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SrtsWeb.Account
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var m = Membership.GetUser();
            var min = ConfigurationManager.AppSettings["minPasswordLife_Hours"].ToInt32();
            var isActive = m.LastPasswordChangedDate.AddHours(min) <= DateTime.Now;
            this.ChangeUserFact2.Enabled = isActive;
        }

        protected void CustPasswordDiff_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var l = this.ChangeUserFact2.Controls[0].FindControl("FailureText") as Literal;
            // Clear error text
            l.Text = String.Empty;

            var oldP = ChangeUserFact2.Controls[0].FindControl("CurrentPassword") as TextBox;
            var s1 = oldP.Text;
            var s2 = args.Value;

            var good = Enumerable.Range(0, Math.Min(s1.Length, s2.Length)).Count(i => s1[i] != s2[i]);
            var minChars = ConfigurationManager.AppSettings["minPasswordCharChange"].ToInt32();
            args.IsValid = good.GreaterThanET(minChars);
            if (args.IsValid) return;
            l.Text = String.Format("At least {0} characters must be different between the old and new password", minChars);
        }

        protected void ChangeUserFact2_ChangedPassword(object sender, EventArgs e)
        {
            var u = Membership.GetUser();
            SrtsWeb.BusinessLayer.Concrete.SessionService.CreateAndStoreSessionTicket(u.UserName);
        }

        protected void ChangeUserFact2_ChangePasswordError(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(((SrtsMembership)Membership.Provider).ChangeError)) return;
            else
                this.ChangeUserFact2.ChangePasswordFailureText = ((SrtsMembership)Membership.Provider).ChangeError;
        }
    }
}