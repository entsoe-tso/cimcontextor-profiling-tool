using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using CimContextor.Utilities;
using CimContextor.Integrity_Checking;

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
    public class Utilitaires
    {

        EA.Repository repo = null;
        //static CimContextor.utilitaires.Ta ta = new CimContextor.utilitaires.Ta();
        int Rang = 0;  // for ordering the attributes
        // ABA20230228 public static Utilitaires selfut = new Utilitaires();
        private XMLParser XMLP = null;
        private List<string> Facets = null;
        ConstantDefinition CD = new ConstantDefinition();
        UtilitiesConstantDefinition UCD = new UtilitiesConstantDefinition();
        public Dictionary<string, List<long>> dicProfElemByParentGuid = new Dictionary<string, List<long>>();
        public List<EA.Element> ListElem = new List<EA.Element>();
        List<long> condejatraite = new List<long>();
        public Dictionary<string, List<long>> dicACCguidToABIEcon = new Dictionary<string, List<long>>();//<accvonguid/abieconid>
        public static Dictionary<long, List<long>> dicAncestors = new Dictionary<long, List<long>>();// all ancestors for en elementID
        public static Dictionary<long, List<long>> dicDescendants = new Dictionary<long, List<long>>();// all descendants for an element ID
        public static Dictionary<string, EA.Package> dicSelectedPackage = null;// is used by DialTree to select packages
        Dictionary<string, EA.Package> dicCimPacksByName = new Dictionary<string, EA.Package>();

        public Utilitaires(EA.Repository repos) //constructor
        {
            this.repo = repos;
            XMLP = new XMLParser(repos);
            Facets = XMLP.GetXmlClassifierConstraintNames();
            //this.ta = new Ta();;
            //if (!CimContextor.utilitaires.Utilitaires.ta.isRunning()) CimContextor.utilitaires.Utilitaires.ta.start("utilitaires-r");

        }

        /* ABA20230228
        public Utilitaires() //constructor
        {
            this.repo = null;
            // this.ta = new Ta();
            // if (!CimContextor.utilitaires.Utilitaires.ta.isRunning()) CimContextor.utilitaires.Utilitaires.ta.start("utilitaires");
        }
        */

        public void setRepo(EA.Repository repos)
        {
            this.repo = repos;
        }

        //-----------------

        /// <summary>
        /// give the profile envelop and all possible packages of a given element
        /// 2 elements of individually IsBasedOn package cannot be associated
        /// whereas 2 elements of a same envelop package may be associated even in different packages
        /// </summary>
        /// <param name="packageID" the ID of a package></param>
        /// <returns>List of PackageIDs</returns>

        public List<long> GetNotIBOPackages(long packageID, List<long> listpackids)
        {
            EA.Package thepackage = repo.GetPackageByID((int)packageID);
            string prov = thepackage.Name;
            foreach (EA.Package pa in thepackage.Packages)
            {
                if (!HasIBOPackage(pa.PackageID))
                {
                    if (!listpackids.Contains(pa.PackageID)) listpackids.Add(pa.PackageID);
                    listpackids = GetNotIBOPackages(pa.PackageID, listpackids);
                }
            }
            return listpackids;
        }
        /// <summary>
        /// give thelist of all packages containing the package up to the Model
        /// 
        /// <param> name="package" the ID of a package</param>
        /// <returns>List of Packages</returns>

        public void GetAllContainmentPackages(EA.Package package, Dictionary<string, EA.Package> dicpacks)
        {
            if (!dicpacks.ContainsKey(package.Name)) dicpacks[package.Name] = package;
            string prov = package.Name;
            if (package.ParentID != 0)
            {
                GetAllContainmentPackages(repo.GetPackageByID((int)package.ParentID), dicpacks);
            }
        }

        /// <summary>
        /// create the isBasedOn dependency with the CIMPackage for a profile package given 
        /// the package and the package on which is based on
        /// </summary>
        /// <param name="thepackage"></param>
        public void crePackageIBODependency(EA.Package thepackage, EA.Package cimPackage)
        {
            try
            {
                Dictionary<string, EA.Package> dicpacks = new Dictionary<string, EA.Package>();
                GetAllContainmentPackages(cimPackage, dicpacks);
                EA.Package maincimpackage = null;

                bool ok = false;

                foreach (string pname in dicpacks.Keys)
                {
                    if (
                    pname.Contains("TC57CIM")
                    )
                    {
                        maincimpackage = dicpacks[pname];
                        ok = true;
                        break;
                    }
                }

                if (!ok) throw new Exception(" selected package is not in a cim package or in 351");
                EA.Element theElement = thepackage.Element;
                EA.Element cimElement = null;
                foreach (EA.Package cimpackage in maincimpackage.Packages)
                {
                    if (cimpackage.Name.Contains("Dependencies")) continue;
                    cimElement = cimpackage.Element;
                    ok = false;
                    foreach (EA.Connector co in theElement.Connectors)
                    {
                        if (
                            (co.Type == "Dependency")
                            &&
                            (cimElement.ElementID == co.SupplierID)
                            &&
                            (co.Stereotype == CD.GetIsBasedOnStereotype())
                            )
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (ok) continue;
                    EA.Connector con = (EA.Connector)thepackage.Connectors.AddNew("", "Dependency");
                    con.ClientID = theElement.ElementID;
                    con.SupplierID = cimElement.ElementID;
                    thepackage.Connectors.Refresh();
                    con.Stereotype = CD.GetIsBasedOnStereotype();
                    con.Update();
                }
            }
            catch (Exception e)
            {
                wlog("crePackageIBODependency", " PB " + e.Message);
            }

        }
        /// <summary>
        /// create the isBasedOn dependency with the CIMPackage for a profile package given 
        /// we do not know which cimpackage will be considered so we take all the top level
        /// packages as base
        /// the package and the package on which is based on
        /// </summary>
        /// <param name="thepackage"></param>
        public void creGlobalPackageIBODependency(EA.Package thepackage, EA.Package cimPackage)
        {
            try
            {
                Dictionary<string, EA.Package> dicpacks = new Dictionary<string, EA.Package>();
                GetAllContainmentPackages(cimPackage, dicpacks);
                EA.Package maincimpackage = null;

                bool ok = false;

                foreach (string pname in dicpacks.Keys)
                {
                    if (
                    pname.Contains("T57CIM")
                    )
                    {
                        maincimpackage = dicpacks[pname];
                        ok = true;
                        break;
                    }
                }

                if (!ok) throw new Exception(" selected package is not in a cim package or in 351");
                EA.Element theElement = thepackage.Element;
                EA.Element cimElement = maincimpackage.Element;

                foreach (EA.Package cimpack in maincimpackage.Packages)
                {
                    if (cimpack.Name == "PackageDependencies") continue;
                    cimElement = cimpack.Element;
                    ok = false;
                    foreach (EA.Connector co in theElement.Connectors)
                    {
                        if (
                            (co.Type == "Dependency")
                            &&
                            (cimElement.ElementID == co.SupplierID)
                            &&
                            (co.Stereotype == CD.GetIsBasedOnStereotype())
                            )
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (ok) continue; ;
                    EA.Connector con = (EA.Connector)thepackage.Connectors.AddNew("", "Dependency");
                    con.ClientID = theElement.ElementID;
                    con.SupplierID = cimElement.ElementID;
                    thepackage.Connectors.Refresh();
                    con.Stereotype = CD.GetIsBasedOnStereotype();
                    con.Update();
                }

            }
            catch (Exception e)
            {
                wlog("crePackageIBODependency", " PB " + e.Message);
            }

        }
        /// <summary>
        /// create the isBasedOn dependency with the CIMPackage for a profile package given 
        /// the package and the package on which is based on
        /// </summary>
        /// <param name="thepackage"></param>
        public void crePackageIBODependencyold(EA.Package thepackage, EA.Package cimPackage)
        {
            try
            {
                Dictionary<string, EA.Package> dicpacks = new Dictionary<string, EA.Package>();
                GetAllContainmentPackages(cimPackage, dicpacks);
                bool ok = false;

                foreach (string pname in dicpacks.Keys)
                {
                    if (
                    pname.Contains("IEC61970")
                    ||
                    pname.Contains("IEC61968")
                    ||
                    pname.Contains("IEC62325")
                    ||
                    pname.Contains("IEC62325-351")
                    )
                    {
                        ok = true;
                        break;
                    }
                }

                if (!ok) throw new Exception(" selected package is not in a cim package or in 351");


                EA.Element theElement = thepackage.Element;
                EA.Element cimElement = cimPackage.Element;
                EA.Connector con = (EA.Connector)thepackage.Connectors.AddNew("", "Dependency");
                con.ClientID = theElement.ElementID;
                con.SupplierID = cimElement.ElementID;
                thepackage.Connectors.Refresh();
                con.Stereotype = CD.GetIsBasedOnStereotype();
                con.Update();
            }
            catch (Exception e)
            {
                wlog("crePackageIBODependency", " PB " + e.Message);
            }

        }
        /// <summary>
        /// get the packageid of the envelopping IsBasedOn package
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        public long GetIBOParentPackage(long packageID)
        {
            EA.Package thepackage = repo.GetPackageByID((int)packageID);
            string prov = thepackage.Name;
            long packid = packageID;
            while (!HasIBOPackage(packid))
            {
                packid = repo.GetPackageByID((int)packid).ParentID;
                if (packid == 0) break;
            }
            return packid;
        }
        /// <summary>
        /// give the package on which this package is based on
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        public EA.Package GetIBOPackage(long packageID)
        {
            EA.Package res = null;
            EA.Package thepackage = repo.GetPackageByID((int)packageID);
            EA.Element thepackageelement = thepackage.Element;
            foreach (EA.TaggedValue tag in thepackageelement.TaggedValues)
            {
                if (tag.Name == CD.GetIBOTagValue())
                {
                    try
                    {
                        res = (EA.Package)repo.GetPackageByGuid(tag.Value);
                        break;
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show("error there semms to be no IBO tagvalue for:" + thepackage.Name);
                    }
                }
            }

            return res;
        }
        /// <summary>
        /// gives the based on package  if the package is based on an another package
        /// 0 otherwise
        /// </summary>
        /// <param name="packageid"></param>
        /// <returns></returns>
        public bool HasIBOPackage(long packageid)
        {
            bool ret = false;

            try
            {
                EA.Package thepackage = repo.GetPackageByID((int)packageid);

                EA.Element packagelt = thepackage.Element;
                if (packagelt != null)
                {
                    foreach (EA.Connector con in packagelt.Connectors)
                    {
                        if ((con.Type == "Dependency")
                            && (con.Stereotype.Contains(CD.GetIsBasedOnStereotype()))
                            && (con.ClientID.Equals(packagelt.ElementID))) // must be based on something
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Pb with accessing packageID=" + packageid.ToString() + " " + e.Message);
            }
            return ret;

        }
        /// <summary>
        /// g  if the package is based on an another package
        /// 0 otherwise
        /// </summary>
        /// <param name="packageid"></param>
        /// <returns>pacckageid or 0</returns>
        public bool isAFirstLevelPackage(EA.Repository repo, long packageid)
        {
            bool ret = false;

            try
            {
                EA.Package thepackage = repo.GetPackageByID((int)packageid);

                EA.Element packagelt = thepackage.Element;
                EA.Element newpackagelt = null;
                if (packagelt != null)
                {
                    foreach (EA.Connector con in packagelt.Connectors)
                    {
                        if ((con.Type == "Dependency")
                            && (con.Stereotype.Contains(CD.GetIsBasedOnStereotype()))
                            && (con.ClientID.Equals(packagelt.ElementID))) // must be based on something
                        {
                            newpackagelt = repo.GetElementByID((int)con.SupplierID); // tj=he corresponding package element                         
                            break;
                        }

                    }
                    if (newpackagelt != null)
                    {
                        packagelt = null;
                        foreach (EA.Connector con in newpackagelt.Connectors)
                        {
                            if ((con.Type == "Dependency")
                                && (con.Stereotype.Contains(CD.GetIsBasedOnStereotype()))
                                && (con.ClientID.Equals(newpackagelt.ElementID))) // must be based on something
                            {
                                packagelt = repo.GetElementByID((int)con.SupplierID); // tj=he corresponding package
                                break;
                            }

                        }
                    }
                }
                if (packagelt == null)
                {
                    ret = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Pb with accessing packageID=" + packageid.ToString() + " " + e.Message);
            }

            return ret;

        }
        /// <summary>
        /// this programes will create a list of possible inheritance class 
        /// filtered by possible packages
        /// it applies only on  updated EA classes
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="eaclass"></param>
        /// <returns></returns>
        public ArrayList GetPossibleFilteredExInheritance(EAClass eaclass)
        {
            string prov = eaclass.GetName();
            prov = (eaclass.GetIBOElement()).Name;
            prov = (repo.GetPackageByID((int)(eaclass.GetIBOElement().PackageID))).Name;
            ArrayList ret = new ArrayList();
            if (eaclass.GetIBOElement() == null) return ret;
            ArrayList PossibleInheritance = new ArrayList();
            // EA.Element iboelt = populatedEAClass.GetIBOElement();
            EA.Element iboelt = eaclass.GetIBOElement(); // la classe en 
            List<long> eligiblepackages = new List<long>(); // the package of the profile
            long packageid = GetIBOParentPackage(iboelt.PackageID); // the first parent package to be IB
                                                                    //long packageid = (repo.GetCurrentDiagram()).PackageID;
            if (packageid == 0)
            { // no dependency between package established
                string texte = " WARNING the package of your selected element must have a parent package with an IsBasedOn dependency with the information model layer";


                XMLParser XMLP = new XMLParser(repo);
                if (XMLP.GetXmlValueConfig("Log") == ("Checked"))
                {
                    XMLP.AddXmlLog("DefaultOfIsBasedOnDependency", texte);
                }
                MessageBox.Show(texte);
                return PossibleInheritance;
            }
            eligiblepackages.Add(packageid);

            eligiblepackages = GetNotIBOPackages(packageid, eligiblepackages);
            //  buildDicElems(repo.GetPackageByID((int)packageid), false);
            List<EA.Element> possiblebaseparents = new List<EA.Element>();
            ArrayList FilteredPossibleInheritance = new ArrayList();
            GetIheritanceParentChain(repo.GetElementByGuid(getEltParentGuid(iboelt)), possiblebaseparents);
            string parentguid = "";
            foreach (EA.Element elt in possiblebaseparents)
            {
                parentguid = elt.ElementGUID;
                if (parentguid != "")
                {
                    if (dicProfElemByParentGuid.ContainsKey(parentguid))
                    {
                        foreach (long elid in dicProfElemByParentGuid[parentguid])
                        {
                            if (eligiblepackages.Contains(repo.GetElementByID((int)elid).PackageID) && !FilteredPossibleInheritance.Contains(elid)) FilteredPossibleInheritance.Add(elid);
                        }
                    }
                }
            }
            foreach (long eid in FilteredPossibleInheritance)
            {
                ret.Add(repo.GetElementByID((int)eid));
            }
            return ret;
        }

        public List<EA.Element> GetIheritanceParentChain(EA.Element elt, List<EA.Element> listelts)
        {

            EA.Element parentelt= null; ;

            foreach (EA.Connector con in elt.Connectors)
            {
                if (con.Type == "Generalization")
                {
                    if (elt.ElementID == con.ClientID) // if elt is the source of the generalization
                    {
                        parentelt = repo.GetElementByID(con.SupplierID);
                        if (!listelts.Contains(parentelt)) listelts.Add(parentelt);
                        break;
                    }
                }
            }
            if (parentelt != null)
            {
                listelts = GetIheritanceParentChain(parentelt, listelts);
            }
            return listelts;
        }
        /// <summary>
        /// recupere la chaine d'heritage en excluant les extensions cim
        /// dans le cas d'un heritage dans le cim (cim=true)
        /// </summary>
        /// <param name="elt"></param>
        /// <param name="listelts"></param>
        /// <param name="cim"></param>
        /// <returns></returns>
        public List<EA.Element> GetIheritanceChain(EA.Element elt, List<EA.Element> listelts,bool cim)
        {

            EA.Element ancester = null;
            if (cim)
            {
                if (dicCimPacksByName.Count == 0)
                {
                    EA.Package apackage = repo.GetPackageByID((int)elt.PackageID);
                    getAllCimPackages(apackage, dicCimPacksByName, true);
                }
            }
            foreach (EA.Connector con in elt.Connectors)
            {
                if (con.Type == "Generalization")
                {
                    if (elt.ElementID == con.ClientID) // if elt is the source of the generalization
                    {
                        ancester = repo.GetElementByID(con.SupplierID);
                        EA.Package pack = repo.GetPackageByID((int)ancester.PackageID);
                        if (cim &&(!this.dicCimPacksByName.ContainsKey(pack.Name)))
                        {
                            ancester = null;
                        }
                        else
                        {
                            if (!listelts.Contains(ancester)) listelts.Add(ancester);
                        }
                        break;
                       
                    }
                }
            }
            if (ancester != null)
            {
                listelts = GetIheritanceChain(ancester, listelts,cim);
            }
            return listelts;
        }

        //------------------
        public int getAtRang(EA.Attribute at)
        {
            int resul = -1;
            foreach (EA.AttributeTag tag in at.TaggedValues)
            {
                if (tag.Name == CD.GetRangTagValue())
                {

                    resul = System.Convert.ToInt32(tag.Value);
                    Rang = resul; // met a jour le rang courant
                    break;
                }
            }
            if (resul == -1)
            {
                EA.AttributeTag tag = (EA.AttributeTag)at.TaggedValues.AddNew(CD.GetRangTagValue(), Rang.ToString());
                tag.Update();
                at.TaggedValues.Refresh();
                resul = Rang;
                Rang++;
            }
            return resul;
        }

        public int setAtRang(EA.Attribute at, int rang)
        {
            int resul = -1;
            foreach (EA.AttributeTag tag in at.TaggedValues)
            {
                if (tag.Name == CD.GetRangTagValue())
                {
                    resul = rang;
                    tag.Value = rang.ToString();
                    tag.Update();
                    break;
                }
            }
            if (resul == -1)
            {
                MessageBox.Show("Error the attribute has no tag RangTagValue");
            }
            return resul;
        }


        public int getAtRang(EA.Connector con)
        {
            int resul = -1;
            foreach (EA.ConnectorTag tag in con.TaggedValues)
            {
                if (tag.Name == CD.GetRangTagValue())
                {
                    resul = System.Convert.ToInt32(tag.Value);
                    break;
                }
            }
            if (resul == -1)
            {
                EA.ConnectorTag tag = (EA.ConnectorTag)con.TaggedValues.AddNew(CD.GetRangTagValue(), Rang.ToString());
                tag.Update();
                resul = Rang;
                Rang++;
            }
            return resul;
        }

        public int setAtRang(EA.Connector con, int rang)
        {
            int resul = -1;
            foreach (EA.ConnectorTag tag in con.TaggedValues)
            {
                if (tag.Name == CD.GetRangTagValue())
                {
                    resul = rang;
                    tag.Value = rang.ToString();
                    tag.Update();
                    break;
                }
            }
            if (resul == -1)
            {
                MessageBox.Show("Error the connector has no tag RangTagValue");
            }
            return resul;
        }



        //------------------------------------
        //---
        /// <summary>
        /// build necessaries dictionaries
        /// </summary>
        /// <param name="apckage"></param>
        public void buildDicElems(EA.Package apackage, bool again)
        {
            string parentguid = "";
            if ((dicProfElemByParentGuid.Count == 0) || (again = true))
            { // build or rebuild
                GetAllProfElements(apackage, ListElem);
                foreach (EA.Element el in ListElem)
                {
                    parentguid = getEltParentGuid(el);

                    if (parentguid != "")
                    {
                        if (!dicProfElemByParentGuid.ContainsKey(parentguid))
                        {
                            dicProfElemByParentGuid.Add(parentguid, new List<long>());
                        }
                        if (!dicProfElemByParentGuid[parentguid].Contains(el.ElementID)) dicProfElemByParentGuid[parentguid].Add(el.ElementID);
                    }
                }
            }
        }
        //
        /// <summary>
        /// get all element "class" of a profile
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="listelem"></param>
        public void GetAllProfElements(EA.Package pack, List<EA.Element> listelem)
        {
            foreach (EA.Element el in pack.Elements)
            {
                if (((el.Type == "Class") || (el.Type == "Enumeration")) && !listelem.Contains(el))
                {
                    listelem.Add(el);
                }
            }
            foreach (EA.Package pa in pack.Packages)
            {
                GetAllProfElements(pa, listelem);
            }
        }
        /// <summary>
        /// get all packages id from a package
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="listpackid"></param>
        public void GetAllPackageIds(EA.Package pack,List<long> listpackid)
        {
            if (!listpackid.Contains(pack.PackageID)) listpackid.Add(pack.PackageID);
            foreach (EA.Package pa in pack.Packages)
            {
                if(!listpackid.Contains(pa.PackageID)) listpackid.Add(pa.PackageID);  
            }
        }

        /// <summary>
        /// get all elements with a non-empty name (ABA20230113)
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="listelem"></param>
        public void GetAllElements(EA.Package pack, List<EA.Element> listelem)
        {
            foreach (EA.Element el in pack.Elements)
            {
                if (!listelem.Contains(el) && (el.Name != ""))
                {
                    listelem.Add(el);
                }
            }
            foreach (EA.Package pa in pack.Packages)
            {
                GetAllElements(pa, listelem);
            }
        }
        /// <summary>
        /// get all element "class" of a profile
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="listelem"></param>
        public void GetAllPureProfElements(EA.Package pack, List<EA.Element> listelem)
        {
            List<string> ll = new List<string>(); // pour test am aout 2016
            foreach (EA.Element el in pack.Elements)
            {
                if ((el.Type == "Class") && !listelem.Contains(el) && ("" != getEltParentGuid(el)))
                {
                    ll.Add(el.Name);
                    listelem.Add(el);
                }
            }
            foreach (EA.Package pa in pack.Packages)
            {
                GetAllPureProfElements(pa, listelem);
            }
        }
        /// <summary>
        /// summarizes the number of elements and connectors in a profile
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="compte"></param>
        /// <returns></returns>
        public int GetAllProfElementsCount(EA.Package pack, int compte)
        {
            pack.Update();
            compte = compte + pack.Elements.Count + pack.Connectors.Count;

            foreach (EA.Package pa in pack.Packages)
            {
                compte = compte + GetAllProfElementsCount(pa, compte);
            }
            return compte;
        }
        /// <summary>
        /// get all element "class" of a profile which is not a datatype
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="listelem"></param>
        public void GetAllPureProfElements(EA.Package pack, List<EA.Element> listelem, bool withdatatypes)
        {
            List<string> ll = new List<string>(); // pour test am aout 2016

            foreach (EA.Element el in pack.Elements)
            {
                if ((el.Type == "Class") && !listelem.Contains(el) && ("" != getEltParentGuid(el)))
                {
                    if (withdatatypes // am oct 2016
                        && (!el.StereotypeEx.Contains(CD.GetPrimitiveStereotype()))
                        && (!el.StereotypeEx.Contains(CD.GetEnumStereotype()))
                        && (!el.StereotypeEx.Contains(CD.GetCompoundStereotype()))
                        && (!el.StereotypeEx.Contains(CD.GetDatatypeStereotype()))
                       )
                    {
                        ll.Add(el.Name);
                        listelem.Add(el);
                    }
                }
            }

            foreach (EA.Package pa in pack.Packages)
            {
                GetAllPureProfElements(pa, listelem, withdatatypes);
            }
        }
        //---------------
        public string getEltParentGuid(EA.Element elt)
        {
            string ret = "";
            //string prov = elt.Name; ABA 20240914
            foreach (EA.TaggedValue tag in elt.TaggedValues)
            {
                if (tag.Name == CD.GetIBOTagValue()) // "GUIDBasedOn"
                {
                    ret = tag.Value;
                    break;
                }

            }
            return ret;
        }
        /// <summary>
        /// IBOguid du connecteur IBO
        /// </summary>
        /// <param name="elt"></param>
        /// <returns></returns>
        public string getConParentGuid(EA.Connector con)
        {
            string ret = "";
            foreach (EA.ConnectorTag tag in con.TaggedValues)
            {
                if (tag.Name == CD.GetIBOTagValue())
                {
                    ret = tag.Value;
                    break;
                }

            }
            return ret;
        }
        /// <summary>
        /// recupere l'element IBO
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public EA.Element recupIBOElement(EA.Element el)
        {
            EA.Element res = null;
            string parGUID = getEltParentGuid(el); // ABA20221223
            if (parGUID != "")
            {
                res = repo.GetElementByGuid(parGUID);
            }
            return res;
        }
        //-----
        /// <summary>
        /// for an EA object given by its id 
        /// for test purposes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string getNameFromID(EA.Repository repos, long id, string type)
        {
            string rett = "";
            switch (type)
            {
                case "package":
                    rett = repos.GetPackageByID((int)id).Name;
                    break;
                case "element":
                    rett = repos.GetElementByID((int)id).Name;
                    break;
                case "attribute":
                    rett = repos.GetAttributeByID((int)id).Name;
                    break;

                default:
                    rett = "";
                    break;
            }
            return rett;
        }
        //--------------------------------------------------------------------------
        /// <summary>
        /// remove /r/n from the end of the s
        /// </summary>
        /// <param name="s"></param>
        static public string RemoveRc(string s)
        {
            char[] rc = { '\n', '\r' };
            s = s.TrimEnd(rc);
            return s;
        }
        //-----------------------------------------------------
        public void setAttributeValueAsConstraintIfNecessary(EA.Element el, EA.Attribute atr, EA.AttributeConstraint atrco, List<string> presentfacets)
        {


            if (!Facets.Contains(atrco.Name)) return;
            string notes = RemoveRc(atrco.Notes);
            notes = notes.ToLower();
            int ind1 = notes.IndexOf("->"); // position de operateur OCL
            int ind2 = notes.IndexOf('(', ind1); // parenthese fermante de operateur , suppose aucune parenthese pealable
            int ind3 = notes.IndexOf(')');
            if (!(ind1 > 0) || !(ind2 > ind1 + 2) || !(ind3 > ind1 + 3))
            {
                string texte = " ATTENTION la contrainte " + atrco.Name + " contient " + notes + " et semble mal formee";
                if (XMLP.GetXmlValueConfig("Log") == ("Checked"))
                {
                    XMLP.AddXmlLog("DefaultOfExecuteIsBasedOn", texte);
                }
                return;
            }
            string s = notes.Substring(ind1 + 2, ind2 - ind1 - 2);
            string value = notes.Substring(ind2 + 1, ind3 - ind2 - 1);
            bool found = false;
            foreach (EA.AttributeTag atrtag in atr.TaggedValues)
            {
                if (atrtag.Name == atrco.Name)
                {
                    atrtag.Value = value;
                    atrtag.Update();
                    found = true;
                }

            }
            if (!found)
            {
                EA.AttributeTag attag = (EA.AttributeTag)atr.TaggedValues.AddNew(atrco.Name, value);
                attag.Update();
                atr.TaggedValues.Refresh();
            }
            presentfacets.Add(atrco.Name);
        }


        public void deleteAttributeTaggedValuesIfNecessary(EA.Attribute atr, List<string> presentfacets)
        {
            try
            {
                List<string> tobedeletedatags = new List<string>();
                foreach (EA.AttributeTag atag in atr.TaggedValues)
                {
                    if (Facets.Contains(atag.Name) && !presentfacets.Contains(atag.Name))
                    {
                        tobedeletedatags.Add(atag.Name);
                    }
                }

                for (int i = 0; i < atr.TaggedValues.Count; i++)
                {
                    EA.AttributeTag tag = (EA.AttributeTag)atr.TaggedValues.GetAt((short)i);
                    if (tobedeletedatags.Contains(tag.Name))
                    {// must be deleted
                        atr.TaggedValues.DeleteAt((short)i, true);
                        break; // ABA20230227
                    }
                }
                atr.TaggedValues.Refresh();
                atr.Update();
            }
            catch(Exception ex)
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_032, ex);
            }
        }


        //-----------------------------------------------------------
        /// <summary>
        /// met a jour les tag de IBO GUI pour le package d'assemblage
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="mbiepackage"></param>
        public void majGuids(EA.Repository repo, EA.Package mbiepackage, EA.Package packtoregroup)
        {
            ConstantDefinition CD = new ConstantDefinition();
            Dictionary<string, List<EA.Element>> dicACCguidToAbieElt = new Dictionary<string, List<EA.Element>>();

            // EA.Package packtoregroup = selfut.GetIBOPackage(mbiepackage.PackageID);
            string prov = packtoregroup.Name;
            List<long> dejatraite = new List<long>();
            List<long> condejatrate = new List<long>();


            Utilitaires ut = new Utilitaires(repo);
            List<EA.Element> listeltregroup = new List<EA.Element>();
            ut.GetAllProfElements(packtoregroup, listeltregroup);

            List<EA.Element> listelem = new List<EA.Element>();

            ut.GetAllProfElements(mbiepackage, listelem);

            /********* on cree le dico des elements du package a regrouper ****************/
            foreach (EA.Element el in listeltregroup)
            {
                prov = el.Name;
                if (prov == "Domain")
                {
                    prov = "";
                }
                if (!dejatraite.Contains(el.ElementID))
                {
                    // ABA20230228 string IBOguid = selfut.getEltParentGuid(el);
                    string IBOguid = this.getEltParentGuid(el);
                    if (!dicACCguidToAbieElt.ContainsKey(IBOguid))
                    {
                        dicACCguidToAbieElt.Add(IBOguid, new List<EA.Element>());
                    }
                    dicACCguidToAbieElt[IBOguid].Add(el);
                    foreach (EA.Connector con in el.Connectors)
                    {
                        if ((con.Type == "Association") || (con.Type == "Aggregation"))
                        {
                            EA.ConnectorTag ctag = null;
                            foreach (EA.ConnectorTag contag in con.TaggedValues)
                            {
                                if (contag.Name == CD.GetIBOTagValue())
                                {
                                    ctag = contag;
                                    break;
                                }
                            }
                            if ((ctag != null) && (ctag.Value != ""))
                            {
                                // ABA20230228 selfut -> this
                                if (!this.dicACCguidToABIEcon.ContainsKey(ctag.Value))
                                {
                                    this.dicACCguidToABIEcon.Add(ctag.Value, new List<long>());
                                }
                                if (!this.dicACCguidToABIEcon[ctag.Value].Contains(con.ConnectorID)) this.dicACCguidToABIEcon[ctag.Value].Add(con.ConnectorID);
                            }
                        }
                    }
                }

            }

            prov = "";
            foreach (KeyValuePair<string, List<long>> kv in this.dicACCguidToABIEcon)
            {
                prov = "  guid=" + kv.Key + "  conids= ";
                foreach (long conid in kv.Value)
                {
                    prov = prov + "[" + conid.ToString() + " | " + repo.GetConnectorByID((int)conid).ConnectorGUID + "],";
                }
                // selfut.wlog("TEST", prov + "\n");
                // selfut.wlog("TEST", "************************");

            }
            //--------------
            this.condejatraite = new List<long>();
            foreach (EA.Element el in listelem)
            {
                if (el.Name == "Domain")
                {
                    prov = "";
                }
                string texte = el.Name + "|" + mbiepackage.Name + "|";
                this.setAddParentGuid(repo, el, mbiepackage, dicACCguidToAbieElt); // ABA20230228
            }
        }
        /// <summary>
        /// met à jour l'attribut guid des connexions
        /// </summary>
        /// <param name="el"></param>
        /// <param name="parentel"></param>
        public void majAssocguids(EA.Element el, List<long> condejatraite)
        {
            // wlog("TEST", " ******* MAJ des assocs de " + el.Name + "****************");
            foreach (EA.Connector con in el.Connectors)
            {
                if ((con.Type == "Association") || (con.Type == "Aggregation"))
                {
                    if (!condejatraite.Contains(con.ConnectorID))
                    {
                        EA.Connector IBOcon = repo.GetConnectorByID(con.ConnectorID);
                        //wlog("TEST", " ******* connector conid=" + con.ConnectorID.ToString() + " guid=" + getConParentGuid(con) + " **********");
                        EA.Connector cn = null;
                        condejatraite.Add(con.ConnectorID);
                        //wlog("TEST", " traitement de la connexion " + con.ConnectorID.ToString());

                        if (this.dicACCguidToABIEcon.ContainsKey(this.getConParentGuid(con))) // ABA20230228 selfut -> this
                        {
                            List<long> eligiblecons = this.dicACCguidToABIEcon[this.getConParentGuid(con)]; // ABA20230228 selfut -> this
                            string prov = "";
                            EA.Element otherend = null;
                            string sens = "source";
                            if (el.ElementID == con.ClientID)
                            {
                                otherend = repo.GetElementByID((int)con.SupplierID);
                                sens = "source";
                            }
                            else
                            {
                                otherend = repo.GetElementByID((int)con.ClientID);
                                sens = "target";
                            }
                            foreach (long co in eligiblecons)
                            {
                                prov = prov + co.ToString() + ",";
                            }
                            //wlog("TEST", " ******* pour la connexion " +con.ConnectorID.ToString() + "/" + con.ConnectorGUID +" les cons eligibles sont " + prov + " *******");
                            foreach (long cnid in eligiblecons)
                            {
                                cn = repo.GetConnectorByID((int)cnid);
                                string prov1 = " *****  Analyse de la connexion  " + cnid.ToString();
                                prov1 = prov1 + " entre " + el.Name + "(" + con.ClientEnd.Role + ") et " + otherend.Name + "(" + con.SupplierEnd.Role + ")";
                                EA.Element parentsource = repo.GetElementByID((int)cn.ClientID);
                                EA.Element parenttarget = repo.GetElementByID((int)cn.SupplierID);
                                prov1 = prov1 + " parentSource=" + parentsource.Name + " parenttarget=" + parenttarget.Name;
                                //wlog("TEST",   prov1);
                                if (
                                    ((sens == "source") && (el.Name == parentsource.Name) && (otherend.Name == parenttarget.Name))
                                    || ((sens == "target") && (el.Name == parenttarget.Name) && (otherend.Name == parentsource.Name))
                                    )
                                {
                                    if (
                                        ((con.ClientEnd.Role == cn.ClientEnd.Role)
                                     && (con.SupplierEnd.Role == cn.SupplierEnd.Role))

                                        )
                                    {
                                        foreach (EA.ConnectorTag ctag in con.TaggedValues)
                                        {
                                            if (ctag.Name == CD.GetIBOTagValue())
                                            {
                                                //wlog("TEST", " le tagguid a ete mis a jour pour " + el.Name + " connector guid=" + con.ConnectorID.ToString() + "/" + con.ConnectorGUID + " parentconnector guid=" + cn.ConnectorGUID);
                                                ctag.Value = cn.ConnectorGUID;
                                                ctag.Update();
                                                break;
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

        /// <summary>
        /// met à jour l'attribut de IBOguid d'un element par rapport à son parent
        /// </summary>
        /// <param name="el"></param>
        /// <param name="parentel"></param>
        ///
        public void majAtrguids(EA.Element el, ref EA.Element parentel)
        {
            string prov = el.Name + "|" + parentel.ElementID;
            foreach (EA.TaggedValue tt in el.TaggedValues)
                if (tt.Name == CD.GetIBOTagValue())
                {
                    prov = tt.Value + "|" + parentel.ElementGUID;
                    break;

                }

            foreach (EA.Attribute at in el.Attributes)
            {
                EA.AttributeTag atagguid = null;
                foreach (EA.Attribute atr in parentel.Attributes)
                {
                    if (atr.Name == at.Name)
                    {
                        foreach (EA.AttributeTag atag in at.TaggedValues)
                        {
                            if (atag.Name == CD.GetIBOTagValue())
                            {
                                atag.Value = atr.AttributeGUID;
                                atag.Update();
                                atagguid = atag;
                                break;
                            }
                        }
                        if (atagguid == null)
                        {
                            string texte = "Error l'atribut " + at.Name + " de " + el.Name + " n'a pas de tag guid";
                            MessageBox.Show(texte);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// change le guidbasedonpour celui du package regroupe
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="el"></param>
        /// <param name="mbiepackage"></param>
        /// <param name="dicACCguidToAbieElt"></param>
        public void setAddParentGuid(EA.Repository repo, EA.Element el, EA.Package mbiepackage, Dictionary<string, List<EA.Element>> dicACCguidToAbieElt)
        {
            // ABA20230228 selfut.repo = repo; //pour eviter le null
            ConstantDefinition CD = new ConstantDefinition();
            EA.TaggedValue tag = (EA.TaggedValue)el.TaggedValues.GetByName(CD.GetIBOTagValue());
            List<EA.Element> possibleelts = dicACCguidToAbieElt[this.getEltParentGuid(el)];
            EA.Element parentel = null;
            //List<long> condejatraite = new List<long>();
            //  selfut.condejatraite = new List<long>();


            foreach (EA.Element elt in possibleelts)
            {
                if (elt.Name == el.Name)
                {
                    parentel = elt;
                    break;
                }
            }
            if (parentel != null)
            {
                tag.Value = parentel.ElementGUID;
                string prov = tag.Name;
                bool test = tag.Update();
            }
            el.Update();
            majIBODependency(el, parentel);
            el.Update();

            this.majAtrguids(el, ref parentel);
            //  selfut.wlog("TEST", " avant conddejatraite "+ el.Name + "  " + selfut.condejatraite.Count.ToString());
            this.majAssocguids(el, this.condejatraite);
            // selfut.wlog("TEST", " apres conddejatraite " + el.Name + "  " + selfut.condejatraite.Count.ToString());
        }

        public static void majIBODependency(EA.Element el, EA.Element parentel)
        {
            ConstantDefinition CD = new ConstantDefinition();
            bool existe = false;
            foreach (EA.Connector con in el.Connectors)
            {
                if ((con.Type == "Dependency") && (con.Stereotype == CD.GetIsBasedOnStereotype()))
                {
                    con.SupplierID = parentel.ElementID;
                    con.Update();
                    el.Update();
                    existe = true;
                    break;
                }
            }
            if (!existe)
            {
                string texte = "Error  in propertygrouping  process the element : " + el.Name + " has no IsBasedOn relationship";
                MessageBox.Show(texte);
            }
        }


        //---------------------------------
        public bool isAssociation(EA.Connector con)
        {
            if ((con.Type == "Association") || (con.Type == "Aggregation"))
            {
                return true;
            }
            return false;
        }
        //-----------------------------------
        /// <summary>
        /// get the first parent package (or itself ) which is IBO
        /// if the result is 0 there should be a warning in the log file
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public long getFirstIBOPackage(EA.Repository repos, EA.Package pack)
        {
            long ret = 0;
            long packid = pack.PackageID;
            long parentpackid = 0;

            if (HasIBOPackage(packid))
            {
                ret = pack.PackageID;
            }

            else
            {
                parentpackid = pack.ParentID;
                if (parentpackid != 0)
                    ret = getFirstIBOPackage(repos, repos.GetPackageByID((int)parentpackid));
            }

            return ret;
        }


        /// <summary>
        /// gives the profile package of an element in a given package
        /// The Profile package is necesary IseBasedOn to a ModelInfo package
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="packid"></param>
        /// <returns></returns>
        public EA.Package getProfilePackage(EA.Repository repos, long packid)
        {
            EA.Package ret = null;
            long parentpackid = 0;
            try
            {
                EA.Package pack = repos.GetPackageByID((int)packid);
                if (HasIBOPackage(packid))
                {
                    ret = pack;
                }

                else
                {
                    parentpackid = pack.ParentID;
                    if (parentpackid != 0)
                        ret = getProfilePackage(repos, parentpackid);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Pb while getting profile package  for" + packid.ToString() + " " + e.Message);
            }

            return ret;
        }

        public bool getAllPackagesInAPackage(EA.Repository repo, EA.Package apack, Dictionary<string, EA.Package> lpa)
        {
            string texte;
            bool res = true;
            try
            {
                foreach (EA.Package pa in apack.Packages)
                {
                    if (!lpa.ContainsKey(pa.Name)) 
                    {
                        lpa.Add(pa.Name, pa);
                    }  else
                    { // ABA20221229
                        Validations.validationEntries.Add(new Integrity_Checking.ValidationEntry(Integrity_Checking.Severity.WARNING, ValidationCode.W002, "The package name " + pa.Name + " is not unique!", pa.Name, null, null));
                        res = false;
                        break;
                    }
                    getAllPackagesInAPackage(repo, pa, lpa);
                }
            }
            catch (Exception e)
            {
                texte = "Issue on names of packages of " + apack.Name + "  " + e.Message;
                wlog("getAllPackage", texte);
                res = false;
            }
            return res;
        }
        /// <summary>
        /// loging error in validations
        /// </summary>
        /// <param name="texte"></param>
        public void wlog(string logtype, string texte)
        {
            if (XMLP.GetXmlValueConfig("Log") == ("Checked"))
            {
                XMLP.AddXmlLog(logtype, texte);
            }
        }


        /// <summary>
        /// remove the qualifier from  a name 
        /// </summary>
        /// <param name="s"></param>
        public string RemoveQual(string s)
        {
            Int32 ind = s.IndexOfAny(new char[] { '_' });
            string ss = s.Substring(ind + 1);
            return ss;
        }
        /// <summary>
        /// remove all prefixes before possible several underscores
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string RemoveDQual(string s)
        {
            string ss = s;
            Int32 index = ss.IndexOfAny(new char[] { '_' });
            while (index != -1)
            {
                ss = ss.Substring(index + 1);
                index = ss.IndexOfAny(new char[] { '_' });
            }
            return ss;


        }
        public bool haveSimilarStereotype(string parentelstereo, string elstereo)
        {
            List<string> datatypes = new List<string>(){CD.GetCompoundStereotype(),
                                                     CD.GetDatatypeStereotype(),
                                                     CD.GetEnumStereotype(),
                                                     CD.GetPrimitiveStereotype()
           };
            if (elstereo.Contains(CD.GetDatatypeStereotype()))
            //chek if this class isbasedon a Primitive Class


            {
            }
            if (elstereo == "")
            {
                if (parentelstereo == "")
                {
                    return true;
                }
                else
                {
                    foreach (string ss in datatypes)
                    {
                        if (parentelstereo.Contains(ss))
                        {
                            return false;
                        }
                    }
                }
            }
            string[] tparentelstereo = parentelstereo.Split(',');
            string[] telstereo = elstereo.Split(',');
            bool ret = false;
            for (int i = 0; i < tparentelstereo.Length; i++)
            {
                for (int j = 0; j < telstereo.Length; j++)
                {
                    if (telstereo[j] == tparentelstereo[i])
                    {
                        ret = true;
                        break;
                    }
                    if (ret) break;
                }
            }
            // ****  specific to ESMP
            if (!ret)
            {
                // may be due to the presence of ABIE
                if (XMLP.GetXmlValueConfig("EnableESMPHierarchy") == ("Checked"))
                {
                    for (int j = 0; j < telstereo.Length; j++)
                    {
                        if (telstereo[j] == "ABIE")
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// retrieve the id of the attibute "value" classifier
        /// or 0 if non existant
        /// </summary>
        /// <param name="cimdatatype"></param>
        /// <returns></returns>
        public long getDatatypeValueIdentifierID(EA.Element cimdatatype)
        {
            long ret = 0;
            foreach (EA.Attribute at in cimdatatype.Attributes)
            {
                if (at.Name == "value")
                {
                    ret = at.ClassifierID;
                    return ret;
                }
            }
            return ret;
        }
        /// <summary>
        /// get the ElementID of the Element wich
        /// has a "isbasedon" dependency with the input element
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public long getIBOElementIDFromDependency(EA.Element el)
        {
            long ret = 0;
            foreach (EA.Connector con in el.Connectors)
            {
                if ((con.Type == "Dependency") && (con.Stereotype == CD.GetIsBasedOnStereotype()))
                {
                    if (con.ClientID == el.ElementID)
                    {
                        if (ret == 0)
                        {
                            ret = con.SupplierID;
                        }
                        else
                        {
                            // there should be only one isbasedon dependency
                            return 0;
                        }
                    }
                }
            }
            return ret;
        }
        
        /// <summary>
        /// get the parentAttributeId(basedon)  of a given attribute
        /// returns 0 if not found
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        public EA.Attribute recupIBOAttribute(EA.Attribute at)
        {

            EA.Attribute res = null;
            try
            {
                if (getAtrParentGuid(at) != "")
                {
                    res = repo.GetAttributeByGuid(getAtrParentGuid(at));
                }
            }
            catch (Exception e)
            {
                wlog("recupIBOAttribute", " ISSUE: trying to get parent attribute of "
                    + at.Name + " , " + e.Message);
            }
            return res;
        }
        public string getAtrParentGuid(EA.Attribute atr)
        {
            string ret = "";

            foreach (EA.AttributeTag tag in atr.TaggedValues)
            {
                if (tag.Name == CD.GetIBOTagValue())
                {
                    ret = tag.Value;
                    break;
                }
            }
            return ret;

        }

        public EA.Connector recupIBOConnector(EA.Element el, EA.Connector con)
        {

            EA.Connector res = null;
            try
            {
                if (getConParentGuid(con) != "")
                {
                    res = repo.GetConnectorByGuid(getConParentGuid(con));
                }
            }
            catch (Exception e)
            {
                wlog("recupIBOConnector", " ISSUE: trying to get parent connector of element "
                + el.Name + " with roles " + con.SupplierEnd.Role + "," + con.ClientEnd.Role + "," + e.Message);
            }
            return res;
        }


        public bool isInRelation(EA.Element el, EA.Element autrel)
        {
            bool resul = false;
            if (el.ElementID == autrel.ElementID)
            {
                return true;
            }
            foreach (EA.Connector con in el.Connectors)
            {
                string prov = con.ConnectorGUID;
                if (con.Type == "Generalization")
                {
                    if (con.ClientID == el.ElementID)
                    {
                        if (con.SupplierID == autrel.ElementID)
                        {
                            resul = true;
                            break;
                        }
                        else
                        {
                            resul = isInRelation(repo.GetElementByID((int)con.SupplierID), autrel);
                            break;
                        }
                    }
                }
            }
            return resul;
        }
        /// <summary>
        /// true if multiplicity is with limits of parentmultiplicity
        /// </summary>
        /// <param name="multiplicity"></param>
        /// <param name="parentmutiplicity"></param>
        /// <returns></returns>
        public bool IsCompatibleCardinality(string multiplicity, string parentmultiplicity)
        {
            string sparentuppervalue = "";
            string sparentlowervalue = "";
            string suppervalue = "";
            string slowervalue = "";
            int parentuppervalue = 0;
            int uppervalue = 0;
            int parentlowervalue = 0;
            int lowervalue = 0;
            bool resul = false;
            if ((multiplicity == "") && (parentmultiplicity == "")) return true;
            if (parentmultiplicity != "")
            {
                if (parentmultiplicity.Contains(".."))
                {
                    sparentuppervalue = parentmultiplicity.Trim().Substring(3, 1);
                    sparentlowervalue = parentmultiplicity.Trim().Substring(0, 1);
                }
                else
                {
                    sparentuppervalue = sparentlowervalue = parentmultiplicity;
                }
            }
            if (multiplicity != "")
            {
                if (multiplicity.Contains(".."))
                {
                    suppervalue = multiplicity.Trim().Substring(3, 1);
                    slowervalue = multiplicity.Trim().Substring(0, 1);
                }
                else
                {
                    suppervalue = slowervalue = multiplicity;
                }
            }
            if (sparentlowervalue == "") return false;
            if ((slowervalue != "*") && (sparentlowervalue != "*"))
            {
                lowervalue = Convert.ToInt32(slowervalue);
                parentlowervalue = Convert.ToInt32(sparentlowervalue);
                if (lowervalue <= parentlowervalue)
                {
                    resul = true;

                }

                if (sparentuppervalue == "*")
                {
                    resul = true;
                }
                else
                {
                    resul = false;
                    if (suppervalue != "*")
                    {
                        parentuppervalue = Convert.ToInt32(sparentuppervalue);
                        uppervalue = Convert.ToInt32(suppervalue);
                        if (uppervalue <= parentuppervalue)
                        {
                            resul = true;
                        }
                    }
                }

            }
            return resul;
        }
        public static void getAllPackages(EA.Package package, List<EA.Package> resultList)
        {
            string prov;
            // reportlog.WriteLine("\npackage-1 " + package.Name + " | " + package.PackageID + "| count " + resultList.Count);
            if (!resultList.Contains(package))
            {
                prov = package.Name;
                resultList.Add(package);
                //     reportlog.WriteLine("\npackage-2 " + package.Name + " | " + package.PackageID + "| count " + resultList.Count);
            }
            foreach (EA.Package pack in package.Packages)
            {
                //   reportlog.WriteLine("\npackage-3 " + pack.Name + " | " + pack.PackageID + "| count " + resultList.Count);
                getAllPackages(pack, resultList);
            }
        }
        /// <summary>
        /// Get the top model package   from a given Package
        /// the result is the model package
        /// </summary>
        /// <param name="pa"></param>
        /// <param name="repo"></param>
        static public EA.Package GetModelPackage(EA.Package pa, EA.Repository repo)
        {
            EA.Package retpa = null;
            long paid = pa.ParentID;
            if (paid == 0)  //on a trouve
            {
                return pa;
            }
            while (paid != 0)
            {
                retpa = repo.GetPackageByID((int)paid);
                paid = retpa.ParentID;
                if (paid == 0) break;   // on a trouve                            
            }
            return retpa;
        }
        /// <summary>
        /// Get the package directly under the top model package   from a given Package
        /// the result is the submodel package
        /// </summary>
        /// <param name="pa"></param>
        /// <param name="repo"></param>
        static public EA.Package GetSubModelPackage(EA.Package pa, EA.Repository repo)
        {
            EA.Package retpa = null;
            EA.Package subretpa = null;
            long paid = pa.ParentID;
            while (paid != 0)
            {
                subretpa = retpa;
                retpa = repo.GetPackageByID((int)paid);
                paid = retpa.ParentID;
                if (paid == 0) break;   // on a trouve                            
            }
            return subretpa;
        }
        /// <summary>
        /// getAncesters
        /// find all the ancestersID of an element el as a list ancesters
        /// this program is recursive
        /// It populates a List<long>>
        /// memorizing step by step the ancesters of a given element
        /// hence accelerating the computing for the following steps
        /// </summary>
        /// <param name="el"></param>
        /// <param name="ancesters"></param>
        public void getAncesters(EA.Element el, List<long> ancesters)
        {
            List<long> locancesters = ancesters;
            if (!dicAncestors.ContainsKey(el.ElementID)) dicAncestors[el.ElementID] = new List<long>();
            if (dicAncestors[el.ElementID].Count != 0) // the list of ancestors is not empty
            {
                List<long> ll = dicAncestors[el.ElementID];
                for (int i = 0; i < ll.Count; i++)
                {
                    if (!ancesters.Contains(ll[i]))
                    {
                        ancesters.Add(ll[i]);
                    }
                }
            }
            else
            {
                List<long> newancesters = new List<long>();
                for (short i = 0; i < el.Connectors.Count; i++)
                {
                    EA.Connector con = (EA.Connector)el.Connectors.GetAt(i);
                    if ((con.Type == "Generalization") && (el.ElementID == con.ClientID)) // the connector generalizes con.supplier
                    {
                        if (!ancesters.Contains(con.SupplierID)) ancesters.Add(con.SupplierID);// am oct 17
                        getAncesters(repo.GetElementByID((int)con.SupplierID), newancesters);
                        // this.wlog("TESTgetAncesters", "après appel getancester EltName=" + el.Name.ToString() + "dicnb=" + dicAncestors.Count + " nbanc=" + ancesters.Count + " nbnewanc=" + newancesters.Count);                        
                        for (int j = 0; j < newancesters.Count; j++)
                        {
                            if (!ancesters.Contains(newancesters[j]))
                            {
                                ancesters.Add(newancesters[j]);
                            }
                        }
                        // this.wlog("TESTgetAncesters", "après incorporation newancesters getancester EltName=" + el.Name.ToString() + "dicnb=" + dicAncestors.Count + " nbanc=" + ancesters.Count );

                    }
                }
                dicAncestors[el.ElementID] = new List<long>(ancesters);
                //  this.wlog("TESTgetAncesters", "final EltName=" + el.Name.ToString() + "dicanc[" + el.ElementID.ToString() + "]nb=" + dicAncestors[el.ElementID].Count + " nbanc=" + ancesters.Count);
            }
        }
        public void oldgetAncesters(EA.Element el, List<long> ancesters)
        {
            // ta.elapsetime(el.Name);

            foreach (EA.Connector con in el.Connectors)
            {
                if ((con.Type == "Generalization") && (el.ElementID == con.ClientID)) // the connector generalizes con.supplier
                {
                    //ancesters.Add(con.SupplierID);//am oct 17
                    ancesters.Add(con.SupplierID);// am avr 18
                    if (!ancesters.Contains(con.SupplierID)) ancesters.Add(con.SupplierID);// am oct 17
                                                                                           // this.wlog("TESTgetAncesters", " EltName=" + el.Name.ToString() + " EltAncestor=" + repo.GetElementByID((int)con.SupplierID) + " EltAncestorID=" + con.SupplierID.ToString());
                    getAncesters(repo.GetElementByID((int)con.SupplierID), ancesters);
                    //break;//am oct 17
                    break; //am avr 18
                }
            }

            // ta.diffelaps(el.Name);
        }


        public void getAncesters(EA.Element el, List<long> ancesters, Dictionary<long, EA.Element> dicel)
        {
            //   this.wlog("TESTgetAncesters", "Entree du ss programme EltName=" + el.Name.ToString() + " ancestersCount="+ ancesters.Count+ "dicelcount="+dicel.Count);
            foreach (EA.Connector con in el.Connectors)
            {
                if ((con.Type == "Generalization") && (el.ElementID == con.ClientID)) // the connector generalizes con.supplier
                {
                    //ancesters.Add(con.SupplierID);// am oct 17 
                    // ancesters.Add(con.SupplierID);// am avr 18
                    if (!ancesters.Contains(con.SupplierID))
                    {
                        ancesters.Add(con.SupplierID);// am oct 17

                        // this.wlog("TESTgetAncesters", " EltName=" +ancesters.Count.ToString() + " " + el.Name.ToString() + " EltAncestor=" + repo.GetElementByID((int)con.SupplierID) + " EltAncestorID=" + con.SupplierID.ToString());
                    }
                    if (!dicel.ContainsKey(con.SupplierID))
                    {
                        dicel[con.SupplierID] = repo.GetElementByID((int)con.SupplierID);
                        // this.wlog("TESTgetAncesters", " EltName=" + dicel.Count.ToString() + " " +el.Name.ToString() + " EltAncestor=" + repo.GetElementByID((int)con.SupplierID) + " EltAncestorID=" + con.SupplierID.ToString());
                    }
                    EA.Element ancester = repo.GetElementByID((int)con.SupplierID);
                    getAncesters(ancester, ancesters, dicel);
                    // break; //am oct 17
                    break; //am avr 18
                }
            }
            // this.wlog("TESTgetAncesters", "Sortie du ss programme EltName=" + el.Name.ToString() + " ancestersCount=" + ancesters.Count + "dicelcount=" + dicel.Count);
        }

        /// <summary>
        ///  if the elements hare in hierachy
        ///  gives them from ascendant to descendant
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<EA.Element> areInHierarchy(List<EA.Element> elts)
        {
            //foreach (EA.Element el in elts)
            //{
            //    string prov = "a";
            //}
            List<EA.Element> res = null;

            return res;
        }

        public void getDescendants(EA.Element el, List<long> descendants)
        {
            foreach (EA.Connector con in el.Connectors)
            {
                if ((con.Type == "Generalization") && (el.ElementID == con.SupplierID)) // the connector generalizes con.ClientID
                {
                    descendants.Add(con.ClientID);
                    // this.wlog("TESTgetDescendants", " EltName=" + el.Name.ToString() + " EltDescsndant=" + repo.GetElementByID((int)con.ClientID) + " EltDescendantID=" + con.SupplierID.ToString());
                    getDescendants(repo.GetElementByID((int)con.ClientID), descendants);
                    break;
                }
            }

        }

        public void getDescendants(EA.Element el, List<long> descendants, bool avecmemoire)
        {
            //this.wlog("TESTgetDescendants", " EltName=" + el.Name + " nbdescendants" + descendants.Count  );
            if (!dicDescendants.ContainsKey(el.ElementID)) dicDescendants[el.ElementID] = new List<long>();
            //this.wlog("TESTgetDescendants", " EltName=" + el.Name + "dicnb=" + dicDescendants.Count);
            if (dicDescendants[el.ElementID].Count != 0) // the list of descendants is not empty
            {
                List<long> ll = dicDescendants[el.ElementID];
                // this.wlog("TESTgetDescendants", " EltName=" + el.Name + "llnb=" + ll.Count);
                for (int i = 0; i < ll.Count; i++)
                {
                    if (!descendants.Contains(ll[i]))
                    {
                        descendants.Add(ll[i]);
                    }
                }
            }
            else
            {
                List<long> newdescendants = new List<long>();
                for (short i = 0; i < el.Connectors.Count; i++)
                {
                    EA.Connector con = (EA.Connector)el.Connectors.GetAt(i);
                    if ((con.Type == "Generalization") && (el.ElementID == con.SupplierID)) // the connector generalizes con.supplier
                    {
                        if (!descendants.Contains(con.ClientID)) descendants.Add(con.ClientID);// am oct 17
                        getDescendants(repo.GetElementByID((int)con.ClientID), newdescendants);
                        // this.wlog("TESTgetDescendants", "après appel getancester EltName=" + el.Name.ToString() + "dicnb=" + dicDescendants.Count + " nbanc=" + descendants.Count + " nbnewanc=" + newdescendants.Count);                        
                        for (int j = 0; j < newdescendants.Count; j++)
                        {
                            if (!descendants.Contains(newdescendants[j]))
                            {
                                descendants.Add(newdescendants[j]);
                            }
                        }
                        // this.wlog("TESTgetDescendants", "après incorporation newdescendants getancester EltName=" + el.Name.ToString() + "dicnb=" + dicDescendants.Count + " nbanc=" + descendants.Count );

                    }
                }
                dicDescendants[el.ElementID] = new List<long>(descendants);
                //  this.wlog("TESTgetDescendants", "final EltName=" + el.Name.ToString() + "dicanc[" + el.ElementID.ToString() + "]nb=" + dicDescendants[el.ElementID].Count + " nbanc=" + descendants.Count);
            }

        }
        /// <summary>
        /// gives the list of all the profiles elements corresponding to aan element in the  profile package
        /// </summary>
        /// <param name="elid"></param>
        /// <param name="dicIBOElemntByParent"></param>
        /// <returns></returns>
        public List<EA.Element> getCorrespondingProfElts(EA.Element el, Dictionary<long, List<EA.Element>> dicIBOElemntByParent, EA.Package profpack)
        {
            List<EA.Element> ret = new List<EA.Element>();
            List<EA.Element> elts = new List<EA.Element>();
            if (dicIBOElemntByParent.ContainsKey(el.ElementID))
            {
                elts = dicIBOElemntByParent[el.ElementID]; // on recupere tous les enments (ATTENTION verifier la coherence en cas de relance sur un 
                                                           // autre profil
            }
            else
            {
                dicIBOElemntByParent[el.ElementID] = new List<EA.Element>();
                List<long> ll = new List<long>();

                getDescendants(el, ll);
                foreach (long l in ll)
                {
                    EA.Element elt = Main.Repo.GetElementByID((int)l); // on regarde si l'element est bien dans le package
                    if (getProfilePackage(Main.Repo, elt.PackageID).PackageID == profpack.PackageID)
                    {
                        // si oui alors on le rajoute aux elements
                        if (!elts.Contains(elt)) elts.Add(elt); // esperont que l'on recupere bine toujours le meme pointeur pour un mem indexe
                    }
                }
            }
            dicIBOElemntByParent[el.ElementID] = elts; // dicIBOElementByParent is updated
            ret = elts;
            return ret;
        }
        /// <summary>
        /// decide si il y a inversion des source, target en cas d'update d'une association
        /// </summary>
        /// <param name="con"></param>
        /// <param name="originalcon"></param>
        /// <param name="el"></param>
        /// <returns></returns>
        public bool isSwitched(EA.Connector con, EA.Connector parencon, EA.Element el)
        {
            bool ret = true;


            EA.Element elparent = repo.GetElementByGuid(getEltParentGuid(el));
            bool cli = false;// am nov 2018
            if (utilitaires.Utilitaires.dicAncestors.ContainsKey(parencon.ClientID))
            {
                if (utilitaires.Utilitaires.dicAncestors[parencon.ClientID].Contains(elparent.ElementID)) cli = true;

            }
            bool sup = false;
            if (utilitaires.Utilitaires.dicAncestors.ContainsKey(parencon.SupplierID))
            {
                if (utilitaires.Utilitaires.dicAncestors[parencon.SupplierID].Contains(elparent.ElementID)) sup = true;

            }
            if (
              ((con.ClientID == el.ElementID) && (parencon.ClientID == elparent.ElementID))
              || ((con.ClientID == el.ElementID) && cli) //(utilitaires.Utilitaires.dicAncestors[parencon.ClientID].Contains(elparent.ElementID)))// am nov 2018
              || ((con.SupplierID == el.ElementID) && (parencon.SupplierID == elparent.ElementID))  // am juin 2016
              || ((con.SupplierID == el.ElementID) && sup) //(utilitaires.Utilitaires.dicAncestors[parencon.SupplierID].Contains(elparent.ElementID)))// am nov 2018
              )
            {
                //il n'y a pas d'inversion
                ret = false;
            }

            return ret;

        }


        public string isContained(EA.Element selectedElement, EditEAClassConnector parentEAConnector)
        {
            string ret = "";
            EA.Element sel = parentEAConnector.GetSelectedElementConnector();
            EA.Connector ocon = parentEAConnector.GetOriginalConnector();
            if (ocon.ClientEnd.Aggregation == 1)
            {
                ret = "Source";
            }
            else
            {
                if (ocon.SupplierEnd.Aggregation == 1)
                {
                    ret = "Target";
                }
            }

            return ret;
        }


        public string testConnectorState(EA.Connector con, string mes)
        {

            if (con == null)
            {
                // wlog("TEST", " EditEAClassCon con=null" + mes);
                return "";
            }
            else
            {
                string text = " EditEAClassCon " + mes + " ";
                text = text + " ClientAgg= " + con.ClientEnd.Aggregation;
                // text=text + " ClientCardinality= " + con.ClientEnd.Cardinality ;
                text = text + " ClientNavigability=" + con.ClientEnd.Navigable;
                text = text + " SupplierAggreg= " + con.SupplierEnd.Aggregation;
                //text=text + "role= " + con.SupplierEnd.Role + "\n";
                // text=text + " SupplierCardinality= " + con.SupplierEnd.Cardinality;
                text = text + " SupplierNavigability=" + con.SupplierEnd.Navigable;
                text = text + " Guid" + con.ConnectorGUID;
                // MessageBox.Show(text);
                //  wlog("TEST",text);
                return text;
            }
        }

        /// <summary>
        ///pour editconnectors recupere l'inversion eventuelle de la connexion d'origine donnee par son client
        /// et de la connexion profil
        /// on passe en parametre l'element parent de l'element du profil selectionne
        /// comme la connexion peut etre heritee il faut aller chercher eventuellement
        /// un des ancetres de cet element
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="elt"></param>
        /// <returns></returns>
        public bool getSSwitch(long clientid, EA.Element elt)
        {
            if (clientid == elt.ElementID) // am janv 2017
            {
                return false;

            }
            bool res = true;
            List<long> ancestors = new List<long>();
            getAncesters(elt, ancestors);
            foreach (long eltid in ancestors)
            {
                if (clientid == eltid) // un element de la liste est egal a l'un des ancetres
                {
                    res = false;
                    break;
                }
            }
            return res;
        }

        public string getEltAttributeValue(EA.Element elt, string atrname)
        {
            string res = "";
            foreach (EA.Attribute at in elt.Attributes)
            {
                if (at.Name == atrname)
                {
                    res = at.Default;
                }
            }
            return res;
        }
        /// <summary>
        /// loging error if modetest in validations
        /// </summary>
        /// <param name="texte"></param>
        public void wtest(string logtype, string texte)
        {
            // if (modetest)
            if (XMLP.GetXmlValueConfig("Log") == ("Checked"))
            {
                XMLP.AddXmlLog(logtype, texte);
            }
        }
        /// <summary>
        /// check if an element is in the profile of the given element or in the datatype Domain
        /// according to kind = entsoe| esmp
        ///
        /// </summary>
        /// <param name="el"></param>
        /// <param name="Classifier"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public bool isClassifierInProfile(EA.Element el, EA.Element Classifier, string kind)
        {
            bool res = true;
            EA.Package profpack = getProfilePackage(repo, el.PackageID);// the profile pack
            EA.Package classifierpack = repo.GetPackageByID(Classifier.PackageID);
            string prov = profpack.Name;
            if (kind == "entsoe")
            {
                // si le classifier n'est pas dans le meme prifilepack que l'element
                //le signaler
                if (classifierpack.PackageID != profpack.PackageID) res = false;
                return res;
            }
            EA.Package elmodelpack = GetSubModelPackage(profpack, repo); //the model pack for el
            EA.Package classifiermodelpack = GetSubModelPackage(classifierpack, repo);
            prov = elmodelpack.Name;
            prov = classifiermodelpack.Name;
            EA.Package domainpack = null;
            string namedomain = ""; // the name of the package including th datatypes
            switch (kind)
            {
                case "entsoe":
                  
                    break;
                case "ESMPEnumeration":
                    namedomain = "ESMPEnumerations";
                    break;
                case "ESMPDatatype":
                    namedomain = "ESMPDataTypes";
                    break;
                default:
                    break;
            }
            if (namedomain == "") return res;
            domainpack = GetAPackageByName(repo, elmodelpack, namedomain);// the name of all packages must be different


            prov = domainpack.Name;
            // if the classifier is defined localy
            if (Classifier.PackageID == domainpack.PackageID) return true; // the classifier is in local datatype domain
            if (elmodelpack.PackageID != classifiermodelpack.PackageID) return false; // the classifier is not in the same model as the element
            EA.Package pack = getProfilePackage(repo, Classifier.PackageID);
            prov = pack.Name;
            if (pack.PackageID != profpack.PackageID) return false;// if the classifier is not in the local domain it can be in the profile
            return res;
        }
        /// <summary>
        /// search a package by a name
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="pack"></param>
        /// <param name="packname"></param>
        /// <returns></returns>
        public EA.Package GetAPackageByName(EA.Repository repo, EA.Package pack, string packname)
        {
            EA.Package res = null;
            foreach (EA.Package pa in pack.Packages)
            {
                if (pa.Name == packname)
                {
                    res = pa;
                    break;
                }
                else
                {
                    res = GetAPackageByName(repo, pa, packname);
                }
                if (res != null) break;
            }
            return res;
        }

        /// <summary>
        /// copy an element (classifier) to the package for datatypes
        /// return the copied classifier
        /// </summary>
        /// <param name="el"></param>
        /// <param name="domain"></param>
        public EA.Element copyElement(EA.Element el, EA.Package domain)
        {
            //if( el.Name=="String")
            //{
            //    int prov = 1;
            //}
            XMLP.AddXmlLog("utilitaires", String.Format("copyElement debut el={0} nbn=domainelents={1}", el.Name,domain.Elements.Count));
            EA.Element ret = null;
            bool isenum = false; try
            {
                foreach (EA.Element elt in domain.Elements)
                {
                    if (elt.Name == el.Name) // it is not necessary to copy 
                    {
                        ret = elt;

                        break;
                    }
                }

                if (ret == null) //must be copied
                {
                    if (
                       (el.StereotypeEx.Contains("enumeration"))
                       ||
                        (el.StereotypeEx.Contains("Enumeration"))
                       ||
                        ((el.Type == "Enumeration"))
                       ||
                       ((el.MetaType == "Enumeration"))
                       )
                    {
                        isenum = true;
                    }

                    ret = (EA.Element)domain.Elements.AddNew(el.Name, el.Type);
                    domain.Elements.Refresh();
                    if (!ret.Update()) throw new Exception(ret.GetLastError());

                    ret.Abstract = el.Abstract;
                    ret.ActionFlags = el.ActionFlags;
                    ret.Alias = el.Alias;
                    ret.Author = el.Author;
                    ret.Complexity = el.Complexity;
                    ret.Created = el.Created;
                    ret.Gentype = el.Gentype;
                    ret.IsActive = el.IsActive;
                    ret.IsComposite = el.IsComposite;
                    ret.IsLeaf = el.IsLeaf;
                    ret.Multiplicity = el.Multiplicity;
                    ret.Name = el.Name;
                    ret.Notes = el.Notes;
                    ret.PackageID = domain.PackageID;
                    ret.Persistence = el.Persistence;
                    ret.Status = el.Status;
                    ret.Stereotype = el.Stereotype;
                    ret.StereotypeEx = el.StereotypeEx;
                    ret.Tag = el.Tag;
                    ret.Type = el.Type;
                    ret.Version = el.Version;
                    ret.Visibility = el.Visibility;
                    if (!ret.Update()) throw new Exception(ret.GetLastError());
                    // -------------- collections -------------

                    // treatment of isBasedOn
                    // taggedValues el.TaggedValues
                    bool iselbasedon = false; // to detect if el is based on another element
                    foreach (EA.TaggedValue tag in el.TaggedValues)
                    {
                        if (tag.Name == CD.GetIBOTagValue()) // it is based on
                        {
                            iselbasedon = true;
                        }
                        EA.TaggedValue newtag = (EA.TaggedValue)ret.TaggedValues.AddNew(tag.Name, "");
                        ret.TaggedValues.Refresh();
                        if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                        newtag.ElementID = ret.ElementID;
                        newtag.Name = tag.Name;
                        newtag.Notes = tag.Notes;
                        newtag.Value = tag.Value;
                        if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                    }
                    // treatment of collections
                    //  ATTENTION  --- here we will have to take care of the inheritance when we have a spec
                    foreach (EA.Connector con in el.Connectors)
                    {
                        if (con.Type == "Generalization")
                        {
                            MessageBox.Show(" inheritance of datatypes not yet treated");
                        }
                        if (
                                iselbasedon  
                                && (con.Type == "Dependency")
                                && (con.Stereotype== CD.GetIsBasedOnStereotype())
                                &&  (con.ClientID==el.ElementID)

                           )
                                {
                                EA.Connector co = (EA.Connector)ret.Connectors.AddNew("", "Dependency");
                                co.ClientID = ret.ElementID;
                                co.SupplierID = con.SupplierID;
                                co.Stereotype = CD.GetIsBasedOnStereotype();
                                co.StereotypeEx = CD.GetIsBasedOnStereotype();
                                if (!co.Update()) throw new Exception(co.GetLastError());
                                ret.Connectors.Refresh();
                            XMLP.AddXmlLog("utilitaires", String.Format("copyElement debut el={0} ret={1} nbibo={2}", el.Name, ret.Elements, ret.Connectors.Count));
                              }
                    }
                    if (!iselbasedon) // if el is not based on it is part of CIM and we must base ret on it
                    {     
                    EA.TaggedValue newtag = (EA.TaggedValue)ret.TaggedValues.AddNew(CD.GetIBOTagValue(), "");
                        ret.TaggedValues.Refresh();
                        if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                        newtag.ElementID = ret.ElementID;
                        newtag.Name = CD.GetIBOTagValue();
                        newtag.Notes = "";
                        newtag.Value = el.ElementGUID;
                        if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                        EA.Connector coo = (EA.Connector)ret.Connectors.AddNew("", "Dependency");
                        ret.Connectors.Refresh();
                        XMLP.AddXmlLog("utilitaires", String.Format("copyElement el={0} refresh apres ajout isbadeon nb elts={1}", el.Name,domain.Elements.Count));
                        coo.ClientID = ret.ElementID;
                        coo.SupplierID = el.ElementID;
                        coo.Stereotype = CD.GetIsBasedOnStereotype();
                        coo.StereotypeEx = CD.GetIsBasedOnStereotype();
                        if (!coo.Update()) throw new Exception(coo.GetLastError());
                    }

                    //constraints el.Constraints
                    foreach (EA.Constraint cc in el.Constraints)
                    {
                        EA.Constraint newcc = (EA.Constraint)ret.Constraints.AddNew(cc.Name, "");
                        newcc.Update();
                        ret.Constraints.Refresh();
                        newcc.Name = cc.Name;
                        newcc.Notes = cc.Notes;
                        newcc.Status = cc.Status;
                        newcc.Weight = cc.Weight;
                        newcc.Type = cc.Type;
                        if (!newcc.Update()) throw new Exception(newcc.GetLastError());
                    }
                    // Attributes el.Attributes
                    foreach (EA.Attribute at in el.Attributes)
                    {
                        if (!isenum)
                        {
                            if (at.ClassifierID == 0)// the attribute has no valid type
                            {
                                throw new Exception(" attribute " + at.Name + " has not a valid classifier");

                            }
                        }
                        EA.Attribute atr = copyAttribute(at, el, ret, domain);
                        if (atr == null) // it did not go well
                        {
                            ret = null;
                            throw new Exception(" pb in copying attribute " + at.Name);
                        }

                    }

                }

            }
            catch (Exception e)
            {
                throw new Exception("problem in copying the classifier" + el.Name + " in domain: " + domain.Name + " " + e.Message);
            }
            XMLP.AddXmlLog("utilitaires", String.Format("copyElement fin  el={0} nbn=domainelents={1}", el.Name, domain.Elements.Count));
            return ret;

        }



        /// <summary>
        /// copy an element (classifier) to the package cible and if a datatype to the domain if exits
        /// return the copied element
        /// </summary>
        /// <param name="el"></param>
        /// <param name="cible"></param>
        /// <param name="domain"></param>
        public EA.Element copyElement(EA.Element el, EA.Package cible, string typcopy)
        {
            Dictionary<string, EA.Package> dicProfIntermedPacks = new Dictionary<string, EA.Package>();// dictionary qui donne les package inter
            EA.Element ret = null;
            bool isenum = false;
            try
            {

                foreach (EA.Element elt in cible.Elements)
                {
                    if (elt.Name == el.Name) // it is not necessary to copy 
                    {
                        ret = elt;
                        break;
                    }
                }
                foreach (EA.Package pa in cible.Packages)
                {
                    foreach (EA.Element elem in pa.Elements)
                    {
                        if (elem.Name == el.Name) // it is not necessary to copy 
                        {
                            ret = elem;
                            break;
                        }
                        if (ret != null) break;
                    }
                }

                if (ret != null) return el; //must not be copied



                if (
                     (el.StereotypeEx.Contains("enumeration"))
                       ||
                     (el.StereotypeEx.Contains("Enumeration"))
                       ||
                     ((el.Type == "Enumeration"))
                       ||
                     ((el.MetaType == "Enumeration"))
                   )
                {
                    isenum = true;
                }
                //----------------------------------
                // on recre les packages intermédiaires
                //--------------------------------------
                // on commence par rechercher un eventuel package intermediare
                //


                ret = (EA.Element)cible.Elements.AddNew(el.Name, el.Type);
                cible.Elements.Refresh();
                if (!ret.Update()) throw new Exception(ret.GetLastError());
                ret.Abstract = el.Abstract;
                ret.ActionFlags = el.ActionFlags;
                ret.Alias = el.Alias;
                ret.Author = el.Author;
                ret.Complexity = el.Complexity;
                ret.Created = el.Created;
                ret.Gentype = el.Gentype;
                ret.IsActive = el.IsActive;
                ret.IsComposite = el.IsComposite;
                ret.IsLeaf = el.IsLeaf;
                ret.Multiplicity = el.Multiplicity;
                ret.Name = el.Name;
                ret.Notes = el.Notes;
                ret.PackageID = cible.PackageID;
                ret.Persistence = el.Persistence;
                ret.Status = el.Status;
                ret.Stereotype = el.Stereotype;
                ret.StereotypeEx = el.StereotypeEx;
                ret.Tag = el.Tag;
                ret.Type = el.Type;
                ret.Version = el.Version;
                ret.Visibility = el.Visibility;
                if (!ret.Update()) throw new Exception(ret.GetLastError());
                // -------------- collections -------------

                // treatment of isBasedOn
                // taggedValues el.TaggedValues
                bool iselbasedon = false; // to detect if el is based on another element
                foreach (EA.TaggedValue tag in el.TaggedValues)
                {
                    if (tag.Name == CD.GetIBOTagValue()) // it is based on
                    {
                        iselbasedon = true;
                    }
                    EA.TaggedValue newtag = (EA.TaggedValue)ret.TaggedValues.AddNew(tag.Name, "");
                    ret.TaggedValues.Refresh();
                    if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                    newtag.ElementID = ret.ElementID;
                    newtag.Name = tag.Name;
                    newtag.Notes = tag.Notes;
                    newtag.Value = tag.Value;
                    if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                }



                if (!iselbasedon) // if el is not based on it is part of CIM and we must base ret on it
                {
                    EA.TaggedValue newtag = (EA.TaggedValue)ret.TaggedValues.AddNew(CD.GetIBOTagValue(), "");
                    ret.TaggedValues.Refresh();
                    if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                    newtag.ElementID = ret.ElementID;
                    newtag.Name = CD.GetIBOTagValue();
                    newtag.Notes = "";
                    newtag.Value = el.ElementGUID;
                    if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                    EA.Connector co = (EA.Connector)ret.Connectors.AddNew("", "Dependency");
                    ret.Connectors.Refresh();
                    XMLP.AddXmlLog("utilitaires", String.Format("copyElementtypcopy debut el={0} nb={1}", el.Name, ret.Connectors.Count));
                    co.ClientID = ret.ElementID;
                    co.SupplierID = el.ElementID;
                    co.Stereotype = CD.GetIsBasedOnStereotype();
                    co.StereotypeEx = CD.GetIsBasedOnStereotype();

                    if (!co.Update()) throw new Exception(co.GetLastError());
                }

                //constraints el.Constraints
                foreach (EA.Constraint cc in el.Constraints)
                {
                    EA.Constraint newcc = (EA.Constraint)ret.Constraints.AddNew(cc.Name, "");
                    newcc.Update();
                    ret.Constraints.Refresh();
                    newcc.Name = cc.Name;
                    newcc.Notes = cc.Notes;
                    newcc.Status = cc.Status;
                    newcc.Weight = cc.Weight;
                    newcc.Type = cc.Type;
                    if (!newcc.Update()) throw new Exception(newcc.GetLastError());
                }
                // Attributes el.Attributes
                foreach (EA.Attribute at in el.Attributes)
                {
                    if (!isenum)
                    {
                        if (at.ClassifierID == 0)// the attribute has no valid type
                        {
                            throw new Exception(" attribute " + at.Name + " has not a valid classifier");

                        }
                    }


                    EA.Attribute atr = copyAttribute(at, el, ret, cible, typcopy);
                    if (atr == null) // it did not go well
                    {
                        ret = null;
                        throw new Exception(" pb in copying attribute " + at.Name);
                    }


                    if (typcopy == "avecliens")
                    {
                        // treatment of collections
                        //  ATTENTION  --- here we will have to take care of the inheritance when we have a spec
                        foreach (EA.Connector con in el.Connectors)
                        {
                            if (con.Type == "Generalization")
                            {
                                MessageBox.Show(" inheritance of datatypes not yet treated");
                            }
                            else
                            {
                                if ((con.Type == "Dependency") && (con.Stereotype == CD.GetIsBasedOnStereotype()))
                                {
                                    XMLP.AddXmlLog("utilitaires", String.Format("copyelement el={0} ", el.Name));
                                    EA.Connector co = (EA.Connector)ret.Connectors.AddNew("", "Dependency");
                                    ret.Connectors.Refresh();
                                    XMLP.AddXmlLog("utilitaires", String.Format("copyElement2536 debut el={0} nbn=domainelents={1}", el.Name, ret.Connectors.Count));
                                    co.ClientID = ret.ElementID;
                                    co.SupplierID = con.SupplierID;
                                    co.Stereotype = CD.GetIsBasedOnStereotype();
                                    co.StereotypeEx = CD.GetIsBasedOnStereotype();
                                    if (!co.Update()) throw new Exception(co.GetLastError());
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                throw new Exception("problem in copying the element" + el.Name + " in domain: " + cible.Name + " " + e.Message);
            }
            return ret;

        }


        /// <summary>
        /// copy an attribute from element to element 
        /// iwith localization of the identifier
        ///
        /// </summary>
        /// <param name="elsource"></param>
        /// <param name="eltarget"></param>
        /// <returns></returns>
        public EA.Attribute copyAttribute(EA.Attribute atr, EA.Element elsource, EA.Element eltarget, EA.Package domain)
        {
            EA.Attribute ret = null;
            bool isenum = false;
            try
            {
                if (
                    (eltarget.StereotypeEx.Contains("enumeration"))
                    ||
                     (eltarget.StereotypeEx.Contains("Enumeration"))
                    ||
                     (eltarget.Type == "Enumeration")
                    ||
                    (eltarget.MetaType == "Enumeration")
                    )
                {
                    isenum = true;
                }
                ret = (EA.Attribute)eltarget.Attributes.AddNew(atr.Name, "");
                eltarget.Attributes.Refresh();
                if (!ret.Update()) throw new Exception(ret.GetLastError());
                EA.Element classifier;
                if (!isenum)
                {
                    if (atr.ClassifierID == 0) // does not points on a valid datatype
                    {
                        throw new Exception("attribute does not  have a valid type " + atr.Name);
                    }
                    classifier = (EA.Element)repo.GetElementByID((int)atr.ClassifierID);

                    if (classifier.PackageID != domain.PackageID)
                    {
                        classifier = copyElement(classifier, domain);
                    }
                    ret.ClassifierID = classifier.ElementID;
                }
                ret.Default = atr.Default;
                ret.IsCollection = atr.IsCollection;
                ret.IsConst = atr.IsConst;
                ret.IsDerived = atr.IsDerived;
                ret.IsStatic = atr.IsStatic;
                ret.Length = atr.Length;
                ret.LowerBound = atr.LowerBound;
                ret.Name = atr.Name;
                ret.Notes = atr.Notes;
                ret.Pos = atr.Pos;
                ret.Precision = atr.Precision;
                ret.Scale = atr.Scale;
                ret.Stereotype = atr.Stereotype;
                ret.StereotypeEx = atr.StereotypeEx;
                ret.Style = atr.Style;
                ret.Type = atr.Type;
                ret.UpperBound = atr.UpperBound;
                ret.Visibility = atr.Visibility;
                bool isBasedOn = false;
                foreach (EA.AttributeTag tag in atr.TaggedValues)
                {
                    if (tag.Name == CD.GetIBOTagValue())
                    {
                        isBasedOn = true;
                    }
                    EA.AttributeTag newtag = (EA.AttributeTag)ret.TaggedValues.AddNew(tag.Name, "");
                    newtag.Update();
                    ret.TaggedValues.Refresh();
                    newtag.AttributeID = ret.AttributeID;
                    newtag.Name = tag.Name;
                    newtag.Notes = tag.Notes;
                    newtag.Value = tag.Value;
                    if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                }
                if (!isBasedOn) // the attribute must be basedon
                {
                    EA.AttributeTag newtag = (EA.AttributeTag)ret.TaggedValues.AddNew(CD.GetIBOTagValue(), "");
                    newtag.Update();
                    ret.TaggedValues.Refresh();
                    newtag.AttributeID = ret.AttributeID;
                    newtag.Name = CD.GetIBOTagValue();
                    newtag.Notes = "";
                    newtag.Value = atr.AttributeGUID;
                    if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                }

                foreach (EA.AttributeConstraint cc in atr.Constraints)
                {
                    EA.AttributeConstraint newcc = (EA.AttributeConstraint)ret.Constraints.AddNew(cc.Name, "");
                    newcc.Update();
                    ret.Constraints.Refresh();
                    newcc.AttributeID = ret.AttributeID;
                    newcc.Name = cc.Name;
                    newcc.Notes = cc.Notes;
                    newcc.Type = cc.Type;
                    if (!newcc.Update()) throw new Exception(newcc.GetLastError());
                }

                if (!ret.Update()) throw new Exception(ret.GetLastError());
            }
            catch (Exception e)
            {
                throw new Exception("problem in copying the cattribute " + atr.Name + " in datatype: " + eltarget.Name + " " + e.Message);
            }
            return ret;


        }

        /// <summary>
        /// copy an attribute from element to element 
        /// iwith localization of the identifier
        ///
        /// </summary>
        /// <param name="elsource"></param>
        /// <param name="eltarget"></param>
        /// <returns></returns>
        public EA.Attribute copyAttribute(EA.Attribute atr, EA.Element elsource, EA.Element eltarget, EA.Package domain, string typcopy)
        {
            EA.Attribute ret = null;
            bool isenum = false;
            try
            {
                if (
                    (eltarget.StereotypeEx.Contains("enumeration"))
                    ||
                     (eltarget.StereotypeEx.Contains("Enumeration"))
                    ||
                     (eltarget.Type == "Enumeration")
                    ||
                    (eltarget.MetaType == "Enumeration")
                    )
                {
                    isenum = true;
                }
                ret = (EA.Attribute)eltarget.Attributes.AddNew(atr.Name, "");
                eltarget.Attributes.Refresh();
                if (!ret.Update()) throw new Exception(ret.GetLastError());
                EA.Element classifier;
                if (!isenum)
                {
                    if (atr.ClassifierID == 0) // does not points on a valid datatype
                    {
                        throw new Exception("attribute does not  have a valid type " + atr.Name);
                    }
                    classifier = (EA.Element)repo.GetElementByID((int)atr.ClassifierID);
                    if (typcopy == "avecliens") // si typcopy est avecliens on recopie le clissifier localement
                    {
                        if (classifier.PackageID != domain.PackageID)
                        {
                            classifier = copyElement(classifier, domain, typcopy);
                        }
                    }
                    ret.ClassifierID = classifier.ElementID;
                }
                ret.Default = atr.Default;
                ret.IsCollection = atr.IsCollection;
                ret.IsConst = atr.IsConst;
                ret.IsDerived = atr.IsDerived;
                ret.IsStatic = atr.IsStatic;
                ret.Length = atr.Length;
                ret.LowerBound = atr.LowerBound;
                ret.Name = atr.Name;
                ret.Notes = atr.Notes;
                ret.Pos = atr.Pos;
                ret.Precision = atr.Precision;
                ret.Scale = atr.Scale;
                ret.Stereotype = atr.Stereotype;
                ret.StereotypeEx = atr.StereotypeEx;
                ret.Style = atr.Style;
                ret.Type = atr.Type;
                ret.UpperBound = atr.UpperBound;
                ret.Visibility = atr.Visibility;
                bool isBasedOn = false;
                foreach (EA.AttributeTag tag in atr.TaggedValues)
                {
                    if (tag.Name == CD.GetIBOTagValue())
                    {
                        isBasedOn = true;
                    }
                    EA.AttributeTag newtag = (EA.AttributeTag)ret.TaggedValues.AddNew(tag.Name, "");
                    newtag.Update();
                    ret.TaggedValues.Refresh();
                    newtag.AttributeID = ret.AttributeID;
                    newtag.Name = tag.Name;
                    newtag.Notes = tag.Notes;
                    newtag.Value = tag.Value;
                    if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                }
                if (!isBasedOn) // the attribute must be basedon
                {
                    EA.AttributeTag newtag = (EA.AttributeTag)ret.TaggedValues.AddNew(CD.GetIBOTagValue(), "");
                    newtag.Update();
                    ret.TaggedValues.Refresh();
                    newtag.AttributeID = ret.AttributeID;
                    newtag.Name = CD.GetIBOTagValue();
                    newtag.Notes = "";
                    newtag.Value = atr.AttributeGUID;
                    if (!newtag.Update()) throw new Exception(newtag.GetLastError());
                }

                foreach (EA.AttributeConstraint cc in atr.Constraints)
                {
                    EA.AttributeConstraint newcc = (EA.AttributeConstraint)ret.Constraints.AddNew(cc.Name, "");
                    newcc.Update();
                    ret.Constraints.Refresh();
                    newcc.AttributeID = ret.AttributeID;
                    newcc.Name = cc.Name;
                    newcc.Notes = cc.Notes;
                    newcc.Type = cc.Type;
                    if (!newcc.Update()) throw new Exception(newcc.GetLastError());
                }

                if (!ret.Update()) throw new Exception(ret.GetLastError());
            }
            catch (Exception e)
            {
                throw new Exception("problem in copying the cattribute " + atr.Name + " in datatype: " + eltarget.Name + " " + e.Message);
            }
            return ret;


        }

        /// retrieves recursively elements with a stereotype in a list of  stereotypes or all elements
        /// if no stereotype (null) is given
        /// </sUMM2ary>
        /// <param name="package">the package to start from</param>
        /// <param name="resultList">list to which the found elements should be added</param>
        /// <param name="stereotype">either a given stereotype if only specific elements
        /// should be retrieved, or null if all elements should be retrieved</param>
        /// <returns>a List containing all elements that correspond to the given
        /// criteria (stereotype)</returns>
        public List<EA.Element> getAllElements(EA.Package package, List<EA.Element> resultList, List<string> stereotypes)
        {
            string texte = "";
            string prov = package.Name;
            if (((package == null) || stereotypes == null) || (resultList == null))
            {
                texte = "Error pb in parameters call of ManageIBIutilitaires.getAllElements";
                MessageBox.Show(texte);
                wlog("utilitaires", texte);
                return resultList;
            }
            texte = "************************************************************************";
            wlog("utilitaires", texte);
            texte = " getAllElements(" + package.Name + ")" + DateTime.Now;
            wlog("utilitaires", texte);
            texte = "************************************************************************";
            wlog("utilitaires", texte);
            foreach (EA.Element e in package.Elements)
            {
                prov = e.Name;
                texte = " => processing ... element " + package.Name + "::" + e.Name;
                //  wlog("utilitaires", texte);
                bool ok = true;
                if (stereotypes.Count > 0)
                { // not all
                    foreach (string s in stereotypes)
                    {
                        ok = false;
                        if (e.StereotypeEx.Contains(s))
                        {
                            ok = true;
                            break;
                        }
                        else
                        {
                            if ((stereotypes.Contains("enumeration") || stereotypes.Contains("Enumeration"))
                                &&
                                ((e.Type == "Enumeration") || (e.MetaType == "Enumeration"))
                                )
                            {
                                ok = true;
                                break;
                            }
                        }
                    }
                }
                if (ok)
                {
                    resultList.Add(e);
                }

            }

            foreach (EA.Package p in package.Packages)
            {
                getAllElements(p, resultList, stereotypes);
            }
            return resultList;
        }

        //------------------------------------
        public static Boolean isNotPureElement(EA.Element el)
        {
            string prov = el.Name;
            Boolean res = true;
            foreach (string ss in CimContextor.Utilities.UtilitiesConstantDefinition.CimStereoDatatypes)
            {
                if ((el.StereotypeEx.Contains(ss)) || (el.Type == "Enumeration") || (el.MetaType == "Enumeration"))
                {
                    res = false;
                    break;
                }
            }
            return res;
        }
        public void getAllDiagrams(EA.Repository repo, EA.Package pack, Dictionary<int, EA.Diagram> diagramsbyid)
        {
            List<EA.Package> packages = new List<EA.Package>();
            getAllPackages(pack, packages);
            foreach (EA.Package pa in packages)
            {
                string prov = pa.Name;
                foreach (EA.Diagram diag in pa.Diagrams)
                {
                    if (!diagramsbyid.ContainsKey(diag.DiagramID)) diagramsbyid.Add(diag.DiagramID, diag);
                }
            }

        }
        /// <summary>
        /// est'ce dans le profil?
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="el"></param>
        /// <param name="packenvelop"></param>
        /// <returns></returns>
        public bool IsInProfile(EA.Repository repo, EA.Element el, EA.Package packenvelop)
        {
            bool res = false;

            try
            {
                long envelopid = packenvelop.PackageID;
                string prov = el.Name;
                prov = packenvelop.Name;

                long packid = el.PackageID;
                while (packid != envelopid && packid != 0)
                {
                    prov = repo.GetPackageByID((int)packid).Name;
                    packid = repo.GetPackageByID((int)packid).ParentID;

                }
                if (packid == envelopid) res = true;
                /********* pour test ****************/

                string texte = " IsInprofile " + repo.GetPackageByID(el.PackageID).Name + "::" + el.Name;
                texte = texte + " envelop:  " + packenvelop.Name + " resultat=" + res.ToString();
                // wlog("isInProfil",texte);
                /************************************/
            }
            catch (Exception e)
            {
                /********* pour test ****************/
                string texte = " IsInprofile " + repo.GetPackageByID(el.PackageID).Name + "::" + el.Name;
                texte = texte + " envelop:  " + packenvelop.Name + " exception=" + e.Message;
                //  wlog("isInProfil",texte);
                /************************************/
            }
            return res;

        }
        /// <summary>
        /// recupere l'identifierID d'un attribut soit celui du profil
        ///                                       soit celui d'origine
        /// </summary>
        /// <param name="atr"></param>
        /// <param name="profdatatypes"></param>
        /// <returns></returns>
        public int recoverClassifierID(EA.Repository repo, EA.Element el, EA.Attribute atr, Dictionary<string, long> profdatatypes)
        {
            string prov = atr.Name;
            try
            {
                if (profdatatypes.ContainsKey(atr.Type))
                {
                    atr.ClassifierID = (int)profdatatypes[atr.Type];
                }
                else
                {
                    string IBOguid = getAtrParentGuid(atr);//ManageIBOUtilitaires.getAttributeIBOGuid(atr);
                    if (IBOguid != "")
                    {
                        atr.ClassifierID = repo.GetAttributeByGuid(IBOguid).ClassifierID;
                        if (atr.ClassifierID == 0)
                        {
                            string texte = "Error in recoverClassifierID : the original attribute " + el.Name + "." + atr.Name;
                            texte = texte + " has an invalid classifier";
                            //  MessageBox.Show(texte);
                            // GlobalIBOCopy.Errorspresent = true;
                            wlog("recoverClassifierID", texte);
                        }
                    }
                    else
                    {
                        string texte = "recoverClassifierID : the attribute " + atr.Name + " has no IBOGUID";
                        MessageBox.Show(texte);
                        wlog("recoverClassifierID", texte);
                    }

                }
                atr.Update();
            }
            catch (Exception e)
            {
                string texte = "recoverClassifierID " + e.Message;
                MessageBox.Show(texte);
                wlog("recoverClassifierID", texte);
            }
            return atr.ClassifierID;
        }
        /// <summary>
        /// recupere l'identifierID d'un attribut soit celui du profil
        ///                                       soit celui d'origine
        ///                                       el le met dans un package domain au cas ou celui-ci existe
        ///                                        (utilise ou cree)
        /// </summary>
        /// <param name="atr"></param>
        /// <param name="profdatatypes"></param>
        /// <param name="hasDomain"></param>    true if there is a domain for datatypes of all profiles
        /// <returns></returns>
        public int recoverClassifierID(EA.Repository repo, EA.Element el, EA.Attribute atr, Dictionary<string, long> profdatatypes, EA.Package domainpack)
        {
            XMLP.AddXmlLog("utilitaires", String.Format("recoverClassifierID el={0} atr={1}", el.Name, atr.Name));
            string prov = atr.Name;
            // if(prov=="crsUrn")
            // {
            //     int a = 1;
            // }
            try
            {
                // we suppose that el is not an enumeration and has been filterd before calling this method
                /*    
                if (
                    (el.StereotypeEx.Contains("enumeration"))
                    ||
                    (el.StereotypeEx.Contains("Enumeration"))
                    ||
                    (el.MetaType == "Enumeration")
                    ||
                    (el.Type == "Enumeration")
                    )
                    return 0;
                    */
                if (profdatatypes.ContainsKey(atr.Type))  // pointe sur lov=cal ou domain
                {
                    atr.ClassifierID = (int)profdatatypes[atr.Type];
                }
                else
                {


                    string IBOguid = getAtrParentGuid(atr);//ManageIBOUtilitaires.getAttributeIBOGuid(atr);
                    if (IBOguid != "")
                    {
                        atr.ClassifierID = repo.GetAttributeByGuid(IBOguid).ClassifierID;
                        if (atr.ClassifierID == 0)
                        {
                            string texte = "Error in recoverClassifierID : the original attribute " + el.Name + "." + atr.Name;
                            texte = texte + " has an invalid classifier";

                            throw (new Exception(texte));
                        }
                        if (domainpack != null)// we must copy the classifier from cim o the domain
                        {
                            EA.Element datatype = repo.GetElementByID((int)atr.ClassifierID);
                            EA.Element newelement = copyElement(datatype, domainpack);
                            atr.ClassifierID = newelement.ElementID;
                        }
                    }
                    else
                    {
                        string texte = "recoverClassifierID : the attribute " + atr.Name + " has no IBOGUID";
                        throw (new Exception(texte));

                    }

                }


                atr.Update();
            }
            catch (Exception e)
            {
                string texte = "recoverClassifierID " + e.Message;
                MessageBox.Show(texte);
                wlog("recoverClassifierID", texte);
            }
            return atr.ClassifierID;
        }

        public void replacediagobj(EA.Diagram diag, EA.DiagramObject diagobject, string elemname, long elemid, List<short> diagotodelete)
        {
            EA.DiagramObject newdiagobject = (EA.DiagramObject)diag.DiagramObjects.AddNew(elemname, "element");
            //newdiagobject.Update();

            newdiagobject.ElementID = (int)elemid;
            newdiagobject.left = diagobject.left;
            newdiagobject.right = diagobject.right;
            newdiagobject.top = diagobject.top;
            newdiagobject.bottom = diagobject.bottom;
            string texte = "Dimension of object: l=" + newdiagobject.left + " r=" + newdiagobject.right + " t=" + newdiagobject.top + " b=" + newdiagobject.bottom;
            //reportlog.WriteLine(texte);
            newdiagobject.Sequence = diagobject.Sequence;
            newdiagobject.Style = diagobject.Style;
            //"BCol=16577251;BFol=9342520;LCol=9342520;LWth=1;" ;         

            newdiagobject.Update();
            diag.DiagramObjects.Refresh();
            for (short i = 0; i < diag.DiagramObjects.Count; i++)
            {
                if (((EA.DiagramObject)diag.DiagramObjects.GetAt(i)).ElementID == diagobject.ElementID)
                {
                    if (!diagotodelete.Contains(i)) diagotodelete.Add(i);
                    break;
                }
            }

        }
        /// <summary>
        /// recupere le guid de la connexion sur laquelle on est basee 
        /// a condition que l'element otherend soit dans le profil
        /// sinon on enlève cette connexion du modèle
        /// au passage ceci eliminera les "IsBasedOn" parasites donc les "dependency" sont importantes.
        /// </summary>
        /// <param name="el"></param>
        /// <param name="con"></param>
        /// <returns>soit "ok" , soit "" si la connexion est eliminee</returns>
        public EA.Element SetConGuid(EA.Repository repo, EA.Package packenvelop, EA.Element el, EA.Connector con)
        {
            EA.Element res = null;
            ArrayList otherend;
            EA.Element thisendparent;
            EA.Element theotherendparent;
            try
            {

                string prov = con.ConnectorID.ToString();
                otherend = getOtherend(el, con);
                EA.Element theotherend = (EA.Element)otherend[0];
                short sens = (short)otherend[1];
                string role = (string)otherend[2];
                string texte = "";

                if (!IsInProfile(repo, theotherend, packenvelop)) return theotherend; // la classe externe est à rapatrier
                string pname = repo.GetPackageByID((int)theotherend.PackageID).Name;
                if (pname == packenvelop.Name) // on enleve le prefix
                {
                    pname = pname.Remove(0, UCD.ProfileNameSuffix.Length);                 //constantes.ProfileNameSuffix.Length);
                }
                theotherendparent = (GlobalIBOCopy.IBOelements[pname])[theotherend.Name];
                pname = repo.GetPackageByID((int)el.PackageID).Name;
                if (pname == packenvelop.Name) // on enleve le prefix
                {
                    pname = pname.Remove(0, UCD.ProfileNameSuffix.Length);
                }
                thisendparent = (GlobalIBOCopy.IBOelements[pname])[el.Name];
                foreach (EA.Connector conex in thisendparent.Connectors)
                {

                    ArrayList arotherend = getOtherend(thisendparent, conex);
                    /******************************************/
                    texte = texte + " arotherend " + repo.GetPackageByID(((EA.Element)arotherend[0]).PackageID).Name + "::" + ((EA.Element)arotherend[0]).Name;
                    texte = texte + " role: " + (string)arotherend[2];
                    //  wlog("setConGuid",texte);
                    /*************************************************/
                    if ((((EA.Element)arotherend[0]).ElementID == theotherendparent.ElementID)
                    && (((short)arotherend[1]) == sens)
                        && (((string)arotherend[2]) == role))
                    {
                        //on a trouve le parent
                        setParentGuid(con, conex);
                    }

                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(" dans SetConGuid " +  e.Message);
                wlog("setConGuid", " dans SetConGuid " + e.Message);
            }
            return res;
        }
        //--------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        /// <param name="con"></param>
        /// <returns>arrayList</returns> arraylist[0]=theotheren_element (EA.Element)
        ///                              arraylist[1]=TARGET ou SOURCE    (short)
        ///                              arraylist[2]= role                (string)
        public ArrayList getOtherend(EA.Element el, EA.Connector con)
        {
            string prov = con.ConnectorID.ToString();
            string texte;
            ArrayList res = new ArrayList();
            if (con.ClientID == el.ElementID)
            { // theother end is target
                try
                {
                    res.Add(repo.GetElementByID((int)con.SupplierID));
                    res.Add(UCD.TARGET);
                    res.Add(con.SupplierEnd.Role);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                try
                {
                    res.Add(repo.GetElementByID(con.ClientID));
                    res.Add(UCD.SOURCE);
                    res.Add(con.ClientEnd.Role);
                }
                catch (Exception e)
                {
                    texte = " Probleme dans getTheotherend " + el.Name + " con.stereotype=" + con.Stereotype + " " + e.Message;
                    wlog("getOtherEnd", texte);
                }
            }
            return res;
        }

        //----------------------------
        public List<EA.Element> getAllElementsByType(EA.Package package, List<EA.Element> resultList, List<string> types)
        {
            string texte = "";
            string prov = package.Name;
            if (((package == null) || types == null) || (resultList == null))
            {
                texte = "Error pb in parameters call ";
                MessageBox.Show(texte);
                wlog("getAllElementByType", texte);
                return resultList;
            }

            foreach (EA.Element e in package.Elements)
            {
                prov = e.Name;
                bool ok = true;
                if (types.Count > 0)
                { // not all
                    foreach (string s in types)
                    {
                        ok = false;
                        if (e.Type == s)
                        {
                            ok = true;
                            break;
                        }
                    }
                }
                if (ok)
                {
                    resultList.Add(e);
                }

            }

            foreach (EA.Package p in package.Packages)
            {
                getAllElementsByType(p, resultList, types);
            }
            return resultList;
        }
        //--------------------------------------------------------------------------
        /// <summary>
        /// met la tag value GUIDBasedON
        /// </summary>
        /// <param name="con"></param>
        /// <param name="conex"></param>
        void setParentGuid(EA.Connector con, EA.Connector conex)
        {
            string IBOguid = conex.ConnectorGUID;
            EA.ConnectorTag contag = null;
            try
            {
                foreach (EA.ConnectorTag ctag in con.TaggedValues)
                {
                    if (ctag.Name == UCD.GetIBOTagValue())
                    {
                        contag = ctag;
                        contag.Value = IBOguid;
                        break;
                    }
                }
                if (contag == null)
                {
                    contag = (EA.ConnectorTag)con.TaggedValues.AddNew(UCD.GetIBOTagValue(), IBOguid);             //constantes.GetIBOTagValue(), IBOguid);
                }

                contag.Update();
                con.TaggedValues.Refresh();
                con.Update();

                /**********   pour test ***********************/
                string texte = "creation tag  GUID for connector ID " + con.ConnectorID.ToString() + "between" + GlobalIBOCopy.repository.GetElementByID(con.ClientID).Name + " and " + GlobalIBOCopy.repository.GetElementByID(con.SupplierID).Name;
                // wlog("setParentGuid",texte);
                /**********************************************/
            }
            catch (Exception e)
            {
                string texte = "error creation tag  GUID for connector ID " + con.ConnectorID.ToString() + "between" + GlobalIBOCopy.repository.GetElementByID(con.ClientID).Name + " and " + GlobalIBOCopy.repository.GetElementByID(con.SupplierID).Name;
                wlog("setParentGuid", texte);
                texte = e.Message;
                wlog("setParentGuid", texte);
                MessageBox.Show(texte);
            }
        }
        //--------------------
        public void copyConEnd(EA.ConnectorEnd conend, EA.ConnectorEnd newconend)
        {
            newconend.Aggregation = conend.Aggregation;
            newconend.Cardinality = conend.Cardinality;
            newconend.Constraint = conend.Constraint;
            newconend.Containment = conend.Containment;
            newconend.Derived = conend.Derived;
            newconend.IsChangeable = conend.IsChangeable;
            newconend.IsNavigable = conend.IsNavigable;
            newconend.Navigable = conend.Navigable;
            newconend.Role = conend.Role;
            newconend.RoleNote = conend.RoleNote;
            newconend.RoleType = conend.RoleType;
            newconend.Stereotype = conend.Stereotype;
            newconend.StereotypeEx = conend.StereotypeEx;
            newconend.Visibility = conend.Visibility;
            newconend.Update();
            string prov = newconend.GetLastError();
            if (prov != "")
            {
                MessageBox.Show("Some error in copying the end role" + newconend.Role + "errormessage=" + prov);
            }


            EA.RoleTag newcontag = null;
            foreach (EA.RoleTag contag in conend.TaggedValues)
            {
                newcontag = (EA.RoleTag)newconend.TaggedValues.AddNew(contag.Tag, contag.Value);

                newcontag.BaseClass = contag.BaseClass;
                newcontag.Update();

            }

            newconend.TaggedValues.Refresh();
        }
        /// <summary>
        /// for each attribute look if the classsifer
        ///    - is local
        ///    -  in domain
        ///    - not in domain then put it there or create it
        ///   
        /// </summary>
        /// <param name="elt"></param>
        /// <param name="domainpack"></param>
        /// <param name="datatypeList"></param>
        public long recoverDatatype(EA.Attribute at, EA.Package domainpack, List<long> datatypeIDList)
        {
            long ret = 0;
            try
            {


                if (!datatypeIDList.Contains(at.ClassifierID))
                // the classifier is not in the domain : it must be copied to it
                {
                    EA.Element classifier = Main.Repo.GetElementByID((int)at.ClassifierID);
                    EA.Element newclassifier = copyElement(classifier, domainpack);
                    at.ClassifierID = newclassifier.ElementID;
                    at.Update();
                }

            }
            catch (Exception ee)
            {
                wlog("recoverDatatype", "pb for" + at.Name + "  " + ee.Message);
            }
            return ret;
        }

        /// <summary>
        /// le package enveloppe est le premier package
        /// qui contient le ackage appelé et qui n'est pas IsBaseOn d'un autre package (en tant que client)
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="package"></param>
        EA.Package getPackageEnveloppe(EA.Repository repos, EA.Package package)
        {
            string prov = package.Name;// pour test
            EA.Package packenveloppe = null;
            EA.Package possiblepack = package;
            while (packenveloppe == null)
            {
                long packid = possiblepack.ParentID;
                if (packid == 0)
                {
                    packenveloppe = possiblepack;
                    break;
                }
                possiblepack = repos.GetPackageByID((int)packid);
                if (!HasIBOPackage(possiblepack.PackageID))
                {
                    packenveloppe = possiblepack;
                    break;
                }

            }
            return packenveloppe;
        }
        /// <summary>
        /// return the package to where an element of the cim  must be copied
        ///  -if el is not in a subpackage the targetedPackage is kept
        ///  - if el is in a subpackage a similar one is used or created in targetedPackage
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="el"></param>
        /// <param name="targetedPackage"></param>
        /// <param name="le profilepack eventuel"></param>
        /// <returns></returns>
        public EA.Package getWhereToBeCopied(EA.Repository repo, EA.Element el, EA.Package targetedPackage, EA.Package profilepack)
        {
            EA.Package res = targetedPackage;
            // il y a 2 cas de figures selon l'existence ou non d'un package du profile enveloppe

            try
            {
                EA.Package elpack = repo.GetPackageByID((int)el.PackageID);
                EA.Package cimparentpack = repo.GetPackageByID((int)elpack.PackageID);
                EA.Package subpack = null;
                if (!(cimparentpack.Name == "TC57CIM"))
                {
                    // we must find the sub package or create it
                    bool ok = false;

                    //foreach(EA.Package pa in targetedPackage.Packages)
                    foreach (EA.Package pa in profilepack.Packages)
                    {
                        if (cimparentpack.Name == pa.Name)
                        {
                            subpack = pa;
                            ok = true;
                        }
                    }
                    if (!ok) //subpak doit etre cree 
                    {
                        subpack = (EA.Package)targetedPackage.Packages.AddNew(cimparentpack.Name, "Package");
                        subpack.Update();
                        targetedPackage.Packages.Refresh();
                        targetedPackage.Update();
                    }
                    res = subpack;
                }

            }
            catch (Exception ee)
            {
                string texte = " Pb in getWhereToBeCopied ";
                throw (new Exception(texte + ee.Message));
            }
            return res;
        }
        /// <summary>
        /// get all the CIMPackages
        /// pack is whichever package
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="dicCimPacksByName"></param>
        public void getAllCimPackages(EA.Package pack,Dictionary<string,EA.Package> dicCimPacksByName)
        {
            EA.Package modelpack = GetModelPackage(pack, repo);
            List<EA.Package> allpackageslist = new List<EA.Package>();
            getAllPackages(modelpack, allpackageslist);
            EA.Package envelop = null;
            Dictionary<string, string> dicdomains = new Dictionary<string, string>(); // pour test guid/parentname
            foreach(EA.Package pa in allpackageslist)
            {
                envelop = getPackageEnveloppe(repo, pa);
                if(!HasIBOPackage(envelop.PackageID))
                    {
                    if(pa.Name.Contains("Domain")) // pour test
                    {
                        string pguid = pa.PackageGUID;
                        if(pa.ParentID != 0)
                        {
                            EA.Package pp = repo.GetPackageByID(pa.ParentID);
                            if (!dicdomains.ContainsKey(pguid)) dicdomains[pguid] = pp.Name;
                        }
                        
                    }
                    if (!dicCimPacksByName.ContainsKey(pa.Name)) dicCimPacksByName[pa.Name] = pa;
                }
            }

        }
        /// <summary>
        /// get all the CIMPackages
        /// but not extensions package if strict=true
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="dicCimPacksByName"></param>
        /// /// <param name="strict"></param>
        public void getAllCimPackages(EA.Package pack, Dictionary<string, EA.Package> dicCimPacksByName,bool strict)
        {
            EA.Package modelpack = GetModelPackage(pack, repo);
            List<EA.Package> allpackageslist = new List<EA.Package>();
            if (strict)
            {
                EA.Package cimpack = null;
                foreach (EA.Package pa in modelpack.Packages)
                {
                    if (pa.Name =="TC57CIM")
                    {
                        cimpack = pa;
                        break;
                    }
                }
                if (cimpack == null) cimpack = modelpack;
                getAllPackages(cimpack, allpackageslist);
            }
            else
            {
                getAllPackages(modelpack, allpackageslist);
            }
            EA.Package envelop = null;
            Dictionary<string, string> dicdomains = new Dictionary<string, string>(); // pour test guid/parentname
            foreach (EA.Package pa in allpackageslist)
            {
                envelop = getPackageEnveloppe(repo, pa);
                if (!HasIBOPackage(envelop.PackageID))
                {
                    if (pa.Name.Contains("Domain")) // pour test
                    {
                        string pguid = pa.PackageGUID;
                        if (pa.ParentID != 0)
                        {
                            EA.Package pp = repo.GetPackageByID(pa.ParentID);
                            if (!dicdomains.ContainsKey(pguid)) dicdomains[pguid] = pp.Name;
                        }

                    }
                    if (!dicCimPacksByName.ContainsKey(pa.Name)) dicCimPacksByName[pa.Name] = pa;
                }
            }

        }
        //-------------------------------
    }
}
