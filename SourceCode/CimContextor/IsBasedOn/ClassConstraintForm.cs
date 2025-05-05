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
    public partial class ClassConstraintForm : Form
    {
        private EAClass populatedEAClass;
        private string currentEditedConstraint = "";
        private string uiState = "";
        private ArrayList traditionnalConstraintList = new ArrayList();
        private ConstantDefinition CD = new ConstantDefinition();

        public ClassConstraintForm(EAClass populatedEAClass, EA.Repository repo)
        {
            InitializeComponent();
            this.populatedEAClass = populatedEAClass;
            LabAttributName.Text = LabAttributName.Text + " " + populatedEAClass.GetName();
            ListViewItem lvi;
            String[] aHeaders = new string[2];
            ArrayList ConstraintList = new ArrayList();
            ConstraintList = populatedEAClass.GetConstraints();
            foreach (EAClassConstraint aConstraint in ConstraintList)
            {
                aHeaders[0] = aConstraint.GetName();
                aHeaders[1] = aConstraint.GetNotes().Trim();
                lvi = new ListViewItem(aHeaders);
                LVConstraint.Items.Add(lvi);
            }
            aHeaders[0] = "New constraint";
            aHeaders[1] = "";
            lvi = new ListViewItem(aHeaders);
            LVConstraint.Items.Add(lvi);
            //init CBTradConstraint
            XMLParser xmlParser = new XMLParser(repo);

            String type = "";
            if (populatedEAClass.GetStereotype()!=null)
            {
            if((populatedEAClass.GetStereotype().ToLower()==CD.GetDatatypeStereotype().ToLower())||(populatedEAClass.GetStereotype().ToLower()=="<<"+CD.GetDatatypeStereotype().ToLower()+">>")){
            type = CD.GetDatatypeStereotype();
            }
            else if((populatedEAClass.GetStereotype().ToLower()==CD.GetEnumStereotype().ToLower())||(populatedEAClass.GetStereotype().ToLower()=="<<"+CD.GetEnumStereotype().ToLower()+">>")){
            type = CD.GetEnumStereotype();
            }
            else if((populatedEAClass.GetStereotype().ToLower()==CD.GetCompoundStereotype().ToLower())||(populatedEAClass.GetStereotype().ToLower()=="<<"+CD.GetCompoundStereotype().ToLower()+">>")){
            type = CD.GetCompoundStereotype();
            }
            else{
                type = CD.GetClass();
            }
            }
            else
            {
                type = CD.GetClass();
            }

            traditionnalConstraintList = xmlParser.GetXmlConstraint(type);
            
            foreach (XMLConstraint aConstraint in traditionnalConstraintList)
            {
                CBTradConstraint.Items.Add(aConstraint.GetType() + "-" + aConstraint.GetName());
            }
            CBTradConstraint.Items.Add("Cancel");
            //Init CBConstraintType
            CBConstraintType.Items.Add("OCL");
            CBConstraintType.Items.Add("Invariant");
            CBConstraintType.SelectedItem="OCL";
        }



        
        private void ButSaveEditedNote_Click(object sender, EventArgs e)
        {
            if (uiState.Equals("EDITINGCONSTRAINT"))
            {
                EAClassConstraint editedConstraint = populatedEAClass.GetAConstraint(currentEditedConstraint);
                editedConstraint.SetNotes(TBEditedNote.Text);
                editedConstraint.SetName(TBConstraintName.Text);

                foreach (ListViewItem anItem in LVConstraint.Items)
                {
                    anItem.Remove();
                }

                ListViewItem lvi;
                String[] aHeaders = new string[2];
                foreach (EAClassConstraint aConstraint in populatedEAClass.GetConstraints())
                {
                    aHeaders[0] = aConstraint.GetName();
                    aHeaders[1] = aConstraint.GetNotes().Trim();
                    lvi = new ListViewItem(aHeaders);
                    LVConstraint.Items.Add(lvi);
                }
                aHeaders[0] = "New constraint";
                aHeaders[1] = "";
                lvi = new ListViewItem(aHeaders);
                LVConstraint.Items.Add(lvi);

                currentEditedConstraint = "";
                TBEditedNote.Text = "";
                ButSaveEditedNote.Visible = false;
                LabConstraintName.Visible = false;
                TBConstraintName.Visible = false;
                TBEditedNote.Visible = false;
                TBEditedNote.Enabled = true;
                ButDeleteConst.Visible = false;
            }
            else if (uiState.Equals("SCRATCHNEWCONSTRAINT"))
            {
                if(!CBConstraintType.Items.Contains(CBConstraintType.Text)){
                    MessageBox.Show("You must select a valid type from the constraint type list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }

                if (!TBConstraintName.Text.Equals("") && !TBEditedNote.Text.Equals(""))
                {

                    EAClassConstraint newConstraint = populatedEAClass.AddConstraint(TBConstraintName.Text, CBConstraintType.Text, TBEditedNote.Text);

                    foreach (ListViewItem anItem in LVConstraint.Items)
                    {
                        anItem.Remove();
                    }

                    ListViewItem lvi;
                    String[] aHeaders = new string[2];
                    foreach (EAClassConstraint aConstraint in populatedEAClass.GetConstraints())
                    {
                        aHeaders[0] = aConstraint.GetName();
                        aHeaders[1] = aConstraint.GetNotes().Trim();
                        lvi = new ListViewItem(aHeaders);
                        LVConstraint.Items.Add(lvi);
                    }
                    aHeaders[0] = "New constraint";
                    aHeaders[1] = "";
                    lvi = new ListViewItem(aHeaders);
                    LVConstraint.Items.Add(lvi);

                    ButSaveEditedNote.Visible = false;
                    TBEditedNote.Text = "";
                    TBEditedNote.Visible = false;
                    TBConstraintName.Text = "Name :";
                    TBConstraintName.Visible = false;
                    LabConstraintName.Visible = false;
                    CBConstraintType.Visible = false;
                }
            }
            else if (uiState.Equals("TRADNEWCONSTRAINT"))
            {
                XMLConstraint anXMLConstraint = null;

                foreach (XMLConstraint aXMLConstraint in traditionnalConstraintList)
                {
                    if ((aXMLConstraint.GetType()+"-"+aXMLConstraint.GetName()).Equals(CBTradConstraint.Text))
                    {
                        anXMLConstraint = aXMLConstraint;
                    }
                }
                if (!(anXMLConstraint==null))
                {
                    EAClassConstraint newConstraint = populatedEAClass.AddConstraint(anXMLConstraint.GetName(), anXMLConstraint.GetType(), anXMLConstraint.GetNote());
                    newConstraint.SetNotes(TBEditedNote.Text);

                    foreach (ListViewItem anItem in LVConstraint.Items)
                    {
                        anItem.Remove();
                    }

                    ListViewItem lvi;
                    String[] aHeaders = new string[2];
                    foreach (EAClassConstraint aConstraint in populatedEAClass.GetConstraints())
                    {
                        aHeaders[0] = aConstraint.GetName();
                        aHeaders[1] = aConstraint.GetNotes().Trim();
                        lvi = new ListViewItem(aHeaders);
                        LVConstraint.Items.Add(lvi);
                    }
                    aHeaders[0] = "New constraint";
                    aHeaders[1] = "";
                    lvi = new ListViewItem(aHeaders);
                    LVConstraint.Items.Add(lvi);

                    currentEditedConstraint = "";
                    TBEditedNote.Text = "";
                    CBTradConstraint.Visible = false;
                    ButSaveEditedNote.Visible = false;
                    TBEditedNote.Visible = false;
                }
            }
        }

        private void ButDone_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void LVConstraint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVConstraint.SelectedItems.Count.Equals(1))
            {

                CBTradConstraint.Visible = false;
                RBCancel.Checked = false;
                RBScratch.Checked = false;
                RBTradConstraint.Checked = false;
                TBEditedNote.Enabled = true;
                LabConstraintName.Visible = false;
                TBConstraintName.Visible = false;
                CBTradConstraint.Text = null;

                if (!LVConstraint.SelectedItems[0].Text.Equals("New constraint"))
                {
                    RBCancel.Visible = false;
                    RBScratch.Visible = false;
                    RBTradConstraint.Visible = false;
                    LabConstraintName.Visible = true;
                    TBConstraintName.Visible = true;
                    TBEditedNote.Visible = true;
                    ButSaveEditedNote.Visible = true;
                    ButDeleteConst.Visible = true;
                    CBConstraintType.Visible = false;
                    EAClassConstraint aConstraint = populatedEAClass.GetAConstraint(LVConstraint.SelectedItems[0].SubItems[0].Text);
                    TBEditedNote.Text = aConstraint.GetNotes();
                    TBConstraintName.Text = aConstraint.GetName();
                    currentEditedConstraint = aConstraint.GetName();
                    uiState = "EDITINGCONSTRAINT";
                }
                else if (LVConstraint.SelectedItems[0].Text.Equals("New constraint"))
                {
                    RBCancel.Visible = true;
                    RBScratch.Visible = true;
                    RBTradConstraint.Visible = true;
                    TBEditedNote.Visible = false;
                    LabConstraintName.Visible = false;
                    TBConstraintName.Visible = false;
                    ButSaveEditedNote.Visible = false;
                    CBConstraintType.Visible = false;
                    ButDeleteConst.Visible = false;

                    uiState = "NEWCONSTRAINT";
                }
            }
        }

        private void RBScratch_CheckedChanged(object sender, EventArgs e)
        {
            RBCancel.Visible = false;
            RBScratch.Visible = false;
            RBTradConstraint.Visible = false;
            RBCancel.Checked = false;
            RBScratch.Checked = false;
            RBTradConstraint.Checked = false;
            TBEditedNote.Text = "";
            TBEditedNote.Visible = true;
            ButSaveEditedNote.Visible = true;
            LabConstraintName.Text = "Name :";
            LabConstraintName.Visible = true;
            TBConstraintName.Text = "";
            TBConstraintName.Visible = true;
            CBConstraintType.Visible = true;
            LVConstraint.SelectedItems.Clear();
            uiState = "SCRATCHNEWCONSTRAINT";
        }

        private void RBTradConstraint_CheckedChanged(object sender, EventArgs e)
        {
            RBCancel.Visible = false;
            RBScratch.Visible = false;
            RBTradConstraint.Visible = false;
            RBCancel.Checked = false;
            RBScratch.Checked = false;
            RBTradConstraint.Checked = false;
            uiState = "TRADNEWCONSTRAINT";

            LVConstraint.SelectedItems.Clear();
            CBTradConstraint.Visible = true;
        }

        private void RBCancel_CheckedChanged(object sender, EventArgs e)
        {
            RBCancel.Visible = false;
            RBScratch.Visible = false;
            RBTradConstraint.Visible = false;
            RBCancel.Checked = false;
            RBScratch.Checked = false;
            RBTradConstraint.Checked = false;
            TBEditedNote.Visible = false;
            CBTradConstraint.Visible = false;
            ButSaveEditedNote.Visible = false;
            CBConstraintType.Visible = false;
            ButDeleteConst.Visible = false;
            uiState = "";
            LVConstraint.SelectedItems.Clear();
        }

        private void CBTradConstraint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBTradConstraint.Text.Equals("Cancel"))
            {
                CBTradConstraint.Visible = false;
                TBEditedNote.Text = "";
                TBEditedNote.Visible = false;
                TBEditedNote.Enabled = true;
                ButSaveEditedNote.Visible = false;
                CBTradConstraint.Text = null;
                LVConstraint.SelectedItems.Clear();
                uiState = "";
            }
            else if (CBTradConstraint.Items.Contains(CBTradConstraint.Text))
            {

                TBEditedNote.Enabled = true;
                TBEditedNote.Visible = true;
                foreach (XMLConstraint aConstraint in traditionnalConstraintList)
                {
                    if (CBTradConstraint.Text.Equals(aConstraint.GetType()+"-"+aConstraint.GetName()))
                    {
                        TBEditedNote.Text = aConstraint.GetNote();
                    }
                }
                ButSaveEditedNote.Visible = true;
                TBConstraintName.Text = CBTradConstraint.Text;
                CBTradConstraint.Visible = false;
                CBTradConstraint.Text = "";
                LabConstraintName.Visible = true;
                TBConstraintName.Visible = true;

            }
        }

        private void ButShowDef_Click(object sender, EventArgs e)
        {
            DefForm DF = new DefForm();
            DF.Show();
        }

        private void ButDeleteConst_Click(object sender, EventArgs e)
        {
            populatedEAClass.DeleteConstraint(currentEditedConstraint);

            foreach (ListViewItem anItem in LVConstraint.Items)
            {
                anItem.Remove();
            }

            ListViewItem lvi;
            String[] aHeaders = new string[2];
            foreach (EAClassConstraint aConstraint in populatedEAClass.GetConstraints())
            {
                aHeaders[0] = aConstraint.GetName();
                aHeaders[1] = aConstraint.GetNotes().Trim();
                lvi = new ListViewItem(aHeaders);
                LVConstraint.Items.Add(lvi);
            }
            aHeaders[0] = "New constraint";
            aHeaders[1] = "";
            lvi = new ListViewItem(aHeaders);
            LVConstraint.Items.Add(lvi);

            currentEditedConstraint = "";
            TBEditedNote.Text = "";
            ButSaveEditedNote.Visible = false;
            LabConstraintName.Visible = false;
            TBConstraintName.Visible = false;
            TBEditedNote.Visible = false;
            TBEditedNote.Enabled = true;
            ButDeleteConst.Visible = false;
        }

    }
}
