#region using
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using CimContextor.utilitaires;
using CimContextor.Configuration;
using CimContextor.Utilities;
#endregion
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
    public class EAClass
    {
        #region variables
        private ConstantDefinition CD = new ConstantDefinition();
        private string parentElementGUID;
        private string childElementGUID;
        private int parentElementID;
        private bool isAbstract = false;
        private ArrayList attributeList = new ArrayList();
        private ArrayList constraintList = new ArrayList();
        private string classQualifier = "";
        private EA.Repository repo;
        private EA.Diagram targetedDiagram;
        private EA.Package targetedPackage;
        private EA.Element IBOElement = null;
        private string classStereotype;
        private string Mode;
        private int ParentObjectTop;
        private int ParentObjectBottom;
        private int ParentObjectRight;
        private int ParentObjectLeft;
        private string ParentObjectStyle;
        private bool ParentWasHere = false;
        private bool IsRoot = false;
        private ArrayList PossibleInheritance = new ArrayList();
        private ArrayList FilteredPossibleInheritance = new ArrayList(); // am may 2011
        private EA.Element SelectedInheritance = null;
        private XMLParser XMLP = null;
        public Dictionary<string, EAClassAttribute> dicAttributeByGuid; //am avr 2019
        public Dictionary<string, EA.Attribute> dicParentEltAttributeByName;//am avr 19
        #endregion

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="repo">repository of enterprise architect</param>
        /// <param name="targetedDiagram">the selected diagram</param>
        /// <param name="parentElementGUID">the Guid of the parent element</param>
        /// <param name="childElementGUID">the GUID of the child element</param>
        /// <param name="Mode"></param>
        /// <param name="ParentObjectTop">EA's top location of the object</param>
        /// <param name="ParentObjectBottom">EA's bottom location of the object</param>
        /// <param name="ParentObjectRight">EA's right location of the object</param>
        /// <param name="ParentObjectLeft">EA's left location of the object</param>
        /// <param name="ParentObjectStyle">EA's style (background color and such)of the object</param>
        /// <param name="ParentWasHere">is true if the parent was on the diagram at the on pre new diagram object method</param>
        public EAClass(EA.Repository repo, EA.Diagram targetedDiagram, string parentElementGUID, string childElementGUID, string Mode, int ParentObjectTop, int ParentObjectBottom, int ParentObjectRight, int ParentObjectLeft, string ParentObjectStyle, bool ParentWasHere)
        {
            this.parentElementGUID = parentElementGUID;
            this.repo = repo;
            XMLP = new XMLParser(repo);
            this.Mode = Mode;
            this.ParentWasHere = ParentWasHere;
            this.targetedDiagram = targetedDiagram;
            this.childElementGUID = childElementGUID;
            EA.Element parentelt = repo.GetElementByGuid(parentElementGUID);//am avr 
            this.dicAttributeByGuid = new Dictionary<string, EAClassAttribute>();

            this.dicParentEltAttributeByName = new Dictionary<string, EA.Attribute>();//am avr 19
            foreach (EA.Attribute at in parentelt.Attributes)//am avr 19
            {
                this.dicParentEltAttributeByName[at.Name] = at;//am avr 19
            }

            #region CreateMode
            if (Mode.Equals(CD.GetCreate()))
            {
                this.targetedPackage = repo.GetPackageByID(targetedDiagram.PackageID);

                this.parentElementID = repo.GetElementByGuid(parentElementGUID).ElementID;

                this.classStereotype = repo.GetElementByGuid(parentElementGUID).Stereotype;
                this.ParentObjectBottom = ParentObjectBottom;
                this.ParentObjectLeft = ParentObjectLeft;
                this.ParentObjectRight = ParentObjectRight;
                this.ParentObjectTop = ParentObjectTop;
                this.ParentObjectStyle = ParentObjectStyle;

                if (repo.GetElementByGuid(parentElementGUID).Abstract.Equals("0"))
                {
                    isAbstract = false;
                }
                else
                {
                    isAbstract = true;
                }
                this.SetIsRoot(repo.GetElementByGuid(parentElementGUID).IsActive);

                int j = 0; //am avr 19 pour test
                for (short i = 0; parentelt.AttributesEx.Count > i; i++)  //am avr 2019
                    {
                    if (i== j )
                    {
                         j = j+10;
                    }
                    //  EA.Attribute aParentAttribute = (EA.Attribute)repo.GetElementByGuid(parentElementGUID).AttributesEx.GetAt(i); //am avr 2019
                    EA.Attribute aParentAttribute = (EA.Attribute)parentelt.AttributesEx.GetAt(i); //am avr 2019
                    EAClassAttribute atr = new EAClassAttribute(repo, aParentAttribute.AttributeGUID, this, null, Mode,dicParentEltAttributeByName);//am avr 19
                   // attributeList.Add(new EAClassAttribute(repo, aParentAttribute.AttributeGUID, this, null, Mode));
                   attributeList.Add(atr);
                    dicAttributeByGuid[atr.GetGUID()] = atr;
                }

                //Copy class constraints
                foreach (EA.Constraint aParentConstraint in repo.GetElementByGuid(parentElementGUID).Constraints)
                {
                    constraintList.Add(new EAClassConstraint(repo, aParentConstraint.Name, aParentConstraint.Type, aParentConstraint.Notes, aParentConstraint.Status, this));
                }

                //Inheritance
                string ParentStereotype = repo.GetElementByGuid(this.GetParentElementGUID()).Stereotype;

                //  if ((!(ParentStereotype.Equals(CD.GetDatatypeStereotype()))) && (!(ParentStereotype.Equals("<<" + CD.GetDatatypeStereotype() + ">>"))) && (!(ParentStereotype.Equals(CD.GetPrimitiveStereotype()))) && (!(ParentStereotype.Equals("<<" + CD.GetPrimitiveStereotype() + ">>"))) && (!(ParentStereotype.Equals(CD.GetEnumStereotype()))) && (!(ParentStereotype.Equals("<<" + CD.GetEnumStereotype() + ">>"))))
                if (
                    (!(ParentStereotype.Equals(CD.GetDatatypeStereotype())))
                    &&
                    (!(ParentStereotype.Equals("<<" + CD.GetDatatypeStereotype() + ">>")))
                    && (!(ParentStereotype.Equals(CD.GetPrimitiveStereotype())))
                    && (!(ParentStereotype.Equals("<<" + CD.GetPrimitiveStereotype() + ">>")))
                    && (!(ParentStereotype.Equals(CD.GetEnumStereotype())))
                    && (!(ParentStereotype.Equals("<<" + CD.GetEnumStereotype() + ">>")))
                    && (parentelt.Type != "Enumeration")//am avr 19
                    && (parentelt.MetaType != "Enumeration")//am avr 19
                    )
                {
                    EA.Element ParentInheritance = null;
                    for (short i = 0; repo.GetElementByGuid(this.GetParentElementGUID()).Connectors.Count > i; i++)
                    {
                        EA.Connector AParentConnector = (EA.Connector)repo.GetElementByGuid(this.GetParentElementGUID()).Connectors.GetAt(i);
                        if (AParentConnector.Type.Equals("Generalization"))
                        {
                            if (AParentConnector.ClientID.Equals(repo.GetElementByGuid(this.GetParentElementGUID()).ElementID))
                            {

                                ParentInheritance = repo.GetElementByID(AParentConnector.SupplierID);
                                break;
                            }
                        }
                    }

                    if (!(ParentInheritance == null))
                    {
                        XMLParser XMLP = new XMLParser(repo);
                        for (short i = 0; ParentInheritance.Connectors.Count > i; i++)
                        {
                            EA.Connector AInheritedParentConnector = (EA.Connector)ParentInheritance.Connectors.GetAt(i);
                            if (AInheritedParentConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower()))
                            {
                                if (AInheritedParentConnector.SupplierID.Equals(ParentInheritance.ElementID))
                                {
                                    //   if (repo.GetElementByID(AInheritedParentConnector.ClientID).Abstract.Equals("1")) // am 5/5/2011 possible concrete inheritance
                                    if ((repo.GetElementByID(AInheritedParentConnector.ClientID).Abstract.Equals("1")) || XMLP.GetXmlValueConfig("EnableConcreteInheritanceInProfiles") == (ConfigurationManager.CHECKED))
                                    {
                                        PossibleInheritance.Add(repo.GetElementByID(AInheritedParentConnector.ClientID));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region UpdateMode
            else
            {
                EA.Element ParentElement = repo.GetElementByGuid(parentElementGUID);
                IBOElement = repo.GetElementByGuid(childElementGUID);

                this.targetedPackage = repo.GetPackageByID(targetedDiagram.PackageID);
                this.parentElementID = repo.GetElementByGuid(parentElementGUID).ElementID;
                this.classStereotype = repo.GetElementByGuid(parentElementGUID).Stereotype;

                this.SetIsRoot(IBOElement.IsActive);

                if (classStereotype.Equals(CD.GetPrimitiveStereotype()) || classStereotype.Equals("<<" + CD.GetPrimitiveStereotype() + ">>"))
                {
                    this.classStereotype = CD.GetDatatypeStereotype();
                    for (short i = 0; repo.GetElementByGuid(IBOElement.ElementGUID).Attributes.Count > i; i++)
                    {
                        EA.Attribute AnAttribute = (EA.Attribute)repo.GetElementByGuid(IBOElement.ElementGUID).Attributes.GetAt(i);
                        this.LoadPrimitiveAttribute(AnAttribute.AttributeGUID);
                    }

                    //Copy class constraints
                    for (short i = 0; IBOElement.Constraints.Count > i; i++)
                    {
                        EA.Constraint aParentConstraint = (EA.Constraint)IBOElement.Constraints.GetAt(i);
                        constraintList.Add(new EAClassConstraint(repo, aParentConstraint.Name, aParentConstraint.Type, aParentConstraint.Notes, aParentConstraint.Status, this));
                    }

                    if (IBOElement.Name.Contains("_"))
                    {
                        string[] tmp = IBOElement.Name.Split("_".ToCharArray());
                        this.SetQualifier(tmp[0]);
                    }
                    else
                    {
                        this.SetQualifier("");
                    }
                }
                else
                {

                    //Inheritance
                    #region inheritance
                    if ((!classStereotype.Equals(CD.GetDatatypeStereotype())) && (!(classStereotype.Equals("<<" + CD.GetDatatypeStereotype() + ">>"))))
                    {
                        EA.Element ParentInheritance = null;
                        EA.Element parentElem = repo.GetElementByGuid(this.GetParentElementGUID());
                        EA.Collection conns = parentElem.Connectors;
                        for (short i = 0; conns.Count > i; i++)
                        {
                            EA.Connector AParentConnector = (EA.Connector)conns.GetAt(i);
                            if (AParentConnector.Type.Equals("Generalization"))
                            {
                                //if (AParentConnector.ClientID.Equals(parentElem.ElementID))
                                if (AParentConnector.ClientID == parentElem.ElementID)
                                {
                                    ParentInheritance = repo.GetElementByID(AParentConnector.SupplierID);
                                    break;
                                }
                            }
                        }

                        if (!(ParentInheritance == null))
                        {
                            XMLParser XMLP = new XMLParser(repo);
                            for (short i = 0; ParentInheritance.Connectors.Count > i; i++)
                            {
                                EA.Connector AInheritedParentConnector = (EA.Connector)ParentInheritance.Connectors.GetAt(i);
                                if (AInheritedParentConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower()))
                                {
                                    if (AInheritedParentConnector.SupplierID.Equals(ParentInheritance.ElementID))
                                    {
                                        //  if (repo.GetElementByID(AInheritedParentConnector.ClientID).Abstract.Equals("1")) am 5/5/2011
                                        if ((repo.GetElementByID(AInheritedParentConnector.ClientID).Abstract.Equals("1")) || XMLP.GetXmlValueConfig("EnablePropertyGrouping") == (ConfigurationManager.CHECKED))
                                        {
                                            PossibleInheritance.Add(repo.GetElementByID(AInheritedParentConnector.ClientID));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (short i = 0; IBOElement.Connectors.Count > i; i++)
                    {
                        EA.Connector AConnector = (EA.Connector)IBOElement.Connectors.GetAt(i);
                        if (AConnector.Type.Equals("Generalization"))
                        {
                            if (AConnector.ClientID.Equals(IBOElement.ElementID))
                            {
                                SelectedInheritance = repo.GetElementByID(AConnector.SupplierID);
                            }
                        }
                    }

                    #endregion

                    if (IBOElement.Abstract.Equals("0"))
                    {
                        isAbstract = false;
                    }
                    else
                    {
                        isAbstract = true;
                    }
                    //Copy Attribute
                    ArrayList TmpList = new ArrayList();
                    CimContextor.utilitaires.Utilitaires util = new CimContextor.utilitaires.Utilitaires(repo);
                    EA.Collection attrs = IBOElement.Attributes;
                    for (short i = 0; i < IBOElement.Attributes.Count; i++)
                    {
                        // ABA 20230606 
                        EA.Attribute attr = (EA.Attribute)attrs.GetAt(i);
                        EAClassAttribute eaClassAttr = new EAClassAttribute(repo, attr.AttributeGUID, this, null, Mode, dicParentEltAttributeByName);
                        eaClassAttr.SetUISelectedState(true);
                        attributeList.Add(eaClassAttr);//am avr 19
                                                       // ABA 20230606 this.GetAttribute(((EA.Attribute)IBOElement.Attributes.GetAt(i)).AttributeGUID).SetUISelectedState(true);
                        TmpList.Add(attr.Name);

                    }

                    for (short i = 0; ParentElement.AttributesEx.Count > i; i++)
                    {
                        EA.Attribute AParentAttribute = (EA.Attribute)ParentElement.AttributesEx.GetAt(i);
                        if (!TmpList.Contains(AParentAttribute.Name))
                        {
                            // attributeList.Add(new EAClassAttribute(repo, AParentAttribute.AttributeGUID, this, null, Mode));
                            EAClassAttribute eaClassAttr = new EAClassAttribute(repo, AParentAttribute.AttributeGUID, this, null, Mode, dicParentEltAttributeByName);
                            attributeList.Add(eaClassAttr); // ABA20230606 
                            // util.wlog("TEST", "attribute" + attributeList[attributeList.Count-1]);//am avr 19
                            eaClassAttr.SetUISelectedState(false);
                            // ABA20230606 this.GetAttribute(AParentAttribute.AttributeGUID).SetUISelectedState(false);

                        }
                    }

                    //Copy class constraints
                    EA.Collection constr = IBOElement.Constraints;
                    for (short i = 0; constr.Count > i; i++)
                    {
                        EA.Constraint aParentConstraint = (EA.Constraint)constr.GetAt(i);
                        constraintList.Add(new EAClassConstraint(repo, aParentConstraint.Name, aParentConstraint.Type, aParentConstraint.Notes, aParentConstraint.Status, this));
                    }

                    if (ParentElement.Name.Equals(IBOElement.Name))
                    {
                        this.SetQualifier("");
                    }
                    else
                    {
                        if (IBOElement.Name.Contains("_"))
                        {
                            string[] tmp = IBOElement.Name.Split("_".ToCharArray());
                            this.SetQualifier(tmp[0]);
                        }
                        else
                        {
                            this.SetQualifier("");
                        }
                    }


                }
            }
            #endregion
        }

        /// <summary>
        /// Return the IsBasedOn Element
        /// </summary>
        /// <returns></returns>
        public EA.Element GetIBOElement()
        {
            return IBOElement;
        }

        public EA.Package GetTargetedPackage()
        {
            return this.targetedPackage;
        }

        /// <summary>
        /// Set the "ui's SelectedState" (if the object is checked in the ui) to either true of false.
        /// </summary>
        /// <param name="itemGUID">the Guid of the element to set</param>
        /// <param name="state">the state (true or false meaning checked or unchecked)</param>
        public void SetAttributeState(string itemGUID, bool state)
        {
            foreach (EAClassAttribute anAttribute in attributeList)
            {
                if (anAttribute.GetGUID().Equals(itemGUID))
                {
                    anAttribute.SetUISelectedState(state);
                }
            }
        }

        /// <summary>
        /// Return the attributeList that contain all the attribute of the object (also inherited attribute)
        /// </summary>
        /// <returns></returns>
        public ArrayList GetAttributeList()
        {
            return this.attributeList;
        }

        public void SetIsRoot(bool Value)
        {
            IsRoot = Value;
        }
        public bool GetIsRoot()
        {
            return IsRoot;
        }

        /// <summary>
        /// return an arraylist of EA.element available for inheritance
        /// </summary>
        public ArrayList GetPossibleInheritance()
        {
            return PossibleInheritance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ArrayList GetFilteredPossibleInheritance()  //am may 2011
        {
            return FilteredPossibleInheritance;
        }
        public void SetFilteredPossibleInheritance(ArrayList fltposinheritance)//am may 2011 
        {
            this.FilteredPossibleInheritance = fltposinheritance;
        }

        public bool CheckIfSameElement()
        {

            if (!this.classStereotype.Equals(IBOElement.Stereotype))
            {
                return false;
            }

            if (IBOElement.Abstract.Equals("0"))
            {
                if (!this.GetAbstract().Equals(false))
                {
                    return false;
                }
            }
            else
            {
                if (!this.GetAbstract().Equals(true))
                {
                    return false;
                }
            }




            //Attribute
            foreach (EAClassAttribute AnAttribute in attributeList)
            {
                if (AnAttribute.CheckIfSameElement().Equals(false))
                {
                    return false;
                }
            }
            //

            //for(short i = 0; constraintList.Count()>i ; i++)
            //{
            //Constraint  
            //}

            // inheritance
            EA.Element ActualInheritedElement = null;
            for (short i = 0; IBOElement.Connectors.Count > i; i++)
            {
                EA.Connector AConnector = (EA.Connector)IBOElement.Connectors.GetAt(i);
                if (AConnector.Type.Equals("Generalization"))
                {
                    if (AConnector.ClientID.Equals(IBOElement.ElementID))
                    {
                        ActualInheritedElement = repo.GetElementByID(AConnector.SupplierID);
                    }
                    else
                    {
                        ActualInheritedElement = repo.GetElementByID(AConnector.ClientID);
                    }
                }
            }
            if (((ActualInheritedElement == null)) && ((SelectedInheritance == null)))
            {

            }
            else if ((!(ActualInheritedElement == null)) && (!(SelectedInheritance == null)))
            {
                if (!ActualInheritedElement.ElementID.Equals(SelectedInheritance.ElementID))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            //

            return true;
        }

        public void SetSelectedInheritance(EA.Element AnElement)
        {
            this.SelectedInheritance = AnElement;
        }
        public EA.Element GetSelectedInheritance()
        {
            return this.SelectedInheritance;
        }


        /// <summary>
        /// Set the classifier of an attribute
        /// </summary>
        /// <param name="attributeGUID">the GUID of the attribute</param>
        /// <param name="classifierID">The new classifier</param>
        public void SetAttributeClassifier(string attributeGUID, int classifierID)
        {
            EAClassAttribute anAttribute = this.GetAttribute(attributeGUID);
            anAttribute.SetClassifier(classifierID);
        }

        /// <summary>
        /// Return the searched attribute
        /// </summary>
        /// <param name="searchedAttributeGUID">Guid of the searched attribute</param>
        /// <returns>The element searched</returns>
        public EAClassAttribute GetAttribute(string searchedAttributeGUID)
        {
            foreach (EAClassAttribute anAttribute in this.attributeList)
            {
                if (anAttribute.GetGUID().Equals(searchedAttributeGUID))
                {
                    return anAttribute;
                }
            }

            return null;
        }

        /// <summary>
        /// Return the stereotype of the class
        /// </summary>
        /// <returns>The stereotype of the class</returns>
        public string GetStereotype()
        {
            return this.classStereotype;
        }

        public void SetStereotype(string StereotypeList)
        {
            this.classStereotype = StereotypeList;
        }

        /// <summary>
        /// Value is either CREATE or UPDATE.
        /// Use the ConstantDefinition.GetCreate() or GetUpdate() for safer comparison.
        /// </summary>
        /// <returns>will either return CREATE or UPDATE</returns>
        public string GetMode()
        {
            return this.Mode;
        }

        /// <summary>
        /// Value is either CREATE or UPDATE.
        /// Use the ConstantDefinition.GetCreate() or GetUpdate() for safer initialization.
        /// </summary>
        /// <param name="Mode"></param>
        public void SetMode(string Mode)
        {
            this.Mode = Mode;
        }

        /// <summary>
        /// Return the parent element GUID (the draged and droped class) 
        /// </summary>
        /// <returns></returns>
        public string GetParentElementGUID()
        {
            return this.parentElementGUID;
        }

        /// <summary>
        /// Return the name of the class
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return GetQualifier() + this.repo.GetElementByGuid(parentElementGUID).Name;
        }

        public bool GetAbstract()
        {
            return isAbstract;
        }

        /// <summary>
        /// Set the qualifier of the class that will be created.
        /// </summary>
        /// <param name="elementQualifier"></param>
        public void SetQualifier(string elementQualifier)
        {
            this.classQualifier = elementQualifier;
        }

        /// <summary>
        /// Reset the constraintList of the class
        /// </summary>
        public void ResetConstraints()
        {
            constraintList = new ArrayList();
            //foreach (EA.Constraint aParentConstraint in repo.GetElementByGuid(parentElementGUID).Constraints)
            for (short i = 0; repo.GetElementByGuid(parentElementGUID).Constraints.Count > i; i++)
            {
                EA.Constraint aParentConstraint = (EA.Constraint)repo.GetElementByGuid(parentElementGUID).Constraints.GetAt(i);
                constraintList.Add(new EAClassConstraint(repo, aParentConstraint.Name, aParentConstraint.Type, aParentConstraint.Notes, aParentConstraint.Status, this));
            }
        }

        public ArrayList GetConstraints()
        {
            return constraintList;
        }

        /// <summary>
        /// Add a constraint to the class
        /// </summary>
        /// <param name="constraintName">Name of the constraint</param>
        /// <param name="constraintType">Type of the constraint (commonly OCL)</param>
        /// <param name="constraintNotes">The value/note of the constraint</param>
        /// <returns></returns>
        public EAClassConstraint AddConstraint(string constraintName, string constraintType, string constraintNotes)
        {
            EAClassConstraint aConstraint = new EAClassConstraint(repo, constraintName, constraintType, constraintNotes, "", this);
            constraintList.Add(aConstraint);
            return aConstraint;
        }

        /// <summary>
        /// Delete a constraint from the class
        /// </summary>
        /// <param name="constraintName">Name of the constraint</param>
        /// <param name="constraintType">Type of the constraint (commonly OCL)</param>
        /// <param name="constraintNotes">The value/note of the constraint</param>
        /// <returns></returns>
        public void DeleteConstraint(string constraintName)
        {
            EAClassConstraint ConstToDelete = null;
            foreach (EAClassConstraint aConstraint in constraintList)
            {
                if (aConstraint.GetName().Equals(constraintName))
                {
                    ConstToDelete = aConstraint;
                    break;
                }
            }

            constraintList.Remove(ConstToDelete);
        }


        /// <summary>
        /// Add an unexisting attribute for the case of the drag and drop of a primitive.
        /// </summary>
        /// <param name="attributeGUID"></param>
        /// <returns></returns>
        public EAClassAttribute LoadPrimitiveAttribute(string attributeGUID)
        {
            EAClassAttribute AnAttribute = new EAClassAttribute(repo, attributeGUID, this, repo.GetElementByGuid(this.GetParentElementGUID()).Name, CD.GetPrimitiveStereotype());
            attributeList.Add(AnAttribute);
            return AnAttribute;
        }

        public EAClassAttribute AddPrimitiveAttribute()
        {
            EAClassAttribute AnAttribute = new EAClassAttribute(repo, null, this, repo.GetElementByGuid(this.GetParentElementGUID()).Name, CD.GetPrimitiveStereotype());
            attributeList.Add(AnAttribute);
            return AnAttribute;
        }

        /// <summary>
        /// Get the searched constraint
        /// </summary>
        /// <param name="searchedConstraintName">the name of the constraint we're looking for</param>
        /// <returns></returns>
        public EAClassConstraint GetAConstraint(string searchedConstraintName)
        {
            foreach (EAClassConstraint aConstraint in constraintList)
            {
                if (aConstraint.GetName().Equals(searchedConstraintName))
                {
                    return aConstraint;
                }
            }
            return null;
        }

        /// <summary>
        /// Return the qualifier of the class.
        /// </summary>
        /// <returns></returns>
        public string GetQualifier()
        {
            if (this.classQualifier == null)
            {
                return "";
            }
            return this.classQualifier;
        }

        /// <summary>
        /// Reset the attributeList from the original isbasedon (in the case of a reset in an update mode)
        /// </summary>
        /// <param name="NewGUID"></param>
        public void ResetAttributeFromOriginalIsBasedOn(string NewGUID)
        {
            if (!NewGUID.Equals(""))
            {
                foreach (EAClassAttribute AClassAttribute in attributeList)
                {
                    if (AClassAttribute.GetName().Equals(repo.GetAttributeByGuid(NewGUID).Name))
                    {
                        AClassAttribute.ResetFromGUID(NewGUID);
                        break;
                    }
                }
            }
            else
            {
                attributeList = new ArrayList();
                //foreach (EA.Attribute aParentAttribute in repo.GetElementByGuid(parentElementGUID).Attributes)
                for (short i = 0; repo.GetElementByGuid(parentElementGUID).Attributes.Count > i; i++)
                {
                    EA.Attribute aParentAttribute = (EA.Attribute)repo.GetElementByGuid(parentElementGUID).Attributes.GetAt(i);
                    attributeList.Add(new EAClassAttribute(repo, aParentAttribute.AttributeGUID, this, null, Mode));
                }
            }
        }

        /// <summary>
        /// Metod used to create the IBO in the case of an update.
        /// </summary>
        /// <param name="copyTagValues"></param>
        /// <param name="copyNotes"></param>
        /// <param name="copyConstraints"></param>
        /// <param name="copyParentElement"></param>
        private void UpdateIsBasedOn(IsBasedOnForm IBOF)
        {
            bool copyTagValues, copyNotes, copyConstraints, copyParentElement, copyStereotype;
            copyTagValues = IBOF.GetCBCopyTagValues();
            copyNotes = IBOF.GetCBCopyNotes();
            copyConstraints = IBOF.GetCBCopyConstraints();
            copyParentElement = IBOF.GetCBCopyParent();
            copyStereotype = IBOF.GetCBCopyStereotype();

            /////////////////// Duplicate the targeted Class
            EA.Element parentElement = repo.GetElementByGuid(parentElementGUID);
            EA.Element duplicatedClass;

            EA.DiagramObject diagParentObj = null;
            for (short i = 0; targetedDiagram.DiagramObjects.Count > i; i++)
            {
                EA.DiagramObject AnObject = (EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(i);
                if (AnObject.ElementID.Equals(parentElement.ElementID))
                {
                    diagParentObj = AnObject;
                    break;
                }
            }
            duplicatedClass = this.GetIBOElement();
            duplicatedClass.Name = this.GetName();

            //Dupplication of stereotype
            if (copyStereotype.Equals(true))
            {
                duplicatedClass.StereotypeEx = repo.GetElementByGuid(parentElementGUID).StereotypeEx;
            }
            else
            {
                duplicatedClass.StereotypeEx = this.GetStereotype();
            }

            duplicatedClass.Update();
            duplicatedClass.Refresh();

            XMLParser xmlParser = new XMLParser(repo);
            if (xmlParser.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
            {
                xmlParser.AddXmlLog("IsBasedOnExecuting", "IsBasedOn executed on " + duplicatedClass.Name);
            }



            if (copyTagValues.Equals(true))
            {


                //Copy TaggedValue
                for (short i = 0; parentElement.TaggedValues.Count > i; i++)
                {
                    EA.TaggedValue aTagValue = (EA.TaggedValue)parentElement.TaggedValues.GetAt(i);
                    if (!aTagValue.Name.Equals(CD.GetIBOTagValue()))
                    {
                        bool TagValueFound = false;
                        EA.TaggedValue OldTagValue = null;

                        for (short j = 0; duplicatedClass.TaggedValues.Count > j; j++)
                        {
                            EA.TaggedValue aChildTagValue = (EA.TaggedValue)duplicatedClass.TaggedValues.GetAt(j);
                            if ((aChildTagValue.Name.Equals(aTagValue.Name)))
                            {
                                TagValueFound = true;
                                OldTagValue = aChildTagValue;
                                break;
                            }
                        }
                        if (TagValueFound.Equals(false))
                        {
                            EA.TaggedValue newTag = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(aTagValue.Name, aTagValue.Value);
                            newTag.Notes = aTagValue.Notes;
                            newTag.Update();
                        }
                        else
                        {
                            OldTagValue.Name = aTagValue.Name;
                            OldTagValue.Value = aTagValue.Value;
                            OldTagValue.Notes = aTagValue.Notes;
                            //Do not set the Element ID !!! or it will bug !!
                            //OldTagValue.ElementID = aTagValue.ElementID;
                            //
                            OldTagValue.Update();
                        }
                    }
                }
                duplicatedClass.TaggedValues.Refresh();
                //End copy TaggedValue
            }


            //Copy Attribute
            foreach (EAClassAttribute anAttribute in attributeList)
            {
                anAttribute.ExecuteIsBasedOn(duplicatedClass, copyTagValues, Mode);
            }



            //Class's Constraint
            for (short i = 0; duplicatedClass.Constraints.Count > i; i++)
            {
                duplicatedClass.Constraints.DeleteAt(i, false);
            }
            duplicatedClass.Constraints.Refresh();
            foreach (EAClassConstraint AConstraint in constraintList)
            {
                AConstraint.ExecuteIsBasedOn(duplicatedClass, false);
            }
            duplicatedClass.Constraints.Refresh();
            //End class's Constraint


            duplicatedClass.Status = parentElement.Status;
            duplicatedClass.Version = parentElement.Version;
            duplicatedClass.Multiplicity = parentElement.Multiplicity;
            duplicatedClass.IsActive = IsRoot;
            duplicatedClass.Priority = parentElement.Priority;
            duplicatedClass.Alias = parentElement.Alias;
            //EA Abstract is string but 0 = not abstract; 1 abstract
            duplicatedClass.Abstract = IBOF.GetCBAbstract();

            if (copyNotes.Equals(true))
            {
                duplicatedClass.Notes = parentElement.Notes;
            }



            string ObjectColor = "Default";
            try
            {
                ObjectColor = xmlParser.GetXmlValueConfig("ConfigColor").Trim();
            }
            catch
            {
                MessageBox.Show("Couldn't find the selected color for IsBasedOn object", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            if (!ObjectColor.Equals("Default"))
            {
                EA.DiagramObject ASelectedObject = null;
                for (short i = 0; targetedDiagram.DiagramObjects.Count > i; i++)
                {
                    EA.DiagramObject AnObject = (EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(i);
                    if (AnObject.ElementID.Equals(duplicatedClass.ElementID))
                    {
                        ASelectedObject = AnObject;
                        break;
                    }
                }
                if (!(ASelectedObject == null))
                {
                    ASelectedObject.Style = "BCol=" + ObjectColor + ";BFol=0;LCol=0;LWth=1;";
                    ASelectedObject.Update();
                }
            }
            duplicatedClass.Update();
            /************************  temporary addition  for WG16 european style ******************************/
            // At this level for temporary compatibility reasons for WG16 europeanstyle

            EA.Element duplel = repo.GetElementByID((int)duplicatedClass.ElementID);
            Utilitaires util = new Utilitaires(repo);
            foreach (EA.Attribute atr in duplel.Attributes)
            {
                string prov = atr.Name;
                List<string> presentfacets = new List<string>(); // list of present constraint
                if (atr.Constraints.Count > 0)
                {
                    foreach (EA.AttributeConstraint atrco in atr.Constraints)
                    {
                        if (atrco.Type == "OCL")
                        {
                            
                            util.setAttributeValueAsConstraintIfNecessary(duplicatedClass, atr, atrco, presentfacets);
                        }
                    }
                }
                
                util.deleteAttributeTaggedValuesIfNecessary(atr, presentfacets);
            }

            duplel.Update();
            // all constraints  on attribute value which have a facet receives a tagvalue of the name of the facet with the relevant value
            /***********************************************************************************************/

            repo.ReloadDiagram(targetedDiagram.DiagramID);
            /////////////////End duplicate the targeted class

            /////////////////Creating link for inheritance

            //////Clearing old inheritance
            short ConnectorToDelete = -1;
            for (short i = 0; duplicatedClass.Connectors.Count > i; i++)
            {
                EA.Connector AConnector = (EA.Connector)duplicatedClass.Connectors.GetAt(i);
                if (AConnector.Type.Equals("Generalization"))
                {
                    if (AConnector.ClientID.Equals(this.GetIBOElement().ElementID))
                    {
                        ConnectorToDelete = i;
                        break;
                    }
                }
            }
            if (!(ConnectorToDelete.Equals(-1)))
            {
                duplicatedClass.Connectors.DeleteAt(ConnectorToDelete, true);
                duplicatedClass.Connectors.Refresh();
            }

            //////Creating new inheritance
            if (!(SelectedInheritance == null))
            {
                EA.Connector newInherit = (EA.Connector)duplicatedClass.Connectors.AddNew("", "Generalization");
                newInherit.SupplierID = SelectedInheritance.ElementID;
                newInherit.Update();
                duplicatedClass.Connectors.Refresh();
            }


            //////////////////Refreshing
            targetedDiagram.DiagramObjects.Refresh();
            targetedDiagram.Update();
            repo.ReloadDiagram(targetedDiagram.DiagramID);
            repo.RefreshModelView(targetedPackage.PackageID);
            repo.RefreshOpenDiagrams(false);
            //targetedDiagram.

        }

        /// <summary>
        /// Metod used to create the IBO in the case of a creation.
        /// </summary>
        /// <param name="copyTagValues"></param>
        /// <param name="copyNotes"></param>
        /// <param name="copyConstraints"></param>
        /// <param name="copyParentElement"></param>
        private void CreateIsBasedOn(IsBasedOnForm IBOF)
        {
            //repo.SaveAllDiagrams(); //am fev 2012
            bool copyTagValues, copyNotes, copyConstraints, copyParentElement, copyStereotype;
            copyTagValues = IBOF.GetCBCopyTagValues();
            copyNotes = IBOF.GetCBCopyNotes();
            copyConstraints = IBOF.GetCBCopyConstraints();
            copyParentElement = IBOF.GetCBCopyParent();
            copyStereotype = IBOF.GetCBCopyStereotype();
            /////////////////// Duplicate the targeted Class
            EA.Element parentElement = repo.GetElementByGuid(parentElementGUID);
            EA.Element duplicatedClass;
            if (parentElement.Type == "Enumeration") // am mai 2018
            {
                duplicatedClass = (EA.Element)targetedPackage.Elements.AddNew(this.GetQualifier() + parentElement.Name, parentElement.Type);

                duplicatedClass.MetaType = repo.GetElementByGuid(parentElementGUID).MetaType;//am mai 2018
                EA.Attribute att = (EA.Attribute)repo.GetElementByGuid(parentElementGUID).Attributes.GetAt(0);//am pour test
                string prov = att.Type;
            }
            else
            {
                duplicatedClass = (EA.Element)targetedPackage.Elements.AddNew(this.GetQualifier() + parentElement.Name, parentElement.Type);
            }


            EA.DiagramObject diagParentObj = null;
            for (short i = 0; targetedDiagram.DiagramObjects.Count > i; i++)
            {
                EA.DiagramObject AnObject = (EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(i);
                if (AnObject.ElementID.Equals(parentElement.ElementID))
                {
                    diagParentObj = AnObject;
                    break;
                }
            }


            XMLParser xmlParser = new XMLParser(repo);
            if (xmlParser.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
            {
                xmlParser.AddXmlLog("IsBasedOnExecuting", "IsBasedOn executed on " + duplicatedClass.Name);
            }

            #region CopyTagValue
            if (copyTagValues.Equals(true))
            {
                //Copy TaggedValue
                /* old copytagvalue
                //foreach (EA.TaggedValue aTagValue in parentElement.TaggedValues)
                for (short i = 0; parentElement.TaggedValues.Count > i; i++)
                {
                    EA.TaggedValue aTagValue = (EA.TaggedValue)parentElement.TaggedValues.GetAt(i);
                    EA.TaggedValue newTag = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(aTagValue.Name, aTagValue.Value);
                    newTag.Notes = aTagValue.Notes;
                    newTag.Update();
                }
                duplicatedClass.TaggedValues.Refresh();
                //End copy TaggedValue
            }*/
                for (short i = 0; parentElement.TaggedValues.Count > i; i++)
                {
                    EA.TaggedValue aTagValue = (EA.TaggedValue)parentElement.TaggedValues.GetAt(i);
                    if (!aTagValue.Name.Equals(CD.GetIBOTagValue()))
                    {
                        EA.TaggedValue newTag = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(aTagValue.Name, aTagValue.Value);
                        newTag.Notes = aTagValue.Notes;
                        newTag.Update();
                    }
                }
                duplicatedClass.TaggedValues.Refresh();
            }
            // EA.TaggedValue newTag2 = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(CD.GetIBOTagValue(), duplicatedClass.ElementGUID);
            EA.TaggedValue newTag2 = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(CD.GetIBOTagValue(), parentElement.ElementGUID);
            newTag2.Notes = CD.GetIBOTagValueNote();
            newTag2.Update();
            duplicatedClass.TaggedValues.Refresh();
            //End copy TaggedValue
            #endregion

            //Copy Attribute
            foreach (EAClassAttribute anAttribute in attributeList)
            {
                anAttribute.ExecuteIsBasedOn(duplicatedClass, copyTagValues, Mode);
            }


            //Class's Constraint
            foreach (EAClassConstraint AConstraint in constraintList)
            {
                AConstraint.ExecuteIsBasedOn(duplicatedClass, true);
            }
            duplicatedClass.Constraints.Refresh();
            //End class's Constraint


            duplicatedClass.IsActive = IsRoot;
            duplicatedClass.Status = parentElement.Status;
            duplicatedClass.Version = parentElement.Version;
            duplicatedClass.Multiplicity = parentElement.Multiplicity;
            if (copyNotes.Equals(true))
            {
                duplicatedClass.Notes = parentElement.Notes;
            }

            //Dupplication of stereotype
            if (copyStereotype.Equals(true))
            {
                duplicatedClass.StereotypeEx = repo.GetElementByGuid(parentElementGUID).StereotypeEx;
            }
            else
            {
                duplicatedClass.StereotypeEx = this.GetStereotype();
            }

            duplicatedClass.Priority = parentElement.Priority;
            duplicatedClass.Alias = parentElement.Alias;
            //EA Abstract is string but 0 = not abstract; 1 abstract
            duplicatedClass.Abstract = IBOF.GetCBAbstract();
            duplicatedClass.Update();
            /************************  temporary addition  for WG16 european style ******************************/
            // At this level for temporary compatibility reasons for WG16 europeanstyle

            EA.Element duplel = repo.GetElementByID((int)duplicatedClass.ElementID);
            try
            {
                Utilitaires util = new Utilitaires(repo);

                foreach (EA.Attribute atr in duplel.Attributes)
                {
                    string prov = atr.Name;
                    List<string> presentfacets = new List<string>(); // list of present constraint
                    if (atr.Constraints.Count > 0)
                    {
                        foreach (EA.AttributeConstraint atrco in atr.Constraints)
                        {
                            if (atrco.Type == "OCL")
                            {
                                util.setAttributeValueAsConstraintIfNecessary(duplicatedClass, atr, atrco, presentfacets);
                            }
                        }
                    }
                    util.deleteAttributeTaggedValuesIfNecessary(atr, presentfacets);
                }
            } 
            catch(TypeInitializationException tie)
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_034, tie);
            }
            catch (Exception ex)
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_033, ex);
            }

            duplel.Update();
            // all constraints  on attribute value which have a facet receives a tagvalue of the name of the facet with the relevant value
            /***********************************************************************************************/


            repo.ReloadDiagram(targetedDiagram.DiagramID);

            //Adding Duplicated class to the diagram
            EA.DiagramObject diagTargetedObj = (EA.DiagramObject)targetedDiagram.DiagramObjects.AddNew("l=200;r=300;t=200;b=300;", "");
            diagTargetedObj.ElementID = duplicatedClass.ElementID;

            string ObjectColor = "Default";
            try
            {
                ObjectColor = xmlParser.GetXmlValueConfig("ConfigColor").Trim();
            }
            catch
            {

            }
            if (!ObjectColor.Equals("Default"))
            {
                diagTargetedObj.Style = "BCol=" + ObjectColor + ";BFol=0;LCol=0;LWth=1;";
            }

            diagTargetedObj.bottom = diagParentObj.bottom;
            diagTargetedObj.top = diagParentObj.top;
            diagTargetedObj.right = diagParentObj.right;
            diagTargetedObj.left = diagParentObj.left;

            diagTargetedObj.Update();

            //Deleting the parent class if copy parent =true
            if (copyParentElement.Equals(true))
            {
                if (!(diagParentObj == null))
                {
                    if (ParentWasHere.Equals(false))
                    {
                        diagParentObj.bottom = diagTargetedObj.bottom;
                        diagParentObj.top = diagTargetedObj.top;
                        diagParentObj.left = diagTargetedObj.right + 50;
                        diagParentObj.right = diagParentObj.left + (diagTargetedObj.right
                            - diagTargetedObj.left);
                        diagParentObj.Update();
                    }
                    else
                    {
                        diagParentObj.bottom = ParentObjectBottom;
                        diagParentObj.top = ParentObjectTop;
                        diagParentObj.left = ParentObjectLeft;
                        diagParentObj.right = ParentObjectRight;
                        diagParentObj.Style = ParentObjectStyle;
                        diagParentObj.Update();
                    }

                }
            }
            else
            {
                short CptToDelete = -1;
                for (short i = 0; targetedDiagram.DiagramObjects.Count > i; i++)
                {
                    EA.DiagramObject AnObject = (EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(i);
                    if (AnObject.ElementID.Equals(diagParentObj.ElementID))
                    {
                        CptToDelete = i;
                        break;
                    }
                }
                targetedDiagram.DiagramObjects.Delete(CptToDelete);
                targetedDiagram.DiagramObjects.Refresh();
                targetedDiagram.Update();
                repo.ReloadDiagram(targetedDiagram.DiagramID);
            }

            /////////////////End duplicate the targeted class

            /////////////////Creating Link IsBaseOn
            EA.Connector newBasedOn = (EA.Connector)duplicatedClass.Connectors.AddNew("", CD.GetDependency());

            newBasedOn.Stereotype = CD.GetIsBasedOnStereotype();
            newBasedOn.SupplierID = parentElement.ElementID;
            //Allow to add a cardinality to the IBO
            //newBasedOn.SupplierEnd.Cardinality="0..*";
            newBasedOn.Update();
            duplicatedClass.Connectors.Refresh();


            /////////////////Creating link for inheritance
            if (!(SelectedInheritance == null))
            {
                EA.Connector newInherit = (EA.Connector)duplicatedClass.Connectors.AddNew("", "Generalization");
                newInherit.SupplierID = SelectedInheritance.ElementID;
                newInherit.Update();
                duplicatedClass.Connectors.Refresh();
            }

            //////////////////Refreshing
            targetedDiagram.DiagramObjects.Refresh();
            targetedDiagram.Update();
            repo.ReloadDiagram(targetedDiagram.DiagramID);
            repo.RefreshModelView(targetedPackage.PackageID);
            repo.RefreshOpenDiagrams(false);
            //-------------- added mars 2019 -----------------------------------------
            if (XMLP.GetXmlValueConfig("W13AutomaticAnscesterInProfile") == ConfigurationManager.CHECKED)
            {
                addAncesters(repo, targetedDiagram, targetedPackage, duplel, parentElement, ParentObjectTop, ParentObjectBottom, ParentObjectRight, ParentObjectLeft, ParentObjectStyle, ParentWasHere);
            }
        }

        /// <summary>
        /// The method called to rollback the currentdiagram in the case of a cancel on the ibo window
        /// </summary>
        public void CancelIsBasedOn()
        {

            bool found = false;
            short cpt = 0;
            for (short i = 0; targetedDiagram.DiagramObjects.Count > i; i++)
            {
                if (((EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(i)).ElementID.Equals(parentElementID))
                {
                    found = true;
                    cpt = i;
                    break;
                }
            }
            EA.DiagramObject diagParentObj = null;
            if (!found.Equals(false))
            {
                if (ParentWasHere.Equals(true))
                {
                    diagParentObj = (EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(cpt);
                    diagParentObj.ElementID = parentElementID;
                    diagParentObj.top = ParentObjectTop;
                    diagParentObj.bottom = ParentObjectBottom;
                    diagParentObj.right = ParentObjectRight;
                    diagParentObj.left = ParentObjectLeft;
                    diagParentObj.Style = ParentObjectStyle;
                }
                else
                {
                    targetedDiagram.DiagramObjects.Delete(cpt);
                    targetedDiagram.DiagramObjects.Refresh();
                    targetedDiagram.Update();
                    repo.ReloadDiagram(targetedDiagram.DiagramID);
                }
            }
            else
            {
                if (ParentWasHere.Equals(true))
                {
                    diagParentObj = ((EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(cpt));
                    diagParentObj.ElementID = parentElementID;
                    diagParentObj.top = ParentObjectTop;
                    diagParentObj.bottom = ParentObjectBottom;
                    diagParentObj.right = ParentObjectRight;
                    diagParentObj.left = ParentObjectLeft;
                    diagParentObj.Style = ParentObjectStyle;
                }
            }

            if (!(diagParentObj == null))
            {
                diagParentObj.Update();
                targetedDiagram.DiagramObjects.Refresh();
                targetedDiagram.Update();
                repo.ReloadDiagram(targetedDiagram.DiagramID);
            }

        }

        /// <summary>
        /// Metod used to create the IBO in the case of a primitive.
        /// </summary>
        /// <param name="copyTagValues"></param>
        /// <param name="copyNotes"></param>
        /// <param name="copyConstraints"></param>
        /// <param name="copyParentElement"></param>
        private void PrimitiveIsBasedOn(IsBasedOnForm IBOF)
        {

            bool copyTagValues, copyNotes, copyConstraints, copyParentElement;
            copyTagValues = IBOF.GetCBCopyTagValues();
            copyNotes = IBOF.GetCBCopyNotes();
            copyConstraints = IBOF.GetCBCopyConstraints();
            copyParentElement = IBOF.GetCBCopyParent();

            /////////////////// Duplicate the targeted Class
            EA.Element parentElement = repo.GetElementByGuid(parentElementGUID);
            EA.Element duplicatedClass;
            duplicatedClass = (EA.Element)targetedPackage.Elements.AddNew(this.GetQualifier() + parentElement.Name, parentElement.Type);

            EA.DiagramObject diagParentObj = null;
            for (short i = 0; targetedDiagram.DiagramObjects.Count > i; i++)
            {
                EA.DiagramObject AnObject = (EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(i);
                if (AnObject.ElementID.Equals(parentElement.ElementID))
                {
                    diagParentObj = AnObject;
                    break;
                }
            }


            XMLParser xmlParser = new XMLParser(repo);
            if (xmlParser.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
            {
                xmlParser.AddXmlLog("IsBasedOnExecuting", "IsBasedOn executed on " + duplicatedClass.Name);
            }



            //Copy Attribute
            foreach (EAClassAttribute anAttribute in attributeList)
            {
                anAttribute.ExecuteIsBasedOn(duplicatedClass, false, Mode);
            }

            //copy class's constraints
            //Class's Constraint
            foreach (EAClassConstraint AConstraint in constraintList)
            {
                AConstraint.ExecuteIsBasedOn(duplicatedClass, true);
            }
            duplicatedClass.Constraints.Refresh();
            //End class's Constraint


            if (copyNotes.Equals(true))
            {
                duplicatedClass.Notes = parentElement.Notes;
            }

            if (copyTagValues.Equals(true))
            {
                //Copy TaggedValue
                for (short i = 0; parentElement.TaggedValues.Count > i; i++)
                {
                    EA.TaggedValue aTagValue = (EA.TaggedValue)parentElement.TaggedValues.GetAt(i);
                    if (!aTagValue.Name.Equals(CD.GetIBOTagValue()))
                    {
                        EA.TaggedValue newTag = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(aTagValue.Name, aTagValue.Value);
                        newTag.Notes = aTagValue.Notes;
                        newTag.Update();
                    }
                }
                duplicatedClass.TaggedValues.Refresh();
            }
            // EA.TaggedValue newTag2 = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(CD.GetIBOTagValue(), duplicatedClass.ElementGUID);
            EA.TaggedValue newTag2 = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(CD.GetIBOTagValue(), parentElement.ElementGUID);
            newTag2.Notes = CD.GetIBOTagValueNote();
            newTag2.Update();
            //End copy TaggedValue

            duplicatedClass.IsActive = IsRoot;
            duplicatedClass.StereotypeEx = CD.GetDatatypeStereotype();
            duplicatedClass.Status = parentElement.Status;
            duplicatedClass.Version = parentElement.Version;
            duplicatedClass.Multiplicity = parentElement.Multiplicity;
            duplicatedClass.Priority = parentElement.Priority;
            duplicatedClass.Alias = parentElement.Alias;
            duplicatedClass.Abstract = parentElement.Abstract;

            duplicatedClass.Update();

            /************************  temporary addition  for WG16 european style ******************************/
            // At this level for temporary compatibility reasons for WG16 europeanstyle

            EA.Element duplel = repo.GetElementByID((int)duplicatedClass.ElementID);
            Utilitaires util = new Utilitaires(repo);

            foreach (EA.Attribute atr in duplel.Attributes)
            {
                string prov = atr.Name;
                List<string> presentfacets = new List<string>(); // list of present constraint
                if (atr.Constraints.Count > 0)
                {
                    foreach (EA.AttributeConstraint atrco in atr.Constraints)
                    {
                        if (atrco.Type == "OCL")
                        {
                            util.setAttributeValueAsConstraintIfNecessary(duplicatedClass, atr, atrco, presentfacets);
                        }
                    }
                }

                util.deleteAttributeTaggedValuesIfNecessary(atr, presentfacets);
            }

            duplel.Update();
            // all constraints  on attribute value which have a facet receives a tagvalue of the name of the facet with the relevant value
            /***********************************************************************************************/



            repo.ReloadDiagram(targetedDiagram.DiagramID);
            //Adding Duplicated class to the diagram
            EA.DiagramObject diagTargetedObj = (EA.DiagramObject)targetedDiagram.DiagramObjects.AddNew("l=200;r=300;t=200;b=300;", "");
            diagTargetedObj.ElementID = duplicatedClass.ElementID;
            //
            string ObjectColor = "Default";
            try
            {
                ObjectColor = xmlParser.GetXmlValueConfig("ConfigColor").Trim();
            }
            catch
            {

            }
            if (!ObjectColor.Equals("Default"))
            {
                diagTargetedObj.Style = "BCol=" + ObjectColor + ";BFol=0;LCol=0;LWth=1;";
            }
            //

            diagTargetedObj.bottom = diagParentObj.bottom;
            diagTargetedObj.top = diagParentObj.top;
            diagTargetedObj.right = diagParentObj.right;
            diagTargetedObj.left = diagParentObj.left;

            diagTargetedObj.Update();

            //Deleting the parent class if copy parent =true
            if (copyParentElement.Equals(true))
            {
                if (!(diagParentObj == null))
                {
                    diagParentObj.bottom = ParentObjectBottom;
                    diagParentObj.top = ParentObjectTop;
                    diagParentObj.left = ParentObjectLeft;
                    diagParentObj.right = ParentObjectRight;
                    diagParentObj.Style = ParentObjectStyle;
                    diagParentObj.Update();
                }
            }
            else
            {
                short CptToDelete = -1;
                for (short i = 0; targetedDiagram.DiagramObjects.Count > i; i++)
                {
                    EA.DiagramObject AnObject = (EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(i);
                    if (AnObject.ElementID.Equals(diagParentObj.ElementID))
                    {
                        CptToDelete = i;
                        break;
                    }
                }
                targetedDiagram.DiagramObjects.Delete(CptToDelete);
                targetedDiagram.DiagramObjects.Refresh();
                targetedDiagram.Update();
                repo.ReloadDiagram(targetedDiagram.DiagramID);
                diagParentObj = null;
            }

            diagTargetedObj.Update();
            if (!(diagParentObj == null))
            {
                diagParentObj.Update();
            }
            /////////////////End duplicate the targeted class

            /////////////////Creating Link IsBaseOn
            EA.Connector newBasedOn = (EA.Connector)duplicatedClass.Connectors.AddNew("", "Dependency");

            newBasedOn.Stereotype = CD.GetIsBasedOnStereotype();
            newBasedOn.SupplierID = parentElement.ElementID;
            //Allow to add a cardinlaity to the IBO
            //newBasedOn.SupplierEnd.Cardinality="0..*";
            newBasedOn.Update();
            duplicatedClass.Connectors.Refresh();


            //////////////////Refreshing
            targetedDiagram.DiagramObjects.Refresh();
            targetedDiagram.Update();
            repo.ReloadDiagram(targetedDiagram.DiagramID);
            repo.RefreshModelView(targetedPackage.PackageID);
            repo.RefreshOpenDiagrams(false);
            //targetedDiagram.
        }

        /// <summary>
        /// Will create the IBO (will use either createIBO, updateIBO, primitiveIBO depending of the selected element).
        /// </summary>
        /// <param name="copyTagValues"></param>
        /// <param name="copyNotes"></param>
        /// <param name="copyConstraints"></param>
        /// <param name="copyParentElement"></param>
        public void ExecuteIsBasedOn(IsBasedOnForm IBOF)
        {

            bool copyParentElement;
            copyParentElement = IBOF.GetCBCopyParent();

            /////////////////// Save the diagram
            repo.SaveDiagram(targetedDiagram.DiagramID);


            /////////////////// If it's updating and we want to remove the parent class

            if ((Mode.Equals(CD.GetUpdate())) && (copyParentElement.Equals(false)))
            {
                bool ElementFound = false;
                short ElementShort = -1;
                for (short i = 0; repo.GetCurrentDiagram().DiagramObjects.Count > i; i++)
                {
                    EA.DiagramObject ADiagObj = (EA.DiagramObject)repo.GetCurrentDiagram().DiagramObjects.GetAt(i);
                    ElementShort++;
                    if (ADiagObj.ElementID.Equals(parentElementID))
                    {
                        ElementFound = true;
                        break;
                    }
                }
                if (ElementFound.Equals(true))
                {
                    repo.GetCurrentDiagram().DiagramObjects.DeleteAt(ElementShort, true);
                }
                repo.GetCurrentDiagram().DiagramObjects.Refresh();
            }


            //////////////////////////Creation case
            if (this.Mode.Equals(CD.GetCreate()))
            {
                //this.CreateIsBasedOn(copyTagValues, copyNotes, copyConstraints, copyParentElement);
                try
                {
                    CreateIsBasedOn(IBOF);
                } 
                catch(SystemException se)
                {
                    MessageBox.Show(se.Message + "\n" + se.StackTrace);
                }
            }////////////////////////////Update case
            else if (this.Mode.Equals(CD.GetUpdate()))
            {
                //this.UpdateIsBasedOn(copyTagValues, copyNotes, copyConstraints, copyParentElement);
                UpdateIsBasedOn(IBOF);
            }
            else
            {
                //this.PrimitiveIsBasedOn(copyTagValues, copyNotes, copyConstraints, copyParentElement);
                PrimitiveIsBasedOn(IBOF);
            }

        }

        //------------------------------------------------ ajouts am july 2011 -----------------------------
        public void PurePrimitiveIsBasedOn()
        {
            repo.SaveDiagram(targetedDiagram.DiagramID);
            /////////////////// Duplicate the targeted Class
            EA.Element parentElement = repo.GetElementByGuid(parentElementGUID);
            EA.Element duplicatedClass;
            duplicatedClass = (EA.Element)targetedPackage.Elements.AddNew(parentElement.Name, "Class");          //this.GetQualifier() + parentElement.Name, parentElement.Type);

            EA.DiagramObject diagParentObj = null;
            for (short i = 0; targetedDiagram.DiagramObjects.Count > i; i++)
            {
                EA.DiagramObject AnObject = (EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(i);
                if (AnObject.ElementID.Equals(parentElement.ElementID))
                {
                    diagParentObj = AnObject;
                    break;
                }
            }


            XMLParser xmlParser = new XMLParser(repo);
            if (xmlParser.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
            {
                xmlParser.AddXmlLog("IsBasedOnExecuting", "IsBasedOn executed on " + duplicatedClass.Name);
            }




            // duplicate notes

            duplicatedClass.Notes = parentElement.Notes;


            // taggedvalues
            //Copy TaggedValue
            for (short i = 0; parentElement.TaggedValues.Count > i; i++)
            {
                EA.TaggedValue aTagValue = (EA.TaggedValue)parentElement.TaggedValues.GetAt(i);
                if (!aTagValue.Name.Equals(CD.GetIBOTagValue()))
                {
                    EA.TaggedValue newTag = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(aTagValue.Name, aTagValue.Value);
                    newTag.Notes = aTagValue.Notes;
                    newTag.Update();
                }
            }
            duplicatedClass.TaggedValues.Refresh();



            // EA.TaggedValue newTag2 = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(CD.GetIBOTagValue(), duplicatedClass.ElementGUID);
            EA.TaggedValue newTag2 = (EA.TaggedValue)duplicatedClass.TaggedValues.AddNew(CD.GetIBOTagValue(), parentElement.ElementGUID);
            newTag2.Notes = CD.GetIBOTagValueNote();
            newTag2.Update();
            //End copy TaggedValue

            // duplicatedClass.IsActive = IsRoot;
            duplicatedClass.Stereotype = CD.GetPrimitiveStereotype();
            duplicatedClass.StereotypeEx = CD.GetPrimitiveStereotype();
            duplicatedClass.Status = parentElement.Status;
            duplicatedClass.Version = parentElement.Version;
            duplicatedClass.Multiplicity = parentElement.Multiplicity;
            duplicatedClass.Priority = parentElement.Priority;
            duplicatedClass.Alias = parentElement.Alias;
            // duplicatedClass.Abstract = parentElement.Abstract;
            duplicatedClass.Update();
            repo.ReloadDiagram(targetedDiagram.DiagramID);

            //Adding Duplicated class to the diagram
            EA.DiagramObject diagTargetedObj = (EA.DiagramObject)targetedDiagram.DiagramObjects.AddNew("l=200;r=300;t=200;b=300;", "");
            diagTargetedObj.ElementID = duplicatedClass.ElementID;
            //
            string ObjectColor = "Default";
            try
            {
                ObjectColor = xmlParser.GetXmlValueConfig("ConfigColor").Trim();
            }
            catch
            {

            }
            if (!ObjectColor.Equals("Default"))
            {
                diagTargetedObj.Style = "BCol=" + ObjectColor + ";BFol=0;LCol=0;LWth=1;";
            }
            //

            diagTargetedObj.bottom = diagParentObj.bottom;
            diagTargetedObj.top = diagParentObj.top;
            diagTargetedObj.right = diagParentObj.right;
            diagTargetedObj.left = diagParentObj.left;

            diagTargetedObj.Update();

            //Deleting the parent class if copy parent =true


            short CptToDelete = -1;
            for (short i = 0; targetedDiagram.DiagramObjects.Count > i; i++)
            {
                EA.DiagramObject AnObject = (EA.DiagramObject)targetedDiagram.DiagramObjects.GetAt(i);
                if (AnObject.ElementID.Equals(diagParentObj.ElementID))
                {
                    CptToDelete = i;
                    break;
                }
            }
            targetedDiagram.DiagramObjects.Delete(CptToDelete);
            targetedDiagram.DiagramObjects.Refresh();
            targetedDiagram.Update();
            repo.ReloadDiagram(targetedDiagram.DiagramID);
            diagParentObj = null;
            diagTargetedObj.Update();


            /////////////////End duplicate the targeted class

            /////////////////Creating Link IsBaseOn
            EA.Connector newBasedOn = (EA.Connector)duplicatedClass.Connectors.AddNew("", "Dependency");

            newBasedOn.Stereotype = CD.GetIsBasedOnStereotype();
            newBasedOn.SupplierID = parentElement.ElementID;
            //Allow to add a cardinlaity to the IBO
            //newBasedOn.SupplierEnd.Cardinality="0..*";
            newBasedOn.Update();
            duplicatedClass.Connectors.Refresh();

            //  duplicatedClass.Update();


            //////////////////Refreshing
            targetedDiagram.DiagramObjects.Refresh();
            targetedDiagram.Update();
            repo.ReloadDiagram(targetedDiagram.DiagramID);
            repo.RefreshModelView(targetedPackage.PackageID);
            //  repo.RefreshOpenDiagrams(false);
            //targetedDiagram.
        }
        /// <summary>
        /// program to automaticaly download in the profile (targetedPackage) all the non
        /// yet existing ancesters of a profile class just downloaded
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="targetedDiagram"></param>
        /// <param name="targetedPackage"></param>
        /// <param name="IBOElement"></param>
        /// <param name="parentElement"></param>
        /// <param name="ParentObjectTop"></param>
        /// <param name="ParentObjectBottom"></param>
        /// <param name="ParentObjectRight"></param>
        /// <param name="ParentObjectLeft"></param>
        /// <param name="ParentObjectStyle"></param>
        /// <param name="ParentWasHere"></param>
        public void addAncesters(EA.Repository repo, EA.Diagram targetedDiagram, EA.Package targetedPackage, EA.Element IBOElement, EA.Element parentElement, int ParentObjectTop, int ParentObjectBottom, int ParentObjectRight, int ParentObjectLeft, string ParentObjectStyle, bool ParentWasHere)
        {
            try
            {
                Utilitaires util = new Utilitaires(repo);
                Dictionary<long, EA.Element> dicAncestersByID = new Dictionary<long, EA.Element>();// gives the ancester element by its ID
                Dictionary<long, EA.Element> dicProfelementsByAncesterID = new Dictionary<long, EA.Element>();// gives the corresponding profelement by its ID
                List<long> ancesters = new List<long>();
                List<EA.Element> downloadedAncesters = new List<EA.Element>();
                util.getAncesters(parentElement, ancesters, dicAncestersByID);
                if (!dicAncestersByID.ContainsKey(parentElement.ElementID)) dicAncestersByID[parentElement.ElementID] = parentElement;
                List<EA.Element> profelements = new List<EA.Element>();
                
                IBOElement.Update();
                targetedPackage.Update();
                repo.SaveDiagram(targetedDiagram.DiagramID);
                util.GetAllElements(targetedPackage, profelements);

                // we now collect all the children which are already in the profile
                
                foreach (EA.Element el in profelements) // collect the profelement instance of an ancester
                {
                    string prov = el.Name;
                    EA.Element parent = repo.GetElementByGuid(util.getEltParentGuid(el));
                    long parentid = parent.ElementID;
                    if (dicAncestersByID.ContainsKey(parentid))// this element is an ancester
                    {
                        if (!dicProfelementsByAncesterID.ContainsKey(parentid)) dicProfelementsByAncesterID[parentid] = el;
                    }
                }
                if (!dicProfelementsByAncesterID.ContainsKey(IBOElement.ElementID)) dicProfelementsByAncesterID[parentElement.ElementID]=IBOElement;
                
                foreach (long ancestid in dicAncestersByID.Keys)
                {
                    if (!dicProfelementsByAncesterID.ContainsKey(ancestid)) // this anscester must be downloaded
                    {
                        EA.Element profel = util.copyElement(dicAncestersByID[ancestid], targetedPackage,"sanslien");
                        downloadedAncesters.Add(profel);
                        dicProfelementsByAncesterID[ancestid] = profel;
                    }
                }
                // now we create the generalization hierarchy 
                foreach (long ancestid in dicAncestersByID.Keys)
                {
                  
                    EA.Element profel = dicProfelementsByAncesterID[ancestid];
                    string prov = profel.Name;
                    bool ok = false; // true if the profelement ihherits already from another profelement
                    foreach (EA.Connector con in profel.Connectors)
                    {
                        if ((con.Type == "Generalization") && (profel.ElementID == con.ClientID))
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (ok) continue; // nothing todo
                    // we mut establish the generalization association
                    EA.Element ancester = dicAncestersByID[ancestid]; // recover the ancester
                                                                      // look for its ancester
                    string guid = ""; // the connector guid
                    int theotherendid=0;
                    foreach (EA.Connector con in ancester.Connectors)
                    {
                        if ((con.Type == "Generalization") && (ancester.ElementID == con.ClientID))
                        {
                            ok = true;
                            guid = con.ConnectorGUID;
                            theotherendid =dicProfelementsByAncesterID[con.SupplierID].ElementID;
                            break;
                        }
                    }
                    if (ok) // there is an ancester an association mustbe created
                    {
                        EA.Connector newcon = (EA.Connector)profel.Connectors.AddNew("", "Generalization");
                        newcon.ClientID = profel.ElementID;
                        newcon.SupplierID = theotherendid;
                        newcon.Update();
                        profel.Connectors.Refresh();
                        EA.ConnectorTag contag = (EA.ConnectorTag)newcon.TaggedValues.AddNew(CD.GetIBOTagValue(), guid);
                        contag.Update();
                        newcon.TaggedValues.Refresh();
                        newcon.Update();
                        profel.Update();
                    }


                }

                // now we create the diagram objects
               
                repo.ReloadDiagram(targetedDiagram.DiagramID);
                foreach (EA.Element duplicatedClass in downloadedAncesters)
                {
                    //Adding Duplicated class to the diagram
                    EA.DiagramObject diagTargetedObj = (EA.DiagramObject)targetedDiagram.DiagramObjects.AddNew("l=200;r=300;t=200;b=300;", "");
                    diagTargetedObj.ElementID = duplicatedClass.ElementID;
                    //
                    string ObjectColor = "Default";
                    try
                    {
                        ObjectColor = XMLP.GetXmlValueConfig("ConfigColor").Trim();
                    }
                    catch
                    {

                    }
                    if (!ObjectColor.Equals("Default"))
                    {
                        diagTargetedObj.Style = "BCol=" + ObjectColor + ";BFol=0;LCol=0;LWth=1;";
                    }
                    //
                    diagTargetedObj.bottom = ParentObjectBottom;
                    diagTargetedObj.top = ParentObjectTop;
                    diagTargetedObj.right = ParentObjectRight;
                    diagTargetedObj.left =ParentObjectLeft;

                    diagTargetedObj.Update();

                    //Deleting the parent class if copy parent =true

                    targetedDiagram.DiagramObjects.Refresh();
                    targetedDiagram.Update();
                   // repo.ReloadDiagram(targetedDiagram.DiagramID);
                   // diagTargetedObj.Update();
                }
                repo.SaveDiagram(targetedDiagram.DiagramID);
                repo.ReloadDiagram(targetedDiagram.DiagramID);
            }
            catch (Exception ee)
            {
                Utilitaires util = new Utilitaires(repo);
                util.wlog("CreIsBasedOn", "error in addAncesters " + ee.Message);
            }

        }

        //----------------------------------------------------------------

        //---------------------------  modifs am march 2019 -------------------------------------------------
        /// <summary>
        /// prepare a dictionary of the newlist of attributes based on existing IBOElement 
        /// and his parenelement in case of update
        /// </summary>
        /// <param name="IBOElement"></param>
        /// <param name="ParentElement"></param>
        /// <param name="dicNewAttributesByName"></param>
        void prepareUpdateAttributeList(EA.Element IBOElement, EA.Element ParentElement, Dictionary<string, EA.Attribute> dicNewAttributesByName)
        {

        }
        //-------------------------------------------------------------
    }

}
