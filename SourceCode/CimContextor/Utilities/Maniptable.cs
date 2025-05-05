using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using CimContextor.utilitaires;
using CimContextor.Utilities;
/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.9.11                                       *  october 2020  *
*                                                                         *
***************************************************************************
*                                                                         *
*       Credit to:                                                        *
*                   Andre Maizener   andre.maizener@zamiren.fr            *
*                   Jean-Luc Sanson  jean-luc.sanson@noos.fr              *
*                                                                         *
*       Contact: +33148854006                                             *
*                                                                         *
***************************************************************************

**************************************************************************/
namespace CimContextor
{

    /// <summary>
    /// manipulate a table as an excel table
    /// </summary>
    /// 
     
  public  class ManipTable
    {
       // public Dictionary<string, int> ChecpPColums = new Dictionary<string, int>(); // dictionaire nom colonne/index
        Array table;
        Array newtable;
        public TextReader fichierl = null;
        public StreamWriter savfichierw = null;
        ConstantDefinition CD = new ConstantDefinition();
        string adresse = ""; // chemin du fichier    
        TextWriter dumpfic;
        Utilitaires ul = null;
        int errorIndex = 0;  // number of  error line registered

        public TextWriter tableFile
       {
           get { return dumpfic; }
       }
        /// <summary>
        /// le constructeur qui initialise la table à manipuler
        /// </summary>
        /// <param name="param"></param>
      public  ManipTable(Array table,Utilitaires ul)
        {        
         
            try
            {
                this.table = table;
                this.ul = ul;
                string Path = FileManager.GetParentDirPath(); // ABA20221020

                dumpfic = new StreamWriter(Path+ "\\" + CD.CheckPFile, false);
                errorIndex = 0; 
            }
            catch (Exception e)
            {
                ul.wlog("CheckProfile", "ISSUE accessing " + adresse + " , " + e.Message);
                return;
            }
        }

       public  string[] getRaw(int i)
        {
           int dim=table.GetLength(1); // colonne
           String[] result=new String[dim];

           for(int j=0;j < dim;j++)
           {
               result[j] = (string)table.GetValue(i, j);
            }
            return result;
        }
  
       public ArrayList getLRaw(int i)
       {
           int dim = table.GetLength(1); // colonne
           ArrayList result = new ArrayList();
           for (int j = 0; j < dim; j++)
           {
               result.Add( (string)table.GetValue(i, j));
           }
           return result;
       }
        public int setRaw(int i,String[] ls)
        {
            int result = i;
            int dim = table.GetLength(1); // dim colonne
            for (int j = 0; j < dim; j++)
            {
                table.SetValue(ls[j],i, j);
            }
            return result;
        }
        public int setErrorLine(String[] ls)
        {
            ul.wlog("TEST", string.Format("setErrorLine errorIndex={0}", errorIndex));
            int res = errorIndex;
            if (errorIndex >= table.GetLength(0))
            {
                this.resizeRawsTable();
                ul.wlog("CheckProfile", " INFO: the raw table content has been resized to  " + table.GetLength(0));
             }
            
            setRaw(errorIndex, ls);
            ul.wlog("TEST", string.Format("setErrorLine {0}", ls));
            errorIndex++;
            return res;
        }
      /// <summary>
      /// add some raws to the table 
      /// </summary>
      /// <returns></returns>
      public int resizeRawsTable()
        {
            int res = 0;
            int dim = table.GetLength(0); // dim ligne
             newtable = Array.CreateInstance(typeof(string), dim+CD.CPTABLERAWDIM, CD.CheckPColumsNames.Count);
            for (int i = 0; i < dim; i++)
            {
                for(int j=0;j<table.GetLength(1);j++)
                {
                    newtable.SetValue(table.GetValue(i, j), i, j);
                }
                
            }
            table = Array.CreateInstance(typeof(string), dim + CD.CPTABLERAWDIM, CD.CheckPColumsNames.Count);
            table = (Array) newtable.Clone();
                return res;
        }
        public String[] getCol(int j)
        {
            int dim = table.GetLength(0); // dim ligne
            String[] result =new  String[dim];
            for (int i = 0; i < dim; i++)
            {
                result[i] = (string)table.GetValue(i, j);
            }
            return result;
        }
        public ArrayList getLCol(int j)
        {
            int dim = table.GetLength(0); // dim ligne
            ArrayList result = new ArrayList();
            for (int i = 0; i < dim; i++)
            {
                result.Add((string)table.GetValue(i, j));
            }
            return result;
        }
        public int setCol(int j,String ls)
        {
            int result = j;
            int dim = table.GetLength(0); // dim ligne
            for (int i = 0; i < dim; i++)
            {
                table.SetValue(ls[j], i, j);
            }
            return result;
        }
        /// <summary>
        /// sauve varexp en memoire dans le fichier sPathVAR
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public bool ToFile()
        {
            bool result = true;

            try
            {
               
                TextWriter tw = dumpfic;
                int dimligne = table.GetLength(1); //nb de colonnes
                int dimcol = table.GetLength(0); // nb de lignes
               
                for (int noline = 0; noline < dimcol; noline++)
                {
                    StringBuilder sb = new StringBuilder(dimligne);
                    String[] ligne = getRaw(noline);
                    for (int j = 0; j < dimligne; j++)
                    {                    
                        if (j != dimligne - 1)
                        {
                            sb.Append(ligne[j] + CD.CPSEPARATOR);
                        }
                        else
                        {
                            sb.Append(ligne[j]);
                        }
                    }
                    tw.WriteLine(sb.ToString());
                }
                tw.Flush();
                tw.Close();
            }
            catch (Exception e)
            {
                ul.wlog("CheckProfile","ERROR sauvegarde fichier " + CD.CheckPFile + e.Message);
                MessageBox.Show(" ISSUE in trying to write in csv file " + e.Message);
            }
            return result;
        }
        public int GetDim(int dim)
        {
            return table.GetLength(dim);
        }
    public  string GetValue(int i,int j)
        {
            return (string)table.GetValue(i, j);

        }
    public void SetValue(string value, int i, int j)
    {
        table.SetValue(value, i, j);
    }

    }
}
