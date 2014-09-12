using System;
using System.IO;
using Ops.Cms.Cache;
using Ops.Cms.Conf;
using Ops.Cms.Domain.Interface.Common.Language;
using Ops.Cms.Infrastructure;

namespace Ops.Cms.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class CmsLanguagePackage
    {
        private static LanguagePackage lang;

        static CmsLanguagePackage()
        {

            lang = new LanguagePackage();

            lang.LoadFromXml(ResourceMap.XmlLangPackage);





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

        internal CmsLanguagePackage()
        {

        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String Get(LanguagePackageKey key)
        {
            return lang.Get(key,
                Cms.Context.CurrentSite.Language);
        }

        public string Get(string key, Languages language)
        {
            const string cacheKey = "lang_local";


            LanguagePackage localLang = CacheFactory.Sington.GetResult<LanguagePackage>(
                cacheKey,
                () =>
                {
                    LanguagePackage lang = new LanguagePackage();

                    try
                    {
                        string myLang = ResourceMap.XmlMyLangPackage;
                        if (myLang != null)
                        {
                            lang.LoadFromXml(myLang);
                        }
                    }
                    catch
                    {
                        throw new FileLoadException(String.Format(
                            "本地语言包无法识别！请参考:http://{0}/framework/local/lang_package.xml修改.",
                          Settings.SERVER_STATIC));
                    }

                    CacheFactory.Sington.Insert(cacheKey, lang, String.Concat(Cms.PyhicPath + "framework/local/lang_package.xml"));

                    return lang;
                }

                );

            return localLang.GetOtherLangItemValue(key, language);
        }
    }
}
