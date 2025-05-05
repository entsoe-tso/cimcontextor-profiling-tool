using System;
using System.Collections.Generic;
using System.Text;
using CimContextor;


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


namespace CimContextor.Utilities
{
    public sealed class UtilitiesConstantDefinition
    {
        public short SOURCE = 0;
        public short TARGET = 1;
        public string  ProfileNameSuffix="P_"; // nom rajoute a un profile lors de la creation globale d'un package
                                               //----------------------  pour localRecoverDatatypes ---------------------------------
        public string DomainName="DomainProfile";
        public string Profilespack = "Profiles";
 //----------------------- pour tc69 -----------------------------------------
   public List<string> TVToBeDeleted = new List<string>() { "annotation", "author","businessTermName","dateLastChanged","definition","dictionaryEntryName","displayName","ECDMVersion","reference","uniqueID","sequenceKey" };
   public List<string> StereoToBeDeleted = new List<string>() { "Enexis ECDM::Facet", "Facet" };
        //-------------------------- pour ENTSO-E ------------------------------------
   public List<string>  typesubstitution=new List<string>(){"Float","Simple_Foat"}; 
   public List<string> CheckPColumsNames = new List<string>() { "Message","Package","Class","Attribute","Association","CIMText","ProfileText" };
   public List<string> CIMPackageNames = new List<string> { "IEC61970", "IEC61968", "IEC62325" };
   public string CheckPFile = "CheckProfileErrorFiles.csv";
   public string CPINFO = "Info";
   public string CPERR = "Err";
   public string CPSEPARATOR = ";";
   public int CPSUBNOTELENGTH = 100;
   public int CPTABLERAWDIM = 1000;
   public List<string> CPERRORS= new List<string>{
     "NotProperlyLinked"                        //there is an IsBasedOn link but not to the correct class
    ,"NotIsBasedOn"                             // not isBasedOn: a problem with the guid present in the tagvalue: "GUIDBasedOn"
    ,"NotIsBasedOnLinked"                       // Dependency stereotyped <<isBesedOn>> non present
    ,"NotSameNote"                              // the description in CIM and profile are different
    ,"HasNonAttachedNote"                       // the package has a note note linked to a class
    ,"HasAttachedNote"                          // a class has an attached note
    ,"NotSameStereotypes"                       // different stereotype in CIM and the Profile
    ,"NotSameCardinality"                       // not the same cardinality in CIM and the profile
    ,"HasConstraint"                            // an OCL constraint present in the profile
    ,"NotSameName"                              // the name is different in CIM and the profile
    ,"NotAGoodCimVersion"                       // the Version Class of the CIM package s not good
    ,"NotAGoodProfileVersion"                   // the Version Class of the profile package is not good
    ,"NotGoodPackNames"                         //some of the profile package names are different from the CIM package names
    ,"NotAccessibleElement"                     // some element are not accessible in the repository (CIM may have evolved)
    ,"NotInProfiles"                            // some element part of an association are not in the profile
    ,"NotSameType"                              // not the same type
    ,"NotConsistantRoles"                       // some AssociationEnds are not  consistant
    ,"NotSameRole"                              // the role label is different in CIM and in the profile
    ,"NotSameRoleNote"                          // the description of the roles are different
    ,"NotNaturalDirection"                      // the direction of an association is a choice to be checked
    ,"HasAnUnkownLink"                          // an unexpected link from an element
    ,"HasMoreThanOneIsBasedOnLink"              // an element can't have more than one isbasedon dependency
    ,"HasInheritanceIssue"                      // problem encountered while retriving an element's ancestor
    ,"NotConformedType"                         // the type of an attribute is not conformed for instanve value of a datatype is not a primitive
     ,"NotSameAbstract"                         // the CIM and Profile class have a different abstraction
     ,"CheckAborted"                            // the analysis is aborted because there is a serious issue
     ,"HasNote"                                 // the item has a description
     ,"NotLinkedToAClassNote"                   // a note which is found not linked to a class, probably linked to an or several associations
     ,"NotProperlyTyped"                        // the ID of the classifier of an attibute is equal to zero
   };
   
  
        //------------  partie pour htmltoprofile -----------------------
    public string h1concreteclass ="Concrete Classes";
    public string h1abstractclass="Abstract Classes";
    public string h1compound="Compound types";
    public string h1enumeration="Enumerations";
    public string h1datatypes = "Datatypes";


//----------------------------------------------------------------

    public const string EnumStereotype = "enumeration";
    public const string DatatypeStereotype = "CIMDatatype";
    public const string PrimitiveStereotype = "Primitive";
    public const string IsBasedOnStereotype = "IsBasedOn";
    public const string CompoundStereotype = "Compound";
    public const string AssemblyStereotype = "Assembly";
    public const string InlineStereotype = "Anonymous";
    public const string UnionStereotype = "Union";

    public const string StringType = "String";
    public const string EnumType = "enumeration";
    public const string BooleanType = "Boolean";
    public const string IntegerType = "Integer";
    public const string FloatType = "Float";

    public const bool AnnCategoryOption = false; // enable special annotations in xsds
    public static List<string> Cimstereos = new List<string>()
                                      {
	                                    "Class",
	                                    "Datatype",   // for ascending compatibility
			             	            "CIMDatatype", // take care of the consistency with above definitions
				                    	"enumeration",
                                        "Enumeration",
	                                    "Compound"
        	                                  
                                       };
    public static List<string> CimStereoDatatypes = new List<string>
    {
	                                    "Datatype",   // for ascending compatibility
			             	            "CIMDatatype", // take care of the consistency with above definitions
				                    	"enumeration",   
    };

    public List<string> SubprofilesStereos = new List<string>()
                                    {
                                        "ShortCircuit",
                                        "Operation",

                                    };
    //will need to add it as  a primitive type so that when you click on facet it give you the right window
    private string DecimalType = "Decimal";
    //
    private bool CheckExpirationDate = true;


    public enum VariableType
    {
        Numeric,
        Defined,
        Any
    }

    public static UtilitiesConstantDefinition Instance
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
        internal static readonly UtilitiesConstantDefinition instance = new UtilitiesConstantDefinition();
    }




    public string GetBooleanType()
    {
        return BooleanType;
    }
    public string GetIntegerType()
    {
        return IntegerType;
    }
    public string GetFloatType()
    {
        return FloatType;
    }
    public string GetProfilesPackageName()
    {
        return "Profiles";
    }
    public string GetIBOTagValue()
    {
        return "GUIDBasedOn";
    }
    public string GetRdfTagValue()
    {
        return "CIM:IsRdfAbout";
    }

    /// <summary>
    /// Return the date limit at witch the software must be desactivated.
    /// !!This will only be checked if CheckExpirationDate is = true!!
    /// </summary>
    /// <returns>date : 04 Nov 2009 23:59:59</returns>
    public DateTime GetExpirationDate()
    {
        DateTime date = DateTime.Parse("1 February 2017 23:59:59");
        return date;
    }

    /// <summary>
    /// Return the string to be shown in case of an expiration of the licence
    /// </summary>
    /// <returns></returns>
    public string GetExpirationWarning()
    {
        return "Your licence has expired." + " \r\n" + " \r\n" + "If it shouldn't have, please contact ENTSO-E." + " \r\n" + "Emails :" + " \r\n" + "    " + Constants.SUPPORT_EMAIL;
    }

    /// <summary>
    /// Return the format of an enumeration stereotype
    /// </summary>
    /// <returns></returns>
    public string GetEnumType()
    {
        return EnumType;
    }

    /// <summary>
    /// Return the format of a string type
    /// </summary>
    /// <returns></returns>
    public string GetStringType()
    {
        return StringType;
    }

    /// <summary>
    /// Return the format of the decimal stereotype
    /// </summary>
    /// <returns></returns>
    public string GetDecimalType()
    {
        return DecimalType;
    }

    /// <summary>
    /// Return the format of the enum stereotype
    /// </summary>
    /// <returns></returns>
    public string GetEnumStereotype()
    {
        return EnumStereotype;
    }
    /// <summary>
    /// Return the format of the Union Stereotype
    /// </summary>
    /// <returns></returns>
    public string GetUnionStereotype()
    {
        return UnionStereotype;
    }
    /// <summary>
    /// Return the format of the Compound  stereotype
    /// </summary>
    /// <returns></returns>
    public string GetCompoundStereotype()
    {
        return CompoundStereotype;
    }
    /// <summary>
    /// Return the format of the Assembly  stereotype
    /// </summary>
    /// <returns></returns>
    public string GetAssemblyStereotype()
    {
        return AssemblyStereotype;
    }

    /// <summary>
    /// Return the format of the Datatype stereotype
    /// </summary>
    /// <returns></returns>
    public string GetDatatypeStereotype()
    {
        return DatatypeStereotype;
    }
    /// <summary>
    /// Return the format of the IBO stereotype
    /// </summary>
    /// <returns></returns>
    public string GetIsBasedOnStereotype()
    {
        return IsBasedOnStereotype;
    }
    /// <summary>
    /// Return the format of the primitive stereotype
    /// </summary>
    /// <returns></returns>
    public string GetPrimitiveStereotype()
    {
        return PrimitiveStereotype;
    }
    public string GetInlineStereotype()
    {
        return InlineStereotype;
    }

    /////////////////////Code dependant constant
    /// <summary>
    /// Used as possible mode for EAClass
    /// </summary>
    /// <returns></returns>
    public string GetCreate()
    {
        return "CREATE";
    }
    /// <summary>
    /// Used as possible mode for EAClass
    /// </summary>
    /// <returns></returns>
    public string GetUpdate()
    {
        return "UPDATE";
    }

    /// <summary>
    /// Return the string representing the "no qualifier" case.
    /// </summary>
    /// <returns></returns>
    public string GetNoQualifier()
    {
        return "No qualifier";
    }
    /// <summary>
    /// Return the format used for any in the xmlconfig describing the different roles
    /// </summary>
    /// <returns></returns>
    public string GetAny()
    {
        return "any";
    }
    /// <summary>
    /// Return the format used for class in the xmlconfig describing the different roles
    /// </summary>
    /// <returns></returns>
    public string GetClass()
    {
        return "class";
    }


    public string GetDependency()
    {
        return "Dependency";
    }

    /// <summary>
    /// Define if the expiration date must be checked.
    /// </summary>
    /// <returns></returns>
    public bool GetExpirationCheckState()
    {
        return CheckExpirationDate;
    }
    public bool GetAnnCategoryOption()
    {
        return AnnCategoryOption;
    }
    }
}



