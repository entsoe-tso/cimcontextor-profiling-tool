  using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Win32;
using CimContextor.Configuration;
using EA;
using CimContextor.Utilities;
/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.9.4                                         *  october 2019 *
*                                                                         *
***************************************************************************
*                                                                         *
*       Credit to:  Sebastien Maligue-Clausse                             *
*                   Andre Maizener   andre.maizener@zamiren.fr            *
*                   Jean-Luc Sanson  jean-luc.sanson@noos.fr              *
*                                                                         *
*       Contact: +33148854006                                             *
*                                                                         *
***************************************************************************

**************************************************************************/

namespace CimContextor
{
    public class XMLParser
    { 
        private static string CONFIG_FILE = @"\CimContextor-Config.xml";
        private static string LOG_FILE = @"\Log.xml";
        public string Path = "";        
        private XmlReader xmlReader = null;
        //private FileStream StreamFile = null;
        private string ActivatedFilePath;
        private XmlDocument confdoc;
        private Repository repo;
        public string ActualFilePath
        {
            get
            {
                return (ActivatedFilePath);
            }
        }

        public XMLParser(Repository repo)
        {
            /* ABA20230219
            string filepath= System.IO.Path.GetDirectoryName(Main.Repo.ConnectionString);//am aout 2018
            confdoc = new XmlDocument(); // am juil 2015
            
            if (File.Exists(filepath + CONFIG_FILE)) // if there is a config file in the database directory
                                                     // its path is take as Path or the directory is appdata
            {
                Path = filepath;
            }
            else
            {
                Path = Utilities.FileManager.GetParentDirPath(); // ABA20201020
            }
            */
            this.repo = repo;
            confdoc = new XmlDocument(); // am juil 2015
            Path = Utilities.FileManager.GetParentDirPath(); // ABA20201020
            string logFilePath = Path + LOG_FILE; 
            if (System.IO.File.Exists(logFilePath)) System.IO.File.Delete(logFilePath);
        }        

        private void setFilePath(string FilePath){
            if (FilePath.Equals("DefaultLog"))
            {
                ActivatedFilePath = Path + LOG_FILE;
            }
            else if (FilePath.Equals("DefaultConfig"))
            {
                ActivatedFilePath = Path + CONFIG_FILE;
                confdoc = new XmlDocument();
                confdoc.Load(ActivatedFilePath); // chargement du config
            }
            else { ActivatedFilePath = FilePath; }
        }

        public XmlReader ReadConfig()
        {
            try
            {
                /* ABA20230219
                StreamFile = new FileStream(ActivatedFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                XmlReader = new XmlTextReader(StreamFile);
                */
                ConfigurationManager.GetConfigurationManager(repo).LoadCustomConfiguration();
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
            }
            catch(IOException ex)
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_002, ex);
            }
            return xmlReader;
        }
 
        public void Close()
        {
            try
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
                /* ABA20230219
                if (StreamFile != null)
                {
                    StreamFile.Close();
                }
                */
            } catch(Exception) 
            { 
                // nothing to do ABA 20230215
            }
        }

        /* ABA20230219
        private void InitXMLConfig()
        {
            this.Close();
            XmlDocument NewXmlWriter = new XmlDocument();     
            XmlNode RootNode = NewXmlWriter.CreateNode(XmlNodeType.XmlDeclaration, "1.0", "UTF-8");
            NewXmlWriter.AppendChild(RootNode);
            XmlElement RootElement = NewXmlWriter.CreateElement("", "add-in", "");
            NewXmlWriter.AppendChild(RootElement);
            XmlElement tmpAppSet = NewXmlWriter.CreateElement("", "appSettings", "");
            XmlNode sectionNode = RootElement.AppendChild(tmpAppSet);
            XmlElement tmpIsBasedOn = NewXmlWriter.CreateElement("", "configuration", "");
            XmlAttribute AttributeXml = NewXmlWriter.CreateAttribute("name");
            AttributeXml.Value = "IsBasedOn";
            tmpIsBasedOn.Attributes.Append(AttributeXml);
            AttributeXml = NewXmlWriter.CreateAttribute("value");
            AttributeXml.Value = ConfigurationManager.UNCHECKED;
            tmpIsBasedOn.Attributes.Append(AttributeXml);
            sectionNode = tmpAppSet.AppendChild(tmpIsBasedOn);
            XmlElement TmpConfirm = NewXmlWriter.CreateElement("", "configuration", "");
            AttributeXml = NewXmlWriter.CreateAttribute("name");
            AttributeXml.Value = "Confirm";
            TmpConfirm.Attributes.Append(AttributeXml);
            AttributeXml = NewXmlWriter.CreateAttribute("value");
            AttributeXml.Value = ConfigurationManager.UNCHECKED;
            TmpConfirm.Attributes.Append(AttributeXml);
            sectionNode = tmpAppSet.AppendChild(TmpConfirm);
            XmlElement TmpLog = NewXmlWriter.CreateElement("", "configuration", "");
            AttributeXml = NewXmlWriter.CreateAttribute("name");
            AttributeXml.Value = "Log";
            TmpLog.Attributes.Append(AttributeXml);
            sectionNode = tmpAppSet.AppendChild(TmpLog);
            AttributeXml = NewXmlWriter.CreateAttribute("value");
            AttributeXml.Value = ConfigurationManager.UNCHECKED;
            TmpLog.Attributes.Append(AttributeXml);
            sectionNode = tmpAppSet.AppendChild(TmpLog);
            XmlElement TmpLogFolder = NewXmlWriter.CreateElement("", "LogFolder", "");
            AttributeXml = NewXmlWriter.CreateAttribute("name");
            AttributeXml.Value = "LogFolder";
            TmpLogFolder.Attributes.Append(AttributeXml);
            AttributeXml = NewXmlWriter.CreateAttribute("value");
            AttributeXml.Value = "C:/";
            TmpLogFolder.Attributes.Append(AttributeXml);
            sectionNode = tmpAppSet.AppendChild(TmpLogFolder);
            XmlElement Tmpelem = NewXmlWriter.CreateElement("", "data", "");
            sectionNode = RootElement.AppendChild(Tmpelem);
            NewXmlWriter.Save(ActivatedFilePath);
            XmlReader.Close();
        }
        */

        public string GetXmlValueConfig(string ElementName)
        {
            //this.setFilePath("DefaultConfig"); ABA20230220

            string Value = null;
            XmlReader XmlReader = null;

            try 
            { 
                XmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration())); 
            }
            catch(Exception ex) 
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_009, ex);
                /* ABA20230219
                this.InitXMLConfig();
                XmlReader = ReadConfig();
                */
            }

            try
            {
                while ( XmlReader.Read() )
                {
                    XmlNodeType NodeType = XmlReader.NodeType;
                    if( NodeType == XmlNodeType.Element )
                    {
                        if (XmlReader.Name.Equals("configuration") && XmlReader.GetAttribute("name").Equals(ElementName))
                        {
                            Value = XmlReader.GetAttribute("value");
                            XmlReader.Close();
                            return Value;
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_010, ex);
            }
            finally
            {
                if ( XmlReader != null )
                {
                    XmlReader.Close();
                }
            }
 
            return Value;
        }
                
        public void SetXmlValueConfig(string ElementName, string Value)
        {
            if(this.GetXmlValueConfig("Log") == ConfigurationManager.CHECKED) {
                this.AddXmlLog("Config Saved", "Element:"+ElementName +" Value:"+Value);
            }

            // ABA20230220 this.setFilePath("DefaultConfig");
            XmlReader xmlReader = null;
            XmlDocument NewXmlWriter = null;
                               
            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                while ( xmlReader.Read() )
                {                                       
                    if (xmlReader.Name.Equals("configuration") && xmlReader.GetAttribute("name").Equals(ElementName))
                    {
                        this.Close();
                        NewXmlWriter = new XmlDocument();
                        NewXmlWriter.LoadXml(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration());

                        XmlNode groupNode = NewXmlWriter.DocumentElement;
                        XmlNode sectionNode = groupNode.SelectSingleNode("appSettings");
 
                        if (sectionNode == null)
                        {
                            sectionNode = groupNode.AppendChild(NewXmlWriter.CreateElement("appSettings"));
                        }                                               
 
                        XmlNode NodeCherche = sectionNode.SelectSingleNode("configuration[@name=\"" + ElementName + "\"]");

                        XmlAttribute AttributeXml = null;
                        if ( NodeCherche == null )
                        {
                            XmlElement Element = NewXmlWriter.CreateElement("configuration");
                            AttributeXml = NewXmlWriter.CreateAttribute("name");
                            AttributeXml.Value = ElementName;
                            Element.Attributes.Append(AttributeXml);                        
                            NodeCherche = sectionNode.AppendChild(Element);                    
                        }

                        AttributeXml = NewXmlWriter.CreateAttribute("value");
                        AttributeXml.Value = Value.ToString();
                        NodeCherche.Attributes.Append(AttributeXml);
                        // save configuration                 
                        ConfigurationManager configManager = ConfigurationManager.GetConfigurationManager(repo);
                        configManager.UpdateConfiguration(NewXmlWriter);
                    }
                }
            }
            catch (XmlException ex)
            {
                if (this.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
                {
                    this.AddXmlLog("Error", "Error in XMLParser (SetXmlValueConfig) XMLException:" + ex.StackTrace);
                }
                ErrorCodes.ShowException(ErrorCodes.ERROR_011, ex);
            }
            catch (Exception ex)
            {
                this.AddXmlLog("Warning", "Error in XMLParser (SetXmlValueConfig)Exception:" + ex.StackTrace);
                ErrorCodes.ShowException(ErrorCodes.ERROR_012, ex);
            }
            finally
            {
                if ( xmlReader != null )
                {
                    xmlReader.Close();
                }
            }
        }

        //Quallifier
        public ArrayList GetXmlQualifier(string Allowed)
        {
            // ABA20230220 this.setFilePath("DefaultConfig");
            ArrayList ValueList = new ArrayList();
            XmlReader xmlReader = null;

            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                while (xmlReader.Read())
                {
                    XmlNodeType NodeType = xmlReader.NodeType;
                    if (NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name.Equals("qualifier") )
                        {
                            if (xmlReader.GetAttribute("allowedTo").Equals(Allowed) || xmlReader.GetAttribute("allowedTo").Equals("any"))
                            {
                                ValueList.Add(xmlReader.GetAttribute("name"));
                            }
                        }
                    }
                }
                xmlReader.Close();
                return ValueList;
            }
            catch (XmlException ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (GetXMLQualifier) XMLException:" + ex.StackTrace);
                ErrorCodes.ShowException(ErrorCodes.ERROR_013, ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }

            return ValueList;
        }

        public void AddXmlQualifier(string ElementName, string Allowed)
        {

            this.AddXmlLog("Adding Qualifier", "Added Qualifier:"+ElementName);

            // ABA20230220 this.setFilePath("DefaultConfig");
            XmlReader xmlReader = null;
            XmlDocument NewXmlWriter = null;
                    

            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));

                while (xmlReader.Read())
                {
                    if (xmlReader.Name.Equals("appSettings"))
                    {
                        this.Close();
                        NewXmlWriter = new XmlDocument();
                        NewXmlWriter.LoadXml(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration());

                        XmlElement RootElement = NewXmlWriter.DocumentElement;
                                

                        XmlAttribute AttributeXml = null;
                        XmlNode sectionNode = null;

                        XmlNode groupNode = RootElement;
                        sectionNode = groupNode.SelectSingleNode("dataQualifier");
                        if (sectionNode == null)
                        {
                            sectionNode = groupNode.AppendChild(NewXmlWriter.CreateElement("data"));
                        }

                        XmlNode SearchedNode = sectionNode.SelectSingleNode("qualifier[@name=\"" + ElementName + "\"]");

                        if (SearchedNode == null)
                        {
                            XmlElement Element = NewXmlWriter.CreateElement("qualifier");
                            AttributeXml = NewXmlWriter.CreateAttribute("name");
                            AttributeXml.Value = ElementName;
                            Element.Attributes.Append(AttributeXml);

                            AttributeXml = NewXmlWriter.CreateAttribute("allowedTo");
                            AttributeXml.Value = Allowed;
                            Element.Attributes.Append(AttributeXml);
                            SearchedNode = sectionNode.AppendChild(Element);
                        }
                        else{
                            sectionNode.RemoveChild(SearchedNode);
                            XmlElement Element = NewXmlWriter.CreateElement("qualifier");
                            AttributeXml = NewXmlWriter.CreateAttribute("name");
                            AttributeXml.Value = ElementName;
                            Element.Attributes.Append(AttributeXml);

                            AttributeXml = NewXmlWriter.CreateAttribute("allowedTo");
                            AttributeXml.Value = Allowed;
                            Element.Attributes.Append(AttributeXml);
                                    
                            SearchedNode = sectionNode.AppendChild(Element);
                        }

                        // save configuration                 
                        ConfigurationManager configManager = ConfigurationManager.GetConfigurationManager(repo);
                        configManager.UpdateConfiguration(NewXmlWriter);
                    }
                }
            }
            catch (XmlException Ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (AddXmlQualifier) XMLException:" + Ex);
                ErrorCodes.ShowException(ErrorCodes.ERROR_014, Ex);
            }
            catch (Exception Ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (AddXmlQualifier) Exception:" + Ex);
                ErrorCodes.ShowException(ErrorCodes.ERROR_015, Ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
        }

        public void DeleteXmlQualifier(string ElementName)
        {
            this.AddXmlLog("Deleting Qualifier", "Deleted Qualifier:"+ElementName);
            // ABA20230220 this.setFilePath("DefaultConfig");
            if (!ElementName.Equals(null)) {
                XmlReader xmlReader = null;
                XmlDocument NewXmlWriter = null;

                try
                {
                    xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                    while (xmlReader.Read())
                    {
                        if (xmlReader.Name.Equals("qualifier"))
                        {
                            this.Close();
                            NewXmlWriter = new XmlDocument();
                            NewXmlWriter.LoadXml(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration());
                            XmlElement ElementRacine = NewXmlWriter.DocumentElement;
                            XmlNode sectionNode = null;
                            XmlNode groupNode = ElementRacine;
                            sectionNode = groupNode.SelectSingleNode("dataQualifier");
                            XmlNode SearchedNode = sectionNode.SelectSingleNode("qualifier[@name=\"" + ElementName + "\"]");

                            if (!(SearchedNode==null))
                            {
                                sectionNode.RemoveChild(SearchedNode);
                            }
                            // save configuration                 
                            ConfigurationManager configManager = ConfigurationManager.GetConfigurationManager(repo);
                            configManager.UpdateConfiguration(NewXmlWriter);
                        }
                    }
                }
                    catch (XmlException Ex)
                    {
                        this.AddXmlLog("Warning!", "Error in XMLParser (DeleteXmlQualifier) XMLException:" + Ex);
                        ErrorCodes.ShowException(ErrorCodes.ERROR_016, Ex);
                    }
                    catch (Exception Ex)
                    {
                        this.AddXmlLog("Warning!", "Error in XMLParser (DeleteXmlQualifier) Exception:" + Ex);
                        ErrorCodes.ShowException(ErrorCodes.ERROR_017, Ex);
                }
                finally
                    {
                        if (xmlReader != null)
                        {
                            xmlReader.Close();
                        }
                    }
            }
        }

        //Log
        public void AddXmlLog(string EventType, string LogValue)
        {
            if (this.GetXmlValueConfig("Log") != ConfigurationManager.CHECKED)
            {
                return;
            }
            //eventtype must not have any whitespace
            EventType = EventType.Replace(" ","");
            //XmlReader xmlReader = null;
            XmlDocument NewXmlWriter = null;
            this.setFilePath("DefaultLog");

            try
            {//ErreurPar ICI
                /* ABA20230223
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                this.Close();
                */
                NewXmlWriter = new XmlDocument();
                XmlElement RootElement;
                try
                {
                        NewXmlWriter.Load(ActivatedFilePath);
                        RootElement = NewXmlWriter.DocumentElement;
                }
                catch
                {
                    XmlNode RootNode = NewXmlWriter.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                    NewXmlWriter.AppendChild(RootNode);
                    RootElement = NewXmlWriter.CreateElement("", "add-inLogs", "");
                    NewXmlWriter.AppendChild(RootElement);
                }

                XmlAttribute AttributeXml = null;
                XmlNode sectionNode = null;

                foreach( XmlNode ANode in RootElement.SelectNodes("logDate")) {
                    if (ANode.Attributes[0].Value.Equals(DateTime.Now.ToString("MM-dd-yyyy")))
                    {
                        sectionNode = ANode;
                    }
                }

                if (sectionNode == null)
                {
                    XmlElement tmpelem = NewXmlWriter.CreateElement("","logDate","");
                    AttributeXml = NewXmlWriter.CreateAttribute("date");
                    AttributeXml.Value = (DateTime.Now.ToString("MM-dd-yyyy"));
                    tmpelem.Attributes.Append(AttributeXml);
                    sectionNode = RootElement.AppendChild(tmpelem);
                }
                                
                XmlElement Element = NewXmlWriter.CreateElement("","log","");

                AttributeXml = NewXmlWriter.CreateAttribute("time");
                AttributeXml.Value = DateTime.Now.ToString("HH:mm:ss");
                Element.Attributes.Append(AttributeXml);
                                    
                AttributeXml = NewXmlWriter.CreateAttribute("username");
                AttributeXml.Value = WindowsIdentity.GetCurrent().Name.ToString();
                Element.Attributes.Append(AttributeXml);

                string evType = EventType.Replace("!", " ").Replace("$", " ").Replace("?", " ").Replace("\"", "");
                AttributeXml = NewXmlWriter.CreateAttribute(evType);
                AttributeXml.Value = LogValue;
                Element.Attributes.Append(AttributeXml);

                XmlNode NewNode = sectionNode.AppendChild(Element);
                NewNode.Attributes.Append(AttributeXml);
                NewXmlWriter.Save(ActivatedFilePath);
            }
            catch (XmlException Ex)
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_018, "EventType: "  + EventType, Ex);
            }
            catch (Exception Ex)
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_019, Ex);
            }
            /* ABA20230223
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
            */
        }

        //Constraint
        public ArrayList GetXmlConstraint(string Allowed)
        {
            // ABA20230220 this.setFilePath("DefaultConfig");
            ArrayList ValueList = new ArrayList();
            XmlReader xmlReader = null;

            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                while (xmlReader.Read())
                {
                    XmlNodeType NodeType = xmlReader.NodeType;
                            
                    if (NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name.Equals("constraint"))
                        {
                            if (xmlReader.GetAttribute("allowedTo").ToLower().Equals(Allowed.ToLower()) || xmlReader.GetAttribute("allowedTo").ToLower().Equals("any"))
                            {
                            ValueList.Add( new XMLConstraint(xmlReader.GetAttribute("name"), xmlReader.GetAttribute("type"), xmlReader.GetAttribute("notes")) );
                            }
                        }
                    }
                }
                xmlReader.Close();
                return ValueList;
            }
            catch (XmlException Ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (GetXMLConstraint) XMLException:" + Ex);
                ErrorCodes.ShowException(ErrorCodes.ERROR_020, Ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }

            return ValueList;
        }

        public ArrayList GetXmlClassifierConstraint(string Allowed)
        {
            // ABA20230220 this.setFilePath("DefaultConfig");
            ArrayList ValueList = new ArrayList();
            XmlReader xmlReader = null;

            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                while (xmlReader.Read())
                {
                    XmlNodeType NodeType = xmlReader.NodeType;
                    if (NodeType == XmlNodeType.Element)
                    {
                        //MessageBox.Show("element name=" + XmlReader.GetAttribute("name") + ",allowedTo=" + XmlReader.GetAttribute("allowedTo")); // pour test
                        if (xmlReader.Name.Equals("classifierconstraint"))
                        {
                            // MessageBox.Show("contrainte name=" + XmlReader.GetAttribute("name") + ",allowedTo=" + XmlReader.GetAttribute("allowedTo")); // pour test
                            if (xmlReader.GetAttribute("allowedTo").ToLower().Equals(Allowed.ToLower()) || xmlReader.GetAttribute("allowedTo").ToLower().Equals("any".ToLower()))
                            {
                                try
                                {

                                    ValueList.Add(new XMLClassifierConstraint(xmlReader.GetAttribute("name"), xmlReader.GetAttribute("type"), xmlReader.GetAttribute("variableType"), xmlReader.GetAttribute("variableList"), xmlReader.GetAttribute("comment"), xmlReader.ReadString()));
                                }
                                catch(Exception Ex)
                                {
                                    MessageBox.Show("Error while recovering the XML data of " + xmlReader.GetAttribute("name") + "\nThe constraint list will probably be corrupted, you should close this window and check the config file. \n" +Ex.Message , "Error while recovering XML data", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                                }
                            }
                        }
                    }
                }
                xmlReader.Close();
                return ValueList;
            }
            catch (XmlException Ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (GetXMLConstraint) XMLException:" + Ex);
                ErrorCodes.ShowException(ErrorCodes.ERROR_021, Ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }

            return ValueList;
        }

        /// <summary>
        /// for WG16 purpose
        /// </summary>
        /// <returns></returns>
        public List<string>  GetXmlClassifierConstraintNames() 
        {
            // ABA20230220 this.setFilePath("DefaultConfig");
            List<string> ValueList = new List<string>();
            XmlReader xmlReader = null;

            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                while (xmlReader.Read())
                {
                    XmlNodeType NodeType = xmlReader.NodeType;
                    if (NodeType == XmlNodeType.Element)
                    {
                        //MessageBox.Show("contrainte name=" + xmlReader.GetAttribute("name") + ",allowedTo=" + xmlReader.GetAttribute("allowedTo")); // pour test
                        if (xmlReader.Name.Equals("classifierconstraint"))
                        {
                                try
                                {
                                    if(!ValueList.Contains(xmlReader.GetAttribute("name"))) ValueList.Add(xmlReader.GetAttribute("name"));
                                }
                                catch (Exception Ex)
                                {
                                    MessageBox.Show("Error while recovering the XML data of " + xmlReader.GetAttribute("name") + "\nThe constraint list will probably be corrupted, you should close this window and check the config file. \n" + Ex.Message, "Error while recovering XML data", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                                }
                        }
                    }
                }
                xmlReader.Close();
                return ValueList;
            }
            catch (XmlException Ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (GetXMLConstraint) XMLException:" + Ex);
                ErrorCodes.ShowException(ErrorCodes.ERROR_022, Ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }

            return ValueList;
        }

        public void DeleteXmlConstraint(string ElementName)
        {
            this.AddXmlLog("Deleting Constraint", "Deleted Constraint:" + ElementName);
            // ABA20230220 this.setFilePath("DefaultConfig");
            if (!ElementName.Equals(null))
            {
                XmlReader xmlReader = null;
                XmlDocument NewXmlWriter = null;

                try
                {
                    xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));

                    while (xmlReader.Read())
                    {
                        if (xmlReader.Name.Equals("constraint"))
                        {
                            this.Close();
                            NewXmlWriter = new XmlDocument();
                            NewXmlWriter.LoadXml(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration());
                            XmlElement ElementRacine = NewXmlWriter.DocumentElement;
                            XmlNode sectionNode = null;
                            XmlNode groupNode = ElementRacine;
                            sectionNode = groupNode.SelectSingleNode("dataConstraint");
                            XmlNode SearchedNode = sectionNode.SelectSingleNode("constraint[@name=\"" + ElementName + "\"]");

                            if (!(SearchedNode == null))
                            {
                                sectionNode.RemoveChild(SearchedNode);
                            }
                            // save configuration                 
                            ConfigurationManager configManager = ConfigurationManager.GetConfigurationManager(repo);
                            configManager.UpdateConfiguration(NewXmlWriter);
                        }
                    }
                }
                catch (XmlException Ex)
                {
                    this.AddXmlLog("Warning!", "Error in XMLParser (DeleteXmlConstraint) XMLException:" + Ex);
                    MessageBox.Show("Unexpected XML error(n°3).\n" + Ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                catch (Exception Ex)
                {
                    this.AddXmlLog("Warning!", "Error in XMLParser (DeleteXmlConstraint) Exception:" + Ex);
                    ErrorCodes.ShowException(ErrorCodes.ERROR_023, Ex);
                }
                finally
                {
                    if (xmlReader != null)
                    {
                        xmlReader.Close();
                    }
                }
            }
        }

        public void  AddXmlConstraint(string ElementName, string notes, string type , string Allowed)
        {
            this.AddXmlLog("Adding Constraint", "Added Constraint:" + ElementName);
            // ABA20230220 this.setFilePath("DefaultConfig");
            XmlReader xmlReader = null;
            XmlDocument NewXmlWriter = null;

            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));

                while (xmlReader.Read())
                {
                    if (xmlReader.Name.Equals("appSettings"))
                    {
                        this.Close();
                        NewXmlWriter = new XmlDocument();
                        NewXmlWriter.LoadXml(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration());
                        XmlElement RootElement = NewXmlWriter.DocumentElement;
                        XmlAttribute AttributeXml = null;
                        XmlNode sectionNode = null;
                        XmlNode groupNode = RootElement;
                        sectionNode = groupNode.SelectSingleNode("dataConstraint");
                        if (sectionNode == null)
                        {
                            sectionNode = groupNode.AppendChild(NewXmlWriter.CreateElement("dataConstraint"));
                        }

                        XmlNode SearchedNode = sectionNode.SelectSingleNode("Constraint[@name=\"" + ElementName + "\"]");

                        if (SearchedNode == null)
                        {
                            XmlElement Element = NewXmlWriter.CreateElement("Constraint");
                            AttributeXml = NewXmlWriter.CreateAttribute("name");
                            AttributeXml.Value = ElementName;
                            Element.Attributes.Append(AttributeXml);

                            AttributeXml = NewXmlWriter.CreateAttribute("type");
                            AttributeXml.Value = type;
                            Element.Attributes.Append(AttributeXml);

                            AttributeXml = NewXmlWriter.CreateAttribute("allowedTo");
                            AttributeXml.Value = Allowed;
                            Element.Attributes.Append(AttributeXml);

                            AttributeXml = NewXmlWriter.CreateAttribute("notes");
                            AttributeXml.Value = notes;
                            Element.Attributes.Append(AttributeXml);

                            SearchedNode = sectionNode.AppendChild(Element);
                        }
                        else
                        {
                            sectionNode.RemoveChild(SearchedNode);
                            XmlElement Element = NewXmlWriter.CreateElement("Constraint");
                            AttributeXml = NewXmlWriter.CreateAttribute("name");
                            AttributeXml.Value = ElementName;
                            Element.Attributes.Append(AttributeXml);

                            AttributeXml = NewXmlWriter.CreateAttribute("type");
                            AttributeXml.Value = type;
                            Element.Attributes.Append(AttributeXml);

                            AttributeXml = NewXmlWriter.CreateAttribute("allowedTo");
                            AttributeXml.Value = Allowed;
                            Element.Attributes.Append(AttributeXml);

                            AttributeXml = NewXmlWriter.CreateAttribute("notes");
                            AttributeXml.Value = notes;
                            Element.Attributes.Append(AttributeXml);

                            SearchedNode = sectionNode.AppendChild(Element);
                        }
                        // save configuration                 
                        ConfigurationManager configManager = ConfigurationManager.GetConfigurationManager(repo);
                        configManager.UpdateConfiguration(NewXmlWriter);
                    }
                }
            }
            catch (XmlException Ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (AddXmlConstraint) XMLException:" + Ex);
                MessageBox.Show("Unexpected XML error(n°3).\n" + Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            catch (Exception Ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (AddXmlConstraint) Exception:" + Ex);
                ErrorCodes.ShowException(ErrorCodes.ERROR_024, Ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
        }


        /// <summary>
        /// recupere une contraite de nom donne
        /// </summary>
        /// <param name="Name"></param>
        /// <returns> un dictionaire T
        /// T[type]=type
        /// T[allwedTo]=allowedto
        /// T[notes]=notes
        /// </returns>
        public XMLConstraint GetXmlNameConstraint(string name)
        {
            // ABA20230220 .setFilePath("DefaultConfig");
            XMLConstraint ValueList = null;
            XmlReader xmlReader = null;

            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                while (xmlReader.Read())
                {
                    XmlNodeType NodeType = xmlReader.NodeType;

                    if (NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name.Equals("constraint"))
                        {
                            if (xmlReader.GetAttribute("name").Equals(name))
                            {
                                ValueList=new XMLConstraint(name,xmlReader.GetAttribute("type"),xmlReader.GetAttribute("notes"));
                            }
                        }
                    }
                }
                xmlReader.Close();
                return ValueList;
            }
            catch (XmlException Ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (GetXMLConstraint) XMLException:" + Ex);
                ErrorCodes.ShowException(ErrorCodes.ERROR_025, Ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }

            return ValueList;
        }

        public List<string> GetXmlAllProfstereo()
        {
            // ABA20230220 this.setFilePath("DefaultConfig");
            List<string> ValueList = new List<string>();
            XmlReader xmlReader = null;
            string stereo = "";

            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                while (xmlReader.Read())
                {
                    XmlNodeType NodeType = xmlReader.NodeType;
                    if (NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name.Equals("profstereo"))
                        {
                            stereo = xmlReader.GetAttribute("name");
                            ValueList.Add(stereo);
                        }
                    }
                }
                xmlReader.Close();
                return ValueList;
            }
            catch (XmlException Ex)
            {
                if (xmlReader != null) //am juil 2015
                {
                    xmlReader.Close();
                }
                this.AddXmlLog("Warning!", "Error in XMLParser (GetXMLQualifier) XMLException:" + Ex);
                ErrorCodes.ShowException(ErrorCodes.ERROR_026, Ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }

            return ValueList;
        }

        /* ABA20230401
        public string oldGetXmlValueProfData(string ElementName)
        {
            string ret = null;
            try
            {
                // ABA20230220 this.setFilePath("DefaultConfig");
                XmlNode elnode = confdoc.SelectSingleNode("//profdata[@name=\"" + ElementName + "\"]");
                if (elnode == null) return ret;
                System.Xml.XPath.XPathNavigator elnav = elnode.CreateNavigator();
                ret = elnav.GetAttribute("value", "");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error   in getting profdatavalue " + ElementName + "," + e.Message);
            }
            return ret;
        }
        */

        public string GetXmlValueProfData(string ElementName)
        {
            string ret = null;
            try
            {
                // ABA20230220 this.setFilePath("DefaultConfig");
                XmlNode elnode = confdoc.SelectSingleNode("//profdata[@name=\"" + ElementName + "\"]");
                if (elnode == null) return ret;
                System.Xml.XPath.XPathNavigator elnav = elnode.CreateNavigator();
                ret = elnav.GetAttribute("value", "");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error   in getting profdatavalue " + ElementName + "," + e.Message);
            }
            return ret;
        }
        public void SetXmlValueProfData(string ElementName, string Value)
        {
            if (this.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
            {
                this.AddXmlLog("Config Saved", "Element:" + ElementName + " Value:" + Value);
            }

            // ABA20230220 this.setFilePath("DefaultConfig");
            XmlReader xmlReader = null;
            XmlDocument NewXmlWriter = null;

            try
            {
                xmlReader = XmlReader.Create(new StringReader(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration()));
                while (xmlReader.Read())
                {
                    if (xmlReader.Name.Equals("profdata") && xmlReader.GetAttribute("name").Equals(ElementName))
                    {
                        this.Close();
                        NewXmlWriter = new XmlDocument();
                        NewXmlWriter.LoadXml(ConfigurationManager.GetConfigurationManager(repo).GetCustomConfiguration());
                        XmlElement ElementRacine = NewXmlWriter.DocumentElement;
                        XmlAttribute AttributeXml = null;
                        XmlNode sectionNode = null;
                        XmlNode groupNode = ElementRacine;
                        sectionNode = groupNode.SelectSingleNode("dataProfiles");

                        if (sectionNode == null)
                        {
                            sectionNode = groupNode.AppendChild(NewXmlWriter.CreateElement("dataProfiles"));
                        }

                        XmlNode NodeCherche = sectionNode.SelectSingleNode("profdata[@name=\"" + ElementName + "\"]");

                        if (NodeCherche == null)
                        {
                            XmlElement Element = NewXmlWriter.CreateElement("profdata");
                            AttributeXml = NewXmlWriter.CreateAttribute("name");
                            AttributeXml.Value = ElementName;
                            Element.Attributes.Append(AttributeXml);
                            NodeCherche = sectionNode.AppendChild(Element);
                        }

                        AttributeXml = NewXmlWriter.CreateAttribute("value");
                        AttributeXml.Value = Value.ToString();
                        NodeCherche.Attributes.Append(AttributeXml);
                        // save configuration                 
                        ConfigurationManager configManager = ConfigurationManager.GetConfigurationManager(repo);
                        configManager.UpdateConfiguration(NewXmlWriter);
                    }
                }
            }
            catch (XmlException Ex)
            {
                if (this.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
                {
                    this.AddXmlLog("Warning!", "Error in XMLParser (SetXmlValueConfig) XMLException:" + Ex);
                }
                MessageBox.Show("Unexpected error(n°3).\n" + Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            catch (Exception Ex)
            {
                this.AddXmlLog("Warning!", "Error in XMLParser (SetXmlValueProfdata)Exception:" + Ex);
                ErrorCodes.ShowException(ErrorCodes.ERROR_027, Ex);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
        }
    }
}
