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
    public partial class AttributeClassifierConstraintForm : Form
    {
        private EA.Repository Repository;
        private EAClassAttribute AnAttribute;
        private EA.Attribute SelectedEAAttribute;
        private ArrayList AvailableConstraintList;
        private EAClass populatedEAClass;
        private string EditedType;
        private bool CreateMode;
        private ConstantDefinition CD = new ConstantDefinition();

        public AttributeClassifierConstraintForm(bool CreateMode, EA.Repository Repository, EAClassAttribute AnAttribute, string AttributeType, string EditedType)
        {
            InitializeComponent();
            this.Repository = Repository;
            this.AnAttribute = AnAttribute;
            string ConstraintType = AttributeType;
            this.EditedType = EditedType;
            this.CreateMode = CreateMode;

            this.Text = "Editing constraints of " + AnAttribute.GetName();
            LabAttributeName.Text = "Selected attribute : " + AnAttribute.GetName();

            CBConstantValue.Checked = AnAttribute.GetConstantState();
            CBStaticValue.Checked = AnAttribute.GetStaticState();
            TBDefaultValue.Text = AnAttribute.GetDefaultValue();
            populatedEAClass = AnAttribute.getClass();

            if (EditedType.Equals(CD.GetPrimitiveStereotype()))
            {
                CBStaticValue.Enabled = false;
                CBConstantValue.Enabled = false;
                TBDefaultValue.Enabled = false;
            }

            if (EditedType.Equals(CD.GetDatatypeStereotype()))
            {
                SelectedEAAttribute = Repository.GetAttributeByGuid(AnAttribute.GetGUID());
                if (CreateMode.Equals(CD.GetCreate()))
                {
                    if (CBStaticValue.Checked.Equals(true))
                    {
                        CBStaticValue.Enabled = false;
                    }
                    if (CBConstantValue.Checked.Equals(true))
                    {
                        CBConstantValue.Enabled = false;
                        TBDefaultValue.Enabled = false;
                    }
                }
                else
                {
                    EA.Attribute ParentAttribute = null;
                    for (short i = 0; Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Attributes.Count > i; i++)
                    {
                        EA.Attribute PAttribute = (EA.Attribute)Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).Attributes.GetAt(i);
                        if (PAttribute.Name.Equals(AnAttribute.GetName()))
                        {
                            ParentAttribute = PAttribute;
                            break;
                        }
                    }
                    if (!(ParentAttribute == null))
                    {

                        if (ParentAttribute.IsStatic.Equals(true))
                        {
                            CBStaticValue.Enabled = false;
                        }
                        if (ParentAttribute.IsConst.Equals(true))
                        {
                            CBConstantValue.Enabled = false;
                            TBDefaultValue.Enabled = false;
                        }
                    }
                }

            }
            XMLParser XMLP = new XMLParser(Repository);
            AvailableConstraintList = new ArrayList();

            foreach (EAClassAttributeConstraint AConstraint in AnAttribute.GetClassifierConstraints())
            {
                CBExistingConstraint.Items.Add(AConstraint.GetName());
            }

            AvailableConstraintList = XMLP.GetXmlClassifierConstraint(ConstraintType.ToLower());
            foreach (XMLClassifierConstraint AConstraint in AvailableConstraintList)
            {
                if (!CBExistingConstraint.Items.Contains(AConstraint.GetName()))
                {
                    CBAvailableConstraint.Items.Add(AConstraint.GetName());
                }
            }

            ListViewItem lvi;

            foreach (EAClassAttributeConstraint AConstraint in AnAttribute.GetClassifierConstraints())
            {
                String[] aHeaders = new string[2];
                aHeaders[0] = AConstraint.GetName();
                aHeaders[1] = AConstraint.GetNotes();
                lvi = new ListViewItem(aHeaders);
                LVExistingConstraint.Items.Add(lvi);
            }

        }

        private void ButAddConstraint_Click(object sender, EventArgs e)
        {
            if (CBAvailableConstraint.Items.Contains(CBAvailableConstraint.Text))
            {
                XMLClassifierConstraint SelectedConstraint = null;
                foreach (XMLClassifierConstraint AConstraint in AvailableConstraintList)
                {
                    if (AConstraint.GetName().Equals(CBAvailableConstraint.Text))
                    {
                        SelectedConstraint = AConstraint;
                        break;
                    }
                }
                AttributeClassifierConstraintEditForm ACCEF = new AttributeClassifierConstraintEditForm(this, Repository, CD.GetCreate(), EditedType, AnAttribute, null, SelectedConstraint);
                ACCEF.ShowDialog();
            }
        }

        private void ButEditConstraint_Click(object sender, EventArgs e)
        {
            if (CBExistingConstraint.Items.Contains(CBExistingConstraint.Text))
            {

                EAClassAttributeConstraint SelectedConstraint = null;

                foreach (EAClassAttributeConstraint AConstraint in AnAttribute.GetClassifierConstraints())
                {
                    if (AConstraint.GetName().Equals(CBExistingConstraint.Text))
                    {
                        SelectedConstraint = AConstraint;
                        break;
                    }
                }
                XMLClassifierConstraint SelectedXMLConstraint = null;
                foreach (XMLClassifierConstraint AConstraint in AvailableConstraintList)
                {
                    if (AConstraint.GetName().Equals(CBExistingConstraint.Text))
                    {
                        SelectedXMLConstraint = AConstraint;
                        break;
                    }
                }
                AttributeClassifierConstraintEditForm ACCEF = new AttributeClassifierConstraintEditForm(this, Repository, CD.GetUpdate(), EditedType, AnAttribute, SelectedConstraint, SelectedXMLConstraint);
                ACCEF.ShowDialog();
            }
        }

        public void UpdateConstraintToUI(EAClassAttributeConstraint ANewConstraint, string Mode)
        {
            ListViewItem lvi;
            if (Mode.Equals(CD.GetCreate()))
            {
                String[] aHeaders = new string[2];
                aHeaders[0] = ANewConstraint.GetName();
                aHeaders[1] = ANewConstraint.GetNotes();
                lvi = new ListViewItem(aHeaders);
                LVExistingConstraint.Items.Add(lvi);
                CBExistingConstraint.Items.Add(ANewConstraint.GetName());
                CBAvailableConstraint.Items.Remove(ANewConstraint.GetName());
            }
            if(Mode.Equals("DELETE")){

                ListViewItem lviToDel = null;
                foreach(ListViewItem alvi in LVExistingConstraint.Items)
                {
                    if (alvi.SubItems[0].Text.Equals(ANewConstraint.GetName()))
                    {
                        lviToDel = alvi;
                    }
                }
                if (lviToDel != null)
                {
                    LVExistingConstraint.Items.Remove(lviToDel);
                }

                CBExistingConstraint.Items.Remove(ANewConstraint.GetName());
                CBAvailableConstraint.Items.Add(ANewConstraint.GetName());
            }
            else
            {
                foreach (ListViewItem ALVI in LVExistingConstraint.Items)
                {
                    if (ALVI.SubItems[0].Text.Equals(ANewConstraint.GetName()))
                    {
                        ALVI.SubItems[1].Text = ANewConstraint.GetNotes();
                        break;
                    }
                }
            }
        }

        private void ButDispose_Click(object sender, EventArgs e)
        {


            if (!AnAttribute.GetConstantState().Equals(true))
            {
                AnAttribute.SetDefaultValue(TBDefaultValue.Text);
            }


            if (CBConstantValue.Checked.Equals(true))
            {
                AnAttribute.SetConstantState(true);
            }
            else
            {
                AnAttribute.SetConstantState(false);
            }



            if (CBStaticValue.Checked.Equals(true))
            {
                AnAttribute.SetStaticState(true);
            }
            else
            {
                AnAttribute.SetStaticState(false);
            }

            this.Dispose();
        }

        private void ButSelectEnum_Click(object sender, EventArgs e)
        {
            EnumClassifierForm ECF = new EnumClassifierForm(Repository, AnAttribute);
            ECF.ShowDialog();
        }

        private void CBDeleteConstraint_Click(object sender, EventArgs e)
        {
                if (CBExistingConstraint.Items.Contains(CBExistingConstraint.Text))
                {

                    EAClassAttributeConstraint SelectedConstraint = null;

                    foreach (EAClassAttributeConstraint AConstraint in AnAttribute.GetClassifierConstraints())
                    {
                        if (AConstraint.GetName().Equals(CBExistingConstraint.Text))
                        {
                            SelectedConstraint = AConstraint;
                            break;
                        }
                    }

                    AnAttribute.DeleteAClassifierConstraint(SelectedConstraint.GetName());
                    UpdateConstraintToUI(SelectedConstraint,"DELETE");
                    
                }
            
        }


    }
}
