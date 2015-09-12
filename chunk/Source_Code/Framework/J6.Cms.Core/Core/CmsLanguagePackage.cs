using System;
using System.ComponentModel;
using System.IO;
using J6.Cms.Cache;
using J6.Cms.Conf;
using J6.Cms.Domain.Interface.Common.Language;
using J6.Cms.Infrastructure;

namespace J6.Cms.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class CmsLanguagePackage
    {
        private static LanguagePackage _lang;
        private static CmsLanguagePackage _instance;


        internal static CmsLanguagePackage Create()
        {
            return _instance ?? (_instance = new CmsLanguagePackage());
        }

        private CmsLanguagePackage()
        {
            _lang = new LanguagePackage();
            _lang.LoadFromXml(ResourceMap.XmlLangPackage);
            LoadLocaleXml();

            /*
           IDictionary<Languages,String> dict = new Dictionary<Languages,String>();

           //标签
           dict.Add(Languages.Zh_CN,"无标签");
           dict.Add(Languages.Zh_TW,"无标签");
           dict.Add(Languages.En_US,"no tags");

           lang.Add(LanguagePackageKey.PAGE_NO_TAGS, dict);

           dict.Clear();
            const string zh_cn_pack = "上一页|下一页|{0}|选择页码：{0}页";
           const string zh_tw_pack = "上一頁|下一頁|{0}|選擇頁碼：{0}頁";
           const string en_us_pack = "Previous|Next|{0}|Select Page：{0}";


           dict.Add(Languages.Zh_CN, "上一页");
           */
        }


        /// <summary>
        /// 加载系统内置的语言配置文件
        /// </summary>
        private void LoadLocaleXml()
        {
            DirectoryInfo dir = new DirectoryInfo(Cms.PyhicPath + CmsVariables.FRAMEWORK_PATH + "locale");
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles("*.xml");
                foreach (var fileInfo in files)
                {
                    _lang.LoadStandXml(GetXmlString(fileInfo.FullName));
                }
            }
        }

        private string GetXmlString(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return null;
        }



        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String Get(LanguagePackageKey key)
        {
            return _lang.Get(Cms.Context.CurrentSite.Language,key);
        }

        public string Get(Languages language,string key)
        {
            const string cacheKey = "lang_local";


            LanguagePackage localLang = CacheFactory.Sington.GetCachedResult<LanguagePackage>(
                cacheKey,
                () =>
                {
                    LanguagePackage languagePackage = new LanguagePackage();

                    try
                    {
                        string myLang = ResourceMap.XmlMyLangPackage;
                        if (myLang != null)
                        {
                            languagePackage.LoadFromXml(myLang);
                        }
                    }
                    catch
                    {
                        throw new FileLoadException(String.Format(
                            "本地语言包无法识别！请参考:http://{0}/framework/local/lang_package.xml修改.",
                          Settings.SERVER_STATIC));
                    }

                    CacheFactory.Sington.Insert(cacheKey, languagePackage,string.Format("{0}framework/local/lang_package.xml", Cms.PyhicPath));

                    return languagePackage;
                },DateTime.MaxValue
                );

            return localLang.GetOtherLangItemValue(key, language);
        }
    }
}
