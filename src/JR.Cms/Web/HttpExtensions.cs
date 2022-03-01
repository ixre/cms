using System;
using System.Text;
using JR.Stand.Abstracts.Web;

namespace JR.Cms.Web
{
    /// <summary>
    /// HTTP扩展
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// 同步输出到Response
        /// </summary>
        /// <param name="rsp"></param>
        /// <param name="content"></param>
        public static void Write(this ICompatibleResponse rsp, String content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            rsp.Write(bytes,0,bytes.Length);
        }
    }
}