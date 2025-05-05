/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// File: Qualifier.cs
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CimContextor.Configuration
{
    public class Qualifier
    {
        public static string XML_NAME = "qualifier";
        private string name;
        private string allowedTo;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string AllowedTo
        {
            get { return allowedTo; }
            set { allowedTo = value; }
        }

        public Qualifier(string name, string allowedTo)
        {
            this.name = name;
            this.allowedTo = allowedTo;
        }

        public XmlNode GetXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement(XML_NAME);
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
            nameAttribute.Value = name;
            node.Attributes.Append(nameAttribute);

            XmlAttribute allowedToAttribute = xmlDoc.CreateAttribute("allowedTo");
            allowedToAttribute.Value = allowedTo;
            node.Attributes.Append(allowedToAttribute);

            return node;
        }

        
        public string GetAsString()
        {
            return "<" + XML_NAME + " name=\"" + name + "\" allowedTo=\"" + allowedTo + "\"/>";
        }

        public Qualifier GetCopy()
        {
            return new Qualifier(this.name, this.allowedTo);
        }
    }
}
