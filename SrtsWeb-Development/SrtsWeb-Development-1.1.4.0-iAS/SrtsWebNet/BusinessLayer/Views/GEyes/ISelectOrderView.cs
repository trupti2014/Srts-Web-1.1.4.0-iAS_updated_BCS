using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Views.GEyes
{
    public interface ISelectOrderView
    {
        GEyesSession myInfo { get; set; }

        List<OrderDisplayEntity> OrdersData { get; set; }

        string FrameDescription { get; set; }

        string LensTint { get; set; }

        string LensTypeLong { get; set; }

        string Comment { get; set; }
    }
}