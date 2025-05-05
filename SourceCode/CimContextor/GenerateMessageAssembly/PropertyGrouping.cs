using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
//using System.Linq;
using System.Text;
using EA;
using CimContextor.GenerateMessageAssembly;
using CimContextor.utilitaires;
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
namespace CimContextor.GenerateMessageAssembly
{
    /// <summary>
    /// This is a way to decrease the amount of classes in the final message
    /// document according to ccts methodology (MBIE)
    /// We follow here the decisions taken by European WG16 TF
    /// the chosen  algorithm is the following:
    /// the operator select a package which must be included in a Profile package
    /// the parent package  is exported , and reimported in the new selected package
    /// then for each classes in the profile
    /// for each connexion
    /// if the connexion is an agreggation or an association 
    /// with a shared end from the class
    /// if the other end role is not empty  and its cardinality is [0.1] or [1..1]
    /// then all attributes of the other end class are incorporated in the class
    /// keeping their type and the cardinality of the association
    /// each new attribute at from class OtherEnd is named in the class
    /// at.OtherEnd
    /// if fact these programs manage 2 cases
    /// case 1  the assembly directory (empty to be selected) is included in the profilecontextual package
    /// case 2 the package profile is an envelop comprising at most in 1 level :
    ///                       - the contextual model package
    ///                       - the assembly model package (empty to be selected)
    ///              
    /// case 3 the program operate from the contextual package and greate an assembly package
    /// which name is "Assembly contextualpackage.name"
    /// </summary>
    class PropertyGrouping
    {
        private const int LAYOUT_DIRECTION_DOWN = 131072;
        private EA.Repository repo;
        public  ConstantDefinition CD;
        XMLParser XMLP = null;
        Utilitaires ut;
         static ConstantDefinition cons = new ConstantDefinition();
        List<string> stereos = new List<string>(){cons.GetEnumStereotype(),cons.GetDatatypeStereotype(),
                cons.GetCompoundStereotype(),cons.GetPrimitiveStereotype()};
        List<long> dejatraite;// to remember classes already treated
                              // that means that all connectors of the class has been treated
       Dictionary<long,bool> ClassToBeDeleted; // a class can be deleted when it is treated and and
                                               // no non deletable "out"  connector has been encountered
          Dictionary<string,Dictionary<string,long>> dicidentifiers; // <class.name,<attribute.name,attribute.classifierid>
          List<long> ghostmbies = new List<long>(); // liste des classes abie des classes mbie regroupees et detruites
          string oldassemblydescription = ""; // save old assembly description
        static EA.Project project;
        public static long ConexesID;
        //int multiplicateur = 100; // pour incrementer le repere des rangs lorsqu'on regroupe
        public struct Connexion
        {
            public long ConexID;
            public long ConnectorID;
            public EA.Element Source_element;
            public EA.Element Target_element;
            public EA.ConnectorEnd Source;
            public int SourceAggregation;
            public EA.ConnectorEnd Target;
            public int TargetAggregation;
          
            
            public Connexion(EA.Repository repo, EA.Connector con)
            {

                //ConstDefinition constantes = new ConstDefinition(); // constants
                ConexesID++;
                ConexID = ConexesID;
                ConnectorID = con.ConnectorID;
                Source_element = repo.GetElementByID(con.ClientID);
                Target_element = repo.GetElementByID(con.SupplierID);
                Source = con.ClientEnd;
                SourceAggregation = (con.ClientEnd).Aggregation;
                Target = con.SupplierEnd;
                TargetAggregation = (con.SupplierEnd).Aggregation;
            }
            public Connexion(EA.Repository repo, Connexion conex)
            {
                ConexesID++;
                ConexID = ConexesID;
                ConnectorID = conex.ConnectorID;
                Source_element = conex.Source_element;
                Target_element = conex.Target_element;
                Source = conex.Source;
                SourceAggregation = conex.SourceAggregation;
                Target = conex.Target;
                TargetAggregation = conex.TargetAggregation;
            }
        }
        public PropertyGrouping(EA.Repository repo) //the constructor
        {
            this.repo = repo;
            XMLP = new XMLParser(repo);
            this.ut = new Utilitaires(repo);
            this.CD = new ConstantDefinition();
            project = repo.GetProjectInterface();
         
            ClassToBeDeleted = new Dictionary<long,bool>();
	    dicidentifiers = new  Dictionary<string,Dictionary<string,long>>(); // pour chaque classe repr�e par son nom on se rememore le ClassifierID de chaque attribut

	    

            EA.Package package = repo.GetTreeSelectedPackage();
            writelog("Begining of propertygrouping of ..." + package.Name);
            int parentID = package.ParentID;
            EA.Package parentpack = repo.GetPackageByID(package.ParentID);
            repo.SaveAllDiagrams(); // sauvegarde tous les diagrammes au cas ou 
            EA.Package packtoregroup = null;
            
                packtoregroup = SelectPackage(package);
                //MessageBox.Show("selected package to regroup: " + packtoregroup.Name)      
                if (packtoregroup == null) return;
          
            string packagename = package.Name;
     
            //packtoregroupID = package.PackageID;
                   oldassemblydescription = package.Notes;
                     exportparentpack(repo, package,packtoregroup);// package == assembly
                     
                    importpack(repo, parentpack, packagename,packtoregroup); //parentpack = package parent de assembly

                    parentpack = repo.GetPackageByID(parentID);
    

            foreach (EA.Package pa in  parentpack.Packages)
            {
                
               
                if (pa.Name == packagename)
                {
                    package = pa;
                    break;
                }
                
            }
            Utilitaires util = new Utilitaires(repo);
            util.majGuids(repo, package,packtoregroup);

                //treat each class

            // first pass
            dejatraite = new List<long>();
                foreach (EA.Element el in package.Elements)
                {

                   if (!dejatraite.Contains(el.ElementID)) {
			treatclass(el);
			}
                }
                // second pass pass
                dejatraite = new List<long>();
                foreach (EA.Element el in package.Elements)
                {

                    if (!dejatraite.Contains(el.ElementID))
                    {
                        treatclass2(el);
                    }
                }

                //delete all the necessary  classes
                 EA.Element elt;
                 foreach (KeyValuePair<long, bool> kvp in ClassToBeDeleted)
                 {
                     if (kvp.Value)
                     {
                         for (short i = 0; i < package.Elements.Count; i++)
                         {
                             elt = (EA.Element)package.Elements.GetAt(i);
                             //MessageBox.Show("is class to be deleted ?=" + elt.Name + " | ClassToBedeleted[ID]=" + ClassToBeDeleted[elt.ElementID].ToString()); //pour test
                             if (elt.ElementID == kvp.Key)
                             {
                                 //MessageBox.Show("yes"); //pour test
                                 package.Elements.Delete(i);
                                 package.Elements.Refresh();
                                 break;
                             }
                         }
                     }
                 }
                 IncludeBTinAPackage(package);
                
                 EA.Diagram diag = (EA.Diagram)package.Diagrams.GetAt(0);
            //package.Diagrams.GetByName
                 bool res = project.LayoutDiagramEx(diag.DiagramGUID, LAYOUT_DIRECTION_DOWN, 4, 20, 20, false);
                 repo.ReloadDiagram(diag.DiagramID);
                 foreach (EA.DiagramLink dl in diag.DiagramLinks)
                 {
                     EA.Connector con = repo.GetConnectorByID((int)dl.ConnectorID);
                     //string prov1 = con.SupplierEnd.Role + "|" + con.ClientEnd.Role;
                     string style = "";
                     con.RouteStyle=CD.LineStyleAutorouting;


                     if (con.Type != "Dependency")
                     {
                         //dl.IsHidden = true;

                         dl.Update();
                          
                          style = dl.Style;
                          string style1 = style;
                          if (style.Contains(CD.LineStyleCustom.ToString()))
                          {

                              dl.Style = style.Replace("=" + CD.LineStyleCustom.ToString(),"=" +  CD.LineStyleAutorouting.ToString());
                               style=dl.Style;
                          }
                         dl.Update();
                     }
                 }
                  // diag.Update();
  repo.SaveAllDiagrams();
 // creDiagramDependance(repo, package);
 // repo.SaveAllDiagrams();
  MessageBox.Show("The end");
                
        
}
        
/// <summary>
/// Add an element in a package by copying an element from another package 
/// it consists in copying everything from the original element except of the name which is given already
/// </summary>
/// <param name="elsource"></param>
/// <param name="eltarget"></param>
private void addelement(EA.Package pack, EA.Element elsource, EA.Element eltarget)
{
}
/// <summary>
/// copy an attribute from an element into a given element
/// </summary>
/// <param name="atr"></param>
/// <param name="connectorid"></param>
/// <param name="offset"></param>
/// <param name="toelt"></param>
/// <param name="fromelt"></param>
/// <param name="conend"></param>
/// <param name="mandatory"></param>
private void copyattribute(EA.Attribute atr,int conrang, EA.Element toelt, EA.Element fromelt, EA.ConnectorEnd conend,bool mandatory)
{
            
//writelog("copryattribute ... atr=" + atr.Name +", toelt=" + toelt.Name 
//  + ", fromelt=" + fromelt.Name + ", Role=" + conend.Role + ", With rolenote="+ conend.RoleNote);//pour test
short index = toelt.Attributes.Count;
string  newname=conend.Role + "." + atr.Name;
// writelog(" attribute to incorporate: " + newname); // pour test
char[] cnewname = newname.ToCharArray();
cnewname[0]=Char.ToLower(cnewname[0]);
newname = new string(cnewname);
            


EA.Attribute newatr=(EA.Attribute) toelt.Attributes.AddNew(newname, atr.Type);
newatr.Update();
//MessageBox.Show(" Error in copying an attribute " + (toelt.Attributes).GetLastError());
toelt.Update();
toelt.Attributes.Refresh();
toelt.Update();
//MessageBox.Show("count apres " + toelt.Attributes.Count.ToString());
     
newatr.Containment = atr.Containment;
newatr.UpperBound = "1";
if (mandatory && atr.LowerBound == "1")
{
 newatr.LowerBound = "1";
}
else
{
 newatr.LowerBound = "0";
}
// newatr.Notes = atr.Notes;
// l'endroit de la concatenantion am fev 2012
//newatr.Notes = conend.RoleNote + " " + atr.Notes;
//newatr.Notes = atr.Notes + '\n' + "---" + conend.RoleNote;
int ind = atr.Notes.IndexOf("---");
string  reste = "";
string deb = atr.Notes;
if (ind >= 0)
{
reste = atr.Notes.Substring(ind);
deb = atr.Notes.Substring(0, ind);
}
if ((conend.RoleNote == "") && (reste == ""))
{
    newatr.Notes = deb;
}
else
{
    newatr.Notes = deb + "\n\r" + "--- " + conend.RoleNote + "\n\r" + reste; // +"\n\r";
}
//MessageBox.Show(" après concatenation note de l'attribut " + newatr.Notes); // pour test
newatr.Precision = atr.Precision;
newatr.IsConst = atr.IsConst;
newatr.ClassifierID = atr.ClassifierID;
newatr.Update();
newatr.Type = atr.Type;
newatr.Default = atr.Default;
newatr.StereotypeEx = atr.StereotypeEx;
newatr.Update();
//writelog(fromelt.Name +  "." + atr.Name + "| atr ClassifierID=" + atr.ClassifierID.ToString() + " newatr ClassifierID =" +newatr.ClassifierID.ToString()); //pour test
copyATRCollection(atr.Constraints, newatr.Constraints, "constr",conrang);
copyATRCollection(atr.TaggedValues, newatr.TaggedValues, "tag",conrang);
// copyATRCollection(atr.TaggedValuesEx, newatr.TaggedValuesEx, "tagex");
toelt.Update();
writelog(" copyattribute " + newatr.Name + " newrang=" + ut.getAtRang(newatr));
}
/// <summary>
/// if this connector is an agreggation or an association with the role of the element shared 
/// and the other end with a role non empty and a cardinality <=1
/// then the attribute of the otherend class is incorporated
/// and the descriptions are concatenated
/// </summary>
/// <param name="elementanalysed"></param>
/// <param name="con"></param>
/// <returns>  
///	"canbedeleted",  (aggregation regoroupee)
///	"cannotbedeleted",(la multiplicity n'est pas 1" ou association simple)
////	"mustbedeleted" (pas une association)
///                </returns>
/// 
private string treatconnector(EA.Element el, EA.Connector con)
{
    string prov = el.Name;
string ret = "nothingtodo";
// if ((con.Type == "Dependency") && (con.Stereotype == "IsBasedOn")) { return "dependency"; }
if (con.Type == "Association" ||
 con.Type == "Aggregation")
{
 Connexion conex = new Connexion(repo, con);

 if ((conex.SourceAggregation == 0) && (conex.TargetAggregation == 0))
 {
     //it is a simple association
     // on interdit la supression d'aucune des classes
     if (!ClassToBeDeleted.ContainsKey(conex.Target_element.ElementID))
     {
         ClassToBeDeleted.Add(conex.Target_element.ElementID, false);
     }
     else
     {
         ClassToBeDeleted[conex.Target_element.ElementID] = false;
     }
     if (!ClassToBeDeleted.ContainsKey(conex.Source_element.ElementID))
     {
         ClassToBeDeleted.Add(conex.Source_element.ElementID, false);
     }
     else
     {
         ClassToBeDeleted[conex.Source_element.ElementID] = false;
     }

     return "cannotbedeleted";
 }


 //it is a legitimate connection if one of the end is "shared"
 if (conex.Source_element.ElementID == el.ElementID)  // the otherend is target
 {
     if (conex.SourceAggregation != 1) return "nothingtodo"; // from a given element we treat only the connexion
     //which are "descending" from it 
     //{
     if (ClassToBeDeleted.ContainsKey(conex.Target_element.ElementID))
     {
         if (!ClassToBeDeleted[conex.Target_element.ElementID])
         {
             return "cannotbedeleted"; // the other class cannot be deleted
         }
     }
     if ((conex.Target.Role != "") && (conex.TargetAggregation == 0))
     {

         if ((conex.Target.Cardinality == "0..1")
             || (conex.Target.Cardinality == "1..1")
             || (conex.Target.Cardinality == "1"))
         { // this connexion is eligible for grouping

             if (!dejatraite.Contains(conex.Target_element.ElementID))
             {
                 treatclass(conex.Target_element);
             }

             if (ClassToBeDeleted[conex.Target_element.ElementID])// cet element de dictionaire a ete forcement cree dans traitclass
             {
                 ret = "canbedeleted";
             }
             else
             {
                 ret = "cannotbedeleted";
             }
         }
         else
         { // the cardinality does not allow the class to be deleted
             ret = "cannotbedeleted";
             if (!ClassToBeDeleted.ContainsKey(conex.Target_element.ElementID))
             {
                 ClassToBeDeleted.Add(conex.Target_element.ElementID, false);
             }
             else
             {
                 ClassToBeDeleted[conex.Target_element.ElementID] = false;
             }
         }

     }
     else
     {
         writelog("Error in treatconnector one of the connectiondescending  from class " + el.Name + " has an otherend with void role or a shared end");
         MessageBox.Show("Error in treatconnector one of the connectiondescending  from class " + el.Name);

     }



 }
 else
 { //the other end is source

     if (conex.TargetAggregation != 1) return "nothingtodo"; // from a given element we treat only the connexion
     //which are "descending" from it 
     //{
     if (ClassToBeDeleted.ContainsKey(conex.Source_element.ElementID))
     {
         if (!ClassToBeDeleted[conex.Source_element.ElementID])
         {
             return "cannotbedeleted"; // the other class cannot be deleted
         }
     }
     if ((conex.Source.Role != "") && (conex.SourceAggregation == 0))
     {

         if ((conex.Source.Cardinality == "0..1")
             || (conex.Source.Cardinality == "1..1")
             || (conex.Source.Cardinality == "1"))
         { // this connexion is eligible for grouping

             if (!dejatraite.Contains(conex.Source_element.ElementID))
             {
                 treatclass(conex.Source_element);
             }

             //if (ClassToBeDeleted[conex.Source_element.ElementID])// cet element de dictionaire a ete forcement cree dans traitclass // am dec 2012
             if (!ClassToBeDeleted[conex.Source_element.ElementID])// cet element de dictionaire a ete forcement cree dans traitclass
             {
                 
                 ret = "cannotbedeleted";
             }
             else
             {
                 ret = "canbedeleted";
             }
         }
         else
         { // the cardinality does not allow the class to be deleted
             ret = "cannotbedeleted";
             if (!ClassToBeDeleted.ContainsKey(conex.Source_element.ElementID))
             {
                 ClassToBeDeleted.Add(conex.Source_element.ElementID, false);
             }
             else
             {
                 ClassToBeDeleted[conex.Source_element.ElementID] = false;
             }
         }

     }
     else
     {
         writelog("Error in treatconnector one of the connectiondescending  from class " + el.Name + " has an otherend with void role or a shared end");
         MessageBox.Show("Error in treatconnector one of the connectiondescending  from class " + el.Name);

     }
 }
               
}
return ret;
}
/// <summary>
///
/// if this connector is an agreggation or an association with the role of the element shared 
/// and the other end with a role non empty and a cardinality <=1
/// then the attribute of the otherend class is incorporated
/// this second pass does the effective job and concatenates the descriptions
// while the first pass just marked the classes to know which one should or shourd not be deleted
/// </summary>
/// <param name="elementanalysed"></param>
/// <param name="con"></param>
/// <returns>  
	
///                </returns>
/// 
private int  treatconnector2(EA.Element el, EA.Connector con,int rangcourant)
{
int resul = -1;
int conrang=   rangcourant+  ut.getAtRang(con);
if ((con.Type == "Dependency") && (con.Stereotype == "IsBasedOn")) { return resul; }
if (con.Type == "Association" ||
 con.Type == "Aggregation")
{
 Connexion conex = new Connexion(repo, con);
	         
 if ((conex.SourceAggregation == 0) && (conex.TargetAggregation == 0)){//it is a simple association
     return resul; // on end must be aggregated
} 
             
ConstantDefinition CD=new ConstantDefinition();
//it is a legitimate connection if one of the end is "shared"
 if (conex.Source_element.ElementID == el.ElementID)  // the otherend is target
 {
     if (conex.SourceAggregation == 1) // from a given element we treat only the connexion
     //which are "descending" from it 
     {
         if ((conex.Target.Role != "") && (conex.TargetAggregation == 0))
         {
             if (ClassToBeDeleted.ContainsKey(conex.Target_element.ElementID)
                 && !ClassToBeDeleted[conex.Target_element.ElementID]) return resul;  // nothing to do this class cannot be deleted
             if ((conex.Target.Cardinality == "0..1")
                 || (conex.Target.Cardinality == "1..1")
                 || (conex.Target.Cardinality == "1"))
             { // this connexion is eligible for grouping
                                
                 if (!dejatraite.Contains(conex.Target_element.ElementID))
                 {
                     treatclass2(conex.Target_element);
                 }

                 resul = repo.GetElementByID((int)conex.Target_element.ElementID).Attributes.Count; // le nb d'attribut ramenés
                 foreach (EA.Attribute at in conex.Target_element.Attributes)
                 {
                     bool mandatory = false;
                     if (
                         (conex.Target.Cardinality == "1..1")
                         || (conex.Target.Cardinality == "1"))
                     {
                         mandatory = true;
                      
                     }
                                    
                     //copyattribute(at, conrang, conex.Source_element, conex.Target_element, conex.Target, mandatory);
                     copyattribute(at, conrang, el, conex.Target_element, conex.Target, mandatory);
                 }
                // on trace le lien de dependance entre la classe et la classe de regroupement
                 // de la classe target vers la classe source
                 // creation de la dependance
                 EA.Connector depcon = (EA.Connector)conex.Target_element.Connectors.AddNew("", "Dependency");
                 depcon.SupplierID = conex.Source_element.ElementID;
                 string parentguid = ut.getEltParentGuid(conex.Target_element);
                 depcon.ClientID = repo.GetElementByGuid(parentguid).ElementID;
                 if (!ghostmbies.Contains(depcon.ClientID)) { ghostmbies.Add(depcon.ClientID); };
                 depcon.Stereotype = CD.GetRegroupStereotype();
                  depcon.Update();
                                 
                               
                 (conex.Target_element.Connectors).Refresh();

                 string texte = "Creation de la Dependency <<RegroupedInto>> entre " + "<<" +
                         repo.GetElementByGuid(parentguid).Stereotype   + ">>" + conex.Target_element.Name + " et "
                          + "<<" + conex.Source_element.Stereotype + ">>" + conex.Source_element.Name;
               //writelog.Show(texte);
                 RedirectElementderegroup(conex.Target_element, conex.Source_element);
                                
             }
}	
				
         }

     }

     else
 { //the other end is source
                 
      if (conex.TargetAggregation == 1) // il s'agit bien d'une aggregation
           {
      
                 if ((conex.Source.Role != "") && (conex.SourceAggregation == 0))
				
                 {
                     if ( ClassToBeDeleted.ContainsKey(conex.Source_element.ElementID) &&
                         !ClassToBeDeleted[conex.Source_element.ElementID]) return resul;  // nothing to do this class cannot be deleted
                     if ((conex.Source.Cardinality == "0..1")
                         || (conex.Source.Cardinality == "1..1")
                         || (conex.Source.Cardinality == "1"))
                     { // this connexion is eligible for grouping
                                        
                         if (!dejatraite.Contains(conex.Source_element.ElementID))
                         {
                             treatclass2(conex.Source_element);
                         }
                         resul = repo.GetElementByID((int)conex.Source_element.ElementID).Attributes.Count; // le nb d'attribut ramenés

                         foreach (EA.Attribute at in conex.Source_element.Attributes)
                         {
                             bool mandatory = false;
                             if (
                                 (conex.Source.Cardinality == "1..1")
                                 || (conex.Source.Cardinality == "1"))
                             {
                                 mandatory = true;
                                // resul = (int)conex.Source_element.Attributes.Count;
                                              
                             }

                             //copyattribute(at, conrang,conex.Target_element, conex.Source_element, conex.Source, mandatory);
                             copyattribute(at, conrang, el, conex.Source_element, conex.Source, mandatory);
                         }
                         // on trace le lien de dependance entre la classe et la classe de regroupement
                         // de la classe source vers la classe target
                                        
                                        
                         EA.Connector depcon = (EA.Connector)conex.Source_element.Connectors.AddNew("", "Dependency");
                         depcon.SupplierID = conex.Target_element.ElementID;
                         string parentguid = ut.getEltParentGuid(conex.Source_element);
                         depcon.ClientID = repo.GetElementByGuid(parentguid).ElementID;
                         if(!ghostmbies.Contains(depcon.ClientID)){ ghostmbies.Add(depcon.ClientID);};
                         depcon.Stereotype = CD.GetRegroupStereotype();
                         depcon.Update();
                         conex.Source_element.Connectors.Refresh();
                         string texte = "Creation de la Dependency <<RegroupedInto>> entre " + "<<" +
                            repo.GetElementByGuid(parentguid).Stereotype   + ">>" + conex.Source_element.Name + " et "
                            + "<<" + conex.Target_element.Stereotype + ">>" + conex.Target_element.Name;
                         //writelog(texte);
                         RedirectElementderegroup(conex.Source_element, conex.Target_element);
                                       
                     }
                                   
             }
}

}
}
el.Update();
writelog(" treatconnector2...nb attributs incorpores for class: " + el.Name + "connecteur de rang=" +ut.getAtRang(con) +   "count=" + resul);
return resul;
 }
            
        
/// <summary>
/// export parent (cas ==1) or sibling (cas ==2) package after deleting pack
/// in the file cimcontextor_nameofpack_prov.xml
/// </summary>
/// <param name="repo"></param>
/// <param name="pack"></param> 
/// <param name="cas"></param>

void exportparentpack(EA.Repository repo, EA.Package pack,int cas)
{
string nomfichier = FileManager.GetParentDirPath() + "\\" + "cimcontextor_" + pack.Name + "_prov"; ;
System.IO.File.Delete(nomfichier);
EA.Project project = repo.GetProjectInterface();// we get project interface
EA.Package parentpack = repo.GetPackageByID(pack.ParentID);
EA.Package patoanalyse=null; // package to save the classifierID of the attributes

// delete pack both cases 
short index = 0; // index of pack to delete
foreach (EA.Package pa in parentpack.Packages)
{
 if ((cas == 1) || (cas == 2) || cas==4)
 {
     if (pa.PackageID == pack.PackageID)
     {
         break;
     }
 } if (pa.Name == "Assembly " + pack.Name)
 {
     break;
 }
 index++;
}
if (index < parentpack.Packages.Count)
{
 parentpack.Packages.Delete(index);
 parentpack.Packages.Refresh();
}

string toguid="";
switch (cas)
{
 case 1:
     toguid = parentpack.PackageGUID;
patoanalyse=parentpack;
     break;
 case 2:
     foreach (EA.Package pa in parentpack.Packages)
     {
         if (pa.PackageID != pack.PackageID)
         {
             toguid = pa.PackageGUID;
             patoanalyse=pa;
             break;
         }
     }
     break;
 case 3:
     toguid = pack.PackageGUID;
     patoanalyse = pack;
     break;
            
 default:
     break;
  
}

int ExportImages = 1;
int ImageFormat = -1;//-1=NONE, 0=EMF, 1=BMP, 2=GIF,3=PNG, 4=JPG
int FormatXML = 0;//True if XML output should be formatted prior to saving.
int UseDTD = 0; // int ExportImages=1;
string XmiFileName = nomfichier + ".xml"; // ABA20230313

project.ExportPackageXMI(toguid, EnumXMIType.xmiEA21, ExportImages, ImageFormat, FormatXML, UseDTD,
XmiFileName);

DicIdentifiersInit(patoanalyse);
}
/// <summary>
/// 
/// </summary>
/// <param name="repo"></param>
/// <param name="pack"></param> assembly
/// <param name="packtoregroup"></param>
void exportparentpack(EA.Repository repo, EA.Package pack,EA.Package packtoregroup)
{
string nomfichier = FileManager.GetParentDirPath() + "\\" + "cimcontextor_" + pack.Name + "_prov"; // ABA20230313
System.IO.File.Delete(nomfichier);
EA.Project project = repo.GetProjectInterface();// we get project interface
EA.Package parentpack = repo.GetPackageByID(pack.ParentID);
EA.Package patoanalyse=packtoregroup; // package to save the classifierID of the attributes

// delete pack both cases 
short index = 0; // index of pack to delete
foreach (EA.Package pa in parentpack.Packages)
{
                
     if (pa.PackageID == pack.PackageID)
     {
         break;
     }
                
 index++;
}
if (index < parentpack.Packages.Count)
{
 parentpack.Packages.Delete(index);
 parentpack.Packages.Refresh();
}

string toguid=patoanalyse.PackageGUID;
int ExportImages = 1;
int ImageFormat = -1;//-1=NONE, 0=EMF, 1=BMP, 2=GIF,3=PNG, 4=JPG
int FormatXML = 0;//True if XML output should be formatted prior to saving.
int UseDTD = 0; // int ExportImages=1;
string XmiFileName = nomfichier + ".xml"; // ABA20230313

project.ExportPackageXMI(toguid, EnumXMIType.xmiEA21, ExportImages, ImageFormat, FormatXML, UseDTD,
XmiFileName);

DicIdentifiersInit(patoanalyse);
}
/// <summary>
/// import file cimcontextor_nameofpack_prov.xml
/// into pack (cas ==1) or parent of pack (if cas ==2)
/// rename the imported package nameofpack
/// </summary>
/// <param name="repo"></param>
/// <param name="pack"></param>  pack is the package parent of the assembly model
///                              the contextual model cas==1
///                              the envelop model cas ==2
/// <param name="newname"></param>
/// <param name="cas"></param> cas==1 if assemmbly package is included in contectualpackage
///                            cas==2 if assembly package is in an envelop package with contextualpackage
///                            cas=3 if assembly is in a package parallel to contextual
void importpack(EA.Repository repo, EA.Package pack, string nameofpack,int cas)
{

EA.Project project = repo.GetProjectInterface();// we get project interface

int ImportDiagram = 1;
int StripGUID = 1;
short ind = 0;

// first import of the packge root profile
string XmiFileName = FileManager.GetParentDirPath() + "\\cimcontextor_" + nameofpack + "_prov.xml";
string intoguid = pack.PackageGUID;                
project.ImportPackageXMI(intoguid, XmiFileName, ImportDiagram, StripGUID);
pack.Packages.Refresh();
EA.Package newpack = null;
switch (cas)
{
 case 1:
foreach (EA.Package pa in pack.Packages)
{
 if ((pa.Name == pack.Name) && (pa.ParentID == pack.PackageID))
 {
     pa.Name = nameofpack;                   
     newpack = pa;
     //pa.Notes = oldassemblydescription; // update sept 2012
     pa.Update();
     break;
 }
}
EA.Diagram diagram;
try
{
 diagram = (EA.Diagram)newpack.Diagrams.GetByName(pack.Name);
 diagram.Name = nameofpack;
 diagram.Update();
}
catch (Exception e)
{
 MessageBox.Show("The diagram " + pack.Name + " is inexistant " + e.Message + " : creating it");
 for (short i = 0; i < newpack.Diagrams.Count; i++) //destroy all diagrams
 {
     newpack.Diagrams.Delete(i);
 }
 EA.Diagram diag=(EA.Diagram)newpack.Diagrams.AddNew(nameofpack, "Logical");
 diag.Update();
 newpack.Diagrams.Refresh();
 DiagPopulate(repo, diag,stereos);
 repo.RefreshModelView(pack.PackageID);
}

break;
  case 2:
   ind = 0;

        for (ind = 0; ind < pack.Packages.Count; ind++)
        { // delete asssembly  package(there are may be  three packs, one is the parent one is assemmbly
            //and the others are  the contextual models
            EA.Package pa = (EA.Package) pack.Packages.GetAt(ind);
            // if (pa.Name == nameofpack) { pack.Packages.DeleteAt(ind, true); } ABA20230219
            // break;
            if (pa.Name == nameofpack)  // ABA20230219
            { 
                pack.Packages.DeleteAt(ind, true);
                break;
            }
        }
        foreach (EA.Package pa in pack.Packages)
{
 if (pa.Name != nameofpack) // at this stage there should be two identical package with the same name
 {
     pa.Name = nameofpack;

     newpack = pa;
     pa.Update();
     break;
 }
}
   break;
  case 3:
   ind = 0;
   /*
   for (ind = 0; ind < pack.Packages.Count; ind++)
   { // delete asssembly  package(there are may be  three packs, one is the parent one is assemmbly
       //and the others are  the contextual models
       EA.Package pa = (EA.Package)pack.Packages.GetAt(ind);
       if (pa.Name == nameofpack) { pack.Packages.DeleteAt(ind, true); }
       break;
   }
    * */
                  foreach (EA.Package pa in pack.Packages)
                  {
                      if ((pa.Name == nameofpack)&& (pa.PackageID != 0)) // at this stage there should be two identical package with the same name
                      {
                          foreach (EA.Diagram diag in pa.Diagrams)
                          {// on renomme aussi les diagrammes
                              diag.Name = cons.prefixforgroupping + diag.Name;
                              diag.Update();
                          }
                          pa.Name = cons.prefixforgroupping + nameofpack;
                          newpack = pa;
                          pa.Update();
                          break;
                      }
                  }
                  break;
             }
            repo.SaveAllDiagrams();
            bool ret = pack.Update();
            string erreur = "";
            if (ret == false)
            {
                erreur = repo.GetLastError();
                MessageBox.Show("Error in importing file \n" + XmiFileName + erreur);
                writelog("Error in importing file \n" + XmiFileName + erreur);
                return;
            }
             repo.RefreshModelView(pack.PackageID);


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="pack"></param>
        /// <param name="nameofpack"></param>
        /// <param name="patoregroup"></param>
        void importpack(EA.Repository repo, EA.Package pack, string nameofpack, EA.Package patoregroup)
        {

            EA.Project project = repo.GetProjectInterface();// we get project interface

            int ImportDiagram = 1;
            int StripGUID = 1;

            // first import of the packge root profile
            string XmiFileName = FileManager.GetParentDirPath() + "\\cimcontextor_" + nameofpack + "_prov.xml";
            string intoguid = pack.PackageGUID;
            try
            {
                project.ImportPackageXMI(intoguid, XmiFileName, ImportDiagram, StripGUID);
                pack.Packages.Refresh();
            }
            catch (Exception)
            {
                MessageBox.Show("Error import" + "cimcontextor_" + nameofpack + "_prov.xml");
            }
            EA.Package newpack = null;
         
                    foreach (EA.Package pa in pack.Packages)
                    {
                        if ((pa.Name == patoregroup.Name) && (pa.PackageID != patoregroup.PackageID))
                        {
                            pa.Name = nameofpack;

                            newpack = pa;
                            newpack.Notes = oldassemblydescription;
                            EA.Element eltsource = newpack.Element;
                            EA.Element elttarget = patoregroup.Element;
                            // totag=(EA.AttributeTag)atr.TaggedValues.AddNew(ENTSOEBT, ""); 
                            EA.Connector con = (EA.Connector)eltsource.Connectors.AddNew("", "Dependency");
                            con.Stereotype = cons.GetAssemblyStereotype();
                            con.ClientID = eltsource.ElementID;
                            con.SupplierID = elttarget.ElementID;
                            con.Update();
                            eltsource.Connectors.Refresh();
                            eltsource.Update();
                            newpack.Update();
                            break;
                        }
                    }
                   
                        short i = 0;
                        List<short> diagtodeletes = new List<short>();
                        int prov = newpack.Diagrams.Count; // for test
                        foreach (EA.Diagram diag in newpack.Diagrams)
                        // for (short i = 0; i < newpack.Diagrams.Count; i++) //destroy all other diagrams 
                        {
                            if (!diag.Name.Contains(patoregroup.Name)) // en general le nom du document est dans le nom du diagramme
                            {
                                if (!diagtodeletes.Contains(i)) diagtodeletes.Add(i);
                                i++;
                            }
                            else
                            {
                                diag.Name = nameofpack;

                                diag.Update();
                                int style = ConstLayoutStyles.lsLayoutDirectionUp;
                                int iterations = 4;
                                int layerspacing = 20;
                                int ColumnSpacing = 20;
                                bool savetodiagram = true;

                                style = ConstLayoutStyles.lsInitializeDFSOut + ConstLayoutStyles.lsLayeringOptimalLinkLength + ConstLayoutStyles.lsCycleRemoveDFS + ConstLayoutStyles.lsLayoutDirectionUp;
                                project = repo.GetProjectInterface();
                                bool res = project.LayoutDiagramEx(diag.DiagramGUID, style, iterations, layerspacing, ColumnSpacing, savetodiagram);
                                diag.Update();
                                
                                i++;
                            }
                        }

                     
                    foreach(short j in diagtodeletes){
                        newpack.Diagrams.Delete(j);
                    }
                        newpack.Diagrams.Refresh();
                        newpack.Update();
                       if(newpack.Diagrams.Count==0){
                        MessageBox.Show("The diagram " + pack.Name + " is inexistant " +  " : creating it");
                        
                        EA.Diagram diag = (EA.Diagram)newpack.Diagrams.AddNew(nameofpack, "Logical");
                        diag.Update();
                        newpack.Diagrams.Refresh();
                        DiagPopulate(repo, diag, stereos);
                        repo.RefreshModelView(pack.PackageID);
                    }

                    
               
            repo.SaveAllDiagrams();
            bool ret = pack.Update();
            string erreur = "";
            if (ret == false)
            {
                erreur = repo.GetLastError();
                MessageBox.Show("Error in importing file \n" + XmiFileName + erreur);
                return;
            }
            repo.RefreshModelView(pack.PackageID);


        }
        //-----------------
        void copyATRCollection(EA.Collection fromcol, EA.Collection tocol,string type,int conrang)
        {
            ConstantDefinition CD=new ConstantDefinition();
            
            switch (type) {
                case "tag":
                    EA.AttributeTag fromtag;
                    
                    EA.AttributeTag totag=null;
                    
		
                    for (short i = 0; i < fromcol.Count; i++)
                    {
                        fromtag = (EA.AttributeTag)fromcol.GetAt(i);
                        totag=(EA.AttributeTag)tocol.AddNew(fromtag.Name, "");
                        totag.Update();
                        tocol.Refresh();

                        if (fromtag.Name == CD.GetRangTagValue())
                        {
                            
                            totag.Value = (conrang + Convert.ToInt32(fromtag.Value)).ToString();
                        }
                        else
                        {
                            totag.Value = fromtag.Value;
                        }
                        totag.Notes = fromtag.Notes;
                        totag.Update();
                    }

                    break;
              
                case "constr":
                    EA.AttributeConstraint fromconstr;
                    EA.AttributeConstraint toconstr;
                    for (short i = 0; i < fromcol.Count; i++)
                    {
                        fromconstr = (EA.AttributeConstraint)fromcol.GetAt(i);
                        toconstr=(EA.AttributeConstraint)tocol.AddNew(fromconstr.Name, fromconstr.Type);
                        toconstr.Update();
                        tocol.Refresh();
                        toconstr.Type = fromconstr.Type;
                        toconstr.Notes = fromconstr.Notes;
                        toconstr.Update();
                    }
                    tocol.Refresh();
                    break;

		            case "tagex":
                    EA.AttributeTag fromtagex;
                    EA.AttributeTag totagex=null;
	

                    for (short i = 0; i < fromcol.Count; i++)
                    {
                        fromtagex = (EA.AttributeTag)fromcol.GetAt(i);
			
                        totagex=(EA.AttributeTag)tocol.AddNew(fromtagex.Name, "non specified");
                        totagex.Update();
                        tocol.Refresh();
                        totagex.Value = fromtagex.Value;
                        totagex.Notes = fromtagex.Notes;
                        totagex.Update();
                    }
                    break;
                default:
                    break;
            }
            
        }

        //-------------------------------------------------------------------------
       /// <summary>
       /// Test if a class must be deleted
       /// that means that it has no more associations
       /// except for inheritance or dependency
       /// </summary>
       /// <param name="?"></param>
       /// <returns>true if must be deleted</returns>
        bool MustDelete(EA.Element el)
        {
            bool ret = true;
            foreach (EA.Connector con in el.Connectors)
            {
                if (con.Type == "Association" || con.Type == "Aggregation")
                { // thereis still a valid association
                    // can't be deleted
                    ret = false;
                    break;
                }
              
            }
            return ret;
        }
/// <summary>
/// treatclass examines a class to incorporate the attributes of regrouped classe
/// the examine is "downstream" the composition association 
/// there are two passes
/// treatclass justo mark the classes which have too be deleted
/// it creates an entry in ClassToBeDeleted
/// treatclass2 to do the effective job for the class which can be deleted
/// </summary>
/// <param name="el"></param>
   void  treatclass(EA.Element el){
       if (el.Name != "")
       {
          // MessageBox.Show("dans traiclass: " + el.Name);
           string prov = el.Name;
           //  MessageBox.Show(" nb de connecteurs pour" + el.Name + " = " + el.Connectors.Count.ToString()); //pour test

           if (!ClassToBeDeleted.ContainsKey(el.ElementID))
           {
               if (el.Status == "Mandatory")
               {
                   ClassToBeDeleted.Add(el.ElementID, false);//this class should not be deleted a priori 
               }
               else
               {
                   ClassToBeDeleted.Add(el.ElementID, true);//this class can be deleted a priori 
               }
           }
           writelog("Trace treatclass " + el.Name + " (" + el.ElementID + ")" + " canbedeleted?=" + ClassToBeDeleted[el.ElementID].ToString());
           bool ok = true;
           foreach (string s in stereos)
           {
               if (el.StereotypeEx.Contains(s))
               {
                   ok = false;
                   break;
               }
           }

           //MessageBox.Show("element " + el.Name); //pour test
           if (ok && !dejatraite.Contains(el.ElementID))
           {// this an eligible  class , we can start the algorithm            
               // ie when all the associations are resoluted

               string res; // result of treatconnector;
               Reassociateclassifierids(el); // reassociation of all ClassifierIDs of the attributes
               if (!ClassToBeDeleted.ContainsKey(el.ElementID))
               {

                   if (el.Status == "Mandatory")
                   {
                       ClassToBeDeleted.Add(el.ElementID, false);//this class should not be deleted a priori 
                   }
                   else
                   {
                       ClassToBeDeleted.Add(el.ElementID, true);//this class can be deleted a priori 
                   }
               }
               EA.Connector link = null;
               for (short i = 0; i < el.Connectors.Count; i++)
               // foreach(EA.Connector link in el.Connectors)
               {
                   link = (EA.Connector)el.Connectors.GetAt(i);
                   //	if(!ConnectorToBeDeleted.ContainsKey(con.ConnectorID)) ConnectorToBeDeleted.Add(con.ConnectorID,true);
                   res = treatconnector(el, link);
                   // what to do according to result of treatconnector


                   switch (res)
                   {
                       case "canbedeleted":
                           //already positioned to be deleted
                           break;
                       case "cannotbedeleted":
                           //	ConnectorToBeDeleted[con.ConnectorID]=false;
                           ClassToBeDeleted[el.ElementID] = false;  //une fois positionne restera positionne

                           break;
                       case "nothingtodo":
                           //   ConnectorToBeDeleted[con.ConnectorID]=false;
                           break;

                       default:
                           MessageBox.Show("error of result of treatconnector");
                           break;
                   }
                   string texte = "TreatClass " + el.Name + " apres traitement connecteur " + i.ToString() + " roles=" + link.ClientEnd.Role + "::" + link.SupplierEnd.Role;
                   texte = texte + " resultat=" + res.ToString() + " todelete=" + ClassToBeDeleted[el.ElementID].ToString();
                   writelog(texte);
                   /*
                     if (res == "cannotbedeleted")
                     {
                         ClassToBeDeleted[el.ElementID] = false;
                         break;
                     }
                    * */
               }

               //el.Update();
               dejatraite.Add(el.ElementID);
               if (!ClassToBeDeleted[el.ElementID])
               {// ne doit pas etre deletee donc elle peut être racine
                   if (canBeRootClass(el))
                   {
                       el.IsActive = true;
                       el.Update();
                   }
               }
               else
               {
                   if (el.IsActive)
                   {
                   // elle ne doit pas etre delete par definition
                       ClassToBeDeleted[el.ElementID] = false;
                   }
               }
           }
       }
         }
   /// <summary>
   /// treatclass examines a class to incorporate the attributes of regrouped classe
   /// the examine is "downstream" the composition association 
   /// there are two passes
   /// treatclass justo mark the classes which have too be deleted
   /// treatclass2 to do the effective job for the class which can be deleted
   /// </summary>
   /// <param name="el"></param>
   void treatclass2(EA.Element el)
   {
       if (el.Name != "")
       {
           //MessageBox.Show(" nb de connecteurs pour" + el.Name + " = " + el.Connectors.Count.ToString()); //pour test
           string pp = el.Name;
           bool ok = true;
           //  Dictionary<int, int> dicoffsets = new Dictionary<int, int>(); // to memorize the offsets of ranks during regrouping
           foreach (string s in stereos)
           {
               if (el.StereotypeEx.Contains(s))
               {
                   ok = false;
                   break;
               }
           }
           // replacement of <<ABIE>> with << MBIE>> to comply with WG16 requirement
           if (ok && el.StereotypeEx.Contains("ABIE"))
           {
               string ss = el.StereotypeEx;
               el.StereotypeEx = ss.Replace("ABIE", "MBIE");
               el.Update();
           }
           // MessageBox.Show("element " + el.Name); //pour test
           if (ok && !dejatraite.Contains(el.ElementID))
           {// this an eligible  class , we can start the algorithm 
               if (isESMPGPresent(el))
               {
                   Dictionary<int, Object> members = new Dictionary<int, Object>(); // rang,membre (attribut ou connector)
                   Dictionary<int, string> membertypes = new Dictionary<int, string>(); // rang/type membre("A" ou "C")
                   getMembersOrder(el, members, membertypes);
                   int rang = 0;
                   foreach (int rg in members.Keys)
                   {
                       switch (membertypes[rg])
                       {
                           case "A":
                               ut.setAtRang((EA.Attribute)members[rg], rang + rg);
                               break;
                           case "C":
                               int offset = treatconnector2(el, (EA.Connector)members[rg], rang);  // offset=-1 if it is not an elligible connection for regrouping
                               if ((offset != -1) && (offset > 0))
                               {
                                   //ut.setAtRang((EA.Connector)members[rg], rang + rg);
                                   rang = rang + offset - 1;
                               }
                               else
                               {
                                   ut.setAtRang((EA.Connector)members[rg], rang + rg);
                               }
                               break;
                           default:
                               break;
                       }
                   }





                   el.Update();
                   if (!ClassToBeDeleted[el.ElementID])
                   {
                       string prov = el.Name;
                       reorderAttributes(el);
                   }
               }
               else
               {
                   MessageBox.Show(" Warning the attributes of " + el.Name + " have not be ordered");
                   EA.Connector con;

                   for (short i = 0; i < el.Connectors.Count; i++)
                   {
                       con = (EA.Connector)el.Connectors.GetAt(i);
                       //	if(!ConnectorToBeDeleted.ContainsKey(con.ConnectorID)) ConnectorToBeDeleted.Add(con.ConnectorID,true);
                       treatconnector2(el, con);
                   }

                   el.Update();
               }

               dejatraite.Add(el.ElementID);
           }
      }
  
   }

        /// <summary>
        /// Populate the Diagram with all the elements of the package
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="pack"></param>
	/// <param name="stereos"></param>
      void DiagPopulate(Repository repo, EA.Diagram diag, List<string> stereos)
      {
          EA.Package pack = repo.GetPackageByID(diag.PackageID);
          diag.ShowPackageContents = true;
          diag.Update();
          repo.ReloadDiagram(diag.DiagramID);
          repo.OpenDiagram(diag.DiagramID);
          repo.ActivateDiagram(diag.DiagramID);
          EA.DiagramObject diagobj;
          int offset = 0;
          foreach (EA.Element el in pack.Elements)
          {
             // MessageBox.Show("creation decimal l'element " + el.Name + " dans le diagrame " + diag.Name); 
              bool ok = true;
              foreach (string s in stereos)
              {
                  if (el.StereotypeEx.Contains(s))
                  {
                      ok = false;
                      break;
                  }
              }
              if (ok)
              {
                  string position = "l=" + (200 + offset).ToString() + "r=" + (300 + offset).ToString();
                  position = position + "t=" + (200 +offset ).ToString() + "b=" + (300 +offset).ToString();
                                 
                  diagobj = (EA.DiagramObject)diag.DiagramObjects.AddNew("l=200;r=300;t=200;b=300;", "");
           
                  offset = offset + 300; 
                 
                  diagobj.ElementID = el.ElementID;
                  diagobj.Update();
                  diag.DiagramObjects.Refresh();
                   
                 // string message = " positions par defaut du diagramme pour " + el.Name;
                  //MessageBox.Show(message + " l=" + diagobj.left + ",r=" + diagobj.right + ",t=" + diagobj.top + ",b=" + diagobj.bottom);
                 
   
              }
      
          }
         // repo.ReloadDiagram(diag.DiagramID);
         // repo.OpenDiagram(diag.DiagramID);
         // repo.ActivateDiagram(diag.DiagramID);
       
          int style=ConstLayoutStyles.lsLayoutDirectionUp;
          int iterations=4;
          int layerspacing=20;
          int ColumnSpacing=20;
          bool savetodiagram=true;
          
          //bool res=project.LayoutDiagramEx(diag.DiagramGUID, ConstLayoutStyles.lsDiagramDefault,4,20,20, savetodiagram);
          //bool res= project.LayoutDiagramEx(diag.DiagramGUID,ConstLayoutStyles.lsCycleRemoveDFS,4,20,20, savetodiagram);
         // bool res = project.LayoutDiagramEx(diag.DiagramGUID,ConstLayoutStyles.lsLayeringOptimalLinkLength ,4,20,20, savetodiagram);
         // project.LayoutDiagramEx(diag.DiagramGUID, ConstLayoutStyles.lsInitializeDFSOut, 4, 20, 20, savetodiagram);
        // bool res = project.LayoutDiagramEx(diag.DiagramGUID, style, iterations, layerspacing, ColumnSpacing, savetodiagram);
          style = ConstLayoutStyles.lsInitializeDFSOut + ConstLayoutStyles.lsLayeringOptimalLinkLength + ConstLayoutStyles.lsCycleRemoveDFS + ConstLayoutStyles.lsLayoutDirectionUp;
          project = repo.GetProjectInterface();
          bool res = project.LayoutDiagramEx(diag.DiagramGUID, style, iterations, layerspacing, ColumnSpacing, savetodiagram);
     
           
         res= diag.Update();
         string err = diag.GetLastError();
          //MessageBox.Show("apres layout " + res.ToString()+ "|" + err);
          repo.RefreshOpenDiagrams(false);
          
          // repo.ReloadDiagram(diag.DiagramID);
           //repo.OpenDiagram(diag.DiagramID);
           //repo.ActivateDiagram(diag.DiagramID);

      }   
        
        /// <summary>
      /// Populate the Diagram with all the elements of the package
      /// and all the cooresponding IBOElements
      /// </summary>
      /// <param name="repo"></param>
      /// <param name="pack"></param>
      /// <param name="stereos"></param>
      void DiagPopulate(Repository repo, EA.Diagram diag)
      {
          repo.SaveAllDiagrams();
          EA.Package pack = repo.GetPackageByID(diag.PackageID);
          diag.ShowPackageContents = true;
         
          //repo.ReloadDiagram(diag.DiagramID);
        //  repo.OpenDiagram(diag.DiagramID);
         // repo.ActivateDiagram(diag.DiagramID);
          EA.DiagramObject diagobj;
          int offset = 0;
          List<EA.Element> listelem = new List<EA.Element>();
          ut.GetAllProfElements(pack,listelem);
          foreach (EA.Element el in listelem)
          {
              // MessageBox.Show("creation decimal l'element " + el.Name + " dans le diagrame " + diag.Name); 

                  string position = "l=" + (200 + offset).ToString() + "r=" + (300 + offset).ToString();
                  position = position + "t=" + (200 + offset).ToString() + "b=" + (300 + offset).ToString();

                  diagobj = (EA.DiagramObject)diag.DiagramObjects.AddNew("l=200;r=300;t=200;b=300;", "");

                  offset = offset + 300;

                  diagobj.ElementID = el.ElementID;
                  diagobj.Update();

                  diagobj = (EA.DiagramObject)diag.DiagramObjects.AddNew("l=200;r=300;t=200;b=300;", "");

                  offset = offset + 300;

                  diagobj.ElementID =repo.GetElementByGuid( ut.getEltParentGuid( el)).ElementID;
                  diagobj.Update();


                  diag.DiagramObjects.Refresh();

                 // string message = " positions par defaut du diagramme pour " + el.Name;
                  //MessageBox.Show(message + " l=" + diagobj.left + ",r=" + diagobj.right + ",t=" + diagobj.top + ",b=" + diagobj.bottom);


              

          }

          // on rajoute les abie des mbie detruits
          foreach (long elid in ghostmbies)
          {
              // MessageBox.Show("creation decimal l'element " + el.Name + " dans le diagrame " + diag.Name); 

              string position = "l=" + (200 + offset).ToString() + "r=" + (300 + offset).ToString();
              position = position + "t=" + (200 + offset).ToString() + "b=" + (300 + offset).ToString();

              diagobj = (EA.DiagramObject)diag.DiagramObjects.AddNew("l=200;r=300;t=200;b=300;", "");

              offset = offset + 300;

              diagobj.ElementID = (int)elid;
              diagobj.Update();

              diag.DiagramObjects.Refresh();

            

          }
          EA.Connector con = null;
          foreach (EA.DiagramLink dl in diag.DiagramLinks)
          {
              con = repo.GetConnectorByID((int)dl.ConnectorID);
              string ss = dl.Style;
              if (con.Type != "Dependency")
              {
                  dl.IsHidden = true;
                  dl.Update();
              }
          }
          int style = ConstLayoutStyles.lsLayoutDirectionUp;
          int iterations = 4;
          int layerspacing = 20;
          int ColumnSpacing = 20;
          bool savetodiagram = true;

          //bool res=project.LayoutDiagramEx(diag.DiagramGUID, ConstLayoutStyles.lsDiagramDefault,4,20,20, savetodiagram);
          //bool res= project.LayoutDiagramEx(diag.DiagramGUID,ConstLayoutStyles.lsCycleRemoveDFS,4,20,20, savetodiagram);
          // bool res = project.LayoutDiagramEx(diag.DiagramGUID,ConstLayoutStyles.lsLayeringOptimalLinkLength ,4,20,20, savetodiagram);
          // project.LayoutDiagramEx(diag.DiagramGUID, ConstLayoutStyles.lsInitializeDFSOut, 4, 20, 20, savetodiagram);
          // bool res = project.LayoutDiagramEx(diag.DiagramGUID, style, iterations, layerspacing, ColumnSpacing, savetodiagram);
          style = ConstLayoutStyles.lsInitializeDFSOut + ConstLayoutStyles.lsLayeringOptimalLinkLength + ConstLayoutStyles.lsCycleRemoveDFS + ConstLayoutStyles.lsLayoutDirectionUp;
          project = repo.GetProjectInterface();
          bool res = project.LayoutDiagramEx(diag.DiagramGUID, style, iterations, layerspacing, ColumnSpacing, savetodiagram);
          diag.Update();
           //repo.ReloadDiagram(diag.DiagramID);
           //repo.OpenDiagram(diag.DiagramID);
          // repo.ActivateDiagram(diag.DiagramID);
          
          


          //res = diag.Update();
          string err = diag.GetLastError();
          //MessageBox.Show("apres layout " + res.ToString()+ "|" + err);
          //repo.RefreshOpenDiagrams(false);
          pack.Update();
         // repo.SaveAllDiagrams();
          // repo.ReloadDiagram(diag.DiagramID);
         // repo.OpenDiagram(diag.DiagramID);
         // repo.ActivateDiagram(diag.DiagramID);
          

      }

/// <summary>
/// Initialisation of dictionay to collect attributes ClassifierID
/// </summary>
/// <param name="package"></param>
    void DicIdentifiersInit(EA.Package package){
    foreach(EA.Element  elt in package.Elements) {// beware that only one level is treated (no included packages)
     Dictionary<string,long> classifierids =new Dictionary<string,long>(); // attributename,ClassifierID
	//if(!IsPureClass(elt)) break; // we consider only pure classes and no datatypes
     if (!IsPureClass(elt) &&  (!elt.StereotypeEx.Contains(CD.GetCompoundStereotype()))) break; // we consider only pure classes and no datatypes
      foreach(EA.Attribute atr in elt.Attributes){
	if(!classifierids.ContainsKey(atr.Name)) classifierids.Add(atr.Name,atr.ClassifierID);
     }
      if (!dicidentifiers.ContainsKey(elt.Name)) dicidentifiers.Add(elt.Name, classifierids);	
   }
   
}
        /// <summary>
        /// reassociate the type of attribute with the correct ClassifierID
        /// </summary>
        /// <param name="elt"></param>
 void Reassociateclassifierids(EA.Element elt){
      Dictionary<string,long> identifierids=dicidentifiers[elt.Name];
	foreach(EA.Attribute atr in elt.Attributes){
              atr.ClassifierID=(int)identifierids[atr.Name];
              atr.Update();
          }
    elt.Update();
             
}

 /// <summary>
 /// test if an element is a Class
 /// </summary>
 /// <param name="el"></param>
 /// <returns></returns>
 bool IsPureClass(EA.Element el){
     if (!(el.Type == "Class")) return false;
	bool ok = true;
                foreach (string s in stereos)
                {
                    if (el.StereotypeEx.Contains(s))
                    {
                        ok = false;
                        break;
                    }
                }
return ok;
}

        /// <summary>
        /// include a tagvalue BusinessTerme in attributes of each element of a package when absent
        /// </summary>
        /// <param name="package"></param>
 void IncludeBTinAPackage(EA.Package package){
 foreach(EA.Element elt in package.Elements){
    IncludeBusinessTerm(elt);
}
}
        /// <summary>
 /// include a tagvalue BusinessTerme in attributes of an element 
        /// </summary>
        /// <param name="elt"></param>
 void IncludeBusinessTerm(EA.Element elt){
    EA.AttributeTag totag=null;
    const string ENTSOEBT ="ETSOBusinessTerm"; //this an UGLY provisional code should be soon be  replaced 
                                                      // by a proper configuration constant
		   // if there is no tag ENTSOEBT create one with novalue to prepare the filling
	foreach(EA.Attribute atr  in elt.Attributes){
            /* ABA20230219
		    foreach(EA.AttributeTag atag in atr.TaggedValues){
	if(atag.Name == ENTSOEBT) BusinessTermExists=true; // this an UGLY provisional code should be soon   replaced 
                                                                                        // by a proper configuration accessed constant
                         }		                  			 
            */
              totag=(EA.AttributeTag)atr.TaggedValues.AddNew(ENTSOEBT, ""); 
              
            // MessageBox.Show("error in trying create tagvalue " ); // pour test
             if (totag != null)
             {
                 try
                 {
		    //MessageBox.Show("totag not null"); //pour test
                     totag.Update();
                     atr.TaggedValues.Refresh();
                 }
                 catch (Exception e)
                 {
                     MessageBox.Show("error in creating  attribute tag " + totag.ToString() + e.Message);
                 }
             }
             else
             {
                 MessageBox.Show(" totag is null");
             }
}
 
}
//-------------------------------------------------------------  
  bool IsRegroupable(EA.Element el){

      return false;
  }
//---------------------------------------------------------
  /// <summary>
  /// a class is root if there are no association leading to it
  /// </summary>
  /// <param name="repo"></param>
  /// <param name="package"></param>
  /// <returns></returns>
        bool  canBeRootClass(EA.Element el)
  {
            bool res=true;
      foreach (EA.Connector con in el.Connectors)
      {
           
          if((con.Type == "Association" ) || (con.Type=="Aggregation"))
          {
              if (con.ClientID == el.ElementID)
              {// je suis client
                  if (con.ClientEnd.Aggregation == 0)
                  {  // for the moment the losange (sharable ) marks the direction
                      res = false; // peut être root
                      break;
                  }
              }
              else
              {// je suis fournisseur
                  if (con.SupplierEnd.Aggregation == 0)
                  {  // for the moment the losange (sharable ) marks the direction
                      res = false; // peut être root
                      break;
                  }
              }
          }
      }
          return res;
  }
        /// <summary>
        /// ayant clicke sur le package assembly a faire,
        /// le programe aide a selectionner le package a regrouper
        /// dans l'ensemble des packages parents
        /// </summary>
        /// <param name="assemblypack"></param>
        /// <returns> le nom du package selectionne</returns>
        public EA.Package  SelectPackage(EA.Package assemblypack)
        {
            EA.Package res = null;
            repo.RefreshModelView(0);
            int i = 0;
            // Sets up the initial objects in the possible CheckedListBox.
            string[] packagesname = new string[100];// a priori 
            Dictionary<string, EA.Package> packages = new Dictionary<string, EA.Package>();
            EA.Package pack = repo.GetPackageByID(assemblypack.ParentID);
            foreach (EA.Package pa in pack.Packages)
            {
                if (pa.PackageID != assemblypack.PackageID)
                {
                    packagesname[i] = pa.Name;
                    if (!packages.ContainsKey(pa.Name)) packages.Add(pa.Name, pa);
                    i++;
                }
            }
            if (!packages.ContainsKey(pack.Name)) packages.Add(pack.Name, pack);


            foreach (EA.Connector con in assemblypack.Connectors)
            {
                if ((con.Type == "Dependency") && (con.Stereotype == cons.GetAssemblyStereotype()))
                {
                    if (con.ClientID == assemblypack.Element.ElementID)
                    {
                        string prov = assemblypack.Element.Name;
                        EA.Element elementoregroup = repo.GetElementByID((int)con.SupplierID);
                        prov = elementoregroup.Name;
                        if(packages.ContainsKey(elementoregroup.Name))
                        {
                            res = packages[elementoregroup.Name];
                            string texte = " package to regroup: " + res.Name;
                        MessageBox.Show(" package to regroup: " + res.Name);
                        if (XMLP.GetXmlValueConfig("Log") == ("Checked"))
                        {
                            XMLP.AddXmlLog("propertygrouping", texte);
                        }
                        break; // oct 2012
                        }else{
                            MessageBox.Show(" Error the assembly package seems to be linked to a package outside the profile");
                            res = null;
                        }
                    }
                }
            }

            if (res == null)
            {

                DialSelect dialog = new DialSelect();

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

                dialog.ShowDialog();
                string packname = dialog.selectedpackage;
                dialog.Dispose(); // ABA20230401
                if (packname != "") {
                res = packages[packname];
               EA.Element elementoregroup = res.Element;
               string prov = elementoregroup.Name;
                EA.Element elementtoassemble=assemblypack.Element;
                prov = elementtoassemble.Name;
                   
                EA.Connector con=(EA.Connector) elementtoassemble.Connectors.AddNew("","Dependency");
                con.ClientID=elementtoassemble.ElementID;
                con.SupplierID=elementoregroup.ElementID;
                con.Stereotype=cons.GetAssemblyStereotype();
                 con.Update();
                elementtoassemble.Connectors.Refresh();
                    elementtoassemble.Update();
                     
                    /*
                    bool existe=false;
                    foreach(EA.TaggedValue tag in elementtoassemble.TaggedValues){
                        if(tag.Name==cons.GetIBOTagValue()){
                            tag.Value=elementoregroup.ElementGUID;
                            existe=true;
                            break;
                        }
                    }
                        if(!existe)
                        {
                      EA.TaggedValue atag=(EA.TaggedValue)   elementtoassemble.TaggedValues.AddNew(cons.GetIBOTagValue(),elementoregroup.ElementGUID);
                      atag.Value = elementoregroup.ElementGUID;
                            atag.Update();
                       elementtoassemble.TaggedValues.Refresh();
                       elementtoassemble.Update();

                        }
                    */
                    }
                
            }
            if (res != null) res.Update();
            return res;


        }
        /// <summary>
        /// crer un diagramme du package assembly
        /// avec les dependances
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="mbiepack"></param>
        void creDiagramDependance(EA.Repository repo, EA.Package mbiepack)
        {
            ConstantDefinition CD=new ConstantDefinition();
            List<EA.Element> listelem = new List<EA.Element>();
            string nomdiag=mbiepack.Name + CD.GetsuffixDiagGrouppingDependancies();
            ut.GetAllProfElements(mbiepack,listelem);
            
            EA.Diagram newdiag = null;
            foreach (EA.Diagram diag in mbiepack.Diagrams)
            {
                if (diag.Name == nomdiag)
                {
                    newdiag = diag;
                    for(short i=0;i < newdiag.DiagramObjects.Count;i++)
                    {
                        newdiag.DiagramObjects.DeleteAt(i,false);
                    }
                    newdiag.DiagramObjects.Refresh();
                    break;
                }
            }
            if (newdiag == null)
            {
                newdiag = (EA.Diagram)mbiepack.Diagrams.AddNew(nomdiag, "Class");
                newdiag.Update();
                mbiepack.Diagrams.Refresh();
            }
            repo.SaveAllDiagrams();
            
            newdiag.Name = nomdiag;
            DiagPopulate(repo, newdiag);
            repo.ReloadDiagram((int)newdiag.DiagramID);
            repo.RefreshOpenDiagrams(false);

        }
        /// <summary>
        /// receupere tous les element qui sont regroupe dans l'element 
        /// reoriente les association que l'element d'appel regoupe
        /// vers l'element 
        /// dans lequel il est regoupe.
        /// </summary>
        /// <param name="toel"></param>
        /// <returns></returns>
        public void RedirectElementderegroup(EA.Element el,EA.Element toel)
        {
            foreach(EA.Connector con  in el.Connectors)
            {
                if((con.Type=="Dependency") && (con.SupplierID==el.ElementID))
                {
                   // MessageBox.Show("el=" + el.Name +" toel=" +toel.Name+ " client=" +repo.GetElementByID((int)con.ClientID).Name);// oct 2012
                    con.SupplierID = toel.ElementID;
                    con.Update();
                }
            }
        }
        /// <summary>
        ///  set the attributes in order of their rank
        /// </summary>
        /// <param name="el"></param>
        void reorderAttributes(EA.Element el)
        {
            EA.Element elt = repo.GetElementByID((int)el.ElementID);
            string prov = el.Name;
            Dictionary <int,EA.Attribute>  dicattributes = new Dictionary<int, EA.Attribute>();
            List<int> attribs = new List<int>();
            foreach (EA.Attribute atr in elt.Attributes)
            {
                try
                {
                    dicattributes.Add(ut.getAtRang(atr), atr);
                    attribs.Add(ut.getAtRang(atr));
                }
                catch (Exception e)
                {
                    MessageBox.Show(" Error in dealing with " + elt.Name + "." + atr.Name + " avec rang" + ut.getAtRang(atr));
                    writelog(" Error in dealing with " + elt.Name + "." + atr.Name + " avec rang " + ut.getAtRang(atr)+ e.Message);
                }
            }
            attribs.Sort();
            EA.Attribute att =  null;
            for (int i = 0; i < attribs.Count; i++)
            {
                att=dicattributes[attribs[i]];

                             att.Pos = i;
                             att.Update();
            }

        }
        void writelog(string texte)
        {
            if (XMLP.GetXmlValueConfig("Log") == ("Checked"))
            {
                XMLP.AddXmlLog("propertygrouping", texte);
            }
        }

        void getMembersOrder(EA.Element el, Dictionary<int, Object> newmembers, Dictionary<int, string> newmembertypes)
        {
            Dictionary<int, Object> members = new Dictionary<int, Object>();
            Dictionary<int, string> membertypes = new Dictionary<int, string>();
            writelog("getMembersOrder ... " + el.Name);
            try
            {
                
                foreach (EA.Attribute at in el.Attributes)
                {
                    
                    members.Add(ut.getAtRang(at),at);
                   
                    membertypes.Add(ut.getAtRang(at),"A");
                }
                foreach (EA.Connector con  in el.Connectors)
                {
                    if (!ut.isAssociation(con)) continue;
                    if (((el.ElementID == con.SupplierID) && (con.SupplierEnd.Aggregation == 1))
                        || ((el.ElementID == con.ClientID) && (con.ClientEnd.Aggregation == 1)))
                    {
                        members.Add(ut.getAtRang(con), con);
                        membertypes.Add(ut.getAtRang(con), "C");
                    }
                }

                
                List<int> memberlist=new List<int>();
                foreach (int i in members.Keys)
                {
                    memberlist.Add(i);
                }
                
                memberlist.Sort();   
              
                foreach (int ii in memberlist)
                {

                    newmembers.Add(ii,members[ii]);
                    
                    
                   newmembertypes.Add(ii, membertypes[ii]);
                    
                }
                
            }
            catch (Exception e)
            {
                writelog("Error in getMembersOrder ..." + e.Message);
                MessageBox.Show("Error in getMembersOrder ..." + e.Message);
            }
        }
        //-------------------------------------------------------
        public bool isESMPGPresent(EA.Element el)
        {
          //  if ((this.ESMPGPresent != null)&&( this.ESMPGPresent)) return this.ESMPGPresent;
           
             if (el.Attributes.Count > 0)
      {
          foreach (EA.AttributeTag atag in ((EA.Attribute)el.Attributes.GetAt(0)).TaggedValues)
          {
              if(atag.Name==CD.GetRangTagValue())
              {
                  return true;
              }
          }
      }
      else
      {
                 if(el.Connectors.Count > 0)
                 {
          foreach (EA.ConnectorTag ctag in ((EA.Connector)el.Connectors.GetAt(0)).TaggedValues)
          {
              if (ctag.Name == CD.GetRangTagValue())
              {
                  return true;
              }
          }
                 }
      }

             return false;        
        }

        //-------------------------
        private void treatconnector2(EA.Element el, EA.Connector con)
        {

            if ((con.Type == "Dependency") && (con.Stereotype == "IsBasedOn")) { return; }
            if (con.Type == "Association" ||
                con.Type == "Aggregation")
            {
                Connexion conex = new Connexion(repo, con);

                if ((conex.SourceAggregation == 0) && (conex.TargetAggregation == 0))
                {//it is a simple association

                }


                //it is a legitimate connection if one of the end is "shared"
                if (conex.Source_element.ElementID == el.ElementID)  // the otherend is target
                {
                    if (conex.SourceAggregation == 1) // from a given element we treat only the connexion
                    //which are "descending" from it 
                    {
                        if ((conex.Target.Role != "") && (conex.TargetAggregation == 0))
                        {
                            if (ClassToBeDeleted.ContainsKey(conex.Target_element.ElementID)
                                && !ClassToBeDeleted[conex.Target_element.ElementID]) return;  // nothing to do this class cannot be deleted
                            if ((conex.Target.Cardinality == "0..1")
                                || (conex.Target.Cardinality == "1..1")
                                || (conex.Target.Cardinality == "1"))
                            { // this connexion is eligible for grouping

                                if (!dejatraite.Contains(conex.Target_element.ElementID))
                                {
                                    treatclass2(conex.Target_element);
                                }
                                foreach (EA.Attribute at in conex.Target_element.Attributes)
                                {
                                    bool mandatory = false;
                                    if (
                                        (conex.Target.Cardinality == "1..1")
                                        || (conex.Target.Cardinality == "1"))
                                    {
                                        mandatory = true;
                                    }

                                    copyattribute(at, conex.Source_element, conex.Target_element, conex.Target, mandatory);
                                }


                            }
                        }

                    }

                }

                else
                { //the other end is source

                    if (conex.TargetAggregation == 1) // il s'agit bien d'une aggregation
                    {

                        if ((conex.Source.Role != "") && (conex.SourceAggregation == 0))
                        {
                            if (ClassToBeDeleted.ContainsKey(conex.Source_element.ElementID) &&
                                !ClassToBeDeleted[conex.Source_element.ElementID]) return;  // nothing to do this class cannot be deleted
                            if ((conex.Source.Cardinality == "0..1")
                                || (conex.Source.Cardinality == "1..1")
                                || (conex.Source.Cardinality == "1"))
                            { // this connexion is eligible for grouping

                                if (!dejatraite.Contains(conex.Source_element.ElementID))
                                {
                                    treatclass2(conex.Source_element);
                                }
                                foreach (EA.Attribute at in conex.Source_element.Attributes)
                                {
                                    bool mandatory = false;
                                    if (
                                        (conex.Source.Cardinality == "1..1")
                                        || (conex.Source.Cardinality == "1"))
                                    {
                                        mandatory = true;
                                    }

                                    copyattribute(at, conex.Target_element, conex.Source_element, conex.Source, mandatory);
                                }


                            }

                        }
                    }

                }
            }
            return;
        }

        //---------------------------------------------------------- 
        private void copyattribute(EA.Attribute atr, EA.Element toelt, EA.Element fromelt, EA.ConnectorEnd conend, bool mandatory)
        {
            ////MessageBox.Show("atr " + atr.Name +",toelt " + toelt.Name 
            //  + ",fromelt " + fromelt.Name + ",Role " + conend.Role);
            short index = toelt.Attributes.Count;
            // MessageBox.Show("count avant " + toelt.Attributes.Count.ToString());
            string newname = conend.Role + "." + atr.Name;
            char[] cnewname = newname.ToCharArray();
            cnewname[0] = Char.ToLower(cnewname[0]);
            newname = new string(cnewname);

            EA.Attribute newatr = (EA.Attribute)toelt.Attributes.AddNew(newname, atr.Type);
            newatr.Update();
            //MessageBox.Show(" Error in copying an attribute " + (toelt.Attributes).GetLastError());
            toelt.Update();
            toelt.Attributes.Refresh();
            //MessageBox.Show("count apres " + toelt.Attributes.Count.ToString());

            newatr.Containment = atr.Containment;
            newatr.UpperBound = "1";
            if (mandatory && atr.LowerBound == "1")
            {
                newatr.LowerBound = "1";
            }
            else
            {
                newatr.LowerBound = "0";
            }
            newatr.Notes = atr.Notes;
            newatr.Precision = atr.Precision;
            newatr.IsConst = atr.IsConst;
            newatr.ClassifierID = atr.ClassifierID;
            newatr.Update();
            newatr.Type = atr.Type;
            newatr.Default = atr.Default;
            newatr.StereotypeEx = atr.StereotypeEx;
            newatr.Update();
            //MessageBox.Show(fromelt.Name +  "." + atr.Name + "| atr ClassifierID=" + atr.ClassifierID.ToString() + " newatr ClassifierID =" +newatr.ClassifierID.ToString()); //pour test
            //MessageBox.Show("toelt=" + toelt.Name + " ; atr=" + atr.Name); // for test
            copyATRCollection(atr.Constraints, newatr.Constraints, "constr");
            copyATRCollection(atr.TaggedValues, newatr.TaggedValues, "tag");
            // copyATRCollection(atr.TaggedValuesEx, newatr.TaggedValuesEx, "tagex");

        }
        //-----------------
        void copyATRCollection(EA.Collection fromcol, EA.Collection tocol, string type)
        {
            switch (type)
            {
                case "tag":
                    EA.AttributeTag fromtag;
                    EA.AttributeTag totag = null;

                    for (short i = 0; i < fromcol.Count; i++)
                    {
                        fromtag = (EA.AttributeTag)fromcol.GetAt(i);
                        totag = (EA.AttributeTag)tocol.AddNew(fromtag.Name, "");
                        totag.Update();
                        tocol.Refresh();
                        totag.Value = fromtag.Value;
                        totag.Notes = fromtag.Notes;
                        totag.Update();
                    }

                    break;

                case "constr":
                    EA.AttributeConstraint fromconstr;
                    EA.AttributeConstraint toconstr;
                    for (short i = 0; i < fromcol.Count; i++)
                    {
                        fromconstr = (EA.AttributeConstraint)fromcol.GetAt(i);
                        toconstr = (EA.AttributeConstraint)tocol.AddNew(fromconstr.Name, fromconstr.Type);
                        toconstr.Update();
                        tocol.Refresh();
                        toconstr.Type = fromconstr.Type;
                        toconstr.Notes = fromconstr.Notes;
                        toconstr.Update();
                    }
                    tocol.Refresh();
                    break;

                case "tagex":
                    EA.AttributeTag fromtagex;
                    EA.AttributeTag totagex = null;


                    for (short i = 0; i < fromcol.Count; i++)
                    {
                        fromtagex = (EA.AttributeTag)fromcol.GetAt(i);

                        totagex = (EA.AttributeTag)tocol.AddNew(fromtagex.Name, "non specified");
                        totagex.Update();
                        tocol.Refresh();
                        totagex.Value = fromtagex.Value;
                        totagex.Notes = fromtagex.Notes;
                        totagex.Update();
                    }
                    break;
                default:
                    break;
            }

        }
        //--------------------------------------------------------
    }
}

