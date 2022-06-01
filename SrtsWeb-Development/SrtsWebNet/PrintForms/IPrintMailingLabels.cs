using System.Xml;

namespace SrtsWeb.PrintForms
{
    public interface IPrintMailingLabels
    {
        XmlDocument AddAddress(System.Data.DataRow dr, System.Xml.XmlDocument _doc);

        XmlDocument OpenXMLFile();

        void PrintLabels(string _path, string _siteCode);

        string SaveXml(System.Xml.XmlDocument _doc, string _siteCode);
    }
}