using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface IMembershipService
    {
        List<PasswordHistory> GetPasswordSaltHistory(String userName);
        [System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = "MgmtEnterprise")]
        bool AdminSetPassword(string userName, string passwordHash, string salt);
        string GetCurrentSalt(string username);
    }
}