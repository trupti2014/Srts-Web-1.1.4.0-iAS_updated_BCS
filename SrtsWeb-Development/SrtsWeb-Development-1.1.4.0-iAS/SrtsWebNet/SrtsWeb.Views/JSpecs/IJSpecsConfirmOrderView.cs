using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.JSpecs
{
    public interface IJSpecsConfirmOrderView
    {
        JSpecsSession userInfo { get; set; }

        string ErrorMessage { set; }
    }
}
