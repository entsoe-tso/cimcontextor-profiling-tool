/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20230205
/////////////////////////////////////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Xml;

namespace CimContextor.Configuration
{
    public class DataQualifier
    {
        public static string XML_NAME = "dataQualifier";
        private List<Qualifier> qualifiers = new List<Qualifier>();

        public DataQualifier() { }

        public void AddQualifier(Qualifier qualifier)
        {
            qualifiers.Add(qualifier);
        }

        public List<Qualifier> GetQualifiers()
        {
            return qualifiers;
        }

        public XmlNode GetXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement(XML_NAME);
            foreach (Qualifier qualifier in qualifiers)
            {
                XmlNode qualifierNode = qualifier.GetXmlNode(xmlDoc);
                node.AppendChild(qualifierNode);
            }

            return node;
        }

        
        public string GetAsString()
        {
            string start = "<" + XML_NAME + ">\n";
            string end = "\t</" + XML_NAME + ">";
            string children = "";
            foreach (Qualifier qualifier in qualifiers)
            {
                children += "\t\t" + qualifier.GetAsString() + "\n";
            }
            return start + children + end;
        }

        public DataQualifier GetCopy()
        {
            DataQualifier cpy = new DataQualifier();
            foreach(Qualifier qualifier in qualifiers)
            {
                cpy.qualifiers.Add(qualifier.GetCopy());
            }
            return cpy;
        }

    }
}
