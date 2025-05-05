using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CimContextor.Utilities
{
    internal class EAUtilities
    {

        public static void DeleteElement(EA.Repository repo, EA.Element elem)
        {
            EA.Collection elements;

            if (elem.ParentID > 0)
            {
                elements = repo.GetElementByID(elem.ParentID).Elements;
            }
            else
            {
                elements = repo.GetPackageByID(elem.PackageID).Elements;
            }

            for (short i = 0; i < elements.Count; i++)
            {
                if (((EA.Element)elements.GetAt(i)).ElementID == elem.ElementID)
                {
                    elements.DeleteAt(i, false);
                    elements.Refresh();
                    return;
                }
            }
        }

        public static void DeleteElementFromCurrentDiagram(EA.Repository repo, short objID)
        {
            repo.GetCurrentDiagram().DiagramObjects.Delete(objID);
            repo.GetCurrentDiagram().DiagramObjects.Refresh();
            repo.GetCurrentDiagram().Update();
        }

    }
}
