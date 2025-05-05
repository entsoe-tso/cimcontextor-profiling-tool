using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using CimContextor.Integrity_Checking;
/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.9.6                                        *  december 2019 *
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
namespace CimContextor.utilitaires
{
    class Validations
    {

        EA.Repository repo = null;
        // ABA20230228 public static Utilitaires selfut = new Utilitaires();
        public XMLParser XMLP;
        public  List<string> Facets;
        ConstantDefinition CD = new ConstantDefinition();
        public static Dictionary<string, List<long>> dicProfElemByParentGuid = new Dictionary<string, List<long>>();
        public static List<EA.Element> ListElem = new List<EA.Element>();
        public static Dictionary<string, long> dicDatatypes = new Dictionary<string, long>();// datatypename/id 
        public Dictionary<string, List<long>> dicACCguidToABIEcon = new Dictionary<string, List<long>>();//<accvonguid/abieconid>
        public bool therearewarnings = false;
        public Utilitaires ul = null; // ABA20230228 new Utilitaires();
        public  int errors=0; // errors (issues) counter
        List<long> conAlreadyTreated = null;
        bool isESMP = false;  // specific to ESMP 
        public static List<ValidationEntry> validationEntries = new List<ValidationEntry>(); // ABA20221228

        public Validations(EA.Repository repos, EA.Package thepack) //constructor          
        {
            errors = 0;
            conAlreadyTreated = new List<long>();
            this.repo = repos;
            ul = new Utilitaires(this.repo); // ABA20230228
            ul.setRepo(repos);
             isESMP = false;
             XMLP = new XMLParser(repo);
             Facets= XMLP.GetXmlClassifierConstraintNames();
            if (XMLP.GetXmlValueConfig("EnableESMPHierarchy") == ("Checked"))
            {
                XMLP.AddXmlLog("IntegrityCheck", " info: the ESMP mark of hierarchy is enabled");
                validationEntries.Add(new ValidationEntry(Severity.INFO, ValidationCode.I001, "The ESMP mark of hierarchy is enabled", null, null, null));
                isESMP = true;
            }
            verif_package(repos, thepack);
          
        }

        // ABA 20230207
        public Validations(EA.Repository repos)
        {
            this.repo = repos;
        }

        public List<ValidationEntry>  ValidationEntries
        {
            get { return validationEntries; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// recurssive verification for each package, elements and association of a selected package;
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        public void verif_package(EA.Repository repos, EA.Package thepack)
        {
            verif_coherence_pa(repos, thepack);
            foreach (EA.Element el in thepack.Elements)
            {
                if (el.Name != "")
                {
                    verif_element(repos, thepack, el);
                }
                
            }
            foreach (EA.Package pa in thepack.Packages)
            {
                verif_package(repos, pa);
            }
           
        }
        public Validations() //constructor
        {
            this.repo = null;
        }
        public void setRepo(EA.Repository repos)
        {
            this.repo = repos;
        }
        /// <summary>
        /// to write log enties
        /// </summary>
        /// <param name="texte"></param>

        /// <summary>
        /// possible consistency verification at the level of package
        ///  a profile package should be 
        /// - either directly based on
        /// - or in a package isbased on
        /// - a package may be envelopp of other non based on package
        ///      -if it is basedon it will be consistent and all its subdirectory will bee
        ///      - if is not basedon it will be inconsistent with all non isbasedon subdirectory
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        void verif_coherence_pa(EA.Repository repos, EA.Package thepack)
        {
            /*****  profile must be basedon ****/
            long res = ul.getFirstIBOPackage(repos, thepack);
            string texte = " info:  ++++++++  the package " + thepack.Name + " is beeing checked for consistency ++++++++++++";
            
            ul.wlog("IntegrityCheck", texte);
            if (res == 0)
            {
                texte = " WARNING:  the package " + thepack.Name + " is not IsbasedOn and non of his parents is";
                validationEntries.Add(new ValidationEntry(Severity.WARNING, ValidationCode.W001, "The package " + thepack.Name + " is not 'IsBasedOn' and non of its parent packages!", thepack.Name, null, null));
                ul.wlog("IntegrityCheck", texte);
                return;
            }
            
            /*** all package of a profile must have a unique name ****/
            Dictionary<string, EA.Package> lpa = new Dictionary<string, EA.Package>();
            ul.getAllPackagesInAPackage(repos, thepack, lpa);
        }

        //-----------------------------------
        /// <summary>
        /// check the consistency of 
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        /// <param name="el"></param>
        void verif_element(EA.Repository repos, EA.Package thepack, EA.Element el)
        {
           // ul.wlog("IntegrityCheck", " ******************* entree dans verif_element " +  el.Name + "*******************************");
            string texte = "";
            if (el.Name == "")
            {
                texte = "";// pour test am aout 2016
            }
    
            string type = el.Type;
            EA.Element parentel = null;
            if ((el.Type == "Note") && (el.Name == ""))
            {
                texte = el.Notes;
                ul.wlog("IntegrityCheck", "info note beginning with " + texte.Substring(0, 10) + " ...");
                validationEntries.Add(new ValidationEntry(Severity.INFO, ValidationCode.I002, "Info note beginning with " + texte.Substring(0, 10) + " ...", null, null, null));
            }

            if (el.Type == "Class")
            {
                if (el.StereotypeEx.Contains(CD.GetPrimitiveStereotype())) return; // pas d'attributs
 
                if (el.StereotypeEx.Contains(CD.GetEnumStereotype()) && el.Name.Contains("TypeList"))
                {
                    return; //local to esmp
                }
                else
                {
                    parentel = verif_ClassIBOIntegrity(repos, thepack, el);
                    if (parentel == null) return; ; // IBO check not passed
                }

                if (el.StereotypeEx.Contains(CD.GetDatatypeStereotype()))
                {
                    bool resul = false;
                    foreach (EA.Attribute at in el.Attributes)
                    {
                        if (at.Name == "value")
                        {
                            resul = true;
                            break;
                        }
                    }
                    if (resul == true)
                    {
                        if (
                            parentel.StereotypeEx.Contains(CD.GetPrimitiveStereotype()))
                        {
                            // the IBO element is a primitive : no need to check GUID validity or names 
                            // except value
                            return;
                        }
                    }
                    else
                    {
                        // not normal we should find at least one attribute named "value"
                        ul.wlog("IntegrityCheck", " ISSUE: a CimDatatType must contain at leat an attribute named value");
                        errors++;
                        validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E005, "Missing attrribute named 'value' in CimDatatType (there must be at least one)!", thepack.Name, el.Name, null));
                    }
                }
                
               
                foreach (EA.Attribute at in el.Attributes)
                {
                    //if (el.Name == "ModificationLeadTime_Duration")  ABalka
                    //{
                    //    string gg = at.Name;
                    //}
                    verif_attribute(el, at); // check if this attribute is well based on and that it has the same name
                                             // useful also if one attribute has been deleted in Information Model
                }

                //***** check the associations     *****

                // if the element is a Class but is a datatype or enumeration it has no association
                // so this is the end of the program
                if (
                    el.StereotypeEx.Contains(CD.GetCompoundStereotype())
                    || el.StereotypeEx.Contains(CD.GetDatatypeStereotype())
                    || el.StereotypeEx.Contains(CD.GetEnumStereotype())
                    || el.StereotypeEx.Contains(CD.GetPrimitiveStereotype())
                    || (el.MetaType=="Enumeration")
                    || (el.Type=="Enumeration")
                    )
                {
                    return;
                }
                
                foreach (EA.Connector con in el.Connectors)
                {
                    verif_association(repo, thepack, el,con);
                }
                verif_rangmembres(el);

            }

        }

        private void verif_membervalue(EA.Element el, EA.Attribute at)
        {
             EA.Attribute parentat = ul.recupIBOAttribute(at);

             if (parentat == null) return; // already treated by verif-attribute
            
             if (parentat.ClassifierID !=0)
             {
                 EA.Element originaltype=repo.GetElementByID((int)parentat.ClassifierID);
             }
             {

                 if(at.ClassifierID==0)
                 {// the attribut has been affected a value and not an enumeration
                    // we must collect all the possible values
                     
                     

                 // int prov=1; ABA20221225

                 }
                 /* ABA20221225
                 if ( // le classifier est une enumeration on doit voir si la valeur initiale est conforme
                    el.StereotypeEx.Contains(CD.GetEnumStereotype())
                    || (el.MetaType == "Enumeration")
                    || (el.Type == "Enumeration")
                     )
                 {
                     int prov= 1;
                 }
                 */
             }

            if(parentat.Default != "")
            {
                if(at.Default != parentat.Default)
                {
                    ul.wlog("IntegrityCheck", " ISSUE: the default value of an element attribute whose type is an enumeration " + el.Name +"." + at.Name + " is different from his parent" );
                    errors++;
                }
            }
        }
        /// <summary>
        /// check if
        /// an attribute is correctly basedon
        /// its name is not changed
        /// check if it has an idenfier
        /// inform which identifier
        /// </summary>
        /// <param name="el"></param>
        /// <param name="at"></param>
        void verif_attribute(EA.Element el, EA.Attribute at)
        {
            EA.Attribute parentat = ul.recupIBOAttribute(at);
            
            if (parentat == null)

            {
                EA.Element iboEl = ul.recupIBOElement(el); // ABA20221223
                if ((iboEl == null) || (!iboEl.StereotypeEx.Contains(CD.GetPrimitiveStereotype())))
                {
                    ul.wlog("IntegrityCheck", " ISSUE:  IsBasedOn attribute " + at.Name + " of element "+ el.Name+ " is not found");
                    errors++;
                    validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E006, "IsBasedOn attribute " + at.Name + " of element "+ el.Name+ " is not found!", null, el.Name, at.Name));
                    return;
                }
            }
            else
            {
                if (parentat.Name != at.Name)
                {
                    ul.wlog("IntegrityCheck", " ISSUE: for element " + el.Name + " attribute name " + at.Name
                        + " different from its CIM parent attribute " + parentat.Name);
                    errors++;
                    validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E007, "The attribute name " + at.Name + " of the element " + el.Name +
                        " is different from its CIM parent attribute " + parentat.Name, null, el.Name, at.Name));
                }
            }
            // information on identifier
            if (XMLP.GetXmlValueConfig("EnableCheckAttributeIdentifier")== "Checked")
            {
                if (at.ClassifierID != 0)
                {
                    if (at.ClassifierID != parentat.ClassifierID)
                    {
                        ul.wlog("IntegrityCheck", "info: the classifier of attribute " + el.Name + "." + at.Name + " is " + repo.GetElementByID((int)at.ClassifierID).Name + " is different from the classifier of its parent attribute");
                        errors++;
                        validationEntries.Add(new ValidationEntry(Severity.INFO, ValidationCode.I004, "The classifier of attribute " + el.Name + "." + at.Name + " is " + repo.GetElementByID((int)at.ClassifierID).Name + " and is different from the classifier of its parent attribute!", null, el.Name, at.Name));
                    }
                    try
                    {
                        repo.GetElementByID((int)at.ClassifierID);

                        ul.wlog("IntegrityCheck", "info: the type of attribute " + el.Name + "." + at.Name + " is " + repo.GetElementByID((int)at.ClassifierID).Name);
                        errors++;
                        validationEntries.Add(new ValidationEntry(Severity.INFO, ValidationCode.I005, "The type of attribute " + el.Name + "." + at.Name + " is " + 
                            repo.GetElementByID((int)at.ClassifierID).Name, null, el.Name, at.Name));
                    }
                    catch (Exception e)
                    {
                        ul.wlog("IntegrityCheck", "ISSUE: problem in accessing an attribute's classifier for: " + el.Name + "." + at.Name + "  " + e.Message);
                        validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E008, "Error on accessing an attribute's classifier for: " + el.Name + "." + at.Name + 
                            "  " + e.Message, null, el.Name, at.Name));
                        return;
                    }
                }
            }
           
            EA.Element parentattype=null;
            if (parentat.ClassifierID != 0)
            {// there is a type for the SI
                parentattype = repo.GetElementByID((int)parentat.ClassifierID);
                if (at.ClassifierID != 0)
                {
                    EA.Element classifier = repo.GetElementByID((int)at.ClassifierID);

                    //we must decide if this classifier is in the profile or in the datatype Domain (for ESMP)

                    if (XMLP.GetXmlValueConfig("EnableESMPHierarchy")=="Checked")
                    {
                        bool isInProfile = true;
                        if (
                        classifier.StereotypeEx.Contains(CD.GetEnumStereotype())
                       || (classifier.MetaType == "Enumeration")
                       || (classifier.Type == "Enumeration")
                       )
                        {
                             isInProfile = ul.isClassifierInProfile(el, classifier, "ESMPEnum");
                        }
                        else
                        {
                             isInProfile = ul.isClassifierInProfile(el, classifier, "ESMDatatype");
                        }
                        if (!isInProfile)
                        {
                            ul.wlog("IntegrityCheck", "WARNING: the type of attribute " + el.Name + "." + at.Name + " is " + repo.GetElementByID((int)at.ClassifierID).Name + " is not included  in the profile or Datatype Domain  ");
                            errors++;
                            validationEntries.Add(new ValidationEntry(Severity.WARNING, ValidationCode.W007, "The type of attribute " + el.Name + "." + at.Name + " is " + repo.GetElementByID((int)at.ClassifierID).Name + 
                                " is not included  in the profile or Datatype Domain!", null, el.Name, at.Name));
                        }
                    }

                    // if the element is an enumaration the initial values must be compliant
                    if (
                         parentattype.StereotypeEx.Contains(CD.GetEnumStereotype())
                        || (parentattype.MetaType == "Enumeration")
                        || (parentattype.Type == "Enumeration")
                        )
                    { // the parenttype is an enumeration we must check the value of the attribute
                      // this might be a value or a default value from a global or local enumeration
                        EA.Element attype = repo.GetElementByID((int)at.ClassifierID);
                        if (
                        (!attype.StereotypeEx.Contains(CD.GetEnumStereotype()))
                        && (attype.MetaType != "Enumeration")
                        && (attype.Type != "Enumeration")
                            )
                        {
                            ul.wlog("IntegrityCheck", "ISSUE: the type of attribute " + el.Name + "." + at.Name + " is " + repo.GetElementByID((int)at.ClassifierID).Name + " is not is not of the same type than the CIM one or is not an Enumeration");
                            errors++;
                            validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E009, "The type of attribute " + el.Name + "." + at.Name + 
                                " is " + repo.GetElementByID((int)at.ClassifierID).Name + 
                                " and is not of the same type as the CIM one or is not an Enumeration", null, el.Name, at.Name));
                            return;
                        }
                        //  isInProfile=checkClassifierInProfile(attype); // réservé à ENTSOE
                        // collect all possible values
                        Dictionary<string, string> dicEnumValues = new Dictionary<string, string>(); // dictionnaire member/value
                        foreach (EA.Attribute member in attype.AttributesEx)
                        {
                            dicEnumValues[member.Name] = member.Default;
                        }
                        if (
                            (at.Default != "")
                            &&
                            (!dicEnumValues.ContainsKey(at.Default))
                            &&
                            (!dicEnumValues.ContainsValue(at.Default))
                            )

                        {
                            ul.wlog("IntegrityCheck", "ISSUE: the type of attribute " + el.Name + "." + at.Name + " has not a compliant value " + at.Default);
                            errors++;
                            validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E010, "The type of attribute " + el.Name + "." + at.Name + " has not a compliant value " + at.Default, null, el.Name, at.Name));
                        }
                    }
                }
            }
        }

        private bool checkClassifierInProfile(EA.Element attype)
        {
 	        throw new NotImplementedException();
        }
        
        /// <summary>

        /// <summary>
        /// check if class has a GUID tag value and that it corresponds to a class 
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        /// <param name="el"></param>
        EA.Element verif_ClassIBOIntegrity(EA.Repository repos, EA.Package thepack, EA.Element el)
        {
           
            string parentguid = ul.getEltParentGuid(el);
            EA.Element parentel = null;  // the IBO element
            if (isESMP && el.StereotypeEx.Contains(CD.GetEnumStereotype()))
            {// case of ESMP and the name of enumeration ends with TypeList
                if (el.Name.Contains("TypeList")) return parentel;
            }
            if (parentguid == "")
            {
                ul.wlog("IntegrityCheck", " ISSUE: the element " + el.Name + " is not IsBasedOn (CIM element canceled?");
                errors++;
                validationEntries.Add(new ValidationEntry(Severity.WARNING, ValidationCode.W003, "The element " + el.Name + " is not IsBasedOn!", thepack.Name, el.Name, null));
            }
            else
            {
                try
                {
                     parentel = repo.GetElementByGuid(parentguid);
                    
                    long dependid = ul.getIBOElementIDFromDependency(el);
                    //check if the parentelement found is basedon dependent with el
                    if (dependid == 0)
                    {
                        ul.wlog("IntegrityCheck", " ISSUE: there is no (or too many) IsBasedon dependency link for the element " + el.Name);
                        errors++;
                        validationEntries.Add(new ValidationEntry(Severity.WARNING, ValidationCode.W004, "there is no (or more than one) IsBasedOn dependency link for the element " + el.Name, thepack.Name, el.Name, null));
                        // the isbased on dependency is not required , so we move on
                    }
                    else
                    {
                        if (dependid != parentel.ElementID)
                        {
                            ul.wlog("IntegrityCheck", " ISSUE: for the element " + el.Name +
                            "the parent element from GUIDBasedOn TaggedValue " + repo.GetPackageByID((int)parentel.PackageID).Name +
                            "::" + parentel.Name + " is diffentent from the supplier element of the IsBasedOn dependency link");
                            errors++;
                            validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E002, "For the element " + el.Name +
                            " the parent element from GUIDBasedOn TaggedValue " + repo.GetPackageByID((int)parentel.PackageID).Name +
                            "::" + parentel.Name + " is different from the supplier element of the IsBasedOn dependency link!", thepack.Name, el.Name, null));
                            return null;
                        }
                    }

                    if (el.StereotypeEx.Contains(CD.GetDatatypeStereotype()))
                    {
                        // Cimdatatypes should  be basedon primitive classes or another CimDataType class
                        if ((!parentel.StereotypeEx.Contains(CD.GetPrimitiveStereotype()))
                            && (!parentel.StereotypeEx.Contains(CD.GetDatatypeStereotype()))) 
                        
                        {
                            ul.wlog("IntegrityCheck", " ISSUE: non consistency between element " + el.Name +
                             "(Cimdatatype) and non primitive parent element " + parentel.Name);
                            errors++;
                            validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E003, "Non consistency between element " + el.Name +
                             " (Cimdatatype) and non primitive parent element " + parentel.Name, thepack.Name, el.Name, null));
                            return null;
                        }
                        else
                        {
                            // check if there is an attribute value and well typed
                            long classifierid = ul.getDatatypeValueIdentifierID(el);
                            if (classifierid == 0)
                            {
                                ul.wlog("IntegrityCheck", " ISSUE: attribute value of  a Cimdatatype " + el.Name +
                          " is not properly typed");
                            errors++;
                                validationEntries.Add(new ValidationEntry(Severity.WARNING, ValidationCode.W005, "Attribute value of a Cimdatatype " + el.Name +
                                                        " is not properly typed!", thepack.Name, el.Name, null));
                            }
                            else
                            {
                                EA.Element classifier = repo.GetElementByID((int)classifierid);

                                ul.wlog("IntegrityCheck", " info: attribute value of element  " + el.Name +
                              "(Cimdatatype) is typed with "
                                          + repo.GetPackageByID((int)classifier.PackageID).Name
                                          + "::"
                                          + classifier.Name
                                              );
                                validationEntries.Add(new ValidationEntry(Severity.INFO, ValidationCode.I003, "Attribute value of element  " + el.Name +
                                    "(Cimdatatype) is typed with "
                                    + repo.GetPackageByID((int)classifier.PackageID).Name + "::" + classifier.Name, thepack.Name, el.Name, null));
                            }
                        }
                    }
                    else
                    {
                        if (ul.RemoveQual(el.Name) != ul.RemoveQual(parentel.Name))
                        {
                            ul.wlog("IntegrityCheck", " ISSUE: non consistency between element " + el.Name
                                + " parent element " + parentel.Name);
                            errors++;
                            validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E004, "Non consistency between element " + el.Name
                                + " parent element " + parentel.Name, thepack.Name, el.Name, null));
                        }
                        if(!ul.haveSimilarStereotype(parentel.StereotypeEx, el.StereotypeEx))
                        {
                             ul.wlog("IntegrityCheck", " WARNING: non consistency in list of stereotypes  between element " + el.Name
                                + " and its parent element " );
                            validationEntries.Add(new ValidationEntry(Severity.WARNING, ValidationCode.W006, "Non consistency in list of stereotypes  between element " + el.Name
                                + " and its parent element!", thepack.Name, el.Name, null));
                            
                        }
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(" validation issue: get IBO element for " + el.Name + " " + e.Message);
                    ul.wlog(" IntegrityCheck", " issue: get IBO element for " + el.Name + " " + e.Message);
                    errors++;
                    parentel = null;
                }

            }
            return parentel;

        }
        /// <summary>
        /// check validity of members order of a class
        /// </summary>
        /// <param name="el"></param>
        void verif_rangmembres(EA.Element el)
        {
            if (!isESMP) return;
            List<int> rangs = new List<int>();
            int rg = -1;
            foreach (EA.Attribute at in el.Attributes)
            {
                foreach (EA.AttributeTag tag in at.TaggedValues)
                {
                    if (tag.Name == CD.GetRangTagValue())
                    {
                        rg = System.Convert.ToInt32(tag.Value);
                        break;
                    }
                }
                if (rg == -1)
                {
                    ul.wlog("IntegrityCheck", "the ESMPG TaggedValue is missing for " + el.Name + "." + at.Name);
                    validationEntries.Add(new ValidationEntry(Severity.WARNING, ValidationCode.W009, "The ESMPG tagged value is missing for " + el.Name + "." + at.Name + "!", null, el.Name, at.Name));
                }
                else
                {
                    if (!rangs.Contains(rg))
                    {
                        rangs.Add(rg);
                    }
                    else
                    {
                        ul.wlog("IntegrityCheck", "ISSUE: the order" + rg + " already exits  for  " + el.Name + "." + at.Name);
                        errors++;
                        validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E019, "The order " + rg + " already exits  for  " + el.Name + "." + at.Name + "!", null, el.Name, at.Name));
                    }
                }
            }
            foreach (EA.Connector con in el.Connectors)
            {
                if (
                    ((con.Type == "Association") || (con.Type == "Aggregation"))
                    &&
                    (((con.ClientID == el.ElementID) && (con.SupplierEnd.Role != "")
                    && ((con.ClientEnd.Aggregation == 1) || (con.SupplierEnd.IsNavigable))) // am sept 201                       ||
                    ||
                    ((con.SupplierID == el.ElementID) && (con.ClientEnd.Role != "")
                     && ((con.SupplierEnd.Aggregation == 1) || (con.ClientEnd.IsNavigable))) // am sept 2013
                    )
                    )
                {
                    rg = -1;
                    foreach (EA.ConnectorTag tag in con.TaggedValues)
                    {
                        if (tag.Name == CD.GetRangTagValue())
                        {
                            rg = System.Convert.ToInt32(tag.Value);
                            break;
                          
                        }
                    }
                    if (rg == -1)
                    {
                        ul.wlog("IntegrityCheck", " WARNING: the order tagvalue is missing for  "
                            + el.Name + "con ("+ con.ClientEnd.Role + " -> " + con.SupplierEnd.Role +" )" );
                        validationEntries.Add(new ValidationEntry(Severity.WARNING, ValidationCode.W010, "The order tagged value is missing for "
                            + el.Name + "'s connector ("+ con.ClientEnd.Role + " -> " + con.SupplierEnd.Role + " )!", null, el.Name, null));
                    }
                    else
                    {
                        if (!rangs.Contains(rg))
                        {
                            rangs.Add(rg);
                        }
                        else
                        {
                            ul.wlog("IntegrityCheck", "ISSUE: the order" + rg + " already exits  for  " + el.Name + "con (" + con.ClientEnd.Role + " -> " + con.SupplierEnd.Role + " )");
                            errors++;
                            validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E020,"The order " + rg + " already exits for " + el.Name + "'s connector (" + con.ClientEnd.Role + " -> " + con.SupplierEnd.Role + ")!", null, el.Name, null));
                        }
                    }
                }
            }
        }


        //----------------------------------------------
         ///<summary>
        /// this program check every association of a profile
        /// an association:
        /// must be based on an association
        /// each ends of it must be based on each resppecive end
        /// if there is a role without label the oposite label must be non empty 
        ///    and derived , possibly by a qualifier , from the isbased on role
        ///    more generally a ole is always derived from the original role
        ///    there must be either a sign of aggregation or an arrow
        ///    on the side opposite the aggregation the cardinality must be within the original limit
         /// </summary>
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        /// <param name="el"></param>
        /// <param name="con"></param>
        void verif_association(EA.Repository repos, EA.Package thepack, EA.Element el, EA.Connector con)
        {
            if ((con.Type != "Association") && (con.Type != "Aggregation"))
            {
                return;
            }
            if (conAlreadyTreated.Contains(con.ConnectorID))
            {
                return; // already done
            }
            else
            {
                conAlreadyTreated.Add(con.ConnectorID); // it is now treated
            }
            
            EA.Connector parentcon = ul.recupIBOConnector(el, con);
            if (parentcon == null)
            {
                ul.wlog(" IntegrityCheck", " ISSUE: the connector of  " 
                    + el.Name +  " " + con.ClientEnd.Role + " --- " + con.SupplierEnd.Role + " not properly basedon");
                errors++;
                validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E011, "The connector of  " 
                    + el.Name +  " " + con.ClientEnd.Role + " --- " + con.SupplierEnd.Role + " is not properly based on!", null, el.Name, null));
                return;
            }
            // if (el.Name == "Bay") ABA20221225
            // {
            //     int a = 1;
            // }
            EA.Element source = repo.GetElementByID((int)con.ClientID);
                 string sourcerole=con.ClientEnd.Role;
            EA.Element target = repo.GetElementByID((int)con.SupplierID);
                 string targetrole=con.SupplierEnd.Role;

            EA.Element parentsource = repo.GetElementByID((int)parentcon.ClientID);     
	    // string parentsourcerole=parentcon.ClientEnd.Role; ABA20221225       
            EA.Element parenttarget = repo.GetElementByID((int)parentcon.SupplierID);
            // string parenttargetrole=parentcon.SupplierEnd.Role; ABA20221225

            EA.Element desource = ul.recupIBOElement(source);
            if(desource==null)
            {
                ul.wlog("IntegrityCheck", " ISSUE: the source " + source.Name + " of one connector  for "
                      + el.Name + " is not properly BasedOn on the source of the IBO connector");
                errors++;
                validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E012, "The source " + source.Name + " of one connector of "
                      + el.Name + " is not properly BasedOn on the source of the IBO connector!", null, el.Name, null));
                return;
            }
            // string desourcerole=""; ABA20221225
            EA.Element detarget = ul.recupIBOElement(target);
            if (detarget == null)
            {
                ul.wlog("IntegrityCheck", " ISSUE: the target " + target.Name + " of one connector  for "
                      + el.Name + " is not properly based on on the target of the IBO connector");
                errors++;
                validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E013, "The target " + target.Name + " of one connector of "
                      + el.Name + " is not properly based on on the target of the IBO connector", null, el.Name, null));
                return;
            }
            // string detargetrole=""; ABA20221225

            // ABA20221225
            // string assocdir =""; // can be SS if the source is parent with the source or ST if the source is parent with target
            EA.ConnectorEnd parentsourceend = null;
            EA.ConnectorEnd parenttargetend = null;            
            
            // each parent ends must be in hierarchy with the other
	        bool  isOK=false;
            if  (ul.isInRelation(desource, parentsource)) // this means that parentsource and desource are related, detarget and  parenttarget must be also in relation
            {                                
		        if (ul.isInRelation(detarget, parenttarget))	
                {                   
                    parentsourceend=parentcon.ClientEnd;
                    parenttargetend=parentcon.SupplierEnd;
			        isOK=true;
                } 
                else
                {
                    ul.wlog("IntegrityCheck", " ISSUE: the target " + target.Name + " of one connector  for "
                    + el.Name + " is not properly based on on the target of the IBO connector");
                    errors++;
                    validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E014, "The target " + target.Name + " of one connector of "
                    + el.Name + " is not properly based on on the target of the IBO connector!", null, el.Name, null));
                    return;
                }
		    }
            else
            {
                if	 (ul.isInRelation(desource, parenttarget)) // this means that parenttarget and desource are related, detarget and  parentsource must be also in relation
                {
			        if (ul.isInRelation(detarget, parentsource))
                    {
    			        parentsourceend=parentcon.SupplierEnd;
                        parenttargetend=parentcon.ClientEnd; 
                        isOK=true;
                    }
                    else
                    {
                        ul.wlog("IntegrityCheck", " ISSUE: the source " + source.Name + " of one connector  for "
                            + el.Name + " is not properly based on on the source of the IBO connector");
                        errors++;
                        validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E015, "The source " + source.Name + " of one connector of "
                            + el.Name + " is not properly based on on the source of the IBO connector!", null, el.Name, null));
                        return;
                    }
    		    }	
	    	}  
                   

                if (parenttarget.ElementID == parentsource.ElementID)
                {
                    ul.wlog("IntegrityCheck", "WARNING: the test about hierarchy has been excluded for the  association between "
                      + source.Name + " and " + target.Name + " because based on  a recursive association: check if necessary");
                    validationEntries.Add(new ValidationEntry(Severity.WARNING, ValidationCode.W008, "The test about hierarchy has been excluded for the  association between "
                      + source.Name + " and " + target.Name + " because based on  a recursive association: check if necessary!", null, el.Name, null));
                    return;// the recursive associations must be excluded from the following  tests
                }
                    if (((sourcerole != "")
                                       && (ul.RemoveDQual(sourcerole) != (ul.RemoveDQual(parentsourceend.Role))))
                            ||
                     ((targetrole != "")
                                       && (ul.RemoveDQual(targetrole) != (ul.RemoveDQual(parenttargetend.Role)))))
                    {
                        ul.wlog("IntegrityCheck", " ISSUE: for the association of element " + el.Name + " " +
                             source.Name + "." + sourcerole + " towards " + target.Name + "." + targetrole +
                           " there is an inconsistency in end role name with the IsBasedOn association");
                        errors++;
                        validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E016, "For the association of element " + el.Name + " " +
                                     source.Name + "." + sourcerole + " to " + target.Name + "." + targetrole +
                                   " there is an inconsistency in end role names with the IsBasedOn association", null, el.Name, null));
                        return;
                    }
                
          // test of theconsistency of  hierarchy in all possible cases
	                 string upperend="";
                     string parentupperend="";
                       isOK = false;
                       if(
			((con.SupplierEnd.Aggregation == 1) && (con.ClientEnd.Aggregation == 0))                    
                         ||
                         ((con.SupplierEnd.Role=="")&& (con.ClientEnd.Role!=""))
			             ||
                         ((con.SupplierEnd.Cardinality == "1") &&
                         ( (con.ClientEnd.Cardinality != "0..1") || con.ClientEnd.IsNavigable))
                         ||
                         ((con.SupplierEnd.Cardinality == "0..1") && 
                             ((con.ClientEnd.Cardinality !="")
                            && (con.ClientEnd.Cardinality != "0")
                            && (con.ClientEnd.Cardinality != "0..1")
                            && (con.ClientEnd.Cardinality != "1"))
                            || con.ClientEnd.IsNavigable)
                          )
                    {
                        isOK = true;
                        upperend = "target";
                    }else{
			 if(
			((con.ClientEnd.Aggregation == 1) && (con.SupplierEnd.Aggregation == 0))                    
                         ||
                         ((con.ClientEnd.Role=="")&& (con.SupplierEnd.Role!=""))
			             ||
                         ((con.ClientEnd.Cardinality == "1") &&
                         ( ((con.SupplierEnd.Cardinality != "") &&
                            (con.SupplierEnd.Cardinality != "0..1")) || con.SupplierEnd.IsNavigable))
                         ||
                         ((con.ClientEnd.Cardinality == "0..1") &&
                           ( (con.SupplierEnd.Cardinality !="")
                            &&(con.SupplierEnd.Cardinality != "0")
                            && (con.SupplierEnd.Cardinality != "0..1")
                            && (con.SupplierEnd.Cardinality != "1")
                            || con.SupplierEnd.IsNavigable)))
                 
                    {
                        isOK = true;
                        upperend = "source";
                    }
                  }
                   if(!isOK)
                    {
                       string texte = "ISSUE: the hierarchy of one association ( "
                        + con.ClientEnd.Role + "," + con.SupplierEnd.Role + " )"
                      + "from " + el.Name + " is not valid";
                      ul.wlog("IntegrityCheck", texte);
                     errors++;
                        validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E017, "The hierarchy of one association ("
                            + con.ClientEnd.Role + "," + con.SupplierEnd.Role + ")"
                          + " from " + el.Name + " is not valid!", null, el.Name, null));
                     return;
                    }
               
                    // if there is a hierarchy in the isbasedon association it must be respected
                    /* ABA20221225
                   if (el.Name == "MarketParticipant")
                   {
                       string prov = "ee";
                      
                   }
                    */
                        if(
			         ((parenttargetend.Aggregation == 1) && (parentsourceend.Aggregation == 0))                    
                         ||
                         ((parenttargetend.Role=="")&& (parentsourceend.Role!=""))
			              ||
                         ((parenttargetend.Cardinality == "1") &&
                         ( (parentsourceend.Cardinality != "0..1") || parentsourceend.IsNavigable))
                         ||
                         ((parenttargetend.Cardinality == "0..1") &&
                           ((parentsourceend.Cardinality != "")
                            && (parentsourceend.Cardinality != "0")
                            && (parentsourceend.Cardinality != "0..1")
                            && (parentsourceend.Cardinality != "1")
                           // && (parentsourceend.Cardinality != "1..1")
                            || parentsourceend.IsNavigable))
                     
                          )
                    {
                        parentupperend = "target";
                    }else{
		     if(
			((parentsourceend.Aggregation == 1) && (parenttargetend.Aggregation == 0))                    
                         ||
                         ((parentsourceend.Role=="")&& (parenttargetend.Role!=""))
			              ||
                         ((parentsourceend.Cardinality == "1") &&
                         ( (parenttargetend.Cardinality != "0..1") || parenttargetend.IsNavigable))
                         ||
                         ((parentsourceend.Cardinality == "0..1") &&
                           ( (parenttargetend.Cardinality != "")
                            &&(parenttargetend.Cardinality != "0")
                            && (parenttargetend.Cardinality != "0..1")
                            && (parenttargetend.Cardinality != "1")
                           // && (parenttargetend.Cardinality != "1..1")
                            || parenttargetend.IsNavigable))
                          )
                    {
                        parentupperend = "source";
                    }
                  }
                        isOK = false;
                        EA.Element parentel = repo.GetElementByGuid(ul.getEltParentGuid(el));
                        if (!ul.isAFirstLevelPackage(repo, el.PackageID))
                        { // if we are in a second level hierarchy we must respect the upper level one
                            
                            if (   // l'element el son parent sont tous les deux sources ou tous les deux targets
                                // ils doivent etre la meme extremite de la hierarchie
                                ((con.ClientID == el.ElementID)
                                &&
                                (parentcon.ClientID == parentel.ElementID))

                                ||
                              ((con.ClientID != el.ElementID)
                                &&
                                (parentcon.ClientID != parentel.ElementID))

                                 )
                            {
                                if (upperend == parentupperend)
                                {
                                    isOK = true;
                                }
                            }
                            else  // les roles sont inverse
                            {
                                if (upperend != parentupperend)
                                {
                                    isOK = true;
                                }

                            }
                                if (!isOK)
                                {
                                    string texte = "ISSUE: the hierarchy of one association ( "
                                     + con.ClientEnd.Role + "," + con.SupplierEnd.Role + " )"
                                   + "from " + el.Name + " should exist or reflects upperlevel hierarchy";
                                    ul.wlog("IntegrityCheck", texte);
                                        validationEntries.Add(new ValidationEntry(Severity.ERROR, ValidationCode.E018, "The hierarchy of one association ("
                                         + con.ClientEnd.Role + "," + con.SupplierEnd.Role + ")"
                                       + " from " + el.Name + " should exist or reflect upperlevel hierarchy!", null, el.Name, null));
                                    errors++;
                                    return;
                                }
                            
                        }
        }

        //-----------------------------------
        ///<summary>
        /// this program check every association of a profile
        /// an association:
        /// must be based on an association
        /// each ends of it must be based on each resppecive end
        /// if there is a role without label the oposite label must be non empty 
        ///    and derived , possibly by a qualifier , from the isbased on role
        ///    more generally a ole is always derived from the original role
        ///    there must be either a sign of aggregation or an arrow
        ///    on the side opposite the aggregation the cardinality must be within the original limit
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        /// <param name="el"></param>
        /// <param name="con"></param>
        void oldverif_association(EA.Repository repos, EA.Package thepack, EA.Element el, EA.Connector con)
        {
            if ((con.Type != "Association") && (con.Type != "Aggregation"))
            {
                return;
            }
            if (conAlreadyTreated.Contains(con.ConnectorID))
            {
                return; // already done
            }
            else
            {
                conAlreadyTreated.Add(con.ConnectorID); // it is now treated
            }
            EA.Connector parentcon = ul.recupIBOConnector(el, con);
            if (parentcon == null)
            {
                ul.wlog(" IntegrityCheck", " ISSUE: there is a connector of  " + el.Name + " not properly basedon");
                errors++;
                return;
            }

            EA.Element source = repo.GetElementByID((int)con.ClientID);

            EA.Element target = repo.GetElementByID((int)con.SupplierID);

            EA.Element parentsource = repo.GetElementByID((int)parentcon.ClientID);
            EA.Element parenttarget = repo.GetElementByID((int)parentcon.SupplierID);

            EA.Element desource = ul.recupIBOElement(source);
            EA.Element detarget = ul.recupIBOElement(target);

            EA.ConnectorEnd parentsourceend = null;
            EA.ConnectorEnd parenttargetend = null;


            // one of the ends is not correctly based on

            if (desource.ElementID == parentsource.ElementID)
            {
                // parent is in fact client
                parentsourceend = parentcon.ClientEnd;

            }
            else
            {
                if (desource.ElementID == parenttarget.ElementID)
                {
                    parentsourceend = parentcon.SupplierEnd;
                }
                else
                {
                    if (ul.isInRelation(desource, parentsource))
                    {
                        parentsourceend = parentcon.ClientEnd;
                    }
                    else
                    {
                        if (ul.isInRelation(desource, parenttarget))
                        {
                            parentsourceend = parentcon.SupplierEnd;
                        }
                        else
                        {
                            ul.wlog("IntegrityCheck", " ISSUE: the source " + source.Name + " of one connector  for "
                       + el.Name + " is not properly based on on the source of the IBO connector");
                            errors++;
                            return;
                        }
                    }
                }
            }
            if (detarget.ElementID == parenttarget.ElementID)
            {
                // parent is in fact client
                parenttargetend = parentcon.SupplierEnd;

            }
            else
            {
                if (detarget.ElementID == parentsource.ElementID)
                {
                    parenttargetend = parentcon.ClientEnd;
                }
                else
                {
                    if (ul.isInRelation(detarget, parenttarget))
                    {
                        parenttargetend = parentcon.SupplierEnd;
                    }
                    else
                    {
                        if (ul.isInRelation(detarget, parentsource))
                        {
                            parenttargetend = parentcon.ClientEnd;
                        }
                        else
                        {
                            ul.wlog("IntegrityCheck", " ISSUE: the target " + target.Name + " of one connector  for "
                       + el.Name + " is not properly based on on the source of the IBO connector");
                            errors++;
                            return;
                        }
                    }
                }
            }

            //ther must be a hierarchy
            // either an aggregate mark for ESMP, or a cardinality=one or a navigable sign

            string upperend = ""; //will remember hierarchy direction (source,target)
            bool isOK = false; // the hierarchy is of
            if ( isESMP)
            {
                if (con.SupplierEnd.Aggregation == 1)// sharable
                {
                    if (con.ClientEnd.Aggregation == 0)
                    {
                        isOK = true;
                        upperend = "target";
                    }
                }
                else
                {
                    if ((con.ClientEnd.Aggregation == 1) && (con.SupplierEnd.Aggregation == 0))
                    {
                        isOK = true;
                        upperend = "source";
                    }
                }
                if (!isOK)
                {
                    string texte = "ISSUE: the hierarchy of one association ( "
                        + repo.GetElementByID((int)con.ClientID).Name + "." + con.ClientEnd.Role
                        + " --- "
                        + repo.GetElementByID((int)con.SupplierID).Name + "." + con.SupplierEnd.Role + " )"
                    + " from " + el.Name + " is not valid";
                    ul.wlog("IntegrityCheck", texte);
                    errors++;
                    return;
                }
            }
            else
            {// not an ESMP checking
                if ((con.SupplierEnd.Aggregation == 1) && (con.ClientEnd.Aggregation == 0))
                {
                    isOK = true;
                    upperend = "target";
                }
                else
                {
                    if ((con.ClientEnd.Aggregation == 1) && (con.SupplierEnd.Aggregation == 0))
                    {
                        isOK = true;
                        upperend = "source";
                    }
                    else
                    {
                        if (((con.SupplierEnd.Cardinality == "1") || (con.SupplierEnd.Cardinality == "0..1"))
                                && ((con.ClientEnd.Cardinality != "0")
                                && (con.ClientEnd.Cardinality != "0..1")
                                && (con.ClientEnd.Cardinality != "1")
                                || con.ClientEnd.IsNavigable))
                        {
                            isOK = true;
                            upperend = "target";
                        }
                        else
                        {
                            if (((con.ClientEnd.Cardinality == "1") || (con.ClientEnd.Cardinality == "0..1"))
                                && ((con.SupplierEnd.Cardinality != "0")
                                && (con.SupplierEnd.Cardinality != "0..1")
                                && (con.SupplierEnd.Cardinality != "1"))
                                || con.SupplierEnd.IsNavigable)
                            {
                                isOK = true;
                                upperend = "source";
                            }
                            else
                            {
                                if ((con.SupplierEnd.IsNavigable) && (!con.ClientEnd.IsNavigable))
                                {
                                    isOK = true;
                                    upperend = "source";
                                }
                                else
                                {
                                    if ((con.ClientEnd.IsNavigable) && (!con.SupplierEnd.IsNavigable))
                                        isOK = true;
                                    upperend = "target";
                                }
                            }

                        }
                    }
                }
                if (!isOK)
                {
                    string texte = "ISSUE: the hierarchy of one association ( "
                        + con.ClientEnd.Role + "," + con.SupplierEnd.Role + " )"
                    + "from " + el.Name + " is not vald";
                    ul.wlog("IntegrityCheck", texte);
                    errors++;
                    return;
                }
            } // end ESMP or NOT
            // the name of the roles must be consistant
            isOK = false;

            if (upperend == "source") //top hierarchy is source
            {
                if ((ul.RemoveQual(con.SupplierEnd.Role) != "")
                    && (ul.RemoveQual(parenttargetend.Role) != "")
                    && (ul.RemoveQual(con.SupplierEnd.Role) == ul.RemoveQual(parenttargetend.Role)))
                {
                    isOK = true;
                }
            }
            else
            {
                if (upperend == "target") //top hierarchy is target
                {
                    if ((ul.RemoveQual(con.ClientEnd.Role) != "")
                        && (ul.RemoveQual(parentsourceend.Role) != "")
                        && (ul.RemoveQual(con.ClientEnd.Role) == ul.RemoveQual(parentsourceend.Role)))
                    {
                        isOK = true;
                    }
                }
            }

            if (!isOK)
            {
                string texte = "ISSUE: one of the label of Roles  (" + con.ClientEnd.Role
              + "," + con.SupplierEnd.Role + ") of one association for the element "
              + el.Name + " is not consistant with the IBO association";
                ul.wlog("Integrity", texte);

                errors++;
                return;
            }
            // the cardinality must be within the limites
            string multiplicity;
            string parentmultiplicity;
            if (upperend == "source")
            {
                multiplicity = con.SupplierEnd.Cardinality; // we consider the otherend
                parentmultiplicity = parenttargetend.Cardinality;
            }
            else
            {
                multiplicity = con.ClientEnd.Cardinality;
                parentmultiplicity = parentsourceend.Cardinality;
            }
            isOK = ul.IsCompatibleCardinality(multiplicity, parentmultiplicity);
            if (!isOK)
            {
                ul.wlog("IntegrityCheck", "ISSUE: in the association"
                    + repo.GetElementByID((int)con.ClientID).Name
                    + " > "
                    + repo.GetElementByID((int)con.SupplierID).Name
                    + " the cardinality in the profile " + multiplicity
                    + " is not compatible with the IBO multiplicity " + parentmultiplicity);
                errors++;
                return;
            }

        }
        //--------------------------------------------------------------------
    }

}
