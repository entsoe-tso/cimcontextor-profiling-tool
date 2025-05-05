using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.ComponentModel;
//using CimExportImport;
using EA;
//using CimExportImport.manageIBO;
using CimSyntaxGen;
using CimSyntaxGen.manageIBO;
/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.8.27                                         *    march 2019*
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
namespace CimSyntaxGen
{
    /// <summary>
    /// this class will create (if it does not exists)
    /// the IsBasedOn dependency betweel classes according tho the tag value
    /// GUIDBasedOn found in the class
    /// </summary>
   public class WithoutIBODependency
    {
        public StreamWriter reportlog=null; // to report details of recreation

       // public Dictionary<long, EA.Package> dicpackbyid; // packageid,package
       // public List<long> listpackids; // list of all packages in a given package
       // public Dictionary<long, List<long>> dicIBOpackbyid; // packageid, list of packageid (from which it depends)
        static EA.Package aPackage; // selected top package
        //public static List<EA.Element> ListElem = null; // list of all model elements
       // public static Dictionary<long, long> DicPackElem = null; // dictionay of all elements tied with package elemid/packid
        //public static Dictionary<string, List<long>> dicElems = null; // dictionnaire par nom et liste de ID
        EA.Repository repo;
    
        static public List<string> Cimstereos = new List<string>()
                                      {
	                                    "Class",
	                                    "Datatype",
                                        "Primitive",
	                                    "Compound",
	                                    "enumeration",
                                        "ACC",
                                        "ABIE",
                                        "MBIE",
                                       };


        
/// <summary>
/// dele from a given package all <<IsBasedOn>> Dependency
/// </summary>
/// <param name="repository"></param>
       public WithoutIBODependency(EA.Repository repository)
        {
            repo = repository;
            string texte = ""; // pour report
            ConstantDefinition constantes=new ConstantDefinition();
            string IBOtgv =constantes.GetIBOTagValue();
            aPackage = repository.GetTreeSelectedPackage();
               //18 reportlog=new StreamWriter("WithoutIBODependency.txt"); //  the report  filePath
            reportlog = Main.reportlog;
           ManageIBOUtilitaires miboutilitaires=new ManageIBOUtilitaires (reportlog); 
            List<EA.Element> ElemList = new List<EA.Element>();
            //ManageIBOUtilitaires.getAllElements(aPackage, ElemList, Cimstereos);
            ManageIBOUtilitaires.getAllElementsByType(aPackage, ElemList, new List<string>{"Class"});



            Dictionary<long, EA.Package> dicpackbyid = new Dictionary<long, EA.Package>();
            CimSyntaxGen.RDFutilitaires.buildDicrepobyid(repo, dicpackbyid);
            /******** pour test *************/
            EA.Package patest = GetPackagebyName(repo, dicpackbyid, aPackage.Name);
            miboutilitaires.PrintIBOElements(repo,patest);

    

            /*****************************************************/
            
            foreach (EA.Element el in ElemList)
            {
                try
                {
                    string prov = el.Name;
                    List<long> conindices = new List<long>();
                    for (short i=0;i <el.Connectors.Count;i++)
                    {
                        //string typ = ((EA.Connector)el.Connectors.GetAt(i)).Type;
                       // string stereotyp = ((EA.Connector)el.Connectors.GetAt(i)).Stereotype;
                        if (
                            (((EA.Connector)el.Connectors.GetAt(i)).Type == "Dependency") && 
                            (((EA.Connector)el.Connectors.GetAt(i)).Stereotype == constantes.GetIsBasedOnStereotype())
                            )
                        {
                            long conid = ((EA.Connector)el.Connectors.GetAt(i)).ConnectorID;
                            if (!conindices.Contains(conid))
                            {  
                                conindices.Add(conid);
                            }
                           
                            //el.Refresh();
                            //break;
                        }
                    }
                    if (conindices.Count > 0)
                    {
                        for (short j = 0; j < el.Connectors.Count; j++)
                        {
                            long conid = ((EA.Connector)el.Connectors.GetAt(j)).ConnectorID;
                            if(conindices.Contains(conid))
                            {
                                el.Connectors.DeleteAt(j, true);
                                el.Connectors.Refresh();
                                j = 0;
                            }                                                  
                        }
                        
                        el.Update();
                        el.Refresh();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
               
            }
            

        }
       /// <summary>
       /// obtient le premier package ayant un nom donne
       /// </summary>
       EA.Package GetPackagebyName(EA.Repository repo, Dictionary<long, EA.Package> dicpackbyid, string paname)
       {
           EA.Package ret = null;
           foreach (EA.Package pa in dicpackbyid.Values)
           {
               if (pa.Name == paname)
               {
                   ret = pa;
                   break;
               }
           }
           return ret;

       }

//- - - - - - - - - - - - - - - - - - - - - 
    }
}
