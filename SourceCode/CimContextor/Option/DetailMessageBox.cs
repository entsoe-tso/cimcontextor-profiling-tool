using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

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
    public partial class DetailMessageBox : Form
    {
        public DetailMessageBox(string MainMessage, string DetailMessage, ArrayList ObjectList)
        {
            InitializeComponent();
            LabMain.Text = MainMessage;
            LabelError.Text = DetailMessage;
            this.Text = "Warning !";
            ListViewItem lvi;
            String[] aHeaders = new string[1];
            foreach(string AString in ObjectList){
                aHeaders[0] = AString;
                lvi = new ListViewItem(aHeaders);
                LVObjectError.Items.Add(lvi);
            }
            this.Size = new Size(660, 180);
            this.ShowDialog();
        }

        private void ButOk_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void ButDetails_Click(object sender, EventArgs e)
        {
            if (this.Size.Equals(new Size(660, 180)))
            {
                this.Size = new Size(660,400);
            }else{
                this.Size = new Size(660, 180);
            }
        }
    }
}
