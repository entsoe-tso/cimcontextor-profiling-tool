using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.ComponentModel;
using EA;
using CimContextor.utilitaires;
using CimContextor;

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

/********************************************************************************************************************/

namespace CimContextor.Utilities
{
    public class GlobalIBOCopyDiag

    {
        static public List<string> Cimstereos = new List<string>()
                                      {
	                                    "Class",
	                                    "Datatype",
                                        "CIMDatatype",
                                        "Primitive",
	                                    "Compound",
	                                    "enumeration",
                                        "Enumeration",
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
                                        "Enumeration"
                                      };
        static Dictionary<string,ArrayList> dicElemByName; //  name;ArrayList fo elements
        static List<EA.Element> listelem;
        static string repofile = "";
        //static XMLParser xmlp = null;
        static public EA.Repository repository;
        static string aPackageGuid; // the package to gobally copy
        static UtilitiesConstantDefinition constantes = new UtilitiesConstantDefinition();
        static public Dictionary<string, Dictionary<string, EA.Element>> IBOelements=new Dictionary<string, Dictionary<string, EA.Element>>();
        static public List<EA.Package> IBOpackages = new List<EA.Package>();
        static List<long> condejatraite = new List<long>();// connexions deja traitees
        static public Dictionary<string,long> profdatatypes;// nom/elementid datatype
        static public bool Errorspresent = false;
        static public long profilepackid=0;
        public Utilitaires util = null; // ABA20230228 new Utilitaires();
        public EA.Package cimpack; //package of the original diag 
        public EA.Package profilespack; // package for  profiles 
        public EA.Package domainpack; // package for profiles
        public EA.Package parentpack; // 
        public EA.Diagram profdiagram;  //
        public EA.Package profpack;// package of the new profile
        public static int echu = 0;
        public static Repository repos;
       

        public static EA.Package origpak;
        Dictionary<long, long> dicProfElIds = new Dictionary<long, long>(); // donne CCimElementID / ProfElementId

        public GlobalIBOCopyDiag(EA.Repository repo, EA.Diagram diagram,EA.Package profilespackage,EA.Package domainpackage) // the constructor 
        {
            Utilitaires util = new Utilitaires(repo);
            repos = repo;
            repofile = repo.ConnectionString;
            cimpack = repo.GetTreeSelectedPackage();
            repository = repo;
            repo.RefreshModelView(0);
            aPackageGuid = cimpack.PackageGUID;
            UtilitiesConstantDefinition constantes = new UtilitiesConstantDefinition();
            this.parentpack = Utilitaires.GetModelPackage(cimpack, repo); // le package le plus eleve de la hierarchie
            dicProfElIds = new Dictionary<long, long>(); // donne CCimElementID / ProfElementId
            /*********************************************************************************************
                       *  we copy the selected  diagram in a file copy it in a new profile package qualified by P_
                       *    - for each diagramobject 
                       *              -we copy the corresponding element in the new package based on it
                       *              - we build a corresponding dictionary giving a previouID to a new ID
                       *              - we copy the corresponding links based on previous links
             *********************************************************************************************/

            /*
            if (profilespackage != null)
            {
                profilespack = profilespackage;
            }
            else
            {
                string profpackname = xmlp.GetXmlValueProfData("ProfilesPackage");
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


                if (profilespack == null) // no recipient for profile
                {
                    try
                    {
                        profilespack = (EA.Package)parentpack.Packages.AddNew(constantes.GetProfilesPackageName(), "Package");

                        profilespack.Update();
                        parentpack.Packages.Refresh();

                    }
                    catch (Exception e)
                    {
                        string texte = "error " + e.Message;

                        //MessageBox.Show(texte);
                        util.wlog("GlobalIBOCopyDiagram", texte);
                        GlobalIBOCopy.Errorspresent = true;

                    }
                }
            }
            if(domainpackage != null)
            {
                domainpack = domainpackage;
            }
            else
            {
                string domainpackname = xmlp.GetXmlValueProfData("EntsoeDataTypesDomain");
                if (domainpackname == "") throw new Exception(" pb to access  domainpackname ");
                foreach (EA.Package pa in profilespack.Packages)
                {
                    if (pa.Name == domainpackname)
                    {
                        domainpack = pa; // the recipient for profiles exists already
                        break;
                    }
                }
               
            }
            */
            XMLParser xmlp = new XMLParser(repo);
            string domainpackname = "";
            string profilespackname = xmlp.GetXmlValueProfData("ProfilesPackage");
           

            if (profilespackage != null)
            {
                this.profilespack = profilespackage;
            }
            if (domainpackage != null)
            {
                this.domainpack = domainpackage;
            }

            {
                DialDoubleTree dialtree2 = new DialDoubleTree(repo, parentpack, false);
                dialtree2.cball = false;
                dialtree2.inputlabelvisible = false;
                dialtree2.textinput = false;
                dialtree2.inputLabel = " chose a Profile Container Package Name";
                dialtree2.profilename = "";
                dialtree2.treeLabel = " Select the Profile Container Package ";
                dialtree2.domaininputlabelvisible = false;
                dialtree2.domaininputlabelvisible = false;
                dialtree2.domaininputLabel = " chose a DataTypes Container Package Name";
                dialtree2.domainname = "";
                dialtree2.domaintreeLabel = " Select the DataTypes Container Package ";
                Main.ongoing = true;
                dialtree2.ShowDialog();
                if (!Main.ongoing) { dialtree2.Dispose(); return; }
                
                if (Utilitaires.dicSelectedPackage.Count > 0)
                {
                    foreach (string name in Utilitaires.dicSelectedPackage.Keys)
                    {
                        if (name.Contains("v1_"))
                        {
                            this.profilespack = Utilitaires.dicSelectedPackage[name];
                        }
                        if (name.Contains("v2_"))
                        {
                            this.domainpack = Utilitaires.dicSelectedPackage[name];
                        }
                    }
                }
                if (this.profilespack == null)
                {
                    if (dialtree2.profilename != "")
                    {
                        profilespackname = dialtree2.profilename;
                        foreach (EA.Package pa in parentpack.Packages)
                        {
                            if (pa.Name == profilespackname)
                            {
                                this.profilespack = pa; // the recipient for profiles exists already
                                string prov1 = this.profilespack.Name;
                                break;
                            }
                        }
                    }
                    else
                    {
                        profilespackname = ""; // on ne cree pas d eprofile
                    }
                }
                xmlp.SetXmlValueProfData("ProfilesPackage", profilespackname);



                if (profilespack == null)
                {
                    util.wlog("createGlobalProfile", " no profile container has been selected");
                    MessageBox.Show(" no profile container has been selected");
                    return;
                }

                if (domainpack == null)
                {
                    if (dialtree2.domainname != "")
                    {
                        domainpackname = dialtree2.domainname;
                        foreach (EA.Package pa in profilespack.Packages)
                        {
                            if (pa.Name == domainpackname)
                            {
                                this.domainpack = pa; // the recipient for profiles exists already
                                break;
                            }
                        }
                        if (this.domainpack == null) // il faut creer le domainpackage
                        {
                            this.domainpack = (EA.Package)profilespack.Packages.AddNew(domainpackname, "Package");
                            this.domainpack.Update();
                            this.profilespack.Packages.Refresh();
                            this.profilespack.Update();
                        }
                    }
                }

            }
            //----------   copy of the diagram  --------------------
            try
            {
               
                profpack = (EA.Package)profilespack.Packages.AddNew("P_" + diagram.Name, "Package");
                profilespack.Packages.Refresh();
                profpack.Update();

               
                profdiagram = (EA.Diagram)profpack.Diagrams.AddNew("P_" + diagram.Name, "Diagram");
                profpack.Diagrams.Refresh();
                profdiagram.Update();

                // we begin by copying all diagobjects
                List<EA.Element> profelements = new List<EA.Element>();
                List<long> profelementIDs = new List<long>();
                foreach (EA.DiagramObject diagobj in diagram.DiagramObjects)
                {
                    //we begin by copying the corresponding element 
                    EA.Element el = repo.GetElementByID((int)diagobj.ElementID);
                    cimpack = repo.GetPackageByID((int)el.PackageID);
                    EA.Package profpackcible = profpack;
                    if(el.PackageID != diagram.PackageID)
                    {
                        bool ok = false;
                        foreach(EA.Package pa in profpack.Packages)
                        {
                            if(pa.Name==cimpack.Name) // le package eiste deja , pas utile de la =e creer
                            {
                                ok = true;
                                profpackcible = pa;
                                break;
                            }
                        }
                        if(!ok)  // il faut creer le package intermediare
                        {
                            profpackcible = (EA.Package)profpack.Packages.AddNew(cimpack.Name, "Package");
                            profpackcible.Update();
                            profpack.Packages.Refresh();
                            profpack.Update();
                            int prov = profpack.Packages.Count;
                        }
                    }
                   
                  
                  //  util.crePackageIBODependency(profpack, cimpack);

                    EA.Element newel = util.copyElement(el, profpackcible,"sansliens");
                    if (!profelements.Contains(newel)) profelements.Add(newel);
                    if (!profelementIDs.Contains(newel.ElementID)) profelementIDs.Add(newel.ElementID);                     
                    dicProfElIds[el.ElementID] = newel.ElementID;
                    EA.DiagramObject newdiagobject = (EA.DiagramObject)profdiagram.DiagramObjects.AddNew(newel.Name, "element");
                    //newdiagobject.Update();
                    newdiagobject.ElementID = newel.ElementID;
                    newdiagobject.left = diagobj.left;
                    newdiagobject.right = diagobj.right;
                    newdiagobject.top = diagobj.top;
                    newdiagobject.bottom = diagobj.bottom;
                    string texte = "Dimension of object: l=" + newdiagobject.left + " r=" + newdiagobject.right + " t=" + newdiagobject.top + " b=" + newdiagobject.bottom;
                    //reportlog.WriteLine(texte);
                    newdiagobject.Sequence = diagobj.Sequence;
                    newdiagobject.Style = diagobj.Style;
                    //"BCol=16577251;BFol=9342520;LCol=9342520;LWth=1;" ;         
                    newdiagobject.Update();
                    profdiagram.DiagramObjects.Refresh();
                    newdiagobject.Update();
                }
                // a cet endroit on ajoute le lien de dependance
                // le fichier qu'on a copié pack est forcement dans le CIM on est donc basé sur unr des package du cim 
                //IEC61970,IEC62325,IEC61968 ou IEC62325-351
                util.crePackageIBODependency(profpack, cimpack);
                if (domainpack != null)  
                {
                    List<EA.Element> datatypeList = new List<EA.Element>();
                    List<long> datatypeIDList = new List<long>();
                    util.GetAllElements(domainpack,datatypeList);
                    foreach(EA.Element data in datatypeList)
                    {
                        if (!datatypeIDList.Contains(data.ElementID)) datatypeIDList.Add(data.ElementID);
                    }
                    foreach (long id  in profelementIDs)
                    {
                        EA.Element elt = Main.Repo.GetElementByID((int)id);
                        string prov = elt.Name;
                        foreach (EA.Attribute at in elt.Attributes)
                        {
                            if (!profelementIDs.Contains(at.ClassifierID)) // le datatype n'est as local
                            {
                                util.recoverDatatype(at, domainpack, datatypeIDList);
                            }
                        }
                    }
               
                    
                   
                        }
                foreach(EA.DiagramLink diaglink in diagram.DiagramLinks)
                {
                    copyConlink(diaglink, profdiagram);
                    

                        
                }
            }
            catch (Exception e)
            {
                string texte = "error " + e.Message;

                MessageBox.Show(texte);
                util.wlog("GlobalIBOCopyDiag", texte);
                GlobalIBOCopy.Errorspresent = true;
            }
        }

           


        
           
            


        //----------------------------------------------------------------------------


        static void DeletePackage(EA.Repository repo, EA.Package pack)
        {
            long packid = pack.PackageID;
            int parentid = pack.ParentID;
            if ((parentid != 0) && (packid != 0))
            {

                // the package should be deleted from its parent package
                EA.Package parent = repo.GetPackageByID(parentid);
                EA.Collection packages = parent.Packages;
                short index = new int();
                try
                {
                    for (short i = 0; i < packages.Count; i++)
                    {
                        EA.Package pa = (EA.Package)packages.GetAt(i);
                        if (pa.PackageID == packid)
                        { // we found the index of the package to delete
                            index = i;
                            break;
                        }
                    }
                    packages.Delete(index);
                    packages.Refresh();
                    repo.SaveAllDiagrams();
                    repo.RefreshOpenDiagrams(true);
                }
                catch (Exception e)
                {
                    Utilitaires util = new Utilitaires(repo);
                    //MessageBox.Show(" Error in deleting a package " + e.Message);
                    util.wlog("GlobalIBOCopy"," Error in deleting a package " + e.Message);
                    GlobalIBOCopy.Errorspresent = true;
                }
                try
                {
                    repo.RefreshModelView(parentid);
                    parent.Update();

                }
                catch (Exception e)
                {
                    Utilitaires util = new Utilitaires(repo);
                    MessageBox.Show(" Error in deleting a package " + e.Message);
                    util.wlog("GlobalIBOCopy"," Error in deleting a package " + e.Message);
                    GlobalIBOCopy.Errorspresent = true;
                }
            }
        }


        //--------------------------------------------------------------------------------------------------

        public void Import(EA.Repository repo, EA.Package pack, string profilename) // the constructor 
        {
            bool profileFilePresent = false;
                UtilitiesConstantDefinition constantes =new UtilitiesConstantDefinition();
            string savDirpackage = ".";
            try
            {
                string[] files = Directory.GetFiles(savDirpackage, "*.xml");
                if (files.Length > 0)
                {
                    string[] packagesname = new string[files.Length];
                    if (files.Length > 1)
                    {
                        Array.Resize(ref packagesname, files.Length - 1);
                    }

                    int j = 0;
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (Path.GetFileName(files[i]) == (profilename + ".xml"))
                        {
                            profileFilePresent = true;
                        }
                        else
                        {
                            if (files.Length > 1) packagesname[j] = Path.GetFileName(files[i]);
                            j++;
                        }
                    }

                }

                if (profileFilePresent == false)
                {
                   
                    MessageBox.Show("Error the profile's directory must contain a xmi file of the same name \n wich will be the root package for the profile");
                    util.wlog("GlobalIBOCopy","Error the profile's directory must contain a xmi file of the same name \n wich will be the root package for the profile");
                    GlobalIBOCopy.Errorspresent = true;
                    return;
                }
                else
                {
                    EA.Project project = repo.GetProjectInterface();// we get project interface

                    int ImportDiagram = 1;
                    int StripGUID = 1;
                    // first import of the packge root profile
                    string XmiFileName = savDirpackage + "\\" + profilename + ".xml";
                    string intoguid = pack.PackageGUID;
                    project.ImportPackageXMI(intoguid, XmiFileName, ImportDiagram, StripGUID);
                    repo.SaveAllDiagrams();
                    bool ret = pack.Update();
                    string erreur = "";
                    if (ret == false)
                    {
                        erreur = repo.GetLastError();
                        MessageBox.Show("Error in importing file \n" + XmiFileName + erreur);
                        util.wlog("GlobalIBOCopy","Error in importing file \n" + XmiFileName + erreur);
                        GlobalIBOCopy.Errorspresent = true;
                        return;
                    }

                    repo.RefreshModelView(pack.PackageID);

                    

                    intoguid = "";
                    pack = repo.GetPackageByGuid(pack.PackageGUID);//oups l'objet initial n'est pas raffraichi
                   
                    EA.Package profilepa = null; // the package of the profile
                    foreach (EA.Package pa in pack.Packages)
                    {
                        if (pa.Name == profilename)
                        {
                            intoguid = pa.PackageGUID;
                            profilepa = pa;
                            break;
                        }
                    }


                    if (intoguid != "")
                    {
                        profilepa.Name = constantes.ProfileNameSuffix + profilepa.Name; // the profile name will be prefixed by P_
                        profilepa.Update();
                    }
                      /* 
                        if (selectedpackages != null)
                        {
                            foreach (string paname in selectedpackages)
                            {// import  selected packages
                                XmiFileName = savDirpackage + "\\" + paname + ".xml"; // savDirectory + "\\" + paname;
                                project.ImportPackageXMI(intoguid, XmiFileName, ImportDiagram, StripGUID);
                                repo.SaveAllDiagrams();
                            }
                        }
                        ret = pack.Update();
                        if (ret == false)
                        {
                            erreur = repo.GetLastError();
                            MessageBox.Show("Error in importing files \n" + erreur);
                            return;
                        }
                    }
                    
                     */

                    //----------- rebuild of ClassifierIDs ---------------------
                    
               //     buidDics(repo.GetPackageByGuid(intoguid));
                    
                    /********************************************
                    foreach (EA.Element ell in listelem)
                    {
                       // MessageBox.Show("traite element " + ell.Name );
                        RecoverIdentifier(ell);
                        
                    }
                     /**************************************************
                    //---------------------   create IBOs ----------------------------
                   // create a dependency "IsbasedOn" betwween the profile and its parent
                    EA.Package profpa=(EA.Package)repo.GetPackageByGuid(intoguid);
                    string prov = profpa.Name;
                    EA.Package parentpa = (EA.Package)repo.GetPackageByGuid(aPackageGuid);
                    prov = parentpa.Name;
                    EA.Element profelt = profpa.Element;
                    EA.Element parentelt = parentpa.Element;

                    EA.Connector con = (EA.Connector)profelt.Connectors.AddNew("", "Dependency");
                    con.ClientID = profelt.ElementID;
                    con.SupplierID = parentelt.ElementID;
                    con.Stereotype = constantes.GetIsBasedOnStereotype();
                    con.Update();
                    profelt.Connectors.Refresh();
                    profelt.Update();   
                    **********************************************************/
                  //  RecreateIBO recreate = new RecreateIBO(repo, profpa);
                }
                
            }

            catch (Exception e)
            {
                Utilitaires util = new Utilitaires(repo);
                string texte = "Error in dealing with  profile's directory \n" + e.Message;
                MessageBox.Show(texte);
                util.wlog("GlobalIBOCopy",texte);
                GlobalIBOCopy.Errorspresent = true;

            }
             

        }
/// <summary>
/// build necessary dictionaries
///     dicElemByName
/// </summary>
        void buidDics(EA.Package pack)
        {
            dicElemByName=new Dictionary<string,ArrayList>();
            listelem = new List<EA.Element>();
            string prov = pack.Name + "::" + pack.Packages.Count.ToString();
            util.getAllElements(pack, listelem, Cimstereos);//ManageIBOUtilitaires.getAllElements(pack, listelem, Cimstereos);
            foreach (EA.Element el in listelem)
            {
                //MessageBox.Show(" Element " + "element=" + el.Name);
                    if (dicElemByName.ContainsKey(el.Name))
                    {
                        dicElemByName[el.Name].Add(el);
                    }
                    else
                    {
                        ArrayList ar = new ArrayList();
                        ar.Add(el);
                        dicElemByName.Add(el.Name, ar);
                    }                

            }
        }
        /// <summary>
        /// construit le dictionnaire donnant par nom de package,
        /// la liste des elements selon leur nom
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="packnames"></param>les paquetages selectionnes par dialogue
        void buildIBOdics(EA.Package pack,List<string> packnames)
        {
            Utilitaires.getAllPackages(pack, IBOpackages);//ManageIBOUtilitaires.getAllPackages(pack, IBOpackages);
            bool ok = false;
            foreach (EA.Package pp in IBOpackages)
            {
                if (pp.Name == pack.Name)
                {
                    ok = true;
                    break;
                }
            }
            if (!ok) IBOpackages.Add(pack);
           
            string prov = pack.Name;
            foreach (EA.Package pa in IBOpackages)
            {
                string paname = pa.Name;
                List<EA.Package> papacks = new List<EA.Package>();
                if ((paname==pack.Name) || ((packnames!=null) && (packnames.Contains(paname))))
                {
                    
                    if (!IBOelements.ContainsKey(paname))
                    {
                        IBOelements.Add(paname, new Dictionary<string, EA.Element>());
                        Utilitaires.getAllPackages(pa, papacks);
                         ok = false;
                        foreach (EA.Package pp in papacks)
                        {
                            if(pp.Name == pa.Name)
                            {
                                ok = true;
                                break;
                            }
                        }
                        if(!ok) papacks.Add(pa);
                        foreach (EA.Package souspa in papacks)
                        {
                            if (souspa.Name == "SynchronousMachineDynamics")
                            {
                               // ManageIBOUtilitaires.reportlog.Flush();
                            }
                            if (!IBOelements.ContainsKey(souspa.Name))
                            {
                                IBOelements.Add(souspa.Name, new Dictionary<string, EA.Element>());
                            }
                            foreach (EA.Element el in souspa.Elements)
                                {
                                    if ((el.Type == "Class") && (el.Name != ""))
                                    {
                                        if(!IBOelements[souspa.Name].ContainsKey(el.Name)) IBOelements[souspa.Name].Add(el.Name, el);
                                        string texte = "Creation entree dictionnaire IOBelements package=" + souspa.Name + " el=" + el.Name;
                                     //   util.wlog("GlobalIBOCopy",texte);
                                    }
                                }
                            
                        }
                    }
                }
            }
        }
        /// <summary>
        /// construit un dictionnaire donnant un datatype du profile par son nom
        /// </summary>
        /// <param name="profpack"></param>
        /// <param name="?"></param>
         public void buildProfDatatypes(EA.Package profpack,Dictionary<string,long> profdatatypes)
        {
            List<EA.Element> listprofdatatypes=new List<EA.Element>();
            util.getAllElements(profpack, listprofdatatypes, Cimdatatypes);
            foreach (EA.Element elt in listprofdatatypes)
            {
                if (!profdatatypes.ContainsKey(elt.Name)) profdatatypes.Add(elt.Name, elt.ElementID);
            }
        }
        public void buildProfElements(EA.Package profpack, Dictionary<string, long> profelements)
       {
           string prov = profpack.Name;
            List<string> Classtereos = new List<string>()
                                      {
	                                    "Class",
                                        "ACC",
                                        "ABIE",
                                        "MBIE",
                                       };
           List<EA.Element> listprofelements= new List<EA.Element>();
           util.getAllElements(profpack, listprofelements, Cimstereos);
           foreach (EA.Element elt in listprofelements)
           {
               if (!profelements.ContainsKey(elt.Name)) profelements.Add(elt.Name, elt.ElementID);
           }
           
       }
/// <summary>
/// 
/// </summary>
/// <param name="type"></param>
/// <returns></returns>
      long  getNewIdentifierID(string type) {
          long ret=-1;
          ArrayList ar=new ArrayList();
          if (dicElemByName.ContainsKey(type))
          {
              ar = dicElemByName[type];
              if (ar.Count == 1)
              {//ok
                  ret = ((EA.Element)(dicElemByName[type])[0]).ElementID;
              }
              else
              {
                  // there is more than one response
                  MessageBox.Show("there are more than one identifier  for identifier" + type);

                  ret = 0;
              }
          }
          else
          {// there is no identifier in the profiles
              if (type != "")
              {
                  EA.Collection col = null;
                  try
                  {
                      col = repository.GetElementsByQuery("Simple", type);
               
                  if (col.Count == 1)
                  {
                      ret = ((EA.Element)col.GetAt(0)).ElementID;
                  }
                  else
                  {
                      string texte = "there are no identifier '" + type + "' in the profile and more than one else where";
                      //MessageBox.Show(texte);
                util.wlog("GlobalIBOCopy",texte);
                GlobalIBOCopy.Errorspresent = true;
                      ret = 0;
                  }
                  }
                  catch (Exception e)
                  {
                        Utilitaires util = new Utilitaires(repository);
                        string texte="warning for " + type + " " + e.Message;
                      //MessageBox.Show(texte);
                      util.wlog("GlobalIBOCopy",texte);
                      GlobalIBOCopy.Errorspresent = true;
                  }
              }
          }
          return ret;
      }
/// <summary>
/// recover each identifier for each attribute
/// in an element
/// </summary>
/// <param name="elt"></param>
      void SetIdentifiers(EA.Element elt)
      {
          long identifier = 0;
          foreach (EA.Attribute atr in elt.Attributes)
          {
              string prov = elt.Name + "|" + atr.Name;
              identifier = getNewIdentifierID(atr.Type);
              if (identifier > 0)
              {
                  atr.ClassifierID = (int) identifier;
                  atr.Update();
              }
              else
              {
                    if (identifier == 0)
                  {
                      string texte=" problem with classifierID for " + elt.Name + "." + atr.Name;
                      //MessageBox.Show(texte);
                      util.wlog("GlobalIBOCopy",texte);
                      GlobalIBOCopy.Errorspresent = true;
                  }
              }
          }
      }
        /// <summary>
        /// ajoute les tag value IBO de IBOel (et ses attributs) à un element el 
        /// </summary>
        /// <param name="el"></param> //l'element
        /// <param name="IBOel"></param> // l'element sur lequel il est base 
      void traitelementIBO(EA.Element el, EA.Element IBOel)
      {
          string texte = "";
          string problem = "";
          EA.TaggedValue tagv = null;

          /*********** pour test **************/
          texte = "Appel fonction traitelement avec " + repository.GetPackageByID(el.PackageID).Name + "::" + el.Name;
         // util.wlog("GlobalIBOCopy",texte);
          /************************************/

          problem = "";
          texte = "create tag value for element=" + el.Name + " in '" + repository.GetPackageByID(el.PackageID).Name;
          texte = texte + "' from element=" + IBOel.Name + " in '" + repository.GetPackageByID(IBOel.PackageID).Name;
         // util.wlog("GlobalIBOCopy",texte); // for test

          /****************************************************/
           foreach (EA.TaggedValue tag in el.TaggedValues)
           {
               texte = "tag value of element=" + el.Name + "=> " + tag.Name + "," + tag.Value ;
              // util.wlog("GlobalIBOCopy",texte); // for test
           }
           /****************************************************/
          try
          {
              tagv = null;
              foreach (EA.TaggedValue tagva in el.TaggedValues)
              {
                  if (tagva.Name == constantes.GetIBOTagValue())
                  {
                      tagv = tagva;
                      tagv.Value = IBOel.ElementGUID;
                  }
              }
              if (tagv == null)
              {
                  tagv = (EA.TaggedValue)el.TaggedValues.AddNew(constantes.GetIBOTagValue(), IBOel.ElementGUID);
              }
              tagv.Update(); // NE PAS OUBLIER
              el.TaggedValues.Refresh();
              //MessageBox.Show("lastrefresh error on taggedvalues an nb of tags " + el.TaggedValues.GetLastError().ToString() + "|" + el.TaggedValues.Count.ToString());
              el.Refresh();
              el.Update();
              //-----------  treatment of all attributes --------------------------------//
              Dictionary<string, EA.Attribute> dicatrname = new Dictionary<string, EA.Attribute>(); // atr.name/atr
              foreach (EA.Attribute at in IBOel.Attributes)
              {
                  if (!dicatrname.ContainsKey(at.Name))
                  {
                      dicatrname.Add(at.Name, at);
                  }
                  else
                  {
                      problem = "There are at least two attributes with the same name in" + repository.GetPackageByID(IBOel.PackageID).Name + "::" + IBOel.Name;
                      break;
                  }
              }
              EA.AttributeTag attag = null;

              foreach (EA.Attribute atr in el.Attributes)
              {
                  attag = null;
                  if (dicatrname.ContainsKey(atr.Name))
                  {
                      foreach (EA.AttributeTag tagav in atr.TaggedValues)
                      {
                          if (tagav.Name == constantes.GetIBOTagValue())
                          {
                              attag = tagav;
                              attag.Value = dicatrname[atr.Name].AttributeGUID;
                          }
                      }
                      if (attag == null)
                      {
                          attag = (EA.AttributeTag)atr.TaggedValues.AddNew(constantes.GetIBOTagValue(), dicatrname[atr.Name].AttributeGUID);
                      }
                      attag.Update();
                      atr.TaggedValues.Refresh();
                      atr.Update();
                      el.Update();
                  }
                  else
                  {
                      problem = problem = "the attribute of name " + atr.Name + " in " + repository.GetPackageByID(el.PackageID).Name + "::" + el.Name + " has no correspondant in " +
                  repository.GetPackageByID(IBOel.PackageID).Name + "::" + IBOel.Name;
                      break;  // no reason to continue
                  }
              }


          }

          catch (Exception e)
          {
              
               texte=  "Error insert tag" + e.Message;
              //MessageBox.Show(texte);
              util.wlog("GlobalIBOCopy",texte);
              GlobalIBOCopy.Errorspresent = true;

          }

          // MessageBox.Show("result de el.update:" + (el.Update()).ToString());
          /******************* pour test ***************************
              foreach (EA.TaggedValue tag in el.TaggedValues)
              {
                  texte = "relecture apres refresh et el.update tag value of element=" + el.Name + "=> " + tag.Name + "," + tag.Value;
                  util.wlog("GlobalIBOCopy",texte); // for test
              }
           *****************************************************/
      }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="el"></param>
        /// <param name="topack"></param>
        /// <returns></returns>
        public EA.Element copyRefElementIntoProfile(EA.Repository repo, EA.Element el, EA.Package topack)
        {
            string paname = topack.Name;
            EA.Element newel = null;

            if (!CimContextor.Utilities.GlobalIBOCopy.IBOelements[paname].ContainsKey(el.Name))
            {

                // il faut creer l'element mirroir du Modele Informmation  sans ses attributs

                newel = (EA.Element)topack.Elements.AddNew(el.Name, "Class");
                newel.Update();
                CimContextor.Utilities.GlobalIBOCopy.IBOelements[paname].Add(el.Name, newel);
                newel.Abstract = el.Abstract;
                newel.ActionFlags = el.ActionFlags;
                newel.Alias = el.Alias;
                newel.Author = el.Author;
                newel.ClassifierID = el.ClassifierID;
                newel.ClassifierName = el.ClassifierName;
                newel.Complexity = el.Complexity;
                newel.Difficulty = el.Difficulty;
                newel.EventFlags = el.EventFlags;
                newel.ExtensionPoints = el.ExtensionPoints;
                newel.IsActive = el.IsActive;
                newel.IsLeaf = el.IsLeaf;
                newel.IsNew = el.IsNew;
                newel.IsSpec = el.IsSpec;
                newel.Multiplicity = el.Multiplicity;
                newel.Name = el.Name;
                newel.Notes = el.Notes;
                newel.Status = el.Status;
                newel.Stereotype = el.Stereotype;
                newel.StereotypeEx = el.StereotypeEx;
                newel.StyleEx = el.StyleEx;
                newel.Subtype = el.Subtype;
                newel.Tablespace = el.Tablespace;
                newel.Tag = el.Tag;
                newel.Type = el.Type;
                newel.Version = el.Version;
                newel.Visibility = el.Visibility;
                newel.Update();
                topack.Elements.Refresh();
                EA.TaggedValue newtv = null;
                bool isbasedon = false;
                foreach (EA.TaggedValue tv in el.TaggedValues)
                {
                    newtv = (EA.TaggedValue)newel.TaggedValues.AddNew(tv.Name, tv.Value);
                    newtv.Name = tv.Name;
                    newtv.Notes = tv.Notes;

                    if (tv.Name == constantes.GetIBOTagValue())
                    {
                        isbasedon = true;
                        newtv.Value = el.ElementGUID;
                    }
                    else
                    {
                        newtv.Value = tv.Value;
                    }

                    newtv.Update();
                    newel.TaggedValues.Refresh();
                }
                if (!isbasedon)
                {
                    newtv = (EA.TaggedValue)newel.TaggedValues.AddNew(constantes.GetIBOTagValue(), el.ElementGUID);
                    newtv.Update();
                    newel.TaggedValues.Refresh();
                }
                EA.Constraint newco = null;
                foreach (EA.Constraint co in el.Constraints)
                {
                    newco = (EA.Constraint)newel.Constraints.AddNew(co.Name, "INV");
                    newco.Name = co.Name;
                    newco.Notes = co.Notes;
                    newco.Status = co.Status;
                    newco.Type = co.Type;
                    newco.Weight = co.Weight;
                    newco.Update();
                    newel.Constraints.Refresh();

                }

                newel.Update();

            }
            else
            {
                // la classe avait deja ete cree
                newel = GlobalIBOCopy.IBOelements[paname][el.Name];
            }
            return newel;
        }
        //----------------------------------- 
        /// <summary>
        /// cree un lien mirroir du lien con 
        /// entre la classe et la nouvelle classe du profil
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="el"></param>
        /// <param name="newel"></param>
        /// <param name="con"></param>
        /// <returns></returns>
         public long creNewCon(EA.Repository repository, EA.Element el, EA.Element newel, EA.Connector con)
        {
            long res = 0;
            EA.Element parentel = repository.GetElementByGuid(util.getEltParentGuid(el));//ManageIBOUtilitaires.getParentGuid(el));
            EA.Element parentnewtel = repository.GetElementByGuid(util.getEltParentGuid(newel));
            short sens = 0;
            string role = "";
            EA.Connector newcon = null;
            if (con.ClientID == el.ElementID)
            {
                sens = constantes.SOURCE;  // l'element el est la source
                role = con.SupplierEnd.Role;
            }
            else
            {
                sens = constantes.TARGET;
                role = con.ClientEnd.Role;
            }
            foreach (EA.Connector lk in parentel.Connectors)
            {
                if ((sens == constantes.SOURCE) && (lk.ClientID == parentel.ElementID))// eligible
                {
                    if (lk.SupplierID == parentnewtel.ElementID)
                    {
                        if (lk.SupplierEnd.Role == role)
                        { // on a trouve la bonne connection
                            try
                            {
                                newcon = (EA.Connector)el.Connectors.AddNew("", con.Type);
                                string texte = newcon.Name + "," + newcon.ConnectorID;
                                copyCon(lk, newcon, el, newel, sens); // le update est fait dans le ss programme
                                res = newcon.ConnectorID;

                            }
                            catch (Exception e)
                            {
                                string texte = "Pb dans creNewCon pour " + el.Name + ", " + e.Message;
                                util.wlog("creNeCon",texte);
                                MessageBox.Show(texte);
                            }
                        }
                    }
                }
                else
                {
                    if ((sens == constantes.TARGET) && (lk.SupplierID == parentel.ElementID))
                    {

                        if (lk.ClientID == parentnewtel.ElementID)
                        {
                            if (lk.ClientEnd.Role == role)
                            { // on a trouve la bonne connection
                                try
                                {
                                    newcon = (EA.Connector)el.Connectors.AddNew("", con.Type);
                                    copyCon(lk, newcon, el, newel, sens);
                                    res = newcon.ConnectorID;
                                }
                                catch (Exception e)
                                {
                                    string texte = "Pb dans creNewCon pour " + el.Name + ", " + e.Message;
                                    util.wlog("creNewCon",texte);
                                    MessageBox.Show(texte);
                                }

                            }
                        }
                    }
                }
                el.Connectors.Refresh();


            }



            return res;
        }

        /// <summary>
        /// copie d'un connecteur
        /// </summary>
        /// <param name="con"></param>
        /// <param name="newcon"></param>
        /// <param name="el"></param>
        /// <param name="newel"></param>
        /// <param name="sens"></param>
         public void copyCon(EA.Connector con, EA.Connector newcon, EA.Element el, EA.Element newel, int sens)
        {
            if (sens == constantes.SOURCE)
            {
                newcon.ClientID = el.ElementID;
                newcon.SupplierID = newel.ElementID;
            }
            else
            {
                newcon.SupplierID = el.ElementID;
                newcon.ClientID = newel.ElementID;

            }
            newcon.Color = con.Color;
            newcon.Direction = con.Direction;
            newcon.Name = con.Name;
            newcon.RouteStyle = con.RouteStyle;
            newcon.Notes = con.Notes;
            newcon.Stereotype = con.Stereotype;
            newcon.StereotypeEx = con.StereotypeEx;
            newcon.StyleEx = con.StyleEx;
            newcon.Type = con.Type;
            newcon.Width = con.Width;
            newcon.Update();
            ConnectorTag newcontag = null;
            bool isbasedon = false;
            foreach (ConnectorTag contag in con.TaggedValues)
            {
                newcontag = (EA.ConnectorTag)newcon.TaggedValues.AddNew(contag.Name, contag.Value);
                if (contag.Name == constantes.GetIBOTagValue())
                {
                    newcontag.Value = con.ConnectorGUID;
                }
                newcontag.Notes = contag.Notes;
                newcontag.Update();

            }
            if (!isbasedon)
            {
                newcontag = (EA.ConnectorTag)newcon.TaggedValues.AddNew(constantes.GetIBOTagValue(), con.ConnectorGUID);
                newcontag.Update();
                isbasedon = true;
            }
            newcon.TaggedValues.Refresh();

            /*
            if (sens == constantes.SOURCE)
            {
                copyConEnd(con.SupplierEnd, newcon.SupplierEnd);

            }
            else
            {
                copyConEnd(con.ClientEnd, newcon.ClientEnd);

            }
            */
            con.Update();
            util.copyConEnd(con.SupplierEnd, newcon.SupplierEnd);
            util.copyConEnd(con.ClientEnd, newcon.ClientEnd);
            newcon.Update();

        }
        /// <summary>
        /// copie d'un connecteurlink d'un diagramme dans un autre diagramme
        /// on suppose que les elements relies sont presents
        /// </summary>
        /// <param name="conlink"></param>
        /// <param name="diag"></param> // le diagrramme cible
        
        public void copyConlink(EA.DiagramLink conlink, EA.Diagram diag)
        {
            EA.DiagramLink newconlink = (EA.DiagramLink)diag.DiagramLinks.AddNew("", "DiagramLink");
            diag.DiagramLinks.Refresh();
            newconlink.Update();

            EA.Connector conorig = repos.GetConnectorByID((int)conlink.ConnectorID); // le connecteur d'origine
            long clientorigid = conorig.ClientID;
            long supplierorigid = conorig.SupplierID;
            EA.Element profelsource = repos.GetElementByID((int)dicProfElIds[clientorigid]);
            EA.Element profeltarget = repos.GetElementByID((int)dicProfElIds[supplierorigid]);
            EA.Connector profcon = (EA.Connector)profelsource.Connectors.AddNew("", conorig.Type);
            profcon.ClientID = profelsource.ElementID;
            profcon.SupplierID = profeltarget.ElementID;
            profelsource.Connectors.Refresh();
            profcon.Update();









            profcon.Color = conorig.Color;
            profcon.Direction = conorig.Direction;
            profcon.Name = conorig.Name;
            profcon.RouteStyle = conorig.RouteStyle;
            profcon.Notes = conorig.Notes;
            profcon.Stereotype = conorig.Stereotype;
            profcon.StereotypeEx = conorig.StereotypeEx;
            profcon.StyleEx = conorig.StyleEx;
            profcon.Type = conorig.Type;
            profcon.Width = conorig.Width;
            profcon.Update();
            ConnectorTag newcontag = null;
            bool isbasedon = false;
            foreach (ConnectorTag contag in conorig.TaggedValues)
            {
                newcontag = (EA.ConnectorTag)profcon.TaggedValues.AddNew(contag.Name, contag.Value);
                if (contag.Name == constantes.GetIBOTagValue())
                {
                    newcontag.Value = conorig.ConnectorGUID;
                }
                newcontag.Notes = contag.Notes;
                newcontag.Update();

            }
            if (!isbasedon)
            {
                newcontag = (EA.ConnectorTag)profcon.TaggedValues.AddNew(constantes.GetIBOTagValue(), conorig.ConnectorGUID);
                newcontag.Update();
                isbasedon = true;
            }
            profcon.TaggedValues.Refresh();

            profcon.Update();
            util.copyConEnd(conorig.SupplierEnd, profcon.SupplierEnd);
            util.copyConEnd(conorig.ClientEnd, profcon.ClientEnd);
            profcon.Update();

        }


        //-----------------------------------------------------------

    }

}