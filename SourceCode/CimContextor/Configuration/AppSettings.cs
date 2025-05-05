/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20230205
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CimContextor.Configuration
{
    public class AppSettings
    {
        public static string XML_NAME = "appSettings";
        private List<Configuration> configurations = new List<Configuration>();

        public AppSettings() { }

        public void AddConfiguration(Configuration configuration)
        {
            configurations.Add(configuration);
        }

        public List<Configuration> GetConfigurations() 
        {
            return configurations;
        }

        public XmlNode GetXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement(XML_NAME);
            foreach (Configuration configuration in configurations)
            {
                XmlNode configNode = configuration.GetXmlNode(xmlDoc);
                node.AppendChild(configNode);
            }

            return node;
        }

       
        public string GetAsString()
        {
            string start = "<" + XML_NAME + ">\n";
            string end = "\t</" + XML_NAME + ">";
            string children = "";
            foreach (Configuration configuration in configurations)
            {
                children += "\t\t" + configuration.GetAsString() + "\n";
            }
            return start + children + end;
        }

        public AppSettings GetCopy()
        {
            AppSettings cpy = new AppSettings();
            foreach(Configuration configuration in configurations)
            {
                cpy.AddConfiguration(configuration.GetCopy());
            }
            return cpy;
        }

    }
}
