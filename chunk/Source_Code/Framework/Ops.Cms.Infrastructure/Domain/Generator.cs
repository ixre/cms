using System;
using AtNet.DevFw.Framework.Extensions;

namespace AtNet.Cms.Infrastructure.Domain
{
    public static class Generator
    {
        public static string Md5Pwd(string password, string offset)
        {
            return (password + offset).Encode16MD5().EncodeMD5();
        }
    }
}
