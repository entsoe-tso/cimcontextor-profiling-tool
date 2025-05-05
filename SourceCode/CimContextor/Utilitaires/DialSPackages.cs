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
namespace CimContextor.utilitaires
{
    public partial class DialSPackages : Form
    {

        List<string> selectedpackages1;
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
        public List<string> selectedpackages
        {
            get
            {
                return this.selectedpackages1;
            }
            set
            {

            }
        }

      
        public string profname
        {
            get
            {
                return this.textBox1.Text;
            }
            set
            {

            }
        }


        public DialSPackages()
        {
            InitializeComponent();
            // modifs am mars 2019

            // Changes the selection mode from double-click to single click.
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.SelectionMode = SelectionMode.One;
            this.textBox1.Text = "newprofile";
            this.label1.Visible = false;
            this.textBox1.Visible = false;
            

        }

        private void DialSPackages_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Main.ongoing = true;

            StringBuilder ss = new StringBuilder();
            selectedpackages1 = new List<string>();
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                if (this.checkedListBox1.GetItemChecked(i))
                {
                    // MessageBox.Show(checkedListBox1.Items[i].ToString());
                    ss.Append("||" + checkedListBox1.Items[i].ToString());
                    selectedpackages1.Add(checkedListBox1.Items[i].ToString());

                }
            }
            //MessageBox.Show(ss.ToString());
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main.ongoing = false;
            this.Close();
        }

       

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textbox2_enter(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {



            }
        }

        private void checkBox1_Changed(object sender, EventArgs e)
        {
             if (this.checkBox1.Checked == true)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);

                }
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
        }
        private void label2_Click_1(object sender, EventArgs e)
        {

        }

       
    }
        }
    
