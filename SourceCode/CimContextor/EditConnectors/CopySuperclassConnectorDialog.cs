using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CimContextor.EditConnectors
{
    public partial class CopySuperclassConnectorDialog : Form
    {
        private bool takeOverRoles = true;
        private EA.Repository repo = null;
        public CopySuperclassConnectorDialog(EA.Repository repo)
        {
            this.repo = repo;
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        private void createBtn_Click(object sender, EventArgs e)
        {
            SuperclassConnectorCopier scCopier = new SuperclassConnectorCopier(this.repo);
            scCopier.CopySuperclassConnectors(true);
            this.Dispose();
        }

        private void explanationLB_Click(object sender, EventArgs e)
        {

        }
    }
}
