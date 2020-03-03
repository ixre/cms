//
// Copyright (C) 2007-2008 TO2.NET,All rights reseved.
// 
// Project: jr.Cms.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using JR.Cms.WebImpl.Util;

namespace JR.Cms.WebImpl.WebManager.Handle
{
    /// <summary>
    /// 上传
    /// </summary>
    public class UploadC:BasePage
    {

        /// <summary>
        /// 上传图片
        /// </summary>
        public void UploadImage_POST()
        {
            string uploadfor = base.Request["for"];
            //string id = base.Request["upload.id"];
            String dir = UploadUtils.GetUploadDirPath(base.CurrentSite.SiteId, "image",true);
            string name = UploadUtils.GetUploadFileName(base.Request, uploadfor);
            UploadResultResponse(dir, name,false); 
        }

        private void UploadResultResponse(string dir, string name,bool autoName)
        {
            try
            {
                string file = new FileUpload(dir, name, autoName).Upload();
                Response.Write("{" + String.Format("\"url\":\"{0}\"", file) + "}");
            }
            catch (Exception ex)
            {
                Response.Write("{" + String.Format("\"error\":\"{0}\"", ex.Message) + "}");
            }
        }


        /// <summary>
        /// 上传分类缩略图
        /// </summary>
        public void UploadCatThumb_POST()
        {
            string uploadfor = base.Request["for"];
            //string id = base.Request["upload.id"];
            String dir = UploadUtils.GetUploadDirPath(base.CurrentSite.SiteId, "thumb_cat",false);
            string name = UploadUtils.GetUploadFileRawName(base.Request);
            UploadResultResponse(dir, name, true);

        }


        /// <summary>
        /// 上传文档缩略图
        /// </summary>
        public void UploadArchiveThumb_POST()
        {
            String dir = UploadUtils.GetUploadDirPath(base.CurrentSite.SiteId, "thumb_ar",true);
            string name = UploadUtils.GetUploadFileName(base.Request, "");
            UploadResultResponse(dir, name, true);
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        public void UploadPropertyFile_POST()
        {
            DateTime dt = DateTime.Now;
            String dir = UploadUtils.GetUploadDirPath(base.CurrentSite.SiteId, "prop",true);
            string name = UploadUtils.GetUploadFileName(base.Request, "");
            UploadResultResponse(dir, name, true);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public void UploadFile_POST()
        {
            string uploadfor = base.Request["for"];
            string id = base.Request["upload.id"];
            DateTime dt = DateTime.Now;
            String dir = UploadUtils.GetUploadDirPath(base.CurrentSite.SiteId, "file",true);
            string name = UploadUtils.GetUploadFileName(base.Request, "");
            UploadResultResponse(dir, name, false);
        }

        #region 文件上传至远程服务器
        /// <summary>
        /// 文件上传至远程服务器
        /// </summary>
        /// <param name="url">远程服务地址</param>
        /// <param name="postedFile">上传文件</param>
        /// <param name="parameters">POST参数</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="output">远程服务器响应字符串</param>
        public void HttpPostFile(string url, HttpPostedFile postedFile, Dictionary<string, object> parameters, CookieContainer cookieContainer, ref string output)
        {
            //1>创建请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //2>Cookie容器
            request.CookieContainer = cookieContainer;
            request.Method = "POST";
            request.Timeout = 20000;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.KeepAlive = true;

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");//分界线
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            request.ContentType = "multipart/form-data; boundary=" + boundary; ;//内容类型

            //3>表单数据模板
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            //4>读取流
            byte[] buffer = new byte[postedFile.ContentLength];
            postedFile.InputStream.Read(buffer, 0, buffer.Length);

            //5>写入请求流数据
            string strHeader = "Content-Disposition:application/x-www-form-urlencoded; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
            strHeader = string.Format(strHeader, "filedata", postedFile.FileName, postedFile.ContentType);
            //6>HTTP请求头
            byte[] byteHeader = ASCIIEncoding.ASCII.GetBytes(strHeader);
            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    //写入请求流
                    if (null != parameters)
                    {
                        foreach (KeyValuePair<string, object> item in parameters)
                        {
                            stream.Write(boundaryBytes, 0, boundaryBytes.Length);//写入分界线
                            byte[] formBytes = Encoding.UTF8.GetBytes(string.Format(formdataTemplate, item.Key, item.Value));
                            stream.Write(formBytes, 0, formBytes.Length);
                        }
                    }
                    //6.0>分界线============================================注意：缺少次步骤，可能导致远程服务器无法获取Request.Files集合
                    stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                    //6.1>请求头
                    stream.Write(byteHeader, 0, byteHeader.Length);
                    //6.2>把文件流写入请求流
                    stream.Write(buffer, 0, buffer.Length);
                    //6.3>写入分隔流
                    byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    stream.Write(trailer, 0, trailer.Length);
                    //6.4>关闭流
                    stream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    output = reader.ReadToEnd();
                }
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("上传文件时远程服务器发生异常！", ex);
            }
        }
        #endregion
        
    }
}
