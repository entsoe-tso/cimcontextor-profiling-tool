using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
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
    public class EAClassAttribute
    {
        private ConstantDefinition CD = new ConstantDefinition();
        private EA.Repository repo;
        private EAClass ParentEAClass;
        private string stereotype;
        private string guid;
        private string lowerBound;
        private string upperBound;
        private string ParentUB;
        private string ParentLB;
        private EA.Attribute AParentAttribute = null;
        private string name;
        private string type;
        private string notes;
        private int classifierID;
        private bool isConstant = false;
        private bool isStatic = false;
        private string defaultValue = "";
        private ArrayList constraintsList = new ArrayList();
        private ArrayList classifierConstraint = new ArrayList();
        private bool uiSelectedState = true;
        private bool isHerited=false;
        private int Pos=0;
        private ArrayList ClassifierConstraintList = new ArrayList();
        private Dictionary<string, EA.Attribute> dicParentEltAttributeByName ; //am avr 19 dictionnaire des atributs de l'element parent par son nom
        #region newconstructor 
        // newconstructor  am avr 19
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="repo">EA's Repository</param>
        /// <param name="attributeGUID">EA's Attribute GUID</param>
        /// <param name="ParentObject">EA's Parent Object</param>
        /// <param name="PrimitiveType">For primitive attribute, it's the type of the primitive (string, boolean...)</param>
        /// <param name="Mode">Mode is depending of the type of the class (class, datatype, primitive)</param>
        /// <param name="dicEltAttributes">dictionnaire des atributs de l'element parent par son nom</param>//am avr 19
        public EAClassAttribute(EA.Repository repo, string attributeGUID, EAClass ParentObject, string PrimitiveType, string Mode,Dictionary <string,EA.Attribute> dicEltAttributes)
        {
            EA.Attribute atr = repo.GetAttributeByGuid(attributeGUID);//am avr 19
            #region NotPrimitive
            if (!Mode.Equals(CD.GetPrimitiveStereotype()))
            {
                this.repo = repo;
                this.guid = attributeGUID;
                this.ParentEAClass = ParentObject;
                this.dicParentEltAttributeByName = dicEltAttributes;//am avr 19
                /******  am avr 19 **********************************/
                this.classifierID = atr.ClassifierID;
                this.name = atr.Name;
                this.type = atr.Type;
                this.SetStereotype(atr.StereotypeEx);
                this.SetNotes(atr.Notes);
                this.SetConstantState(atr.IsConst);
                this.SetDefaultValue(atr.Default);
                this.SetStaticState(atr.IsStatic);
                this.SetUpperBound(atr.UpperBound);
                this.SetLowerBound(atr.LowerBound);
                this.SetPos(atr.Pos);
                /****************************************************/
                #region heritedOrNot
                /* ABA20230607
                bool found = false;
                if (this.dicParentEltAttributeByName.ContainsKey(this.name)) found = true;
                if (found)
                {
                    this.isHerited = false;
                }
                else
                {
                    this.isHerited = true;
                }
                */
                if (this.dicParentEltAttributeByName.ContainsKey(this.name))
                {
                    this.isHerited = false;
                }
                else
                {
                    this.isHerited = true;
                }
                #endregion
                EA.Element AParentObj = repo.GetElementByGuid(ParentObject.GetParentElementGUID());
                EA.Collection attrsEx = AParentObj.AttributesEx;
                for (short i = 0; AParentObj.AttributesEx.Count > i; i++)
                {
                    EA.Attribute AnAttribute = (EA.Attribute)attrsEx.GetAt(i);
                    if (this.name.Equals(AnAttribute.Name))
                    {
                        AParentAttribute = AnAttribute;
                        break;
                    }
                }

                if (!(AParentAttribute == null))
                {
                    this.ParentLB = AParentAttribute.LowerBound;
                    this.ParentUB = AParentAttribute.UpperBound;
                }
                else
                {
                    this.ParentLB = "";
                    this.ParentUB = "";
                }
                XMLParser XMLP = new XMLParser(repo);
                ArrayList genericClassifierConstraintList = XMLP.GetXmlClassifierConstraint(AParentAttribute.Type.ToLower());
                EA.Collection attrConstraints = repo.GetAttributeByGuid(attributeGUID).Constraints;
                short count = attrConstraints.Count;
                for (short i = 0; i < count; i++)
                {
                    EA.AttributeConstraint aConstraint = (EA.AttributeConstraint)attrConstraints.GetAt(i);
                    bool ConstraintFound = false;
                    foreach (XMLClassifierConstraint AGenConstraint in genericClassifierConstraintList)
                    {
                        if (AGenConstraint.GetName().Equals(aConstraint.Name))
                        {
                            ConstraintFound = true;
                            break;
                        }
                    }
                    if (ConstraintFound == false)
                    {
                        constraintsList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", this.guid, aConstraint.Name, this.ParentEAClass));
                    }
                    else
                    {
                        ClassifierConstraintList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", this.guid, aConstraint.Name, this.ParentEAClass));
                    }
                }
            }
            #endregion
            #region AddPrimitive
            else
            {   //Case of AddprimitiveAttribute
                if (attributeGUID == null)
                {
                    this.repo = repo;
                    this.ParentEAClass = ParentObject;
                    EA.Element parElem = repo.GetElementByGuid(ParentObject.GetParentElementGUID());
                    this.classifierID = parElem.ElementID;
                    this.guid = "";
                    this.lowerBound = "";
                    this.upperBound = "";
                    this.name = "value";
                    this.type = parElem.Name;
                    this.SetNotes(parElem.Notes);
                }
                #endregion
                #region LoadPrimitive
                else//Case of loadPrimitiveAttribute
                {
                    this.repo = repo;
                    this.ParentEAClass = ParentObject;
                    this.classifierID = repo.GetElementByGuid(ParentObject.GetParentElementGUID()).ElementID;
                    this.guid = attributeGUID;
                    this.isConstant = repo.GetAttributeByGuid(attributeGUID).IsConst;
                    this.stereotype = ((EA.Attribute)ParentObject.GetIBOElement().Attributes.GetAt(0)).StereotypeEx;
                    EA.Attribute attr = repo.GetAttributeByGuid(attributeGUID);
                    this.isStatic = attr.IsStatic;
                    this.defaultValue = attr.Default;
                    this.lowerBound = attr.LowerBound;
                    this.upperBound = attr.UpperBound;
                    this.ParentLB = lowerBound;
                    this.ParentUB = upperBound;

                    this.name = attr.Name;
                    this.type = repo.GetElementByGuid(ParentObject.GetParentElementGUID()).Name;
                    this.SetNotes(attr.Notes);

                    XMLParser XMLP = new XMLParser(repo);
                    ArrayList genericClassifierConstraintList = XMLP.GetXmlClassifierConstraint(this.type.ToLower());
                    EA.Collection attrConstraints = attr.Constraints;
                    short count = attrConstraints.Count;
                    for (short i = 0; i < count; i++)
                    {
                        EA.AttributeConstraint aConstraint = (EA.AttributeConstraint)attrConstraints.GetAt(i);
                        bool ConstraintFound = false;
                        foreach (XMLClassifierConstraint AGenConstraint in genericClassifierConstraintList)
                        {
                            if (AGenConstraint.GetName().Equals(aConstraint.Name))
                            {
                                ConstraintFound = true;
                                break;
                            }
                        }
                        if (ConstraintFound == false)
                        {
                            constraintsList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", this.guid, aConstraint.Name, this.ParentEAClass));
                        }
                        else
                        {
                            ClassifierConstraintList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", this.guid, aConstraint.Name, this.ParentEAClass));
                        }

                    }
                }
            }
            #endregion
        }
        #endregion



        #region oldconstructor
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="repo">EA's Repository</param>
        /// <param name="attributeGUID">EA's Attribute GUID</param>
        /// <param name="ParentObject">EA's Parent Object</param>
        /// <param name="PrimitiveType">For primitive attribute, it's the type of the primitive (string, boolean...)</param>
        /// <param name="Mode">Mode is depending of the type of the class (class, datatype, primitive)</param>
        public EAClassAttribute(EA.Repository repo, string attributeGUID, EAClass ParentObject, string PrimitiveType, string Mode)
        {
            EA.Attribute atr = repo.GetAttributeByGuid(attributeGUID);//am avr 19
            #region NotPrimitive
            if (!Mode.Equals(CD.GetPrimitiveStereotype()))
            {
                this.repo = repo;
                this.guid = attributeGUID;
                this.ParentEAClass = ParentObject;
                /******  am avr 19 **********************************/
                this.classifierID = atr.ClassifierID;
                this.name = atr.Name;
                this.type = atr.Type;
                this.SetStereotype(atr.StereotypeEx);
                this.SetNotes(atr.Notes);
                this.SetConstantState(atr.IsConst);
                this.SetDefaultValue(atr.Default);
                this.SetStaticState(atr.IsStatic);
                this.SetUpperBound(atr.UpperBound);
                this.SetLowerBound(atr.LowerBound);
                this.SetPos(atr.Pos);
                /****************************************************/
                #region heritedOrNot
                EA.Element parentelt = repo.GetElementByGuid(ParentEAClass.GetParentElementGUID());//
                bool found = false;
                for (short i = 0; repo.GetElementByGuid(ParentEAClass.GetParentElementGUID()).Attributes.Count >i ;i++ )
                {
                    EA.Attribute ADirectAttribute = (EA.Attribute) repo.GetElementByGuid(ParentEAClass.GetParentElementGUID()).Attributes.GetAt(i); 
                    if(ADirectAttribute.Name.Equals(this.name)){
                        found = true;
                        break; //am avr 19
                    }
                }
                if (found.Equals(true))
                {
                    this.isHerited = false;
                }
                else
                {
                    this.isHerited = true;
                }
#endregion

                EA.Element AParentObj = repo.GetElementByGuid(ParentObject.GetParentElementGUID());

                for (short i = 0; AParentObj.AttributesEx.Count > i; i++)
                {
                    EA.Attribute AnAttribute = (EA.Attribute)AParentObj.AttributesEx.GetAt(i);
                    if (this.name.Equals(AnAttribute.Name))
                    {
                        AParentAttribute = AnAttribute;
                        break;
                    }
                }
                if (!(AParentAttribute == null))
                {
                    this.ParentLB = AParentAttribute.LowerBound;
                    this.ParentUB = AParentAttribute.UpperBound;
                }
                else
                {
                    this.ParentLB = "";
                    this.ParentUB = "";
                }
                XMLParser XMLP = new XMLParser(repo);
                ArrayList genericClassifierConstraintList = XMLP.GetXmlClassifierConstraint(AParentAttribute.Type.ToLower());

                for (short i = 0; repo.GetAttributeByGuid(attributeGUID).Constraints.Count > i; i++)
                {
                    EA.AttributeConstraint aConstraint = (EA.AttributeConstraint)repo.GetAttributeByGuid(attributeGUID).Constraints.GetAt(i);
                    bool ConstraintFound = false;
                    foreach (XMLClassifierConstraint AGenConstraint in genericClassifierConstraintList)
                    {
                        if (AGenConstraint.GetName().Equals(aConstraint.Name))
                        {
                            ConstraintFound = true;
                            break;
                        }
                    }
                    if (ConstraintFound.Equals(false))
                    {
                        constraintsList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", this.guid, aConstraint.Name, this.ParentEAClass));
                    }
                    else
                    {
                        ClassifierConstraintList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", this.guid, aConstraint.Name, this.ParentEAClass));
                    }
                }
            }
            #endregion
            #region AddPrimitive
            else
            {   //Case of AddprimitiveAttribute
                if (attributeGUID == null)
                {
                    this.repo = repo;
                    this.ParentEAClass = ParentObject;
                    this.classifierID = repo.GetElementByGuid(ParentObject.GetParentElementGUID()).ElementID;
                    this.guid = "";
                    this.lowerBound = "";
                    this.upperBound = "";
                    this.name = "value";
                    this.type = repo.GetElementByGuid(ParentObject.GetParentElementGUID()).Name;
                    this.SetNotes(repo.GetElementByGuid(ParentObject.GetParentElementGUID()).Notes);
                }
            #endregion
            #region LoadPrimitive
                else//Case of loadPrimitiveAttribute
                {
                    this.repo = repo;
                    this.ParentEAClass = ParentObject;
                    this.classifierID = repo.GetElementByGuid(ParentObject.GetParentElementGUID()).ElementID;
                    this.guid = attributeGUID;
                    this.isConstant = repo.GetAttributeByGuid(attributeGUID).IsConst;
                    this.stereotype = ((EA.Attribute)ParentObject.GetIBOElement().Attributes.GetAt(0)).StereotypeEx;
                    this.isStatic = repo.GetAttributeByGuid(attributeGUID).IsStatic;
                    this.defaultValue = repo.GetAttributeByGuid(attributeGUID).Default;
                    this.lowerBound = repo.GetAttributeByGuid(attributeGUID).LowerBound;
                    this.upperBound = repo.GetAttributeByGuid(attributeGUID).UpperBound;
                    this.ParentLB = lowerBound;
                    this.ParentUB = upperBound;

                    this.name = repo.GetAttributeByGuid(attributeGUID).Name;
                    this.type = repo.GetElementByGuid(ParentObject.GetParentElementGUID()).Name;
                    this.SetNotes(repo.GetAttributeByGuid(attributeGUID).Notes);

                    XMLParser XMLP = new XMLParser(repo);
                    ArrayList genericClassifierConstraintList = XMLP.GetXmlClassifierConstraint(this.type.ToLower());
                    for (short i = 0; repo.GetAttributeByGuid(attributeGUID).Constraints.Count > i; i++)
                    {
                        EA.AttributeConstraint aConstraint = (EA.AttributeConstraint)repo.GetAttributeByGuid(attributeGUID).Constraints.GetAt(i);
                        bool ConstraintFound = false;
                        foreach (XMLClassifierConstraint AGenConstraint in genericClassifierConstraintList)
                        {
                            if (AGenConstraint.GetName().Equals(aConstraint.Name))
                            {
                                ConstraintFound = true;
                                break;
                            }
                        }
                        if (ConstraintFound.Equals(false))
                        {
                            constraintsList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", this.guid, aConstraint.Name, this.ParentEAClass));
                        }
                        else
                        {
                            ClassifierConstraintList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", this.guid, aConstraint.Name, this.ParentEAClass));
                        }

                    }
                }
            }
                #endregion
        }
        #endregion
        /// <summary>
        /// Add a constraint to the attribute
        /// </summary>
        /// <param name="constraintName">the name of the constraint (also the identifier)</param>
        /// <param name="constraintType">the type of the constraint (mostly OCL)</param>
        /// <param name="constraintNotes">the constraint value/note</param>
        /// <returns></returns>
        public EAClassAttributeConstraint AddConstraint(string constraintName, string constraintType, string constraintNotes)
        {
            EAClassAttributeConstraint aConstraint = new EAClassAttributeConstraint(repo, CD.GetCreate(), "", constraintName, this.ParentEAClass);
            aConstraint.SetNotes(constraintNotes);
            aConstraint.SetType(constraintType);
            constraintsList.Add(aConstraint);
            return aConstraint;
        }

        /// <summary>
        /// Add a classifier constraint to the attribute (only used for datatype where you can't change the classifier)
        /// </summary>
        /// <param name="constraintName">the name of the constraint (also the identifier)</param>
        /// <param name="constraintType">the type of the constraint (mostly OCL)</param>
        /// <param name="constraintNotes">the constraint value/note</param>
        /// <returns></returns>
        public EAClassAttributeConstraint AddClassifierConstraint(string constraintName, string constraintType, string constraintNotes)
        {
            EAClassAttributeConstraint aConstraint = new EAClassAttributeConstraint(repo, CD.GetCreate(), "", constraintName, this.ParentEAClass);
            aConstraint.SetNotes(constraintNotes);
            aConstraint.SetType(constraintType);
            ClassifierConstraintList.Add(aConstraint);
            return aConstraint;
        }

        /// <summary>
        /// Destroy the classifier constraint list
        /// </summary>
        public void RemoveAllClassifierConstraint()
        {
            ClassifierConstraintList = new ArrayList();
        }

        /// <summary>
        /// Return the parent's upper bound cardinality of the attribute
        /// </summary>
        /// <returns></returns>
        public string GetParentUB()
        {
            return this.ParentUB;
        }

        public bool CheckIfSameElement()
        {
            if (uiSelectedState.Equals(true))
            {
                bool found = false;
                for(short i =0;ParentEAClass.GetIBOElement().Attributes.Count>i;i++){
                    EA.Attribute AnAttribute = (EA.Attribute) ParentEAClass.GetIBOElement().Attributes.GetAt(i);
                    if(AnAttribute.Name.Equals(this.name)){
                        found = true;
                    }
                }
                if(found.Equals(false)){
                    return false;
                }

            }
            else
            {
                bool found = false;
                for (short i = 0; ParentEAClass.GetIBOElement().Attributes.Count > i; i++)
                {
                    EA.Attribute AnAttribute = (EA.Attribute)ParentEAClass.GetIBOElement().Attributes.GetAt(i);
                    if (AnAttribute.Name.Equals(this.name))
                    {
                        found = true;
                    }
                }
                if (found.Equals(true))
                {
                    return false;
                }
                return true;
            }

            if(!(this.GetName().Equals(AParentAttribute.Name)))
            {
                return false;
            }
            
            if(!(this.defaultValue.Equals(AParentAttribute.Default))){
                return false;
            }

            if(!(this.isConstant.Equals(AParentAttribute.IsConst))){
                return false;
            }

            if(!(this.GetLowerBound().Equals(AParentAttribute.LowerBound))){
                return false;
            }

            if(!(this.GetUpperBound().Equals(AParentAttribute.UpperBound))){
                return false;
            }

            if(!(this.GetStaticState().Equals(AParentAttribute.IsStatic))){
                return false;
            }

            if(!(this.stereotype.Equals(AParentAttribute.Stereotype))){
                return false;
            }

            if (!(this.notes.Equals(AParentAttribute.Notes)))
            {
                return false;
            }

            for (short i = 0; AParentAttribute.Constraints.Count>i ;i++)
            {
               EA.AttributeConstraint AnAttConst =  (EA.AttributeConstraint)AParentAttribute.Constraints.GetAt(i);
               bool Found = false;

                if(!(AnAttConst.Name.Equals(CD.GetIBOTagValue()))){
                    foreach (EAClassAttributeConstraint AConstraint in constraintsList)
                    {
                        if(AConstraint.GetName().Equals(AnAttConst.Name)){
                            Found = AConstraint.CheckIfSameElement(AnAttConst);
                        }
                    }

                   if(Found.Equals(false)){
                       return false;
                   }
                }
            }
            

            return true;
        }

        private void SetPos(int value)
        {
            this.Pos = value;
        }

        private int GetPos()
        {
            return this.Pos;
        }

        /// <summary>
        /// Return the parent's lowerbound cardinality of the attribute
        /// </summary>
        /// <returns></returns>
        public string GetParentLB()
        {
            return this.ParentLB;
        }

        /// <summary>
        /// Return the parent's EAClass of the attribute
        /// </summary>
        /// <returns></returns>
        public EAClass getClass()
        {
            return this.ParentEAClass;
        }

        /// <summary>
        /// Return the name of the attribute
        /// </summary>
        /// <returns>string</returns>
        public string GetName()
        {
            return this.name;
        }

        public bool GetHeritedState()
        {
            return isHerited;
        }

        /// <summary>
        /// Return if the attribute is a constant (true=yes)
        /// </summary>
        /// <returns>boolean</returns>
        public bool GetConstantState()
        {
            return this.isConstant;
        }

        /// <summary>
        /// Return the default value of the attribute if he have one.
        /// </summary>
        /// <returns></returns>
        public string GetDefaultValue()
        {
            return this.defaultValue;
        }

        /// <summary>
        /// Set the type of the attribute.
        /// </summary>
        /// <param name="newType"></param>
        public void SetType(string newType)
        {
            this.type = newType;
        }

        /// <summary>
        /// Set the default value of the attribute
        /// </summary>
        /// <param name="defaultValue"></param>
        public void SetDefaultValue(string defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// Set the static value of the attribute
        /// </summary>
        /// <param name="StaticState"></param>
        public void SetStaticState(bool StaticState)
        {
            this.isStatic = StaticState;
        }

        /// <summary>
        /// Return the static state of the attribute
        /// </summary>
        /// <returns></returns>
        public bool GetStaticState()
        {
            return isStatic;
        }

        /// <summary>
        /// Set the constant state of the attribute
        /// </summary>
        /// <param name="state"></param>
        public void SetConstantState(bool state)
        {
            this.isConstant = state;
        }

        /// <summary>
        /// Return the classifier ID of the attribute
        /// </summary>
        /// <returns></returns>
        public int GetClassifier()
        {
            return classifierID;
        }

        /// <summary>
        /// Set the GUID of the attribute
        /// </summary>
        /// <param name="newGUID"></param>
        public void SetGUID(string newGUID)
        {
            this.guid = newGUID;
        }

        /// <summary>
        /// Set the classifier of the attribute
        /// </summary>
        /// <param name="newClassifierID"></param>
        public void SetClassifier(int newClassifierID)
        {
            this.classifierID = newClassifierID;
            EA.Element classifierElement = repo.GetElementByID(newClassifierID);
            this.SetType(classifierElement.Name);
        }

        /// <summary>
        /// Used to know if the attribute have been checked in the ui.
        /// true = checked = attribute will be copied during the IBO
        /// false = unchecked = attribute will be ignored during the IBO
        /// </summary>
        /// <returns></returns>
        public bool GetUISelectedState()
        {
            return this.uiSelectedState;
        }

        /// <summary>
        /// Used to set if the attribute have been checked in the ui.
        /// true = checked = attribute will be copied during the IBO
        /// false = unchecked = attribute will be ignored during the IBO
        /// </summary>
        /// <param name="newState"></param>
        public void SetUISelectedState(bool newState)
        {
            this.uiSelectedState = newState;
        }

        /// <summary>
        /// Return the type of the attribute
        /// </summary>
        /// <returns></returns>
        public new string GetType()
        {
            return this.type;
        }

        /// <summary>
        /// Return the attribute upperbound
        /// </summary>
        /// <returns></returns>
        public string GetUpperBound()
        {
            return this.upperBound;
        }

        /// <summary>
        /// Retuurn the attribute lower bound cardinality
        /// </summary>
        /// <returns></returns>
        public string GetLowerBound()
        {
            return this.lowerBound;
        }

        /// <summary>
        /// Set the attribute upperbound cardinality
        /// </summary>
        /// <param name="newUpperBound"></param>
        public void SetUpperBound(string newUpperBound)
        {
            this.upperBound = newUpperBound;
        }

        /// <summary>
        /// Set the attribute lowerbound cardinality
        /// </summary>
        /// <param name="newLowerBound"></param>
        public void SetLowerBound(string newLowerBound)
        {
            this.lowerBound = newLowerBound;
        }

        /// <summary>
        /// Return the attribute stereotype
        /// </summary>
        /// <returns></returns>
        public string GetStereotype()
        {
            return this.stereotype;
        }

        /// <summary>
        /// Set the attribute stereotype
        /// </summary>
        /// <param name="newStereotype"></param>
        public void SetStereotype(string newStereotype)
        {
            this.stereotype = newStereotype;
        }

        /// <summary>
        /// Return the note of the attribute
        /// </summary>
        /// <returns></returns>
        public string GetNotes()
        {
            return this.notes;
        }

        /// <summary>
        /// Set the attribute note
        /// </summary>
        /// <param name="newNotes"></param>
        public void SetNotes(string newNotes)
        {
            this.notes = newNotes;
        }

        /// <summary>
        /// Return the guid of the attribute
        /// </summary>
        /// <returns></returns>
        public string GetGUID()
        {
            return guid;
        }

        /// <summary>
        /// Return the constraint list on the attribute
        /// </summary>
        /// <returns>array list of EAClassAttributeConstraint</returns>
        public ArrayList GetConstraints()
        {
            return constraintsList;
        }

        /// <summary>
        /// Return all classifier constraint into an arraylist of EAClassAttributeConstraint
        /// </summary>
        /// <returns>array list of EAClassAttributeConstraint</returns>
        public ArrayList GetClassifierConstraints()
        {
            return ClassifierConstraintList;
        }

        /// <summary>
        /// Return the searched classifier constraint
        /// </summary>
        /// <param name="searchedConstraintName">the name of the searched constraint</param>
        /// <returns></returns>
        public EAClassAttributeConstraint GetAnAttributeClassifierConstraint(string searchedConstraintName)
        {
            ArrayList populatedConstraints = new ArrayList();
            foreach (EAClassAttributeConstraint aConstraint in ClassifierConstraintList)
            {
                if (aConstraint.GetName().Equals(searchedConstraintName))
                {
                    return aConstraint;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Return the searched attribute constraint
        /// </summary>
        /// <param name="searchedConstraintName"></param>
        /// <returns></returns>
        public EAClassAttributeConstraint GetAnAttributeConstraint(string searchedConstraintName)
        {
            ArrayList populatedConstraints = new ArrayList();
            foreach (EAClassAttributeConstraint aConstraint in constraintsList)
            {
                if (aConstraint.GetName().Equals(searchedConstraintName))
                {
                    return aConstraint;
                }
            }
            return null;
        }

        public void DeleteAnAttributeConstraint(string ConstraintToDelete)
        {
            EAClassAttributeConstraint aConstraint = this.GetAnAttributeConstraint(ConstraintToDelete);
            if(!(aConstraint==null)){
                constraintsList.Remove(aConstraint);
            }
        }

        //
        public void DeleteAClassifierConstraint(string constraintName)
        {
            EAClassAttributeConstraint ElementToDelete = null;
            foreach (EAClassAttributeConstraint aConst in ClassifierConstraintList)
            {
                if (aConst.GetName().Equals(constraintName))
                {
                    ElementToDelete = aConst;
                    break;
                }
            }
            if (ElementToDelete != null)
            {

                ClassifierConstraintList.Remove(ElementToDelete);
            }

        }
        //


        /// <summary>
        /// Reset the attribute data to the original value
        /// </summary>
        public void Reset()
        {
            EA.Attribute EAAttribute = repo.GetAttributeByGuid(guid);
            this.SetUpperBound(EAAttribute.UpperBound);
            this.SetLowerBound(EAAttribute.LowerBound);
            this.SetClassifier(EAAttribute.ClassifierID);
            this.name = EAAttribute.Name;
            this.type = EAAttribute.Type;
            this.SetStereotype(EAAttribute.Stereotype);
            this.SetNotes(EAAttribute.Notes);
            this.SetConstantState(EAAttribute.IsConst);
            this.SetDefaultValue(EAAttribute.Default);

            constraintsList = new ArrayList();
            //foreach (EA.AttributeConstraint aConstraint in repo.GetAttributeByGuid(guid).Constraints)
            //{
            for (short i = 0; repo.GetAttributeByGuid(guid).Constraints.Count > i; i++)
            {
                EA.AttributeConstraint aConstraint = (EA.AttributeConstraint)repo.GetAttributeByGuid(guid).Constraints.GetAt(i);
                constraintsList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", this.guid, aConstraint.Name, this.ParentEAClass));
            }
        }

        /// <summary>
        /// Reset an attribute
        /// </summary>
        /// <param name="NewGUID"></param>
        public void ResetFromGUID(string NewGUID)
        {
            EA.Attribute EAAttribute = null;
            //foreach(EA.Attribute AnAttribute in  repo.GetElementByGuid(ParentEAClass.GetParentElementGUID()).Attributes ){
            for (short i = 0; repo.GetElementByGuid(ParentEAClass.GetParentElementGUID()).Attributes.Count > i; i++)
            {
                EA.Attribute AnAttribute = (EA.Attribute)repo.GetElementByGuid(ParentEAClass.GetParentElementGUID()).Attributes.GetAt(i);
                if (AnAttribute.Name.Equals(repo.GetAttributeByGuid(NewGUID).Name))
                {
                    EAAttribute = AnAttribute;
                    break;
                }
            }
            if (!(EAAttribute == null))
            {
                this.SetUpperBound(EAAttribute.UpperBound);
                this.SetLowerBound(EAAttribute.LowerBound);

                this.SetClassifier(EAAttribute.ClassifierID);
                this.name = EAAttribute.Name;
                this.type = EAAttribute.Type;
                this.SetStereotype(EAAttribute.Stereotype);
                this.SetNotes(EAAttribute.Notes);
                this.SetConstantState(EAAttribute.IsConst);
                this.SetDefaultValue(EAAttribute.Default);

                constraintsList = new ArrayList();
                //foreach (EA.AttributeConstraint aConstraint in repo.GetAttributeByGuid(guid).Constraints)
                for (short i = 0; repo.GetAttributeByGuid(guid).Constraints.Count > i; i++)
                {
                    EA.AttributeConstraint aConstraint = (EA.AttributeConstraint)repo.GetAttributeByGuid(guid).Constraints.GetAt(i);
                    constraintsList.Add(new EAClassAttributeConstraint(repo, "ATTRIBUT", NewGUID, aConstraint.Name, this.ParentEAClass));
                }
            }
        }

        /// <summary>
        /// Will create the attribute and ask for each sub object to create himself.
        /// Only used in the case of an update (not a primitive).
        /// </summary>
        /// <param name="duplicatedClass"></param>
        /// <param name="copyTagValues"></param>
        private void UpdateIsBasedOn(EA.Element duplicatedClass, bool copyTagValues)
        {

            if (this.GetUISelectedState().Equals(true))
            {
                EA.Attribute newChildAttribute = null;
                for (short i = 0; duplicatedClass.Attributes.Count > i; i++)
                {
                    EA.Attribute AnAttribute = (EA.Attribute)duplicatedClass.Attributes.GetAt(i);
                    if (AnAttribute.Name.Equals(this.GetName()))
                    {
                        newChildAttribute = AnAttribute;
                        break;
                    }
                }

                newChildAttribute.LowerBound = this.GetLowerBound();
                newChildAttribute.Type = this.GetType();
                newChildAttribute.UpperBound = this.GetUpperBound();
                newChildAttribute.StereotypeEx = this.GetStereotype();
                newChildAttribute.Notes = this.GetNotes();
                newChildAttribute.IsConst = this.GetConstantState();
                newChildAttribute.IsStatic = this.GetStaticState();
                newChildAttribute.ClassifierID = this.classifierID;
            
                newChildAttribute.Default = this.GetDefaultValue();
                // if (!(this.GetPos() == null)) ABAB20230219
                if (this.GetPos() > 0) // ABA20230219
                {
                        newChildAttribute.Pos = this.GetPos();
                }
                newChildAttribute.Update();

                if (copyTagValues.Equals(true))
                {
                    EA.Attribute parentAttribute = null;
                    

                    for (short i = 0; repo.GetElementByGuid(ParentEAClass.GetParentElementGUID()).Attributes.Count > i; i++)
                    {
                        EA.Attribute AnAttribute = (EA.Attribute)repo.GetElementByGuid(ParentEAClass.GetParentElementGUID()).Attributes.GetAt(i);
                        if (AnAttribute.Name.Equals(this.GetName()))
                        {
                            parentAttribute = AnAttribute;
                            break;
                        }
                    }

                    if (!(parentAttribute == null))
                    {
                        //-------------- glute am mars 2019 ------------------------
                        //  mise a jour de la valeur par default
                        if(newChildAttribute.Default != parentAttribute.Default)
                        {
                            if ((parentAttribute.IsStatic) && (parentAttribute.IsConst))
                            {
                                newChildAttribute.Default = parentAttribute.Default;
                            }
                        }
                        //  -------------------------------------

                        for (short i = 0; parentAttribute.TaggedValues.Count > i; i++)
                        {
                            EA.AttributeTag aTagValue = (EA.AttributeTag)parentAttribute.TaggedValues.GetAt(i);
                            
                            bool Found = false;
                            EA.AttributeTag OldTagValue = null;

                            for (short j = 0; newChildAttribute.TaggedValues.Count > j; j++)
                            {
                                EA.AttributeTag AChildTagValue = (EA.AttributeTag)newChildAttribute.TaggedValues.GetAt(j);
                                if (aTagValue.Name.Equals(AChildTagValue.Name))
                                {
                                    Found = true;
                                    OldTagValue = AChildTagValue;
                                    break;
                                }
                            }
                            
                            if (Found.Equals(false))
                            {
                                if (!aTagValue.Name.Equals(CD.GetIBOTagValue()))
                                {
                                    EA.AttributeTag newChildTaggedValue = (EA.AttributeTag)newChildAttribute.TaggedValues.AddNew(aTagValue.Name, aTagValue.Value);
                                    newChildTaggedValue.Notes = aTagValue.Notes;
                                    newChildTaggedValue.Update();
                                }
                            }
                            else
                            {
                                if (!aTagValue.Name.Equals(CD.GetIBOTagValue()))
                                {
                                    OldTagValue.Value = aTagValue.Value;
                                    OldTagValue.Notes = aTagValue.Notes;
                                    OldTagValue.Update();
                                }
                            }

                        }
                    }
                }
                newChildAttribute.Update();


                foreach (EAClassAttributeConstraint aConstraint in constraintsList)
                {
                    aConstraint.ExecuteIsBasedOn(newChildAttribute, false);
                }

                for (short i = 0; newChildAttribute.Constraints.Count>i ;i++ )
                {
                    EA.AttributeConstraint AConstToCheck = (EA.AttributeConstraint)newChildAttribute.Constraints.GetAt(i);
                    bool found = false;
                    //Checking constraintlist
                    foreach (EAClassAttributeConstraint aConstraint in constraintsList)
                    {

                        if (aConstraint.GetName().Equals(AConstToCheck.Name))
                        {
                            found = true;
                            break;
                        }
                    }

                    //checking ClassifierConstraintList
                    foreach (EAClassAttributeConstraint aConstraint in ClassifierConstraintList)
                    {

                        if (aConstraint.GetName().Equals(AConstToCheck.Name))
                        {
                            found = true;
                            break;
                        }
                    }

                    if(found==false){
                        newChildAttribute.Constraints.DeleteAt(i,true);
                    }
                }
                //add new constraint
                foreach (EAClassAttributeConstraint aConstraint in ClassifierConstraintList)
                {
                    aConstraint.ExecuteIsBasedOn(newChildAttribute, false);
                }



            }
            else
            {
                EA.Attribute newChildAttribute = null;
                short ifound = 0;
                
                for (short j = 0; duplicatedClass.Attributes.Count > j; j++)
                {
                    EA.Attribute AnAttribute = (EA.Attribute)duplicatedClass.Attributes.GetAt(j);
                    if (AnAttribute.Name.Equals(this.GetName()))
                    {
                        newChildAttribute = AnAttribute;
                        ifound = j;
                        break;
                    }
                }
                if (!(newChildAttribute == null))
                {
                    duplicatedClass.Attributes.Delete(ifound);
                    duplicatedClass.Attributes.Refresh();
                    duplicatedClass.Update();
                    duplicatedClass.Refresh();
                }
            }

        }

        /// <summary>
        /// Will create the attribute and ask for each sub object to create himself.
        /// Only used in the case of a creation (not primitive).
        /// </summary>
        /// <param name="duplicatedClass"></param>
        /// <param name="copyTagValues"></param>
        private void CreateIsBasedOn(EA.Element duplicatedClass, bool copyTagValues)
        {
            if (this.GetUISelectedState().Equals(true))
            {
                EA.Attribute newChildAttribute = (EA.Attribute)duplicatedClass.Attributes.AddNew(this.GetName(), this.GetType());
                
                newChildAttribute.LowerBound = this.GetLowerBound();
                newChildAttribute.UpperBound = this.GetUpperBound();
                newChildAttribute.StereotypeEx = this.GetStereotype();
                /** am mai 2018 **/
                if(duplicatedClass.Type=="Enumeration")
                {
                    newChildAttribute.Stereotype = "enum";
                }
                /**************************/
                //if(!(this.GetPos()==null)) ABA20230219
                if(this.GetPos() > 0) // ABA20230219
                {
                    newChildAttribute.Pos = this.GetPos();
                }
                newChildAttribute.Notes = this.GetNotes();
                newChildAttribute.IsStatic = this.GetStaticState();
                newChildAttribute.ClassifierID = this.classifierID;
                newChildAttribute.Default = this.GetDefaultValue();
                newChildAttribute.IsConst = this.GetConstantState();
                newChildAttribute.Update();

                #region copyTagValue
                if (copyTagValues.Equals(true))
                {
                    EA.Attribute parentAttribute = repo.GetAttributeByGuid(guid);
                    if (!parentAttribute.Equals(null))
                    {
                        for (short i = 0; parentAttribute.TaggedValues.Count > i; i++)
                        {
                            EA.AttributeTag aTagValue = (EA.AttributeTag)parentAttribute.TaggedValues.GetAt(i);
                            if(!aTagValue.Name.Equals(CD.GetIBOTagValue())){
                                EA.AttributeTag newChildTaggedValue = (EA.AttributeTag)newChildAttribute.TaggedValues.AddNew(aTagValue.Name, aTagValue.Value);
                                newChildTaggedValue.Notes = aTagValue.Notes;
                                newChildTaggedValue.Update();
                            }
                        }
                    }
                }
               // EA.AttributeTag newChildTaggedValue2 = (EA.AttributeTag)newChildAttribute.TaggedValues.AddNew(CD.GetIBOTagValue(), this.GetGUID());
                EA.AttributeTag newChildTaggedValue2 = (EA.AttributeTag)newChildAttribute.TaggedValues.AddNew(CD.GetIBOTagValue(), this.GetGUID());
                newChildTaggedValue2.Notes = CD.GetIBOTagValueNote();
                newChildTaggedValue2.Update();
                newChildAttribute.Update();
                #endregion


                foreach (EAClassAttributeConstraint aConstraint in constraintsList)
                {
                    aConstraint.ExecuteIsBasedOn(newChildAttribute, true);
                }
                foreach (EAClassAttributeConstraint aConstraint in ClassifierConstraintList)
                {
                    aConstraint.ExecuteIsBasedOn(newChildAttribute, true);
                }
            }
        }

        /// <summary>
        /// Will create the attribute and ask for each sub object to create himself.
        /// Only used in the case of a primitive.
        /// </summary>
        /// <param name="duplicatedClass"></param>
        private void PrimitiveIsBasedOn(EA.Element duplicatedClass)
        {
            if (this.GetUISelectedState().Equals(true))
            {
                EA.Attribute newChildAttribute = (EA.Attribute)duplicatedClass.Attributes.AddNew(this.GetName(), this.GetType());
                newChildAttribute.Notes = this.GetNotes();
                newChildAttribute.ClassifierID = this.classifierID;
                newChildAttribute.LowerBound = this.lowerBound;
                newChildAttribute.UpperBound = this.upperBound;
                newChildAttribute.IsStatic = this.isStatic;
                newChildAttribute.StereotypeEx = this.GetStereotype();
                newChildAttribute.IsConst = this.isConstant;
                newChildAttribute.Default = this.defaultValue;
                newChildAttribute.Update();

                foreach (EAClassAttributeConstraint aConstraint in constraintsList)
                {
                    aConstraint.ExecuteIsBasedOn(newChildAttribute, true);
                }
                foreach (EAClassAttributeConstraint aConstraint in ClassifierConstraintList)
                {
                    aConstraint.ExecuteIsBasedOn(newChildAttribute, true);
                }
            }
        }

        /// <summary>
        /// Will either use create/update/primitiveIsBasedOn.
        /// Depending of the stereotype of the class.
        /// </summary>
        /// <param name="duplicatedClass"></param>
        /// <param name="copyTagValues"></param>
        /// <param name="Mode"></param>
        public void ExecuteIsBasedOn(EA.Element duplicatedClass, bool copyTagValues, string Mode)
        {

            if (Mode.Equals(CD.GetPrimitiveStereotype()))
            {
                PrimitiveIsBasedOn(duplicatedClass);
            }
            else
            {
                if (Mode.Equals(CD.GetCreate()))
                {
                    CreateIsBasedOn(duplicatedClass, copyTagValues);
                }
                else
                {
                    bool AttributeFound = false;
                    for (short i = 0; duplicatedClass.Attributes.Count > i; i++)
                    {
                        EA.Attribute AnAttribute = (EA.Attribute)duplicatedClass.Attributes.GetAt(i);
                        if (AnAttribute.Name.Equals(this.GetName()))
                        {
                            AttributeFound = true;
                            break;
                        }
                    }
                    if (AttributeFound.Equals(true))
                    {
                        UpdateIsBasedOn(duplicatedClass, copyTagValues);
                    }
                    else
                    {
                        CreateIsBasedOn(duplicatedClass, copyTagValues);
                    }
                }
            }
        }
    }
}
