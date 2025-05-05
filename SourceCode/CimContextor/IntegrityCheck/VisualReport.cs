using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace CimContextor
{
    public partial class VisualReport : Form
    {
        private ArrayList reportList;

        public VisualReport(ArrayList ReportList)
        {
            if(ReportList!=null){
                reportList = ReportList;
            }
            else{
                reportList = new ArrayList();
            }
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {

            foreach(Report aReport in reportList){
                
                   String[] aHeaders = new string[1];
                 aHeaders[0] = aReport.GetIBOElementName();
                 ListViewItem lvi = new ListViewItem(aHeaders);

                if(aReport.GetIsOk()!=true){
                    lvi.BackColor=Color.Crimson;  
                }
                LVCheckedElements.Items.Add(lvi);
            }


        }

        private void ButExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void VisualReport_Load(object sender, EventArgs e)
        {

        }
        
        private void LVCheckedElements_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVCheckedElements.SelectedItems != null)
            {
                if (LVCheckedElements.SelectedItems.Count>0)
                {
                InitLVSelectedItem(LVCheckedElements.SelectedItems[0]);
                }
            }
            else
            {
                foreach(ListViewItem lvi in LVSelectedItem.Items){
                    lvi.Remove();
                }
            }
        }

        private void InitLVSelectedItem(ListViewItem LVCheckedelementLvi)
        {
            foreach (ListViewItem alvi in LVSelectedItem.Items)
            {
                alvi.Remove();
            }

            Report SelectedReport=null;
            foreach(Report aReport in reportList){
                if (aReport.GetIBOElementName() == LVCheckedelementLvi.SubItems[0].Text)
                {
                    SelectedReport = aReport;
                    break;
                }
            }

            if(SelectedReport!=null){
                if (SelectedReport.GetClassReport()!= null)
                {
                foreach(String aString in SelectedReport.GetClassReport()){
                    String[] aHeaders = new string[2];
                    aHeaders[0] = SelectedReport.GetClassReportType();
                    aHeaders[1] = aString;
                    ListViewItem lvi = new ListViewItem(aHeaders);
                    LVSelectedItem.Items.Add(lvi);
                }
                }
            }
        }

        private void LVSelectedItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(LVSelectedItem.SelectedItems!=null){
                if(LVSelectedItem.SelectedItems.Count>0){
                    MessageBox.Show(LVSelectedItem.SelectedItems[0].SubItems[1].Text, "Error details", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }
        
    }
}
