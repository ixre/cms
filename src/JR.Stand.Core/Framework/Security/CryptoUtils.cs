using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JR.Stand.Core.Framework.Security
{
    /// <summary>
    /// 加密工具类
    /// </summary>
    public static class CryptoUtils
    {

        /// <summary>
        /// 基于Md5的自定义加密字符串方法：输入一个字符串，返回一个由32个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string MD5(this string str)
        {
            return HashString("md5", Encoding.UTF8.GetBytes(str));
        }


        /// <summary>
        /// 转换为16位的md5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD516(this String str)
        {
            return MD5(str).Substring(8, 24);
        }
        /// <summary>
        /// 基于Sha1的自定义加密字符串方法：输入一个字符串，返回一个由40个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string Sha1(this string str)
        {
            return HashString("sha1", Encoding.UTF8.GetBytes(str));
        }


        /// <summary>
        /// 计算Hash值
        /// </summary>
        /// <param name="algorithm">算法</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static string HashString(string algorithm, byte[] data)
        {
            var hashBytes = GetAlgorithm(algorithm).ComputeHash(data);
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 计算哈希值
        /// </summary>
        /// <param name="stream">要计算哈希值的 Stream</param>
        /// <param name="algorithm">算法:sha1,md5</param>
        /// <returns>哈希值字节数组</returns>
        private static byte[] HashData(string algorithm, Stream stream)
        {
            return GetAlgorithm(algorithm).ComputeHash(stream);
        }


        private static HashAlgorithm GetAlgorithm(string algorithm)
        {
            if (!String.IsNullOrEmpty(algorithm))
            {
                switch (algorithm.ToLower())
                {
                    case "sha1": return SHA1.Create();
                    case "md5": return System.Security.Cryptography.MD5.Create();
                }
            }
            throw new ArgumentNullException("不支持的算法:" + algorithm ?? "");
        }

        /// <summary>
        /// 字节数组转换为16进制表示的字符串
        /// </summary>
        public static string ByteHex(byte[] buf)
        {
            return BitConverter.ToString(buf).Replace("-", "");
        }
    }

}
