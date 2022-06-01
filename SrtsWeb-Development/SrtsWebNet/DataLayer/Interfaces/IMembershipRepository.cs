using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IMembershipRepository
    {
        List<PasswordHistory> GetPasswordHistoryByUserName(String UserName);
        string GetCurrentSalt(string UserName);
        [System.Security.Permissions.PrincipalPermission(System.Security.Permissions.SecurityAction.Demand, Role = "MgmtEnterprise")]
        bool AdminSetPasswordHash(string UserName, string password, string salt);
    }
}