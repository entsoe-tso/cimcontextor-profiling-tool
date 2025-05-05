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
    public  class XMLClassifierConstraint
    {
        private string ConstraintName;
        private string ConstraintType;
        private string ConstraintNote;
        private string VariableType;
        private string VariableList;
        private string Comment;


        public XMLClassifierConstraint(string ConstraintName, string ConstraintType, string VariableType, string VariableList, string Comment, string ConstraintNote)
        {
            this.ConstraintName = ConstraintName;
            this.ConstraintType = ConstraintType;
            this.ConstraintNote = ConstraintNote;
            this.VariableType = VariableType;
            this.VariableList = VariableList;
            this.Comment = Comment;
        }

        public string GetName(){
            return this.ConstraintName;
        }
        public new string GetType() => this.ConstraintType;
        public string GetNote()
        {
            return this.ConstraintNote;
        }
        public string GetVariableType()
        {
            return this.VariableType;
        }
        public string GetVariableList()
        {
            return this.VariableList;
        }
        public string GetComment()
        {
            return this.Comment;
        }
    }
}
