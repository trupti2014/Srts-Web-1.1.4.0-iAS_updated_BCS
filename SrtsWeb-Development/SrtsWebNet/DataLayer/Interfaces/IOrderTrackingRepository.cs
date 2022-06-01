using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.DataLayer.Interfaces
{
    public interface IOrderTrackingRepository
    {
        List<OrderTrackingEntity> GetTrackedOrders(string SiteCode);
    }

    public interface IOrderTrackingScanRepository
    {
        List<OrderTrackingScanEntity> GetTrackingNumberOrders(string TrackingNumber);

        String IUTrackingNumber(OrderTrackingScanEntity OTSEntity, string OrderNumber, string Shipper, string What);

        String ITrackingNumberManual(OrderTrackingScanEntity OTSEntity, string OrderNumber, string Shipper);
    }

    public interface IOrderTrackingPatientRepository
    {
        List<OrderTrackingPatientEntity> GetTrackingNumbersPatientSite(int PatientID, string SiteCode);
    }

    public interface IOrderTrackingOrderRepository
    {
        List<OrderTrackingOrderEntity> GetPatientID(string OrderNumber);
    }

}
