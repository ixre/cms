/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2013/12/12
 * Time: 7:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;

namespace JR.Stand.Core.Framework.Security
{
    /// <summary>
    /// Description of Md5Crypto.
    /// </summary>
    class Md5CryptoImpl
    {
        /// <summary>
        /// 用md5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeMD5(String str)
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
        public static string Encode16MD5(String str)
        {
            //取32位的中间部分
            var md5 = System.Security.Cryptography.MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(str)), 4, 8).Replace("-", String.Empty);
        }




    
    }
}