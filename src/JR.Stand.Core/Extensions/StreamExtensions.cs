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
    }
}