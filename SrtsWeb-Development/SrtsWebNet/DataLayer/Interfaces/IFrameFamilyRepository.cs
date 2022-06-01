using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IFrameFamilyRepository
    {
        List<FrameFamilyEntity> GetFrameFamily();
    }
}
