using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;/*************************************************************************\
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
using CimContextor.Configuration;
using EA;

namespace CimContextor.EditConnectors
{
    public partial class EditSubdivideForXsdConnectorForm : Form
    {
        private EditEAClassConnector ParentConnector;
        private int SubConnectorGUID;
        private bool CreateNewConnector;
        private EditDuplicateForXsdConnectorsForm DCF;
        private ConstantDefinition CD = new ConstantDefinition();
        private bool isFirstProfileLevel = true;
        private utilitaires.Utilitaires util;
        private bool isnav = false;
        private bool isSimpleProfiling = false;
        private XMLParser XMLP;
        private bool SSwitch = false; // am juil 2016
        EditEAClassConnector EditedConnector = null;
        EA.Repository Repo;

        // ABA 20230202

        private void fillInStereotypes(EA.Repository Repo, EditEAClassConnector ParentConnector)
        {
            foreach (EA.Stereotype AStereotype in Repo.Stereotypes)
            {
                if (AStereotype.AppliesTo.ToLower().Equals(ParentConnector.GetType().ToLower()))
                {
                    CBStereotype.Items.Add(AStereotype.Name);
                }
            }
            CBStereotype.Items.Add("No Stereotype");
        }

        // ABA 20230202
        private void fillInConstraints(XMLParser xmlp)
        {
            ArrayList ConstraintList = xmlp.GetXmlConstraint("any");
            foreach (object aConstraint in ConstraintList)
            {
                this.TBConstraints.Items.Add((string)((XMLConstraint)aConstraint).GetName());
            }
            this.TBConstraints.Items.Add("No Constraint");
            this.TBConstraints.SelectedItem = "No Constraint";
        }

        // ABA 20230202
        private void setQualifiersForSimpleProfiling()
        {
            this.CBClientQualifier.Enabled = false;
            this.CBSupplierQualifier.Enabled = false;
            this.button1.Enabled = false;
        }

        // ABA 20230202
        private void setupQualifierList(XMLParser xmlp)
        {
            ArrayList QualifierList = xmlp.GetXmlQualifier("role");
            QualifierList.Add("No qualifier");
            foreach (object aQualifier in QualifierList)
            {
                CBClientQualifier.Items.Add((string)aQualifier);
                CBSupplierQualifier.Items.Add((string)aQualifier);
            }

            CBClientQualifier.SelectedItem = "No qualifier";
            CBSupplierQualifier.SelectedItem = "No qualifier";
        }

        // ABA 20230202
        private void setNavigabilityControls(EA.Connector econ, bool isnav, bool SSwitch, bool isFirstProfileLevel)
        {
            if (SSwitch)
            {
                if (((!isnav) && econ.ClientEnd.Aggregation == 1) ||
                    (isnav && econ.SupplierEnd.Navigable.Equals("Navigable")))
                {
                    this.CBAgregateTarget.Checked = true;
                    this.CBByRefTarget.Enabled = false; //am juil 2016
                    if (econ.SupplierEnd.Containment == "Reference")
                    {
                        this.CBByRefSource.Checked = true;
                    }
                }
                else if (((!isnav) && econ.SupplierEnd.Aggregation == 1) ||
                        (isnav && econ.ClientEnd.Navigable.Equals("Navigable")))
                {
                    this.CBAgregateSource.Checked = true;
                    this.CBByRefSource.Enabled = false; //am juil 2016
                    if (econ.ClientEnd.Containment == "Reference")
                    {
                        this.CBByRefTarget.Checked = true;
                    }
                }
                else
                {
                    if (!isFirstProfileLevel)
                    {
                        ErrorCodes.ShowError(ErrorCodes.ERROR_040);
                    }
                }
            }// if sswitch
            else
            {
                if (((!isnav) && econ.ClientEnd.Aggregation == 1) ||
                    (isnav && (econ.SupplierEnd.Navigable.Equals("Navigable"))))
                {
                    this.CBAgregateSource.Checked = true;
                    this.CBByRefSource.Enabled = false; //am juil 2016
                    if (econ.SupplierEnd.Containment == "Reference")
                    {
                        this.CBByRefTarget.Checked = true;
                    }
                    // ###
                }
                else
                {
                    if (((!isnav) && econ.SupplierEnd.Aggregation == 1) ||
                            (isnav && (econ.ClientEnd.Navigable.Equals("Navigable"))))
                    {
                        this.CBAgregateTarget.Checked = true;
                        this.CBByRefTarget.Enabled = false; //am juil 2016
                        if (econ.ClientEnd.Containment == "Reference")
                        {
                            this.CBByRefSource.Checked = true;
                        }
                    }
                    else
                    {
                        if (!isFirstProfileLevel) // am juil 2016
                        {
                            ErrorCodes.ShowError(ErrorCodes.WARNING_041);
                        }
                    }
                }
            }
        }

        public EditSubdivideForXsdConnectorForm(EditDuplicateForXsdConnectorsForm DCF, bool CreateNewConnector, EditEAClassConnector ParentConnector, int SubConnectorGUID)
        {
            #region Contructor
            InitializeComponent();
            this.DCF = DCF;
            Repo = DCF.GetRepository();
            this.util=new utilitaires.Utilitaires(Repo); // am mars 2016
            this.XMLP = new XMLParser(Repo);
            isnav = this.XMLP.GetXmlValueConfig("NavigationEnabled") == ConfigurationManager.CHECKED;
            isSimpleProfiling = this.XMLP.GetXmlValueConfig("SimpleProfiling") == ConfigurationManager.CHECKED;
            this.ParentConnector = ParentConnector;
            this.SubConnectorGUID = SubConnectorGUID;
            this.CreateNewConnector = CreateNewConnector;
           
            if (isSimpleProfiling)
            {
                setQualifiersForSimpleProfiling();
            }

            EA.Element sel = this.ParentConnector.GetSelectedIBOElement();
            long packid = sel.PackageID;
            isFirstProfileLevel = util.isAFirstLevelPackage(Repo, util.getProfilePackage(Repo,packid).PackageID);//am sept 2016

            this.fillInConstraints(this.XMLP);
            this.fillInStereotypes(Repo, ParentConnector);
            this.setupQualifierList(XMLP);

            if (CreateNewConnector.Equals(true))
            {
                #region NewSubConnector
                #region FirstWay

                if ((DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                     ||
                    (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Contains(this.ParentConnector.GetSelectedElementConnector().ElementID)))
                 {
                    this.groupBox2.Text = DCF.GetSelectedIBOItem().Name; // ABA 20230204 .GetSelectedItem().Name ; // am mars 2016
                    this.groupBox3.Text = ParentConnector.GetTargetedIBOElement().Name ; // am mars 2016
                    
                    if(isFirstProfileLevel)  //am juil 2016
                    { // am juil 201
                        // ABA 20230202 EA.Connector con = ParentConnector.GetOriginalConnector(); //am juil 2016
                        this.CBAgregateSource.Enabled = true; //am mars 2016
                        this.CBAgregateTarget.Enabled = true; //am mars 2016
                        this.CBByRefSource.Enabled = false;//am mars 2016
                        this.CBByRefTarget.Enabled = false;//am mars 2016
                    } // am juil 2016
                    else // am juil 2016
                    {  // am juil 2016                       
                        EA.Connector con = ParentConnector.GetOriginalConnector(); // am juol 160716
                        this.CBByRefTarget.Enabled = false; //am juil 2016
                        this.CBByRefSource.Enabled = false; //am juil 2016
                        this.CBAgregateSource.Checked = false; //am mars 2016
                        this.CBAgregateTarget.Enabled = false;
                        bool SSwitch = util.getSSwitch(con.ClientID, ParentConnector.GetSelectedElementConnector());
                        if(!SSwitch)
                        {
                            if ((!isnav && con.ClientEnd.Aggregation==1)
                                || (isnav &&( con.SupplierEnd.Navigable.Equals("Navigable"))))
                            {
                                this.CBAgregateSource.Checked = true;
                                this.CBByRefTarget.Enabled = true; //am juil 2016
                                if (con.SupplierEnd.Containment == "Reference")
                                {
                                    this.CBByRefTarget.Checked = true;
                                }
                            }
                            else
                            {
                                if ((!isnav && con.SupplierEnd.Aggregation==1)
                                    || (isnav &&( con.ClientEnd.Navigable.Equals("Navigable"))))
                                {
                                    this.CBAgregateTarget.Checked = true;
                                    this.CBByRefSource.Enabled = true; //am juil 2016
                                    if (con.ClientEnd.Containment == "Reference")
                                    {
                                        this.CBByRefSource.Checked = true;
                                    }
                                }
                                else
                                {
                                    ErrorCodes.ShowError(ErrorCodes.ERROR_042);
                                }
                            }
                        }
                        else
                        {
                            if ((!isnav && con.ClientEnd.Aggregation == 1)
                                || (isnav &&( con.SupplierEnd.Navigable.Equals("Navigable"))))
                            {
                                this.CBAgregateTarget.Checked = true;
                                this.CBByRefSource.Enabled = true; //am juil 2016
                                if (con.SupplierEnd.Containment == "Reference")
                                {
                                    this.CBByRefSource.Checked = true;
                                }
                            }
                            else
                            {
                                if ((!isnav && con.SupplierEnd.Aggregation == 1)
                                    || (isnav &&( con.ClientEnd.Navigable.Equals("Navigable")))
                                    )
                                {
                                    this.CBAgregateSource.Checked = true;
                                    this.CBByRefTarget.Enabled = true; //am juil 2016
                                    if (con.ClientEnd.Containment == "Reference")
                                    {
                                        this.CBByRefTarget.Checked = true;
                                    }
                                }
                                else
                                {
                                    if (!isFirstProfileLevel)  //am juil 2016
                                    {
                                        ErrorCodes.ShowError(ErrorCodes.ERROR_043);
                                    }
                                }
                            }
                        }
                        if (isFirstProfileLevel)  //am juil 2016
                        { // am juil 201
                            this.CBAgregateSource.Enabled = true; //am mars 2016
                            this.CBAgregateTarget.Enabled = true; //am mars 2016
                        }
                        else
                        {
                            this.CBAgregateSource.Enabled = false; //am mars 2016
                            this.CBAgregateTarget.Enabled = false; //am mars 2016
                        }
                      } // am juil 2016
                   
                    this.CBByRefSource.Text = "By Ref"; // am mars 2016
                    this.CBByRefTarget.Text = "By Ref"; // am mars 2016

                    this.Text = "Editing Association  " + DCF.GetSelectedItem().Name + " - " + ParentConnector.GetTargetedIBOElement().Name; // am juil 2016
                    LabSupplierRole.Text = "Parent's role : " + ParentConnector.GetSupplierRole(); // am mars 2016
                    LabClientRole.Text = "Parent's role : " + ParentConnector.GetClientRole();

                        if (
                            (XMLP.GetXmlValueConfig("AutomaticChangeOfRoleName") == ConfigurationManager.CHECKED)
                            &&
                             (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Count > 0)
                            &&
                           (//18 utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID][0] == ParentConnector.GetSelectedElementConnector().ElementID)
                             utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Contains(ParentConnector.GetSelectedElementConnector().ElementID))
                           )
                    
                        {//if the selected element inherits
                          if (ParentConnector.GetClientRole().Contains("_"))
                          { 
                            string[] split = ParentConnector.GetClientRole().Split(new char[] { '_' }); 
                            TBClientRole.Text = split[0] + "_" + DCF.GetSelectedItem().Name;
                          }
                          else
                           {
                            TBClientRole.Text = DCF.GetSelectedItem().Name;// we chose the name of the element as role
                           }                           
                        }
                        else
                        {
                            TBClientRole.Text = ParentConnector.GetClientRole();
                        }
                        TBClientRole.Enabled = false; // ABA20230711

                        EA.Element target = Repo.GetElementByGuid(util.getEltParentGuid(ParentConnector.GetTargetedIBOElement()));

                        if (
                            (XMLP.GetXmlValueConfig("AutomaticChangeOfRoleName") == ConfigurationManager.CHECKED)
                            &&
                            (utilitaires.Utilitaires.dicAncestors[target.ElementID].Count > 0)
                             &&
                            (//18 utilitaires.Utilitaires.dicAncestors[target.ElementID][0] == ParentConnector.GetTargetedElementConnector().ElementID) // am aout 2018
                            utilitaires.Utilitaires.dicAncestors[target.ElementID].Contains(ParentConnector.GetTargetedElementConnector().ElementID)) // am aout 2018
                            )
                        {//if the selected element inherits
                            if (ParentConnector.GetSupplierRole().Contains("_"))//am dec 2019
                            { //am dec 2019
                               string[] split = ParentConnector.GetSupplierRole().Split(new char[] { '_' }); //am dec 2019
                               TBSupplierRole.Text = split[0] + "_" + ParentConnector.GetTargetedIBOElement().Name;//am dec 2019
                            }//am dec 2019
                            else//am dec 2019
                            {//am dec 2019
                               TBSupplierRole.Text = ParentConnector.GetTargetedIBOElement().Name;// we chose the name of the element as role
                            }//am dec 2019
                        }
                        else
                        {
                        TBSupplierRole.Text = ParentConnector.GetSupplierRole();
                        }
                    
                   

                    this.CBLBClientCardinality.Items.Add(ParentConnector.GetClientLBCardinality());
                    this.CBLBClientCardinality.SelectedItem = ParentConnector.GetClientLBCardinality();

                    this.CBUBClientCardinality.Items.Add(ParentConnector.GetClientUBCardinality());
                    this.CBUBClientCardinality.SelectedItem = ParentConnector.GetClientUBCardinality();

                    this.CBLBSupplierCardinality.Items.Add(ParentConnector.GetSupplierLBCardinality());
                    this.CBLBSupplierCardinality.SelectedItem = ParentConnector.GetSupplierLBCardinality();

                    this.CBUBSupplierCardinality.Items.Add(ParentConnector.GetSupplierUBCardinality());
                    this.CBUBSupplierCardinality.SelectedItem = ParentConnector.GetSupplierUBCardinality();

                }
                #endregion
                 #endregion
            }
            else
            {
                #region EditingSubConnector
                this.EditedConnector = ParentConnector.GetSubConnector(SubConnectorGUID); // am juil 2016
                if (
                    (DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                    ||
                    (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Contains(this.ParentConnector.GetSelectedElementConnector().ElementID)) //am aout 2018
               )
                {
                    this.Text = "Editing a subdivision  " + ParentConnector.GetSelectedIBOElement().Name + " - " + ParentConnector.GetTargetedIBOElement().Name;// am juil 2016
                    this.groupBox2.Text = DCF.GetSelectedIBOItem().Name; // ABA 20230204 .GetSelectedItem().Name; // am mars 2016
                    this.groupBox3.Text = ParentConnector.GetTargetedIBOElement().Name; // am mars 2016
                    EA.Connector con = ParentConnector.GetOriginalConnector();
                    EditedConnector = ParentConnector.GetSubConnector(SubConnectorGUID); // am juil 2016
                    EA.Connector econ = EditedConnector.GetEditedConnector(); // am juil 2016
                    if (econ == null)//am aout 18
                    {
                        MessageBox.Show("Impossible to modify a connector which is not existing yet!");
                        DCF.subok = false;
                        return;
                    }

                    if (econ.ClientID != this.DCF.GetSelectedIBOItem().ElementID) //am aour 2018                    
                    {
                        SSwitch = true; // The selected element is not identical with the start end of the connector
                    }

                    setNavigabilityControls(econ, isnav, SSwitch, isFirstProfileLevel); // !!!

                    if (isFirstProfileLevel)
                    {
                        if (this.CBAgregateSource.Checked == true)
                        {
                            this.CBAgregateSource.Enabled = true; //am mars 2016
                            this.CBAgregateTarget.Enabled = false; //am mars 2016
                            this.CBByRefSource.Enabled= false; //am juil 2016
                            this.CBByRefTarget.Enabled = true; //am juil 2016
                        }
                        else
                        {
                            this.CBAgregateSource.Enabled = false; //am mars 2016
                            this.CBAgregateTarget.Enabled = true; //am mars 2016
                            this.CBByRefSource.Enabled = true; //am juil 2016
                            this.CBByRefTarget.Enabled = false; //am juil 2016
                         
                        }
                    }else{
                        this.CBAgregateSource.Enabled = false; //am mars 2016
                        this.CBAgregateTarget.Enabled = false; //am mars 2016
                    }

                    this.CBByRefSource.Text = "By Ref"; 
                    this.CBByRefTarget.Text = "By Ref"; 
                    this.Text = "Editing Association  " + DCF.GetSelectedItem().Name + " - " + ParentConnector.GetTargetedIBOElement().Name; // am juil 2016

                    if (SSwitch) // am juil 2016
                    {
                        // am aout 2018
                       LabClientRole.Text = ParentConnector.GetClientRole(); //util.RemoveQual(EditedConnector.GetClientRole());//am aout 2018
                       LabSupplierRole.Text = ParentConnector.GetSupplierRole(); //util.RemoveQual(EditedConnector.GetSupplierRole());//am aout 2018   
                       TBClientRole.Text = ParentConnector.GetClientRole();//EditedConnector.GetClientRole();//  ParentConnector.GetSupplierRole();// EditedConnector.GetSupplierRole();//ParentConnector.GetSupplierRole(); //EditedConnector.GetSupplierRole();// am juil 2016 
                       TBSupplierRole.Text = ParentConnector.GetSupplierRole();//EditedConnector.GetClientRole();//ParentConnector.GetClientRole();// EditedConnector.GetClientRole();// am juil 2016
                    }                   
                    else
                    { // selected element is client of connector
                        if(EditedConnector.GetEditedConnector().ClientID == sel.ElementID)
                        {
                            TBClientRole.Text = EditedConnector.GetClientRole();
                            TBSupplierRole.Text = EditedConnector.GetSupplierRole();
                        }
                        else // selected element is supplier of connector
                        {
                            TBClientRole.Text = EditedConnector.GetSupplierRole();
                            TBSupplierRole.Text = EditedConnector.GetClientRole();
                        }
                    }
                    TBClientRole.Enabled = false; // ABA20230711

                    CBCopyStereotype.Checked = EditedConnector.GetCopyStereotype();
                    if ((CBCopyStereotype.Checked.Equals(false)) && (CBStereotype.Items.Contains(EditedConnector.GetStereotype())))
                    {
                        CBStereotype.SelectedItem = EditedConnector.GetStereotype();
                    }

                    if (!this.SSwitch)
                    {
                        if (EditedConnector.GetClientRoleQualifier().Equals(""))
                        {
                            CBClientQualifier.SelectedItem = CD.GetNoQualifier();
                        }
                        else
                        {
                            CBClientQualifier.Items.Add(EditedConnector.GetClientRoleQualifier());
                            CBSupplierQualifier.Items.Add(EditedConnector.GetClientRoleQualifier());
                            CBClientQualifier.SelectedItem = EditedConnector.GetClientRoleQualifier();
                        }

                        if (EditedConnector.GetSupplierRoleQualifier().Equals(""))
                        {
                            CBSupplierQualifier.SelectedItem = "No qualifier";
                        }
                        else
                        {
                            CBSupplierQualifier.Items.Add(EditedConnector.GetSupplierRoleQualifier());
                            CBClientQualifier.Items.Add(EditedConnector.GetSupplierRoleQualifier());
                            CBSupplierQualifier.SelectedItem = EditedConnector.GetSupplierRoleQualifier();
                        }
                    }
                    else
                    {
                        if (EditedConnector.GetClientRoleQualifier().Equals(""))
                        {
                            CBSupplierQualifier.SelectedItem = CD.GetNoQualifier();
                        }
                        else
                        {
                            CBSupplierQualifier.Items.Add(EditedConnector.GetClientRoleQualifier());
                            CBClientQualifier.Items.Add(EditedConnector.GetClientRoleQualifier());
                            CBSupplierQualifier.SelectedItem = EditedConnector.GetClientRoleQualifier();
                        }

                        if (EditedConnector.GetSupplierRoleQualifier().Equals(""))
                        {
                            CBClientQualifier.SelectedItem = "No qualifier";
                        }
                        else
                        {
                            CBSupplierQualifier.Items.Add(EditedConnector.GetSupplierRoleQualifier());
                            CBClientQualifier.Items.Add(EditedConnector.GetSupplierRoleQualifier());
                            CBClientQualifier.SelectedItem = EditedConnector.GetSupplierRoleQualifier();
                        } 
                    }
                 
                    if (EditedConnector.GetSupplierContainmentByRef().Equals(true))
                    {
                        CBByRefTarget.Checked = true;
                    }
                    else if (EditedConnector.GetClientContainmentByRef().Equals(true))
                    {
                        CBByRefSource.Checked = true;
                    }


                    CBCopyNotes.Checked = EditedConnector.GetCopyNotes();
                    CBCopyTagValues.Checked = EditedConnector.GetCopyTagValues();

                    if (SSwitch)
                    {
                        this.CBLBClientCardinality.Items.Add(EditedConnector.GetSupplierLBCardinality());
                        this.CBLBClientCardinality.SelectedItem = EditedConnector.GetSupplierLBCardinality();
                        this.CBUBClientCardinality.Items.Add(EditedConnector.GetSupplierUBCardinality());
                        this.CBUBClientCardinality.SelectedItem = EditedConnector.GetSupplierUBCardinality();

                        this.CBLBSupplierCardinality.Items.Add(EditedConnector.GetClientLBCardinality());
                        this.CBLBSupplierCardinality.SelectedItem = EditedConnector.GetClientLBCardinality();
                        this.CBUBSupplierCardinality.Items.Add(EditedConnector.GetClientUBCardinality());
                        this.CBUBSupplierCardinality.SelectedItem = EditedConnector.GetClientUBCardinality();
                    }
                    else
                    {
                        this.CBLBClientCardinality.Items.Add(EditedConnector.GetClientLBCardinality());
                        this.CBLBClientCardinality.SelectedItem = EditedConnector.GetClientLBCardinality();
                        this.CBUBClientCardinality.Items.Add(EditedConnector.GetClientUBCardinality());
                        this.CBUBClientCardinality.SelectedItem = EditedConnector.GetClientUBCardinality();

                        this.CBLBSupplierCardinality.Items.Add(EditedConnector.GetSupplierLBCardinality());
                        this.CBLBSupplierCardinality.SelectedItem = EditedConnector.GetSupplierLBCardinality();
                        this.CBUBSupplierCardinality.Items.Add(EditedConnector.GetSupplierUBCardinality());
                        this.CBUBSupplierCardinality.SelectedItem = EditedConnector.GetSupplierUBCardinality();
                    }
                }
                else
                {
                    this.Text = "Editing a subdivision  " + ParentConnector.GetTargetedIBOElement().Name + " - " + ParentConnector.GetSelectedIBOElement().Name; // am juil 2016
                    LabSupplierRole.Text = "Parent's role : " + ParentConnector.GetClientRole();
                    LabClientRole.Text = "Parent's role : " + ParentConnector.GetSupplierRole();
                    TBClientRole.Text = EditedConnector.GetSupplierRole();
                    //ABA20240919 TBClientRole.Enabled = false; // ABA20230711
                    TBSupplierRole.Text = EditedConnector.GetClientRole();
                    CBCopyStereotype.Checked = EditedConnector.GetCopyStereotype();
                    if ((CBCopyStereotype.Checked.Equals(false)) && (CBStereotype.Items.Contains(EditedConnector.GetStereotype())))
                    {
                        CBStereotype.SelectedItem = EditedConnector.GetStereotype();
                    }

                    if (CBClientQualifier.Items.Contains(EditedConnector.GetSupplierRoleQualifier()))
                    {
                        CBClientQualifier.SelectedItem = EditedConnector.GetSupplierRoleQualifier();
                    }
                    else if (EditedConnector.GetSupplierRoleQualifier().Equals(""))
                    {
                        CBClientQualifier.SelectedItem = "No qualifier";
                    }
                    else
                    {
                        MessageBox.Show("Can't find the previously selected client qualifier in the list, reseting it", "Qualifier Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        CBClientQualifier.SelectedItem = "No qualifier";
                        EditedConnector.SetClientRoleQualifier("");
                    }
                    if (CBSupplierQualifier.Items.Contains(EditedConnector.GetClientRoleQualifier()))
                    {
                        CBSupplierQualifier.SelectedItem = EditedConnector.GetClientRoleQualifier();
                    }
                    else if (EditedConnector.GetClientRoleQualifier().Equals(""))
                    {
                        CBSupplierQualifier.SelectedItem = "No qualifier";
                    }
                    else
                    {
                        MessageBox.Show("Can't find the previously selected supplier qualifier in the list, reseting it", "Qualifier Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        CBSupplierQualifier.SelectedItem = "No qualifier";
                        EditedConnector.SetSupplierRoleQualifier("");
                    }

                    if (EditedConnector.GetSupplierContainmentByRef().Equals(true) )//|| EditedConnector.GetClientContainmentByRef().Equals(true))
                    {
                        CBByRefTarget.Checked = true;
                    }//am mars 2016
                    else
                    {
                    if ( EditedConnector.GetClientContainmentByRef().Equals(true))
                    {
                        CBByRefSource.Checked = true;
                    }
                    }//am mars 2016
                    CBCopyNotes.Checked = EditedConnector.GetCopyNotes();
                    CBCopyTagValues.Checked = EditedConnector.GetCopyTagValues();
                    this.CBLBClientCardinality.Items.Add(EditedConnector.GetSupplierLBCardinality());
                    this.CBLBClientCardinality.SelectedItem = EditedConnector.GetSupplierLBCardinality();
                    this.CBUBClientCardinality.Items.Add(EditedConnector.GetSupplierUBCardinality());
                    this.CBUBClientCardinality.SelectedItem = EditedConnector.GetSupplierUBCardinality();
                    this.CBLBSupplierCardinality.Items.Add(EditedConnector.GetClientLBCardinality());
                    this.CBLBSupplierCardinality.SelectedItem = EditedConnector.GetClientLBCardinality();
                    this.CBUBSupplierCardinality.Items.Add(EditedConnector.GetClientUBCardinality());
                    this.CBUBSupplierCardinality.SelectedItem = EditedConnector.GetClientUBCardinality();
                }
                #endregion               
            }
            #endregion
        }

        

        private void RefreshParentUI()
        {
            DCF.RefreshUI();
        }

        private void AddSubdividedConnector_Click(object sender, EventArgs e)
        {
            bool CheckClient = false;
            bool CheckSupplier = false;
            bool CheckOtherEnd = false;
            EditEAClassConnector ANewConnector=null;
           // util.wlog("TEST", "SaveSC " + ParentConnector.GetTrace());
            string p1 = DCF.GetSelectedItem().ElementGUID;
            string p11 = DCF.GetSelectedItem().Name;
            string p2 = this.ParentConnector.GetSelectedElementConnector().ElementGUID;
            string p21 = this.ParentConnector.GetSelectedElementConnector().Name;
            /*
            if (!CBAgregateSource.Checked.Equals(true))
            {
                if (DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                {
                    CheckClient = CheckClientCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                    CheckSupplier = CheckSupplierCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                }
                else
                {
                    CheckClient = CheckClientCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                    CheckSupplier = CheckSupplierCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                }
            }
            else
            {
                if (DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                {
                    CheckOtherEnd = CheckSupplierCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                }
                else
                {
                    CheckOtherEnd = CheckSupplierCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                }
            }
            */
          //  if ((CheckClient.Equals(true) && CheckSupplier.Equals(true)) || ((CBAgregateSource.Checked.Equals(true) && CheckOtherEnd.Equals(true))))
           // {
                if (CreateNewConnector.Equals(true))
                {

                    #region CreateNewConnector
                    if (!CBAgregateSource.Checked.Equals(true)) //am juil 2016
                    {
                        if(
                            (DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                            ||
                            (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Contains(this.ParentConnector.GetSelectedElementConnector().ElementID))// am aout 2018
                       )
                        {
                            CheckClient = CheckClientCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                            CheckSupplier = CheckSupplierCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                        }
                        else
                        {
                            CheckClient = CheckClientCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                            CheckSupplier = CheckSupplierCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                        }
                    }
                    else
                    {
                        int prov1 = DCF.GetSelectedItem().ElementID;
                        int prov2 = this.ParentConnector.GetSelectedElementConnector().ElementID;

                        if (
                            (DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                            ||
                        (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Contains(this.ParentConnector.GetSelectedElementConnector().ElementID))// am aout 2018
                       )
                        {
                            CheckOtherEnd = CheckSupplierCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                        }
                        else
                        {
                            CheckOtherEnd = CheckSupplierCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                        }
                    }
                    if ((CheckClient.Equals(true) && CheckSupplier.Equals(true)) || ((CBAgregateSource.Checked.Equals(true) && CheckOtherEnd.Equals(true))))
                         {
                        ANewConnector = ParentConnector.GetSubConnector(ParentConnector.AddSubConnector());
                        if ( (CBAgregateSource.Checked.Equals(false) && CBAgregateTarget.Checked.Equals(false)))
                        {
                            MessageBox.Show(" Warning an association should be oriented for Xsd profiling");
                            return;
                        }
                        else if (CBAgregateSource.Checked.Equals(true))
                        {
                            ANewConnector.SetAgregate("SOURCE");
                            ANewConnector.SetSupplierLBCardinality(CBLBSupplierCardinality.Text);
                            ANewConnector.SetSupplierUBCardinality(CBUBSupplierCardinality.Text);

                           // ANewConnector.SetClientLBCardinality(CBLBClientCardinality.Text);//am mars 2016
                           // ANewConnector.SetClientUBCardinality(CBUBClientCardinality.Text);//am mars 2016
                        }
                        else if (CBAgregateTarget.Checked.Equals(true))
                        {
                            ANewConnector.SetAgregate("TARGET");
                            ANewConnector.SetClientLBCardinality(CBLBClientCardinality.Text);
                            ANewConnector.SetClientUBCardinality(CBUBClientCardinality.Text);
                           // ANewConnector.SetSupplierLBCardinality(CBLBSupplierCardinality.Text);//am mars 2016
                           // ANewConnector.SetSupplierUBCardinality(CBUBSupplierCardinality.Text);//am mars 2016
                        }
                        if (CBByRefTarget.Checked.Equals(true) || CBByRefSource.Checked.Equals(true))//am mars 2016
                        {
                         //   util.wlog("TEST", "CreateNewConnector Reftarget= " + CBByRefTarget.Checked.ToString() + "  Refsource=" + CBByRefSource.Checked.ToString());//am mars 2016
                            if (CBByRefTarget.Checked.Equals(true))
                            {
                                ANewConnector.SetSupplierContainmentByRef(true); //am mars 2016
                                ANewConnector.SetClientContainmentByRef(false); //am mars 2016
                                // the target side must be navigable 
                                // ANewConnector.SetAgregate("SOURCE");//am mars 2016
                                // ANewConnector.SetClientLBCardinality(CBLBSupplierCardinality.Text);//am mars 2016
                                // ANewConnector.SetClientUBCardinality(CBUBSupplierCardinality.Text);//am mars 2016
                            }
                            else
                            {
                                ANewConnector.SetClientContainmentByRef(true); //am mars 2016
                                ANewConnector.SetSupplierContainmentByRef(false); //am mars 2016
                                //ANewConnector.SetAgregate("TARGET");//am mars 2016
                                // ANewConnector.SetSupplierLBCardinality(CBLBClientCardinality.Text);//am mars 2016
                                // ANewConnector.SetSupplierUBCardinality(CBUBClientCardinality.Text);//am mars 2016
                            }
                        }
                        else
                        {
                            ANewConnector.SetClientContainmentByRef(false); //am mars 2016
                            ANewConnector.SetSupplierContainmentByRef(false); //am mars 2016
                        }

                     
                        if (CBAgregateTarget.Checked.Equals(true)) ANewConnector.SetSupplierRole(TBSupplierRole.Text);// am mars 2016 aout 18
                        if (CBAgregateSource.Checked.Equals(true)) ANewConnector.SetClientRole(TBClientRole.Text);// am mars 2016 aout 18
                        ANewConnector.SetCopyNotes(CBCopyNotes.Checked);
                        ANewConnector.SetCopyTagValues(CBCopyTagValues.Checked);
                        ANewConnector.SetTaggedValue(ParentConnector.GetTaggedValue());
                        ANewConnector.SetCopyStereotype(CBCopyStereotype.Checked);

                        if ((CBCopyStereotype.Checked.Equals(false)) && (CBStereotype.Items.Contains(CBStereotype.SelectedItem)))
                        {
                            ANewConnector.SetStereotype(CBStereotype.SelectedItem.ToString());
                        }
                        else { ANewConnector.SetStereotype(""); }


                        if (!CBClientQualifier.Text.Equals("No qualifier") && !CBClientQualifier.Text.Equals(""))
                        {
                            ANewConnector.SetClientRoleQualifier(GetFormatedQualifierFromClientComboBox(true));
                        }
                        if (!CBSupplierQualifier.Text.Equals("No qualifier") && !CBSupplierQualifier.Text.Equals(""))
                        {
                            ANewConnector.SetSupplierRoleQualifier(GetFormatedQualifierFromClientComboBox(false));
                        }
                    ANewConnector.SetSupplierRole(TBSupplierRole.Text);//am aout 2018
                    ANewConnector.SetClientRole(TBClientRole.Text);//am aout 2018
                    //---------------  constraints -------------------------------------

                    #endregion
                }
                }
                else
                {
                    #region UpdateConnector
                    EditedConnector = ParentConnector.GetSubConnector(SubConnectorGUID); // am juil 2016
                    EA.Connector econ = EditedConnector.GetEditedConnector(); // am juil 2016
                    long p4 = econ.ClientID;
                    string P44 = econ.ConnectorGUID;
                    long p5 = DCF.GetSelectedIBOItem().ElementID;
                    string p55 = DCF.GetSelectedIBOItem().Name;


                    // if (con.ClientID != ParentConnector.GetSelectedElementConnector().ElementID)
                    //if (con.ClientID != EditedConnector.GetSelectedElementConnector().ElementID) //160716 0958
                    // if (con.ClientID != this.DCF.GetSelectedItem().ElementID)
                    if (econ.ClientID != this.DCF.GetSelectedIBOItem().ElementID)
                    {
                        SSwitch = true;
                    }
                    /*
                    if(this.SSwitch)
                    {
                        CheckClient = CheckClientCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                        CheckSupplier = CheckSupplierCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                    }
                    else
                    {
                        CheckClient = CheckClientCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                        CheckSupplier = CheckSupplierCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                    }
                     * */
                    CheckClient = true;
                    CheckSupplier = true;
                    /*
                    if (!CBAgregateSource.Checked.Equals(true)) //
                    {
                        if (DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                        {
                            CheckClient = CheckClientCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                            CheckSupplier = CheckSupplierCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                        }
                        else
                        {
                            CheckClient = CheckClientCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                            CheckSupplier = CheckSupplierCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                        }
                    }
                    else
                    {
                        if (DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                        {
                            CheckOtherEnd = CheckSupplierCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
                        }
                        else
                        {
                            CheckOtherEnd = CheckSupplierCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                        }
                    }
                         * */
                    if ((CheckClient.Equals(true) && CheckSupplier.Equals(true)) || ((CBAgregateSource.Checked.Equals(true) && CheckOtherEnd.Equals(true))))
                    {
                        ANewConnector = ParentConnector.GetSubConnector(SubConnectorGUID);
                           // util.wlog("TEST", "updateConnector parent.subc" + ParentConnector.GetTrace());
                            this.CBAgregateSource.Enabled = false; //am mars 2016
                            this.CBAgregateTarget.Enabled = false; //am mars 2016
                            this.CBByRefSource.Enabled = false; //am mars 2016
                            this.CBByRefTarget.Enabled = false; //am mars 2016
                            this.CBByRefSource.Text = "By Ref"; // am mars 2016
                            this.CBByRefTarget.Text = "By Ref"; // am mars 2016
                        if (!this.SSwitch)
                        {
                            

                            if (CBAgregateSource.Checked.Equals(false))
                            {
                                //ANewConnector.SetSupplierContainmentByRef(false);
                                // ANewConnector.SetSupplierContainmentByRef(true);//am mars 2016
                                this.CBByRefSource.Enabled = true; //am juil 2016
                            }
                            if (CBAgregateTarget.Checked.Equals(false))
                            {
                                //ANewConnector.SetClientContainmentByRef(false);
                                //  ANewConnector.SetClientContainmentByRef(true);//am mars 2016
                                this.CBByRefTarget.Enabled = true; //am mars 2016
                            }
                            //util.wlog("TEST", "updateConnector newACon" + ANewConnector.GetTrace());

                            if ((CBAgregateTarget.Checked.Equals(false) && CBAgregateSource.Checked.Equals(false)))
                            {
                                ANewConnector.SetAgregate("");
                            }
                            if (CBAgregateSource.Checked.Equals(true))
                            {
                                ANewConnector.SetAgregate("SOURCE");

                                if (CBByRefTarget.Checked.Equals(true))
                                {
                                    ANewConnector.SetSupplierContainmentByRef(true);
                                }
                                else
                                {
                                    ANewConnector.SetSupplierContainmentByRef(false);
                                }
                                ANewConnector.SetClientRole(TBClientRole.Text); //am juil 2016
                                ANewConnector.SetSupplierRole(TBSupplierRole.Text); // am juil 2016

                            }
                            if (CBAgregateTarget.Checked.Equals(true))
                            {
                                ANewConnector.SetAgregate("TARGET");

                                if (CBByRefSource.Checked.Equals(true))
                                {
                                    ANewConnector.SetClientContainmentByRef(true);
                                }
                                else
                                {
                                    ANewConnector.SetClientContainmentByRef(false);
                                }
                                ANewConnector.SetSupplierRole(TBSupplierRole.Text); //am juil 2016
                                ANewConnector.SetClientRole(TBClientRole.Text); // am juil 2016
                            }


                           // ANewConnector.SetClientRole(TBClientRole.Text); //am juil 2016
                           // ANewConnector.SetSupplierRole(TBSupplierRole.Text); // am juil 2016
                            // if (CBAgregateTarget.Checked.Equals(true)) ANewConnector.SetSupplierRole("");// am mars 2016
                            // if (CBAgregateSource.Checked.Equals(true)) ANewConnector.SetClientRole("");// am mars 2016
                            ANewConnector.SetCopyTagValues(CBCopyTagValues.Checked);
                            ANewConnector.SetTaggedValue(ParentConnector.GetTaggedValue());
                            ANewConnector.SetCopyNotes(CBCopyNotes.Checked);

                            ANewConnector.SetCopyStereotype(CBCopyStereotype.Checked);
                            if ((CBCopyStereotype.Checked.Equals(false)) && (CBStereotype.Items.Contains(CBStereotype.SelectedItem)))
                            {
                                ANewConnector.SetStereotype(CBStereotype.SelectedItem.ToString());
                            }
                            else { ANewConnector.SetStereotype(""); }

                            ANewConnector.SetClientLBCardinality(CBLBClientCardinality.Text);
                            ANewConnector.SetClientUBCardinality(CBUBClientCardinality.Text);
                            ANewConnector.SetSupplierLBCardinality(CBLBSupplierCardinality.Text);
                            ANewConnector.SetSupplierUBCardinality(CBUBSupplierCardinality.Text);
                            if (!CBClientQualifier.Text.Equals("No qualifier") && !CBClientQualifier.Text.Equals(""))
                            {
                                ANewConnector.SetClientRoleQualifier(GetFormatedQualifierFromClientComboBox(true));
                              }
                            else
                             {
                            ANewConnector.SetClientRoleQualifier(""); //am aout 2018
                             }
                            if ((CBClientQualifier.Text.Equals("No qualifier")|| (CBClientQualifier.Text.Equals(""))))
                            {
                                ANewConnector.SetClientRoleQualifier("");
                            }
                            if (!CBSupplierQualifier.Text.Equals("No qualifier") && !CBSupplierQualifier.Text.Equals(""))
                            {
                                ANewConnector.SetSupplierRoleQualifier(GetFormatedQualifierFromClientComboBox(false));
                            }
                            if ((CBSupplierQualifier.Text.Equals("No qualifier") || (CBSupplierQualifier.Text.Equals(""))))
                        {
                                ANewConnector.SetSupplierRoleQualifier("");
                            }
                        }
                        else   // am juil 2016
                        {

                            if (CBAgregateSource.Checked.Equals(false))
                            {
                                //ANewConnector.SetClientContainmentByRef(false);
                                // ANewConnector.SetSupplierContainmentByRef(true);//am mars 2016
                                this.CBByRefSource.Enabled = true; //am juil 2016
                            }
                            if (CBAgregateTarget.Checked.Equals(false))
                            {
                               /// ANewConnector.SetSupplierContainmentByRef(false);
                                //  ANewConnector.SetClientContainmentByRef(true);//am mars 2016
                                this.CBByRefTarget.Enabled = true; //am mars 2016
                            }
                         //   util.wlog("TEST", "updateConnector newACon" + ANewConnector.GetTrace());

                            if ((CBAgregateTarget.Checked.Equals(false) && CBAgregateSource.Checked.Equals(false)))
                            {
                                ANewConnector.SetAgregate("");
                            }
                            if (CBAgregateSource.Checked.Equals(true))
                            {
                                ANewConnector.SetAgregate("TARGET");

                                if (CBByRefTarget.Checked.Equals(true))
                                {
                                    ANewConnector.SetClientContainmentByRef(true);
                                }
                                else
                                {
                                    ANewConnector.SetClientContainmentByRef(false);
                                }

                            }
                            if (CBAgregateTarget.Checked.Equals(true))
                            {
                                ANewConnector.SetAgregate("SOURCE");

                                if (CBByRefSource.Checked.Equals(true))
                                {
                                    ANewConnector.SetSupplierContainmentByRef(true);
                                }
                                else
                                {
                                    ANewConnector.SetSupplierContainmentByRef(false);
                                }

                            }

                            ANewConnector.SetClientRole(TBSupplierRole.Text);
                            ANewConnector.SetSupplierRole(TBClientRole.Text);
                            // if (CBAgregateTarget.Checked.Equals(true)) ANewConnector.SetSupplierRole("");// am mars 2016
                            // if (CBAgregateSource.Checked.Equals(true)) ANewConnector.SetClientRole("");// am mars 2016
                            ANewConnector.SetCopyTagValues(CBCopyTagValues.Checked);
                            ANewConnector.SetTaggedValue(ParentConnector.GetTaggedValue());
                            ANewConnector.SetCopyNotes(CBCopyNotes.Checked);

                            ANewConnector.SetCopyStereotype(CBCopyStereotype.Checked);
                            if ((CBCopyStereotype.Checked.Equals(false)) && (CBStereotype.Items.Contains(CBStereotype.SelectedItem)))
                            {
                                ANewConnector.SetStereotype(CBStereotype.SelectedItem.ToString());
                            }
                            else { ANewConnector.SetStereotype(""); }

                            ANewConnector.SetClientLBCardinality(CBLBSupplierCardinality.Text);
                            ANewConnector.SetClientUBCardinality(CBUBSupplierCardinality.Text);
                            ANewConnector.SetSupplierLBCardinality(CBLBClientCardinality.Text);
                            ANewConnector.SetSupplierUBCardinality(CBUBClientCardinality.Text);
                            if (!CBClientQualifier.Text.Equals("No qualifier") && !CBClientQualifier.Text.Equals(""))
                            {
                                ANewConnector.SetSupplierRoleQualifier(GetFormatedQualifierFromClientComboBox(true));
                        }
                        
                            if ((CBClientQualifier.Text.Equals("No qualifier")|| (CBClientQualifier.Text.Equals(""))))
                            {
                                ANewConnector.SetSupplierRoleQualifier("");
                            }
                            if (!CBSupplierQualifier.Text.Equals("No qualifier") && !CBSupplierQualifier.Text.Equals(""))
                            {
                                ANewConnector.SetClientRoleQualifier(GetFormatedQualifierFromClientComboBox(false));
                            }
                            if ((CBSupplierQualifier.Text.Equals("No qualifier")) || (CBSupplierQualifier.Text.Equals("")))
                            {
                                ANewConnector.SetClientRoleQualifier("");
                            }
                        }
                    //####
                       // util.wlog("TEST", "UpdatedNewConnector Reftarget= " + CBByRefTarget.Checked.ToString() + "  Refsource=" + CBByRefSource.Checked.ToString());//am mars 2016
                       // util.wlog("TEST", "UpdatedNewConnector " + ANewConnector.GetTrace());
                    }
                    
                    #endregion
                }
                if (!(ANewConnector==null))
                {

                    ANewConnector.SetSelectedState(true);
                }
                this.RefreshParentUI();
                this.Dispose();
            //}
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private bool CheckClientCardinality(string LBClientCardinality, string UBClientCardinality)
        {
            if(CBAgregateSource.Checked.Equals(false)){
            if (ParentConnector.GetClientLBCardinality().Equals("") && ParentConnector.GetClientUBCardinality().Equals(""))
            {
                return CheckSimpleClientCardinality(LBClientCardinality, UBClientCardinality);
            }
            else if (ParentConnector.GetClientUBCardinality().Equals(""))
            {
                return CheckSimpleUBClientCardinality(LBClientCardinality, UBClientCardinality);
            }
            else if (ParentConnector.GetClientLBCardinality().Equals(""))
            {
                return CheckSimpleLBClientCardinality(LBClientCardinality, UBClientCardinality);
            }
            else
            {
                return CheckComplexClientCardinality(LBClientCardinality, UBClientCardinality);
            }
            }
            return true;
        }
        //Both parent equal ""
        private bool CheckSimpleClientCardinality(string LBClientCardinality, string UBClientCardinality)
        {
            if (LBClientCardinality.Equals("*"))
            {
                MessageBox.Show("A lowerbound cardinality can't be equal to *", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (LBClientCardinality.Equals("") && UBClientCardinality.Equals(""))
            {
                return true;
            }


            if (UBClientCardinality.Equals("*"))
            {
                try
                {
                    int.Parse(LBClientCardinality);
                }
                catch
                {
                    MessageBox.Show("The lowerbound client cardinality doesn't seem correct.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                if (UBClientCardinality.Equals(""))
                {

                }
                else
                {
                    try
                    {
                        int.Parse(LBClientCardinality);
                    }
                    catch
                    {
                        MessageBox.Show("The lowerbound client cardinality doesn't seem correct.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }


                    try
                    {
                        int.Parse(UBClientCardinality);
                    }
                    catch
                    {
                        MessageBox.Show("The upperbound client cardinality doesn't seem correct.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }


                    if (int.Parse(LBClientCardinality) > int.Parse(UBClientCardinality))
                    {
                        MessageBox.Show("LowerBound cardinality can't be higher than the upperbound.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }
            return true;
        }
        //parent UB  equal ""
        private bool CheckSimpleUBClientCardinality(string LBClientCardinality, string UBClientCardinality)
        {

            if (!UBClientCardinality.Equals(""))
            {
                MessageBox.Show("Client's upper bound must be empty.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                if (ParentConnector.GetClientLBCardinality().Equals("*"))
                {

                    if (LBClientCardinality.Equals("*"))
                    {
                        return true;
                    }
                    else
                    {
                        try
                        {
                            int.Parse(LBClientCardinality);
                        }
                        catch
                        {
                            MessageBox.Show("Lower bound client cardinality doesn't seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }
                else
                {
                    try
                    {
                        int.Parse(LBClientCardinality);
                    }
                    catch
                    {
                        MessageBox.Show("Lower bound client cardinality doesn't seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    if (int.Parse(ParentConnector.GetClientLBCardinality()) < int.Parse(LBClientCardinality))
                    {
                        MessageBox.Show("The set number of the cardinality seem to be higher than his parent's cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }
            return true;
        }
        //parent LB equal ""
        private bool CheckSimpleLBClientCardinality(string LBClientCardinality, string UBClientCardinality)
        {
            MessageBox.Show("The parent's LowerBound cardinality shouldn't be equal to null.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return true;
        }
        //no parent card equal ""
        private bool CheckComplexClientCardinality(string LBClientCardinality, string UBClientCardinality)
        {
            bool SkipGlobalCheck = false;

            #region LBClientCardinality


            if (LBClientCardinality.Equals(""))
            {
                MessageBox.Show("A lowerlound cardinality can't be equal to null.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (LBClientCardinality.Equals("*"))
            {
                MessageBox.Show("A lowerbound cardinality can't be equal to *", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                try
                {
                    int.Parse(LBClientCardinality);
                    if (int.Parse(LBClientCardinality) < int.Parse(ParentConnector.GetClientLBCardinality()))
                    {
                        MessageBox.Show("Value entered for the lowerbound client cardinality must be higher than the parent cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                }
                catch
                {
                    MessageBox.Show("Value entered for the lowerbound client cardinality does'nt seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            #endregion

            #region UBClientCardinality

            if (UBClientCardinality.Equals("*"))
            {
                if (!ParentConnector.GetClientUBCardinality().Equals("*"))
                {
                    MessageBox.Show("Value entered for the upperbound client cardinality can't be higher than the parent cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                else
                {
                    SkipGlobalCheck = true;
                }
            }
            else
            {
                try
                {
                    int.Parse(UBClientCardinality);
                    if (!ParentConnector.GetClientUBCardinality().Equals("*"))
                    {
                        if (int.Parse(UBClientCardinality) > int.Parse(ParentConnector.GetClientUBCardinality()))
                        {
                            MessageBox.Show("Value entered for the upperbound client cardinality must be lower than the parent cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Value entered for the upperbound client cardinality does'nt seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            #endregion



            if (SkipGlobalCheck.Equals(false))
            {
                #region GlobalCheck
                try
                {
                    if (int.Parse(LBClientCardinality) > int.Parse(UBClientCardinality))
                    {
                        MessageBox.Show("Value entered for the client cardinality does'nt seem valid (Lowerbound must be inferior to the uppperbound).", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                catch { return false; }
                #endregion
            }
            return true;
        }

        private bool CheckSupplierCardinality(string LBSupplierCardinality, string UBSupplierCardinality)
        {
            if(CBAgregateTarget.Checked.Equals(false)){
            if (ParentConnector.GetSupplierLBCardinality().Equals("") && ParentConnector.GetSupplierUBCardinality().Equals(""))
            {
                return CheckSimpleSupplierCardinality(LBSupplierCardinality, UBSupplierCardinality);
            }
            else if (ParentConnector.GetSupplierUBCardinality().Equals(""))
            {
                return CheckSimpleUBSupplierCardinality(LBSupplierCardinality, UBSupplierCardinality);
            }
            else if (ParentConnector.GetSupplierLBCardinality().Equals(""))
            {
                return CheckSimpleLBSupplierCardinality(LBSupplierCardinality, UBSupplierCardinality);
            }
            else
            {
                return CheckComplexSupplierCardinality(LBSupplierCardinality, UBSupplierCardinality);
            }
            }
            return true;
        }
        //Both parent equal ""
        private bool CheckSimpleSupplierCardinality(string LBSupplierCardinality, string UBSupplierCardinality)
        {
            if (LBSupplierCardinality.Equals("*"))
            {
                MessageBox.Show("A lowerbound cardinality can't be equal to *", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (LBSupplierCardinality.Equals("") && UBSupplierCardinality.Equals(""))
            {
                return true;
            }


            if (UBSupplierCardinality.Equals("*"))
            {
                try
                {
                    int.Parse(LBSupplierCardinality);
                }
                catch
                {
                    MessageBox.Show("The lowerbound Supplier cardinality doesn't seem correct.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                if (UBSupplierCardinality.Equals(""))
                {

                }
                else
                {
                    try
                    {
                        int.Parse(LBSupplierCardinality);
                    }
                    catch
                    {
                        MessageBox.Show("The lowerbound Supplier cardinality doesn't seem correct.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }


                    try
                    {
                        int.Parse(UBSupplierCardinality);
                    }
                    catch
                    {
                        MessageBox.Show("The upperbound Supplier cardinality doesn't seem correct.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }


                    if (int.Parse(LBSupplierCardinality) > int.Parse(UBSupplierCardinality))
                    {
                        MessageBox.Show("Supplier's lowerbound cardinality can't be higher than the upperbound.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

            }
            return true;
        }
        //parent UB  equal ""
        private bool CheckSimpleUBSupplierCardinality(string LBSupplierCardinality, string UBSupplierCardinality)
        {

            if (!UBSupplierCardinality.Equals(""))
            {
                MessageBox.Show("Supplier's upper bound must be empty.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                if (ParentConnector.GetSupplierLBCardinality().Equals("*"))
                {

                    if (LBSupplierCardinality.Equals("*"))
                    {
                        return true;
                    }
                    else
                    {
                        try
                        {
                            int.Parse(LBSupplierCardinality);
                        }
                        catch
                        {
                            MessageBox.Show("Lower bound Supplier cardinality doesn't seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }
                else
                {
                    try
                    {
                        int.Parse(LBSupplierCardinality);
                    }
                    catch
                    {
                        MessageBox.Show("Lower bound supplier cardinality doesn't seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    if (int.Parse(ParentConnector.GetSupplierLBCardinality()) < int.Parse(LBSupplierCardinality))
                    {
                        MessageBox.Show("The set number of the cardinality seem to be higher than his parent's cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }
            return true;
        }
        //parent LB equal ""
        private bool CheckSimpleLBSupplierCardinality(string LBSupplierCardinality, string UBSupplierCardinality)
        {
            MessageBox.Show("The parent's LowerBound cardinality shouldn't be equal to null.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return true;
        }
        //no ""
        private bool CheckComplexSupplierCardinality(string LBSupplierCardinality, string UBSupplierCardinality)
        {

            bool SkipGlobalCheck = false;

            #region LBSupplierCardinality


            if (LBSupplierCardinality.Equals(""))
            {
                MessageBox.Show("A lower bound cardinality can't be equal to null.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (LBSupplierCardinality.Equals("*"))
            {
                MessageBox.Show("A lowerbound cardinality can't be equal to *", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                try
                {
                    int.Parse(LBSupplierCardinality);
                    if (int.Parse(LBSupplierCardinality) < int.Parse(ParentConnector.GetSupplierLBCardinality()))
                    {
                        MessageBox.Show("Value entered for the lowerbound supplier cardinality must be higher than the parent cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                }
                catch
                {
                    MessageBox.Show("Value entered for the lower bound supplier cardinality does'nt seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            #endregion

            #region UBSupplierCardinality

            if (UBSupplierCardinality.Equals("*"))
            {
                if (!ParentConnector.GetSupplierUBCardinality().Equals("*"))
                {
                    MessageBox.Show("Value entered for the upper bound supplier cardinality can't be higher than the parent cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                else
                {
                    SkipGlobalCheck = true;
                }
            }
            else
            {
                try
                {
                    int.Parse(UBSupplierCardinality);
                    if (!ParentConnector.GetSupplierUBCardinality().Equals("*"))
                    {
                        if (int.Parse(UBSupplierCardinality) > int.Parse(ParentConnector.GetSupplierUBCardinality()))
                        {
                            MessageBox.Show("Value entered for the upper bound supplier cardinality must be lower than the parent cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Value entered for the upper bound supplier cardinality does'nt seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            #endregion



            if (SkipGlobalCheck.Equals(false))
            {
                #region GlobalCheck
                try
                {
                    if (int.Parse(LBSupplierCardinality) > int.Parse(UBSupplierCardinality))
                    {
                        MessageBox.Show("Value entered for the supplier cardinality does'nt seem valid (Lowerbound must be inferior to the uppperbound).", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                catch { return false; }
                #endregion
            }
            return true;
        }




      


      
  
        private void CBCopyStereotype_CheckedChanged(object sender, EventArgs e)
        {
            if (CBCopyStereotype.Checked.Equals(true))
            {
                CBStereotype.SelectedItem = "No Stereotype";
                CBStereotype.Enabled = false;
            }
            else if (CBCopyStereotype.Checked.Equals(false))
            {
                CBStereotype.SelectedItem = "No Stereotype";
                CBStereotype.Enabled = true;
            }
        }
        private void CBAgregateSource_CheckedChanged(object sender, EventArgs e)
        {
            if (CBAgregateSource.Checked.Equals(true))
            {
                CBLBClientCardinality.Text = "";
                CBUBClientCardinality.Text = "";
                CBLBSupplierCardinality.Text = ParentConnector.GetSupplierLBCardinality();//am mars 2016
                CBUBSupplierCardinality.Text = ParentConnector.GetSupplierUBCardinality();//am mars 2016
                CBByRefTarget.Enabled = true;
                CBByRefSource.Enabled = false;
                CBAgregateTarget.Enabled = false; //am mars 2016
                CBAgregateTarget.Checked = false;//am mars 2016

                if (// am aout 2018
                    (XMLP.GetXmlValueConfig("AutomaticChangeOfRoleName") == ConfigurationManager.CHECKED)  &&
                        (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Count > 0)  &&
                        (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Contains(ParentConnector.GetSelectedElementConnector().ElementID)))
                {//if the selected element inherits
                    if (ParentConnector.GetClientRole().Contains("_"))//am dec 2019
                    {
                        string[] split = ParentConnector.GetClientRole().Split(new char[] { '_' }); //am dec 2019
                        TBClientRole.Text = split[0] + "_" + DCF.GetSelectedItem().Name;//am dec 2019
                    }
                    else
                    {
                        TBClientRole.Text = ParentConnector.GetTargetedIBOElement().Name;// DCF.GetSelectedItem().Name;// we chose the name of the element as role
                    }
                }
                else
                {
                    TBClientRole.Text = this.EditedConnector.GetClientRole();//ABA 20230204 ParentConnector.GetClientRole();
                }

                EA.Element target = Main.Repo.GetElementByGuid(util.getEltParentGuid(ParentConnector.GetTargetedIBOElement()));
                if ((XMLP.GetXmlValueConfig("AutomaticChangeOfRoleName") == ConfigurationManager.CHECKED) &&
                        (utilitaires.Utilitaires.dicAncestors[target.ElementID].Count > 0) &&
                        (utilitaires.Utilitaires.dicAncestors[target.ElementID].Contains(ParentConnector.GetTargetedElementConnector().ElementID)))
                    {//if the selected element inherits
                    if (ParentConnector.GetSupplierRole().Contains("_"))//am dec 2019
                    { 
                        string[] split = ParentConnector.GetSupplierRole().Split(new char[] { '_' }); //am dec 2019
                        TBSupplierRole.Text = split[0] + "_" + ParentConnector.GetTargetedIBOElement().Name;//am dec 2019
                    }
                    else
                    {
                        TBSupplierRole.Text = ParentConnector.GetTargetedIBOElement().Name;// we chose the name of the element as role
                    }
                }
                else
                {
                    TBSupplierRole.Text = this.EditedConnector.GetSupplierRole();//ABA 20230204 ParentConnector.GetSupplierRole();
                }
            }
            else if (CBAgregateSource.Checked.Equals(false))
            {
                CBLBClientCardinality.Text = ParentConnector.GetClientLBCardinality();
                CBUBClientCardinality.Text = ParentConnector.GetClientUBCardinality();
                CBByRefTarget.Enabled = false;
                CBByRefSource.Enabled = true;//am juil 2016
                CBByRefTarget.Checked = false;
                CBAgregateTarget.Enabled = true;
                CBAgregateTarget.Enabled = true;
                // TBClientRole.Text = ParentConnector.GetClientRole(); //am aout 2018
                //TBSupplierRole.Text = ParentConnector.GetSupplierRole();//am aout 2018
   
                    if (// am aout 2018
                   (XMLP.GetXmlValueConfig("AutomaticChangeOfRoleName") == ConfigurationManager.CHECKED)
                   &&
                    (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Count > 0)
                    &&
                    (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Contains(ParentConnector.GetSelectedElementConnector().ElementID))
                    )
                    {//if the selected element inherits
                         if (ParentConnector.GetClientRole().Contains("_"))//am dec 2019
                         { //am dec 2019
                            string[] split = ParentConnector.GetClientRole().Split(new char[] { '_' }); //am dec 2019
                            TBClientRole.Text = split[0] + "_" + DCF.GetSelectedItem().Name;//am dec 2019
                         }//am dec 2019
                         else//am dec 2019
                         {//am dec 2019
                            TBClientRole.Text = DCF.GetSelectedItem().Name;// we chose the name of the element as role
                         }//am dec 2019
                    //19TBClientRole.Text = DCF.GetSelectedItem().Name;// we chose the name of the element as role
                    }
                    else
                    {
                        TBClientRole.Text = ParentConnector.GetClientRole();
                    }

                    EA.Element target = Main.Repo.GetElementByGuid(util.getEltParentGuid(ParentConnector.GetTargetedIBOElement()));

                    if (// am aout 2018
                        (utilitaires.Utilitaires.dicAncestors[target.ElementID].Count > 0)
                         &&
                        (utilitaires.Utilitaires.dicAncestors[target.ElementID].Contains(ParentConnector.GetTargetedElementConnector().ElementID)) // am aout 2018
                        )
                    {//if the selected element inherits
                        TBSupplierRole.Text = ParentConnector.GetTargetedIBOElement().Name;// we chose the name of the element as role
                    }
                    else
                    {
                        TBSupplierRole.Text = ParentConnector.GetSupplierRole();
                    }
                
            }         //CBByRefTarget.Checked = false;   

        }

        private void CBAgregateTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (CBAgregateTarget.Checked.Equals(true))
            {
                CBLBSupplierCardinality.Text = "";
                CBUBSupplierCardinality.Text = "";
                CBLBClientCardinality.Text = ParentConnector.GetClientLBCardinality();//am mars 2016
                CBUBClientCardinality.Text = ParentConnector.GetClientUBCardinality(); //am mars 2016
                CBByRefSource.Enabled = true;
                CBByRefTarget.Checked = false;
                CBAgregateSource.Enabled = false; //am mars 2016
                CBAgregateSource.Checked = false; //am mars 2016
                                                  // TBClientRole.Text = ParentConnector.GetClientRole(); //am aout 2018
                                                  // TBSupplierRole.Text = ParentConnector.GetSupplierRole();//am aout 2018
          
                    if (// am aout 2018
                        (XMLP.GetXmlValueConfig("AutomaticChangeOfRoleName") == ConfigurationManager.CHECKED)
                        &&
                     (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Count > 0)
                     &&
                     (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Contains(ParentConnector.GetSelectedElementConnector().ElementID))
                     )
                    {//if the selected element inherits
                      if (ParentConnector.GetClientRole().Contains("_"))//am dec 2019
                      { //am dec 2019
                           string[] split = ParentConnector.GetClientRole().Split(new char[] { '_' }); //am dec 2019
                         TBClientRole.Text = split[0] + "_" + DCF.GetSelectedItem().Name;//am dec 2019
                      }//am dec 2019
                      else//am dec 2019
                       {//am dec 2019
                          TBClientRole.Text = DCF.GetSelectedItem().Name;// we chose the name of the element as role
                       }//am dec 2019
                    //19TBClientRole.Text = DCF.GetSelectedItem().Name;// we chose the name of the element as role
                    }
                    else
                    {
                        TBClientRole.Text = ParentConnector.GetClientRole();
                    }

                    EA.Element target = Main.Repo.GetElementByGuid(util.getEltParentGuid(ParentConnector.GetTargetedIBOElement()));

                    if (// am aout 2018
                        (utilitaires.Utilitaires.dicAncestors[target.ElementID].Count > 0)
                         &&
                        (utilitaires.Utilitaires.dicAncestors[target.ElementID].Contains(ParentConnector.GetTargetedElementConnector().ElementID)) // am aout 2018
                        )
                    {//if the selected element inherits
                        TBSupplierRole.Text = ParentConnector.GetTargetedIBOElement().Name;// we chose the name of the element as role
                    }
                    else
                    {
                        TBSupplierRole.Text = ParentConnector.GetSupplierRole();
                    }
                
            }
            else if (CBAgregateTarget.Checked.Equals(false))
            {
                CBLBSupplierCardinality.Text = ParentConnector.GetSupplierLBCardinality();
                CBUBSupplierCardinality.Text = ParentConnector.GetSupplierUBCardinality();
                CBByRefSource.Enabled = false;
                // CBByRefTarget.Enabled = false;
                CBByRefTarget.Enabled = false; // am juil 2016
                CBByRefSource.Checked = false;
                CBAgregateSource.Enabled = true; //am mars 2016
                CBAgregateTarget.Enabled = true; //am mars 2016

                //  TBSupplierRole.Text = ParentConnector.GetSupplierRole(); //am aout 2018
                //  TBClientRole.Text = ParentConnector.GetClientRole();//am aout 2018
 
                    if (// am aout 2018
                     (XMLP.GetXmlValueConfig("AutomaticChangeOfRoleName") == ConfigurationManager.CHECKED)
                        &&
                    (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Count > 0)
                    &&
                    (utilitaires.Utilitaires.dicAncestors[DCF.GetSelectedItem().ElementID].Contains(ParentConnector.GetSelectedElementConnector().ElementID))
                    )
                    {//if the selected element inherits
                        if (ParentConnector.GetClientRole().Contains("_"))//am dec 2019
                        { //am dec 2019
                             string[] split = ParentConnector.GetClientRole().Split(new char[] { '_' }); //am dec 2019
                             TBClientRole.Text = split[0] + "_" + DCF.GetSelectedItem().Name;//am dec 2019
                        }//am dec 2019
                        else//am dec 2019
                        {//am dec 2019
                        TBClientRole.Text = DCF.GetSelectedItem().Name;// we chose the name of the element as role
                        }//am dec 2019
                   //19 TBClientRole.Text = DCF.GetSelectedItem().Name;// we chose the name of the element as role
                    }
                    else
                    {
                        TBClientRole.Text = ParentConnector.GetClientRole();
                    }

                    EA.Element target = Main.Repo.GetElementByGuid(util.getEltParentGuid(ParentConnector.GetTargetedIBOElement()));

                    if (// am aout 2018
                        (utilitaires.Utilitaires.dicAncestors[target.ElementID].Count > 0)
                         &&
                        (utilitaires.Utilitaires.dicAncestors[target.ElementID].Contains(ParentConnector.GetTargetedElementConnector().ElementID)) // am aout 2018
                        )
                    {//if the selected element inherits
                        TBSupplierRole.Text = ParentConnector.GetTargetedIBOElement().Name;// we chose the name of the element as role
                    }
                    else
                    {
                        TBSupplierRole.Text = ParentConnector.GetSupplierRole();
                    }
                
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void CBByRefSource_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CBByRefSource.Checked == true)
            {
               
                this.CBByRefTarget.Checked = false;  
            }
            else
            {
                if (!CBAgregateTarget.Checked)
                {
                    this.CBByRefSource.Enabled = false;
                    CBLBSupplierCardinality.Text = ParentConnector.GetSupplierLBCardinality();
                    CBUBSupplierCardinality.Text = ParentConnector.GetSupplierUBCardinality();
                }
                else
                {
                    this.CBByRefSource.Enabled = true;
                }
                
            }
           // CBLBClientCardinality.Text = ParentConnector.GetClientLBCardinality();
          //  CBUBClientCardinality.Text = ParentConnector.GetClientUBCardinality();
           
        }

        private void CBByRefTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CBByRefTarget.Checked == true)
            {
             
                this.CBByRefSource.Checked = false;
                
            } else {
                if (!CBAgregateSource.Checked)
                {
                    this.CBByRefTarget.Enabled = false;
                    CBLBClientCardinality.Text = ParentConnector.GetClientLBCardinality();
                    CBUBClientCardinality.Text = ParentConnector.GetClientUBCardinality();
                }
                else
                {
                    this.CBByRefTarget.Enabled = true;
                }
              
            }
            
          //  CBLBSupplierCardinality.Text = ParentConnector.GetSupplierLBCardinality();
          //  CBUBSupplierCardinality.Text = ParentConnector.GetSupplierUBCardinality();
        }

        private void BUTManageQualifier_Click(object sender, EventArgs e)
        {
            QualifierManagerForm qf = new QualifierManagerForm(this.CBSupplierQualifier, this.CBClientQualifier, this.Repo);
            qf.ShowDialog();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Client">if false, mean it's the supplierCb that must be used</param>
        /// <returns></returns>
        private string GetFormatedQualifierFromClientComboBox(bool Client)
        {
            if (Client.Equals(true))
            {
              //  if (!(CBQualifier == null))
               // {
                    return (CBClientQualifier.Text.Substring(0, 1).ToUpper() + CBClientQualifier.Text.Remove(0, 1)).Replace(" ", "").Replace("_", "");
               // }
              //  else
              //  {
              //      return "";
              //  }
            }
            else
            {
              //  if (!(CBQualifier == null))
               // {
                    return (CBSupplierQualifier.Text.Substring(0, 1).ToUpper() + CBSupplierQualifier.Text.Remove(0, 1)).Replace(" ", "").Replace("_", "");
              //  }
               // else
               // {
                  //  return "";
               // }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void BUTConstraint_Click(object sender, EventArgs e)
        {
            ConstraintManagerForm cmf = new ConstraintManagerForm(this.TBConstraints, this.Repo);
            cmf.ShowDialog();
        }

        private void CBConstraints_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CBConstaints_CheckedChanged(object sender, EventArgs e)
        {

        }
   
    }
}
