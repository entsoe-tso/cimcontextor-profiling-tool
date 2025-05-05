using CimContextor.Configuration;
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
    public partial class AttributeCardinalityForm : Form
    {
        private IsBasedOnForm ibof;
        private EAClass populatedEAClass;
        private EAClassAttribute activeAttribute;
        private ConstantDefinition CD = new ConstantDefinition();
        private EA.Repository repo;

        public AttributeCardinalityForm(IsBasedOnForm ibof, EAClass populatedEAClass, EAClassAttribute activeAttribute, EA.Repository repo)
        {
            InitializeComponent();
            this.repo = repo;
            this.ibof = ibof;
            this.populatedEAClass = populatedEAClass;
            this.activeAttribute = activeAttribute;
            LabAttributeName.Text = "Attribute name : " + activeAttribute.GetName();
            LabUpperBound.Visible = false;


                CBLowerBound.Items.Add("0");
                CBUpperBound.Items.Add("1");
                CBLowerBound.Items.Add("1");
                CBUpperBound.Visible = false;
                //18 CBUpperBound.Items.Add("*");
                //18 CBLowerBound.Items.Add("*");
            
            if(! populatedEAClass.GetMode().Equals(CD.GetPrimitiveStereotype())){


            if (!CBLowerBound.Items.Contains(activeAttribute.GetLowerBound()))
            {

                try
                {
                    int.Parse(activeAttribute.GetLowerBound());
                    CBLowerBound.Items.Add(activeAttribute.GetLowerBound());
                    CBLowerBound.SelectedItem = activeAttribute.GetLowerBound();
                }
                catch {
                    MessageBox.Show("Value entered in the Lower Bound's parent class doesn't seem valid(0,1...n,*). Use the cancel button and correct it.", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }

            }
            else
            {
                CBLowerBound.SelectedItem = activeAttribute.GetLowerBound();
            }

            if (!CBUpperBound.Items.Contains(activeAttribute.GetUpperBound()))
            {
                try
                {
                    int.Parse(activeAttribute.GetUpperBound());
                    CBUpperBound.Items.Add(activeAttribute.GetUpperBound());
                    CBUpperBound.SelectedItem = activeAttribute.GetUpperBound();
                }
                catch {
                    MessageBox.Show("Value entered in the Upper Bound's parent class doesn't seem valid(0,1...n,*). Use the cancel button and correct it.", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);                   
                }
            }
            else
            {
                CBUpperBound.SelectedItem = activeAttribute.GetUpperBound();
            }

            }

        }

        
        // Deprecated and buged now using the same algo than the dupplicateconnectors cardinalties check
        /*
        private bool CheckCardinalityCreate()
        {                       
            //Checking parent cardinality
            if (!activeAttribute.GetUpperBound().Equals("*"))
            {
                try {
                    int.Parse(activeAttribute.GetUpperBound());
                }
                catch
                {
                    MessageBox.Show("Value entered in the parent class doesn't seem valid(0,1...n,*). Use the cancel button." , "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            if (!activeAttribute.GetLowerBound().Equals("*"))
            {
                try
                {
                    int.Parse(activeAttribute.GetLowerBound());
                }
                catch
                {
                    MessageBox.Show("Value entered in the parent class doesn't seem valid(0,1...n,*). Use the cancel button.", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            if ((!activeAttribute.GetLowerBound().Equals("*")) && (!activeAttribute.GetUpperBound().Equals("*")))
            {
                if (int.Parse(activeAttribute.GetLowerBound()) > int.Parse(activeAttribute.GetUpperBound()))
                {
                    MessageBox.Show("Value entered in the parent class doesn't seem valid(LowerBound > to UpperBound). Use the cancel button.", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }

            if (CBLowerBound.Text.Equals("*") || CBUpperBound.Text.Equals("*") || activeAttribute.GetLowerBound().Equals("*") || activeAttribute.GetUpperBound().Equals("*"))
            {

                if (CBLowerBound.Text.Equals("*"))
                {
                    MessageBox.Show("Value entered doesn't seem valid(* as lowerBound is forbidden).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (activeAttribute.GetLowerBound().Equals("*"))
                {
                    MessageBox.Show("Old cardinality doesn't seem valid.(* as lowerBound is forbidden. Use Cancel button and change it from within EA.)", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }

                if (CBUpperBound.Text.Equals("*"))
                {
                    if (activeAttribute.GetUpperBound().Equals("*"))
                    {

                        if (int.Parse(activeAttribute.GetLowerBound()) <= int.Parse(CBLowerBound.Text))
                        {
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            return false;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return false;
                    }
                }
                else
                {
                    if (activeAttribute.GetUpperBound().Equals("*"))
                    {
                        if (int.Parse(CBLowerBound.Text) <= int.Parse(activeAttribute.GetLowerBound()))
                        {
                            if (int.Parse(CBLowerBound.Text) < int.Parse(CBUpperBound.Text))
                            {
                                return true;
                            }
                            else
                            {
                                MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            return false;
                        }
                    }


                }


            }
            else
            {
                if (int.Parse(CBLowerBound.Text) <= int.Parse(CBUpperBound.Text))
                {
                    if ((int.Parse(activeAttribute.GetLowerBound()) <= int.Parse(CBLowerBound.Text)) && (int.Parse(activeAttribute.GetUpperBound()) >= int.Parse(CBUpperBound.Text)))
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
                else {
                    MessageBox.Show("Value entered doesn't seem valid(LowerBound>UpperBound).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            return false;
        }

        private bool CheckCardinalityUpdate()
        {
            //Checking parent cardinality
            if (!activeAttribute.GetParentUB().Equals("*"))
            {
                try
                {
                    //int.Parse(activeAttribute.GetUpperBound());
                    int.Parse(activeAttribute.GetParentUB());
                
                }
                catch
                {
                    MessageBox.Show("Value entered in the parent class doesn't seem valid(0,1...n,*). Use the cancel button.", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
           
            if (!activeAttribute.GetParentLB().Equals("*"))
            {
                try
                {
                    int.Parse(activeAttribute.GetParentLB());
                }
                catch
                {
                    MessageBox.Show("Value entered in the parent class doesn't seem valid(0,1...n,*). Use the cancel button.", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }

            else if ((!activeAttribute.GetParentLB().Equals("*")) && (!activeAttribute.GetParentUB().Equals("*")))
            {
                if (int.Parse(activeAttribute.GetParentLB()) > int.Parse(activeAttribute.GetParentUB()))
                {
                    MessageBox.Show("Value entered in the parent class doesn't seem valid(LowerBound > to UpperBound). Use the cancel button.", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }

            else if (CBLowerBound.Text.Equals("*") || CBUpperBound.Text.Equals("*") || activeAttribute.GetParentLB().Equals("*") || activeAttribute.GetParentUB().Equals("*"))
            {

                if (CBLowerBound.Text.Equals("*"))
                {
                    MessageBox.Show("Value entered doesn't seem valid(* as lowerBound is forbidden).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (activeAttribute.GetParentLB().Equals("*"))
                {
                    MessageBox.Show("Old cardinality doesn't seem valid.(* as lowerBound is forbidden. Use Cancel button and change it from within EA.)", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }

                if (CBUpperBound.Text.Equals("*"))
                {
                    if (activeAttribute.GetParentUB().Equals("*"))
                    {
                        if (int.Parse(activeAttribute.GetParentLB()) <= int.Parse(CBLowerBound.Text))
                        {
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            return false;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return false;
                    }
                }
                else
                {
                    if (activeAttribute.GetParentUB().Equals("*"))
                    {
                        if (int.Parse(CBLowerBound.Text) <= int.Parse(activeAttribute.GetParentLB()))
                        {
                            if (int.Parse(CBLowerBound.Text) < int.Parse(CBUpperBound.Text))
                            {
                                return true;
                            }
                            else
                            {
                                MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            return false;
                        }
                    }


                }


            }
            else
            {
                if (int.Parse(CBLowerBound.Text) <= int.Parse(CBUpperBound.Text))
                {
                    if ((int.Parse(activeAttribute.GetParentLB()) <= int.Parse(CBLowerBound.Text)) && (int.Parse(activeAttribute.GetParentUB()) >= int.Parse(CBUpperBound.Text)))
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Value entered doesn't seem valid(only restricting cardinality is allowed).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
                else
                {
                    MessageBox.Show("Value entered doesn't seem valid(LowerBound>UpperBound).", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            return false;
        }
        private bool CheckCardinality()
        {
            if (populatedEAClass.GetCreateMode().Equals(true))
            {
                return CheckCardinalityCreate();
            }
            else
            {
                return CheckCardinalityUpdate();
            }
        }
        */


        //Both parent equal ""
        private bool CheckSimpleCardinality()
        {
            if (CBLowerBound.Text.Equals("*"))
            {
                MessageBox.Show("A lowerbound cardinality can't be equal to *", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (CBLowerBound.Text.Equals("") && CBUpperBound.Text.Equals(""))
            {
                return true;
            }


            if (CBUpperBound.Text.Equals("*"))
            {
                try
                {
                    int.Parse(CBLowerBound.Text);
                }
                catch
                {
                    MessageBox.Show("The lowerbound cardinality doesn't seem correct.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                if (CBUpperBound.Text.Equals(""))
                {

                }
                else
                {
                    try
                    {
                        int.Parse(CBUpperBound.Text);
                    }
                    catch
                    {
                        MessageBox.Show("The lowerbound cardinality doesn't seem correct.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }


                    try
                    {
                        int.Parse(CBUpperBound.Text);
                    }
                    catch
                    {
                        MessageBox.Show("The upperbound cardinality doesn't seem correct.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }


                    if (int.Parse(CBLowerBound.Text) > int.Parse(CBUpperBound.Text))
                    {
                        MessageBox.Show("LowerBound cardinality can't be higher than the upperbound.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }
            return true;
        }

        //parent UB  equal ""
        private bool CheckSimpleUBCardinality()
        {

            if (!CBUpperBound.Text.Equals(""))
            {
                MessageBox.Show("Upper bound must be empty.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                if (activeAttribute.GetParentLB().Equals("*"))
                {
                    if (CBLowerBound.Text.Equals("*"))
                    {
                        return true;
                    }
                    else
                    {
                        try
                        {
                            int.Parse(CBLowerBound.Text);
                        }
                        catch
                        {
                            MessageBox.Show("Lower bound cardinality doesn't seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }
                else
                {
                    try
                    {
                        int.Parse(CBLowerBound.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Lower bound cardinality doesn't seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    if (int.Parse(activeAttribute.GetParentLB()) < int.Parse(CBLowerBound.Text))
                    {
                        MessageBox.Show("The set number of the cardinality seem to be higher than his parent's cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }
            return true;
        }

        //parent LB equal ""
        private bool CheckSimpleLBCardinality()
        {
            MessageBox.Show("The parent's LowerBound cardinality shouldn't be equal to null.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return true;
        }

        //no parent card equal ""
        private bool CheckComplexCardinality()
        {
            bool SkipGlobalCheck = false;

            #region LBClientCardinality


            if (CBLowerBound.Text.Equals(""))
            {
                MessageBox.Show("A lowerlound cardinality can't be equal to null.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (this.CBLowerBound.Text.Equals("*"))
            {
                MessageBox.Show("A lowerbound cardinality can't be equal to *", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                try
                {
                    int.Parse(this.CBLowerBound.Text);
                    if (int.Parse(this.CBLowerBound.Text) < int.Parse(activeAttribute.GetParentLB()))
                    {
                        MessageBox.Show("Value entered for the lowerbound cardinality must be higher than the parent cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                }
                catch
                {
                    MessageBox.Show("Value entered for the lowerbound cardinality does'nt seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            #endregion

            #region UBClientCardinality

            if (this.CBUpperBound.Text.Equals("*"))
            {
                if (!activeAttribute.GetParentUB().Equals("*"))
                {
                    MessageBox.Show("Value entered for the upperbound cardinality can't be higher than the parent cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                else
                {
                    SkipGlobalCheck = true;
                }
            }
            else
            {
                try
                {
                    int.Parse(this.CBUpperBound.Text);
                    if (!activeAttribute.GetParentUB().Equals("*"))
                    {
                        if (int.Parse(this.CBUpperBound.Text) > int.Parse(activeAttribute.GetParentUB()))
                        {
                            MessageBox.Show("Value entered for the upperbound cardinality must be lower than the parent cardinality.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Value entered for the upperbound cardinality does'nt seem valid.", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            #endregion



            if (SkipGlobalCheck.Equals(false))
            {
                #region GlobalCheck
                try
                {
                    if (int.Parse(this.CBLowerBound.Text) > int.Parse(this.CBUpperBound.Text))
                    {
                        MessageBox.Show("Value entered for the cardinality does'nt seem valid (Lowerbound must be inferior to the uppperbound).", "Cardinality Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                catch { return false; }
                #endregion
            }
            return true;
        }



        


        private bool CheckCardinality()
        {
            if (activeAttribute.GetParentLB().Equals("") && activeAttribute.GetParentUB().Equals(""))
            {
                return CheckSimpleCardinality();
            }
            else if (activeAttribute.GetParentUB().Equals(""))
            {
                return CheckSimpleUBCardinality();
            }
            else if (activeAttribute.GetParentLB().Equals(""))
            {
                return CheckSimpleLBCardinality();
            }
            else
            {
                return CheckComplexCardinality();
            }
        }


        private void ButSave_Click(object sender, EventArgs e)
        {
            if(populatedEAClass.GetMode().Equals(CD.GetPrimitiveStereotype())){      
                
                XMLParser XMLP = new XMLParser(repo);       
                if (XMLP.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED)){
                    XMLP.AddXmlLog("IsBasedOnAttributeCardinality", "Saving cardinality for the (IsBasedOn primitive) attribute : " + activeAttribute.GetName() + " -Value : lowerBound : " + CBLowerBound.Text + " upperBound : " + CBUpperBound.Text);
                }
                activeAttribute.SetLowerBound(CBLowerBound.Text);
                activeAttribute.SetUpperBound(CBUpperBound.Text);
                ibof.SetUIItemCardinality(activeAttribute.GetGUID(), CBLowerBound.Text, CBUpperBound.Text);
                this.Dispose();

            }
            else{
                if(this.CheckCardinality().Equals(true)){       
                    XMLParser XMLP = new XMLParser(repo);       
                        if (XMLP.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
                        {
                            XMLP.AddXmlLog("IsBasedOnAttributeCardinality", "Saving cardinality for the attribute : " + activeAttribute.GetName() + " -Value : lowerBound : " + CBLowerBound.Text + " upperBound : " + CBUpperBound.Text);
                        }
                    activeAttribute.SetLowerBound(CBLowerBound.Text);
                    activeAttribute.SetUpperBound(CBUpperBound.Text);
                    ibof.SetUIItemCardinality(activeAttribute.GetGUID(), CBLowerBound.Text, CBUpperBound.Text);
                    this.Dispose();
                }
            }
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            XMLParser XMLP = new XMLParser(repo);
            if (XMLP.GetXmlValueConfig("Log") == (ConfigurationManager.CHECKED))
            {
                XMLP.AddXmlLog("IsBasedOnAttributeCardinality", "Exiting without save from the edition of the cardinality's attribute : " + activeAttribute.GetName());
            }
            this.Dispose();
        }
    }
}
