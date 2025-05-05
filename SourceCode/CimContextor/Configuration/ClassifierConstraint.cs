/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// File: Classifierconstraint.cs
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CimContextor.Configuration
{
    public class ClassifierConstraint
    {
        public static string XML_NAME = "classifierconstraint";
        private string name;
        private string type;
        private string allowedTo;
        private string variableType;
        private string variableList;
        private string comment;
        private string content;

        public ClassifierConstraint(string name, string type, string allowedTo, string variableType, string variableList, string comment, string content)
        {
            this.name = name;
            this.type = type;
            this.allowedTo = allowedTo;
            this.variableType = variableType;
            this.variableList = variableList;
            this.comment = comment;
            this.content = content;
        }   

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
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
            set { allowedTo= value; }
        }

        public string VariableType
        {
            get { return variableType; }
            set { variableType = value; }
        }

        public string VariableList
        {
            get { return variableList; } 
            set { variableList= value; }
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

            XmlAttribute variableTypeAttribute = xmlDoc.CreateAttribute("variableType");
            variableTypeAttribute.Value = variableType;
            node.Attributes.Append(variableTypeAttribute);

            XmlAttribute variableListAttribute = xmlDoc.CreateAttribute("variableList");
            variableListAttribute.Value = variableList;
            node.Attributes.Append(variableListAttribute);

            XmlAttribute commentAttribute = xmlDoc.CreateAttribute("comment");
            commentAttribute.Value = comment;
            node.Attributes.Append(commentAttribute);

            node.InnerText = content;
            return node;
        }

        
        public string GetAsString()
        {
            string start = "<" + XML_NAME + " name=\"" + name + "\" type=\"" + type + "\" allowedTo=\"" + allowedTo + "\" variableType=\"" + variableType + "\" variableList=\"" + variableList + "\" comment=\"" + comment + "\">";
            string end = "</" + XML_NAME + ">";
            return start + content + end;
        }

        public ClassifierConstraint GetCopy()
        {
            return new ClassifierConstraint(this.name, this.type, this.allowedTo, this.variableType, this.variableList, this.comment, this.content);
        }
    }
}
