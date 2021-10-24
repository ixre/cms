using System.IO;
using System.Text;
using JR.Stand.Core.Framework.Security;

namespace JR.Stand.Core.Framework.IO
{
    /// <summary>
    /// 文件加密
    /// </summary>
    public static class FileEncoder
    {
        /// <summary>
        /// 是否已经加密
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="header">文件头</param>
        /// <returns></returns>
        public static bool IsEncoded(string filePath,string header)
        {
            int headerSize = header.Length;
            char[] buffers = new char[headerSize];
            StreamReader sr = new StreamReader(filePath);
            int i = sr.Read(buffers, 0, buffers.Length);
            sr.Dispose();
            return i == headerSize && new string(buffers) == header;
        }

        /// <summary>
        /// 加密并保存
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="targetFilePath"></param>
        /// <param name="header"></param>
        /// <param name="token"></param>
        public static byte[] EncodeFile(string filePath, string targetFilePath,string header,string token)
        {
            byte[] sourceData;
            byte[] encodedData;

            //读取原始数据
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            sourceData = new byte[fs.Length];
            fs.Read(sourceData, 0, (int)fs.Length);
            fs.Dispose();

            int headerStrLength = header.Length;
            byte[] headerBytes = Encoding.Default.GetBytes(header);


            byte[] tempData = AesCryptoImpl.Encrypt(sourceData, token);
            encodedData = new byte[tempData.Length + headerStrLength];

            //添加头信息
            for (var i = 0; i < headerStrLength; i++)
            {
                encodedData[i] = headerBytes[i];
            }

            for (int i = 0; i < tempData.Length; i++)
            {
                encodedData[headerStrLength + i] = tempData[i];
            }

            if (!string.IsNullOrEmpty(targetFilePath))
            {
                fs = new FileStream(targetFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(encodedData, 0, encodedData.Length);
                fs.Flush();
                fs.Dispose();
            }

            return encodedData;
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="targetFilePath"></param>
        /// <param name="header"></param>
        /// <param name="token"></param>
        public static byte[] DecodeFile(string filePath, string targetFilePath,string header,string token)
        {
            byte[] sourceData;
            byte[] decodedData;

            //读取原始数据
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            sourceData = new byte[fs.Length];
            fs.Read(sourceData, 0, (int)fs.Length);
            fs.Dispose();

            //移除头信息
            int headerStrLength = header.Length;

            byte[] tempData = new byte[sourceData.Length - headerStrLength];
            for (var i = 0; i < tempData.Length; i++)
            {
                tempData[i] = sourceData[i + headerStrLength];
            }

            //解码
            decodedData = AesCryptoImpl.Decrypt(tempData, token);

            if (!string.IsNullOrEmpty(targetFilePath))
            {
                //保存文件
                fs = new FileStream(targetFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(decodedData, 0, decodedData.Length);
                fs.Flush();
                fs.Dispose();
            }

            return decodedData;
        }
    }
}

