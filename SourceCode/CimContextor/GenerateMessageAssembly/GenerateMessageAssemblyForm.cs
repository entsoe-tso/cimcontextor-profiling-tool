using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Xml.Schema;
using System.Xml;

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
    public partial class GenerateMessageAssemblyForm : Form
    {
        string Path = "";
        EA.Repository Repository;
        public GenerateMessageAssemblyForm(EA.Repository Repository)
        {
            this.Repository = Repository;

            if (Repository.GetCurrentDiagram() == null)
            {
                MessageBox.Show("You must be on an opened diagram before using this function.", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            InitializeComponent();

            RegistryKey EAReg = Registry.CurrentUser.OpenSubKey("Software\\Sparx Systems\\EAAddins\\CimContextor");
            Path = (string)EAReg.GetValue("RessourcesFolder");
            Path = Path + "\\GeneratedMessageAssembly\\GeneratedAssembly";
            string TmpPath = Path;

            bool PathValidated = false; int i = 0;
            while (PathValidated.Equals(false))
            {
                if (File.Exists(Path + ".xml"))
                {
                    Path = TmpPath + i;
                    i++;
                }
                else
                {
                    PathValidated = true;
                }
            }
            Path = Path + ".xml";
            TBFilePath.Text = Path;
            this.ShowDialog();
        }

        private void ButSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.SelectedPath = Path;
            FBD.ShowDialog();
            TBFilePath.Text = FBD.SelectedPath;
            FBD.Dispose();// ABA20230401
        }

        private void ButExecuteMessageAssembly_Click(object sender, EventArgs e)
        {
            if (!TBFilePath.Text.Contains(".xml"))
            {
                MessageBox.Show("The output file must be a .xml format.", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            if (File.Exists(TBFilePath.Text))
            {
                DialogResult DR = MessageBox.Show("The output file already exist. \nDo you wish to erase it ?", "Warning !", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (DR.Equals(DialogResult.Yes))
                {
                    File.Delete(TBFilePath.Text);
                    ExecuteGenerateMessageAssembly(Repository);
                }
                else
                {
                    return;
                }
            }
            else
            {
                ExecuteGenerateMessageAssembly(Repository);
            }
        }


        private void ExecuteGenerateMessageAssembly(EA.Repository Repository)
        {
            /*
            TBFilePath.Enabled = false;
            EA.Diagram SelectedDiagram = Repository.GetCurrentDiagram();
            XmlDocument NewXmlWriter = new XmlDocument();
            EAClassToXML ClassConverter = new EAClassToXML(NewXmlWriter,TBFilePath.Text);
            ClassConverter.InitXml(Repository,SelectedDiagram);
            
            LF.AppendTitle("Processing diagram : " + SelectedDiagram.Name );
            LF.JumpALine();
            LF.AppendSubTitle("Converting classes");
            foreach(EA.DiagramObject ADiagramElement in SelectedDiagram.DiagramObjects){
                EA.Element AnElement = Repository.GetElementByID(ADiagramElement.ElementID);
                if (AnElement.Type.ToString().Equals("Class"))
                {
                    LF.AppendLog("Processing " + AnElement.Name);
                    ClassConverter.ConvertElement(Repository,AnElement);
                }
            }
            LF.AppendSubTitle("Class convertion done");
            LF.JumpALine();
            LF.AppendTitle("Done");
            this.Dispose();
             */
            LogForm LF = new LogForm();
            LF.Show();
            XmlSchema NewXmlSchema = new XmlSchema();
            XmlWriter writer = XmlWriter.Create(TBFilePath.Text);


            TBFilePath.Enabled = false;
            EA.Diagram SelectedDiagram = Repository.GetCurrentDiagram();
            EAClassToXML ClassConverter = new EAClassToXML(NewXmlSchema);
            //ClassConverter.InitXml(Repository, SelectedDiagram);

            LF.AppendTitle("Processing diagram : " + SelectedDiagram.Name);
            LF.JumpALine();
            LF.AppendSubTitle("Converting classes");
            foreach (EA.DiagramObject ADiagramElement in SelectedDiagram.DiagramObjects)
            {
                EA.Element AnElement = Repository.GetElementByID(ADiagramElement.ElementID);
                if (AnElement.Type.ToString().Equals("Class"))
                {
                    LF.AppendLog("Processing " + AnElement.Name);
                    ClassConverter.ConvertElement(Repository, AnElement);
                }
            }
            LF.AppendSubTitle("Class convertion done");
            LF.JumpALine();
            LF.AppendTitle("Done");

            NewXmlSchema.Write(writer);
            writer.Close();
            this.Dispose();
        }
    }
}
