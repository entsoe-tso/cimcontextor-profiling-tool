using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using EA; // am janv 2016
using CimContextor.EditConnectors;
using System.Diagnostics;
using CimContextor.utilitaires;

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
    public partial class EditDuplicateForOptimConnectorsForm : Form
    {
        private EA.Repository Repo;
        private EA.Element IBOSelectedItem;
        private EA.Element ParentSelectedItem;
        private ArrayList ConnectorsList = new ArrayList();
        private ArrayList ConnectorsListWithoutIBO = new ArrayList();
        private ArrayList TargetChildClass = new ArrayList();
        private ArrayList SourceChildClass = new ArrayList();
        private ConstantDefinition CD = new ConstantDefinition();
        private Ta ta = new Ta(); // pour analyse temps
        /****************  new development am janv 2016 ***********************************/      
        Dictionary<long, List<EA.Element>> dicIBOElemntByParent = new Dictionary<long, List<EA.Element>>(); // gives  profiles elements given its parent id
        Dictionary<long, bool> dicPossibleElements = new Dictionary<long, bool>(); // each id is marked ok if on pf the element in hierarchy has an IBO element
        Dictionary<long, long> dicIBOInheritedbyElementID = new Dictionary<long,long>();// pour un profileElement donne son parent et son profileancetre
        List<long> profEltIDs = new List<long>(); // the IBOelements ids
        List<long> parentEltIDs = new List<long>(); // their parent's ids
        utilitaires.Utilitaires util = null; // ABA20230228 new utilitaires.Utilitaires();
        List<long> dejatraite = new List<long>(); // the connexion has already treated
        static List<EA.Element> profElements =new List<EA.Element>() ;  // all profile elements
        static Dictionary<long,EA.Element> dicProfParentElement=new Dictionary<long,EA.Element>();// elementid/parent
        List<long> ElligibleElementIds = new List<long>();
        static long profileID = 0; // the profile on which one works at a given moment (changed when the profile changed)
        ArrayList EditedConnectorList = new ArrayList();  // list of connectors already displayed
        Dictionary<string, ArrayList> dicProfParentGuidList = new Dictionary<string, ArrayList>();// give for each connector their IBOs
       public bool subok; // to show if there is a concrete subconnector
        /***************************************************/
        #region checkRole
        //For checking role when executing dupplicating connectors
        ArrayList TargetRoleList = new ArrayList();
        ArrayList SourceRoleList = new ArrayList();
#endregion



        /// <param name="Repo"></param>
        /// <param name="SubConnector">true if it's a subconnector</param>
        /// <param name="SubID">The ID of the SubConnector</param>
        /// <param name="ConnectorGUID">The Original connectors GUID to dupplicate</param>
        /// <param name="SelectedSideElementID">The element ID of the selected side object</param>
        /// <param name="IBOSelectedSideElementID">The element ID of the selected side object IBO</param>
        /// <param name="TargetedElementID">The element ID of the targeted side object</param>
        /// <param name="IBOTargetedElementID">The element ID of the targeted side object IBO</param>
        /// <param name="IBOSelectedItemID">Diagram's object selected by user</param>
        public EditDuplicateForOptimConnectorsForm(EA.Repository Repo)
        {

            util = new utilitaires.Utilitaires(Repo);
            ta.start(new StackFrame(0,true));
          bool Optim = true;
            
            if(Main.EditType=="graphwg13")
            {
                 Optim = true;
            }else{
                if(Main.EditType=="graph")
                {
                 Optim = false;
                }
                else
                {
                    return;
                }
                
            }
          
       //   CimContextor.utilitaires.Utilitaires.dicAncestors = new Dictionary<long, List<long>>();// am aout 18 prov
            InitializeComponent();
            this.LVConnectors.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.LVConnectors_ItemChecked);
            this.LVSubConnectors.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LVSubConnectors_ItemCheck);
            this.Repo = Repo;

            //
            LVConnectors.Enabled = true;
            LVConnectors.CheckBoxes = true;

            //CBDefaultParentConnector.Checked = false;
            //
            //Init of Selected Item
        
            CheckSelectedItem();
 
            LabName.Text = IBOSelectedItem.Name;
            string DirName = Repo.GetPackageByID((int)IBOSelectedItem.PackageID).Name;
            this.label2.Text = DirName + "::" + IBOSelectedItem.Name;//am mars 2016



            //+++++++++++++++++++++++++++++++++++++++++ new development +++++++++++++++++++++++++++++++++++++++++++++++++
            util = new utilitaires.Utilitaires(Repo);
            /*
             compte=util.GetAllProfElementsCount(profPack,compte);
            if ((profElements.Count==0)
               || (profileID != profPack.PackageID) // the profile has changed
                || ( compte != profElementsCount)
                )
            {
                profElements = new List<EA.Element>();
                util.GetAllPureProfElements(profPack, profElements, true);// am oct 2016
                profileID = profPack.PackageID;
                profElementsCount=0;
                profElementsCount=util.GetAllProfElementsCount(profPack,profElementsCount);
                utilitaires.Utilitaires.dicProfElemByParentGuid=new Dictionary<string,List<long>>();
            }
           */
            EA.Package profPack = util.getProfilePackage(Repo, IBOSelectedItem.PackageID);
            if (
                (profElements.Count == 0)
                ||
                (Main.isBasedOnExecuted)// an is basedon  has been created has been executed 
                ||
                (profileID != profPack.PackageID) // the profile has changed)
                )
            {
                profElements = new List<EA.Element>();
                util.GetAllPureProfElements(profPack, profElements, true);// am oct 2016
                util.dicProfElemByParentGuid = new Dictionary<string, List<long>>();
                profileID = profPack.PackageID;
                Main.isBasedOnExecuted = false; // done
            }
            

            CheckElligibleParentElements(Optim); // si optim false on ne reste pas au premier niveau 
             EditedConnectorList = new ArrayList();
             dicProfParentGuidList = new Dictionary<string, ArrayList>();// give for each connector their IBOs
            // we collect all guids for already parent connector

            foreach (EA.Connector co in IBOSelectedItem.Connectors)
            {
                if ((co.Type == "Dependency") || (co.Type == "Generalization")) continue; // on ne traite que les asssociations
                string conguid = util.getConParentGuid(co);
                if (conguid != "")
                {
                    if (!dicProfParentGuidList.ContainsKey(conguid)) dicProfParentGuidList[conguid] = new ArrayList();
                    dicProfParentGuidList[conguid].Add(co);
                }
            }
            if (dicIBOInheritedbyElementID.ContainsKey(IBOSelectedItem.ElementID)) // l'element sectionne a-t-i un ancetre local
            {
                EA.Element locinh = Main.Repo.GetElementByID((int)dicIBOInheritedbyElementID[IBOSelectedItem.ElementID]);
            
                foreach (EA.Connector co in locinh.Connectors) //oui il faut aussii tracer toutes les connexions possibles
                {
                    if ((co.Type == "Dependency") || (co.Type == "Generalization")) continue; // on ne traite que les asssociations
                    string conguid = util.getConParentGuid(co);
                    if (conguid != "")
                    {
                        if (!dicProfParentGuidList.ContainsKey(conguid)) dicProfParentGuidList[conguid] = new ArrayList();
                        dicProfParentGuidList[conguid].Add(co);
                    }
                }
            }


            // iterrates on all possible connectors for parentdelecteditemf   
            EA.Element otherend = null;
            long otherendid = 0;
            bool swap = false;
            
            foreach (EA.Connector con in ParentSelectedItem.Connectors)
            {
                if (dejatraite.Contains(con.ConnectorID)) continue; // the connector is already treated
                if ((con.Type != "Generalization") && (con.Type != "Dependency"))
                {
                    dejatraite.Add(con.ConnectorID);
                    string conguid = con.ConnectorGUID; // guid of connector between parent elements
                    if (con.ClientID == ParentSelectedItem.ElementID) // l'element est-il la source?
                    {
                        swap = false;
                        otherendid = con.SupplierID;

                    }
                    else
                    {
                        swap = true;
                        otherendid = con.ClientID;
                    }
                    if (!ElligibleElementIds.Contains(otherendid)) continue; // go to next connexion the otherend is not possible
                    otherend = Main.Repo.GetElementByID((int)otherendid);
                    List<EA.Element> elts = dicIBOElemntByParent[otherendid];// dicIBOElemntByParent was updated by checkElligbleParent
                    ArrayList PotentialEditedConnectorList = new ArrayList(); ;// tous les connecteurs du profil qui sont IBO du connecteur parent
                    if (dicProfParentGuidList.ContainsKey(con.ConnectorGUID)) PotentialEditedConnectorList = dicProfParentGuidList[con.ConnectorGUID]; //EditedConnectorList = dicProfParentGuidList[con.ConnectorGUID];
                    if (elts.Count != 0)
                    {
                        foreach (EA.Element elt in elts)
                        {
                            EA.Element IBOotherend = elt;
                            long prov = IBOotherend.ElementID;
                            // Generation de la liste de connection puisque con est elligible
                            EA.Element source = ParentSelectedItem;
                          //18  if (dicProfParentGuidList.ContainsKey(con.ConnectorGUID)) EditedConnectorList = dicProfParentGuidList[con.ConnectorGUID
                            EditedConnectorList = new ArrayList();
                          //  PotentialEditedConnectorList = new ArrayList();
                            // On doit se  poser la  question de savoir si un des connnecteurs potentiels a comme extremite le Ie IBOOtherend
                           // if (dicProfParentGuidList.ContainsKey(con.ConnectorGUID)) PotentialEditedConnectorList = dicProfParentGuidList[con.ConnectorGUID]; //EditedConnectorList = dicProfParentGuidList[con.ConnectorGUID];

                            foreach (EA.Connector co in PotentialEditedConnectorList)
                            {
                               
                                    if (

                                  ((ParentSelectedItem.ElementID != otherendid) // this is not a self association
                                   &&
                                  (
                                  ((co.ClientID == IBOotherend.ElementID) && (co.SupplierID == IBOSelectedItem.ElementID))
                                    ||
                                  ((co.SupplierID == IBOotherend.ElementID) && (co.ClientID == IBOSelectedItem.ElementID))
                                  ) 
                                  )
                                   ||
                                    ((ParentSelectedItem.ElementID == otherendid) // this is  a self association
                                      &&
                                          ((IBOSelectedItem.ElementID != IBOotherend.ElementID) // the profile elements are different
                                             &&
                                          ((co.ClientID == IBOotherend.ElementID)
                                           ||
                                          (co.SupplierID == IBOotherend.ElementID))))
                                    ||
                                     ((ParentSelectedItem.ElementID == otherendid) // this is  a self association
                                      &&
                                          ((IBOSelectedItem.ElementID == IBOotherend.ElementID) // the profile elements are equal
                                             &&
                                          (co.ClientID == co.SupplierID)))
                                   )
                                        
                                    EditedConnectorList.Add(co);
                            }  
                            EditEAClassConnector ANewConnector = new EditEAClassConnector(Main.Repo, false, 0, con.ConnectorGUID, ParentSelectedItem, otherend, IBOSelectedItem, IBOotherend, swap, EditedConnectorList);
                            
                            if (!Optim)
                            {

                                long inherited = 0;

                                //if (dicIBOInheritedbyElementID.ContainsKey(IBOotherend.ElementID)) 

                              if(dicIBOInheritedbyElementID.ContainsKey(IBOSelectedItem.ElementID))
                                {
                                    inherited = dicIBOInheritedbyElementID[IBOSelectedItem.ElementID];
                                }
                                if (inherited != 0)
                                {
                                    foreach(EA.Connector co in PotentialEditedConnectorList )
                                    {
                                        if((inherited==co.ClientID) || (inherited == co.SupplierID))
                                        {
                                            ANewConnector.SetInherited(true);
                                        }
                                    }
                                    /*
                                    //List<long> ancestors = utilitaires.Utilitaires.dicAncestors[inherited];
                                    bool ok = false;
                                    
                                    foreach(EA.Element el in elts)
                                    {
                                        if (el.ElementID == inherited)
                                        {
                                            ok = true;
                                            break;
                                        }
                                   
                                    
                                    if (ok)
                                    {
                                        ANewConnector.SetInherited(true);
                                    }
                                    */
                                }
                                 
                                
                            }
                            
                            ConnectorsList.Add(ANewConnector);                    

                        }

                    }
                    
                    if (!Optim)
                    {
                        long selfinherited = 0;

                        if (dicIBOInheritedbyElementID.ContainsKey(IBOSelectedItem.ElementID))
                        {
                            selfinherited = dicIBOInheritedbyElementID[IBOSelectedItem.ElementID];
                        }
                        if (selfinherited != 0)
                        {
                            traitOneInheritedConnexion(ParentSelectedItem.ElementID, IBOSelectedItem);
                        }

                    }
                     
                }

            }

            if (!Optim)   traitAllInheritedconnexion(IBOSelectedItem);   
            //+++++++++++++++++++++++++++++++++++++++++ new development +++++++++++++++++++++++++++++++++++++++++++++++++
            ListViewItem lvi;
            //String[] aHeaders = new string[7];
            String[] aHeaders = new string[6]; // am mars 2016
            int i = 0;
              //  ta.start("EditDuplicateCorrectors-5");
            // trier les associationclasse
            IComparer mc = new myComparer(); // am mars 2016
            ConnectorsList.Sort(mc);   // am mars 2016

              foreach (EditEAClassConnector AConnector in ConnectorsList)
            {
                aHeaders[0] = i.ToString();
                bool coteclient = true; // la selection se trouve du cote du client
                if(
                    (ParentSelectedItem.ElementID != AConnector.GetSelectedElementConnector().ElementID)
                    &&
                    (!utilitaires.Utilitaires.dicAncestors[ParentSelectedItem.ElementID].Contains(AConnector.GetSelectedElementConnector().ElementID)) 
              )
                {
                    coteclient = false;
                }
                  // if (ParentSelectedItem.ElementGUID.Equals(AConnector.GetSelectedElementConnector().ElementGUID))
                  if(coteclient)
              //  if(AConnector.GetSwitch())
                {
                   // aHeaders[1] = Repo.GetPackageByID(AConnector.GetSelectedIBOElement().PackageID).Name + "::" + AConnector.GetSelectedIBOElement().Name.ToString(); //am mars 2016
                    aHeaders[1] = Repo.GetPackageByID(AConnector.GetTargetedIBOElement().PackageID).Name + "::" + AConnector.GetTargetedIBOElement().Name.ToString();
                    aHeaders[2] = AConnector.GetClientRole();
                    aHeaders[3] = AConnector.GetSupplierRole();
                }
                else
                {
                    //aHeaders[1] = Repo.GetPackageByID(AConnector.GetTargetedIBOElement().PackageID).Name + "::" + AConnector.GetTargetedIBOElement().Name.ToString(); // am mars 2016
                    aHeaders[1] = Repo.GetPackageByID(AConnector.GetTargetedIBOElement().PackageID).Name + "::" + AConnector.GetTargetedIBOElement().Name.ToString();
                    aHeaders[2] = AConnector.GetSupplierRole();
                    aHeaders[3] = AConnector.GetClientRole();
                }

                aHeaders[4] = AConnector.GetGUID(); //am mars 2016
                aHeaders[5] = AConnector.GetTargetedIBOElement().ElementID.ToString();
                lvi = new ListViewItem(aHeaders);
                if(AConnector.GetInherited().Equals(true)){
                    lvi.BackColor=Color.Gold;
                }
                if (
                    (AConnector.GetSelectedState().Equals(true))
                    //            &&
                   // (AConnector.GetInherited().Equals(false))// am sept 2018 ajout
                    )
                {
                    lvi.Checked = true;
                }
                else
                {
                    lvi.Checked = false;
                }
                LVConnectors.Items.Add(lvi);
                i++;
            }
            ta.stop();
            this.Show();
        }
/// <summary>
/// l'algorithme choisi est d'explorer le profile pas a pas et non à priori
/// </summary>
/// <param name="Repo"></param>
        private void newEitDuplicateForm(Repository Repo,bool Optim)
        {
            ta.start(new StackFrame(0, true));



            //   CimContextor.utilitaires.Utilitaires.dicAncestors = new Dictionary<long, List<long>>();// am aout 18 prov
           // InitializeComponent();
          //  this.LVConnectors.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.LVConnectors_ItemChecked);
          //  this.LVSubConnectors.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LVSubConnectors_ItemCheck);
          //  this.Repo = Repo;

            //
            //LVConnectors.Enabled = true;
           // LVConnectors.CheckBoxes = true;

            // positions    IBOSelectedItem and  ParentSelectedItem;
            CheckSelectedItem(); 

            LabName.Text = IBOSelectedItem.Name;
            string DirName = Repo.GetPackageByID((int)IBOSelectedItem.PackageID).Name;
            this.label2.Text = DirName + "::" + IBOSelectedItem.Name;//am mars 2016



            /*
            if (!ConnectorsListWithoutIBO.Count.Equals(0))
            {
                ArrayList StringList = new ArrayList();
                ta.elapsetime("EditDuplicateCorrectors-1");
                foreach (String ASupplierElementName in ConnectorsListWithoutIBO)
                {
                    if (!StringList.Contains("The element " + ASupplierElementName + " doesn't have an IsBasedOn"))
                    {
                        StringList.Add("The element " + ASupplierElementName + " doesn't have an IsBasedOn");
                    }
                }
                ta.diffelaps("EditDuplicateCorrectors-1");
                // DetailMessageBox DMB = new DetailMessageBox("Warning : " + ConnectorsListWithoutIBO.Count + " connector(s) have been ignored.", "CimContextor : The following classe(s) doesn't have an IsBasedOn :", StringList); //am janv 2016
            }


            ArrayList ConnectorsToRemove = new ArrayList();
            ta.elapsetime("EditDuplicateCorrectors-2");
            foreach (EditEAClassConnector AConnector in ConnectorsList)
            {
                if (AConnector.GetInitializationState().Equals(false))
                {
                    ConnectorsToRemove.Add(AConnector);
                }
            }
            ta.diffelaps("EditDuplicateCorrectors-2");
            ArrayList RemovedList = new ArrayList();
            
           


            foreach (EditEAClassConnector AConnector in ConnectorsToRemove)
            {

                RemovedList.Add(AConnector.GetFailedInitReason());

            }
            ta.elapsetime("EditDuplicateCorrectors-4");
            foreach (EditEAClassConnector AConnectorToRemove in ConnectorsToRemove)
            {
                if (ConnectorsList.Contains(AConnectorToRemove))
                {
                    ConnectorsList.Remove(AConnectorToRemove);
                }
            }
            ta.diffelaps("EditDuplicateCorrectors-4");
            if (!ConnectorsToRemove.Count.Equals(0))
            {
                DetailMessageBox DMB = new DetailMessageBox("Warning : " + ConnectorsToRemove.Count + " element(s) have been ignored.", "CimContextor : The following classe(s) doesn't follow the package rule :", RemovedList);
            }
            */

//+++++++++++++++++++++++++++++++++++++++++ new development +++++++++++++++++++++++++++++++++++++++++++++++++
            util = new utilitaires.Utilitaires(Repo);
            CheckElligibleParentElements(Optim); // on reste au premier 
            ArrayList EditedConnectorList=new ArrayList();
            Dictionary<string,List<EA.Connector>>  dicProfParentGuidList = new  Dictionary<string,List<EA.Connector>>();// give for each connector their IBOs
            // we collect all guids for already parent connector

            foreach(EA.Connector co in IBOSelectedItem.Connectors)
            {
                if ((co.Type == "Dependency") || (co.Type == "Generalization")) continue; // on ne traite que les asssociations
                string conguid=util.getConParentGuid(co);
                if(conguid !="")
                {
                   if(!dicProfParentGuidList.ContainsKey(conguid )) dicProfParentGuidList[conguid]=new List<EA.Connector>();
                       dicProfParentGuidList[conguid].Add(co); 
                }
            }
            // iterrates on all possible connectors for parentdelecteditemf   
            EA.Element otherend=null;
            long otherendid=0;
            bool swap=false;
            foreach (EA.Connector con in ParentSelectedItem.Connectors)
            {
             if((con.Type !="Generalization") && (con.Type !="Dependency"))
             {
                 string conguid = con.ConnectorGUID; // guid of connector between parent elements
                 if(con.ClientID==ParentSelectedItem.ElementID) // l'element est-il la source?
                 {
                    swap=false;
                    otherendid = con.SupplierID;
                     
                 }else{
                    swap=true;
                    otherendid = con.ClientID;
                 }
                 if(!ElligibleElementIds.Contains(otherendid)) continue; // go to next connexion the otherend is not possible
                 otherend = Main.Repo.GetElementByID((int)otherendid);
                 List<EA.Element> elts =dicIBOElemntByParent[otherendid];// dicIBOElemntByParent was updated by checkElligbleParent
                if(elts.Count!=0) 
                 {
                    foreach(EA.Element elt in elts){
                    EA.Element IBOotherend= elt;
                     // Generation de la liste de connection puisque con est elligible
                     //EditEAClassConnector ANewConnector = new EditEAClassConnector(Repo, false, 0, AConnector.ConnectorGUID, Repo.GetElementByID(ASourceClassID), Repo.GetElementByID(ATargetClassID), IBOSelectedItem, IBOTargetedItem, Switch, EditedConnectorList);
                        EA.Element source=ParentSelectedItem;

                        EditEAClassConnector ANewConnector = new  EditEAClassConnector(Main.Repo, false, 0, con.ConnectorGUID, ParentSelectedItem, otherend, IBOSelectedItem, IBOotherend, swap, EditedConnectorList);
                        ConnectorsList.Add(ANewConnector);
                    }
                }
                 }
                 
             }
            
//+++++++++++++++++++++++++++++++++++++++++ new development +++++++++++++++++++++++++++++++++++++++++++++++++
   // display the associations to be selected
            ListViewItem lvi;
            //String[] aHeaders = new string[7];
            String[] aHeaders = new string[6]; // am mars 2016
            int i = 0;
            //  ta.start("EditDuplicateCorrectors-5");
            // trier les associationclasse
            IComparer mc = new myComparer(); // am mars 2016
            ConnectorsList.Sort(mc);   // am mars 2016

            foreach (EditEAClassConnector AConnector in ConnectorsList)
            {
                aHeaders[0] = i.ToString();
                if (ParentSelectedItem.ElementGUID.Equals(AConnector.GetSelectedElementConnector().ElementGUID))
                {
                    // aHeaders[1] = Repo.GetPackageByID(AConnector.GetSelectedIBOElement().PackageID).Name + "::" + AConnector.GetSelectedIBOElement().Name.ToString(); //am mars 2016
                    aHeaders[1] = Repo.GetPackageByID(AConnector.GetTargetedIBOElement().PackageID).Name + "::" + AConnector.GetTargetedIBOElement().Name.ToString();
                    aHeaders[2] = AConnector.GetClientRole();
                    aHeaders[3] = AConnector.GetSupplierRole();
                }
                else
                {
                    //aHeaders[1] = Repo.GetPackageByID(AConnector.GetTargetedIBOElement().PackageID).Name + "::" + AConnector.GetTargetedIBOElement().Name.ToString(); // am mars 2016
                    aHeaders[1] = Repo.GetPackageByID(AConnector.GetTargetedIBOElement().PackageID).Name + "::" + AConnector.GetTargetedIBOElement().Name.ToString();
                    aHeaders[2] = AConnector.GetSupplierRole();
                    aHeaders[3] = AConnector.GetClientRole();
                }

                aHeaders[4] = AConnector.GetGUID(); //am mars 2016
                aHeaders[5] = AConnector.GetTargetedIBOElement().ElementID.ToString();
                lvi = new ListViewItem(aHeaders);
                if (AConnector.GetInherited().Equals(true))
                {
                    lvi.BackColor = Color.Gold;
                }
                if (AConnector.GetSelectedState().Equals(true))
                {
                    lvi.Checked = true;
                }
                else
                {
                    lvi.Checked = false;
                }
                LVConnectors.Items.Add(lvi);
                i++;
            }
            ta.reset();
            this.Show();
        }

 


        public EA.Repository GetRepository()
        {
            return Repo;
        }

        private EditEAClassConnector GetConnectorFromUI()
        {
            if (LVConnectors.SelectedItems.Count.Equals(1))
            {
                if (LVConnectors.SelectedItems[0].Checked.Equals(true))
                {
                    foreach (EditEAClassConnector AConnector in ConnectorsList)
                    {
                      
                        if ((LVConnectors.SelectedItems[0].SubItems[4].Text.ToString().Equals(AConnector.GetGUID().ToString())) && (LVConnectors.SelectedItems[0].SubItems[5].Text.Equals(AConnector.GetTargetedIBOElement().ElementID.ToString()))) // am mars 2016
                        {
                            return AConnector;
                        }
                    }
                }
            }
            return null;
        }

        public EA.Element GetSelectedItem()
        {
            return this.ParentSelectedItem;
        }

        public void RefreshUI()
        {
            foreach (ListViewItem AnLVI in LVSubConnectors.Items)
            {
                AnLVI.Remove();
            }
            if (!LVConnectors.SelectedItems.Equals(null))
            {
                ArrayList SubConnectors = new ArrayList();
                SubConnectors = this.GetConnectorFromUI().GetSubConnectors();
                ListViewItem lvi;
                String[] aHeaders = new string[6];
                int i = 0;
                foreach (EditEAClassConnector ASubConnector in SubConnectors)
                {
                    aHeaders[0] = i.ToString();
                    if (ASubConnector.GetSelectedElementConnector().ElementGUID.Equals(ParentSelectedItem.ElementGUID))
                    {
                        if (!ASubConnector.GetClientRoleQualifier().Equals(""))
                        {
                            aHeaders[1] = ASubConnector.GetClientRoleQualifier() + "_" + ASubConnector.GetClientRole();
                        }
                        else
                        {
                            aHeaders[1] = ASubConnector.GetClientRole();
                        }
                        if (!ASubConnector.GetSupplierRoleQualifier().Equals(""))
                        {
                            aHeaders[2] = ASubConnector.GetSupplierRoleQualifier() + "_" + ASubConnector.GetSupplierRole();
                        }
                        else
                        {
                            aHeaders[2] = ASubConnector.GetSupplierRole();
                        }
                    }
                    else
                    {
                        if (!ASubConnector.GetSupplierRoleQualifier().Equals(""))
                        {
                            aHeaders[1] = ASubConnector.GetSupplierRoleQualifier() + "_" + ASubConnector.GetSupplierRole();
                        }
                        else
                        {
                            aHeaders[1] = ASubConnector.GetSupplierRole();
                        }

                        if (!ASubConnector.GetClientRoleQualifier().Equals(""))
                        {
                            aHeaders[2] = ASubConnector.GetClientRoleQualifier() + "_" + ASubConnector.GetClientRole();
                        }
                        else
                        {
                            aHeaders[2] = ASubConnector.GetClientRole();
                        }
                    }
                    if (!ASubConnector.GetAgregate().Equals(""))
                    {
                        aHeaders[3] = "true";
                    }
                    else
                    {
                        aHeaders[3] = "false";
                    }
                    aHeaders[4] = ASubConnector.GetTargetedElementConnector().Name;
                    aHeaders[5] = ASubConnector.GetSubID().ToString();
                    lvi = new ListViewItem(aHeaders);
                    lvi.Checked = ASubConnector.GetSelectedState();
                    LVSubConnectors.Items.Add(lvi);
                    i++;
                }
            }
        }

        //Return the selected Ea element if he is in the diagram
        private void CheckSelectedItem()
        {
            bool errorHappened = false;
            try
            {
                Ta tb = new Ta("CheckSelectedItem");
                EA.Diagram SelectedDiagram = (EA.Diagram)Repo.GetCurrentDiagram();
                EA.Collection ObjectList = SelectedDiagram.SelectedObjects;
                IBOSelectedItem = Repo.GetElementByID(((EA.DiagramObject)ObjectList.GetAt(0)).ElementID);

                if (IBOSelectedItem == null)
                {
                    ErrorCodes.ShowError(ErrorCodes.ERROR_035);
                    errorHappened = true;
                    // ABA20230401 this.Dispose();
                }

                if (IBOSelectedItem.Type.ToString().Equals("Class"))
                {
                    string eltguid = util.getEltParentGuid(IBOSelectedItem);
                    if (eltguid != "")
                    {
                        ParentSelectedItem = Repo.GetElementByGuid(eltguid);
                        //if (AnIsBasedOnConnector == null)
                    }
                    else
                    {
                        ErrorCodes.ShowError(ErrorCodes.ERROR_036);
                        ta.stop();
                        tb.stop();
                        ta.quit();
                        errorHappened = true;
                        //this.Dispose();  ABA20230401
                    }
                }

                tb.stop();
            }
            catch
            {
                ErrorCodes.ShowError(ErrorCodes.ERROR_037);
                //this.Dispose(); ABA20230401
            }
            finally
            {
                if (errorHappened)
                {
                    this.Dispose();
                }
            }
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            ta.stop();
            ta.quit(); //c'est la fin
            this.Dispose();
        }

        private bool SubCheckRole(EditEAClassConnector aConnector)
        {

            //Target
            RoleStorage TheStoredTarget = null;
            foreach (RoleStorage AStoredTarget in TargetRoleList)
            {
                if (AStoredTarget.GetTargetName().Equals(aConnector.GetTargetedIBOElement().Name))
                {
                    TheStoredTarget = AStoredTarget;
                    break;
                }
            }

            if (TheStoredTarget == null)
            {
                TheStoredTarget = new RoleStorage(aConnector.GetTargetedIBOElement().Name);
                TargetRoleList.Add(TheStoredTarget);
            }


            if (TheStoredTarget.AddRole(aConnector.GetEAFormatRole(aConnector.GetSupplierRoleQualifier(), aConnector.GetSupplierRole())).Equals(false))
            {
                MessageBox.Show("You can't have several connectors with the exact same role targeting an element( " + aConnector.GetEAFormatRole(aConnector.GetSupplierRoleQualifier(), aConnector.GetSupplierRole()) + " ).\nChange a qualifier or role and try again.", "Qualifier Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            //end target
            //Source
            RoleStorage TheStoredSource = null;
            foreach (RoleStorage AStoredSource in SourceRoleList)
            {
                if (AStoredSource.GetTargetName().Equals(aConnector.GetTargetedIBOElement().Name))
                {
                    TheStoredSource = AStoredSource;
                    break;
                }
            }
            if (TheStoredSource == null)
            {
                TheStoredSource = new RoleStorage(aConnector.GetTargetedIBOElement().Name);
                SourceRoleList.Add(TheStoredSource);
            }

            if (TheStoredSource.AddRole(aConnector.GetEAFormatRole(aConnector.GetClientRoleQualifier(), aConnector.GetClientRole())).Equals(false))
            {
                MessageBox.Show("You can't have several connectors with the exact same role targeting an element ( " + aConnector.GetEAFormatRole(aConnector.GetClientRoleQualifier(), aConnector.GetClientRole()) + " ).\nChange a qualifier or role and try again.", "Qualifier Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            //End source

            return true;
        }

        private bool CheckRole()
        {
            TargetRoleList = new ArrayList();
            SourceRoleList = new ArrayList();

            foreach (EditEAClassConnector aConnector in ConnectorsList)
            {
                if (aConnector.GetSubConnectors().Count == 0)
                {
                    if(aConnector.GetSelectedState().Equals(true)){

                        if (SubCheckRole(aConnector).Equals(false))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    foreach (EditEAClassConnector aSubConnector in aConnector.GetSubConnectors())
                    {
                        if(aSubConnector.GetSelectedState().Equals(true))
                        {

                            if (SubCheckRole(aSubConnector).Equals(false))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private ArrayList CheckSubConnectors()
        {
            ArrayList AffectedConnectors = new ArrayList();
            foreach (EditEAClassConnector AConnecor in ConnectorsList)
            {
                if (!(AConnecor.GetEditedConnector() == null))
                {
                    if (AConnecor.CheckIfSameConnector().Equals(false))
                    {


                        for (short i = 0; AConnecor.GetSelectedIBOElement().Connectors.Count > i; i++)
                        {
                            EA.Connector APossibleIBOConnector = (EA.Connector)AConnecor.GetSelectedIBOElement().Connectors.GetAt(i);
                            if (APossibleIBOConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower()) || (APossibleIBOConnector.Stereotype.ToLower().Equals(("<<" + CD.GetIsBasedOnStereotype() + ">>").ToLower())))
                            {
                                if (!APossibleIBOConnector.ClientID.Equals(AConnecor.GetSelectedIBOElement().ElementID))
                                {
                                    EA.Element AChildElement = Repo.GetElementByID(APossibleIBOConnector.ClientID);
                                    if (AChildElement.Connectors.Count > 0)
                                    {
                                        for (short j = 0; AChildElement.Connectors.Count > j; j++)
                                        {
                                            EA.Connector APossibleImpactedConnector = (EA.Connector)AChildElement.Connectors.GetAt(j);
                                            if (APossibleImpactedConnector.TaggedValues.Count > 0)
                                            {
                                                for (short k = 0; APossibleImpactedConnector.TaggedValues.Count > k; k++)
                                                {
                                                    EA.ConnectorTag ATag = (EA.ConnectorTag)APossibleImpactedConnector.TaggedValues.GetAt(k);
                                                    if (ATag.Name.Equals(CD.GetIBOTagValue()))
                                                    {
                                                        if (ATag.Value.Equals(AConnecor.GetEditedConnector().ConnectorGUID))
                                                        {
                                                            AffectedConnectors.Add(APossibleImpactedConnector);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return AffectedConnectors;
        }

        private void ButDupplicateConnectors_Click(object sender, EventArgs e)
        {
           //  util = new utilitaires.Utilitaires();// pour test am aug 2016

            foreach (EditEAClassConnector AConnector in ConnectorsList)
                if (Main.valtest1 != null) // am aug 2016
                {

                    util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), "Eduplicate ligne 480");
                }
            this.ButEditSubConnector.Enabled = true; // am juil 2016 
            if (CheckRole().Equals(false))
            {
                ta.stop();
                ta.quit(); //c'est la fin
                return;
            }

            ArrayList ConnectorToChange = CheckSubConnectors();
           
              if (!ConnectorToChange.Count.Equals(0))
            {
                DialogResult DResult = MessageBox.Show("There is connector(s) that have a GUID based on a connector that will be deleted or modified.\nDo you wish to see the list ?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (DResult.Equals(DialogResult.Cancel))
                {
                    ta.stop();
                    ta.quit(); //c'est la fin
                    return;
                }
                else if (DResult.Equals(DialogResult.Yes))
                {
                    ArrayList MessageBoxList = new ArrayList();
                    foreach (EA.Connector AConnector in ConnectorToChange)
                    {
                        MessageBoxList.Add("The connector from " + Repo.GetElementByID(AConnector.ClientID).Name + " to " + Repo.GetElementByID(AConnector.SupplierID).Name + " need to be updated");
                    }
                    // ABA20230401 DetailMessageBox DMB = new DetailMessageBox("List of connectors that will have his parent modified.", "You should delete these connectors or modify them to keep the integrity of the IsBasedOn chain.", MessageBoxList);
                    DResult = MessageBox.Show(@"Do you wish to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (DResult.Equals(DialogResult.No))
                    {
                        ta.stop();
                        ta.quit(); //c'est la fin
                        return;
                    }
                }
                ta.stop();
                ta.quit(); //c'est la fin
            }



            if (CBCopyConstraints.Checked.Equals(false))
            {
                foreach (EditEAClassConnector AConnector in ConnectorsList)
                {
                    AConnector.SetCopyConstraint(CBCopyConstraints.Checked);
                }
            }
            util = new utilitaires.Utilitaires(Repo);// pour test am aug 2016

            foreach (EditEAClassConnector AConnector in ConnectorsList)
            {
                if (Main.valtest1 != null) // am aug 2016
                {

                    util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), "Eduplicate ligne 529");
                }
                AConnector.ExecuteCheckConnector();
                if (Main.valtest1 != null) // am aug 2016
                {

                    util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), "Eduplicate ligne 535");
                }
            }
  
            if(Main.valtest1 != null) // am aug 2016
            {
                
                util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1),"Eduplicate ligne 542");
            }

            ta.stop();
            ta.quit(); //c'est la fin
            this.Dispose(); // am aug 2016
            
        }

        private void LVConnectors_ItemChecked(object sender, EventArgs e)
        {
            foreach (EditEAClassConnector AConnector in ConnectorsList)
            {
                foreach (ListViewItem ALvi in LVConnectors.Items)
                {
                    if ((AConnector.GetGUID().Equals(ALvi.SubItems[4].Text)) && (AConnector.GetTargetedIBOElement().ElementID.ToString().Equals(ALvi.SubItems[5].Text))) // am mars 2016
                    {
                        AConnector.SetSelectedState(ALvi.Checked);
                        if (ALvi.Checked.Equals(true))
                        {
                            LVSubConnectors.Enabled = true;
                        }
                    }
                }
            }
            foreach (ListViewItem ALvi in LVSubConnectors.Items)
            {
                ALvi.Remove();
            }
            LVConnectors.SelectedItems.Clear();
        }

        private void LVConnectors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVConnectors.SelectedItems.Count.Equals(1))
            {
                if (LVConnectors.SelectedItems[0].Checked.Equals(true))
                {
                    LVSubConnectors.Enabled = true;
                    foreach (ListViewItem AnItem in LVSubConnectors.Items)
                    {
                        AnItem.Remove();
                    }

                    LabParentConnectorName.Text = "Detail of the connector n° : " + LVConnectors.SelectedItems[0].SubItems[0].Text;

                    ArrayList SubConnectors = new ArrayList();
                    SubConnectors = this.GetConnectorFromUI().GetSubConnectors();

                    ListViewItem lvi;
                    String[] aHeaders = new string[6];
                    int i = 0;
                    foreach (EditEAClassConnector ASubConnector in SubConnectors)
                    {
                        aHeaders[0] = i.ToString();
                        string prov1 = ASubConnector.GetSelectedElementConnector().ElementGUID;
                        string prov11 = ASubConnector.GetEditedConnector().ClientEnd.Role;
                        string prov2 = this.GetSelectedItem().ElementGUID;
                        long prov22 = this.GetSelectedItem().ElementID;
                        long prov3=ASubConnector.GetOriginalConnector().ClientID;
                        long prov4= ASubConnector.GetSelectedElementConnector().ElementID;
                        long prov5 = ASubConnector.GetOriginalConnector().ClientID;
                  
                        if (ASubConnector.GetEditedConnector() == null) throw new Exception("Error connector null");
                        if (
                        (IBOSelectedItem.ElementID == ASubConnector.GetEditedConnector().ClientID)
                        &&
                        (ASubConnector.GetEditedConnector().ClientEnd.Role.Contains(ASubConnector.GetClientRole()))// client side   
                                )
                        {
                            if (!ASubConnector.GetClientRoleQualifier().Equals(""))
                            {
                                aHeaders[1] = ASubConnector.GetClientRoleQualifier() + "_" + ASubConnector.GetClientRole();
                            }
                            else
                            {
                                aHeaders[1] = ASubConnector.GetClientRole();
                            }
                            if (!ASubConnector.GetSupplierRoleQualifier().Equals(""))
                            {
                                aHeaders[2] = ASubConnector.GetSupplierRoleQualifier() + "_" + ASubConnector.GetSupplierRole();
                            }
                            else
                            {
                                aHeaders[2] = ASubConnector.GetSupplierRole();
                            }
                        }
                        else
                        {
                            if (!ASubConnector.GetSupplierRoleQualifier().Equals(""))
                            {
                                aHeaders[1] = ASubConnector.GetSupplierRoleQualifier() + "_" + ASubConnector.GetSupplierRole();
                            }
                            else
                            {
                                aHeaders[1] = ASubConnector.GetSupplierRole();
                            }

                            if (!ASubConnector.GetClientRoleQualifier().Equals(""))
                            {
                                aHeaders[2] = ASubConnector.GetClientRoleQualifier() + "_" + ASubConnector.GetClientRole();
                            }
                            else
                            {
                                aHeaders[2] = ASubConnector.GetClientRole();
                            }
                        }
                        
                        /*
                        // si l'element profil selecte est le client de la connextion dans le profil

                        if(
                        (ASubConnector.GetEditedConnector().ClientID==ASubConnector.GetSelectedIBOElement().ElementID)
                        )
                        {
                            aHeaders[1] = util.RemoveQual(ASubConnector.GetEditedConnector().ClientEnd.Role);
                            aHeaders[2] = util.RemoveQual(ASubConnector.GetEditedConnector().SupplierEnd.Role);
                        }
                        else
                        {
                            aHeaders[1] = util.RemoveQual(ASubConnector.GetEditedConnector().SupplierEnd.Role);
                            aHeaders[2] = util.RemoveQual(ASubConnector.GetEditedConnector().ClientEnd.Role);
                        }

                        */
                        if (!ASubConnector.GetAgregate().Equals(""))
                        {
                            aHeaders[3] = "true";
                        }
                        else
                        {
                            aHeaders[3] = "false";
                        }

                        aHeaders[4] = ASubConnector.GetTargetedElementConnector().Name;
                        aHeaders[5] = ASubConnector.GetSubID().ToString();
                        lvi = new ListViewItem(aHeaders);
                        lvi.Checked = ASubConnector.GetSelectedState();
                        LVSubConnectors.Items.Add(lvi);
                        i++;

                    }

                }
            }
            else
            {
                LabParentConnectorName.Text = "Detail of the connector n° : No valid connector selected";
                foreach (ListViewItem AnItem in LVSubConnectors.Items)
                {
                    AnItem.Remove();
                }
            }
        }

        private void LVSubConnectors_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ListViewItem AnItem = ((ListView)sender).Items[e.Index];
            if (e.NewValue.Equals(CheckState.Checked))
            {
                this.GetConnectorFromUI().GetSubConnector(int.Parse(AnItem.SubItems[5].Text)).SetSelectedState(true);
            }
            else
            {
                this.GetConnectorFromUI().GetSubConnector(int.Parse(AnItem.SubItems[5].Text)).SetSelectedState(false);
            }
        }



        private void ButSubdivideAConnector_Click(object sender, EventArgs e)
        {
            if (!(GetConnectorFromUI() == null))
            {
                EditSubdivideForOptimConnectorForm SCF = new EditSubdivideForOptimConnectorForm(this, true, GetConnectorFromUI(), 0);
                SCF.ShowDialog();
            }
        }

        private void ButEditSubConnector_Click(object sender, EventArgs e)
        {
            if (!(GetConnectorFromUI() == null))
            {
                if (LVSubConnectors.SelectedItems.Count.Equals(1))
                {
                   // this.ButEditSubConnector.Enabled = false; // am juil 2016
                    EditSubdivideForOptimConnectorForm SCF = new EditSubdivideForOptimConnectorForm(this, false, GetConnectorFromUI(), int.Parse(LVSubConnectors.SelectedItems[0].SubItems[5].Text));
                    if (subok)
                    {
                        SCF.ShowDialog();
                    }
                    else
                    {
                        subok = true;
                    }
                }
            }
        }

        private void CBCopyConstraints_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CBDefaultParentConnector_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void CheckElligibleOptimParentElements()
        {
            Ta tb = new Ta("CheckElligibleParentElements");
            try
            {
                /*
                util = new utilitaires.Utilitaires(Repo);
                EA.Package profPack = util.getProfilePackage(Repo, IBOSelectedItem.PackageID);
                profElelements = new List<EA.Element>();

                util.GetAllPureProfElements(profPack, profElelements, true);// am oct 2016
                */
                foreach (EA.Element el in profElements)
                {
                    string parentguid = util.getEltParentGuid(el);

                    if (parentguid != "")
                    {
                        EA.Element parentel = Repo.GetElementByGuid(parentguid);
                        if (!profEltIDs.Contains(el.ElementID)) profEltIDs.Add(el.ElementID);

                        if (!dicIBOElemntByParent.ContainsKey(parentel.ElementID))
                        {
                            dicIBOElemntByParent.Add(parentel.ElementID, new List<EA.Element>());
                        }
                        if (!dicIBOElemntByParent[parentel.ElementID].Contains(el)) dicIBOElemntByParent[parentel.ElementID].Add(el);
                        if (!parentEltIDs.Contains(parentel.ElementID)) parentEltIDs.Add(parentel.ElementID);
                        //if (!dicPossibleElements.ContainsKey(parentel.ElementID)) dicPossibleElements.Add(parentel.ElementID, true);


                        ta.elapsetime("CheckElligibleParentElements");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(" pb in retreiving modelinfo elements from profile elements   " + e.Message);
            }
            tb.stop();
        }
        /// <summary>
        /// The elligible modelinfo connectors link elements of the modelinfo which have an IBOElement in the profile
        /// this program collects them
        /// look for all ancestors of all classes parent of a class in the profile
        /// this list of classes of the profile is kept in the list ProfElements <ElementID>>
        /// this list of ancestors is kept in a dictionary:
        /// dicIBOElemntByParent <ElementID,List<EA.ElementID>>
        /// la liste totale des ancetres possibles est gardée dans dicPossibleElements <elementid></elementid>
        /// 
        /// 
        /// </summary>
        private void  CheckElligibleParentElements(bool Optim)
        {
            Ta tb = new Ta("CheckElligibleParentElementsOptim");
            try
            {
                EA.Package profPack = util.getProfilePackage(Repo, IBOSelectedItem.PackageID);
               

                foreach (EA.Element el in profElements)
                {
                    string prov = el.Name;
                    string parentguid = util.getEltParentGuid(el);

                    if (parentguid != "")
                    {
                        EA.Element parentel = Repo.GetElementByGuid(parentguid);

                        if (!dicIBOElemntByParent.ContainsKey(parentel.ElementID))
                        {
                            dicIBOElemntByParent.Add(parentel.ElementID, new List<EA.Element>());
                        }
                        if (!dicIBOElemntByParent[parentel.ElementID].Contains(el)) dicIBOElemntByParent[parentel.ElementID].Add(el);
                        if(Optim)
                        {// on ne retient que le premier niveau de parent
                         if(!ElligibleElementIds.Contains(parentel.ElementID)) ElligibleElementIds.Add(parentel.ElementID);
                            
                        }else{
                        if(!dicProfParentElement.ContainsKey(el.ElementID)) dicProfParentElement[el.ElementID]=parentel;
                        if (!ElligibleElementIds.Contains(parentel.ElementID)) ElligibleElementIds.Add(parentel.ElementID);
                            
                        foreach (EA.Connector con in el.Connectors)
                        {
                            

                            if (
                                (con.Type == "Generalization")
                                &&
                                (con.ClientID == el.ElementID)
                                )
                            {
                                dicIBOInheritedbyElementID[el.ElementID] = 0;
                                dicIBOInheritedbyElementID[el.ElementID]=con.SupplierID;
                                
                                break; // pas la peine de continuer on a trouve ce qu'il fallait
                            }
                        }
                             
                        // we add ascendants to the possible analysed call of Model Info
                        List<long> ancestors = new List<long>();
                        //ancestors.Add(parentel.ElementID);  // on s'autoajoute dans la liste
                         util.getAncesters(parentel, ancestors);
                         //update of ElligibleElements  and update of dicIBOElemntByParent for all ancestors of parentel
                         long parentelid = parentel.ElementID;
                            foreach(long ancestor in ancestors)
                            {
                                if (!ElligibleElementIds.Contains(ancestor)) ElligibleElementIds.Add(ancestor);

                                if (!dicIBOElemntByParent.ContainsKey(ancestor))
                                {
                                    dicIBOElemntByParent[ancestor]=new List<EA.Element>();
                                }
                                foreach(EA.Element elt in dicIBOElemntByParent[parentel.ElementID])
                                {
                                if (!dicIBOElemntByParent[ancestor].Contains(elt)) dicIBOElemntByParent[ancestor].Add(elt);
                                }
                            }
                        }
                         
                    ta.elapsetime("CheckElligibleParentElements13");
                } 
            }
            }
            catch (Exception e)
            {
                MessageBox.Show(" pb in retreiving modelinfo elements from profile elements   " + e.Message + "\n" + e.StackTrace);
            }
            tb.stop();
        }
        /// <summary>
        /// The elligible modelinfo connectors link elements of the modelinfo which have an IBOElement in the profile
        /// this program collects them
        /// look for all ancestors of all classes parent of a class in the profile
        /// this list of classes of the profile is kept in the list ProfElements <ElementID>>
        /// this list of ancestors is kept in a dictionary:
        /// dicIBOElemntByParent <ElementID,List<EA.ElementID>>
        /// la liste totale des ancetres possibles est gardée dans dicPossibleElements <elementid></elementid>
        /// 
        /// 
        /// </summary>
        private void CheckElligibleParentElements()
        {
           Ta tb=new Ta("CheckElligibleParentElements");
            try{

             util = new utilitaires.Utilitaires(Repo);
            EA.Package profPack=util.getProfilePackage(Repo,IBOSelectedItem.PackageID);
            profElements = new List<EA.Element>();
 
            util.GetAllPureProfElements(profPack, profElements,true);// am oct 2016

            foreach (EA.Element el in profElements)
            {
                string parentguid = util.getEltParentGuid(el);

                if(parentguid != "")
                {
                    EA.Element parentel = Repo.GetElementByGuid(parentguid);
                    if(!profEltIDs.Contains(el.ElementID)) profEltIDs.Add(el.ElementID);
                
                    if (!dicIBOElemntByParent.ContainsKey(parentel.ElementID))
                    {
                        dicIBOElemntByParent.Add(parentel.ElementID, new List<EA.Element>());
                    }
                    if (!dicIBOElemntByParent[parentel.ElementID].Contains(el)) dicIBOElemntByParent[parentel.ElementID].Add(el);
                    if (!parentEltIDs.Contains(parentel.ElementID)) parentEltIDs.Add(parentel.ElementID);
                    //if (!dicPossibleElements.ContainsKey(parentel.ElementID)) dicPossibleElements.Add(parentel.ElementID, true);
                    /*
                    util.wlog("TEST", " CheckElligibleParent add dans dicPossibleElements  ParentelID=" 
                             + parentel.ElementID.ToString());// am pour test
                     * */
                    // we add descendants and ascendants to the possible analysed call of Model Info
                    List<long> ancestors=new List<long>();
                    List<long> descendants=new List<long>();
                   // ta.elapsetime("b776");
                   //am 18 util.getAncesters(parentel, ancestors);
                   //am 18 util.getAncesters(parentel, ancestors);
                    
                  //  ta.diffelaps("a776");
                   // util.getDescendants(parentel, descendants);
                   // ta.diffelaps("a778");
                    /*
                     foreach (long id in ancestors)
                    {
                        if (!dicPossibleElements.ContainsKey(id))
                        {
                            dicPossibleElements.Add(id, true);
                           
                        }
              
                      
                    }
                     * */
                    foreach(long id in descendants)
                    {
                        if (!dicPossibleElements.ContainsKey(id))
                        {
                            dicPossibleElements.Add(id, true);
                            /*
                            util.wlog("TEST", " CheckElligibleParent add dans dicPossibleElements  ElementID=" + id.ToString()
                               + "  pour parentelID=" + parentel.ElementID.ToString());// am pour test
                             * */
                        }
                    }
                   if(!dicPossibleElements.ContainsKey(parentel.ElementID)) dicPossibleElements.Add(parentel.ElementID,true);
                }

                ta.elapsetime("CheckElligibleParentElements");
            }
            }catch(Exception e)
            {
                MessageBox.Show(" pb in retreiving modelinfo elements from profile elements   " + e.Message);
            }
            tb.stop();
        }

        /// <summary>
        /// the connexions of the first level have been already treated 
        /// for all the possible connexion we explore the possible inherited connexion
        /// for
        /// 
        /// </summary>
        void  traitAllInheritedconnexion(EA.Element IBOSelectedElt)
        {
            List<long> ancestors = utilitaires.Utilitaires.dicAncestors[ParentSelectedItem.ElementID];
            List<long> localElligibles = new List<long>();
            foreach(long ancetreid in ancestors)
            {
                traitOneInheritedConnexion(ancetreid,IBOSelectedElt); 
            }
        }

        private void traitOneInheritedConnexion(long ancetreid,EA.Element IBOSelectedElt)  
        {
            EA.Element ancetre = Main.Repo.GetElementByID((int)ancetreid);
            
          // iterrates on all possible connectors for the ancetre of  parentdelecteditemf   
            EA.Element otherend = null;
            long otherendid = 0;
            bool swap = false;
            int i = 0;//pour test
            foreach (EA.Connector con  in  ancetre.Connectors)
            {
                i++;//pour test
                if (dejatraite.Contains(con.ConnectorID)) continue; // the connector is already treated
                if ((con.Type != "Generalization") && (con.Type != "Dependency"))
                {
                    dejatraite.Add(con.ConnectorID);
                    string conguid = con.ConnectorGUID; // guid of connector between parent elements
                    if (con.ClientID == ancetreid) // l'element est-il la source?
                    {
                        swap = false;
                        otherendid = con.SupplierID;

                    }
                    else
                    {
                        swap = true;
                        otherendid = con.ClientID;
                    }

                    if (!ElligibleElementIds.Contains(otherendid)) continue;
                     ArrayList PotentialEditedConnectorList = new ArrayList(); ;// tous les connecteurs du profil qui sont IBO du connecteur parent
                                                                                // On doit se  poser la  question de savoir si un des connnecteurs potentiels a comme extremite le Ie IBOOtherend
                    if (dicProfParentGuidList.ContainsKey(con.ConnectorGUID)) PotentialEditedConnectorList = dicProfParentGuidList[con.ConnectorGUID];
                    otherend = Main.Repo.GetElementByID((int)otherendid);
                    List<EA.Element> elts =dicIBOElemntByParent[otherendid] ; // 
                    //ATTENTION à ce niveau regarder le lien avec le IBO BasedOn de ancetreid
                    if (elts.Count != 0)
                    {
                        foreach (EA.Element elt in elts)
                        {
                            EA.Element IBOotherend = elt;
                            // Generation de la liste de connection puisque con est elligible
                            EditedConnectorList = new ArrayList();
                            // if (dicProfParentGuidList.ContainsKey(con.ConnectorGUID)) EditedConnectorList = dicProfParentGuidList[con.ConnectorGUID];
                            //  ArrayList PotentialEditedConnectorList = new ArrayList(); ;// tous les connecteurs du profil qui sont IBO du connecteur parent
                            // On doit se  poser la  question de savoir si un des connnecteurs potentiels a comme extremite le Ie IBOOtherend
                            //  if (dicProfParentGuidList.ContainsKey(con.ConnectorGUID)) PotentialEditedConnectorList = dicProfParentGuidList[con.ConnectorGUID]; //EditedConnectorList = dicProfParentGuidList[con.ConnectorGUID];

                            foreach (EA.Connector co in PotentialEditedConnectorList)
                            {
                                /*
                                if (
                                    (co.ClientID == IBOotherend.ElementID)
                                    ||
                                    (co.SupplierID == IBOotherend.ElementID)
                                    )
                                    */
                                if (

                            ((ancetre.ElementID != otherendid) // this is not a self association
                             &&
                            (
                            ((co.ClientID == IBOotherend.ElementID) && (co.SupplierID == IBOSelectedItem.ElementID))
                             ||
                             ((co.SupplierID == IBOotherend.ElementID) && (co.ClientID == IBOSelectedItem.ElementID))
                             )
                             )
                             ||
                              ((ancetre.ElementID == otherendid) // this is  a self association
                                &&
                                    ((IBOSelectedItem.ElementID != IBOotherend.ElementID) // the profile elements are different
                                       &&
                                    ((co.ClientID == IBOotherend.ElementID)
                                     ||
                                    (co.SupplierID == IBOotherend.ElementID))))
                              ||
                               ((ancetre.ElementID == otherendid) // this is  a self association
                                &&
                                    ((IBOSelectedItem.ElementID == IBOotherend.ElementID) // the profile elements are equal
                                       &&
                                    (co.ClientID == co.SupplierID)))
                             )
                                {
                                    EditedConnectorList.Add(co);
                                }
                                else
                                {
                                    continue;
                                }

                            }

                            EditEAClassConnector ANewConnector = new EditEAClassConnector(Main.Repo, false, 0, con.ConnectorGUID, ancetre, otherend, IBOSelectedItem, IBOotherend, swap, EditedConnectorList);

                            // string prov="ancetreid=" + ancetreid + " otherendid" + otherend.ElementID.ToString() + " IBOSelectedItemid=" + IBOSelectedItem.ElementID.ToString() + " IBOotherend" + IBOotherend.Name;
                            // prov="targetedIBD=" +  ANewConnector.GetTargetedIBOElement().ElementID.ToString() + " name=" + ANewConnector.GetTargetedIBOElement().Name;
                            
                            
                            long inherited = 0;
                            if (dicIBOInheritedbyElementID.ContainsKey(IBOSelectedElt.ElementID)) inherited = dicIBOInheritedbyElementID[IBOSelectedElt.ElementID];

                            if (inherited != 0)
                            {

                                foreach (EA.Connector co in PotentialEditedConnectorList)
                                {
                                    if ((co.ClientID) == inherited || (co.SupplierID == inherited)) ANewConnector.SetInherited(true);
                                   
                                }
           
                            }
                            ConnectorsList.Add(ANewConnector);

                        }


                    }
                }
            }
        }

        private List<EA.Element> getProfElementFromHierarchy(long otherendid)
        {
            List<EA.Element> ret = new List<EA.Element>();
            foreach(long eltid in dicProfParentElement.Keys) // we look in all hierarchies of profile elements
            {
                List<long> ancetres = utilitaires.Utilitaires.dicAncestors[eltid];
                foreach(long ancid in ancetres )
                {
                    if(ancid==otherendid) // we found one
                    {
                        if (!ret.Contains(Main.Repo.GetElementByID((int)ancid))) ret.Add(Main.Repo.GetElementByID((int)ancid));
                    }
                }
            }
            return ret;
        }
        void CheckEligibleOptimOwnConnectors()  // am
        {
            Ta tb = new Ta("CheckEligibleOwnConnectors");
            for (short i = 0; ParentSelectedItem.Connectors.Count > i; i++) // for all connectors of the selected class (in info model) 21/10/15
            {
                EA.Connector AConnector = (EA.Connector)ParentSelectedItem.Connectors.GetAt(i);
                //if (!AConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower())) // non isbasedon dependency
              //  {
                    if ((!AConnector.Type.Equals("Generalization")) && (!AConnector.Type.Equals("Dependency"))) //non a generalization
                    {
                        TargetChildClass = new ArrayList(); // recieves the targets
                        SourceChildClass = new ArrayList(); // receives the source
                        bool Switch = false;
                        if ((AConnector.ClientID.Equals(AConnector.SupplierID))) // am 20/10/15 it is a self_association
                        {
                            SourceChildClass.Add(AConnector.ClientID);
                            TargetChildClass.Add(AConnector.ClientID);

                        }
                        else
                        {
                            if ((!AConnector.ClientID.Equals(ParentSelectedItem.ElementID)))
                            {
                                Switch = true;
                                SourceChildClass.Add(AConnector.SupplierID);

                                TargetChildClass.Add(AConnector.ClientID);
                            }

                            else if (!AConnector.SupplierID.Equals(ParentSelectedItem.ElementID))
                            {
                                Switch = false;
                                SourceChildClass.Add(AConnector.ClientID);
                                TargetChildClass.Add(AConnector.ClientID);

                            }

                        }

                        foreach (int ATargetClassID in TargetChildClass)
                        {
                            foreach (int ASourceClassID in SourceChildClass)
                            {
                                EA.Element IBOTargetedItem = null;

                                /// <param name="Repo"></param>
                                /// <param name="SubConnector">true if it's a subconnector</param>
                                /// <param name="SubID">The ID of the SubConnector</param>
                                /// <param name="ConnectorGUID">The Original connectors GUID to dupplicate</param>
                                /// <param name="SelectedElementConnector">The element of the object on the connector's selected side</param>
                                /// <param name="TargetedElementConnector">The element of the object on the connector's targeted (other end) side</param>
                                /// <param name="SelectedIBOElement">The element of the IBO's user selected object</param>
                                /// <param name="TargetedIBOElement">The element of the IBO's targeted (other end) object</param>


                                if (dicIBOElemntByParent.ContainsKey(ATargetClassID))// Is there a class based on the targetclass?
                                {
                                    List<EA.Element> targets = dicIBOElemntByParent[ATargetClassID];
                                    foreach (EA.Element tt in targets) // in Optim there is only one element possible
                                    {
                                        IBOTargetedItem = tt;
                                        //                                   
                                        ArrayList EditedConnectorList = new ArrayList();
                                        for (short k = 0; IBOTargetedItem.Connectors.Count > k; k++)
                                        {
                                            //public EditEAClassConnector(EA.Repository Repo, bool SubConnector, int SubID, string ConnectorGUID, EA.Element SelectedElementConnector, EA.Element TargetedElementConnector, EA.Element SelectedIBOElement, EA.Element TargetedIBOElement, bool Switch, ArrayList EditedConnectorList)

                                            if ((IBOSelectedItem.ElementID.Equals(((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).SupplierID)) || ((IBOSelectedItem.ElementID.Equals(((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).ClientID))))
                                            {
                                                for (short l = 0; ((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).TaggedValues.Count > l; l++)
                                                {
                                                    if (((EA.ConnectorTag)((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).TaggedValues.GetAt(l)).Value.Equals(AConnector.ConnectorGUID))
                                                    {
                                                        EditedConnectorList.Add(((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)));  // this connector already exists in the profile and can be edited
                                                    }
                                                }
                                            }

                                        }
                                        //

                                        EditEAClassConnector ANewConnector = new EditEAClassConnector(Repo, false, 0, AConnector.ConnectorGUID, Repo.GetElementByID(ASourceClassID), Repo.GetElementByID(ATargetClassID), IBOSelectedItem, IBOTargetedItem, Switch, EditedConnectorList);
                                        ConnectorsList.Add(ANewConnector);
                                    }
                                }
                                else
                                {

                                    ConnectorsListWithoutIBO.Add(Repo.GetElementByID(ATargetClassID).Name);
                                }
                            }
                        }
                    }

               // }
            }

            tb.stop();
        }

        /// <summary>
        /// collect all the elligible connectors 
        /// look all the connectors which are not a dependency or generlalization
        /// for each found connection we record its  supplierID in TargetChildClass
        /// for each found connection we record its  ClientID in TargetChildClass
        /// When the clientID does ot coincide with the selecteditem the swith is positionned to true
        /// </summary>
        private void CheckEligibleOwnConnectors() // am
        {
           Ta tb=new Ta("CheckEligibleOwnConnectors");
            for (short i = 0; ParentSelectedItem.Connectors.Count > i; i++) // for all connectors of the selected class (in info model) 21/10/15
            {
                EA.Connector AConnector = (EA.Connector)ParentSelectedItem.Connectors.GetAt(i);
                if (!AConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower())) // non isbasedon dependency
                {
                    if (!AConnector.Type.Equals("Generalization")) //non a generalization
                    {
                        TargetChildClass = new ArrayList();
                        SourceChildClass = new ArrayList();
                       /* // pour Optim
                        if (
                             (!dicPossibleElements.ContainsKey(AConnector.ClientID)) || (!dicPossibleElements.ContainsKey(AConnector.SupplierID))
                            )
                        {
                            continue; //am  the connector is not legitime
                        }
                        */
                        bool Switch = false;
                        if ((AConnector.ClientID.Equals(AConnector.SupplierID))) // am 20/10/15 it is a self_association
                        {
                            SourceChildClass.Add(AConnector.ClientID);
                            CheckGeneralizeOppositeElement(Repo.GetElementByID(AConnector.ClientID), false);

                        }
                        else
                        {
                            if ((!AConnector.ClientID.Equals(ParentSelectedItem.ElementID))) 
                            {
                                Switch = true;
                                SourceChildClass.Add(AConnector.SupplierID);

                                CheckGeneralizeOppositeElement(Repo.GetElementByID(AConnector.ClientID), false);
                            }
                     
                            else if (!AConnector.SupplierID.Equals(ParentSelectedItem.ElementID)) 
                            {
                                Switch = false;
                                SourceChildClass.Add(AConnector.ClientID);
                                CheckGeneralizeOppositeElement(Repo.GetElementByID(AConnector.SupplierID), true);

                            }
                            
                        }
                        
                        foreach (int ATargetClassID in TargetChildClass)
                        {
                            foreach (int ASourceClassID in SourceChildClass)
                            {
                                EA.Element IBOTargetedItem = null;
                               
                                /// <param name="Repo"></param>
                                /// <param name="SubConnector">true if it's a subconnector</param>
                                /// <param name="SubID">The ID of the SubConnector</param>
                                /// <param name="ConnectorGUID">The Original connectors GUID to dupplicate</param>
                                /// <param name="SelectedElementConnector">The element of the object on the connector's selected side</param>
                                /// <param name="TargetedElementConnector">The element of the object on the connector's targeted (other end) side</param>
                                /// <param name="SelectedIBOElement">The element of the IBO's user selected object</param>
                                /// <param name="TargetedIBOElement">The element of the IBO's targeted (other end) object</param>
                              

                                if (dicIBOElemntByParent.ContainsKey(ATargetClassID))
                                {
                                    List<EA.Element> targets = dicIBOElemntByParent[ATargetClassID];
                                    foreach (EA.Element tt in targets)
                                    {
                                        IBOTargetedItem = tt;
                                        //                                   
                                        ArrayList EditedConnectorList = new ArrayList();
                                        for (short k = 0; IBOTargetedItem.Connectors.Count > k; k++)
                                        {
                                            //public EditEAClassConnector(EA.Repository Repo, bool SubConnector, int SubID, string ConnectorGUID, EA.Element SelectedElementConnector, EA.Element TargetedElementConnector, EA.Element SelectedIBOElement, EA.Element TargetedIBOElement, bool Switch, ArrayList EditedConnectorList)

                                            if ((IBOSelectedItem.ElementID.Equals(((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).SupplierID)) || ((IBOSelectedItem.ElementID.Equals(((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).ClientID))))
                                            {
                                                for (short l = 0; ((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).TaggedValues.Count > l; l++)
                                                {
                                                    if (((EA.ConnectorTag)((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).TaggedValues.GetAt(l)).Value.Equals(AConnector.ConnectorGUID))
                                                    {
                                                        EditedConnectorList.Add(((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)));
                                                    }
                                                }
                                            }

                                        }
                                        //

                                        EditEAClassConnector ANewConnector = new EditEAClassConnector(Repo, false, 0, AConnector.ConnectorGUID, Repo.GetElementByID(ASourceClassID), Repo.GetElementByID(ATargetClassID), IBOSelectedItem, IBOTargetedItem, Switch, EditedConnectorList);
                                        ConnectorsList.Add(ANewConnector);
                                    }
                                }
                                else
                                {

                                    ConnectorsListWithoutIBO.Add(Repo.GetElementByID(ATargetClassID).Name);
                                }     
                            }
                        }
                    }

                }
            }
            
            tb.stop();
        }

        /// <summary>
        /// this explore the opposite side of the selectedElement for a given 
        /// </summary>
        /// <param name="AnElement">The element to newCheck</param>
        /// <param name="Target">if it's a target or a source (true = target/false = source)</param>
        public void CheckGeneralizeOppositeElement(EA.Element AnElement, bool Target) //am
        {
            
           // util.wlog("TEST", "CheckGeneralizedOppositeElement el=" + AnElement.Name);// am janj 2016
             
            Ta tb=new Ta("CheckGeneralizeOppositeElement");// am pour test
            if (!dicPossibleElements.ContainsKey(AnElement.ElementID))
            {
                
              //  util.wlog("TEST", "return for AnElementID=" + AnElement.ElementID.ToString());
                 
                return; // it is not necessary to explore this possibility because no element of the hierarchy is elligible
            }
            TargetChildClass.Add(AnElement.ElementID);
            /* //on s'arrete au premier element pour Optim
            for (short i = 0; AnElement.Connectors.Count > i; i++)
            {
                EA.Connector AConnector = (EA.Connector)AnElement.Connectors.GetAt(i);
                
                if (AConnector.Type.Equals("Generalization"))
                {
                   // if (!parentEltIDs.Contains(AConnector.ClientID)) continue; //am it is impossible to keep this connector
                    if (!AConnector.ClientID.Equals(AnElement.ElementID))
                    {
                        CheckGeneralizeOppositeElement(Repo.GetElementByID(AConnector.ClientID), Target);
                    }
                }
             
                 
            }
            **/
            tb.stop();
        }

        //ATTENTION il y possibilité d'améliorer cet algorithme en adoptant getancesters
        /// <summary>
        /// SourceChildClass va contenir la hierarchie complete d'un élément (ses ancêtres)
        /// </summary>
        /// <param name="AnElement"></param>
        /// <param name="FirstTime"></param>
        public void  
            CheckGeneralizeOwnElement(EA.Element AnElement, bool FirstTime)
        {
           Ta tb=new Ta("CheckGeneralizedOwnElement");
            if (FirstTime.Equals(false))
            {
                SourceChildClass.Add(AnElement.ElementID);
            }
            /* // on s'arrete au premeir element por Optim
            for (short i = 0; AnElement.Connectors.Count > i; i++)
            {
                EA.Connector AConnector = (EA.Connector)AnElement.Connectors.GetAt(i);
                if (AConnector.Type.Equals("Generalization"))
                {
                    if (!AConnector.SupplierID.Equals(AnElement.ElementID))
                    {
                        CheckGeneralizeOwnElement(Repo.GetElementByID(AConnector.SupplierID), false);
                    }
                }
            }
            */
            tb.stop();
            
        }


        private void CheckEligibleInheritedConnectors() //am janv 2016
        {
           Ta tb=new Ta("Begin CheckEligibleInheritedConnectors");     
            SourceChildClass = new ArrayList();
            TargetChildClass = new ArrayList();
            CheckGeneralizeOwnElement(ParentSelectedItem, true); //am  get all ancesters of ParentSelectedItem 
            ArrayList ParentClass = SourceChildClass;
            foreach (int AnElementID in ParentClass) // am for all ancesters of parentSelectedItem in InfoModel
            {
                EA.Element AnElement = Repo.GetElementByID(AnElementID); //am for this ancester
              //  util.wlog("TEST", " CheckEligibleInherited  boucle sur tous les ancetres de ElementID=" + AnElementID.ToString()); // am pour test
                for (short i = 0; AnElement.Connectors.Count > i; i++)
                {
                    EA.Connector AConnector = (EA.Connector)AnElement.Connectors.GetAt(i);
                    if (!AConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower())) //am for all associations which are not basedon or aggregation
                    {
                        if (!AConnector.Type.Equals("Generalization"))
                        {
                           // util.wlog("TEST", " CheckEligibleInherited  connecteur ClientID=" + AConnector.ClientID.ToString() + " , " + " SupplierID=" + AConnector.SupplierID.ToString());// am pour test
                            TargetChildClass = new ArrayList();
                            SourceChildClass = new ArrayList();
                            bool Switch = false;

                            if (!AConnector.ClientID.Equals(AnElementID)
                            || (AConnector.ClientID.Equals(AConnector.SupplierID)) // juil 11 //am cas du self
                                )
                            {
                                Switch = true;
                                SourceChildClass.Add(ParentSelectedItem.ElementID);
                                CheckGeneralizeOppositeElement(Repo.GetElementByID(AConnector.ClientID), false);
                            }
                            else if (!AConnector.SupplierID.Equals(AnElementID)
                                 || (AConnector.ClientID.Equals(AConnector.SupplierID)) // juil 11
                                )
                            {
                                Switch = false;
                                SourceChildClass.Add(ParentSelectedItem.ElementID);
                                CheckGeneralizeOppositeElement(Repo.GetElementByID(AConnector.SupplierID), true);
                            }
                            foreach (int ATargetClassID in TargetChildClass) // am 
                            {
                               // util.wlog("TEST", " CheckEligibleInherited  boucle sur tous les targets nb de target=" + TargetChildClass.Count.ToString()); // am pour test

                                foreach (int ASourceClassID in SourceChildClass)
                                {
                                   
                                    EA.Element IBOTargetedItem = null;
                                    /// <param name="Repo"></param>
                                    /// <param name="SubConnector">true if it's a subconnector</param>
                                    /// <param name="SubID">The ID of the SubConnector</param>
                                    /// <param name="ConnectorGUID">The Original connectors GUID to dupplicate</param>
                                    /// <param name="SelectedElementConnector">The element of the object on the connector's selected side</param>
                                    /// <param name="TargetedElementConnector">The element of the object on the connector's targeted (other end) side</param>
                                    /// <param name="SelectedIBOElement">The element of the IBO's user selected object</param>
                                    /// <param name="TargetedIBOElement">The element of the IBO's targeted (other end) object</param>
                                    EA.Element ATargetClass = Repo.GetElementByID(ATargetClassID);
                                   // for (short j = 0; ATargetClass.Connectors.Count > j; j++) //am 3
                                   // {  //am 3
                                      //  EA.Connector AnIBOConnector = (EA.Connector)Repo.GetElementByID(ATargetClassID).Connectors.GetAt(j);
                                       // if (AnIBOConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower())) //am 1
                                        //{ //am 1
                                           // if (!AnIBOConnector.ClientID.Equals(ATargetClassID)) //am 2
                                           // { // am 2
                                               // IBOTargetedItem = Repo.GetElementByID(AnIBOConnector.ClientID);
                                        //if (util.getEltParentGuid(ATargetClass) != "")
                                    if (dicIBOElemntByParent.ContainsKey(ATargetClassID))
                                    {
                                        List<EA.Element> targets = dicIBOElemntByParent[ATargetClassID];
                                        foreach (EA.Element tt in targets)
                                        {
                                            IBOTargetedItem = tt;
                                            ArrayList EditedConnectorList = new ArrayList();



                                            ///
                                            for (short k = 0; IBOTargetedItem.Connectors.Count > k; k++)
                                            {
                                                if ((IBOSelectedItem.ElementID.Equals(((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).SupplierID)) || ((IBOSelectedItem.ElementID.Equals(((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).ClientID))))
                                                {
                                                    //  for (short l = 0; IBOSelectedItem.Connectors.Count > l; l++)
                                                    //  {
                                                    //  for (short m = 0; ((EA.Connector)IBOSelectedItem.Connectors.GetAt(l)).TaggedValues.Count > m; m++
                                                    //{
                                                    for (short m = 0; ((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).TaggedValues.Count > m; m++)
                                                    {
                                                        // if (((EA.ConnectorTag)((EA.Connector)IBOSelectedItem.Connectors.GetAt(l)).TaggedValues.GetAt(m)).Value.Equals(AConnector.ConnectorGUID))
                                                        if (((EA.ConnectorTag)((EA.Connector)IBOTargetedItem.Connectors.GetAt(k)).TaggedValues.GetAt(m)).Value.Equals(AConnector.ConnectorGUID))
                                                        {
                                                            //EditedConnectorList.Add((EA.Connector)IBOSelectedItem.Connectors.GetAt(l));
                                                            EditedConnectorList.Add((EA.Connector)IBOTargetedItem.Connectors.GetAt(k));
                                                        }
                                                    }
                                                    //}
                                                }
                                            }
                                            ///



                                            //  EditEAClassConnector ANewConnector = new EditEAClassConnector(Repo, false, 0, AConnector.ConnectorGUID, Repo.GetElementByID(ASourceClassID), Repo.GetElementByID(ATargetClassID), IBOSelectedItem, Repo.GetElementByID(AnIBOConnector.ClientID), Switch, EditedConnectorList);
                                            EditEAClassConnector ANewConnector = new EditEAClassConnector(Repo, false, 0, AConnector.ConnectorGUID, Repo.GetElementByID(ASourceClassID), Repo.GetElementByID(ATargetClassID), IBOSelectedItem, IBOTargetedItem, Switch, EditedConnectorList); // am janv 2016

                                            for (short l = 0; IBOSelectedItem.Connectors.Count > l; l++)
                                            {
                                                if (((EA.Connector)IBOSelectedItem.Connectors.GetAt(l)).Type.Equals("Generalization"))
                                                {
                                                    if (((EA.Connector)IBOSelectedItem.Connectors.GetAt(l)).ClientID.Equals(IBOSelectedItem.ElementID))
                                                    {
                                                        for (short m = 0; (Repo.GetElementByID(((EA.Connector)IBOSelectedItem.Connectors.GetAt(l)).SupplierID)).Connectors.Count > m; m++)
                                                        {
                                                            for (short n = 0; ((EA.Connector)(Repo.GetElementByID(((EA.Connector)IBOSelectedItem.Connectors.GetAt(l)).SupplierID)).Connectors.GetAt(m)).TaggedValues.Count > n; n++)
                                                            {
                                                                if (((EA.ConnectorTag)((EA.Connector)(Repo.GetElementByID(((EA.Connector)IBOSelectedItem.Connectors.GetAt(l)).SupplierID)).Connectors.GetAt(m)).TaggedValues.GetAt(n)).Value.Equals(AConnector.ConnectorGUID))
                                                                {
                                                                    //checking is target is the same
                                                                    EA.Element OtherEndOfGeneralisation = Repo.GetElementByID(((EA.Connector)IBOSelectedItem.Connectors.GetAt(l)).SupplierID);
                                                                    if ((((EA.Connector)OtherEndOfGeneralisation.Connectors.GetAt(m))).ClientID.Equals(OtherEndOfGeneralisation.ElementID))
                                                                    {
                                                                        //if (((EA.Connector)OtherEndOfGeneralisation.Connectors.GetAt(m)).SupplierID.Equals(AnIBOConnector.ClientID))
                                                                        if (((EA.Connector)OtherEndOfGeneralisation.Connectors.GetAt(m)).SupplierID.Equals(IBOTargetedItem.ElementID)) // am janv 2016
                                                                        {
                                                                            ANewConnector.SetInherited(true);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        //MessageBox.Show("src =" + Repo.GetElementByID(((EA.Connector)OtherEndOfGeneralisation.Connectors.GetAt(m)).ClientID).Name + " target" + Repo.GetElementByID(AnIBOConnector.ClientID).Name, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1); 
                                                                        // if (((EA.Connector)OtherEndOfGeneralisation.Connectors.GetAt(m)).ClientID.Equals(AnIBOConnector.ClientID))
                                                                        if (((EA.Connector)OtherEndOfGeneralisation.Connectors.GetAt(m)).ClientID.Equals(IBOTargetedItem.ElementID)) //am janv 2016
                                                                        {
                                                                            ANewConnector.SetInherited(true);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            ConnectorsList.Add(ANewConnector);
                                        }
                                    }
                                    else
                                    {
                                        ConnectorsListWithoutIBO.Add(ATargetClass.Name);
                                    }
                                           // } // am 2

                                       // } //am 1
                                   // }//am 3
                                    /*
                                    if ((IBOTargetedItem == null))
                                    {
                                        ConnectorsListWithoutIBO.Add(Repo.GetElementByID(ATargetClassID).Name);
                                    }
                                     * */
                                }
                            }
                        }
                    }
                }
            }
            tb.stop();
        }

        private void EditDuplicateForOptimConnectorsForm_Load(object sender, EventArgs e)
        {

        }

/**********************************************************************************/
        //end of dialog
       

    }
}
