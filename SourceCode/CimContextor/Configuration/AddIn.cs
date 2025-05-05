/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20230205
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CimContextor.Configuration
{
    public class AddIn
    {
        private AppSettings appSettings;
        private DataProfiles dataProfiles;
        private DataQualifier dataQualifier;
        private DataConstraint dataConstraint;
        private DataClassifierConstraint dataClassifierConstraint;
        public static string XML_NAME = "add-in";

        public AddIn()
        {
        }

        public AppSettings GetAppSettings()
        {
            return appSettings;
        }

        public void SetAppSettings(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public DataProfiles GetDataProfiles()
        {
            return dataProfiles;
        }

        public void SetDataProfiles(DataProfiles dataProfiles)
        {
            this.dataProfiles = dataProfiles;
        }

        public DataQualifier GetDataQualifier()
        {
            return dataQualifier;
        }

        public void SetDataQualifier(DataQualifier dataQualifier)
        {
            this.dataQualifier = dataQualifier;
        }

        public DataConstraint GetDataConstraint()
        {
            return dataConstraint;
        }

        public void SetDataConstraint(DataConstraint dataConstraint)
        {
            this.dataConstraint = dataConstraint;
        }

        public DataClassifierConstraint GetDataClassifierConstraint()
        {
            return dataClassifierConstraint;
        }

        public void SetDataClassifierConstraint(DataClassifierConstraint dataClassifierConstraint)
        {
            this.dataClassifierConstraint = dataClassifierConstraint;
        }


        public XmlNode GetXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement(XML_NAME);
            node.AppendChild(appSettings.GetXmlNode(xmlDoc));
            node.AppendChild(dataProfiles.GetXmlNode(xmlDoc));
            node.AppendChild(dataQualifier.GetXmlNode(xmlDoc));
            node.AppendChild(dataConstraint.GetXmlNode(xmlDoc));
            node.AppendChild(dataClassifierConstraint.GetXmlNode(xmlDoc));
            return node;
        }

        
        public string GetAsString()
        {
            string start = "<" + XML_NAME + ">\n";
            string end = "</" + XML_NAME + ">";
            string children = "";
            children += "\t" + appSettings.GetAsString() + "\n";
            children += "\t" + dataProfiles.GetAsString() + "\n";
            children += "\t" + dataQualifier.GetAsString() + "\n";
            children += "\t" + dataConstraint.GetAsString() + "\n";
            children += "\t" + dataClassifierConstraint.GetAtString() + "\n";

            return start + children + end;
        }

        public AddIn GetCopy()
        {
            AddIn cpy = new AddIn();
            cpy.appSettings = appSettings.GetCopy();
            cpy.dataProfiles = dataProfiles.GetCopy();
            cpy.dataQualifier = dataQualifier.GetCopy();
            cpy.dataConstraint = dataConstraint.GetCopy();
            cpy.dataClassifierConstraint = dataClassifierConstraint.GetCopy();
            return cpy;
        }

    }
}
