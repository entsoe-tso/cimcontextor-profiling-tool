using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
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
    public partial class GlobalIsBasedOnForm : Form
    {
        private EA.Repository Repository;
        private EA.Package SelectedPackage;
        private LogForm LF;
        private ConstantDefinition CD = new ConstantDefinition();

        public GlobalIsBasedOnForm(EA.Repository Repository, EA.Package SelectedPackage)
        {
            InitializeComponent();
            this.Repository = Repository;
            this.SelectedPackage = SelectedPackage;
            this.Text = "Package wide IsBasedOn on " + SelectedPackage.Name;
            XMLParser XMLP = new XMLParser(Repository);
            ArrayList QualifierList = XMLP.GetXmlQualifier("class");

            foreach(string AQualifier in QualifierList){
                CBQualifier.Items.Add(AQualifier);
            }
            CBQualifier.Items.Add("No qualifier");
            CBQualifier.SelectedItem = "No qualifier";
        }

        private void ButGlobalIBO_Click(object sender, EventArgs e)
        {
            if (CBNewPackage.Checked.Equals(true))
            {
                if (TBPackageName.Text.Equals(""))
                {
                    MessageBox.Show("If you wish to use the global IsBasedOn in a new package you must specify his name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    LF = new LogForm();
                    LF.Show();
                    LF.AppendLog("Starting global IsBasedOn");
                    if (CBQualifier.Items.Contains(CBQualifier.Text))
                    {
                        if(CBQualifier.Text.Equals("No qualifier")){
                            LF.AppendLog("Settings:");
                            LF.AppendLog("Qualifier : No qualifier");
                            LF.AppendLog("Package : "+TBPackageName.Text);
                            LF.JumpALine();
                            ExecuteGlobalIsBasedOn(TBPackageName.Text, "");
                        }
                        else{
                            LF.AppendLog("Settings:");
                            LF.AppendLog("Qualifier : "+ CBQualifier.Text);
                            LF.AppendLog("Package : " + TBPackageName.Text);
                            ExecuteGlobalIsBasedOn(TBPackageName.Text, CBQualifier.Text + "_");
                        }
                    }
                }
            }
            else if(CBNewPackage.Checked.Equals(false))
            {
                if(CBQualifier.Text==null){
                    MessageBox.Show("If you wish to use the global IsBasedOn in the current package, you need to select a global qualifier.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }


                if (CBQualifier.Items.Contains(CBQualifier.Text))
                {
                    if (CBQualifier.Text.Equals("No qualifier"))
                    {
                        MessageBox.Show("If you wish to use the global IsBasedOn in the current package, you need to select a global qualifier.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        LF = new LogForm();
                        LF.Show();
                        LF.AppendLog("Starting global IsBasedOn");
                        LF.AppendLog("Settings:");
                        LF.AppendLog("Qualifier : " + CBQualifier.Text);
                        LF.AppendLog("Package : " + SelectedPackage.Name);
                        ExecuteGlobalIsBasedOn("",CBQualifier.Text+"_");
                    }
                }
                else
                {
                    MessageBox.Show("you must select a valid item from the qualifier list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void CBNewPackage_CheckedChanged(object sender, EventArgs e)
        {
            if (CBNewPackage.Checked.Equals(true))
            {
                TBPackageName.Enabled = true;
            }
            else if(CBNewPackage.Checked.Equals(false))
            {
                TBPackageName.Text = "";
                TBPackageName.Enabled = false;
            }
        }

        private void ExecuteGlobalIsBasedOn(string PackageName, string Qualifier)
        {
            LF.JumpALine();
            LF.AppendTitle("Creating diagram environment");
            EA.Package TargetedPackage = CreateDiagramEnvironment(SelectedPackage, PackageName, Qualifier);
            if (TargetedPackage == null)
            {
                MessageBox.Show("Package couldn't be created ! \nAborting functionnality.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
            else
            {
                LF.AppendTitle("Diagram environment creation ended");
                LF.JumpALine();
                LF.AppendTitle("Creating elements");
             
                //foreach (EA.Element AParentElement in SelectedPackage.Elements)
                for (short i = 0; SelectedPackage.Elements.Count>i; i++)
                {
                    EA.Element AParentElement = (EA.Element)SelectedPackage.Elements.GetAt(i);
                    EA.Element AChildElement = null;
                    AChildElement = (EA.Element)TargetedPackage.Elements.AddNew(Qualifier + AParentElement.Name, AParentElement.Type);
                    LF.JumpALine();
                    LF.AppendSubTitle("Creating " + AChildElement.Name);
                    DupplicateElement(AParentElement, AChildElement);
                    //IBO
                    EA.Connector newBasedOn = (EA.Connector)AChildElement.Connectors.AddNew("", "Dependency");
                    LF.AppendLog("Creating IsBasedOn");
                    newBasedOn.Stereotype = "IsBasedOn";
                    newBasedOn.SupplierID = AParentElement.ElementID;
                    newBasedOn.Update();
                    AChildElement.Connectors.Refresh();
                    //END IBO
                    LF.AppendSubTitle(AChildElement.Name + " creation ended");
                    LF.JumpALine();
                }
                LF.AppendTitle("Elements creation ended");
                LF.JumpALine();
                LF.AppendTitle("Dupplicating diagram objects");
                TargetedPackage.Diagrams.Refresh();

                //foreach (EA.Diagram ASelectedDiagram in SelectedPackage.Diagrams)
                for (short j = 0; SelectedPackage.Diagrams.Count>j; j++)
                {
                    EA.Diagram ASelectedDiagram = (EA.Diagram)SelectedPackage.Diagrams.GetAt(j);
                    //foreach (EA.Diagram ATargetedDiagram in TargetedPackage.Diagrams)
                    for (short k = 0; TargetedPackage.Diagrams.Count>k; k++)
                    {
                        EA.Diagram ATargetedDiagram = (EA.Diagram)TargetedPackage.Diagrams.GetAt(k);
                        if ((Qualifier + ASelectedDiagram.Name).Equals(ATargetedDiagram.Name))
                        {
                            DupplicateDiagramObjects(ASelectedDiagram, ATargetedDiagram);
                            Repository.RefreshOpenDiagrams(true);
                        }
                    }
                }
                LF.AppendTitle("Diagram objects dupplication ended");
                LF.JumpALine();
                LF.AppendTitle("Dupplicating connectors");
                //foreach (EA.Element AParentElement in SelectedPackage.Elements)
                for (short l = 0; SelectedPackage.Elements.Count>l; l++)
                {
                    EA.Element AParentElement = (EA.Element)SelectedPackage.Elements.GetAt(l);
                    DupplicateConnectors(AParentElement, TargetedPackage);
                }
                LF.AppendTitle("Connectors dupplication ended");
                TargetedPackage.Diagrams.Refresh();
                Repository.RefreshModelView(TargetedPackage.PackageID);
                LF.AppendLog("Global IsBasedOn ended");
                this.Dispose();
            }
        }

        private void DupplicateDiagramObjects(EA.Diagram SelectedDiagram, EA.Diagram TargetedDiagram)
        {
            EA.Package TargetedPackage = Repository.GetPackageByID(TargetedDiagram.PackageID);

            for (short i = 0; SelectedDiagram.DiagramObjects.Count > i; i++)
            {
                EA.DiagramObject ASelectedObject = (EA.DiagramObject)SelectedDiagram.DiagramObjects.GetAt(i);
                EA.Element ATargetedObject = null;
                EA.Element ASelectedElement = (EA.Element)Repository.GetElementByID(ASelectedObject.ElementID);
            for (short k = 0; ASelectedElement.Connectors.Count > k; k++)
                    {
                        EA.Connector ATargetedConnectors = (EA.Connector)ASelectedElement.Connectors.GetAt(k);
                        if ((ATargetedConnectors.Stereotype.Equals("IsBasedOn")) && (ATargetedConnectors.SupplierID.Equals(ASelectedObject.ElementID)))
                        {
                            ATargetedObject = Repository.GetElementByID(ATargetedConnectors.ClientID);
                            break;
                        }
                    }
              
                EA.DiagramObject ATargetedDiagramObject=null;
                if (!(ATargetedObject == null))
                {
                ATargetedDiagramObject = (EA.DiagramObject)TargetedDiagram.DiagramObjects.AddNew(ATargetedObject.Name, ATargetedObject.Type);
                ATargetedDiagramObject.ElementID = ATargetedObject.ElementID;
                ATargetedDiagramObject.Update();
                LF.AppendLog("Copying " + Repository.GetElementByID(ASelectedObject.ElementID).Name);
 
                string ObjectColor = "Default";
                try
                {
                    XMLParser XMLP = new XMLParser(Repository);
                    ObjectColor = XMLP.GetXmlValueConfig("ConfigColor").Trim();
                }
                catch
                {

                }
                if (!ObjectColor.Equals("Default"))
                {
                    ATargetedDiagramObject.Style = "BCol=" + ObjectColor + ";BFol=0;LCol=0;LWth=1;";
                }
                
                }
               else
                {
                    ATargetedDiagramObject = (EA.DiagramObject)TargetedDiagram.DiagramObjects.AddNew(Repository.GetElementByID(ASelectedObject.ElementID).Name, Repository.GetElementByID(ASelectedObject.ElementID).Type);
                    ATargetedDiagramObject.ElementID = ASelectedObject.ElementID;
                    ATargetedDiagramObject.Style = ASelectedObject.Style;
                    LF.AppendLog("Copying " + Repository.GetElementByID(ASelectedObject.ElementID).Name);
                }
                
                ATargetedDiagramObject.left = ASelectedObject.left;
                ATargetedDiagramObject.right = ASelectedObject.right;
                ATargetedDiagramObject.top = ASelectedObject.top;
                ATargetedDiagramObject.bottom = ASelectedObject.bottom;
                
                ATargetedDiagramObject.Update();
                TargetedDiagram.DiagramObjects.Refresh();
                TargetedDiagram.Update();
                TargetedPackage.Diagrams.Refresh();
            }
        }
       
        

        private void DupplicateDiagramLinks(EA.Connector AChildConnector, EA.Package TargetedPackage)
        {
            
            //foreach(EA.Diagram ADiagram in TargetedPackage.Diagrams){
            for (short i = 0; TargetedPackage.Diagrams.Count>i; i++)
            {
                EA.Diagram ADiagram = (EA.Diagram)TargetedPackage.Diagrams.GetAt(i);
                bool ClientFound = false;
                bool SupplierFound = false;
                ADiagram.DiagramObjects.Refresh();
                //foreach (EA.DiagramObject AnObject in ADiagram.DiagramObjects)
                for (short j = 0; ADiagram.DiagramObjects.Count>j; j++)
                {
                    EA.DiagramObject AnObject = (EA.DiagramObject)ADiagram.DiagramObjects.GetAt(j);
                    if (AnObject.ElementID.Equals(AChildConnector.ClientID))
                    {
                        ClientFound = true;
                    }
                    if (AnObject.ElementID.Equals(AChildConnector.SupplierID))
                    {
                        SupplierFound = true;
                    }
                }

                if (ClientFound.Equals(true) && SupplierFound.Equals(true))
                {
                    EA.DiagramLink ANewLink = (EA.DiagramLink)ADiagram.DiagramLinks.AddNew(AChildConnector.Name, AChildConnector.Type);
                    ANewLink.ConnectorID = AChildConnector.ConnectorID;
                    ANewLink.Update();
                }
            }
        }

        private void DupplicateConnectors(EA.Element AParentElement,EA.Package TargetedPackage)
        {
            
                    EA.Element AChildElement = null;

                    //foreach(EA.Connector AParentConnector in AParentElement.Connectors){
                    for (short i = 0; AParentElement.Connectors.Count>i; i++)
                    {
                        EA.Connector AParentConnector = (EA.Connector)AParentElement.Connectors.GetAt(i);
                        if (AParentConnector.Stereotype.Equals("IsBasedOn"))
                        {
                            AChildElement = Repository.GetElementByID(AParentConnector.ClientID);
                            break;
                        }
                    }
                    if (!(AChildElement == null))
                    {
                        //foreach (EA.Connector AParentConnector in AParentElement.Connectors)
                        for (short i = 0; AParentElement.Connectors.Count > i;i++ )
                        {
                            EA.Connector AParentConnector = (EA.Connector)AParentElement.Connectors.GetAt(i);
                            if (!AParentConnector.Stereotype.Equals("IsBasedOn"))
                            {
                                if (AParentConnector.ClientID.Equals(AParentElement.ElementID))
                                {
                                    LF.AppendSubTitle("Copying the connector between " + Repository.GetElementByID(AParentConnector.ClientID).Name + "to" + Repository.GetElementByID(AParentConnector.SupplierID).Name);
                                    EA.Element AnIBOTargetElement = null;

                                    //foreach (EA.Connector AnIBOTargetConnector in Repository.GetElementByID(AParentConnector.SupplierID).Connectors)
                                    for(short j=0;Repository.GetElementByID(AParentConnector.SupplierID).Connectors.Count>j;j++)
                                    {
                                    EA.Connector AnIBOTargetConnector = (EA.Connector)Repository.GetElementByID(AParentConnector.SupplierID).Connectors.GetAt(j);
                                        if (AnIBOTargetConnector.Stereotype.Equals("IsBasedOn"))
                                        {
                                            AnIBOTargetElement = Repository.GetElementByID(AnIBOTargetConnector.ClientID);
                                            break;
                                        }
                                    }
                                    LF.AppendLog("Copying properties");

                                    EA.Connector AChildConnector = (EA.Connector)AChildElement.Connectors.AddNew(AParentConnector.Name, AParentConnector.Type);
                                    AChildConnector.SupplierID = AnIBOTargetElement.ElementID;
                                    AChildConnector.ClientEnd.Aggregation = AParentConnector.ClientEnd.Aggregation;
                                    AChildConnector.SupplierEnd.Aggregation = AParentConnector.SupplierEnd.Aggregation;
                                    AChildConnector.Stereotype = AParentConnector.Stereotype;
                                    AChildConnector.Notes = AParentConnector.Notes;
                                    AChildConnector.Alias = AParentConnector.Alias;
                                    AChildConnector.Color = AParentConnector.Color;
                                    AChildConnector.ClientEnd.Cardinality = AParentConnector.ClientEnd.Cardinality;
                                    AChildConnector.SupplierEnd.Cardinality = AParentConnector.SupplierEnd.Cardinality;
                                    AChildConnector.ClientEnd.Role = AParentConnector.ClientEnd.Role;
                                    AChildConnector.ClientEnd.RoleNote = AParentConnector.ClientEnd.RoleNote;
                                    AChildConnector.SupplierEnd.Role = AParentConnector.SupplierEnd.Role;
                                    AChildConnector.SupplierEnd.RoleNote = AParentConnector.SupplierEnd.RoleNote;
                                    AChildConnector.ClientEnd.RoleType = AParentConnector.ClientEnd.RoleType;
                                    AChildConnector.SupplierEnd.RoleType = AParentConnector.SupplierEnd.RoleType;
                                    AChildConnector.Direction = AParentConnector.Direction;
                                    AChildConnector.Update();


                                    //foreach (EA.ConnectorTag AParentTagValue in AParentConnector.TaggedValues)
                                    for (short j = 0; AParentConnector.TaggedValues.Count>j; j++)
                                    {
                                        EA.ConnectorTag AParentTagValue = (EA.ConnectorTag)AParentConnector.TaggedValues.GetAt(j);
                                        EA.ConnectorTag ANewTag = (EA.ConnectorTag)AChildConnector.TaggedValues.AddNew(AParentTagValue.Name, AParentTagValue.Value);
                                        LF.AppendLog("Copying TagValue : " + ANewTag.Name);
                                        ANewTag.Notes = AParentTagValue.Notes;
                                        ANewTag.Update();
                                    }

                                    //foreach (EA.ConnectorConstraint AParentConstraint in AParentConnector.Constraints)
                                    for (short j = 0; AParentConnector.Constraints.Count>j; j++)
                                    {
                                        EA.ConnectorConstraint AParentConstraint = (EA.ConnectorConstraint)AParentConnector.Constraints.GetAt(j);
                                        EA.ConnectorConstraint ANewConstraint = (EA.ConnectorConstraint)AChildConnector.Constraints.AddNew(AParentConstraint.Name, AParentConstraint.Type);
                                        LF.AppendLog("Copying Connectors's constraint : " + ANewConstraint.Name);
                                        ANewConstraint.Notes = AParentConstraint.Notes;
                                        ANewConstraint.Update();
                                    }
                                    AChildConnector.Update();
                                    TargetedPackage.Diagrams.Refresh();
                                    TargetedPackage.Update();
                                    this.DupplicateDiagramLinks(AChildConnector, TargetedPackage);
                                    LF.AppendSubTitle("Connector copy ended");
                                    LF.JumpALine();

                                }
                            }
                        }
                        }
        }

        private void CBQualifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TBPackageName.Text.Equals("") && (CBNewPackage.Checked.Equals(true)) && ((!(CBQualifier.SelectedItem.ToString() == "")) && (!(CBQualifier.SelectedItem.ToString() == "No qualifier"))))
            {
                    TBPackageName.Text = CBQualifier.SelectedItem.ToString() + "_" + SelectedPackage.Name;
            }
        }

        private void DupplicateElement(EA.Element AParentElement, EA.Element AChildElement)
        {
            LF.AppendLog("Copying properties from " + AParentElement.Name + " to " + AChildElement.Name);
            AChildElement.Status = AParentElement.Status;
            AChildElement.Priority = AParentElement.Priority;
            AChildElement.Notes = AParentElement.Notes;
            AChildElement.Tag = AParentElement.Tag;
            //foreach (EA.TaggedValue AParentTag in AParentElement.TaggedValues)
            for(short i =0;AParentElement.TaggedValues.Count>i ;i++)
            {
            EA.TaggedValue AParentTag = (EA.TaggedValue) AParentElement.TaggedValues.GetAt(i);
                EA.TaggedValue newTag = (EA.TaggedValue)AChildElement.TaggedValues.AddNew(AParentTag.Name, AParentTag.Value);
                LF.AppendLog("Copying TagValue : " + AParentTag.Name);
                newTag.Notes = AParentTag.Notes;
                newTag.Update();
            }

            //foreach (EA.Constraint AParentConstraint in AParentElement.Constraints)
            for(short i = 0; AParentElement.Constraints.Count>i;i++)
            {
                EA.Constraint AParentConstraint = (EA.Constraint)AParentElement.Constraints.GetAt(i);
                EA.Constraint AChildConstraint = (EA.Constraint)AChildElement.Constraints.AddNew(AParentConstraint.Name, AParentConstraint.Type);
                LF.AppendLog("Copying Constraint : " + AParentConstraint.Name);
                AChildConstraint.Notes = AParentConstraint.Notes;
                AChildConstraint.Update();
            }

            //foreach (EA.Attribute AParentAttribute in AParentElement.Attributes)
            for (short i = 0; AParentElement.Attributes.Count>i; i++)
            {
                EA.Attribute AParentAttribute = (EA.Attribute)AParentElement.Attributes.GetAt(i);
                EA.Attribute NewChildAttribute = (EA.Attribute)AChildElement.Attributes.AddNew(AParentAttribute.Name, AParentAttribute.Type);
                LF.AppendLog("Copying attribute : " + AParentAttribute.Name);
                NewChildAttribute.ClassifierID = AParentAttribute.ClassifierID;
                NewChildAttribute.Update();
                //foreach (EA.AttributeConstraint AParentConstraint in AParentAttribute.Constraints)
                for (short j = 0; AParentAttribute.Constraints.Count>j; j++)
                {
                    EA.AttributeConstraint AParentConstraint = (EA.AttributeConstraint)AParentAttribute.Constraints.GetAt(j);
                    EA.AttributeConstraint AChildConstraint = (EA.AttributeConstraint)NewChildAttribute.Constraints.AddNew(AParentConstraint.Name, AParentConstraint.Type);
                    LF.AppendLog("Copying attribute's constraint : " + AParentConstraint.Name);
                    AChildConstraint.Notes = AParentConstraint.Notes;
                    AChildConstraint.Update();
                }

                NewChildAttribute.Default = AParentAttribute.Default;
                NewChildAttribute.IsConst = AParentAttribute.IsConst;
                NewChildAttribute.LowerBound = AParentAttribute.LowerBound;
                NewChildAttribute.UpperBound = AParentAttribute.UpperBound;
                NewChildAttribute.Stereotype = AParentAttribute.Stereotype;

                //foreach (EA.AttributeTag AParentTagValue in AParentAttribute.TaggedValues)
                for (short j = 0; AParentAttribute.TaggedValues.Count>j ;j++ )
                {
                    EA.AttributeTag AParentTagValue = (EA.AttributeTag)AParentAttribute.TaggedValues.GetAt(j);
                    EA.AttributeTag NewChildTaggedValue = (EA.AttributeTag)NewChildAttribute.TaggedValues.AddNew(AParentTagValue.Name, AParentTagValue.Value);
                    LF.AppendLog("Copying attribute's TagValue : " + AParentTagValue.Name);
                    NewChildTaggedValue.Notes = AParentTagValue.Notes;
                    NewChildTaggedValue.Update();
                }
                NewChildAttribute.Update();
            }
        }

        private EA.Package CreateDiagramEnvironment(EA.Package SelectedPackage, string PackageName, string Qualifier)
        {
            EA.Package TargetedPackage = null;
            if (!PackageName.Equals(""))
            {
                LF.AppendLog("Creating package " + PackageName);
                TargetedPackage = (EA.Package)SelectedPackage.Packages.AddNew(PackageName, "Package");
                TargetedPackage.Update();
            }
            else
            {
                TargetedPackage = SelectedPackage;
            }

            if (TargetedPackage == null)
            {
                MessageBox.Show("Package couldn't be created.", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return null;
            }
            LF.JumpALine();
            LF.AppendSubTitle("Creating diagrams");
            //foreach(EA.Diagram ADiagram in SelectedPackage.Diagrams)
            for (short i = 0; SelectedPackage.Diagrams.Count>i; i++)
            {
                EA.Diagram ADiagram = (EA.Diagram)SelectedPackage.Diagrams.GetAt(i);
                EA.Diagram TargetedDiagram = (EA.Diagram)TargetedPackage.Diagrams.AddNew(Qualifier + ADiagram.Name, "Diagram");
                LF.AppendLog("Processing " + TargetedDiagram.Name);
                TargetedDiagram.cx = ADiagram.cx;
                TargetedDiagram.cy = ADiagram.cy;
                TargetedDiagram.Notes = ADiagram.Notes;
                TargetedDiagram.Orientation = ADiagram.Orientation;
                TargetedDiagram.Scale = ADiagram.Scale;
                TargetedDiagram.Stereotype = ADiagram.Stereotype;
                TargetedDiagram.Version = ADiagram.Version;
                TargetedDiagram.Update();
                Repository.RefreshModelView(TargetedPackage.PackageID);
            }
            LF.AppendSubTitle("Diagrams creation ended");
            return TargetedPackage;
        }
        
    }
}
