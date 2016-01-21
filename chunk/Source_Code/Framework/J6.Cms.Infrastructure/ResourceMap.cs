using System;
using System.IO;

namespace J6.Cms.Infrastructure
{
    /// <summary>
    /// 资源映射
    /// </summary>
    public static class ResourceMap
    {
        /// <summary>
        /// 语言包
        /// </summary>
        public static String XmlLangPackage { get { return Resources.CmsResource.lang_package; } }

        /// <summary>
        /// 获取自定义的语言包
        /// </summary>
        public static String XmlMyLangPackage
        {
            get
            {
                //local表示本地目录
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "public/local/lang_package.xml";
                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    return File.ReadAllText(filePath);
                }
                return null;
            }
        }
    }
}
