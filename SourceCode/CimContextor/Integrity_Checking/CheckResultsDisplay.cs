/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20221226
/////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CimContextor.Integrity_Checking
{
    public partial class CheckResultsDisplay : Form
    {
        private List<ValidationEntry> validationsList = new List<ValidationEntry>();
        private string csvText = "";
        private readonly string integrityCheckTitle = "";
        private string csvDelim = OptionForm.COMMA_CHAR;
        private XMLParser xmlParser = null;
        public CheckResultsDisplay(string integrityCheckTitle, List<ValidationEntry> validationsList, XMLParser xmlParser)
        {
            InitializeComponent();
            this.validationsList = validationsList;
            this.integrityCheckTitle = integrityCheckTitle;
            this.xmlParser = xmlParser;
            string csvDelimName = xmlParser.GetXmlValueConfig("CsvDelimiter");
            if(csvDelimName == null ) 
            {
                MessageBox.Show("Cannot retrieve CSV delimiter from configuration. Take default value (comma).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                csvDelim = OptionForm.COMMA_CHAR; 
            } else
            {
                switch(csvDelimName)
                {
                    case OptionForm.COMMA: csvDelim = OptionForm.COMMA_CHAR; break;
                    case OptionForm.SEMICOLON: csvDelim = OptionForm.SEMICOLON_CHAR; break;
                    case OptionForm.TABULATOR: csvDelim = OptionForm.TABULATOR_CHAR; break;
                    default: csvDelim = OptionForm.COMMA_CHAR; break;
                }
            }
        }

        public string CsvDelim
        {
            get { return csvDelim; }
            set { csvDelim = value; }
        }

        public void fillText()
        {
            if(validationsList.Count <= 0)
            {
                resultsTB.SelectionColor = Color.DarkGreen;
                resultsTB.AppendText("Everything is correct.");
                saveCSVBn.Enabled = false;
            }
            else
            {
                // title and header of csv output
                csvText = csvText + integrityCheckTitle + csvDelim + "\n";
                foreach(string head in ValidationEntry.headers)
                {
                    csvText = csvText + head + csvDelim; 
                }
                csvText += "\n";

                // output of errors and warnings...
                foreach (ValidationEntry entry in validationsList)
                {
                    
                    if(entry.SeverityLevel.Equals(Severity.ERROR))
                    {
                        resultsTB.SelectionColor = Color.DarkRed;
                    } 
                    else if(entry.SeverityLevel.Equals(Severity.WARNING))
                    {
                        resultsTB.SelectionColor = Color.DarkOrange;
                    }
                    resultsTB.AppendText(entry.SeverityLevel);
                    resultsTB.SelectionColor = resultsTB.ForeColor;
                    resultsTB.AppendText(" " + entry.ValidationCode + " ");
                    if(entry.CheckedPackage != null)
                    {
                        resultsTB.AppendText(" in package " + entry.CheckedPackage + ": ");
                    } 
                    else if(entry.CheckedClass != null)
                    {
                        resultsTB.AppendText(" in class " + entry.CheckedClass + ": ");
                    }
                    resultsTB.AppendText(entry.Message + "\n");

                    csvText = csvText + entry.SeverityLevel + csvDelim + entry.ValidationCode + csvDelim + entry.Message + "\n";
                }
                saveCSVBn.Enabled = true;
            }
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void CloseDisplay(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void SaveToCSV(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = @"C:\";
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.Filter = @"CSV (*.csv)|*.csv";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.Title = @"Save an Integrity Check Result File";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog.FileName != "")
                {
                    Stream fileStream = null;
                    if ((fileStream = saveFileDialog.OpenFile()) != null)
                    {

                        byte[] info = new UTF8Encoding(true).GetBytes(csvText);
                        fileStream.Write(info, 0, info.Length);
                        fileStream.Close();
                    }
                }
                saveFileDialog.Dispose();
            } 
            else // ABA20230504
            {
                saveFileDialog.Dispose();
            }
        }
    }
}
