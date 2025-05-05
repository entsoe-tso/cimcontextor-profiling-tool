using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
//using System.ComponentModel;
//using CimSyntaxGen.manageIBO;
using EA;
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
    public class GlobalRecoverDatatypes
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
        static string repofile = "";
        static public EA.Repository repository;
        static string aPackageGuid; // the package to gobally copy
        static UtilitiesConstantDefinition constantes = new UtilitiesConstantDefinition();
        static public Dictionary<string, Dictionary<string, EA.Element>> IBOelements = new Dictionary<string, Dictionary<string, EA.Element>>();
        static public List<EA.Package> IBOpackages = new List<EA.Package>();
        static List<long> condejatraite = new List<long>();// connexions deja traitees
        static public Dictionary<string, long> profdatatypes;// nom/elementid datatype
        static public bool Errorspresent = false;
        static CimContextor.utilitaires.Utilitaires util = new CimContextor.utilitaires.Utilitaires(Main.Repo);
        string texte = "";
        EA.Package domainpack = null;
        EA.Package parentpack;
        XMLParser XMLP = null;
        Dictionary<string, long> dicProfDomainDatatypeByName = new Dictionary<string, long>();
        bool hasDomain = false;
        //GlobalIBOCopy globalibocopy;
        public GlobalRecoverDatatypes(EA.Repository repo, EA.Package pack) // the constructor 
        {
            try
            {
                repofile = repo.ConnectionString;
                repository = repo;
                XMLP = new XMLParser(repo);
                repo.RefreshModelView(0);
                aPackageGuid = pack.PackageGUID;
                UtilitiesConstantDefinition constantes = new UtilitiesConstantDefinition();
                List<EA.Element> listprofelements = new List<EA.Element>();
              //  this.parentpack = util.GetIBOPackage(pack.PackageID);
                /* version avec DialGlobalCopy  abandon mars2019 pour dialtree
                DialGlobalCopy dialog = new DialGlobalCopy(repo, "");
                
                dialog.ShowDialog();
                EA.Package cimpack=null;
                string domainpackname = XMLP.GetXmlValueProfData("EntsoeDataTypesDomain");
                //if (profilespackage != null)
              //  {
                //    profilespack = profilespackage;
              //  }
              //  else
               // {
                    string profpackname = XMLP.GetXmlValueProfData("ProfilesPackage");
                    foreach(EA.Connector con in pack.Element.Connectors)
                    {
                    if((con.Type=="Dependency") && (con.Stereotype=="IsBasedOn"))
                    {
                        cimpack=repo.GetPackageByID((int)(repo.GetElementByID((int)con.SupplierID)).PackageID);
                        break;
                    }
                   }
                    if (profpackname == "") throw new Exception(" pb to access profpackname ");
                    EA.Package parentpack = Utilitaires.GetModelPackage(cimpack, repo); // le package le plus eleve de la hierarchie
                    foreach (EA.Package pa in parentpack.Packages)
                    {
                        if (pa.Name == profpackname)
                        {
                            profilespack = pa; // the recipient for profiles exists already
                            break;
                        }
                    }
               // }

                    domainpack = null;
                if ((domainpackname == "") || (domainpackname != null)) 
                {
                    hasDomain = true;
                    foreach (EA.Package pa in profilespack.Packages)
                    {
                        if (pa.Name == domainpackname)
                        {
                            domainpack = pa; // the recipient for profiles exists already
                            hasDomain = true;
                            break;
                        }
                    }
                }
                */
                // on cherche un package cim lie 
                foreach(EA.Connector con in pack.Connectors)
                {
                    if((con.Type=="Dependency") && (con.Stereotype.Contains("IsBasedOn")) && (con.ClientID==pack.Element.ElementID))
                        {// we are in relation with one parent
                        EA.Element parentpackel= repo.GetElementByID((int)con.SupplierID);
                       parentpack= repo.GetPackageByID((int)parentpackel.PackageID);
                       }
                }

                EA.Package toppack = Utilitaires.GetModelPackage(this.parentpack, repo); // le package le plus eleve de la hierarchie

                domainpack = null;
                
                DialTree dialtree = new DialTree(repo, toppack,false);
                dialtree.textinput = false;
                dialtree.inputlabelvisible = false;
                dialtree.ShowDialog();
                dialtree.Dispose();
                string domainname = "";
                if (Utilitaires.dicSelectedPackage.Count > 0)
                {

                    foreach (string name in Utilitaires.dicSelectedPackage.Keys)
                    {
                        domainname = name;
                        domainpack = Utilitaires.dicSelectedPackage[domainname];
                        hasDomain = true;
                        break;
                    }

                }
                
                if (domainpack == null)
                {
                    util.wlog(" GlobalRecoverDatatypes","no DomainProfile was selected the datatypes are kept as in CIM");
                    MessageBox.Show("no DomainProfile was selected the datatypes are kept as in CIM");
                   // return;
                }
                

                /**************  reconstitut les classifierID des attributs des elements *****************/
                List<string> cl = new List<string>();
                List<EA.Element> domainDatatypes = new List<EA.Element>();// les datatypes du domain
                List<EA.Element> profDatatypes = new List<EA.Element>(); // les datatypes du profil
                if (hasDomain)
                {
                    util.GetAllElements(domainpack, profDatatypes);
                        }
                foreach (EA.Element datatype in profDatatypes)
                {
                   
                    if (!dicProfDomainDatatypeByName.ContainsKey(datatype.Name)) dicProfDomainDatatypeByName[datatype.Name] = datatype.ElementID;
                }
                if (hasDomain)// il faut reconstituer à partir du domain local et non du cim
                {
                    util.getAllElements(domainpack, domainDatatypes, Cimdatatypes);

                    foreach (EA.Element elt in domainDatatypes)
                    {
                        if (!dicProfDomainDatatypeByName.ContainsKey(elt.Name)) dicProfDomainDatatypeByName[elt.Name] = elt.ElementID;
                    }
                
                }
                util.GetAllElements(pack, listprofelements);


                foreach (EA.Element el in listprofelements)
                {
                    if (el.Name != "")
                    {
                        {
                            if (
                                  (el.StereotypeEx.Contains("enumeration"))
                                  ||
                                  (el.StereotypeEx.Contains("Enumeration"))
                                  ||
                                  (el.MetaType == "Enumeration")
                                   ||
                                  (el.Type == "Enumeration")
                                )
                                continue;

                            foreach (EA.Attribute at in el.Attributes)
                            {
                                if (hasDomain)
                                {
                                    util.recoverClassifierID(repo, el, at, dicProfDomainDatatypeByName, domainpack);
                                }
                                else
                                {
                                    util.recoverClassifierID(repo, el, at, dicProfDomainDatatypeByName, null);
                                }

                            }
                        }

                    }
                }
            }
            catch (Exception)
            {
                texte = "Error in recovering classifier ";
                util.wlog("GlobalRecoverDataTypes", texte);
            }

        }
   //---------------------------------
    }
}
         
//----------------------------------------



    