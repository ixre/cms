using System;
using System.IO;
using System.Xml;
using JR.Cms.Conf;
using JR.Stand.Core.Framework;

namespace JR.Cms.Core
{
    public static class BuildSet
    {
        public const string SystemLogo = "system_logo";
        public const string EntryFrameUrl = "entry_frame_url";
    }
    /// <summary>
    /// 定制设置
    /// </summary>
    public class BuildOEM
    {
        private SettingFile _sf;
        private String prefix = CmsVariables.OEM_PATH;

        public BuildOEM()
        {
            this._sf = new SettingFile(this.GetMappingXmlPath("build_set"));
        }

        private XmlNode SelectSingleNode(string xpath)
        {
            XmlDocument xd = new XmlDocument();
            String filePath = String.Concat(Cms.PyhicPath, CmsVariables.OEM_PATH, "meta.xml");
            if (!File.Exists(filePath))
            {
                filePath = String.Concat(Cms.PyhicPath, CmsVariables.FRAMEWORK_PATH, "registry_meta.xml");
                this.prefix = CmsVariables.FRAMEWORK_PATH;
            }
            xd.Load(filePath);
            return xd.SelectSingleNode(xpath);
        }

        /// <summary>
        /// 获取节点文件路径
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public String SelectNodeValuePath(String xpath)
        {
            string mapTo = null;
            XmlNode node = this.SelectSingleNode(xpath);
            if (node == null
               || node.Attributes["value"] == null
               || (mapTo = node.Attributes["value"].Value) == String.Empty)
            {
                Cms.TraceLog("management mapping lose!");
                return null;
            }

            return this.GetMapToPath(mapTo);
        }

        private string GetMappingXmlPath(String key)
        {
            return this.SelectNodeValuePath("/keys/item[@key='"+key+"']");
        }

        private string GetMapToPath(string mapTo)
        {
            //拼接路径
            if (mapTo.StartsWith("/"))
            {
                return String.Concat(Cms.PyhicPath, mapTo.Substring(1));
            }
            else
            {
                return String.Concat(Cms.PyhicPath, this.prefix, mapTo);
            }
        }

        public String Get(String key)
        {
            if (this._sf.Contains(key))
            {
                return this._sf.Get(key);
            }
            return String.Empty;
        }
    }
}
