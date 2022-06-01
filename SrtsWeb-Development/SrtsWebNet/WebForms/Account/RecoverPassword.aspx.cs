using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Base;
using System;
using System.Text;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SrtsWeb.Account
{
    public partial class RecoverPassword : PageBase
    {
        private MembershipUser _User { get { return ViewState["user"] as MembershipUser; } set { ViewState["user"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ForgotPassword_SendingMail(object sender, MailMessageEventArgs e)
        {
#if DEBUG
            using (SrtsWeb.BusinessLayer.Concrete.MethodTracer.Trace(SrtsTraceSource.LoginSource, "RecoverPassword_ForgotPassword_SendingMail", mySession.MyUserID))
#endif
            {
                if (this._User.LastPasswordChangedDate.AddHours(24) > DateTime.Now)
                {
                    e.Message.Body = String.Format("The system does not allow password changes within 24 hours of setting a new password. We do however recommend, after the required wait time has expired, that you change this password at the login screen by clicking the Change Password link.");
                    LogEvent("User {0} attempted to recover password within 24 hour time limit of their last password change date, {1}, and was sent and email on {2}.",
                        new Object[] { ForgotPassword.UserName, this._User.LastPasswordChangedDate, DateTime.Now });


                    ((Label)ForgotPassword.SuccessTemplateContainer.FindControl("lblMessage")).Text = "There was an error recovering your password, please contact an administrator.";
                    return;
                }

                // Get the "old" password from the body of the message
                var oldP = e.Message.Body;
                oldP = oldP.Substring(5);

                // Generate a new password
                var newP = String.Empty;
#if DEBUG
                using (SrtsWeb.BusinessLayer.Concrete.MethodTracer.Trace(SrtsTraceSource.LoginSource, "RecoverPassword_Helpers.GetRandomPwd", mySession.MyUserID))
#endif
                    newP = Helpers.GetRandomPwd(15);

                // Save the password
                e.Cancel = !this._User.ChangePassword(oldP, newP);

                // Set the body of the message to the new pwd
                var body = new StringBuilder(e.Message.Body.Replace(oldP, ""));
                body.Append(newP);
                body.AppendLine();
                body.AppendLine();
                body.AppendLine("The system does not allow password changes within 24 hours of setting a new password. We do however recommend, after the required wait time has expired, that you change this password at the login screen by clicking the Change Password link.");
                e.Message.Body = body.ToString();
                LogEvent(String.Format("User {0} was emailed a new password to {2} at {1}.", mySession.MyUserID, DateTime.Now, this._User.Email));
            }
        }

        protected void ContinuePushButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/Account/Login.aspx");
        }

        // Get and verify the user by username before the inital password change.
        protected void ForgotPassword_VerifyingUser(object sender, LoginCancelEventArgs e)
        {
            this._User = Membership.GetUser(ForgotPassword.UserName);
        }
    }
}