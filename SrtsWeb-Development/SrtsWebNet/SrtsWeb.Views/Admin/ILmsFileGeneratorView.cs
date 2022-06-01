using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface ILmsFileGeneratorView
    {
        List<String> BadOrders { get; set; }

        List<String> GoodOrders { get; set; }

        String ErrorMessage { get; set; }

        List<SiteCodeEntity> LabList { set; }

        String SiteCode { get; }

        Boolean MarkComplete { get; }
    }
}