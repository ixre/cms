using System.Runtime.CompilerServices;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Web
{
    /// <summary>
    /// Http主机
    /// </summary>
    public static class HttpHosting
    {
        private static ICompatibleHttpContext Instance;

        /// <summary>
        /// 获取当前上下文 
        /// </summary>
        public static ICompatibleHttpContext Context => Instance;

       
        /// <summary>
        /// 设置HttpContextAccessor
        /// </summary>
        /// <param name="accessor"></param>
        public static void ConfigureAccessor(IHttpContextAccessor accessor)
        {
            #if NETSTANDARD
            Instance = new HttpContextImpl(accessor);
            #endif
        }

        public static void Configure(ICompatibleHttpContext context)
        {
            Instance = context;
        }
    }
}