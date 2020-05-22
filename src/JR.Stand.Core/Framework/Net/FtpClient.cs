/*
 * Copyright 2010 OPS.CC,All right reserved .
 * name     : ftpclient
 * author   : newmin
 * date     : 2010/12/13
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Framework.Net
{
    public class FtpClient
    {
        private FtpWebRequest request;
        private string ftp;

        /// <summary>
        /// 服务器
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// FTP端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 根目录
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// FTP请求
        /// </summary>
        public FtpWebRequest Request
        {
            get { return request; }
        }

        public FtpClient(string server, int port, string userName, string password, string rootPath)
        {
            if(port <= 0)
            {
                port = 21;
            }
            this.Server = server;
            this.Port = port;
            this.RootPath = rootPath ?? "/";
            this.UserName = userName;
            this.Password = password;
            ftp = "ftp://" + server + ":" + port + rootPath;
        }

        public FtpClient(string server, string userName, string password)
        {
            this.Server = server;
            this.Port = 21;
            this.RootPath = "/";
            this.UserName = userName;
            this.Password = password;
            ftp = "ftp://" + server + ":" + Port + RootPath;
        }

        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <returns>返回是否连接成功</returns>
        public bool Connection()
        {
            try
            {
                request = WebRequest.Create(ftp) as FtpWebRequest;
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(UserName, Password);
                FtpWebResponse fr = request.GetResponse() as FtpWebResponse;
                using (StreamReader sr = new StreamReader(fr.GetResponseStream()))
                {
                    if (!String.IsNullOrEmpty(sr.ReadToEnd())) return true;
                }
                return false;
            }
            catch
            {
                throw new Exception("FTP信息不正确!请检查FTP地址，端口和用户是否正确!");
            }
        }

        /// <summary>
        /// 是否存在文件或目录
        /// </summary>
        /// <param name="folderOrFileName"></param>
        /// <returns></returns>
        public bool Exists(string folderOrFileName)
        {
            request = WebRequest.Create(ftp) as FtpWebRequest;
            request.Credentials = new NetworkCredential(UserName, Password);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            using (StreamReader rd = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                /*
                dir = rd.ReadToEnd();
                System.Web.HttpContext.Current.Response.Write(dir + "<br />" + count.ToString());
                 */
                string line;
                while ((line = rd.ReadLine()) != null)
                {
                    GroupCollection gc = new Regex("[^\\s]*$").Match(line).Groups;
                    if (gc.Count != 1) return false;
                    if (gc[0].Value == folderOrFileName) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="floderName"></param>
        public void CreateDirectory(string floderName)
        {
            request = WebRequest.Create(ftp + floderName) as FtpWebRequest;
            request.Credentials = new NetworkCredential(UserName, Password);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.GetResponse();
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileStream"></param>
        public void PostFile(string filePath, Stream fileStream)
        {
            const int bufferLength = 1;
            byte[] buffer = new byte[bufferLength];
            request = WebRequest.Create(ftp + filePath) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(UserName, Password);
            request.KeepAlive = false;
            request.UseBinary = true;
            Stream requestStream = request.GetRequestStream();
            int readBytes = 0;
            do
            {
                readBytes = fileStream.Read(buffer, 0, bufferLength);
                if (readBytes != 0)
                    requestStream.Write(buffer, 0, bufferLength);
            } while (readBytes != 0);

            requestStream.Dispose();
            fileStream.Dispose();
        }

        /// <summary>
        /// Obtains a simple file list with the filenames on the FTP server using a wildcard starting at the current remote FTP folder
        /// </summary>
        /// <param name="fileSpec">A wildcard to specify the filenames to be searched for and listed. Example: "subfolder/*.txt"</param>
        /// <returns>A list with the filenames found</returns>
        public IList<String> GetFileList(string fileSpec)
        {
            if (fileSpec == null) fileSpec = "";
            if (fileSpec.Length > 0 && !fileSpec.StartsWith("/"))
            {
                fileSpec = "/" + fileSpec;
            }
            var ret = new List<String>();
            var req = (FtpWebRequest)WebRequest.Create(ftp+fileSpec);
            req.Proxy = null;
            req.EnableSsl = false;
            req.UseBinary = true;
            req.Credentials = new NetworkCredential(UserName, Password);
            req.Method = WebRequestMethods.Ftp.ListDirectory;
            using (WebResponse resp = req.GetResponse())
            {
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        ret.Add(reader.ReadLine());
                    };
                };
            }
            return ret;
        }


    }
}