/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// File: ElementSorter.cs
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CimContextor.Utilities
{
    public class ElementSorter
    {
        EA.Repository repository = null;
        private List<EA.Diagram> diagramList = new List<EA.Diagram>();
        private List<EA.Element> classList = new List<EA.Element>();
        private List<EA.Element> enumerationList = new List<EA.Element>();
        private List<EA.Element> cimDatatypeList = new List<EA.Element>();
        private List<EA.Element> primitiveList = new List<EA.Element>();
        private List<EA.Element> restList = new List<EA.Element>();
        private List<EA.Element> bottomList = new List<EA.Element>();
        EA.Package selectedPackage = null;
        public ElementSorter(EA.Repository repository)
        {
            this.repository = repository;
            this.selectedPackage = repository.GetTreeSelectedPackage();
        }

        private void FillLists()
        {
            foreach (EA.Diagram diagram in selectedPackage.Diagrams)
            {
                this.diagramList.Add(diagram);
            }

            foreach (EA.Element element in selectedPackage.Elements)
            {
                string stereotype = element.Stereotype;
                if (stereotype.Equals("") && element.Type.Equals("Class"))
                {
                    this.classList.Add(element);
                }
                else if (stereotype.Equals("Enumeration") || stereotype.Equals("enumeration"))
                {
                    this.enumerationList.Add(element);
                }
                else if (stereotype.Equals("CIMDatatype"))
                {
                    this.cimDatatypeList.Add(element);
                }
                else if (stereotype.Equals("Primitive"))
                {
                    this.primitiveList.Add(element);
                }
                else if(stereotype.Equals("owl"))
                {
                    this.bottomList.Add(element);
                }
                else
                {
                    this.restList.Add(element);
                }
            }
        }

        void SetPositionOfElement(string GUID, int pos)
        {
            EA.Element elem = ((EA.Element)this.repository.GetElementByGuid(GUID));
            elem.TreePos = pos;
            elem.Update();
            elem.Refresh();
        }

        // returns last position
        int SetPositionsInList(List<EA.Element> elemList, int startPos)
        {
            int i;
            for (i = 0; i < elemList.Count; i++)
            {
                SetPositionOfElement(((EA.Element)elemList[i]).ElementGUID, i + startPos);
            }
            return (i + startPos);
        }

        public void SortElementsInPackage()
        {
            this.FillLists();

            this.diagramList.Sort((d1, d2) => d1.Name.CompareTo(d2.Name));
            this.classList.Sort((d1, d2) => d1.Name.CompareTo(d2.Name));
            this.enumerationList.Sort((e1, e2) => e1.Name.CompareTo(e2.Name));
            this.cimDatatypeList.Sort((e1, e2) => e1.Name.CompareTo(e2.Name));
            this.primitiveList.Sort((e1, e2) => e1.Name.CompareTo(e2.Name));
            this.restList.Sort((e1, e2) => e1.Name.CompareTo(e2.Name));

            int lastPos = SetPositionsInList(classList, 1);
            lastPos = SetPositionsInList(enumerationList, lastPos);
            lastPos = SetPositionsInList(cimDatatypeList, lastPos);
            lastPos = SetPositionsInList(primitiveList, lastPos);
            lastPos = SetPositionsInList(restList, lastPos);
            lastPos = SetPositionsInList(bottomList, lastPos);
            selectedPackage.Elements.Refresh();
            selectedPackage.Update();
            repository.GetProjectInterface().ReloadProject();
        }
    }
}
