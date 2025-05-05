/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20230205
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CimContextor.Configuration
{
    public class DataClassifierConstraint
    {
        public static string XML_NAME = "dataClassifierConstraint";
        private List<ClassifierConstraint> classifierConstraints = new List<ClassifierConstraint>();

        public DataClassifierConstraint() { }   

        public void AddClassifierConstraint(ClassifierConstraint constraint)
        {
            classifierConstraints.Add(constraint);
        }

        public List<ClassifierConstraint> GetClassifierConstraints() { return classifierConstraints; }

        public XmlNode GetXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement(XML_NAME);
            foreach (ClassifierConstraint classifierConstraint in classifierConstraints)
            {
                XmlNode classifierConstraintNode = classifierConstraint.GetXmlNode(xmlDoc);
                node.AppendChild(classifierConstraintNode);
            }

            return node;
        }

        
        public string GetAtString()
        {
            string start = "<" + XML_NAME + ">\n";
            string end = "\t</" + XML_NAME + ">";
            string children = "";
            foreach (ClassifierConstraint classifierConstraint in classifierConstraints)
            {
                children += "\t\t" + classifierConstraint.GetAsString() + "\n";
            }
            return start + children + end;
        }

        public DataClassifierConstraint GetCopy()
        {
            DataClassifierConstraint cpy = new DataClassifierConstraint();
            foreach(ClassifierConstraint classifierConstraint in classifierConstraints)
            {
                cpy.AddClassifierConstraint(classifierConstraint.GetCopy());
            }
            return cpy;
        }
    }
}
