/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20230205
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CimContextor.Configuration
{
    public class DataProfiles
    {
        public static string XML_NAME = "dataProfiles";
        private List<Profdata> profdatas= new List<Profdata>();

        public DataProfiles() { }

        public void AddProfData(Profdata profdata)
        {
            profdatas.Add(profdata);
        }

        public List<Profdata> GetProfData() 
        { 
            return profdatas; 
        }

        public XmlNode GetXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement(XML_NAME);
            foreach (Profdata profdata in profdatas)
            {
                XmlNode profdataNode = profdata.GetXmlNode(xmlDoc);
                node.AppendChild(profdataNode);
            }

            return node;
        }

       
        public string GetAsString()
        {
            string start = "<" + XML_NAME + ">\n";
            string end = "\t</" + XML_NAME + ">";
            string children = "";
            foreach (Profdata profdata in profdatas)
            {
                children += "\t\t" + profdata.GetAsString() + "\n";
            }
            return start + children + end;
        }

        public DataProfiles GetCopy()
        {
            DataProfiles cpy = new DataProfiles();
            foreach(Profdata profdata in profdatas)
            {
                cpy.AddProfData(profdata.GetCopy());
            }
            return cpy;
        }

    }
}
