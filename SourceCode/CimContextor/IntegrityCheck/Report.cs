using System;
using System.Collections;
using System.Text;

namespace CimContextor
{
    public class Report
    {
        private ArrayList classReport = new ArrayList();
        private String anIBOElementName ="";
        private Boolean isOk = true;

        public Report()
        {
        }

        public void SetIBOElementName(String IBOElementName)
        {
            anIBOElementName = IBOElementName;
        }
        public String GetIBOElementName()
        {
            return anIBOElementName;
        }

        public Boolean GetIsOk()
        {
            return isOk;
        }
    
        public void AddClassReport(String Data){
            classReport.Add(Data);
            isOk = false;
        }

        public String GetClassReportType()
        {
            return "Class";
        }

        public ArrayList GetClassReport()
        {
            return classReport;
        }



    }
}
