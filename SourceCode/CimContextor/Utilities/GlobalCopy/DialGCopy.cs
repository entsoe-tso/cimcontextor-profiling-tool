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
    public partial class DialGlobalCopy : Form
    {
        UtilitiesConstantDefinition CD = new UtilitiesConstantDefinition();
        //CimContextor.utilitaires.Utilitaires util = new Utilitaires();
        XMLParser xmlp = null;
        static public List<string> Cimdatatypes = new List<string>()
                                      {
                                        "Datatype",
                                        "CIMDatatype",
                                        "Primitive",
                                        "Compound",
                                        "enumeration",
                                        "Enumeration"
                                      };
        string typ = "";

        /// <summary>
        /// lance le dialogue avec le 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="thepack"></param>
        public DialGlobalCopy(EA.Repository repo,string type)
        {
           
            InitializeComponent();
            xmlp = new XMLParser(repo);
            typ = type;
            if (typ == "RecoverDtattypes")
            {            
                TBProfpack.Visible = false;
            }
            this.TBDomainPack.Text = xmlp.GetXmlValueProfData("EntsoeDataTypesDomain");
            this.TBProfpack.Text = xmlp.GetXmlValueProfData("ProfilesPackage");

            //this.button2.Enabled = false
        }
       
        private void DialGlobalCopy_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            { 
            
            Main.ongoing = true;
                if (typ != "RecoverDatatatypes")
                {
                    xmlp.SetXmlValueProfData("ProfilesPackage", TBProfpack.Text);
                }

                    xmlp.SetXmlValueProfData("EntsoeDataTypesDomain", TBDomainPack.Text);

                this.Dispose();
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw new Exception(" pb in DialGCopy ok " + ex.Message);
            }
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main.ongoing = false;
            this.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
         
           // this.Close();
           
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

       

        private void button4_click(object sender, EventArgs e)
        {
         
           // util.wlog("STATE", " click= " + "saisie_domainpack");
           // this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
        }
    
