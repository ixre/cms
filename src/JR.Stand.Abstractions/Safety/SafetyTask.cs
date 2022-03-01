using System.Threading.Tasks;
using System;
namespace JR.Stand.Abstracts.Safety
{
    /// <summary>
    /// 
    /// </summary>
    public static class SafetyTask
    {
        /// <summary>
        /// 安全的返回Task,在.NET46以下返回null
        /// </summary>
        public static Task CompletedTask
        {
            get{
                if (Environment.Version.Major <= 4)
                {
//#if NET40 || NET451 || NET452 || NETFRAMEWORK
                try{return Task.CompletedTask;}catch{return null;}
//#endif
                }
                return Task.CompletedTask;
            }
        }
    }
}