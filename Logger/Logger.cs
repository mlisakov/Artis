using System;
using System.IO;
using System.Text;

namespace Artis.Logger
{
    public static class Log
    {
        private static string path = @"C:\Sofit\Log";

        public static void WriteLog(string name, Exception ex)
        {
            string prefix = "[" + DateTime.Now + "] ";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            FileStream fs2 = new FileStream(path + @"\" + name, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs2, Encoding.Default);
            sw.WriteLine(prefix+ex.Message);
            sw.WriteLine(ex.StackTrace);
            sw.WriteLine();
            sw.Close();
        }

        public static void WriteLog(string name, string source)
        {
            string prefix = "[" + DateTime.Now + "] ";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            FileStream fs2 = new FileStream(path + @"\" + name, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs2, Encoding.Default);
            sw.WriteLine(prefix + source);
            sw.WriteLine();
            sw.Close();
        }
    }
}