using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Linq;
using SrtsWeb.BusinessLayer.Abstract;
using System.Configuration;
using SrtsWeb.Entities;

namespace SrtsWeb.BusinessLayer.Concrete
{
    public class USPSService : IUSPSService
    {
        public AddressEntity ValidateAddress(AddressEntity address)
        {
            string userID = ConfigurationManager.AppSettings["uspsUsername"];
            string requestBaseURL = ConfigurationManager.AppSettings["uspsAPICallBase"];
            AddressEntity resultAddress = new AddressEntity();
            resultAddress.Country = "US"; // USPS can only fetch US

            XDocument requestDoc = new XDocument(
                new XElement("AddressValidateRequest",
                new XAttribute("USERID", userID),
                new XElement("Revision", "1"),
                new XElement("Address",
                new XAttribute("ID", "0"),
                new XElement("Address1", address.Address1),
                new XElement("Address2", address.Address2),
                new XElement("City", address.City),
                new XElement("State", address.State),
                new XElement("Zip5", address.ZipCode),
                new XElement("Zip4", "")
                    )
                )
            );

            try
            {
                var url = requestBaseURL + "?API=Verify&XML=" + requestDoc;
                var client = new WebClient();
                var response = client.DownloadString(url);

                var xdoc = XDocument.Parse(response.ToString());

                foreach (XElement element in xdoc.Descendants("Address"))
                {
                    resultAddress.Address1 = GetXMLElement(element, "Address2");
                    resultAddress.Address2 = GetXMLElement(element, "Address1");
                    resultAddress.City = GetXMLElement(element, "City");
                    resultAddress.State = GetXMLElement(element, "State");
                    resultAddress.ZipCode = GetXMLElement(element, "Zip5");
                    resultAddress.ZipCode += "-" + GetXMLElement(element, "Zip4");
                }

            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
            }

            return resultAddress;
        }

        private string GetXMLElement(XElement element, string name)
        {
            var el = element.Element(name);
            if (el != null)
            {
                return el.Value;
            }
            return "";
        }

        private string GetXMLAttribute(XElement element, string name)
        {
            var el = element.Attribute(name);
            if (el != null)
            {
                return el.Value;
            }
            return "";
        }

    }
}
