using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.JSpecs
{
    public interface IJSpecsOrdersView
    {
        JSpecsSession userInfo { get; set; }

        List<JSpecsOrderDisplayEntity> OrdersData { get; set; }
        string ErrorMessage { set; }
    }
}
