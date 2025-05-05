/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// File: SuperclassConnectorCopier.cs
/////////////////////////////////////////////////////////////////////////////////////////


using EA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CimContextor.EditConnectors
{
    public class SuperclassConnectorCopier
    {
        private EA.Repository repo = null;
        private EA.Element selElem = null;
        private EA.Element parentCIMElem = null;
        private readonly string COPY = "GUIDBasedOn";
        private readonly string IS_BASED_ON = "IsBasedOn";

        public SuperclassConnectorCopier(EA.Repository repo) 
        {
            this.repo = repo;
        }

        public void CopySuperclassConnectors(bool takeOverRoles)
        {
            try
            {
                this.parentCIMElem = checkElementAndGetCIMParent();
                if (this.parentCIMElem == null)
                {
                    return;
                }

                List<EA.Element> superclasses = GetNonBasedOnSuperclasses(this.parentCIMElem);
                if (superclasses.Count > 0)
                {
                    List<ConnectorAndElement> connectorsAndElements = GetNonBasedSuperClassConnectors(superclasses, parentCIMElem);
                    foreach (ConnectorAndElement connAndElem in connectorsAndElements)
                    {
                        if (connAndElem.Connector.ClientID == connAndElem.Connector.SupplierID) // self-connector
                        {
                            CreateConnectorOnSuperConnector(this.selElem, this.selElem.ElementID, connAndElem.Connector, takeOverRoles);
                        }
                        else
                        {
                            List<EA.Element> otherBasedOnElemes = GetBasedOnElementsViaSuperclass(connAndElem.Element);
                            foreach (Element otherElem in otherBasedOnElemes)
                            {
                                CreateConnectorOnSuperConnector(this.selElem, otherElem.ElementID, connAndElem.Connector, takeOverRoles);
                            }

                        }
                    }
                }
                this.repo.RefreshOpenDiagrams(true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }

        private bool AlreadyCopied(EA.Connector conn, EA.Element selectedElem, int otherID)
        {
            EA.Collection connectors = selectedElem.Connectors;
            foreach (EA.Connector connector in connectors)
            {
                if (((connector.ClientID == selectedElem.ElementID) && (connector.SupplierID == otherID)) || 
                    ((connector.ClientID == otherID) && (connector.SupplierID == selectedElem.ElementID)))
                {
                    foreach (EA.ConnectorTag ct in connector.TaggedValues)
                    {
                        if (ct.Name.Equals(COPY) && ct.Value.Equals(conn.ConnectorGUID))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private void CreateConnectorOnSuperConnector(EA.Element selectedElem, int otherID, EA.Connector conn, bool takeOverRoles)
        {
            if(AlreadyCopied(conn, selectedElem, otherID))
            {
                // EA.Element suppl = repo.GetElementByID(supplierID);
                // ABA20231118 MessageBox.Show("Connector " + client.Name + " to " + supplier.Name + " exists already!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            EA.Connector newConn = (EA.Connector)selectedElem.Connectors.AddNew(conn.Name, conn.Type);
            newConn.SupplierID = otherID;
            newConn.ClientEnd.Aggregation = conn.ClientEnd.Aggregation;
            newConn.SupplierEnd.Aggregation = conn.SupplierEnd.Aggregation;
            newConn.Stereotype = conn.Stereotype;
            newConn.Notes = conn.Notes;
            newConn.Alias = conn.Alias;
            newConn.Color = conn.Color;
            newConn.ClientEnd.Cardinality = conn.ClientEnd.Cardinality;
            newConn.SupplierEnd.Cardinality = conn.SupplierEnd.Cardinality;
            newConn.Direction = conn.Direction;
            if (takeOverRoles)
            {
                newConn.ClientEnd.Role = conn.ClientEnd.Role;
                newConn.ClientEnd.RoleNote = conn.ClientEnd.RoleNote;
                newConn.SupplierEnd.Role = conn.SupplierEnd.Role;
                newConn.SupplierEnd.RoleNote = conn.SupplierEnd.RoleNote;
                newConn.ClientEnd.RoleType = conn.ClientEnd.RoleType;
                newConn.SupplierEnd.RoleType = conn.SupplierEnd.RoleType;
            }
            newConn.Update();
            selectedElem.Connectors.Refresh();
            SetCopied(newConn, conn);
        }

        private void SetCopied(EA.Connector newConn, EA.Connector conn)
        {
            EA.ConnectorTag ct = (EA.ConnectorTag)newConn.TaggedValues.AddNew(COPY, conn.ConnectorGUID);
            ct.Update();
            newConn.TaggedValues.Refresh();
            newConn.Update();
        }

        // Finds the "basedOn" elements connected with the CIM element that inherits 
        // from the superclass. 
        // 1. Gets the CIM element via a Generalization.
        // 2. Finds the elements based on the CIM element and collects them in a list.
        private List<EA.Element> GetBasedOnElementsViaSuperclass(EA.Element superclass)
        {
            List<EA.Element> basedOnElems = new List<EA.Element>();
            foreach(EA.Connector conn in superclass.Connectors) // Get CIM class
            {
                if(conn.Type.Equals("Generalization") && (conn.SupplierID == superclass.ElementID))
                {
                    EA.Element cimElem = repo.GetElementByID(conn.ClientID);
                    foreach(EA.Connector depConn in cimElem.Connectors) // check dependencies of CIM element
                    {
                        if(depConn.Type.Equals("Dependency") && depConn.Stereotype.Equals(IS_BASED_ON))
                        {
                            EA.Element otherElem = null;
                            if (depConn.Direction.Equals("Source -> Destination"))
                            {
                                otherElem = repo.GetElementByID(depConn.ClientID);
                            } else if(depConn.Direction.Equals("Destination -> Source"))
                            {
                                otherElem = repo.GetElementByID(depConn.SupplierID);
                            }
                            if (otherElem != null)
                            {
                                string basedOnGuid = GetBasedOnGuid(otherElem);
                                if (basedOnGuid.Equals(cimElem.ElementGUID))
                                {
                                    basedOnElems.Add(otherElem);
                                }
                            }
                            else MessageBox.Show(ErrorCodes.ERROR_045[0] + ErrorCodes.ERROR_045[1] + " connector end " + depConn.SupplierID + " not found!");
                        }
                    }
                }
            }
            return basedOnElems;
        }

        private EA.Element checkElementAndGetCIMParent()
        {
            if (repo.GetContextItemType() != EA.ObjectType.otElement)
            {
                // ABA20230703 MessageBox.Show("Selected a valid 'based on' child element!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageBox.Show("The selected modeling element must be a valid 'based on' child element!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            EA.Element selElem = (EA.Element)repo.GetContextObject();
            if (selElem.Type != "Class")
            {
                MessageBox.Show("The selected element must be a class!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            this.selElem = selElem;

            string parentGuid = null;
            if ((parentGuid = GetBasedOnGuid(selElem)) == null)
            {
                MessageBox.Show("Missing GUIDBasedOn tagged value.\nThe selected element must be a 'Based on' class!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            EA.Element parentElem = repo.GetElementByGuid(parentGuid);
            if(parentElem == null)
            {
                MessageBox.Show("Invalid 'BasedOn' parent element!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return parentElem;
        }

        private string GetBasedOnGuid(EA.Element elem)
        {
            foreach (TaggedValue tv in elem.TaggedValues)
            {
                if (tv.Name.Equals("GUIDBasedOn"))
                {
                    return tv.Value;
                }
            }
            return null;
        }

        private bool IsBasedOn(EA.Element elem)
        {
            string guid = GetBasedOnGuid(elem);
            if(guid != null)
            {
                EA.Element otherElem = repo.GetElementByGuid(guid);
                if (otherElem != null)
                {
                    foreach (EA.Connector conn in elem.Connectors)
                    {
                        if ((conn.ClientID == otherElem.ElementID) || 
                            (conn.SupplierID == otherElem.ElementID))
                        {
                            if (conn.Type.Equals("Dependency") && conn.Stereotype.Equals(IS_BASED_ON)) return true;
                        }
                    }
                }
            }
            return false;
        }

        private List<EA.Element> GetNonBasedOnSuperclasses(EA.Element parentElem)
        {
            List<EA.Element> superclasses = new List<EA.Element>();
            foreach(EA.Connector conn in parentElem.Connectors)
            {
                if(conn.Type.Equals("Generalization"))
                {
                    EA.Element superclass = repo.GetElementByID(conn.SupplierID);
                    if (!IsBasedOn(superclass) && !IsCIM(superclass)) // not basedOn
                    {
                        superclasses.Add(superclass);
                    }
                }
            }
            return superclasses;
        }

       
        private bool IsCIM(EA.Element elem)
        {
            // Note: ConstantDefinition.CIMPackageNames = new List<string> { "IEC61970", "IEC61968", "IEC62325" };
            ConstantDefinition constDef = new ConstantDefinition();
            EA.Package pack = repo.GetPackageByID(elem.PackageID);
            return IsInCIMPackage(constDef, pack);
        }

        private bool IsInCIMPackage(ConstantDefinition constDef, EA.Package pack)
        {
            int parentID = pack.ParentID;
            if (parentID == 0) // is a Model
            {
                return false;
            }
            EA.Package nextPack = repo.GetPackageByID(parentID);
            if (constDef.CIMPackageNames.Contains(nextPack.Name))
            {
                return true;
            }
            return IsInCIMPackage(constDef, nextPack);
        }


        private List<ConnectorAndElement> GetNonBasedSuperClassConnectors(List<EA.Element> nonBasedSuperclasses, EA.Element cimParent)
        {
            List<ConnectorAndElement> connectorsAndElements = new List<ConnectorAndElement>();
            foreach (EA.Element superclass in nonBasedSuperclasses)
            {
                foreach(EA.Connector conn in superclass.Connectors)
                {
                    if(conn.ClientID == conn.SupplierID) // self-connector
                    {
                        connectorsAndElements.Add(new ConnectorAndElement(conn, superclass));
                    } 
                    else
                    {
                        if(conn.ClientID == superclass.ElementID)
                        {
                            EA.Element otherSuperclass = repo.GetElementByID(conn.SupplierID);
                            if(!IsBasedOn(otherSuperclass) && (cimParent.ElementID != conn.SupplierID)) // not BasedOn and not CIM
                            {
                                connectorsAndElements.Add(new ConnectorAndElement(conn, otherSuperclass));
                            }
                        } 
                        else if(conn.SupplierID == superclass.ElementID)
                        {
                            EA.Element otherSuperclass = repo.GetElementByID(conn.ClientID);
                            if(!IsBasedOn(otherSuperclass) && (cimParent.ElementID != conn.ClientID)) // not BasedOn and not CIM
                            {
                                connectorsAndElements.Add(new ConnectorAndElement(conn, otherSuperclass));
                            }
                        }
                    }
                }
            }
            return connectorsAndElements;
        }


        class ConnectorAndElement
        {
            private EA.Connector connector;
            private EA.Element element; 

            public ConnectorAndElement(EA.Connector connector, EA.Element element)
            {
                this.connector = connector;
                this.element = element;
            }

            public EA.Connector Connector
            {
                get { return connector;}
                set { connector = value;}
            }

            public EA.Element Element
            {
                get { return element;}
                set { element = value;}
            }
        }

    }

}
