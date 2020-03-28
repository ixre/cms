//
//
//  Copyright 2011 (C) S1N1.COM,All rights reseved.
//
//  Project : tagsplugin
//  File Name : Tags.cs
//  Date : 8/27/2011
//  Author : Newmin
//  ---------------------------------------------
//  2011-09-13 newmin[+]:��������滻������ǩReplaceSingleTag()
//                   [!]:�޸Ŀ���������Ŀ��
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace JR.Stand.Toolkit.HttpTag
{
    public class TagsManager
    {
        //�����ļ�·��
        private string configFilePath;

        private string[] nameArray;

        private TagsCollection tags=new TagsCollection();


        public class TagsCollection:IEnumerable<Tag>
        {
            internal IDictionary<string, Tag> dict = new Dictionary<string, Tag>();

            /// <summary>
            /// ��ǩ����
            /// </summary>
            public int Count
            {
                get { return dict.Count; }
            }

            public IEnumerator<Tag> GetEnumerator()
            {
                
                foreach (KeyValuePair<string,Tag> pair in dict)
                {
                    yield return pair.Value;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }


        public TagsManager(string configFilePath)
        {
            this.configFilePath = configFilePath;
            if (!File.Exists(this.configFilePath))
            {
                //����ļ�������,�򴴽�����ʼ������
                File.Create(this.configFilePath).Dispose();
                const string initData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<tags>\r\n\t<config/>\r\n\t<list/>\r\n</tags>";
                byte[] data = Encoding.UTF8.GetBytes(initData);
                FileStream fs = new FileStream(this.configFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Dispose();
            }
           this.LoadAllTags();
        }

        /// <summary>
        /// ��ǩ����
        /// </summary>
        public TagsCollection Tags { get { return tags; } }


        /// <summary>
        /// ��������Tags
        /// </summary>
        /// <returns></returns>
        private void LoadAllTags()
        {

            XmlDocument xd = new XmlDocument();

            //���������ļ�
            xd.Load(configFilePath);

            //���б�ǩ�ڵ�
            XmlNodeList tagNodes = xd.SelectNodes("/tags/list/tag");

            //�������
            tags.dict.Clear();

            //�����ؼ�������
            nameArray = new String[tagNodes.Count];

            //��ӹؼ��ʵ��ʵ䣬��Ϊ�ؼ������鸳ֵ
            for (int i = 0; i < tagNodes.Count; i++)
            {
                nameArray[i] = tagNodes[i].InnerText;

                tags.dict.Add(nameArray[i],
                    new Tag
                    {
                         Indent=int.Parse(tagNodes[i].Attributes["indent"].Value),
                        Name = tagNodes[i].InnerText,
                        Description = tagNodes[i].Attributes["description"].Value,
                        LinkUri = tagNodes[i].Attributes["linkuri"].Value
                    });
            }

            Array.Sort(nameArray, (a, b) => { return b.Length-a.Length; });

        }

        /// <summary>
        /// ��ӱ�ǩ
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool Add(Tag tag)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);

            //��ǩ�����Ƿ��ظ�
            if (xd.SelectSingleNode(String.Format("/tags/list/tag[@name=\"{0}\"]", tag.Name)) != null) return false;

            //��ȡ���һ���ڵ�����
            XmlNode node = xd.SelectSingleNode("/tags/list/tag[last()]");
            if (node != null)
            {
                int tagIndex;
                int.TryParse(node.Attributes["indent"].Value, out tagIndex);
                tag.Indent = tagIndex + 1;
            }
            else
            {
                tag.Indent=1;
            }
            

            //���б�ǩ�ڵ�
            XmlNode root = xd.SelectSingleNode("/tags/list");

            XmlNode tagNode = xd.CreateElement("tag");

            XmlAttribute xn =xd.CreateAttribute("indent");
            xn.Value = tag.Indent.ToString();
            tagNode.Attributes.Append(xn);

            //xn = xd.CreateAttribute("name");
            //xn.Value = tag.Name;
            //tagNode.Attributes.Append(xn);

            xn = xd.CreateAttribute("description");
            xn.Value = tag.Description;
            tagNode.Attributes.Append(xn);

            xn = xd.CreateAttribute("linkuri");
            xn.Value = tag.LinkUri;
            tagNode.Attributes.Append(xn);

            tagNode.InnerText = tag.Name;

            root.AppendChild(tagNode);

            //����
            xd.Save(configFilePath);

            //������ݲ����¼���
            LoadAllTags();

            return true;

        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfig(string key)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);
            XmlNode keyNode = xd.SelectSingleNode(String.Format("/tags/config/add[@key=\"{0}\"]",key));
            if (keyNode != null)
            {
                return keyNode.Attributes["value"].Value;
            }
            return null;
        }

        /// <summary>
        /// �޸�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetConfig(string key, string value)
        {

            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);
            XmlNode keyNode = xd.SelectSingleNode(String.Format("/tags/config/add[@key=\"{0}\"]", key));
            if (keyNode != null)
            {
                keyNode.Attributes["value"].Value = value;
            }
            else
            {
                XmlNode rootNode = xd.SelectSingleNode("/tags/config");
                keyNode = xd.CreateElement("add");

                XmlAttribute xa=xd.CreateAttribute("key");
                xa.Value=key;
                keyNode.Attributes.Append(xa);

                xa=xd.CreateAttribute("value");
                xa.Value=value;
                keyNode.Attributes.Append(xa);

                rootNode.AppendChild(keyNode);
            }

            xd.Save(configFilePath);
        }

        /// <summary>
        /// ���±�ǩ
        /// </summary>
        /// <param name="tag"></param>
        public void Update(Tag tag)
        {

            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);

            XmlNode tagNode = xd.SelectSingleNode(String.Format("/tags/list/tag[@indent=\"{0}\"]", tag.Indent.ToString()));

            tagNode.InnerText = tag.Name;
            tagNode.Attributes["description"].Value = tag.Description;
            tagNode.Attributes["linkuri"].Value = tag.LinkUri;

            //����
            xd.Save(configFilePath);

            //������ݲ����¼���
            LoadAllTags();

        }

        /// <summary>
        /// ɾ����ǩ
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);

            XmlNode root = xd.SelectSingleNode("/tags/list");
            XmlNode tagNode = xd.SelectSingleNode(String.Format("/tags/list/tag[@indent=\"{0}\"]", id));

            if (tagNode != null)
            {
                root.RemoveChild(tagNode);
                xd.Save(configFilePath);

                //������ݲ����¼���
                LoadAllTags();
            }
        }

        /// <summary>
        /// �������ƻ�ȡ��ǩ����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Tag FindByName(string name)
        {
            if (tags.dict.Keys.Contains(name)) return tags.dict[name];
            return null;
        }

        private string Replace(string content,bool openInBlank)
        {
            //if (!defaultTagLinkFormat.Contains("{0}")) throw new ArgumentException("������{0}��ʾ�������ñ�ǩID����");

            Tag tag;
            Regex reg;

            //��ǩ���Ӹ�ʽ
            string tagLinkFormat =openInBlank?
                "<a href=\"{0}\" title=\"{1}\" class=\"auto-tag\" target=\"_blank\">{2}</a>" :
                "<a href=\"{0}\" title=\"{1}\" class=\"auto-tag\">{2}</a>";


            foreach (string key in nameArray)
            {
                tag=FindByName(key);
                if(tag==null)continue;

                reg = new Regex(String.Format("<a[^>]+>(?<key>{0})</a>|(?!<a[^>]*)(?<key>{0})(?![^<]*</a>)", Regex.Escape(key)), RegexOptions.IgnoreCase);


                content = reg.Replace(content, match =>
                {
                    return String.Format(tagLinkFormat,
                        String.IsNullOrEmpty(tag.LinkUri)?"javascript:;":tag.LinkUri,
                        tag.Description,
                        key);
                });
            }

            return content;
        }


        /// <summary>
        /// �滻����
        /// </summary>
        /// <param name="content"></param>
        /// <param name="openInBlank"></param>
        /// <param name="singleMode"></param>
        /// <returns></returns>
        public string Replace(string content, bool openInBlank,bool singleMode)
        {
            //���ȫ���滻
            if (!singleMode) return Replace(content, openInBlank);

            Tag tag;
            Regex reg;

            //��������
            int _index=0;

            foreach(string key in nameArray)
            {
                tag = FindByName(key);
                if (tag == null) continue;

                reg = new Regex(String.Format("<a[^>]+>(?<key>{0})</a>|(?!<a[^>]*)(?<key>{0})(?![^<]*</a>)", 
                    Regex.Escape(key)), RegexOptions.IgnoreCase);


                content = reg.Replace(content, match =>
                {
                   
                    if (++_index == 1)
                    {
                        return String.Format(

                            //��ǩ���Ӹ�ʽ
                            openInBlank ?"<a href=\"{0}\" title=\"{1}\" class=\"auto-tag\" target=\"_blank\">{2}</a>" :
                            "<a href=\"{0}\" title=\"{1}\" class=\"auto-tag\">{2}</a>",


                            String.IsNullOrEmpty(tag.LinkUri) ?"javascript:;":tag.LinkUri,
                            tag.Description,
                            key);
                    }
                    else
                    {
                        return match.Groups["key"].Value;
                    }
                });

                _index = 0;
            }
            return content;
        }

        /// <summary>
        /// ��������ǩ�滻������
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ReplaceSingleTag(string content)
        {
            return Replace(content, true, true);
        }

        /// <summary>
        /// �Ƴ������ڵ�Tag
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string RemoveAutoTags(string content)
        {
            string linkText;
            return Regex.Replace(content, "<a(.+?)class=\"auto-tag\"[^>]+>(.+?)</a>", match =>
            {
                linkText = match.Groups[2].Value;
                if (!tags.dict.Keys.Contains(linkText))
                {
                    return linkText;
                }
                return match.Value;
            },RegexOptions.Multiline);
        }


        /// <summary>
        /// �滻����Ϊ��ǩ
        /// </summary>
        /// <param name="defaultTagLinkFormat"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string Replace2(string content)
        {
            return null;



            Regex reg = new Regex(@"(?i)(?:^|(?<!<a\b[^<>]*)>)[^<>]*(?:<|$)");

#if DEBUG
            //System.Web.HttpContext.Current.Response.Write(String.Format("<br ><br />ƥ��{0}<br /><br />",reg.IsMatch(content)));




#endif
            //int length = 0;
            string temp;
            Tag tag;

            //�Ѿ���ӵ�ջ,ȷ��ֻ�滻һ��
            Stack<int> stack = new Stack<int>();

            return reg.Replace(content, m =>
            {
                temp =Regex.Replace(m.Value,"<|>|\\/",String.Empty);


               // System.Web.HttpContext.Current.Response.Write(String.Format("<br />({0})<br />", m.Value));

                //length = temp.Length;

                //�����ؼ�������,�ұ�֤�ؼ������в���ջ��
                for (int i = nameArray.Length - 1; i >= 0 && !stack.Contains(i); i--)
                {
                    tag = this.FindByName(nameArray[i]);

                    if (tag != null)
                    {
                        temp = Regex.Replace(temp, String.Format(@"(?is)^((?:(?:(?!{0}|</?a\b).)*<a\b(?:(?!</?a\b).)*</a>)*(?:(?!{0}|</?a\b).)*)(?<tag>{0})", Regex.Escape(tag.Name))
                            , String.Format("$1<a href=\"{0}\" target=\"_blank\" title=\"{1}\">{2}</a>",
                            String.IsNullOrEmpty(tag.LinkUri)?"javascript:;":tag.LinkUri,       //���δ�������ӣ���ʹ��Ĭ�ϸ�ʽ
                            tag.Description, "${tag}"));
                    }

                    //��֤����ֻ���滻һ��
                    stack.Push(i);

                    // if (length != temp.Length)
                    //{
                    //   stack.Push(i);

                    //  }
                    //length = temp.Length;
                }

                return temp;
            });
        }

        /// <summary>
        /// ���ؼ��ּ����ӣ�ͬһ�ؼ���ֻ��һ��
        /// </summary>
        /// <param name="content">Դ�ַ���</param>
        /// <param name="keys">�ؼ��ַ���</param>
        /// <returns>�滻����</returns>
        private string keyAddUrl(string content)
        {
            return null;
            /*
            Regex reg = new Regex(@"(?i)(?:^|(?<!<a\b[^<>]*)>)[^<>]*(?:<|$)");
            int length = 0;
            string temp;
            Tag tag;

            return reg.Replace(content, m =>
            {
                temp = m.Value;
                length = temp.Length;
                for (int i = tags.Count - 1; i >= 0; i--)
                {
                    tag = this.FindByName(nameArray[i]);
                    temp = Regex.Replace(temp, String.Format(@"(?is)^((?:(?:(?!{0}|</?a\b).)*<a\b(?:(?!</?a\b).)*</a>)*(?:(?!{0}|</?a\b).)*)(?<tag>{0})", Regex.Escape(tag.Name))
                        , String.Format("$1<a href=\"{0}\" target=\"_blank\" title=\"{1}\">${tag}</a>",tag.LinkUri,tag.Description));
                    
                    if (length != temp.Length)
                    {
                       //tags.nameDict.Remove(tags.nameDict[i]);
                     }
                    length = temp.Length;
                }
                return temp;
            });*/
        }
    }
}
