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
    public partial class AttributeClassifierForm : Form
    {
        private ConstantDefinition CD = new ConstantDefinition();
        private EA.Repository repo;
        private EAClass populatedEAClass;
        private string attributeGUID;
        private EAClassAttribute selectedAttribute;
        private EA.Attribute SelectedEAAttribute;
        private EA.Element parentClassifier;
        private ArrayList ClassifierDatatypeList = new ArrayList();
        private ArrayList ClassifierCompoundList = new ArrayList();
        private ArrayList ClassifierEnumList = new ArrayList();
        private string LVClassifierState;
        private bool CreateMode;

        public AttributeClassifierForm(bool CreateMode, EA.Repository Repository, EAClass populatedEAClass, string attributeGUID)
        {
            InitializeComponent();
            LVClassifierState = CD.GetDatatypeStereotype();
            this.CreateMode = CreateMode;
            this.repo = Repository;
            this.populatedEAClass = populatedEAClass;
            this.attributeGUID = attributeGUID;
            this.selectedAttribute = populatedEAClass.GetAttribute(attributeGUID);

            #region Create
            if (populatedEAClass.GetMode().Equals(CD.GetCreate()))
            {
                this.SelectedEAAttribute = Repository.GetAttributeByGuid(attributeGUID);
                if (!(SelectedEAAttribute == null))
                {

                    if (SelectedEAAttribute.IsStatic.Equals(true))
                    {
                        CBStaticValue.Enabled = false;
                    }
                    if (SelectedEAAttribute.IsConst.Equals(true))
                    {
                        CBConstantValue.Enabled = false;
                        TBDefaultValue.Enabled = false;
                    }
                }


                CBConstantValue.Checked = selectedAttribute.GetConstantState();
                CBStaticValue.Checked = selectedAttribute.GetStaticState();
                TBDefaultValue.Text = selectedAttribute.GetDefaultValue();


                ListViewItem lvi;
                try
                {
                    parentClassifier = repo.GetElementByID(populatedEAClass.GetAttribute(attributeGUID).GetClassifier());
                }
                catch
                {
                    MessageBox.Show("Link to the classifier of this attribut seem broken.\nDid you delete the classifier of this attribut ?\nYou must edit the link of this attribut and select his type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (parentClassifier == null)
                {
                    MessageBox.Show("Link to the classifier of this attribut seem broken.\nDid you delete the classifier of this attribut ?\nYou must edit the link of this attribut and select his type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                EA.Element SearchResult = parentClassifier;

                if (parentClassifier.Stereotype.Equals(CD.GetDatatypeStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetDatatypeStereotype() + ">>"))
                {
                    ClassifierDatatypeList.Add(SearchResult);
                }
                else if (parentClassifier.Stereotype.Equals(CD.GetPrimitiveStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetPrimitiveStereotype() + ">>"))
                {
                    ClassifierDatatypeList.Add(SearchResult);
                }
                else if (parentClassifier.Stereotype.Equals(CD.GetCompoundStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetCompoundStereotype() + ">>"))
                {
                    ClassifierCompoundList.Add(SearchResult);
                }
                else
                {
                    ClassifierEnumList.Add(SearchResult);
                }

                SearchPossibleClassifier(SearchResult);

                if (parentClassifier.Stereotype.Equals(CD.GetDatatypeStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetDatatypeStereotype() + ">>") || parentClassifier.Stereotype.Equals(CD.GetPrimitiveStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetPrimitiveStereotype() + ">>"))
                {
                    foreach (EA.Element AClassifier in ClassifierDatatypeList)
                    {
                        String[] aHeaders = new string[2];
                        aHeaders[0] = ""; //am
                        int packid = AClassifier.PackageID; //am
                        if (packid != 0) aHeaders[0] = repo.GetPackageByID(packid).Name; //am
                        aHeaders[0] = aHeaders[0] + "::" + AClassifier.Name; //am
                        aHeaders[1] = AClassifier.ElementID.ToString();
                        lvi = new ListViewItem(aHeaders);
                        LVClassifier.Items.Add(lvi);
                        if (AClassifier.ElementID.Equals(selectedAttribute.GetClassifier()))
                        {
                            lvi.Selected = true;
                        }
                    }
                }
                else if (parentClassifier.Stereotype.Equals(CD.GetCompoundStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetCompoundStereotype() + ">>"))
                {
                    LVClassifierState = CD.GetCompoundStereotype();
                    ButEnum.Text = "Show Datatype";
                    ButEnum.Visible = false;
                    LabState.Text = "Compound list";
                    foreach (EA.Element AClassifier in ClassifierCompoundList)
                    {
                        String[] aHeaders = new string[2];
                        aHeaders[0] = ""; //am
                        int packid = AClassifier.PackageID; //am
                        if (packid != 0) aHeaders[0] = repo.GetPackageByID(packid).Name; //am
                        aHeaders[0] = aHeaders[0] + "::" + AClassifier.Name; //am
                        aHeaders[1] = AClassifier.ElementID.ToString();
                        lvi = new ListViewItem(aHeaders);
                        LVClassifier.Items.Add(lvi);
                        if (AClassifier.ElementID.Equals(selectedAttribute.GetClassifier()))
                        {
                            lvi.Selected = true;
                        }
                    }
                }
                else
                {
                    LVClassifierState = CD.GetEnumStereotype();
                    ButEnum.Text = "Show Datatype";
                    LabState.Text = "Enumeration list";
                    foreach (EA.Element AClassifier in ClassifierEnumList)
                    {
                        String[] aHeaders = new string[2];
                        aHeaders[0] = ""; //am
                        int packid = AClassifier.PackageID; //am
                        if (packid != 0) aHeaders[0] = repo.GetPackageByID(packid).Name; //am
                        aHeaders[0] = aHeaders[0] + "::" + AClassifier.Name; //am
                       // aHeaders[0] = AClassifier.Name;
                        aHeaders[1] = AClassifier.ElementID.ToString();
                        lvi = new ListViewItem(aHeaders);
                        LVClassifier.Items.Add(lvi);
                        if (AClassifier.ElementID.Equals(selectedAttribute.GetClassifier()))
                        {
                            lvi.Selected = true;
                        }
                    }
                }

            }
            #endregion
            #region update
            else
            {
                //
                for (short i = 0; Repository.GetElementByGuid(selectedAttribute.getClass().GetParentElementGUID()).AttributesEx.Count > i; i++)
                {
                   
                    EA.Attribute AnAttribute=(EA.Attribute)Repository.GetElementByGuid(selectedAttribute.getClass().GetParentElementGUID()).AttributesEx.GetAt(i);
                    //
                    if (AnAttribute.Name.Equals(selectedAttribute.GetName()))
                    {
                        SelectedEAAttribute = AnAttribute;
                        break;
                    }
                }
                EA.Attribute ParentAttribute=null;
                for (short i = 0; Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).AttributesEx.Count > i; i++)
                {
                    EA.Attribute PAttribute = (EA.Attribute)Repository.GetElementByGuid(populatedEAClass.GetParentElementGUID()).AttributesEx.GetAt(i);
                    if (PAttribute.Name.Equals(selectedAttribute.GetName()))
                    {
                        ParentAttribute = PAttribute;
                        break;
                    }
                }
                if (!(ParentAttribute == null))
                {

                    if (ParentAttribute.IsStatic.Equals(true))
                    {
                        CBStaticValue.Enabled = false;
                    }
                    if (ParentAttribute.IsConst.Equals(true))
                    {
                        CBConstantValue.Enabled = false;
                        TBDefaultValue.Enabled = false;
                    }
                }



                CBConstantValue.Checked = selectedAttribute.GetConstantState();
                CBStaticValue.Checked = selectedAttribute.GetStaticState();
                TBDefaultValue.Text = selectedAttribute.GetDefaultValue();


                EA.Element SelectedClassifier = null; ;

                ListViewItem lvi;
                try
                {
                    parentClassifier = repo.GetElementByID(ParentAttribute.ClassifierID);
                }
                catch
                {
                    MessageBox.Show("The link to the classifier of this IBO parent attribute seem broken.\nDid you delete the classifier of this attribut ?\nYou must edit the link of this attribut and select his type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }

                try
                {
                    SelectedClassifier = repo.GetElementByID(selectedAttribute.GetClassifier());
                }
                catch
                {
                    MessageBox.Show("The link to the classifier of this attribute seem broken.\nDid you delete the classifier of this attribut ?\nYou must edit the link of this attribut and select his type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }


                if (parentClassifier == null)
                {
                    MessageBox.Show("The link to the classifier of this IBO parent attribute seem broken.\nDid you delete the classifier of this attribut ?\nYou must edit the link of this attribut and select his type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (SelectedClassifier == null)
                {
                    MessageBox.Show("The link to the classifier of this attribute seem broken.\nDid you delete the classifier of this attribut ?\nYou must edit the link of this attribut and select his type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }

                EA.Element SearchResult = parentClassifier;

                if (parentClassifier.Stereotype.Equals(CD.GetDatatypeStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetDatatypeStereotype() + ">>"))
                {
                    ClassifierDatatypeList.Add(SearchResult);
                }
                else if (parentClassifier.Stereotype.Equals(CD.GetPrimitiveStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetPrimitiveStereotype() + ">>"))
                {
                    ClassifierDatatypeList.Add(SearchResult);
                }
                else if (parentClassifier.Stereotype.Equals(CD.GetCompoundStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetCompoundStereotype() + ">>"))
                {
                    ClassifierCompoundList.Add(SearchResult);
                }
                else
                {
                    ClassifierEnumList.Add(SearchResult);
                }

                SearchPossibleClassifier(SearchResult);

                if (parentClassifier.Stereotype.Equals(CD.GetDatatypeStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetDatatypeStereotype() + ">>") || parentClassifier.Stereotype.Equals(CD.GetPrimitiveStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetPrimitiveStereotype() + ">>"))
                {
                    foreach (EA.Element AClassifier in ClassifierDatatypeList)
                    {
                        String[] aHeaders = new string[2];
                        aHeaders[0] = ""; //am
                        int packid = AClassifier.PackageID; //am
                        if (packid != 0) aHeaders[0] = repo.GetPackageByID(packid).Name; //am
                        aHeaders[0] = aHeaders[0] + "::" + AClassifier.Name; //am
                        aHeaders[1] = AClassifier.ElementID.ToString();
                        lvi = new ListViewItem(aHeaders);
                        LVClassifier.Items.Add(lvi);
                        if (AClassifier.ElementID.Equals(selectedAttribute.GetClassifier()))
                        {
                            lvi.Selected = true;
                        }
                    }
                }
                else if (parentClassifier.Stereotype.Equals(CD.GetCompoundStereotype()) || parentClassifier.Stereotype.Equals("<<" + CD.GetCompoundStereotype() + ">>"))
                {
                    LVClassifierState = CD.GetCompoundStereotype();
                    ButEnum.Text = "";
                    ButEnum.Visible = false;
                    LabState.Text = "Compound list";
                    foreach (EA.Element AClassifier in ClassifierCompoundList)
                    {
                        String[] aHeaders = new string[2];
                        aHeaders[0] = ""; //am
                        int packid = AClassifier.PackageID;  //am
                        aHeaders[0] = repo.GetPackageByID(packid).Name + "::" + AClassifier.Name; //am
                        //aHeaders[0] = AClassifier.Name;
                        aHeaders[1] = AClassifier.ElementID.ToString();
                        lvi = new ListViewItem(aHeaders);
                        LVClassifier.Items.Add(lvi);
                        if (AClassifier.ElementID.Equals(selectedAttribute.GetClassifier()))
                        {
                            lvi.Selected = true;
                        }
                    }
                }
                else
                {
                    LVClassifierState = CD.GetEnumStereotype();
                    ButEnum.Text = "Show Datatype";
                    LabState.Text = "Enumeration list";
                    foreach (EA.Element AClassifier in ClassifierEnumList)
                    {
                        String[] aHeaders = new string[2];
                        aHeaders[0] = ""; //am
                        int packid = AClassifier.PackageID; //am
                        if (packid != 0) aHeaders[0] = repo.GetPackageByID(packid).Name; //am
                        aHeaders[0] = aHeaders[0] + "::" + AClassifier.Name; //am
                        aHeaders[1] = AClassifier.ElementID.ToString();
                        //aHeaders[0] = AClassifier.Name;
                        lvi = new ListViewItem(aHeaders);
                        LVClassifier.Items.Add(lvi);
                        if (AClassifier.ElementID.Equals(selectedAttribute.GetClassifier()))
                        {
                            lvi.Selected = true;
                        }
                    }
                }

            }
            #endregion
        }






        private void  SearchPossibleClassifier(EA.Element ElementToSearch)
        {
            EA.Collection parentConnectorsList = ElementToSearch.Connectors;
            foreach (EA.Connector aConnector in parentConnectorsList)
            {
                if (aConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower()))
                {
                    EA.Element clientElement = repo.GetElementByID(aConnector.ClientID);

                    if (!clientElement.ElementID.Equals(ElementToSearch.ElementID))
                    {
                        if (clientElement.Stereotype.Equals(CD.GetPrimitiveStereotype()) || clientElement.Stereotype.Equals("<<" + CD.GetPrimitiveStereotype() + ">>"))//am aout 18
                        {
                            ClassifierDatatypeList.Add(clientElement);
                            SearchPossibleClassifier(clientElement);
                        }
                        if (clientElement.Stereotype.Equals(CD.GetDatatypeStereotype()) || clientElement.Stereotype.Equals("<<"+CD.GetDatatypeStereotype()+">>")){
                            ClassifierDatatypeList.Add(clientElement);
                            SearchPossibleClassifier(clientElement);
                        }
                        if (clientElement.Stereotype.Equals(CD.GetEnumStereotype()) || clientElement.Stereotype.Equals("<<"+CD.GetEnumStereotype()+">>"))
                        {
                            ClassifierEnumList.Add(clientElement);
                            SearchPossibleClassifier(clientElement);
                        }
                        if (clientElement.Stereotype.Equals(CD.GetCompoundStereotype()) || clientElement.Stereotype.Equals("<<" + CD.GetCompoundStereotype() + ">>"))
                        {
                            ClassifierCompoundList.Add(clientElement);
                            SearchPossibleClassifier(clientElement);
                        }

                    }
                }
            }
            //return null;
        }


        private void ButCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void ButSelect_Click(object sender, EventArgs e)
        {
           // if(populatedEAClass.get.GetMode().Equals(CD.GetCreate())){
             if(CreateMode.Equals(true)){
                SaveDataCreateMode();
                }
             else if (CreateMode.Equals(false))
                {
                SaveDataUpdateMode();
                }
            
        }

        public void SaveDataCreateMode()
        {
            if (LVClassifier.SelectedItems.Count.Equals(1))
            {

                if (!selectedAttribute.GetConstantState().Equals(true))
                {
                    selectedAttribute.SetDefaultValue(TBDefaultValue.Text);
                }

                if (!selectedAttribute.GetStaticState().Equals(true))
                {
                    populatedEAClass.SetAttributeClassifier(this.attributeGUID, int.Parse(LVClassifier.SelectedItems[0].SubItems[1].Text));
                }

                if (CBConstantValue.Checked.Equals(true))
                {
                    selectedAttribute.SetConstantState(true);
                }
                else
                {
                    selectedAttribute.SetConstantState(false);
                }
                if (CBStaticValue.Checked.Equals(true))
                {
                    selectedAttribute.SetStaticState(true);
                }
                else
                {
                    selectedAttribute.SetStaticState(false);
                }
                this.Dispose();
            }
            else
            {
                if(selectedAttribute.GetStaticState().Equals(true)){
                    if (!selectedAttribute.GetConstantState().Equals(true))
                    {
                        selectedAttribute.SetDefaultValue(TBDefaultValue.Text);
                    }
                    if (CBConstantValue.Checked.Equals(true))
                    {
                        selectedAttribute.SetConstantState(true);
                    }
                    else
                    {
                        selectedAttribute.SetConstantState(false);
                    }
                   
                    this.Dispose();
                }
            }
        }
        
        public void SaveDataUpdateMode()
        {

            if (LVClassifier.SelectedItems.Count.Equals(1))
            {

                if (!selectedAttribute.GetConstantState().Equals(true))
                {
                    selectedAttribute.SetDefaultValue(TBDefaultValue.Text);
                }

                if (!selectedAttribute.GetStaticState().Equals(true))
                {
                    populatedEAClass.SetAttributeClassifier(this.attributeGUID, int.Parse(LVClassifier.SelectedItems[0].SubItems[1].Text));
                }

                if (CBConstantValue.Checked.Equals(true))
                {
                    selectedAttribute.SetConstantState(true);
                }
                else
                {
                    selectedAttribute.SetConstantState(false);
                }
                if (CBStaticValue.Checked.Equals(true))
                {
                    selectedAttribute.SetStaticState(true);
                }
                else
                {
                    selectedAttribute.SetStaticState(false);
                }
                this.Dispose();
            }
            else
            {
                if (selectedAttribute.GetStaticState().Equals(true))
                {
                    if (!selectedAttribute.GetConstantState().Equals(true))
                    {
                        selectedAttribute.SetDefaultValue(TBDefaultValue.Text);
                    }
                    if (CBConstantValue.Checked.Equals(true))
                    {
                        selectedAttribute.SetConstantState(true);
                    }
                    else
                    {
                        selectedAttribute.SetConstantState(false);
                    }

                    this.Dispose();
                }
            }

        }

        private void ButEnum_Click(object sender, EventArgs e)
        {
            if (LVClassifierState.Equals(CD.GetDatatypeStereotype()))
            {
                LVClassifierState =CD.GetEnumStereotype();
                ButEnum.Text = "Show Datatype";
                LabState.Text="Enumeration list";
                foreach (ListViewItem AnItem in LVClassifier.Items)
                {
                    AnItem.Remove();
                }

                ListViewItem lvi;
                foreach (EA.Element AClassifier in ClassifierEnumList)
                {
                    String[] aHeaders = new string[2];
                    aHeaders[0] = ""; //am
                    int packid = AClassifier.PackageID; //am
                    if (packid != 0) aHeaders[0] = repo.GetPackageByID(packid).Name; //am
                    aHeaders[0] = aHeaders[0] + "::" + AClassifier.Name; //am
                  //  aHeaders[0] = AClassifier.Name;
                    aHeaders[1] = AClassifier.ElementID.ToString();
                    lvi = new ListViewItem(aHeaders);
                    LVClassifier.Items.Add(lvi);
                    if (AClassifier.ElementID.Equals(selectedAttribute.GetClassifier()))
                    {
                        lvi.Selected = true;
                    }
                }

            }
            else {
                LVClassifierState = CD.GetDatatypeStereotype();
                ButEnum.Text = "Show Enum";
                LabState.Text = "Datatype list";
                foreach (ListViewItem AnItem in LVClassifier.Items)
                {
                    AnItem.Remove();
                }

                ListViewItem lvi;
                foreach (EA.Element AClassifier in ClassifierDatatypeList)
                {
                    String[] aHeaders = new string[2];
                    aHeaders[0] = ""; //am
                    int packid = AClassifier.PackageID; //am
                    if (packid != 0) aHeaders[0] = repo.GetPackageByID(packid).Name; //am
                    aHeaders[0] = aHeaders[0] + "::" + AClassifier.Name; //am
                   // aHeaders[0] = AClassifier.Name;
                    aHeaders[1] = AClassifier.ElementID.ToString();
                    lvi = new ListViewItem(aHeaders);
                    LVClassifier.Items.Add(lvi);
                    if (AClassifier.ElementID.Equals(selectedAttribute.GetClassifier()))
                    {
                        lvi.Selected=true;
                    }
                }
            }
        }


    }
}
