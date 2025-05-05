/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// File: Configuration.cs
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CimContextor.Configuration
{
    public class Configuration
    {
        public static string XML_NAME = "configuration";
        private string name;
        private string value;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public Configuration(string name, string value) 
        { 
            this.name = name;
            this.value = value;
        }

        public XmlNode GetXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement(XML_NAME);
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
            nameAttribute.Value = name;
            node.Attributes.Append(nameAttribute);

            XmlAttribute valueAttribute = xmlDoc.CreateAttribute("value");
            valueAttribute.Value = value;
            node.Attributes.Append(valueAttribute);

            return node;
        }

        
        public string GetAsString()
        {
            return "<" + XML_NAME + " name=\"" + name + "\" value=\"" + value + "\"/>";
        }

        public Configuration GetCopy()
        {
            return new Configuration(this.name, this.value);
        }
    }
}
