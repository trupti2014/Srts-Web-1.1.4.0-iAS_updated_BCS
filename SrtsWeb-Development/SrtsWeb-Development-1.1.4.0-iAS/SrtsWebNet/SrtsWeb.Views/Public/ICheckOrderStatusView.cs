using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;

namespace SrtsWeb.Views.Public
{
    public interface ICheckOrderStatusView
    {
        //PatientEntity Patient { get; set; }
        String IdNumber { get; }
        //List<Order> Orders { get; set; }
        //List<OrderStateEntity> OrderStatus { get; set; }
        List<CheckOrderStatusEntity> PatientStatuses { get; set; }
        List<IndividualEntity> PatientInfo { get; set; }
    }
}
