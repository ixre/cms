using System;
using System.IO;
using System.Text;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Infrastructure;

namespace JR.Cms.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class CmsLanguagePackage
    {
        private readonly LanguagePackage _lang;


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LanguagePackage GetPackage()
        {
            return _lang;
        }

        internal CmsLanguagePackage()
        {
            _lang = new LanguagePackage();
            _lang.LoadFromXml(ResourceMap.GetXmlLangPackage());
            //todo: 加载自定义的配置,已经过时为了兼容以前数据
            LoadLocaleXml(Cms.PhysicPath + CmsVariables.SITE_CONF_PATH + "locale");

            //加载JSON格式语言
            LoadFromFile(Cms.PhysicPath + CmsVariables.FRAMEWORK_ASSETS_PATH + "locale/locale.db");
            // 加载系统内置的
            LoadLocaleXml(Cms.PhysicPath + CmsVariables.FRAMEWORK_PATH + "locale");
            // 加载用户通过界面添加的本地化资源
            LoadFromFile(Cms.PhysicPath + CmsVariables.SITE_LOCALE_PATH);


            /*
           IDictionary<Languages,String> dict = new Dictionary<Languages,String>();

           //标签
           dict.Add(Languages.zh_CN,"无标签");
           dict.Add(Languages.zh_TW,"无标签");
           dict.Add(Languages.En_US,"no tags");

           lang.Add(LanguagePackageKey.PAGE_NO_TAGS, dict);

           dict.Clear();
            const string zh_cn_pack = "上一页|下一页|{0}|选择页码：{0}页";
           const string zh_tw_pack = "上一頁|下一頁|{0}|選擇頁碼：{0}頁";
           const string en_us_pack = "Previous|Next|{0}|Select Page：{0}";


           dict.Add(Languages.zh_CN, "上一页");
           */
        }

        private void LoadFromFile(string file)
        {
            if (File.Exists(file))
            {
                var rd = new StreamReader(file, Encoding.UTF8);
                var json = rd.ReadToEnd();
                rd.Close();
                _lang.LoadFromJson(json);
            }
        }


        /// <summary>
        /// 加载系统内置的语言配置文件
        /// </summary>
        private void LoadLocaleXml(string dirPath)
        {
            var dir = new DirectoryInfo(dirPath);
            if (dir.Exists)
            {
                var files = dir.GetFiles("*.xml");
                // 加载
                foreach (var fileInfo in files)
                {
                    var result =
                        Enum.TryParse(fileInfo.Name.Substring(0, fileInfo.Name.IndexOf(".", StringComparison.Ordinal)),
                            true, out Languages lang);
                    if (result) _lang.LoadStandXml(lang, fileInfo.FullName);
                }
            }
        }


        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(LanguagePackageKey key)
        {
            return _lang.Get(Cms.Context.UserLanguage, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(Languages language, string key)
        {
            return _lang.GetValueByKey(language, key);
        }

        public string Gets(Languages lang, string[] keys)
        {
            return this._lang.GetValues(lang, keys);
        }
    }
}