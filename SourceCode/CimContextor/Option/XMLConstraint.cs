using System;
using System.Collections.Generic;
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
    public class XMLConstraint
    {
        private string constraintName;
        private string constraintType;
        private string constraintNote;
        private string constraintAllowedTo = "any";
        public XMLConstraint(string constraintName, string constraintType, string constraintNote)
        {
            this.constraintName = constraintName;
            this.constraintType = constraintType;
            this.constraintNote = constraintNote;

        }
        public XMLConstraint(string constraintName, string constraintType, string constraintNote,string constraintAllowedTo)
        {
            this.constraintName = constraintName;
            this.constraintType = constraintType;
            this.constraintNote = constraintNote;
            this.constraintAllowedTo = constraintAllowedTo;

        }

        public string GetName(){
            return this.constraintName;
        }
        public new string GetType()
        {
            return this.constraintType;
        }
        public string GetNote()
        {
            return this.constraintNote;
        }
        public string GetAllowedTo()
        {
            return this.constraintAllowedTo;
        }
    }
}
