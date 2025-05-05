// Product: CimContextor
// Author: Alexander Balka
// Date: 20221127
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace CimContextor
{
    public static class ErrorCodes
    {
        public static readonly string[] ERROR_001 = { "ERROR_001", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_002 = { "ERROR_002", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_003 = { "ERROR_003", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_004 = { "ERROR_004", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_005 = { "ERROR_005", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_006 = { "ERROR_006", "Loading custom configuration failed, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_007 = { "ERROR_007", "XML validation error:\n" };
        public static readonly string[] ERROR_008 = { "ERROR_008", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_009 = { "ERROR_009", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_010 = { "ERROR_010", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_011 = { "ERROR_011", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_012 = { "ERROR_012", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_013 = { "ERROR_013", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_014 = { "ERROR_014", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_015 = { "ERROR_015", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_016 = { "ERROR_016", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_017 = { "ERROR_017", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_018 = { "ERROR_018", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_019 = { "ERROR_019", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_020 = { "ERROR_020", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_021 = { "ERROR_021", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_022 = { "ERROR_022", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_023 = { "ERROR_023", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_024 = { "ERROR_024", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_025 = { "ERROR_025", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_026 = { "ERROR_026", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_027 = { "ERROR_027", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_028 = { "ERROR_028", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_029 = { "ERROR_029", "Invalid CSV delimiter in configuration!" };
        public static readonly string[] ERROR_030 = { "ERROR_030", "Missing CSV delimiter definition in configuration!" };
        public static readonly string[] ERROR_031 = { "ERROR_031", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_032 = { "ERROR_032", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_033 = { "ERROR_033", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_034 = { "ERROR_034", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_035 = { "ERROR_035", "You must select an IsBasedOn child class (only one) from a diagram before using this function!" };
        public static readonly string[] ERROR_036 = { "ERROR_036", "You must select an IsBasedOn child class (only one) from a diagram before using this function!" };
        public static readonly string[] ERROR_037 = { "ERROR_037", "You must select an IsBasedOn child class (only one) from a diagram before using this function!" };
        public static readonly string[] ERROR_038 = { "ERROR_038", "Internal system error, please inform CimConteXtor's support!" };
        public static readonly string[] ERROR_039 = { "ERROR_039", "Invalid license!" };
        public static readonly string[] ERROR_040 = { "ERROR_040", "Is not first profile level!" };
        public static readonly string[] WARNING_041 = { "WARNING_041", "Is not first profile level!" };
        public static readonly string[] ERROR_042 = { "ERROR_042", "A subdivided connector must be oriented!" };
        public static readonly string[] ERROR_043 = { "ERROR_043", "A subdivided connector must be oriented!" };
        public static readonly string[] ERROR_044 = { "ERROR_044", "Internal system error on event handling: " };
        public static readonly string[] ERROR_045 = { "ERROR_045", "Internal system error on copy of connector: " };

        public static void ShowException(string[] error, Exception ex)
        {
            MessageBox.Show(error[0] + ": " + error[1] + "\n" + ex.Message + "\n" + ex.StackTrace,
                "Exception",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

        }

        public static void ShowException(string[] error, string message, Exception ex)
        {
            MessageBox.Show(error[0] + ": " + error[1] + "\n" + message + "\n" + ex.Message + "\n" + ex.StackTrace,
                "Exception",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

        }

        public static void ShowError(string[] error)
        {
            MessageBox.Show(error[0] + ": " + error[1],
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
        }
    }
}


