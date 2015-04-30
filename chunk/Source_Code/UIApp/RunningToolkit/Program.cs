using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsharpIpy.cs;
using IronPython;

namespace CsharpIpy
{
    class Program
    {
        static void Main(string[] args)
        {
        	
        	if(!BeginExecute()){
        		return;
        	}
        	
            PyObject.AppendSysPath("Lib\\pythonlib.zip");
            PyObject.AppendSysPath("Scripts\\\\");

            do
            {
               var obj= PyObject.Create(AppDomain.CurrentDomain.BaseDirectory + "scripts/main.py");
               // Console.WriteLine(result);
            } while (Console.ReadKey().Key == ConsoleKey.Enter);

            Console.WriteLine("¼´½«¹Ø±Õ");

        }
        
        static bool BeginExecute(){
        	ArchiveReId.ReNotIdArchives();
        	ArchiveReId.ArchiveReCreateDate();
        	return true;
        }
    }
}
