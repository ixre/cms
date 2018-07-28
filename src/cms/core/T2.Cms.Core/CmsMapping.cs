using System;
using System.Xml;
using T2.Cms.Conf;

namespace T2.Cms
{
    public static class CmsMapping
    {


        /// <summary>
        /// 获取管理后台资源映射
        /// </summary>
        /// <returns></returns>
        public static string GetManagementMappingXml()
        {
            string mapTo=null;

            XmlNode node = SelectSingleNode("/maps/map[@for='management_resource']");
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        private static XmlNode SelectSingleNode(string xpath)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(String.Concat(Cms.PyhicPath, CmsVariables.FRAMEWORK_PATH, "maps.xml"));
            return xd.SelectSingleNode(xpath);
        }
    }
}
