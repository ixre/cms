using System;
using System.Security.Cryptography;

namespace JR.Stand.Core.Framework.Security
{

    /// <summary>
    /// RAS工具
    /// </summary>
    public class RSA
    {
        public struct KeyPair{
            private  string PrivateKey;
            private  string PublicKey;

            public KeyPair(String privateKey, String publicKey)
            {
                this.PrivateKey = privateKey;
                this.PublicKey = publicKey;
            }
        } 
        /// <summary>
        /// 生成公钥与私钥方法
        /// </summary>
        /// <returns></returns>
        public static KeyPair CreateKey()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                //C# 私钥
                String privateKey = rsa.ToXmlString(true);
                //C# 公钥
                String publicKey = rsa.ToXmlString(false);
                return new KeyPair(privateKey, publicKey);
            }
        }
    }
}