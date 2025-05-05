using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace CimContextor.EditConnectors
{
    public partial class  EditSubdivideForRdfsConnectorForm : Form
    {
        private EditEAClassConnector ParentConnector;
        private int SubConnectorGUID;
        private bool CreateNewConnector;
        private EditEAClassConnector NewEditedConnector;//am juin 2016
        private EditDuplicateForRdfsConnectorsForm DCF;
        private ConstantDefinition CD = new ConstantDefinition();
        private utilitaires.Utilitaires util = new utilitaires.Utilitaires();
        public  EditSubdivideForRdfsConnectorForm(EditDuplicateForRdfsConnectorsForm DCF, bool CreateNewConnector, EditEAClassConnector ParentConnector, int SubConnectorGUID)
        {
            XMLParser XMLP = new XMLParser();
            XMLP.SetXmlValueConfig("NavigationEnabled", "Checked");
            #region Contructor
            InitializeComponent();
            this.DCF = DCF;
            this.ParentConnector = ParentConnector;
            this.SubConnectorGUID = SubConnectorGUID;
            this.CreateNewConnector = CreateNewConnector;

            EA.Repository Repo = DCF.GetRepository();
            foreach (EA.Stereotype AStereotype in Repo.Stereotypes)
            {
                if (AStereotype.AppliesTo.ToLower().Equals(ParentConnector.GetType().ToLower()))
                {
                    CBStereotype.Items.Add(AStereotype.Name);
                }
            }
            CBStereotype.Items.Add("No Stereotype");

            //XMLParser XMLP = new XMLParser();
            ArrayList QualifierList = new ArrayList();
            QualifierList = XMLP.GetXmlQualifier("role");
            QualifierList.Add("No qualifier");

            foreach (object aQualifier in QualifierList)
            {
                CBClientQualifier.Items.Add((string)aQualifier);
                CBSupplierQualifier.Items.Add((string)aQualifier);
               // CBQualifier.Items.Add((string)aQualifier);//am  mars 2016
            }
            // CBQualifier.SelectedItem = "No qualifier"; //am  mars 2016
            CBClientQualifier.SelectedItem = "No qualifier";
            CBSupplierQualifier.SelectedItem = "No qualifier";
            NewEditedConnector = null; // am juin 2016
            if (CreateNewConnector.Equals(true))
            {
                #region NewSubConnector
                #region FirstWay
                if (DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                {
                    util.wlog("TEST", "EditSubdividedRdfsConnector  entree dans NewSubConnector Firstway  ParentConnector.switch="+ ParentConnector.GetSwitch().ToString() );
                    this.groupBox2.Text = DCF.GetSelectedItem().Name ; // am mars 2016
                    this.groupBox3.Text = ParentConnector.GetTargetedIBOElement().Name ; // am mars 2016
                    this.CBAgregateSource.Enabled = false; //am mars 2016 avril 2016
                     this.CBAgregateTarget.Enabled = false; //am mars 2016 avril 2016 
                     this.CBAgregateSource.Visible = false; //am mars 2016 avril 2016
                     this.CBAgregateTarget.Visible = false; //am mars 2016 avril 2016
                     this.LSContainment.Visible = false;
                     this.LTContainment.Visible = false;
                    string PcCientRole=util.RemoveQual(this.ParentConnector.GetClientRole());
                    string PcSupplierRole= util.RemoveQual(this.ParentConnector.GetSupplierRole());
                    string OcClientRole=util.RemoveQual(ParentConnector.GetOriginalConnector().ClientEnd.Role);
                    long PcClientAggreg=ParentConnector.GetOriginalConnector().ClientEnd.Aggregation;
                    string OcSupplierRole = util.RemoveQual(ParentConnector.GetOriginalConnector().SupplierEnd.Role);
                    long PcSupplierAggreg = ParentConnector.GetOriginalConnector().SupplierEnd.Aggregation;


                    if ((PcClientAggreg == 1) || (PcSupplierAggreg == 1))
                    {
                        if (PcClientAggreg == 1)
                        {
                            if(PcCientRole==OcClientRole)
                            {
                                this.LSContainment.Visible = true;
                                this.LTContainment.Visible = false;
                            }
                            else
                            {
                                this.LSContainment.Visible = false;
                                this.LTContainment.Visible = true;
                            }
                        }
                        if (PcSupplierAggreg == 1)
                        {
                            if (PcCientRole == OcClientRole)
                            {
                                this.LSContainment.Visible = false;
                                this.LTContainment.Visible = true;
                            }
                            else
                            {
                                this.LSContainment.Visible = true;
                                this.LTContainment.Visible = false;
                            }
                        }
                    }



                    this.CBByRefSource.Text = "Is Direction End"; // am mars 2016 
                    this.CBByRefTarget.Text = "Is Direction End"; // am mars 2016
                  //  this.Text = "Subdividing from " + ParentConnector.GetTargetedIBOElement().Name + " to " + ParentConnector.GetTargetedIBOElement().Name;
                   // this.Text = "Editing Association from " + DCF.GetSelectedItem().Name + " to " + ParentConnector.GetTargetedIBOElement().Name; // am mars 2016
                    this.Text = "Editing Association" + DCF.GetSelectedItem().Name + " - " + ParentConnector.GetTargetedIBOElement().Name; // am juil 2016
                    // LabSupplierRole.Text = "Parent's role : " + ParentConnector.GetSupplierRole();
                    LabSupplierRole.Text = "Parent's role : " + ParentConnector.GetSupplierRole(); // am mars 2016
                    LabClientRole.Text = "Parent's role : " + ParentConnector.GetClientRole();
                    TBClientRole.Text = ParentConnector.GetClientRole();
                    TBSupplierRole.Text = ParentConnector.GetSupplierRole();


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
                if (DCF.GetSelectedItem().ElementGUID.Equals(this.ParentConnector.GetSelectedElementConnector().ElementGUID))
                {
                    util.wlog("TEST", "EditSubdividedRdfsConnector si le selecteditem est les meme que celui du parent  ParentConnector.switch="+ ParentConnector.GetSwitch().ToString() );
                    //this.Text = "Editing a subdivision from " + ParentConnector.GetSelectedIBOElement().Name + " to " + ParentConnector.GetTargetedIBOElement().Name;
                    EditEAClassConnector EditedConnector = ParentConnector.GetSubConnector(SubConnectorGUID);//am avrif 2016
                    NewEditedConnector = EditedConnector; // am juin 2016
                  //this.groupBox2.Text = EditedConnector.GetSelectedIBOElement().Name; // DCF.GetSelectedItem().Name; // am mars 2016
                   // this.groupBox3.Text = EditedConnector.GetTargetedIBOElement().Name; //ParentConnector.GetTargetedIBOElement().Name; // am mars 2016
                    this.groupBox2.Text = "";
                    this.groupBox3.Text = "";
                    this.CBAgregateSource.Enabled = false; //am mars 2016
                    this.CBAgregateTarget.Enabled = false; //am mars 2016
                    this.CBAgregateSource.Visible = false; //am mars 2016 avril 2016
                    this.CBAgregateTarget.Visible = false; //am mars 2016 avril 2016
                    // this.CBAgregateSource.Visible = false; //am mars 2016 avril 2016
                    //this.CBAgregateTarget.Visible = false; //am mars 2016 avril 2016
                    this.CBByRefSource.Text = "Is Direction End"; // am mars 2016 
                     this.CBByRefTarget.Text = "Is Direction End"; // am mars 2016  

                    //  this.Text = "Subdividing from " + ParentConnector.GetTargetedIBOElement().Name + " to " + ParentConnector.GetTargetedIBOElement().Name;
                    //this.Text = "Editing Association from " + DCF.GetSelectedItem().Name + " to " + ParentConnector.GetTargetedIBOElement().Name; // am mars 2016
                    this.Text = "Editing Association  " + DCF.GetSelectedItem().Name + " - " + ParentConnector.GetTargetedIBOElement().Name; // am juil 2016
                    LabSupplierRole.Text = "Parent's role : " + EditedConnector.GetSupplierRole(); //ParentConnector.GetSupplierRole(); //am avril 2016 
                    LabClientRole.Text = "Parent's role : " + EditedConnector.GetClientRole();  //ParentConnector.GetClientRole();  //am avril 2016

                   // EditEAClassConnector EditedConnector = ParentConnector.GetSubConnector(SubConnectorGUID);
                    TBClientRole.Text = EditedConnector.GetClientRole();
                    TBSupplierRole.Text = EditedConnector.GetSupplierRole();

                    CBCopyStereotype.Checked = EditedConnector.GetCopyStereotype();
                    if ((CBCopyStereotype.Checked.Equals(false)) && (CBStereotype.Items.Contains(EditedConnector.GetStereotype())))
                    {
                        CBStereotype.SelectedItem = EditedConnector.GetStereotype();
                    }


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
                        CBSupplierQualifier.Items.Add(EditedConnector.GetSupplierRoleQualifier()) ;
                        CBClientQualifier.Items.Add(EditedConnector.GetSupplierRoleQualifier());
                        CBSupplierQualifier.SelectedItem = EditedConnector.GetSupplierRoleQualifier();
                    }

                    string texte = "EditSubdividedRdfsConnector update EditedConnector.GetAgregate()={0} editedcon.aggreg={1} editedcon.aggreg={2}";
                    util.wlog("TEST",string.Format(texte, EditedConnector.GetAgregate(),EditedConnector.GetEditedConnector().ClientEnd.Aggregation,EditedConnector.GetEditedConnector().SupplierEnd.Aggregation));
                    if (EditedConnector.GetAgregate().Equals("SOURCE"))
                    {
                       
                        if(EditedConnector.GetEditedConnector().ClientEnd.Aggregation==1)
                        {
                            LSContainment.Visible = true;
                            LTContainment.Visible = false;
                        }
                        else
                        {
                            if (EditedConnector.GetEditedConnector().SupplierEnd.Aggregation == 1)
                            {
                                LSContainment.Visible = false;
                                LTContainment.Visible = true;
                            }
                            else
                            {
                                LSContainment.Visible = false;
                                LTContainment.Visible = false;
                            }
                        }
                      // CBAgregateSource.Checked = true;
                       CBByRefTarget.Enabled = true;
                       CBByRefTarget.Checked = true;
                    }
                    else if (EditedConnector.GetAgregate().Equals("TARGET"))
                    {
                        if (EditedConnector.GetEditedConnector().ClientEnd.Aggregation == 1)
                        {
                            LSContainment.Visible = true;
                            LTContainment.Visible = false;
                        }
                        else
                        {
                            if (EditedConnector.GetEditedConnector().SupplierEnd.Aggregation == 1)
                            {
                                LSContainment.Visible = false;
                                LTContainment.Visible = true;
                            }
                            else
                            {
                                LSContainment.Visible = false;
                                LTContainment.Visible =  false;
                            }
                        }
                        //CBAgregateTarget.Checked = true;
                        CBByRefSource.Enabled = true;
                        CBByRefSource.Checked = true;
                    }

                    util.wlog("TEST", "ESdRdfsConnector EditedConnector.ClientEnd=" + EditedConnector.GetEditedConnector().ClientEnd.Aggregation);
                    util.wlog("TEST", string.Format("ESdRdfsC LSContainment.Visible={0}  LTContainment.Visible={1} CBByRefSource.Enabled= {2}  CBByRefSource.Checked ={3} CBByRefTarget.Enabled={4}  CBByRefTarget.Checked={5}", LSContainment.Visible, LTContainment.Visible, CBByRefSource.Enabled, CBByRefSource.Checked, CBByRefTarget.Enabled, CBByRefTarget.Checked));
                 
                    /** am avril 2016 
                    if (EditedConnector.GetSupplierContainmentByRef().Equals(true))
                    {
                        CBByRefTarget.Checked = true;
                    }
                    else if (EditedConnector.GetClientContainmentByRef().Equals(true))
                    {
                        CBByRefSource.Checked = true;
                    }
                    */

                    CBCopyNotes.Checked = EditedConnector.GetCopyNotes();
                    CBCopyTagValues.Checked = EditedConnector.GetCopyTagValues();


                    this.CBLBClientCardinality.Items.Add(EditedConnector.GetClientLBCardinality());
                    this.CBLBClientCardinality.SelectedItem = EditedConnector.GetClientLBCardinality();
                    this.CBUBClientCardinality.Items.Add(EditedConnector.GetClientUBCardinality());
                    this.CBUBClientCardinality.SelectedItem = EditedConnector.GetClientUBCardinality();

                    this.CBLBSupplierCardinality.Items.Add(EditedConnector.GetSupplierLBCardinality());
                    this.CBLBSupplierCardinality.SelectedItem = EditedConnector.GetSupplierLBCardinality();
                    this.CBUBSupplierCardinality.Items.Add(EditedConnector.GetSupplierUBCardinality());
                    this.CBUBSupplierCardinality.SelectedItem = EditedConnector.GetSupplierUBCardinality();
                }
                else
                {
                    util.wlog("TEST", "EditSubdividedRdfsConnector  editing subconnector with  EditedConnector selectedElement different that the parent selectedElementParentConnector.switch=" + ParentConnector.GetSwitch().ToString());
                    EditEAClassConnector EditedConnector = ParentConnector.GetSubConnector(SubConnectorGUID); //am avril 2016
                    //this.Text = "Editing a subdivision from " + ParentConnector.GetTargetedIBOElement().Name + " to " + ParentConnector.GetSelectedIBOElement().Name;
                    this.Text = "Editing a subdivision from " + EditedConnector.GetTargetedIBOElement().Name + " to " + EditedConnector.GetSelectedIBOElement().Name;
                    LabSupplierRole.Text = "Parent's role : " + EditedConnector.GetClientRole();//ParentConnector.GetClientRole();
                    LabClientRole.Text = "Parent's role : " + EditedConnector.GetSupplierRole(); //ParentConnector.GetSupplierRole();
                    NewEditedConnector = EditedConnector; // am juin 2016
                    this.groupBox2.Text = EditedConnector.GetSelectedIBOElement().Name; // DCF.GetSelectedItem().Name; // am mars 2016
                    this.groupBox3.Text = EditedConnector.GetTargetedIBOElement().Name; //ParentConnector.GetTargetedIBOElement().Name; // am mars 2016
                    this.CBAgregateSource.Visible = false; //am mars 2016 avril 2016
                    this.CBAgregateTarget.Visible = false; //am mars 2016 avril 2016
                    this.CBAgregateSource.Enabled = false; //am mars 2016 avril 2016
                    this.CBAgregateTarget.Enabled = false; //am mars 2016 avril 2016
                    //EditEAClassConnector EditedConnector = ParentConnector.GetSubConnector(SubConnectorGUID); //am avril 2016
                    TBClientRole.Text = EditedConnector.GetSupplierRole();
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
                    util.wlog("TEST", "EditSubdividedRdfsConnector  editing subconnector EditedConnector.GetAgregate()=" + EditedConnector.GetAgregate());
                    if (EditedConnector.GetAgregate().Equals("TARGET"))
                    {                     
                        if (EditedConnector.GetEditedConnector().ClientEnd.Aggregation == 1)
                        {
                            LSContainment.Visible = true;
                            LTContainment.Visible = false;
                        }
                        else
                        {
                            if (EditedConnector.GetEditedConnector().SupplierEnd.Aggregation == 1)
                            {
                                LSContainment.Visible = false;
                                LTContainment.Visible = true;
                            }
                        }
                        //CBAgregateTarget.Checked = true;
                        CBByRefSource.Enabled = true;
                        CBByRefSource.Checked = true;
                       // CBAgregateTarget.Checked = true
                    }
                    else
                    {
                        if (EditedConnector.GetAgregate().Equals("SOURCE"))
                        {
                            if (EditedConnector.GetEditedConnector().ClientEnd.Aggregation == 1)
                            {
                                LSContainment.Visible = true;
                                LTContainment.Visible = false;
                            }
                            else
                            {
                                if (EditedConnector.GetEditedConnector().SupplierEnd.Aggregation == 1)
                                {
                                    LSContainment.Visible = false;
                                    LTContainment.Visible = true;
                                }
                            }
                            // CBAgregateSource.Checked = true;
                            CBByRefTarget.Enabled = true;
                            CBByRefTarget.Checked = true;
                           // CBAgregateSource.Checked = true;
                          
                        }
                        else
                        {
                            //CBAgregateTarget.Checked = false;
                            CBByRefSource.Checked = false;
                           // CBAgregateTarget.Checked = false;
                            CBByRefSource.Checked = false;
                        }
                    }

                    util.wlog("TEST", string.Format("ESdRdfsC LSContainment.Visible={0}  LTContainment.Visible={1} CBByRefSource.Enabled= {2}  CBByRefSource.Checked ={3} CBByRefTarget.Enabled={4}  CBByRefTarget.Checked={5}", LSContainment.Visible, LTContainment.Visible, CBByRefSource.Enabled, CBByRefSource.Checked, CBByRefTarget.Enabled, CBByRefTarget.Checked));
                    /* am avril 2016 
                    if (EditedConnector.GetAgregate().Equals("SOURCE"))//|| EditedConnector.GetAgregate().Equals("TARGET"))
                    {
                        CBAgregateSource.Checked = true;
                    }

                    if (EditedConnector.GetSupplierContainmentByRef().Equals(true) || EditedConnector.GetClientContainmentByRef().Equals(true))
                    {
                        CBByRefTarget.Checked = true;
                    }
                    */
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
            if (CreateNewConnector.Equals(true))
            {
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
            }
            else
            {// am juin 2016
                CheckClient =CheckSimpleClientCardinality(CBLBClientCardinality.Text, CBUBClientCardinality.Text);
                CheckSupplier = CheckSimpleClientCardinality(CBLBSupplierCardinality.Text, CBUBSupplierCardinality.Text);
            }
            if ((CheckClient.Equals(true) && CheckSupplier.Equals(true)) || ((CBAgregateSource.Checked.Equals(true) && CheckOtherEnd.Equals(true))))
            {
                if (CreateNewConnector.Equals(true))
                {
                    #region CreateNewConnector
                        ANewConnector = ParentConnector.GetSubConnector(ParentConnector.AddSubConnector());
                        if ( (CBAgregateSource.Checked.Equals(false) && CBAgregateTarget.Checked.Equals(false)))
                        {
                       
                            ANewConnector.SetAgregate("");
                            ANewConnector.SetClientLBCardinality(CBLBClientCardinality.Text);
                            ANewConnector.SetClientUBCardinality(CBUBClientCardinality.Text);
                            ANewConnector.SetSupplierLBCardinality(CBLBSupplierCardinality.Text);
                            ANewConnector.SetSupplierUBCardinality(CBUBSupplierCardinality.Text);

                        }
                        else if (CBAgregateSource.Checked.Equals(true))
                        {
                            ANewConnector.SetAgregate("SOURCE");
                            ANewConnector.SetSupplierLBCardinality(CBLBSupplierCardinality.Text);
                            ANewConnector.SetSupplierUBCardinality(CBUBSupplierCardinality.Text);
                            ANewConnector.SetClientLBCardinality(CBLBClientCardinality.Text);//am mars 2016
                            ANewConnector.SetClientUBCardinality(CBUBClientCardinality.Text);//am mars 2016
                        }
                        else if (CBAgregateTarget.Checked.Equals(true))
                        {
                            ANewConnector.SetAgregate("TARGET");
                            ANewConnector.SetClientLBCardinality(CBLBClientCardinality.Text);
                            ANewConnector.SetClientUBCardinality(CBUBClientCardinality.Text);
                            ANewConnector.SetSupplierLBCardinality(CBLBSupplierCardinality.Text);//am mars 2016
                            ANewConnector.SetSupplierUBCardinality(CBUBSupplierCardinality.Text);//am mars 2016
                        } 

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
                            ANewConnector.SetClientContainmentByRef(true); //am mars 2016 avril 2016 pas de containment en rdfs
                            ANewConnector.SetSupplierContainmentByRef(false); //am mars 2016 avril 2016 pas de containment en rdfs
                            //ANewConnector.SetAgregate("TARGET");//am mars 2016
                           // ANewConnector.SetSupplierLBCardinality(CBLBClientCardinality.Text);//am mars 2016
                           // ANewConnector.SetSupplierUBCardinality(CBUBClientCardinality.Text);//am mars 2016
                        }
                        
                        ANewConnector.SetClientRole(TBClientRole.Text);
                        ANewConnector.SetSupplierRole(TBSupplierRole.Text);
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
                    #endregion
                }
                else
                {
                    #region UpdateConnector
                        
                        ANewConnector = ParentConnector.GetSubConnector(SubConnectorGUID);
                        this.CBAgregateSource.Enabled = false; //am mars 2016
                        this.CBAgregateTarget.Enabled = false; //am mars 2016
                        this.CBByRefSource.Text = "Is Direction End"; // am mars 2016
                        this.CBByRefTarget.Text = "Is Direction End"; // am mars 2016
                        if (CBAgregateSource.Checked.Equals(false) )
                        {
                            //ANewConnector.SetSupplierContainmentByRef(false);
                           // ANewConnector.SetSupplierContainmentByRef(true);//am mars 2016
                        }
                        if (CBAgregateTarget.Checked.Equals(false))
                        {
                           // ANewConnector.SetClientContainmentByRef(false);
                           // ANewConnector.SetClientContainmentByRef(true);//am mars 2016
                        }
                        util.wlog("TEST", string.Format("ESdRdfsCButAdd deb   Newcon.Aggregate ={0} CBCBAgregateSource.Checked={1} CBAgregateTarget.Checked= {2} CBByRefSource.Checked={3} CBByRefTarget.Checked={4}", ANewConnector.GetAgregate(), ANewConnector.GetClientContainmentByRef(), ANewConnector.GetSupplierContainmentByRef(), CBByRefSource.Checked, CBByRefTarget.Checked));
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
                        }

                        util.wlog("TEST", string.Format("ESdRdfsCButAdd   Newcon.Aggregate ={0} NewConClientContainment={1} NewconSupplierContainment= {2} ", ANewConnector.GetAgregate(), ANewConnector.GetClientContainmentByRef(), ANewConnector.GetSupplierContainmentByRef()));
                        
                    ANewConnector.SetClientRole(TBClientRole.Text);
                        ANewConnector.SetSupplierRole(TBSupplierRole.Text);
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
                        if (CBClientQualifier.Text.Equals("No qualifier"))
                        {
                            ANewConnector.SetClientRoleQualifier("");
                        }
                        if (!CBSupplierQualifier.Text.Equals("No qualifier") && !CBSupplierQualifier.Text.Equals(""))
                        {
                            ANewConnector.SetSupplierRoleQualifier(GetFormatedQualifierFromClientComboBox(false));
                        }
                        if (CBSupplierQualifier.Text.Equals("No qualifier"))
                        {
                            ANewConnector.SetSupplierRoleQualifier("");
                        }
                    #endregion
                }
                if (!(ANewConnector==null))
                {
                    ANewConnector.SetSelectedState(true);
                }
                this.RefreshParentUI();
                this.Dispose();
            }
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
            /* //am juin 2016
            if (!UBClientCardinality.Equals(""))
            {
                MessageBox.Show("Client's upper bound must be empty.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
             * */
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
           // } // am juin 2016
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
                    //int.Parse(UBClientCardinality); am juin 2016
                    if ((UBClientCardinality=="") && (LBClientCardinality=="1"))// am juin 2016
                    {
                        UBClientCardinality = "1";
                    }
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
            /*
            if (!UBSupplierCardinality.Equals(""))
            {
                MessageBox.Show("Supplier's upper bound must be empty.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
             * */
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
           // }
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
               // CBLBClientCardinality.Text = ""; // am juil 2016
               //  CBUBClientCardinality.Text = ""; // am juil 2016
               // CBByRefTarget.Enabled = true;
                CBAgregateTarget.Enabled = false;
                CBAgregateSource.Enabled = false;
              
            }
            else if (CBAgregateSource.Checked.Equals(false))
            {
                //CBLBClientCardinality.Text = ParentConnector.GetClientLBCardinality(); // am juil 2016
                //CBUBClientCardinality.Text = ParentConnector.GetClientUBCardinality(); //am juil 2016
               // CBByRefTarget.Enabled = true;
                //CBAgregateTarget.Enabled = false;
               // CBByRefTarget.Checked = false;
            }

        }

        private void CBAgregateTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (CBAgregateTarget.Checked.Equals(true)) 
            {
                //CBLBSupplierCardinality.Text = "";  //am juil 2016
               // CBUBSupplierCardinality.Text = ""; //am juil 2016
               // CBByRefSource.Enabled = true;
               // CBByRefTarget.Enabled = true;
               CBAgregateSource.Enabled = false;
               CBAgregateTarget.Enabled = false;
             
            }
            else if (CBAgregateTarget.Checked.Equals(false))
            {
                //CBLBSupplierCardinality.Text = ParentConnector.GetSupplierLBCardinality(); //am juil 2016
               // CBUBSupplierCardinality.Text = ParentConnector.GetSupplierUBCardinality();  // am Juil 2016
               // CBByRefSource.Enabled = true;
                //CBByRefSource.Checked = false;
               // CBAgregateSource.Enabled = false; //am mars 2016
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void CBByRefSource_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CBByRefSource.Checked == true)
            {
                this.CBAgregateTarget.Checked = true;
                this.CBAgregateTarget.Enabled = false;
                this.CBAgregateSource.Checked = false;
                this.CBByRefTarget.Checked = false;  
            }
            else
            {
                this.CBAgregateTarget.Checked = false;
                this.CBAgregateTarget.Enabled = false;
                this.CBAgregateSource.Enabled = false;
                this.CBAgregateSource.Checked = true;

                // this.CBByRefTarget.Checked = false;  
                //this.CBByRefSource.Enabled = true;
                //this.CBByRefTarget.Enabled = true;
               
               // this.CBAgregateTarget.Checked = false;
               // this.CBAgregateSource.Checked = false;
            }
            /*
            if (NewEditedConnector == null) // am juin 2016
            {
                CBLBClientCardinality.Text = ParentConnector.GetClientLBCardinality();
                CBUBClientCardinality.Text = ParentConnector.GetClientUBCardinality();
                CBLBSupplierCardinality.Text = ParentConnector.GetSupplierLBCardinality();
                CBUBSupplierCardinality.Text = ParentConnector.GetSupplierUBCardinality();
            }
            else
            {
                CBLBClientCardinality.Text = NewEditedConnector.GetClientLBCardinality();
                CBUBClientCardinality.Text = NewEditedConnector.GetClientUBCardinality();
                CBLBSupplierCardinality.Text = NewEditedConnector.GetSupplierLBCardinality();
                CBUBSupplierCardinality.Text = NewEditedConnector.GetSupplierUBCardinality();
            }
             * */
        }

        private void CBByRefTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CBByRefTarget.Checked == true)
            {
                this.CBAgregateSource.Checked = true;
                this.CBAgregateSource.Enabled = false;
                this.CBAgregateTarget.Checked = false;
                this.CBByRefSource.Checked = false;
            } else {
                //this.CBByRefTarget.Enabled = true;
                this.CBAgregateSource.Checked = false;
               
            }

                //this.CBAgregateSource.Checked = false;
            /*
            if (NewEditedConnector == null) // am juin 2016
            {
                CBLBClientCardinality.Text = ParentConnector.GetClientLBCardinality();
                CBUBClientCardinality.Text = ParentConnector.GetClientUBCardinality();
                CBLBSupplierCardinality.Text = ParentConnector.GetSupplierLBCardinality();
                CBUBSupplierCardinality.Text = ParentConnector.GetSupplierUBCardinality();
            }
            else
            {
                CBLBClientCardinality.Text = NewEditedConnector.GetClientLBCardinality();
                CBUBClientCardinality.Text = NewEditedConnector.GetClientUBCardinality();
                CBLBSupplierCardinality.Text = NewEditedConnector.GetSupplierLBCardinality();
                CBUBSupplierCardinality.Text = NewEditedConnector.GetSupplierUBCardinality();
            }
             * */
          
        }

        private void BUTManageQualifier_Click(object sender, EventArgs e)
        {
            QualifierManagerForm qf = new QualifierManagerForm(this.CBSupplierQualifier, this.CBClientQualifier);
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

    }
}
