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
namespace CimContextor.EditConnectors
{
    public partial class    QualifierManagerForm : Form
    {
      
        private ConstantDefinition CD = new ConstantDefinition();
        private ComboBox CBSupplierQualifier;
        private ComboBox  CBClientQualifier;
        private EA.Repository repo;

        public QualifierManagerForm(ComboBox  CBSupplierQualifier,ComboBox CBClientQualifier, EA.Repository repo)
        {
            InitializeComponent();
       
            this.CBSupplierQualifier = CBSupplierQualifier;
            this.CBClientQualifier = CBClientQualifier;
            this.repo = repo;
            XMLParser XMLP = new XMLParser(repo);
            ArrayList QualifierList = new ArrayList();
            QualifierList = XMLP.GetXmlQualifier("role");
            QualifierList.Add("No qualifier");

            foreach (object aQualifier in QualifierList)
            {
                this.CBQualifier.Items.Add((string)aQualifier);
        
            }
           this.CBQualifier.SelectedItem = "No qualifier";       
           
        }

   

        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

 
        private void ButDeleteQualifier_Click(object sender, EventArgs e)
        {
            if (!(CBQualifier.SelectedItem == null))
            {

                if ((!CBQualifier.SelectedItem.ToString().Equals("No qualifier")) && (!CBQualifier.Text.Equals("")))
                {
                    if ((!CBSupplierQualifier.Text.Equals(CBQualifier.Text)) && (!CBClientQualifier.Text.Equals(CBQualifier.Text)))
                    {

                        DialogResult result = MessageBox.Show("Do you really want to delete this qualifier ? It won't affect already qualified objects", "Add-in",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (result.Equals(DialogResult.Yes))
                        {
                            XMLParser XMLP = new XMLParser(repo);
                            string stringToDelete = CBQualifier.SelectedItem.ToString();
                            XMLP.DeleteXmlQualifier(stringToDelete);
                            CBQualifier.Items.Remove(stringToDelete);
                            CBClientQualifier.Items.Remove(stringToDelete);
                            CBSupplierQualifier.Items.Remove(stringToDelete);
                            this.CBQualifier.SelectedItem = "No qualifier";
                        }
                    }
                    else
                    {
                        MessageBox.Show("You should select another qualifier before deleting it.", "IsBasedOn Add-in",
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

        private string GetFormatedQualifierFromCreate()
        {
            if(!(TBCreateQualifier.Text==null)){
            return (TBCreateQualifier.Text.Substring(0, 1).ToUpper() + TBCreateQualifier.Text.Remove(0, 1)).Replace(" ", "").Replace("_","");
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
        private string GetFormatedQualifierFromClientComboBox(bool Client)
        {
            if(Client.Equals(true)){
            if (!(CBQualifier == null))
            {
                return (CBClientQualifier.Text.Substring(0, 1).ToUpper() + CBClientQualifier.Text.Remove(0, 1)).Replace(" ", "").Replace("_", "");
            }
            else
            {
                return "";
            }
            }else{
                if (!(CBQualifier == null))
            {
                return (CBSupplierQualifier.Text.Substring(0, 1).ToUpper() + CBSupplierQualifier.Text.Remove(0, 1)).Replace(" ", "").Replace("_", "");
            }
            else
            {
                return "";
            }
            }
        }


        private void ButCreateQualifier_Click(object sender, EventArgs e)
        {
            if (!TBCreateQualifier.Text.Equals(""))
            {
                if (!CBSupplierQualifier.Items.Contains(TBCreateQualifier.Text))
                {
                    CBSupplierQualifier.Items.Add(GetFormatedQualifierFromCreate());
                    CBClientQualifier.Items.Add(GetFormatedQualifierFromCreate());
                }
                if (CBAllowedToAny.Checked.Equals(true))
                {
                    XMLParser xmlParser = new XMLParser(repo);
                    xmlParser.AddXmlQualifier(GetFormatedQualifierFromCreate(), "any");
                }
                else
                {
                    XMLParser xmlParser = new XMLParser(repo);
                    xmlParser.AddXmlQualifier(GetFormatedQualifierFromCreate(), "role");
                }
                CBAllowedToAny.Checked = true;
                TBCreateQualifier.Text = "";
            }
        }
       
   

  

    }
}
