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
        private string prefix = CmsVariables.OEM_PATH;

        public BuildOEM()
        {
            _sf = new SettingFile(GetMappingXmlPath("build_set"));
        }

        private XmlNode SelectSingleNode(string xpath)
        {
            var xd = new XmlDocument();
            var filePath = string.Concat(Cms.PhysicPath, CmsVariables.OEM_PATH, "meta.xml");
            if (!File.Exists(filePath))
            {
                filePath = string.Concat(Cms.PhysicPath, CmsVariables.FRAMEWORK_PATH, "registry_meta.xml");
                prefix = CmsVariables.FRAMEWORK_PATH;
            }

            //Console.WriteLine($@"[ Sys][ OEM]: oem file :{filePath}");
            xd.Load(filePath);
            return xd.SelectSingleNode(xpath);
        }

        /// <summary>
        /// 获取节点文件路径
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public string SelectNodeValuePath(string xpath)
        {
            string mapTo = null;
            var node = SelectSingleNode(xpath);
            if (node == null
                || node.Attributes["value"] == null
                || (mapTo = node.Attributes["value"].Value) == string.Empty)
            {
                Cms.TraceLog("management mapping lose!");
                return null;
            }

            return GetMapToPath(mapTo);
        }

        private string GetMappingXmlPath(string key)
        {
            return SelectNodeValuePath("/keys/item[@key='" + key + "']");
        }

        private string GetMapToPath(string mapTo)
        {
            //拼接路径
            if (mapTo.StartsWith("/"))
                return string.Concat(Cms.PhysicPath, mapTo.Substring(1));
            else
                return string.Concat(Cms.PhysicPath, prefix, mapTo);
        }

        public string Get(string key)
        {
            if (_sf.Contains(key)) return _sf.Get(key);
            return string.Empty;
        }
    }
}