using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static List<String> ConvertToStringList(this XmlDocument xDoc)
        {
            var sList = new List<String>();
            foreach (XmlNode n in xDoc.DocumentElement.ChildNodes)
                sList.Add(n.OuterXml);

            return sList;
        }

        public static T DeserializeXml<T>(this XmlDocument xDoc)
            where T : class, new()
        {
            T tIn = new T();

            XmlSerializer deserializer = new XmlSerializer(typeof(T));

            using (TextReader textReader = new StringReader(xDoc.OuterXml))
            {
                tIn = (T)deserializer.Deserialize(textReader);
            }

            return tIn;
        }
    }
}