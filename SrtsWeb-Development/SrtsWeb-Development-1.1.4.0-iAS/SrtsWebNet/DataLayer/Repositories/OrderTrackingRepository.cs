using DataBaseAccessLayer;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.DataLayer.Repositories
{
    public sealed class OrderTrackingRepository : RepositoryBase<OrderTrackingEntity>, IOrderTrackingRepository
    {

        /// <summary>
        /// Default ctor.
        /// </summary>
        public OrderTrackingRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        /// <summary>
        /// Gets inital datatable load for page.
        /// </summary>
        /// <returns>OrderTrackingEntity list of tracked orders for site within 48hrs.</returns>
        public List<OrderTrackingEntity> GetTrackedOrders(string SiteCode)
        {
            var Command = this.DAL.GetCommandObject();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "GetTrackedOrders";

            Command.Parameters.Add(this.DAL.GetParamenter("@SiteCode", SiteCode));
            Command.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));
            return GetRecords(Command).ToList();
        }

        /// <summary>
        /// Fill Tracking Base List
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected override OrderTrackingEntity FillRecord(IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            var OTEntity = new OrderTrackingEntity();

            OTEntity.TrackingNumber = dr.AsString("TrackingNumber", c);
            OTEntity.OrderCount = dr.ToInt32("OrderCount", c);
            OTEntity.Patient = dr.AsString("Patient", c);

            return OTEntity;
        }

    }

    public sealed class OrderTrackingScanRepository : RepositoryBase<OrderTrackingScanEntity>, IOrderTrackingScanRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public OrderTrackingScanRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        /// <summary>
        /// Gets the scanned tracking number orders.
        /// </summary>
        /// <returns>OrderTrackingScanEntity list of order numbers for that patient.</returns>
        public List<OrderTrackingScanEntity> GetTrackingNumberOrders(string TrackingNumber)
        {
            var Command = this.DAL.GetCommandObject();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "GetTrackingNumberOrders";

            Command.Parameters.Add(this.DAL.GetParamenter("@TrackingNumber", TrackingNumber));
            Command.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));
            return GetRecords(Command).ToList();
        }

        /// <summary>
        /// Adds Updates Deletes Order Tracking in the database.
        /// </summary>
        /// <param name="TrackingNumber">Tracking Number.</param>
        /// <param name="OrderNumber">Order.</param>
        /// <param name="PatientID">Patient.</param>
        /// <param name="What">Action Item.</param>
        /// <returns>Success/failure of insert.</returns>
        public String IUTrackingNumber(OrderTrackingScanEntity OTSEntity, string OrderNumber, string Shipper, string What)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "IUTrackingNumber";
            cmd.Parameters.Add(this.DAL.GetParamenter("@TrackingNumber", OTSEntity.TrackingNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", OrderNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@PatientID", OTSEntity.PatientID));
            cmd.Parameters.Add(this.DAL.GetParamenter("@What", What));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Shipper", Shipper));
            cmd.Parameters.Add(this.DAL.GetParamenter("@WrongPatient", "", ParameterDirection.Output));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));


            InsertData(cmd);

            var wp = cmd.Parameters["@WrongPatient"] as IDataParameter;
            var i = cmd.Parameters["@Success"] as IDataParameter;
            string WrongPatient = wp.Value.ToString();
            string success = i.Value.ToString();

            if (success == "1")
            {
                return success;
            }
            if (WrongPatient == "WP")
            {
                return WrongPatient;
            }
            return success;

        }

        /// <summary>
        /// Adds Updates Deletes Order Tracking in the database.
        /// </summary>
        /// <param name="TrackingNumber">Tracking Number.</param>
        /// <param name="OrderNumber">Order.</param>
        /// <param name="PatientID">Patient.</param>
        /// <param name="What">Action Item.</param>
        /// <returns>Success/failure of insert.</returns>
        public String ITrackingNumberManual(OrderTrackingScanEntity OTSEntity, string OrderNumber, string Shipper)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertTrackingNumbersMultipleOrders";
            cmd.Parameters.Add(this.DAL.GetParamenter("@TrackingNumber", OTSEntity.TrackingNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", OrderNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Shipper", Shipper));
            cmd.Parameters.Add(this.DAL.GetParamenter("@NotShipTOPatient", "", ParameterDirection.Output));
            cmd.Parameters.Add(this.DAL.GetParamenter("@WrongPatient", "", ParameterDirection.Output));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));


            InsertData(cmd);

            var wp = cmd.Parameters["@WrongPatient"] as IDataParameter;
            var sp = cmd.Parameters["@NotShipTOPatient"] as IDataParameter;
            var i = cmd.Parameters["@Success"] as IDataParameter;
            string WrongPatient = wp.Value.ToString();
            string nShip2Patient = sp.Value.ToString();
            string success = i.Value.ToString();

            if (success == "1")
            {
                return success;
            }
            if (WrongPatient.Length > 27)
            {
                return WrongPatient;
            }
            if (nShip2Patient.Length > 27)
            {
                return nShip2Patient;
            }
            return success;

        }

        /// <summary>
        /// Fill Tracking Scan List
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected override OrderTrackingScanEntity FillRecord(IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            var OTSEntity = new OrderTrackingScanEntity();

            OTSEntity.TrackingNumber = dr.AsString("TrackingNumber", c);
            OTSEntity.PatientID = dr.ToInt32("PatientID", c);
            OTSEntity.Order1 = dr.AsString("Order1", c);
            OTSEntity.Order2 = dr.AsString("Order2", c);
            OTSEntity.Order3 = dr.AsString("Order3", c);
            OTSEntity.Order4 = dr.AsString("Order4", c);

            return OTSEntity;
        }

    }

    public sealed class OrderTrackingPatientRepository : RepositoryBase<OrderTrackingPatientEntity>, IOrderTrackingPatientRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public OrderTrackingPatientRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        /// <summary>
        /// Gets second table for adding the tracking information.
        /// </summary>
        /// <returns>OrderTrackingPatientEntity list of all patient tracked orders in 48hrs.</returns>
        public List<OrderTrackingPatientEntity> GetTrackingNumbersPatientSite(int PatientID, string SiteCode)
        {
            var Command = this.DAL.GetCommandObject();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = "GetTrackingNumbersPatientSite";

            Command.Parameters.Add(this.DAL.GetParamenter("@PatientID", PatientID));
            Command.Parameters.Add(this.DAL.GetParamenter("@SiteCode", SiteCode));
            Command.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));
            return GetRecords(Command).ToList();
        }

        /// <summary>
        /// Fill Tracking Order Patient List
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected override OrderTrackingPatientEntity FillRecord(IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            var OTPEntity = new OrderTrackingPatientEntity();

            OTPEntity.TrackingNumber = dr.AsString("TrackingNumber", c);
            OTPEntity.OrderNumber = dr.AsString("OrderNumber", c);
            OTPEntity.OrderDate = dr.ToDateTime("OrderDate", c);
            OTPEntity.FrameCode = dr.AsString("FrameCode", c);
            OTPEntity.LensType = dr.AsString("LensType", c);

            return OTPEntity;
        }

    }

    public sealed class OrderTrackingOrderRepository : RepositoryBase<OrderTrackingOrderEntity>, IOrderTrackingOrderRepository
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public OrderTrackingOrderRepository()
            : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
        { }

        /// <summary>
        /// get PatientID from 771 Scan.
        /// </summary>
        /// <param name="OrderNumber">OrderNumber to get PatientID.</param>
        /// <returns>Success/failure of insert.</returns>
        public List<OrderTrackingOrderEntity> GetPatientID(string OrderNumber)
        {
            var cmd = this.DAL.GetCommandObject();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetPatientID";
            cmd.Parameters.Add(this.DAL.GetParamenter("@OrderNumber", OrderNumber));
            cmd.Parameters.Add(this.DAL.GetParamenter("@Success", "", ParameterDirection.Output));

            return GetRecords(cmd).ToList();
        }

        /// <summary>
        /// Fill Tracking Order List
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected override OrderTrackingOrderEntity FillRecord(IDataReader dr)
        {
            var c = dr.GetColumnNameList();
            var OTOEntity = new OrderTrackingOrderEntity();

            OTOEntity.PatientID = dr.ToInt32("PatientID", c);

            return OTOEntity;
        }

    }


}
