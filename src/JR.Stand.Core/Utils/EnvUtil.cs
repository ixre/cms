using System;
using System.Runtime.InteropServices;

namespace JR.Stand.Core
{
    public static class EnvUtil
    {
        public static string GetBaseDirectory()
        {
#if NETFRAMEWORK
           return AppDomain.CurrentDomain.BaseDirectory;
#endif
            return Environment.CurrentDirectory + "/";
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