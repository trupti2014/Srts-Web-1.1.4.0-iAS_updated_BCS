using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Public
{
    public interface ISystemAccessRequest
    {
        String RequesterName { get; set; }

        String RequesterTitle { get; set; }

        String PhoneNumber { get; set; }

        String Email { get; set; }

        String SiteCode { get; set; }

        List<SiteCodeEntity> SiteList { get; set; }
    }
}