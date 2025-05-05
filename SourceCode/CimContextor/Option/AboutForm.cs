using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CimContextor.Option
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink();
            }
            catch(Exception)
            {
                MessageBox.Show("Unable to open link.");
            }
        }

        private void VisitLink()
        {
            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.entsoe.eu");
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
