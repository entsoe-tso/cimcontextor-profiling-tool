using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
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
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();   
        }

        
        public void AppendLog(String Log)
        {
            TBLog.Text = TBLog.Text + Log + "\r\n";
            TBLog.SelectionStart = TBLog.Text.Length;
            TBLog.ScrollToCaret();
        }

        public void AppendTitle(string Title)
        {
            TBLog.Text = TBLog.Text + "** " + Title + " **" + "\r\n";
            TBLog.SelectionStart = TBLog.Text.Length;
            TBLog.ScrollToCaret();
        }
        public void AppendSubTitle(string Title)
        {
            TBLog.Text = TBLog.Text + "* " + Title + " *" + "\r\n";
            TBLog.SelectionStart = TBLog.Text.Length;
            TBLog.ScrollToCaret();
        }

        public void JumpALine()
        {
            TBLog.Text = TBLog.Text + "\r\n";
            TBLog.SelectionStart = TBLog.Text.Length;
            TBLog.ScrollToCaret();
        }

        private void LogForm_Load(object sender, EventArgs e)
        {

        }

        private void TBLog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
