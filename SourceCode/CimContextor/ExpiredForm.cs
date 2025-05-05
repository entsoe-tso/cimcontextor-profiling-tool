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
    public partial class ExpiredForm : Form
    {
        public ExpiredForm()
        {
            InitializeComponent();
            this.Text = "Thank you for trying CimContextor";
            ConstantDefinition CD = new ConstantDefinition();
            TBExpired.Text = CD.GetExpirationWarning();
            LabExpirationDate.Text = LabExpirationDate.Text + " " + ConstantDefinition.GetExpirationDate();
        }

        private void ButClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
