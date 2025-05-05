/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20221226
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.IO;
using System.Windows.Forms;
//using System.Security.Cryptography;

namespace CimContextor.Utilities
{
    
    internal class LicenseManager
    {
        private static readonly string SALT = "Entsoe4Ever";
        private static readonly int RAWKEY_LEN = 8;
        private static readonly string DATE_FORMAT = "YYYYMMDD";
        private string fileFullPath;
        //private byte[] key = { 0x08, 0x02, 0x06, 0x03, 0x03, 0x09, 0x11, 0x12, 0x09, 0x09, 0x02, 0x11, 0x05, 0x03, 0x11, 0x13 };
        public static readonly int WARNING_DAYS = 30; // start 30 days before expiry to warn user
        private static readonly int LIC_KEY = 8;
        private static readonly int STORE_KEY = 3;
        public LicenseManager()
        {
            this.fileFullPath = FileManager.GetParentDirPath() + "\\key.lic";
        }

        public bool LicenseExists()
        {
            return File.Exists(fileFullPath);
        }

        public string LoadLicense()
        {
            string licKey = "";
            string decLicKey = "";
            using (StreamReader sr = new StreamReader(fileFullPath))
            {
                CeasarCipher cCipher = new CeasarCipher();
                licKey = sr.ReadToEnd();
                if (licKey != null)
                {
                    licKey = licKey.Trim();
                    decLicKey = cCipher.Decipher(licKey, STORE_KEY);
                }
            }
            return decLicKey;
            //FileStream fileStream = new FileStream(fileFullPath, FileMode.Open);
            //Aes aes = Aes.Create();
            //byte[] iv = new byte[aes.IV.Length];
            //fileStream.Read(iv, 0, iv.Length);
            //CryptoStream cryptStream = new CryptoStream(fileStream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read);
            //StreamReader sReader = new StreamReader(cryptStream);
            //string licKey = sReader.ReadToEnd();
            //sReader.Close();
            //return licKey.Trim();
        }

        public void SaveLicense(string licenseKey)
        {
            using (StreamWriter sw = new StreamWriter(fileFullPath))
            {
                CeasarCipher cCipher = new CeasarCipher();
                string encLicenseKey = cCipher.Encipher(licenseKey, STORE_KEY);
                sw.WriteLine(encLicenseKey.Trim());
            }
            //FileStream fileStream = new FileStream(fileFullPath, FileMode.OpenOrCreate);
            //Aes aes = Aes.Create();
            //aes.Key = key;
            //byte[] iv = aes.IV;
            //fileStream.Write(iv, 0, iv.Length);
            //CryptoStream cryptStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            //StreamWriter sWriter = new StreamWriter(cryptStream);
            //sWriter.WriteLine(licenseKey.Trim());
            //sWriter.Close();
        }

        public int GetDateDiff(string licKey)
        {
            string licDate = licKey.Substring(RAWKEY_LEN, DATE_FORMAT.Length);
            DateTime licDateTime = DateTime.ParseExact(licDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime currDateTime = DateTime.Now;
            return (licDateTime - currDateTime).Days;
        }
        public bool IsExpired(string licDate)
        {
            return (GetDateDiff(licDate) < 0);
        }

        public bool IsValidLicenseKey(string licKey)
        {
            if ((licKey == null) || (licKey.Length < (RAWKEY_LEN + DATE_FORMAT.Length))) return false;
            string clearTxt = licKey.Substring(0, RAWKEY_LEN + DATE_FORMAT.Length);
            string hashKey = licKey.Substring(RAWKEY_LEN + DATE_FORMAT.Length);
            return (GetHash(clearTxt, SALT) == hashKey);
        }

        public static string GetExpiryDate(string licKey)
        {
            return licKey.Substring(RAWKEY_LEN, DATE_FORMAT.Length);
        }

        public string GetHash(string text, string salt = "")
        {
            CeasarCipher cCipher = new CeasarCipher();
            return cCipher.Encipher(text, LIC_KEY);
            //using (SHA1 sha = SHA1.Create())
            //{
            //    byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text + salt));

            //    // Convert back to a string, removing the '-' that BitConverter adds
            //    string hash = BitConverter
            //                   .ToString(hashBytes)
            //                    .Replace("-", String.Empty);
            //    return hash;
            //}
        }

    }
}
