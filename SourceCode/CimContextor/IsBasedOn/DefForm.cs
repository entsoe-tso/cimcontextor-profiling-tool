using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using CimContextor.Utilities;
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
    public partial class DefForm : Form
    {
        public DefForm()
        {
            InitializeComponent();

            try
            {
                String text  = FileManager.GetTextByName("CimContextor.IsBasedOn.", "Constraint-list.txt");
                if(text != null)
                {
                    TBDef.Text = text;
                }
            }
            catch (Exception e)
            {
                TBDef.Text = "An exception occured while reading the versionInfo.txt :" + e;
            }
        }

        private void TBDef_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
