using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers
{
    public static partial class SrtsHelper
    {
        private static XmlDocument doc;

        static SrtsHelper()
        {
            doc = new XmlDocument();
            doc.Load(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/XML/SrtsData.xml"));
        }

        public static string FormatZipCodeForSave(string zipCode)
        {
            if (zipCode.Length == 5)
            {
                return zipCode.PadRight(9, '0');
            }
            if (zipCode.Contains('-'))
            {
                return zipCode.Remove(5, 1);
            }
            else
            {
                return zipCode;
            }
        }

        public static string CalculateFiscalYear()
        {
            string fiscalYear = string.Empty;
            DateTime t1 = DateTime.Now;
            DateTime t2 = new DateTime(DateTime.Now.Year, 10, 1);
            if (DateTime.Compare(t1, t2) >= 0)
            {
                fiscalYear = t1.AddYears(1).ToString("yy");
            }
            else
            {
                fiscalYear = t1.ToString("yy");
            }
            return fiscalYear;
        }

        public static bool CompareBaseToPrism(string _base, string _prism)
        {
            if ((string.IsNullOrEmpty(_base) && _prism == "0.00") || (!string.IsNullOrEmpty(_base) && _prism != "0.00"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetIDTypeDescription(string _key)
        {
            Dictionary<string, string> idTypes = new Dictionary<string, string>();
            string description = string.Empty;
            idTypes = GetIdentificationTypes();
            if (idTypes.ContainsKey(_key))
            {
                description = idTypes[_key];
            }
            return description;
        }

        public static Dictionary<string, string> GetIdentificationTypes()
        {
            Dictionary<string, string> idTypes = new Dictionary<string, string>();
            XmlNodeList xList = doc.SelectNodes("//IDType[@key]");

            foreach (XmlNode node in xList)
            {
                XmlNodeReader reader = new XmlNodeReader(node);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "IDType")
                        {
                            if (!idTypes.ContainsKey(reader.GetAttribute("key")))
                            {
                                idTypes.Add(reader.GetAttribute("key"), reader.ReadString());
                            }
                        }
                    }
                }
            }
            return idTypes;
        }
    }
}