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
    public partial class DialSetDescriptionStereotype : Form
    {
        private EA.Element elem;
        private EA.Repository repo;

        public DialSetDescriptionStereotype(EA.Repository repo, EA.Element elem)
        {
            InitializeComponent();
            this.elem = elem;
            this.repo = repo;
        }

        private void buttonStereo_Click(object sender, EventArgs e)
        {
            bool exists = false;

            if(elem != null && elem.Type.Equals("Class"))
            {
                foreach(EA.Stereotype stereo in repo.Stereotypes)
                {
                    if (stereo.Name.Equals(Constants.DESCRIPTION))
                    {
                        exists = true;
                        break;
                    }
                }

                if(!exists)
                {
                    EA.Stereotype stereo = (EA.Stereotype)repo.Stereotypes.AddNew(Constants.DESCRIPTION, "Class");
                    if(stereo != null)
                    {
                        stereo.Update();
                    }
                    repo.Stereotypes.Refresh(); 
                }

                if(!elem.StereotypeEx.Contains(Constants.DESCRIPTION))
                {
                    elem.Stereotype = Constants.DESCRIPTION;
                    elem.Update();
                }
                EA.Collection tagValues = elem.TaggedValues;
                if(!TaggedValueExists(tagValues, Constants.RDF_ABOUT))
                {
                    EA.TaggedValue newTV = (EA.TaggedValue)tagValues.AddNew(Constants.RDF_ABOUT, "");
                    if(newTV != null)
                    {
                        newTV.Value = "true";
                        newTV.Update();
                    }
                    tagValues.Refresh();
                    elem.Update();
                    elem.Refresh();
                }
            }
            EA.Diagram currentDiag = repo.GetCurrentDiagram();
            if(currentDiag != null) repo.ReloadDiagram(currentDiag.DiagramID);
            this.Dispose();
        }

        private bool TaggedValueExists(EA.Collection tagValues, string tagValueName)
        {
            foreach(EA.TaggedValue tv in tagValues)
            {
                if (tv.Name.Equals(tagValueName)) return true;
            }
            return false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
