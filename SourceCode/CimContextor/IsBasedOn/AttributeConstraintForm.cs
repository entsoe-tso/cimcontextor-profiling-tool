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
    public partial class AttributeConstraintForm : Form
    {
        private EAClassAttribute activeAttribute;
        private string currentEditedConstraint="";
        private string uiState = "";
        private ArrayList traditionnalConstraintList = new ArrayList();
        private ConstantDefinition CD = new ConstantDefinition();

        public AttributeConstraintForm(EAClass populatedEAClass,string attributeGUID, EA.Repository repo)
        {   
            InitializeComponent();
            activeAttribute = populatedEAClass.GetAttribute(attributeGUID);
            LabAttributName.Text = LabAttributName.Text + " " + activeAttribute.GetName();
            ListViewItem lvi;
            String[] aHeaders = new string[2];
            ArrayList ConstraintList = new ArrayList();
               ConstraintList = activeAttribute.GetConstraints();
            foreach(EAClassAttributeConstraint aConstraint in ConstraintList){         
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
            traditionnalConstraintList = xmlParser.GetXmlConstraint("attribute");
            foreach (XMLConstraint aConstraint in traditionnalConstraintList)
            {
                CBTradConstraint.Items.Add(aConstraint.GetName());
            }

            CBTradConstraint.Items.Add("Cancel");
            //Init CBConstrainttype
            CBConstraintType.Items.Add("OCL");
            CBConstraintType.Items.Add("Invariant");
            CBConstraintType.SelectedItem="OCL";

        }

        /*
        private void ButEditNote_Click(object sender, EventArgs e)
        {
            if(LVConstraint.SelectedItems.Count>0){
                EAClassAttributeConstraint aConstraint = activeAttribute.GetAnAttributeConstraint(LVConstraint.SelectedItems[0].SubItems[0].Text);
                TBEditedNote.Text = aConstraint.GetNotes();
                currentEditedConstraint=aConstraint.GetName();  
            }
        }*/

        private void ButSaveEditedNote_Click(object sender, EventArgs e)
        {
            if(uiState.Equals("EDITINGCONSTRAINT")){
            EAClassAttributeConstraint editedConstraint = activeAttribute.GetAnAttributeConstraint(currentEditedConstraint);
            editedConstraint.SetNotes(TBEditedNote.Text);
            editedConstraint.SetName(TBConstraintName.Text);

            foreach(ListViewItem anItem in LVConstraint.Items ){
                anItem.Remove();
            }

            ListViewItem lvi;
            String[] aHeaders = new string[2];
            foreach(EAClassAttributeConstraint aConstraint in activeAttribute.GetConstraints()){
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
            CBConstraintType.Visible = false;
            ButSaveEditedNote.Visible = false;
            LabConstraintName.Visible = false;
            TBConstraintName.Visible = false;
            TBEditedNote.Visible = false;
            TBEditedNote.Enabled = true;
            ButDeleteConstraint.Visible = false;
            CBConstraintType.Visible = false;
            }
            else if (uiState.Equals("SCRATCHNEWCONSTRAINT"))
            {
                if(!TBConstraintName.Text.Equals("") && !TBEditedNote.Text.Equals("") ){

                    if(!CBConstraintType.Items.Contains(CBConstraintType.Text)){
                        MessageBox.Show("You must select a valid item from for the constraint type list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return;
                    }

                    EAClassAttributeConstraint newConstraint = activeAttribute.AddConstraint(TBConstraintName.Text, CBConstraintType.Text , TBEditedNote.Text);
                    newConstraint.SetNotes(TBEditedNote.Text);

                    foreach (ListViewItem anItem in LVConstraint.Items)
                    {
                        anItem.Remove();
                    }

                    ListViewItem lvi;
                    String[] aHeaders = new string[2];
                    foreach (EAClassAttributeConstraint aConstraint in activeAttribute.GetConstraints())
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
                    CBConstraintType.Visible = true;
                ButSaveEditedNote.Visible = false;
                TBEditedNote.Text = "";
                TBEditedNote.Visible = false;
                TBConstraintName.Text = "Name :";
                TBConstraintName.Visible = false;
                LabConstraintName.Visible = false;
                CBConstraintType.Visible = false;
                ButDeleteConstraint.Visible = false;
                }
            }
            else if (uiState.Equals("TRADNEWCONSTRAINT"))
            {
                XMLConstraint anXMLConstraint=null;
                
                foreach(XMLConstraint aXMLConstraint in traditionnalConstraintList ){
                    if(aXMLConstraint.GetName().Equals(CBTradConstraint.Text)){
                        anXMLConstraint=aXMLConstraint;
                    }
                }
                if (!anXMLConstraint.Equals(null))
                {
                    EAClassAttributeConstraint newConstraint = activeAttribute.AddConstraint(anXMLConstraint.GetName(), anXMLConstraint.GetType(), anXMLConstraint.GetNote());
                    newConstraint.SetNotes(TBEditedNote.Text);

                    foreach (ListViewItem anItem in LVConstraint.Items)
                    {
                        anItem.Remove();
                    }

                    ListViewItem lvi;
                    String[] aHeaders = new string[2];
                    foreach (EAClassAttributeConstraint aConstraint in activeAttribute.GetConstraints())
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
                    CBConstraintType.Visible = false;
                    CBTradConstraint.Visible = false;
                    ButSaveEditedNote.Visible = false;
                    TBEditedNote.Visible = false;
                    ButDeleteConstraint.Visible = false;
                    CBConstraintType.Visible = false;
                }
            }
        }

        private void ButDone_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void LVConstraint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(LVConstraint.SelectedItems.Count.Equals(1)){

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
                    ButDeleteConstraint.Visible = true;
                    CBConstraintType.Visible = false;
                    EAClassAttributeConstraint aConstraint = activeAttribute.GetAnAttributeConstraint(LVConstraint.SelectedItems[0].SubItems[0].Text);
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
                    ButDeleteConstraint.Visible = false;
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
            CBConstraintType.Visible = false;
        }

        private void RBCancel_CheckedChanged(object sender, EventArgs e)
        {
            RBCancel.Visible=false;
            RBScratch.Visible=false;
            RBTradConstraint.Visible=false;
            RBCancel.Checked = false;
            RBScratch.Checked = false;
            RBTradConstraint.Checked = false;
            TBEditedNote.Visible = false;
            CBTradConstraint.Visible = false;
            ButSaveEditedNote.Visible = false;
            CBConstraintType.Visible = false;
            ButDeleteConstraint.Visible = false;
            uiState = "";
            LVConstraint.SelectedItems.Clear();
        }

        private void CBTradConstraint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(CBTradConstraint.Text.Equals("Cancel")){
                CBTradConstraint.Visible=false;
                TBEditedNote.Text = "";
                TBEditedNote.Visible=false;
                TBEditedNote.Enabled = true;
                ButSaveEditedNote.Visible=false;
                CBTradConstraint.Text = null;
                LVConstraint.SelectedItems.Clear();
                uiState="";
            }
            else if(CBTradConstraint.Items.Contains(CBTradConstraint.Text)){
                TBEditedNote.Enabled = true;
                TBEditedNote.Visible = true;
                foreach (XMLConstraint aConstraint in traditionnalConstraintList)
                {
                   if(CBTradConstraint.Text.Equals(aConstraint.GetName())){
                       TBEditedNote.Text = aConstraint.GetNote();
                   }
                }
                ButSaveEditedNote.Visible=true;        
                TBConstraintName.Text=CBTradConstraint.Text;
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

        private void ButDeleteConstraint_Click(object sender, EventArgs e)
        {

                    activeAttribute.DeleteAnAttributeConstraint(currentEditedConstraint);


                    foreach (ListViewItem anItem in LVConstraint.Items)
                    {
                        anItem.Remove();
                    }

                    ListViewItem lvi;
                    String[] aHeaders = new string[2];
                    foreach (EAClassAttributeConstraint aConstraint in activeAttribute.GetConstraints())
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
                    CBConstraintType.Visible = false;
                    ButSaveEditedNote.Visible = false;
                    LabConstraintName.Visible = false;
                    TBConstraintName.Visible = false;
                    TBEditedNote.Visible = false;
                    TBEditedNote.Enabled = true;
                    ButDeleteConstraint.Visible = false;
                    CBConstraintType.Visible = false;
        }


    }
}
