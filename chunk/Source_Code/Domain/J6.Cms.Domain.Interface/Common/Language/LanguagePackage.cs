using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace J6.Cms.Domain.Interface.Common.Language
{
    public class LanguagePackage
    {
        private IDictionary<string, string> languagePack = new Dictionary<string, string>();

        /// <summary>
        /// 从XML中加载语言
        /// </summary>
        /// <param name="xml">XML内容</param>
        public void LoadFromXml(string xml)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xml);

            XmlNodeList nodes = xd.SelectNodes("/lang/item");

            Type langKeyType=typeof(LanguagePackageKey);
            Type langType=typeof(Languages);
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
                        {
                            if (xa.Name != "key")
                            {
                                lang = (Languages)System.Enum.Parse(langType, xa.Name);
                                dict.Add(lang, xa.Value);
                            }
                        }

                        strKey = node.Attributes["key"].Value;

                        if (System.Enum.IsDefined(langKeyType, strKey))
                        {

                            langKey = (LanguagePackageKey)System.Enum.Parse(langKeyType, strKey, true);
                            this.Add(langKey, dict);
                        }
                        else
                        {
                            this.AddOtherLangItem(node.Attributes["key"].Value, dict);
                        }
                    }
                }
        }

        /// <summary>
        /// 添加默认语言
        /// </summary>
        /// <param name="key"></param>
        /// <param name="langs"></param>
        public void Add(LanguagePackageKey key, IDictionary<Languages, string> langs)
        {
            String keyStr = ((int)key).ToString();
            string packKeyStr;

            foreach (KeyValuePair<Languages, string> pair in langs)
            {
                packKeyStr = String.Concat(keyStr, ">", ((int)pair.Key).ToString());
                if(languagePack.ContainsKey(packKeyStr))
                    break;
                languagePack.Add(packKeyStr , pair.Value);
            }
        }

        /// <summary>
        /// 获取语言项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public string Get(LanguagePackageKey key, Languages lang)
        {
            string dictKey = String.Concat(
                ((int)key).ToString(CultureInfo.InvariantCulture), 
                ">", 
                ((int)lang).ToString(CultureInfo.InvariantCulture));

            string outStr = null;
            if (languagePack.TryGetValue(dictKey, out outStr))
            {
                return outStr;
            }

            throw new ArgumentNullException("key", 
                String.Format("({0}->{1})不包含当前语言的配置项!", 
                lang.ToString(),
                key.ToString()));
        }

        /// <summary>
        /// 添加自定义语言项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="langs"></param>
        public void AddOtherLangItem(string key,IDictionary<Languages,string> langs)
        {
            foreach (KeyValuePair<Languages, string> pair in langs)
            {
                languagePack.Add(String.Concat(key, ">", ((int)pair.Key).ToString()), pair.Value);
            }
        }

        /// <summary>
        /// 获取自定义的语言项值，如果不存在此项，则返回String.Empty
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public String GetOtherLangItemValue(string key, Languages lang)
        {
            string dictKey = String.Concat(key, ">", ((int)lang).ToString());
            string outStr = null;
            if (languagePack.TryGetValue(dictKey, out outStr))
            {
                return outStr;
            }
            return String.Empty;
        }


    }
}
