using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.MessageCenter
{
    public interface IMEssageCenterView
    {
        SRTSSession mySession { get; set; }

        List<CMSEntity> Messages { get; set; }
    }
}