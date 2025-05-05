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
    public class EAClassConstraint
    {
        private EA.Repository Repo;
        private string Name;
        private string Notes;
        private string Type;
        private string Status;
        private EAClass ParentEAClass;
        private ConstantDefinition CD = new ConstantDefinition();

        public EAClassConstraint(EA.Repository Repo, string Name, string Type, string Notes, string Status, EAClass ParentObj)
        {
            this.Repo = Repo;
            this.ParentEAClass = ParentObj;
            this.Name = Name;
            this.Notes = Notes;
            this.Status = Status;
            this.Type = Type;

        }

        public void SetName(string Name)
        {
            this.Name = Name;
        }
        public void SetType(string Type)
        {
            this.Type = Type;
        }
        public void SetNotes(string Notes)
        {
            this.Notes = Notes;
        }
        public void SetStatus(string Status)
        {
            this.Status = Status;
        }

        public string GetStatus()
        {
            return Status;
        }
        public string GetNotes()
        {
            return Notes;
        }
        public string GetName()
        {
            return Name;
        }
        public new string GetType()
        {
            return Type;
        }

        private void CreateIsBasedOn(EA.Element NewElement)
        {
            NewElement.Constraints.Refresh();
            bool ConstraintFound = false;
            EA.Constraint OldConstraint = null;
            for (short i = 0; NewElement.Constraints.Count > i; i++)
            {
                EA.Constraint AConstraint = (EA.Constraint)NewElement.Constraints.GetAt(i);
                if (AConstraint.Name.Equals(this.GetName()))
                {
                    ConstraintFound = true;
                    OldConstraint = AConstraint;
                    break;
                }
            }
            if (ConstraintFound.Equals(false))
            {
                EA.Constraint NewElementConstraint = (EA.Constraint)NewElement.Constraints.AddNew(this.GetName(), this.GetType());
                NewElementConstraint.Notes = this.GetNotes();
                NewElementConstraint.Update();
            }
            else
            {
                OldConstraint.Notes = this.GetNotes();
                OldConstraint.Update();
            }
        }

        private void UpdateIsBasedOn(EA.Element NewElement)
        {
            NewElement.Constraints.Refresh();
            bool ConstraintFound = false;
            EA.Constraint OldConstraint = null;
            for (short i = 0; NewElement.Constraints.Count > i; i++)
            {
                EA.Constraint AConstraint = (EA.Constraint)NewElement.Constraints.GetAt(i);
                if (AConstraint.Name.Equals(this.GetName()))
                {
                    ConstraintFound = true;
                    OldConstraint = AConstraint;
                    break;
                }
            }
            if (ConstraintFound.Equals(false))
            {
                EA.Constraint NewElementConstraint = (EA.Constraint)NewElement.Constraints.AddNew(this.GetName(), this.GetType());
                NewElementConstraint.Notes = this.GetNotes();
                NewElementConstraint.Update();
            }
            else
            {
                OldConstraint.Notes = this.GetNotes();
                OldConstraint.Update();
            }
        }

        public void ExecuteIsBasedOn(EA.Element NewElement, bool CreateMode)
        {

            if (CreateMode.Equals(true))
            {
                CreateIsBasedOn(NewElement);
            }
            else
            {
                UpdateIsBasedOn(NewElement);
            }
        }

    }
}
