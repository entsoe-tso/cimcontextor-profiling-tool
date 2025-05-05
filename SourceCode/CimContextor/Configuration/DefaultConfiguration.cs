/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20230205
/////////////////////////////////////////////////////////////////////////////////////////

using System.Windows.Forms;

namespace CimContextor.Configuration
{
    public class DefaultConfiguration
    {
        private static DefaultConfiguration defaultConfiguration = null;
        private static AddIn addIn = new AddIn();

        private DefaultConfiguration()
        {
        }

        public static DefaultConfiguration GetDefaultConfiguration()
        {
            if (defaultConfiguration == null) // singleton
            {
                defaultConfiguration = new DefaultConfiguration();
                defaultConfiguration.Initialize();
            }
            return new DefaultConfiguration();
        }

        public AddIn GetAddIn()
        {
            return addIn;
        }

        private void Initialize()
        {
            AppSettings appSettings = new AppSettings();
            appSettings.AddConfiguration(new Configuration("IsBasedOn", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("Confirm", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("Log", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("Warning", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("ConfigColor", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("QualifyDatatypeEnumCompound", ConfigurationManager.UNCHECKED));
            appSettings.AddConfiguration(new Configuration("CopyParentElement", ConfigurationManager.UNCHECKED));
            appSettings.AddConfiguration(new Configuration("EnablePropertyGrouping", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("EnableConcreteInheritanceInProfiles", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("EnabledIntermediaryInheritance", ConfigurationManager.UNCHECKED));
            appSettings.AddConfiguration(new Configuration("EnableESMPHierarchy", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("EnableCheckAttributeIdentifier", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("NavigationEnabled", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("SimpleProfiling", ConfigurationManager.UNCHECKED));
            appSettings.AddConfiguration(new Configuration("AutomaticChangeOfRoleName", ConfigurationManager.CHECKED));
            appSettings.AddConfiguration(new Configuration("W13AutomaticAnscesterInProfile", ConfigurationManager.UNCHECKED));
            appSettings.AddConfiguration(new Configuration("CsvDelimiter", OptionForm.COMMA));

            DataProfiles dataProfiles = new DataProfiles();
            dataProfiles.AddProfData(new Profdata("ListStereoNamespace", "Entsoe|ShortCircuit|Operation|Abstract"));
            dataProfiles.AddProfData(new Profdata("EntsoeDataTypesDomain", "ProfileDomain"));
            dataProfiles.AddProfData(new Profdata("ProfileUMLName", "profileUML"));
            dataProfiles.AddProfData(new Profdata("ProfilesPackage", "Profiles"));

            DataQualifier dataQualifier = new DataQualifier();
            dataQualifier.AddQualifier(new Qualifier("AvaillableToAll", "any"));
            dataQualifier.AddQualifier(new Qualifier("OnlyAvailableToClass", "class"));
            dataQualifier.AddQualifier(new Qualifier("OnlyAvailableToDataType", "datatype"));
            dataQualifier.AddQualifier(new Qualifier("OnlyAvailableToRole", "role"));
            dataQualifier.AddQualifier(new Qualifier("Reason", "any"));
            dataQualifier.AddQualifier(new Qualifier("Acknowledgement", "any"));
            dataQualifier.AddQualifier(new Qualifier("TimeSeries", "any"));
            dataQualifier.AddQualifier(new Qualifier("TestElement", "any"));
            dataQualifier.AddQualifier(new Qualifier("ABC", "any"));

            DataConstraint dataConstraint = new DataConstraint();
            dataConstraint.AddConstraint(new Constraint("testDatatype", "OCL", "datatype", "mystringinOCL"));

            DataClassifierConstraint dataClassifierConstraint = new DataClassifierConstraint();
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "DateTime", "any", "", @"For datetime the correct pattern is : \n\r -?([1-9][0-9]{3,}|0[0-9]{3})-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])T(([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\.[0-9]+)?|(24:00:00(\.0+)?))(Z|(\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00))? \r\n&#xD;&#xA; for TruncationOrReduce DateTime the correct pattern is: \r\n&#xD;&#xA;[1-9][0-9]{3,}|0[0-9]{3})-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])T(([01][0-9]|2[0-3]):([0-5][0-9]Z \r\n", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("TruncationOrReduced", "INV", "DateTime", "DEFINED", "gYearMonthDayHourMinute,toto", @"[1-9][0-9]{3,}|0[0-9]{3})-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])T(([01][0-9]|2[0-3]):([0-5][0-9]Z", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "DateTime", "any", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "DateTime", "any", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "DateTime", "any", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "DateTime", "any", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "DateTime", "any", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "Date", "any", "", @"the correct pattern is :\n\r -?([1-9][0-9]{3,}|0[0-9]{3})-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])(Z|(\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00))?", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Date", "any", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Date", "any", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Date", "any", "", @"For date the correct pattern is : -?([1-9][0-9]{3,}|0[0-9]{3})-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])(Z|(\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00))?\r\n", @"	inv: self-&gt;minExclusive($N$)	"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Date", "any", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Date", "any", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Time", "any", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Time", "any", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Time", "any", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Time", "any", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Time", "any", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "Time", "any", "", @"", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("length", "OCL", "String", "Numeric", "", @"", @"inv: self-&gt;Length($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("string_kind", "INV", "String", "DEFINED", "normalizedString,token,NMTOKEN,Name,NCName,anyURI,ID,IDREF", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minLength", "OCL", "String", "Numeric", "", @"", @"inv: self-&gt;Minlength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxLength", "OCL", "String", "Numeric", "", @"", @"inv: self-&gt;MaxLength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "String", "Numeric", "", @"", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("whiteSpace", "INV", "String", "DEFINED", "preserve,replace,collapse", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "String", "String", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("integer_kind", "INV", "Integer", "DEFINED", "long,int,short,byte,unsignedLong,unsignedInt,unsignedShort,unsignedByte", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "Integer", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Integer", "Integer", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Integer", "Integer", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Integer", "Integer", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Integer", "Integer", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Integer", "Integer", "", @"For an ennumeration enter values separated with a comma", @"inv: self-&gt;let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("precision", "INV", "Float", "DEFINED", "simple,double", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Float", "Numeric", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Float", "Numeric", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Float", "Numeric", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Float", "Numeric", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Float", "String", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "Float", "String", "", @"", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "Decimal", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("fractionDigits", "OCL", "Decimal", "Integer", "", @"", @"inv: self-&gt;fractionDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Decimal", "Decimal", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Decimal", "Decimal", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Decimal", "Decimal", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Decimal", "Decimal", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Decimal", "String", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Duration", "any", "", @"", @"	inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Duration", "any", "", @"", @"	inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Duration", "any", "", @"", @"	inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Duration", "any", "", @"", @"	inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Duration", "any", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "Duration", "any", "", @"", @"	inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("long_kind", "INV", "Long", "DEFINED", "int,short,byte,unsignedLong,unsignedInt,unsignedShort,unsignedByte", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "Long", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Long", "Integer", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Long", "Integer", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Long", "Integer", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Long", "Integer", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Long", "Integer", "", @"For an ennumeration enter values separated with a comma", @"inv: self-&gt;let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("length", "OCL", "HexBinary", "Numeric", "", @"", @"inv: self-&gt;Length($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minLength", "OCL", "HexBinary", "Numeric", "", @"", @"inv: self-&gt;Minlength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxLength", "OCL", "HexBinary", "Numeric", "", @"", @"inv: self-&gt;MaxLength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "HexBinary", "String", "", @"", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "HexBinary", "String", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("length", "OCL", "Base64Binary", "Numeric", "", @"", @"inv: self-&gt;Length($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minLength", "OCL", "Base64Binary", "Numeric", "", @"", @"inv: self-&gt;Minlength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxLength", "OCL", "Base64Binary", "Numeric", "", @"", @"inv: self-&gt;MaxLength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "Base64Binary", "String", "", @"", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Base64Binary", "String", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("length", "OCL", "ID", "Numeric", "", @"", @"inv: self-&gt;Length($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minLength", "OCL", "ID", "Numeric", "", @"", @"inv: self-&gt;Minlength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxLength", "OCL", "ID", "Numeric", "", @"", @"inv: self-&gt;MaxLength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "ID", "String", "", @"", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "ID", "String", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("length", "OCL", "IDREF", "Numeric", "", @"", @"inv: self-&gt;Length($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minLength", "OCL", "IDREF", "Numeric", "", @"", @"inv: self-&gt;Minlength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxLength", "OCL", "IDREF", "Numeric", "", @"", @"inv: self-&gt;MaxLength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "IDREF", "String", "", @"", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "IDREF", "String", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("int_kind", "INV", "Int", "DEFINED", "short,byte,unsignedInt,unsignedShort,unsignedByte", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "Int", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Int", "Integer", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Int", "Integer", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Int", "Integer", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Int", "Integer", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Int", "Integer", "", @"For an ennumeration enter values separated with a comma", @"inv: self-&gt;let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("short_kind", "INV", "Short", "DEFINED", "byte,unsignedShort,unsignedByte", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "Short", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Short", "Integer", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Short", "Integer", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Short", "Integer", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Short", "Integer", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Short", "Integer", "", @"For an ennumeration enter values separated with a comma", @"inv: self-&gt;let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("byte_kind", "INV", "Byte", "DEFINED", "unsignedByte", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "Byte", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "Byte", "Integer", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "Byte", "Integer", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "Byte", "Integer", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "Byte", "Integer", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "Byte", "Integer", "", @"For an ennumeration enter values separated with a comma", @"inv: self-&gt;let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("unsignedLong_kind", "INV", "UnsignedLong", "DEFINED", "unsignedInt,unsignedShort,unsignedByte", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "UnsignedLong", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "UnsignedLong", "Integer", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "UnsignedLong", "Integer", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "UnsignedLong", "Integer", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "UnsignedLong", "Integer", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "UnsignedLong", "Integer", "", @"For an ennumeration enter values separated with a comma", @"inv: self-&gt;let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("unsignedInt_kind", "INV", "UnsignedInt", "DEFINED", "unsignedShort,unsignedByte", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "UnsignedInt", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "UnsignedInt", "Integer", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "UnsignedInt", "Integer", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "UnsignedInt", "Integer", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "UnsignedInt", "Integer", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "UnsignedInt", "Integer", "", @"For an ennumeration enter values separated with a comma", @"inv: self-&gt;let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("unsignedShort_kind", "INV", "UnsignedShort", "DEFINED", "unsignedByte", @"", @"choice=$C$"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "UnsignedShort", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "UnsignedShort", "Integer", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "UnsignedShort", "Integer", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "UnsignedShort", "Integer", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "UnsignedShort", "Integer", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "UnsignedShort", "Integer", "", @"For an ennumeration enter values separated with a comma", @"inv: self-&gt;let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("totalDigits", "OCL", "UnsignedByte", "Integer", "", @"", @"inv: self-&gt;TotalDigits($I$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minInclusive", "OCL", "UnsignedByte", "Integer", "", @"", @"inv: self-&gt;minInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxInclusive", "OCL", "UnsignedByte", "Integer", "", @"", @"inv: self-&gt;maxInclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minExclusive", "OCL", "UnsignedByte", "Integer", "", @"", @"inv: self-&gt;minExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxExclusive", "OCL", "UnsignedByte", "Integer", "", @"", @"inv: self-&gt;maxExclusive($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "UnsignedByte", "Integer", "", @"For an ennumeration enter values separated with a comma", @"inv: self-&gt;let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("length", "OCL", "AnyURI", "Numeric", "", @"", @"inv: self-&gt;Length($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("minLength", "OCL", "AnyURI", "Numeric", "", @"", @"inv: self-&gt;Minlength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("maxLength", "OCL", "AnyURI", "Numeric", "", @"", @"inv: self-&gt;MaxLength($N$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("pattern", "OCL", "AnyURI", "String", "", @"", @"inv: self-&gt;Pattern($P$)"));
            dataClassifierConstraint.AddClassifierConstraint(new ClassifierConstraint("enumeration", "OCL", "AnyURI", "String", "", @"For an ennumeration enter values separated with a comma", @"inv: let answerlist=Set{$L$} in answerlist-&gt;includes(self)"));
            
            addIn.SetAppSettings(appSettings);
            addIn.SetDataProfiles(dataProfiles);
            addIn.SetDataQualifier(dataQualifier);
            addIn.SetDataConstraint(dataConstraint);
            addIn.SetDataClassifierConstraint(dataClassifierConstraint);
        }




    }
}
