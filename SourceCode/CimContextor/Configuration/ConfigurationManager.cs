/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// File: ConfigurationManager.cs
/////////////////////////////////////////////////////////////////////////////////////////


using CimContextor.Utilities;
using EA;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace CimContextor.Configuration
{
    public class ConfigurationManager
    {
        public static readonly string CONFIG_PACKAGE = ".Configuration";
        public static readonly string CONFIG_ELEMENT = "CimConteXtor";
        public static readonly string CONFIG_TAGGGEDVALUE = "configuration";
        private AddIn addIn = null;
        private EA.Package theModel = null;
        private readonly EA.Repository repo = null;
        private static ConfigurationManager configurationManager = null;
        private string customConfiguration = null;
        public static readonly string CHECKED = "Checked";
        public static readonly string UNCHECKED = "Unchecked";

        private ConfigurationManager(EA.Repository repository)
        {
            repo = repository;
            SetModel();
        }

        public static ConfigurationManager GetConfigurationManager(EA.Repository repo) // access to singleton
        {
            if(configurationManager == null)
            {
                configurationManager = new ConfigurationManager(repo);
                configurationManager.StartUp();
            }
            return configurationManager;
        }

        public void DefineStereotypes()
        {
            EA.Stereotype stereotype;
            string name = "Enumeration";
            if (!ExistsStereotype(name) && !ExistsStereotype(name.ToLower()))
            {
                stereotype = (EA.Stereotype)repo.Stereotypes.AddNew(name, "Class");
                stereotype.Notes = "CIM Profile Stereotype";
                stereotype.Update();
                repo.Stereotypes.Refresh();
            }

            name = "Compound";
            if (!ExistsStereotype(name))
            {
                stereotype = (EA.Stereotype)repo.Stereotypes.AddNew(name, "Class");
                stereotype.Notes = "CIM Profile Stereotype";
                stereotype.Update();
                repo.Stereotypes.Refresh();
            }

            name = "CIMDatatype";
            if (!ExistsStereotype(name))
            {
                stereotype = (EA.Stereotype)repo.Stereotypes.AddNew(name, "Class");
                stereotype.Notes = "CIM Profile Stereotype";
                stereotype.Update();
                repo.Stereotypes.Refresh();
            }

            name = "Primitive";
            if (!ExistsStereotype(name))
            {
                stereotype = (EA.Stereotype)repo.Stereotypes.AddNew(name, "Class");
                stereotype.Notes = "CIM Profile Stereotype";
                stereotype.Update();
                repo.Stereotypes.Refresh();
            }

            name = "IsBasedOn";
            if (!ExistsStereotype(name))
            {
                stereotype = (EA.Stereotype)repo.Stereotypes.AddNew(name, "Dependency");
                stereotype.Notes = "CIM Profile Stereotype";
                stereotype.Update();
                repo.Stereotypes.Refresh();
            }

            name = "Description";
            if (!ExistsStereotype(name) && !ExistsStereotype(name.ToLower()))
            {
                stereotype = (EA.Stereotype)repo.Stereotypes.AddNew(name, "Class");
                stereotype.Notes = "CIM Profile Stereotype";
                stereotype.Update();
                repo.Stereotypes.Refresh();
            }
        }

        private bool ExistsStereotype(string name)
        {
            foreach(EA.Stereotype stereotype in repo.Stereotypes)
            {
                if(stereotype.Name.Equals(name)) return true;
            }
            return false;
        }

        private void SetModel()
        {
            EA.Collection models = repo.Models;
            try
            {
                if (models.Count > 1) 
                {   // There are several. Let's try to find the one with the configuration  package. 
                    // If there is no configuration package, then take the first model.
                    foreach (EA.Package model in models)
                    {
                        EA.Package pack = GetConfigPackage(model);
                        if (pack != null)
                        {
                            theModel = (EA.Package)model;
                            return;
                        }
                    }
                }
                theModel = (EA.Package)models.GetAt(0);
            }
            catch (Exception e)
            {
                MessageBox.Show(ErrorCodes.ERROR_003 + ": " + ErrorCodes.ERROR_003[1] + "\n" + e.Message);
            }
        }

        public string StartUp()
        {
            try
            {
                // check whether there's a custom configuration 
                string configXml = this.LoadCustomConfiguration();
                if (configXml != null)
                {
                    this.customConfiguration = configXml;
                } else
                {
                    // there's no custom configuration, hence take a copy of the default:
                    this.addIn = DefaultConfiguration.GetDefaultConfiguration().GetAddIn();
                    this.customConfiguration = "<?xml version=\"1.0\"?>" + "\n" + addIn.GetAsString();
                }
            } 
            catch(Exception e)
            {
                MessageBox.Show(ErrorCodes.ERROR_004[0] + ": " + ErrorCodes.ERROR_004[1] + "\n" + e.StackTrace);
            }
            return this.customConfiguration;
        }

        public string GetCustomConfiguration()
        {
            return customConfiguration;
        }

        public string LoadCustomConfiguration()
        {
            this.theModel.Packages.Refresh(); // in case of someone deleted the custom configuration manually
            EA.Package configPkg = GetConfigPackage(theModel);
            if (configPkg != null)
            {
                EA.Element configElem = GetExistingConfigElement(configPkg);
                if (configElem != null)
                {
                    EA.TaggedValue tv = GetExistingConfigTaggedValue(configElem) ?? CreateConfigTaggedValue(configElem);
                    string configXml = tv.Notes;
                    if (configXml != null && configXml.Length > 0)
                    {
                        this.customConfiguration = configXml.Trim(); 
                        return this.customConfiguration;
                    }
                }
            }
            // there's no custom configuration, hence take a copy of the default:
            this.addIn = DefaultConfiguration.GetDefaultConfiguration().GetAddIn();
            this.customConfiguration = "<?xml version=\"1.0\"?>" + "\n" + addIn.GetAsString();
            return this.customConfiguration;
        }

        public void LoadDefaultConfiguration()
        {
            this.addIn = DefaultConfiguration.GetDefaultConfiguration().GetAddIn();
            this.customConfiguration = "<?xml version=\"1.0\"?>" + "\n" + addIn.GetAsString();
        }


        public EA.Element GetConfigElement()
        {
            EA.Package configPkg = GetConfigPackage(theModel) ?? this.CreateConfigPackage(theModel);
            EA.Element configElem = GetExistingConfigElement(configPkg) ?? CreateConfigElement(configPkg);
            return configElem;
        }

        private EA.Package CreateConfigPackage(EA.Package theModel)
        {
            EA.Package pkg = (EA.Package)theModel.Packages.AddNew(CONFIG_PACKAGE, "Nothing");
            pkg.Update();
            theModel.Packages.Refresh();
            return pkg;
        }

        private EA.Element CreateConfigElement(EA.Package pkg)
        {
            EA.Element elem = (EA.Element)pkg.Elements.AddNew(CONFIG_ELEMENT, "Class");
            elem.Update();
            pkg.Elements.Refresh();
            return elem;
        }

        public EA.TaggedValue CreateConfigTaggedValue(EA.Element elem)
        {
            EA.TaggedValue tv = (EA.TaggedValue)elem.TaggedValues.AddNew(CONFIG_TAGGGEDVALUE, "");
            tv.Value = "<memo>";
            tv.Notes = "";
            tv.Update();
            elem.TaggedValues.Refresh();
            return tv;
        }

        public EA.TaggedValue GetExistingConfigTaggedValue(EA.Element elem)
        {
            EA.Collection tags = elem.TaggedValues;
            for (short i = 0; i < tags.Count; i++)
            {
                EA.TaggedValue tv = (EA.TaggedValue)tags.GetAt(i);
                if (tv.Name.Equals(CONFIG_TAGGGEDVALUE))
                {
                    return tv;
                }
            }
            return null;
        }

        private EA.Package GetConfigPackage(EA.Package theModel)
        {
            EA.Collection pkgColl = theModel.Packages;
            for (short i = 0; i < pkgColl.Count; i++) // foreach is unstable here!
            {
                EA.Package pack = (EA.Package)pkgColl.GetAt(i);
                if (pack.Name.Equals(CONFIG_PACKAGE))
                {
                    return pack;
                }
            }
            return null;
        }

        private EA.Element GetExistingConfigElement(EA.Package pkg)
        {
            foreach (EA.Element element in pkg.Elements)
            {
                if (element.Name.Equals(CONFIG_ELEMENT))
                {
                    return element;
                }
            }
            return null;
        }

        public void ExportConfiguration()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = @"C:\",
                DefaultExt = "xml",
                Filter = @"XML (*.xml)|*.xml",
                FilterIndex = 1,
                Title = @"Save Configuration File"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog.FileName != "")
                {

                    Stream fileStream;
                    if ((fileStream = saveFileDialog.OpenFile()) != null)
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration());
                        fileStream.Write(info, 0, info.Length);
                        fileStream.Close();
                    }
                }
                saveFileDialog.Dispose();                    
            } 
            else // ABA20230504
            {
                saveFileDialog.Dispose ();
            }
        }
        
        public void ImportConfiguration()
        {
            ImportConfigurationDialog importConfigDialog = new ImportConfigurationDialog(repo);
            importConfigDialog.ShowDialog();
            importConfigDialog.Dispose();
        }

        public string ValidateConfigXML(string xmlString, string xsdSting)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);
            XmlReader xmlReader = XmlReader.Create(new StringReader(xsdSting));
            xml.Schemas.Add(null, xmlReader);

            try
            {
                xml.Validate(null);
            }
            catch (XmlSchemaValidationException ex)
            {
                return ex.Message;
            }
            return null;
        }

        public static string AsString(XmlDocument xmlDoc)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter tx = new XmlTextWriter(sw))
                {
                    tx.Formatting = Formatting.Indented;
                    xmlDoc.WriteContentTo(tx);
                    tx.Flush();
                    string strXmlText = sw.ToString();
                    return strXmlText;
                }
            }
        }

        public void UpdateConfiguration(XmlDocument xmlDoc)
        {
            this.theModel.Packages.Refresh();
            ConfigurationManager configManager = ConfigurationManager.GetConfigurationManager(repo);
            EA.Element configElem = configManager.GetConfigElement();
            EA.TaggedValue tv = configManager.GetExistingConfigTaggedValue(configElem) ?? configManager.CreateConfigTaggedValue(configElem);
            tv.Value = "<memo>";
            tv.Notes = AsString(xmlDoc);
            tv.Update();
            configElem.TaggedValues.Refresh();
            configManager.LoadCustomConfiguration();
        }

    }
}
