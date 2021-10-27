//
// Copyright (C) 2007-2008 S1N1.COM,All rights reserved.
// 
// Project: OPS.Web.JSON
// FileName : JsonDictionaryFile.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/11/26 20:01:29
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace JR.Stand.Core.Framework
{
    /// <summary>
    /// 设置数据文件
    /// </summary>
    public class SettingFile
    {
        private readonly string _filePath;
        private readonly XmlDocument _xdoc;
        private readonly XmlNode _rootNode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public SettingFile(string filePath)
        {
            this._xdoc = new XmlDocument();
            this._filePath = filePath;

            try
            {
                //不存在，则创建
                if (!File.Exists(this._filePath))
                {
                    File.Create(filePath).Dispose();
                    const string initData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<settings>\r\n</settings>";

                    byte[] data = Encoding.UTF8.GetBytes(initData);
                    FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                    fs.Dispose();
                }

                //读取文档
                using (TextReader tr = new StreamReader(this._filePath))
                {
                    _xdoc.LoadXml(tr.ReadToEnd().Trim());
                    tr.Dispose();
                    this._rootNode = _xdoc.SelectSingleNode("/settings");
                }
            }catch(Exception ex)
            {
                throw new IOException(ex.Message + ", File:" + filePath);
            }
        }


        /// <summary>
        /// 是否包含某个键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return this._rootNode.SelectSingleNode($"/settings/add[@key='{key}']") != null;
        }

        /// <summary>
        /// 获取或设置指定键值的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return this.Get(key); }
            set
            {
                this.Set(key, value);
            }
        }

        /// <summary>
        /// Get the field value by key
        /// </summary>
        /// <param name="key"></param>
         
        public string Get(String key)
        {
            XmlNode node = this._rootNode.SelectSingleNode($"add[@key='{key}']");
            if (node == null)
            {
                throw new ArgumentOutOfRangeException("key", "no such key named \"" + key + "\"");
            }
            return node.InnerText;
        }

        /// <summary>
        /// Set field value, if not exits will add new one.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Set(String key, String value)
        {
            XmlNode node = this._rootNode.SelectSingleNode(String.Format("add[@key='{0}']", key));
            if (node == null)
            {
                this.Add(key,value??"",false);
                return;
                //throw new ArgumentOutOfRangeException("key", "no such key named \"" + key + "\"");
            }


            //如果不是文本注释,删除第一个节点并重新保存值
            if (node.FirstChild != null)
            {

                if (node.FirstChild.Name == "#cdata-section")
                {
                    XmlCDataSection xmlCDataSection = node.FirstChild as XmlCDataSection;
                    if (xmlCDataSection != null) xmlCDataSection.InnerText = value ?? "";
                }
                else
                {
                    node.RemoveChild(node.FirstChild);
                    node.InsertBefore(_xdoc.CreateCDataSection(value ?? ""), node.FirstChild);
                }
            }
            else
            {
                node.AppendChild(_xdoc.CreateCDataSection(value ?? ""));
            }
        }

        /// <summary>
        /// 添加新的设置
        /// </summary>
        [Obsolete]
        public void Add(string key, string value)
        {
            this.Add(key, value, false);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ignoreExist">是否忽略已经存在的配置</param>
        private void Add(string key, string value, bool ignoreExist)
        {
            //检查是否已经存在
            XmlNode _xn;
            _xn = this._rootNode.SelectSingleNode(String.Format("add[@key='{0}']", key));
            if (_xn != null)
            {
                if (ignoreExist)
                {
                    return;
                }
                else
                {
                    throw new ArgumentException(String.Format("已经存在一个名为{0}的节点!", key));
                }
            }

            XmlNode root = this._rootNode;


            XmlNode xn = _xdoc.CreateElement("add");

            //添加key属性
            XmlAttribute xa = _xdoc.CreateAttribute("key");
            xa.Value = key;
            if (xn.Attributes != null) xn.Attributes.Append(xa);

            //添加JSON内容
            xn.AppendChild(_xdoc.CreateCDataSection(value));

            //将新元素添加到DOM
            root.AppendChild(xn);
        }

        /// <summary>
        /// 移除指定的设置
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            XmlNode _xn = this._rootNode.SelectSingleNode(String.Format("add[@key='{0}']", key));
            if (_xn != null)
            {
                this._rootNode.RemoveChild(_xn);
            }
        }


        /// <summary>
        /// 搜索键值并以字典的形式返回
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IDictionary<string, string> SearchKey(string keyword)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();

            XmlNodeList node = this._rootNode.SelectNodes(String.Format("add[contains(@key,'{0}')]", keyword));

            if (node != null && node.Count != 0)
            {
                foreach (XmlNode xn in node)
                {
                    if (xn.Attributes != null)
                    {
                        dict.Add(xn.Attributes["key"].Value, xn.InnerText);
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// 保存到文件中
        /// </summary>
        public void Flush()
        {
            /*
            using (TextWriter tr = new StreamWriter(this.filePath))
            {
                tr.Write(xmlContent);
                tr.Flush();
                tr.Dispose();
            }*/

            _xdoc.Save(this._filePath);
        }
    }
}