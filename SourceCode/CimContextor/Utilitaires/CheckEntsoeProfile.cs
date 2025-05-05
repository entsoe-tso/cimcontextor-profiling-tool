using EA;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CimContextor;
using System.Data.Common;
using System.Runtime.InteropServices;
/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.9.24                                       *  november 2020 *
*                                                                         *
***************************************************************************
*                                                                         *
*       Credit to:                                                        *
*                   Andre Maizener   andre.maizener@zamiren.fr            *
*                   Jean-Luc Sanson  jean-luc.sanson@noos.fr              *
*                                                                         *
*       Contact: +33148854006                                             *
*                                                                         *
***************************************************************************

**************************************************************************/
namespace CimContextor.utilitaires
{
    public  class CheckEntsoeProfile
    {
        EA.Repository repo = null;
        //int Rang = 0;  // for ordering the attributes
     //   public static Utilitaires selfut = new Utilitaires();
        public XMLParser XMLP;
        public  List<string> Facets;
        ConstantDefinition CD = new ConstantDefinition();
        //string ID = ""; // identfier of the rule in the specification document table
        public static Dictionary<string, List<long>> dicProfElemByParentGuid = new Dictionary<string, List<long>>();
        public static List<EA.Element> ListElem = new List<EA.Element>();
        public static Dictionary<string, long> dicDatatypes = new Dictionary<string, long>();// datatypename/id 
        public Dictionary<string, List<long>> dicACCguidToABIEcon = new Dictionary<string, List<long>>();//<accvonguid/abieconid>
        public string ListStereoNamespace = "";
        Dictionary<string, EA.Package> dicProfPackages = new Dictionary<string, Package>();// pakages in a profile
        string domainPackage = ""; // the name of the package containing the datatypes
        Dictionary<string, int> dicCheckPCols = new Dictionary<string, int>(); // nom colonne/index
        ManipTable maniptable = null;
        public bool therearewarnings = false;
        public Utilitaires ul = null;
        List<EA.Package> CIMPackages = new List<EA.Package>();  // packages on which profile is based on
        Dictionary<string, EA.Package> dicCIMPackages  = new Dictionary<string, EA.Package>(); // packeges of the CIM
        Dictionary<string, Dictionary<string, string>> EnumValuesdic = new Dictionary<string, Dictionary<string, string>>(); //  enumeration/dict(member/initialvalue)
        Dictionary<string, Dictionary<string, string>> ProfEnumValuesdic = new Dictionary<string, Dictionary<string, string>>();
        public  int errors=0; // errors (issues) counter
        List<long> conAlreadyTreated = null;
        bool isESMP = false;  // specific to ESMP 
        Array ErrorTable;
        string[] linerror = null;
        
        

        public CheckEntsoeProfile(EA.Repository repos, EA.Package thepack) //constructor          
        {
            try
            {
                errors = 0;
                
                linerror = new string[CD.CheckPColumsNames.Count];
                ul = new Utilitaires(repos);
                conAlreadyTreated = new List<long>();
                this.repo = repos;
                ul.setRepo(repos);
                isESMP = false;
                XMLP = new XMLParser(repo);
               
                ListStereoNamespace = XMLP.GetXmlValueProfData("ListStereoNamespace");
                domainPackage = XMLP.GetXmlValueProfData("EntsoeDataTypesDomain");
                ErrorTable = Array.CreateInstance(typeof(string), CD.CPTABLERAWDIM, CD.CheckPColumsNames.Count);

                for (int i = 0; i < CD.CheckPColumsNames.Count; i++)
                {
                    dicCheckPCols.Add(CD.CheckPColumsNames[i], i);

                }
                maniptable = new ManipTable(ErrorTable, ul);
                CIMPackages = getCIMPackages(thepack);
                if(CIMPackages==null)
                {
                    throw new Exception(" CheckProfile profilepackage not based on CIM");
                }
                foreach(EA.Package pa in CIMPackages)
                {
                    if (pa.Name == "IEC62325") isESMP = true;
                    break;
                }
                // debut des messages d'erreur 
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = CD.CheckPColumsNames[0];
                linerror[dicCheckPCols["Severity"]] = CD.CheckPColumsNames[1];
                linerror[dicCheckPCols["Package"]] = CD.CheckPColumsNames[2];
                linerror[dicCheckPCols["Class"]] = CD.CheckPColumsNames[3];
                linerror[dicCheckPCols["Attribute"]] = CD.CheckPColumsNames[4];
                linerror[dicCheckPCols["Association"]] = CD.CheckPColumsNames[5];
                linerror[dicCheckPCols["CIMText"]] = CD.CheckPColumsNames[6];
                linerror[dicCheckPCols["ProfileText"]] = CD.CheckPColumsNames[7];
                maniptable.setErrorLine(linerror);
                emptyLine(linerror);


                verif_profile(repos, thepack);
              
            }
            catch (Exception e)
            {
                ul.wlog("CheckProfile", " ERROR in validating the profile " + e.Message);
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "CheckAborted";
                linerror[dicCheckPCols["Severity"]] = "Violation";
                linerror[dicCheckPCols["Package"]] = thepack.Name;           
                maniptable.setErrorLine(linerror);
                maniptable.ToFile();
                maniptable.tableFile.Close();
                 return;
            }
            maniptable.ToFile();
            maniptable.tableFile.Close();
       
        }
        /// <summary>
        /// 
        /// </summary>
        /// recurssive verification for each package, elements and association of a selected package;
        /// - for entsoe names of the packages in a package profile must have the same name as in teh CIM
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        public void  verif_profile(EA.Repository repos, EA.Package thepack)
        {
            string prov = thepack.Name;
            //if (prov.Contains("LimitsDocument"))  ABA20230103
            //{
            //    prov = thepack.Name;
            //}
            if (!verif_coherence_pa(repos, thepack))
            {
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "CheckAborted";
                linerror[dicCheckPCols["Severity"]] = "Violation";
                linerror[dicCheckPCols["Package"]] = thepack.Name; ;            
                maniptable.setErrorLine(linerror);
                ul.wtest("TEST", String.Format("verif_ profile  checkaborted pack={0}", thepack.Name));
                return;
            }

            


            bool r = ul.getAllPackagesInAPackage(repo, thepack, dicProfPackages);
            if (!r)
            {
                emptyLine(linerror);
                linerror[dicCheckPCols["Typ"]] = "CheckAborted";
                linerror[dicCheckPCols["Severity"]] = "Violation" ;
                linerror[dicCheckPCols["Package"]] = thepack.Name; ;
                maniptable.setErrorLine(linerror);
                return;
            }
            //***************** note of the profile
            if (thepack.Notes != "")
            {
                // ID = "6";
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "PackageDescriptionIs";
                linerror[dicCheckPCols["Severity"]] = "Info";
                linerror[dicCheckPCols["Package"]] = thepack.Name;

                if (thepack.Notes.Length < CD.CPSUBNOTELENGTH)
                {
                    string notes = thepack.Notes;
                    int indn = thepack.Notes.IndexOf("\n");
                    if (indn == -1)
                    {
                        linerror[dicCheckPCols["ProfileText"]] = thepack.Notes;
                    }
                    else
                    {
                        int indr = thepack.Notes.Substring(0, indn).IndexOf("\r");
                        if (indr == -1)
                        {
                            linerror[dicCheckPCols["ProfileText"]] = thepack.Notes;
                        }
                        else
                        {
                            linerror[dicCheckPCols["ProfileText"]] = thepack.Notes.Substring(0, indr);

                        }
                    }
                }
                else
                {
                    int indn = thepack.Notes.IndexOf("\n"); ;
                    int indr = thepack.Notes.Substring(0, indn).IndexOf("\r");

                    linerror[dicCheckPCols["ProfileText"]] = thepack.Notes.Substring(0, indr) + " ...";
                }
               
                maniptable.setErrorLine(linerror);
            }
            // ******************** the names of the package in a profile must be unique
            List<string> packages = new List<string>();
            foreach(EA.Package pa in thepack.Packages)
            {
                if(!packages.Contains(pa.Name))
                {
                    packages.Add(pa.Name);
                }
                else
                {
                    // ID = "2";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "NotUniquePackageName";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["ProfileText"]] = "package names must be unique" + pa.Name ;
                    maniptable.setErrorLine(linerror);
                    break;
                }
            }


            // **************** the names of the profile packages must be names of the CIM packages
            List<string> profpacknames = new List<string>();
            foreach (string packname in dicProfPackages.Keys)
            {
                if(!profpacknames.Contains(packname)) profpacknames.Add(packname);
            }
            List<string> notfounds = profpacknames;
            List<string> notyetfounds = new List<string>();
            foreach (EA.Package cimpa in CIMPackages)
            {
                dicCIMPackages = new Dictionary<string, Package>();


                bool rr = ul.getAllPackagesInAPackage(repo, cimpa, dicCIMPackages);
                if (!rr)
                {
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Typ"]] = "CheckAborted";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name; 
                    maniptable.setErrorLine(linerror);
                    return;
                }

                //**************** are the package names of the profile good


                foreach (string packname in notfounds)
                {
                    if (!dicCIMPackages.ContainsKey(packname))
                    {
                        notyetfounds.Add(packname);
                    }
                }
                notfounds=notyetfounds;
            }

            if (notfounds.Count > 0)
            {
                string badpackages = "";
                foreach(string  panam in notfounds)
                {
                    badpackages = badpackages + "," + panam;
                }
                emptyLine(linerror);
                // ID = "7";
                linerror[dicCheckPCols["Message"]] = "NotGoodPackageNames";
                linerror[dicCheckPCols["Severity"]] = "Warning";
                linerror[dicCheckPCols["Package"]] = badpackages;
                maniptable.setErrorLine(linerror);
            }

            //***************  verification that 2 classes have not the same name  //

            List<EA.Element> listelem = new List<EA.Element>();
            List<string> listelemnames = new List<string>();
            ul.GetAllElements(thepack, listelem);
            foreach(EA.Element el in listelem)
            {
                if (el.Type == "Boundary") continue; // the boundaries may have the same name
                if ((el.Name != "") && !listelemnames.Contains(el.Name))
                {
                    listelemnames.Add(el.Name);
                }
                else
                {
                    // ID = "8";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "DuplicateElementName";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;               
                    maniptable.setErrorLine(linerror);
                    emptyLine(linerror);
                }
            }

          
            


            //****************** validation of the profile elements ********************
            verif_package(repos, thepack);
            //*************** validation of packages *******************

            
                foreach (EA.Package pack in dicProfPackages.Values)
                {
                    verif_package(repos, pack);
                }

            }
        
        /// <summary>
        /// -gives an info if there is an unattached not
        /// -verif all elements in case of the package profile
        /// 
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="pa"></param>
        void verif_package(EA.Repository repos, EA.Package pa)
        {
            try
            {
                ul.wtest("TEST", "verif_package  thepack=" + pa.Name);
                // if there are constraints OCL they must appear
                foreach (EA.Constraint cst in pa.Element.Constraints)
                {
                    if (cst.Type == "OCL")
                    {
                        // ID = "5";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "HasOCLConstraint";
                        linerror[dicCheckPCols["Severity"]] = "Info";
                        linerror[dicCheckPCols["Package"]] = pa.Name;
                        linerror[dicCheckPCols["ProfileText"]] = "Constraint of Type" + cst.Type + "=" + cst.Name;
                        maniptable.setErrorLine(linerror);
                    }
                }
                if (pa.Notes == "") // the description is empty
                {
                        // ID = "36";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "HasEmptyNote";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = pa.Name;
                        linerror[dicCheckPCols["ProfileText"]] = " description is empty";
                        maniptable.setErrorLine(linerror);
                }
                foreach (EA.Element el in pa.Elements)
                {
                        string prov1 = el.Name + " ," + el.Type;
                        switch (el.Type)
                        {
                            case "Note":
                                string misc = el.get_MiscData(3);// is empty if note associated to an association
                                if (el.Connectors.Count == 0)
                                {
                                    if (misc == "")
                                    {
                                        // the note is not attached to a Class or an association but belongs to the package
                                        // ID = "9";
                                        emptyLine(linerror);
                                        linerror[dicCheckPCols["Message"]] = "HasNonAttachedNote";
                                        linerror[dicCheckPCols["Severity"]] = "Warning";
                                        linerror[dicCheckPCols["Package"]] = pa.Name;
                                    string texte = el.Notes.Replace("\n", "");
                                    texte = texte.Replace("\r", "");
                                    if (el.Notes.Length < CD.CPSUBNOTELENGTH)
                                        {
                                            
                                            linerror[dicCheckPCols["ProfileText"]] = " Beginning of the note =\"" + texte;
                                        }
                                        else
                                        {
                                            linerror[dicCheckPCols["ProfileText"]] = " Beginning of the note =\"" + texte.Substring(0, CD.CPSUBNOTELENGTH) + " ...";
                                        }            
                                        maniptable.setErrorLine(linerror);
                                    }
                                }
                                else
                                {
                                    foreach (EA.Connector con in el.Connectors)
                                    {
                                        if (con.Type != "NoteLink") continue;  // only note links are treated at this level

                                        long otherID = 0;
                                        if (el.ElementID == con.ClientID)
                                        {
                                            otherID = con.SupplierID;
                                        }
                                        else
                                        {
                                            otherID = con.ClientID;
                                        }
                                        try
                                        {
                                            EA.Element elt = repo.GetElementByID((int)otherID);
                                            continue; // this will be treated with the class
                                        }
                                        catch (Exception e)
                                        {
                                        ul.wlog("Error", "verif_pakage " + e.Message);
                                        }

                                    }

                                }
                                break;
                            case "Boundary":
                                break;
                            default:
                                verif_element(pa, el);
                                break;

                        }
                    ul.wtest("TEST", String.Format("element={0} dumpfic={1}", el.Name, maniptable.tableFile));
                }
            }
            catch (Exception e)
            {
                ul.wlog("CheckProfile", " ISSUE in checking package=" + pa.Name + " , " + e.Message);
            }
        }
        public CheckEntsoeProfile() //constructor
        {
            this.repo = null;
        }
        public void setRepo(EA.Repository repos)
        {
            this.repo = repos;
        }
        /// <summary>
        /// to write log enties
        /// </summary>
        /// <param name="texte"></param>

        /// <summary>
        /// possible consistency verification at the level of package
        ///  a profile package should be 
        /// - either directly based on
        /// - or in a package isbased on
        /// - a package may be envelopp of other non based on package
        ///      -if it is basedon it will be consistent and all its subdirectory will bee
        ///      - if is not basedon it will be inconsistent with all non isbasedon subdirectory
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        bool verif_coherence_pa(EA.Repository repos, EA.Package thepack)
        {
              ul.wtest("TEST", "verif_coherence  thepack=" + thepack.Name );

            string[] linerror = new string[CD.CheckPColumsNames.Count];
            /*****  profile must be basedon ****/
            string texte = " info:  ++++++++  the package " + thepack.Name + " is checked for consistency ++++++++++++";
            //if(thepack.Name.Contains("Limits")) ABA20230103
            //{
            //    string prov=thepack.Name;
            //}
            ul.wlog("CheckProfile", texte);
           
            if (!ul.HasIBOPackage(thepack.PackageID))
                {
                // ID = "1";
                linerror[dicCheckPCols["Message"]] = "NotIsBasedOnPackage";
                linerror[dicCheckPCols["Package"]] = thepack.Name;
                linerror[dicCheckPCols["Class"]] = "";
                linerror[dicCheckPCols["Attribute"]] = "";
                linerror[dicCheckPCols["Association"]] = "";
                linerror[dicCheckPCols["CIMText"]] = "";
                linerror[dicCheckPCols["ProfileText"]] = "";
                maniptable.setErrorLine(linerror);
                ul.wtest("TEST", String.Format("verif_coherence issue 1 thepack={0}", thepack.Name));
                }
                //
            
                /*** all package of a profile must have a unique name ****/
           

                Dictionary<string, EA.Package> lpa = new Dictionary<string, EA.Package>();
                /* ABA 20230219 OBSOLETE
                if (thepack.Packages.Count > 0)
                {
                    if (!ul.getAllPackagesInAPackage(repos, thepack, lpa))
                    {

                        result = false;
                    }
                }
                */
                //*** the CIM and the Profile must have a version element
                string[] profileversion = getEntsoeProfileVersion(thepack); 
                
                string cimv = "";
                string cimd = "";
                foreach (EA.Package cimpa in CIMPackages)
                {
                    string[] cimversion = getCIMVersion(cimpa);
                
                    if (cimversion[0] == "")
                    {
                    ul.wlog("WARNING", String.Format("verif_coherence_pa issue 3 cimpa={0} thepack={1}", cimpa.Name, thepack.Name));
                    /*
                    ID = "3";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "NotAGoodCimVersion";
                        linerror[dicCheckPCols["Severity"]] = "Warning";
                        linerror[dicCheckPCols["CIMText"]] = "CIMpackage=" + cimpa.Name + " The Version element must be present ";
                        maniptable.setErrorLine(linerror);
                        ul.wtest("TEST", String.Format("verif_coherence_pa issue 3 cimpa={0} thepack={1}", cimpa.Name,thepack.Name));
                        //return result = false;
                    */
                }
                
                    if (cimv == "")
                    {
                        cimv = cimversion[0];
                    }
                    else
                    {
                        cimv = cimv + "," + cimversion[0];
                    }
                    if (cimd == "")
                    {
                        cimd = cimversion[1];
                    }
                    else
                    {
                        cimd = cimd + "," + cimversion[1];
                    }
                }
                if (profileversion[0] == "")
                {
                // ID = "4";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "NotAGoodProfileVersion";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["ProfileText"]] = "The Version element must be present and single in the Profile Package";
                    maniptable.setErrorLine(linerror);
                ul.wtest("TEST", String.Format("verif_coherence_pa issue 4  thepack={0}", thepack.Name));
                //return result = true; //was false  ABA20230219
                return true; // ABA20230219
                }

                if (profileversion[2] != "1")
                {
                    // ID = "3";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "NotAGoodProfileOntology";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["ProfileText"]] = "The Ontology element must be present and single in the Profile Package";
                    maniptable.setErrorLine(linerror);
                    ul.wtest("TEST", String.Format("verif_coherence_pa issue 4  thepack={0}", thepack.Name));
                    //return result = true; //was false  ABA20230219
                    return true;
            }

            //***********  line to mark the validation
            // ID = "41";
            emptyLine(linerror);
            linerror[dicCheckPCols["Message"]] = "InfoOnCheck";
            linerror[dicCheckPCols["Severity"]] = "Info";
            linerror[dicCheckPCols["Package"]] = thepack.Name;
            //linerror[dicCheckPCols["Class"]] = " CIMversion=" +cimv+ " , date=" + cimd ;
            linerror[dicCheckPCols["ProfileText"]] = "Profile Version=" + profileversion[0] + " , from CIMVersion="  +cimv + " , date=" + cimd; 
            maniptable.setErrorLine(linerror);

            //************** publish  note of the packages *******************

            ul.wtest("TEST", String.Format("verif_coherence_pa issue 41 cimv={0} thepack={1}", cimv, thepack.Name));
            return true;
        }
        //---------------------- 
           void emptyLine(string[] line)
        {
            for (int i = 0; i < CD.CheckPColumsNames.Count; i++)
            {
                line[i] = "";
            }
        }
        //-----------------------------------
        /// <summary>
        /// check the consistency of elements
        /// for classes we verify
        /// - it is isBasedOn
        /// - the name of parent is the same 
        /// - there are notes
        /// -is isabstract when cim is not
        /// -it has some additional stereotype 
        /// - it has the same notes as in CIM
        /// - has somedifferent  tagvalues (in top of guid)
        /// -has some constraints
        /// the attributes are well is basedOn
        /// - there are more attributes in CIM
        /// - the attributes have the same name
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        /// <param name="el"></param>
        void  verif_element( EA.Package thepack, EA.Element el)
        {
            ul.wtest("TEST", "verif_element  thepack=" + thepack.Name + "  el=" + el.Name +  " type=" + el.Type);
            string prov = el.Name;
            if (el.Name == "ExcitationSystemDynamics")
            {
               prov = el.Type;
            }
            try
            {
                EA.Element parentel = null;
                if (el.Type == "Note") return; // the only elements checked at this stage are classes and Enumeration if they exist
                if (el.Type == "Boundary") return;
                if (el.Name.Contains("Version")) return; // no need to chek the version element checked before
                if (el.Name != "")// n'est pas une note
                {
                    parentel = verif_ClassIBOIntegrity(thepack, el); // verify isbasedon link
                    if (parentel == null)
                    {
                        ul.wlog("CheckProfile", " Validation of " + el.Name + " abandonned because of found IBO issues");
                        return;
                    }

                    // if there are constraints OCL they must appear
                    foreach (EA.Constraint cst in el.Constraints)
                    {
                        if (cst.Type == "OCL")
                        {
                            // ID = "5";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "HasOCLConstraint";
                            linerror[dicCheckPCols["Severity"]] = "Info";
                            linerror[dicCheckPCols["Package"]] = thepack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            linerror[dicCheckPCols["ProfileText"]] = "Constraint of Type" + cst.Type + "=" + cst.Name;
                            maniptable.setErrorLine(linerror);
                        }

                    }

                    // *************  verif note *************************
                    if (el.Notes == "") // the description is empty
                    {
                        // ID = "36";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "HasEmptyNote";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = thepack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["ProfileText"]] = " description is empty";
                        maniptable.setErrorLine(linerror);
                    }
                    else
                    {
                        if (!el.Notes.Equals(parentel.Notes))
                        {
                            string parentnote = parentel.Notes.Replace('\r', ' ');
                            parentnote = parentnote.Replace('\n', ' ');
                            string elnote = el.Notes.Replace('\r', ' ');
                            elnote = elnote.Replace('\n', ' ');
                            // ID = "11";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "NotSameDescription";
                            linerror[dicCheckPCols["Severity"]] = "Violation";
                            linerror[dicCheckPCols["Package"]] = thepack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            linerror[dicCheckPCols["CIMText"]] = parentnote; ;
                            linerror[dicCheckPCols["ProfileText"]] = el.Notes;
                            maniptable.setErrorLine(linerror);
                        }
                    }

                    //************* verif all connectors which are not association (only for classes  and associations*****************
                    int countbase = 0;
                    foreach (EA.Connector con in el.Connectors)
                    {
                        switch (con.Type)
                        {
                            case "Dependency":
                                if (con.Stereotype == CD.GetIsBasedOnStereotype())
                                {

                                    countbase++;

                                }
                                if (countbase > 1)
                                {
                                    //thereis more than one isbase connection
                                    // ID = "12";
                                    emptyLine(linerror);
                                    linerror[dicCheckPCols["Message"]] = "HasMoreThanOneIsBasedOnLink";
                                    linerror[dicCheckPCols["Severity"]] = "Violation";
                                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                                    linerror[dicCheckPCols["Class"]] = el.Name;
                                    linerror[dicCheckPCols["ProfileText"]] = " isBasedOn " + repo.GetElementByID((int)con.SupplierID).Name;
                                    maniptable.setErrorLine(linerror);
                                }
                                break;

                            case "NoteLink":
                                verif_Note(thepack, el, con);
                                break;
                            case "Association":
                            case "Aggregation":
                            case "Generalization":
                                break;
                            default:
                                // ID = "13";
                                emptyLine(linerror);
                                linerror[dicCheckPCols["Message"]] = "HasAnUnkownLink";
                                linerror[dicCheckPCols["Severity"]] = "Violation";
                                linerror[dicCheckPCols["Package"]] = thepack.Name;
                                linerror[dicCheckPCols["Class"]] = el.Name;
                                linerror[dicCheckPCols["Association"]] = " type of link=" + con.Type;
                                maniptable.setErrorLine(linerror);

                                break;
                        }
                    }

                    //  type = "";// pour test am dec 15
                    switch (el.Type)
                    {
                        case "Class":

                            if (el.StereotypeEx.Contains(CD.GetPrimitiveStereotype()))
                            {
                                // verif_Primitive(thepack, el);
                                // tout a deja ete verifie
                                break;
                            }

                            if (el.StereotypeEx.Contains(CD.GetDatatypeStereotype()))
                            {
                                verif_Datatype(thepack, el);
                                break;
                            }

                            if (
                               (el.StereotypeEx.Contains(CD.GetEnumStereotype()))
                               ||
                                (el.MetaType == "Enumeration")
                               )

                            {
                                verif_Enum(thepack, el);
                                break;
                            }
                            verif_Class(thepack, el, parentel);
                            break;
                        case "Enumeration":
                            verif_Enum(thepack, el);
                            break;
                        default:


                            break;
                    }
                }
                
            }
            catch (Exception e)
            {
                ul.wlog("CheckProfile", " ISSUE in verifying the element  " + thepack.Name +"::" + el.Name + " " + e.Message);
            }

        }

        void verif_Enum(Package thepack, Element el)
        {
            try
            { 
            // check if the enumeration is a subset of its parents 
            // ------------ is based on ----------------------
            string parentguid = ul.getEltParentGuid(el);
                EA.Element parent;
            if (parentguid == "")
            {
                // ID = "26";
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "MissingGUIDBasedOnTag";
                linerror[dicCheckPCols["Severity"]] = "Violation";
                linerror[dicCheckPCols["Package"]] = thepack.Name;
                linerror[dicCheckPCols["Class"]] = el.Name;
                maniptable.setErrorLine(linerror);
                return;
            }
            else
            {
                parent = ul.recupIBOElement(el);
                if (parent == null) 
                    {
                        // ID = "45";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "NotAccessibleElement";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = thepack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        maniptable.setErrorLine(linerror);
                        return;
                }
            }
                //   --- what about the type
            if (// the parenttype is an enumeration we must check the values 
                    // we must inform in case some values are different from the parent
                    (!parent.StereotypeEx.Contains(CD.GetEnumStereotype()))
                    &&
                    (parent.MetaType != "Enumeration")
                    &&
                    (parent.Type == "Enumeration")
               )
            {    
                    // ID = "27";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "ElementInconsistancy";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    linerror[dicCheckPCols["ProfileText"]] = "the parent is not an enumeration " ;
                    maniptable.setErrorLine(linerror);
                    return;
            }
                   
                    // EA.Element attype = repo.GetElementByID((int)at.ClassifierID);
            if (!ProfEnumValuesdic.ContainsKey(el.Name))
            {
                     ProfEnumValuesdic.Add(el.Name, new Dictionary<string, string>());
                     foreach (EA.Attribute member in el.AttributesEx)
                     {
                            ProfEnumValuesdic[el.Name].Add(member.Name, member.Default);
                     }
            }
            if (!EnumValuesdic.ContainsKey(parent.Name))
            {
                     EnumValuesdic.Add(parent.Name, new Dictionary<string, string>());
                     foreach (EA.Attribute memb in parent.AttributesEx)
                     {
                            EnumValuesdic[parent.Name].Add(memb.Name, memb.Default);
                     }
            }
                // the members are a subset of the parent members
                    foreach(EA.Attribute memb in el.Attributes)
                    {
                         if(!EnumValuesdic[parent.Name].ContainsKey(memb.Name))// the member is not in the possible set
                         {
                           // ID = "21";
                           emptyLine(linerror);
                           linerror[dicCheckPCols["Message"]] = "InconsistantEnumeratedLiteral";
                           linerror[dicCheckPCols["Severity"]] = "Violation";
                           linerror[dicCheckPCols["Package"]] = thepack.Name;
                           linerror[dicCheckPCols["Class"]] = el.Name;
                           linerror[dicCheckPCols["Attribute"]] = memb.Name;
                           maniptable.setErrorLine(linerror);
                         }
                         else
                         {
                             verif_attribute(thepack, el, memb, "member");
                         }
                    }
                    
               
        }
         catch (Exception e)
         {
                ul.wlog("CheckProfile", " ISSUE in verifying the element  " + thepack.Name + "::" + el.Name + " " + e.Message);
         }

        }

        private void verif_Datatype(Package thepack, Element el)
        {
           
            
                if (el.Notes == "") // the description is empty
                {
                    // ID = "36";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "HasEmptyNote";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    linerror[dicCheckPCols["ProfileText"]] = " description is empty";
                    maniptable.setErrorLine(linerror);
                }
            
           // ul.wtest("TEST", "verif_Datatype  thepack=" + thepack.Name + " el=" + el.Name );
            // we must check what is specific to datatype

            foreach(EA.Attribute at in el.Attributes)
            {
                if (at.Name == "value")
                {
                    verif_attribute(thepack, el, at, "valuedatatype");
                }
                else
                {
                    verif_attribute(thepack, el, at, "value");
                }
            }
        }

        private void verif_Compound(Package thepack, Element el)
        {
            throw new NotImplementedException();
        }

        private void verif_Primitive(Package thepack, Element el)
        {
            throw new NotImplementedException();
        }

        private void verif_primitive(Package thepack, Element el)
        {
            throw new NotImplementedException();
        }

        private void verif_Enumeration(Package thepack, Element el)
        {
            throw new NotImplementedException();
        }

        private void verif_Note(EA.Package thepack, EA.Element el,EA.Connector con )
        {
          //  ul.wtest("TEST", "verif_Note  thepack=" + thepack.Name + " el=" + el.Name + " client="+ con.ClientEnd.Role + " supplier=" + con.SupplierEnd.Role );
            EA.Element note=null;
            try
            { 
            if(con.ClientID==el.ElementID)
            {
                note = repo.GetElementByID((int)con.SupplierID);
            }
            else
            {
                note = repo.GetElementByID((int)con.ClientID);
            }
            }
            catch (Exception)
            {
                ul.wlog("CheckFile", " ISSUE in accessing attached note of " + el.Name + " in EA Repository");
                return;
            }
            if (note.Type != "Note")
            {
                return;
            }
            // the note is  attached to a Class 
            // ID = "42";
            emptyLine(linerror);
            linerror[dicCheckPCols["Message"]] = "HasAttachedNote";
            linerror[dicCheckPCols["Severity"]] = "Info"; 
            linerror[dicCheckPCols["Package"]] = thepack.Name;
            linerror[dicCheckPCols["Class"]] = el.Name;
            if (note.Notes.Length < CD.CPSUBNOTELENGTH)
            {
                linerror[dicCheckPCols["ProfileText"]] = " Beginning of the note =\"" + note.Notes;
            }
            else
            {
                linerror[dicCheckPCols["ProfileText"]] = " Beginning of the note =\"" + note.Notes.Substring(0, CD.CPSUBNOTELENGTH);
            }

            if (note.Notes.Length > CD.CPSUBNOTELENGTH)
            {
                linerror[dicCheckPCols["ProfileText"]] = linerror[dicCheckPCols["ProfileText"]] + " ...\"";
            }
            maniptable.setErrorLine(linerror);
        }

        private void verif_Class(EA.Package thepack, EA.Element el , EA.Element parentel)
        {
            
            ul.wtest("TEST", "verif_Class  thepack=" + thepack.Name + " el=" + el.Name + " parentel=" + parentel.Name);
            if (el.Name == "ACDCConverter")
            {
                string gg = el.Name;
                
            }
            //******************** verif abstract ***************************************
            if (el.Abstract != parentel.Abstract)
            {

                // ID = "43";
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "NotSameAbstract";
                linerror[dicCheckPCols["Severity"]] = "Warning";
                linerror[dicCheckPCols["Package"]] = thepack.Name;
                linerror[dicCheckPCols["Class"]] = el.Name;
                linerror[dicCheckPCols["CIMText"]] = " Abstract=" + parentel.Abstract;
                linerror[dicCheckPCols["ProfileText"]] = " Abstract=" + el.Abstract;
                maniptable.setErrorLine(linerror);
            }
            // is based on, constrainsts, stereotypes and notes  consistency have been tested previously
            foreach (EA.Attribute at in el.Attributes)
            {
                
                    verif_attribute(thepack, el, at, "value"); // check if this attribute is well based on and that it has the same name
                                                               // useful also if one attribute has been deleted in Information Model
            }
            //***   check the consistency of inheritance chain  ***
            // A class must have the same inheritance chain as in the CIM
            List<EA.Element> IBOchain = new List<EA.Element>();
            List<EA.Element> chain = new List<EA.Element>();
            EA.Element parentelt = ul.recupIBOElement(el);
            ul.GetIheritanceChain(parentelt, IBOchain,true);

            ul.GetIheritanceChain(el, chain,false);
            string chemin="";
            if(IBOchain.Count != chain.Count)
            {
                for (int i = IBOchain.Count - 1; i >= 0; i--)
                {
                    
                        if (i == 0)
                        {
                            chemin = chemin + IBOchain[0].Name;
                        }
                        else
                        {
                            chemin = chemin + IBOchain[i].Name + "/";
                        }
                    
                }
                chemin = chemin + "/" + el.Name;
                // ID = "37";
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "NotSameInheritance Chain";
                linerror[dicCheckPCols["Severity"]] = "Warning";
                linerror[dicCheckPCols["Package"]] = thepack.Name;
                linerror[dicCheckPCols["Class"]] = el.Name;
                linerror[dicCheckPCols["CIMText"]] = " Cim path: " + chemin;
                linerror[dicCheckPCols["ProfileText"]] = ""; 
                maniptable.setErrorLine(linerror);
            }
            
                //***** check the associations     *****
                
                foreach (EA.Connector con in el.Connectors)
                {
                    switch(con.Type)
                    { 
                        case "Association":
                        case "Aggregation":
                    verif_association( thepack, el,con);
                    break;
                        case "Generalization":
                    verif_generalization(thepack, el, con);
                    break;
                     default:
                    break;
                    }
                }
            
            }

        private void verif_generalization(Package thepack, Element el, Connector con)
        {
          //  ul.wtest("TEST", "verif_generalization  thepack=" + thepack.Name + " el=" + el.Name + " client=" + con.ClientEnd.Role + " supplier=" + con.SupplierEnd.Role);
            if (el.ElementID != con.ClientID)
            {
                // we check only the ancestors , not the descendants
                return;
            }
            EA.Element parentel = ul.recupIBOElement(el);
            EA.Element ancestor = null;
            EA.Element parentancestor = null;
            try
            {
                ancestor = repo.GetElementByID((int)con.SupplierID);
                parentancestor = ul.recupIBOElement(ancestor);

                if ((ancestor != null) && (parentancestor == null))
                {
                    // ID = "44";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "HasInheritanceIssue";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    linerror[dicCheckPCols["ProfileText"]] = "ancestor=" + ancestor.Name;
                    maniptable.setErrorLine(linerror);
                }
                else
                {
                    bool ok=false;
                    foreach (EA.Connector co in parentel.Connectors)
                    {
                        if (((co.Type == "Generalization") && (co.SupplierID == parentancestor.ElementID))

                            )
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (!ok)
                    {
                        // ID = "44";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "HasInheritanceIssue";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = thepack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["CIMText"]] = "parentel=" + parentel.Name + "," + "parentancestor=" + parentancestor.Name;
                        linerror[dicCheckPCols["ProfileText"]] = "ancestor=" + ancestor.Name;
                        linerror[dicCheckPCols["CIMText"]] = "parentancestor=" + parentancestor.Name;
                        maniptable.setErrorLine(linerror);
                    }
                }
             }catch(Exception e)
                    {
                        ul.wlog("CheckFile", " ISSUE in accessing the ancsestor of  " + el.Name + " in EA Repository " + e.Message);
                    }
        }


        
        /// <summary>
        /// check if
        /// an attribute is correctly basedon
        /// its name is not changed
        /// check if it has an idenfier
        /// inform which identifier
        /// </summary>
        /// <param name="el"></param>
        /// <param name="at"></param>
        void verif_attribute(EA.Package pack, EA.Element el, EA.Attribute at, string type)
        {
            ul.wtest("TEST", "verif_attribute  thepack=" + pack.Name + " el=" + el.Name + "atr=" + at.Name);

            EA.Element classifier = null;

            // ul.wtest("TEST", "verif_attribute  thepack=" + pack.Name + " el=" + el.Name + " at=" + at.Name +  " valuetype=" + at.Type);
            switch (type)
            {
                case "value":
                case "valuedatatype":
                    if (!el.StereotypeEx.Contains("CIMDatatype"))
                    {
                        if (at.Notes == "") // the description is empty
                        {
                            // ID = "36";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "HasEmptyNote";
                            linerror[dicCheckPCols["Severity"]] = "Violation";
                            linerror[dicCheckPCols["Package"]] = pack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            linerror[dicCheckPCols["Attribute"]] = at.Name;
                            linerror[dicCheckPCols["ProfileText"]] = " description is empty";
                            maniptable.setErrorLine(linerror);
                        }
                    }
                    /********  check constraints  ************************/

                    foreach (EA.AttributeConstraint cst in at.Constraints)
                    {
                        if (cst.Type == "OCL")
                        {
                            // ID = "5";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "HasConstraint";
                            linerror[dicCheckPCols["Severity"]] = "Violation";
                            linerror[dicCheckPCols["Package"]] = pack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            linerror[dicCheckPCols["Attribute"]] = at.Name;
                            linerror[dicCheckPCols["ProfileText"]] = "Constraint of Type" + cst.Type + "=" + cst.Name;
                            maniptable.setErrorLine(linerror);
                        }
                    }
                    /******** is it based on ? **************************/
                    string parentatguid = ul.getAtrParentGuid(at);
                    EA.Attribute parentat;
                    if (parentatguid == "")
                    {
                        // ID = "26";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "MissingGUIDBasedOnTag";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = pack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["Attribute"]] = at.Name;
                        maniptable.setErrorLine(linerror);
                        return;
                    }
                    else
                    {
                        parentat = ul.recupIBOAttribute(at);
                    }
                    if (parentat == null)
                    {
                        // ID = "14";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "AttributeInconsistency";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = pack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["Attribute"]] = at.Name;
                        linerror[dicCheckPCols["CIMText"]] = " the no parent attribute, it may have been deleted in the CIM";
                        maniptable.setErrorLine(linerror);
                        ul.wtest("TEST", String.Format(" verif_atrribute return issue  14 "));
                        ul.wlog(" verif_attribute", " interrupt because no parent for" + el.Name + "." + at.Name);
                        return;
                    }
                    if (!el.StereotypeEx.Contains("CIMDatatype"))
                    { 
                        if (at.Notes == "") // the description is empty
                        {

                        }
                        else
                        {
                            if (!parentat.Notes.Equals(at.Notes)) // ABA20230114
                            {
                                string parentnote = parentat.Notes.Replace('\r', ' ');
                                parentnote = parentnote.Replace('\n', ' ');
                                string atnote = at.Notes.Replace('\r', ' ');
                                atnote = atnote.Replace('\n', ' ');
                                // ID = "11";
                                emptyLine(linerror);
                                linerror[dicCheckPCols["Message"]] = "NotSameDescription";
                                linerror[dicCheckPCols["Severity"]] = "Violation";
                                linerror[dicCheckPCols["Package"]] = pack.Name;
                                linerror[dicCheckPCols["Class"]] = el.Name;
                                linerror[dicCheckPCols["Attribute"]] = at.Name;
                                linerror[dicCheckPCols["CIMText"]] = parentnote; ;
                                linerror[dicCheckPCols["ProfileText"]] = at.Notes;
                                maniptable.setErrorLine(linerror);
                            }

                        }
                    }
                    /***********  check the name ******************/
                    if (parentat.Name != at.Name)
                    {
                        // ID = "25";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "NotSameName";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = pack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["Attribute"]] = at.Name;
                        linerror[dicCheckPCols["CIMText"]] = " the attribute name have changed, it may have evolved in the CIM";
                        maniptable.setErrorLine(linerror);
                    }

            /****************** check the stereotypes  *************************/
                    if (!isSimilarStereo(parentat.StereotypeEx,at.StereotypeEx))
                    {
                       // ID = "15";
                       emptyLine(linerror);
                       linerror[dicCheckPCols["Message"]] = "NotSameStereotypes";
                       linerror[dicCheckPCols["Severity"]] = "Warning";
                       linerror[dicCheckPCols["Package"]] = pack.Name;
                       linerror[dicCheckPCols["Class"]] = el.Name;
                       linerror[dicCheckPCols["Attribute"]] = at.Name;
                       linerror[dicCheckPCols["CIMText"]] = " Stereotypes in CIM=" + parentat.StereotypeEx;
                       linerror[dicCheckPCols["ProfileText"]] = " Stereotypes in Profile=" + at.StereotypeEx; ;
                       maniptable.setErrorLine(linerror);
                    }

            /************** check the type ***************************************/
                     if (at.ClassifierID == 0)
                     {
                         // ID = "16";
                         emptyLine(linerror);
                         linerror[dicCheckPCols["Message"]] = "NotProperlyTyped";
                         linerror[dicCheckPCols["Severity"]] = "Violation";
                         linerror[dicCheckPCols["Package"]] = pack.Name;
                         linerror[dicCheckPCols["Class"]] = el.Name;
                         linerror[dicCheckPCols["Attribute"]] = at.Name;
                         linerror[dicCheckPCols["CIMText"]] = " classifierID = 0 and in CIM=" + parentat.ClassifierID;
                         linerror[dicCheckPCols["ProfileText"]] = " Stereotypes in Profile=" + at.StereotypeEx; ;
                         maniptable.setErrorLine(linerror);
                     }
                     else
                     {
                        //EA.Element classifier = null;
                         try
                         {
                            classifier = repo.GetElementByID((int)at.ClassifierID);
                            if (classifier==null)
                            {
                                // ID = "16";
                                emptyLine(linerror);
                                linerror[dicCheckPCols["Message"]] = "NotProperlyTyped";
                                linerror[dicCheckPCols["Severity"]] = "Violation";
                                linerror[dicCheckPCols["Package"]] = pack.Name;
                                linerror[dicCheckPCols["Class"]] = el.Name;
                                linerror[dicCheckPCols["Attribute"]] = at.Name;
                                linerror[dicCheckPCols["CIMText"]] = " classifierID = 0 and in CIM=" + parentat.ClassifierID;
                                linerror[dicCheckPCols["ProfileText"]] = " classifier not known test fot this attribuute interrupted" ;
                                maniptable.setErrorLine(linerror);
                                return;
                            }
                            //bool isInProfile = ul.isClassifierInProfile(el, classifier, "entsoe");
                            bool isInProfile = ul.IsInProfile(Main.Repo, el, pack);
                            if (!isInProfile)
                            {
                                // ID = "17";
                                emptyLine(linerror);
                                linerror[dicCheckPCols["Message"]] = "NotInProfileDatatype";
                                linerror[dicCheckPCols["Severity"]] = "Violation";
                                linerror[dicCheckPCols["Package"]] = pack.Name;
                                linerror[dicCheckPCols["Class"]] = el.Name;
                                linerror[dicCheckPCols["Attribute"]] = at.Name;
                                linerror[dicCheckPCols["ProfileText"]] = " the classifier " + classifier.Name + " is not local";
                                maniptable.setErrorLine(linerror);
                            }
                            if(type=="valuedatatype")
                            if (classifier.Stereotype != "Primitive")
                            {
                                    // ID = "18";
                                emptyLine(linerror);
                                linerror[dicCheckPCols["Message"]] = "NotConformedPrimitive";
                                linerror[dicCheckPCols["Severity"]] = "Violation";
                                linerror[dicCheckPCols["Package"]] = pack.Name;
                                linerror[dicCheckPCols["Class"]] = el.Name;
                                linerror[dicCheckPCols["Attribute"]] = at.Name;
                                linerror[dicCheckPCols["ProfileText"]] = " type in Profile=" + classifier.Name;
                                maniptable.setErrorLine(linerror);
                            }

                            if (parentat.Type != at.Type)
                            {
                                // ID = "19";
                                emptyLine(linerror);
                                linerror[dicCheckPCols["Message"]] = "NotSameType";
                                linerror[dicCheckPCols["Severity"]] = "Violation";
                                linerror[dicCheckPCols["Package"]] = pack.Name;
                                linerror[dicCheckPCols["Class"]] = el.Name;
                                linerror[dicCheckPCols["Attribute"]] = at.Name;
                                linerror[dicCheckPCols["CIMText"]] = " type in CIM=" + parentat.Type;
                                maniptable.setErrorLine(linerror);
                            }                  
                         }
                         catch (Exception e)
                         {
                              ul.wlog("CheckFile", " ISSUE in accessing classifier " + classifier.Name + " for " + el.Name +"." + at.Name + " " +e.Message);                 
                         }
                     }

            
                     /*************************** Test sur const and static ***********************************/
                     if (
                        ((at.IsConst == true) && (parentat.IsConst == false))
                        ||
                        ((at.IsStatic == true) && (parentat.IsStatic == false))
                        )
                    {
                        // ID = "20";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "NotConsistantFeature";
                        linerror[dicCheckPCols["Severity"]] = "Info";
                        linerror[dicCheckPCols["Package"]] = pack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["Attribute"]] = at.Name;
                        linerror[dicCheckPCols["CIMText"]] = " for CIM isConst=" + parentat.IsConst.ToString() + " isStatic=" + parentat.IsStatic.ToString();
                        linerror[dicCheckPCols["ProfileText"]] = " for profile  isConst=" + at.IsConst.ToString() + " isStatic=" + at.IsStatic.ToString();
                        maniptable.setErrorLine(linerror);
                    }
                     if(
                         ((at.IsConst==false) &&  (parentat.IsConst==true))
                         ||
                         ((at.IsStatic == false) && (parentat.IsStatic == true))
                         )
                     {
                         // ID = "20";
                         emptyLine(linerror);
                         linerror[dicCheckPCols["Message"]] = "NotConsistantFeature";
                         linerror[dicCheckPCols["Severity"]] = "Violation";
                         linerror[dicCheckPCols["Package"]] = pack.Name;
                         linerror[dicCheckPCols["Class"]] = el.Name;
                         linerror[dicCheckPCols["Attribute"]] = at.Name;
                         linerror[dicCheckPCols["CIMText"]] = " for CIM isConst=" + parentat.IsConst.ToString() + " isStatic=" + parentat.IsStatic.ToString();
                         linerror[dicCheckPCols["ProfileText"]] = " for profile  isConst=" + at.IsConst.ToString() + " isStatic=" + at.IsStatic.ToString();
                         maniptable.setErrorLine(linerror);
                     }
                    /**************** is there a default value different from parent or  ****************/
                    if (at.Default == null) at.Default = "";
                    if (parentat.Default == null) parentat.Default = "";
                     if (
                         (at.Default != "")
                            ||
                         (parentat.Default != at.Default)
                        )
                     {
                        // if the element is an enumeration the initial values must be compliant
                     EA.Element parentattype = repo.GetElementByID((int)parentat.ClassifierID);
                        //Dictionary<string, string> dicEnumValues = new Dictionary<string, string>(); // dictionnaire member/value
                        if (// the local tyoe is an enumeration?
                            (classifier != null)
                            &&
                            (
                             classifier.StereotypeEx.Contains(CD.GetEnumStereotype())
                            || (classifier.MetaType == "Enumeration")
                            || (classifier.Type == "Enumeration")
                            ))
                        { // case on an enumeration classifier
                          // EA.Element attype = repo.GetElementByID((int)at.ClassifierID);
                            if(!ProfEnumValuesdic.ContainsKey(classifier.Name))
                                {
                                ProfEnumValuesdic.Add(classifier.Name, new Dictionary<string, string>());
                                foreach (EA.Attribute member in classifier.AttributesEx)
                                {
                                    ProfEnumValuesdic[classifier.Name].Add(member.Name, member.Default);
                                }
                            }
                            
                            if (// the parenttype is an enumeration we must check the values 
                                // we must inform in case some values are different from the parent
                            (parentattype.StereotypeEx.Contains(CD.GetEnumStereotype()))
                            || (parentattype.MetaType == "Enumeration")
                            || (parentattype.Type == "Enumeration")
                                )
                            {
                                if (!EnumValuesdic.ContainsKey(classifier.Name))
                                {
                                    EnumValuesdic.Add(classifier.Name, new Dictionary<string, string>());
                                    foreach (EA.Attribute member in parentattype.AttributesEx)
                                    {
                                        EnumValuesdic[classifier.Name].Add(member.Name, member.Default);
                                    }
                                }
                            }

                          
                            if (
                                (at.Default != "")
                                &&
                                (
                                    !ProfEnumValuesdic[classifier.Name].ContainsKey(at.Default))
                                )
                            {
                                // ID = "22";
                                emptyLine(linerror);
                                linerror[dicCheckPCols["Message"]] = "NotConsistantEnumerationInitialValues";
                                linerror[dicCheckPCols["Severity"]] = "Violation";
                                linerror[dicCheckPCols["Package"]] = pack.Name;
                                linerror[dicCheckPCols["Class"]] = el.Name;
                                linerror[dicCheckPCols["Attribute"]] = at.Name;
                                linerror[dicCheckPCols["ProfileText"]] = " for profile" + at.Name + "  initial value=" + at.Default;
                                maniptable.setErrorLine(linerror);
                            }
                        
                        // parent default is different
                            if (at.Default != parentat.Default)
                            {
                                // ID = "23";
                                emptyLine(linerror);
                                linerror[dicCheckPCols["Message"]] = "NotSameInitialValue";
                                linerror[dicCheckPCols["Severity"]] = "Warning";
                                linerror[dicCheckPCols["Package"]] = pack.Name;
                                linerror[dicCheckPCols["Class"]] = el.Name;
                                linerror[dicCheckPCols["Attribute"]] = at.Name;
                                linerror[dicCheckPCols["CIMText"]] = " for CIM initial value=" + parentat.Default;
                                linerror[dicCheckPCols["ProfileText"]] = " for profile  initial value=" + at.Default;
                                maniptable.setErrorLine(linerror);
                            }
                    }
            }


            /***************   verif  cardinality ******************************************/
            if (
                  (at.LowerBound != parentat.LowerBound)
                || (parentat.UpperBound != at.UpperBound)
                )
            {
                // ID = "24";
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "NotSameCardinality";
                linerror[dicCheckPCols["Severity"]] = "Warning";
                linerror[dicCheckPCols["Package"]] = pack.Name;
                linerror[dicCheckPCols["Class"]] = el.Name;
                linerror[dicCheckPCols["Attribute"]] = at.Name;
                linerror[dicCheckPCols["CIMText"]] = " for CIM attribute LowerBound=" + parentat.LowerBound + "," + "LowerBound=" + parentat.UpperBound;
                linerror[dicCheckPCols["ProfileText"]] = " for Profile LowerBound=" + at.LowerBound + "," + "LowerBound=" + at.UpperBound;
                maniptable.setErrorLine(linerror);
            }

                    break;
                case "member":  // attribute for enumeration
                    /******** is it based on ? **************************/
                     parentatguid = ul.getAtrParentGuid(at);
                    if(parentatguid=="")
                    {
                        // ID = "26";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "MissingGUIDBasedOnTag";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = pack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["Attribute"]] = at.Name;
                        maniptable.setErrorLine(linerror);
                        return;
                    }
                     parentat = ul.recupIBOAttribute(at);

                    if (parentat == null)

                    {
                        // ID = "27";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "ElementInconsistancy";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = pack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["Attribute"]] = at.Name;
                        linerror[dicCheckPCols["CIMText"]] = " the member may have been deleted in the CIM";
                        maniptable.setErrorLine(linerror);
                        ul.wtest("TEST", String.Format("verif_attribute return issue 27 continue to next member"));
                        return;
                    }

                    /***********  check the name ******************/
                    if (parentat.Name != at.Name)
                    {
                        // ID = "25";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "NotSameName";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = pack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["Attribute"]] = at.Name;
                        linerror[dicCheckPCols["CIMText"]] = " the member may have evolved in the CIM";
                        maniptable.setErrorLine(linerror);
                    }

                    /****************** check the stereotypes  *************************/
                    if (!parentat.StereotypeEx.Equals(at.StereotypeEx))
                    {
                        // ID = "15";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "NotSameStereotypes";
                        linerror[dicCheckPCols["Severity"]] = "Warning";
                        linerror[dicCheckPCols["Package"]] = pack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["Attribute"]] = at.Name;
                        linerror[dicCheckPCols["CIMText"]] = " Stereotypes in CIM=" + parentat.StereotypeEx;
                        linerror[dicCheckPCols["ProfileText"]] = " Stereotypes in Profile=" + at.StereotypeEx; ;
                        maniptable.setErrorLine(linerror);
                    }

                    /****************  default value is the same ****************/
                    if (at.Default == null) at.Default = "";
                    if (parentat.Default == null) parentat.Default = "";
                    if (at.Default != parentat.Default)
                    {
                        // ID = "23";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "NotSameInitialValue";
                        linerror[dicCheckPCols["Severity"]] = "Warning";
                        linerror[dicCheckPCols["Package"]] = pack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["Attribute"]] = at.Name;
                        linerror[dicCheckPCols["CIMText"]] = " for CIM initial value=" + parentat.Default;
                        linerror[dicCheckPCols["ProfileText"]] = " for profile  initial value=" + at.Default;
                        maniptable.setErrorLine(linerror);
                    }
                    break;
                default:
                    break;
        }
        }

        /// <summary>

        /// <summary>
        /// check if class has a GUID tag value and that it corresponds to a class 
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        /// <param name="el"></param>
        EA.Element verif_ClassIBOIntegrity(EA.Package thepack,EA.Element el)
        {
            if (el.Name == "BasePower")
            {
                string prov = el.StereotypeEx;
            }
            string parentguid = ul.getEltParentGuid(el);
            EA.Element parentel = null;  // the IBO element
          
            if (parentguid == "")
            {
                // ID = "26";
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "MissingGUIDBasedOnTag";
                linerror[dicCheckPCols["Severity"]] = "Violation";
                linerror[dicCheckPCols["Package"]] = thepack.Name;
                linerror[dicCheckPCols["Class"]] = el.Name;
                maniptable.setErrorLine(linerror);
            }
            else
            {
                try
                {
                    try
                    {
                        parentel = repo.GetElementByGuid(parentguid);
                        if(parentel==null)
                        {
                            // ID = "27";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "ElementInconsistancy";
                            linerror[dicCheckPCols["Severity"]] = "Violation";
                            linerror[dicCheckPCols["Package"]] = thepack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            linerror[dicCheckPCols["CIMText"]] = " CIM with guid not found";
                            maniptable.setErrorLine(linerror);
                            ul.wtest("TEST", String.Format("verif_class issue 45 el={0}", el.Name));
                            return parentel = null;
                        }
                    }
                    catch (Exception e)
                    {
                        // ID = "45";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "NotAccessibleElement";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = thepack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        linerror[dicCheckPCols["CIMText"]] = e.Message;
                        maniptable.setErrorLine(linerror);
                        ul.wtest("TEST", String.Format("verif_class issue 45 el={0}", el.Name));
                        return parentel = null;

                    }
                    
                    long dependid = ul.getIBOElementIDFromDependency(el);
                    //check if the parentelement found is basedon dependent with el
                    if (dependid == 0)
                    {
                        // ID = "28";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "NotIsBasedOnLink";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = thepack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        maniptable.setErrorLine(linerror);
                    }
                    else
                    {
                        if (dependid != parentel.ElementID)
                        {
                            // ID = "29";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "NotValidIsBasedOnLink";
                            linerror[dicCheckPCols["Severity"]] = "Violation";
                            linerror[dicCheckPCols["Package"]] = thepack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            linerror[dicCheckPCols["CIMText"]] = " IBO class different from Dependency supplier";
                            maniptable.setErrorLine(linerror);
                            ul.wtest("TEST", String.Format("verif_cClassIBOIntegrty issue 29 el={0}", el.Name));
                            return null;
                        }
                    }

                    if (el.StereotypeEx.Contains(CD.GetDatatypeStereotype()))
                    {
                        // Cimdatatypes should  be basedon primitive classes or another CimDataType class
                        if ((!parentel.StereotypeEx.Contains(CD.GetPrimitiveStereotype()))
                            && (!parentel.StereotypeEx.Contains(CD.GetDatatypeStereotype()))) 
                        
                        {
                            // ID = "46";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "NotProperlyBasedOn";
                            linerror[dicCheckPCols["Package"]] = thepack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            linerror[dicCheckPCols["CIMText"]] = " should be BasedOn with primitive or dataMessagee";
                            maniptable.setErrorLine(linerror);
                            ul.wtest("TEST", String.Format("verif_class issue 46 el={0}", el.Name));
                            return null;
                        }
                       
                    }
                        if (ul.RemoveQual(el.Name) != ul.RemoveQual(parentel.Name))
                        {
                            // ID = "25";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "NotSameName";
                            linerror[dicCheckPCols["Severity"]] = "Violation";
                            linerror[dicCheckPCols["Package"]] = thepack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            linerror[dicCheckPCols["CIMText"]] = " Name of IBOClass="+parentel.Name;
                            maniptable.setErrorLine(linerror);
                         
                        }
                        if(!isSimilarStereo(parentel.StereotypeEx,el.StereotypeEx))
                        {
                            // ID = "15";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "NotSameStereotypes";
                            linerror[dicCheckPCols["Severity"]] = "Warning";
                            linerror[dicCheckPCols["Package"]] = thepack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            linerror[dicCheckPCols["CIMText"]] = " CIMstereotypes=" +parentel.StereotypeEx;
                            linerror[dicCheckPCols["ProfileText"]] = "Profstereotypes=" + el.StereotypeEx;
                            maniptable.setErrorLine(linerror);
                            ul.wtest("TEST", String.Format("verif_class return issue 15 el={0} parentel={1}", el.Name, parentel.Name));
                    }
                    

                }
                catch (Exception e)
                {
                    MessageBox.Show(" validation issue: get IBO element forP" + thepack.Name + "::" + el.Name + " " + e.Message);
                    
                    parentel = null;
                }

            }
            
            return parentel;

        }
        /// <summary>
        /// check validity of members order of a class
        /// </summary>
        /// <param name="el"></param>
        void verif_rangmembres(EA.Element el)
        {
            if (!isESMP) return;
            List<int> rangs = new List<int>();
            foreach (EA.Attribute at in el.Attributes)
            {
                /*********  am nov 2015
               foreach (EA.AttributeTag tag in at.TaggedValues)
               {
                   
                   if (tag.Name == CD.GetRangTagValue())
                   {
                       rg = System.Convert.ToInt32(tag.Value);
                       break;
                   }
                     
               }
               if (rg == -1)
               {
                   ul.wlog("CheckProfile", " WARNING: the order tagvalue is missing for  " + el.Name + "." + at.Name);

               }
               else
               {
                   if (!rangs.Contains(rg))
                   {
                       rangs.Add(rg);
                   }
                   else
                   {
                       ul.wlog("CheckProfile", "ISSUE: the order" + rg + " already exits  for  " + el.Name + "." + at.Name);
                       errors++;
                   }
               }
                    * */
            }
            foreach (EA.Connector con in el.Connectors)
            {
                if (
                    ((con.Type == "Association") || (con.Type == "Aggregation"))
                    &&
                    (((con.ClientID == el.ElementID) && (con.SupplierEnd.Role != "")
                    && ((con.ClientEnd.Aggregation == 1) || (con.SupplierEnd.IsNavigable))) // am sept 201                       ||
                    ||
                    ((con.SupplierID == el.ElementID) && (con.ClientEnd.Role != "")
                     && ((con.SupplierEnd.Aggregation == 1) || (con.ClientEnd.IsNavigable))) // am sept 2013
                    )
                    )
                {
                    /*
                    rg = -1;
                    foreach (EA.ConnectorTag tag in con.TaggedValues)
                    {
                        if (tag.Name == CD.GetRangTagValue())
                        {
                            rg = System.Convert.ToInt32(tag.Value);
                            break;
                          
                        }
                    }
                    if (rg == -1)
                    {
                        ul.wlog("CheckProfile", " WARNING: the order tagvalue is missing for  "
                            + el.Name + "con ("+ con.ClientEnd.Role + " -> " + con.SupplierEnd.Role +" )" );
                    }
                    else
                    {
                        if (!rangs.Contains(rg))
                        {
                            rangs.Add(rg);
                        }
                        else
                        {
                            ul.wlog("CheckProfile", "ISSUE: the order" + rg + " already exits  for  " + el.Name + "con (" + con.ClientEnd.Role + " -> " + con.SupplierEnd.Role + " )");
                            errors++;
                        }
                    }
                    */
                }
                
            }
        }
        //----------------------------------------------
         ///<summary>
        /// this program check every association of a profile
        /// an association:
        /// must be based on an association
        /// each ends of it must be based on each resppecive end
        /// if there is a role without label the oposite label must be non empty 
        ///    and derived , possibly by a qualifier , from the isbased on role
        ///    more generally a ole is always derived from the original role
        ///    there must be either a sign of aggregation or an arrow
        ///    on the side opposite the aggregation the cardinality must be within the original limit
         /// </summary>
        /// <param name="repos"></param>
        /// <param name="thepack"></param>
        /// <param name="el"></param>
        /// <param name="con"></param>
        void 
            verif_association( EA.Package thepack, EA.Element el, EA.Connector con)
        {
            ul.wtest("TEST", "verif_association  thepack=" + thepack.Name + " el=" + el.Name + " client=" + con.ClientEnd.Role + " supplier=" + con.SupplierEnd.Role);
            if (el.Name == "DiagramObjectPoint")
            {
                string sourcename = con.ClientEnd.Role;
            }
            if (conAlreadyTreated.Contains(con.ConnectorID))
            {
                ul.wtest("TEST", String.Format("verif_association return alreadydone el={0}", el.Name));
                return; // already done
            }
            else
            {
                conAlreadyTreated.Add(con.ConnectorID); // it is now treated
            }
            if(con.Name!="")
            {
                // ID = "35";
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "AssociationNameNotEmpty";
                linerror[dicCheckPCols["Severity"]] = "Violation";
                linerror[dicCheckPCols["Package"]] = thepack.Name;
                linerror[dicCheckPCols["Class"]] = el.Name;
                linerror[dicCheckPCols["Association"]] = String.Format( "  {0} -->  {1}",con.ClientEnd.Role,con.SupplierEnd.Role);
                linerror[dicCheckPCols["ProfileText"]] = "assocations are anonymous";
                maniptable.setErrorLine(linerror);
            }
            if (con.Notes == "") // the description is empty
            {
                /*
                ID = "36";
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "HasEmptyNote";
                linerror[dicCheckPCols["Severity"]] = "Violation";
                linerror[dicCheckPCols["Package"]] = thepack.Name;
                linerror[dicCheckPCols["Class"]] = el.Name;
                linerror[dicCheckPCols["Association"]] = String.Format("  {0} -->  {1}", con.ClientEnd.Role, con.SupplierEnd.Role);
                linerror[dicCheckPCols["ProfileText"]] = " description in an association  is empty";
                maniptable.setErrorLine(linerror);
                */
            }
            // if there are constraints OCL they must appear
            foreach (EA.ConnectorConstraint cst in con.Constraints)
            {
                if (cst.Type == "OCL")
                {
                    // ID = "5";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "HasOCLConstraint";
                    linerror[dicCheckPCols["Severity"]] = "Info";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    linerror[dicCheckPCols["Association"]] = String.Format("  {0} -->  {1}", con.ClientEnd.Role, con.SupplierEnd.Role);
                    linerror[dicCheckPCols["ProfileText"]] = "Association Constraint for  of Type" + cst.Type + "=" + cst.Name;
                    maniptable.setErrorLine(linerror);
                }
            }
            try
            {
                string parentguid = ul.getConParentGuid(con);
                EA.Connector parentcon = null;
                if (parentguid == "")
                {

                }
                else
                {
                    parentcon = ul.recupIBOConnector(el, con);

                    if (parentcon == null) throw new Exception("no accessible parent (it may have been deleted in cim");
                }



                string sens = "";
                EA.Element source = repo.GetElementByID((int)con.ClientID);
                string sourcerole = con.ClientEnd.Role;
                EA.ConnectorEnd sourceend = con.ClientEnd;
                EA.Element target = repo.GetElementByID((int)con.SupplierID);
                string targetrole = con.SupplierEnd.Role;
                EA.ConnectorEnd targetend = con.SupplierEnd;
                if (con.ClientID == el.ElementID)
                {
                    sens = "source";
                }
                else
                {
                    sens = "target";
                }
                if (parentguid == "")
                {
                    // ID = "26";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "MissingGUIDBasedOnTag";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;


                    if (sens == "source")
                    {
                        linerror[dicCheckPCols["Association"]] = sourcerole + " -> " + targetrole;
                    }
                    else
                    {
                        linerror[dicCheckPCols["Association"]] = targetrole + " -> " + sourcerole;
                    }
                    maniptable.setErrorLine(linerror);
                    ul.wtest("TEST", String.Format("verif_association issue 26 el={0}", el.Name));
                    return;
                }
                if (!isSimilarStereo(parentcon.StereotypeEx, con.StereotypeEx))
                {
                    // ID = "15";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "NotSameStereotypes";
                    linerror[dicCheckPCols["Severity"]] = "Warning";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    linerror[dicCheckPCols["Association"]] = String.Format("  {0} -->  {1}", con.ClientEnd.Role, con.SupplierEnd.Role);
                    linerror[dicCheckPCols["CIMText"]] = " Stereotypes in CIM=" + parentcon.StereotypeEx;
                    linerror[dicCheckPCols["ProfileText"]] = " Stereotypes in one association";
                    maniptable.setErrorLine(linerror);
                }
                // similarity of notes
                if (!con.Notes.Equals(parentcon.Notes))
                {
                    string parentnote = parentcon.Notes.Replace('\r', ' ');
                    parentnote = parentnote.Replace('\n', ' ');
                    string elnote = el.Notes.Replace('\r', ' ');
                    elnote = elnote.Replace('\n', ' ');
                    // ID = "11";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "NotSameDescription";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    linerror[dicCheckPCols["Association"]] = String.Format("  {0} -->  {1}", con.ClientEnd.Role, con.SupplierEnd.Role);
                    linerror[dicCheckPCols["CIMText"]] = parentnote; ;
                    linerror[dicCheckPCols["ProfileText"]] = con.Notes;
                    maniptable.setErrorLine(linerror);
                }

                EA.Element parentsource = repo.GetElementByID((int)parentcon.ClientID);
                string parentsourcerole = parentcon.ClientEnd.Role;
                EA.Element parenttarget = repo.GetElementByID((int)parentcon.SupplierID);
                string parenttargetrole = parentcon.SupplierEnd.Role;
                string dsguid = ul.getEltParentGuid(source);
                string tgguid = ul.getEltParentGuid(target);
                EA.Element desource = null;
                EA.Element detarget = null;
                if ((dsguid == "") || (tgguid == ""))
                {
                    // ID = "26";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "MissingGUIDBasedOnTag";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    linerror[dicCheckPCols["Association"]] = String.Format("  {0} -->  {1}", con.ClientEnd.Role, con.SupplierEnd.Role);
                    linerror[dicCheckPCols["ProfileText"]] = " One of the parent of an end of the association has no guid";
                    maniptable.setErrorLine(linerror);
                }
                else
                {
                    desource = ul.recupIBOElement(source);

                    detarget = ul.recupIBOElement(target);
                }



                EA.ConnectorEnd parentsourceend = null;
                EA.ConnectorEnd parenttargetend = null;

                // each parent ends must be in hierarchy with the other
                if (ul.isInRelation(desource, parentsource))
                // this means that parentsource and desource are related 
                // detarget and  parenttarget must be also in relation
                {
                    if (ul.isInRelation(detarget, parenttarget))
                    {
                        parentsourceend = parentcon.ClientEnd;
                        parenttargetend = parentcon.SupplierEnd;
                    }
                    else
                    {
                        // ID = "30";
                        emptyLine(linerror);
                        linerror[dicCheckPCols["Message"]] = "AssociationEndsInconsistency";
                        linerror[dicCheckPCols["Severity"]] = "Violation";
                        linerror[dicCheckPCols["Package"]] = thepack.Name;
                        linerror[dicCheckPCols["Class"]] = el.Name;
                        if (sens == "source")
                        {
                            linerror[dicCheckPCols["Association"]] = sourcerole + " -> " + targetrole;
                        }
                        else
                        {
                            linerror[dicCheckPCols["Association"]] = targetrole + " -> " + sourcerole;
                        }
                        maniptable.setErrorLine(linerror);
                        ul.wtest("TEST", String.Format("verif_association issue 30 el={0}", el.Name));
                        return;
                    }
                }
                else
                {
                    if (ul.isInRelation(desource, parenttarget))
                    // this means that parenttarget and desource are related 
                    // detarget and  parentsource must be also in relation
                    {
                        if (ul.isInRelation(detarget, parentsource))
                        {
                            //desourcerole=parenttargetrole;
                            // detargetrole=parentsourcerole;
                            parentsourceend = parentcon.SupplierEnd;
                            parenttargetend = parentcon.ClientEnd;
                        }
                        else
                        {
                            // ID = "30";
                            emptyLine(linerror);
                            linerror[dicCheckPCols["Message"]] = "AssociationEndsInconsistency";
                            linerror[dicCheckPCols["Severity"]] = "Violation";
                            linerror[dicCheckPCols["Package"]] = thepack.Name;
                            linerror[dicCheckPCols["Class"]] = el.Name;
                            if (sens == "source")
                            {
                                linerror[dicCheckPCols["Association"]] = sourcerole + " -> " + targetrole;
                            }
                            else
                            {
                                linerror[dicCheckPCols["Association"]] = targetrole + " -> " + sourcerole;
                            }
                            maniptable.setErrorLine(linerror);
                            ul.wtest("TEST", String.Format("verif_association issue 30 el={0}", el.Name));
                            return;

                        }
                    }
                }

                // at this stage we have proved that the profile and parents classes are related ,
                //   now we must prove that the roles are correctly derived

                /************* test of the role consistency   *******************************************/
                if (parenttarget.ElementID == parentsource.ElementID)
                {
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = CD.CPINFO;
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    if (sens == "source")
                    {
                        linerror[dicCheckPCols["Association"]] = sourcerole + " -> " + targetrole;
                    }
                    else
                    {
                        linerror[dicCheckPCols["Association"]] = targetrole + " -> " + sourcerole;
                    }
                    linerror[dicCheckPCols["ProfileText"]] = " the recursive association is not checked by this program";
                    maniptable.setErrorLine(linerror);
                    ul.wtest("TEST", String.Format("verif_association info el=", el.Name));
                    return;// the recursive associations must be excluded from the following  tests
                }
                if (
                        (ul.RemoveDQual(sourcerole) != (ul.RemoveDQual(parentsourceend.Role)))
                            ||
                        (ul.RemoveDQual(targetrole) != (ul.RemoveDQual(parenttargetend.Role)))
                        )
                {
                    // ID = "31";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "NotSameEndRoleName";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    if (sens == "source")
                    {
                        linerror[dicCheckPCols["Association"]] = sourcerole + " -> " + targetrole;
                        linerror[dicCheckPCols["CIMText"]] = " for CIM   " + parentsourceend.Role + " -> " + parenttargetend.Role;
                    }
                    else
                    {
                        linerror[dicCheckPCols["Association"]] = targetrole + " -> " + sourcerole;
                        linerror[dicCheckPCols["CIMText"]] = " for CIM  =" + parenttargetend.Role + " -> " + parentsourceend.Role;
                    }
                    maniptable.setErrorLine(linerror);
                    ul.wtest("TEST", String.Format("verif_association issue 31 el={0}", el.Name));
                    //return;
                }
                /**************************** notes of roles    ****************************************************************/
                if (
                           (sourceend.RoleNote != parentsourceend.RoleNote)
                           ||
                           (targetend.RoleNote != parenttargetend.RoleNote)
                )
                {
                    // ID = "32";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "NotSameEndRoleDescription";
                    linerror[dicCheckPCols["Severity"]] = "Violation";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    if (sens == "source")
                    {
                        linerror[dicCheckPCols["Association"]] = sourcerole + " -> " + targetrole;

                    }
                    else
                    {
                        linerror[dicCheckPCols["Association"]] = targetrole + " -> " + sourcerole;

                    }
                    int lparentmax = 30;
                    int a = parentsourceend.RoleNote.Length;
                    int b = parenttargetend.RoleNote.Length;
                    if (a < lparentmax) lparentmax = a;

                    int c = sourceend.RoleNote.Length;
                    int d = targetend.RoleNote.Length;


                    linerror[dicCheckPCols["CIMText"]] = parentsourceend.Role + ".Note=\"" + parentsourceend.RoleNote.Substring(0, Math.Min(30, a)) + "...\" , " + parenttargetend.Role + ".Note=" + parenttargetend.RoleNote.Substring(0, Math.Min(30, b)) + " ...\"";
                    linerror[dicCheckPCols["ProfileText"]] = sourceend.Role + ".Note=\"" + sourceend.RoleNote.Substring(0, Math.Min(30, c)) + "...\" , " + targetend.Role + ".Note=" + targetend.RoleNote.Substring(0, Math.Min(30, d)) + " ...\"";

                    maniptable.setErrorLine(linerror);
                }


                /**********************   test of cardinality  *****************************************/
                // the cardinality must be within the limites
                if (
                            (sourceend.Cardinality != parentsourceend.Cardinality)
                            ||
                            (targetend.Cardinality != parenttargetend.Cardinality)
                            )
                {
                    // ID = "33";
                    emptyLine(linerror);
                    linerror[dicCheckPCols["Message"]] = "NotSameMultiplicity";
                    linerror[dicCheckPCols["Severity"]] = "Warning";
                    linerror[dicCheckPCols["Package"]] = thepack.Name;
                    linerror[dicCheckPCols["Class"]] = el.Name;
                    if (sens == "source")
                    {
                        linerror[dicCheckPCols["Association"]] = sourcerole + " -> " + targetrole;
                    }
                    else
                    {
                        linerror[dicCheckPCols["Association"]] = targetrole + " -> " + sourcerole;
                    }
                    linerror[dicCheckPCols["CIMText"]] = parentsourceend.Role + "[" + parentsourceend.Cardinality + "] , " + parenttargetend.Role + "[" + parenttargetend.Cardinality + "]";
                    linerror[dicCheckPCols["ProfileText"]] = sourceend.Role + "[" + sourceend.Cardinality + "] ," + targetend.Role + "[" + targetend.Cardinality + "]";


                    maniptable.setErrorLine(linerror);
                }



                /********************* test of direction *********************************************/
                // if the association is orientated form cardinality (n,*) towards 0,1 or 1 it is normal
                // if the association is orientated from cardinatilty 0,1 towards 0,1  it  has to be marked
                // if the association is orientated from cardinality 0,1 ou 1 towards n,* it has to be marked
                // if the association is orientated from cardinality 0,*  towards 0,* it has to be marked

                EA.ConnectorEnd navend = null;
                EA.ConnectorEnd otherend = null;
                if (sourceend.Navigable == "Navigable")
                {
                    navend = sourceend;
                    otherend = targetend;
                    sens = "target";
                }
                else
                {
                    navend = targetend;
                    otherend = sourceend;
                    sens = "source";
                }
                int navupper = 0;
                int otherupper = 0;
                int navlower = 0;
                int otherlower = 0;
                string[] othercard = otherend.Cardinality.Split(new String[] { ".." }, StringSplitOptions.RemoveEmptyEntries);
                otherlower = Int32.Parse(othercard[0]);
                if (othercard.Length == 1)
                {
                    otherupper = 1;
                }
                else
                { 
                     if (othercard[1] != "*")
                     {
                          otherupper = Int32.Parse(othercard[1]);
                     }
                }
                string [] navcard = otherend.Cardinality.Split(new String[] { ".." }, StringSplitOptions.RemoveEmptyEntries);
                if (navcard.Length == 1)
                {
                    navupper = 1;
                }
                else
                {
                    navlower = Int32.Parse(navcard[0]);
                    if (navcard[1] != "*")
                    {
                        navupper = Int32.Parse(navcard[1]);
                    }
                    else
                    {

                    }
                }
                if (
                      (// 1 vers 1
                      ((otherend.Cardinality == "1") || (otherend.Cardinality == "1..1"))
                      &&
                      ((navend.Cardinality == "1") || (navend.Cardinality == "1..1"))
                      )
                      ||

                      (// 0..1 vers 0..1 
                      (otherend.Cardinality == "0..1") 
                      &&
                      (navend.Cardinality == "0..1") 
                      )
                      ||
                      (// 0..* vers 0..* 
                       (otherend.Cardinality == "0..*")
                      &&
                      (navend.Cardinality == "0..*")
                      )
                      ||
                       ( //0..1 or 1 to n..*
                       ( (otherend.Cardinality=="1..1" ) || (otherend.Cardinality=="1"))
                       &&
                      (( navupper >= 1)|| (navcard[1]=="*") || (navend.Cardinality == "0..1"))
                       )

                   )


                {
                    // ID = "34";
                      emptyLine(linerror);
                      linerror[dicCheckPCols["Message"]] = "NotExpectedDirection";
                      linerror[dicCheckPCols["Severity"]] = "Info";
                      linerror[dicCheckPCols["Package"]] = thepack.Name;
                      linerror[dicCheckPCols["Class"]] = el.Name;
                      if (sens == "source")
                      {
                          linerror[dicCheckPCols["Association"]] = sourcerole + " -> " + targetrole;
                      }
                      else
                      {
                          linerror[dicCheckPCols["Association"]] = targetrole + " -> " + sourcerole;
                      }
                      linerror[dicCheckPCols["CIMText"]] = parentsourceend.Role + "[" + parentsourceend.Cardinality + "] ,"  + parenttargetend.Role + "[" + parenttargetend.Cardinality + "]";
                      linerror[dicCheckPCols["ProfileText"]] = sourceend.Role + "[" + sourceend.Cardinality + "] ,"  + targetend.Role + "[" + targetend.Cardinality + "]";                                   
                      maniptable.setErrorLine(linerror); 
                }

            }
            catch (Exception e)
            {
                ul.wlog("CheckFile", " ISSUE 45 in accessing association   in EA Repository " + e.Message);
                // ID = "45";
                emptyLine(linerror);
                linerror[dicCheckPCols["Message"]] = "NotAccessibleElement";
                linerror[dicCheckPCols["Severity"]] = "Violation";
                linerror[dicCheckPCols["Package"]] = thepack.Name;
                linerror[dicCheckPCols["Class"]] = el.Name;
                linerror[dicCheckPCols["Association"]] = String.Format("  {0} -->  {1}", con.ClientEnd.Role, con.SupplierEnd.Role);
                linerror[dicCheckPCols["CIMText"]] = " CIM with guid not found for association " + con.Notes;
                maniptable.setErrorLine(linerror);
                ul.wtest("TEST", String.Format("verif_association issue 45 el={0}", el.Name));
            }
        }


        /// <summary>
        /// return date of profile of an entsoe profile
        /// </summary>
        /// <param name="pf"></param>
        /// <returns> string[0] = date of profile
        ///           string[1] = version of CIM 
        ///           string[2] = "1" si variable ontology presente
        ///           </returns>
        string[]   getEntsoeProfileVersion(EA.Package pf)
        {
            string[] res = { "", "","0" };
            EA.Collection eltco = pf.Elements;
            string name = pf.Name;
            //string nn=name.Replace("Profile","Version");
            foreach (EA.Element el in eltco)
            {
                
                if (el.Name.Contains("Version"))
                {
                    res[0] = ul.getEltAttributeValue(el, XMLP.GetXmlValueProfData("ProfileUMLName"));
                    res[1] = ul.getEltAttributeValue(el, "baseUML");
                    break;
                }
            }
            foreach (EA.Element el in eltco)
            {

                if (el.Name.Contains("Ontology"))
                {
                    res[2] = "1";
                    
                    break;
                }
            }
            return res;
        }
     string[] getCIMVersion(EA.Package cimpa)
     {
         string[] res = { "", "" };
         
                         foreach (EA.Element el in cimpa.Elements)
                         {
                             if(el.Name==cimpa.Name + "CIMVersion")
                             {
                                 res[0] = ul.getEltAttributeValue(el,"version");
                                 res[1] = ul.getEltAttributeValue(el,"date");
                                 return res;
                             }
                         }              
        return res;
     }
        /// <summary>
        /// get the cim packages on which the profile is based on
        /// </summary>
        /// <param name="pf"></param>
        /// <returns></returns>
     List<EA.Package> getCIMPackages(EA.Package pf)
     {
         List<EA.Package> res = new List<EA.Package>();
            
            foreach(EA.Connector con in pf.Connectors)
            {
                EA.Element profel = pf.Element;
                if((con.Type=="Dependency") && (con.ClientID==profel.ElementID))
                {
                     EA.Element cimel = repo.GetElementByID((int)con.SupplierID);
          
                     if (CD.CIMPackageNames.Contains(cimel.Name))
                     {
                         EA.Package topcimpa=repo.GetPackageByID((int)cimel.PackageID);
                         EA.Package cimpa = null;
                         foreach (EA.Package pa in topcimpa.Packages)
                         {
                             if (pa.Name == cimel.Name)
                             {
                                 cimpa = pa;
                                 ul.wlog("CkeckProfile", "info: The profile is based on " + cimpa.Name);
                                 break;
                             }
                         }
                         if (cimpa != null)
                         {
                             if (!res.Contains(cimpa)) res.Add(cimpa);
                         }
                     }
                }
                
            }
         return res;
     }

     
        //--------------------------------------------------------------------
       /// <summary>
       /// is true if the parent stereotypes are equal to the profile stereotype 
       /// and if one of the stereotype is not is the EntsoeList
       /// </summary>
       /// <param name="parentstereoex"></param>
       /// <param name="stereoex"></param>
       /// <returns></returns>
        bool isSimilarStereo(string parentstereoex, string stereoex)
        {
            bool res = true;
            if (parentstereoex!=stereoex) // si la liste des stereotype est differente
            {
                res = false;
               if(stereoex!="")  // alors c'est que des stereotypes ont ete rajoutes au niveau du profil
               {
                    string[] stereos = stereoex.Split(',');
                    string[] parentstereos= parentstereoex.Split(',');
                   bool ok=true;; //on va verifier que tous les stereotypes ajoutes sont conformes
                    for (int i = 0; i < stereos.Length; i++) // pour chacun des stereotype on verifie
                    {
                        for(int j=0; j < parentstereos.Length;j++)
                        {
                            if(stereos[i]==parentstereos[j])
                            {
                                ok=true;                      // s'il est dans la liste des stereoparent ok
                                break;
                            }
                        } 
                        if(!ok)
                        {
                           
                           if (ListStereoNamespace.Contains(stereos[i]))
                             {
                               res=true;                 // sinon si il est pas dans la liste des stereotypes utile alors ok                      
                             }
                           else {
                               res = false; // sinon pas ok confirme , pas la peine de continuer
                               break;
                           }

                        }
                     }
                   
                         
                    }
            }
               
            return res;
        }
        /// <summary>
        /// get domain package
        /// the assumption here is that either 
        /// </summary>
        /// <param name="profilepack"></param>
        /// <returns></returns>
        EA.Package getDomainPackage(EA.Package profilepack)
        {
            EA.Package  res = null;
             domainPackage = XMLP.GetXmlValueProfData("EntsoeDataTypesDomain");
             long envelopID = profilepack.ParentID;
             EA.Package envelop=null;
             try
             {
                 envelop = repo.GetPackageByID((int)envelopID);
                 foreach (EA.Package pack in envelop.Packages)
                 {
                     if (pack.Name == domainPackage)
                         res = pack;
                     break;
                 }
             }
             catch (Exception e)
             {
                 ul.wlog("CheckProfile", " ISSUE  problem in accessing package " + domainPackage + "  " + e.Message);
             }

            return res;
        }
        /// <summary>
        /// get all the packages on which the package
        /// has a "isbasedon" dependency with the input element
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public List<EA.Package> getIBOPackagesfromDependency(EA.Package package)
        {
            List<EA.Package> packlist = new List<EA.Package>();
            try
            {
                foreach (EA.Connector con in package.Element.Connectors)
                {
                    if ((con.Type == "Dependency") && (con.Stereotype == CD.GetIsBasedOnStereotype()))
                    {
                        if (con.ClientID == package.Element.ElementID)
                        {
                            EA.Element elpack = Main.Repo.GetElementByID((int)con.SupplierID);
                            EA.Package pack = Main.Repo.GetPackageByID((int)elpack.PackageID);
                            if (!packlist.Contains(pack)) packlist.Add(pack);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                throw new Exception(" getIBOPackagesfromDependency " + ee.Message);
            }
            return packlist;
        }
        //============================================================================================
    }

}
