using BarcodeLib;
using SrtsWeb.Views.Reporting;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Globalization;

namespace SrtsWeb.BusinessLayer.Presenters.Reporting
{
    public sealed class RptDDForm771Presenter
    {
        //private IOrderRepository orderRepository;
        private IOrder771View _view;
        public Order711Entity entity;

        public RptDDForm771Presenter(IOrder771View view)
        {
            _view = view;
        }

        public void InitView(string orderNumbs)
        {
            GetOrder771(orderNumbs);
        }

        public void GetOrder771(string orderNumbs)
        {
            DataSet ds = new DataSet();
            var orderRepository = new OrderRepository();
            DataTable dt = orderRepository.Get711DataByLabCode(_view.mySession.MySite.SiteCode, orderNumbs, _view.mySession.MyUserID.ToString());
            ImageConverter converter = new ImageConverter();

            if (!(dt == null || dt.Rows.Count <= 0))
            {
                dt.TableName = "OrderData";

                foreach (DataRow dr in dt.Rows)
                {
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                    {
                        Image tImage = Barcode.DoEncode(TYPE.CODE39Extended, ("*" + dr["OrderNumber"] + "*".ToString()), false, Color.Black, Color.White, 290, 50);
                        tImage.Save(stream, ImageFormat.Png);
                        dr["ONBarCode"] = stream.ToArray();
                    }
                }

                ds.Tables.Add(dt);
                _view.Order771 = ds;
            }
        }

        public void Build771Report(DataTable DD771Table)
        {
            List<DD771Entity> DD771EntityList = new List<DD771Entity>();
            DD771Entity DD771Data = new DD771Entity();

            foreach (DataRow dr in DD771Table.Rows)
            {
                DD771Data.ONBarCode = System.Text.Encoding.UTF8.GetBytes(dr["ONBarCode"].ToString());

                DD771Data.DateOrderCreated = DateTime.ParseExact(dr["dateordercreated"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DD771Data.ClinicSiteCode = dr["ClinicSiteCode"].ToString();
                DD771Data.OrderNumber = dr["OrderNumber"].ToString();

                DD771Data.LabSiteCode = dr["LabSiteCode"].ToString();
                DD771Data.LabName = dr["labname"].ToString();
                DD771Data.LabAddress1 = dr["labaddress1"].ToString();
                DD771Data.LabAddress2 = dr["labaddress2"].ToString();
                DD771Data.LabCity = dr["labcity"].ToString();
                DD771Data.LabCountry = dr["labcountry"].ToString();
                DD771Data.LabState = dr["labstate"].ToString();
                DD771Data.LabZipCode = dr["labzipcode"].ToString();

                DD771Data.ClinicName = dr["clinicname"].ToString();
                DD771Data.ClinicAddress1 = dr["clinicaddress1"].ToString();
                DD771Data.ClinicAddress2 = dr["clinicaddress2"].ToString();
                DD771Data.ClinicCity = dr["cliniccity"].ToString();
                DD771Data.ClinicCountry = dr["cliniccountry"].ToString();
                DD771Data.ClinicState = dr["clinicstate"].ToString();
                DD771Data.ClinicZipCode = dr["cliniczipcode"].ToString();

                DD771Data.FirstName = dr["FirstName"].ToString();
                DD771Data.MiddleName = dr["MiddleName"].ToString();
                DD771Data.LastName = dr["LastName"].ToString();
                DD771Data.PatientIDNumber = dr["patientidnumber"].ToString();
                DD771Data.RankCode = dr["rankcode"].ToString();

                DD771Data.ShipAddress1 = dr["ShipAddress1"].ToString();
                DD771Data.ShipAddress2 = dr["ShipAddress2"].ToString();
                DD771Data.ShipCity = dr["ShipCity"].ToString();
                DD771Data.ShipState = dr["ShipState"].ToString();
                DD771Data.ShipZipCode = dr["ShipZipCode"].ToString();
                DD771Data.ShipToPatient = bool.Parse(dr["ShipToPatient"].ToString());

                DD771Data.PatientPhoneNumber = dr["patientphonenumber"].ToString();
                DD771Data.PatientEmail = dr["PatientEmail"].ToString();

                DD771Data.StatusCode = dr["statuscode"].ToString();
                DD771Data.BOS = dr["bos"].ToString();

                DD771Data.FrameCode = dr["FrameCode"].ToString();
                DD771Data.FrameEyeSize = dr["FrameEyeSize"].ToString();
                DD771Data.FrameBridgeSize = dr["FrameBridgeSize"].ToString();
                DD771Data.FrameTempleType = dr["FrameTempleType"].ToString();
                DD771Data.FrameColor = dr["FrameColor"].ToString();

                DD771Data.PDDistant = decimal.Parse(dr["PDDistant"].ToString());
                DD771Data.PDNear = decimal.Parse(dr["PDNear"].ToString());
                DD771Data.LensType = dr["LensType"].ToString();
                DD771Data.Tint = dr["Tint"].ToString();
                DD771Data.LensMaterial = dr["LensMaterial"].ToString();
                DD771Data.Pairs = int.Parse(dr["Pairs"].ToString());
                DD771Data.NumberOfCases = int.Parse(dr["NumberOfCases"].ToString());


                DD771Data.ODSphere = dr["ODSphere"].ToString();
                DD771Data.ODCylinder = dr["ODCylinder"].ToString();
                DD771Data.ODAxis = int.Parse(dr["ODAxis"].ToString());
                DD771Data.ODDistantDecenter = decimal.Parse(dr["oddistantdecenter"].ToString());
                DD771Data.ODNearDecenter = decimal.Parse(dr["odneardecenter"].ToString());
                DD771Data.ODHPrism = decimal.Parse(dr["ODHPrism"].ToString());
                DD771Data.ODHBase = dr["ODHBase"].ToString();
                DD771Data.ODVPrism = decimal.Parse(dr["ODVPrism"].ToString());
                DD771Data.ODVBase = dr["ODVBase"].ToString();

                DD771Data.OSSphere = dr["OSSphere"].ToString();
                DD771Data.OSCylinder = dr["OSCylinder"].ToString();
                DD771Data.OSAxis = int.Parse(dr["OSAxis"].ToString());
                DD771Data.OSDistantDecenter = decimal.Parse(dr["osdistantdecenter"].ToString());
                DD771Data.OSNearDecenter = decimal.Parse(dr["osneardecenter"].ToString());
                DD771Data.OSHPrism = decimal.Parse(dr["OSHPrism"].ToString());
                DD771Data.OSHBase = dr["OSHBase"].ToString();
                DD771Data.OSVPrism = decimal.Parse(dr["OSVPrism"].ToString());
                DD771Data.OSVBase = dr["OSVBase"].ToString();

                DD771Data.ODAdd = decimal.Parse(dr["ODAdd"].ToString());
                DD771Data.ODSegHeight = decimal.Parse(dr["OSAdd"].ToString());


                DD771Data.OSAdd = decimal.Parse(dr["ODSegHeight"].ToString());
                DD771Data.OSSegHeight = decimal.Parse(dr["OSSegHeight"].ToString());

                DD771Data.ODBase = dr["odbase"].ToString();
                DD771Data.OSBase = dr["osbase"].ToString();

                DD771Data.OrderPriority = dr["orderpriority"].ToString();
                DD771Data.TechInitials = dr["techinitials"].ToString();

                DD771Data.UserComment1 = dr["UserComment1"].ToString();
                DD771Data.UserComment2 = dr["UserComment2"].ToString();

                DD771Data.Provider = dr["doctor"].ToString();

                DD771Data.IsMultivision = bool.Parse(dr["IsMultivision"].ToString());


                DD771EntityList.Add(DD771Data);
            }
            BuildReportPDF(DD771EntityList);
        }

        public void BuildReportPDF(List<DD771Entity> DD771EntityList)
        {
        }
    }
}