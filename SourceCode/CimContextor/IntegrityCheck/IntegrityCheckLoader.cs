using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace CimContextor
{
    class IntegrityCheckLoader
    {
        private EA.Repository Repository;
        private ArrayList ReportList = new ArrayList();
        private ConstantDefinition CD = ConstantDefinition.Instance;

        public IntegrityCheckLoader(EA.Repository repository)
        {
            Repository = repository;
            EA.Package SelectedPackage;
            try
            {
                SelectedPackage = (EA.Package)Repository.GetTreeSelectedObject();
            }
            catch
            {
                MessageBox.Show("You must select a package before usin this option!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
                
            if(SelectedPackage==null){
                MessageBox.Show("You must select a package before usin this option!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ObjectCheckerFactory aFactory = ObjectCheckerFactory.Instance;
            for (short i = 0; SelectedPackage.Elements.Count > i; i++)
            {
                EA.Element AnElement = (EA.Element) SelectedPackage.Elements.GetAt(i);
                ObjectCheckerInterface anObjectChecker = aFactory.GenerateObjectchecker(repository,AnElement,CD.GetClass());
                if (anObjectChecker!=null)
                {
                    Report aReport = new Report();
                    EA.Package aPackage = repository.GetPackageByID(AnElement.PackageID);
                    String ElementName=AnElement.Name;

                    while(aPackage!=null){

                        ElementName = aPackage.Name+"::"+ElementName;
                        if (aPackage.ParentID != 0)
                        {
                            aPackage = repository.GetPackageByID(aPackage.ParentID);
                        }
                        else
                        {
                            break;
                        }
                    
                    }
                    aReport.SetIBOElementName(ElementName);
                    ReportList.Add(anObjectChecker.CheckObjects(aReport));
                }
                else{
                    MessageBox.Show("A null element have been generated from the factory.\nTargeted element:"+AnElement.Name+".\nCarefull, elements might have been skipped.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }


            VisualReport VR = new VisualReport(ReportList);
            VR.ShowDialog();
        }
    }
}
