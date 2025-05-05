/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// File: Constraint.cs
/////////////////////////////////////////////////////////////////////////////////////////

using System.Xml;

namespace CimContextor.Configuration
{
    public class Constraint
    {
        public static string XML_NAME = "constraint";
        private string name;
        private string type;
        private string allowedTo;
        private string notes;

        public Constraint(string name, string type, string allowedTo, string notes)
        {
            this.name = name;
            this.type = type;
            this.allowedTo = allowedTo;
            this.notes = notes;
        }

        public string Name 
        { 
            get { return name; }
            set { name = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string AllowedTo
        {
            get { return allowedTo; }
            set { allowedTo = value; }
        }

        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        public XmlNode GetXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement(XML_NAME);
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
            nameAttribute.Value = name;
            node.Attributes.Append(nameAttribute);

            XmlAttribute typeAttribute = xmlDoc.CreateAttribute("type");
            typeAttribute.Value = type;
            node.Attributes.Append(typeAttribute);

            XmlAttribute allowedToAttribute = xmlDoc.CreateAttribute("allowedTo");
            allowedToAttribute.Value = allowedTo;
            node.Attributes.Append(allowedToAttribute);

            XmlAttribute notesAttribute = xmlDoc.CreateAttribute("notes");
            notesAttribute.Value = notes;
            node.Attributes.Append(notesAttribute);

            return node;
        }

        
        public string GetAsString()
        {
            return "<" + XML_NAME + " name=\"" + name + "\" type=\"" + type + "\" allowedTo=\"" + allowedTo + "\" notes=\"" + notes + "\"/>";
        }

        public Constraint GetCopy()
        {
            return new Constraint(this.name, this.type, this.allowedTo, this.notes);
        }
    }
}
