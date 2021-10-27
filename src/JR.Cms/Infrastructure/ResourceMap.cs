using System;
using System.IO;
using JR.Stand.Core;

namespace JR.Cms.Infrastructure
{
    /// <summary>
    /// 资源映射
    /// </summary>
    public static class ResourceMap
    {
        private static string InternalXmlLangPackage;

        /// <summary>
        /// 获取内置语言文件
        /// </summary>
        /// <returns></returns>
        public static string GetXmlLangPackage()
        {
            if (InternalXmlLangPackage == null)
            {
                InternalXmlLangPackage = ResourcesReader.Read(typeof(ResourceMap).Assembly, "Infrastructure/Resources/lang_package.xml");
            }

            return InternalXmlLangPackage;
        }


        /// <summary>
        /// 获取自定义的语言包
        /// </summary>
        public static string XmlMyLangPackage
        {
            get
            {
                //local表示本地目录
                var filePath = EnvUtil.GetBaseDirectory() + "/public/local/lang_package.xml";
                var file = new FileInfo(filePath);
                if (file.Exists) return File.ReadAllText(filePath);
                return null;
            }
        }
    }
}