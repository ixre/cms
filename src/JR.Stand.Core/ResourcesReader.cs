using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using JR.Stand.Core.Extensions;

namespace JR.Stand.Core
{
    /// <summary>
    /// 内嵌资源读取器
    /// </summary>
    public static class ResourcesReader
    {
        public static string Read(Assembly assembly,string filePath)
        {
            var fs = ReadStream(assembly, filePath);
            return fs.ReadToEnd();
        }
        
        public static byte[] ReadBytes(Assembly assembly,string filePath)
        {
            var fs = ReadStream(assembly,filePath);
            return fs.ReadAllBytes();
        }

        private static Stream ReadStream(Assembly assembly,string filePath)
        {
            if (assembly == null) assembly = Assembly.GetExecutingAssembly();
            var ns = assembly.FullName.Split(',')[0];
            var path = filePath.Replace("\\", "/");
            var resPath = path.Replace("/", ".");
            // Mac和Linux下,使用mono运行. 资源不需要前缀,如存放: Static目录下的1.txt .
            // Linux下为: Namespace.1.txt  而Windows下是: Namespace.Static.1.txt
            Stream fs = assembly.GetManifestResourceStream(ns+"."+resPath);
            if (fs == null)
            {
                resPath = path.Substring(path.LastIndexOf("/") + 1);
                fs =  assembly.GetManifestResourceStream(ns+"."+resPath);
            }

            // var str = assembly.FullName + " | ";
            // foreach (var v in  assembly.GetManifestResourceNames())
            // {
            //     str += v + "|";
            // }
            // throw  new Exception(str);
            //
            if (fs == null)
            {
                throw new Exception($"资源不存在{filePath}");
            }
            return fs;
        }
    }
}