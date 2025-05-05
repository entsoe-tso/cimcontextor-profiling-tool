using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
//using System.Linq;
using System.Text;
//using CimExportImport.manageIBO;
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
namespace CimContextor.utilitaires
{
    /// <summary>
    ///  // create a profile from a profiles directory
    // we  try and select CIM packages to which we will be dependant and then 
    //  create a profilepackage under the clicked package
    /// </summary>
    class CreateProfile
    {
         
       // static string repofile = "";
        static public EA.Repository repository;
        string profname = ""; // the chosen name of the profile
        static EA.Package atop; // le model top
        static EA.Package profpack;
        static ConstantDefinition CD = new ConstantDefinition();
        XMLParser XMLP;
        
        public CreateProfile(EA.Repository repo,EA.Package pack)
        {

            atop = Utilitaires.GetModelPackage(pack, repo);  // we suppose that the container of the profiles is at the same level of the major CIM Packages
           // repofile = repo.ConnectionString;
            repository = repo;
            repo.RefreshModelView(0);
            XMLP = new XMLParser(repo);
 
            EA.Package profilespack = repo.GetTreeSelectedPackage();
            EA.Package parentpack = Utilitaires.GetModelPackage(profilespack, repo);
            
            try
            {
                pack.Packages.Refresh();
                /******************selection des packages based on******************************/
              //  EA.Package parentpack = Utilitaires.GetModelPackage(profpack, repo); // le package le plus eleve de la hierarchie

                DialTree dtree = new DialTree(repo, parentpack,true);
                dtree.cball = false;
                dtree.treeLabel = "Select CIM Packages on which the Profile Package is based on";
                dtree.ShowDialog();
              
                profname = dtree.profilename;
                dtree.Dispose(); // ABA20230401
                /************** creation package *****************/
                profpack = (EA.Package)pack.Packages.AddNew(profname, "Package");
                profpack.Update();
                string prov = profpack.Name;
                

                //foreach (string paname in selectedpackages)
                foreach (string paname in Utilitaires.dicSelectedPackage.Keys)
                {
                    EA.Connector con = (EA.Connector)profpack.Element.Connectors.AddNew("", "Dependency");
                    con.ClientID = profpack.Element.ElementID;
                    // con.SupplierID = packages[paname].Element.ElementID;
                    con.SupplierID = Utilitaires.dicSelectedPackage[paname].Element.ElementID;
                    con.Stereotype = CD.GetIsBasedOnStereotype();
                    con.Update();
                    profpack.Element.Connectors.Refresh();
                }
                EA.Diagram profdiag = (EA.Diagram)profpack.Diagrams.AddNew(profname, "Class");
                profdiag.Update();
                profpack.Diagrams.Refresh();
            }
            catch (Exception e)
            {
                string texte = "Error in creating the profile " + profname + e.Message;
                MessageBox.Show(texte);
            }
        }

        //-----------------------------------------------------------------------    }
       
    }
}