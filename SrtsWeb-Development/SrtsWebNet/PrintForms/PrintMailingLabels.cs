using System;
using System.Data;
using System.Web;
using System.Xml;

namespace SrtsWeb.PrintForms
{
    public class PrintMailingLabels : IPrintMailingLabels
    {
        public XmlDocument OpenXMLFile()
        {
            XmlDocument _doc = new XmlDocument();
            XmlDeclaration xmlDec = _doc.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlElement rootNode = _doc.CreateElement("employees");
            _doc.InsertBefore(xmlDec, _doc.DocumentElement);
            _doc.AppendChild(rootNode);
            return _doc;
        }

        public string SaveXml(XmlDocument _doc, string _siteCode)
        {
            DateTime dt = DateTime.Now;
            string _localPath = HttpContext.Current.Server.MapPath("~/PrintForms/Temp");
            string _docName = string.Format("{0}_{1}_{2}_{3}.Xml", _siteCode, dt.Day, dt.Month, dt.Year);
            string _path = string.Format("{0}/{1}", _localPath, _docName);
            _doc.Save(_path);
            return _path;
        }

        public XmlDocument AddAddress(DataRow dr, XmlDocument _doc)
        {
            XmlNode _newContact = _doc.CreateElement("employee");
            XmlNode _name = _doc.CreateElement("Name");
            XmlNode _address = _doc.CreateElement("Address");
            XmlNode _city = _doc.CreateElement("City");
            XmlNode _state = _doc.CreateElement("State");
            XmlNode _postalCode = _doc.CreateElement("PostalCode");

            _name.InnerText = string.Format("{0} {1}", dr["FirstName"].ToString(), dr["LastName"].ToString());
            if (string.IsNullOrEmpty(dr["ShipAddress2"].ToString()))
            {
                _address.InnerText = dr["ShipAddress1"].ToString();
            }
            else
            {
                _address.InnerText = string.Format("{0}{1}{2}", dr["ShipAddress1"].ToString(), Environment.NewLine, dr["ShipAddress2"].ToString());
            }
            _city.InnerText = dr["ShipCity"].ToString();
            _state.InnerText = dr["ShipState"].ToString();
            _postalCode.InnerText = dr["ShipZipCode"].ToString();

            _newContact.AppendChild(_name);
            _newContact.AppendChild(_address);
            _newContact.AppendChild(_city);
            _newContact.AppendChild(_state);
            _newContact.AppendChild(_postalCode);

            _doc.SelectSingleNode("//employees").AppendChild(_newContact);

            return _doc;
        }

        public DataTable LoadLabelData(string _path)
        {
            DataSet dsLabels = new DataSet();
            dsLabels.ReadXml(_path);
            return dsLabels.Tables[0];
        }

        public void PrintLabels(string _path, string _siteCode)
        {
            HttpContext.Current.Response.Redirect("~/WebForms/PrintForms/LabelRptViewer.aspx");
        }
    }
}