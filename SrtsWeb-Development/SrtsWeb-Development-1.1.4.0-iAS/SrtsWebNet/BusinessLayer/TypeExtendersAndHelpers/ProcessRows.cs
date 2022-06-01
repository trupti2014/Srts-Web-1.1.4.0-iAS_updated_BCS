using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers
{
    public sealed class SrtsHelperObject
    {
        public SrtsHelperObject()
        {
        }

        public List<NostraFileEntity> ProcessNostraFileDataTable(DataTable dt)
        {
            var lne = new List<NostraFileEntity>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var ne = new NostraFileEntity();
                    ne.OrderNumber = dr.GetStringVal("OrderNumber");
                    ne.DocumentNumber = dr.GetStringVal("DocumentNumber");
                    ne.IndividualID_Patient = dr.GetIntVal("IndividualID_Patient");
                    ne.Demographic = dr.GetStringVal("Demographic");

                    ne.LensType = dr.GetStringVal("LensType");
                    ne.LensMaterial = dr.GetStringVal("LensMaterial");
                    ne.Tint = dr.GetStringVal("Tint");
                    ne.ODSegHeight = dr.GetNullableDecimalVal("ODSegHeight");
                    ne.OSSegHeight = dr.GetNullableDecimalVal("OSSegHeight");

                    ne.FrameCode = dr.GetStringVal("FrameCode");
                    ne.FrameColor = dr.GetStringVal("FrameColor");
                    ne.FrameEyeSize = dr.GetStringVal("FrameEyeSize");
                    ne.FrameBridgeSize = dr.GetStringVal("FrameBridgeSize");

                    ne.FrameTempleType = dr.GetStringVal("FrameTempleType");
                    ne.ODPDDistant = dr.GetNullableIntVal("ODPDDistant");
                    ne.OSPDDistant = dr.GetNullableIntVal("OSPDDistant");
                    ne.PDDistant = dr.GetNullableIntVal("PDDistant");

                    ne.PDNear = dr.GetNullableIntVal("PDNear");
                    ne.ClinicSiteCode = dr.GetStringVal("ClinicSiteCode");
                    ne.ClinicAddress1 = dr.GetStringVal("ClinicAddress1");
                    ne.ClinicAddress2 = dr.GetStringVal("ClinicAddress2");
                    ne.ClinicCity = dr.GetStringVal("ClinicCity");
                    ne.ClinicState = dr.GetStringVal("ClinicState");
                    ne.ClinicCountry = dr.GetStringVal("ClinicCountry");
                    ne.ClinicZipCode = dr.GetStringVal("ClinicZipCode");
                    ne.LabSiteCode = dr.GetStringVal("LabSiteCode");
                    ne.ShipToPatient = dr.GetBoolVal("ShipToPatient");
                    ne.ShipAddress1 = dr.GetStringVal("ShipAddress1");
                    ne.ShipAddress2 = dr.GetStringVal("ShipAddress2");
                    ne.ShipCity = dr.GetStringVal("ShipCity");
                    ne.ShipState = dr.GetStringVal("ShipState");
                    ne.ShipZipCode = dr.GetStringVal("ShipZipCode");
                    ne.ShipAddressType = dr.GetStringVal("ShipAddressType");
                    ne.LocationCode = dr.GetStringVal("LocationCode");
                    ne.UserComment1 = dr.GetStringVal("UserComment1");
                    ne.UserComment2 = dr.GetStringVal("UserComment2");
                    ne.UserComment3 = dr.GetStringVal("UserComment3");
                    ne.UserComment4 = dr.GetStringVal("UserComment4");
                    ne.UserComment5 = dr.GetStringVal("UserComment5");
                    ne.UserComment6 = dr.GetStringVal("UserComment6");
                    ne.IsGEyes = dr.GetBoolVal("IsGEyes");

                    ne.CorrespondenceEmail = dr.GetStringVal("PatientEmail");

                    ne.ODSphere = dr.GetStringVal("ODSphere");
                    ne.OSSphere = dr.GetStringVal("OSSphere");
                    ne.ODCylinder = dr.GetStringVal("ODCylinder");
                    ne.OSCylinder = dr.GetStringVal("OSCylinder");
                    ne.ODAxis = dr.GetNullableIntVal("ODAxis");
                    ne.OSAxis = dr.GetNullableIntVal("OSAxis");
                    ne.ODHPrism = dr.GetNullableDecimalVal("ODHPrism");
                    ne.OSHPrism = dr.GetNullableDecimalVal("OSHPrism");
                    ne.ODVPrism = dr.GetNullableDecimalVal("ODVPrism");
                    ne.OSVPrism = dr.GetNullableDecimalVal("OSVPrism");
                    ne.ODHBase = dr.GetStringVal("ODHBase");
                    ne.OSHBase = dr.GetStringVal("OSHBase");
                    ne.ODVBase = dr.GetStringVal("ODVBase");
                    ne.OSVBase = dr.GetStringVal("OSVBase");
                    ne.ODAdd = dr.GetNullableDecimalVal("ODAdd");
                    ne.OSAdd = dr.GetNullableDecimalVal("OSAdd");

                    ne.FirstName = dr.GetStringVal("FirstName");
                    ne.MiddleName = dr.GetStringVal("MiddleName");
                    ne.LastName = dr.GetStringVal("LastName");
                    ne.Unit = dr.GetStringVal("Unit");
                    lne.Add(ne);
                }
            }

            return lne;
        }
    }

    public static partial class SrtsHelper
    {
        //public static List<ReleaseNote> ProcessReleaseNotes(DataTable dt)
        //{
        //    var rn = new List<ReleaseNote>();

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        var n = new ReleaseNote()
        //        {
        //            ReleaseNotes = dr.GetStringVal("ChangesMade"),
        //            VersionDate = dr.GetDateTimeVal("VersionDate"),
        //            VersionNumber = dr.GetStringVal("VersionNbr")
        //        };

        //        rn.Add(n);
        //    }

        //    return rn;
        //}

        public static string GetLookupValue(List<LookupTableEntity> lookups, string code, string value)
        {
            var ans = (from d in lookups
                       where d.Code == code
                       select d).FirstOrDefault(d => d.Value == value);
            return ans.Text.ToString();
        }

        //public static List<CmsMessage> ProcessCmsMessageTable(DataTable dt)
        //{
        //    var l = new List<CmsMessage>();
        //    foreach (DataRow dr in dt.Rows)
        //        l.Add(ProcessCmsMessageTable(dr));
        //    return l;
        //}
        //public static CmsMessage ProcessCmsMessageTable(DataRow dr)
        //{
        //    if (dr == null) return default(CmsMessage);
        //    return new CmsMessage()
        //    {
        //        cmsContentId = Convert.ToInt32(dr["cmsContentID"]).ToTypeDefault(),
        //        cmsContentTitle = dr["cmsContentTitle"].ToString().ToTypeDefault(),
        //        cmsContentBody = dr["cmsContentBody"].ToString().ToTypeDefault()
        //    };
        //}

        //public static List<CMSEntity> ProcessCMSEntityTable(DataTable dt)
        //{
        //    var l = new List<CMSEntity>();
        //    foreach (DataRow dr in dt.Rows)
        //        l.Add(ProcessCMSEntityTable(dr));
        //    return l;
        //}
        //public static CMSEntity ProcessCMSEntityTable(DataRow dr)
        //{
        //    if (dr == null) return default(CMSEntity);
        //    return new CMSEntity()
        //    {
        //        AuthorFirstName = dr["AuthorFirstName"].ToString().ToTypeDefault(),
        //        AuthorLastName = dr["AuthorLastName"].ToString().ToTypeDefault(),
        //        AuthorMiddleName = dr["AuthorMiddleName"].ToString().ToTypeDefault(),
        //        cmsContentAuthorID = Convert.ToInt32(dr["cmsContentAuthorID"]).ToTypeDefault(),
        //        cmsContentBody = dr["cmsContentBody"].ToString().ToTypeDefault(),
        //        cmsContentDescription = dr["cmsContentDescription"].ToString().ToTypeDefault(),
        //        cmsContentDisplayDate = Convert.ToDateTime(dr["cmsContentDisplayDate"]).ToTypeDefault(),
        //        cmsContentExpireDate = Convert.ToDateTime(dr["cmsContentExpireDate"]).ToTypeDefault(),
        //        cmsCreatedDate = Convert.ToDateTime(dr["cmsCreatedDate"]).ToTypeDefault(),
        //        cmsContentID = Convert.ToInt32(dr["cmsContentID"]).ToTypeDefault(),
        //        cmsContentRecipientGroupID = dr["cmsRecipientGroupID"].ToString().ToTypeDefault(),
        //        cmsContentRecipientIndividualID = Convert.ToInt32(dr["cmsContentRecipientIndividualID"]).ToTypeDefault(),
        //        cmsContentRecipientSiteID = dr["cmsContentRecipientSiteID"].ToString().ToTypeDefault(),
        //        cmsContentRecipientTypeID = dr["cmsRecipientTypeID"].ToString().ToTypeDefault(),
        //        cmsContentTitle = dr["cmsContentTitle"].ToString().ToTypeDefault(),
        //        cmsContentTypeID = dr["cmsContentTypeID"].ToString().ToTypeDefault(),
        //        cmsContentTypeName = dr["cmsContentTypeName"].ToString().ToTypeDefault(),
        //        cmsRecipientGroupDescription = dr["cmsRecipientGroupDescription"].ToString().ToTypeDefault(),
        //        cmsRecipientGroupname = dr["cmsRecipientGroupName"].ToString().ToTypeDefault(),
        //        cmsRecipientTypeDescription = dr["cmsRecipientTypeDescription"].ToString().ToTypeDefault(),
        //        cmsRecipientTypeName = dr["cmsRecipientTypeName"].ToString().ToTypeDefault(),
        //        RecipientFristName = dr["RecipientFirstName"].ToString().ToTypeDefault(),
        //        RecipientLastName = dr["RecipientLastName"].ToString().ToTypeDefault(),
        //        RecipientMiddleName = dr["RecipientMiddleName"].ToString().ToTypeDefault()
        //    };
        //}

        public static List<Order711Entity> ProcessLMSTable(DataTable dt)
        {
            List<Order711Entity> line = new List<Order711Entity>();
            foreach (DataRow dr in dt.Rows)
            {
                line.Add(ProcessLMSRow(dr));
            }
            return line;
        }
        public static Order711Entity ProcessLMSRow(DataRow dr)
        {
            Order711Entity ote = new Order711Entity();
            ote = new Order711Entity();
            ote.OrderNumber = dr.GetStringVal("OrderNumber");
            ote.BOS = dr.GetStringVal("bos");
            ote.RankCode = dr.GetStringVal("rankcode");
            ote.SexCode = dr.GetStringVal("sexcode");
            ote.PatientPhoneNumber = dr.GetStringVal("patientphonenumber");
            ote.TechInitials = dr.GetStringVal("techinitials");
            ote.PatientIDNumber = dr.GetStringVal("patientidnumber");
            ote.PatientEmail = dr.GetStringVal("patientemail");
            ote.StatusCode = dr.GetStringVal("statuscode");
            ote.ClinicAddress1 = dr.GetStringVal("clinicaddress1");
            ote.ClinicAddress2 = dr.GetStringVal("clinicaddress2");
            ote.ClinicCity = dr.GetStringVal("cliniccity");
            ote.ClinicCountry = dr.GetStringVal("cliniccountry");
            ote.ClinicName = dr.GetStringVal("clinicname");
            ote.ClinicState = dr.GetStringVal("clinicstate");
            ote.ClinicZipCode = dr.GetStringVal("cliniczipcode");
            ote.DateOrderCreated = dr.GetDateTimeVal("dateordercreated");
            ote.Doctor = dr.GetStringVal("doctor");
            ote.ExamDate = dr.GetNullableDateTimeVal("examdate");
            ote.LabAddress1 = dr.GetStringVal("labaddress1");
            ote.LabAddress2 = dr.GetStringVal("labaddress2");
            ote.LabCity = dr.GetStringVal("labcity");
            ote.LabCountry = dr.GetStringVal("labcountry");
            ote.LabName = dr.GetStringVal("labname");
            ote.LabState = dr.GetStringVal("labstate");
            ote.LabZipCode = dr.GetStringVal("labzipcode");
            ote.OrderPriority = dr.GetStringVal("orderpriority");
            ote.LensType = dr.GetStringVal("LensType");
            ote.LensMaterial = dr.GetStringVal("LensMaterial");
            ote.Tint = dr.GetStringVal("Tint");
            ote.ODSegHeight = dr.GetDecimalVal("ODSegHeight");
            ote.OSSegHeight = dr.GetDecimalVal("OSSegHeight");
            ote.NumberOfCases = dr.GetIntVal("NumberOfCases");
            ote.Pairs = dr.GetIntVal("Pairs");
            ote.FrameCode = dr.GetStringVal("FrameCode");
            ote.FrameColor = dr.GetStringVal("FrameColor");
            ote.FrameEyeSize = dr.GetStringVal("FrameEyeSize");
            ote.FrameBridgeSize = dr.GetStringVal("FrameBridgeSize");
            ote.FrameTempleType = dr.GetStringVal("FrameTempleType");
            ote.ODPDDistant = dr.GetDecimalVal("ODPDDistant");
            ote.OSPDDistant = dr.GetDecimalVal("OSPDDistant");
            ote.PDDistant = dr.GetDecimalVal("PDDistant");
            ote.ODPDNear = dr.GetDecimalVal("ODPDNear");
            ote.OSPDNear = dr.GetDecimalVal("OSPDNear");
            ote.PDNear = dr.GetDecimalVal("PDNear");
            ote.ClinicSiteCode = dr.GetStringVal("ClinicSiteCode");
            ote.LabSiteCode = dr.GetStringVal("LabSiteCode");
            ote.ShipToPatient = dr.GetBoolVal("ShipToPatient");
            ote.ShipAddress1 = dr.GetStringVal("ShipAddress1");
            ote.ShipAddress2 = dr.GetStringVal("ShipAddress2");
            ote.ShipCity = dr.GetStringVal("ShipCity");
            ote.ShipState = dr.GetStringVal("ShipState");
            ote.ShipZipCode = dr.GetStringVal("ShipZipCode");
            ote.ShipAddressType = dr.GetStringVal("ShipAddressType");
            ote.LocationCode = dr.GetStringVal("LocationCode");
            ote.UserComment1 = dr.GetStringVal("UserComment1");
            ote.UserComment2 = dr.GetStringVal("UserComment2");
            ote.IsGEyes = dr.GetBoolVal("IsGEyes");
            ote.IsMultivision = dr.GetBoolVal("IsMultivision");
            ote.OnholdForConfirmation = dr.GetBoolVal("OnholdForConfirmation");
            ote.ODSphere = dr.GetStringVal("ODSphere");
            ote.OSSphere = dr.GetStringVal("OSSphere");
            ote.ODCylinder = dr.GetStringVal("ODCylinder");
            ote.OSCylinder = dr.GetStringVal("OSCylinder");
            ote.ODAxis = dr.GetIntVal("ODAxis");
            ote.OSAxis = dr.GetIntVal("OSAxis");
            ote.ODHPrism = dr.GetDecimalVal("ODHPrism");
            ote.OSHPrism = dr.GetDecimalVal("OSHPrism");
            ote.ODVPrism = dr.GetDecimalVal("ODVPrism");
            ote.OSVPrism = dr.GetDecimalVal("OSVPrism");
            ote.ODHBase = dr.GetStringVal("ODHBase");
            ote.OSHBase = dr.GetStringVal("OSHBase");
            ote.ODVBase = dr.GetStringVal("ODVBase");
            ote.OSVBase = dr.GetStringVal("OSVBase");
            ote.ODAdd = dr.GetDecimalVal("ODAdd");
            ote.OSAdd = dr.GetDecimalVal("OSAdd");
            ote.ODCorrectedAcuity = dr.GetStringVal("ODCorrectedAcuity");
            ote.ODUncorrectedAcuity = dr.GetStringVal("ODUncorrectedAcuity");
            ote.OSCorrectedAcuity = dr.GetStringVal("OSCorrectedAcuity");
            ote.OSUncorrectedAcuity = dr.GetStringVal("OSUncorrectedAcuity");
            ote.ODOSCorrectedAcuity = dr.GetStringVal("ODOSCorrectedAcuity");
            ote.ODOSUncorrectedAcuity = dr.GetStringVal("ODOSUncorrectedAcuity");
            ote.FirstName = dr.GetStringVal("FirstName");
            ote.MiddleName = dr.GetStringVal("MiddleName");
            ote.LastName = dr.GetStringVal("LastName");
            return ote;
        }

        //public static List<OrderTransferEntity> ProcessTransferTable(DataTable dt)
        //{
        //    List<OrderTransferEntity> lote = new List<OrderTransferEntity>();
        //    OrderTransferEntity ote = new OrderTransferEntity();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        ote = new OrderTransferEntity();
        //        ote.OrderNumber = dr.GetStringVal("OrderNumber");
        //        ote.IndividualID_Tech = dr.GetIntVal("IndividualID_Tech");

        //        ote.Demographic = dr.GetStringVal("Demographic");
        //        ote.TechInitials = dr.GetStringVal("techinitials");
        //        ote.LensType = dr.GetStringVal("LensType");
        //        ote.LensMaterial = dr.GetStringVal("LensMaterial");
        //        ote.Tint = dr.GetStringVal("Tint");
        //        ote.ODSegHeight = dr.GetNullableDecimalVal("ODSegHeight");
        //        ote.OSSegHeight = dr.GetNullableDecimalVal("OSSegHeight");
        //        ote.NumberOfCases = dr.GetIntVal("NumberOfCases");
        //        ote.Pairs = dr.GetIntVal("Pairs");
        //        ote.FrameCode = dr.GetStringVal("FrameCode");
        //        ote.FrameColor = dr.GetStringVal("FrameColor");
        //        ote.FrameEyeSize = dr.GetStringVal("FrameEyeSize");
        //        ote.FrameBridgeSize = dr.GetStringVal("FrameBridgeSize");
        //        ote.FrameTempleType = dr.GetStringVal("FrameTempleType");
        //        ote.ODPDDistant = dr.GetNullableIntVal("ODPDDistant");
        //        ote.OSPDDistant = dr.GetNullableIntVal("OSPDDistant");
        //        ote.PDDistant = dr.GetNullableIntVal("PDDistant");
        //        ote.ODPDNear = dr.GetNullableIntVal("ODPDNear");
        //        ote.OSPDNear = dr.GetNullableIntVal("OSPDNear");
        //        ote.PDNear = dr.GetNullableIntVal("PDNear");
        //        ote.ClinicSiteCode = dr.GetStringVal("ClinicSiteCode");
        //        ote.LabSiteCode = dr.GetStringVal("LabSiteCode");
        //        ote.ShipToPatient = dr.GetBoolVal("ShipToPatient");
        //        ote.ShipAddress1 = dr.GetStringVal("ShipAddress1");
        //        ote.ShipAddress2 = dr.GetStringVal("ShipAddress2");
        //        ote.ShipCity = dr.GetStringVal("ShipCity");
        //        ote.ShipState = dr.GetStringVal("ShipState");
        //        ote.ShipZipCode = dr.GetStringVal("ShipZipCode");
        //        ote.ShipAddressType = dr.GetStringVal("ShipAddressType");
        //        ote.LocationCode = dr.GetStringVal("LocationCode");
        //        ote.UserComment1 = dr.GetStringVal("UserComment1");
        //        ote.UserComment2 = dr.GetStringVal("UserComment2");
        //        ote.IsGEyes = dr.GetBoolVal("IsGEyes");
        //        ote.IsMultivision = dr.GetBoolVal("IsMultivision");
        //        ote.VerifiedBy = dr.GetIntVal("VerifiedBy");

        //        ote.CorrespondenceEmail = dr.GetStringVal("CorrespondenceEmail");
        //        ote.OnholdForConfirmation = dr.GetBoolVal("OnholdForConfirmation");
        //        ote.ODSphere = dr.GetStringVal("ODSphere");
        //        ote.OSSphere = dr.GetStringVal("OSSphere");
        //        ote.ODCylinder = dr.GetStringVal("ODCylinder");
        //        ote.OSCylinder = dr.GetStringVal("OSCylinder");
        //        ote.ODAxis = dr.GetNullableIntVal("ODAxis");
        //        ote.OSAxis = dr.GetNullableIntVal("OSAxis");
        //        ote.ODHPrism = dr.GetNullableDecimalVal("ODHPrism");
        //        ote.OSHPrism = dr.GetNullableDecimalVal("OSHPrism");
        //        ote.ODVPrism = dr.GetNullableDecimalVal("ODVPrism");
        //        ote.OSVPrism = dr.GetNullableDecimalVal("OSVPrism");
        //        ote.ODHBase = dr.GetStringVal("ODHBase");
        //        ote.OSHBase = dr.GetStringVal("OSHBase");
        //        ote.ODVBase = dr.GetStringVal("ODVBase");
        //        ote.OSVBase = dr.GetStringVal("OSVBase");
        //        ote.ODAdd = dr.GetNullableDecimalVal("ODAdd");
        //        ote.OSAdd = dr.GetNullableDecimalVal("OSAdd");
        //        ote.ODCorrectedAcuity = dr.GetStringVal("ODCorrectedAcuity");
        //        ote.ODUncorrectedAcuity = dr.GetStringVal("ODUncorrectedAcuity");
        //        ote.OSCorrectedAcuity = dr.GetStringVal("OSCorrectedAcuity");
        //        ote.OSUncorrectedAcuity = dr.GetStringVal("OSUncorrectedAcuity");
        //        ote.ODOSCorrectedAcuity = dr.GetStringVal("ODOSCorrectedAcuity");
        //        ote.ODOSUncorrectedAcuity = dr.GetStringVal("ODOSUncorrectedAcuity");
        //        ote.FirstName = dr.GetStringVal("FirstName");
        //        ote.MiddleName = dr.GetStringVal("MiddleName");
        //        ote.LastName = dr.GetStringVal("LastName");
        //        lote.Add(ote);
        //    }
        //    return lote;
        //}

        public static List<IdentificationNumbersEntity> ProcessIdentificationNumberTable(DataTable dt)
        {
            List<IdentificationNumbersEntity> line = new List<IdentificationNumbersEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                line.Add(ProcessIdentificationNumberRows(dr));
            }
            return line;
        }
        public static IdentificationNumbersEntity ProcessIdentificationNumberRows(DataRow dr)
        {
            IdentificationNumbersEntity ine = new IdentificationNumbersEntity();
            ine.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            ine.ID = dr.GetIntVal("ID");
            ine.IndividualID = dr.GetIntVal("IndividualID");
            ine.IsActive = dr.GetBoolVal("IsActive");
            ine.ModifiedBy = dr.GetStringVal("ModifiedBy");
            ine.IDNumber = dr.GetStringVal("IDNumber");
            ine.IDNumberType = dr.GetStringVal("IDNumberType");
            ine.IDNumberTypeDescription = GetIDTypeDescription(ine.IDNumberType);
            return ine;
        }

        //public static List<OrderStateEntity> ProcessOrderStateTable(DataTable dt)
        //{
        //    List<OrderStateEntity> lose = new List<OrderStateEntity>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        lose.Add(ProcessOrderStateRows(dr));
        //    }
        //    return lose;
        //}
        //public static OrderStateEntity ProcessOrderStateRows(DataRow dr)
        //{
        //    OrderStateEntity ose = new OrderStateEntity();
        //    ose.ID = dr.GetIntVal("ID");
        //    ose.OrderNumber = dr.GetStringVal("OrderNumber");
        //    ose.OrderStatusTypeID = dr.GetIntVal("OrderStatusTypeID");
        //    ose.OrderStatusType = dr.GetStringVal("OrderStatusDescription");
        //    ose.StatusComment = dr.GetStringVal("StatusComment");
        //    ose.LabCode = dr.GetStringVal("LabSiteCode");
        //    ose.IsActive = dr.GetBoolVal("IsActive");

        //    ose.ModifiedBy = dr.GetStringVal("ModifiedBy");
        //    ose.DateLastModified = dr.GetDateTimeVal("DateLastModified");
        //    return ose;
        //}

        //public static List<string> ProcessAllSiteCodes(DataTable dt)
        //{
        //    List<string> ls = new List<string>();
        //    string tmp = string.Empty;
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        tmp = dr.GetStringVal("SiteCode");
        //        ls.Add(tmp);
        //    }
        //    return ls;
        //}

        //public static List<String> ProcessLabsbyIsMultivision(DataTable dt, Boolean isMulti)
        //{
        //    List<string> ls = new List<string>();
        //    string tmp = string.Empty;
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        if (dr.GetBoolVal("IsMultivision") != isMulti) continue;
        //        tmp = dr.GetStringVal("SiteCode");
        //        ls.Add(tmp);
        //    }
        //    return ls;
        //}

        //public static List<SiteCodeEntity> ProcessSiteTable(DataTable dt)
        //{
        //    List<SiteCodeEntity> sce = new List<SiteCodeEntity>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        sce.Add(ProcessSiteRows(dr));
        //    }
        //    return sce;
        //}
        //public static SiteCodeEntity ProcessSiteRows(DataRow dr)
        //{
        //    SiteCodeEntity se = new SiteCodeEntity();
        //    se.ID = dr.GetIntVal("ID");
        //    se.AddressType = dr.Table.Columns.Contains("AddressType") ? dr.GetStringVal("AddressType") : String.Empty;
        //    se.Address1 = dr.GetStringVal("Address1");
        //    se.Address2 = dr.GetStringVal("Address2");
        //    se.Address3 = dr.GetStringVal("Address3");
        //    se.City = dr.GetStringVal("City");
        //    se.Country = dr.GetStringVal("Country");
        //    se.State = dr.GetStringVal("State");
        //    se.ZipCode = dr.GetStringVal("ZipCode");
        //    se.DateLastModified = dr.GetDateTimeVal("DateLastModified");
        //    se.DSNPhoneNumber = dr.GetStringVal("DSNPhoneNumber");
        //    se.EMailAddress = dr.GetStringVal("EMailAddress");
        //    se.IsActive = dr.GetBoolVal("IsActive");
        //    se.IsConus = dr.GetBoolVal("IsConus");
        //    se.IsReimbursable = dr.GetBoolVal("IsReimbursable");
        //    se.ModifiedBy = dr.GetStringVal("ModifiedBy");
        //    se.RegPhoneNumber = dr.GetStringVal("RegPhoneNumber");
        //    se.SiteDescription = dr.GetStringVal("SiteDescription");
        //    se.SiteCode = dr.GetStringVal("SiteCode");
        //    se.SiteName = dr.GetStringVal("SiteName");
        //    se.SiteType = dr.GetStringVal("SiteType");
        //    se.BOS = dr.GetStringVal("BOS");
        //    se.BOSDescription = dr.GetStringVal("BOS").ToBOSValue();
        //    se.IsMultivision = dr.GetBoolVal("IsMultivision");
        //    se.IsAPOCompatible = dr.GetBoolVal("IsAPOCompatible");
        //    se.MaxEyeSize = dr.GetIntVal("MaxEyeSize");
        //    se.MaxFramesPerMonth = dr.GetIntVal("MaxFramesPerMonth");
        //    se.MaxPower = dr.GetDoubleVal("MaxPower");
        //    se.HasLMS = dr.GetBoolVal("HasLMS");
        //    se.ShipToPatientLab = dr.GetBoolVal("ShipToPatientLab");
        //    se.Region = dr.GetIntVal("Region");
        //    se.MultiPrimary = dr.GetStringVal("MultiPrimary");
        //    se.MultiSecondary = dr.GetStringVal("MultiSecondary");
        //    se.SinglePrimary = dr.GetStringVal("SinglePrimary");
        //    se.SingleSecondary = dr.GetStringVal("SingleSecondary");
        //    return se;
        //}

        public static List<OrderDisplayEntity> ProcessOrderDisplayTable(DataTable dt)
        {
            List<OrderDisplayEntity> lode = new List<OrderDisplayEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                lode.Add(ProcessOrderDisplayRow(dr));
            }
            return lode;
        }
        public static OrderDisplayEntity ProcessOrderDisplayRow(DataRow dr)
        {
            OrderDisplayEntity oe = new OrderDisplayEntity();
            oe = new OrderDisplayEntity();
            oe.FrameCode = dr.GetStringVal("FrameCode");
            oe.FrameColor = dr.GetStringVal("FrameColor");
            oe.LensType = dr.GetStringVal("LensType");
            oe.ODAdd = dr.GetDoubleVal("ODAdd");
            oe.ODAxis = dr.GetIntVal("ODAxis");
            oe.ODCylinder = dr.GetStringVal("ODCylinder");
            oe.ODSphere = dr.GetStringVal("ODSphere");
            oe.OSAdd = dr.GetDoubleVal("OSAdd");
            oe.OSAxis = dr.GetIntVal("OSAxis");
            oe.OSCylinder = dr.GetStringVal("OSCylinder");
            oe.OSSphere = dr.GetStringVal("OSSphere");
            oe.OrderNumber = dr.GetStringVal("OrderNumber");
            oe.ClinicSiteCode = dr.GetStringVal("ClinicSiteCode");
            oe.LabSiteCode = dr.GetStringVal("LabSiteCode");
            oe.OrderNumber = dr.GetStringVal("OrderNumber");
            oe.DateCreated = dr.GetDateTimeVal("DateCreated");
            oe.LensTypeLong = dr.GetStringVal("LensTypeLong");

            oe.LensTint = dr.GetStringVal("LensTint");

            oe.FrameDescription = dr.GetStringVal("FrameDescription");

            return oe;
        }

        public static List<OrderDisplayStatusEntity> ProcessOrderDisplayStatusTable(DataTable dt)
        {
            List<OrderDisplayStatusEntity> lode = new List<OrderDisplayStatusEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                lode.Add(ProcessOrderDisplayStatusRow(dr));
            }
            return lode;
        }
        public static OrderDisplayStatusEntity ProcessOrderDisplayStatusRow(DataRow dr)
        {
            OrderDisplayStatusEntity oe = new OrderDisplayStatusEntity();
            oe = new OrderDisplayStatusEntity();

            oe.LabSiteCode = dr.GetStringVal("LabSiteCode");
            oe.OrderNumber = dr.GetStringVal("OrderNumber");
            oe.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            oe.IsActive = dr.GetBoolVal("IsActive");
            oe.ModifiedBy = dr.GetStringVal("ModifiedBy");

            oe.OrderStatusTypeID = dr.GetIntVal("OrderStatusTypeID");
            oe.StatusComment = dr.GetStringVal("StatusComment");
            return oe;
        }

        //public static List<ManageOrderEntity> ProcessManageOrdersTable(DataTable dt)
        //{
        //    List<ManageOrderEntity> lmoe = new List<ManageOrderEntity>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        lmoe.Add(ProcessManageOrdersRow(dr));
        //    }
        //    return lmoe;
        //}
        //public static ManageOrderEntity ProcessManageOrdersRow(DataRow dr)
        //{
        //    var moe = new ManageOrderEntity()
        //    {
        //        ClinicSiteCode = dr.GetStringVal("ClinicSiteCode"),
        //        DateLastModified = dr.GetDateTimeVal("DateLastModified"),
        //        DateOrderCreated = dr.GetDateTimeVal("DateOrderCreated"),
        //        DateReceivedByLab = dr.GetDateTimeVal("DateReceivedByLab"),
        //        DaysPastDue = dr.GetIntVal("DaysPastDue"),
        //        FirstName = dr.GetStringVal("FirstName"),
        //        FrameCode = dr.GetStringVal("FrameCode"),
        //        IsActive = dr.GetBoolVal("IsActive"),
        //        LabSiteCode = dr.GetStringVal("LabSiteCode"),
        //        LastName = dr.GetStringVal("LastName"),
        //        LensMaterial = dr.GetStringVal("LensMaterial"),
        //        LensType = dr.GetStringVal("LensType"),
        //        MiddleName = dr.GetStringVal("MiddleName"),
        //        ModifiedBy = dr.GetStringVal("ModifiedBy"),
        //        OrderNumber = dr.GetStringVal("OrderNumber"),
        //        OrderStatusDescription = dr.GetStringVal("OrderStatusDescription"),
        //        OrderStatusTypeID = dr.GetIntVal("OrderStatusTypeID"),
        //        ShipAddress1 = dr.GetStringVal("ShipAddress1"),
        //        ShipAddress2 = dr.GetStringVal("ShipAddress2"),
        //        ShipAddress3 = dr.GetStringVal("ShipAddress3"),
        //        ShipCity = dr.GetStringVal("ShipCity"),
        //        ShipCountry = dr.GetStringVal("ShipCountry"),
        //        ShipState = dr.GetStringVal("ShipState"),
        //        ShipToPatient = dr.GetBoolVal("ShipToPatient"),
        //        ShipZipCode = dr.GetStringVal("ShipZipCode"),
        //        StatusComment = dr.GetStringVal("StatusComment")
        //    };
        //    if (moe.DateOrderCreated.Equals(default(DateTime)))
        //        moe.DateOrderCreated = dr.GetDateTimeVal("DateCreated");

        //    return moe;
        //}

        public static List<OrderEntity> ProcessOrderTable(DataTable dt)
        {
            List<OrderEntity> loe = new List<OrderEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                loe.Add(ProcessOrderRow(dr));
            }
            return loe;
        }
        public static OrderEntity ProcessOrderRow(DataRow dr)
        {
            OrderEntity oe = new OrderEntity();
            oe.ClinicSiteCode = dr.GetStringVal("ClinicSiteCode");
            oe.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            oe.PrescriptionID = dr.GetIntVal("PrescriptionID");
            oe.IndividualID_Patient = dr.GetIntVal("IndividualID_Patient");
            oe.IndividualID_Tech = dr.GetIntVal("IndividualID_Tech");
            oe.PatientPhoneID = dr.GetIntVal("PatientPhoneID");
            oe.LabSiteCode = dr.GetStringVal("LabSiteCode");
            oe.LensMaterial = dr.GetStringVal("LensMaterial");
            oe.LensType = dr.GetStringVal("LensType");
            oe.LocationCode = dr.GetStringVal("LocationCode");
            oe.ModifiedBy = dr.GetStringVal("ModifiedBy");
            oe.NumberOfCases = dr.GetIntVal("NumberOfCases");
            oe.Pairs = dr.GetIntVal("Pairs");
            oe.OrderNumber = dr.GetStringVal("OrderNumber");
            oe.ShipAddress1 = dr.GetStringVal("ShipAddress1");
            oe.ShipAddress2 = dr.GetStringVal("ShipAddress2");
            oe.ShipAddress3 = dr.GetStringVal("ShipAddress3");
            oe.ShipCity = dr.GetStringVal("ShipCity");
            oe.ShipState = dr.GetStringVal("ShipState");
            oe.ShipZipCode = dr.GetStringVal("ShipZipCode");
            oe.ShipAddressType = dr.GetStringVal("ShipAddressType");
            oe.ShipCountry = dr.GetStringVal("ShipCountry");
            oe.ShipToPatient = dr.GetBoolVal("ShipToPatient");
            oe.Tint = dr.GetStringVal("Tint");
            oe.UserComment1 = dr.GetStringVal("UserComment1");
            oe.UserComment2 = dr.GetStringVal("UserComment2");
            oe.FrameBridgeSize = dr.GetStringVal("FrameBridgeSize");
            oe.FrameCode = dr.GetStringVal("FrameCode");
            oe.FrameColor = dr.GetStringVal("FrameColor");
            oe.FrameEyeSize = dr.GetStringVal("FrameEyeSize");
            oe.FrameTempleType = dr.GetStringVal("FrameTempleType");
            oe.Demographic = dr.GetStringVal("Demographic");
            oe.IsGEyes = dr.GetBoolVal("IsGEyes");
            oe.IsActive = dr.GetBoolVal("IsActive");
            oe.IsMultivision = dr.GetBoolVal("IsMultivision");
            oe.ODSegHeight = dr.GetStringVal("ODSegHeight");
            oe.OSSegHeight = dr.GetStringVal("OSSegHeight");
            oe.VerifiedBy = dr.GetIntVal("VerifiedBy");
            oe.CorrespondenceEmail = dr.GetStringVal("PatientEmail");
            oe.OnholdForConfirmation = dr.GetBoolVal("OnholdForConfirmation");
            oe.MedProsDispense = dr.GetBoolVal("MedProsDispense");
            oe.PimrsDispense = dr.GetBoolVal("PimrsDispense");
            oe.FocDate = dr.GetNullableDateTimeVal("FOCDate");
            oe.LinkedID = dr.GetStringVal("LinkedID");
            return oe;
        }

        public static List<OrderEntity> ProcessGEyesOrderTable(DataTable dt)
        {
            List<OrderEntity> loe = new List<OrderEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                loe.Add(ProcessGEyesOrderRow(dr));
            }
            return loe;
        }
        public static OrderEntity ProcessGEyesOrderRow(DataRow dr)
        {
            OrderEntity oe = new OrderEntity();
            oe.ClinicSiteCode = dr.GetStringVal("ClinicSiteCode");
            oe.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            oe.PrescriptionID = dr.GetIntVal("PrescriptionID");
            oe.IndividualID_Patient = dr.GetIntVal("IndividualID_Patient");
            oe.IndividualID_Tech = dr.GetIntVal("IndividualID_Tech");
            oe.PatientPhoneID = dr.GetIntVal("PatientPhoneID");
            oe.LabSiteCode = dr.GetStringVal("LabSiteCode");
            oe.LensMaterial = dr.GetStringVal("LensMaterial");
            oe.LensType = dr.GetStringVal("LensType");
            oe.LensTypeLong = dr.GetStringVal("LensTypeLong");
            oe.LensTint = dr.GetStringVal("LensTint");
            oe.LocationCode = dr.GetStringVal("LocationCode");
            oe.ModifiedBy = dr.GetStringVal("ModifiedBy");
            //oe.NumberOfCases = dr.GetIntVal("NumberOfCases");
            oe.NumberOfCases = 1;
            //oe.Pairs = dr.GetIntVal("Pairs");
            oe.Pairs = 1;
            oe.OrderNumber = dr.GetStringVal("OrderNumber");
            oe.ShipAddress1 = dr.GetStringVal("ShipAddress1");
            oe.ShipAddress2 = dr.GetStringVal("ShipAddress2");
            oe.ShipAddress3 = dr.GetStringVal("ShipAddress3");
            oe.ShipCity = dr.GetStringVal("ShipCity");
            oe.ShipState = dr.GetStringVal("ShipState");
            oe.ShipZipCode = dr.GetStringVal("ShipZipCode");
            oe.ShipAddressType = dr.GetStringVal("ShipAddressType");
            oe.ShipCountry = dr.GetStringVal("ShipCountry");
            oe.ShipToPatient = dr.GetBoolVal("ShipToPatient");
            oe.Tint = dr.GetStringVal("Tint");
            oe.UserComment1 = dr.GetStringVal("UserComment1");
            oe.UserComment2 = dr.GetStringVal("UserComment2");
            oe.FrameBridgeSize = dr.GetStringVal("FrameBridgeSize");
            oe.FrameCode = dr.GetStringVal("FrameCode");
            oe.FrameColor = dr.GetStringVal("FrameColor");
            oe.FrameEyeSize = dr.GetStringVal("FrameEyeSize");
            oe.FrameTempleType = dr.GetStringVal("FrameTempleType");
            oe.FrameDescription = dr.GetStringVal("FrameDescription");

            oe.Demographic = dr.GetStringVal("Demographic");
            oe.IsGEyes = dr.GetBoolVal("IsGEyes");
            oe.IsActive = dr.GetBoolVal("IsActive");
            oe.IsMultivision = dr.GetBoolVal("IsMultivision");
            oe.ODSegHeight = dr.GetStringVal("ODSegHeight");
            oe.OSSegHeight = dr.GetStringVal("OSSegHeight");
            oe.VerifiedBy = dr.GetIntVal("VerifiedBy");
            oe.CorrespondenceEmail = dr.GetStringVal("PatientEmail");
            oe.OnholdForConfirmation = dr.GetBoolVal("OnholdForConfirmation");
            oe.MedProsDispense = dr.GetBoolVal("MedProsDispense");
            oe.PimrsDispense = dr.GetBoolVal("PimrsDispense");
            return oe;
        }

        public static List<ExamEntity> ProcessExamTable(DataTable dt)
        {
            List<ExamEntity> lee = new List<ExamEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                lee.Add(ProcessExamRows(dr));
            }
            return lee;
        }
        public static ExamEntity ProcessExamRows(DataRow dr)
        {
            ExamEntity ee = new ExamEntity();
            ee.IndividualID_Patient = dr.GetIntVal("IndividualID_Patient");
            ee.ODOSCorrectedAcuity = dr.GetStringVal("ODOSCorrectedAcuity");
            ee.ODOSUncorrectedAcuity = dr.GetStringVal("ODOSUncorrectedAcuity");
            ee.OSCorrectedAcuity = dr.GetStringVal("OSCorrectedAcuity");
            ee.OSUncorrectedAcuity = dr.GetStringVal("OSUncorrectedAcuity");
            ee.ODCorrectedAcuity = dr.GetStringVal("ODCorrectedAcuity");
            ee.ODUncorrectedAcuity = dr.GetStringVal("ODUncorrectedAcuity");
            ee.Comments = dr.GetStringVal("Comment");
            ee.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            ee.ExamDate = dr.GetDateTimeVal("ExamDate");
            ee.ID = dr.GetIntVal("ID");
            ee.ModifiedBy = dr.GetStringVal("ModifiedBy");
            ee.IndividualID_Examiner = dr.GetIntVal("IndividualID_Examiner");
            ee.IndividualID_Patient = dr.GetIntVal("IndividualID_Patient");
            return ee;
        }

        public static List<PrescriptionEntity> ProcessPresciptionTable(DataTable dt)
        {
            List<PrescriptionEntity> lpe = new List<PrescriptionEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                lpe.Add(ProcessPresciptionRows(dr));
            }
            return lpe;
        }
        public static PrescriptionEntity ProcessPresciptionRows(DataRow dr)
        {
            PrescriptionEntity pr = new PrescriptionEntity();
            pr.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            pr.ExamID = dr.GetIntVal("ExamID");
            pr.ID = dr.GetIntVal("ID");
            pr.IndividualID_Patient = dr.GetIntVal("IndividualID_Patient");
            pr.IndividualID_Doctor = dr.GetIntVal("IndividualID_Doctor");
            pr.ModifiedBy = dr.GetStringVal("ModifiedBy");
            pr.ODAxis = dr.GetIntVal("ODAxis");
            pr.ODCylinder = dr.GetDecimalVal("ODCylinder");
            pr.ODHBase = dr.GetStringVal("ODHBase");
            pr.ODHPrism = dr.GetDecimalVal("ODHPrism");
            pr.ODAdd = dr.GetDecimalVal("ODAdd");
            pr.ODSphere = dr.GetDecimalVal("ODSphere");
            pr.ODVBase = dr.GetStringVal("ODVBase");
            pr.ODVPrism = dr.GetDecimalVal("ODVPrism");
            pr.OSAxis = dr.GetIntVal("OSAxis");
            pr.OSCylinder = dr.GetDecimalVal("OSCylinder");
            pr.OSHBase = dr.GetStringVal("OSHBase");
            pr.OSHPrism = dr.GetDecimalVal("OSHPrism");
            pr.OSAdd = dr.GetDecimalVal("OSAdd");
            pr.OSSphere = dr.GetDecimalVal("OSSphere");
            pr.OSVBase = dr.GetStringVal("OSVBase");
            pr.OSVPrism = dr.GetDecimalVal("OSVPrism");
            pr.PDTotal = dr.GetDecimalVal("PDDistant");
            pr.PDOD = dr.GetDecimalVal("ODPDDistant");
            pr.PDOS = dr.GetDecimalVal("OSPDDistant");
            pr.PDTotalNear = dr.GetDecimalVal("PDNear");
            pr.PDODNear = dr.GetDecimalVal("ODPDNear");
            pr.PDOSNear = dr.GetDecimalVal("OSPDNear");
            pr.IsMonoCalculation = dr.GetBoolVal("IsMonoCalculation");
            pr.EnteredODSphere = dr.GetDecimalVal("EnteredODSphere");
            pr.EnteredOSSphere = dr.GetDecimalVal("EnteredOSSphere");
            pr.EnteredODCylinder = dr.GetDecimalVal("EnteredODCylinder");
            pr.EnteredOSCylinder = dr.GetDecimalVal("EnteredOSCylinder");
            pr.EnteredODAxis = dr.GetIntVal("EnteredODAxis");
            pr.EnteredOSAxis = dr.GetIntVal("EnteredOSAxis");
            pr.PrescriptionDate = dr.GetDateTimeVal("PrescriptionDate");
            pr.IsActive = dr.GetBoolVal("IsActive");
            pr.IsUsed = dr.GetBoolVal("IsUsed");
            pr.IsDeletable = dr.GetBoolVal("IsDeletable");
            return pr;
        }

        public static List<IndividualEntity> ProcessIndividualTable(DataTable dt)
        {
            List<IndividualEntity> lie = new List<IndividualEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                lie.Add(ProcessIndividualRow(dr));
            }
            return lie;
        }
        public static IndividualEntity ProcessIndividualRow(DataRow dr)
        {
            IndividualEntity ie = new IndividualEntity();
            ie.Comments = dr.GetStringVal("Comments");
            ie.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            ie.DateOfBirth = dr.GetDateTimeVal("DateOfBirth");
            ie.EADStopDate = dr.GetNullableDateTimeVal("EADStopDate");
            ie.NextFocDate = dr.GetNullableDateTimeVal("NextFocDate");
            ie.FirstName = dr.GetStringVal("FirstName");
            ie.Demographic = dr.GetStringVal("Demographic");
            ie.ID = dr.GetIntVal("ID");
            ie.IDNumberType = dr.GetStringVal("IDNumberType");
            ie.IDNumber = dr.GetStringVal("IDNumber");
            ie.IDNumberTypeDescription = (GetIDTypeDescription(ie.IDNumberType));

            if (string.IsNullOrEmpty(ie.IDNumber))
            {
                ie.IDNumber = dr.GetStringVal("IDNbrs");
                ie.IDNumberTypeDescription = dr.GetStringVal("IDType");
            }

            ie.IsActive = dr.GetBoolVal("IsActive");
            ie.IsPOC = dr.GetBoolVal("IsPOC");
            ie.LastName = dr.GetStringVal("LastName");
            ie.TheaterLocationCode = dr.GetStringVal("TheaterLocationCode");
            ie.MiddleName = dr.GetStringVal("MiddleName");
            ie.ModifiedBy = dr.GetStringVal("ModifiedBy");
            ie.SiteCodeID = dr.GetStringVal("SiteCodeID");
            ie.PersonalType = dr.GetStringVal("PersonalType");
            ie.BOSDescription = ie.Demographic.ToBOSValue();
            ie.StatusDescription = ie.Demographic.ToPatientStatusValue();
            if (dr.Table.Columns.Contains("IsPatient"))
                ie.IsNewPatient = !dr.GetBoolVal("IsPatient");

            return ie;
        }

        public static List<PersonnelEntity> ProcessPersonnelDataTable(DataTable dt)
        {
            List<PersonnelEntity> lte = new List<PersonnelEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                lte.Add(ProcessPersonnelDataRow(dr));
            }
            lte.Sort(delegate(PersonnelEntity p1, PersonnelEntity p2)
            { return p1.LastName.CompareTo(p2.LastName); });
            return lte;
        }
        public static PersonnelEntity ProcessPersonnelDataRow(DataRow dr)
        {
            PersonnelEntity ie = new PersonnelEntity();
            ie.Comments = dr.GetStringVal("Comments");
            ie.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            ie.DateOfBirth = dr.GetDateTimeVal("DateOfBirth");
            ie.EADStopDate = dr.GetNullableDateTimeVal("EADStopDate");
            ie.FirstName = dr.GetStringVal("FirstName");
            ie.Demographic = dr.GetStringVal("Demographic");
            ie.ID = dr.GetIntVal("ID");
            ie.IsActive = dr.GetBoolVal("IsActive");
            ie.IsPOC = dr.GetBoolVal("IsPOC");
            ie.LastName = dr.GetStringVal("LastName");
            ie.TheaterLocationCode = dr.GetStringVal("TheaterLocationCode");
            ie.MiddleName = dr.GetStringVal("MiddleName");
            ie.ModifiedBy = dr.GetStringVal("ModifiedBy");
            ie.PersonalType = dr.GetStringVal("PersonalType");
            ie.SiteCodeID = dr.GetStringVal("SiteCodeID");
            return ie;
        }

        //public static List<SiteAddressEntity> ProcessSiteAddressTable(DataTable dt)
        //{
        //    List<SiteAddressEntity> lae = new List<SiteAddressEntity>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        lae.Add(ProcessSiteAddressRow(dr));
        //    }
        //    return lae;
        //}
        //public static SiteAddressEntity ProcessSiteAddressRow(DataRow dr)
        //{
        //    SiteAddressEntity ae = new SiteAddressEntity();
        //    ae.ID = dr.GetIntVal("ID");
        //    ae.Address1 = dr.GetStringVal("Address1");
        //    ae.Address2 = dr.GetStringVal("Address2");
        //    ae.Address3 = dr.GetStringVal("Address3");
        //    ae.ID = dr.GetIntVal("ID");
        //    ae.SiteCode = dr.GetStringVal("SiteCode");
        //    ae.AddressType = dr.GetStringVal("AddressType");
        //    ae.City = dr.GetStringVal("City");
        //    ae.Country = dr.GetStringVal("Country");
        //    ae.DateLastModified = dr.GetDateTimeVal("DateLastModified");
        //    ae.IsConus = dr.GetBoolVal("IsConus");
        //    ae.IsActive = dr.GetBoolVal("IsActive");
        //    ae.ModifiedBy = dr.GetStringVal("ModifiedBy");
        //    ae.State = dr.GetStringVal("State");
        //    ae.ZipCode = dr.GetStringVal("ZipCode").ToZipCodeDisplay();
        //    return ae;
        //}

        public static List<AddressEntity> ProcessAddressTable(DataTable dt)
        {
            List<AddressEntity> lae = new List<AddressEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                lae.Add(ProcessAddressRow(dr));
            }
            return lae;
        }
        public static AddressEntity ProcessAddressRow(DataRow dr)
        {
            AddressEntity ae = new AddressEntity();
            ae.Address1 = dr.GetStringVal("Address1");
            ae.Address2 = dr.GetStringVal("Address2");
            ae.Address3 = dr.GetStringVal("Address3");
            ae.ID = dr.GetIntVal("ID");
            ae.AddressType = dr.GetStringVal("AddressType");
            ae.City = dr.GetStringVal("City");
            ae.Country = dr.GetStringVal("Country");
            ae.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            ae.IndividualID = dr.GetIntVal("IndividualID");
            ae.IsActive = dr.GetBoolVal("IsActive");
            ae.UIC = dr.GetStringVal("UIC");
            ae.ModifiedBy = dr.GetStringVal("ModifiedBy");
            ae.State = dr.GetStringVal("State");
            ae.ZipCode = dr.GetStringVal("ZipCode").ToZipCodeDisplay();
            return ae;
        }

        //public static List<UnitEntity> ProcessUnitTable(DataTable dt)
        //{
        //    List<UnitEntity> lae = new List<UnitEntity>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        lae.Add(ProcessUnitRow(dr));
        //    }
        //    return lae;
        //}
        //public static UnitEntity ProcessUnitRow(DataRow dr)
        //{
        //    UnitEntity ae = new UnitEntity();
        //    ae.UnitAddress1 = dr.GetStringVal("UnitAddress1");
        //    ae.UnitAddress2 = dr.GetStringVal("UnitAddress2");
        //    ae.UnitName = dr.GetStringVal("UnitName");
        //    ae.UIC = dr.GetStringVal("UIC");
        //    ae.UnitCity = dr.GetStringVal("UnitCity");
        //    ae.DateLastModified = dr.GetDateTimeVal("DateLastModified");
        //    ae.IsActive = dr.GetBoolVal("IsActive");
        //    ae.ModifiedBy = dr.GetStringVal("ModifiedBy");
        //    ae.UnitState = dr.GetStringVal("UnitState");
        //    ae.UnitZipCode = dr.GetStringVal("UnitZipCode").ToZipCodeDisplay();
        //    return ae;
        //}

        public static List<EMailAddressEntity> ProcessEMailAddressTable(DataTable dt)
        {
            List<EMailAddressEntity> lea = new List<EMailAddressEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                lea.Add(ProcessEMailAddressRow(dr));
            }
            return lea;
        }
        public static EMailAddressEntity ProcessEMailAddressRow(DataRow dr)
        {
            EMailAddressEntity ea = new EMailAddressEntity();
            ea.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            ea.EMailAddress = dr.GetStringVal("EMailAddress");
            ea.ID = dr.GetIntVal("ID");
            ea.EMailType = dr.GetStringVal("EMailType");
            ea.IndividualID = dr.GetIntVal("IndividualID");
            ea.IsActive = dr.GetBoolVal("IsActive");
            ea.ModifiedBy = dr.GetStringVal("ModifiedBy");
            return ea;
        }

        public static List<IndividualTypeEntity> ProcessIndividualTypeTable(DataTable dt)
        {
            List<IndividualTypeEntity> ite = new List<IndividualTypeEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                ite.Add(ProcessIndividualTypeRow(dr));
            }
            return ite;
        }
        public static IndividualTypeEntity ProcessIndividualTypeRow(DataRow dr)
        {
            IndividualTypeEntity ie = new IndividualTypeEntity();
            ie.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            ie.ID = dr.GetIntVal("ID");
            ie.TypeId = dr.GetIntVal("TypeID");
            ie.TypeDescription = dr.GetStringVal("TypeDescription");
            ie.IndividualId = dr.GetIntVal("IndividualID");
            ie.IsActive = dr.GetBoolVal("IsActive");
            ie.ModifiedBy = dr.GetStringVal("ModifiedBy");
            return ie;
        }

        public static List<PhoneNumberEntity> ProcessPhoneTable(DataTable dt)
        {
            List<PhoneNumberEntity> lpe = new List<PhoneNumberEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                lpe.Add(ProcessPhoneRow(dr));
            }
            return lpe;
        }
        public static PhoneNumberEntity ProcessPhoneRow(DataRow dr)
        {
            PhoneNumberEntity pe = new PhoneNumberEntity();
            pe.IndividualID = dr.GetIntVal("IndividualID");
            pe.DateLastModified = dr.GetDateTimeVal("DateLastModified");

            var testPhone = dr.GetStringVal("PhoneNumber");
            if (!testPhone.Contains("-") && testPhone.Length.Equals(7))
                testPhone = string.Format("{0:###-####}", dr.GetIntVal("PhoneNumber"));
            pe.PhoneNumber = testPhone;

            pe.ID = dr.GetIntVal("ID");
            pe.PhoneNumberType = dr.GetStringVal("PhoneNumberType");
            pe.IsActive = dr.GetBoolVal("IsActive");
            pe.ModifiedBy = dr.GetStringVal("ModifiedBy");
            pe.AreaCode = dr.GetStringVal("AreaCode");
            pe.Extension = dr.GetStringVal("Extension");
            return pe;
        }

        public static List<FrameEntity> ProcessFrameTable(DataTable dt)
        {
            var fe = new List<FrameEntity>();
            foreach (DataRow r in dt.Rows)
                fe.Add(ProcessFrameRow(r));

            return fe;
        }
        public static FrameEntity ProcessFrameRow(DataRow dr)
        {
            FrameEntity fe = new FrameEntity();
            fe.FrameCode = dr.GetStringVal("FrameCode");
            fe.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            fe.FrameDescription = dr.GetStringVal("FrameDescription");
            fe.FrameNotes = dr.GetStringVal("FrameNotes");
            fe.IsActive = dr.GetBoolVal("IsActive");
            fe.IsInsert = dr.GetBoolVal("IsInsert");
            fe.ModifiedBy = dr.GetStringVal("ModifiedBy");
            fe.MaxPair = dr.GetIntVal("MaxPair");
            fe.IsFoc = dr.GetBoolVal("IsFOC");
            fe.FrameType = dr.GetStringVal("FrameType");

            return fe;
        }

        public static List<FrameItemEntity> ProcessFrameItemTable(DataTable dt)
        {
            var fie = new List<FrameItemEntity>();
            foreach (DataRow r in dt.Rows)
                fie.Add(ProcessFrameItemRow(r));

            return fie;
        }
        public static FrameItemEntity ProcessFrameItemRow(DataRow dr)
        {
            FrameItemEntity fe = new FrameItemEntity();
            fe.Value = dr.GetStringVal("Value");
            fe.TypeEntry = dr.GetStringVal("TypeEntry");
            fe.DateLastModified = dr.GetDateTimeVal("DateLastModified");
            fe.Text = dr.GetStringVal("Text");
            fe.IsActive = dr.GetBoolVal("IsActive");
            fe.ModifiedBy = dr.GetStringVal("ModifiedBy");
            return fe;
        }

        //public static List<LookupTableEntity> ProcessLookupTable(DataTable dt)
        //{
        //    List<LookupTableEntity> lte = new List<LookupTableEntity>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        lte.Add(ProcessLookUpTableRows(dr));
        //    }
        //    return lte;
        //}
        //public static LookupTableEntity ProcessLookUpTableRows(DataRow dr)
        //{
        //    LookupTableEntity lt = new LookupTableEntity();
        //    lt.Id = dr.GetIntVal("Id");
        //    lt.Code = dr.GetStringVal("Code");
        //    lt.Text = dr.GetStringVal("Text");
        //    lt.Value = dr.GetStringVal("Value");
        //    lt.Description = dr.GetStringVal("Description");
        //    lt.IsActive = dr.GetBoolVal("IsActive");
        //    lt.ModifiedBy = dr.GetStringVal("ModifiedBy");
        //    lt.DateLastModified = dr.GetDateTimeVal("DateLastModified");
        //    return lt;
        //}

        public static PatientEntity ProcessAllPatientInfo(DataSet ds)
        {
            PatientEntity pe = new PatientEntity();
            if (ds.Tables[0].Rows.Count >= 1)
            {
                pe.Individual = SrtsHelper.ProcessIndividualRow(ds.Tables[0].Rows[0]);
                pe.IDNumbers = SrtsHelper.ProcessIdentificationNumberTable(ds.Tables[1]);
                pe.EMailAddresses = SrtsHelper.ProcessEMailAddressTable(ds.Tables[2]);
                pe.PhoneNumbers = SrtsHelper.ProcessPhoneTable(ds.Tables[3]);
                pe.Addresses = SrtsHelper.ProcessAddressTable(ds.Tables[4]);
                pe.Orders = SrtsHelper.ProcessOrderTable(ds.Tables[5]);
                pe.Exams = SrtsHelper.ProcessExamTable(ds.Tables[6]);
                pe.Prescriptions = SrtsHelper.ProcessPresciptionTable(ds.Tables[7]);
            }
            return pe;
        }

        //public static Dictionary<BOSEntity, Dictionary<StatusEntity, List<RankEntity>>> ProcessFrameEligibilityPartsTable(DataTable dt)
        public static Dictionary<String, Dictionary<String, List<String>>> ProcessFrameEligibilityPartsTable(DataTable dt)
        {
            var l = dt.Rows.Cast<DataRow>().ToList(); ;
            var branches = l.Select(x => x["BOS"].ToString()).Distinct().ToList();

            var bosStatGrades = new Dictionary<String, Dictionary<String, List<String>>>();

            foreach (var b in branches)
            {
                var stats = l.Where(x => x["BOS"].ToString().ToLower() == b.ToLower()).Select(x => x["Status"].ToString()).Distinct().ToList();
                var statGrades = new Dictionary<String, List<String>>();
                foreach (var stat in stats)
                {
                    var grades = l.Where(x => x["BOS"].ToString().ToLower() == b.ToLower() && x["Status"].ToString().ToLower() == stat.ToLower()).Select(x => x["Grade"].ToString()).ToList();
                    statGrades.Add(stat, grades);
                }
                bosStatGrades.Add(b, statGrades);
            }

            return bosStatGrades;
            //foreach (var branch in branches)
            //{
            //    var stats = d.GetStatusByBOS(branch.BOSValue);
            //    var statGrades = new Dictionary<StatusEntity, List<RankEntity>>();
            //    foreach (var stat in stats)
            //    {
            //        var grades = d.GetRanksByBOSAndStatus(branch.BOSValue, stat.StatusValue);
            //        statGrades.Add(stat, grades);
            //    }
            //    bosStatGrades.Add(branch, statGrades);
            //}
        }

        public static List<String> ProcessSingleStringColumn(DataTable dt, Int32 colIdx)
        {
            var l = new List<String>();
            foreach (DataRow r in dt.Rows)
                l.Add(r[colIdx].ToString());
            return l;
        }
    }
}