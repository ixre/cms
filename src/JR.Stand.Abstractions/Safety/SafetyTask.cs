using System.Threading.Tasks;

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
            get
            {
#if NET40 || NET451 || NET452 || NETFRAMEWORK
                try{return Task.CompletedTask;}catch{return null;}
#endif
                return Task.CompletedTask;
            }
        }
    }
}