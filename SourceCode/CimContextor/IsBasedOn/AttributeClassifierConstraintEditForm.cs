using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
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
    public partial class AttributeClassifierConstraintEditForm : Form
    {
        private ConstantDefinition CD = new ConstantDefinition();
        private string Mode;
        private AttributeClassifierConstraintForm ACCF;
        private EAClassAttribute SelectedAttribute;
        private EAClassAttributeConstraint SelectedEAConstraint;
        public XMLClassifierConstraint SelectedXMLConstraint;
        private EA.Repository Repository;
        private string EditedType;
        //private int NumberOfVariable;
        private ArrayList SplitedNote = new ArrayList();
        private ArrayList SplitedVariable = new ArrayList();
        
        public struct SplitedVariableStruct
        {
            public string variable;
            public string value;
        }

        public AttributeClassifierConstraintEditForm(AttributeClassifierConstraintForm ACCF, EA.Repository Repository, string Mode, string EditedType, EAClassAttribute SelectedAttribute, EAClassAttributeConstraint SelectedEAConstraint, XMLClassifierConstraint SelectedXMLConstraint)
        {
            InitializeComponent();
            this.Mode = Mode;
            this.ACCF = ACCF;
            this.Repository=Repository;
            this.EditedType = EditedType;
            this.SelectedAttribute = SelectedAttribute;
            this.SelectedEAConstraint = SelectedEAConstraint;
            this.SelectedXMLConstraint = SelectedXMLConstraint;

           

            #region UIInit

            TBComment.Text = SelectedXMLConstraint.GetComment().Replace("\\n","\n").Replace("\\r","\r");

            if (SelectedXMLConstraint.GetVariableType().Equals("DEFINED"))
            {
                TBNotes.Visible = false;
                CB.Visible = true;
                LabDefinedValue.Visible = true;
                LabDefinedValue.Text="Possible value list :";

                string TmpString = SelectedXMLConstraint.GetVariableList();
                bool DoneParsing = false;
                while(DoneParsing.Equals(false))
                {
                    if (!TmpString.IndexOf(",").Equals(-1))
                    {
                        CB.Items.Add(TmpString.Substring(0,TmpString.IndexOf(",")));
                        TmpString = TmpString.Substring(TmpString.IndexOf(",")+1, TmpString.Length-(TmpString.IndexOf(",")+1));
                    }
                    else{
                        DoneParsing=true;
                    }
                }
                CB.Items.Add(TmpString);
            }
            else{
                TBNotes.Visible = true;
                CB.Visible = false;
                LabDefinedValue.Visible = true;
                LabDefinedValue.Text = "Select a value for each variable :";
            }

            if(!SelectedXMLConstraint.GetVariableType().Equals("DEFINED")){
                    string Note = SelectedXMLConstraint.GetNote().Replace("\r","").Replace("\n","").Trim();                 
                    while(Note.Contains("$")){
                       
                            int First = Note.IndexOf("$");
                            SplitedNote.Add(Note.Substring(0,First));
                            int Lenght = Note.Substring(First + 1, (Note.Length - (First+1))).IndexOf("$");
                            SplitedNote.Add(Note.Substring(First + 1, Lenght));
                            SplitedVariableStruct AStruct = new SplitedVariableStruct();
                            AStruct.variable = (Note.Substring(First + 1, Lenght));    
                            SplitedVariable.Add(AStruct);
                            Note = Note.Substring(First + 2 + Lenght, (Note.Length - (First + 2+Lenght)));
                    }
                    SplitedNote.Add(Note);
                    bool FirstTime = true;
                    //int i = 0;
                    foreach (SplitedVariableStruct AStruct in SplitedVariable)
                    {
                        CB.Items.Add(AStruct.variable);
                        if(FirstTime.Equals(true)){
                            CB.SelectedItem = AStruct.variable;
                            TBNotes.Text = AStruct.value;
                            FirstTime = false;
                        }
                        //i++;
                    }                
               
            
                    //Update case
                    if (Mode.Equals(CD.GetUpdate()))
                    {
                        string TmpNote = SelectedEAConstraint.GetNotes();
                        for (int i = 0; SplitedVariable.Count > i; i++)
                        {
                            SplitedVariableStruct UpdatedVarStruct = new SplitedVariableStruct();
                            //string VarValue = "";
                            for (int j = 0; SplitedNote.Count > j; j++)
                            {
                                if (((string)SplitedNote[j]).Equals( ((SplitedVariableStruct)SplitedVariable[i]).variable ))
                                {
                                    if (!j.Equals(0))
                                    {
                                        int Size1 = ((string)SplitedNote[j - 1]).Length;
                                        TmpNote = TmpNote.Substring(Size1, TmpNote.Length - Size1);
                                        int EndSize1 =0;
                                        if (SplitedNote.Count>= j+1)
                                        {
                                            for (int k = 0; TmpNote.Length > k; k++ )
                                            {
                                                if (TmpNote.Substring(k, TmpNote.Length - k).Equals(((string)SplitedNote[j+1])))
                                                {
                                                    EndSize1 = k;
                                                }
                                            }
                                        }
                                        UpdatedVarStruct.value = TmpNote.Substring(0, EndSize1);
                                    }
                                }
                            }
                            UpdatedVarStruct.variable = ((SplitedVariableStruct)SplitedVariable[i]).variable;
                            SplitedVariable[i] = UpdatedVarStruct;
                        }
                        //end update case
                    }
                    }
                    else if(SelectedXMLConstraint.GetVariableType().Equals("DEFINED"))
                    {


                        string Note = SelectedXMLConstraint.GetNote().Replace("\r","").Replace("\n","").Trim();                 
                        while(Note.Contains("$")){
                            int First = Note.IndexOf("$");
                            SplitedNote.Add(Note.Substring(0,First));
                            int Lenght = Note.Substring(First + 1, (Note.Length - (First+1))).IndexOf("$");
                            SplitedNote.Add(Note.Substring(First + 1, Lenght));
                            SplitedVariableStruct AStruct = new SplitedVariableStruct();
                            AStruct.variable = (Note.Substring(First + 1, Lenght));    
                            SplitedVariable.Add(AStruct);
                            Note = Note.Substring(First + 2 + Lenght, (Note.Length - (First + 2+Lenght)));
                    }
                    SplitedNote.Add(Note);

                                
               
            
                    //Update case
                    if (Mode.Equals(CD.GetUpdate()))
                    {
                        string TmpNote = SelectedEAConstraint.GetNotes();
                        for (int i = 0; SplitedVariable.Count > i; i++)
                        {
                            SplitedVariableStruct UpdatedVarStruct = new SplitedVariableStruct();
                            //string VarValue = "";
                            for (int j = 0; SplitedNote.Count > j; j++)
                            {
                                if (((string)SplitedNote[j]).Equals(((SplitedVariableStruct)SplitedVariable[i]).variable))
                                {
                                    if (!j.Equals(0))
                                    {
                                        int Size1 = ((string)SplitedNote[j - 1]).Length;
                                        TmpNote = TmpNote.Substring(Size1, TmpNote.Length - Size1);
                                        int EndSize1 = 0;
                                        if (SplitedNote.Count >= j + 1)
                                        {
                                            for (int k = 0; TmpNote.Length > k; k++)
                                            {
                                                if (TmpNote.Substring(k, TmpNote.Length - k).Equals(((string)SplitedNote[j + 1])))
                                                {
                                                    EndSize1 = k;
                                                }
                                            }
                                        }
                                        UpdatedVarStruct.value = TmpNote;
                                    }
                                }
                            }
                            //UpdatedVarStruct.variable = ((SplitedVariableStruct)SplitedVariable[i]).variable;
                            //SplitedVariable[i] = UpdatedVarStruct;
                            CB.SelectedItem = UpdatedVarStruct.value ;
                        }



                    }

                    


            }
            
            #endregion


            if (Mode.Equals(CD.GetCreate()))
            {
                this.Text = "Adding " + SelectedXMLConstraint.GetName() + " to " + SelectedAttribute.GetName(); ;
                LabConstraintName.Text = SelectedXMLConstraint.GetName() + " note :";

            }
            else
            {
                
                this.Text = "Editing " + SelectedEAConstraint.GetName() + " from " + SelectedAttribute.GetName(); ;
                LabConstraintName.Text = SelectedEAConstraint.GetName()+" note :";
                if (!SelectedXMLConstraint.GetVariableType().Equals("DEFINED"))
                {
                    for (int i = 0; SplitedVariable.Count > i;i++)
                    {
                        if (((SplitedVariableStruct)SplitedVariable[i]).variable.Equals(CB.SelectedItem.ToString()))
                        {
                            TBNotes.Text = ((SplitedVariableStruct)SplitedVariable[i]).value;
                        }
                    }
                }
            }
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        private bool CheckData()
        {
            if (SelectedXMLConstraint.GetVariableType().Equals("ANY") || SelectedXMLConstraint.GetVariableType().Equals("Numeric"))
            {
                for (int i = 0; SplitedVariable.Count > i; i++)
                {
                    SplitedVariableStruct AStruct = ((SplitedVariableStruct)SplitedVariable[i]);
                    if (AStruct.value == ("") || AStruct.value==null)
                    {
                        MessageBox.Show("The variable must have a value.", "Wrong variable type!", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);              
                        return false;
                    }
                }
            }
            else if (SelectedXMLConstraint.GetVariableType().Equals("DEFINED"))
            {
                if (!CB.Items.Contains(CB.Text))
                {
                    MessageBox.Show("The variable must have a value.", "Wrong variable type!", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);              
                    return false;
                }
            }
            else if (SelectedXMLConstraint.GetVariableType().Equals("Decimal") || SelectedXMLConstraint.GetVariableType().Equals("Integer"))
            {
                try
                {
                    for (int i = 0; SplitedVariable.Count > i; i++)
                    {
                        SplitedVariableStruct AStruct = ((SplitedVariableStruct)SplitedVariable[i]);
                        if (AStruct.value == ("") || AStruct.value == null)
                        {
                            MessageBox.Show("The variable must have a value.", "Wrong variable type!", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);              
                            return false;
                        }
                        int.Parse(AStruct.value);
                    }
                }
                catch
                {
                    MessageBox.Show("The variable doesn't seem to be of the correct type.", "Wrong variable type!", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }

            return true;
        }

        private void ButSave_Click(object sender, EventArgs e)
        {
            
            

            if (!SelectedXMLConstraint.GetVariableType().Equals("DEFINED"))
            {
                //Adding the value to the var
                for (int i = 0; SplitedVariable.Count > i; i++)
                {
                    if (((SplitedVariableStruct)SplitedVariable[i]).variable.Equals(CB.SelectedItem.ToString()))
                    {
                        SplitedVariableStruct AStruct = ((SplitedVariableStruct)SplitedVariable[i]);
                        AStruct.value = TBNotes.Text;
                        SplitedVariable[i] = AStruct;
                        break;
                    }
                }
                //end adding the value to the var
            }
            else
            {
                //Adding the value to tue var
                if (SplitedVariable.Count > 0)
                {
                    SplitedVariableStruct AStruct = ((SplitedVariableStruct)SplitedVariable[0]);
                    AStruct.value = CB.Text;
                    SplitedVariable[0] = AStruct;
                }
                //End Adding the value to tue var
            }


           if(CheckData().Equals(false)){
               return;
           }

                
            ////

           string AFinalizedNote = "";
           foreach (string AString in SplitedNote)
           {
               bool IsVar = false;
               string VarValue = "";
               for (int i = 0; SplitedVariable.Count > i; i++)
               {
                   SplitedVariableStruct AStruct = ((SplitedVariableStruct)SplitedVariable[i]);
                   if (AStruct.variable.Equals(AString))
                   {
                       IsVar = true;
                       VarValue = AStruct.value;
                   }
               }

               if (IsVar.Equals(true))
               {
                   AFinalizedNote = AFinalizedNote + VarValue;
               }
               else
               {
                   AFinalizedNote = AFinalizedNote + AString;
               }
               TBNotes.Text = AFinalizedNote;
           }

            ////
                    if (Mode.Equals(CD.GetCreate()))
                    {
                        EAClassAttributeConstraint ANewConstraint = SelectedAttribute.AddClassifierConstraint(SelectedXMLConstraint.GetName(), SelectedXMLConstraint.GetType(), TBNotes.Text);
                        ACCF.UpdateConstraintToUI(ANewConstraint, CD.GetCreate());
                    }
                    else
                    {
                        SelectedEAConstraint.SetNotes(TBNotes.Text);
                        ACCF.UpdateConstraintToUI(SelectedEAConstraint, CD.GetUpdate());
                    }
               
                this.Dispose();
            
        }

        /*
        private void ButSaveVar_Click(object sender, EventArgs e)
        {
            if(CB.Items.Contains(CB.Text)){
                if(SelectedXMLConstraint.GetVariableType().Equals("Numeric")){

                    try{
                        int.Parse(TBNotes.Text);
                    }
                    catch{
                        try{
                            float.Parse(TBNotes.Text);
                        }
                        catch{
                            return;
                        }
                    }

                    for (int i = 0; SplitedVariable.Count > i; i++)
                    {
                        if (((SplitedVariableStruct)SplitedVariable[i]).variable.Equals(CB.SelectedItem.ToString()))
                        {
                            SplitedVariableStruct AStruct = ((SplitedVariableStruct)SplitedVariable[i]);
                            AStruct.value = TBNotes.Text;
                            SplitedVariable[i] = AStruct;
                            break;
                        }
                    }


                }
                else{
                    for (int i = 0; SplitedVariable.Count < i;i++ )
                    {
                        if (((SplitedVariableStruct)SplitedVariable[i]).variable.Equals(CB.SelectedItem.ToString()))
                        {
                            SplitedVariableStruct AStruct = ((SplitedVariableStruct)SplitedVariable[i]);
                            AStruct.value = TBNotes.Text;
                            break;
                        }
                    }
                }
            }
        }

        private void CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!SelectedXMLConstraint.GetVariableType().Equals("DEFINED")){
            if (CB.Items.Contains(CB.Text))
            {
                foreach (SplitedVariableStruct AStruct in SplitedVariable)
                {
                    if (AStruct.variable.Equals(CB.SelectedItem.ToString()))
                    {
                        TBNotes.Text = AStruct.value;
                        break;
                    }
                }
            }
            }
        }*/

    }
}
