/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// File: Logger.cs
/////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CimContextor.Utilities
{
    public class Logger
    {
        private string fileName = null;
        private bool on = false;

        // FileManager.GetParentDirPath() + "\\cimcontextor.log";
        public Logger(string fileName, bool on)
        {
            this.fileName = fileName;
            this.on = on;
        }
        public void Log(string msg)
        {
            if (!on) return;
            try
            {
                using (StreamWriter w = System.IO.File.AppendText(fileName))
                {
                    w.Write("|> " + msg + "\n");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static void LogFile(string fileName, string content)
        {
            Stream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            if (fileStream != null)
            {
                byte[] info = new UTF8Encoding(true).GetBytes(content);
                fileStream.Write(info, 0, info.Length);
                fileStream.Flush();
                fileStream.Close();
            }
        }

    }
}
