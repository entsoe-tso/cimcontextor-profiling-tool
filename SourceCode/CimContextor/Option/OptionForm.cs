using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using CimContextor.Utilities;
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
    public partial class OptionForm : Form
    {
        private Color SelectedColor;
        private XMLParser XMLP;
        string hexColor;
        public const string COMMA     = "Comma";
        public const string SEMICOLON = "Semicolon";
        public const string TABULATOR = "Tabulator";
        public const string COMMA_CHAR = ",";
        public const string SEMICOLON_CHAR = ";";
        public const string TABULATOR_CHAR = "\t";
        private string csvDelim = COMMA_CHAR; 
        public OptionForm(EA.Repository repo)
        {
            StackFrame fr = new StackFrame(0, true);
            string prov = fr.GetMethod().ToString() + " in " + Path.GetFileName(fr.GetFileName()); // ABA20230521 language 
            StackTrace st = new StackTrace(true);
            StackFrame sf;
            for (int i = 0; i < st.FrameCount; i++)
            {
                // Note that high up the call stack, there is only
                // one stack frame.
                sf = st.GetFrame(i);
                prov = sf.GetMethod().ToString();
            }
            InitializeComponent();
            XMLP = new XMLParser(repo);
            XMLP.ReadConfig();

            if (XMLP.GetXmlValueConfig("EnablePropertyGrouping") == Configuration.ConfigurationManager.CHECKED)
            {
                EnablePropertyGrouping.CheckState = CheckState.Checked;
            }
            else
            { EnablePropertyGrouping.CheckState = CheckState.Unchecked; }

            if (XMLP.GetXmlValueConfig("IsBasedOn") == Configuration.ConfigurationManager.CHECKED)
            {
                CBIsBasedOn.CheckState = CheckState.Checked;
            }
            else
            { CBIsBasedOn.CheckState = CheckState.Unchecked; }

            if (XMLP.GetXmlValueConfig("Confirm") == Configuration.ConfigurationManager.CHECKED)
            { CBConfirm.CheckState = CheckState.Checked; }
            else
            { CBConfirm.CheckState = CheckState.Unchecked; }

            if (XMLP.GetXmlValueConfig("Log") == Configuration.ConfigurationManager.CHECKED)
            { CBLog.CheckState = CheckState.Checked; }
            else
            { CBLog.CheckState = CheckState.Unchecked; }

            if (XMLP.GetXmlValueConfig("QualifyDatatypeEnumCompound") == Configuration.ConfigurationManager.CHECKED)
            { CBDataEnumQualifierNeeded.CheckState = CheckState.Checked; }
            else
            { CBDataEnumQualifierNeeded.CheckState = CheckState.Unchecked; }


            if (XMLP.GetXmlValueConfig("Warning") == Configuration.ConfigurationManager.CHECKED)
            { CBWarning.CheckState = CheckState.Checked; }
            else
            { CBWarning.CheckState = CheckState.Unchecked; }

            if (XMLP.GetXmlValueConfig("CopyParentElement") == Configuration.ConfigurationManager.CHECKED)
            { CBCopyParentElement.CheckState = CheckState.Checked; }
            else
            { CBCopyParentElement.CheckState = CheckState.Unchecked; }

            if (XMLP.GetXmlValueConfig("EnableConcreteInheritanceInProfiles") == Configuration.ConfigurationManager.CHECKED)
            { CBEnableConcreteInheritanceInProfiles.CheckState = CheckState.Checked; }
            else
            { CBEnableConcreteInheritanceInProfiles.CheckState = CheckState.Unchecked; }

            if (XMLP.GetXmlValueConfig("ProfileModelKind") == "hierarchical")
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
            }
            else
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }

            hexColor = XMLP.GetXmlValueConfig("ConfigColor");
            if (hexColor.Equals("Default") || hexColor.Equals(Configuration.ConfigurationManager.CHECKED))
            {
                CBDefaultBackgroundColor.CheckState = CheckState.Checked;
                ButSelectColor.Enabled = false;
            }
            else
            {
                CBDefaultBackgroundColor.CheckState = CheckState.Unchecked;
                ButSelectColor.Enabled = true;
            }
            if (XMLP.GetXmlValueConfig("NavigationEnabled") == Configuration.ConfigurationManager.CHECKED)
            { checkBox2.CheckState = CheckState.Checked; }
            else
            { checkBox2.CheckState = CheckState.Unchecked; }
            
            
                if (XMLP.GetXmlValueConfig("AutomaticChangeOfRoleName") == Configuration.ConfigurationManager.CHECKED)
                { CBAutoNames.CheckState = CheckState.Checked; }
                else
                { CBAutoNames.CheckState = CheckState.Unchecked; }
            if (XMLP.GetXmlValueConfig("W13AutomaticAnscesterInProfile") == Configuration.ConfigurationManager.CHECKED)
            { CBWG13Ancesters.CheckState = CheckState.Checked; }
            else
            { CBWG13Ancesters.CheckState = CheckState.Unchecked; }

            this.StartPosition = FormStartPosition.CenterScreen;
            this.csvDelimComboBox.Items.Insert(0, COMMA);
            this.csvDelimComboBox.Items.Insert(1, SEMICOLON);
            this.csvDelimComboBox.Items.Insert(2, TABULATOR);
            this.csvDelimComboBox.SelectedIndex = 0; // default if the following will not work (ABA20230227)

            string csvDelim = XMLP.GetXmlValueConfig("CsvDelimiter");
            if (csvDelim != null)
            {
                if(csvDelim.Equals(COMMA)) this.csvDelimComboBox.SelectedIndex = 0;
                else if (csvDelim.Equals(SEMICOLON)) this.csvDelimComboBox.SelectedIndex = 1;
                else if (csvDelim.Equals(TABULATOR)) this.csvDelimComboBox.SelectedIndex = 2;
                else
                {
                    ErrorCodes.ShowError(ErrorCodes.ERROR_029);
                }
            }
            else
            {
                ErrorCodes.ShowError(ErrorCodes.ERROR_030);
            }
        }

        public string CsvDelim
        {
            get { return csvDelim;  }
            set { csvDelim = value; }
        } 
        private void ButSave_Click(object sender, EventArgs e)
        {
            MsgBox msgBox = new MsgBox();
            msgBox.ShowBox("Saving data ...");
            XMLP.SetXmlValueConfig("IsBasedOn", CBIsBasedOn.CheckState.ToString());
            XMLP.SetXmlValueConfig("Confirm", CBConfirm.CheckState.ToString());
            XMLP.SetXmlValueConfig("Log", CBLog.CheckState.ToString());
            XMLP.SetXmlValueConfig("Warning", CBWarning.CheckState.ToString());
            XMLP.SetXmlValueConfig("QualifyDatatypeEnumCompound", CBDataEnumQualifierNeeded.CheckState.ToString());
            XMLP.SetXmlValueConfig("CopyParentElement", CBCopyParentElement.CheckState.ToString());
            XMLP.SetXmlValueConfig("EnablePropertyGrouping", EnablePropertyGrouping.CheckState.ToString());
            XMLP.SetXmlValueConfig("EnableConcreteInheritanceInProfiles", CBEnableConcreteInheritanceInProfiles.CheckState.ToString());
            XMLP.SetXmlValueConfig("NavigationEnabled", checkBox2.CheckState.ToString());
            XMLP.SetXmlValueConfig("AutomaticChangeOfRoleName", CBAutoNames.CheckState.ToString());
            XMLP.SetXmlValueConfig("W13AutomaticAnscesterInProfile", CBWG13Ancesters.CheckState.ToString());
            XMLP.SetXmlValueConfig("CsvDelimiter", Convert.ToString(csvDelimComboBox.SelectedItem));
            if (radioButton2.Checked == true)
            {
                XMLP.SetXmlValueConfig("ProfileModelKind", "graph");
            }
            else
            {
                XMLP.SetXmlValueConfig("ProfileModelKind", "hierarchical");
            }
            
            
            if (CBDefaultBackgroundColor.Checked.Equals(true))
            {
                XMLP.SetXmlValueConfig("ConfigColor", "Default");
            }
            else
            {
                if(SelectedColor==null)
                {
                DialogResult Choice = MessageBox.Show("No color selected;\n Do you wish to continue (color will be set back to default)?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    if (Choice.Equals(DialogResult.OK))
                    {
                        XMLP.SetXmlValueConfig("ConfigColor", "Default");
                    }
                    else
                    {
                        return;
                    }
                }
                else{
                    if (!(SelectedColor==null))
                    {
                    hexColor = SelectedColor.B.ToString("X2") + SelectedColor.G.ToString("X2") + SelectedColor.R.ToString("X2");
                    }
                    XMLP.SetXmlValueConfig("ConfigColor", int.Parse(hexColor,System.Globalization.NumberStyles.HexNumber).ToString());
                }
            }
            msgBox.CloseBox();
            this.Dispose();
        }

        private void CBDefaultBackgroundColor_CheckedChanged(object sender, EventArgs e)
        {
            if (CBDefaultBackgroundColor.Checked.Equals(true))
            {
                ButSelectColor.Enabled = false;
            }
            else
            {
                ButSelectColor.Enabled = true;
            }
        }

        private void ButSelectColor_Click(object sender, EventArgs e)
        {
            CDBackground = new ColorDialog();
            if (CDBackground.ShowDialog() != DialogResult.Cancel)
            {
                SelectedColor = CDBackground.Color;
            }
        }

        private void EnablePropertyGrouping_CheckedChanged(object sender, EventArgs e)
        {
            /*

            if (EnablePropertyGrouping.Checked.Equals(true))
            {
                EnablePropertyGrouping.Checked = false;
            }
            else
            {
                EnablePropertyGrouping.Checked = true;
            }
             * */
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void GBAddinSettings_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void RB1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                radioButton2.Checked = false; // can't have simultaneously the two buttons checked
            }
        }

        private void checkBox1_CheckedChanged_2(object sender, EventArgs e)
        {
        }
        private void RB2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                radioButton1.Checked = false; // can't have simultaneously the two buttons checked
            }
           
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void csvDelimSelected(object sender, EventArgs e)
        {
            string selectedTxt = this.csvDelimComboBox.Text;
            if (selectedTxt == COMMA) this.csvDelim = ",";
            else if (selectedTxt == SEMICOLON) this.csvDelim = ";";
            else if (selectedTxt == TABULATOR) this.csvDelim = "\t";
        }
    }
}
