using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.DataLayer.Repositories;

namespace SrtsWeb.BusinessLayer.Concrete
{
    public class RoleProviderService : System.Web.Security.RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            var r = new RoleProviderRepository();
            r.AddUsersToRoles(usernames, roleNames, this.ApplicationName);
        }

        public override string ApplicationName
        {
            get
            {
                return System.Web.Security.Roles.ApplicationName;
            }
            set
            {
                System.Web.Security.Roles.ApplicationName = value;
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var r = new RoleProviderRepository();
            return r.FindUsersInRole(roleName, usernameToMatch, this.ApplicationName);
        }

        public override string[] GetAllRoles()
        {
            var r = new RoleProviderRepository();
            return r.GetAllRoles(this.ApplicationName);
        }

        public override string[] GetRolesForUser(string username)
        {
            var r = new RoleProviderRepository();
            return r.GetRolesForUser(username, this.ApplicationName);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var r = new RoleProviderRepository();
            return r.GetUsersInRole(roleName, this.ApplicationName);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var r = new RoleProviderRepository();
            return r.IsUserInRole(username, roleName, this.ApplicationName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var r = new RoleProviderRepository();
            r.RemoveUsersFromRoles(usernames, roleNames, this.ApplicationName);
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
