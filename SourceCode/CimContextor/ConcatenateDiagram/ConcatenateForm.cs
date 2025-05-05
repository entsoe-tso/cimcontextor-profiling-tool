using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
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
    public partial class ConcatenateForm : Form
    {
        private EA.Repository Repo;
        private EA.Diagram TargetedDiagram;
        private LogForm LF;

        public ConcatenateForm(EA.Repository Repo)
        {
            this.Repo = Repo;

            if (Repo.GetCurrentDiagram() == null)
            {
                MessageBox.Show("You must be on an opened diagram before using this function.", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            InitializeComponent();

            TBDiagramName.Text = Repo.GetCurrentDiagram().Name;
            TBPackageName.Text = Repo.GetPackageByID(Repo.GetCurrentDiagram().PackageID).Name;

            this.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButConcatenate_Click(object sender, EventArgs e)
        {
            if( (!TBPackageName.Text.Equals("")) && (!TBDiagramName.Text.Equals("")) ){

                EA.Diagram SelectedDiagram  = Repo.GetCurrentDiagram();
                LF = new LogForm();
                LF.Show();
                LF.AppendLog("Starting concatenate functionnality's logs:");
                LF.JumpALine();
                ExecuteConcatenateDiagram(SelectedDiagram);
            }
        }
        
        /// <summary>
        /// Exit of the WindowsForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// Method that will create all the environement of the future diagram.
        /// </summary>
        /// <param name="SelectedDiagram">The SelectedDiagram that must be concatenated</param>
        private void DupplicateDiagram(EA.Diagram SelectedDiagram)
        {
            LF.AppendLog("Selected package's name: " + Repo.GetPackageByID(SelectedDiagram.PackageID).Name);
            LF.AppendLog("Selected diagram's name: "+ SelectedDiagram.Name);
            LF.JumpALine();
            TargetedDiagram = CreateDiagramEnvironment(SelectedDiagram);
            LF.AppendSubTitle("Diagram's environment creation ended");
            
            
            if (TargetedDiagram == null)
            {
                MessageBox.Show("New diagram or package couldn't be created.", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                LF.JumpALine();
                LF.AppendSubTitle("Dupplicating classes :");
                //Creating classes
                EA.Package TargetedPackage = Repo.GetPackageByID(TargetedDiagram.PackageID);
                foreach(EA.DiagramObject ADiagObj in SelectedDiagram.DiagramObjects){
                    EA.Element AParentElement = Repo.GetElementByID(ADiagObj.ElementID);
                    LF.AppendLog("Processing : " + AParentElement.Name);
                    EA.Element AChildElement = (EA.Element) TargetedPackage.Elements.AddNew(AParentElement.Name,AParentElement.Type);
                    DupplicateElement(AParentElement,AChildElement);
                    EA.DiagramObject NewObject = (EA.DiagramObject)TargetedDiagram.DiagramObjects.AddNew(AChildElement.Name, AChildElement.Type);
                    NewObject.ElementID = AChildElement.ElementID;
                    NewObject.left = ADiagObj.left;
                    NewObject.right = ADiagObj.right;
                    NewObject.top = ADiagObj.top;
                    NewObject.bottom = ADiagObj.bottom;
                    NewObject.Style = ADiagObj.Style;
                    NewObject.Update();
                }
                TargetedDiagram.DiagramObjects.Refresh();
                TargetedDiagram.Update();
                LF.AppendSubTitle("Dupplicating classes Ended");
                
                //Creating connectors
                LF.JumpALine();
                LF.AppendSubTitle("Dupplicating connectors :");
                foreach(EA.DiagramLink AParentLink in SelectedDiagram.DiagramLinks){
                    EA.Connector AParentConnector = Repo.GetConnectorByID(AParentLink.ConnectorID);
                    LF.AppendLog("Processing connector between: " + Repo.GetElementByID(AParentConnector.ClientID).Name + " " + Repo.GetElementByID(AParentConnector.SupplierID).Name);
                    EA.Connector AChildConnector = DupplicateConnector(AParentConnector, TargetedDiagram);
                    EA.DiagramLink ANewLink = (EA.DiagramLink) TargetedDiagram.DiagramLinks.AddNew(AChildConnector.Name,AChildConnector.Type);
                    ANewLink.ConnectorID = AChildConnector.ConnectorID;
                    ANewLink.Style = AParentLink.Style;
                    ANewLink.Geometry = AParentLink.Geometry;
                    ANewLink.Update();
                }
                TargetedDiagram.Update();
                TargetedDiagram.DiagramObjects.Refresh();
                TargetedDiagram.DiagramLinks.Refresh();
                LF.AppendSubTitle("Dupplicating connectors ended");
                LF.JumpALine();
            }
        }
        
        /// <summary>
        /// Root method to concatenate a diagram
        /// </summary>
        /// <param name="SelectedDiagram">A SelectedDiagram that will be concatenated</param>
        private void ExecuteConcatenateDiagram(EA.Diagram SelectedDiagram){

                LF.AppendTitle("Starting dupplication");
                DupplicateDiagram(SelectedDiagram);
                LF.AppendTitle("Dupplication ended");
                LF.JumpALine();
                LF.AppendTitle("Starting concatenation");
                LF.JumpALine();
                bool CheckResult=true;
                while (CheckResult.Equals(true)){
                    LF.AppendSubTitle("Starting the diagram's check");
                    CheckResult = CheckCardinality();
                    LF.AppendSubTitle("Checking endeds");
                    LF.JumpALine();
                }
                LF.AppendTitle("Concatenation ended");
                LF.JumpALine();
                LF.AppendLog("Concatenate functionnality's logs ended");
                LF.Show();
                this.Dispose();
        }

        /// <summary>
        /// Method that is called once the environment is created.
        /// It check the cardinalities of objects and call concatenate and delete method on them.
        /// It return if he have made a change to the diagram.
        /// </summary> 
        /// <returns></returns>
        public bool CheckCardinality()
        {
            bool ReturnValue = false;
        
            foreach (EA.DiagramObject AnObject in TargetedDiagram.DiagramObjects)
            {
                EA.Element AnElement = Repo.GetElementByID(AnObject.ElementID);
                
                bool LeefElement = true;
                
                foreach (EA.Connector AConnector in AnElement.Connectors)
                {
                    if(!AConnector.Stereotype.Equals("IsBasedOn")){
                        if(AConnector.ClientID.Equals(AnElement.ElementID)){
                            LeefElement = false;
                        }
                    }
                }
                if(LeefElement.Equals(true)){
                    LF.AppendLog("Leef Classe found: " + AnElement.Name);
                    foreach (EA.Connector AConnector in AnElement.Connectors)
                    {
                        if ((AConnector.SupplierEnd.Cardinality.Equals("1")) || (AConnector.SupplierEnd.Cardinality.Equals("0..1")))
                        {
                            int NumberOfConnector = 0;
                            LF.AppendLog(AnElement.Name +" connectors's cardinality allow concatenation");
                            EA.Element AnElementToCheck = Repo.GetElementByID(DeleteConcatenatedConnectors(ConcatenateAttribute(AConnector.ClientID, AConnector.SupplierID, AConnector.ConnectorID)));
                            LF.AppendLog(AnElementToCheck.Name + " have been succefully concatenated");

                            foreach (EA.Connector ACheckConnector in AnElementToCheck.Connectors)
                            {
                                if (!ACheckConnector.Stereotype.Equals("IsBasedOn"))
                                {
                                    NumberOfConnector = NumberOfConnector +1;
                                }
                            }

                            if (NumberOfConnector.Equals(0))
                            {
                                LF.AppendLog(AnElementToCheck.Name + " is without connectors, deleting it");
                                DeleteConcatenatedClass(AnElementToCheck.ElementID);
                            }
                            else
                            {
                                LF.AppendLog(AnElementToCheck.Name + " still have connectors, skipping deletion"); 
                            }

                            TargetedDiagram.Update(); ;
                            ReturnValue = true;
                        }
                        else { LF.AppendLog("Connector doesn't meet the requirement for a concatenation."); }
                    } 
                }
            }
            return ReturnValue;
        }

        /// <summary>
        /// Delete an ElementID's object from a diagram.
        /// </summary>
        /// <param name="ElementID">The targeted element's ID.</param>
        private void DeleteConcatenatedClass(int ElementID)
        {
            int i = 0;
            foreach (EA.DiagramObject AnObject in TargetedDiagram.DiagramObjects)
            {
                if (AnObject.ElementID.Equals(ElementID))
                {
                    TargetedDiagram.DiagramObjects.DeleteAt((short)i, true);
                    TargetedDiagram.Update();
                    DeleteConcatenatedObject(ElementID);
                }
                i++;
            } 
        }


        private void DeleteConcatenatedObject(int ElementID)
        {
            EA.Package TargetedPackage = Repo.GetPackageByID(Repo.GetElementByID(ElementID).PackageID);
            int i = 0;
            foreach (EA.Element AnElement in TargetedPackage.Elements)
            {
                if (AnElement.ElementID.Equals(ElementID))
                {
                    TargetedPackage.Elements.DeleteAt((short)i, true);
                    TargetedPackage.Update();
                }
                i++;
            }
        }



        /// <summary>
        /// Delete a ConnectorID's object.
        /// Return the SupplierID's of the object that could be usefull to deleted.
        /// </summary>
        /// <param name="ConnectorID">The ID of the connector that must be deleted.</param>
        /// <returns>Return the SupplierID's of the object that could be usefull to delete.</returns>
        private int DeleteConcatenatedConnectors(int ConnectorID)
        {
                EA.Element AnElementToChange = Repo.GetElementByID(Repo.GetConnectorByID(ConnectorID).ClientID);
                int SupplierID=0;
                int i = 0, cpt = 0;
                foreach (EA.Connector AConnector in AnElementToChange.Connectors)
                {
                    if (AConnector.ConnectorID.Equals(ConnectorID))
                    {
                        SupplierID = AConnector.SupplierID;
                        cpt = i;
                    }
                    i++;
                }
                AnElementToChange.Connectors.Delete((short)cpt);
                AnElementToChange.Connectors.Refresh();
                AnElementToChange.Update();
            return SupplierID;
        }

        /// <summary>
        /// Concatenate all the attributes from a ClientID's object to a SupplierID's object.
        /// Return the ConnectorID that can be deleted.
        /// </summary>
        /// <param name="ClientID">The target element's ID of the data.</param>
        /// <param name="SupplierID">The source element's ID of the data.</param>
        /// <param name="ConnectorID">The connector's ID currently worked on.</param>
        /// <returns></returns>
        private int ConcatenateAttribute(int ClientID, int SupplierID, int ConnectorID)
        {

            EA.Element AClient = Repo.GetElementByID(ClientID);
            EA.Element ASupplier = Repo.GetElementByID(SupplierID);
  
            foreach(EA.Attribute ASupplierAttribute in ASupplier.Attributes){

                EA.Attribute AClientAttribute = (EA.Attribute)AClient.Attributes.AddNew(Repo.GetConnectorByID(ConnectorID).SupplierEnd.Role + "_" + ASupplierAttribute.Name, ASupplierAttribute.Type);
                AClientAttribute.ClassifierID = ASupplierAttribute.ClassifierID;
                AClientAttribute.Update(); 
                foreach(EA.AttributeConstraint ASupplierAttributeConstraint in ASupplierAttribute.Constraints){
                        EA.AttributeConstraint AClientConstraint = (EA.AttributeConstraint) AClientAttribute.Constraints.AddNew(ASupplierAttributeConstraint.Name,ASupplierAttributeConstraint.Type);
                        AClientConstraint.Notes = ASupplierAttributeConstraint.Notes;
                        AClientConstraint.Update();
                }
                AClientAttribute.Default = ASupplierAttribute.Default;
                AClientAttribute.IsConst = ASupplierAttribute.IsConst;
                AClientAttribute.Stereotype = ASupplierAttribute.Stereotype;

                if (Repo.GetConnectorByID(ConnectorID).SupplierEnd.Cardinality.Equals("1"))
                {
                    AClientAttribute.LowerBound = ASupplierAttribute.LowerBound;
                    AClientAttribute.UpperBound = ASupplierAttribute.UpperBound;
                    LF.AppendLog(AClientAttribute.Name + "'s cardinalities : " + ASupplierAttribute.LowerBound + ".." + ASupplierAttribute.UpperBound);
                }
                else
                {
                    if(Repo.GetConnectorByID(ConnectorID).SupplierEnd.Cardinality.Contains("..")){
                        string[] Cardinlities = Repo.GetConnectorByID(ConnectorID).SupplierEnd.Cardinality.Split("..".ToCharArray());
                        if(Cardinlities.Length>1){

                            AClientAttribute.LowerBound = "0";
                            AClientAttribute.UpperBound = ASupplierAttribute.UpperBound;
                            LF.AppendLog(AClientAttribute.Name + "'s cardinalities : " + "0.." + ASupplierAttribute.UpperBound);
                
                        }
                        else{
                            AClientAttribute.LowerBound = Cardinlities[0];
                            AClientAttribute.UpperBound = Cardinlities[0];
                            LF.AppendLog(AClientAttribute.Name + "'s cardinalities : " + Cardinlities[0]);
                        }
                    }else{
                        AClientAttribute.LowerBound = "0";
                        AClientAttribute.UpperBound = Repo.GetConnectorByID(ConnectorID).SupplierEnd.Cardinality;
                        LF.AppendLog(AClientAttribute.Name + "'s cardinalities : 0.." + Repo.GetConnectorByID(ConnectorID).SupplierEnd.Cardinality);
                    }
                }

                foreach (EA.AttributeTag ASupplierTagValue in ASupplierAttribute.TaggedValues)
                {
                        EA.AttributeTag NewClientTaggedValue = (EA.AttributeTag)AClientAttribute.TaggedValues.AddNew(ASupplierTagValue.Name,ASupplierTagValue.Value);
                        NewClientTaggedValue.Notes = ASupplierTagValue.Notes;
                        NewClientTaggedValue.Update();
                }
                AClientAttribute.Update();
             }
            return ConnectorID;
        }

        /// <summary>
        /// Copy all the attributes from AParentElement to his AChildElement
        /// </summary>
        /// <param name="AParentElement">The source of the data.</param>
        /// <param name="AChildElement">The target of the data.</param>
        private void DupplicateElement(EA.Element AParentElement, EA.Element AChildElement)
        {
            AChildElement.Status = AParentElement.Status;
            AChildElement.Priority = AParentElement.Priority;
            AChildElement.Notes = AParentElement.Notes;
            AChildElement.Tag = AParentElement.Tag;

            foreach(EA.TaggedValue AParentTag in AParentElement.TaggedValues){
                EA.TaggedValue newTag  = (EA.TaggedValue) AChildElement.TaggedValues.AddNew(AParentTag.Name, AParentTag.Value);
                newTag.Notes = AParentTag.Notes;
                newTag.Update();
            }

            foreach(EA.Constraint AParentConstraint in AParentElement.Constraints){
                EA.Constraint AChildConstraint = (EA.Constraint) AChildElement.Constraints.AddNew(AParentConstraint.Name, AParentConstraint.Type);
                AChildConstraint.Notes = AParentConstraint.Notes;
                AChildConstraint.Update();
            }

            foreach(EA.Attribute AParentAttribute in AParentElement.Attributes ){
                EA.Attribute NewChildAttribute = (EA.Attribute) AChildElement.Attributes.AddNew(AParentAttribute.Name,AParentAttribute.Type);
                NewChildAttribute.ClassifierID = AParentAttribute.ClassifierID;
                NewChildAttribute.Update(); 
                foreach(EA.AttributeConstraint AParentConstraint in AParentAttribute.Constraints){
                        EA.AttributeConstraint AChildConstraint = (EA.AttributeConstraint) NewChildAttribute.Constraints.AddNew(AParentConstraint.Name,AParentConstraint.Type);
                        AChildConstraint.Notes = AParentConstraint.Notes;
                        AChildConstraint.Update();
                }
                
                NewChildAttribute.Default = AParentAttribute.Default;
                NewChildAttribute.IsConst = AParentAttribute.IsConst;
                NewChildAttribute.LowerBound = AParentAttribute.LowerBound;
                NewChildAttribute.UpperBound = AParentAttribute.UpperBound;
                NewChildAttribute.Stereotype = AParentAttribute.Stereotype;

                foreach (EA.AttributeTag AParentTagValue in AParentAttribute.TaggedValues)
                {
                        EA.AttributeTag NewChildTaggedValue = (EA.AttributeTag)NewChildAttribute.TaggedValues.AddNew(AParentTagValue.Name,AParentTagValue.Value);
                        NewChildTaggedValue.Notes = AParentTagValue.Notes;
                        NewChildTaggedValue.Update();
                }
                NewChildAttribute.Update(); 
                }  
            }

        /// <summary>
        /// Copy AParentConnector and his attribute (Tagged value and such) to a TargetedDiagram.
        /// </summary>
        /// <param name="AParentConnector">The connector to be copied.</param>
        /// <param name="TargetedDiagram">The location to copy the connector.</param>
        /// <returns></returns>
        private EA.Connector DupplicateConnector(EA.Connector AParentConnector, EA.Diagram TargetedDiagram)
        {
            int ClientID=0;
            int SupplierID=0;
            TargetedDiagram.Update();
            foreach (EA.DiagramObject AChildDiagObject in TargetedDiagram.DiagramObjects)
            {
                if (Repo.GetElementByID(AChildDiagObject.ElementID).Name.Equals(Repo.GetElementByID(AParentConnector.ClientID).Name))
                {
                    ClientID = AChildDiagObject.ElementID;
                }
                if (Repo.GetElementByID(AChildDiagObject.ElementID).Name.Equals(Repo.GetElementByID(AParentConnector.SupplierID).Name))
                {
                    SupplierID = AChildDiagObject.ElementID;
                }
            }
            EA.Connector AChildConnector = (EA.Connector) Repo.GetElementByID(ClientID).Connectors.AddNew(AParentConnector.Name,AParentConnector.Type);
            AChildConnector.SupplierID = SupplierID;
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
            AChildConnector.EndPointX = AParentConnector.EndPointX;
            AChildConnector.EndPointY = AParentConnector.EndPointY;
            AChildConnector.StartPointX = AParentConnector.StartPointX;
            AChildConnector.StartPointY = AParentConnector.StartPointY;
            AChildConnector.Update();
            foreach(EA.ConnectorTag AParentTagValue in AParentConnector.TaggedValues){
                EA.ConnectorTag ANewTag = (EA.ConnectorTag) AChildConnector.TaggedValues.AddNew(AParentTagValue.Name, AParentTagValue.Value);
                ANewTag.Notes = AParentTagValue.Notes;
                ANewTag.Update();
            }
            foreach(EA.ConnectorConstraint AParentConstraint in AParentConnector.Constraints ){
                EA.ConnectorConstraint ANewConstraint = (EA.ConnectorConstraint) AChildConnector.Constraints.AddNew(AParentConstraint.Name,AParentConstraint.Type);
                ANewConstraint.Notes = AParentConstraint.Notes;
                ANewConstraint.Update();
            }
            AChildConnector.Update();
            return AChildConnector;
        }
        
        /// <summary>
        /// Attempt to create a new package and a class diagram with the parameters from the user interface.
        /// </summary>
        /// <param name="SelectedDiagram">It's the diagram currently selected in the browser.</param>
        /// <returns>Return the created package or null if it couldn't be created.</returns>
        private EA.Diagram CreateDiagramEnvironment(EA.Diagram SelectedDiagram)
        {    
            LF.AppendSubTitle("Creating diagram environment");
            EA.Package SelectedPackage = Repo.GetPackageByID(SelectedDiagram.PackageID);
               EA.Package TargetedPackage = (EA.Package)SelectedPackage.Packages.AddNew(TBPackageName.Text, "Package");
               LF.AppendLog("Destination package's name: " + TargetedPackage.Name);
               TargetedPackage.Update();
               if(TargetedPackage == null){
                   MessageBox.Show("Package couldn't be created.", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                   return null;
               }
               EA.Diagram TargetedDiagram = (EA.Diagram) TargetedPackage.Diagrams.AddNew(TBDiagramName.Text, "Diagram");       
               TargetedDiagram.cx = SelectedDiagram.cx;
               TargetedDiagram.cy = SelectedDiagram.cy;
               TargetedDiagram.Notes = SelectedDiagram.Notes;
               TargetedDiagram.Orientation = SelectedDiagram.Orientation;
               TargetedDiagram.Scale = SelectedDiagram.Scale;
               TargetedDiagram.Stereotype = SelectedDiagram.Stereotype;
               TargetedDiagram.Version = SelectedDiagram.Version;
               TargetedDiagram.Update();
               LF.AppendLog("Destination diagram's name: " + TargetedPackage.Name);              
               Repo.RefreshModelView(TargetedPackage.PackageID);
               return TargetedDiagram;
            
        }
    }
}
