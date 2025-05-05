using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
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
        class IBOCleaner
        {
            private LogForm LF;
            private EA.Repository Repository;
            private EA.Diagram ActiveDiagram;
            private ConstantDefinition CD = new ConstantDefinition();
            public IBOCleaner(EA.Repository Repository)
            {
                this.Repository = Repository;

                DialogResult result = MessageBox.Show("CimContextor :\n This function will delete from the current diagram any IsBasedOn parent.\n Are you sure to want to remove every IBO parent from the diiagram ?", "CimContextor : IsBasedOn",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result.Equals(DialogResult.No))
                {
                    return;
                }
                else
                {

                    ActiveDiagram = Repository.GetCurrentDiagram();
                    LF = new LogForm();
                    bool Done = true;
                    while (Done.Equals(true))
                    {
                        Done = ExecuteCleaning();
                    }
                    Repository.ReloadDiagram(ActiveDiagram.DiagramID);
                }
            }

            public bool ExecuteCleaning()
            {
                bool DeletedSomething = false;
                LF.Show();
                LF.AppendTitle("Checking connectors...");
                
                for (short i = 0; ActiveDiagram.DiagramLinks.Count > i; i++)
                {

                    EA.DiagramLink ALink = (EA.DiagramLink)ActiveDiagram.DiagramLinks.GetAt(i);
                    EA.Connector AConnector = Repository.GetConnectorByID(ALink.ConnectorID);
                    LF.AppendSubTitle("Checking the connector between " + Repository.GetElementByID(AConnector.ClientID).Name + " and " + Repository.GetElementByID(AConnector.SupplierID).Name + ", checking stereotype...");
                    if (AConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower()))
                    {
                        LF.AppendLog("IBO Stereotype found, deleting from diagram...");
                        
                        for (short j = 0; ActiveDiagram.DiagramObjects.Count > j; j++)
                        {
                            EA.DiagramObject AnObject = (EA.DiagramObject)ActiveDiagram.DiagramObjects.GetAt(j);
                            if (AnObject.ElementID.Equals(AConnector.SupplierID))
                            {
                                ActiveDiagram.DiagramObjects.DeleteAt(j, true);
                                DeletedSomething = true;
                                ActiveDiagram.Update();
                                ActiveDiagram.DiagramObjects.Refresh();
                                ActiveDiagram.DiagramLinks.Refresh();
                                LF.AppendLog("Deleting class : " + Repository.GetElementByID(AConnector.SupplierID).Name);
                                LF.AppendSubTitle("Finished checking the connector between " + Repository.GetElementByID(AConnector.ClientID).Name + " and " + Repository.GetElementByID(AConnector.SupplierID).Name + ", checking stereotype...");
                                break;
                            }
                        }
                    }
                    else
                    {
                        LF.AppendLog("Element isn't an IBO Stereotype.");
                        LF.AppendSubTitle("Finished checking the connector between " + Repository.GetElementByID(AConnector.ClientID).Name + " and " + Repository.GetElementByID(AConnector.SupplierID).Name + ", checking stereotype...");
                    }
                }
                LF.AppendTitle("Finished checking connectors...");
                return DeletedSomething;
            }
            
        }
    

}
