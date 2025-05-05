using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
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
    public partial class EditClassStereotype : Form
    {
        private EA.Repository Repository;
        private EAClass populatedEAClass;

        public EditClassStereotype(EA.Repository Repository, EAClass populatedEAClass, bool CreateMode)
        {
            InitializeComponent();
            this.Repository = Repository;
            this.populatedEAClass = populatedEAClass;

            Repository.Stereotypes.Refresh();

            for (short i = 0; i < Repository.Stereotypes.Count; i++)
            {
                EA.Stereotype AStereotype = (EA.Stereotype)Repository.Stereotypes.GetAt(i);
                if(AStereotype.AppliesTo.ToLower().Equals("class")){        
                    CLBStereotype.Items.Add(AStereotype.Name, false);
                }
            }

            #region UpDateMode
            if (CreateMode.Equals(false))
            {
                if (!populatedEAClass.GetIBOElement().GetStereotypeList().Equals(""))
                {
                    for (int i = 0; i < CLBStereotype.Items.Count; i++)
                    {
                        ArrayList SplitedStereotypeList = new ArrayList(populatedEAClass.GetIBOElement().GetStereotypeList().Split(",".ToCharArray()));
                        if (SplitedStereotypeList.Contains(CLBStereotype.GetItemText(CLBStereotype.Items[i])))
                        {
                            CLBStereotype.SetItemCheckState(i, CheckState.Checked);
                        }
                    }
                    if (CLBStereotype.CheckedItems.Count >= 1)
                    {
                        CLBStereotype.SelectedItem = CLBStereotype.CheckedItems[0];
                    }

                }
            #endregion
            #region CreateMode
                else
                {
                    
                    if (!Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).GetStereotypeList().Equals(""))
                    {
                        for (int i = 0; i < CLBStereotype.Items.Count; i++)
                        {
                            if (populatedEAClass.GetIBOElement().GetStereotypeList().Contains(CLBStereotype.GetItemText(CLBStereotype.Items[i])))
                            {
                                CLBStereotype.SetItemCheckState(i, CheckState.Checked);
                            }
                        }
                        if (CLBStereotype.CheckedItems.Count >= 1)
                        {
                            CLBStereotype.SelectedItem = CLBStereotype.CheckedItems[0];
                        }
                    }

                }
                #endregion
            }
        }

        private void EditClassStereotype_Load(object sender, EventArgs e)
        {

        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void ButSave_Click(object sender, EventArgs e)
        {
            string newStereotypeList = "";
            for (int i = 0; i < CLBStereotype.CheckedItems.Count; i++)
            {
                if (newStereotypeList.Equals(""))
                {
                    newStereotypeList = CLBStereotype.GetItemText(CLBStereotype.CheckedItems[i]);
                }
                else
                {
                    newStereotypeList = newStereotypeList + "," + CLBStereotype.GetItemText(CLBStereotype.CheckedItems[i]);
                }
            }
            populatedEAClass.SetStereotype(newStereotypeList);
            this.Dispose();
        }
    }
}
