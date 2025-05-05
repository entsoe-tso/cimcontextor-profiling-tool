using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CimContextor.Configuration;
using CimContextor.utilitaires;
using CimContextor.Utilities;
/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.9.24                                        * november 2020 *
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
    public partial class IsBasedOnForm : Form
    {
        private ConstantDefinition CD = new ConstantDefinition();
        private EAClass populatedEAClass;
        private ArrayList qualifierList = new ArrayList();
        private ArrayList editedConstraints = new ArrayList();
        private ArrayList completeAttributeList;
        private EA.Repository Repository;
        private bool CreateMode;
        private bool UILoading = true;
        private bool OriginalCBCopyTagValues = true;
        private bool OriginalCBCopyNotes = true;

        public IsBasedOnForm(EA.Repository Repository, EAClass populatedEAClass, ArrayList qualifierList, bool CreateMode, LoadingIndicator loadingIndicator)
        {
            InitializeComponent();

            InitUI(Repository, populatedEAClass, qualifierList, CreateMode, loadingIndicator);
        }

        private void InitUI(EA.Repository Repository, EAClass populatedEAClass, ArrayList qualifierList, bool CreateMode, LoadingIndicator loadingIndicator)
        {
            this.Repository = Repository;
            XMLParser XMLP = new XMLParser(Repository);
            if (XMLP.GetXmlValueConfig("CopyParentElement").Equals(ConfigurationManager.UNCHECKED))
            {
                CBCopyParent.CheckState = CheckState.Unchecked;
            }
            else
            {
                CBCopyParent.CheckState = CheckState.Checked;
            }

            if(loadingIndicator != null) loadingIndicator.ProgressChanged(20);
            #region CreateMode
            if (CreateMode.Equals(true))
            {
                //Primitive Case
                #region Primitive
                if (((populatedEAClass.GetStereotype().Equals("<<" + CD.GetPrimitiveStereotype() + ">>".ToLower())) || (populatedEAClass.GetStereotype().Equals(CD.GetPrimitiveStereotype())))
                  && (!IsBasedOnClass.pureprimitive)  )
                {
                    this.Repository = Repository;
                    this.populatedEAClass = populatedEAClass;
                    this.ButEditCardinality.Enabled = false;
                    this.ButEditClassifier.Text = "Edit facets";
                    this.ButEditConstraint.Enabled = false;
                    this.CBCopyStereotype.Enabled = false;
                    this.ButEditStereotype.Enabled = false;
                    ButEditClassConstraint.Enabled = false;
                    populatedEAClass.SetMode(CD.GetPrimitiveStereotype());
                    this.CreateMode = CreateMode;
                    this.Text = "IsBasedOn functionality : Creating " + populatedEAClass.GetName();
                    this.qualifierList = qualifierList;
                    CBAstract.Enabled = false;
                    CBRoot.Enabled = false;

                    foreach (object aQualifier in this.qualifierList)
                    {
                        CBQualifier.Items.Add((string)aQualifier);
                    }

                    if (XMLP.GetXmlValueConfig("QualifyDatatypeEnumCompound").Equals(ConfigurationManager.UNCHECKED))
                    {
                        if (!CBQualifier.Items.Contains(CD.GetNoQualifier()))
                        {
                            CBQualifier.Items.Add(CD.GetNoQualifier());
                            CBQualifier.SelectedItem = CD.GetNoQualifier();
                        }
                    }
                    completeAttributeList = populatedEAClass.GetAttributeList();
                    populatedEAClass.AddPrimitiveAttribute();
                    completeAttributeList = populatedEAClass.GetAttributeList();
                    ListViewItem lvi;
                    foreach (EAClassAttribute anAttribute in completeAttributeList)
                    {
                        String[] aHeaders = new string[6];
                        if (anAttribute.GetHeritedState().Equals(true))
                        {
                            aHeaders[1] = "*";
                        }
                        else
                        {
                            aHeaders[1] = "";
                        }
                        aHeaders[2] = anAttribute.GetName();
                        aHeaders[3] = anAttribute.GetLowerBound();
                        aHeaders[4] = anAttribute.GetUpperBound();
                        aHeaders[5] = anAttribute.GetGUID();
                        lvi = new ListViewItem(aHeaders);
                        LVAttribute.Items.Add(lvi);
                        lvi.Checked = true;
                    }
                    CBCopyConstraints.Enabled = false;
                    CBCopyNotes.Enabled = false;
                    CBCopyTagValues.Enabled = false;
                    ButEditCardinality.Enabled = true;
                    ButEditClassifier.Enabled = true;
                }
                #endregion

                //Datatype Case
                #region Datatype
                else if ((populatedEAClass.GetStereotype().ToLower().Equals("<<"+CD.GetDatatypeStereotype().ToLower()+">>")) || (populatedEAClass.GetStereotype().ToLower().Equals(CD.GetDatatypeStereotype().ToLower())))
                {
                    this.Repository = Repository;
                    this.populatedEAClass = populatedEAClass;
                    this.CreateMode = CreateMode;
                    //populatedEAClass.SetMode(CD.GetDatatypeStereotype());
                    populatedEAClass.SetMode(CD.GetCreate());
                    this.Text = "IsBasedOn functionnality : Creating " + populatedEAClass.GetName();
                    this.ButEditClassifier.Text = "Edit facets";
                    this.ButEditConstraint.Enabled = false;
                    this.ButEditStereotype.Enabled = false;
                    this.CBCopyStereotype.Enabled = false;
                    ButEditClassConstraint.Enabled = false;
                    this.qualifierList = qualifierList;
                    CBRoot.Enabled = false;
                    CBAstract.Enabled = false;
                    foreach (object aQualifier in this.qualifierList)
                    {
                        CBQualifier.Items.Add((string)aQualifier);
                    }
                    completeAttributeList = populatedEAClass.GetAttributeList();
                    ListViewItem lvi;
                    foreach (EAClassAttribute anAttribute in completeAttributeList)
                    {
                        String[] aHeaders = new string[6];
                        if (anAttribute.GetHeritedState() == true) // ABA20230607
                        {
                            aHeaders[1] = "*";
                        }
                        else
                        {
                            aHeaders[1] = "";
                        }
                        aHeaders[2] = anAttribute.GetName();
                        aHeaders[3] = anAttribute.GetLowerBound();
                        aHeaders[4] = anAttribute.GetUpperBound();
                        aHeaders[5] = anAttribute.GetGUID();
                        lvi = new ListViewItem(aHeaders);
                        LVAttribute.Items.Add(lvi);
                        lvi.Checked = true;
                    }
                    ButEditCardinality.Enabled = true;
                    ButEditClassifier.Enabled = true;

                    if (XMLP.GetXmlValueConfig("QualifyDatatypeEnumCompound").Equals(ConfigurationManager.UNCHECKED))
                    {
                        if (!CBQualifier.Items.Contains(CD.GetNoQualifier()))
                        {
                            CBQualifier.Items.Add(CD.GetNoQualifier());
                            CBQualifier.SelectedItem = CD.GetNoQualifier();
                        }
                    }                    
                }
                #endregion

                #region Compound
                else if ((populatedEAClass.GetStereotype().ToLower().Equals("<<"+CD.GetCompoundStereotype().ToLower()+">>")) || (populatedEAClass.GetStereotype().ToLower().Equals(CD.GetCompoundStereotype().ToLower())))
                {
                    this.Repository = Repository;
                    this.populatedEAClass = populatedEAClass;
                    this.CreateMode = CreateMode;
                    populatedEAClass.SetMode(CD.GetCreate());
                    this.Text = "IsBasedOn functionnality : Creating " + populatedEAClass.GetName();
                    this.ButEditClassifier.Text = "Edit facets";
                    this.ButEditConstraint.Enabled = false;
                    this.ButEditStereotype.Enabled = false;
                    this.CBCopyStereotype.Enabled = false;

                    this.qualifierList = qualifierList;

                    ButEditClassConstraint.Enabled = false;
                    CBRoot.Enabled = false;
                    CBAstract.Enabled = false;
                    foreach (object aQualifier in this.qualifierList)
                    {
                        CBQualifier.Items.Add((string)aQualifier);
                    }


                    if (XMLP.GetXmlValueConfig("QualifyDatatypeEnumCompound").Equals(ConfigurationManager.UNCHECKED))
                    {
                        if (!CBQualifier.Items.Contains(CD.GetNoQualifier()))
                        {
                            CBQualifier.Items.Add(CD.GetNoQualifier());
                            CBQualifier.SelectedItem = CD.GetNoQualifier();
                        }
                    }

                    completeAttributeList = populatedEAClass.GetAttributeList();
                    ListViewItem lvi;
                    foreach (EAClassAttribute anAttribute in completeAttributeList)
                    {
                        String[] aHeaders = new string[6];
                        if (anAttribute.GetHeritedState() == true) // ABA 20230607
                        {
                            aHeaders[1] = "*";
                        }
                        else
                        {
                            aHeaders[1] = "";
                        }
                        aHeaders[2] = anAttribute.GetName();
                        aHeaders[3] = anAttribute.GetLowerBound();
                        aHeaders[4] = anAttribute.GetUpperBound();
                        aHeaders[5] = anAttribute.GetGUID();
                        lvi = new ListViewItem(aHeaders);
                        LVAttribute.Items.Add(lvi);
                        lvi.Checked = true;
                    }
                    ButEditCardinality.Enabled = true;
                    ButEditClassifier.Enabled = true;
                }
                #endregion

                #region class
                //Case if parent is a class/enum
                else
                {


                    this.Repository = Repository;
                    this.populatedEAClass = populatedEAClass;
                    this.CreateMode = CreateMode;
                    this.Text = "IsBasedOn functionnality : Creating " + populatedEAClass.GetName();
                    this.qualifierList = qualifierList;
                    this.ButEditStereotype.Enabled = false;
                    ButEditClassConstraint.Enabled = false;
                    CBRoot.Enabled = true;
                    CBAstract.Enabled = true;

                    CBRoot.Checked = populatedEAClass.GetIsRoot();



                    //If the parent is an enum
                    if ((Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Stereotype.ToLower().Equals(CD.GetEnumStereotype().ToLower())) || (Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Stereotype.ToLower().Equals("<<" + CD.GetEnumStereotype().ToLower() + ">>")))
                    {
                        ButEditCardinality.Enabled = false;
                        ButEditConstraint.Enabled = false;
                        CBAstract.Enabled = false;
                        CBRoot.Enabled = false;


                        if (XMLP.GetXmlValueConfig("QualifyDatatypeEnumCompound").Equals(ConfigurationManager.UNCHECKED))
                        {
                            if (!CBQualifier.Items.Contains(CD.GetNoQualifier()))
                            {
                                CBQualifier.Items.Add(CD.GetNoQualifier());
                                CBQualifier.SelectedItem = CD.GetNoQualifier();
                            }
                        }

                    }
                    else
                    {
                        CBAstract.Checked = populatedEAClass.GetAbstract();

                        if (!populatedEAClass.GetPossibleInheritance().Count.Equals(0))
                        {
                            ArrayList PossibleInheritance = populatedEAClass.GetPossibleInheritance();
                            /** am may 2011 at this level we are going to filter the possible inheritances
                            according to their  belonging to the IsBasedOn classes
                             **/
                            Utilitaires utils=new Utilitaires(Repository);
                           // EA.Element iboelt = populatedEAClass.GetIBOElement();
                            List<long> eligiblepackages=new List<long>(); // the package of the profile
                           // long packageid = utils.GetIBOParentPackage(iboelt.PackageID); // the first parent package to be IB
                            long packageid = (Repository.GetCurrentDiagram()).PackageID;
                            /******************** fix bub am october 2012 ****************/
                            utilitaires.Utilitaires ut = new utilitaires.Utilitaires(Repository);
                            
                            if(!ut.HasIBOPackage(packageid))
                            {
                                packageid=ut.GetIBOParentPackage(packageid); // look for profile envelop
                            }

                            /************************************************************/
                            eligiblepackages.Add(packageid);
                            eligiblepackages = utils.GetNotIBOPackages(packageid, eligiblepackages);
                            ArrayList FilteredPossibleInheritance = new ArrayList();
                            // foreach (EA.Element el in PossibleInheritance) am may 2011
                            foreach (EA.Element el in PossibleInheritance)
                            {
                                if (eligiblepackages.Contains(el.PackageID) && !FilteredPossibleInheritance.Contains(el.PackageID))
                                {
                                    FilteredPossibleInheritance.Add(el);
                                }
                            }

                            populatedEAClass.SetFilteredPossibleInheritance(FilteredPossibleInheritance);// am may 2011          




                            /***************************/
                            CBInheritList.Enabled = true;
                            CBInheritList.Items.Add("None");
                          //  foreach (EA.Element AnElement in PossibleInheritance)
                            foreach (EA.Element AnElem in FilteredPossibleInheritance) // am may 2011
                            {
                                CBInheritList.Items.Add(AnElem.Name);
                            }
                            CBInheritList.SelectedItem = "None";
                        }


                        #region CheckingIfNoQualifierAllowed
                        bool NoQualifierAllowed = true;
                        EA.Element parElem = Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID());
                        EA.Collection elems = Repository.GetPackageByID(Repository.GetCurrentDiagram().PackageID).Elements;
                        foreach (EA.Element AnElement in elems)
                        {
                            if (AnElement.Name.Equals(parElem.Name))
                            {
                                NoQualifierAllowed = false;
                                break;
                            }
                        }
                        if (NoQualifierAllowed.Equals(true))
                        {
                            CBQualifier.Items.Add(CD.GetNoQualifier());
                            CBQualifier.SelectedItem = CD.GetNoQualifier();
                        }
                        #endregion

                    }

                    foreach (object aQualifier in this.qualifierList)
                    {
                        CBQualifier.Items.Add((string)aQualifier);
                    }




                    completeAttributeList = populatedEAClass.GetAttributeList();
                    ListViewItem lvi;
                    foreach (EAClassAttribute anAttribute in completeAttributeList)
                    {
                        String[] aHeaders = new string[6];
                        if (anAttribute.GetHeritedState() == true) // ABA20230607
                        {
                            aHeaders[1] = "*";
                        }
                        else
                        {
                            aHeaders[1] = "";
                        }
                        aHeaders[2] = anAttribute.GetName();
                        aHeaders[3] = anAttribute.GetLowerBound();
                        aHeaders[4] = anAttribute.GetUpperBound();
                        aHeaders[5] = anAttribute.GetGUID();
                        lvi = new ListViewItem(aHeaders);
                        lvi.Checked = true;
                        LVAttribute.Items.Add(lvi);
                    }
                }
                #endregion

            }
            #endregion
            #region UpdateMode
            else
            {
                this.Repository = Repository;
                this.populatedEAClass = populatedEAClass;
                this.CreateMode = CreateMode;
                this.Text = "IsBasedOn functionality : Updating " + populatedEAClass.GetName();
                this.qualifierList = qualifierList;


                ///


                if (populatedEAClass.GetIBOElement().GetStereotypeList().Equals(Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).GetStereotypeList()))
                {
                    CBCopyStereotype.Checked = true;
                    ButEditStereotype.Enabled = false;
                }
                else
                {
                    CBCopyStereotype.Checked = false;
                    ButEditStereotype.Enabled = true;
                }

                //Constraints
                #region CheckConstraints
                bool SameConstraint = true;
                //if (!(populatedEAClass.GetIBOElement().Constraints.Count.Equals(Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Constraints.Count)))// am 
                if (!(populatedEAClass.GetIBOElement().ConstraintsEx.Count.Equals(Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).ConstraintsEx.Count)))
                { SameConstraint = false; }
                else
                {
                    //for (short i = 0; populatedEAClass.GetIBOElement().Constraints.Count > i; i++) // am oct 2017
                    for (short i = 0; populatedEAClass.GetIBOElement().ConstraintsEx.Count > i; i++)
                    {
                        //EA.Constraint AnIBOConstraint = (EA.Constraint)populatedEAClass.GetIBOElement().Constraints.GetAt(i); //am oct 2017
                        EA.Constraint AnIBOConstraint = (EA.Constraint)populatedEAClass.GetIBOElement().ConstraintsEx.GetAt(i);
                        bool NameFound = false;
                        //foreach (EA.Constraint AParentConstraint in Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Constraints)
                        //for (short j = 0; Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Constraints.Count > j; j++) // am oct 2017
                        for (short j = 0; Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).ConstraintsEx.Count > j; j++)
                        {
                           // EA.Constraint AParentConstraint = (EA.Constraint)Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Constraints.GetAt(j);// am oct 2017
                            EA.Constraint AParentConstraint = (EA.Constraint)Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).ConstraintsEx.GetAt(j);
                            if (AnIBOConstraint.Name.Equals(AParentConstraint.Name))
                            {
                                NameFound = true;
                                if ((!AParentConstraint.Notes.Equals(AnIBOConstraint.Notes)) || (!AParentConstraint.Type.Equals(AnIBOConstraint.Type)))
                                {
                                    SameConstraint = false;
                                }
                            }
                        }
                        if (NameFound.Equals(false))
                        {
                            SameConstraint = false;
                        }
                    }
                }
                if (SameConstraint.Equals(false))
                {
                    this.CBCopyConstraints.Checked = false;
                    this.ButEditClassConstraint.Enabled = true;
                }
                else
                {
                    this.CBCopyConstraints.Checked = true;
                    this.ButEditClassConstraint.Enabled = false;
                }
                #endregion

                populatedEAClass.SetStereotype(populatedEAClass.GetIBOElement().StereotypeEx);

                //notes
                if (!populatedEAClass.GetIBOElement().Notes.Equals(Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Notes))
                {
                    this.CBCopyNotes.Checked = false;
                    this.OriginalCBCopyNotes = false;
                }
                //TaggedValues
                #region CheckTaggedValue
                bool SameTagValues = true;
                EA.Element iboElem = populatedEAClass.GetIBOElement();
                EA.Collection tgValues = iboElem.TaggedValues;
                EA.Element parentElem = Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID());
                EA.Collection parentTgValues = parentElem.TaggedValues;
                if (tgValues.Count > Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).TaggedValues.Count)
                {
                    for (short i = 0; tgValues.Count > i; i++)
                    {
                        EA.TaggedValue AnIBOTag = (EA.TaggedValue)tgValues.GetAt(i);
                        bool NameFound = false;
                        EA.TaggedValue OldTagValue = null;
                        for (short j = 0; parentTgValues.Count > j; j++)
                        {
                            EA.TaggedValue AParentTag = (EA.TaggedValue)parentTgValues.GetAt(j);
                            if (AnIBOTag.Name.Equals(AParentTag.Name) && NameFound.Equals(false))
                            {
                                NameFound = true;
                                OldTagValue = AParentTag;
                                break;
                            }
                        }
                        if (AnIBOTag.Name.Equals(CD.GetIBOTagValue()))
                        {
                            NameFound = true;
                            OldTagValue = AnIBOTag;
                        }
                        if (NameFound.Equals(false))
                        {
                            SameTagValues = false;
                        }
                        else
                        {
                            if ((!OldTagValue.Notes.Equals(AnIBOTag.Notes)) || (!OldTagValue.Value.Equals(AnIBOTag.Value)))
                            {
                                SameTagValues = false;
                            }
                        }
                    }
                }
                else
                {
                    for (short j = 0; Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).TaggedValues.Count > j; j++)
                    {
                        EA.TaggedValue AParentTag = (EA.TaggedValue)Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).TaggedValues.GetAt(j);
                        bool NameFound = false;
                        EA.TaggedValue OldTagValue = null;
                        iboElem = populatedEAClass.GetIBOElement();
                        tgValues = iboElem.TaggedValues;
                        for (short i = 0; tgValues.Count > i; i++)
                        {
                            EA.TaggedValue AnIBOTag = (EA.TaggedValue)tgValues.GetAt(i);
                            if (AnIBOTag.Name.Equals(AParentTag.Name) && NameFound.Equals(false))
                            {
                                NameFound = true;
                                OldTagValue = AnIBOTag;
                                break;
                            }
                        }
                        if (AParentTag.Name.Equals(CD.GetIBOTagValue()))
                        {
                            NameFound = true;
                            OldTagValue = AParentTag;
                        }
                        if (NameFound.Equals(false))
                        {
                            SameTagValues = false;
                        }
                        else
                        {
                            if ((!OldTagValue.Notes.Equals(AParentTag.Notes)) || (!OldTagValue.Value.Equals(AParentTag.Value)))
                            {
                                SameTagValues = false;
                            }
                        }
                    }
                }
                if (SameTagValues.Equals(false))
                {
                    this.CBCopyTagValues.Checked = false;
                    this.OriginalCBCopyTagValues = false;
                }
                #endregion


                #region CheckIfParentIsOnDiagram
                bool ParentElementFound = false;
                for (short i = 0; Repository.GetCurrentDiagram().DiagramObjects.Count > i; i++)
                {
                    EA.DiagramObject AnObject = (EA.DiagramObject)Repository.GetCurrentDiagram().DiagramObjects.GetAt(i);
                    if (Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).ElementID.Equals(AnObject.ElementID))
                    {
                        ParentElementFound = true;
                        break;
                    }
                }
                if (ParentElementFound.Equals(true))
                {
                    CBCopyParent.Checked = true;
                }
                else
                {
                    CBCopyParent.Checked = false;
                }
                #endregion

                foreach (object aQualifier in this.qualifierList)
                {
                    CBQualifier.Items.Add((string)aQualifier);
                }

                if (populatedEAClass.GetQualifier().Equals(""))
                {
                    if (!(CBQualifier.Items.Contains(CD.GetNoQualifier())))
                    {
                        CBQualifier.Items.Add(CD.GetNoQualifier());
                    }
                    CBQualifier.SelectedItem = CD.GetNoQualifier();
                }
                else
                {
                    CBQualifier.Items.Add(populatedEAClass.GetQualifier());
                    CBQualifier.SelectedItem = populatedEAClass.GetQualifier();
                }
                LVAttribute.Enabled = true;

                completeAttributeList = populatedEAClass.GetAttributeList();
                ListViewItem lvi;
                foreach (EAClassAttribute anAttribute in completeAttributeList)
                {
                    String[] aHeaders = new string[6];
                     if (anAttribute.GetHeritedState().Equals(true))
                    {
                        aHeaders[1] = "*";
                    }
                    else
                    {
                        aHeaders[1] = "";
                    }
                    aHeaders[2] = anAttribute.GetName();
                    aHeaders[3] = anAttribute.GetLowerBound();
                    aHeaders[4] = anAttribute.GetUpperBound();
                    aHeaders[5] = anAttribute.GetGUID();
                    lvi = new ListViewItem(aHeaders);
                    LVAttribute.Items.Add(lvi);
                }

                foreach (ListViewItem ALVI in LVAttribute.Items)
                {
                    string prov=populatedEAClass.GetAttribute(ALVI.SubItems[5].Text).GetName();
                    if (populatedEAClass.GetAttribute(ALVI.SubItems[5].Text).GetUISelectedState().Equals(true))
                    {
                        ALVI.Checked = true;
                    }
                    else { ALVI.Checked = false; }
                }
                ///

                //If parent is a DataType or primitive
                if ((populatedEAClass.GetStereotype().ToLower().Equals(CD.GetDatatypeStereotype().ToLower()) || (populatedEAClass.GetStereotype().ToLower().Equals("<<" + CD.GetDatatypeStereotype().ToLower() + ">>"))) || (populatedEAClass.GetStereotype().ToLower().Equals(CD.GetPrimitiveStereotype().ToLower()) || (populatedEAClass.GetStereotype().ToLower().Equals("<<" + CD.GetPrimitiveStereotype().ToLower() + ">>"))))
                {


                    if (XMLP.GetXmlValueConfig("QualifyDatatypeEnumCompound").Equals(ConfigurationManager.UNCHECKED))
                    {
                        if (!CBQualifier.Items.Contains(CD.GetNoQualifier()))
                        {
                            CBQualifier.Items.Add(CD.GetNoQualifier());
                            if(CBQualifier.SelectedItem==null){
                                CBQualifier.SelectedItem = CD.GetNoQualifier();
                            }
                        }
                    }

                    this.ButEditClassifier.Text = "Edit facets";
                    this.ButEditConstraint.Enabled = false;
                    this.ButEditStereotype.Enabled = false;
                    this.CBCopyStereotype.Enabled = false;
                    CBRoot.Enabled = false;
                    CBAstract.Enabled = false;
                }
                if (loadingIndicator != null) loadingIndicator.ProgressChanged(40);

                //If parent is a Compound
                if ((populatedEAClass.GetStereotype().ToLower().Equals(CD.GetCompoundStereotype().ToLower()) || (populatedEAClass.GetStereotype().ToLower().Equals("<<" + CD.GetCompoundStereotype().ToLower() + ">>"))))
                {
                    this.ButEditClassifier.Text = "Edit facets";
                    this.ButEditConstraint.Enabled = false;
                    this.ButEditStereotype.Enabled = false;
                    this.CBCopyStereotype.Enabled = false;
                    CBRoot.Enabled = false;
                    CBAstract.Enabled = false;


                    if (XMLP.GetXmlValueConfig("QualifyDatatypeEnumCompound").Equals(ConfigurationManager.UNCHECKED))
                    {
                        if (!CBQualifier.Items.Contains(CD.GetNoQualifier()))
                        {
                            CBQualifier.Items.Add(CD.GetNoQualifier());
                            if (CBQualifier.SelectedItem == null)
                            {
                                CBQualifier.SelectedItem = CD.GetNoQualifier();
                            }
                        }
                    }

                }//If the parent is an enum
                else if (Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Stereotype.ToLower().Equals(CD.GetEnumStereotype().ToLower()))
                {
                    if (XMLP.GetXmlValueConfig("QualifyDatatypeEnumCompound").Equals(ConfigurationManager.UNCHECKED))
                    {
                        if (!CBQualifier.Items.Contains(CD.GetNoQualifier()))
                        {
                            CBQualifier.Items.Add(CD.GetNoQualifier());
                            if (CBQualifier.SelectedItem == null)
                            {
                                CBQualifier.SelectedItem = CD.GetNoQualifier();
                            }
                        }
                    }
                    ButEditCardinality.Enabled = false;
                    ButEditConstraint.Enabled = false;
                    CBRoot.Enabled = false;
                    CBAstract.Enabled = false;
                }//If the parent is a class
                else
                {
                    CBRoot.Enabled = true;
                    CBAstract.Enabled = true;
                    #region CheckingIfNoQualifierAllowed
                    bool NoQualifierAllowed = true;
                    EA.Collection iboeElemPackElems = Repository.GetPackageByID(populatedEAClass.GetIBOElement().PackageID).Elements;
                    EA.Element oarentElem = Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID());
                    foreach (EA.Element AnElement in iboeElemPackElems)
                    {
                        if (AnElement.Name.Equals(oarentElem.Name))
                        {
                            NoQualifierAllowed = false;
                            break;
                        }
                    }
                    if (NoQualifierAllowed.Equals(true))
                    {
                        CBQualifier.Items.Add(CD.GetNoQualifier());
                    }
                    #endregion

                    CBRoot.Checked = populatedEAClass.GetIsRoot();
                    CBAstract.Checked = populatedEAClass.GetAbstract();
                    if (!populatedEAClass.GetPossibleInheritance().Count.Equals(0))
                    {
                        Utilitaires utils = new Utilitaires(Repository);
                        ArrayList FilteredPossibleInheritance = new ArrayList();
                        CBInheritList.Enabled = true;
                        CBInheritList.Items.Add("None");
                        if (XMLP.GetXmlValueConfig("EnabledIntermediaryInheritance") == null)
                        {
                            MessageBox.Show(" tag EnabledIntermediaryInheritance seems missing from configuration file");
                            this.Close();
                        }
                         EA.Element iboelt = populatedEAClass.GetIBOElement();
                         long packageid = utils.GetIBOParentPackage(iboelt.PackageID); // the first parent package to be IB
                        if (XMLP.GetXmlValueConfig("EnabledIntermediaryInheritance").Equals(ConfigurationManager.UNCHECKED))
                        {
                            ArrayList PossibleInheritance = populatedEAClass.GetPossibleInheritance();
                            

                            /** am may 2011 at this level we are going to filter the possible inheritances
                               according to their  belonging to the IsBasedOn classes
                                **/

                           // EA.Element iboelt = populatedEAClass.GetIBOElement();
                            List<long> eligiblepackages = new List<long>(); // the package of the profile
                           // long packageid = utils.GetIBOParentPackage(iboelt.PackageID); // the first parent package to be IB
                            if (packageid == 0)
                            {
                                string texte = " Warning the package containing the current  diagram :" + Repository.GetCurrentDiagram().Name + "should be based on another package (or one of its parent package)";
                                MessageBox.Show(texte);
                            }
                            else
                            {
                                eligiblepackages.Add(packageid);
                                eligiblepackages = utils.GetNotIBOPackages(packageid, eligiblepackages);
                            }
                            // foreach (EA.Element el in PossibleInheritance) am may 2011
                         //   ArrayList FilteredPossibleInheritance = new ArrayList();
                            foreach (EA.Element elt in PossibleInheritance)
                            {
                                if (eligiblepackages.Contains(elt.PackageID))
                                {
                                    if (!FilteredPossibleInheritance.Contains(elt)) FilteredPossibleInheritance.Add(elt);
                                }
                            }

                            populatedEAClass.SetFilteredPossibleInheritance(FilteredPossibleInheritance);// am may 2011

                        }
                        else
                        {
                            EA.Package apackage = Repository.GetPackageByID((int)packageid); //Repository.GetTreeSelectedPackage();
                           // long packageid = utils.GetIBOParentPackage(iboelt.PackageID); // the first parent package to be IB

                            utils.buildDicElems( apackage, false);

                            FilteredPossibleInheritance = utils.GetPossibleFilteredExInheritance(populatedEAClass);
                           
                            populatedEAClass.SetFilteredPossibleInheritance(FilteredPossibleInheritance);// am may 2011
                        }
 
                            foreach (EA.Element el in FilteredPossibleInheritance)
                            {
                                CBInheritList.Items.Add(el.Name);
                            }
                            if (!(populatedEAClass.GetSelectedInheritance() == null))
                            {
                                CBInheritList.SelectedItem = populatedEAClass.GetSelectedInheritance().Name;
                            }
                            else
                            {
                                CBInheritList.SelectedItem = "None";
                            }
                       
                    }
                }


            }
            #endregion
            if (loadingIndicator != null) loadingIndicator.ProgressChanged(90);
        }

        public void SetUILoading(bool newState)
        {
            this.UILoading = newState;
        }

        public void SetUIItemCardinality(string attributeGUID, string lowerBound, string upperBound)
        {
            foreach (ListViewItem lvi in LVAttribute.Items)
            {
                if (lvi.SubItems[5].Text.Equals(attributeGUID))
                {
                    lvi.SubItems[3].Text = lowerBound.ToString();
                    lvi.SubItems[4].Text = upperBound.ToString();
                }
            }
        }

        /// <summary>
        /// Return true if a qualifier is needed. false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfFullNameAlreadyExist()
        {
            if (this.CreateMode.Equals(true))
            {
                foreach (EA.Element AnElement in Repository.GetPackageByID(Repository.GetCurrentDiagram().PackageID).Elements)
                {
                    if (CBQualifier.Text.Equals(CD.GetNoQualifier()))
                    {
                        if (Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Name.Equals(AnElement.Name))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if ((CBQualifier.Text + "_" + Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Name).Equals(AnElement.Name))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                //if update mode and element won't change his name

                String fullName = "";

                if (CBQualifier.Text.Equals(CD.GetNoQualifier()))
                {
                    fullName = Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Name;
                }
                else
                {
                    fullName = CBQualifier.Text + "_" + Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Name;
                }

                if (fullName.Equals(populatedEAClass.GetIBOElement().Name))
                {
                    return false;
                }
                else{//doing the same thing than in create mode
                  foreach (EA.Element AnElement in Repository.GetPackageByID(Repository.GetCurrentDiagram().PackageID).Elements)
                {
                    if (CBQualifier.Text.Equals(CD.GetNoQualifier()))
                    {
                        if (Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Name.Equals(AnElement.Name))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if ((CBQualifier.Text + "_" + Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Name).Equals(AnElement.Name))
                        {
                            return true;
                        }
                    }
                }
                return false;
                }
            }
        }

        private ArrayList CheckPossibleAffectedIBO()
        {
            ArrayList AffectedIBO = new ArrayList();
            if (CreateMode.Equals(CD.GetCreate()))
            {
                return AffectedIBO;
            }

            if (populatedEAClass.CheckIfSameElement().Equals(false))
            {
                for (short i = 0; populatedEAClass.GetIBOElement().Connectors.Count > i; i++)
                {
                    EA.Connector APossibleIBOConnecor = (EA.Connector)populatedEAClass.GetIBOElement().Connectors.GetAt(i);

                    if (APossibleIBOConnecor.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower()))
                    {

                        if (!APossibleIBOConnecor.ClientID.Equals(populatedEAClass.GetIBOElement().ElementID))
                        {
                            AffectedIBO.Add(Repository.GetElementByID(APossibleIBOConnecor.SupplierID));
                        }

                    }
                }
            }

            return AffectedIBO;
        }

        private void 
            ButExecuteIsBasedOn_Click(object sender, EventArgs e)
        {
            populatedEAClass.SetIsRoot(CBRoot.Checked);

            if (CBInheritList.Enabled.Equals(true))
            {
                if (!(CBInheritList.SelectedItem == null))
                {
                    if (!CBInheritList.SelectedItem.Equals("None"))
                    {
                       // foreach (EA.Element AnElement in populatedEAClass.GetPossibleInheritance())
                        foreach (EA.Element AnElement in populatedEAClass.GetFilteredPossibleInheritance()) //am may 2011
                        {
                            if (AnElement.Name.Equals(CBInheritList.SelectedItem))
                            {
                                populatedEAClass.SetSelectedInheritance(AnElement);
                            }
                        }
                    }
                    else
                    {
                        populatedEAClass.SetSelectedInheritance(null);
                    }
                }
                else
                {
                    populatedEAClass.SetSelectedInheritance(null);
                }
            }
            else
            {
                populatedEAClass.SetSelectedInheritance(null);
            }

            if (CBQualifier.Text == "")
            {
                MessageBox.Show("A qualifier can't be null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                if (!CBQualifier.Text.Equals(CD.GetNoQualifier()))
                {
                    if (CBQualifier.Text.Contains(" ") || CBQualifier.Text.Contains("_"))
                    {
                        MessageBox.Show("This is not a valid qualifier(these elements are forbidden : Blank space, _).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                }
            }

            if (CheckIfFullNameAlreadyExist().Equals(true))
            {
                MessageBox.Show("This element already exist in the package, add or change the qualifier.", "Warning ! Trying to create an already existing class.", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }


            if (CreateMode.Equals(CD.GetUpdate()))
            {
                ArrayList AffectedIBO = CheckPossibleAffectedIBO();
                if (AffectedIBO.Count > 0)
                {

                    DialogResult DResult = MessageBox.Show("There is element(s) based on a another element that will be modified.\nDo you wish to see the list ?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (DResult.Equals(DialogResult.Cancel))
                    {
                        return;
                    }
                    else if (DResult.Equals(DialogResult.Yes))
                    {
                        ArrayList MessageBoxList = new ArrayList();
                        foreach (EA.Element AnElement in AffectedIBO)
                        {
                            MessageBoxList.Add("The element " + AnElement.Name + " need to be updated");
                        }
                        // ABA20230401 DetailMessageBox DMB = new DetailMessageBox("List of elements that will need to be updated.", "You should delete these elements or modify them to keep the integrity of the IsBasedOn chain.", MessageBoxList);
                        DResult = MessageBox.Show("Do you wish to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (DResult.Equals(DialogResult.No))
                        {
                            return;
                        }
                    }
                }
            }


            if (!CBQualifier.Text.Equals(CD.GetNoQualifier()))
            {
                populatedEAClass.SetQualifier(GetFormatedQualifierFromComboBox() + "_");
            }
            else
            {
                populatedEAClass.SetQualifier("");
            }
            populatedEAClass.ExecuteIsBasedOn(this);
            Main.isBasedOnExecuted = true;//to allow duplicate programs to know that the proflile has evolved
            this.Dispose();

        }

        /// <summary>
        /// Return the value of the checkBox CBAbstract to be used by EA.
        /// EA use the format string for EA.element.Abstract
        /// </summary>
        /// <returns>
        /// "0" for uncchecked = not abstract
        /// "1" for checked = abstract
        /// </returns>
        public string GetCBAbstract()
        {
            if (CBAstract.Checked.Equals(true))
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        private string GetFormatedQualifierFromCreate()
        {
            if (!(TBQualifier.Text == null))
            {
                return (TBQualifier.Text.Substring(0, 1).ToUpper() + TBQualifier.Text.Remove(0, 1)).Replace(" ", "").Replace("_", "");
            }
            else
            {
                return "";
            }
        }
        
        private string GetFormatedQualifierFromComboBox()
        {
            if (!(CBQualifier.Text == null))
            {
                return (CBQualifier.Text.Substring(0, 1).ToUpper() + CBQualifier.Text.Remove(0, 1)).Replace(" ", "").Replace("_", "");
            }
            else
            {
                return "";
            }
        }

        private void ButCreateQualifier_Click(object sender, EventArgs e)
        {
            if (!TBQualifier.Text.Trim().Equals(""))
            {
                if (!CBQualifier.Items.Contains(TBQualifier.Text))
                {
                    CBQualifier.Items.Add(GetFormatedQualifierFromCreate());
                }
                if (CBQualifierAllowedToAny.Checked.Equals(true))
                {
                    XMLParser xmlParser = new XMLParser(Repository);
                    xmlParser.AddXmlQualifier(GetFormatedQualifierFromCreate(), CD.GetAny());
                }
                else
                {
                    if ((populatedEAClass.GetStereotype().Equals(CD.GetDatatypeStereotype()) || (populatedEAClass.GetStereotype().Equals("<<" + CD.GetDatatypeStereotype() + ">>"))) || (populatedEAClass.GetStereotype().Equals(CD.GetPrimitiveStereotype()) || (populatedEAClass.GetStereotype().Equals("<<" + CD.GetPrimitiveStereotype() + ">>")) || (populatedEAClass.GetStereotype().Equals("<<" + CD.GetEnumStereotype() + ">>")) || (populatedEAClass.GetStereotype().Equals(CD.GetEnumStereotype()))))
                    {
                        XMLParser xmlParser = new XMLParser(Repository);
                        xmlParser.AddXmlQualifier(GetFormatedQualifierFromCreate(), CD.GetDatatypeStereotype().ToLower());
                    }
                    else
                    {
                        XMLParser xmlParser = new XMLParser(Repository);
                        xmlParser.AddXmlQualifier(GetFormatedQualifierFromCreate(), CD.GetClass().ToLower());
                    }
                }
                CBQualifierAllowedToAny.Checked = true;
                TBQualifier.Text = "";
            }

        }

        private void ButDeleteQualifier_Click(object sender, EventArgs e)
        {
            if (!CBQualifier.SelectedItem.ToString().Equals(CD.GetNoQualifier()))
            {
                DialogResult result = MessageBox.Show("Do you really want to delete this qualifier ? It won't affect already qualified objects", "Add-in",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result.Equals(DialogResult.Yes))
                {
                    XMLParser XMLP = new XMLParser(Repository);
                    string stringToDelete = CBQualifier.SelectedItem.ToString();
                    XMLP.DeleteXmlQualifier(stringToDelete);
                    CBQualifier.Items.Remove(stringToDelete);
                    CBQualifier.SelectedItem = CD.GetNoQualifier();
                }
            }
        }

        private void ButUnselectAll_Click(object sender, EventArgs e)
        {
            XMLParser XMLP = new XMLParser(Repository);
            if (XMLP.GetXmlValueConfig("Log") == Configuration.ConfigurationManager.CHECKED)
            {
                XMLP.AddXmlLog("IsBasedOnSelectAttribute", "All attributes have been selected");
            }
            foreach (ListViewItem anItem in LVAttribute.Items)
            {
                anItem.Checked = false;
            }

        }

        private void ButSelectAll_Click(object sender, EventArgs e)
        {
            XMLParser XMLP = new XMLParser(Repository);
            if (XMLP.GetXmlValueConfig("Log") == Configuration.ConfigurationManager.CHECKED)
            {
                XMLP.AddXmlLog("IsBasedOnSelectAttribute", "All attributes have been selected");
            }
            foreach (ListViewItem anItem in LVAttribute.Items)
            {
                anItem.Checked = true;
            }
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            XMLParser XMLP = new XMLParser(Repository);
            if (XMLP.GetXmlValueConfig("Log") == Configuration.ConfigurationManager.CHECKED)
            {
                XMLP.AddXmlLog("IsBasedOnEnd", "IsBasedOn Functionnality have been canceled");
            }
            if (CreateMode.Equals(true))
            {
                populatedEAClass.CancelIsBasedOn();
            }
            this.Dispose();
        }

        private void ButEditCardinality_Click(object sender, EventArgs e)
        {
            if (populatedEAClass.GetMode().Equals(CD.GetPrimitiveStereotype()))
            {
                EAClassAttribute SelectedAttribute = null;
                ArrayList AttributeList = populatedEAClass.GetAttributeList();
                if (AttributeList.Count > 0)
                {
                    SelectedAttribute = (EAClassAttribute)AttributeList[0];
                }
                if (!(SelectedAttribute == null))
                {

                    XMLParser XMLP = new XMLParser(Repository);
                    if (XMLP.GetXmlValueConfig("Log") == Configuration.ConfigurationManager.CHECKED)
                    {
                        XMLP.AddXmlLog("IsBasedOnAttributeCardinality", "Editing cardinality of the attribute : " + populatedEAClass.GetAttribute(LVAttribute.SelectedItems[0].SubItems[5].Text).GetName());
                    }
                    AttributeCardinalityForm ACF = new AttributeCardinalityForm(this, populatedEAClass, SelectedAttribute, Repository);
                    ACF.ShowDialog();
                }
            }
            else
            {
                if ((LVAttribute.SelectedItems.Count.Equals(1)))
                {
                    if (LVAttribute.SelectedItems[0].Checked.Equals(true))
                    {
                        XMLParser XMLP = new XMLParser(Repository);
                        if (XMLP.GetXmlValueConfig("Log") == Configuration.ConfigurationManager.CHECKED)
                        {
                            XMLP.AddXmlLog("IsBasedOnAttributeCardinality", "Editing cardinality of the attribute : " + populatedEAClass.GetAttribute(LVAttribute.SelectedItems[0].SubItems[5].Text).GetName());
                        }
                        AttributeCardinalityForm ACF = new AttributeCardinalityForm(this, populatedEAClass, populatedEAClass.GetAttribute(LVAttribute.SelectedItems[0].SubItems[5].Text), Repository);
                        ACF.ShowDialog();
                    }
                }
            }

        }


        private void EditClassifierNotPrimitive()
        {
            if (!LVAttribute.SelectedItems.Count.Equals(1))
            {
                
                    return;
                
            }
            if (!LVAttribute.SelectedItems[0].Checked.Equals(true))
            {
                return;
            }

                    if (((populatedEAClass.GetStereotype().Equals(CD.GetPrimitiveStereotype())) || (populatedEAClass.GetStereotype().Equals("<<" + CD.GetPrimitiveStereotype() + ">>"))) || ((populatedEAClass.GetStereotype().Equals("<<" + CD.GetDatatypeStereotype() + ">>")) || (populatedEAClass.GetStereotype().Equals(CD.GetDatatypeStereotype()))))
                    {
                        string TargetType;
                        if (Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Stereotype.Equals(CD.GetDatatypeStereotype()) || Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Stereotype.Equals("<<" + CD.GetDatatypeStereotype() + ">>"))
                        {
                            TargetType = CD.GetDatatypeStereotype();
                        }
                        else if (Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Stereotype.Equals(CD.GetCompoundStereotype()) || Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Stereotype.Equals("<<" + CD.GetCompoundStereotype() + ">>"))
                        {
                            TargetType = CD.GetCompoundStereotype();
                        }
                        else
                        {
                            TargetType = CD.GetPrimitiveStereotype();
                        }

                        if (!(Repository.GetAttributeByGuid(LVAttribute.SelectedItems[0].SubItems[5].Text).ClassifierID == 0))
                        {
                            try { string Name = Repository.GetElementByID(populatedEAClass.GetAttribute(LVAttribute.SelectedItems[0].SubItems[5].Text).GetClassifier()).Name; }
                            catch
                            {
                                MessageBox.Show("An exception have been raised.\nCan't find the cassifier's class of this element.\nMake sur you didn't destroyed it beforehand, if it's the case recreate it and re link it by editing your class.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                                this.Dispose();
                            }
                            //  A REVOIR pour liste primitives
                            string AttributeClassifierName = Repository.GetElementByID(populatedEAClass.GetAttribute(LVAttribute.SelectedItems[0].SubItems[5].Text).GetClassifier()).Name;
                            //string prov1 = "|" + AttributeClassifierName + "|";
                            //string prov2 = "|" + CD.GetStringType() + "|";
                            if (AttributeClassifierName.Equals(CD.GetStringType()) 
                                || AttributeClassifierName.Equals(CD.GetIntegerType()) 
                                || AttributeClassifierName.Equals(CD.GetEnumType()) 
                                || AttributeClassifierName.Equals(CD.GetFloatStereotype()) 
                                || AttributeClassifierName.Equals(CD.GetBooleanType()) 
                                || AttributeClassifierName.Equals(CD.GetDecimalType())
                                || AttributeClassifierName.Equals(CD.GetDurationType())
                                || AttributeClassifierName.Equals(CD.GetDateTimeType())
                                || AttributeClassifierName.Equals(CD.GetDateType())
                                || AttributeClassifierName.Equals(CD.GetTimeType())
                                || AttributeClassifierName.Equals(CD.GetAnyUriType())
                                || AttributeClassifierName.Equals(CD.GetBase64BinaryType()) 
                                || AttributeClassifierName.Equals(CD.GetByteType()) 
                                || AttributeClassifierName.Equals(CD.GetHexBinaryType()) 
                                || AttributeClassifierName.Equals(CD.GetIDType()) 
                                || AttributeClassifierName.Equals(CD.GetIDREFType())
                                || AttributeClassifierName.Equals(CD.GetIntType())
                                || AttributeClassifierName.Equals(CD.GetLongType())
                                || AttributeClassifierName.Equals(CD.GetShortType())
                                || AttributeClassifierName.Equals(CD.GetUnsignedByteType())
                                || AttributeClassifierName.Equals(CD.GetUnsignedIntType())
                                || AttributeClassifierName.Equals(CD.GetUnsignedLongType())
                                || AttributeClassifierName.Equals(CD.GetUnsignedShortType())

                                )
         
                                 {
                                bool ExceptionBool = false;
                                try { LVAttribute.SelectedItems[0].SubItems[5].Text.Equals("Madeby Maligue clausse"); }
                                catch { ExceptionBool = true; }

                                if (ExceptionBool.Equals(false))
                                {
                                    AttributeClassifierConstraintForm ACCF = null;
                                    if (populatedEAClass.GetName().ToLower().Contains("absolutedatetime"))
                                    {
                                        ACCF = new AttributeClassifierConstraintForm(CreateMode, Repository, populatedEAClass.GetAttribute(LVAttribute.SelectedItems[0].SubItems[5].Text), "AbsoluteDateTime", TargetType);
                                        ACCF.ShowDialog();
                                    }
                                    else if (populatedEAClass.GetName().ToLower().Contains("absolutedate"))
                                    {
                                        ACCF = new AttributeClassifierConstraintForm(CreateMode, Repository, populatedEAClass.GetAttribute(LVAttribute.SelectedItems[0].SubItems[5].Text), "AbsoluteDate", TargetType);
                                        ACCF.ShowDialog();
                                    }
                                    else
                                    {
                                        ACCF = new AttributeClassifierConstraintForm(CreateMode, Repository, populatedEAClass.GetAttribute(LVAttribute.SelectedItems[0].SubItems[5].Text), AttributeClassifierName, TargetType);
                                        ACCF.ShowDialog();
                                    }

                                    ACCF.Dispose(); // ABA20230401

                                }
                            }
                            else
                            {
                                AttributeClassifierForm ACF1 = new AttributeClassifierForm(CreateMode, Repository, populatedEAClass, LVAttribute.SelectedItems[0].SubItems[5].Text);
                                ACF1.ShowDialog();
                                ACF1.Dispose();
                            }

                        }
                        else
                        {
                            AttributeClassifierForm ACF2 = new AttributeClassifierForm(CreateMode, Repository, populatedEAClass, LVAttribute.SelectedItems[0].SubItems[5].Text);
                            ACF2.ShowDialog();
                            ACF2.Dispose();
                        }
                    }
                    else//test
                    {
                        AttributeClassifierForm ACF3 = new AttributeClassifierForm(CreateMode, Repository, populatedEAClass, LVAttribute.SelectedItems[0].SubItems[5].Text);
                        ACF3.ShowDialog();
                        ACF3.Dispose();
                    }
                
            
        }


        private void ButEditClassifier_Click(object sender, EventArgs e)
        {
            if (!populatedEAClass.GetMode().Equals(CD.GetPrimitiveStereotype()))
            {
                EditClassifierNotPrimitive();
            }//Cas primitif
            else
            {
                ArrayList AttributeList = populatedEAClass.GetAttributeList();
                if (AttributeList.Count > 0)
                {
                    string AttributeClassifierType = ((EAClassAttribute)AttributeList[0]).GetType();
                    AttributeClassifierConstraintForm ACCF = new AttributeClassifierConstraintForm(CreateMode, Repository, ((EAClassAttribute)AttributeList[0]), AttributeClassifierType, CD.GetPrimitiveStereotype());
                    ACCF.ShowDialog();
                    ACCF.Dispose();
                }
            }
        }

        private void LVAttribute_ItemCheck(object sender, ItemCheckEventArgs e)
        {

            if (UILoading.Equals(false))
            {
                EAClassAttribute SelectedAttribute = populatedEAClass.GetAttribute(LVAttribute.Items[e.Index].SubItems[5].Text);

                bool WarningNeeded = false;
                bool AttributeIsDisabled = false;


                if (!(SelectedAttribute == null))
                {
                    if (LVAttribute.Items[e.Index].BackColor.Equals(Color.Red))
                    {
                        if (e.CurrentValue.Equals(CheckState.Unchecked) && e.NewValue.Equals(CheckState.Checked))
                        {
                            AttributeIsDisabled = true;
                        }
                        else if (e.CurrentValue.Equals(CheckState.Checked) && e.NewValue.Equals(CheckState.Unchecked))
                        {
                            WarningNeeded = false;
                        }
                    }
                    else
                    {
                        if (!e.CurrentValue.Equals(CheckState.Unchecked))
                        {
                            if (e.CurrentValue.Equals(CheckState.Checked))
                            {
                                if (((SelectedAttribute.GetLowerBound().Equals("1")) && (SelectedAttribute.GetUpperBound().Equals("1"))) && (!LVAttribute.Items[e.Index].BackColor.Equals(Color.Red)))
                                {
                                    XMLParser XMLP = new XMLParser(Repository);
                                    if (XMLP.GetXmlValueConfig("Warning").Equals(ConfigurationManager.CHECKED))
                                    {
                                        WarningNeeded = true;
                                    }
                                }
                                else
                                {
                                    WarningNeeded = false;
                                }
                            }
                        }
                    }
                }

                if (AttributeIsDisabled.Equals(true))
                {

                    MessageBox.Show("You can't change the state of this attribute as he is inherited from another abstract class.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.NewValue = CheckState.Unchecked;

                }
                else if (WarningNeeded.Equals(false))
                {
                    if (e.NewValue.Equals(CheckState.Checked))
                    {
                        populatedEAClass.SetAttributeState(LVAttribute.Items[e.Index].SubItems[5].Text, true);
                    }
                    else
                    {
                        populatedEAClass.SetAttributeState(LVAttribute.Items[e.Index].SubItems[5].Text, false);
                    }

                }
                else
                {
                    DialogResult Result = MessageBox.Show("It seem unwise to uncheck an attribute having a 1..1 cardinality. \n Do you wish to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    if (Result.Equals(DialogResult.Yes))
                    {
                        if (e.NewValue.Equals(CheckState.Checked))
                        {
                            populatedEAClass.SetAttributeState(LVAttribute.Items[e.Index].SubItems[5].Text, true);
                        }
                        else
                        {
                            populatedEAClass.SetAttributeState(LVAttribute.Items[e.Index].SubItems[5].Text, false);
                        }
                    }
                    else
                    {
                        e.NewValue = e.CurrentValue;
                    }
                }
            }
        }

        /*
        private void LVAttribute_ItemChecked(object sender, ItemCheckedEventArgs e)
        {

            if (UILoading.Equals(false))
            {
                EAClassAttribute SelectedAttribute = populatedEAClass.GetAttribute(e.Item.SubItems[5].Text);

                bool WarningNeeded = false;
                bool AttributeIsDisabled = false;


                if (!(SelectedAttribute == null))
                {
                    if (e.Item.Checked.Equals(true))
                    {

                        if (e.Item.BackColor.Equals(Color.Red))
                        {
                            AttributeIsDisabled = true;
                        }

                    }
                    else
                    {

                        if (((SelectedAttribute.GetLowerBound().Equals("1")) && (SelectedAttribute.GetUpperBound().Equals("1"))) && (!e.Item.BackColor.Equals(Color.Red)))
                        {
                            XMLParser XMLP = new XMLParser();
                            if (XMLP.GetXmlValueConfig("Warning").Equals("Checked"))
                            {
                                WarningNeeded = true;
                            }
                        }
                    }
                }

                if (AttributeIsDisabled.Equals(true))
                {

                    MessageBox.Show("You can't change the state of this attribute as he is inherited from another abstract class.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.Item.Checked = false;

                }
                else if (WarningNeeded.Equals(false))
                {
                    populatedEAClass.SetAttributeState(e.Item.SubItems[5].Text, e.Item.Checked);
                }
                else
                {
                    DialogResult Result = MessageBox.Show("It seem unwise to uncheck an attribute having a 1..1 cardinality. \n Do you wish to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    if (Result.Equals(DialogResult.Yes))
                    {
                        populatedEAClass.SetAttributeState(e.Item.SubItems[5].Text, e.Item.Checked);
                    }
                    else
                    {
                        if (e.Item.Checked.Equals(true))
                        {
                            e.Item.Checked = false;
                        }
                        else
                        {
                            e.Item.Checked = true;
                        }
                    }
                }
            }
        }*/

        private void ButEditConstraint_Click(object sender, EventArgs e)
        {
            if (LVAttribute.SelectedItems.Count.Equals(1))
            {
                if (LVAttribute.SelectedItems[0].Checked.Equals(true))
                {
                    AttributeConstraintForm acf = new AttributeConstraintForm(populatedEAClass, LVAttribute.SelectedItems[0].SubItems[5].Text, this.Repository);
                    acf.ShowDialog();
                    acf.Dispose();// ABA20230402
                }
            }
        }

        private void CBCopyConstraints_CheckedChanged(object sender, EventArgs e)
        {
            if (CBCopyConstraints.Checked.Equals(true))
            {
                populatedEAClass.ResetConstraints();
                ButEditClassConstraint.Enabled = false;
            }
            if (CBCopyConstraints.Checked.Equals(false))
            {
                ButEditClassConstraint.Enabled = true;
            }
        }

        private void ButEditClassConstraint_Click(object sender, EventArgs e)
        {
            ClassConstraintForm CCF = new ClassConstraintForm(populatedEAClass, this.Repository);
            CCF.ShowDialog();
            CCF.Dispose();
        }

        private void CBCopyTagValues_CheckedChanged(object sender, EventArgs e)
        {
            if (CBCopyTagValues.Checked.Equals(true) && CreateMode.Equals(false) && UILoading.Equals(false) && OriginalCBCopyTagValues.Equals(false))
            {
                DialogResult Reset = MessageBox.Show("Be Careful:\n  The element have been loaded from the IsBasedOn's object.\n" +
                "  If the box is unchecked, it mean that some values can be different from the parent element.\n  Checking this box will reset their value to the parent's value.\n" +
                "  Any element added that is not on the parent should remain unchanged.", "Warning !", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (Reset.Equals(DialogResult.Cancel))
                {
                    CBCopyTagValues.Checked = false;
                }
            }
        }

        private void CBCopyNotes_CheckedChanged(object sender, EventArgs e)
        {
            if (CBCopyNotes.Checked.Equals(true) && CreateMode.Equals(false) && UILoading.Equals(false) && OriginalCBCopyNotes.Equals(false))
            {
                DialogResult Reset = MessageBox.Show("Be Careful:\n  The element have been loaded from the IsBasedOn's object.\n" +
                "  If the box is unchecked, it mean that some values can be different from the parent element.\n  Checking this box will reset their value to the parent's value.\n" +
                "  Any element added that is not on the parent should remain unchanged.", "Warning !", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (Reset.Equals(DialogResult.Cancel))
                {
                    CBCopyNotes.Checked = false;
                }
            }
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void CBCopyStereotype_CheckedChanged(object sender, EventArgs e)
        {
            if (UILoading.Equals(false))
            {
                if (CBCopyStereotype.Checked.Equals(true))
                {
                    ButEditStereotype.Enabled = false;
                    populatedEAClass.SetStereotype(Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).StereotypeEx);
                }
                else
                {
                    ButEditStereotype.Enabled = true;
                    populatedEAClass.SetStereotype("");
                }
            }
        }

        public bool GetCBCopyTagValues()
        {
            return CBCopyTagValues.Checked;
        }

        public bool GetCBCopyNotes()
        {
            return CBCopyNotes.Checked;
        }

        public bool GetCBCopyConstraints()
        {
            return CBCopyConstraints.Checked;
        }

        public bool GetCBCopyParent()
        {
            return CBCopyParent.Checked;
        }

        public bool GetCBCopyStereotype()
        {
            return CBCopyStereotype.Checked;
        }

        private void ButEditStereotype_Click(object sender, EventArgs e)
        {
            EditClassStereotype ECS = new EditClassStereotype(Repository, populatedEAClass, CreateMode);
            ECS.ShowDialog();
            ECS.Dispose();
        }

        private void ButEditAttributStereotype_Click(object sender, EventArgs e)
        {
            if ((LVAttribute.SelectedItems.Count.Equals(1)))
            {
                if (LVAttribute.SelectedItems[0].Checked.Equals(true))
                {
                    EditAttributeStereotype EAS = new EditAttributeStereotype(Repository, populatedEAClass, populatedEAClass.GetAttribute(LVAttribute.SelectedItems[0].SubItems[5].Text), CreateMode);
                    EAS.ShowDialog();
                    EAS.Dispose();
                }
            }
        }

        private ArrayList GetInheritanceList(EA.Element InheritedParent, ArrayList InheritedList)
        {

            for (short i = 0; InheritedParent.Connectors.Count > i; i++)
            {
                if (((EA.Connector)InheritedParent.Connectors.GetAt(i)).Type.Equals("Generalization"))
                {
                    if (((EA.Connector)InheritedParent.Connectors.GetAt(i)).ClientID.Equals(InheritedParent.ElementID))
                    {
                        InheritedList.Add(Repository.GetElementByID(((EA.Connector)InheritedParent.Connectors.GetAt(i)).SupplierID));
                        InheritedList = GetInheritanceList(Repository.GetElementByID(((EA.Connector)InheritedParent.Connectors.GetAt(i)).SupplierID), InheritedList);
                        break;
                    }
                }
            }
            return InheritedList;
        }

        private void CBInheritList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBInheritList.Items.Contains(CBInheritList.SelectedItem))
            {

                for (short j = 0; LVAttribute.Items.Count > j; j++)
                {
                    ListViewItem AnItem = (ListViewItem)LVAttribute.Items[j];
                    AnItem.BackColor = Color.FromArgb(255, 255, 255, 255);
                }

                if (!CBInheritList.SelectedItem.Equals("None"))
                {
                    EA.Element SelectedElement = null;
                    for (short k = 0; populatedEAClass.GetPossibleInheritance().Count > k; k++)
                    {
                        ArrayList prov = populatedEAClass.GetPossibleInheritance();
                        if (((EA.Element)populatedEAClass.GetPossibleInheritance()[k]).Name.Equals(CBInheritList.SelectedItem))
                        {
                            SelectedElement = (EA.Element)populatedEAClass.GetPossibleInheritance()[k];
                        }
                    }
                    if (!(SelectedElement == null))
                    {
                        ArrayList InheritedList = new ArrayList();
                        InheritedList.Add(SelectedElement);
                        InheritedList = GetInheritanceList(SelectedElement, InheritedList);
                        foreach (EA.Element AnElement in InheritedList)
                        {
                            for (short i = 0; AnElement.Attributes.Count > i; i++)
                            {
                                EA.Attribute AnAttribute = (EA.Attribute)AnElement.Attributes.GetAt(i);
                                for (short j = 0; LVAttribute.Items.Count > j; j++)
                                {
                                    if (LVAttribute.Items[j].SubItems[2].Text.Equals(AnAttribute.Name))
                                    {
                                        LVAttribute.Items[j].BackColor = Color.FromKnownColor(KnownColor.Red);
                                        LVAttribute.Items[j].Checked = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LVAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void IsBasedOnForm_Load(object sender, EventArgs e)
        {

        }

    }
}
