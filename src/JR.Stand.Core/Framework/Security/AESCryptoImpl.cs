using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JR.Stand.Core.Framework.Security
{  
    
    /// <summary>
    /// AES对称加密算法
    /// </summary>
    public class AesCryptoImpl
    {
        /// <summary>
        /// 默认TOKEN,16位
        /// </summary>
        private const string DEFAULT_TOKEN = "aescodefork3fnet";

        //默认密钥向量   
        private static byte[] ivKeys = { 0x12,0x34, 0x56, 0x78, 0x90, 0xAB,
            0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78,
            0x90, 0xAB, 0xCD, 0xEF };

        #region 加密和解密逻辑


        /// <summary>  
        /// AES加密 
        /// </summary>  
        /// <param name="data">要加密的数据</param>  
        /// <param name="token">密钥(默认128位,16个字母)</param>  
        /// <param name="encoding">编码</param>
        /// <returns>返回加密后的密文字节数组</returns>  
        public static byte[] Encrypt(byte[] data, string token)
        {
            //分组加密算法  
            SymmetricAlgorithm des = Rijndael.Create();

            byte[] _keys = Encoding.UTF8.GetBytes(token);

            //设置密钥及密钥向量  
            des.Key = _keys;
            des.IV = ivKeys;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();//得到加密后的字节数组  
            cs.Dispose();
            ms.Dispose();
            return cipherBytes;
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="decData">密文字节数组</param>  
        /// <param name="token">密钥</param>  
        /// <returns>返回解密后的字符串</returns>  
        public static byte[] Decrypt(byte[] decData, string token)
        {
            // if (decData.Length != ivKeys.Length)
            //{
            //    throw new CryptographicException("keys length is not valid!");
            //}

            SymmetricAlgorithm des = Rijndael.Create();
            des.Key = Encoding.UTF8.GetBytes(token);
            des.IV = ivKeys;
            byte[] decryptBytes = new byte[decData.Length];
            MemoryStream ms = new MemoryStream(decData);
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(decryptBytes, 0, decryptBytes.Length);
            cs.Dispose();
            ms.Dispose();
            return decryptBytes;
        }

        /// <summary>
        /// 加密到Base64字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string EncryptToBase64(byte[] data, string token)
        {
            byte[] encData = Encrypt(data, token);
            return Convert.ToBase64String(encData);
        }

        /// <summary>
        /// 从Base64字符串中解密
        /// </summary>
        /// <param name="encyptStr"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static byte[] DecryptFromBase64(string encyptStr, string token)
        {
            byte[] data = Convert.FromBase64String(encyptStr);
            return Decrypt(data, token);
        }


        public static string EncryptToBase64(string str, string token)
        {
            return EncryptToBase64(Encoding.UTF8.GetBytes(str), token);
        }

        #endregion




        #region 无Token重载


        public static string Decrypt(string encyptBase64Str, string token)
        {
            return Encoding.UTF8.GetString(DecryptFromBase64(encyptBase64Str, token));
        }

        public static byte[] Encrypt(byte[] data)
        {
            return Encrypt(data, DEFAULT_TOKEN);
        }

        public static byte[] Decrypt(byte[] decData)
        {
            return Decrypt(decData, DEFAULT_TOKEN);
        }

        public static string EncryptToBase64(byte[] data)
        {
            return EncryptToBase64(data, DEFAULT_TOKEN);
        }

        public static byte[] DecryptFromBase64(string encyptStr)
        {
            return DecryptFromBase64(encyptStr, DEFAULT_TOKEN);
        }

        public static string EncryptToBase64(string str)
        {
            return EncryptToBase64(str, DEFAULT_TOKEN);
        }

        public static string Decrypt(string encyptBase64Str)
        {
            return Decrypt(encyptBase64Str, DEFAULT_TOKEN);
        }
        #endregion
    }
}