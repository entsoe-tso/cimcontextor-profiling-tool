using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
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
    /// each methode of this class does a specific task to help 
    /// improve performance in creating a pofile
    /// </summary>
   public  class Utilities
    {
       EA.Repository repo = null;
        XMLParser XMLP = null;
        Utilitaires util = null;
        ConstantDefinition CD = new ConstantDefinition();
        EA.Package domain = null; // package for datatypes
        


        public Utilities(EA.Repository repos) // the constructor
        {
            repo = repos; //does nothing 
            XMLP = new XMLParser(repos);
            util = new Utilitaires(repo);
        }
       
        /// <summary>
        /// replace all atribute's type simple-Float by the pimitive Float
        /// in the profile profile
        /// </summary>
        /// <param name="profile"></param>
      public void replaceSimpleByFloat(EA.Package profile)
        {
            // we  collect all the elements (not the enumerations) of the givent profile
            List<EA.Element> profelems = new List<EA.Element>();
            util.GetAllElements(profile, profelems);
            EA.Element flottant=null;
            foreach(EA.Element el in profelems)
            {
                if((el.Name=="") || (el.Type=="Enumeration") || (el.MetaType=="Enumeration") || (el.StereotypeEx.Contains("enumeration") )|| (el.StereotypeEx.Contains("Enumeration")))
                {
                    continue;
                }
                foreach(EA.Attribute at in el.Attributes)
                {
                    if(at.ClassifierID != 0)
                    {
                        EA.Element classifier = (EA.Element)repo.GetElementByID((int)at.ClassifierID);
                        if(classifier.Name=="Simple_Float")
                        {
                            if (flottant == null)
                            {
                                EA.Package domain = (EA.Package)repo.GetPackageByID((int)classifier.PackageID);
                                foreach (EA.Element elt in domain.Elements)
                                {
                                    if (elt.Name == "Float")
                                    {
                                        flottant = elt;
                                        break;
                                    }
                                }
                                if(flottant==null)
                                {
                                    MessageBox.Show(" Error Float primitive not found in the same directory as Simple_Float");
                                    return;
                                }
                            }
                            
                            at.ClassifierID = flottant.ElementID;
                            at.Type = flottant.Name;
                            at.Update();
                        }
                    }
                }
                el.Update();
            }

        }
      /// <summary>
      /// This method looks if there is a local Domain package for datatypes
      /// by convention this package is named  "DOMAIN"+"NameOfProfile's Package"
      /// if there is not such a local package , it is created with a diagram of the same name
      /// then we explore all the elements and their attributes (not enumerations).
      /// for each attribute: if the datatype is local we do nothing ,  if not we copy the identifier locally and reattribute the identifier
      ///  if the attribute has no identifier we prepare a list which be reported at the end of the utility
      /// </summary>
      /// <param name="profile"></param>
      /// <returns></returns>
    public  Dictionary<int,List<string>>  LocalizeDataTypes(EA.Package profile)
        {

            // string[,] ret = null;    // ret[i,0]= "B" | "G" | "C"  bad,good,changed
            // ret[i,1]= el.at
            // ret[i,2]= classifier
            Dictionary<int, List<string>> ret = new Dictionary<int, List<string>>();
            try
            {
                // we first collect all the elements of the givent profile
                // we first check that it is a plausible profile (it is basedon another package
                bool maybeaprofile = false;
            foreach(EA.Connector con in profile.Connectors)
                {
                    if((con.Type=="Dependency") && (con.ClientID.Equals(profile.Element.ElementID)))
                    {
                        maybeaprofile = true; // it seems a plausibleprofile
                        break;
                    }
                }
              if(!maybeaprofile) {
                    util.wlog("Error"," the Package " + profile.Name +" does not seem to be a profile");
                    MessageBox.Show(" the Package " + profile.Name + " does not seem to be a profile");
                    return ret;
                        }
                // we  collect all the elements (not the enumerations) of the givent profile
                List<EA.Element> profelems = new List<EA.Element>();
                    util.GetAllElements(profile, profelems);
                int nbelements = profelems.Count; // nuber of elements in the profile
                for(int jj=0; jj < nbelements;jj++)
                {
                    ret[jj] = new List<string>();
                }
                  

                // we then check if there is a local package for datatypes
                List<EA.Package> profpacks = new List<EA.Package>();
                Utilitaires.getAllPackages(profile, profpacks);
                bool domainexists = false;
               
                DialTree dialtree = new DialTree(repo, profile,true);
                dialtree.treeLabel = "Select Profile Container Package";
                dialtree.inputLabel = " name of your local DataTypes Package \n\r (if you have not selected it , it may be created ) ";
                dialtree.profilename = "";
                dialtree.ShowDialog();
                if (dialtree.profilename == "!!##") { dialtree.Dispose(); return new Dictionary<int, List<string>>(); }

                string domainname = "";

                if(Utilitaires.dicSelectedPackage.Count>0)
                {
                   
                    foreach(string name in Utilitaires.dicSelectedPackage.Keys)
                    {
                        domainname = name;
                        domain = Utilitaires.dicSelectedPackage[domainname];
                        domainexists = true;
                        break;
                    }
                    
                }
                else
                {
                    if(dialtree.profilename != "")
                    {
                        domainname = dialtree.profilename;
                    }
                    else
                    {
                        domainname = "Domain" + profile.Name;
                    }
                }
                dialtree.Dispose(); // ABA20230401
                if (domain == null)
                {
                    foreach (EA.Package pa in profpacks)
                    {
                        //if (pa.Name == "Domain" + profile.Name)
                        if (pa.Name == domainname)
                        {
                            domainexists = true;
                            domain = pa;
                            break;
                        }
                    }
                }
                if(!domainexists)
                {
                    // must create a package
                    EA.Package pack = (EA.Package)profile.Packages.AddNew(domainname, "");
                    profile.Packages.Refresh();
                    pack.Update();
                    //must create a diagram
                    EA.Diagram diag = (EA.Diagram)pack.Diagrams.AddNew(domainname, "Class");
                    pack.Packages.Refresh();
                    diag.Update();
                    profile.Update();
                    domain  = pack;
                }

                //  for all elements wich are not an enumeration we explore the attributes
                int i = -1;
                foreach(EA.Element el in profelems)
                {
                    
                    string prov = el.Name;
                    if (
                        (!el.StereotypeEx.Contains("enumeration"))
                        &&
                         (!el.StereotypeEx.Contains("Enumeration"))
                        &&
                         (!(el.Type == "Enumeration"))
                        &&
                        (!(el.MetaType == "Enumeration"))
                        )
                    {
                        i = i + 1;
                        foreach (EA.Attribute at in el.Attributes)
                        {
                            if (at.ClassifierID == 0)
                            {// dont refer any datatype
                                ret[i].Add("B");
                                ret[i].Add(el.Name + "." + at.Name);
                                ret[i].Add("0");
                                ret[i].Add(";");
                            }
                            else
                            {
                                EA.Element classifier = repo.GetElementByID(at.ClassifierID);
                                if (classifier.PackageID == domain.PackageID)
                                {
                                    ret[i].Add("G");
                                    ret[i].Add(el.Name + "." + at.Name);
                                    ret[i].Add(classifier.Name);
                                    ret[i].Add(";");
                                }
                                else
                                {
                                    EA.Element newclassifier = util.copyElement(classifier, domain);
                                    if (newclassifier == null) //no good
                                    {
                                        ret[i].Add("B");
                                        ret[i].Add(el.Name + "." + at.Name);
                                        ret[i].Add("0");
                                        ret[i].Add(";");
                                    }
                                    if (newclassifier.ElementID != classifier.ElementID)
                                    {
                                        at.ClassifierID = newclassifier.ElementID;
                                        if (!at.Update()) throw new Exception(at.GetLastError());
                                    }
                                    ret[i].Add("C");
                                    ret[i].Add(el.Name + "." + at.Name);
                                    ret[i].Add(classifier.Name);
                                    ret[i].Add(";");
                                }
                            }
                        }
                    }
                    
                }
                // the enumerations must be taken into account
                // we discard the already local datatypes transfered to domain
                foreach (EA.Element elem in profelems)
                {
                    string prov = elem.Name;
                    if (
                        elem.StereotypeEx.Contains(CD.GetCompoundStereotype())
                        ||
                         elem.StereotypeEx.Contains(CD.GetDatatypeStereotype())
                        ||
                         elem.StereotypeEx.Contains(CD.GetEnumStereotype())
                        ||
                         elem.StereotypeEx.Contains("Enumeration")
                        ||
                         (elem.MetaType == "Enumeration")
                        ||
                        (elem.Type == "Enumeration")
                        )
                    {
                        bool isindomain = false;
                        // we check if that datatype exits in domain
                        foreach (EA.Element datatype in domain.Elements)
                        {
                            if (datatype.Name == elem.Name)
                            {
                                isindomain = true;
                                break;
                            }     
                        }
                        /*
                        if (
                         elem.StereotypeEx.Contains("Enumeration")
                        ||
                         (elem.MetaType == "Enumeration")
                        ||
                        (elem.Type == "Enumeration")
                        )
                        {// it is an enumeration all attributes are going to be changed
                            foreach (EA.Attribute at in elem.Attributes)
                            {
                                if (isindomain)
                                {
                                    ret[i].Add("G");
                                }
                                else
                                {
                                    ret[i].Add("C");
                                }
                                ret[i].Add(elem.Name + "." + at.Name);
                                ret[i].Add("0");
                                ret[i].Add(";");
                            }
                            i = i + 1;
                        }
                        */
                        if (!isindomain) // and if not we tranfer it
                        {
                            EA.Element resul = util.copyElement(elem, domain);
                            if (resul == null) throw new Exception(" pb in copying " + elem.Name + "in domain");
                            

                            }

                    EA.Package package = repo.GetPackageByID((int)elem.PackageID);
                    for (short j = 0; j < package.Elements.Count; j++)
                        {
                            EA.Element elt = (EA.Element)package.Elements.GetAt(j);
                            if (elt.ElementID == elem.ElementID)
                            {
                                package.Elements.DeleteAt(j, true);
                                package.Elements.Refresh();
                                break;
                            }
                        }
                    }
                    
                }

                    string Path = XMLP.Path;
                    TextWriter crtxt =new  StreamWriter(Path + "\\LocalizedDatatypes_" + profile.Name + ".txt");
                    for(int j=0;j<ret.Keys.Count;j++)
                    {
                    
                        string ligne = "";
                    foreach (string item in ret[j])
                    {
                        if (item == ";")
                        { 
                        continue;
                        }
                        if(ligne=="")
                        {
                            ligne = item;
                        }
                        else
                        {
                            ligne = ligne + "," + item;
                        }
     
                    }
                        crtxt.WriteLine(ligne);
                    }

                // MessageBox.Show(" list of Chaged attributes \r\n" + changed);
                crtxt.Flush();
                crtxt.Close();
                MessageBox.Show(" List of localized datatypes in " + Path + "\\LocalizedDatatypes_" + profile.Name + ".txt");
                

            }
            catch (Exception e)
            {
                MessageBox.Show(" Error in localizedatatypes  " + e.Message );
                throw new Exception(" Error in localizedatatypes  " + e.Message);
            }
            return ret;
        }
    }
}
