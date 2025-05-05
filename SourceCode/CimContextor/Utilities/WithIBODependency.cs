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
    /// <summary>
    /// this class will create (if it does not exists)
    /// the IsBasedOn dependency betweel classes according tho the tag value
    /// GUIDBasedOn found in the class
    /// </summary>
    public class WithIBODependency
    {

        static EA.Package aPackage; // selected top package
    
        EA.Repository repo;
    
        static public List<string> Cimstereos = new List<string>()
                                      {
	                                    "Class",
	                                    "Datatype",
                                        "Primitive",
	                                    "Compound",
	                                    "enumeration",
                                        "Enumeration",
                                        "ACC",
                                        "ABIE",
                                        "MBIE",
                                       };

        UtilitiesConstantDefinition constantes = new UtilitiesConstantDefinition();
        // ABA20230228 Utilitaires util = new Utilitaires();



        public WithIBODependency(EA.Repository repository)
        {
            Utilitaires util = new Utilitaires(repository);
            repo = repository;
            string texte = ""; // pour report
           
            string IBOtgv =constantes.GetIBOTagValue();
            aPackage = repository.GetTreeSelectedPackage();
        
            List<EA.Element> ElemList = new List<EA.Element>();
            util.getAllElementsByType(aPackage, ElemList, new List<string> { "Class" });
            EA.TaggedValue tgvfrom = null;
            texte = "============================================================";
           // util.wlog("WithIBODependency",texte);

            texte = " recreer les données IBO (les tagvalues sont positionnees) " + DateTime.Now;
           // util.wlog("WithIBODependency",texte);
            texte = "===========================================================";
          //  util.wlog("WithIBODependency",texte);
         

            foreach (EA.Element el in ElemList)
            {
                string  prov = el.Name;
                try
                {
                    tgvfrom = (EA.TaggedValue)el.TaggedValues.GetByName(IBOtgv);
                   
                }
                catch (Exception e)
                {
                    MessageBox.Show("Warning there is an error with  IBO tabvalue for element " + el.Name
                            + "  are you should you build your profile with CimContextor?");
                    tgvfrom = null;
                    MessageBox.Show(e.Message);
                }
                if (tgvfrom != null)
                {// thereis a baseddon element
                    texte = "  element " + repository.GetPackageByID(el.PackageID).Name + "::" + el.Name;
                   // util.wlog("WithIBODependency",texte);
                 
                    /***********  pour test
                    if (el.Name == "MarketRole")
                    {
                        string prov = "";
                    }
                     ************/
                      
                    if (!thereisIBODependency(el))
                    {
                        EA.Connector con = (EA.Connector)el.Connectors.AddNew("", "Dependency");
                        con.ClientID = el.ElementID;
                        try
                        {
                            EA.Element eltbase = repo.GetElementByGuid(tgvfrom.Value);
                            texte = " association  avec element parent " + repo.GetPackageByID(eltbase.PackageID).Name + "::" + eltbase.Name;
                          //  util.wlog("WithIBODependency",texte);
               
                            con.SupplierID = eltbase.ElementID;

                            con.Stereotype = constantes.GetIsBasedOnStereotype();
                            con.Update();
                            el.Connectors.Refresh();
                            el.Update();
                        }
                        catch (Exception e)
                        {
                            util.wlog("WithIBODependency","Error" + e);
                           
                        }
                    }
                }
            }
            

        }
        /// <summary>
        /// teste si il y a deja un lien IsBasedOn 
        /// </summary>
        bool thereisIBODependency(EA.Element  el)
        {
            bool resul = false;
            foreach (EA.Connector con in el.Connectors)
            {
                if((con.Type=="Dependency") &&   (con.Stereotype == constantes.GetIsBasedOnStereotype()))
                {
                    if (con.ClientID == el.ElementID)
                    {
                        resul = true;
                        break;
                    }
                }
            }
            return resul;
        }
  
//- - - - - - - - - - - - - - - - - - - - - 
    }
}
