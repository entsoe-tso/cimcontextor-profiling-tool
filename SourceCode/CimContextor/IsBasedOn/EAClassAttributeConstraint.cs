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
    public class EAClassAttributeConstraint
    {
        private EA.Repository repo;
        private string parentGUID;
        private string constraintName;
        private string constraintType;
        private string constraintNotes;
        private EAClass ParentEAClass;
        private ConstantDefinition CD = new ConstantDefinition();

        public EAClassAttributeConstraint(EA.Repository repo, string parentType, string parentGUID, string constraintName, EAClass ParentObj)
        {
            ParentEAClass = ParentObj;
            if (parentType.Equals(CD.GetCreate()))
            {
                this.repo = repo;
                this.constraintName = constraintName;
            }
            else
            {

                this.repo = repo;
                this.parentGUID = parentGUID;
                EA.AttributeConstraint selectedConstraint = null;
                for (short i = 0; repo.GetAttributeByGuid(parentGUID).Constraints.Count > i; i++)
                {
                    EA.AttributeConstraint aConstraint = (EA.AttributeConstraint)repo.GetAttributeByGuid(parentGUID).Constraints.GetAt(i);
                    if (aConstraint.Name.Equals(constraintName))
                    {
                        selectedConstraint = aConstraint;
                        break;
                    }
                }
                this.constraintName = selectedConstraint.Name;
                this.constraintNotes = selectedConstraint.Notes;
                this.constraintType = selectedConstraint.Type;
            }
        }

        public bool CheckIfSameElement(EA.AttributeConstraint AnEAConstraint)
        {

            if(!(AnEAConstraint.Notes.Equals(this.GetNotes())))
            {
                return false;
            }

            if(!(AnEAConstraint.Type.Equals(this.GetType())))
            {
                return false;
            }

            return true;
        }

        public string GetName()
        {
            return constraintName;
        }

        public void SetName(string newName)
        {
            this.constraintName = newName;
        }

        public new string GetType()
        {
            return constraintType;
        }

        public string GetNotes()
        {
            return this.constraintNotes;
        }

        public void SetType(string newType)
        {
            this.constraintType = newType;
        }

        public void SetNotes(string newNotes)
        {
            this.constraintNotes = newNotes;
        }

        private void CreateIsBasedOn(EA.Attribute newChildAttribute)
        {
            newChildAttribute.Constraints.Refresh();
            EA.IAttributeConstraint childConstraint = (EA.IAttributeConstraint)newChildAttribute.Constraints.AddNew(this.GetName(), this.GetType());
            childConstraint.Notes = this.GetNotes();
            childConstraint.Update();
        }

        private void UpdateIsBasedOn(EA.Attribute newChildAttribute)
        {
            bool constraintFound = false;
            EA.AttributeConstraint AConstraintFound = null;
            for (short i = 0; newChildAttribute.Constraints.Count > i; i++)
            {
                EA.AttributeConstraint AConstraint = (EA.AttributeConstraint)newChildAttribute.Constraints.GetAt(i);
                if (AConstraint.Name.Equals(this.GetName()))
                {
                    constraintFound = true;
                    AConstraintFound = AConstraint;
                    break;
                }
            }
            if (constraintFound.Equals(false))
            {
                newChildAttribute.Constraints.Refresh();
                EA.IAttributeConstraint childConstraint = (EA.IAttributeConstraint)newChildAttribute.Constraints.AddNew(this.GetName(), this.GetType());
                childConstraint.Notes = this.GetNotes();
                childConstraint.Update();
            }
            else
            {
                AConstraintFound.Notes = this.GetNotes();
                AConstraintFound.Update();
            }
        }

        public void ExecuteIsBasedOn(EA.Attribute newChildAttribute, bool CreateMode)
        {
            if (CreateMode.Equals(true))
            {
                CreateIsBasedOn(newChildAttribute);
            }
            else
            {
                UpdateIsBasedOn(newChildAttribute);
            }
        }

        public void ResetConstraint()
        {
            EA.AttributeConstraint selectedConstraint = null;
            //foreach (EA.AttributeConstraint aConstraint in repo.GetAttributeByGuid(parentGUID).Constraints)
            for (short i = 0; repo.GetAttributeByGuid(parentGUID).Constraints.Count > i; i++)
            {
                EA.AttributeConstraint aConstraint = (EA.AttributeConstraint)repo.GetAttributeByGuid(parentGUID).Constraints.GetAt(i);
                if (aConstraint.Name.Equals(constraintName))
                {
                    selectedConstraint = aConstraint;
                    break;
                }
            }
            this.constraintName = selectedConstraint.Name;
            this.constraintNotes = selectedConstraint.Notes;
            this.constraintType = selectedConstraint.Type;
        }
    }
}
