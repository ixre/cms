using System;
using System.Security.Cryptography;
using System.Text;
using JR.Stand.Core.Framework.Extensions;
using JR.Stand.Core.Framework.Security;

namespace JR.Cms.Infrastructure.Domain
{
    public static class Generator
    {
        public static string Salt = "cms#56x.net";

        public static string Md5Pwd(string password, string offset)
        {
            return (password + offset).MD516().Md5();
        }

        /// <summary>
        /// 创建用户密码,默认密码123456的sha1为:a193813b04af48828f08d8853d15cb029642651c
        /// </summary>
        /// <param name="pwd">md5加密后的密码</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string CreateUserPwd(string pwd)
        {
            if (pwd == null || pwd.Length != 32) throw new ArgumentException("不是有效的密码");
            return Sha1Pwd(pwd, Salt);
        }

        public static string Sha1Pwd(string password, string salt)
        {
            // 建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            // 将mystr转换成byte[]
            var dataToHash = Encoding.UTF8.GetBytes(password + salt);
            // Hash运算
            var dataHashed = sha.ComputeHash(dataToHash);
            // 将运算结果转换成string
            var hash = BitConverter.ToString(dataHashed).Replace("-", "");
            return hash.ToLower();
        }

        /// <summary>
        /// 比较用户密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="encodedPassword">加密后的密码</param>
        /// <returns></returns>
        public static bool CompareUserPwd(string password, string encodedPassword)
        {
            if (Md5Pwd(password, null) == encodedPassword) return true;
            return Sha1Pwd(password, Salt) == encodedPassword;
        }
    }
}