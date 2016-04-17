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
using Microsoft.Scripting.Hosting;
using System.IO;

namespace IronPython
{
	/// <summary>
	/// Description of PyObject.
	/// </summary>
	public class PyObject
	{
		public static ScriptEngine engine;
		private ScriptScope scope;
		private ScriptSource source;
		private string filePath;
		
		static PyObject()
		{
			engine = Python.CreateEngine();
		}
		public PyObject(string filePath)
		{
			this.filePath = filePath;
		}
		
		public dynamic Create()
		{
			if (this.filePath == "" || !File.Exists(this.filePath))
			{
				throw new FileNotFoundException();
			}
			
			source = engine.CreateScriptSourceFromFile(this.filePath);
			scope = engine.CreateScope();
			
			source.Execute(scope);
			return scope;
		}
		
		public static dynamic Create(string filePath)
		{
			if (filePath == "" || !File.Exists(filePath))
			{
				throw new FileNotFoundException();
			}
			
			ScriptSource source = engine.CreateScriptSourceFromFile(filePath);
			ScriptScope scope = engine.CreateScope();
			
			source.Execute(scope);
			return scope;
		}
	}
}
