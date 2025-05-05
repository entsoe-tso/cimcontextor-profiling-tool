using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
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
    public partial class DialSelect : Form
    {
         string selectedpackage1="";
        public CheckedListBox dialbox
        {
            get
            {
                return this.checkedListBox1;
            }
            set
            {

            }
        }
        public string selectedpackage
        {
            get
            {
                return this.selectedpackage1;
            }
            set
            {

            }
        }

  
        


        public DialSelect()
        {
            InitializeComponent(); // Changes the selection mode from double-click to single click.
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.SelectionMode = SelectionMode.One;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            StringBuilder ss = new StringBuilder();
           
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                if (this.checkedListBox1.GetItemChecked(i))
                {
                    
                    this.selectedpackage1 = checkedListBox1.Items[i].ToString();

                }
            }
            //MessageBox.Show(ss.ToString());
            this.Close();
        }
       

//-----------------------------
    }
}
