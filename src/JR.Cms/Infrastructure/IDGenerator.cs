using System;
using System.Text;

namespace JR.Cms.Infrastructure
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
            char[] words =
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };

            var rd = new Random();
            var sb = new StringBuilder(length);

            var max = words.Length;

            for (var i = 0; i < length; i++) sb.Append(words[rd.Next(0, max)]);

            return sb.ToString();
        }
    }
}