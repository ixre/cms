//
// Copyright (C) 2007-2008 S1N1.COM,All rights reserved.
// 
// Project: OPS
// FileName : LogFile.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/11/1 20:23:38
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

using System;
using System.IO;
using System.Text;

namespace JR.Stand.Core.Framework
{
    public enum LoggerLevel
    {
        Normal = 1,
        Debug = 2,
        Error = 3,
        Warning = 4,
    }

    /// <summary>
    /// 以文件存储的日志记录器
    /// </summary>
    public class FileLogger
    {
        private readonly string _filePath;
        private readonly bool _printPrefix;
        private Encoding _encoding;

        //种子，用于判断
        public int Seed { get; set; }

        //编码
        public Encoding FileEncoding
        {
            get => this._encoding ?? (this._encoding = Encoding.UTF8);
            set => this._encoding = value;
        }

        public FileLogger(string filePath, bool printPrefix = false)
        {
            this._printPrefix = printPrefix;
            this._filePath = filePath;
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
        }

        /// <summary>
        /// 填充内容
        /// </summary>
        /// <param name="bytes"></param>
        /* [Obsolete]
        public void Append(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);

            if (fs.CanWrite)
            {
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            fs.Dispose();

        }
        */
        private void Print(Byte[] bytes)
        {
            FileStream fs = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read);

            if (fs.CanWrite)
            {
                if (this._printPrefix)
                {
                    byte[] data = this.FileEncoding.GetBytes(string.Format("{0:yyyy-MM-dd HH:mm:ss ", DateTime.Now));
                    fs.Write(data, 0, data.Length);
                }
                fs.Write(bytes, 0, bytes.Length);
            }
            fs.Dispose();
        }

        public void Println(LoggerLevel level,string text)
        {
            var prefix = this.GetPrefix(level);
            this.Print(this.FileEncoding.GetBytes(prefix+text + Environment.NewLine));
        }

        private string GetPrefix(LoggerLevel level)
        {
            switch (level)
            {
                case LoggerLevel.Debug: return "[ Debug]";
                case LoggerLevel.Error: return "[ Error]";
                case LoggerLevel.Normal: return "";
                case LoggerLevel.Warning: return "[ Warning]";
            }

            return "";
        }

        public void Printf(string format, params object[] data)
        {
            this.Print(this.FileEncoding.GetBytes(string.Format(format, data)));
        }

        /// <summary>
        /// 清空内容
        /// </summary>
        /// <param name="text"></param>
        public void Truncate()
        {
            FileStream fs = new FileStream(_filePath, FileMode.Truncate, FileAccess.Write);
            fs.Dispose();
        }
    }
}