using System;
using System.IO;
using System.Text;

namespace JR.DevFw.Template.Compiler.Text
{
    internal class File
    {
        public static String ReadFileToString(String fileName, Encoding encoding)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs, encoding);
                return sr.ReadToEnd();
            }
        }
    }
}