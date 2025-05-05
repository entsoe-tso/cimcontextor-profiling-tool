/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20221226
/////////////////////////////////////////////////////////////////////////////////////////

namespace CimContextor.Integrity_Checking
{
    public class ValidationEntry
    {
        public static string[] headers = {"Severity", "Code", "Message", "Package", "Class", "Attribute"};
        private string severity;
        private string code;
        private string message;
        private string checkedPackage;
        private string checkedClass;
        private string checkedAttribute;

        public ValidationEntry(string severity, string code, string message, string checkedPackage, string checkedClass, string checkedAttribute)
        {
            this.severity = severity;
            this.code = code; // ValidationCode
            this.message = message;
            this.checkedPackage = checkedPackage;
            this.checkedClass = checkedClass;
            this.checkedAttribute = checkedAttribute;
        }

        public string CheckedPackage
        {
            get { return checkedPackage; }
            set { checkedPackage = value; }
        }

        public string CheckedClass
        {
            get { return checkedClass; }
            set { checkedClass = value; }
        }

        public string CheckedAttribute
        {
            get { return checkedAttribute; }
            set { checkedAttribute = value; }
        }
        public string SeverityLevel
        {
            get { return severity; }
            set { severity = value; }
        }

        public string ValidationCode
        {
            get { return code; }
            set { code = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

    }
}
