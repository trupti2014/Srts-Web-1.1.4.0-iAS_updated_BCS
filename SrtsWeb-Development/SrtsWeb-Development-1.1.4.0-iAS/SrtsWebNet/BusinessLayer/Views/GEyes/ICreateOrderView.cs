using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.GEyes
{
    public interface ICreateOrderView
    {
        GEyesSession myInfo { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        DataTable FrameData { get; set; }

        string FrameSelected { get; set; }

        DataTable ColorData { get; set; }

        string ColorSelected { get; set; }

        DataTable TintData { get; set; }

        string TintSelected { get; set; }

        string Message { get; set; }

        DataTable PriorityData { get; set; }

        string PrioritySelected { get; set; }

        string Comment { get; set; }

        DataTable LocationData { get; set; }

        string LocationSelected { get; set; }

        int Cases { get; set; }

        int Pairs { get; set; }
    }
}