using System;
using System.Text;

namespace JR.Stand.Core.Template.Impl
{
    internal class MD5
    {
        /// <summary>
        /// 用md5加密
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string Encode(String str)
        {
            StringBuilder sb = new StringBuilder();
            var md5 = System.Security.Cryptography.MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            foreach (byte b in s)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeTo16(String str)
        {
            //取32位的中间部分
            var md5 = System.Security.Cryptography.MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(str)), 4, 8)
                .Replace("-", String.Empty).ToLower();
        }
    }
}