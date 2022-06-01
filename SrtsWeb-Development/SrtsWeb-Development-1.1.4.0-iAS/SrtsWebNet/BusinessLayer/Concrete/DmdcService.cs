using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.mil.osd.dmdc.sadr;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using SrtsWeb.BusinessLayer.DmdcMock;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class used for performing DMDC search operations
    /// </summary>
    public class DmdcService : IDmdcService
    {
        /// <summary>
        /// Do a search against DMDC for personnel data by DODID number
        /// </summary>
        /// <param name="dodId">DOD Identification Number</param>
        /// <returns>DmdcPerson class list of search results.</returns>
        public IEnumerable<DmdcPerson> DoDmdcByDodId(String dodId)
        {
            try
            {
                using (var svc = GetServiceObject())
                {
                    var res = svc.findByDoDEdi(new EDIIdentifierType()
                    {
                        customer = new CustomerType() { id = 6250, schema = "data_provision", version = "1.0" },
                        DOD_EDI_PN_ID = dodId
                    });

                    if (!res.found) return new List<DmdcPerson>();

                    var xres = ((XmlNode[])res.ResponseData);
                    var xelement = xres[0];

                    XmlNamespaceManager ns = new XmlNamespaceManager(xelement.OwnerDocument.NameTable);
                    ns.AddNamespace("adr", ConfigurationManager.AppSettings["adrNameSpace"].ToString());

                    var record = xelement.SelectNodes("//adr:adrRecord", ns);
                    return ProcessXml(record, ns).ToList();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("DMDC Web Service Execution Error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return new List<DmdcPerson>();
            }
        }

        /// <summary>
        /// Do a search against DMDC for personnel data by Social Security Number
        /// </summary>
        /// <param name="ssn">Social Security Number</param>
        /// <returns>DmdcPerson class list of search results.</returns>
        public IEnumerable<DmdcPerson> DoDmdcBySsn(String ssn)
        {
            try
            {
                using (var svc = GetServiceObject())
                {
                    var res = svc.findByPersonId(new PersonIdentifierRequestType()
                    {
                        customer = new CustomerType() { id = 6250, schema = "data_provision", version = "1.0" },
                        person = new PersonIdentifierType()
                        {
                            PN_ID = ssn,
                            PN_ID_TYP_CD = "S"
                        }
                    });

                    if (!res.found) return new List<DmdcPerson>();

                    var xres = ((XmlNode[])res.ResponseData);
                    var xelement = xres[0];

                    XmlNamespaceManager ns = new XmlNamespaceManager(xelement.OwnerDocument.NameTable);
                    ns.AddNamespace("adr", ConfigurationManager.AppSettings["adrNameSpace"].ToString());

                    var record = xelement.SelectNodes("//adr:adrRecord", ns);
                    return ProcessXml(record, ns).ToList();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("DMDC Web Service Execution Error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return new List<DmdcPerson>();
            }
        }

        /// <summary>
        /// Do a search against DMDC for personnel data by Foreign Service Identification Number
        /// </summary>
        /// <param name="fsId">Foreign Service Identification Number</param>
        /// <returns>DmdcPerson class list of search results.</returns>
        public IEnumerable<DmdcPerson> DoDmdcByFsId(String fsId)
        {
            try
            {
                using (var svc = GetServiceObject())
                {
                    var res = svc.findByPersonId(new PersonIdentifierRequestType()
                    {
                        customer = new CustomerType() { id = 6250, schema = "data_provision", version = "1.0" },
                        person = new PersonIdentifierType()
                        {
                            PN_ID = fsId,
                            PN_ID_TYP_CD = "F"
                        }
                    });

                    if (!res.found) return new List<DmdcPerson>();

                    var xres = ((XmlNode[])res.ResponseData);
                    var xelement = xres[0];

                    XmlNamespaceManager ns = new XmlNamespaceManager(xelement.OwnerDocument.NameTable);
                    ns.AddNamespace("adr", ConfigurationManager.AppSettings["adrNameSpace"].ToString());

                    var record = xelement.SelectNodes("//adr:adrRecord", ns);
                    return ProcessXml(record, ns).ToList();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("DMDC Web Service Execution Error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return new List<DmdcPerson>();
            }
        }

        private IEnumerable<DmdcPerson> ProcessXml(XmlNodeList xList, XmlNamespaceManager xMgr)
        {
            // get person record from the "master" record
            var pList = new List<DmdcPerson>();

            foreach (XmlNode xNode in xList)
            {
                var idType = xNode.SelectSingleNode("//adr:person/adr:PN_ID_TYP_CD", xMgr).HandleNull().InnerText;
                var p = new DmdcPerson()
                {
                    PnLastName = xNode.SelectSingleNode("//adr:person/adr:PN_LST_NM", xMgr).HandleNull().InnerText,
                    PnFirstName = xNode.SelectSingleNode("//adr:person/adr:PN_1ST_NM", xMgr).HandleNull().InnerText,
                    PnMiddleName = xNode.SelectSingleNode("//adr:person/adr:PN_MID_NM", xMgr).HandleNull().InnerText,
                    PnDateOfBirth = xNode.SelectSingleNode("//adr:person/adr:PN_BRTH_DT", xMgr).HandleNull().InnerText.FromDmdcDate(),
                    MailingAddress1 = xNode.SelectSingleNode("//adr:person/adr:MA_LN1_TX", xMgr).HandleNull().InnerText,
                    MailingCity = xNode.SelectSingleNode("//adr:person/adr:MA_CITY_NM", xMgr).HandleNull().InnerText,
                    MailingState = xNode.SelectSingleNode("//adr:person/adr:MA_ST_CD", xMgr).HandleNull().InnerText,
                    MailingZip = xNode.SelectSingleNode("//adr:person/adr:MA_PR_ZIP_CD", xMgr).HandleNull().InnerText,
                    MailingZipExtension = xNode.SelectSingleNode("//adr:person/adr:MA_PR_ZIPX_CD", xMgr).HandleNull().InnerText,
                    MailingCountry = xNode.SelectSingleNode("//adr:person/adr:MA_CTRY_CD", xMgr).HandleNull().InnerText,
                    Email = xNode.SelectSingleNode("//adr:personnel/adr:EMA_TX", xMgr).HandleNull().InnerText,
                    PhoneNumber = xNode.SelectSingleNode("//adr:workPhone/adr:WC_TNUM_CD", xMgr).HandleNull().InnerText,
                    _DmdcIdentifier = new List<DmdcIdentifiers>()
                    {
                        new DmdcIdentifiers()
                        {
                            PnId=xNode.SelectSingleNode("//adr:person/adr:PN_ID", xMgr).HandleNull().InnerText,
                            PnIdType = idType
                        },
                        new DmdcIdentifiers()
                        {
                            PnId=xNode.SelectSingleNode("//adr:DOD_EDI_PN_ID", xMgr).HandleNull().InnerText,
                            PnIdType = "D"
                        }
                    },
                    _DmdcPersonnel = new DmdcPersonnel()
                    {
                        PnCategoryCode = xNode.SelectSingleNode("//adr:personnel/adr:PNL_CAT_CD", xMgr).HandleNull().InnerText,
                        ServiceCode = xNode.SelectSingleNode("//adr:personnel/adr:SVC_CD", xMgr).HandleNull().InnerText,
                        OrganizationCode = xNode.SelectSingleNode("//adr:personnel/adr:ORG_CD", xMgr).HandleNull().InnerText,
                        PayPlanCode = xNode.SelectSingleNode("//adr:personnel/adr:PAY_PLN_CD", xMgr).HandleNull().InnerText,
                        PayGrade = xNode.SelectSingleNode("//adr:personnel/adr:PG_CD", xMgr).HandleNull().InnerText
                    }
                };

                pList.Add(p);
            }

            return pList;
        }

        private RecordGeneratorWebService GetServiceObject()
        {
            X509Certificate2 c = null;

            try
            {
                var s = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                s.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                var cs = s.Certificates.Find(X509FindType.FindBySubjectName, ConfigurationManager.AppSettings["certName"], true);
                if (cs.Count.Equals(0)) return null;
                c = cs[0];
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Certificate error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

            try
            {
                RecordGeneratorWebService svc = new RecordGeneratorWebService();

                svc.ClientCertificates.Add(c);

                return svc;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("DMDC Service Creation Error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public DmdcPersonWs GetMockData(String IdNumber, String IdType)
        {
            X509Certificate2 c = null;

            try
            {
                var s = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                s.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                var cs = s.Certificates.Find(X509FindType.FindBySubjectName, ConfigurationManager.AppSettings["certName"], true);
                if (cs.Count.Equals(0)) return null;
                c = cs[0];
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Certificate error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

            DmdcMockWsClient svc = new DmdcMockWsClient();
            svc.ClientCredentials.ClientCertificate.Certificate = c;
            return svc.DoWork(IdNumber, IdType);
        }

        public static IEnumerable<DmdcPerson_Flat> FlattenDmdcPerson(IEnumerable<DmdcPerson> person)
        {
            var l = new List<DmdcPerson_Flat>();

            foreach (var p in person)
            {
                if (p == null) continue;

                var f = new DmdcPerson_Flat();
                f.AttachedUnitIdCode = p._DmdcPersonnel.AttachedUnitIdCode;
                f.Email = p.Email;
                f.EnterpriseUserName = p.EnterpriseUserName;
                f.MailingAddress1 = p.MailingAddress1;
                f.MailingAddress2 = p.MailingAddress2;
                f.MailingCity = p.MailingCity;
                f.MailingCountry = p.MailingCountry;
                f.MailingState = p.MailingState;
                f.MailingZip = p.MailingZip;
                f.MailingZipExtension = p.MailingZipExtension;
                f.MatchReasonCode1 = p._DmdcIdentifier != null && p._DmdcIdentifier.Count > 0 ? p._DmdcIdentifier[0].MatchReasonCode : "";
                f.MatchReasonCode2 = p._DmdcIdentifier != null && p._DmdcIdentifier.Count > 1 ? p._DmdcIdentifier[1].MatchReasonCode : "";
                f.OrganizationCode = p._DmdcPersonnel.OrganizationCode;
                f.PayGrade = p._DmdcPersonnel.PayGrade;
                f.PayPlanCode = p._DmdcPersonnel.PayPlanCode;
                f.PhoneNumber = p.PhoneNumber;
                f.PnCadencyName = p.PnCadencyName;
                f.PnCategoryCode = p._DmdcPersonnel.PnCategoryCode;
                f.PnDateOfBirth = p.PnDateOfBirth;
                f.PnDeathCalendarDate = p.PnDeathCalendarDate;
                f.PnEntitlementTypeCode = p._DmdcPersonnel.PnEntitlementTypeCode;
                f.PnFirstName = p.PnFirstName;
                f.PnId1 = p._DmdcIdentifier != null && p._DmdcIdentifier.Count > 0 ? p._DmdcIdentifier[0].PnId : "";
                f.PnId2 = p._DmdcIdentifier != null && p._DmdcIdentifier.Count > 1 ? p._DmdcIdentifier[1].PnId : "";
                f.PnIdType1 = p._DmdcIdentifier != null && p._DmdcIdentifier.Count > 0 ? p._DmdcIdentifier[0].PnIdType : "";
                f.PnIdType2 = p._DmdcIdentifier != null && p._DmdcIdentifier.Count > 1 ? p._DmdcIdentifier[1].PnIdType : "";
                f.PnLastName = p.PnLastName;
                f.PnMiddleName = p.PnMiddleName;
                f.PnProjectedEndDate = p._DmdcPersonnel.PnProjectedEndDate;
                f.Rank = p._DmdcPersonnel.Rank;
                f.ServiceCode = p._DmdcPersonnel.ServiceCode;

                l.Add(f);
            }

            return l;
        }
        public static IEnumerable<DmdcPerson_Flat> FlattenDmdcPerson(IEnumerable<DmdcPersonWs> personWs)
        {
            var l = new List<DmdcPerson_Flat>();

            foreach (var p in personWs)
            {
                if (p == null) continue;

                var f = new DmdcPerson_Flat();
                f.AttachedUnitIdCode = p._DmdcPersonnel.AttachedUnitIdCode;
                f.Email = p.Email;
                f.EnterpriseUserName = p.EnterpriseUserName;
                f.MailingAddress1 = p.MailingAddress1;
                f.MailingAddress2 = p.MailingAddress2;
                f.MailingCity = p.MailingCity;
                f.MailingCountry = p.MailingCountry;
                f.MailingState = p.MailingState;
                f.MailingZip = p.MailingZip;
                f.MailingZipExtension = p.MailingZipExtension;
                f.MatchReasonCode1 = p._DmdcIdentifier != null && p._DmdcIdentifier.Length > 0 ? p._DmdcIdentifier[0].MatchReasonCode : "";
                f.MatchReasonCode2 = p._DmdcIdentifier != null && p._DmdcIdentifier.Length > 1 ? p._DmdcIdentifier[1].MatchReasonCode : "";
                f.OrganizationCode = p._DmdcPersonnel.OrganizationCode;
                f.PayGrade = p._DmdcPersonnel.PayGrade;
                f.PayPlanCode = p._DmdcPersonnel.PayPlanCode;
                f.PhoneNumber = p.PhoneNumber;
                f.PnCadencyName = p.PnCadencyName;
                f.PnCategoryCode = p._DmdcPersonnel.PnCategoryCode;
                f.PnDateOfBirth = p.PnDateOfBirth;
                f.PnDeathCalendarDate = p.PnDeathCalendarDate;
                f.PnEntitlementTypeCode = p._DmdcPersonnel.PnEntitlementTypeCode;
                f.PnFirstName = p.PnFirstName;
                f.PnId1 = p._DmdcIdentifier != null && p._DmdcIdentifier.Length > 0 ? p._DmdcIdentifier[0].PnId : "";
                f.PnId2 = p._DmdcIdentifier != null && p._DmdcIdentifier.Length > 1 ? p._DmdcIdentifier[1].PnId : "";
                f.PnIdType1 = p._DmdcIdentifier != null && p._DmdcIdentifier.Length > 0 ? p._DmdcIdentifier[0].PnIdType : "";
                f.PnIdType2 = p._DmdcIdentifier != null && p._DmdcIdentifier.Length > 1 ? p._DmdcIdentifier[1].PnIdType : "";
                f.PnLastName = p.PnLastName;
                f.PnMiddleName = p.PnMiddleName;
                f.PnProjectedEndDate = p._DmdcPersonnel.PnProjectedEndDate;
                f.Rank = p._DmdcPersonnel.Rank;
                f.ServiceCode = p._DmdcPersonnel.ServiceCode;

                l.Add(f);
            }

            return l;
        }
    }

    public static class DmdcExtenders
    {
        /// <summary>
        /// Tests if node is null and creates a blank node if it is, otherwise return the original node.
        /// </summary>
        /// <param name="xNode">XmlNode to test.</param>
        /// <returns>Tested node, blank or original.</returns>
        public static XmlNode HandleNull(this XmlNode xNode)
        {
            if (xNode != null) return xNode;
            XmlDocument d = new XmlDocument();
            d.XmlResolver = null;
            var x = d.CreateNode(XmlNodeType.Element, "adr", "blank", "http://adr.dmdc.osd.mil/adrRecord");
            x.InnerText = String.Empty;
            return x;
        }

        /// <summary>
        /// Check for null node.
        /// </summary>
        /// <param name="xNode">XmlNode to test.</param>
        /// <returns>Result of test.</returns>
        public static Boolean IsNodeNull(this XmlNode xNode)
        {
            return xNode == null;
        }

        /// <summary>
        /// Get a DateTime formatted date from a DMDC formatted date.
        /// </summary>
        /// <param name="sDt">Date to format.</param>
        /// <returns>DateTime formatted date.</returns>
        public static DateTime FromDmdcDate(this String sDt)
        {
            if (String.IsNullOrEmpty(sDt)) return default(DateTime);
            var dt = DateTime.MinValue;
            try
            {
                dt = new DateTime(Convert.ToInt32(sDt.Substring(0, 4)), Convert.ToInt32(sDt.Substring(4, 2)), Convert.ToInt32(sDt.Substring(6, 2)));
            }
            catch
            {
                // Do nothing
            }
            return dt;
        }
    }
}