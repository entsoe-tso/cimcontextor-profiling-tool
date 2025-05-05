/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20230204
/////////////////////////////////////////////////////////////////////////////////////////


using CimContextor.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CimContextor.Utilities
{
    public partial class ImportConfigurationDialog : Form
    {
        private EA.Repository repo;
        public ImportConfigurationDialog(EA.Repository repo)
        {
            this.repo = repo;
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void SelectBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = @"Select Configuration File",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "xml",
                Filter = @"xml files (*.xml)|*.xml",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.selectConfigTB.Text = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }

        private void importBtn_Click(object sender, EventArgs e)
        {
            string fullPath = this.selectConfigTB.Text;
            string configContent = File.ReadAllText(fullPath);
            ConfigurationManager configManager = ConfigurationManager.GetConfigurationManager(repo);
            string xsdString = FileManager.GetTextByName("CimContextor.Configuration.", "CimContextor_Configuration_Schema.xsd");
            string result = configManager.ValidateConfigXML(configContent, xsdString);
            if(result != null)
            {
                MessageBox.Show(ErrorCodes.ERROR_007[0] + ": " + ErrorCodes.ERROR_007[1] + "\n" + result,
                                "XML Parsing Error", 
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            } else
            {
                EA.Element configElem = configManager.GetConfigElement();
                EA.TaggedValue tv = configManager.GetExistingConfigTaggedValue(configElem);
                if (tv == null)
                {
                    tv = configManager.CreateConfigTaggedValue(configElem);
                }
                tv.Value = "<memo>";
                tv.Notes = configContent;
                tv.Update();
                configElem.TaggedValues.Refresh();
                string customConfig = configManager.LoadCustomConfiguration();
                if (customConfig == null)
                {
                    MessageBox.Show(ErrorCodes.ERROR_006[0] + ": " + ErrorCodes.ERROR_006[1]);
                }
            }
            this.Dispose();
        }

        private void selectConfigTB_TextChanged(object sender, EventArgs e)
        {
            if(this.selectConfigTB.Text != null && this.selectConfigTB.Text.Length > 0)
            {
                this.importBtn.Enabled = true;
            }
        }
    }
}
