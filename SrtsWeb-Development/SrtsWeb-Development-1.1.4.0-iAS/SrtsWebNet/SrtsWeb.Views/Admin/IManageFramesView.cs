using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface IManageFramesView
    {

        SRTSSession mySession { get; set; }

        string ContentType { get; set; }

        byte[] FrameImage { get; set; }

        String ImageAngle { get; set; }
        List<FrameImageImageAngleEntity> ImageAngleList { get; set; }

        String FrameFamily { get; set; }
        List<FrameFamilyEntity> FrameFamilyList { get; set; }

        String Frame { get; set; }
        Dictionary<String, String> FrameList { get; set; }

        String Color { get; set; }
        Dictionary<String, String> ColorList { get; set; }

        String Eye { get; set; }
        Dictionary<String, String> EyeList { get; set; }

        String Bridge { get; set; }
        Dictionary<String, String> BridgeList { get; set; }

        String Temple { get; set; }
        Dictionary<String, String> TempleList { get; set; }

        FrameItemDefaultEntity FrameItemDefault { get; set; }

        FrameImageEntity FrameImageItem { get; }
        IEnumerable<FrameImageEntity> FrameImageItemList { get; set; }

    }
}
