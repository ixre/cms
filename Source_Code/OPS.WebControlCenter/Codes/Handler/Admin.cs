using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using OPS.Web;

namespace OPSoft.WebControlCenter.Handler
{
    [WebExecuteable]
    public class Admin
    {
        /// <summary>
        /// 创建码
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        [Get]
        public string GenericKey(string domain)
        {
			
            //(域名+_OPS).MD5然后再Base64
            if (String.IsNullOrEmpty(domain)) return "";
			
			string key=(domain+"_OPS").EncodeMD5();
			
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }
    }
}