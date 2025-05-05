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
    public class GlobalIBOCopy
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
        static public EA.Repository repository;
        string savDirpackage = ""; // the directory to save the profiles
        static string aPackageGuid; // the package to gobally copy
        static UtilitiesConstantDefinition constantes = new UtilitiesConstantDefinition();
        static public Dictionary<string, Dictionary<string, EA.Element>> IBOelements=new Dictionary<string, Dictionary<string, EA.Element>>();
        static public List<EA.Package> IBOpackages = new List<EA.Package>();
        static List<long> condejatraite = new List<long>();// connexions deja traitees
        static public Dictionary<string,long> profdatatypes;// nom/elementid datatype
        static public bool Errorspresent = false;
           string texte = "";
        static public long profilepackid=0;
        static Dictionary<string, EA.Package> dicProfPackagesByName = new Dictionary<string, Package>();// le dictionnire des package du profile copié
        public Utilitaires util = null; // ABA20230228 new Utilitaires();
        public EA.Package profilepack; // the package in which the profile will be copied
        EA.Package profilespack = null; // profile container
        EA.Package parentpack = null; // parent of the packege to copy
        EA.Package domainpack=null; // package for profile datatypes
        public GlobalIBOCopy(EA.Repository repo, EA.Package pack, EA.Package profilepackage, EA.Package domainpackage) // the constructor 
        {

            Utilitaires util = new Utilitaires(repo);
            repofile = repo.ConnectionString;
            repository = repo;
            repo.RefreshModelView(0);
            aPackageGuid = pack.PackageGUID;
            IBOelements = new Dictionary<string, Dictionary<string, EA.Element>>();
            IBOpackages = new List<EA.Package>();
            List<long> condejatraite = new List<long>();// connexions deja traitees
            this.parentpack = Utilitaires.GetModelPackage(pack, repo); // le package le plus eleve de la hierarchie
            UtilitiesConstantDefinition constantes = new UtilitiesConstantDefinition();
            Main.ongoing = true;
            DialSPackages dialog = new DialSPackages();
            

            // Sets up the initial objects in the CheckedListBox.
            string[] packagesname = new string[100];// a priori 
            Dictionary<string, EA.Package> packages = new Dictionary<string, EA.Package>();
            int i = 0;
            if (pack.Packages.Count != 0)
            {
                foreach (EA.Package pa in pack.Packages)
                {
                    packagesname[i] = pa.Name;
                    packages.Add(pa.Name, pa);
                    i++;
                }

                if (packagesname.Length > 0)
                {
                    //string packsname = new string[packagesname.Length - 1];
                    int dim = Array.IndexOf(packagesname, null);
                    if (dim > 0)
                    {
                        Array.Resize(ref packagesname, dim);

                        dialog.dialbox.Items.AddRange(packagesname);
                    }
                }
                Main.ongoing = true;
                dialog.ShowDialog();
                if (!Main.ongoing) { dialog.Dispose(); return; }
                dialog.Dispose(); // ABA20230408
            }
            XMLParser xmlp = new XMLParser(repo);
            string domainpackname = "";
            string profilespackname = xmlp.GetXmlValueProfData("ProfilesPackage");
            if(profilepackage !=null)
            {
                this.profilespack = profilepackage;
            }
            if (domainpackage != null)
            {
                this.domainpack = domainpackage;
            }

            DialDoubleTree dialtree2 = null;
            try {
                dialtree2 = new DialDoubleTree(repo, parentpack, false);
                dialtree2.cball = false;
                dialtree2.inputlabelvisible = false;
                dialtree2.textinput = false; ;
                dialtree2.inputLabel = " chose a Profile Container Package Name";
                dialtree2.profilename = "";
                dialtree2.treeLabel = " Select the Profile Container Package ";
                dialtree2.domaininputlabelvisible = true;
                dialtree2.domaininputlabelvisible = true;
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
                        profilespackname = ""; // on ne cree pas de profile container
                    }
                }
                xmlp.SetXmlValueProfData("ProfilesPackage", profilespackname);

                if(profilespack==null)
                {
                    util.wlog("createGlobalProfile", " no profile container has been selected");
                    MessageBox.Show(" no profile container has been selected");
                    //dialog.Dispose(); // ABA20230401
                    return;
                }
           
                  if(domainpack==null)
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
            finally // ABA20230504
            {
                if(dialog != null) dialog.Dispose();
                if(dialtree2 != null) dialtree2.Dispose();
            }
            string nomfichier = "";
            // sauvegarde le fichier actuel EAet le substitue le  fichier provisoire   
            try
            {
                nomfichier = repo.ConnectionString;
                repo.CloseFile();
                System.IO.File.Copy(nomfichier, nomfichier + "_orig", true);
                repo.OpenFile(nomfichier);
            }
            catch (Exception)
            {
                string texto = "Error in copying temporary EAP file: " + nomfichier;
                MessageBox.Show(texto);
                util.wlog("GlobalIBOCopy", texto);
                GlobalIBOCopy.Errorspresent = true;

            }
            EA.Project project = repo.GetProjectInterface();// we get project interface
            // save all the selected packages
            //create a directory of the same name of the profile to be saved (with optional additional qualifier
            string packqualifier = "";
            // savDirpackage = Environment.GetEnvironmentVariable("APPDATA") + Constants.CIMCONTEXTOR_RES_PATH + packqualifier + pack.Name;
            savDirpackage = FileManager.GetParentDirPath() + "\\" +packqualifier + pack.Name; // ABA20221020
            //MessageBox.Show(savDirpackage); // pour test
            try
            {
                if (Directory.Exists(savDirpackage)) Directory.Delete(savDirpackage, true);
                Directory.CreateDirectory(savDirpackage);
            }
            catch (Exception e)
            {
                string texte = "Error in creating the directory " + savDirpackage + e.Message;
                MessageBox.Show(texte);
                util.wlog("GlobalIBOCopy", texte);
                GlobalIBOCopy.Errorspresent = true;
            }


            int ExportImages = 1;
            int ImageFormat = -1;//-1=NONE, 0=EMF, 1=BMP, 2=GIF,3=PNG, 4=JPG
            int FormatXML = 0;//True if XML output should be formatted prior to saving.
            int UseDTD = 0; // int ExportCopyImages=1;
            string XmiFileName = "";

            if (dialog.selectedpackages != null)
            {

                short ind = 0;
                List<short> packstodelete = new List<short>();

                foreach (EA.Package spa in pack.Packages)
                {// detruire les packages non selectionne
                    if (!dialog.selectedpackages.Contains(spa.Name))
                    {// si le package n'a pas ete selectionne l'effacer
                        if (!packstodelete.Contains(ind)) packstodelete.Add(ind);
                    }
                    ind++;
                }

                foreach (short j in packstodelete)
                {
                    pack.Packages.DeleteAt(j, false);
                }
                // we save the package itself once all the inside packages are deleted
                pack.Packages.Refresh();
                pack.Update();

            }
            XmiFileName = savDirpackage + "\\" + pack.Name + ".xml";
            project.ExportPackageXMI(pack.PackageGUID, EnumXMIType.xmiEA21, ExportImages, ImageFormat, FormatXML, UseDTD,
           XmiFileName);


            /*************************************************
            // restaure le fichier origine EA     
            try
            {
               // repo.Exit();
                System.IO.File.Copy(nomfichier + "_orig", nomfichier , true);
                System.IO.File.Delete(nomfichier + "_orig");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
             ****************************************************/
            //import the new profile in a package name Profiles if it does not exists


            //  profilepack = null; // the profile package
            //EA.Profilespack;
            try
            {
                string packparentguid = ((EA.Package)repo.GetPackageByID(pack.ParentID)).PackageGUID; // tne parent
                repo.CloseFile();
                System.IO.File.Copy(nomfichier + "_orig", nomfichier, true);
                System.IO.File.Delete(nomfichier + "_orig");
                repo.OpenFile(nomfichier);
                // EA.Repository repository = new EA.Repository();
                //  repo.OpenFile(repofile);

                //EA.Package parentpack = (EA.Package)repo.GetPackageByGuid(packparentguid);
                
                
                if (profilespack == null) // no recipient for profile
                {
                    try
                    {

                        profilespack = (EA.Package)parentpack.Packages.AddNew(profilespackname, "Package");

                        profilespack.Update();
                        parentpack.Packages.Refresh();
                        parentpack.Update();

                    }
                    catch (Exception e)
                    {
                        string texte = "error " + e.Message;

                        //MessageBox.Show(texte);
                        util.wlog("GlobalIBOCopy", texte);
                        GlobalIBOCopy.Errorspresent = true;

                    }
                }
                Import(repo, profilespack, pack.Name);
                profilespack.Packages.Refresh();
                profilespack.Update();
                foreach (EA.Package pa in profilespack.Packages)
                {
                    if (pa.Name == constantes.ProfileNameSuffix + pack.Name)
                    {
                        profilepack = pa;
                        profilepackid = pa.PackageID; // am 26-05-12
                       // util.creGlobalPackageIBODependency(profilepack, pack);
                        break;
                    }
                }
                /********************/
                util.crePackageIBODependency(profilepack, pack);
                /********************/
            }
            catch (Exception e)
            {
                string texte = "error " + e.Message;
                //MessageBox.Show(texte);
                util.wlog("GlobalIBOCopy", texte);
                GlobalIBOCopy.Errorspresent = true;

            }

            // restaure le fichier origine EA     
            try
            {
                /*
                repo.CloseFile();
                System.IO.File.Copy(nomfichier + "_orig", nomfichier, true);
                System.IO.File.Delete(nomfichier + "_orig");
                 */


            }
            catch (Exception e)
            {
                string texte = " error " + e.Message;
                MessageBox.Show(texte);
                util.wlog("GlobalIBOCopy", texte);
                GlobalIBOCopy.Errorspresent = true;


            }


            /********  on ajoute les tagvalues IBO a tous les elements et attributs *****************/
            //MessageBox.Show(" avant build"); //pour test
            buildIBOdics(pack, dialog.selectedpackages);
            List<EA.Element> listprofelements = new List<EA.Element>();
            List<EA.Package> profpackages = new List<EA.Package>();
            //MessageBox.Show(" avant acces packages"); // pour test
            Utilitaires.getAllPackages(profilepack, profpackages);
            Dictionary<string, EA.Package> direxts = new Dictionary<string, EA.Package>();
            Dictionary<long, EA.Element> dicAddedElements = new Dictionary<long, EA.Element>(); // les elements descendus dans le profil de l'exterieur
            dicProfPackagesByName = new Dictionary<string, Package>();
            if (!profpackages.Contains(profilepack)) profpackages.Add(profilepack);
            foreach(EA.Package profpack in profpackages)
            {
                dicProfPackagesByName[profpack.Name] = profpack;
            }
            foreach (EA.Package pa in profpackages)
            //foreach (EA.Package pa in dicProfPackagesByName.Values)
             {
                string pname = pa.Name;
                //MessageBox.Show(" processing... package : " + pname + " (click OK to continue)");
                if (pa.Name == profilepack.Name) // on enleve le prefix
                {
                    pname = pname.Remove(0, constantes.ProfileNameSuffix.Length);
                }

                foreach (EA.Element el in pa.Elements)
                {
                    if ((el.Type == "Class") && (el.Name != ""))
                    {
                        try
                        {
                            traitelementIBO(el, IBOelements[pname][el.Name]);
                        }
                        catch (Exception e)
                        {
                            texte = " problem while processing element=" + pa.Name + "::" + el.Name;
                            texte = texte + e.Message;
                            util.wlog("GlobalIBOCopy", texte);
                            // ManageIBOUtilitaires.reportlog.Flush();
                        }



                        int prov3 = profilepack.Packages.Count;
                        /******** on ajoute les classes lorsque les liens sont vers l'exterieur *********/
                        EA.Element extelement = null;
                        List<int> contodelete = new List<int>();
                        short index = 0;
                        EA.Package topack = null;// package des classes externes
                        if (el.Name == "GenLoad")
                        {
                            // ManageIBOUtilitaires.reportlog.Flush();
                        }
                        foreach (EA.Connector con in el.Connectors)
                        {
                            if (!condejatraite.Contains(con.ConnectorID))
                            {
                                condejatraite.Add(con.ConnectorID);
                                extelement = util.SetConGuid(repository, profilepack, el, con);

                                if (extelement == null)
                                {
                                    /**********   pour test ***********************/
                                    texte = "creation tag  GUID for connector ID " + con.ConnectorID.ToString() + " between " + repo.GetElementByID(con.ClientID).Name + " and " + repo.GetElementByID(con.SupplierID).Name;
                                   // util.wlog("GlobalIBOCopy", texte);
                                    /**********************************************/
                                }
                                else
                                {
                                    // le lien est vers une classe externe
                                    if (!contodelete.Contains(index)) contodelete.Add(index);

                                    texte = "GlobalIBOCopy  l'element " + repo.GetPackageByID((int)el.PackageID).Name + "(" + el.PackageID.ToString() + ")::" + el.Name
                                        + " est liee a la classe externe" + repo.GetPackageByID((int)extelement.PackageID).Name + "(" + extelement.PackageID.ToString() + ")::" + extelement.Name;
                                 //   util.wlog("GlobalIBOCopy", texte);



                                    //il  faut d'abord creer la classe dans le profil
                                    if ((con.Type == "Association") || (con.Type == "Generalization"))
                                    {// ce n'est pas une Dependency

                                        string paname = repo.GetPackageByID((int)extelement.PackageID).Name;

                                        EA.Element newel = null;
                                        if (!IBOelements.ContainsKey(paname))
                                       //if(!dicProfPackagesByName.ContainsKey(paname))
                                        {
                                            IBOelements.Add(paname, new Dictionary<string, EA.Element>());
                                            topack = (EA.Package)profilepack.Packages.AddNew(paname, "Package");
                                            topack.Update();
                                            dicProfPackagesByName[paname] = topack;
                                            profilepack.Packages.Refresh();
                                            profilepack.Update();
                                           // direxts.Add(paname, topack);
                                        }
                                        else
                                        {
                                            // topack = direxts[paname];
                                            topack = dicProfPackagesByName[paname];
                                        }
                                        newel = copyRefElementIntoProfile(repo, extelement, topack);    //Utilitaires.copyRefElementIntoProfile(repo, extelement, topack);
                                        if (newel != null)
                                        {
                                            texte = "GlobalIBOCopy apres la copie ref  de l'element" + repo.GetPackageByID((int)extelement.PackageID).Name
                                                + "::" + extelement.Name;
                                           // util.wlog("GlobalIBOCopy", texte);
                                            texte = " vers " + repo.GetPackageByID((int)newel.PackageID).Name + "::" + newel.Name;
                                           // util.wlog("GlobalIBOCopy", texte);
                                            if (!dicAddedElements.ContainsKey(newel.ElementID)) dicAddedElements[newel.ElementID] = newel;

                                            extelement = null;
                                            //IBOguid = Utilitaires.SetConGuid(repository, profilepack, newel, con);
                                            long newconid = 0;
                                            try
                                            {
                                                newconid = creNewCon(repository, el, newel, con); //Utilitaires.creNewCon(repository, el, newel, con);
                                            }
                                            catch (Exception e)
                                            {
                                                texte = "Error creating new connector for elemnt " + el.Name + "   " + e.Message;
                                                //MessageBox.Show(texte);
                                                util.wlog("GlobalIBOCopy", texte);
                                                GlobalIBOCopy.Errorspresent = true;

                                            }
                                            if (!condejatraite.Contains(newconid)) condejatraite.Add(newconid);
                                            if (extelement != null)
                                            {
                                                texte = " GlobalIBOCopy ¨error while copying ext  " + extelement.Name + " element exterieur";
                                                util.wlog("GlobalIBOCopy", texte);
                                                // MessageBox.Show(texte);
                                                GlobalIBOCopy.Errorspresent = true;

                                            }
                                        }
                                        else
                                        {
                                            texte = "GlobalIBOCopy  error while copying ref element " + repo.GetPackageByID((int)extelement.PackageID).Name
                                                + "::" + extelement.Name;
                                            util.wlog("GlobalIBOCopy", texte);
                                            //MessageBox.Show(texte);
                                            GlobalIBOCopy.Errorspresent = true;
                                        }

                                    }
                                }
                            }
                            index++;
                            // ManageIBOUtilitaires.reportlog.Flush();
                        }


                        foreach (short j in contodelete)
                        {
                            /**********   pour test ***********************/
                            texte = "GlobalIBOCopy delete external connector " + j.ToString() + "  for " + el.Name;
                           // util.wlog("GlobalIBOCopy", texte);
                            /**********************************************/

                            el.Connectors.DeleteAt(j, false);
                        }


                        el.Connectors.Refresh();

                        el.Update();

                    }

                }

                /******************************** on ajoute les ancetres des elements exterieurs incorpores dans le profil *********************/
                //targetedPackage.Packages.Refresh();
                //targetedPackage.Update();
                int prov4 = profilepack.Packages.Count;
                foreach (EA.Element elt in dicAddedElements.Values)
                {
                    string prov1 = elt.Name;
                    string nom = pa.Name;
                    addAncesters(repo, pa, elt);

                }
                prov4 = profilepack.Packages.Count;
            }




            /**************  reconstitut les classifierID des attributs *****************/
            Utilitaires.getAllPackages(profilepack, profpackages);
            int prov = profilepack.Packages.Count;
            prov = profpackages.Count;
            /*
            this.domainpack = null;
          
            XMLParser XMLP = new XMLParser();
            
            if (domainpackage != null)
            {
                domainpack = domainpackage;
            }
            else
            {
                string domainpackname = XMLP.GetXmlValueProfData("EntsoeDataTypesDomain");
                if (domainpackname != "")
                {
                    foreach (EA.Package pa in profilespack.Packages)
                    {
                        if (pa.Name == domainpackname)
                        {
                            domainpack = pa; // the recipient for profiles exists already
                            break;
                        }
                    }
                    if(domainpack==null) // il faut creer le domainpackage
                    {
                        domainpack = (EA.Package)profilespack.Packages.AddNew(domainpackname, "Package");
                        domainpack.Update();
                        profilespack.Packages.Refresh();
                        profilespack.Update();
                    }
                }
            }
            */
            if (this.domainpack != null)
            {
                List<EA.Element> datatypeList = new List<EA.Element>();
                List<long> datatypeIDList = new List<long>();
                util.GetAllElements(this.domainpack, datatypeList);
                foreach (EA.Element data in datatypeList)
                {
                    if (!datatypeIDList.Contains(data.ElementID)) datatypeIDList.Add(data.ElementID);
                }
                List<EA.Element> elementsList = new List<EA.Element>();
                List<long> profelementIDs = new List<long>();
                util.GetAllElements(profilepack, elementsList);
                foreach (EA.Element el in elementsList)
                {
                    if (!profelementIDs.Contains(el.ElementID)) profelementIDs.Add(el.ElementID);
                }
                foreach (long id in profelementIDs)
                {
                    EA.Element elt = Main.Repo.GetElementByID((int)id);
                    string prov5 = elt.Name;
                    foreach (EA.Attribute at in elt.Attributes)
                    {
                        if (!profelementIDs.Contains(at.ClassifierID)) // le datatype n'est as local
                        {
                            util.recoverDatatype(at, this.domainpack, datatypeIDList);
                        }
                    }
                }
            }
            else
            {

                profdatatypes = new Dictionary<string, long>();
                buildProfDatatypes(profilepack, profdatatypes);
                foreach (EA.Package pa in profpackages)
                {
                    string prov6 = pa.Name;
                    foreach (EA.Element el in pa.Elements)
                    {
                        if ((el.Type == "Class") && (el.Name != ""))
                        {
                            foreach (EA.Attribute at in el.Attributes)
                            {
                                util.recoverClassifierID(repository, el, at, profdatatypes);
                            }
                        }

                    }

                }
            }
            
                /************************* reconstitue les lien IBO *****************************/

                WithIBODependency withibo = new WithIBODependency(repo);



                /*****************  on reconstitue les diagrammes avec les element exterieurs ****************/
                //------- pour test 

                Dictionary<long, string> packsid = new Dictionary<long, string>();
            /*
                foreach (EA.Package pa in profpackages)
                {
                    if (!packsid.ContainsKey(pa.PackageID))
                    {
                        packsid.Add(pa.PackageID, pa.Name);
                    }
                }
                long envelopid = 0;
                foreach (long paid in packsid.Keys)
                {
                    long parentid = repo.GetPackageByID((int)paid).ParentID;
                    if (packsid.ContainsKey(parentid))
                    {
                        continue;
                    }
                    else
                    {
                        envelopid = paid;
                        break;
                    }
                }
                if (envelopid == 0) { MessageBox.Show("problem withibo envelopid"); }
                EA.Package enveloppe = repo.GetPackageByID((int)envelopid);
                //---------------------

                profilepack = repo.GetPackageByID((int)profilepackid);
                // a cet endroit on ajoute le lien de dependance
                // le fichier qu'on a copié pack est forcement dans le CIM on est donc basé sur unr des package du cim 


                enveloppe = profilepack;
               */
                Dictionary<string, long> profelements = new Dictionary<string, long>();
                buildProfElements(profilepack, profelements);
                List<EA.Package> profilepackages = new List<EA.Package>();
                profilepackages.Add(profilepack);

                foreach (EA.Package pk in profilepack.Packages)
                {
                    profilepackages.Add(pk);
                }
                foreach (EA.Package pa in profilepackages)
                {
                    string pname = pa.Name;
                    long elemid = 0;
                    // MessageBox.Show(" processing diagrames for ... package : " + pname + " (click OK to continue)");
                    Dictionary<int, EA.Diagram> diagramsbyid = new Dictionary<int, EA.Diagram>();
                    util.getAllDiagrams(repo, pa, diagramsbyid);
                    foreach (EA.Diagram diag in diagramsbyid.Values)
                    {
                        List<short> diagotodelete = new List<short>();
                        foreach (EA.DiagramObject diago in diag.DiagramObjects)
                        {
                            elemid = diago.ElementID;
                            EA.Element diagoelt = repo.GetElementByID((int)elemid);
                            string prov7 = diagoelt.Name;
                            //EA.Element profelt=null;
                            if (util.IsInProfile(repo, diagoelt, profilepack)) continue;// l'element est dans le profile
                                                                                        // sinon il faut aller le chercher
                            try
                            {
                                if (profelements.ContainsKey(diagoelt.Name))
                                {
                                    elemid = profelements[diagoelt.Name];
                                    //profelt = repo.GetElementByID((int)elemid);
                                    //il faut remplacer l'element par celui du profil
                                    //diago.ElementID =(int) elemid;
                                    // diago.Update();

                                    util.replacediagobj(diag, diago, diagoelt.Name, elemid, diagotodelete);
                                }
                                else
                                {
                                    continue;
                                }

                            }
                            catch (Exception e)
                            {
                                util.wlog("GlobalIBOCopy", " erreur acces repository pour ElemntID=" + elemid.ToString() + "  " + e.Message);
                            }

                        }
                        foreach (short j in diagotodelete)
                        {
                            diag.DiagramObjects.Delete(j);
                        }
                        diag.DiagramObjects.Refresh();
                        diag.Update();
                    }



                }

                //ManageIBOUtilitaires.reportlog.Close();
            
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
                                       // util.wlog("GlobalIBOCopy",texte);
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
            // util.getAllElements(profpack, listprofelements, Cimstereos);
            util.GetAllElements(profpack, listprofelements);
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
          string texte;
          string problem;
          EA.TaggedValue tagv;

          /*********** pour test **************/
          texte = "Appel fonction traitelement avec " + repository.GetPackageByID(el.PackageID).Name + "::" + el.Name;
         // util.wlog("GlobalIBOCopy",texte);
          /************************************/

          problem = "";
          texte = "create tag value for element=" + el.Name + " in '" + repository.GetPackageByID(el.PackageID).Name;
          texte = texte + "' from element=" + IBOel.Name + " in '" + repository.GetPackageByID(IBOel.PackageID).Name;
        //  util.wlog("GlobalIBOCopy",texte); // for test

          /****************************************************/
           foreach (EA.TaggedValue tag in el.TaggedValues)
           {
               texte = "tag value of element=" + el.Name + "=> " + tag.Name + "," + tag.Value ;
           //    util.wlog("GlobalIBOCopy",texte); // for test
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
                    EA.Connector newcon = (EA.Connector)newel.Connectors.AddNew("", "Dependency");
                    newcon.ClientID = newel.ElementID;
                    newcon.SupplierID = el.ElementID;
                    newcon.Stereotype = "isBasedOn";
                    newcon.Update();
                    newel.Connectors.Refresh();
                    newel.Update();

                }
                else
                {
                 foreach(EA.Connector co in el.Connectors)
                   if(
                           ( (co.Type=="Dependency") && (co.Stereotype=="isBasedOn")))
                            {
                            EA.Connector newcon = (EA.Connector)newel.Connectors.AddNew("", "Dependency");
                            newcon.ClientID = newel.ElementID;
                            newcon.SupplierID = co.SupplierID;
                            newcon.Stereotype = "isBasedOn";
                            newcon.Update();
                            newel.Connectors.Refresh();
                            newel.Update();
                        }
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
                                el.Connectors.Refresh();
                                el.Update();

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
        /// program to automaticaly download in the profile (targetedPackage) all the non
        /// yet existing ancesters of a profile class just downloaded
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="targetedPackage"></param>
        /// <param name="IBOElement"></param>
       
        public void addAncesters(EA.Repository repo, EA.Package targetedPackage, EA.Element IBOElement)
        {
            try
            {
                UtilitiesConstantDefinition UCD = new UtilitiesConstantDefinition();
                Utilitaires util = new Utilitaires(repo);
                EA.Element parentElement = repo.GetElementByGuid(util.getEltParentGuid(IBOElement));
                Dictionary<long, EA.Element> dicAncestersByID = new Dictionary<long, EA.Element>();// gives the ancester element by its ID
                Dictionary<long, EA.Element> dicProfelementsByAncesterID = new Dictionary<long, EA.Element>();// gives the corresponding profelement by its ID
                List<long> ancesters = new List<long>();
                List<EA.Element> downloadedAncesters = new List<EA.Element>();
                EA.Package togopack = null;
                util.getAncesters(parentElement, ancesters, dicAncestersByID);
                if (!dicAncestersByID.ContainsKey(parentElement.ElementID)) dicAncestersByID[parentElement.ElementID] = parentElement;
                List<EA.Element> profelements = new List<EA.Element>();
                IBOElement.Update();
                targetedPackage.Update();
                // util.GetAllElements(targetedPackage, profelements);
                util.GetAllElements(profilepack, profelements);

                // we now collect all the children which are already in the profile

                foreach (EA.Element el in profelements) // collect the profelement instance of an ancester
                {
                    string prov = el.Name;
                    EA.Element parent = repo.GetElementByGuid(util.getEltParentGuid(el));
                    long parentid = parent.ElementID;
                    if (dicAncestersByID.ContainsKey(parentid))// this element is an ancester
                    {
                        if (!dicProfelementsByAncesterID.ContainsKey(parentid)) dicProfelementsByAncesterID[parentid] = el;
                    }
                }
                if (!dicProfelementsByAncesterID.ContainsKey(IBOElement.ElementID)) dicProfelementsByAncesterID[parentElement.ElementID] = IBOElement;

                foreach (long ancestid in dicAncestersByID.Keys)
                {
                    if (!dicProfelementsByAncesterID.ContainsKey(ancestid)) // this anscester must be downloaded
                    {
                        //togopack = util.getWhereToBeCopied(repo, dicAncestersByID[ancestid], targetedPackage);
                        togopack = util.getWhereToBeCopied(repo, dicAncestersByID[ancestid], targetedPackage, this.profilepack);
                        EA.Element profel = util.copyElement(dicAncestersByID[ancestid], togopack, "sanslien");
                        downloadedAncesters.Add(profel);
                        dicProfelementsByAncesterID[ancestid] = profel;
                    }
                }
                // now we create the generalization hierarchy 
                foreach (long ancestid in dicAncestersByID.Keys)
                {

                    EA.Element profel = dicProfelementsByAncesterID[ancestid];
                    string prov = profel.Name;
                    bool ok = false; // true if the profelement ihherits already from another profelement
                    short ind = 0;
                    short indtodele = 0;
                    foreach (EA.Connector con in profel.Connectors)
                    {
                        
                        if ((con.Type == "Generalization") && (profel.ElementID == con.ClientID))
                        {
                            if(!dicProfelementsByAncesterID.ContainsKey(con.SupplierID))
                            {
                               ok = true;
                            }
                            else
                            {
                                // il s'agit d'un lien hierarchique avec un element exterieur au profil
                                // il faut detruire le lien
                                indtodele = ind;
                            }
                            
                        }
                        ind++;
                    }
                    if(indtodele != 0)
                    {
                        //il faut detruire le connecteur correspondant
                        profel.Connectors.DeleteAt(indtodele, true);
                        profel.Connectors.Refresh();
                        profel.Update();

                    }
                    if (ok) continue; // nothing todo
                    // we mut establish the generalization association
                    EA.Element ancester = dicAncestersByID[ancestid]; // recover the ancester
                                                                      // look for its ancester
                    string guid = ""; // the connector guid
                    int theotherendid = 0;
                    foreach (EA.Connector con in ancester.Connectors)
                    {
                        if ((con.Type == "Generalization") && (ancester.ElementID == con.ClientID))
                        {
                            ok = true;
                            guid = con.ConnectorGUID;
                            theotherendid = dicProfelementsByAncesterID[con.SupplierID].ElementID;
                            break;
                        }
                    }
                    if (ok) // there is an ancester an association mustbe created
                    {
                        EA.Connector newcon = (EA.Connector)profel.Connectors.AddNew("", "Generalization");
                        newcon.ClientID = profel.ElementID;
                        newcon.SupplierID = theotherendid;
                        newcon.Update();
                        profel.Connectors.Refresh();
                        EA.ConnectorTag contag = (EA.ConnectorTag)newcon.TaggedValues.AddNew(UCD.GetIBOTagValue(), guid);
                        contag.Update();
                        newcon.TaggedValues.Refresh();
                        newcon.Update();
                        profel.Update();
                    }

                }
            }
            catch (Exception ee)
            {
                Utilitaires util = new Utilitaires(repo);
                util.wlog("addAncesters", "error in addAncesters " + ee.Message);
            }

        }
        //-----------------------------------------------------------

    }

}