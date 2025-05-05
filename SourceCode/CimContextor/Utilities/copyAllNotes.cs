using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.ComponentModel;
using CimContextor.utilitaires;
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

      
    class copyAllNotes
    {
        List<EA.Element> profilelements = new List<EA.Element>();
        List<long> profilepacksID = new List<long>(); // list of profiles package already treated
        Dictionary<string,string>  dicCimPacksNotes = new Dictionary<string,string>(); //  notes of a CimPackege given its name
        Dictionary<string, EA.Package> dicCimPacksByName= null;
        Dictionary<string, EA.Diagram> dicCimDiagramsNotesByName = new Dictionary<string,  EA.Diagram>();
        List<long> condejatraitee = new List<long>();
        List<long> eltdejatraitee = new List<long>();
        public static long ConexesID;
        public static EA.Repository repo = null;
        string texte = "";
        Utilitaires util = null; // ABA20230228 new Utilitaires();

        public struct Connexion
        {
            public long ConexID;
            public long ConnectorID;
            public EA.Element Source_element;
            public EA.Element Target_element;
            public EA.ConnectorEnd Source;
            public int SourceAggregation;
            public EA.ConnectorEnd Target;
            public int TargetAggregation;  // 

            public Connexion(EA.Connector con)
            {

                UtilitiesConstantDefinition constantes = new UtilitiesConstantDefinition(); // constants
                ConexesID++;
                ConexID = ConexesID;
                ConnectorID = con.ConnectorID;
                Source_element = repo.GetElementByID((int) con.ClientID);
                Target_element = repo.GetElementByID((int)con.SupplierID);
                Source = con.ClientEnd;
                SourceAggregation = (con.ClientEnd).Aggregation;
                Target = con.SupplierEnd;
                TargetAggregation = (con.SupplierEnd).Aggregation;
            }
            public Connexion(Connexion conex)
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

      
        /// <summary>
        /// copie toutes les notes de tous les objets
        /// d'un profil à partir des objets sur lesquels ils sont "isbasedon"
        /// cela concerne
        /// - les classes 
        /// - les datatypes, enumeration,compound
        /// -les attributs des classes
        /// - les associations
        /// - les roles des extremites d'association
        /// </summary>
        /// <param name="repos"> repository du modele</param>
        /// <param name="apackage"> paquetage enveloppe du profil</param>
        public copyAllNotes(EA.Repository repos,EA.Package apackage)
        {
            try
            {
                repo = repos;
                util = new Utilitaires(repo); // ABA20230228
                string texte = "";
                util = new Utilitaires(repo);
                EA.Collection colpacks = apackage.Packages;
            // on ne retient que les objets basedon
                util.GetAllElements(apackage, profilelements);                    //profilelements = RDFutilitaires.getAllElements(apackage, profilelements, "all");
                dicCimPacksByName = new Dictionary<string, EA.Package>();
                
                List<EA.Package> ProfPackages = new List<EA.Package>();
                List<EA.Diagram> ProfDiagrams = new List<EA.Diagram>();


                //-------------------- the packages -----------------------------------------
                util.getAllCimPackages(apackage, dicCimPacksByName,true);

                Dictionary<string, EA.Package> dicProfPackagesByName = new Dictionary<string, EA.Package>();
                util.getAllPackagesInAPackage(repo, apackage, dicProfPackagesByName);
                if (!dicProfPackagesByName.ContainsKey(apackage.Name)) dicProfPackagesByName.Add(apackage.Name, apackage);
                foreach (string paname in dicProfPackagesByName.Keys)
                {
                    if (dicCimPacksByName.ContainsKey(paname))
                    {
                        dicProfPackagesByName[paname].Notes = dicCimPacksByName[paname].Notes;
                        dicProfPackagesByName[paname].Update();
                        
                    }
                    else
                    {
                        util.wlog("WARNING", "In copyAllNotes the package " + paname + " has no Package in the CIM of the same name");
                    }
                }
                foreach (EA.Element elt in profilelements)
                {
                    if (elt.Name != "")
                    {
                        // MessageBox.Show(" elt en examen: " + elt.Name);
                        texte = " treating element : " + elt.Name;
                      //  util.wlog("copynotes", texte);//ManageIBOUtilitaires.reportlog.WriteLine(texte);

                        string prov = elt.Name;

                        if (util.getEltParentGuid(elt) != "")                //(Utilitaires.getParentGuid(elt) != "")
                        { // cet element est base sur un element donc il appartient au profil
                            if (!eltdejatraitee.Contains(elt.ElementID)) eltdejatraitee.Add(elt.ElementID);
                            copienoteselt(repos, elt);

                        }
                    }
                }
                //---------- les diagrames ------------
                foreach(EA.Package cimpack in dicCimPacksByName.Values)
                {
                    foreach (EA.Diagram diag in cimpack.Diagrams)
                    {
                        if(!dicCimDiagramsNotesByName.ContainsKey(diag.Name))
                        {
                            dicCimDiagramsNotesByName.Add(diag.Name, diag);

                        }
                    }
                }
               
                foreach(EA.Package pack in dicProfPackagesByName.Values )
                {

                    foreach(EA.Diagram dg in pack.Diagrams)
                    {
                        if(dicCimDiagramsNotesByName.ContainsKey(dg.Name))
                        {
                            dg.Notes = dicCimDiagramsNotesByName[dg.Name].Notes;
                            dg.Update();
                            pack.Update();

                        }
                        
                    }
                }
            }   
            catch (Exception e)
            {
                 texte = "Error in treating element=" + e.Message;
                util.wlog("copynotes",texte);
                MessageBox.Show("copynotes " + texte);
            }
                 }
        //------------------------------
        public EA.Package getCimPack(EA.Package pack)
        {
            if (pack.ParentID == 0) return null; //nous sommes arrive au plus haut niveau
            EA.Package res = null;
            EA.Package pa = pack;
            if (pack == null) return null;
            try
            {
                if (pa.Name == "IEC61970")
                {
                    return pa;
                }
                else
                {
                    if (pa.PackageID == 0) return null;
                    pa = repo.GetPackageByID((int)pa.ParentID);
                         
                }
                res = getCimPack(pa);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error pb access to CIM package " + e.Message);
                res = null;
            }
            return res;
        }
        //------------------------------
        void copienotesatr(EA.Repository repos, EA.Attribute atr)
        {
            if (util.getAtrParentGuid(atr) != "")
            {
                EA.Attribute IBOat = repos.GetAttributeByGuid(util.getAtrParentGuid(atr));
                atr.Notes = IBOat.Notes;
                atr.Update();
            }
        }
        void copienotesconnector(EA.Repository repos, EA.Connector con)
        {

            string sourcename = "";
            string targetname = "";
             string texte = "";
           
            if (!condejatraitee.Contains(con.ConnectorID))
            {
                condejatraitee.Add(con.ConnectorID);
                if (util.getConParentGuid(con) != "")
                {
                    EA.Connector IBOcon = repos.GetConnectorByGuid(util.getConParentGuid(con));
                    if (IBOcon == null) return;
                    con.Notes = IBOcon.Notes;
                    con.Update();
                    Connexion conex = new Connexion(con);
                    Connexion IBOconex = new Connexion(IBOcon);
                     sourcename=conex.Source_element.Name;
                 targetname=conex.Target_element.Name;
                 texte = "--- processing connector between " + sourcename + "::" + conex.Source.Role + " and " + targetname + "::" + conex.Target.Role;
                //util.wlog("copynotes",texte);
                texte = "---   the CIM roles are  sourcerole=" + IBOconex.Source.Role + " and targetrole=" + IBOconex.Target.Role;
                bool inverse = false; // true si source et targetsont inversée
                  if (conex.Source.Role != "")
                {
                    if (util.RemoveQual(conex.Source_element.Name) != IBOconex.Source_element.Name)
                    {
                        inverse = true;
                    }
                }
                else
                {
                    if (util.RemoveQual(conex.Target_element.Name) != IBOconex.Target_element.Name)
                    {
                        inverse = true;
                    }
                }
                if(inverse)
                { 
                texte = "--- source and target are inverse as in CIM  " + inverse.ToString();
               // util.wlog("copynotes",texte);
                }
                  
                    if (conex.Source.Role != "")
                    {
                         if(inverse)
                         {
                        conex.Source.RoleNote = IBOconex.Target.RoleNote;
                        }else{
                          conex.Source.RoleNote = IBOconex.Source.RoleNote;  
                         }
                         conex.Source.Update();
                         
                    }
                    if (conex.Target.Role != "")
                    {
                       if (inverse)
                    {
                        conex.Target.RoleNote = IBOconex.Source.RoleNote;
                    }
                    else
                    {
                        conex.Target.RoleNote = IBOconex.Target.RoleNote;
                    }
                    conex.Target.Update(); 
                    }

                }
            }
             
        }

        //--------------------

//---------------------------------
//--------------------
/// <summary>
/// recopie toues les notes d'un element
/// </summary>
/// <param name="repos"></param>
        void copienoteselt(EA.Repository repos, EA.Element el)
        {

             EA.Element IBOElt = repos.GetElementByGuid(util.getEltParentGuid(el));
           if (IBOElt != null)
                {
                 if (el.Name == "GovSteamIEEE1")
                  {
                     string prov = el.Name;
                      }
                   el.Notes = IBOElt.Notes;
                   el.Update();

 //---------------- package ----------------------
            /*
               if (!profilepacksID.Contains(el.PackageID))
                { // we must copy the notes from Cimpackage
                   
                    EA.Package cimpack = null;
                    EA.Package globcimpack = null;
                    profilepacksID.Add(el.PackageID);
                    EA.Package profpack = repo.GetPackageByID((int)el.PackageID);

                    cimpack = repo.GetPackageByID((int)IBOElt.PackageID);
                    //first copy notes of diagrams
                    copyNotesOfDiagrams(profpack, cimpack);
                    //then copy Notes of the package
                    if (!dicCimPacksNotes.ContainsKey(profpack.Name))
                    {
                       
                        globcimpack = getCimPack(cimpack);
                        if (globcimpack != null)
                        {


                            List<EA.Package> listcimpacks = new List<EA.Package>();
                            Utilitaires.getAllPackages(globcimpack, listcimpacks);
                            foreach (EA.Package pa in listcimpacks)
                            {
                                dicCimPacksNotes[pa.Name] = pa.Notes;
                            }
                        }
                        else { return; }
                    }
                    if (dicCimPacksNotes.ContainsKey(profpack.Name))
                    {
                        profpack.Notes= dicCimPacksNotes[profpack.Name] ;
                        profpack.Update();
                    }
                }
               
            */
           
  //--------------------------------------------
                //les attributs
                foreach (EA.Attribute at in el.Attributes)
        {
            texte = "treating ... attribut " + at.Name + " of :" + el.Name;
            try
            {
                copienotesatr(repos, at);
            }
            catch (Exception)
            {
                texte = "Error pb in copying one attribute " + at.Name + " of : " + el.Name;
                util.wlog("copynotes",texte);
                //ConstDefinition.Errorspresent = true;
            }
        }

        // les connexions
        foreach (EA.Connector con in el.Connectors)
        {
            try
            {
                copienotesconnector(repos, con);
            }
            catch (Exception)
            {
                texte = "Error pb in copying on connector of : " + el.Name;
                util.wlog("copynotes",texte);
                //ConstDefinition.Errorspresent = true;
            }
        }
                //----------- les diagrames -----------------
                Dictionary<string, EA.Diagram> parentdiagsdic = new Dictionary<string, EA.Diagram>(); // diagname/diag
                if (el.Diagrams.Count > 0)
                {
                    foreach (EA.Diagram diag in IBOElt.Diagrams)
                    {
                        if (!parentdiagsdic.ContainsKey(diag.Name))
                        {
                            parentdiagsdic.Add(diag.Name, diag);
                        }
                    }
                    foreach(EA.Diagram diag in el.Diagrams)
                    {
                        if(parentdiagsdic.ContainsKey(diag.Name))
                        {
                            diag.Notes = parentdiagsdic[diag.Name].Notes;
                            diag.Update();
                        }
                    }
                    el.Update();
                }
    }
    else
    {
        texte = "Error in accessing the IBO element of  : " + el.Name;
        util.wlog("copynotes",texte);
                throw new Exception(String.Format(" copienoteselt {0} acces", el.Name));
        //ConstDefinition.Errorspresent = true;
    }
}
        /// <summary>
        /// copy the notes from the diagrams of the cim package
        /// </summary>
        /// <param name="profpack"></param>
        /// <param name="cimpack"></param>
        private void copyNotesOfDiagrams(Package profpack,EA.Package cimpack)
        {
            try
            {
                List<EA.Diagram> listdiagrams = new List<EA.Diagram>();
                Dictionary<string, EA.Diagram> dicdiags = new Dictionary<string, EA.Diagram>(); // dictionary of diagrmas by their name
                foreach(EA.Diagram diag in cimpack.Diagrams)
                {
                    if(!dicdiags.ContainsKey(diag.Name))
                    {
                        dicdiags[diag.Name] = diag;
                    }
                }
                foreach (EA.Diagram diag in profpack.Diagrams)
                {
                    if (dicdiags.ContainsKey(diag.Name))
                    {
                        diag.Notes = dicdiags[diag.Name].Notes;
                        diag.Update();
                    }
                }
            }catch(Exception e)
            {
                util.wlog("copyAllNotes"," error in copying diagrams " + e.Message);
                throw new Exception("copyNotesOfDiagrames");
            }
        }


        //---------------------------------------------------------------------------
    }
}
