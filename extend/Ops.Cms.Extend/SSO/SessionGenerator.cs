using System;
using System.Text;

namespace Ops.Cms.Extend.SSO
{
    internal class SessionGenerator
    {

        private static int[] charArray = new int[]{ 98, 100, 101, 104, 108, 120, 111, 107, 113 };

        /// <summary>
        /// 创建一个新的Key
        /// </summary>
        /// <returns></returns>
        public static string CreateKey()
        {
            StringBuilder sb = new StringBuilder();
            Random rd = new Random();
            int arrayLength = charArray.Length;

            int i = 0;
            do
            {
                ++i;
                sb.Append((char)charArray[rd.Next(0, arrayLength - 1)]);
            }
            while (i < Variables.SeesionKeyLength);

            return sb.ToString();
        }
    }
}
