/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20221230
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace CimContextor.Utilities
{
    public partial class LicenseDialog : Form
    {
        public LicenseDialog()
        {
            InitializeComponent();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            LicenseManager licenseManager = new LicenseManager();
            string fullPath = this.licenseTBox.Text;
            string licKey = File.ReadAllText(fullPath);
            if (licKey != null)
            {
                licKey = licKey.Trim();
            }

            if(licenseManager.IsValidLicenseKey(licKey))
            {
                try
                {
                    licenseManager.SaveLicense(licKey);
                    this.Close();
                    MessageBox.Show("License successfully saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Dispose();
                } catch(Exception ex)
                {
                    ErrorCodes.ShowException(ErrorCodes.ERROR_038, ex);
                }
            }
            else
            {
                ErrorCodes.ShowError(ErrorCodes.ERROR_039);
            }
            this.Dispose();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void selectFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Select License File",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.licenseTBox.Text = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }
    }
}
