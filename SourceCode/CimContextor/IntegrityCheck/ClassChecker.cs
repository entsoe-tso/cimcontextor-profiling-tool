using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CimContextor
{
    class ClassChecker : ObjectCheckerInterface
    {
        private EA.Element anIBOElement;
        private EA.Element aParentElement;
        private EA.Repository repository;
        private ArrayList StringList = new ArrayList();
        private ConstantDefinition CD = ConstantDefinition.Instance;
        private Report aReport;

        public ClassChecker(EA.Repository Repository, EA.Element AnElement)
        {
            repository = Repository;
            anIBOElement = AnElement;
        }

        public Report CheckObjects(Report AReport){
            aReport = AReport;
            EA.Connector anIBOConnector = null;

            //Checking IBO link
            for (short i = 0; anIBOElement.Connectors.Count > i; i++)
            {
                EA.Connector aConnector = (EA.Connector)anIBOElement.Connectors.GetAt(i);
                if(aConnector.Stereotype.ToLower().Equals(CD.GetIsBasedOnStereotype().ToLower())){
                    if(aConnector.ClientID.Equals(anIBOElement.ElementID)){
                        anIBOConnector = aConnector;
                        aParentElement = repository.GetElementByID(anIBOConnector.SupplierID);
                        break;
                    }
                }
            }
            if(anIBOConnector==null){
                aReport.AddClassReport("The element " + anIBOElement.Name + " have lost his Is Based On link!\nAs the IBO link is broken, skipping deeper checking on this element.");
                return aReport;
            }
            //Done checking IBO link 

            NameCheck();
            ClassConstraintCheck();
            VariousElementCheck();



            return aReport;
        }

        private void NameCheck()
        {
            string[] splitedParentName = aParentElement.Name.Split("_".ToCharArray());
            string[] splitedIBOName = anIBOElement.Name.Split("_".ToCharArray());

            foreach (string aParentString in splitedParentName)
            {
                bool found = false;
                foreach (string anIBOString in splitedIBOName)
                {
                    if (aParentString == anIBOString)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    aReport.AddClassReport("Can't match the name of the parent element and of the IBO element (" + anIBOElement.Name + "/" + aParentElement.Name + ")");
                    break;
                }
            }
        }

        private void ClassConstraintCheck()
        {
        }

        private void VariousElementCheck()
        {
        }


    }
}
