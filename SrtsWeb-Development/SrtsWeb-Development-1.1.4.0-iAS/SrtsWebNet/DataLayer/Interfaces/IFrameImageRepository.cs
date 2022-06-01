using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IFrameImageRepository
    {

        List<FrameImageEntity> GetFrameImage(String FrameCode, String FrameFamily);

        List<FrameImageEntity> GetFrame(int RecordID);

        Boolean InsertUpdateFrameImage(FrameImageEntity FrameImage);

        Boolean UpdateFrameImage(FrameImageEntity FrameImage);

        Dictionary<String, String> GetFramesByFrameFamily(int FamilyID);

    }
}
