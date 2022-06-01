using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.Views.Account
{
    public interface IAuthorizationView
    {
        DataTable dtAuthorizations { get; set; }

        bool isCacOnFile { get; set; }

        bool isUserFound { get; set; }

        bool isSsoUserNameFound { get; set; }

        bool isMultipleAccounts { get; set; }

        List<CMSEntity> Announcements { set; }
    }
}