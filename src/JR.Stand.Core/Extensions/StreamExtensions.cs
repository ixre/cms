using System;
using System.IO;

namespace JR.Stand.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 将流拷贝到其他流中(.net 4中自带，仅针对.net 4版本以下)
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="destination"></param>
        public static void CopyTo(this Stream sourceStream, Stream destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (!destination.CanRead && !destination.CanWrite)
            {
                //throw new ObjectDisposedException(null, Environment("ObjectDisposed_StreamClosed"));
            }

            if (!destination.CanRead && !destination.CanWrite)
            {
                //throw new ObjectDisposedException("destination", Environment.GetResourceString("ObjectDisposed_StreamClosed"));
            }

            if (!destination.CanRead)
            {
                //throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnreadableStream"));
            }

            if (!destination.CanWrite)
            {
                //throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnwritableStream"));
            }

            const int bufferSize = 4096;

            byte[] array = new byte[bufferSize];
            int count;
            while ((count = sourceStream.Read(array, 0, array.Length)) != 0)
            {
                destination.Write(array, 0, count);
            }
        }


        /// <summary>
        /// 读取流的所有字节
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            long originalPosition = 0;
            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }
            try
            {
                byte[] readBuffer = new byte[4096];
                int totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;
                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte) nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }
                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        /// <summary>
        /// 读取流的所有文本
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadToEnd(this Stream stream)
        { 
            StreamReader rd = new StreamReader(stream);
            string s = rd.ReadToEnd();
            rd.Close();
            return s;
        }
    }
}