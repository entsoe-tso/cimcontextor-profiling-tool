using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using CimContextor.Configuration;
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
namespace CimContextor
{
    public class EditEAClassConnector
    {
        private ConstantDefinition CD = new ConstantDefinition();
        private string GUID;
        private int SubID;
        private EA.Connector OriginalConnector;
        private EA.Element SelectedElementConnector;
        private EA.Element TargetedElementConnector;
        private EA.Element SelectedIBOElement;
        private EA.Element TargetedIBOElement;

        private bool Inherited =false;
        private ArrayList EditedConnectorList = new ArrayList();
        private EA.Connector EditedConnector = null;
        private utilitaires.Ta ta; //am
        private bool ClientContainmentByRef = false;
        private bool SupplierContainmentByRef = false;

        private string SupplierRole = "";
        private string SupplierRoleQualifier = "";
        private string SupplierNote = "";

        private EA.Collection SupplierTaggedValue;
        private bool CopyTagValues = true;
        private bool CopyNotes = true;
        private bool CopyStereotype = true;
        private bool CopyConstraint = true;

        private string Agregate = "";
        private string Stereotype = "";
        private string ClientRole = "";
        private string ClientRoleQualifier = "";
        private string ClientNote = "";
        private EA.Collection ClientTaggedValue;
        private string ClientLBCardinality = "";
        private string ClientUBCardinality = "";
        private string SupplierLBCardinality = "";
        private string SupplierUBCardinality = "";
        private EA.Collection TaggedValue;
        private EA.Collection Constraints;
        private string Notes = "";
        private string Name;
        private string Type;
        private string Direction="Unspecified";
        private bool SelectedState = true;
        private bool Switch;
        private EA.Repository Repo;
        private ArrayList SubConnectorsList = new ArrayList();
        private bool InitializationSucceded = true;
        private string FailedInitReason;
        //private int OriginalPackageID;

        //The value of the first IBOPackage of the selected element
        private int SelectedIBOParentPackage = -1;
        //private int SelectedIBOIBOParentPackage=-1;
        private int TargetedIBOParentPackage = -1;
        //----------------------------------------
        XMLParser XMLP = null;
        public static EA.Connector ncon;  //am 14/10/15
        public static string nconguid = "";  //am 14/10/15
        utilitaires.Utilitaires util; // am mars 2016
        bool isnav = false;
        bool isSimpleProfiling = true;
        //----------------------------------------
        /// <summary>
        /// Deprecated constructor:
        /// <param name="Repo"></param>
        /// <param name="SubConnector"></param>
        /// <param name="ConnectorGUID">The Original connectors GUID to dupplicate</param>
        /// <param name="OriginalElementID">The element ID of the selected side object</param>
        /// <param name="OriginalTargetedElementID"></param>
        /// <param name="SubID"></param>
        /// <param name="OriginToTarget"></param>
        /// <param name="PackageID"></param>
        /// <param name="ChildSelectedItem"></param>
        /// </summary>
        /// <param name="Repo"></param>
        /// <param name="SubConnector">true if it's a subconnector</param>
        /// <param name="SubID">The ID of the SubConnector</param>
        /// <param name="ConnectorGUID">The Original connectors GUID to dupplicate</param>
        /// <param name="SelectedElementConnector">The element of the object on the connector's selected side</param>
        /// <param name="TargetedElementConnector">The element of the object on the connector's targeted (other end) side</param>
        /// <param name="SelectedIBOElement">The element of the IBO's user selected object</param>
        /// <param name="TargetedIBOElement">The element of the IBO's targeted (other end) object</param>                                   
        public EditEAClassConnector(EA.Repository Repo, bool SubConnector, int SubID, string ConnectorGUID, EA.Element SelectedElementConnector, EA.Element TargetedElementConnector, EA.Element SelectedIBOElement, EA.Element TargetedIBOElement, bool Switch, ArrayList EditedConnectorList)
        {
            this.Repo = Repo;   
            XMLP = new XMLParser(Repo);//am 14/10/15
            ta = new utilitaires.Ta();
            isnav =  XMLP.GetXmlValueConfig("NavigationEnabled") == ConfigurationManager.CHECKED;
            isSimpleProfiling = XMLP.GetXmlValueConfig("SimpleProfiling") == ConfigurationManager.CHECKED;
            #region subconnectorFalse
            ta.start("EditEAClassConnectorSubfalse");
            if (SubConnector.Equals(false))
            {
                this.GUID = ConnectorGUID;
                this.Repo = Repo;
                this.util = new utilitaires.Utilitaires(this.Repo);// am mars 2016
                this.Switch = Switch;
                this.EditedConnectorList = EditedConnectorList;
                OriginalConnector = Repo.GetConnectorByGuid(ConnectorGUID);

                this.SelectedElementConnector = SelectedElementConnector;
                this.TargetedElementConnector = TargetedElementConnector;
                this.SelectedIBOElement = SelectedIBOElement;
                this.TargetedIBOElement = TargetedIBOElement;

                if (EditedConnectorList == null)
                {
                    EditedConnectorList = new ArrayList();
                }

                if (Switch.Equals(false))
                {
                    this.SetClientRole(OriginalConnector.ClientEnd.Role);
                    this.SetClientNote(OriginalConnector.ClientEnd.RoleNote);
                    this.SetClientTaggedValue(OriginalConnector.ClientEnd.TaggedValues);
                    this.SetSupplierRole(OriginalConnector.SupplierEnd.Role);
                    this.SetSupplierNote(OriginalConnector.SupplierEnd.RoleNote);
                    this.SetSupplierTaggedValue(OriginalConnector.SupplierEnd.TaggedValues);
                }
                else
                {
                    this.SetClientRole(OriginalConnector.SupplierEnd.Role);
                    this.SetClientNote(OriginalConnector.SupplierEnd.RoleNote);
                    this.SetClientTaggedValue(OriginalConnector.SupplierEnd.TaggedValues);
                    this.SetSupplierRole(OriginalConnector.ClientEnd.Role);
                    this.SetSupplierNote(OriginalConnector.ClientEnd.RoleNote);
                    this.SetSupplierTaggedValue(OriginalConnector.ClientEnd.TaggedValues);
                }

                this.SetName(OriginalConnector.Name);
                this.Constraints = OriginalConnector.Constraints;
                this.SetType(OriginalConnector.Type);
                this.SetDirection(OriginalConnector.Direction);
                this.Stereotype = OriginalConnector.Stereotype;
                this.SetNotes(OriginalConnector.Notes);



                if (Switch.Equals(false))
                {
                    #region Cardinality
                    if (OriginalConnector.ClientEnd.Cardinality.Contains(".."))
                    {
                        string[] Values = OriginalConnector.ClientEnd.Cardinality.Split("..".ToCharArray());
                        if (Values.Length > 0)
                        {
                            this.SetClientLBCardinality(Values[0]);
                        }
                        if (Values.Length == 3)
                        {
                            this.SetClientUBCardinality(Values[2]);
                        }
                    }
                    else
                    {
                        this.SetClientLBCardinality(OriginalConnector.ClientEnd.Cardinality);
                    }


                    if (OriginalConnector.SupplierEnd.Cardinality.Contains(".."))
                    {
                        string[] Values = OriginalConnector.SupplierEnd.Cardinality.Split("..".ToCharArray());
                        if (Values.Length > 0)
                        {
                            this.SetSupplierLBCardinality(Values[0]);
                        }
                        if (Values.Length == 3)
                        {
                            this.SetSupplierUBCardinality(Values[2]);
                        }
                    }
                    else
                    {
                        this.SetSupplierLBCardinality(OriginalConnector.SupplierEnd.Cardinality);
                    }
                    #endregion
                }
                else
                {
                    #region Cardinality
                    if (OriginalConnector.SupplierEnd.Cardinality.Contains(".."))
                    {
                        string[] Values = OriginalConnector.SupplierEnd.Cardinality.Split("..".ToCharArray());
                        if (Values.Length > 0)
                        {
                            this.SetClientLBCardinality(Values[0]);
                        }
                        if (Values.Length == 3)
                        {
                            this.SetClientUBCardinality(Values[2]);
                        }
                    }
                    else
                    {
                        this.SetClientLBCardinality(OriginalConnector.SupplierEnd.Cardinality);
                    }


                    if (OriginalConnector.ClientEnd.Cardinality.Contains(".."))
                    {
                        string[] Values = OriginalConnector.ClientEnd.Cardinality.Split("..".ToCharArray());
                        if (Values.Length > 0)
                        {
                            this.SetSupplierLBCardinality(Values[0]);
                        }
                        if (Values.Length == 3)
                        {
                            this.SetSupplierUBCardinality(Values[2]);
                        }
                    }
                    else
                    {
                        this.SetSupplierLBCardinality(OriginalConnector.ClientEnd.Cardinality);
                    }
                    #endregion
                }
                this.SetTaggedValue(OriginalConnector.TaggedValues);

                InitializationSucceded = initElement();
                ta.reset(); 
                #region generateSubconnectors
                ta.start("EditEAClassConnectorSubtrue");
                if (InitializationSucceded.Equals(true))
                {
                    if (EditedConnectorList.Count == 1)
                    {
                        bool Result = CheckIfSameConnector();
                        if (Result.Equals(false))
                        {
                            for (short i = 0; EditedConnectorList.Count > i; i++)
                            {
                                ArrayList AnEditedConnectorList = new ArrayList();
                                AnEditedConnectorList.Add(EditedConnectorList[i]);
                                this.AddASubConnector(new EditEAClassConnector(Repo, true, 0, OriginalConnector.ConnectorGUID, SelectedElementConnector, TargetedElementConnector, SelectedIBOElement, TargetedIBOElement, false, EditedConnectorList));
                                this.SelectedState = true;
                            }
                        }
                        else
                        {
                            EditedConnector = (EA.Connector)EditedConnectorList[0];
                            this.SelectedState = true;
                        }
                    }
                    else if (EditedConnectorList.Count > 1)
                    {
                        for (short i = 0; EditedConnectorList.Count > i; i++)
                        {
                            ArrayList EditedConnector = new ArrayList();
                            EditedConnector.Add(EditedConnectorList[i]);
                            this.AddASubConnector(new EditEAClassConnector(Repo, true, 0, OriginalConnector.ConnectorGUID, SelectedElementConnector, TargetedElementConnector, SelectedIBOElement, TargetedIBOElement, Switch, EditedConnector));
                            this.SelectedState = true;
                        }
                    }
                    else
                    {
                        this.SelectedState = false;
                    }
                }
                ta.reset();
                #endregion
            }
            #endregion
            
            
            #region SubConnectorTrue
            else
            {
                this.GUID = ConnectorGUID;
                this.Repo = Repo;
                this.util = new utilitaires.Utilitaires(this.Repo);// am mars 2016
                this.Switch = Switch;
                this.EditedConnectorList = EditedConnectorList;
                OriginalConnector = Repo.GetConnectorByGuid(ConnectorGUID);

                this.SelectedElementConnector = SelectedElementConnector;
                this.TargetedElementConnector = TargetedElementConnector;
                this.SelectedIBOElement = SelectedIBOElement;
                this.TargetedIBOElement = TargetedIBOElement;

                if (EditedConnectorList == null)
                {
                    EditedConnectorList = new ArrayList();
                }

                this.SubID = SubID;

                if (EditedConnectorList.Count > 0)
                {
                        EditedConnector = (EA.Connector)EditedConnectorList[0];
                        InitFromEditedConnector();
                }
                else if ((EditedConnectorList.Count == 0) && (EditedConnector == null))
                {

                    #region Data
                    if (Switch.Equals(false))
                    {
                        this.SetClientRole(OriginalConnector.ClientEnd.Role);
                        this.SetClientNote(OriginalConnector.ClientEnd.RoleNote);
                        this.SetClientTaggedValue(OriginalConnector.ClientEnd.TaggedValues);
                        this.SetSupplierRole(OriginalConnector.SupplierEnd.Role);
                        this.SetSupplierNote(OriginalConnector.SupplierEnd.RoleNote);
                        this.SetSupplierTaggedValue(OriginalConnector.SupplierEnd.TaggedValues);
                    }
                    else
                    {
                        this.SetClientRole(OriginalConnector.SupplierEnd.Role);
                        this.SetClientNote(OriginalConnector.SupplierEnd.RoleNote);
                        this.SetClientTaggedValue(OriginalConnector.SupplierEnd.TaggedValues);
                        this.SetSupplierRole(OriginalConnector.ClientEnd.Role);
                        this.SetSupplierNote(OriginalConnector.ClientEnd.RoleNote);
                        this.SetSupplierTaggedValue(OriginalConnector.ClientEnd.TaggedValues);
                    }

                    this.SetName(OriginalConnector.Name);
                    this.Constraints = OriginalConnector.Constraints;
                    this.SetType(OriginalConnector.Type);
                    this.SetDirection(OriginalConnector.Direction);
                    this.Stereotype = OriginalConnector.Stereotype;
                    this.SetNotes(OriginalConnector.Notes);
                    #endregion

                    this.SelectedState = false;
                }
            }
            #endregion
        }

        public bool CheckIfSameConnector()
        {
           
            EA.Connector AnEditedConnector= ((EA.Connector)EditedConnectorList[0]);

            if (!(AnEditedConnector.Name.Equals(this.GetName())))
            {
                return false;
            }
            else if (!(AnEditedConnector.Notes.Equals(this.GetNotes())))
            {
                return false;
            }
            else if (!(AnEditedConnector.Stereotype.Equals(this.GetStereotype())))
            {
                return false;
            }
            else if (!(AnEditedConnector.ClientEnd.Cardinality.Equals( this.GetEAFormatCardinality(this.GetClientLBCardinality(), this.GetClientUBCardinality()))))
            {
                return false;
            }
            else if (!(AnEditedConnector.SupplierEnd.Cardinality.Equals(this.GetEAFormatCardinality(this.GetSupplierLBCardinality(),this.GetSupplierUBCardinality()))))
            {
                return false;
            }
            else if (!(AnEditedConnector.ClientEnd.Role.Equals(this.GetEAFormatRole(this.GetClientRoleQualifier(),this.GetClientRole()))))
            {
                return false;
            }
            else if (!(AnEditedConnector.SupplierEnd.Role.Equals(this.GetEAFormatRole(this.GetSupplierRoleQualifier(),this.GetSupplierRole()))))
            {
                return false;
            }

            if (this.Agregate.Equals(""))
            {
                if (isnav)
                {
                    if (!((AnEditedConnector.ClientEnd.Navigable.Equals("Unspecified")) && (AnEditedConnector.SupplierEnd.Navigable.Equals("Unspecified"))))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!((AnEditedConnector.ClientEnd.Aggregation.Equals(0)) && (AnEditedConnector.SupplierEnd.Aggregation.Equals(0))))
                    {
                        return false;
                    }
                }
            }
            else if ( this.Agregate.Equals("SOURCE"))
            {
                if ((!isnav && !(AnEditedConnector.ClientEnd.Aggregation.Equals(1)))
                    || (isnav && !(AnEditedConnector.SupplierEnd.Navigable.Equals(ConfigurationManager.CHECKED)))
                    )
                {
                    return false;
                }
            }
            else if (this.Agregate.Equals("TARGET"))
            {
                if ((!isnav && !(AnEditedConnector.SupplierEnd.Aggregation.Equals(1)))
                    || (isnav && !(AnEditedConnector.ClientEnd.Navigable.Equals(ConfigurationManager.CHECKED)))
                    )
                {
                    return false;
                }
            }
            string texte="EDCC CheckIfSame AnEditdConnector / EditedConnectorList[0]) refclient/refclient=" ;
            texte=texte+ AnEditedConnector.ClientEnd.Containment+ "/" + AnEditedConnector.SupplierEnd.Containment;
            texte=texte+ " avec AnEditedConnector  " +AnEditedConnector.ClientEnd.Containment+ "/" + AnEditedConnector.SupplierEnd.Containment;
            // util.wlog("TEST",texte); 
            if( this.GetClientContainmentByRef().Equals(true)  ){
                  if (!AnEditedConnector.ClientEnd.Containment.Equals("Reference")) //am mars 2016
                {
                    return false;
                }
            }
            else//am mars 2016
            {
                if (AnEditedConnector.ClientEnd.Containment.Equals("Reference"))//am mars 2016

                {
                    return false;
                }
            }

            if (this.GetSupplierContainmentByRef().Equals(true))
            {
                if (!AnEditedConnector.SupplierEnd.Containment.Equals("Reference"))//am mars 2016
                {
                    return false;
                }
            }
            else//am mars 2016
            {
                    if (AnEditedConnector.SupplierEnd.Containment.Equals("Reference"))// am mars 2016
                {
                    return false;
                }
            }

            return true;
        }


        public bool GetInherited()
        {
            return Inherited;
        }
        public void SetInherited( bool Value){
            this.Inherited = Value;
        }

        private ArrayList SubStringQualifier(string FullRole)
        {
            ArrayList SplitedQualifier = new ArrayList();
            if (!(FullRole == ""))
            {
                string[] tmp = FullRole.Split("_".ToCharArray());

                if (tmp.Length < 2)
                {
                    SplitedQualifier.Add("");
                    SplitedQualifier.Add(FullRole);
                }
                else if (tmp.Length == 2)
                {
                    SplitedQualifier.Add(tmp[0]);
                    SplitedQualifier.Add(tmp[1]);
                }
                else
                {
                    SplitedQualifier.Add(tmp[0]);
                    string ReconstructedString = "";
                    for (short i = 1; tmp.Length > i; i++)
                    {
                        if (!(ReconstructedString == ""))
                        {
                            ReconstructedString = ReconstructedString + "_" + tmp[i];
                        }
                        else
                        {
                            ReconstructedString = tmp[i];
                        }
                    }
                    SplitedQualifier.Add(ReconstructedString);
                }

            }
            else
            {
                SplitedQualifier.Add("");
                SplitedQualifier.Add("");
            }
            return SplitedQualifier;
        }

        /// <summary>
        /// No need to check switch as i'm initializing from an existing connector that 
        /// have got his value turned if needed
        /// </summary>
        private void InitFromEditedConnector()
        {

            //
            if(EditedConnector.SupplierEnd.Role.Equals(OriginalConnector.SupplierEnd.Role)){
                this.SetSupplierRoleQualifier("");
                this.SetSupplierRole(OriginalConnector.SupplierEnd.Role);
            }else{
                ArrayList SubstringedQualifier = new ArrayList();
                SubstringedQualifier = this.SubStringQualifier(EditedConnector.SupplierEnd.Role);


                this.SetSupplierRoleQualifier(SubstringedQualifier[0].ToString());
                this.SetSupplierRole(SubstringedQualifier[1].ToString());
            }

            if (EditedConnector.ClientEnd.Role.Equals(OriginalConnector.ClientEnd.Role))
            {
                this.SetClientRoleQualifier("");
                this.SetClientRole(OriginalConnector.ClientEnd.Role);
            }
            else
            {
                ArrayList SubstringedQualifier = new ArrayList();
                SubstringedQualifier = this.SubStringQualifier(EditedConnector.ClientEnd.Role);

                this.SetClientRoleQualifier(SubstringedQualifier[0].ToString());
                this.SetClientRole(SubstringedQualifier[1].ToString());
            }
            //
        
                if ((!isnav && EditedConnector.ClientEnd.Aggregation.Equals(1))
                    || (isnav && EditedConnector.SupplierEnd.Navigable.Equals("Navigable"))
                    )
                {
                    this.SetAgregate("SOURCE");
                }
                if ((!isnav && EditedConnector.SupplierEnd.Aggregation.Equals(1))
                   ||  (isnav && (isnav && EditedConnector.ClientEnd.Navigable.Equals("Navigable")))
                    )
                {
                    this.SetAgregate("TARGET");
                }

                if(EditedConnector.ClientEnd.Containment.Equals("Reference")){
                    this.SetClientContainmentByRef(true);
                }
                if (EditedConnector.SupplierEnd.Containment.Equals("Reference"))
                {
                    this.SetSupplierContainmentByRef(true);
                }

                this.SetName(EditedConnector.Name);
                this.Constraints = EditedConnector.Constraints;
                this.SetType(EditedConnector.Type);
                this.SetDirection(EditedConnector.Direction);
                this.Stereotype = EditedConnector.Stereotype;
                this.SetNotes(EditedConnector.Notes);


                this.SetClientNote(EditedConnector.ClientEnd.RoleNote);
                this.SetClientTaggedValue(EditedConnector.ClientEnd.TaggedValues);
                this.SetSupplierNote(EditedConnector.SupplierEnd.RoleNote);
                this.SetSupplierTaggedValue(EditedConnector.SupplierEnd.TaggedValues);

                #region Cardinality
                if (EditedConnector.ClientEnd.Cardinality.Contains(".."))
                {
                    string[] Values = EditedConnector.ClientEnd.Cardinality.Split("..".ToCharArray());
                    if (Values.Length > 0)
                    {
                        this.SetClientLBCardinality(Values[0]);
                    }
                    if (Values.Length == 3)
                    {
                        this.SetClientUBCardinality(Values[2]);
                    }
                }
                else
                {
                    this.SetClientLBCardinality(EditedConnector.ClientEnd.Cardinality);
                }


                if (EditedConnector.SupplierEnd.Cardinality.Contains(".."))
                {
                    string[] Values = EditedConnector.SupplierEnd.Cardinality.Split("..".ToCharArray());
                    if (Values.Length > 0)
                    {
                        this.SetSupplierLBCardinality(Values[0]);
                    }
                    if (Values.Length == 3)
                    {
                        this.SetSupplierUBCardinality(Values[2]);
                    }
                }
                else
                {
                    this.SetSupplierLBCardinality(EditedConnector.SupplierEnd.Cardinality);
                }
                #endregion
  
            this.SetTaggedValue(EditedConnector.TaggedValues);
        }

        private void initPackage(bool SelectedElement)
        {
            int ParentPackage;
            if (SelectedElement.Equals(true))
            {
                ParentPackage = SelectedIBOElement.PackageID;
            }
            else
            {
                ParentPackage = TargetedIBOElement.PackageID;
            }

            EA.Connector AConnector = null;
            bool Search = true;
            while (!ParentPackage.Equals(0))
            {
                for (short i = 0; Repo.GetPackageByID(ParentPackage).Connectors.Count > i; i++)
                {

                    if (ParentPackage.Equals(0))
                    {
                        Search = false;
                        break;
                    }
                    AConnector = (EA.Connector)Repo.GetPackageByID(ParentPackage).Connectors.GetAt(i);

                    if (AConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower()))
                    {
                        int CheckedPackage = -1;
                        try
                        {
                            CheckedPackage = Repo.GetPackageByGuid(Repo.GetElementByID(AConnector.ClientID).ElementGUID).PackageID;
                        }
                        catch (Exception)
                        {
                            ParentPackage = 0;
                            break;
                        }

                        if (CheckedPackage.Equals(ParentPackage))
                        {
                            if (SelectedElement.Equals(true))
                            {
                                Search = false;
                                SelectedIBOParentPackage = ParentPackage;
                                break;
                            }
                            else
                            {
                                Search = false;
                                TargetedIBOParentPackage = ParentPackage;
                                break;
                            }

                        }
                    }
                }
                ParentPackage = Repo.GetPackageByID(ParentPackage).ParentID;
                if (Search.Equals(false))
                {
                    break;
                }
            }
        }

        private bool initElement()
        {
            initPackage(true);
            initPackage(false);


            if (SelectedIBOParentPackage.Equals(TargetedIBOParentPackage))
            {
                return true;
            }
            else if (SelectedIBOParentPackage == -1)
            {
                FailedInitReason = "The selected item is in an IsBasedOn package, the targeted item is not (" + SelectedIBOElement.Name + "/" + TargetedIBOElement.Name + ").";
                return false;
            }
            else if (TargetedIBOParentPackage == -1)
            {
                FailedInitReason = "The targeted item is in an IsBasedOn package, the selected item is not (" + SelectedIBOElement.Name + "/" + TargetedIBOElement.Name + ").";
                return false;
            }
            else if (!SelectedIBOParentPackage.Equals(TargetedIBOParentPackage))
            {
                FailedInitReason = "The targeted item is in a different IsBasedOn package than the selected item (" + SelectedIBOElement.Name + "/" + TargetedIBOElement.Name + ").";
                return false;
            }


            return false;
        }

        public string GetFailedInitReason()
        {
            return this.FailedInitReason;
        }
        public void SetAgregate(string NewAgregate)
        {
            this.Agregate = NewAgregate;
        }
        public string GetAgregate()
        {
            return this.Agregate;
        }
        public EA.Element GetSelectedIBOElement()
        {
            return SelectedIBOElement;
        }
        public EA.Element GetTargetedIBOElement()
        {
            return TargetedIBOElement;
        }
        public bool GetInitializationState()
        {
            return InitializationSucceded;
        }
        public EA.Element GetTargetedElementConnector()
        {
            return TargetedElementConnector;
        }
        public string GetClientRoleQualifier()
        {
            return ClientRoleQualifier;
        }
        public string GetSupplierRoleQualifier()
        {
            return SupplierRoleQualifier;
        }
        public void SetSupplierRoleQualifier(string NewQualifier)
        {
            this.SupplierRoleQualifier = NewQualifier;
        }
        public void SetClientRoleQualifier(string NewQualifier)
        {
            this.ClientRoleQualifier = NewQualifier;
        }
        public EA.Element GetSelectedElementConnector()
        {
            return this.SelectedElementConnector;
        }
        public void SetCopyStereotype(bool Value)
        {
            this.CopyStereotype = Value;
        }
        public bool GetCopyStereotype()
        {
            return this.CopyStereotype;
        }
        public void SetCopyTagValues(bool value)
        {
            this.CopyTagValues = value;
        }
        public bool GetCopyTagValues()
        {
            return CopyTagValues;
        }
        public void SetCopyConstraint(bool value)
        {
            this.CopyConstraint = value;
            if (!(SubConnectorsList == null))
            {
                foreach (EditEAClassConnector AConnector in SubConnectorsList)
                {
                    AConnector.SetCopyConstraint(value);
                }
            }
        }
        public bool GetCopyConstraint()
        {
            return this.CopyConstraint;
        }
        public void SetStereotype(string NewStereotype)
        {
            this.Stereotype = NewStereotype;
        }
        public string GetStereotype()
        {
            return this.Stereotype;
        }
        public void SetCopyNotes(bool value)
        {
            this.CopyNotes = value;
        }
        public bool GetCopyNotes()
        {
            return CopyNotes;
        }
        public void Reset()
        {
            this.SubConnectorsList = new ArrayList();
            this.SetSelectedState(true);
        }
        public int AddSubConnector()
        {
            int TMPSubID;
            TMPSubID = GetFreeSubID();
            //public EAClassConnector(EA.Repository Repo, bool SubConnector, int SubID, string ConnectorGUID, EA.Element SelectedElementConnector, EA.Element TargetedElementConnector, EA.Element SelectedIBOElement, EA.Element TargetedIBOElement)
            
           // this.SubConnectorsList.Add(new EditEAClassConnector(this.Repo, true, TMPSubID, GUID, SelectedElementConnector, TargetedElementConnector, SelectedIBOElement, TargetedIBOElement, false, null));
            this.SubConnectorsList.Add(new EditEAClassConnector(this.Repo, true, TMPSubID, GUID, SelectedElementConnector, TargetedElementConnector, SelectedIBOElement, TargetedIBOElement, this.Switch, null));//am 14/10/2015
            return TMPSubID;
        }
        public int AddASubConnector(EditEAClassConnector ASubConnector)
        {
            int TMPSubID;
            TMPSubID = GetFreeSubID();
            ASubConnector.SetSubID(TMPSubID);
            this.SubConnectorsList.Add(ASubConnector);
            return TMPSubID;
        }
        public void SetSubID(int newSubID)
        {
            this.SubID = newSubID;
        }
        public EditEAClassConnector GetSubConnector(int subID)
        {
            foreach (EditEAClassConnector ASubConnector in SubConnectorsList)
            {

                if (ASubConnector.GetSubID().Equals(subID))
                {
                    return ASubConnector;
                }
            }

            return null;
        }
        public int GetSubID()
        {
            return SubID;
        }
        public void SetNotes(string NewNotes)
        {
            this.Notes = NewNotes;
        }
        public string GetNotes()
        {
            return Notes;
        }
        public int GetFreeSubID()
        {
            int i = 0;
            while (true == true)
            {
                bool countain = false;
                foreach (EditEAClassConnector AConnector in SubConnectorsList)
                {
                    if (AConnector.GetSubID().Equals(i))
                    {
                        countain = true;
                    }
                }
                if (countain.Equals(false))
                {
                    return i;
                }
                i++;
            }
        }
        public void SetName(string Name)
        {
            this.Name = Name;
        }
        public void SetSelectedState(bool value)
        {
            foreach (EditEAClassConnector AConnector in SubConnectorsList)
            {
                AConnector.SetSelectedState(value);
            }
            //if (value.Equals(false))
            //{
            //  this.Reset();
            //}
            this.SelectedState = value;
        }
        public bool GetSelectedState()
        {
            return SelectedState;
        }
        public EA.Collection GetTaggedValue()
        {
            return TaggedValue;
        }
        public void SetTaggedValue(EA.Collection NewTaggedValue)
        {
            this.TaggedValue = NewTaggedValue;
        }
        public void SetClientTaggedValue(EA.Collection NewTaggedValue)
        {
            this.ClientTaggedValue = NewTaggedValue;
        }
        public void SetSupplierTaggedValue(EA.Collection NewTaggedValue)
        {
            this.SupplierTaggedValue = NewTaggedValue;
        }
        public void SetType(string Type)
        {
            this.Type = Type;
        }
        public void SetClientNote(string NewNote)
        {
            this.ClientNote = NewNote;
        }
        public void SetSupplierNote(string NewNote)
        {
            this.SupplierNote = NewNote;
        }
        public new string GetType()
        {
            return Type;
        }
        public void SetDirection(string Direction)
        {
            this.Direction = Direction;
        }
        public string GetDirection()
        {
            return Direction;
        }
        public void SetSupplierLBCardinality(string NewCardinality)
        {
            this.SupplierLBCardinality = NewCardinality;
        }
        public void SetSupplierUBCardinality(string NewCardinality)
        {
            this.SupplierUBCardinality = NewCardinality;
        }
        public void SetClientLBCardinality(string NewCardinality)
        {
            this.ClientLBCardinality = NewCardinality;
        }
        public void SetClientUBCardinality(string NewCardinality)
        {
            this.ClientUBCardinality = NewCardinality;
        }
        public string GetSupplierLBCardinality()
        {
            return SupplierLBCardinality;
        }
        public string GetSupplierUBCardinality()
        {
            return SupplierUBCardinality;
        }
        public string GetClientLBCardinality()
        {
            return ClientLBCardinality;
        }
        public string GetClientUBCardinality()
        {
            return ClientUBCardinality;
        }
        public string GetName()
        {
            return Name;
        }
        public string GetGUID()
        {
            return GUID;
        }
        public EA.Connector GetOriginalConnector()
        {
            return OriginalConnector;
        }
        public void SetSupplierRole(string Role)
        {
            this.SupplierRole = Role;
        }
        public void SetClientRole(string Role)
        {
            this.ClientRole = Role;
        }
          public void SetSelectedElement(EA.Element selectedelement)
        {
            this.SelectedElementConnector = selectedelement;
        }
      
        public string GetClientRole()
        {
            return ClientRole;
        }

        public EA.Connector GetEditedConnector()
        {
            return EditedConnector;
        }

        public string GetSupplierRole()
        {
            return SupplierRole;
        }
        public void ExecuteCheckConnector()
        {
            //No old connector, normal dupplication of a connector or creation of subconnector
            if ((SelectedState.Equals(true)) && ((EditedConnector == null)))
            {
                CreateConnector();
            }
            else if ((SelectedState.Equals(true)) && (!(EditedConnector == null)) && (SubConnectorsList.Count > 0))
            {
                DeleteEditedConnector();
                foreach (EditEAClassConnector AConnector in SubConnectorsList)
                {
                    AConnector.ExecuteCheckConnector();
                }
                //DeleteEditedConnector and create subconnector
            }
            else if ((SelectedState.Equals(false)))
            {
                if (!(EditedConnector == null))
                {
                    this.DeleteEditedConnector();
                }
                if (!(SubConnectorsList.Count.Equals(0)))
                {
                    foreach (EditEAClassConnector AConnector in SubConnectorsList)
                    {
                        AConnector.ExecuteCheckConnector();
                    }
                    //DeleteSubConnector if they have edited connector !=null => executecheckconnector
                }
            }
            else if ((SelectedState.Equals(true)) && ((!(EditedConnector == null)) && (SubConnectorsList.Count.Equals(0))))
            {
                if (CheckIfSameConnector().Equals(false))
                {
                    UpdateEditedConnector();
                    //CreateConnector();
                    //DeleteEditedConnector(); 
                }
            }


        }
        private void UpdateEditedConnector()
        {
            EA.Connector newConnector= EditedConnector;

                        
                        newConnector.Direction = this.GetDirection();

                        newConnector.ClientEnd.Cardinality = GetEAFormatCardinality(this.GetClientLBCardinality(),this.GetClientUBCardinality());
                        newConnector.SupplierEnd.Cardinality = GetEAFormatCardinality(this.GetSupplierLBCardinality(),this.GetSupplierUBCardinality());


                        if (this.GetCopyNotes().Equals(true))
                        {
                            newConnector.Notes = this.GetNotes();
                            newConnector.ClientEnd.RoleNote = ClientNote;
                            newConnector.SupplierEnd.RoleNote = SupplierNote;
                        }
                        else
                        {
                            newConnector.Notes = "";
                            newConnector.ClientEnd.RoleNote = "";
                            newConnector.SupplierEnd.RoleNote = "";
                        }
                        
                      //  newConnector.Update();
                        if (this.GetCopyConstraint().Equals(true))
                        {
                            for (short i = 0; Constraints.Count > i; i++)
                            {
                                EA.ConnectorConstraint AConConstraint = (EA.ConnectorConstraint)Constraints.GetAt(i);
                                EA.ConnectorConstraint NewConstraint = (EA.ConnectorConstraint)newConnector.Constraints.AddNew(AConConstraint.Name, AConConstraint.Type);
                                NewConstraint.Notes = AConConstraint.Notes;
                                NewConstraint.Update();
                                newConnector.Update();
                            }
                        }
                        else
                        {
                            for (short i = 0; newConnector.Constraints.Count > i; i++)
                            {
                                EA.ConnectorConstraint AConConstraint = (EA.ConnectorConstraint)Constraints.GetAt(i);
                                newConnector.Constraints.Delete(i);
                                newConnector.Constraints.Refresh();
                               // newConnector.Update();
                            }
                        }

                        if ((this.GetCopyStereotype().Equals(false)) && (!this.GetStereotype().Equals("No Stereotype")))
                        {
                            newConnector.Stereotype = this.Stereotype;
                        }
                        else if (this.GetCopyStereotype().Equals(true))
                        {
                            newConnector.Stereotype = OriginalConnector.Stereotype;
                        }

                      //  newConnector.Update();
                        if (this.GetCopyTagValues().Equals(true))
                        {
                            //Association TaggedValue
                            EA.ConnectorTag NewTag;
                            if (!(this.GetTaggedValue() == null))
                            {
                                for (short i = 0; this.GetTaggedValue().Count > i; i++)
                                {
                                    EA.ConnectorTag ATagValue = (EA.ConnectorTag)this.GetTaggedValue().GetAt(i);
                                    if (!ATagValue.Name.Equals(CD.GetIBOTagValue()))
                                    {
                                        NewTag = (EA.ConnectorTag)newConnector.TaggedValues.AddNew(ATagValue.Name, ATagValue.Value);
                                        NewTag.Notes = ATagValue.Notes;
                                        NewTag.Update();
                                    }
                                }
                            }
                            //Supplier TaggedValues
                            EA.RoleTag AnotherNewTag;
                            if (!(this.SupplierTaggedValue == null))
                            {
                                for (short i = 0; this.SupplierTaggedValue.Count > i; i++)
                                {
                                    EA.RoleTag ATagValue = (EA.RoleTag)this.SupplierTaggedValue.GetAt(i);
                                    AnotherNewTag = (EA.RoleTag)newConnector.SupplierEnd.TaggedValues.AddNew(ATagValue.Tag, ATagValue.Value);
                                    AnotherNewTag.Update();
                                }
                                newConnector.Update();
                            }
                            //Client TaggedValues
                            if (!(this.ClientTaggedValue == null))
                            {
                                for (short i = 0; this.ClientTaggedValue.Count > i; i++)
                                {
                                    EA.RoleTag ATagValue = (EA.RoleTag)this.ClientTaggedValue.GetAt(i);
                                    AnotherNewTag = (EA.RoleTag)newConnector.ClientEnd.TaggedValues.AddNew(ATagValue.Tag, ATagValue.Value);
                                    AnotherNewTag.Update();
                                }
                             //   newConnector.Update();
                            }
                        }
                        else
                        {
                            //Association TaggedValue
                                for (short i = 0; newConnector.TaggedValues.Count > i; i++)
                                {
                                    EA.ConnectorTag ATagValue = (EA.ConnectorTag)newConnector.TaggedValues.GetAt(i);
                                    if (!ATagValue.Name.Equals(CD.GetIBOTagValue()))
                                    {
                                        newConnector.TaggedValues.Delete(i);
                                        
                                    }
                                }
                            //supplier tagged values
                                    for (short i = 0; newConnector.SupplierEnd.TaggedValues.Count > i; i++)
                                    {
                                        newConnector.SupplierEnd.TaggedValues.Delete(i);
                                        newConnector.SupplierEnd.TaggedValues.Refresh();
                                    }
                                    newConnector.Update();
                                    //Client TaggedValues
                                    if (!(this.ClientTaggedValue == null))
                                    {
                                        for (short i = 0; newConnector.ClientEnd.TaggedValues.Count > i; i++)
                                        {
                                            newConnector.ClientEnd.TaggedValues.Delete(i);
                                            newConnector.ClientEnd.TaggedValues.Refresh();
                                        }
                                        newConnector.Update();
                                    }

                            }
                        
                        newConnector.TaggedValues.Refresh();
                      //  newConnector.Update();



                        
                        newConnector.ClientEnd.Role = GetEAFormatRole(GetClientRoleQualifier(),GetClientRole());
                        newConnector.SupplierEnd.Role = GetEAFormatRole(GetSupplierRoleQualifier(), GetSupplierRole());
                        if (
                            (Main.EditType == "graph")
                            ||
                            (Main.EditType == "graphwg13")
                            )
                        {
                            #region AgregateByRef  graph
                           // util.wlog("TEST", "EditClassConnector UpdateEditedConnector  entree "); // am mars 2016
                            if (this.GetAgregate().Equals("SOURCE"))
                            {
                                if (SupplierContainmentByRef.Equals(true))
                                {
                                    if (newConnector.SupplierEnd.Containment == "Unspecified")
                                    {
                                        newConnector.SupplierEnd.Containment = "Reference";
                                        // newConnector.SupplierEnd.Update();
                                    }
                                }
                                else
                                {
                                    if (newConnector.SupplierEnd.Containment == "Reference")
                                    {
                                        newConnector.SupplierEnd.Containment = "Unspecified";
                                        // newConnector.SupplierEnd.Update();
                                    }

                                }
                                if (newConnector.ClientEnd.Containment == "Reference") // on ne peut avoir de reference du cote aggrege
                                {
                                    newConnector.ClientEnd.Containment = "Unpecified";
                                    // newConnector.ClientEnd.Update();
                                }
                            }
                            if (this.GetAgregate().Equals("TARGET"))
                            {
                                if (ClientContainmentByRef.Equals(true))
                                {
                                    if (newConnector.ClientEnd.Containment == "Unspecified")
                                    {
                                        newConnector.ClientEnd.Containment = "Reference";
                                        //   newConnector.ClientEnd.Update();
                                    }

                                }
                                else
                                {//ClientContainmentByRef.Equals(false)
                                    if (newConnector.ClientEnd.Containment == "Reference")
                                    {
                                        newConnector.ClientEnd.Containment = "Unspecified";
                                        //  newConnector.ClientEnd.Update();
                                    }

                                }
                                if (newConnector.SupplierEnd.Containment == "Reference") // on ne peut avoir de reference du cote aggrege
                                {
                                    newConnector.SupplierEnd.Containment = "Unpecified";
                                    //  newConnector.SupplierEnd.Update();
                                }
                            }

                            //-------------------------------------------------------------------


                            if (this.GetAgregate().Equals("SOURCE"))
                            {
                              //  util.wlog("TEST", "UpdateEditedConnector  SOURCE Navigable= " + isnav.ToString()); // am mars 2016
                                if (!isnav) // am mars 2016
                                {// hierarchy is marked by a lozange                         
                                    newConnector.ClientEnd.Aggregation = 1;
                                    bool resul = newConnector.ClientEnd.Update();
                                    // newConnector.Update();
                                }
                                else
                                {
                                    //am mars 2016

                                    newConnector.SupplierEnd.Navigable = "Navigable";// am mars 2016
                                    newConnector.ClientEnd.Navigable = "Unspecified";// am mars 2016
    
                                    string texte = "";
                                    texte = texte + " ConUpdate mavnav si agregate=SOURCE";
                                    texte = texte + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                                    texte = texte + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                                   // util.wlog("TEST", texte);
                                }
                            }
                            else if (this.GetAgregate().Equals("TARGET"))
                            {
                              //  util.wlog("TEST", "UpdateEditedConnector  TARGET NavigationEnabled=" + isnav.ToString()); // am mars 2016
                                if (!isnav) // am mars 2016
                                {// hierarchy is marked by a lozange
                                    
                                    newConnector.SupplierEnd.Aggregation = 1;
                                    
                                }
                                else
                                {

                                    newConnector.ClientEnd.Navigable = "Navigable";// am mars 2016
                   
                                    newConnector.SupplierEnd.Navigable = "Unspecified";// am mars 2016
             
                                    string texte = "";
                                    texte = texte + " ConUpdate mavnav si agregate=TARGET";
                                    texte = texte + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                                    texte = texte + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                                   // util.wlog("TEST", texte);
                                }
                            }
                            else
                            {
                                newConnector.SupplierEnd.Aggregation = 0;
                                newConnector.ClientEnd.Aggregation = 0;
                                newConnector.SupplierEnd.Containment = "Unspecifier";
                                newConnector.ClientEnd.Containment = "Unspecified";
                            }

                           
                            string text = "";
                            text = text + " ConUpdate avant SSwitch isnav" + isnav.ToString();
                            text = text + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                            text = text + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                           // util.wlog("TEST", text);

                            bool SSwitch = util.isSwitched(newConnector, this.OriginalConnector, SelectedIBOElement);

                            text = "";
                            text = text + " ConUpdate avant containment isnav/switch=" + isnav.ToString() + "/" + SSwitch.ToString();
                            text = text + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                            text = text + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                            //util.wlog("TEST", text);
                            if (isnav && (this.OriginalConnector.ClientEnd.Aggregation == 1))
                            {
                                if (SSwitch == true)
                                {
                                    newConnector.ClientEnd.Update();
                                    newConnector.SupplierEnd.Aggregation = 1;
                                    newConnector.SupplierEnd.Update();

                                }
                                else
                                {
                                    newConnector.SupplierEnd.Update();
                                    newConnector.ClientEnd.Aggregation = 1;
                                    newConnector.ClientEnd.Update();
                                }
                                if (isnav) Repo.GetCurrentDiagram().Update();
                            }
                            else
                            {
                                if (isnav && (this.OriginalConnector.SupplierEnd.Aggregation == 1))
                                {
                                    if (SSwitch == true)
                                    {
                                        newConnector.SupplierEnd.Update();
                                        newConnector.ClientEnd.Aggregation = 1;
                                        newConnector.ClientEnd.Update();
                                    }
                                    else
                                    {
                                        newConnector.ClientEnd.Update();
                                        newConnector.SupplierEnd.Aggregation = 1;
                                        newConnector.SupplierEnd.Update();
                                    }
                                    if (isnav) Repo.GetCurrentDiagram().Update();
                                }//endif
                                else
                                {
                                    newConnector.ClientEnd.Update();
                                    newConnector.SupplierEnd.Update();
                                }
                            }// endelse

                            // newConnector.Update();
                            text = "";
                            text = text + " ConUpdate après containment isnav/switch=" + isnav.ToString() + "/" + SSwitch.ToString();
                            text = text + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                            text = text + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                           // util.wlog("TEST", text);

                            #endregion
                        }
                        else //hierarchical //am aug 2016
                        {
                            #region AgregateByRef xds
                           // util.wlog("TEST", "EditClassConnector UpdateEditedConnector  entree "); // am mars 2016
                            if (this.GetAgregate().Equals("SOURCE"))
                            {
                                if (SupplierContainmentByRef.Equals(true))
                                {
                                    if (
                                        (newConnector.SupplierEnd.Containment == "Unspecified")
                                        ||
                                        (newConnector.SupplierEnd.Containment == "") // am aout 2018
                                        )
                                    {
                                        newConnector.SupplierEnd.Containment = "Reference";
                                         newConnector.SupplierEnd.Update();
                                    }
                                }
                                else
                                {
                                    if (newConnector.SupplierEnd.Containment == "Reference")
                                    {
                                        newConnector.SupplierEnd.Containment = "Unspecified";
                                         newConnector.SupplierEnd.Update();
                                    }

                                }
                                if (newConnector.ClientEnd.Containment == "Reference") // on ne peut avoir de reference du cote aggrege
                                {
                                    newConnector.ClientEnd.Containment = "Unpecified";
                                    newConnector.ClientEnd.Update();
                                }
                            }
                            if (this.GetAgregate().Equals("TARGET"))
                            {
                                if (ClientContainmentByRef.Equals(true))
                                {
                                    if (
                                        (newConnector.ClientEnd.Containment == "Unspecified")
                                        ||
                                        (newConnector.ClientEnd.Containment == "")
                                        )
                                    {
                                        newConnector.ClientEnd.Containment = "Reference";
                                           newConnector.ClientEnd.Update();
                                    }

                                }
                                else
                                {//ClientContainmentByRef.Equals(false)
                                    if (newConnector.ClientEnd.Containment == "Reference")
                                    {
                                        newConnector.ClientEnd.Containment = "Unspecified";
                                          newConnector.ClientEnd.Update();
                                    }

                                }
                                if (newConnector.SupplierEnd.Containment == "Reference") // on ne peut avoir de reference du cote aggrege
                                {
                                    newConnector.SupplierEnd.Containment = "Unpecified";
                                      newConnector.SupplierEnd.Update();
                                }
                            }

                            //-------------------------------------------------------------------


                            if (this.GetAgregate().Equals("SOURCE"))
                            {
                               // util.wlog("TEST", "UpdateEditedConnector  SOURCE Navigable= " + isnav.ToString()); // am mars 2016
                                if (!isnav) // am mars 2016
                                {// hierarchy is marked by a lozange                         
                                    newConnector.ClientEnd.Aggregation = 1;
                                    bool resul = newConnector.ClientEnd.Update();
                                    newConnector.ClientEnd.Update();
                                    newConnector.Update();//am aout 2018
                                }
                                else
                                {
                                    //am mars 2016

                                    newConnector.SupplierEnd.Navigable = "Navigable";// am mars 2016
                                    bool resul = newConnector.SupplierEnd.Update();    // am 14/10/2015
                                    newConnector.ClientEnd.Navigable = "Unspecified";// am mars 2016
                                    resul = newConnector.ClientEnd.Update();    // am 14/10/2015
                                    newConnector.Update();//am aout 2018
                                    string texte = "";
                                    texte = texte + " ConUpdate mavnav si agregate=SOURCE";
                                    texte = texte + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                                    texte = texte + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                                   // util.wlog("TEST", texte);
                                }
                            }
                            else if (this.GetAgregate().Equals("TARGET"))
                            {
                               // util.wlog("TEST", "UpdateEditedConnector  TARGET NavigationEnabled=" + isnav.ToString()); // am mars 2016
                                if (!isnav) // am mars 2016
                                {// hierarchy is marked by a lozange
                                   
                                    newConnector.SupplierEnd.Aggregation = 1;
                                    newConnector.SupplierEnd.Update();//am mars 2016
                                     newConnector.Update();//am aout 2018
                                }
                                else
                                {

                                    newConnector.ClientEnd.Navigable = "Navigable";// am mars 2016
                                    bool  resul = newConnector.ClientEnd.Update();    // am 14/10/2015
                                    newConnector.SupplierEnd.Navigable = "Unspecified";// am mars 2016
                                    resul = newConnector.SupplierEnd.Update();    // am 14/10/2015
                                    newConnector.Update();//am aout 2018
                                    string texte = "";
                                    texte = texte + " ConUpdate mavnav si agregate=TARGET";
                                    texte = texte + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                                    texte = texte + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                                 //   util.wlog("TEST", texte);
                                }
                            }
                            else
                            {
                                newConnector.SupplierEnd.Aggregation = 0;
                                newConnector.ClientEnd.Aggregation = 0;
                                newConnector.SupplierEnd.Containment = "Unspecifier";
                                newConnector.ClientEnd.Containment = "Unspecified";
                                newConnector.Update();//am aout 2018
                            }

                            //#endregion

                            // if (isnav) newConnector.Update(); //ATTENTION bug si present
                            // if (isnav) Repo.GetCurrentDiagram().Update();
                            //  if (isnav) Repo.ReloadDiagram(Repo.GetCurrentDiagram().DiagramID);
                            string text = "";
                            text = text + " ConUpdate avant SSwitch isnav" + isnav.ToString();
                            text = text + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                            text = text + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                           // util.wlog("TEST", text);

                            bool SSwitch = util.isSwitched(newConnector, this.OriginalConnector, SelectedIBOElement);

                            text = "";
                            text = text + " ConUpdate avant containment isnav/switch=" + isnav.ToString() + "/" + SSwitch.ToString();
                            text = text + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                            text = text + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                         //   util.wlog("TEST", text);
                            /*
                            if (!isnav && (this.OriginalConnector.ClientEnd.Aggregation == 1))
                            {
                                if (SSwitch == true)
                                {
                                    newConnector.ClientEnd.Update();
                                    newConnector.SupplierEnd.Aggregation = 1;
                                    newConnector.SupplierEnd.Update();

                                }
                                else
                                {
                                    newConnector.SupplierEnd.Update();
                                    newConnector.ClientEnd.Aggregation = 1;
                                    newConnector.ClientEnd.Update();
                                }
                                if (isnav) Repo.GetCurrentDiagram().Update();
                            }
                            else
                            {
                                if (isnav && (this.OriginalConnector.SupplierEnd.Aggregation == 1))
                                {
                                    if (SSwitch == true)
                                    {
                                        newConnector.SupplierEnd.Update();
                                        newConnector.ClientEnd.Aggregation = 1;
                                        newConnector.ClientEnd.Update();
                                    }
                                    else
                                    {
                                        newConnector.ClientEnd.Update();
                                        newConnector.SupplierEnd.Aggregation = 1;
                                        newConnector.SupplierEnd.Update();
                                    }
                                    if (isnav) Repo.GetCurrentDiagram().Update();
                                }//endif
                                else
                                {
                                    newConnector.ClientEnd.Update();
                                    newConnector.SupplierEnd.Update();
                                }
                            }// endelse
                            */
                            // newConnector.Update();
                            text = "";
                            text = text + " ConUpdate après containment isnav/switch=" + isnav.ToString() + "/" + SSwitch.ToString();
                            text = text + " clientrole=" + newConnector.ClientEnd.Role + " clientNav=" + newConnector.ClientEnd.Navigable;
                            text = text + " supplierrole=" + newConnector.SupplierEnd.Role + " SupplierNav=" + newConnector.SupplierEnd.Navigable;
                           // util.wlog("TEST", text);

                            #endregion

                        }

                         SelectedIBOElement.Update();



        }
        public void DeleteEditedConnector()
        {
            /////////////////// Save the diagram
            Repo.SaveDiagram(Repo.GetCurrentDiagram().DiagramID); 
            short CptToDelete = -1;
            for (short i = 0; TargetedIBOElement.Connectors.Count > i; i++)
            {
                if (((EA.Connector)TargetedIBOElement.Connectors.GetAt(i)).ConnectorGUID.Equals(EditedConnector.ConnectorGUID))
                {
                    CptToDelete = i;
                }
            }
            if (!CptToDelete.Equals(-1))
            {
                TargetedIBOElement.Connectors.Delete(CptToDelete);
                TargetedIBOElement.Connectors.Refresh();
                TargetedIBOElement.Update();
                Repo.GetCurrentDiagram().Update();
                Repo.ReloadDiagram(Repo.GetCurrentDiagram().DiagramID);
            }
        }
        private string GetEAFormatCardinality(string LBCard, string UBCard){
            string FormatedCard="";
            if (UBCard.Equals(""))
            {
                FormatedCard = LBCard;
            }
            else
            {
                FormatedCard = LBCard + ".." + UBCard;
            }
            return FormatedCard;
        }
        public string GetEAFormatRole(string Qualifier, string Role)
        {
            string FormatedString="";
            if (Qualifier.Equals(""))
            {
                FormatedString = Role;
            }
            else {
                FormatedString = Qualifier + "_" + Role;
            }
            return FormatedString;
        }
        public void CreateConnector()
        {
            /////////////////// Save the diagram
           Repo.SaveDiagram(Repo.GetCurrentDiagram().DiagramID);  

            if (SelectedState.Equals(true))
            {

                if (SubConnectorsList.Count > 0)
                {
                    foreach (EditEAClassConnector ASubConnector in SubConnectorsList)
                    {
                        ASubConnector.ExecuteCheckConnector();
                    }
                }
                else
                {
                    EA.Connector newConnector = null;
                    #region EmptyCase
                        //newConnector = (EA.Connector)SelectedIBOElement.Connectors.AddNew(this.GetName(), this.GetType());
                        newConnector = (EA.Connector)SelectedIBOElement.Connectors.AddNew(this.GetName(), "Association");
                        SelectedIBOElement.Connectors.Refresh();  // am 14/10/2015
                        newConnector.ClientID = SelectedIBOElement.ElementID;  //am  14/10/15
                        newConnector.SupplierID = TargetedIBOElement.ElementID;
                        bool resul = false;
                      resul=  newConnector.Update();  // am 14/10/2015
                        SelectedIBOElement.Update(); // am 14/10/2015                
              
                        newConnector.Direction = this.GetDirection();
                        resul = newConnector.Update();

                       
                        newConnector.ClientEnd.Cardinality = GetEAFormatCardinality(this.GetClientLBCardinality(),this.GetClientUBCardinality());
                        resul = newConnector.ClientEnd.Update(); // am 14/10/2015
                        newConnector.SupplierEnd.Cardinality = GetEAFormatCardinality(this.GetSupplierLBCardinality(),this.GetSupplierUBCardinality());
                        resul = newConnector.SupplierEnd.Update();  // am 14/10/2015
                       
                        if (this.GetCopyNotes().Equals(true))
                        {
                            newConnector.Notes = this.GetNotes();
                            newConnector.ClientEnd.RoleNote = ClientNote;
                            resul = newConnector.ClientEnd.Update(); // am 14/10/2015
                           
                            newConnector.SupplierEnd.RoleNote = SupplierNote;
                            resul = newConnector.SupplierEnd.Update();  // am 14/10/2015
                            
                        }
                
                     resul=   newConnector.Update();
                     
                        if (this.GetCopyConstraint().Equals(true))
                        {
                            for (short i = 0; Constraints.Count > i; i++)
                            {
                                EA.ConnectorConstraint AConConstraint = (EA.ConnectorConstraint)Constraints.GetAt(i);
                                EA.ConnectorConstraint NewConstraint = (EA.ConnectorConstraint)newConnector.Constraints.AddNew(AConConstraint.Name, AConConstraint.Type);
                                NewConstraint.Notes = AConConstraint.Notes;
                                newConnector.Constraints.Refresh();
                                NewConstraint.Update();
                               resul= newConnector.Update();
                            }
                        }

                        if ((this.GetCopyStereotype().Equals(false)) && (!this.GetStereotype().Equals("No Stereotype")))
                        {
                            newConnector.Stereotype = this.Stereotype;
                        }
                        else if (this.GetCopyStereotype().Equals(true))
                        {
                            newConnector.Stereotype = OriginalConnector.Stereotype;
                        }

                       resul= newConnector.Update();
                        if (this.GetCopyTagValues().Equals(true))
                        {
                            //Association TaggedValue
                            EA.ConnectorTag NewTag;
                            if (!(this.GetTaggedValue() == null))
                            {
                                for (short i = 0; this.GetTaggedValue().Count > i; i++)
                                {
                                    EA.ConnectorTag ATagValue = (EA.ConnectorTag)this.GetTaggedValue().GetAt(i);
                                    if (!ATagValue.Name.Equals(CD.GetIBOTagValue()))
                                    {
                                        NewTag = (EA.ConnectorTag)newConnector.TaggedValues.AddNew(ATagValue.Name, ATagValue.Value);
                                        NewTag.Notes = ATagValue.Notes;
                                        NewTag.Update();
                                        newConnector.TaggedValues.Refresh(); // am 14/10/15
                                    }
                                }
                            }
                            //Supplier TaggedValues
                            EA.RoleTag AnotherNewTag;
                            if (!(this.SupplierTaggedValue == null))
                            {
                                for (short i = 0; this.SupplierTaggedValue.Count > i; i++)
                                {
                                    EA.RoleTag ATagValue = (EA.RoleTag)this.SupplierTaggedValue.GetAt(i);
                                    AnotherNewTag = (EA.RoleTag)newConnector.SupplierEnd.TaggedValues.AddNew(ATagValue.Tag, ATagValue.Value);
                                    AnotherNewTag.Update();
                                    newConnector.SupplierEnd.TaggedValues.Refresh(); // am 14/10/15
                                }
                              resul=  newConnector.Update();
                             
                            }
                            //Client TaggedValues
                            if (!(this.ClientTaggedValue == null))
                            {
                                for (short i = 0; this.ClientTaggedValue.Count > i; i++)
                                {
                                    EA.RoleTag ATagValue = (EA.RoleTag)this.ClientTaggedValue.GetAt(i);
                                    AnotherNewTag = (EA.RoleTag)newConnector.ClientEnd.TaggedValues.AddNew(ATagValue.Tag, ATagValue.Value);
                                    AnotherNewTag.Update();
                                    newConnector.ClientEnd.TaggedValues.Refresh(); // am 14/10/15
                                }
                              resul=  newConnector.Update();
                             
                            }
                        }
                        EA.ConnectorTag NewTag2 = (EA.ConnectorTag)newConnector.TaggedValues.AddNew(CD.GetIBOTagValue(), this.GetGUID());
                        //EA.ConnectorTag NewTag2 = (EA.ConnectorTag)newConnector.TaggedValues.AddNew(CD.GetIBOTagValue(), );//am avril 2011
                        NewTag2.Notes = CD.GetIBOTagValueNote();
                        NewTag2.Update();
                        newConnector.TaggedValues.Refresh();
                      resul=  newConnector.Update();
                     


                        
                        newConnector.ClientEnd.Role = GetEAFormatRole(GetClientRoleQualifier(),GetClientRole());
                        resul = newConnector.ClientEnd.Update();
                    newConnector.SupplierEnd.Role = GetEAFormatRole(GetSupplierRoleQualifier(), GetSupplierRole());
                       resul= newConnector.SupplierEnd.Update();
                       resul= newConnector.Update();
                    //Main.isBasedOnExecuted = true; // the inclusion modofoes the profile and that has to be notified
                       //---------  //am 14/10/15 ---------------
                       if (Main.EditType == "graph")
                       {
                           #region AgregateByRef graph
                         //  util.wlog("TEST", "CreateConector  entree "); // am mars 2016
                           if (this.GetAgregate().Equals("SOURCE"))
                           {
                              // util.wlog("TEST", "CreateConector  SOURCE  NavigationEnabled=" + isnav.ToString()); // am mars 2016

                               if (!isnav) // am mars 2016
                               {// hierarchy is marked by a lozange
                                   newConnector.ClientEnd.Aggregation = 1;

                                   if (this.GetSupplierContainmentByRef())
                                   {
                                       newConnector.SupplierEnd.Containment = "Reference";
                                       resul = newConnector.SupplierEnd.Update();
                                   }
                                   resul = newConnector.ClientEnd.Update();  // am maars 2016 attention il faut faire cet update apres l'autre
                               }
                               else
                               {
                                   /**********************/
                                   //am mars 2016
                                   // if (SupplierContainmentByRef.Equals(true))
                                   // {
                                   newConnector.SupplierEnd.Navigable = "Navigable";// am mars 2016
                                   if (this.GetSupplierContainmentByRef())
                                   {
                                       newConnector.SupplierEnd.Containment = "Reference";
                                   }

                                   resul = newConnector.SupplierEnd.Update();    // am 14/10/2015
                                   newConnector.ClientEnd.Navigable = "Unspecified";// am mars 2016
                                   resul = newConnector.ClientEnd.Update();    // am 14/10/2015

                                   // }
                               }
                               /************************/
                           }

                           else if (this.GetAgregate().Equals("TARGET"))
                           {
                              // util.wlog("TEST", "CreateConector  TARGET  NavigationEnabled= " + isnav.ToString()); // am mars 2016
                               if (!isnav) // am mars 2016
                               {// hierarchy is marked by a lozange
                                   newConnector.SupplierEnd.Aggregation = 1;

                                   if (this.GetClientContainmentByRef())
                                   {
                                       newConnector.ClientEnd.Containment = "Reference";
                                       resul = newConnector.ClientEnd.Update();
                                   }
                                   resul = newConnector.SupplierEnd.Update(); // am mars 2016
                                   //resul=newConnector.Update(); // am mars 2016
                                   /*********************/
                               }
                               else
                               {
                                   //am mars 2016
                                   // if (CientContainmentByRef.Equals(true))
                                   // {

                                   newConnector.ClientEnd.Navigable = "Navigable";// am mars 2016
                                   if (this.GetClientContainmentByRef())
                                   {
                                       newConnector.ClientEnd.Containment = "Reference";
                                   }
                                   resul = newConnector.ClientEnd.Update();    // am 14/10/2015
                                   newConnector.SupplierEnd.Navigable = "Unspecified"; //am 2016
                                   resul = newConnector.SupplierEnd.Update();    // am 14/10/2015
                                   

                                   // }
                                   /************************/
                               }
                           }
                         
                           if (isnav && (this.OriginalConnector.ClientEnd.Aggregation == 1))
                           {
                               if (this.Switch == true)
                               {
                                   newConnector.SupplierEnd.Aggregation = 1;
                                   newConnector.SupplierEnd.Update();
                               }
                               else
                               {
                                   newConnector.ClientEnd.Aggregation = 1;
                                   newConnector.ClientEnd.Update();
                               }

                           }
                           if (isnav && (this.OriginalConnector.SupplierEnd.Aggregation == 1))
                           {
                               if (this.Switch == true)
                               {
                                   newConnector.ClientEnd.Aggregation = 1;
                                   newConnector.ClientEnd.Update();
                               }
                               else
                               {
                                   newConnector.SupplierEnd.Aggregation = 1;
                                   newConnector.SupplierEnd.Update();
                               }

                           }
                           #endregion
                       }
                       else // hierachical
                       {
                           #region AgregateByRef xsd

                           Main.valtest1 = newConnector.ConnectorGUID;//pour test am aout 2016
                           string prov = ""; //pour test
                           util = new utilitaires.Utilitaires(Repo);//am aug 2016

                           if (this.GetAgregate().Equals("SOURCE"))
                           {
                              // util.wlog("TEST", "CreateConector  SOURCE  NavigationEnabled= " + isnav.ToString()); // am mars 2016
                               prov = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 1855"); // pour test am aout 2016

                               if (!isnav) // am mars 2016
                               {// hierarchy is marked by a lozange
                                   newConnector.ClientEnd.Aggregation = 1;

                                   if (this.GetSupplierContainmentByRef())
                                   {
                                       newConnector.SupplierEnd.Containment = "Reference";
                                       resul = newConnector.SupplierEnd.Update();
                                       prov = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 1865"); // pour test am aout 2016
                                   }
                                   resul = newConnector.ClientEnd.Update();  // am maars 2016 attention il faut faire cet update apres l'autre
                                   prov = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 1868"); // pour test am aout 2016
                               }
                               else
                               {
                                   /**********************/
 
                                   newConnector.SupplierEnd.Navigable = "Navigable";// am mars 2016
                                   if (this.GetSupplierContainmentByRef())
                                   {
                                       newConnector.SupplierEnd.Containment = "Reference";
                                   }

                                   resul = newConnector.SupplierEnd.Update();    // am 14/10/2015
                                   prov = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 1883"); // pour test am aout 2016
                                   newConnector.ClientEnd.Navigable = "Unspecified";// am mars 2016
                                   resul = newConnector.ClientEnd.Update();    // am 14/10/2015
                                   prov = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 1886"); // pour test am aout 2016

                                   
                               }
                               /************************/
                           }

                           else if (this.GetAgregate().Equals("TARGET"))
                           {
                              // util.wlog("TEST", "CreateConector  TARGET  NavigationEnabled= " + isnav.ToString()); // am mars 2016

                               if (!isnav) // am mars 2016
                               {// hierarchy is marked by a lozange
                                   newConnector.SupplierEnd.Aggregation = 1;

                                   if (this.GetClientContainmentByRef())
                                   {
                                       newConnector.ClientEnd.Containment = "Reference";
                                       resul = newConnector.ClientEnd.Update();
                                       prov = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 1904"); // pour test am aout 2016
                                   }
                                   resul = newConnector.SupplierEnd.Update(); // am mars 2016
                                   prov = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 1907"); // pour test am aout 2016
                                   //resul=newConnector.Update(); // am mars 2016
                                   /*********************/
                               }
                               else
                               {
                                   

                                   newConnector.ClientEnd.Navigable = "Navigable";// am mars 2016

                                   if (this.GetClientContainmentByRef())
                                   {
                                       newConnector.ClientEnd.Containment = "Reference";
                                   }
                                   resul = newConnector.ClientEnd.Update();    // am 14/10/2015
                                   prov = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 1923"); // pour test am aout 2016
                                   newConnector.SupplierEnd.Navigable = "Unspecified"; //am 2016
                                   resul = newConnector.SupplierEnd.Update();    // am 14/10/2015
                                   prov = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 1926"); // pour test am aout 2016
                                  
                                   /************************/
                               }
                           }
                               
                               bool firstlevel = util.isAFirstLevelPackage(Repo, Repo.GetCurrentDiagram().PackageID);
                                if(!firstlevel)
                                {
                                    if (this.Switch == true)
                                    {
                                        if (!isnav)
                                        {
                                            if (this.OriginalConnector.ClientEnd.Aggregation == 1)
                                            {
                                                this.SetAgregate("TARGET");
                                                newConnector.SupplierEnd.Aggregation = 1;
                                                newConnector.SupplierEnd.Update();
                                            }
                                            else
                                            { // pasclent aggreg
                                                if (this.OriginalConnector.SupplierEnd.Aggregation == 1)
                                                {
                                                    this.SetAgregate("SOURCE");
                                                    newConnector.ClientEnd.Aggregation = 1;
                                                    newConnector.ClientEnd.Update();
                                                }
                                            }
                                        }
                                        else
                                        {//isnav
                                            if (this.OriginalConnector.SupplierEnd.Navigable == "Navigable")
                                            {
                                                this.SetAgregate("TARGET");
                                                newConnector.ClientEnd.Navigable = "Navigable";
                                                newConnector.ClientEnd.Update();
                                            }
                                            else
                                            { // pasclent aggreg
                                                if (this.OriginalConnector.ClientEnd.Navigable == "Navigable")
                                                {
                                                    this.SetAgregate("SOURCE");
                                                    newConnector.SupplierEnd.Navigable = "Navigable";
                                                    newConnector.SupplierEnd.Update();
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {// SSwitch false
                                        if (!isnav)
                                        {
                                            if (this.OriginalConnector.ClientEnd.Aggregation == 1)
                                            {
                                                this.SetAgregate("SOURCE");
                                                newConnector.ClientEnd.Aggregation = 1;
                                                newConnector.ClientEnd.Update();
                                            }
                                            else
                                            { // pasclent aggreg
                                                if (this.OriginalConnector.SupplierEnd.Aggregation == 1)
                                                {
                                                    this.SetAgregate("TARGET");
                                                    newConnector.SupplierEnd.Aggregation = 1;
                                                    newConnector.SupplierEnd.Update();
                                                }
                                            }
                                        }
                                        else 
                                        {//isnav
                                            if (this.OriginalConnector.SupplierEnd.Navigable == "Navigable")
                                            {
                                                this.SetAgregate("SOURCE");
                                                newConnector.SupplierEnd.Navigable = "Navigable";
                                                newConnector.SupplierEnd.Update();
                                            }
                                            else
                                            { // pasclent aggreg
                                                if (this.OriginalConnector.ClientEnd.Navigable == "Navigable")
                                                {
                                                    this.SetAgregate("TARGET");
                                                    newConnector.ClientEnd.Navigable = "Navigable";
                                                    newConnector.ClientEnd.Update();
                                                }
                                            }
                                        }
                                    }


                                    
                           #endregion

                           }
                       }
                       
                       /*******************************************************/
                       
                      
                        SelectedIBOElement.Update();
                        string prov1 = util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 2008"); // pour test am aout 2016
                       // TargetedIBOElement.Update();  // am 14/10/15
                   
                   // }
                    #endregion
                  
#region commentaire
                    /*
                    #region SourceCase
                    else if (this.GetAgregate().Equals("SOURCE"))
                    {

                        newConnector = (EA.Connector)SelectedIBOElement.Connectors.AddNew(this.GetName(), "Association");
                        newConnector.SupplierID = TargetedIBOElement.ElementID;

                        newConnector.ClientEnd.Aggregation = 1;
                        if (SupplierContainmentByRef.Equals(true))
                        {
                            newConnector.SupplierEnd.Containment = "Reference";
                        }

                        newConnector.Direction = this.GetDirection();



                        newConnector.ClientEnd.Cardinality = GetEAFormatCardinality(this.GetClientLBCardinality(), this.GetClientUBCardinality());
                        newConnector.SupplierEnd.Cardinality = GetEAFormatCardinality(this.GetSupplierLBCardinality(), this.GetSupplierUBCardinality());
                        /*
                        if (this.GetClientUBCardinality().Equals(""))
                        {
                            newConnector.ClientEnd.Cardinality = this.GetClientLBCardinality();
                        }
                        else
                        {
                            newConnector.ClientEnd.Cardinality = this.GetClientLBCardinality() + ".." + this.GetClientUBCardinality();
                        }

                        if (this.GetSupplierUBCardinality().Equals(""))
                        {
                            newConnector.SupplierEnd.Cardinality = this.GetSupplierLBCardinality();
                        }
                        else
                        {
                            newConnector.SupplierEnd.Cardinality = this.GetSupplierLBCardinality() + ".." + this.GetSupplierUBCardinality();
                        }

                        if (this.GetCopyNotes().Equals(true))
                        {
                            newConnector.Notes = this.GetNotes();
                            newConnector.ClientEnd.RoleNote = ClientNote;
                            newConnector.SupplierEnd.RoleNote = SupplierNote;
                        }

                        newConnector.Update();
                        if (this.GetCopyConstraint().Equals(true))
                        {
                            for (short i = 0; Constraints.Count > i; i++)
                            {
                                EA.ConnectorConstraint AConConstraint = (EA.ConnectorConstraint)Constraints.GetAt(i);
                                EA.ConnectorConstraint NewConstraint = (EA.ConnectorConstraint)newConnector.Constraints.AddNew(AConConstraint.Name, AConConstraint.Type);
                                NewConstraint.Notes = AConConstraint.Notes;
                                NewConstraint.Update();
                                newConnector.Update();
                            }
                        }

                        if ((this.GetCopyStereotype().Equals(false)) && (!this.GetStereotype().Equals("No Stereotype")))
                        {
                            newConnector.Stereotype = this.Stereotype;
                        }
                        else if (this.GetCopyStereotype().Equals(true))
                        {
                            newConnector.Stereotype = OriginalConnector.Stereotype;
                        }
                        newConnector.Update();
                        if (this.GetCopyTagValues().Equals(true))
                        {
                            EA.ConnectorTag NewTag;

                            if (!(this.GetTaggedValue() == null))
                            {
                                for (short i = 0; this.GetTaggedValue().Count > i; i++)
                                {
                                    EA.ConnectorTag ATagValue = (EA.ConnectorTag)this.GetTaggedValue().GetAt(i);
                                    if (!ATagValue.Name.Equals(CD.GetIBOTagValue()))
                                    {
                                        NewTag = (EA.ConnectorTag)newConnector.TaggedValues.AddNew(ATagValue.Name, ATagValue.Value);
                                        NewTag.Notes = ATagValue.Notes;
                                        NewTag.Update();
                                    }
                                }
                            }

                            //Supplier TaggedValues
                            EA.RoleTag AnotherNewTag;
                            if (!(this.SupplierTaggedValue == null))
                            {
                                for (short i = 0; this.SupplierTaggedValue.Count > i; i++)
                                {
                                    EA.RoleTag ATagValue = (EA.RoleTag)this.SupplierTaggedValue.GetAt(i);

                                    AnotherNewTag = (EA.RoleTag)newConnector.SupplierEnd.TaggedValues.AddNew(ATagValue.Tag, ATagValue.Value);
                                    AnotherNewTag.Update();
                                }
                                newConnector.Update();
                            }

                            //Client TaggedValues
                            if (!(this.ClientTaggedValue == null))
                            {
                                for (short i = 0; this.ClientTaggedValue.Count > i; i++)
                                {
                                    EA.RoleTag ATagValue = (EA.RoleTag)this.ClientTaggedValue.GetAt(i);
                                    AnotherNewTag = (EA.RoleTag)newConnector.ClientEnd.TaggedValues.AddNew(ATagValue.Tag, ATagValue.Value);
                                    AnotherNewTag.Update();
                                }
                                newConnector.Update();
                            }
                        }
                        EA.ConnectorTag NewTag2 = (EA.ConnectorTag)newConnector.TaggedValues.AddNew(CD.GetIBOTagValue(), this.GetGUID());
                        NewTag2.Notes = CD.GetIBOTagValueNote();
                        NewTag2.Update();
                        newConnector.Update();

                        newConnector.ClientEnd.Role = GetEAFormatRole(GetClientRoleQualifier(), GetClientRole());
                        newConnector.SupplierEnd.Role = GetEAFormatRole(GetSupplierRoleQualifier(), GetSupplierRole());
                        newConnector.Update();
                        SelectedIBOElement.Update();

                    }
                    #endregion
                    #region TargetCase
                    else if (this.GetAgregate().Equals("TARGET"))
                    {

                        newConnector = (EA.Connector)TargetedIBOElement.Connectors.AddNew(this.GetName(), "Association");
                        newConnector.SupplierID = SelectedIBOElement.ElementID;


                        newConnector.ClientEnd.Aggregation = 1;                        
                        if (ClientContainmentByRef.Equals(true))
                        {
                            newConnector.SupplierEnd.Containment = "Reference";
                        }
                        if (SupplierContainmentByRef.Equals(true))
                        {
                            newConnector.ClientEnd.Containment = "Reference";
                        }

                        newConnector.Direction = this.GetDirection();


                        newConnector.ClientEnd.Cardinality = GetEAFormatCardinality(this.GetSupplierLBCardinality(), this.GetSupplierUBCardinality());
                        newConnector.SupplierEnd.Cardinality = GetEAFormatCardinality(this.GetClientLBCardinality(), this.GetClientUBCardinality());
                        

                        if (this.GetCopyNotes().Equals(true))
                        {
                            newConnector.Notes = this.GetNotes();
                            newConnector.ClientEnd.RoleNote = SupplierNote;
                            newConnector.SupplierEnd.RoleNote = ClientNote;
                        }

                        newConnector.Update();
                        if (this.GetCopyConstraint().Equals(true))
                        {
                            for (short i = 0; Constraints.Count > i; i++)
                            {
                                EA.ConnectorConstraint AConConstraint = (EA.ConnectorConstraint)Constraints.GetAt(i);
                                EA.ConnectorConstraint NewConstraint = (EA.ConnectorConstraint)newConnector.Constraints.AddNew(AConConstraint.Name, AConConstraint.Type);
                                NewConstraint.Notes = AConConstraint.Notes;
                                NewConstraint.Update();
                                newConnector.Update();
                            }
                        }


                        if ((this.GetCopyStereotype().Equals(false)) && (!this.GetStereotype().Equals("No Stereotype")))
                        {
                            newConnector.Stereotype = this.Stereotype;
                        }
                        else if (this.GetCopyStereotype().Equals(true))
                        {
                            newConnector.Stereotype = OriginalConnector.Stereotype;
                        }


                        newConnector.Update();
                        if (this.GetCopyTagValues().Equals(true))
                        {
                            EA.ConnectorTag NewTag;

                            if (!(this.GetTaggedValue() == null))
                            {
                                for (short i = 0; this.GetTaggedValue().Count > i; i++)
                                {
                                    EA.ConnectorTag ATagValue = (EA.ConnectorTag)this.GetTaggedValue().GetAt(i);

                                    if (!ATagValue.Name.Equals(CD.GetIBOTagValue()))
                                    {
                                        NewTag = (EA.ConnectorTag)newConnector.TaggedValues.AddNew(ATagValue.Name, ATagValue.Value);
                                        NewTag.Notes = ATagValue.Notes;
                                        NewTag.Update();
                                    }
                                }
                            }

                            //Supplier TaggedValues
                            EA.RoleTag AnotherNewTag;
                            if (!(this.SupplierTaggedValue == null))
                            {
                                for (short i = 0; this.SupplierTaggedValue.Count > i; i++)
                                {
                                    EA.RoleTag ATagValue = (EA.RoleTag)this.SupplierTaggedValue.GetAt(i);
                                    AnotherNewTag = (EA.RoleTag)newConnector.ClientEnd.TaggedValues.AddNew(ATagValue.Tag, ATagValue.Value);
                                    AnotherNewTag.Update();
                                }
                                newConnector.Update();
                            }

                            //Client TaggedValues
                            if (!(this.ClientTaggedValue == null))
                            {
                                for (short i = 0; this.ClientTaggedValue.Count > i; i++)
                                {
                                    EA.RoleTag ATagValue = (EA.RoleTag)this.ClientTaggedValue.GetAt(i);
                                    AnotherNewTag = (EA.RoleTag)newConnector.SupplierEnd.TaggedValues.AddNew(ATagValue.Tag, ATagValue.Value);
                                    AnotherNewTag.Update();
                                }
                                newConnector.Update();
                            }
                        }
                        EA.ConnectorTag NewTag2 = (EA.ConnectorTag)newConnector.TaggedValues.AddNew(CD.GetIBOTagValue(), this.GetGUID());
                        NewTag2.Notes = CD.GetIBOTagValueNote();
                        NewTag2.Update();
                        newConnector.Update();

                        newConnector.ClientEnd.Role = GetEAFormatRole(GetSupplierRoleQualifier(), GetSupplierRole());
                        newConnector.SupplierEnd.Role = GetEAFormatRole(GetClientRoleQualifier(), GetClientRole());
                        newConnector.Update();
                        SelectedIBOElement.Update();
                    }

                    */
                    #endregion
                }
          

                Repo.GetCurrentDiagram().Update();
               
                Repo.ReloadDiagram(Repo.GetCurrentDiagram().DiagramID);  //am 14/10/15
                if (util == null) util = new utilitaires.Utilitaires(Repo);//am aug 2016
                if (Main.valtest1 != null)//am aug 2016
                {
                    util.testConnectorState(Repo.GetConnectorByGuid(Main.valtest1), " ligne 2260"); // pour test am juil 2016

                }
            }
        }
        public bool GetClientContainmentByRef()
        {
            return ClientContainmentByRef;
        }
        public void SetClientContainmentByRef(bool Value)
        {
            this.ClientContainmentByRef = Value;
        }
        public bool GetSupplierContainmentByRef()
        {
            return SupplierContainmentByRef;
        }
        public void SetSupplierContainmentByRef(bool Value)
        {
            this.SupplierContainmentByRef = Value;
        }
        public ArrayList GetSubConnectors()
        {
            return SubConnectorsList;
        }

/*************  ajout pour test am mars 2016  ***************************/
     public string GetTrace()
        {
            string ret = "";
          ret="traceAConnector GUID=" + this.GUID + "nb de subconnectors=" ;
         ret=ret+ this.SubConnectorsList.Count.ToString() + " Aggregate=" + this.Agregate;
         ret=ret+ " ClientRef=" + ClientContainmentByRef + " SupplierRef=" + SupplierContainmentByRef;

            return ret;
        }




     /************************************************************************/
     /// <summary>
     /// am avril 2016
     /// recupere le switch du EditedConnector
     /// </summary>
     /// <returns></returns>
     public bool GetSwitch()
     {
         return this.Switch;
     }
        /************************************************************************/
    }
}

