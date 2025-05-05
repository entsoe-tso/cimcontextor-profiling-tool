using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class EnumClassifierForm : Form
    {
        private EAClassAttribute EditedEAClassAttribute;
        private ArrayList ClassifierList = new ArrayList();
        private EA.Repository Repository;
        private ConstantDefinition CD = new ConstantDefinition();

        public EnumClassifierForm(EA.Repository Repository, EAClassAttribute EditedEAClassAttribute)
        {
            InitializeComponent();
            this.EditedEAClassAttribute = EditedEAClassAttribute;
            this.Repository = Repository;
            this.Text = "Select a new classifier :";


            ListViewItem lvi;
            EA.Element parentClassifier = null;
            bool errorHappened = false; // ABA20230401
            try
            {
                parentClassifier = Repository.GetElementByID(EditedEAClassAttribute.GetClassifier());
            }
            catch
            {
                MessageBox.Show("Link to the classifier of this attribut seem broken.\nDid you delete the classifier of this attribut ?\nYou must edit the link of this attribut and select his type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                errorHappened = true;
                //this.Dispose(); ABA20230401
            }
            if (parentClassifier == null)
            {
                MessageBox.Show("Link to the classifier of this attribut seem broken.\nDid you delete the classifier of this attribut ?\nYou must edit the link of this attribut and select his type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                errorHappened = true;
                // this.Dispose(); ABA20230401
            }

            if (errorHappened)
            {
                this.Dispose();
            }
            ClassifierList.Add(parentClassifier);

            SearchPossibleDataType(parentClassifier);

            foreach (EA.Element AClassifier in ClassifierList)
            {
                string MemberNameList = "";
                foreach (EA.Attribute AnAttribute in AClassifier.Attributes)
                {
                    MemberNameList = MemberNameList + " " + AnAttribute.Name;
                }

                String[] aHeaders = new string[3];
                aHeaders[0] = AClassifier.Name;
                aHeaders[1] = MemberNameList;
                aHeaders[2] = AClassifier.ElementID.ToString();
                lvi = new ListViewItem(aHeaders);
                LVClassifier.Items.Add(lvi);
                if (AClassifier.ElementID.Equals(EditedEAClassAttribute.GetClassifier()))
                {
                    lvi.Selected = true;
                }
            }
        }


        private void SearchPossibleDataType(EA.Element SearchedObject)
        {
            //foreach (EA.Connector AConnector in SearchedObject.Connectors)
            for (short i = 0; SearchedObject.Connectors.Count > i; i++)
            {
                EA.Connector AConnector = (EA.Connector)SearchedObject.Connectors.GetAt(i);
                if (AConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower()))
                {
                    EA.Element NewElement = Repository.GetElementByID(AConnector.ClientID);
                    if (!NewElement.ElementID.Equals(SearchedObject.ElementID))
                    {
                        if (NewElement.Stereotype.Equals(CD.GetEnumStereotype()) || NewElement.Stereotype.Equals("<<" + CD.GetEnumStereotype() + ">>"))
                        {
                            ClassifierList.Add(NewElement);
                            SearchPossibleDataType(NewElement);
                        }
                    }
                }
            }
        }


        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void ButOk_Click(object sender, EventArgs e)
        {
            if (LVClassifier.SelectedItems.Count.Equals(1))
            {
                EditedEAClassAttribute.SetClassifier(int.Parse(LVClassifier.SelectedItems[0].SubItems[2].Text));
                this.Dispose();
            }
        }
    }
}
