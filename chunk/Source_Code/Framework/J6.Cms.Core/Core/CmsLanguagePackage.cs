using System;
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
        private bool _observerStarted;
        private const string CfgLocaleKey = "cfg_locale_observer";


        internal static CmsLanguagePackage Create()
        {
            return _instance ?? (_instance = new CmsLanguagePackage());
        }

        private CmsLanguagePackage()
        {
            _lang = new LanguagePackage();
            _lang.LoadFromXml(ResourceMap.XmlLangPackage);
            // 加载系统内置的
            LoadLocaleXml(Cms.PyhicPath + CmsVariables.FRAMEWORK_PATH + "locale",false,null);
            // 加载自定义的配置
            LoadLocaleXml(Cms.PyhicPath + CmsVariables.SITE_CONF_PATH + "locale",true,CfgLocaleKey);

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
        private void LoadLocaleXml(string dirPath,bool observer,string cacheKey)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles("*.xml");

                // 监视文件目录
                if (files.Length > 0 && observer)
                {
                    this._observerStarted = true;
                    CacheFactory.Sington.Insert(cacheKey,1,dirPath);
                }

                // 加载
                Languages lang;
                foreach (var fileInfo in files)
                {
                    bool result = Enum.TryParse(fileInfo.Name.Substring(0, fileInfo.Name.IndexOf(".", StringComparison.Ordinal)), true, out lang);
                    if (result)
                    {
                        _lang.LoadStandXml(lang, fileInfo.FullName);
                    }
                }
            }
        }


        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String Get(LanguagePackageKey key)
        {
            this.Observer();
            return _lang.Get(Cms.Context.CurrentSite.Language, key);
        }

        private void Observer()
        {
            if (this._observerStarted && CacheFactory.Sington.Get(CfgLocaleKey) == null)
            {
                lock (_lang)
                {
                    _lang = null;
                    _instance = new CmsLanguagePackage();
                }
            }
        }

        public string Get(Languages language, string key)
        {
            this.Observer();
            return _lang.GetValueByKey(language, key);
        }
    }
}
