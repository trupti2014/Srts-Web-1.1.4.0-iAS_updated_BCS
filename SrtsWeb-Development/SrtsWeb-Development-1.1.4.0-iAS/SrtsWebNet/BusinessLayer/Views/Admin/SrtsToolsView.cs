using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.BusinessLayer.Views.Admin
{
    public interface ISrtsToolsView
    {
        SRTSSession mySession { get; set; }
    }
}
