using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CimContextor.Utilities
{
    public partial class LoadingIndicator : Form
    {
        public LoadingIndicator()
        {
            InitializeComponent();
        }

        public void ProgressChanged(int value)
        {
            progressBar.Value = value;
        }

        public void ShowIndicator()
        {
            this.ShowDialog();
        }
    }
}
