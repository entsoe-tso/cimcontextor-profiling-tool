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
    public partial class EditAttributeStereotype : Form
    {
        private EA.Repository Repository;
        private EAClass PopulatedEAClass;
        private EAClassAttribute EditedEAClassAttribute;
        private bool CreateMode;

        public EditAttributeStereotype(EA.Repository Repository, EAClass PopulatedEAClass, EAClassAttribute EditedEAClassAttribute, bool CreateMode)
        {
            InitializeComponent();

            this.Repository = Repository;
            this.PopulatedEAClass = PopulatedEAClass;
            this.EditedEAClassAttribute = EditedEAClassAttribute;
            this.CreateMode = CreateMode;
            Repository.Stereotypes.Refresh();
            for(short i = 0; i < Repository.Stereotypes.Count; i++)
            {
                EA.Stereotype AStereotype = (EA.Stereotype)Repository.Stereotypes.GetAt(i);
                if (AStereotype.AppliesTo.ToLower().Equals("attribute"))
                {
                    CLBStereotype.Items.Add(AStereotype.Name, false);
                }
            }

           
            
                    #region UpDateMode
            if (CreateMode.Equals(false))
            {
                if (!(EditedEAClassAttribute.GetStereotype()==("")))
                {
                    if (!(EditedEAClassAttribute.GetStereotype()==null))
                    {
                        ArrayList SplitedStereotypeList = new ArrayList(EditedEAClassAttribute.GetStereotype().Split(",".ToCharArray()));
                        for (int i = 0; i < CLBStereotype.Items.Count; i++)
                        {
                            if ((SplitedStereotypeList.Contains(CLBStereotype.GetItemText(CLBStereotype.Items[i]))))
                            {
                                CLBStereotype.SetItemCheckState(i, CheckState.Checked);
                            }
                        }
                    }
                }
            }
                    #endregion
                    #region CreateMode
                else
                {
                    if (!(EditedEAClassAttribute.GetStereotype()==("")))
                    {
                        for (int i = 0; i < CLBStereotype.Items.Count; i++)
                        {
                            if (EditedEAClassAttribute.GetStereotype()==(CLBStereotype.GetItemText(CLBStereotype.Items[i])))
                            {
                                CLBStereotype.SetItemCheckState(i, CheckState.Checked);
                            }
                        }
                    }

                
            }
                    #endregion
            

            if (CLBStereotype.Items.Count >= 1)
            {
                CLBStereotype.SelectedItem = CLBStereotype.Items[0];
            }
           
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
            EditedEAClassAttribute.SetStereotype(newStereotypeList);

            this.Dispose();
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
