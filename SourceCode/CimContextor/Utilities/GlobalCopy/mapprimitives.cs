using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using EA;
using CimContextor;
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





namespace CimContextor.Utilities

{
    /// <summary>
    /// attention on suppose que le paquetage enveloppant est à un niveau 
    /// au dessus du paquetage sélectionné
    /// </summary>
    public partial class Dialmapprimitives : Form
    {
        UtilitiesConstantDefinition CD = new UtilitiesConstantDefinition();
        List<string> selecteddatatypes1;
        List<EA.Element> listprofelements = new List<EA.Element>();
        Utilitaires util = null; // ABA20230228 new Utilitaires();
        static public List<string> Cimdatatypes = new List<string>()
                                      {
	                                    "Datatype",
                                        "CIMDatatype",
                                        "Primitive",
	                                    "Compound",
	                                    "enumeration",
                                        "Enumeration"
                                      };

    
     static public Dictionary<string,EA.Element> profdatatypes;// nom/element datatype
     public CheckedListBox dialbox
        {
            get
            {
                return this.checkedListBox1;
            }
            set
            {

            }
        }
        public List<string> selecteddatatypes
        {
            get
            {
                return this.selecteddatatypes1;
            }
            set
            {

            }
        }

      
        

        /// <summary>
        /// lance le dialogue avec le 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="thepack"></param>
        public Dialmapprimitives(EA.Repository repo, EA.Package profpack)
        {
            InitializeComponent();
            util = new Utilitaires(repo); // ABA20230228

            // Changes the selection mode from double-click to single click.
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.SelectionMode = SelectionMode.One;


            //initialise le contenu de la box avec tous les datatype du package 
            UtilitiesConstantDefinition CD = new UtilitiesConstantDefinition(); //ConstantDefinition CD = new ConstantDefinition();

            profdatatypes = new Dictionary<string, EA.Element>();
            selecteddatatypes1 = new List<string>();
            EA.Package profPackEnvelop = repo.GetPackageByID((int)profpack.ParentID);
            List<EA.Element> listEnvelopprofdatatypes = new List<EA.Element>();
            listprofelements = new List<EA.Element>();
            util.getAllElements(profPackEnvelop, listEnvelopprofdatatypes, Cimdatatypes);
            util.getAllElements(profpack, listprofelements, new List<string>(){"","Class"});
            foreach (EA.Element elt in listEnvelopprofdatatypes)
            {
                if (CD.GetFloatType() == util.RemoveQual(elt.Name))
                { // le datatype peut descendre de Float
                    if ((elt.Attributes.Count == 1) && (((EA.Attribute)elt.Attributes.GetAt(0)).Name  == "value"))
                    {
                        string paname = repo.GetPackageByID((int)elt.PackageID).Name;
                        if (!profdatatypes.ContainsKey(paname + "::" + elt.Name))
                        {

                            //selecteddatatypes1.Add(paname + "::" + elt.Name);
                            checkedListBox1.Items.Add(paname + "::" + elt.Name);
                            profdatatypes.Add(paname + "::" + elt.Name, elt);

                        }

                    }
                }
            }
            this.checkedListBox1.SelectionMode = SelectionMode.One;
        }
       
        private void Dialmapprimitives_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main.ongoing = true;

            StringBuilder ss = new StringBuilder();
            selecteddatatypes1 = new List<string>();
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                if (this.checkedListBox1.GetItemChecked(i))
                {
                    // MessageBox.Show(checkedListBox1.Items[i].ToString());
                    ss.Append(checkedListBox1.Items[i].ToString());
                    selecteddatatypes1.Add(checkedListBox1.Items[i].ToString());


                }
            }
            if(selecteddatatypes1.Count >0)
            {
                string nomselecte = selecteddatatypes1[0];
                long Idselecte=0;
                if(profdatatypes.ContainsKey(nomselecte))
                    {
                   Idselecte=profdatatypes[nomselecte].ElementID;
                }
                 MessageBox.Show(ss.ToString());
                 string Type = "";
                 string[] split;
                 if (Idselecte != 0)
                 {
                     // on regarde si une value refere Float
                     char[] c = { ':' };
                     split =nomselecte.Split(c);
                     if (split.Length == 3)
                     {
                         Type = split[2];
                     }
                     else
                     {
                         MessageBox.Show(" the value in refers an ill named type ");
                     }

                     
                 }

                foreach (EA.Element elt in listprofelements) 
                {
                   
                    string prov = elt.Name;
                    if (!Utilitaires.isNotPureElement(elt)) continue;
                    foreach (EA.Attribute at in elt.Attributes)
                    {
                        if (at.Type == CD.GetFloatType())
                        {
                         at.Type = Type;
                         at.ClassifierID = (int)Idselecte;
                         at.Update();
                         elt.Update();
                        }
                    }
                }
            }else{
                MessageBox.Show(" you must select at least one datatype");
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main.ongoing = false;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {

           
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textbox2_enter(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {



            }
        }

        
        private void label2_Click_1(object sender, EventArgs e)
        {

        }

       
    }
        }
    
