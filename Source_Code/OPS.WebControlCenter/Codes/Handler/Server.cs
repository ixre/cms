//
// Server Auto Dect  服务器检测
// Copyright 2011 @ OPS
// Newmin @ 2011/03/02
// 
namespace OPSoft.WebControlCenter.Handler
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Text;
    using System.Xml;
    using OPS.Web;

    [WebExecuteable]
    public class Server
    {
        /// <summary>
        /// 服务器地址词典
        /// </summary>
        internal static IDictionary<string,string> ServerCollection;

        /// <summary>
        /// 检测服务器是否更新
        /// </summary>
        private void ReadServer()
        {
            object obj=HttpRuntime.Cache["ctsys_server"];
            if (obj == null)
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "config/domain.conf";
                ServerCollection =new Dictionary<string,string>();
                XmlDocument xd = new XmlDocument();
                xd.Load(filePath);
                //添加静态服务器地址
                XmlNode xn = xd.SelectSingleNode("/domain/staticServer");
                ServerCollection.Add("staticServer", xn.Attributes["address"].Value);
                HttpRuntime.Cache.Insert("ctsys_server", 1, new System.Web.Caching.CacheDependency(filePath));
            }
        }
        [Valid]
        [Post(AllowRefreshMillliSecond = 2000)]
        public string GetStaticServer(string code)
        {
            ReadServer();
            return ServerCollection["staticServer"];
        }
    }
}