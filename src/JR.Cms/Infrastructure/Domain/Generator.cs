using System;
using System.Security.Cryptography;
using System.Text;

namespace JR.Cms.Infrastructure.Domain
{
    public static class Generator
    {
        public static String Offset = "@cms.z3q.net";
        public static string Md5Pwd(string password, string offset)
        {
            return (password + offset).MD516().EncodeMD5();
        }

        public static string Sha1Pwd(string password, string offset)
        {
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();

            //将mystr转换成byte[]
            byte[] dataToHash = Encoding.UTF8.GetBytes(password + offset);

            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);

            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");

            return hash.ToLower();
        }

        public static bool CompareUserPwd(string password, string encodedPassword)
        {
            if (Md5Pwd(password, null) == encodedPassword)
            {
                return true;
            }
            return Sha1Pwd(password, Offset) == encodedPassword;
        }
    }
}
