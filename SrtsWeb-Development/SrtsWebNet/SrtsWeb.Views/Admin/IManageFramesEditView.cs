using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface IManageFramesEditView
    {
        SRTSSession mySession { get; set; }

        List<FrameImageEntity> FrameImageRecords { get; set; }

        String FrameFamily { get; set; }
        List<FrameFamilyEntity> FrameFamilyList { get; set; }

        String Frame { get; set; }
        Dictionary<String, String> FrameList { get; set; }

    }
}
