//
// Copyright (C) 2007-2008 Z3Q.NET,All rights reseved.
// 
// Project: jr.Cms.Manager
// FileName : File.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/17 18:10:55
// Description :
//
// Get infromation of this software,please visit our site http://k3f.net/cms
//
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using T2.Cms.Conf;
using T2.Cms.Resource;
using T2.Cms.WebManager;
using JR.DevFw.Framework.Web.Cache;
using T2.Cms.Web.Resource;
using T2.Cms.Web.Resource.WebManager;


// http://bbs.csdn.net/topics/300217787 #17
namespace T2.Cms.Web.WebManager.Handle
{
    public class ResourceC
    {

        internal static IDictionary<string, string> fileMImE;

        static ResourceC()
        {
            fileMImE = new Dictionary<string, string>();
            fileMImE.Add("style", "text/css");
            fileMImE.Add("manage_js_min", "text/javascript");   //bWFuYWdlX2pzX21pbg==
            fileMImE.Add("ajax_loader", "image/gif");
            fileMImE.Add("menu_01", "image/gif");
            fileMImE.Add("menu_01_bg", "image/gif");
            //fileMImE.Add("menu_01_current", "image/gif");
            fileMImE.Add("load_process", "image/gif");

            fileMImE.Add("btn_spirites", "text/css");       //YnRuX3NwaXJpdGVz
            fileMImE.Add("btn_spirites_pic", "image/png");  //YnRuX3NwaXJpdGVzX3BpYw==
            fileMImE.Add("icon_trans", "image/png");        //aWNvbl90cmFucw==
            fileMImE.Add("sys_bg", "image/gif");            //c3lzX2Jn
            fileMImE.Add("ui_component", "text/javascript");	//dWlfY29tcG9uZW50
        }

        /// <summary>
        /// 管理后台调用的脚本
        /// </summary>
        public void Read_GET()
        {
            HttpContext context = HttpContext.Current;
            /*
            MemoryStream ms = new MemoryStream();
            WebManagerResource.loading_process.Save(ms, ImageFormat.Gif);
            string x = Convert.ToBase64String(ms.ToArray());
            ms.Dispose();
            throw new Exception(x);
			*/

           //String resName =Encoding.UTF8.GetString(Convert.FromBase64String(context.Request["res"]));
           //context.Response.Write(resName+"/"+Convert.ToBase64String(Encoding.UTF8.GetBytes("manage_js_min"))); return;
            

            /* To Base64 */
            //byte[] data2=Encoding.UTF8.GetBytes("sys_tab");
            //context.Response.Write(Convert.ToBase64String(data2, 0, data2.Length)); return;


            const int maxAge = 31536000;

            DateTime sinceTime;
            DateTime nowTime=DateTime.Now.ToUniversalTime();

            string sinceModified = context.Request.Headers.Get("If-Modified-Since");
            DateTime.TryParse(sinceModified, out sinceTime);
            sinceTime=sinceTime.ToUniversalTime();

            if (!String.IsNullOrEmpty(sinceModified) && (nowTime - sinceTime).TotalSeconds < maxAge)
            {
                context.Response.StatusCode = 304;
                context.Response.Status = "304 Not Modified";
                return;
            }
            else
            {
                if (Settings.Opti_SupportGZip)
                {
                    context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
                    context.Response.AddHeader("Content-Encoding", "gzip");
                }

                context.Response.AddHeader("Cache-Control", "max-age=" + maxAge.ToString());
                context.Response.AddHeader("Last-Modified", nowTime.ToString("r"));
                context.Response.AddHeader("Expires", DateTime.Now.AddYears(1).ToString("r"));
                context.Response.AddHeader("ETag", "\"" + DateTime.Now.Ticks + "\"");

                /*
                    response.Cache.SetETag(lastModified.Ticks.ToString());
     response.Cache.SetLastModified(lastModified);
     //public 以指定响应能由客户端和共享（代理）缓存进行缓存。
     response.Cache.SetCacheability(HttpCacheability.Public);
    //是允许文档在被视为陈旧之前存在的最长绝对时间。
    response.Cache.SetMaxAge(new TimeSpan(7, 0, 0, 0));
    //将缓存过期从绝对时间设置为可调时间
    response.Cache.SetSlidingExpiration(true);
                */

                string filename = Encoding.UTF8.GetString(Convert.FromBase64String(context.Request["res"]));

                //获取文件MImE类型
                string mime = fileMImE.ContainsKey(filename) ? fileMImE[filename] : "text/plain";

                if (Regex.IsMatch(mime, "^image\\/(jpg|png|gif)$", RegexOptions.IgnoreCase))
                {
                    //ajax_loading图片
                    if (filename == "ajax_loader")
                    {
                        //const string data = "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";
                        const string data = "R0lGODlhEAAQAPIAAP///wBg/8LY/kKJ/gBg/2Kc/oKw/pK6/iH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6ImKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==";
                        HttpContext.Current.Response.BinaryWrite(Convert.FromBase64String(data));
                    }
                    else if (filename == "load_process")
                    {
                        const string data="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";
                        HttpContext.Current.Response.BinaryWrite(Convert.FromBase64String(data));
                    }
                    else
                    {

                        #region 管理后台LOGO
                        if (filename == "cms_top_logo")
                        {
                            FileInfo file = new FileInfo(String.Format("{0}{1}local/logo.gif", AppDomain.CurrentDomain.BaseDirectory,CmsVariables.FRAMEWORK_PATH));
                            if (file.Exists)
                            {
                                using (Bitmap bit = new Bitmap(file.FullName))
                                {
                                    MemoryStream stream = new MemoryStream();
                                    bit.Save(stream, ImageFormat.Gif);
                                    bit.Dispose();
                                    context.Response.BinaryWrite(stream.ToArray());
                                    stream.Dispose();
                                }
                                return;
                            }
                        }
                        #endregion

                        using (Bitmap bit = WebManagerResource.ResourceManager.GetObject(filename) as Bitmap)
                        {
                            MemoryStream stream = new MemoryStream();
                            bit.Save(stream, ImageFormat.Gif);
                            bit.Dispose();
                            context.Response.BinaryWrite(stream.ToArray());
                            stream.Dispose();
                        }
                    }
                }
                else
                {
                     if (filename == "style")
                    {
                        context.Response.Write(
                            ResourceUtility.CompressHtml(
                            ResourceMap.GetPageContent(ManagementPage.Css_Style)
                            ));
                    }
                     else
                     {
                         context.Response.Write(ResourceUtility.CompressHtml(WebManagerResource.ResourceManager.GetString(filename)));
                     }
                }

                HttpContext.Current.Response.ContentType = mime;
               // HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=cms");

            }
        }
        
        /// <summary>
        /// 合并代码
        /// </summary>
        public void Combine_GET()
        {
        	HttpContext context=HttpContext.Current;
        	
        	CacheUtil.Output(context.Response,int.MaxValue,() =>
                {
                    string path = context.Request["loc"]??"";
                    string type = context.Request["res_combine"] ?? "js";

                    string[] files = path.Split(',');
                    string appDir = Cms.PyhicPath;

                    const bool compress = true;

                    foreach (string file in files)
                    {
                        if (System.IO.File.Exists(appDir + file))
                        {
                            if (compress && (type == "js" || type == "css"))
                            {
                                context.Response.Write(BasePage.CompressHtml(System.IO.File.ReadAllText(appDir + file)));
                            }
                            else
                            {
                                context.Response.BinaryWrite(System.IO.File.ReadAllBytes(appDir + file));
                            }
                        }
                    }

                    switch (type.ToLower())
                    {
                        case "js":context.Response.ContentType="text/javascript";
                            break;
                        case "css": context.Response.ContentType="text/css";
                            break;
                        case "img": context.Response.ContentType="image/jpeg";
                            break;
                        default:
                            context.Response.ContentType = "application/oct-stream";
                            break;
                    }

                    return "";
                });
        }
    }
}
