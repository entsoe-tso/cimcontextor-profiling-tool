using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Schema;
using System.Windows.Forms;
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
    class EAClassToXML
    {
        private XmlSchema XmlSchema;

        //public EAClassToXML(XmlDocument NewXmlWriter,string FileToWrite)
        public EAClassToXML(XmlSchema XmlSchema)
        {
            //this.NewXmlWriter = NewXmlWriter;
            this.XmlSchema = XmlSchema;
            //this.FileToWrite = FileToWrite;

        }
        /*
        public void InitXml(EA.Repository Repository,EA.Diagram SelectedDiagram)
        {
            if (NewXmlWriter.HasChildNodes.Equals(false))
            {
            XmlDeclaration DeclarationNode = NewXmlWriter.CreateXmlDeclaration("1.0", "UTF-8",null);

            NewXmlWriter.AppendChild(DeclarationNode);

            XmlNode RootNode = NewXmlWriter.CreateNode(XmlNodeType.Comment, "", "");
            RootNode.InnerText="edited with CimContextor for EA v2009 rel 1 U (http:/www.zamiren.fr)";

            NewXmlWriter.AppendChild(RootNode);

            XmlNode ElementNode = NewXmlWriter.CreateNode(XmlNodeType.Element, "schema", "");
            XmlAttribute ElementAttribute = NewXmlWriter.CreateAttribute("sawsdl","");
            ElementAttribute.Value = "http://www.w3.org/ns/sawdsl";
            ElementNode.Attributes.Append(ElementAttribute);

            ElementAttribute = NewXmlWriter.CreateAttribute("cims", "");
            ElementAttribute.Value = "http://iec.ch/TC57/1999/rdf-schema-extensions-19990926#";
            ElementNode.Attributes.Append(ElementAttribute);
             
            ElementAttribute = NewXmlWriter.CreateAttribute("xs", "");
            ElementAttribute.Value = "http://www.w3.org/2001/XMLSchema";
            ElementNode.Attributes.Append(ElementAttribute);

            ElementAttribute = NewXmlWriter.CreateAttribute(Repository.GetPackageByID(SelectedDiagram.PackageID).Name, "");
            DateTime ActualDate = DateTime.Now;
            ElementAttribute.Value = "http://iec.ch/TC57/"+ActualDate.Year+".cimVersion.name/Package.Pathname";
            ElementNode.Attributes.Append(ElementAttribute);
        
            ElementAttribute = NewXmlWriter.CreateAttribute("targetNamespace", "");
            ElementAttribute.Value = "http://iec.ch/TC57/" + ActualDate.Year + ".cimVersion.name/Package.Pathname";        
            ElementNode.Attributes.Append(ElementAttribute);

            ElementAttribute = NewXmlWriter.CreateAttribute("elementFormDefault", "");
            ElementAttribute.Value = "qualified";
            ElementNode.Attributes.Append(ElementAttribute);

            ElementAttribute = NewXmlWriter.CreateAttribute("attributeFormDefault", "");
            ElementAttribute.Value = "unqualified";
            ElementNode.Attributes.Append(ElementAttribute);
        
            ElementAttribute = NewXmlWriter.CreateAttribute("version", "");
            ElementAttribute.Value = "1.0";
            ElementNode.Attributes.Append(ElementAttribute);

            NewXmlWriter.AppendChild(ElementNode);


            ElementNode = NewXmlWriter.CreateNode(XmlNodeType.Element, "annotation", "");
            XmlNode AnnotationNode = NewXmlWriter.CreateNode(XmlNodeType.Element, "documentation", ""); 
            XmlAttribute AnnotationAttribute = NewXmlWriter.CreateAttribute("lang", "");
            ElementAttribute.Value = "en";
            AnnotationNode.InnerText = Repository.GetActivePerspective();
            AnnotationNode.Attributes.Append(AnnotationAttribute);
            ElementNode.AppendChild(AnnotationNode);


            NewXmlWriter.DocumentElement.AppendChild(ElementNode);

            try
            {
                NewXmlWriter.Save(FileToWrite);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("This initialization generated an exception. \nThe ouput file will probably be corrupted. \n Exception : \n Message : " + Ex.Message + "\n Source : " + Ex.Source + "\n Data : " + Ex.Data , "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }   
            }
        }*/

        public void ConvertElement(EA.Repository repository, EA.Element SelectedElement)
        {
            /*
            // <xs:element name="cat" type="xs:string"/>
            XmlSchemaElement anElement = new XmlSchemaElement();
            XmlSchema.Items.Add(anElement);
            anElement.Name = SelectedElement.Name;
            //Annotation
            XmlSchemaAnnotation anElementAnnotation = new XmlSchemaAnnotation();
            anElement.Annotation.Items.Add(anElementAnnotation);
            
            //<xs:documentation>description class 1</documentation>
            XmlSchemaDocumentation anElementDocumentation = new XmlSchemaDocumentation();
            anElementAnnotation.Items.Add(anElementDocumentation);
            //anElementDocumentation.Markup = TextToNodeArray(SelectedElement.Notes);
            
            //<cims:categorizedDocumentation category=”BusinessTerm”>new class 1</cims:categorizedDocumentation>
            //TagValue
            foreach(EA.TaggedValue ATag in SelectedElement.TaggedValues){

                XmlSchemaElement anElementCategorizedDocumentation = new XmlSchemaElement();
                
                anElementAnnotation.Items.Add(anElementDocumentation);
                //anElementDocumentation.;
                //XmlSchemaAttribute;
            }
            //anElementDocumentation. categorizedDocumentation
            //XmlSchemaElement anElementDocumentation = new XmlSchemaElement();
            //XmlSchemaType CategDoc = new XmlSchemaType();
            //CategDoc.Name = "categorizedDocumentation";
            //anElementDocumentation.ElementSchemaType(CategDoc); 
            //
            //anElement.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

            /*
            XmlNode RootNode=null;
            //foreach(XmlNode ANode in NewXmlWriter.FirstChild()){

            RootNode = NewXmlWriter.DocumentElement;
            //}

            XmlNode ElementClassNode = NewXmlWriter.CreateNode(XmlNodeType.Element,"complexType","");
            XmlAttribute ElementAttribute = NewXmlWriter.CreateAttribute("name", "");
            ElementAttribute.Value = SelectedElement.Name;
            ElementClassNode.Attributes.Append(ElementAttribute);

            try
            {
                DateTime ADate = DateTime.Now;
                ElementAttribute = NewXmlWriter.CreateAttribute("modelReference", "");
                ElementAttribute.Value = "http://iec.ch/TC57//" + ADate.Year + "." + "cimVersion.name" + "/" + repository.GetPackageByID(SelectedElement.PackageID).Name + "." + "?PathName?" + " #" + SelectedElement.Name;
                ElementClassNode.Attributes.Append(ElementAttribute);
            }
            catch
            {
            }
            RootNode.AppendChild(ElementClassNode);

            //Annotation
            

            XmlNode ElementAnnotation = NewXmlWriter.CreateNode(XmlNodeType.Element, "annotation", "");
            if(!SelectedElement.Notes.Equals("")){
                XmlNode ElementDocumentation = NewXmlWriter.CreateNode(XmlNodeType.Element, "documentation","");
                ElementDocumentation.InnerText = SelectedElement.Notes;
                ElementAnnotation.AppendChild(ElementDocumentation);
            }

            foreach(EA.TaggedValue ATag in SelectedElement.TaggedValues){
                XmlNode ElementDocumentation = NewXmlWriter.CreateNode(XmlNodeType.Element, "categorizedDocumentation","");
                XmlAttribute ElementDocumentationAttribute = NewXmlWriter.CreateAttribute("category");
                ElementDocumentationAttribute.Value = ATag.Name;
                ElementDocumentation.InnerText = ATag.Notes;
                ElementDocumentation.Attributes.Append(ElementDocumentationAttribute);
                ElementAnnotation.AppendChild(ElementDocumentation); 
            }

            foreach(EA.Constraint AConstraint in SelectedElement.Constraints){
                XmlNode ElementConstraint = NewXmlWriter.CreateNode(XmlNodeType.Element, "constraint", "");
                XmlAttribute ElementConstraintAttribute = NewXmlWriter.CreateAttribute("name");
                ElementConstraintAttribute.Value = AConstraint.Name;
                ElementConstraint.InnerText = AConstraint.Notes;
                ElementConstraint.Attributes.Append(ElementConstraintAttribute);
                ElementAnnotation.AppendChild(ElementConstraint); 
            }

            ElementClassNode.AppendChild(ElementAnnotation);
           


            

            //Attribute
            XmlNode ElementAttributeNode = NewXmlWriter.CreateNode(XmlNodeType.Element, "sequence","");
            
            foreach (EA.Attribute AnAttribute in SelectedElement.Attributes)
            {
                
                XmlNode ElementNode = NewXmlWriter.CreateNode(XmlNodeType.Element, "xs:element","");
                ElementAttribute = NewXmlWriter.CreateAttribute("name", "");
                ElementAttribute.Value = AnAttribute.Name;
                ElementNode.Attributes.Append(ElementAttribute);

                ElementAttribute = NewXmlWriter.CreateAttribute("type", "");
                ElementAttribute.Value = repository.GetElementByID(AnAttribute.ClassifierID).Name;
                ElementNode.Attributes.Append(ElementAttribute);

                if(AnAttribute.LowerBound.Equals("0")){
                    ElementAttribute = NewXmlWriter.CreateAttribute("minOccurs", "");
                    ElementAttribute.Value = AnAttribute.LowerBound;
                    ElementNode.Attributes.Append(ElementAttribute);
                }
                ElementAttributeNode.AppendChild(ElementNode);
            }
            ElementClassNode.AppendChild(ElementAttributeNode);

            

            try
            {
                NewXmlWriter.Save(FileToWrite);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("The object " + SelectedElement.Name + " generated an exception. /n The ouput file will probably be corrupted. /n Exception : " + Ex , "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
         */
        }
    }
}
