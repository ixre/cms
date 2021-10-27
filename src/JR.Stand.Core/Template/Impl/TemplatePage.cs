//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:TemplatePage.cs
// Author:newmin
// Create:2011/06/06
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 模板页
    /// </summary>
    public class TemplatePage
    {
        /// <summary>
        /// 模板初始化，替换数据之前发生的事件
        /// </summary>
        public event TemplateHandler<object> OnPreInit;

        /// <summary>
        /// 编译之前发生的事件
        /// </summary>
        public event BeforeCompileEvent OnBeforeCompile;
        
        /// <summary>
        /// 模板呈现之前发生的事件
        /// </summary>
        public event TemplateHandler<object> OnPreRender;


        private readonly string _templateId;
        private readonly IDataContainer _dc;

        /// <summary>
        /// 模板数据
        /// </summary>
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        private string _templateHtml;

        /// <summary>
        /// 
        /// </summary>
        public TemplatePage(IDataContainer container)
        {
            this._dc = container;
        }

        public TemplatePage(string templateId,IDataContainer container):this(container)
        {
            this._templateId = templateId.ToLower();
        }

        public TemplatePage(string templateId,IDataContainer container, object templateData)
            : this(templateId,container)
        {
            this.AddDataObject(templateData);
        }
        
        /// <summary>
        /// 模板内容
        /// </summary>
        public void SetTemplateContent(String content)
        {
            if (!String.IsNullOrEmpty(_templateId))
            {
                throw new ArgumentException("已经指定模板ID,将自动读取模板内容，无法在过程中设置模板的内容!", "TemplateContent");
            }
            _templateHtml = content;
        }

        /// <summary>
        /// 模板处理对象，用于在OnPreInit和OnInit事件中处理的数据对象
        /// </summary>
        public object TemplateHandleObject { get; set; }

        /// <summary>
        /// 添加变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public TemplatePage AddVariable<T>(string key, T value)
        {
            _dc.DefineVariable(key, value);
            return this;
        }

        /// <summary>
        /// 添加匿名对象实例
        /// </summary>
        /// <param name="templateData"></param>
        public void AddDataObject(object templateData)
        {
            //替换传入的标签参数
            PropertyInfo[] properties = templateData.GetType().GetProperties();
            foreach (PropertyInfo p in properties)
            {
                var dataValue = p.GetValue(templateData, null);
                if (dataValue != null)
                {
                    _data.Add(p.Name, dataValue);
                }
            }
        }


        private void PreRender(object obj, ref string content)
        {
            OnPreRender?.Invoke(obj, ref content);
        }

        private void BeforeCompile(string templateHtml)
        {
            OnBeforeCompile?.Invoke(this, ref templateHtml);
        }
        private void PreInit(object obj, ref string content)
        {
            if (this.OnPreInit != null && obj != null)
            {
                this.OnPreInit(obj, ref content);
            }
        }


        public override string ToString()
        {
            return Compile();
        }

        public string Compile()
        {
            
            // 从缓存中获取模板内容
            if (this._templateHtml == null && !String.IsNullOrEmpty(this._templateId))
            {
                this._templateHtml = TemplateCache.GetTemplateContent(this._templateId);
            }

            this.BeforeCompile(this._templateHtml);
            
            //HttpContext.Current.Response.Write("<br />1." + (DateTime.Now - dt).Milliseconds.ToString());
            //初始化之前发生
            this.PreInit(this.TemplateHandleObject, ref _templateHtml);
            //如果参数不为空，则替换标签并返回内容
            /*
            if (this._data.Count != 0)
            {
                foreach (string key in this._data.Keys)
                {
                    _templateHtml = TemplateRegexUtility.ReplaceHtml(_templateHtml, key, this._data[key].ToString());
                }
            }
            */
            
            //  HttpContext.Current.Response.Write("<br />2." + (DateTime.Now - dt).Milliseconds.ToString());
            //执行模板语法
            _templateHtml = Eval.Compile(_dc, _templateHtml, this.TemplateHandleObject);
            _templateHtml = this.ReplaceDefinedVariables(_templateHtml);
           

            // HttpContext.Current.Response.Write("<br />3." + (DateTime.Now - dt).Milliseconds.ToString());

            //解析实体的值
            //templateHtml = Eval.ExplanEntityProperties(dc,templateHtml);

            //呈现之前处理
            this.PreRender(this.TemplateHandleObject, ref _templateHtml);

            // HttpContext.Current.Response.Write("<br />4."+(DateTime.Now - dt).Milliseconds.ToString());

            return _templateHtml;
        }


        /// <summary>
        /// 替换自定义变量
        /// </summary>
        /// <param name="templateHtml"></param>
        /// <returns></returns>
        private string ReplaceDefinedVariables(string templateHtml)
        {
            if (_dc == null) return templateHtml;
            IDictionary<string, object> defineVars = _dc.GetDefineVariable();
            if (defineVars != null && defineVars.Count != 0)
            {
                foreach (string key in defineVars.Keys)
                {
                    if (defineVars[key] is Variable)
                    {
                        templateHtml = Eval.ResolveVariable(templateHtml, (Variable) defineVars[key]);
                    }
                    else
                    {
                        templateHtml = TemplateRegexUtility.ReplaceHtml(templateHtml, key,
                            (defineVars[key] ?? "").ToString());
                    }
                }
            }
            return templateHtml;
        }

        /// <summary>
        /// 压缩后的字符
        /// </summary>
        /// <returns></returns>
        private string ToCompressedString()
        {
            return TemplateUtils.CompressHtml(ToString());
        }
        
        /// <summary>
        /// 使用指定编码保存成为本地文件
        /// </summary>
        /// <param name="fileName">包含路径的文件名称,如：/html/default.html</param>
        /// <param name="coder"></param>
        /// <param name="compressed"></param>
        /// <param name="html"></param>
        private void SaveToFile(string fileName, Encoding coder, bool compressed, out string html)
        {
            string filePath =  EnvUtil.GetBaseDirectory()+"/" + fileName;

            //FileShare.None  独占方式打开

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

            //byte[] bytes = coder.GetBytes(this.ToString());
            //fs.Write(bytes, 0, bytes.Length);

            StreamWriter sr = new StreamWriter(fs, coder);

            html = compressed
                ? this.ToCompressedString()
                : this.ToString();

            sr.Write(html);

            sr.Flush();
            fs.Flush();
            sr.Dispose();
            fs.Dispose();
        }

        /// <summary>
        /// 保存成为本地文件(Unicode)
        /// </summary>
        /// <param name="fileName">包含路径的文件名称,如：/html/default.html</param>
        public void SaveToFile(string fileName)
        {
            string html;
            SaveToFile(fileName, Encoding.UTF8, false, out html);
        }

        /// <summary>
        /// 保存成为本地文件(Unicode)
        /// </summary>
        /// <param name="fileName">包含路径的文件名称,如：/html/default.html</param>
        public void SaveToFile(string fileName, out string html)
        {
            SaveToFile(fileName, Encoding.UTF8, false, out html);
        }
    }
}