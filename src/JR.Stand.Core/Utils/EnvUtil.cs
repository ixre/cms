using System;

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
    }
}