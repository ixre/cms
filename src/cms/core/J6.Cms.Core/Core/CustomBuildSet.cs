using System;
using System.Xml;
using J6.Cms.Conf;
using J6.DevFw.Framework;

namespace J6.Cms.Core
{
    public static class BuildSet
    {
        public const string SystemLogo = "system_logo";
        public const string EntryFrameUrl = "entry_frame_url";
    }
    /// <summary>
    /// 定制设置
    /// </summary>
    public class CustomBuildSet
    {
        private SettingFile _sf;

        public CustomBuildSet()
        {
            this._sf = new SettingFile(GetMappingXmlPath("custom_build_set"));
        }

        private static XmlNode SelectSingleNode(string xpath)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(String.Concat(Cms.PyhicPath, CmsVariables.FRAMEWORK_PATH, "maps.xml"));
            return xd.SelectSingleNode(xpath);
        }

        private static string GetMappingXmlPath(String key)
        {
            string mapTo = null;

            XmlNode node = SelectSingleNode("/maps/map[@for='"+key+"']");
            if (node == null
                || node.Attributes["to"] == null
                || (mapTo = node.Attributes["to"].Value) == String.Empty)
            {
                Cms.TraceLog("management mapping lose!");
                return null;
            }

            return GetMapToPath(mapTo);
        }

        private static string GetMapToPath(string mapTo)
        {
            //拼接路径
            if (mapTo.StartsWith("/"))
            {
                return String.Concat(Cms.PyhicPath, mapTo.Substring(1));
            }
            else
            {
                return String.Concat(Cms.PyhicPath, CmsVariables.FRAMEWORK_PATH, mapTo);
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
