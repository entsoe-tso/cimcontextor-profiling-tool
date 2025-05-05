using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
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
    class RoleStorage
    {
        private string TargetName;
        private ArrayList RoleName = new ArrayList();

        public RoleStorage(string NewTargeName)
        {
            this.TargetName = NewTargeName;
        }

        /// <summary>
        /// return false if name already exist
        /// </summary>
        /// <param name="FullName"></param>
        /// <returns></returns>
        public bool AddRole(string FullName)
        {
            if (!FullName.Equals(""))
            {
                if (RoleName.Contains(FullName))
                {
                    return false;
                }
                else
                {
                    RoleName.Add(FullName);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public string GetTargetName()
        {
            return TargetName;
        }


    }
}
