
namespace Spc.Web.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Lang
    {
        private static string[] langs;
        internal static string defaultLang;
        public static string[] Langs { get { return langs; } }

        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="supportLangs"></param>
        public static void Set(string[] supportLangs)
        {
            langs = supportLangs;
            defaultLang = supportLangs[0];
        }
    }
}
