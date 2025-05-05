using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.8.27                                         *    march 2019*
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
    class GlobalIsBasedOn
    {
        EA.Repository Repository;
        public GlobalIsBasedOn(EA.Repository Repository)
        {
        this.Repository = Repository;
        EA.Package SelectedPackage = null; 
            if (!((SelectedPackage = CheckContext())==null))
            {
                GlobalIsBasedOnForm GIBOF = new GlobalIsBasedOnForm(Repository,SelectedPackage);
                GIBOF.ShowDialog();
                GIBOF.Dispose(); // ABA20230401
            }
        }

        /// <summary>
        /// Check that a package have been selected in the browser.
        /// </summary>
        /// <returns>Return the selected package or null if none have been selected.</returns>
        private EA.Package CheckContext()
        {
            EA.Package SelectedPackage = null;
            try {
                SelectedPackage = (EA.Package) Repository.GetTreeSelectedPackage();
                if(SelectedPackage==null){
                    MessageBox.Show("You must select a package from the browser before using this functionnality.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return null;
                }
            }
            catch{
                MessageBox.Show("You must select a package from the browser before using this functionnality.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return null;
            }
                       
            return SelectedPackage;
        }

    }
}
