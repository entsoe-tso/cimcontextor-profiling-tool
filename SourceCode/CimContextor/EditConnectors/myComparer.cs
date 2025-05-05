using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
namespace CimContextor.EditConnectors
{
    public class myComparer : IComparer
    {
        //class to sort Ed
        //EA.Repository repo;
        //public myComparer(EA.Repository repos)
        //{
        //   repo=repos;
        // }
        // compare 2 EditEAClassConnector according to the TargetedElement Name
        int IComparer.Compare(Object x, Object y)
        {
            int ret = 0;
            string targetedxName = ((EditEAClassConnector)x).GetTargetedIBOElement().Name;
            string targetedyName = ((EditEAClassConnector)y).GetTargetedIBOElement().Name;

            ret = targetedxName.CompareTo(targetedyName);
            return ret;
        }
    }  
}
