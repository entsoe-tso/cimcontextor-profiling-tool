using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EA;
using CimContextor.utilitaires;
using System.Reflection;
using System.IO;
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
namespace CimContextor.Utilities
{
    public partial class DialTree : Form
    {
        EA.Repository repo = null;
        EA.Package racinepack = null;
        EA.Package toppack = null;
        Dictionary<string, EA.Package> dicCimPackagesByName = null;
        Utilitaires util = null;
        ImageList imagelist;
        bool multiselect = false;
        public string profilename
        {
            get { return this.textBox1.Text; }
            set { this.textBox1.Text = value; }
        }
        public bool textinput
        {
            get { return this.textBox1.Visible; }
            set { this.textBox1.Visible = value; }
        }
        public string treeLabel
        {
            get { return this.label1.Text; }
            set { this.label1.Text = value; }
        }
        public string inputLabel
        {
            get { return this.label2.Text; }
            set { this.label2.Text = value; }
        }
        public bool inputlabelvisible
        {
            get { return this.label2.Visible; }
            set { this.label2.Visible = value; }
        }
        public bool cball
        {
            get { return this.CBAll.Visible; }
            set { this.CBAll.Visible = value; }
        }

        public DialTree(EA.Repository repo, EA.Package racinepack,bool multiselect)
        {
            try
            {
                this.repo = repo;
                this.multiselect = multiselect;
                this.racinepack = racinepack;
                this.toppack = racinepack;
                this.util = new Utilitaires(repo);
                this.dicCimPackagesByName = new Dictionary<string, Package>();
                if (!dicCimPackagesByName.ContainsKey(this.toppack.Name)) dicCimPackagesByName[this.toppack.Name] = toppack;
                util.getAllPackagesInAPackage(repo, toppack, dicCimPackagesByName);


                InitializeComponent();
                this.textBox1.Text = "newprofile";
                imagelist = new ImageList();

                string Path = FileManager.GetParentDirPath(); // ABA20221020
                Bitmap folderClosed = FileManager.GetImageByName("CimContextor.Utilities.", "icons8-dossier-48.png"); // ABA 20230212
                if (folderClosed != null)
                {
                    imagelist.Images.Add(folderClosed);
                }
                Bitmap folderOpen = FileManager.GetImageByName("CimContextor.Utilities.", "icons8-dossier-ouvert-48.png");
                if (folderOpen != null)
                {
                    imagelist.Images.Add(folderOpen);
                }
                treeView1.ImageList = imagelist;
                treeView1.ImageIndex = 0;
                treeView1.SelectedImageIndex = 1;
                if(multiselect)  treeView1.CheckBoxes = true;
                TreeNode rootNode = new TreeNode(toppack.Name);
                TreeNode allnodes = getNodeFrom(toppack);
                treeView1.Nodes.Add(allnodes);

            }
            catch (Exception ee)
            {
                string texte = "Error in DialTree ";
                throw new Exception(texte + ee.Message);
            }
        }
            void PopulateTreeViewWithPack(TreeNode node, TreeView tv, EA.Package pack)
        {
            TreeNode treenode = node.Nodes.Add(pack.Name);
            foreach(EA.Package pa in pack.Packages)
            {
                TreeNode panod=node.Nodes.Add(pa.Name);
                PopulateTreeViewWithPack(panod, tv, pa);

            }
            
        }
        // Handle the After_Select event.
        private void TreeView1_AfterSelect(System.Object sender,
            System.Windows.Forms.TreeViewEventArgs e)
        {

            // Vary the response depending on which TreeViewAction
            // triggered the event. 
            switch ((e.Action))
            {
                case TreeViewAction.ByKeyboard:
                    MessageBox.Show("You like the keyboard!");
                    break;
                case TreeViewAction.ByMouse:
                    MessageBox.Show("You like the mouse!");
                    break;
            }
        }
        void populateTree(TreeView treeview)
        {
            if (!dicCimPackagesByName.ContainsKey(this.toppack.Name)) dicCimPackagesByName[this.toppack.Name] = toppack;
            util.getAllPackagesInAPackage(repo, toppack, dicCimPackagesByName);
           TreeNode rootNode = new TreeNode(toppack.Name);
            PopulateTreeViewWithPack(rootNode, this.treeView1, toppack);
        }

        /// <summary>
        /// v
        /// cree une suite de nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        TreeNode getNodeFrom(EA.Package pack)
        {
            TreeNode res=null;
            TreeNode[] nodes = null;

            if (pack.Packages.Count > 0)
            {
                 nodes = new TreeNode[pack.Packages.Count];
            }
            
            if(pack.Packages.Count==0)
            {
              res = new TreeNode(pack.Name);
            }
            else
            {
               int  ind = 0;
             foreach(EA.Package pa in pack.Packages)
                {
                  nodes[ind]= getNodeFrom(pa);
                    ind++;
                }
                res = new TreeNode(pack.Name, nodes);
            }
            return res;

        }
        /// <summary>
        ///  recupere les packages selectionnes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="selectedpackages"></param>
        void getSelectedPackages(TreeNode node,Dictionary<string,EA.Package> selectedPackages)
        {
            if (node.Checked)
            {
                if (!selectedPackages.ContainsKey(node.Text)) selectedPackages[node.Text] = dicCimPackagesByName[node.Text];
            }
                foreach (TreeNode nod in node.Nodes)
                {
                    getSelectedPackages(nod, selectedPackages);
                }
            
        }
        /// <summary>
        /// 
        ///  recupere tous les packages  d'un arbre
        /// </summary>
        /// <param name="node"></param>
        /// <param name="selectedpackages"></param>
        void getAllPackages(TreeNode node, Dictionary<string, EA.Package> selectedPackages)
        {
                if (!selectedPackages.ContainsKey(node.Text)) selectedPackages[node.Text] = dicCimPackagesByName[node.Text];
            
            foreach (TreeNode nod in node.Nodes)
            {
                getAllPackages(nod, selectedPackages);
            }

        }
        private void BCancel_Click(object sender, EventArgs e)
        {
            //  Main.ongoing = false;
            //  this.profilename = "!!##";
            Main.ongoing = false;
            this.Close();
        }

        /// <summary>
        /// recupere tous les packages selectionnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bok_Click(object sender, EventArgs e)
        {
            try
            {
                
                Utilitaires.dicSelectedPackage = new Dictionary<string, EA.Package>();
                if (this.multiselect)
                {
                    if (this.CBAll.Checked)
                    {
                        getAllPackages(treeView1.Nodes[0], Utilitaires.dicSelectedPackage);
                    }
                    else
                    {
                        getSelectedPackages(treeView1.Nodes[0], Utilitaires.dicSelectedPackage);
                    }
                }
                else
                {
                    Utilitaires.dicSelectedPackage = new Dictionary<string, EA.Package>();
                    TreeNode selectednode = treeView1.SelectedNode;
                    Utilitaires.dicSelectedPackage[selectednode.Text] = dicCimPackagesByName[selectednode.Text];
                }
                
                string texte = "";
                foreach (string paname in Utilitaires.dicSelectedPackage.Keys)
                {
                    texte = texte + " | " + paname;
                }
               // util.wlog("DialTree TEST", texte);
                this.Close();
            }
            catch(Exception ee)
                {
                throw new Exception("Error DialTree " + ee.Message);
                }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
    
}
