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
            var resPath = filePath.Replace("\\", "/");
            // Mac和Linux下,使用mono运行. 资源不需要前缀,如存放: Static目录下的1.txt .
            // Linux下为: Namespace.1.txt  而Windows下是: Namespace.Static.1.txt
            if (EnvUtil.IsOSPlatform(OSPlatform.Linux) || EnvUtil.IsOSPlatform(OSPlatform.OSX))
            {
                resPath = resPath.Substring(resPath.LastIndexOf("/") + 1);
            }
            resPath = resPath.Replace("/", ".");
            var resFullPath = assembly.FullName.Split(',')[0] + "." + resPath;
            // var str = assembly.FullName + " | ";
            // foreach (var v in  assembly.GetManifestResourceNames())
            // {
            //     str += v + "|";
            // }
            // throw  new Exception(str);
            //
            
            Stream fs = assembly.GetManifestResourceStream(resFullPath);
            if (fs == null)
            {
                throw new Exception($"资源不存在{resFullPath}");
            }
            return fs;
        }
    }
}