using System;
using System.Runtime.InteropServices;

namespace JR.Stand.Core
{
    public static class EnvUtil
    {
        public static string GetBaseDirectory()                                                                                                                                                                          
        {
            var ver = Environment.Version;
            if(ver.Major > 4)
            {
                return Environment.CurrentDirectory + "/";
            }
            // .net4 and below .net4
            return AppDomain.CurrentDomain.BaseDirectory;
            
        }


        /// <summary>
        /// 是否为指定的操作系统
        /// </summary>
        /// <returns></returns>
        public static bool IsOSPlatform(OSPlatform os)
        {
            return RuntimeInformation.IsOSPlatform(os);
        }
    }
}