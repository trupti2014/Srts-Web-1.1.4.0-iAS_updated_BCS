using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;

namespace SrtsWeb.MembershipProviders
{
    public class SrtsSqlRoleProvider : SqlRoleProvider
    {
        private RoleProviderService svc;

        public SrtsSqlRoleProvider() : base() { this.svc = new RoleProviderService(); }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["userName"] == null)
            {
                base.Initialize(name, config);
                return;
            }

            var un = HttpContext.Current.Session["userName"].ToString().ToLower();
            var trainingSites = ConfigurationManager.AppSettings["trainingSites"].Split(new[] { ',' }).ToList();

            var cfgNm = trainingSites.FirstOrDefault(x => un.StartsWith(x.ToLower())) ?? "SRTS";

            config["connectionStringName"] = cfgNm;

            base.Initialize(name, config);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            this.svc.AddUsersToRoles(usernames, roleNames);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return this.svc.FindUsersInRole(roleName, usernameToMatch);
        }

        public override string[] GetAllRoles()
        {
            return this.svc.GetAllRoles();
        }

        public override string[] GetRolesForUser(string username)
        {
            return this.svc.GetRolesForUser(username);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return this.svc.GetUsersInRole(roleName);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return this.svc.IsUserInRole(username, roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            this.svc.RemoveUsersFromRoles(usernames, roleNames);
        }
    }
}