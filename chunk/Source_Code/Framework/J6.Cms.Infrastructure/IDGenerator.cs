using System;
using System.Text;

namespace J6.Cms.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public static class IdGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetNext(int length)
        {
            /*
            try
            {
                System.Threading.Thread.Sleep(100);
            }
            catch
            {
            }
             */
            char[] words = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            Random rd = new Random();
            StringBuilder sb = new StringBuilder(length);

            int max = words.Length;

            for (int i = 0; i < length; i++)
            {
                sb.Append(words[rd.Next(0, max)]);
            }

            return sb.ToString();
        }
    }
}
