/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2013/10/31
 * 时间: 17:06
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ops.Cms.Web
{
	/// <summary>
	/// Description of CmsMvc.
	/// </summary>
	public class CmsMvc
	{
		static CmsMvc()
		{
		
		}
		
		/// <summary>
		/// 获取所有的控制器
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllControllers()
		{
			Type[] types;
			Type baseType=typeof(System.Web.Mvc.Controller);
			
			//foreach(Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
			//{
				//types=ass.GetTypes();
				
				types=Assembly.GetExecutingAssembly().GetTypes();
				foreach(Type t in types)
				{
					if(t.IsSubclassOf(baseType) && t.Name.EndsWith("Controller",true,null))
					   yield return t;
				}
			//}
		}
	}
}
