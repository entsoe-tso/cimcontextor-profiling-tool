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
    public class DataConstraint
    {
        public static string XML_NAME = "dataConstraint";
        private List<Constraint> constraints = new List<Constraint>();

        public DataConstraint() { }

        public void AddConstraint(Constraint constraint) { constraints.Add(constraint);}

        public List<Constraint> GetConstraints() { return constraints;}

        public XmlNode GetXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement(XML_NAME);
            foreach (Constraint constraint in constraints)
            {
                XmlNode constraintNode = constraint.GetXmlNode(xmlDoc);
                node.AppendChild(constraintNode);
            }

            return node;
        }

       
        public string GetAsString()
        {
            string start = "<" + XML_NAME + ">\n";
            string end = "\t</" + XML_NAME + ">";
            string children = "";
            foreach (Constraint constraint in constraints)
            {
                children += "\t\t" + constraint.GetAsString() + "\n";
            }
            return start + children + end;
        }

        public DataConstraint GetCopy()
        {
            DataConstraint cpy = new DataConstraint();
            foreach(Constraint constraint in constraints)
            {
                cpy.constraints.Add(constraint.GetCopy());
            }
            return cpy;
        }
    }
}
