/*
 * Copyright 2010 OPS,INC.All rights reseved!
 * Create by newmin at 2010/11/05 07:30
 * 
 * 压缩文件功能,将压缩的文件以日期命名并保存到同目录下
 */
namespace OPSite.WebHandler
{
    using System;
    using System.Web;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Reflection;
    using System.IO;
    using J6.Web;

    [WebExecuteable]
    public class OPSite
    {
        /// <summary>
        /// 显示OPS.OPSite版本号
        /// </summary>
        /// <param name="compareVersion">是否对比版本信息true/false</param>
        /// <returns></returns>
        public string Version()
        {
            Assembly ass = Assembly.GetAssembly(this.GetType());
            return "OPS.OPSite产品版本:<br />OPS组件核心:2.0.0.0<br />系统核心版本:2.0.0.0<br />官方地址:<a href=\"http://csite.j6.cc/get/\">http://csite.j6.cc</a>";
        }
        public string Product()
        {
            return "OPS.OPSite基于微软Asp.net mvc2.0及OPS.Core内核构建!";
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="filePath"></param>
        public void CompressionFile(string filePath)
        {
            FileInfo file=new FileInfo(filePath);
           // string  content=Regex.Replace(resource.GetObject("ops_js_lib").ToString(),"\r+\n+",String.Empty)
        }
    }
}