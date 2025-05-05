using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

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
namespace CimContextor.EditConnectors
{
    public partial class   ConstraintManagerForm : Form
    {
      
        private ConstantDefinition CD = new ConstantDefinition();
        private ComboBox CBConstraints;
        private EA.Repository repo;

        public ConstraintManagerForm(ComboBox  CBConstraints, EA.Repository repo)
        {
            InitializeComponent();
            this.repo = repo;
            this.CBConstraints = CBConstraints;
            
            XMLParser XMLP = new XMLParser(repo);
            
            ArrayList ConstraintList = new ArrayList();
            ConstraintList = XMLP.GetXmlConstraint("any");
           

            foreach (object aConstraint in ConstraintList)
            {
                this.CBConstraint.Items.Add((string)((XMLConstraint)aConstraint).GetName());
        
            }
            this.CBConstraint.Items.Add("No Constraint");
           this.CBConstraint.SelectedItem = "No Constraint";       
           
        }

   

        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

 
        private void ButDeleteConstraint_Click(object sender, EventArgs e)
        {
            if (!(CBConstraint.SelectedItem == null))
            {

                if ((!CBConstraint.SelectedItem.ToString().Equals("No Constraint")) && (!CBConstraint.Text.Equals("")))
                {
                    if ((!CBConstraints.Text.Equals(CBConstraint.Text)))
                    {

                        DialogResult result = MessageBox.Show("Do you really want to delete this Constraint ? It won't affect already qualified objects", "Add-in",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (result.Equals(DialogResult.Yes))
                        {
                            XMLParser XMLP = new XMLParser(repo);
                            string stringToDelete = CBConstraint.SelectedItem.ToString();
                            XMLP.DeleteXmlConstraint(stringToDelete);
                            CBConstraint.Items.Remove(stringToDelete);
                            CBConstraints.Items.Remove(stringToDelete);
                            this.CBConstraint.SelectedItem = "No Constraint";
                            XMLP.DeleteXmlConstraint(stringToDelete);
                        }
                    }
                    else
                    {
                        MessageBox.Show("You should select another Constraint before deleting it.", "IsBasedOn Add-in",
                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            else
            {
                MessageBox.Show("You should select a valid item from the list before using this function.", "IsBasedOn Add-in",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private string GetFormatedConstraintFromCreate()
        {
            if(!(TBCreateConstraint.Text==null)){
                return (TBCreateConstraint.Text);
            }
            else{
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Client">if false, mean it's the supplierCb that must be used</param>
        /// <returns></returns>
        private string  GetFormatedConstraintFromClientComboBox(bool Client)
        {
            if(Client.Equals(true)){
            if (!(CBConstraint == null))
            {
                return CBConstraints.Text;
            }
            else
            {
                return "";
            }
            }else{
                if (!(CBConstraint == null))
            {
                return (CBConstraints.Text);
            }
            else
            {
                return "";
            }
            }
        }


        private void ButCreateConstraint_Click(object sender, EventArgs e)
        {
            if (!TBCreateConstraint.Text.Equals(""))
            {
                if (!CBConstraints.Items.Contains(TBCreateConstraint.Text))
                {
                    CBConstraints.Items.Add(GetFormatedConstraintFromCreate());
                }
                if (CBAllowedToAny.Checked.Equals(true))
                {
                    XMLParser xmlParser = new XMLParser(repo);
                    xmlParser.AddXmlConstraint(this.TBCreateConstraint.Text,this.TBNotes.Text,this.TBType.Text, "any");
                }
                else
                {
                    //XMLParser xmlParser = new XMLParser();
                 // xmlParser.AddXmlConstraint(this.TBCreateConstraint.Text,this.TBType.Text,this.TBNotes.Text, "role");
                }
                CBAllowedToAny.Checked = true;
                TBCreateConstraint.Text = "";
            }
        }

        private void TBCreateConstraint_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void CBConstraint_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
       
   

  

    }
}
