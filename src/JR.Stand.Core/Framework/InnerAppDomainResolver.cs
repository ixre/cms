using System;
using System.IO;
using System.Reflection;

namespace JR.Stand.Core.Framework
{
    internal class InnerAppDomainResolver
    {
        public static void Resolve(String resolveFullDir)
        {
            AppDomain domain = AppDomain.CurrentDomain;
            if (String.IsNullOrEmpty(resolveFullDir))
            {
                resolveFullDir = domain.BaseDirectory + FwCtx.Variables.AssemblyPath;
            }
            //
            //设为隐藏目录
            //
			domain.AssemblyResolve+= delegate(object sender, ResolveEventArgs args) 
			{

                string filePath = String.Concat(resolveFullDir, args.Name.Split(',')[0], ".dll");
				
				//File.WriteAllText(directory+DateTime.Now.Millisecond.ToString()+".txt",args.Name+"//"+filePath);

				if(!File.Exists(filePath))
				{
					return null;
				}
				else
				{
					return Assembly.Load(File.ReadAllBytes(filePath));
				}
			};
		}
    }
}
