using System;
using System.Collections.Generic;
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
    public class IsBasedOnClass
    {
        private bool defaultAttribute = true;
        private EA.Repository repo;
        private EA.EventProperties ep;
        private EA.Element parentElement = null;
        private EA.Diagram targetedDiagram = null;
        private EA.Package targetedPackage = null;
        private ConstantDefinition CD = new ConstantDefinition();
      static  public bool pureprimitive;

        public EA.Repository GetRepo()
        {
            return repo;
        }
        public IsBasedOnClass(EA.Repository repo, EA.EventProperties ep)
        { 
            this.repo = repo;
            this.ep = ep;
        }



        public void SetDefaultAttribute(bool defaultAttribute)
        {
            this.defaultAttribute = defaultAttribute;
        }

        public bool BeforeIsBasedOn()
        {  
            for (short i = 0; ep.Count > i; i++)
            {
                EA.EventProperty anEP = (EA.EventProperty)ep.Get(i);
                if (anEP.Name.Equals("ID"))
                {
                    parentElement = repo.GetElementByID(int.Parse((string)anEP.Value));
                }
                else if (anEP.Name.Equals("DiagramID"))
                {
                    targetedDiagram = repo.GetDiagramByID(int.Parse((string)anEP.Value));
                    targetedPackage = repo.GetPackageByID(targetedDiagram.PackageID);
                }
            }

            if ((!targetedDiagram.Equals(null)) && (!targetedPackage.Equals(null)))
            {
                XMLParser XMLP = new XMLParser(repo);
                if (XMLP.GetXmlValueConfig("Confirm") == ("Checked"))
                {
                    DialogResult result = MessageBox.Show("CimContextor :\n Do you want to override the common EA Function ?\n This will create a new element based on the dragged element.", "CimContextor : IsBasedOn",
                                                          MessageBoxButtons.YesNo, 
                                                          MessageBoxIcon.Exclamation);
                    if (result.Equals(DialogResult.No))
                    {   
                        return true;
                    }
                    pureprimitive = false;
                    if (parentElement.Stereotype == CD.GetPrimitiveStereotype())
                    {
                        result = MessageBox.Show("Do you want to create a Primitive (if not a Datatype)?",
                                                 "Base an object on a Primitive", 
                                                 MessageBoxButtons.YesNo);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        { // create a primitive as a primitive
                            pureprimitive = true;
                        }
                        else
                        {
                            pureprimitive = false;                              
                        }
                    }

                }
            
                return false;
            }
            return true;
        }

        public void ExecuteIsBasedOn(int ParentObjectTop, int ParentObjectBottom, int ParentObjectRight, int ParentObjectLeft, string ParentObjectStyle, bool IsHere)
        {

            //foreach (EA.EventProperty anEP in ep)
            for (short i = 0; ep.Count > i; i++)
            {
                EA.EventProperty anEP = (EA.EventProperty)ep.Get(i);
                if (anEP.Name.Equals("ID"))
                {
                    parentElement = repo.GetElementByID(int.Parse((string)anEP.Value));
                }

            }

            targetedDiagram = repo.GetCurrentDiagram();
            targetedDiagram.Update();
            targetedDiagram.DiagramObjects.Refresh();
            //Possible source de l'erreur !
            //repo.ReloadDiagram(targetedDiagram.DiagramID);


            //null only used for update if it's the child element.
            EAClass populatedEAClass = new EAClass(repo, targetedDiagram, parentElement.ElementGUID, null, CD.GetCreate(), ParentObjectTop, ParentObjectBottom, ParentObjectRight, ParentObjectLeft, ParentObjectStyle, IsHere);

            XMLParser XMLP = new XMLParser(repo);
            IsBasedOnForm ibof=null;


            /*
            if ((parentElement.Stereotype.ToLower().Equals(CD.GetPrimitiveStereotype().ToLower())))
            { // am june 2011
                // Initializes the variables to pass to the MessageBox.Show method.

                string message = "do you wantto create a Primitive (if not a Datatype)?";
                string caption = "Basing an object on a Primitive";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);

                if (result == System.Windows.Forms.DialogResult.Yes)
                { // create a primitive as a primitive

                    // Closes the parent for
                    pureprimitive=true;
                    //populatedEAClass.PurePrimitiveIsBasedOn();
                   // repo.RefreshOpenDiagrams(false);
                  //  return;

                }
            }
             */
                    if (((parentElement.Stereotype.ToLower().Equals(CD.GetDatatypeStereotype().ToLower())) || (parentElement.Stereotype.ToLower().Equals(CD.GetPrimitiveStereotype().ToLower())) ||
                        (parentElement.Stereotype.ToLower().Equals(CD.GetEnumStereotype().ToLower())))
                       && (!pureprimitive) )

                    {
                        
                            //populatedEAClass.PurePrimitiveIsBasedOn();
                        

                            ibof = new IsBasedOnForm(repo, populatedEAClass, XMLP.GetXmlQualifier(CD.GetDatatypeStereotype().ToLower()), true, null);
                           
                    }
                    else
                    {
                        ibof = new IsBasedOnForm(repo, populatedEAClass, XMLP.GetXmlQualifier(CD.GetClass().ToLower()), true, null);
         

                    }
                    if (ibof != null)
                    {
                        ibof.Show();
                        ibof.SetUILoading(false);
                    }
                    
                }
              
        
        public void RefreshActiveDiagram(int diagramId)
        {
            repo.ReloadDiagram(diagramId);
        }


        //End Class       
    }
}