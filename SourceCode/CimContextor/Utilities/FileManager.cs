/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20221018
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace CimContextor.Utilities
{
    internal class FileManager
    {
        private static string parentDirPath = null;
        private static string CreateParentDirPath()
        {
            string path = @"%AppData%" + Constants.CIMCONTEXTOR_RES_PATH;
            return Environment.ExpandEnvironmentVariables(path);
        }

        public static string GetParentDirPath()
        {
            if(parentDirPath == null)
            {
                parentDirPath = CreateParentDirPath();
            }
            return parentDirPath;
        }

        public static void CreateParentDirectory()
        {
            parentDirPath = CreateParentDirPath();
            Directory.CreateDirectory(parentDirPath);
        }
        
        // Creates a subdirectory directly under the parent directory
        public static void CreateSubDirectory(string subDir)
        {
            string pathString = CreateParentDirPath();
            if (!subDir.StartsWith("\\"))
            {
                subDir = "\\" + subDir;
            }
            Directory.CreateDirectory(pathString + subDir);
        }

        public static Bitmap GetImageByName(string packagePathWithDot, string imageNameWithExtension)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream stream = myAssembly.GetManifestResourceStream(packagePathWithDot + imageNameWithExtension);
            if (stream != null)
            {
                return new Bitmap(stream);
            }
            return null;
        }

        public static string GetTextByName(string packagePathWithDot, string textFileNameWithExtension)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream stream = myAssembly.GetManifestResourceStream(packagePathWithDot + textFileNameWithExtension);
            if (stream != null)
            {
                string content;
                using (StreamReader reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
                return content;
            }
            return null;
        }

    }
}
