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
namespace CimContextor.GenerateMessageAssembly
{
    public partial class OrderAssemblyAttributes : Form
    {
        public EA.Element theel=null;
        ConstantDefinition CD = new ConstantDefinition();
        int Rang = 0;
      public   bool ongoing = true; // pour que le dialogue se montre
        EA.Repository repo = null;
        EA.Element elbase = null;
       

         public Dictionary<string,EA.Attribute> elattributes=new  Dictionary<string,EA.Attribute>(); // atname,attribute
         public Dictionary<string, EA.Connector> elcons = new Dictionary<string, EA.Connector>();   //nomname,connector
         List<EA.Element> listelem = new List<EA.Element>();
        List<EA.Attribute> addats=new List<EA.Attribute>(); // attributes which have not yet a position
        List<EA.Connector> addcons=new List<EA.Connector>(); // connectors which have not yet a position

        public OrderAssemblyAttributes(EA.Element el,EA.Repository Repo)
        {
            theel=el;
            Rang = 0;
            repo = Repo;
            elbase = el;
            List<string> nomattributs = new List<string>();
          elattributes=new  Dictionary<string,EA.Attribute>(); // atname,attribute
          elcons = new Dictionary<string, EA.Connector>();   //nomname,connector
            InitializeComponent();
            //MessageBox.Show(" treating class: " + el.Name);

  
            this.dataGridView1.ColumnCount = 3;

            this.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                new Font(this.dataGridView1.Font, FontStyle.Bold);

            this.dataGridView1.Name = "Attributes Order";
            //this.dataGridView1.Location = new Point(8, 8);
           // this.dataGridView1.Size = new Size(500, 250);
            this.dataGridView1.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.dataGridView1.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            this.dataGridView1.GridColor = Color.Black;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.AutoSizeColumnsMode =DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.Columns[0].Name = "element type";
            this.dataGridView1.Columns[1].Name = "element name";
            this.dataGridView1.Columns[2].Name = "origin order";         
            new Font(this.dataGridView1.DefaultCellStyle.Font, FontStyle.Italic);

            this.dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Dock = DockStyle.Fill;

           // this.DataGridView1.CellFormatting += new
            //    DataGridViewCellFormattingEventHandler(
             //   songsDataGridView_CellFormatting);
            try
            {
                List<string> cels = getClassElements(el);
                if (cels != null)
                {
                    PopulateDataGridView(cels);
                }
                else
                {
                    ongoing = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(" Error in Populate DataGridView");
            
            }
        }

     

        List<string> getClassElements(EA.Element el)
        {
            List<string> resul =new List<string>();
            List<string> ls=null;
            int maxrang = 0;
            Dictionary<int, List<string>> RowsByRg = new Dictionary<int, List<string>>(); // the lines will be classified according 
                                                                                           // to there rank
            Rang=-1;
            int rg=0;
            string nom="";
            try
            {
                foreach (EA.Attribute ar in el.Attributes)
                {
                    nom=ar.Name;
                     rg = getAtRang(ar);
                     if (rg == -1)
                     {
                         try
                         {
                             addats.Add(ar);
                         }
                         catch (Exception e)
                         {
                             MessageBox.Show("Issue in getting position of connector (" + el.Name+ "." +ar.Name + ")," + e.Message);
                         }
                     }
                     else
                     {
                         if (maxrang < rg) maxrang = rg;
                         ls = new List<string>();
                         //MessageBox.Show(" rg=" + rg.ToString()+ " at=" + ar.Name); // for test
                         RowsByRg.Add(rg, ls);
                         ls.Add("A");
                         ls.Add(ar.Name);
                         ls.Add(rg.ToString());
                         elattributes.Add(ar.Name, ar);
                     }

                }

                foreach (EA.Connector con in el.Connectors)
                {
                    if (
                        ((con.Type == "Association") || (con.Type == "Aggregation"))
                        && 
                        (((con.ClientID == el.ElementID) && (con.SupplierEnd.Role != "")
                        && ((con.ClientEnd.Aggregation==1) || (con.SupplierEnd.IsNavigable))) // am sept 201                       ||
                        ||
                        ((con.SupplierID == el.ElementID) && (con.ClientEnd.Role != "")
                         && ((con.SupplierEnd.Aggregation == 1) || (con.ClientEnd.IsNavigable))) // am sept 2013
                        )
                        )
                    {
                        EA.ConnectorEnd otherend = null;
                        if (con.ClientID == el.ElementID)
                        {
                            otherend = con.SupplierEnd;
                        }
                        else
                        {
                            otherend = con.ClientEnd;
                        }
                         rg = getAtRang(con);
                         if (rg == -1)
                         {
                             try
                             {
                                 addcons.Add(con);
                             }
                             catch (Exception e)
                             {
                                 MessageBox.Show("Issue in getting position of connector (" + con.SupplierEnd.Role + " --->" + con.ClientEnd.Role + ")," + e.Message);
                             }
                         }
                         else
                         {
                             nom = otherend.Role;
                             if (maxrang < rg) maxrang = rg;
                             ls = new List<string>();
                             // MessageBox.Show(" rg=" + rg.ToString() + " role=" + otherend.Role ); // for test
                             RowsByRg.Add(rg, ls);
                             ls.Add("C");
                             string ss = otherend.Role;
                             ls.Add(ss);
                             ls.Add(rg.ToString());
                             elcons.Add(otherend.Role, con);
                         }
                    }
                }
                //18 Rang++;
                if(maxrang==0)
                {
                    Rang = 0;
                }
                else
                {
                    Rang = maxrang + 1;
                }
                
                foreach (EA.Attribute ar in addats) // give a position to additionnal attributes
                {
                    rg = setAtRang(ar, Rang);
                    Rang++;
                    if (maxrang < rg) maxrang = rg;
                         ls = new List<string>();
                         //MessageBox.Show(" rg=" + rg.ToString()+ " at=" + ar.Name); // for test
                         RowsByRg.Add(rg, ls);
                         ls.Add("A");
                         ls.Add(ar.Name);
                         ls.Add(rg.ToString());
                         elattributes.Add(ar.Name, ar);
                }
                foreach (EA.Connector con in addcons) // give a position to additionnal connectors
                {
                    EA.ConnectorEnd otherend = null;
                    if (con.ClientID == el.ElementID)
                    {
                        otherend = con.SupplierEnd;
                    }
                    else
                    {
                        otherend = con.ClientEnd;
                    }
                    rg = setAtRang(con, Rang);
                    Rang++;
                    nom = otherend.Role;
                    if (maxrang < rg) maxrang = rg;
                    ls = new List<string>();
                    // MessageBox.Show(" rg=" + rg.ToString() + " role=" + otherend.Role ); // for test
                    RowsByRg.Add(rg, ls);
                    ls.Add("C");
                    string ss = otherend.Role;
                    ls.Add(ss);
                    ls.Add(rg.ToString());
                    elcons.Add(otherend.Role, con);

                }


                //18 for (int i = 0; i < maxrang + 1; i++)
                for (int i = 0; i < maxrang + 1; i++)
                {
                    resul.AddRange(RowsByRg[i]);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("warning  in getClassElements rg=" + rg.ToString() + ", at=" + nom + e.Message + " , un reset a ete effectue");
                setClassElements(el);
                resul = null;
                
            }
            return resul;
        }
//-----------------------------------------------
        void removeOrMajTagRg(EA.Attribute at,int rang)
        {
            int resul = -1;
            foreach (EA.AttributeTag tag in at.TaggedValues)
            {
                if (tag.Name == CD.GetRangTagValue())
                {
                    resul = 1;
                    tag.Value = rang.ToString();
                    tag.Update();              
                    break;
                }
            }
            if (resul == -1)
            {
                EA.AttributeTag tag = (EA.AttributeTag)at.TaggedValues.AddNew(CD.GetRangTagValue(), rang.ToString());
                tag.Update();
                at.TaggedValues.Refresh();
                at.Update();
 
            }
            
        }
        void removeOrMajTagRg(EA.Connector con,int rang)
        {
            int resul = -1;
            foreach (EA.ConnectorTag tag in con.TaggedValues)
            {
                if (tag.Name == CD.GetRangTagValue())
                {
                    resul = 1;
                    tag.Value = rang.ToString();
                    tag.Update();
                    break;
                }
            }
            if (resul == -1)
            {
                EA.ConnectorTag tag = (EA.ConnectorTag)con.TaggedValues.AddNew(CD.GetRangTagValue(), rang.ToString());
                tag.Update();
                con.TaggedValues.Refresh();
                con.Update();

            }
        }
        void setClassElements(EA.Element el)
        {
            
         
            Rang = 0;
            int rg = 0;
          
            try
            {
                foreach (EA.Attribute ar in el.Attributes)
                {
                    removeOrMajTagRg(ar, rg);
                    rg=getAtRang(ar);
                    setAtRang(ar,rg );
                    rg++;
                }

                foreach (EA.Connector con in el.Connectors)
                {
                    if (
                       // ((con.Type == "Association") || (con.Type == "Aggregation"))
                      //  && (
                       // ((con.ClientID == el.ElementID) && (con.SupplierEnd.Role != ""))
                       // ||
                       // ((con.SupplierID == el.ElementID) && (con.ClientEnd.Role != ""))
                       // )
                        ((con.Type == "Association") || (con.Type == "Aggregation"))
                        && 
                        (((con.ClientID == el.ElementID) && (con.SupplierEnd.Role != "")
                        && ((con.ClientEnd.Aggregation==1) || (con.SupplierEnd.IsNavigable))) // am sept 201                       ||
                        ||
                        ((con.SupplierID == el.ElementID) && (con.ClientEnd.Role != "")
                         && ((con.SupplierEnd.Aggregation == 1) || (con.ClientEnd.IsNavigable))) // am sept 2013
                        )
                        )
                    {
                        removeOrMajTagRg(con, rg);
                        rg=getAtRang(con);
                        setAtRang(con,rg );
                        rg++;
                    }
                }               
            }
            catch (Exception e)
            {
                MessageBox.Show("Error  in setClassElements " +  e.Message);
            }
    
        }

//---------------------------------------------
        int getAtRang(EA.Attribute at)
        {
            int resul=-1;
            foreach (EA.AttributeTag tag in at.TaggedValues)
            {
                if (tag.Name == CD.GetRangTagValue())
                {
                    
                    resul = System.Convert.ToInt32(tag.Value);
                    Rang = resul; // met a jour le rang courant
                    break;
                }
            }
            if (resul == -1)
            {
                EA.AttributeTag tag = (EA.AttributeTag) at.TaggedValues.AddNew(CD.GetRangTagValue(), Rang.ToString());
                tag.Update();
                at.TaggedValues.Refresh();
                at.Update();
               
               // resul = Rang;
               // Rang++;
            }
            return resul;
        }

        int setAtRang(EA.Attribute at,int rang)
        {
            int resul = -1;
            foreach (EA.AttributeTag tag in at.TaggedValues)
            {
                if (tag.Name == CD.GetRangTagValue())
                {
                    resul = rang;
                    tag.Value = rang.ToString();
                    tag.Update();
                   
                    break;
                }
            }
            if (resul == -1)
            {
                MessageBox.Show("Error the attribute has no tag RangTagValue");
            }
            return resul;
        }


        int getAtRang(EA.Connector con)
        {
            int resul = -1;
            foreach (EA.ConnectorTag tag in con.TaggedValues)
            {
                if (tag.Name == CD.GetRangTagValue())
                {
                    resul = System.Convert.ToInt32(tag.Value);
                    Rang = resul;
                    break;
                }
            }
            if (resul == -1)
            {
                EA.ConnectorTag tag = (EA.ConnectorTag)con.TaggedValues.AddNew(CD.GetRangTagValue(), Rang.ToString());
                tag.Update();
                con.TaggedValues.Refresh();
                con.Update();
               
               // resul = Rang;
               // Rang++;
            }
            return resul;
        }

        int setAtRang(EA.Connector con,int rang)
        {
            int resul = -1;
            string prov = "";
            foreach (EA.ConnectorTag tag in con.TaggedValues)
            {
                /****** pour test ********************/
                if (tag.Name == CD.GetIBOTagValue())
                {
                    prov = tag.Value;
                }
                /*************************************/
                if (tag.Name == CD.GetRangTagValue())
                {
                    resul = rang;
                    tag.Value = rang.ToString();
                    tag.Update();
                    break;
                }
            }
            if (resul == -1)
            {
                MessageBox.Show("Error the connector has no tag RangTagValue");
            }
            return resul;
        }


        /// <summary>
        /// populate grid
        /// </summary>
        /// <param name="dicattributes"></param>  // name,type A or R (attribute or role)
        public void PopulateDataGridView( List<string> attributes)
        {
            if ((attributes.Count % 3) != 0)
            {
                MessageBox.Show("Error  list of intitialisation is of dimension != 3");
                return;
            }
            for(int i=0;i<attributes.Count;i=i+3)
            {

         string[] row = { attributes[i], attributes[i + 1], attributes[i + 2] };
         try
         {
             this.dataGridView1.Rows.Add(row);
         }
         catch (Exception)
         {
             MessageBox.Show("Error in adding a row in dataGridView");
         }
          }

            this.dataGridView1.Columns[0].DisplayIndex = 1;
            this.dataGridView1.Columns[1].DisplayIndex = 2;
            this.dataGridView1.Columns[2].DisplayIndex = 0;
            //this.DataGridView1.Columns[3].DisplayIndex = 1;
           // this.DataGridView1.Columns[4].DisplayIndex = 2;
        }

  

        private void down_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int index = row.Index;
            if (index + 1 >= dataGridView1.Rows.Count - 1) return; // reached the limits
            

                DataGridViewRow rowold = dataGridView1.Rows[index + 1];
                DataGridViewRow rownew = CloneWithValues(rowold);
                string prov = "rowold " + rowold.Cells[0].Value.ToString() + "," + rowold.Cells[1].Value.ToString() + "," + rowold.Cells[2].Value.ToString();
                //MessageBox.Show(prov); // pour test
                string rowoldtype = rowold.Cells[0].Value.ToString();
                string rowtype = row.Cells[0].Value.ToString();
                string rowoldname = rowold.Cells[1].Value.ToString();
                string rowname = row.Cells[1].Value.ToString();
                int rowrg = 0;
                int rowoldrg = 0;
                if (rowtype == "A")
                {
                    rowrg = getAtRang(elattributes[rowname]);       
                }
                if (rowtype == "C")
                {
                    rowrg = getAtRang(elcons[rowname]);
                }
                if (rowoldtype == "A")
                {
                    rowoldrg = getAtRang(elattributes[rowoldname]);
                }
                if (rowoldtype == "C")
                {
                    rowoldrg = getAtRang(elcons[rowoldname]);
                }

                if (rowtype == "A")
                {
                    setAtRang(elattributes[rowname], rowoldrg);
                    
                }

                if (rowtype == "C")
                {
                    setAtRang(elcons[rowname], rowoldrg);

                }

                if (rowoldtype == "A")
                {
                    setAtRang(elattributes[rowoldname], rowrg);

                }

                if (rowoldtype == "C")
                {
                    setAtRang(elcons[rowoldname], rowrg);

                }
                dataGridView1.Rows.Insert(index, rownew);
                dataGridView1.Rows.Remove(rowold);
                this.dataGridView1.Update();

                // mise à jour des rangs

              
            
            
        }
        public DataGridViewRow CloneWithValues(DataGridViewRow row)
        {
            DataGridViewRow clonedRow = (DataGridViewRow)row.Clone();
            for (Int32 index = 0; index < row.Cells.Count; index++)
            {
                clonedRow.Cells[index].Value = row.Cells[index].Value;
            }
            return clonedRow;
        }



        private void ok_Click(object sender, EventArgs e)
        {
            

            List<EA.Attribute> attributeslist = new List<EA.Attribute>();
     
            foreach (EA.Attribute at in theel.Attributes)
            {
                attributeslist.Add(at); 
            }

            attributeslist.Sort(CompareAttributebyRg);
            int ind = 0;
            foreach(EA.Attribute at in attributeslist)
            {
            at.Pos = ind;
                ind++;
                at.Update();
            }
            //this.Close();
            this.Dispose();
        }

      

      

        private void up_Click(object sender, EventArgs e)
        {
          
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int index = row.Index;
            if (index - 1 < 0) return; // blocked on the upperline             
                DataGridViewRow rowold = dataGridView1.Rows[index];
                DataGridViewRow rownew = CloneWithValues(rowold);
                dataGridView1.Rows.Remove(rowold);
                dataGridView1.Rows.Insert(index-1, rownew);
                rownew.Selected = true;

              //  dataGridView1.Rows.Remove(rowold);
                this.dataGridView1.Update();
                row = dataGridView1.Rows[index];
                string rowoldtype = rowold.Cells[0].Value.ToString();
                string rowtype = row.Cells[0].Value.ToString();
                string rowoldname = rowold.Cells[1].Value.ToString();
                string rowname = row.Cells[1].Value.ToString();
                int rowrg = 0;
                int rowoldrg = 0;
                if (rowtype == "A")
                {
                    rowrg = getAtRang(elattributes[rowname]);
                }
                if (rowtype == "C")
                {
                    rowrg = getAtRang(elcons[rowname]);
                }
                if (rowoldtype == "A")
                {
                    rowoldrg = getAtRang(elattributes[rowoldname]);
                }
                if (rowoldtype == "C")
                {
                    rowoldrg = getAtRang(elcons[rowoldname]);
                }

                if (rowtype == "A")
                {
                    setAtRang(elattributes[rowname], rowoldrg);

                }

                if (rowtype == "C")
                {
                    setAtRang(elcons[rowname], rowoldrg);

                }

                if (rowoldtype == "A")
                {
                    setAtRang(elattributes[rowoldname], rowrg);

                }

                if (rowoldtype == "C")
                {
                    setAtRang(elcons[rowoldname], rowrg);

                }
            
        }


        public  int CompareAttributebyRg(EA.Attribute x, EA.Attribute y)
        {

            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {



                    // sort them with ordinary int comparison.
                    //
                    int xrg = getAtRang(x);
                    int  yrg = getAtRang(y);

                    int prov = xrg.CompareTo(yrg);
                    // MessageBox.Show("comppare mobjs by keyname "+ " xname=" + xname + " yname=" + yname + prov.ToString());
                    if (xrg < yrg)
                    {
                        return -1; // yrg is greater
                    }
                    else
                    {
                        if (xrg == yrg)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1; //xrg is greater
                        }
                    }
                }
            }
        }
/// <summary>
/// set initial value of ESMPRG tagvalue 
/// where necessary
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            listelem=new List<EA.Element>();
            EA.Package pack = repo.GetPackageByID((int)elbase.PackageID);
             ConstantDefinition CD = new ConstantDefinition();
             try
             {
                 foreach (EA.Element elt in pack.Elements)
                 {

                     if (
                         (!elt.StereotypeEx.Contains(CD.GetCompoundStereotype()))
                         ||
                        (!elt.StereotypeEx.Contains(CD.GetDatatypeStereotype()))
                        ||
                        (!elt.StereotypeEx.Contains(CD.GetEnumStereotype()))
                        ||
                        (!elt.StereotypeEx.Contains(CD.GetPrimitiveStereotype()))

                         )
                     {
                         if (!listelem.Contains(elt)) listelem.Add(elt);
                     }
                 }
                 foreach (EA.Element elm in listelem)
                 {
                     string prov = elm.Name;
                     setClassElements(elm);
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show("Error reset  " +  ex.Message);
             }
        }
       
    
//-----------------------------------------------------------------------------------------
    }
}
