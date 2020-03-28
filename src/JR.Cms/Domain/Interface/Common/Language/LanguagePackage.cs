using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace JR.Cms.Domain.Interface.Common.Language
{
    public class LanguagePackage
    {
        private readonly IDictionary<Languages, IDictionary<string, string>> _languagePack; //语言字典

        public LanguagePackage()
        {
            _languagePack = new Dictionary<Languages, IDictionary<string, string>>();
            InitPackage();
        }

        private void InitPackage()
        {
            var i = 1;
            while (System.Enum.IsDefined(typeof(Languages), i))
            {
                _languagePack.Add((Languages) i, new Dictionary<string, string>());
                i++;
            }
        }

        /// <summary>
        /// 从XML中加载语言
        /// </summary>
        /// <param name="xml">XML内容</param>
        public void LoadFromXml(string xml)
        {
            var xd = new XmlDocument();
            xd.LoadXml(xml);

            var nodes = xd.SelectNodes("/lang/item");

            var langKeyType = typeof(LanguagePackageKey);
            var langType = typeof(Languages);
            LanguagePackageKey langKey;
            Languages lang;
            string strKey;

            IDictionary<Languages, string> dict = new Dictionary<Languages, string>();

            if (nodes != null)
                foreach (XmlNode node in nodes)
                {
                    dict.Clear();
                    if (node.Attributes != null)
                    {
                        foreach (XmlAttribute xa in node.Attributes)
                            if (xa.Name != "key")
                            {
                                lang = (Languages) System.Enum.Parse(langType, xa.Name);
                                dict.Add(lang, xa.Value);
                            }

                        strKey = node.Attributes["key"].Value;
                        if (System.Enum.IsDefined(langKeyType, strKey))
                        {
                            langKey = (LanguagePackageKey) System.Enum.Parse(langKeyType, strKey, true);
                            Add(langKey, dict);
                        }
                        else
                        {
                            AddOtherLangItem(node.Attributes["key"].Value, dict);
                        }
                    }
                }
        }


        private string GetXmlString(string path)
        {
            if (File.Exists(path)) return File.ReadAllText(path);
            return null;
        }


        /// <summary>
        /// 加载单独的XML
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="xmlPath"></param>
        public void LoadStandXml(Languages lang, string xmlPath)
        {
            var xd = new XmlDocument();
            xd.Load(xmlPath);

            var nodes = xd.SelectNodes("/lang/item");
            string strKey;
            string strVal;

            IDictionary<Languages, string> dict = new Dictionary<Languages, string>();

            var isRewrite = false;

            if (nodes != null)
                foreach (XmlNode node in nodes)
                {
                    dict.Clear();
                    strKey = node.Attributes["key"].Value;
                    strVal = node.InnerText;

                    AddOne(lang, strKey, strVal);

                    //如果不是文本注释,删除第一个节点并重新保存值
                    if (node.FirstChild != null && node.FirstChild.Name != "#cdata-section")
                    {
                        node.RemoveChild(node.FirstChild);
                        node.InsertBefore(xd.CreateCDataSection(strVal ?? ""), node.FirstChild);
                        isRewrite = true;
                    }
                }

            if (isRewrite) xd.Save(xmlPath);
        }

        private void AddOne(Languages lang, string key, string value)
        {
            var dict = _languagePack[lang];
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key] = value;
        }

        /// <summary>
        /// 添加默认语言
        /// </summary>
        /// <param name="key"></param>
        /// <param name="langs"></param>
        public void Add(LanguagePackageKey key, IDictionary<Languages, string> langs)
        {
            var keyStr = ((int) key).ToString();
            foreach (var pair in langs) AddOne(pair.Key, keyStr, pair.Value);
        }

        /// <summary>
        /// 添加自定义语言项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="langs"></param>
        public void AddOtherLangItem(string key, IDictionary<Languages, string> langs)
        {
            foreach (var pair in langs)
                if (_languagePack[pair.Key].ContainsKey(key))
                    _languagePack[pair.Key][key] = pair.Value;
                else
                    _languagePack[pair.Key].Add(key, pair.Value);
        }

        /// <summary>
        /// 获取语言项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public string Get(Languages lang, LanguagePackageKey key)
        {
            string outStr = null;
            if (_languagePack[lang].TryGetValue(((int) key).ToString(), out outStr)) return outStr;
            throw new ArgumentNullException("key",
                string.Format("({0}->{1})不包含当前语言的配置项!", lang, key));
        }

        /// <summary>
        /// 获取自定义的语言项值，如果不存在此项，则返回String.Empty
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public string GetValueByKey(Languages lang, string key)
        {
            string outStr;
            if (_languagePack[lang].TryGetValue(key, out outStr)) return outStr;
            return null;
        }

        public void LoadFromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            IList<LangKvPair> list = JsonConvert.DeserializeObject<List<LangKvPair>>(json);
            if (list != null)
                foreach (var l in list)
                    if (l.value != null)
                        foreach (var k in l.value.Keys)
                        {
                            var v = l.value[k];
                            if (v == null) continue;
                            AddOne((Languages) k, l.key, v);
                        }
        }
    }
}