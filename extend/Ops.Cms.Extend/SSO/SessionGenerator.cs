using System;
using System.Text;
using Ops.Framework.Extensions;

namespace Ops.Cms.Extend.SSO
{
    internal class SessionGenerator
    {
        private readonly int[] _dict;
        private int length;

        public SessionGenerator(String seed,int length)
        {
            this.length = length;

            if (seed == null || seed.Trim().Length == 0)
            {
                throw new ArgumentException("seed");
            }

            int len = seed.Length;
            this._dict = new int[len];
            int i = 0;
            foreach (char c in seed)
            {
                this._dict[i++] = c;
            }
        }
        public SessionGenerator():this(null,5)
        {
        }
        /// <summary>
        /// 创建一个新的Key
        /// </summary>
        /// <returns></returns>
        public string CreateKey()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("{0:yyMMHHssfff}", DateTime.Now));
            Random rd = new Random();
            int arrayLength = this._dict.Length;

            int i = 0;
            do
            {
                ++i;
                sb.Append((char)_dict[rd.Next(0, arrayLength - 1)]);
            }
            while (i < this.length);

            return sb.ToString().Encode16MD5().ToLower();
        }
    }
}
