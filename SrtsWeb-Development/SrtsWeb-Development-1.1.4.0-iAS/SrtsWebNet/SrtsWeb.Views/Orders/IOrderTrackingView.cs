using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.Views.Orders
{
    public interface IOrderTrackingView
    {

        SRTSSession mySession { get; set; }

        /// <summary>
        /// Get the initial table
        /// </summary>
        List<OrderTrackingEntity> TrackingBaseList { get; set; }

        /// <summary>
        /// get edit table
        /// </summary>
        List<OrderTrackingPatientEntity> TrackingOrderPatientList { get; set; }

        /// <summary>
        /// get scan items
        /// </summary>
        List<OrderTrackingScanEntity> TrackingScanList { get; set; }

        // <summary>
        // get Patient
        // </summary>
        List<OrderTrackingOrderEntity> PatientItem { get; set; }

    }
}
