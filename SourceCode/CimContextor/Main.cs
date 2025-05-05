
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using CimContextor.GenerateMessageAssembly;
using CimContextor.EditConnectors;
using CimContextor.Utilities;
using CimContextor.utilitaires;

using System.Text;
using System.IO;
using CimContextor.Integrity_Checking;
using System.ComponentModel;
using EA;
using CimContextor.Configuration;
using System.Threading;



/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.9.26                                         *    june 2021 *
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

namespace CimContextor
{
    public class Main
    {
        private static readonly string version = "3.2.5";
        private readonly bool m_ShowFullMenus = false;
        private static bool ExecuteIBO = false;
        private static int ObjectLeft;
        private static int ObjectRight;
        private static int ObjectTop;
        private static int ObjectBottom;
        private static bool objIsDeleted = false; // ABA20230112
        private static string ObjectStyle;
        private static bool ObjectIsHere = false;
        public static EA.Repository Repo; //am juil 2013
        public static string EditType;// type of connector edition (graph or hierarchy) am juil 2016 
        public static string valtest1; // a place to store a test value accross various instances
        public static bool testperf = false; // if true the time analysis is performed
        public static bool isBasedOnExecuted = false;
        public static bool ongoing = true;
        MsgBox msgBox = new MsgBox();
        private const string LIC_MENU = "Load License";
        private const string CONFIG_IMPORT_MENU = "Import Configuration";
        private const string CONFIG_EXPORT_MENU = "Export Configuration";
        private const string COPY_SUPERCLASS_CONN_MENU = "Copy Superclass Connectors";
        private const string SORT_ELEMENTS_MENU = "Sort Elements in Package";
        private const string SET_DESCRIPTION_STEREOTYPE = "Set 'Description' stereotype";
        private string licKey;

        public static EA.Diagram diag;

        XMLParser XMLP = null;
        private Type objType;

        public Main()
        {

        }

        //Called Before EA starts to check Add-In Exists
        public String EA_Connect(EA.Repository Repository)
        {
            //No special processing required.
            return "";
        }

        public void EA_FileOpen(EA.Repository Repository) // am aout2018
        {
            FileManager.CreateParentDirectory(); // ABA20221020
        }

        //Called when user Click Add-Ins Menu item from within EA.
        //Populates the Menu with our desired selections.
        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {
            Repo = Repository; //am juil 2013

            try
            {
                XMLP = new XMLParser(Repository);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ErrorCodes.ERROR_008[0] + ": " + ErrorCodes.ERROR_008[1] + "\n" + ex.StackTrace, 
                                "Error", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
                return "";
            }

            try
            {
                switch (MenuName)
                {
                    case "":
                        return "-&CimConteXtor";
                    case "-&CimConteXtor":                     
                        string[] ar;
                        if (IsProjectOpen(Repository) && XMLP.GetXmlValueConfig("EnablePropertyGrouping") == (ConfigurationManager.CHECKED))
                        {
                            if (ConstantDefinition.GetExpirationCheckState())
                            {
                                ar = new string[] { LIC_MENU, CONFIG_IMPORT_MENU, CONFIG_EXPORT_MENU, COPY_SUPERCLASS_CONN_MENU, "CreateProfilePackage", "Edit an IsBasedOn", "Edit connectors for WG13(CGMES)", "Edit graph connectors(all inheritances)", "Edit hierarchical connectors", "AttributeOrder", "PropertyGrouping", SET_DESCRIPTION_STEREOTYPE, "-Utilities", "Options", "About..." }; // am aout  2018
                            }
                            else
                            {
                                ar = new string[] { CONFIG_IMPORT_MENU, CONFIG_EXPORT_MENU, COPY_SUPERCLASS_CONN_MENU, "CreateProfilePackage", "Edit an IsBasedOn", "Edit connectors for WG13(CGMES)", "Edit graph connectors(all inheritances)", "Edit hierarchical connectors", "AttributeOrder", "PropertyGrouping", SET_DESCRIPTION_STEREOTYPE, "-Utilities", "Options", "About..." }; // am aout  2018
                            }
                        }
                        else
                        {
                            if (ConstantDefinition.GetExpirationCheckState())
                            {
                                ar = new string[] { LIC_MENU, CONFIG_IMPORT_MENU, CONFIG_EXPORT_MENU, COPY_SUPERCLASS_CONN_MENU, "CreateProfilePackage", "Edit an IsBasedOn", "Edit connectors for WG13(CGMES)", "Edit graph connectors(all inheritances)", "Edit hierarchical connectors", SET_DESCRIPTION_STEREOTYPE, "-Utilities", "Options", "About..." };// am mars 2016
                            }
                            else
                            {
                                ar = new string[] { CONFIG_IMPORT_MENU, CONFIG_EXPORT_MENU, COPY_SUPERCLASS_CONN_MENU, "CreateProfilePackage", "Edit an IsBasedOn", "Edit connectors for WG13(CGMES)", "Edit graph connectors(all inheritances)", "Edit hierarchical connectors", SET_DESCRIPTION_STEREOTYPE, "-Utilities", "Options", "About..." };// am mars 2016
                            }
                        }
                        return ar;
                    case "-IntegrityCheck":
                        string[] era = { "IntegrityCheck", "ESMPIntegrityCheck", "CGMESIntegrityCheck", };
                        return era;
                    case "-Utilities":
                        string[] ura = { "IntegrityCheck", "ESMPIntegrityCheck", "CGMESIntegrityCheck", "ReplaceSimpleFloatByFloat", "LocalizeDataTypes", "CreateGlobalProfile", "CopyAllNotes", "RecoverProfileDatatypes", "MakeMembersMandatory", SORT_ELEMENTS_MENU };
                        return ura;
                }
            }
            catch (Exception ex)
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_028, ex);
            }

            return "";
        }

        //Sets the state of the menu depending if there is an active project or not
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Called once Menu has been opened to see what menu items are active.
        public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(Repository))
            {
                if (ItemName == "Options")
                    IsChecked = m_ShowFullMenus;
            }
            else
                // If no open project, disable all menu options
                IsEnabled = false;
        }

        //Called when user makes a selection in the menu.
        //This is your main entry point to the rest of your Add-in
        public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            if(!IsProjectOpen(Repository))
            {
                return;
            }

            // set stereotypes if they don't exist yet
            ConfigurationManager confMgmter = ConfigurationManager.GetConfigurationManager(Repository);
            confMgmter.DefineStereotypes();

            if (ConstantDefinition.GetExpirationCheckState())
            {
                bool loadingLicense = false;

                if (ItemName == LIC_MENU)
                {
                    loadingLicense = true;
                    LicenseDialog licDialog = new LicenseDialog();
                    licDialog.Show();
                }


                if (!loadingLicense)
                {
                    Utilities.LicenseManager licenseManager = new Utilities.LicenseManager();
                    if (licenseManager.LicenseExists())
                    {
                        licKey = licenseManager.LoadLicense();
                        if (licenseManager.IsValidLicenseKey(licKey))
                        {
                            int diffDays = licenseManager.GetDateDiff(licKey);
                            if (diffDays == 0)
                            {
                                MessageBox.Show("Your license expires tomorrow!", "License Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else if ((0 < diffDays) && (diffDays <= Utilities.LicenseManager.WARNING_DAYS))
                            {
                                MessageBox.Show("Your license will expire in " + diffDays + " days!", "License Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else if (diffDays < 0)
                            {
                                MessageBox.Show("Your license is expired!", "License Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid License: " + licKey, "License Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("License file not found! Load a valid license.", "License Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            Repository.SaveAllDiagrams(); // am fev 2012
            string step = " before dialog launch";
            switch (ItemName)
            {
                case CONFIG_IMPORT_MENU:
                    ConfigurationManager.GetConfigurationManager(Repository).ImportConfiguration();
                    break;
                case CONFIG_EXPORT_MENU:
                    ConfigurationManager.GetConfigurationManager(Repository).ExportConfiguration();
                    break;
                case COPY_SUPERCLASS_CONN_MENU:
                    CopySuperclassConnectorDialog cscDialog = new CopySuperclassConnectorDialog(Repository);
                    cscDialog.ShowDialog();
                    cscDialog.Dispose();
                    break;
                case "PropertyGrouping":
                    PropertyGrouping ptygroup = new PropertyGrouping(Repository);
                    break;
                case "AttributeOrder":
                    try
                    {
                        if (Repository.GetContextItemType() != EA.ObjectType.otElement)
                        {
                            MessageBox.Show(" The selected element is not a diagram class object");
                            return;
                        }


                        EA.Element el = (EA.Element)Repository.GetContextObject();
                        step = "  dialog launch";
                        OrderAssemblyAttributes oa = new OrderAssemblyAttributes(el, Repository);
                        /* ABA20230219
                        List<string> initiale = new List<string>() {"A","to1to","1","R","to2to","2",
                                                                       "A","to3to","3","R","to4to","4",
                                                                       "A","to5to","5","R","to6to","6",
                                                                        "A","to7to","7","R","to8to","8",
                                                                       };
                        */
                        step = " before dialog show";
                        if (oa.ongoing == true)
                        {
                            oa.ShowDialog();
                        }
                        oa.Dispose();
                        //else ABA20230401
                        //{
                        //    oa.Dispose();
                        //}
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(" Selection problem at step " + step + ": " + e.Message);
                    }
                    break;
                case "Global IsBasedOn":
                    GlobalIsBasedOn GIBO = new GlobalIsBasedOn(Repository);
                    break;
                case "Edit an IsBasedOn":
                    try
                    {
                        LoadingIndicator loadingIndicator = new LoadingIndicator();
                        Thread loadingThread = new Thread(new ThreadStart(loadingIndicator.ShowIndicator));
                        loadingThread.Start();
                        EditAnIsBasedOn IBO = new EditAnIsBasedOn(Repository, loadingThread, loadingIndicator);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    break;
                case "Edit hierarchical connectors":
                    if (Repository.GetCurrentDiagram() == null)
                    {
                        MessageBox.Show("You must select an IsBasedOn child class (only one) from a diagram before using this function!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    }
                    else if (!Repository.GetCurrentDiagram().SelectedObjects.Count.Equals(1))
                    {
                        MessageBox.Show("You must select an IsBasedOn child class (only one) from a diagram before using this function.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    }
                    EditType = "hierarchy"; //am juil 2016
                    EditDuplicateForXsdConnectorsForm EDCF = new EditDuplicateForXsdConnectorsForm(Repository);
                    break;
                case "Edit graph connectors(all inheritances)":
                    if (Repository.GetCurrentDiagram() == null)
                    {
                        MessageBox.Show("You must select an IsBasedOn child class (only one) from a diagram before using this function", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    }
                    else if (!Repository.GetCurrentDiagram().SelectedObjects.Count.Equals(1))
                    {
                        MessageBox.Show("You must select an IsBasedOn child class (only one) from a diagram before using this function", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    }
                    EditType = "graph";
                    EditDuplicateForOptimConnectorsForm EDCFF = new EditDuplicateForOptimConnectorsForm(Repository);
                    break;
                case "Edit connectors for WG13(CGMES)":
                    if (Repository.GetCurrentDiagram() == null)
                    {
                        MessageBox.Show("You must select an IsBasedOn child class (only one) from a diagram before using this function", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    }
                    else if (!Repository.GetCurrentDiagram().SelectedObjects.Count.Equals(1))
                    {
                        MessageBox.Show("You must select an IsBasedOn child class (only one) from a diagram before using this function", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    }
                    EditType = "graphwg13";
                    EditDuplicateForOptimConnectorsForm EDCFFF = new EditDuplicateForOptimConnectorsForm(Repository);
                    break;
                case "optim":
                    if (Repository.GetCurrentDiagram() == null)
                    {
                        MessageBox.Show("You must select an IsBasedOn child class (only one) from a diagram before using this function", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    }
                    else if (!Repository.GetCurrentDiagram().SelectedObjects.Count.Equals(1))
                    {
                        MessageBox.Show("You must select an IsBasedOn child class (only one) from a diagram before using this function", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    }
                    EditType = "optim";
                    EditDuplicateForOptimConnectorsForm EDCFFFF = new EditDuplicateForOptimConnectorsForm(Repository);
                    break;
                case "IntegrityCheck":
                    Validations valids = null;
                    msgBox = new MsgBox();
                    msgBox.ShowBox("Processing IntegrityCheck...");
                    try
                    {
                        XMLP.SetXmlValueConfig("EnableESMPHierarchy", ConfigurationManager.UNCHECKED);
                        valids = new Validations(Repository, Repository.GetTreeSelectedPackage());
                    } finally
                    {
                        msgBox.CloseBox();
                    }
                    CheckResultsDisplay checkResultsDisplay = new CheckResultsDisplay("General Integrity Check", valids.ValidationEntries, XMLP);
                    checkResultsDisplay.fillText();
                    checkResultsDisplay.Show();
                    if(valids != null) valids.ValidationEntries.Clear();
                    break;
                case "ESMPIntegrityCheck":
                    msgBox = new MsgBox();
                    msgBox.ShowBox("Processing ESMPIntegrityCheck...");
                    try
                    {
                        XMLP.SetXmlValueConfig("EnableESMPHierarchy", ConfigurationManager.CHECKED);
                        valids = new Validations(Repository, Repository.GetTreeSelectedPackage());
                    }
                    finally
                    {
                        msgBox.CloseBox();
                    }
                    CheckResultsDisplay checkESMPResultsDisplay = new CheckResultsDisplay("ESMP Integrity Check", valids.ValidationEntries, XMLP);
                    checkESMPResultsDisplay.fillText();
                    checkESMPResultsDisplay.Show();
                    valids.ValidationEntries.Clear();
                    break;
                case "CGMESIntegrityCheck":
                    msgBox = new MsgBox();
                    msgBox.ShowBox("Processing CGMESIntegrityCheck...");
                    CheckEntsoeProfile evalids = null;
                    try
                    {
                        evalids = new CheckEntsoeProfile(Repository, Repository.GetTreeSelectedPackage());
                    } finally
                    {
                        msgBox.CloseBox();
                    }
                    if (evalids != null && evalids.errors > 0)
                    {
                        string chemin = System.IO.Path.GetDirectoryName(evalids.XMLP.ActualFilePath) + "/Log.xml";
                        MessageBox.Show(" validations : there are some issues. For details, you can  look log file in \n"
                            + chemin);
                    }
                    break;
                //case "Concatenate diagram": ABA20230401
                //    ConcatenateForm ConcDiagram = new ConcatenateForm(Repository);
                //    break;
                case "CreateProfilePackage":
                    CreateProfile cprof = new CreateProfile(Repository, Repository.GetTreeSelectedPackage());
                    break;
                //case "Testing":// pour essai de nouvelles fonctionalites ABA20230401
                //    DialTree diagsais = new DialTree(Repository, Repository.GetTreeSelectedPackage(), false);
                //    diagsais.ShowDialog();
                //    break;
                case SET_DESCRIPTION_STEREOTYPE:
                    EA.Element selectedElem = null;
                        
                    if(EA.ObjectType.otElement == Repo.GetContextItemType())
                    {
                        selectedElem = (EA.Element)Repo.GetContextObject();
                    }

                    if(selectedElem == null || !selectedElem.Type.Equals("Class"))
                    {
                        MessageBox.Show("Please select an class for setting this stereotype.");
                        return;
                    }
                    DialSetDescriptionStereotype dialSetDescStereo = new DialSetDescriptionStereotype(Repo, selectedElem);
                    dialSetDescStereo.ShowDialog();
                    dialSetDescStereo.Dispose();
                    break;
                case "Options":
                    OptionForm OptF = null;
                    try
                    {
                        OptF = new OptionForm(Repository);
                        OptF.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        ErrorCodes.ShowException(ErrorCodes.ERROR_031, ex);
                    }
                    finally // ABA20230401
                    {
                        if(OptF != null && !OptF.IsDisposed) OptF.Dispose();
                    }
                    break;
                case "About...":
                    Option.AboutForm anAbout = new Option.AboutForm();
                    anAbout.FillForm(version, ConstantDefinition.GetExpirationCheckState(), licKey);
                    anAbout.ShowDialog();
                    anAbout.Dispose();
                    break;
                case "ReplaceSimpleFloatByFloat":
                    Utilities.Utilities utilit = new Utilities.Utilities(Repository);
                    msgBox = new MsgBox();
                    msgBox.ShowBox("Replacing floats...");
                    try
                    {
                        EA.Package profile = Repository.GetTreeSelectedPackage();
                        utilit.replaceSimpleByFloat(profile);
                    }
                    catch (Exception e)
                    {
                        CimContextor.utilitaires.Utilitaires utill = new Utilitaires(Repository);
                        utill.wlog("Error", "Pb in ReplaceSimpleFloatByFloat " + e.Message);
                        MessageBox.Show(" end with error");
                    } finally
                    {
                        msgBox.CloseBox();
                    }
                    break;
                case "LocalizeDataTypes":
                    Utilities.Utilities utility = new Utilities.Utilities(Repository);
                    msgBox = new MsgBox();
                    msgBox.ShowBox("Localizing data types...");
                    try
                    {
                        EA.Package profile = Repository.GetTreeSelectedPackage();
                        int dim = profile.Elements.Count;
                        utility.LocalizeDataTypes(profile);
                    }
                    catch (Exception e)
                    {
                        CimContextor.utilitaires.Utilitaires utilll = new CimContextor.utilitaires.Utilitaires(Repository);
                        utilll.wlog("Error", "Pb in LocalizeDataTypes " + e.Message);
                        MessageBox.Show(" end with error");
                    } finally
                    {
                        msgBox.CloseBox();
                    }
                    break;
                case "CopyAllNotes":
                    Utilities.Utilities utilitys = new Utilities.Utilities(Repository);
                    msgBox = new MsgBox();
                    msgBox.ShowBox("Copying...");
                    try
                    {
                        copyAllNotes can = new copyAllNotes(Repository, Repository.GetTreeSelectedPackage());
                        //MessageBox.Show("Not yet implemented");
                    }
                    catch (Exception e)
                    {
                        CimContextor.utilitaires.Utilitaires utilll = new CimContextor.utilitaires.Utilitaires(Repository);
                        utilll.wlog("Error", "Pb in LocalizeDataTypes " + e.Message);
                        MessageBox.Show(" end with error");
                    } finally
                    {
                        msgBox.CloseBox();
                    }
                    break;
                case "RecoverProfileDatatypes":
                    msgBox = new MsgBox();
                    Utilitaires util = new Utilitaires(Repository);
                    if (!util.HasIBOPackage(Repository.GetTreeSelectedPackage().PackageID))
                    {
                        MessageBox.Show(" The selected package must be a profile");
                        return;
                    }
                    try
                    {
                        msgBox.ShowBox("Recovering...");
                        GlobalRecoverDatatypes proff = new GlobalRecoverDatatypes(Repository, Repository.GetTreeSelectedPackage());
                        Repository.RefreshOpenDiagrams(false);
                        if (GlobalIBOCopy.Errorspresent) MessageBox.Show(" There were somme errors: look logs in createglobalprofile.txt");
                    }
                    finally 
                    {
                        msgBox.CloseBox();
                    }
                    break;
                case "MakeMembersMandatory":
                    //ATTENTION au depard ce module devait traiter des AsociationEnds : 
                    // ce nom est reste dans la denomination dela classe meme si en fait celle ci traite des attributs
                    msgBox = new MsgBox();
                    try
                    {
                        msgBox.ShowBox("Processing...");
                        MakeAssociationEndsMandatory mprof = new MakeAssociationEndsMandatory(Repository, Repository.GetTreeSelectedPackage());
                        Repository.RefreshOpenDiagrams(false);
                    } finally
                    {
                        msgBox.CloseBox();
                    }
                    break;
                case SORT_ELEMENTS_MENU:
                    msgBox = new MsgBox();
                    ElementSorter elemSorter = new ElementSorter(Repository);
                    try
                    {
                        msgBox.ShowBox("Sorting...");
                        elemSorter.SortElementsInPackage();
                    } finally
                    {
                        msgBox.CloseBox();
                    }
                    break;
                case "CreateGlobalProfile":
                    Utilities.Utilities utilitt = new Utilities.Utilities(Repository);
                    msgBox = new MsgBox();
                    try
                    {
                        EA.ObjectType typ = Repository.GetTreeSelectedItemType();
                        switch (typ)
                        {
                            case EA.ObjectType.otDiagram:
                                msgBox.ShowBox("Processing...");
                                diag = (EA.Diagram)Repo.GetTreeSelectedObject();
                                msgBox.CloseBox();
                                CimContextor.Utilities.GlobalIBOCopyDiag pdiag = new CimContextor.Utilities.GlobalIBOCopyDiag(Repository, diag, null, null);
                                break;
                            case EA.ObjectType.otPackage:
                                msgBox.ShowBox("Processing...");
                                EA.Package profile = Repository.GetTreeSelectedPackage();
                                msgBox.CloseBox();
                                CimContextor.Utilities.GlobalIBOCopy prof = new CimContextor.Utilities.GlobalIBOCopy(Repository, Repository.GetTreeSelectedPackage(), null, null);
                                Repository.RefreshOpenDiagrams(false);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        CimContextor.utilitaires.Utilitaires utilv = new CimContextor.utilitaires.Utilitaires(Repository);
                        utilv.wlog("Error", "Pb in  GlobalCopyProfile " + e.Message);
                        MessageBox.Show(" end with error");
                    }
                    break;
            }
        }

        private bool IsNewElement(string elemName)
        {
            int n = 0;
            string num = null;
            if (elemName.StartsWith("Class"))
            {
                num = elemName.Substring("Class".Length);
            } else if(elemName.StartsWith("Enumeration"))
            {
                num = elemName.Substring("Enumeration".Length);
            }
            else if(elemName.StartsWith("PrimitiveType"))
            {
                num = elemName.Substring("PrimitiveType".Length);
            }
            else if(elemName.StartsWith("Type"))
            {
                num = elemName.Substring("Type".Length);
            }
            if (num != null)
            {
                return int.TryParse(num, out n);
            }

            return false;
        }
        //Called before a new object is added to a diagram
        public bool EA_OnPreNewDiagramObject(EA.Repository repo, EA.EventProperties ep)
        {
            ConstantDefinition CD = new ConstantDefinition();
            Repo = repo; //am sept 2013

            XMLParser XMLP = new XMLParser(repo);
            if (XMLP.GetXmlValueConfig("IsBasedOn") == (ConfigurationManager.CHECKED))
            {
                bool classType = false;
                foreach (EA.EventProperty aProp in ep)
                {
                    //Checking if the Event property comme from the drag of a class
                    string prov1 = aProp.Name;
                    string prov2 = (string)aProp.Value;
                    if (
                        (aProp.Name.Equals("Type"))
                        && (
                        (aProp.Value.Equals("Class"))
                        ||
                        (aProp.Value.Equals("Enumeration"))
                        ||
                        (aProp.Value.Equals("PrimitiveType"))
                        )
                        )
                    {
                        classType = true;
                    }

                    if (classType.Equals(true))
                    {
                        //If ID = 0 then it's a new object,that's not currently in the repository
                        if ((aProp.Name.Equals("ID")) && (!aProp.Value.Equals("0")))
                        {
                            int SelectedElementID = int.Parse((string)aProp.Value);
                            if (XMLP.GetXmlValueConfig("IsBasedOn") == (ConfigurationManager.CHECKED))
                            {
                                if (XMLP.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
                                {
                                    XMLP.AddXmlLog("IsBasedOnStart", "A valid item have been dragged to a diagram");
                                }

                                IsBasedOnClass iboc = new IsBasedOnClass(repo, ep);

                                if (iboc.BeforeIsBasedOn().Equals(false)) // isBasedOn Dialog call 
                                {
                                    ExecuteIBO = true;
                                    objIsDeleted = false;
                                    //méthode pour détruire l'objet du diagrame
                                    EA.Collection ObjectList = repo.GetCurrentDiagram().DiagramObjects;
                                    if (ObjectList.Count > 0)
                                    {
                                        short CptToDelete = -1;
                                        int ObjectID = 0;
                                        string diagrObjName = "";
                                        for (short i = 0; ObjectList.Count > i; i++)
                                        {
                                            if (((EA.DiagramObject)ObjectList.GetAt(i)).ElementID.Equals(SelectedElementID))
                                            {
                                                ObjectLeft = ((EA.DiagramObject)ObjectList.GetAt(i)).left;
                                                ObjectRight = ((EA.DiagramObject)ObjectList.GetAt(i)).right;
                                                ObjectTop = ((EA.DiagramObject)ObjectList.GetAt(i)).top;
                                                ObjectBottom = ((EA.DiagramObject)ObjectList.GetAt(i)).bottom;
                                                ObjectStyle = ((EA.DiagramObject)ObjectList.GetAt(i)).Style.ToString();
                                                //
                                                ObjectID = ((EA.DiagramObject)ObjectList.GetAt(i)).ElementID;
                                                diagrObjName = repo.GetElementByID(ObjectID).Name;
                                                //
                                                ObjectIsHere = true;
                                                CptToDelete = i;
                                                break;
                                            }
                                        }
                                        if (!CptToDelete.Equals(-1)) {
                                            if(IsNewElement(diagrObjName))
                                            {
                                                MessageBox.Show("Element " + diagrObjName + " will be deleted from model!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                EAUtilities.DeleteElementFromCurrentDiagram(repo, CptToDelete);
                                                repo.ReloadDiagram(repo.GetCurrentDiagram().DiagramID);
                                                EAUtilities.DeleteElement(repo, repo.GetElementByID(ObjectID));
                                            } else
                                            {
                                                MessageBox.Show("Element " + diagrObjName + " will be deleted from diagram!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                EAUtilities.DeleteElementFromCurrentDiagram(repo, CptToDelete);
                                                repo.ReloadDiagram(repo.GetCurrentDiagram().DiagramID);
                                            }
                                            objIsDeleted = true; // ABA20230112
                                        }
                                    }
                                    return true;
                                }
                                else
                                {
                                    ExecuteIBO = false;
                                    ObjectIsHere = false;
                                    return true;
                                }
                            }
                            else { return true; }
                        }
                    }
                }
            }
            return true;
        }


        public bool EA_OnPostNewDiagramObject(EA.Repository repo, EA.EventProperties ep)
        {
            if (ExecuteIBO.Equals(true) && !objIsDeleted)
            {
                IsBasedOnClass iboc = new IsBasedOnClass(repo, ep);
                iboc.ExecuteIsBasedOn(ObjectTop, ObjectBottom, ObjectRight, ObjectLeft, ObjectStyle, ObjectIsHere);
                ExecuteIBO = false;
                ObjectIsHere = false;
            }
            else
            {
                ExecuteIBO = false;
                ObjectIsHere = false;
            }
            return false;
        }


        /*
        public void EA_OnNotifyContextItemModified(EA.Repository repository, string GUID, EA.ObjectType ot)
        {
            try
            {
                if (ot == EA.ObjectType.otElement)
                {
                    // Works only with the properties dialogue opened by double click on element. 
                    // Doesn't work when using the properties window that is always open and showing element's properties!
                    // ABA20231109
                    EA.Element elem = repository.GetElementByGuid(GUID);
                    if (elem != null && elem.Type.Equals("Class") && !elem.Name.Equals("Configuration"))
                    {
                        elem.Update();
                        if (elem.Stereotype.Contains(Constants.DESCRIPTION) || elem.StereotypeEx.Contains(Constants.DESCRIPTION))
                        {
                            EA.Collection tagValues = elem.TaggedValues;
                            EA.TaggedValue newTV = (EA.TaggedValue)tagValues.AddNew(Constants.RDF_ABOUT, "");
                            newTV.Value = "true"; // ABA20230712
                            newTV.Update();
                            tagValues.Refresh();
                            elem.Update();
                            elem.Refresh();
                        }
                    }
                    else
                    {
                        EA.Diagram currDiagram = repository.GetCurrentDiagram();
                        if (currDiagram == null) return;
                        foreach (EA.DiagramObject dObj in currDiagram.DiagramObjects)
                        {
                            bool found = false;
                            EA.Element elt = repository.GetElementByID(dObj.ElementID);
                            if (elt != null)
                            {
                                elt.Update();
                                elt.Refresh();
                                if (elt.StereotypeEx.Contains(Constants.DESCRIPTION))
                                {
                                    foreach (EA.TaggedValue tv in elt.TaggedValues)
                                    {
                                        if (tv.Name.Equals(Constants.RDF_ABOUT)) found = true;
                                    }
                                    if (!found)
                                    {
                                        EA.Collection tagValues = elt.TaggedValues;
                                        EA.TaggedValue newTV = (EA.TaggedValue)tagValues.AddNew(Constants.RDF_ABOUT, "");
                                        newTV.Value = "true";
                                        newTV.Update();
                                        tagValues.Refresh();
                                        elt.Update();
                                        elt.Refresh();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCodes.ShowException(ErrorCodes.ERROR_044, ex);
            }
        }
        */

        public void EA_OnContextItemChanged(EA.Repository repository, string GUID, EA.ObjectType ot)
        {
            if (ot == EA.ObjectType.otElement)
            {
                EA.Element elem = repository.GetElementByGuid(GUID);
                if (elem != null && elem.Type.Equals("Class") &&
                    elem.Name.Equals(ConfigurationManager.CONFIG_ELEMENT))
                {
                    EA.Package pack = repository.GetPackageByID(elem.PackageID);
                    if (pack != null && pack.Name.Equals(ConfigurationManager.CONFIG_PACKAGE))
                    {
                        MessageBox.Show("Don't modify this element!\nYou might corrupt CimConteXtor's configuration!",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }
                }
            }
        }

        public bool EA_OnPreDeletePackage(EA.Repository repository, EA.EventProperties evProps)
        {
            foreach (EA.EventProperty aProp in evProps)
            {
                string propName = aProp.Name;
                string propValue = (string)aProp.Value;
                if(propName.Equals("PackageID"))
                {
                    EA.Package pack = repository.GetPackageByID(Int32.Parse(propValue));
                    if(pack.Name.Equals(ConfigurationManager.CONFIG_PACKAGE))
                    {
                        DialogResult result = MessageBox.Show("Deleting this package will remove CimTeXtor's specific cnfiguration!", 
                                                              "Warning", 
                                                              MessageBoxButtons.OKCancel, 
                                                              MessageBoxIcon.Warning);
                        if (result == DialogResult.OK)
                        {
                            ConfigurationManager.GetConfigurationManager(repository).LoadDefaultConfiguration();
                            return true;
                        }
                        else return false;
                    }
                } 
            }
            return true;
        }
        
        public bool EA_OnPreDeleteElement(EA.Repository repository, EA.EventProperties evProps)
        {
            foreach (EA.EventProperty aProp in evProps)
            {
                string propName = aProp.Name;
                string propValue = (string)aProp.Value;
                if (propName.Equals("ElementID"))
                {
                    EA.Element elem = repository.GetElementByID(Int32.Parse(propValue));
                    if (elem.Name.Equals(ConfigurationManager.CONFIG_ELEMENT))
                    {
                        DialogResult result = MessageBox.Show("Deleting this element will remove CimTeXtor's specific cnfiguration!",
                                                              "Warning",
                                                              MessageBoxButtons.OKCancel,
                                                              MessageBoxIcon.Warning);
                        if (result == DialogResult.OK)
                        {
                            ConfigurationManager.GetConfigurationManager(repository).LoadDefaultConfiguration();
                            return true;
                        }
                        else return false;
                    }
                }
            }
            return true;
        }

        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}




