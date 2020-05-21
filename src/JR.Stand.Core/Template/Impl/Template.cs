//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:Template.cs
// Author:newmin
// Create:2011/06/13
//

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 模板实体
    /// </summary>
    public sealed class Template
    {
        public Template(string filePath, Options opt)
        {
            this.FilePath = filePath;
            this.Options = opt ?? new Options();
        }
        /// <summary>
        /// 模板路径
        /// </summary>
        public string FilePath { get;}

        
        /// <summary>
        /// 最近修改时间
        /// </summary>
        private long LastModify { get; set; }

        /// <summary>
        /// 模板注释(第一个html注释)
        /// </summary>
        public string Comment
        {
            get
            {
                //读取文件开头部分注释
                Regex reg = new Regex("<!--(.+?)-->", RegexOptions.Multiline);

                //保存内容到变量，避免重复读取
                string content;

                using (StreamReader sr = new StreamReader(FilePath))
                {
                    content = sr.ReadToEnd();
                }

                if (reg.IsMatch(content))
                {
                    return reg.Match(content).Groups[1].Value;
                }

                return String.Empty;
            }
        }

        private String _content;

        /// <summary>
        /// 模板内容
        /// </summary>
        public string GetContent()
        {
            if (this._content == null)
            {
                /*
                string content;
                IDataContainer dc = new HttpDataContainer();

                //从缓存中读取模板内容
                if (Config.EnabledCache)
                {
                    content = dc.GetTemplatePageCacheContent(this.Id);
                    if (content != null) return content;
                }

                if (String.IsNullOrEmpty(this.FilePath)) throw new ArgumentNullException("模板文件不存在!" + this.FilePath);
                */
                FileInfo fi = new FileInfo(this.FilePath);
                long lastWriteUnix = TemplateUtility.Unix(fi.LastWriteTime);
                if (this._content == null || lastWriteUnix > this.LastModify)
                {
                    // 读取内容并缓存
                    StreamReader sr = new StreamReader(this.FilePath);
                    string content = sr.ReadToEnd();
                    sr.Dispose();
                    // 替换注释
                    content = Regex.Replace(content, "<!--[^\\[][\\s\\S]*?-->", String.Empty);
                    // 读取模板里的部分视图
                    content = TemplateRegexUtility.IncludeFileRegex.Replace(content, m =>
                    {
                        string path = m.Groups[1].Value;
                        string tplId =
                            TemplateUtility.GetPartialTemplateId(path, this.FilePath, out var partialFilePath);
                        return m.Value.Replace(path, tplId + "@" + partialFilePath);
                    });
                    // 缓存内容
                    this.LastModify = lastWriteUnix;
                    this._content = content;
                }

                // 替换系统标签
                this._content = TemplateRegexUtility.Replace(_content, m => TemplateCache.Tags[m.Groups[1].Value]);
                //压缩模板代码
                if (this.Options.EnabledCompress)
                {
                    this._content = TemplateUtility.CompressHtml(_content);
                }

                if (!this.Options.EnabledCache)
                {
                    var swp = this._content;
                    this._content = null;
                    return swp;
                }
            }
            return this._content;
        }

        private Options Options { get;}
    }
}