using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using CimContextor.Utilities;
using System.Threading;
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
    class EditAnIsBasedOn
    {
        private EA.Repository Repository;
        private EA.Connector IBOConnector;
        private ConstantDefinition CD = new ConstantDefinition();
        public EditAnIsBasedOn(EA.Repository Repository, Thread messageThread, LoadingIndicator loadingIndicator)
        {
            this.Repository = Repository;

            if (CheckSelectedItem() == null)
            {
                loadingIndicator.Dispose();                
                messageThread.Abort();
                MessageBox.Show("You must select an IsBasedOn child class (only one) with an IsBasedOn from a diagram before using this function", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                this.IBOConnector = CheckSelectedItem();
            }
            XMLParser XMLP = new XMLParser(Repository);
            bool IsHere = false;
            for (short i = 0; Repository.GetCurrentDiagram().DiagramObjects.Count > i;i++ )
            {
                EA.DiagramObject AnObject = (EA.DiagramObject)Repository.GetCurrentDiagram().DiagramObjects.GetAt(i);
                if (AnObject.ElementID.Equals(Repository.GetElementByID(IBOConnector.SupplierID).ElementID))
                {
                    IsHere = true;
                    break;
                }
            }
            loadingIndicator.ProgressChanged(10);
            EAClass populatedEAClass = new EAClass(Repository, Repository.GetCurrentDiagram(), Repository.GetElementByID(IBOConnector.SupplierID).ElementGUID, Repository.GetElementByID(IBOConnector.ClientID).ElementGUID, CD.GetUpdate(),0,0,0,0,"",IsHere);
            EA.Element parentElement = Repository.GetElementByID(IBOConnector.ClientID);
            IsBasedOnForm IBOF;
            if ((parentElement.Stereotype.ToLower().Equals(CD.GetDatatypeStereotype().ToLower())) || (parentElement.Stereotype.ToLower().Equals(CD.GetPrimitiveStereotype().ToLower())) || (parentElement.Stereotype.ToLower().Equals(CD.GetEnumStereotype().ToLower())))
            {
                IBOF = new IsBasedOnForm(Repository, populatedEAClass, XMLP.GetXmlQualifier(CD.GetDatatypeStereotype().ToLower()), false, loadingIndicator);
            }
            else
            {
                IBOF = new IsBasedOnForm(Repository, populatedEAClass, XMLP.GetXmlQualifier(CD.GetClass().ToLower()), false, loadingIndicator);
            }
            loadingIndicator.ProgressChanged(100);
            loadingIndicator.Dispose();
            messageThread.Abort();
            IBOF.Show();
            IBOF.SetUILoading(false);
        }

        private EA.Connector CheckSelectedItem()
        {
            try
            {

                EA.Diagram SelectedDiagram = (EA.Diagram)Repository.GetCurrentDiagram();
                if (!(SelectedDiagram == null))
                {
                    if (!SelectedDiagram.SelectedObjects.Count.Equals(1))
                    {
                        return null;
                    }
                    EA.Collection ObjectList = SelectedDiagram.SelectedObjects;

                    EA.Element SelectedItem = null;
                    for (short i = 0; ObjectList.Count > i; i++)
                    {
                        SelectedItem = Repository.GetElementByID(((EA.DiagramObject)ObjectList.GetAt(i)).ElementID);

                    }

                    if (SelectedItem == null)
                    {
                        return null;
                    }

                    if ((SelectedItem.Type.ToString().ToLower().Equals(CD.GetClass().ToLower()))
                        ||
                        (SelectedItem.Type.ToString().ToLower().Equals("enumeration"))  // am mai 2018
                        )
                    {
                        for (short i = 0; SelectedItem.Connectors.Count > i; i++)
                        {
                            EA.Connector AConnector = (EA.Connector)SelectedItem.Connectors.GetAt(i);
                            if (AConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower()))
                            {
                                if (AConnector.ClientID.Equals(SelectedItem.ElementID))
                                {
                                    return AConnector;
                                }
                            }
                        }
                        return null;
                    }
                    else { return null; }
                }
                else { return null; }
            }
            catch { return null; }
        }
    
  
    }
}
