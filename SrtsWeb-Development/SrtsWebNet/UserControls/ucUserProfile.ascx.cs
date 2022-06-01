using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SrtsWeb.UserControls
{
    public partial class ucUserProfile : UserControlBase
    {
        private CurrentUser myCurrentUser = new CurrentUser();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                try
                {
                    gvUserPersonalInformation.DataSource = myCurrentUser.UserPersonalInfo.ToTable();
                    gvUserPersonalInformation.DataBind();

                    gvMembershipInformation.DataSource = myCurrentUser.UserMembership.ToTable();
                    gvMembershipInformation.DataBind();

                    gvFacilityInformation.DataSource = myCurrentUser.UserSiteInfo;
                    gvFacilityInformation.DataBind();
                }
                catch (NullReferenceException)
                {
                    Response.Redirect("~/WebForms/Default.aspx");
                }
            }
        }

        protected string UserFullName()
        {
            string UserFullName = string.Format("{0} {1} {2}", myCurrentUser.UserPersonalInfo.FirstName,
                                                                myCurrentUser.UserPersonalInfo.MiddleName,
                                                                myCurrentUser.UserPersonalInfo.LastName);
            return UserFullName;
        }

        protected string UserEmailAddress()
        {
            return myCurrentUser.UserMembership.Email;
        }

        protected string UserLoginName()
        {
            string UserLoginName = string.Format("{0}", myCurrentUser.UserMembership.UserName);
            return UserLoginName;
        }

        protected string UserRoles()
        {
            List<string> UserRoles = (from role in myCurrentUser.UserRoles
                                      where myCurrentUser.UserRoles.Length > 0
                                      select role).ToList();
            if (UserRoles.Count > 0)
            {
                return UserRoles.Aggregate((role1, role2) => role1 + ", " + role2);
            }
            else
            {
                return UserRoles.ToString();
            }
        }

        protected DataTable UserSiteInfo()
        {
            DataTable UserSiteInfo = new DataTable();
            UserSiteInfo = myCurrentUser.UserSiteInfo.ToTable();
            return UserSiteInfo;
        }

        protected void lnkAssociateAccounts_Click(object sender, EventArgs e)
        {
            mySession.CacRegistrationCode = 5;
            Response.Redirect("~/WebForms/Account/Login.aspx");
        }
    }
}