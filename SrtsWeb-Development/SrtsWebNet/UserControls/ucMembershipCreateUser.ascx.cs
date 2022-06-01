using SrtsWeb.ExtendersHelpers;
using System;
using System.Security.Permissions;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucMembershipCreateUser : System.Web.UI.UserControl
    {
        public EventHandler UserCreated;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            if (Session["NewUserName"] != null)
                Session.Remove("NewUserName");

            RegisterUser.ActiveStepIndex = 0;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/SrtsUserManager.aspx");
        }

        protected void RegisterUser_CreatingUser(object sender, LoginCancelEventArgs e)
        {
            this.ErrorMessage.Text = String.Empty;
            var err = new StringBuilder();

            if (Membership.GetUser(RegisterUser.UserName) != null)
                err.AppendLine("User name is already taken<br />");

            var p = RegisterUser.Password;
            var r = String.Empty;
            p.ValidatePasswordComplexity(out r);

            if (!String.IsNullOrEmpty(r))
                err.AppendLine(r);

            if (String.IsNullOrEmpty(err.ToString())) return;

            ErrorMessage.Text = err.ToString();
            ErrorMessage.Visible = true;
            e.Cancel = true;
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ErrorMessage.Text));
        }

        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
            System.Web.HttpContext.Current.Session.Add("NewUserName", this.RegisterUser.UserName);
            ScriptManager.RegisterStartupScript(this, typeof(UserControl), "", "alert('Username Added Successfully!');", true);
            UserCreated(sender, e);
        }
    }
}