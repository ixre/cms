using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace JR.Stand.Core.Framework.Security
{

    /// <summary>
    /// RAS工具
    /// </summary>
    public class RSA
    {
        /// <summary>
        /// KeyPair
        /// </summary>
        public struct KeyPair{
            public readonly string PrivateKey;
            public readonly string PublicKey;

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

        /// <summary>
        /// 生成公钥与私钥方法
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static KeyPair GenRSAKeyPair(string name)
        {
            var generator = new RsaKeyPairGenerator();
            var seed = Encoding.UTF8.GetBytes(name);
            var secureRandom = new SecureRandom();
            secureRandom.SetSeed(seed);
            generator.Init(new KeyGenerationParameters(secureRandom, 4096));
            var pair = generator.GenerateKeyPair();
            var twPrivate = new StringWriter();

            PemWriter pwPrivate = new PemWriter(twPrivate);
            pwPrivate.WriteObject(pair.Private);
            pwPrivate.Writer.Flush();
            var privateKey = twPrivate.ToString();

            var twPublic = new StringWriter();
            PemWriter pwPublic = new PemWriter(twPublic);
            pwPublic.WriteObject(pair.Public);
            pwPublic.Writer.Flush();
            var publicKey = twPublic.ToString();

            return new KeyPair(privateKey, publicKey);
        }
    }
}