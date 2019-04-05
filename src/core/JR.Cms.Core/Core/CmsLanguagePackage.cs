using System;
using System.IO;
using System.Text;
using JR.Cms.Cache;
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
        private LanguagePackage _lang;


        public LanguagePackage GetPackage()
        {
            return _lang;
        }

        internal CmsLanguagePackage()
        {
            _lang = new LanguagePackage();
            _lang.LoadFromXml(ResourceMap.XmlLangPackage);
            // 加载系统内置的
            LoadLocaleXml(Cms.PyhicPath + CmsVariables.FRAMEWORK_PATH + "locale");
            // 加载自定义的配置
            this.LoadSiteLocaleXml();

            //todo: 为何这里要加载JSON格式
            //加载JSON格式语言
            LoadFromFile(Cms.PyhicPath + CmsVariables.FRAMEWORK_ASSETS_PATH+"locale/locale.db");
            LoadFromFile( Cms.PyhicPath + CmsVariables.SITE_LOCALE_PATH);


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

        private void LoadSiteLocaleXml()
        {
            String dir = Cms.PyhicPath + CmsVariables.SITE_CONF_PATH + "locale";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir).Create();
                DirectoryInfo target = new DirectoryInfo(Cms.PyhicPath + CmsVariables.FRAMEWORK_PATH + "locale");
                foreach (FileInfo fi in target.GetFiles())
                {
                    File.Copy(fi.FullName, dir+"/" + fi.Name, true); 
                }
            }
            this.LoadLocaleXml(dir);
        }

        private void LoadFromFile(string file)
        {
            if (File.Exists(file))
            {
                StreamReader rd = new StreamReader(file, Encoding.UTF8);
                String json = rd.ReadToEnd();
                rd.Close();
                _lang.LoadFromJson(json);
            }
        }


        /// <summary>
        /// 加载系统内置的语言配置文件
        /// </summary>
        private void LoadLocaleXml(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles("*.xml");
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
            return _lang.Get(Cms.Context.UserLanguage, key);
        }

        public string Get(Languages language, string key)
        {
            return _lang.GetValueByKey(language, key);
        }
    }
}
