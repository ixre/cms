using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using JR.Stand.Core.Utils;
using JR.Stand.Core.Web.Cache;

namespace JR.Cms.Conf
{
    public class LangLabelReader
    {
        private string filePath;
        private string cacheKey;    //缓存键
        private IMemoryCacheWrapper cache;
        public LangLabelReader(string filePath)
        {
            this.filePath = filePath;
            this.cache = CacheFactory.Sington.RawCache();
            cacheKey = String.Format("lang_cache_{0}" ,this.filePath.GetHashCode());
        }

        /// <summary>
        /// 获取标签值
        /// </summary>
        /// <param name="labelKey">标签键</param>
        /// <param name="selection">节点</param>
        /// <returns></returns>
        public string GetValue(string labelKey, string selection)
        {
            throw new NotImplementedException("not implement");
        //     IDictionary<string, IDictionary<string, string>> labelList
        //         = HttpRuntime.Cache[cacheKey] as IDictionary<string, IDictionary<string, string>>;
        //
        //     if (labelList == null)
        //     {
        //         //读取数据
        //
        //         labelList = new Dictionary<string, IDictionary<string, string>>();
        //         XmlDocument xd = new XmlDocument();
        //         xd.Load(this.filePath);
        //         XmlNodeList nodes = xd.SelectNodes("/lang/label");
        //
        //         foreach (XmlNode node in nodes)
        //         {
        //             IDictionary<string, string> dict = new Dictionary<string, string>();
        //             foreach (XmlAttribute xa in node.Attributes)
        //             {
        //                 if (xa.Name != "key")
        //                 {
        //                     dict.Add(xa.Name, xa.Value);
        //                 }
        //             }
        //             labelList.Add(node.Attributes["key"].Value, dict);
        //         }
        //
        //         //缓存数据
        //         Cms.Cache.Insert(cacheKey, labelList, this.filePath);
        //     }
        //     return labelList[labelKey][selection];
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="labelKey"></param>
        /// <param name="selection"></param>
        /// <param name="value"></param>
        public void SetValue(string labelKey, string selection, string value)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(this.filePath);

            XmlNode lang = xd.SelectSingleNode("/lang");
            XmlNode xn = xd.CreateElement("label");
            XmlAttribute xa=xd.CreateAttribute("key");
            xa.Value=value;
            xn.Attributes.Append(xa);
            lang.AppendChild(xn);
        }
    }
}