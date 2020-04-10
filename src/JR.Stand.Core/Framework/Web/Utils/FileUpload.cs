/*
 * 文件上传
 * Copyright 2012 OPS,All right reserved!
 * Newmin(ops.cc)  @  2012-09-29 07:09
 * 
 */

using System;
using System.IO;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Framework.Web.UI
{
    /// <summary>
    /// 文件上传工具, 不兼容AspNet
    /// </summary>
    public class FileUpload
    {
        /// <summary>
        /// 保存文件夹
        /// </summary>
        private readonly string _saveAbsoluteDir;

        /// <summary>
        /// 文件名
        /// </summary>
        private readonly string _fileName;
        private readonly bool _autoRename;
        private UploadFileInfo _fileInfo;

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="saveAbsoluteDir">保存目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="autoRename">如果文件名存在，是否自动命名</param>
        public FileUpload(string saveAbsoluteDir, string fileName,bool autoRename)
        {
            this._saveAbsoluteDir = saveAbsoluteDir;
            this._fileName = fileName;
            this._autoRename = autoRename;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <returns>异步则返回进程ID，同步返回上传文件的路径</returns>
        public string Upload()
        {
            HttpRequest request = (HttpHosting.Context.RawContext() as HttpContext).Request;
            String baseDir = EnvUtil.GetBaseDirectory();
            string[] process = request.Form["upload_process"][0].Split('|');
            string processID = process[1], field = process[0];

            var postedFile = request.Form.Files[field];
            if (postedFile == null)
            {
                return null;
            }
            string fileExt = postedFile.FileName.Substring(postedFile.
                FileName.LastIndexOf('.') + 1); //扩展名
            InitUplDirectory(baseDir, this._saveAbsoluteDir);
            this._fileInfo = new UploadFileInfo
            {
                Id = processID,
                ContentLength = postedFile.Length,
                FilePath = $"{this._saveAbsoluteDir}{this._fileName}.{fileExt}"
            };
            String targetPath = baseDir + this._fileInfo.FilePath;
            if (!this._autoRename && File.Exists(targetPath))
            {
                throw new IOException("文件已存在");
            }
            // 自动将重复的名称命名
            int i = 0;
            while (File.Exists(targetPath))
            {
                i++;
                this._fileInfo.FilePath = $"{this._saveAbsoluteDir}{this._fileName}_{i.ToString()}.{fileExt}";
                targetPath = baseDir + this._fileInfo.FilePath;
            }
            this.SaveStream(postedFile.OpenReadStream(), targetPath);
            return _fileInfo.FilePath;
        }

        private static void InitUplDirectory(String baseDir, String absDir)
        {
            //如果文件夹不存在，则创建文件夹
            String dir = baseDir + absDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir).Create();
            }
        }

        private void SaveStream(Stream stream, string path)
        {
            const int bufferSize = 100; //缓冲区大小
            byte[] buffer = new byte[bufferSize]; //缓冲区

            int bytes; //从流中读取的值
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                while (true)
                {
                    bytes =stream.Read(buffer, 0, bufferSize);
                    if (bytes == 0)
                    {
                        break;
                    }
                    fs.Write(buffer, 0, bytes);
                }
                fs.Flush();
                fs.Close();
            }
        }
    }
}