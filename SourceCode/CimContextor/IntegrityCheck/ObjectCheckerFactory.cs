using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CimContextor
{
    public sealed class ObjectCheckerFactory
    {
        private ConstantDefinition CD = new ConstantDefinition();

        public static ObjectCheckerFactory Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {

            static Nested()
            {
            }
            internal static readonly ObjectCheckerFactory instance = new ObjectCheckerFactory();
        }


        public ObjectCheckerInterface GenerateObjectchecker(EA.Repository Repository, object anObject, string ObjectType){


            if (ObjectType.ToLower() == CD.GetClass().ToLower())
            {
                ObjectCheckerInterface anObjectChecker = new ClassChecker(Repository,((EA.Element)anObject));
                return anObjectChecker;
            }



            return null;
        }



    }
}
