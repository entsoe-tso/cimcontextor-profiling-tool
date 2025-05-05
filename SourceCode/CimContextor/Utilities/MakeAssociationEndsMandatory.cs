using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.ComponentModel;
using CimContextor.Utilities;
//using CimSyntaxGen.manageIBO;
using EA;
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
    /// this class is to make 
    /// - all attributes of all elements of given packages mandatory
    /// -IN THE FUTURE
    ///   some AssociationEnds mandatory
    /// </summary>
    class MakeAssociationEndsMandatory
    {

        static public List<string> Cimstereos = new List<string>()
                                      {
	                                    "Class",
	                                    "Datatype",
                                        "CIMDatatype",
                                        "Primitive",
	                                    "Compound",
	                                    "enumeration",
                                        "ACC",
                                        "ABIE",
                                        "MBIE",
                                       };
        static public List<string> Cimdatatypes = new List<string>()
                                      {
	                                    "Datatype",
                                        "CIMDatatype",
                                        "Primitive",
	                                    "Compound",
	                                    "enumeration",
                                      };
        //static Dictionary<string, ArrayList> dicElemByName; //  name;ArrayList fo elements
        static List<EA.Element> listelem;
        static string repofile = "";
        static public EA.Repository repository;
       // string savDirpackage = ""; // the directory to save the profiles
        static string aPackageGuid; // the package to gobally copy
        static UtilitiesConstantDefinition constantes = new UtilitiesConstantDefinition();
       // static public Dictionary<string, Dictionary<string, EA.Element>> IBOelements = new Dictionary<string, Dictionary<string, EA.Element>>();
       // static public List<EA.Package> IBOpackages = new List<EA.Package>();
       // static List<long> condejatraite = new List<long>();// connexions deja traitees
       // static public Dictionary<string, long> profdatatypes;// nom/elementid datatype
        static public bool Errorspresent = false;
       static  public List<long> dejatraite = new List<long>(); // liste des connections ID deja traitees
       static public List<string> conguids=new List<string>(); // liste des connexion modifiées 
        static string texte = "";
        utilitaires.Utilitaires util;
        public MakeAssociationEndsMandatory(EA.Repository repo, EA.Package pack) // the constructor 
        {
            
            repofile = repo.ConnectionString;
            repository = repo;
            util = new utilitaires.Utilitaires(repo);
            repo.RefreshModelView(0);
            aPackageGuid = pack.PackageGUID;
            ConstantDefinition constantes = new ConstantDefinition();
            /* am avril 2019
            DialMakeAssociationEndsMandatory dialog = new DialMakeAssociationEndsMandatory();

            // Sets up the initial objects in the CheckedListBox.
            string[] packagesname = null;// a priori 
            Dictionary<string, EA.Package> packages = new Dictionary<string, EA.Package>();
            int i = 0;

         //   if (pack.Packages.Count != 0)
          //  {
                setpackages(repo,pack, packages);
                packagesname = new string[packages.Keys.Count];
                foreach (string n in packages.Keys)
                {
                    packagesname[i] = n;
                    i++;
                }
               
   
                dialog.dialbox.Items.AddRange(packagesname);
               
                 dialog.ShowDialog();
                 selectedpackages = dialog.selectedpackages;
           // }
           */
            try
            {
                DialTree dialtree = new DialTree(repo, pack, true);
                dialtree.ShowDialog();
                dialtree.textinput = false;
                dialtree.inputlabelvisible = false;
                dialtree.Dispose(); // ABA20230401
                EA.Package package = null;
                List<long> listelemids = new List<long>();
                listelem = new List<EA.Element>();
              //  foreach (string nompack in selectedpackages)
              foreach(string nompack in utilitaires.Utilitaires.dicSelectedPackage.Keys)
                {
                    //package = packages[nompack];
                    package = utilitaires.Utilitaires.dicSelectedPackage[nompack];
                    if (package == null) continue;
                    foreach (EA.Element elt in package.Elements)
                    {
                        if (!listelemids.Contains(elt.ElementID))
                        {
                            listelem.Add(elt);
                            listelemids.Add(elt.ElementID);
                        }
                    }
                }

                makethemmandatory(listelem);
              
            }
            catch (Exception e)
            {
                string texto = "Error  " + e.Message;
                MessageBox.Show(texto);
                util.wlog("MakeAssociationEndsMandatory",texto);
                Errorspresent = true;

            }
        }
    
       static  void setpackages(EA.Repository repo, EA.Package pack,Dictionary<string,EA.Package> packages)
        {
            string packparentname = repo.GetPackageByID((int)pack.ParentID).Name;
            string nom = packparentname + "::" + pack.Name;
            if(!packages.ContainsKey(nom)) packages.Add(nom,pack);
    
           //dejatraite=new List<long>();
           foreach (EA.Package pa in pack.Packages)
           {
               setpackages(repo, pa, packages);
               

           }
       
           
           
        }
   static    void makethemmandatory(List<EA.Element> listelem)
       {
           dejatraite = new List<long>();
           conguids=new List<string>();
           foreach (EA.Element el in listelem)
           {
               
               if (el.Name == "") continue;
               if(el.StereotypeEx.Contains(constantes.GetPrimitiveStereotype())
                     || el.StereotypeEx.Contains(constantes.GetEnumStereotype())
                     || el.StereotypeEx.Contains(constantes.GetCompoundStereotype())
                     || el.StereotypeEx.Contains(constantes.GetDatatypeStereotype())
                     || (el.MetaType=="Enumeration")
                     || (el.Type=="Enumeration")
                    ) continue;  // this is a datattype

               // --------------------     traitement des attributs -------------------------------------

               foreach (EA.Attribute at in el.Attributes)
               {
                   if ((at.LowerBound == "0") && (at.UpperBound == "1"))
                   {
                       at.LowerBound = "1"; // mandatory
                       at.Update();
                   }
               }

               //---- pour test -------------
               if (conguids != null)
               {
                   // ABA20230228 utilitaires.Utilitaires util = new utilitaires.Utilitaires();
                   texte = " dump avant traitement de element=" + el.Name;
                  // util.wlog("MakeAssociationEndsMandatory",texte);
                   dumpconguids(conguids);
               }

            

               

               //--------------------------------------------
               //----------------
               /*****        on ne traite pas les liens actuellement    
               foreach (EA.Connector con in el.Connectors)
               {
                   if (con.Type != "Association") continue;
                   if (dejatraite.Contains(con.ConnectorID)) continue;
                   dejatraite.Add(con.ConnectorID);
                   EA.ConnectorEnd source = con.ClientEnd;
                   EA.ConnectorEnd target = con.SupplierEnd;
                   if (source.Cardinality == "0..1")
                   {
                       if (target.Cardinality == "0..1")
                       {
                           MessageBox.Show(" Warning there in an incoherence with association" + source.Role
                               + " =>" + target.Role);
                           continue;
                       }
                       prov = source.Role;
                       
                       //source.IsNavigable = true;
                    
                       con.ClientEnd.Cardinality = "1";
                       con.ClientEnd.Navigable = "Navigable";
                       con.ClientEnd.Update();
                       //con.Update();
                       
                      
                       if(!conguids.Contains(con.ConnectorGUID)){
                           conguids.Add(con.ConnectorGUID);
                       }
                       
                       texte = "dump apres mis a jour source de la liaison de guid=" + con.ConnectorGUID;
                       util.wlog("MakeAssociationEndsMandatory",texte);
                       texte = " entre " + repository.GetElementByID((int)con.ClientID).Name;
                       util.wlog("MakeAssociationEndsMandatory",texte);
                       texte = " et " + repository.GetElementByID((int)con.SupplierID).Name;
                       util.wlog("MakeAssociationEndsMandatory",texte);
                    
                       dumpconguids(conguids);
                       //-----------------------
                   }
                   else
                   {
                       if ((target.Cardinality == "0..1"))
                       {

                           if ((source.Cardinality == "0..1"))
                           {
                               MessageBox.Show(" Warning there in an incoherence with association" + source.Role
                                   + " =>" + target.Role);
                               continue;
                           }
                           prov = target.Role;
                           //target.Cardinality = "1";
                           //target.IsNavigable = true;
                     
                           con.SupplierEnd.Cardinality = "1";
                           con.SupplierEnd.Navigable = "Navigable";
                         

                            if(! con.SupplierEnd.Update()) MessageBox.Show(" ERROR update " + con.SupplierEnd.GetLastError());
                           
                           //con.Update();

                     

                           if (!conguids.Contains(con.ConnectorGUID))
                           {
                               conguids.Add(con.ConnectorGUID);
                           }

                           texte = "dump apres mis a jour target de la liaison de guid=" + con.ConnectorGUID;
                           util.wlog("MakeAssociationEndsMandatory",texte);
                           texte = " entre " + repository.GetElementByID((int)con.ClientID).Name;
                           util.wlog("MakeAssociationEndsMandatory",texte);
                           texte = " et " + repository.GetElementByID((int)con.SupplierID).Name;
                           util.wlog("MakeAssociationEndsMandatory",texte);
                           dumpconguids(conguids);
                           //-----------------------
                       }
                   }
 
               }
               ********************************************************************/
           }
       }
        //-------------
    static  void dumpconguids(List<string> cnguids)
   {
            utilitaires.Utilitaires util = new utilitaires.Utilitaires(repository); // ABA20230228 repository
            texte = "  ------";
       util.wlog("MakeAssociationEndsMandatory",texte);
       foreach (string conguid in cnguids)
       {
           EA.Connector lien = repository.GetConnectorByGuid(conguid);
           texte = " --- connexion  " + conguid;
          // util.wlog("MakeAssociationEndsMandatory",texte);
           texte = " ------------------";
          // util.wlog("MakeAssociationEndsMandatory",texte);
           texte = "  source role=" + lien.ClientEnd.Role;
        //   util.wlog("MakeAssociationEndsMandatory",texte);
           texte = "  source navigable=" + lien.ClientEnd.Navigable;
          // util.wlog("MakeAssociationEndsMandatory",texte);
           texte = "  target role=" + lien.SupplierEnd.Role;
          // util.wlog("MakeAssociationEndsMandatory",texte);
           texte = "  target navigable=" + lien.SupplierEnd.Navigable;
         //  util.wlog("MakeAssociationEndsMandatory",texte);
           texte = " ------------------";
        //   util.wlog("MakeAssociationEndsMandatory",texte);

       }
   }
//---------------------------
    }
}
