using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface IReleaseManagementUserGuidesView
    {
        SRTSSession mySession { get; set; }

        ReleaseManagementUserGuideEntity ReleaseManagementUserGuideEntity { get; set; }

        List<ReleaseManagementUserGuideEntity> UserGuideData { get; set; }
   
    }
}