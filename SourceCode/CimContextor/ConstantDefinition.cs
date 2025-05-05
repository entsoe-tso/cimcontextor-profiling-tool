using System;
using System.Collections.Generic;
using System.Text;
/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.9.11                                       *  September 2020 *
*                                                                         *
***************************************************************************
*                                                                         *
*       Credit to:                                                        *
*                   Andre Maizener   andre.maizener@zamiren.fr            *
*                   Jean-Luc Sanson  jean-luc.sanson@noos.fr              *
*                                                                         *
*       Contact: +33148854006                                             *
*                                                                         *
***************************************************************************

**************************************************************************/
namespace CimContextor
{
    public sealed class ConstantDefinition
    {

        //-------------------------- pour ENTSO-EIntegrityCheck ------------------------------------
        public List<string> typesubstitution = new List<string>() { "Float", "Simple_Foat" };
        public List<string> CheckPColumsNames = new List<string>() { "Message", "Severity","Package", "Class", "Attribute", "Association", "CIMText", "ProfileText" };
        public List<string> CIMPackageNames = new List<string> { "IEC61970", "IEC61968", "IEC62325" };
        public string CheckPFile = "CheckError" + Main.Repo.GetTreeSelectedPackage().Name + ".csv"; 
        public string CPINFO = "Info";
        public string CPERR = "Err";
        public string CPSEPARATOR = ";";
        public int CPSUBNOTELENGTH = 100;
        public int CPTABLERAWDIM = 1000;
        public List<string> CPERRORS = new List<string>{
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
     ,"NotProperlyTyped"                     // the ID of the classifier of an attibute is equal to zero
     ,"SomeInconsistency"                       // some thing in the feature is not consistent
     ,"NotsameDefaultValue"                     // not same default value
     ,"NotConsistantEnumerationValues"
   };
   
  
        
        private string EnumStereotype = "enumeration";
        private string DatatypeStereotype = "CIMDatatype";
        private string CompoundStereotype="Compound";
        private string PrimitiveStereotype = "Primitive";
        private string ClassStereotype = "Class";
        private string IsBasedOnStereotype = "IsBasedOn";
        private string AssemblyStereotype = "IsBasedOn";  //"AssembledFrom";

        private string StringType = "String";
        private string EnumType = "enumeration";
        private string BooleanType ="Boolean";
        private string IntegerType ="Integer";
        private string FloatType ="Float";
        //will need to add it as  a primitive type so that when you click on facet it give you the right window
        private string DecimalType  = "Decimal";
        private string DurationType ="Duration";
        private string DateTimeType ="DateTime";
        private string DateType ="Date";
        private string TimeType  = "Time";
        //---- additional list of primitives 
        private string AnyUriType  = "AniURI";
        private string  Base64BinaryType = "Base64Binary";
        private string ByteType  = "Byte";
        private string HexBinaryType  = "HexBinary";
        private string IDType  = "ID";
        private string IDREFType  = "IDREF";
        private string  IntType = "Int";
        private string  LongType = "Long";
        private string ShortType = "Short";
        private string UnsignedByteType = "UnsignedByte";
        private string UnsignedIntType = "UnsignedInt";
        private string UnsignedLongType = "UnsignedLong";
        private string UnsignedShortType = "UnsignedShort";
        private string suffixDiagGrouppingDependancies="-dependances";// pour le diagramme assembly

        public int LineStyleDirect = 1;
        public int LineStyleAutorouting = 2;
        public int LineStyleCustom = 3;

        //
        private static readonly bool CheckExpirationDate = true;//false;
        

        public enum VariableType{
            Numeric,
            Defined,
            Any
        }

    public static ConstantDefinition Instance
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
            internal static readonly ConstantDefinition instance = new ConstantDefinition();
    }

    public string GetBooleanType()
    {
        return BooleanType;
    }
    public string GetIntegerType()
    {
        return this.IntegerType;
    }
    public string GetFloatStereotype()
    {
        return FloatType;
    }

    /// <summary>
    /// Return the date limit at witch the software must be desactivated.
    /// !!This will only be checked if CheckExpirationDate is = true!!
    /// </summary>
    /// <returns>date : 04 Nov 2009 23:59:59</returns>
    public static DateTime GetExpirationDate()
    {//1er septembre
        DateTime date = DateTime.Parse("30 march 2099 23:59:59");  
        return date ;
    }

    /// <summary>
    /// Return the string to be shown in case of an expiration of the licence
    /// </summary>
    /// <returns></returns>
    public string GetExpirationWarning()
    {
        return "Your licence have expired." + " \r\n" + " \r\n" + "If it shouldn't have, please contact Zamiren." + " \r\n" + "Emails :" + " \r\n" + "    andre.maizener@zamiren.fr" + " \r\n" + "    jean-luc.sanson@noos.fr" + " \r\n" + "Phone : +33148854006";
    }

    /// <summary>
    /// Return the format of an enumeration stereotype
    /// </summary>
    /// <returns></returns>
    public string GetEnumType()
    {
        return this.EnumType;
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
    
    public string GetDurationType()
          {
        return DurationType;
    }
    public string GetDateTimeType()
          {
        return DateTimeType;
    }
    public string GetDateType()
          {
        return DateType;
    }
    public string GetTimeType()
    {
        return TimeType;
    }

    public string GetAnyUriType()
{
    return AnyUriType;
    }
    public string GetBase64BinaryType() 
        {
            return Base64BinaryType;
    }
    public string GetByteType() 
        {
            return ByteType;
    }
    public string GetHexBinaryType() 
        {
            return HexBinaryType;
    }
    public string GetIDType() 
        {
            return IDType;
    }
    public string GetIDREFType()
        {
            return IDREFType;
    }
    public string GetIntType()
        {
            return IntType;
    }
    public string GetLongType()
        {
            return LongType;
    }
    public string GetShortType()
        {
            return ShortType;
    }
    public string GetUnsignedByteType()
        {
            return UnsignedByteType;
    }
    public string GetUnsignedIntType()
        {
            return UnsignedIntType;
    }
    public string GetUnsignedLongType()
        {
            return UnsignedLongType;
    }
    public string GetUnsignedShortType()
    {
        return UnsignedShortType;
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
    /// Return the format of the Datatype stereotype
/// </summary>
/// <returns></returns>
    public string GetDatatypeStereotype()
    {
        return DatatypeStereotype;
    }
    /// <summary>
    /// Return the format of the Compound stereotype
    /// </summary>
    /// <returns></returns>
    public string GetCompoundStereotype()
    {
        return CompoundStereotype;
    }

    /// <summary>
    /// Return the format of the IBO stereotype
        /// </summary>
        /// <returns></returns>
    public string GetIsBasedOnStereotype()
    {
        return IsBasedOnStereotype;
    }

    public string GetAssemblyStereotype()
    {
        return AssemblyStereotype;
    }

    public string RegroupStereotype="GroupedInto";
    public string GetRegroupStereotype()
    {
        return RegroupStereotype;
    }

        /// <summary>
    /// Return the format of the primitive stereotype
        /// </summary>
        /// <returns></returns>
    public string GetPrimitiveStereotype()
    {
        return PrimitiveStereotype;
    }

    public string GetIBOTagValue()
    {
        return "GUIDBasedOn";
    }

    public string GetRangTagValue()
    {
        return "ESMPRG";
    }

        public string GetIBOTagValueNote()
    {
        //return "It's the Enterprise architect's GUID of the parent connector.";
        return "";
    }
        public List<string> GetCimPackageNames()
        {

            List<string> ll = new List<string>() { "IEC61970", "IEC61968", "IEC62325" };
            return ll;
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
        return ClassStereotype;
    }


    public string GetDependency()
    {
        return "Dependency";
    }
    public string GetsuffixDiagGrouppingDependancies()
    {
        return suffixDiagGrouppingDependancies;
    }

    /// <summary>
    /// Define if the expiration date must be checked.
    /// </summary>
    /// <returns></returns>
    public static bool GetExpirationCheckState()
    {
        return CheckExpirationDate;
    }

    public string prefixforgroupping = "Assembly ";

    }

    
} 
