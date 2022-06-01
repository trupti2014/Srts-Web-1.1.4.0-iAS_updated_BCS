using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface IOrderPreferencesView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        IDictionary<String, String> PriorityFrameList { get; set; }

        IDictionary<String, String> InitialLoadFrameList { get; set; }

        IEnumerable<LookupTableEntity> PriorityPriorityList { get; set; }

        IEnumerable<LookupTableEntity> InitialLoadPriorityList { get; set; }

        IDictionary<String, String> DistributionMethodList { get; set; }

        IDictionary<String, String> LabList { get; set; }

        List<SitePrefOrderPriorityPreferencesEntity> PriorityPreferenceHistory { get; set; }

        String InitialLoadPriority { get; set; }

        String InitialLoadFrame { get; set; }

        String GlobalOrderLab { get; set; }

        String GlobalDistributionMethod { get; set; }

        String PriorityPriority { get; set; }

        String PriorityFrame { get; set; }

        String SiteCode { get; }

        IEnumerable<SiteCodeEntity> SiteCodes { get; }

        SitePrefOrderEntity OrderPreferences { get; set; }
    }
}