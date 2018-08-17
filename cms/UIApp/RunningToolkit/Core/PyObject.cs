/*
 * 由SharpDevelop创建。
 * 用户： newmin
 * 日期: 2013/11/5
 * 时间: 6:56
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using IronPython.Hosting;
using IronPython.Modules;
using Microsoft.Scripting.Hosting;
using System.IO;
using Microsoft.Scripting.Runtime;

namespace IronPython
{
	/// <summary>
	/// Description of PyObject.
	/// </summary>
	public class PyObject
	{
		public static ScriptEngine engine;
		private static ScriptScope scope;
		private ScriptSource source;
		private string filePath;
		
		static PyObject()
		{
			engine = Python.CreateEngine();
            scope = engine.CreateScope();
		}

		
		public static dynamic Create(string filePath)
		{
			if (filePath == "" || !File.Exists(filePath))
			{
				throw new FileNotFoundException();
			}

            //ScriptScope scope = engine.CreateScope();
			ScriptSource source = engine.CreateScriptSourceFromFile(filePath);
			
			source.Execute(scope);
			return scope;
		}

	    public static void AppendSysPath(string path)
        {
            //ScriptScope scope = engine.CreateScope();
	        ScriptSource source = engine.CreateScriptSourceFromString(
            	"import sys\nsys.path.append(\""+path+"\")");

            source.Execute(scope);
	    }
	}
}
