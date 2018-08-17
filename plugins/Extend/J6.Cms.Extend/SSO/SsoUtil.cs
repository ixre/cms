using System;
using System.Text;

namespace T2.Cms.Extend.SSO
{
    public static class SsoUtil
    {
        public static string EncodeBase64(String str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        public static string DecodeBase64(String str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
    }
}
